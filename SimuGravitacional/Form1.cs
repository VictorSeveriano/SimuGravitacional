// Interface grafica da simulação,controla entrada do usuario, tempo e desenho dos corpos

using SimuGravitacional.Abstrato;
using System;
using System.Drawing;
using System.Globalization;
using System.Windows.Forms;
using Timer = System.Windows.Forms.Timer;

namespace SimuGravitacional
{
    public partial class Form1 : Form
    {
        private Universo universo = new Universo();               // universo que contém os corpos e lógica
        private Timer timer = new Timer();                 // timer do Windows Forms para passos da simulação
        private GravitacaoDAO dao = new GravitacaoArquivoDAO(); //salvar e carregar em arquivo

        private long iteracoesMax = 0;       // número maximo de iterações a executar
        private long iteracaoAtual = 0;      // contador da iteraçao atual
        private float escala = 1f;    // escala usada para desenhar

        private double deltaT = 0.3;

        // Construtor do form
        public Form1()
        {
            InitializeComponent();           // método gerado pelo designer que cria controles
            this.DoubleBuffered = true;
            this.Paint += Form1_Paint;       // associa o evento Paint ao método de desenho
            timer.Tick += Timer_Tick;        // associa o evento Tick do timer ao método que faz o passo da simulação
        }

        // Evento do botão "Calcular" (inicia a simulação com os parâmetros lidos)
        private void BtCalcular_Click(object sender, EventArgs e)
        {
            try
            {
                // le parametros digitados pelo usuario
                int quantidade = int.Parse(txtQtdCorpos.Text);
                iteracoesMax = long.Parse(txtQtdIteracoes.Text);
                int intervalo = int.Parse(txtTempoIteracoes.Text);
                double massaMin = double.Parse(txtMassaMin.Text, CultureInfo.InvariantCulture);
                double massaMax = double.Parse(txtMassaMax.Text, CultureInfo.InvariantCulture);

                // se o campo deltaT não está vazio, sobrescreve o valor padrao
                if (!string.IsNullOrWhiteSpace(txtDeltaT.Text))
                    deltaT = double.Parse(txtDeltaT.Text, CultureInfo.InvariantCulture);

                // gera corpos no universo com os parâmetros fornecidos e tamanho da area do cliente
                universo.InserirCorpos(quantidade, massaMin, massaMax, ClientSize.Width, ClientSize.Height);

                // atualiza a caixa que mostra quantos corpos restam
                txtQtdCorposRestantes.Text = universo.QuantidadeCorpos.ToString();

                // inicia a simulaçao com o intervalo de timer informado
                IniciarSimulacao(intervalo);
            }
            catch (FormatException)
            {
                // trata entradas inválidas
                MessageBox.Show("Por favor insira valores numéricos válidos.");
            }
        }

        // Evento do botão Aleatório
        private void BtAleatorio_Click(object sender, EventArgs e)
        {
            try
            {
                // le quantidade e massas de entrada
                int quantidade = int.Parse(txtQtdCorpos.Text);
                double massaMin = double.Parse(txtMassaMin.Text, CultureInfo.InvariantCulture);
                double massaMax = double.Parse(txtMassaMax.Text, CultureInfo.InvariantCulture);

                // gera os corpos aleatórios
                universo.InserirCorpos(quantidade, massaMin, massaMax, ClientSize.Width, ClientSize.Height);

                // reseta contador de iteração e atualiza contador de corpos restantes 
                iteracaoAtual = 0;
                txtQtdCorposRestantes.Text = universo.QuantidadeCorpos.ToString();
                this.Invalidate(); // solicita redesenho do formulário
                MessageBox.Show($"{quantidade} corpos gerados com sucesso!");
            }
            catch (FormatException)
            {
                MessageBox.Show("Por favor insira valores numéricos válidos.");
            }
        }

        // Evento do timer, executa um passo da simulação a cada tick
        private void Timer_Tick(object sender, EventArgs e)
        {
            // condição de parada,atingiu maximo de iteraçoes ou sobrou 1 ou 0 corpos
            if (iteracaoAtual >= iteracoesMax || universo.QuantidadeCorpos <= 1)
            {
                timer.Stop();
                return;
            }

            // executa um passo da simulação passando deltaT
            universo.SimularPasso(deltaT);
            iteracaoAtual++; // incrementa o contador

            // atualiza a quantidade de corpos remanescentes 
            txtQtdCorposRestantes.Text = universo.QuantidadeCorpos.ToString();

            // ajusta a escala de desenho para caber todos os corpos na janela
            AjustarEscala();

            // solicita redesenho
            this.Invalidate();
        }

        // Método que inicia a simulação configurando o timer
        private void IniciarSimulacao(int intervalo)
        {
            iteracaoAtual = 0;     // zera o contador de iterações
            timer.Interval = intervalo; // define intervalo do timer
            timer.Start();
            this.Invalidate();
        }

        // ---------------------------
        // NOVO: Botão "Parar Simulação"
        // ---------------------------
        private void btParar_Click(object sender, EventArgs e)
        {
            // se o timer estiver ativo, para e informa o usuário
            if (timer.Enabled)
            {
                timer.Stop();
                MessageBox.Show("A simulação foi interrompida com sucesso!");
            }
            else
            {
                MessageBox.Show("A simulação já estava parada.");
            }
        }

        // Ajusta a escala de desenho para que todos os corpos caibam na area disponivel
        private void AjustarEscala()
        {
            if (universo.QuantidadeCorpos == 0) return;

            // inicia limites com valores extremos
            double minX = double.MaxValue, minY = double.MaxValue;
            double maxX = double.MinValue, maxY = double.MinValue;

            // percorre cada corpo para calcular
            for (int i = 0; i < universo.QuantidadeCorpos; i++)
            {
                var corpo = universo.Corpos[i];
                if (corpo == null) continue;

                // calcula limites do corpo
                double corpoMinX = corpo.PosX - corpo.Raio;
                double corpoMinY = corpo.PosY - corpo.Raio;
                double corpoMaxX = corpo.PosX + corpo.Raio;
                double corpoMaxY = corpo.PosY + corpo.Raio;

                // atualiza min/max se necessário
                if (corpoMinX < minX) minX = corpoMinX;
                if (corpoMinY < minY) minY = corpoMinY;
                if (corpoMaxX > maxX) maxX = corpoMaxX;
                if (corpoMaxY > maxY) maxY = corpoMaxY;
            }

            // cálculo da largura e altura ocupadas pelos corpos
            double larguraOcupada = maxX - minX;
            double alturaOcupada = maxY - minY;

            // área disponível para desenho 
            int larguraDisponivel = this.ClientSize.Width - panel1.Width;
            int alturaDisponivel = this.ClientSize.Height;

            // evita divisão por zero
            if (larguraOcupada == 0 || alturaOcupada == 0)
                return;

            // escala em X e Y para caber tudo
            float escalaX = (float)(larguraDisponivel / larguraOcupada);
            float escalaY = (float)(alturaDisponivel / alturaOcupada);

            // escolhe a menor escala
            escala = Math.Min(escalaX, escalaY);

            // trata valores inválidos (mas não limita o máximo)
            if (float.IsInfinity(escala) || float.IsNaN(escala))
                escala = 1f;

            // mostra a escala
            txtEscala.Text = escala.ToString("F10");
        }

        // Evento de desenh, desenha o universo
        private void Form1_Paint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            // suaviza curvas e círculos
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            // limpa o fundo com preto
            g.Clear(Color.Black);

            // Se houver corpos, calcula o menor minX/minY para translação
            if (universo.Corpos != null && universo.QuantidadeCorpos > 0)
            {
                double minX = double.MaxValue, minY = double.MaxValue;
                for (int i = 0; i < universo.QuantidadeCorpos; i++)
                {
                    var corpo = universo.Corpos[i];
                    if (corpo == null) continue;
                    double corpoMinX = corpo.PosX - corpo.Raio;
                    double corpoMinY = corpo.PosY - corpo.Raio;
                    if (corpoMinX < minX) minX = corpoMinX;
                    if (corpoMinY < minY) minY = corpoMinY;
                }

                // aplica transformação, move a origem e aplica escala
                g.TranslateTransform(-(float)minX * escala, -(float)minY * escala);
                g.ScaleTransform(escala, escala);
            }

            // Desenha cada corpo como um círculo preenchido branco
            if (universo.Corpos != null)
            {
                for (int i = 0; i < universo.QuantidadeCorpos; i++)
                {
                    var corpo = universo.Corpos[i];
                    if (corpo == null) continue;

                    // se o raio calculado for 0, usa 1 para desenhar algo visível
                    float raio = (float)(corpo.Raio > 0 ? corpo.Raio : 1);
                    // calcula canto superior esquerdo do círculo (x,y) a partir do centro posX,posY
                    float x = (float)(corpo.PosX - raio);
                    float y = (float)(corpo.PosY - raio);
                    float diametro = raio * 2;

                    // desenha com Brush vermelho
                    using (Brush pincel = new SolidBrush(Color.White))
                    {
                        g.FillEllipse(pincel, x, y, diametro, diametro);
                    }
                }
            }
        }

        // Evento de carregamento do form (vazio aqui, mas existe para o designer)
        private void Form1_Load(object sender, EventArgs e)
        {
        }

        // Botão "Salvar", salva o estado atual da simulaçao
        private void btSalvar_Click(object sender, EventArgs e)
        {
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Filter = "Arquivos TXT (*.txt)|*.txt";
            if (sfd.ShowDialog() == DialogResult.OK)
            {
                // usa o DAO para salvar em arquivo
                dao.Salvar(sfd.FileName, universo, (int)iteracoesMax, timer.Interval, deltaT);
                MessageBox.Show("A sua simulação foi salva com sucesso! Omedetō!!");
            }
        }

        // Botão "Carregar", carrega e atualiza a interface com os dados lidos
        private void btCarregar_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "Arquivos TXT (*.txt)|*.txt";
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                int qtdIteracoes, tempoIteracao;
                double deltaTCarregado;

                // usa o DAO para carregar; retorna universo e out parâmetros
                universo = dao.Carregar(ofd.FileName, out qtdIteracoes, out tempoIteracao, out deltaTCarregado);

                // atualiza variáveis locais com os valores carregados
                iteracoesMax = qtdIteracoes;
                timer.Interval = tempoIteracao;
                deltaT = deltaTCarregado;

                // atualiza os controles de texto para refletir os valores
                txtDeltaT.Text = deltaT.ToString("F2", CultureInfo.InvariantCulture);
                txtQtdCorpos.Text = universo.QuantidadeCorpos.ToString();
                txtQtdIteracoes.Text = iteracoesMax.ToString();
                txtTempoIteracoes.Text = timer.Interval.ToString();

                // reseta contador de iterações, atualiza contador de corpos e redesenha
                iteracaoAtual = 0;
                txtQtdCorposRestantes.Text = universo.QuantidadeCorpos.ToString();
                this.Invalidate();

                MessageBox.Show("A sua simulação foi carregada com sucesso! Totemo yoi!");
            }
        }
    }
}
