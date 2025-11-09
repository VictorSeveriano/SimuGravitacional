namespace SimuGravitacional.Abstrato
{
    public abstract class GravitacaoDAO
    {
        // Função que salva as informações da simulação em um arquivo
        public abstract void Salvar(string caminhoArquivo, Universo universoInicial, Universo universoAtual, int iteracaoAtual, int tempoIteracao);

        // função que carrega as informações da simulação de um arquivo
        public abstract Universo Carregar(string caminhoArquivo, out int iteracaoSalva, out int tempoIteracao);
    }
}