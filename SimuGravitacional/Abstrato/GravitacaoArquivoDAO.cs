// Implementaçao que persiste,recupera simulaçoes em arquivos de texto

using System;
using System.Globalization;
using System.IO;

namespace SimuGravitacional.Abstrato
{
    public class GravitacaoArquivoDAO : GravitacaoDAO
    {
        // Carrega um arquivo de simulaçao e retorna um universo configurado.
        public override Universo Carregar(string caminhoArquivo, out int qtdIteracoes, out int tempoIteracao, out double deltaT)
        {
            Universo universo = new Universo(); // cria instância que será preenchida
            var linhas        = File.ReadAllLines(caminhoArquivo); // le todas as linhas do arquivo 
            var primeiraLinha = linhas[0].Split(';'); // divide a primeira linha por ; para extrair os dados

            int qtdCorpos = int.Parse(primeiraLinha[0]);
            qtdIteracoes  = int.Parse(primeiraLinha[1]);
            tempoIteracao = int.Parse(primeiraLinha[2]);

            // caso deltat não esteja ela usa 0.1 por padrao
            deltaT = primeiraLinha.Length > 3 ? double.Parse(primeiraLinha[3], CultureInfo.InvariantCulture) : 0.1;

            // prepara array de corpos do tamanho informado
            Corpo[] corpos = new Corpo[qtdCorpos];

            // para cada corpo, parseia a linha correspondente
            for (int i = 0; i < qtdCorpos; i++)
            {
                var dadosCorpo = linhas[i + 1].Split(';');

                // Le todos os dados primeiro e converte para double onde necessário
                string nome  = dadosCorpo[0];
                double massa = double.Parse(dadosCorpo[1], CultureInfo.InvariantCulture);
                double raio  = double.Parse(dadosCorpo[2], CultureInfo.InvariantCulture);
                double posX  = double.Parse(dadosCorpo[3], CultureInfo.InvariantCulture);
                double posY  = double.Parse(dadosCorpo[4], CultureInfo.InvariantCulture);
                double velX  = double.Parse(dadosCorpo[5], CultureInfo.InvariantCulture);
                double velY  = double.Parse(dadosCorpo[6], CultureInfo.InvariantCulture);

                // Calcula a densidade a partir do raio e massa
                double volume    = 4.0 / 3.0 * Math.PI * Math.Pow(raio, 3);
                double densidade = volume > 0 ? massa / volume : 0;

                // cria o objeto corpo com a densidade calculada
                Corpo corpo = new Corpo(nome, massa, densidade, posX, posY, velX, velY);

                corpos[i] = corpo;
            }

            // define os corpos lidos no universo 
            universo.DefinirCorpos(corpos);

            return universo;
        }

        // Salva o Universo em um arquivo texto no formato compatível com o metodo Carregar
        public override void Salvar(string caminhoArquivo, Universo universo, int qtdIteracoes, int tempoIteracao, double deltaT)
        {
            // usa StreamWriter dentro de using para garantir fechamento do arquivo
            using (StreamWriter sw = new StreamWriter(caminhoArquivo))
            {
                // escreve a linha de cabeçalho com qtdCorpos, qtdIteracoes, tempoIteracao e deltaT
                sw.WriteLine($"{universo.QuantidadeCorpos};{qtdIteracoes};{tempoIteracao};{deltaT.ToString(CultureInfo.InvariantCulture)}");

                // percorre todos os indices de 0 ate QuantidadeCorpos - 1
                for (int i = 0; i < universo.QuantidadeCorpos; i++)
                {
                    var corpo = universo.Corpos[i];
                    if (corpo != null)
                    {
                        // monta linha com os dados do corpo
                        string linha = string.Join(";",
                            corpo.Nome,
                            corpo.Massa.ToString(CultureInfo.InvariantCulture),
                            corpo.Raio.ToString(CultureInfo.InvariantCulture),
                            corpo.PosX.ToString(CultureInfo.InvariantCulture),
                            corpo.PosY.ToString(CultureInfo.InvariantCulture),
                            corpo.VelX.ToString(CultureInfo.InvariantCulture),
                            corpo.VelY.ToString(CultureInfo.InvariantCulture)
                        );
                        // escreve a linha no arquivo
                        sw.WriteLine(linha);
                    }
                }
            } 
        }
    }
}
