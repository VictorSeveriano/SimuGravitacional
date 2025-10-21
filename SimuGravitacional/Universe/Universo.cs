// Gerencia os corpos e a logica de simulação

using SimuGravitacional;
using System;
using System.Collections.Generic;
using System.Linq;

public class Universo
{
    // Constante de gravitação
    private const double G = 6.6743e-11;

    // Array interno de corpos
    public Corpo[] Corpos { get; private set; }

    // Contador de quantos corpos existem atualmente
    public int QuantidadeCorpos { get; private set; }

    //inicializa universo vazio
    public Universo()
    {
        Corpos = new Corpo[0];
        QuantidadeCorpos = 0;
    }

    // Insere quantidade corpos aleatórios dentro da área definida
    public void InserirCorpos(int quantidade, double massaMin, double massaMax, int larguraMax, int alturaMax)
    {
        // se quantidade for invalida, zera o universo
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

            // posição aleatória, evitando bordas
            double posX = rnd.Next(10, Math.Max(11, larguraMax - 10));
            double posY = rnd.Next(10, Math.Max(11, alturaMax - 10));

            // velocidade inicial nula — os corpos começam parados
            double velX = 0;
            double velY = 0;

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
        // se o array está cheio, dobra a capacidade
        if (QuantidadeCorpos == Corpos.Length)
        {
            int novaCapacidade = Corpos.Length == 0 ? 4 : Corpos.Length * 2;
            Corpo[] novoArray = new Corpo[novaCapacidade];
            Array.Copy(Corpos, novoArray, QuantidadeCorpos);
            Corpos = novoArray;
        }

        Corpos[QuantidadeCorpos] = novoCorpo;
        QuantidadeCorpos++;
    }

    // Simula um passo de tempo, calcula as forças, atualiza velocidade, pos e trata colisoes
    public void SimularPasso(double tempoDoPasso)
    {
        var proximosEstados = new Dictionary<Corpo, (double velX, double velY, double posX, double posY)>();

        //Para cada corpo, calcula a força total exercida por todos os outros corpos
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

            double aceleracaoX = forcaTotalX / corpoA.Massa;
            double aceleracaoY = forcaTotalY / corpoA.Massa;

            double novaVelX = corpoA.VelX + (aceleracaoX * tempoDoPasso);
            double novaVelY = corpoA.VelY + (aceleracaoY * tempoDoPasso);

            double novaPosX = corpoA.PosX + (corpoA.VelX * tempoDoPasso) + (0.5 * aceleracaoX * tempoDoPasso * tempoDoPasso);
            double novaPosY = corpoA.PosY + (corpoA.VelY * tempoDoPasso) + (0.5 * aceleracaoY * tempoDoPasso * tempoDoPasso);

            proximosEstados[corpoA] = (novaVelX, novaVelY, novaPosX, novaPosY);
        }

        // aplica os novos estados
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

        // Detecta e trata colisões
        var corposParaRemover = new HashSet<Corpo>();
        var corposParaAdicionar = new List<Corpo>();

        for (int i = 0; i < QuantidadeCorpos; i++)
        {
            for (int j = i + 1; j < QuantidadeCorpos; j++)
            {
                Corpo c1 = Corpos[i];
                Corpo c2 = Corpos[j];
                if (c1 == null || c2 == null) continue;
                if (corposParaRemover.Contains(c1) || corposParaRemover.Contains(c2)) continue;

                double dX = c2.PosX - c1.PosX;
                double dY = c2.PosY - c1.PosY;
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

        if (corposParaRemover.Any())
        {
            var proximosCorpos = new List<Corpo>(QuantidadeCorpos);
            for (int i = 0; i < QuantidadeCorpos; i++)
            {
                if (!corposParaRemover.Contains(Corpos[i]))
                    proximosCorpos.Add(Corpos[i]);
            }
            proximosCorpos.AddRange(corposParaAdicionar);

            Corpos = proximosCorpos.ToArray();
            QuantidadeCorpos = Corpos.Length;
        }
    }
}
