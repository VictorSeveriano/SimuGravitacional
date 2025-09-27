// Gerencia os corpos e a logica de simulação

using SimuGravitacional;
using System;
using System.Collections.Generic;
using System.Linq;

public class Universo
{
    // Constante de gravitação
    private const double G = 0.0000090;

    // Array interno de corpos
    public Corpo[] Corpos       { get; private set; }

    // Contador de quantos corpos existem atualmente
    public int QuantidadeCorpos { get; private set; }

    //inicializa universo vazio
    public Universo()
    {
        Corpos           = new Corpo[0];
        QuantidadeCorpos = 0;
    }

    // Insere quantidade corpos aleatórios dentro da área definida)
    public void InserirCorpos(int quantidade, double massaMin, double massaMax, int larguraMax, int alturaMax)
    {
        // se quantidade for invlida, zera o universo
        if (quantidade <= 0)
        {
            Corpos = new Corpo[0];
            QuantidadeCorpos = 0;
            return;
        }

        // inicializa array com tamanho quantidade
        Corpos = new Corpo[quantidade];
        QuantidadeCorpos = 0;

        Random rnd = new Random();

        for (int i = 0; i < quantidade; i++)
        {
            // massa aleatória entre massaMin e massaMax
            double massa = rnd.NextDouble() * (massaMax - massaMin) + massaMin;

            // densidade aleatória entre densidade do hidrogênio e do ósmio
            const double DENSIDADE_MIN_HIDROGENIO = 0.0899;
            const double DENSIDADE_MAX_OSMIO = 22590.0;
            double densidade = rnd.NextDouble() * (DENSIDADE_MAX_OSMIO - DENSIDADE_MIN_HIDROGENIO) + DENSIDADE_MIN_HIDROGENIO;

            // posição aleatoria, evitando bordas
            double posX = rnd.Next(10, Math.Max(11, larguraMax - 10));
            double posY = rnd.Next(10, Math.Max(11, alturaMax - 10));

            // velocidade aleatória entre -1 e 1 em cada eixo
            double velX = rnd.NextDouble() * 2.0 - 1.0;
            double velY = rnd.NextDouble() * 2.0 - 1.0;

            // cria o novo corpo e adiciona ao universo
            Corpo novoCorpo = new Corpo($"Corpo{i + 1}", massa, densidade, posX, posY, velX, velY);
            AdicionarCorpo(novoCorpo);
        }
    }

    // Substitui todos os corpos do universo por um array fornecido
    public void DefinirCorpos(Corpo[] novosCorpos)
    {
        if (novosCorpos == null)
        {
            // se nada foi passado, zera o universo
            Corpos = new Corpo[0];
            QuantidadeCorpos = 0;
        }
        else
        {
            // define o array e atualiza a quantidade real de corpos
            Corpos = novosCorpos;
            QuantidadeCorpos = novosCorpos.Length;
        }
    }
    public void AdicionarCorpo(Corpo novoCorpo)
    {
        // se o array está cheio, dobra a capacidade
        if (QuantidadeCorpos == Corpos.Length)
        {
            int novaCapacidade = Corpos.Length == 0 ? 4 : Corpos.Length * 2;
            Corpo[] novoArray  = new Corpo[novaCapacidade];
            Array.Copy(Corpos, novoArray, QuantidadeCorpos); // copia elementos existentes
            Corpos             = novoArray; // substitui o array por um maior
        }
        // insere no final e incrementa contador
        Corpos[QuantidadeCorpos] = novoCorpo;
        QuantidadeCorpos++;
    }

    // Simula um passo de tempo, calcula as forças, atualiza velocidade, pos e trata colisoes
    public void SimularPasso(double tempoDoPasso)
    {
        // dicionário temporario que guarda os proximos estados calculados para cada corpo
        var proximosEstados = new Dictionary<Corpo, (double velX, double velY, double posX, double posY)>();

        //Para cada corpo, calcula a força total exercida por todos os outros corpos
        for (int i = 0; i < QuantidadeCorpos; i++)
        {
            var corpoA = Corpos[i];
            if (corpoA == null) continue; // pula entradas vazias

            double forcaTotalX = 0;
            double forcaTotalY = 0;

            // itera sobre todos os outros corpos para somar as forças
            for (int j = 0; j < QuantidadeCorpos; j++)
            {
                if (i == j) continue; // não calcula a força de um corpo em si mesmo
                var corpoB = Corpos[j];
                if (corpoB == null) continue;

                // vetor diferença entre as posições
                double dX = corpoB.PosX - corpoA.PosX;
                double dY = corpoB.PosY - corpoA.PosY;
                // distância entre os corpos
                double distancia = Math.Sqrt(dX * dX + dY * dY);

                // se a distância é extremamente pequena, evita divisão por zero
                if (distancia < 1e-6) continue;

                // magnitude da força gravitacional
                double forcaMagnitude = (G * corpoA.Massa * corpoB.Massa) / (distancia * distancia);
                // componente X e Y da força
                double forcaX = forcaMagnitude * (dX / distancia);
                double forcaY = forcaMagnitude * (dY / distancia);

                // acumula as componentes da força total 
                forcaTotalX += forcaX;
                forcaTotalY += forcaY;
            }

            // aceleração
            double aceleracaoX = forcaTotalX / corpoA.Massa;
            double aceleracaoY = forcaTotalY / corpoA.Massa;

            // atualiza velocidade
            double novaVelX = corpoA.VelX + (aceleracaoX * tempoDoPasso);
            double novaVelY = corpoA.VelY + (aceleracaoY * tempoDoPasso);

            // atualiza posição usando movimento
            double novaPosX = corpoA.PosX + (corpoA.VelX * tempoDoPasso) + (0.5 * aceleracaoX * tempoDoPasso * tempoDoPasso);
            double novaPosY = corpoA.PosY + (corpoA.VelY * tempoDoPasso) + (0.5 * aceleracaoY * tempoDoPasso * tempoDoPasso);

            // armazena o próximo estado no dicionário para aplicar depois
            proximosEstados[corpoA] = (novaVelX, novaVelY, novaPosX, novaPosY);
        }

        //Aplica os próximos estados calculados a cada corpo
        for (int i = 0; i < QuantidadeCorpos; i++)
        {
            var corpo = Corpos[i];
            if (corpo == null) continue;

            var novoEstado = proximosEstados[corpo];
            corpo.VelX = novoEstado.velX; // aplica nova velocidade X
            corpo.VelY = novoEstado.velY; // aplica nova velocidade Y
            corpo.PosX = novoEstado.posX; // aplica nova posição X
            corpo.PosY = novoEstado.posY; // aplica nova posição Y
        }

        //Detecta colisoes e prepara fusoes
        var corposParaRemover = new HashSet<Corpo>(); // corpos que serao removidos
        var corposParaAdicionar = new List<Corpo>(); // corpos resultantes das fusoes

        // Percorre todos os pares de corpos sem repetir
        for (int i     = 0; i < QuantidadeCorpos; i++)
        {
            for (int j = i + 1; j < QuantidadeCorpos; j++)
            {
                Corpo c1 = Corpos[i];
                Corpo c2 = Corpos[j];
                if (c1   == null || c2 == null) continue;
                if (corposParaRemover.Contains(c1) || corposParaRemover.Contains(c2)) continue; // se já marcados para remoção, pula

                // vetor e distância entre os centros
                double dX        = c2.PosX - c1.PosX;
                double dY        = c2.PosY - c1.PosY;
                double distancia = Math.Sqrt(dX * dX + dY * dY);

                // se a distância for menor que a soma dos raios tem uma colisão e sobreposição
                if (distancia < (c1.Raio + c2.Raio))
                {
                    // usa o operador + sobrescrito para criar o corpo resultante da fusão
                    Corpo corpoResultante = c1 + c2;
                    corposParaAdicionar.Add(corpoResultante); // adiciona o novo corpo à lista de inclusao
                    corposParaRemover.Add(c1);
                    corposParaRemover.Add(c2); 
                }
            }
        }

        //Se houver colisoes, refaz o array de corpos
        if (corposParaRemover.Any())
        {
            var proximosCorpos = new List<Corpo>(QuantidadeCorpos);
            for (int i = 0; i < QuantidadeCorpos; i++)
            {
                // insere apenas os corpos que não estão marcados para remoção
                if (!corposParaRemover.Contains(Corpos[i]))
                {
                    proximosCorpos.Add(Corpos[i]);
                }
            }
            // adiciona os corpos resultantes das colisoes
            proximosCorpos.AddRange(corposParaAdicionar);

            // atualiza o array interno e o contador real
            Corpos           = proximosCorpos.ToArray();
            QuantidadeCorpos = Corpos.Length;
        }
    }
}
