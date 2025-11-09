using System;
using System.Globalization;
using System.IO;

namespace SimuGravitacional.Abstrato
{
    public class GravitacaoArquivoDAO : GravitacaoDAO
    {
        public override Universo Carregar(string caminhoArquivo, out int iteracaoSalva, out int tempoIteracao)
        {
            Universo universo = new Universo();
            var linhas        = File.ReadAllLines(caminhoArquivo);

            // Cabeçalho de Simulação
            var primeiraLinha = linhas[0].Split(';');
            iteracaoSalva  = int.Parse(primeiraLinha[0]);
            tempoIteracao  = int.Parse(primeiraLinha[1]);

            int linhaAtual = 1;

            // Pula linhas até encontrar o bloco ESTADO_INICIAL
            while (linhaAtual < linhas.Length && linhas[linhaAtual] != "[ESTADO_INICIAL]")
            {
                linhaAtual++;
            }

            // Se encontrou o bloco inicial, carrega ele para o reprise
            if (linhaAtual < linhas.Length && linhas[linhaAtual] == "[ESTADO_INICIAL]")
            {
                linhaAtual++; // Move para a linha da quantidade
                int qtdCorpos = int.Parse(linhas[linhaAtual]);
                linhaAtual++; // Move para o primeiro corpo

                Corpo[] corpos = new Corpo[qtdCorpos];

                for (int i = 0; i < qtdCorpos; i++)
                {
                    // Evita ler fora dos limites do arquivo se o arquivo estiver corrompido
                    if (linhaAtual + i >= linhas.Length) break;

                    var dadosCorpo = linhas[linhaAtual + i].Split(';');

                    string nome  = dadosCorpo[0];
                    double massa = double.Parse(dadosCorpo[1], CultureInfo.InvariantCulture);
                    double raio  = double.Parse(dadosCorpo[2], CultureInfo.InvariantCulture);
                    double posX  = double.Parse(dadosCorpo[3], CultureInfo.InvariantCulture);
                    double posY  = double.Parse(dadosCorpo[4], CultureInfo.InvariantCulture);
                    double velX  = double.Parse(dadosCorpo[5], CultureInfo.InvariantCulture);
                    double velY  = double.Parse(dadosCorpo[6], CultureInfo.InvariantCulture);

                    double volume = 4.0 / 3.0 * Math.PI * Math.Pow(raio, 3);
                    double densidade = volume > 0 ? massa / volume : 0;

                    corpos[i] = new Corpo(nome, massa, densidade, posX, posY, velX, velY);
                }

                universo.DefinirCorpos(corpos);
            }
            else
            {
                // Se não encontrar o bloco, o arquivo está em formato antigo ou corrompido
                throw new Exception("Formato de arquivo inválido. Bloco [ESTADO_INICIAL] não encontrado.");
            }

            return universo;
        }

        // Salva ambos os estados Atual e Inicial
        public override void Salvar(string caminhoArquivo, Universo universoInicial, Universo universoAtual, int iteracaoAtual, int tempoIteracao)
        {
            using (StreamWriter sw = new StreamWriter(caminhoArquivo))
            {
                // Cabeçalho de Simulação Iteração atual e tempo
                sw.WriteLine($"{iteracaoAtual};{tempoIteracao}");

                // Estado Atual para sua inspeção, com corpos fundidos
                sw.WriteLine("[ESTADO_ATUAL]");
                sw.WriteLine(universoAtual.QuantidadeCorpos);
                EscreverCorpos(sw, universoAtual); // // Usa o escrevercorpos para não repetir

                // Estado Inicial para o reprise
                sw.WriteLine("[ESTADO_INICIAL]");
                sw.WriteLine(universoInicial.QuantidadeCorpos);
                EscreverCorpos(sw, universoInicial); // Usa o escrevercorpos para não repetir
            }
        }

        //para não repetir código de escrita
        private void EscreverCorpos(StreamWriter sw, Universo universo)
        {
            for (int i = 0; i < universo.QuantidadeCorpos; i++)
            {
                var corpo  = universo.Corpos[i];
                if (corpo != null)
                {
                    string linha = string.Join(";",
                        corpo.Nome,
                        corpo.Massa.ToString(CultureInfo.InvariantCulture),
                        corpo.Raio.ToString(CultureInfo.InvariantCulture),
                        corpo.PosX.ToString(CultureInfo.InvariantCulture),
                        corpo.PosY.ToString(CultureInfo.InvariantCulture),
                        corpo.VelX.ToString(CultureInfo.InvariantCulture),
                        corpo.VelY.ToString(CultureInfo.InvariantCulture)
                    );
                    sw.WriteLine(linha);
                }
            }
        }
    }
}