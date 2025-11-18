// Gerencia os corpos e a lógica de simulação

using SimuGravitacional;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks; 
using System.Collections.Concurrent; 

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

            Corpo novoCorpo = new Corpo($"C{i + 1}", massa, densidade, posX, posY, velX, velY);
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
            Corpo[] novoArray = new Corpo[novaCapacidade];
            Array.Copy(Corpos, novoArray, QuantidadeCorpos);
            Corpos = novoArray;
        }

        Corpos[QuantidadeCorpos] = novoCorpo;
        QuantidadeCorpos++;
    }

    // Cada tick do timer é tratado como uma unidade de tempo fixa
    public void SimularPasso()
    {
        // usando o ConcurrentDictionary para ser threadsafe
        var proximosEstados = new ConcurrentDictionary<Corpo, (double velX, double velY, double posX, double posY)>();

        // Usa Parallel.For para dividir cálculos entre múltiplos núcleos
        Parallel.For(0, QuantidadeCorpos, i =>
        {
            // Corpo atual
            var corpoA = Corpos[i];
            if (corpoA == null) return; // ignora corpo nulo

            double forcaTotalX = 0;
            double forcaTotalY = 0;

            // Loop interno calcula a força de todos os outros corpos sobre corpoA
            for (int j = 0; j < QuantidadeCorpos; j++)
            {
                if (i == j) continue; // evita comparar o mesmo corpo
                var corpoB = Corpos[j];
                if (corpoB == null) continue;

                // Distância entre corpos
                double dX = corpoB.PosX - corpoA.PosX;
                double dY = corpoB.PosY - corpoA.PosY;
                double distancia = Math.Sqrt(dX * dX + dY * dY);
                if (distancia < 1e-6) continue; // evita divisões por zero

                // Calcula força gravitacional
                double forcaMag = (G * corpoA.Massa * corpoB.Massa) / (distancia * distancia);

                // Direciona a força nos eixos X e Y
                double forcaX = forcaMag * (dX / distancia);
                double forcaY = forcaMag * (dY / distancia);

                // Soma as forças totais aplicadas ao corpoA
                forcaTotalX += forcaX;
                forcaTotalY += forcaY;
            }

            // Calcula aceleração resultante
            double acelX = forcaTotalX / corpoA.Massa;
            double acelY = forcaTotalY / corpoA.Massa;

            // Atualiza velocidade com base na aceleração
            double novaVelX = corpoA.VelX + acelX;
            double novaVelY = corpoA.VelY + acelY;

            // Atualiza posição (usa deslocamento + aceleração)
            double novaPosX = corpoA.PosX + corpoA.VelX + (0.5 * acelX);
            double novaPosY = corpoA.PosY + corpoA.VelY + (0.5 * acelY);

            // Armazena o novo estado no dicionário compartilhado
            proximosEstados[corpoA] = (novaVelX, novaVelY, novaPosX, novaPosY);
        }); // paralelismo vai ate aqui


        // Aplica os novos estados calculados
        for (int i = 0; i < QuantidadeCorpos; i++)
        {
            var corpo = Corpos[i];
            if (corpo == null) continue; // ignora corpos inexistentes

            // Obtém o estado calculado na etapa paralela
            if (proximosEstados.TryGetValue(corpo, out var novoEstado))
            {
                // Atualiza propriedades físicas
                corpo.VelX = novoEstado.velX;
                corpo.VelY = novoEstado.velY;
                corpo.PosX = novoEstado.posX;
                corpo.PosY = novoEstado.posY;
            }
        }

        // Verifica e trata colisões entre corpos
        var corposParaRemover = new HashSet<Corpo>(); // corpos fundidos/removidos
        var corposParaAdicionar = new List<Corpo>();  // corpos resultantes de fusão

        // Percorre todos os pares de corpos
        for (int i = 0; i < QuantidadeCorpos; i++)
        {
            for (int j = i + 1; j < QuantidadeCorpos; j++)
            {
                Corpo c1 = Corpos[i];
                Corpo c2 = Corpos[j];

                if (c1 == null || c2 == null) continue; // ignora nulos
                if (corposParaRemover.Contains(c1) || corposParaRemover.Contains(c2)) continue;

                // Distância entre centros
                double dX = c2.PosX - c1.PosX;
                double dY = c2.PosY - c1.PosY;
                double distancia = Math.Sqrt(dX * dX + dY * dY);

                // Verifica colisão (interseção de raios)
                if (distancia < (c1.Raio + c2.Raio))
                {
                    // Cria corpo resultante da fusão
                    Corpo corpoResultante = c1 + c2;

                    // Marca corpos antigos para remoção
                    corposParaRemover.Add(c1);
                    corposParaRemover.Add(c2);

                    // Adiciona novo corpo à lista de criação
                    corposParaAdicionar.Add(corpoResultante);
                }
            }
        }

        // Atualiza a lista principal de corpos se houve fusões
        if (corposParaRemover.Any())
        {
            // Cria nova lista sem os corpos removidos
            var proximosCorpos = new List<Corpo>(QuantidadeCorpos - corposParaRemover.Count);

            // Mantém apenas corpos ativos
            for (int i = 0; i < QuantidadeCorpos; i++)
            {
                var corpo = Corpos[i];
                if (corpo != null && !corposParaRemover.Contains(corpo))
                    proximosCorpos.Add(corpo);
            }

            // Adiciona novos corpos fundidos
            proximosCorpos.AddRange(corposParaAdicionar);

            // Atualiza lista principal e contador
            Corpos = proximosCorpos.ToArray();
            QuantidadeCorpos = Corpos.Length;
        }
    }
}