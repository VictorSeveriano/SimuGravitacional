// Classe que representa um corpo físico na simulaçao, e possui suas propriedades
public class Corpo
{
    public string Nome      { get; set; }
    public double Massa     { get; set; }
    public double Densidade { get; set; }
    public double PosX      { get; set; }
    public double PosY      { get; set; }
    public double VelX      { get; set; }
    public double VelY      { get; set; }
    public double Raio      { get; private set; }

    // Construtor, inicializa todas as propriedades e calcula o raio
    public Corpo(string nome, double massa, double densidade, double posX, double posY, double velX, double velY)
    {
        // atribui valores recebidos aos campos correspondentes
        this.Nome      = nome;
        this.Massa     = massa;
        this.Densidade = densidade;
        this.PosX      = posX;
        this.PosY      = posY;
        this.VelX      = velX;
        this.VelY      = velY;

        this.Raio = CalcularRaio(); // calcula o raio com base na massa e densidade
    }

    // Construtor de Cópia
    // Cria uma nova instância de Corpo baseada em um corpo existente
    public Corpo(Corpo original)
    {
        this.Nome      = original.Nome;
        this.Massa     = original.Massa;
        this.Densidade = original.Densidade;
        this.PosX      = original.PosX;
        this.PosY      = original.PosY;
        this.VelX      = original.VelX;
        this.VelY      = original.VelY;
        this.Raio      = original.Raio; 
    }

    // calcula o raio 
    private double CalcularRaio()
    {
        if (this.Densidade > 0) // se densidade for valida, calcula volume e raio
        {
            double volume = this.Massa / this.Densidade;
            return Math.Cbrt((3.0 * volume) / (4.0 * Math.PI)); // calcula o raio da esfera a partir do volume
        }
        else
        {
            return 0; // se a densidade for invalido vai retorna 0 para evitar divisao por zero
        }
    }

    // Sobrecarga do operador + para representar fusao de dois corpos
    public static Corpo operator +(Corpo c1, Corpo c2)
    {
        // se um dos corpos for nulo, retorna o outro diretamente
        if (c1 == null) return c2;
        if (c2 == null) return c1;

        // soma das massas, vai resultar a nova massa
        double massaTotal = c1.Massa + c2.Massa;

        // se a massa total for zero,vai cria um corpo vazio com massa 0 (nao deve acontecer, mas previne o cod :P)
        if (massaTotal == 0)
        {
            return new Corpo("Vazio", 0, 0, c1.PosX, c1.PosY, 0, 0);
        }

        // calcula a velocidade final do momento linear:
        double velFinalX = (c1.Massa * c1.VelX + c2.Massa * c2.VelX) / massaTotal;
        double velFinalY = (c1.Massa * c1.VelY + c2.Massa * c2.VelY) / massaTotal;

        // posição final calculada como media pelas massas
        double posFinalX = (c1.Massa * c1.PosX + c2.Massa * c2.PosX) / massaTotal;
        double posFinalY = (c1.Massa * c1.PosY + c2.Massa * c2.PosY) / massaTotal;

        // Para densidade resultante vamos calcula volumes individuais
        double volume1 = (c1.Densidade > 0) ? (c1.Massa / c1.Densidade) : 0;
        double volume2 = (c2.Densidade > 0) ? (c2.Massa / c2.Densidade) : 0;
        double volumeTotal = volume1 + volume2;
        double densidadeFinal = (volumeTotal > 0) ? (massaTotal / volumeTotal) : 0; // soma volumes e obtem densidade final

        // cria o corpo resultante com nome composto e parametros calculados
        Corpo corpoResultante = new Corpo(
            nome: $"{c1.Nome}-{c2.Nome}", // concatena nomes para identificar origem
            massa: massaTotal,
            densidade: densidadeFinal,
            posX: posFinalX,
            posY: posFinalY,
            velX: velFinalX,
            velY: velFinalY
        );

        return corpoResultante;
    }
}