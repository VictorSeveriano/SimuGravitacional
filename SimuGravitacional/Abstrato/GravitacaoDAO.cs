// classe abstrata para persistência salvar,carregar

namespace SimuGravitacional.Abstrato
{
    public abstract class GravitacaoDAO
    {
        // Assina o metodo de salvar, caminho, universo, e os parametros da simulação
        public abstract void Salvar(string caminhoArquivo, Universo universo, int qtdIteracoes, int tempoIteracao, double deltaT);

        // Assina o metodo de carregar, retorna Universo e traz parametros da simulação
        public abstract Universo Carregar(string caminhoArquivo, out int qtdIteracoes, out int tempoIteracao, out double deltaT);
    }
}
