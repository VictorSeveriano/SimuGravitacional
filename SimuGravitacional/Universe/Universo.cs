// Gerencia os corpos e a lógica de simulação

using SimuGravitacional; 
using System;
using System.Collections.Generic;
using System.Linq;

public class Universo
{
    // Constante de gravitação universal 
    private const double G = 6.6743e-11;

    // Array interno de corpos
    public Corpo[] Corpos { get; private set; }

    // Contador de quantos corpos existem atualmente
    public int QuantidadeCorpos { get; private set; }

    // Inicializa universo vazio
    public Universo()
    {
        Corpos = new Corpo[0];
        QuantidadeCorpos = 0;
    }

    // Método Clone para criar uma cópia do Universo
    public Universo Clone()
    {
        Universo novoUniverso = new Universo();
        novoUniverso.Corpos = new Corpo[this.QuantidadeCorpos];

        for (int i = 0; i < this.QuantidadeCorpos; i++)
        {
            if (this.Corpos[i] != null)
            {
                // Usa o novo construtor de cópia do Corpo
                novoUniverso.Corpos[i] = new Corpo(this.Corpos[i]);
            }
        }
        novoUniverso.QuantidadeCorpos = this.QuantidadeCorpos;
        return novoUniverso;
    }

    // Insere quantidade de corpos aleatórios dentro da área definida
    public void InserirCorpos(int quantidade, double massaMin, double massaMax, int larguraMax, int alturaMax)
    {
        if (quantidade <= 0)
        {
            Corpos = new Corpo[0];
            QuantidadeCorpos = 0;
            return;
        }

        Corpos = new Corpo[quantidade];
        QuantidadeCorpos = 0;

        Random rnd = new Random();

        for (int i = 0; i < quantidade; i++)
        {
            double massa = rnd.NextDouble() * (massaMax - massaMin) + massaMin;

            // densidade aleatória entre hidrogênio e ósmio
            const double DENSIDADE_MIN_HIDROGENIO = 0.0899;
            const double DENSIDADE_MAX_OSMIO = 22590.0;
            double densidade = rnd.NextDouble() * (DENSIDADE_MAX_OSMIO - DENSIDADE_MIN_HIDROGENIO) + DENSIDADE_MIN_HIDROGENIO;

            double posX = rnd.Next(10, Math.Max(11, larguraMax - 10));
            double posY = rnd.Next(10, Math.Max(11, alturaMax - 10));

            // velocidade inicial zero
            double velX = 0;
            double velY = 0;

            Corpo novoCorpo = new Corpo($"Corpo{i + 1}", massa, densidade, posX, posY, velX, velY);
            AdicionarCorpo(novoCorpo);
        }
    }

    // Substitui todos os corpos do universo por um novo array
    public void DefinirCorpos(Corpo[] novosCorpos)
    {
        if (novosCorpos == null)
        {
            Corpos = new Corpo[0];
            QuantidadeCorpos = 0;
        }
        else
        {
            Corpos = novosCorpos;
            QuantidadeCorpos = novosCorpos.Length;
        }
    }

    public void AdicionarCorpo(Corpo novoCorpo)
    {
        if (QuantidadeCorpos == Corpos.Length)
        {
            int novaCapacidade = Corpos.Length == 0 ? 4 : Corpos.Length * 2;
            Corpo[] novoArray  = new Corpo[novaCapacidade];
            Array.Copy(Corpos, novoArray, QuantidadeCorpos);
            Corpos             = novoArray;
        }

        Corpos[QuantidadeCorpos] = novoCorpo;
        QuantidadeCorpos++;
    }

    // Cada tick do timer é tratado como uma unidade de tempo fixa
    public void SimularPasso()
    {
        var proximosEstados = new Dictionary<Corpo, (double velX, double velY, double posX, double posY)>();

        // Calcula a força gravitacional entre todos os pares de corpos
        for (int i = 0; i < QuantidadeCorpos; i++)
        {
            var corpoA = Corpos[i];
            if (corpoA == null) continue;

            double forcaTotalX = 0;
            double forcaTotalY = 0;

            for (int j = 0; j < QuantidadeCorpos; j++)
            {
                if (i == j) continue;
                var corpoB = Corpos[j];
                if (corpoB == null) continue;

                double dX = corpoB.PosX - corpoA.PosX;
                double dY = corpoB.PosY - corpoA.PosY;
                double distancia = Math.Sqrt(dX * dX + dY * dY);

                if (distancia < 1e-6) continue;

                double forcaMagnitude = (G * corpoA.Massa * corpoB.Massa) / (distancia * distancia);
                double forcaX = forcaMagnitude * (dX / distancia);
                double forcaY = forcaMagnitude * (dY / distancia);

                forcaTotalX += forcaX;
                forcaTotalY += forcaY;
            }

            // Calcula aceleração resultante
            double aceleracaoX = forcaTotalX / corpoA.Massa;
            double aceleracaoY = forcaTotalY / corpoA.Massa;

            // Atualiza velocidade e posição — tempo de passo implícito (1 unidade)
            double novaVelX = corpoA.VelX + aceleracaoX;
            double novaVelY = corpoA.VelY + aceleracaoY;

            double novaPosX = corpoA.PosX + corpoA.VelX + (0.5 * aceleracaoX);
            double novaPosY = corpoA.PosY + corpoA.VelY + (0.5 * aceleracaoY);

            proximosEstados[corpoA] = (novaVelX, novaVelY, novaPosX, novaPosY);
        }

        // Aplica os novos estados calculados
        for (int i = 0; i < QuantidadeCorpos; i++)
        {
            var corpo = Corpos[i];
            if (corpo == null) continue;

            var novoEstado = proximosEstados[corpo];
            corpo.VelX = novoEstado.velX;
            corpo.VelY = novoEstado.velY;
            corpo.PosX = novoEstado.posX;
            corpo.PosY = novoEstado.posY;
        }

        // Trata colisões e fusões de corpos
        var corposParaRemover = new HashSet<Corpo>();
        var corposParaAdicionar = new List<Corpo>();

        // Verifica todas as combinações de corpos para colisões
        for (int i = 0; i < QuantidadeCorpos; i++)
        {
            for (int j = i + 1; j < QuantidadeCorpos; j++)
            {
                Corpo c1 = Corpos[i];
                Corpo c2 = Corpos[j];
                if (c1   == null || c2 == null) continue;
                if (corposParaRemover.Contains(c1) || corposParaRemover.Contains(c2)) continue;

                double dX        = c2.PosX - c1.PosX;
                double dY        = c2.PosY - c1.PosY;
                double distancia = Math.Sqrt(dX * dX + dY * dY);

                if (distancia < (c1.Raio + c2.Raio))
                {
                    Corpo corpoResultante = c1 + c2;
                    corposParaAdicionar.Add(corpoResultante);
                    corposParaRemover.Add(c1);
                    corposParaRemover.Add(c2);
                }
            }
        }

        // Atualiza o array de corpos removendo os fundidos e adicionando os novos
        if (corposParaRemover.Any())
        {
            var proximosCorpos = new List<Corpo>(QuantidadeCorpos);
            for (int i         = 0; i < QuantidadeCorpos; i++)
            {
                if (!corposParaRemover.Contains(Corpos[i]))
                    proximosCorpos.Add(Corpos[i]);
            }
            proximosCorpos.AddRange(corposParaAdicionar);

            Corpos           = proximosCorpos.ToArray();
            QuantidadeCorpos = Corpos.Length;
        }
    }
}