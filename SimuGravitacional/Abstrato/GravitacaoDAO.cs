using System.Collections.Generic;
using System; 

namespace SimuGravitacional.Abstrato
{
    // Classe para transportar os dados de resumo
    public class ResumoSimulacao
    {
        public int      IdSimulacao      { get; set; }
        public DateTime DataSimulacao    { get; set; }
        public int      QtdCorposInicial { get; set; }
    }

    public abstract class GravitacaoDAO
    {
        // Função que salva as informações da simulação em um arquivo
        public abstract void Salvar(string caminhoArquivo, Universo universoInicial, Universo universoAtual, int iteracaoAtual, int tempoIteracao);

        // função que carrega as informações da simulação de um arquivo
        public abstract Universo Carregar(string caminhoArquivo, out int iteracaoSalva, out int tempoIteracao);

        // Método para listar todas as simulações salvas
        public abstract List<ResumoSimulacao> ListarSimulacoes();
    }
}