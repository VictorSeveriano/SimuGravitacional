using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Globalization;
using SimuGravitacional.ConfigBd;

namespace SimuGravitacional.Abstrato
{
    public class GravitacaoMySQLDAO : GravitacaoDAO
    {
        public override void Salvar(string caminhoArquivo, Universo universoInicial, Universo universoAtual, int iteracaoAtual, int tempoIteracao)
        {
            // O parâmetro caminhoArquivo é herdado da classe abstrata, mas não é usado aqui.

            long numSimulacao = -1;
            int qtdCorposInicial = universoInicial.QuantidadeCorpos;

            // Busca a string de conexão da classe de configuração
            using (var conn = new MySqlConnection(ConfiguracaoBd.ConnectionString))
            {
                conn.Open();
                using (var transaction = conn.BeginTransaction())
                {
                    try
                    {
                        // Inserir o registro mestre da Simulação
                        var cmdSimulacao = new MySqlCommand(
                            "INSERT INTO Simulacao (QtdCorposInicial, NumInteracoes, TempoInteracoes, DataSimulacao) " +
                            "VALUES (@QtdCorposInicial, @NumInteracoes, @TempoInteracoes, NOW()); " +
                            "SELECT LAST_INSERT_ID();", // Pega o ID autoincrementado
                            conn, transaction);

                        cmdSimulacao.Parameters.AddWithValue("@QtdCorposInicial", qtdCorposInicial);
                        cmdSimulacao.Parameters.AddWithValue("@NumInteracoes", iteracaoAtual);
                        cmdSimulacao.Parameters.AddWithValue("@TempoInteracoes", tempoIteracao);

                        numSimulacao = Convert.ToInt64(cmdSimulacao.ExecuteScalar());

                        // Salva o estado inicial
                        // Inserir o Resultado 
                        var cmdResInicial = new MySqlCommand("INSERT INTO Resultados (NumSimulacao, NumInteracao) VALUES (@NumSim, 0)", conn, transaction);
                        cmdResInicial.Parameters.AddWithValue("@NumSim", numSimulacao);
                        cmdResInicial.ExecuteNonQuery();

                        // Inserir os Corpos Iniciais
                        SalvarCorposNoBD(conn, transaction, universoInicial, numSimulacao, 0);


                        // Salvar o ESTADO 
                        // Inserir o Resultado 
                        var cmdResAtual = new MySqlCommand("INSERT INTO Resultados (NumSimulacao, NumInteracao) VALUES (@NumSim, @NumIteracao)", conn, transaction);
                        cmdResAtual.Parameters.AddWithValue("@NumSim", numSimulacao);
                        cmdResAtual.Parameters.AddWithValue("@NumIteracao", iteracaoAtual);
                        cmdResAtual.ExecuteNonQuery();

                        // Inserir os Corpos Atuais
                        SalvarCorposNoBD(conn, transaction, universoAtual, numSimulacao, iteracaoAtual);

                        // Se tudo deu certo, commita a transação
                        transaction.Commit();
                    }
                    catch (Exception)
                    {
                        transaction.Rollback(); // Desfaz tudo em caso de erro
                        throw; // lança a exceção
                    }
                }
            }
        }

        private void SalvarCorposNoBD(MySqlConnection conn, MySqlTransaction transaction, Universo universo, long numSimulacao, int numIteracao)
        {
            // Prepara o comando de inserção de corpo
            var cmdCorpo = new MySqlCommand(
                "INSERT INTO Corpos (NomeCorpo, NumSimulacao, NumInteracao, MassaCorpo, DensidadeCorpo, PosX, PosY, VelX, VelY) " +
                "VALUES (@Nome, @NumSim, @NumIteracao, @Massa, @Densidade, @PosX, @PosY, @VelX, @VelY)",
                conn, transaction);

            // Adiciona parâmetros (eles serão reutilizados no loop)
            cmdCorpo.Parameters.AddWithValue("@Nome", "");
            cmdCorpo.Parameters.AddWithValue("@NumSim", numSimulacao);
            cmdCorpo.Parameters.AddWithValue("@NumIteracao", numIteracao);
            cmdCorpo.Parameters.AddWithValue("@Massa", 0.0);
            cmdCorpo.Parameters.AddWithValue("@Densidade", 0.0);
            cmdCorpo.Parameters.AddWithValue("@PosX", 0.0);
            cmdCorpo.Parameters.AddWithValue("@PosY", 0.0);
            cmdCorpo.Parameters.AddWithValue("@VelX", 0.0);
            cmdCorpo.Parameters.AddWithValue("@VelY", 0.0);

            // Itera por todos os corpos e os insere
            for (int i = 0; i < universo.QuantidadeCorpos; i++)
            {
                var corpo = universo.Corpos[i];
                if (corpo == null) continue;

                cmdCorpo.Parameters["@Nome"].Value = corpo.Nome;
                cmdCorpo.Parameters["@Massa"].Value = corpo.Massa;
                cmdCorpo.Parameters["@Densidade"].Value = corpo.Densidade;
                cmdCorpo.Parameters["@PosX"].Value = corpo.PosX;
                cmdCorpo.Parameters["@PosY"].Value = corpo.PosY;
                cmdCorpo.Parameters["@VelX"].Value = corpo.VelX;
                cmdCorpo.Parameters["@VelY"].Value = corpo.VelY;

                cmdCorpo.ExecuteNonQuery();
            }
        }

        public override Universo Carregar(string caminhoArquivo, out int iteracaoSalva, out int tempoIteracao)
        {
            // Vamos usar do parâmetro caminhoArquivo para passar o ID
            if (!int.TryParse(caminhoArquivo, out int numSimulacao))
            {
                throw new ArgumentException("ID da simulação (passado como 'caminhoArquivo') é inválido.");
            }

            var universo = new Universo();
            var corposLista = new List<Corpo>();

            iteracaoSalva = 0;
            tempoIteracao = 0;

            // Busca a string de conexão da classe de configuração
            using (var conn = new MySqlConnection(ConfiguracaoBd.ConnectionString))
            {
                conn.Open();

                // Carregar os dados mestres da simulação
                var cmdSim = new MySqlCommand("SELECT NumInteracoes, TempoInteracoes FROM Simulacao WHERE NumSimulacao = @NumSim", conn);
                cmdSim.Parameters.AddWithValue("@NumSim", numSimulacao);

                using (var readerSim = cmdSim.ExecuteReader())
                {
                    if (readerSim.Read())
                    {
                        iteracaoSalva = readerSim.GetInt32("NumInteracoes");
                        tempoIteracao = (int)readerSim.GetDouble("TempoInteracoes");
                    }
                    else
                    {
                        throw new Exception("Simulação com ID " + numSimulacao + " não encontrada.");
                    }
                } // readerSim é fechado aqui

                // Carregar os corpos do ESTADO INICIAL (Iteração 0)
                var cmdCorpos = new MySqlCommand(
                    "SELECT NomeCorpo, MassaCorpo, DensidadeCorpo, PosX, PosY, VelX, VelY " +
                    "FROM Corpos WHERE NumSimulacao = @NumSim AND NumInteracao = 0", conn);
                cmdCorpos.Parameters.AddWithValue("@NumSim", numSimulacao);

                using (var readerCorpos = cmdCorpos.ExecuteReader())
                {
                    while (readerCorpos.Read())
                    {
                        var corpo = new Corpo(
                            nome: readerCorpos.GetString("NomeCorpo"),
                            massa: readerCorpos.GetDouble("MassaCorpo"),
                            densidade: readerCorpos.GetDouble("DensidadeCorpo"),
                            posX: readerCorpos.GetDouble("PosX"),
                            posY: readerCorpos.GetDouble("PosY"),
                            velX: readerCorpos.GetDouble("VelX"),
                            velY: readerCorpos.GetDouble("VelY")
                        );
                        corposLista.Add(corpo);
                    }
                } // readerCorpos é fechado aqui
            }

            universo.DefinirCorpos(corposLista.ToArray());
            return universo;
        }

        // Implementação do método para listar simulações
        public override List<ResumoSimulacao> ListarSimulacoes()
        {
            var lista = new List<ResumoSimulacao>();
            using (var conn = new MySqlConnection(ConfiguracaoBd.ConnectionString))
            {
                conn.Open();
                // Seleciona os campos desejados da tabela Simulacao, ordenando pelos mais recentes
                var cmd = new MySqlCommand(
                    "SELECT NumSimulacao, DataSimulacao, QtdCorposInicial FROM Simulacao ORDER BY DataSimulacao DESC",
                    conn);

                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        lista.Add(new ResumoSimulacao
                        {
                            IdSimulacao = reader.GetInt32("NumSimulacao"),
                            DataSimulacao = reader.GetDateTime("DataSimulacao"),
                            QtdCorposInicial = reader.GetInt32("QtdCorposInicial")
                        });
                    }
                }
            }
            return lista;
        }
    }
}