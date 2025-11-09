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
        private Timer timer       = new Timer();                  // timer do Windows Forms para passos da simulação
        private GravitacaoDAO dao = new GravitacaoArquivoDAO();   // salvar e carregar em arquivo

        private long iteracoesMax  = 0;       // número máximo de iterações a executar
        private long iteracaoAtual = 0;      // contador da iteração atual
        private float escala       = 1f;    // escala usada para desenhar

        // VARIÁVEIS PARA REPRISE
        private Universo universoInicial  = null; // Armazena o estado inicial
        private long iteracoesParaReprise = 0;   // Se > 0, indica que estamos em modo reprise

        // Construtor do form
        public Form1()
        {
            InitializeComponent();
            this.DoubleBuffered = true;
            this.Paint += Form1_Paint;
            timer.Tick += Timer_Tick;
        }

        // Evento do botão Calcular, inicia a simulação com os parâmetros lidos
        private void BtCalcular_Click(object sender, EventArgs e)
        {
            // modo reprise se um arquivo for carregado
            if (iteracoesParaReprise > 0 && universoInicial != null)
            {
                universo      = universoInicial.Clone(); // Reseta para o estado inicial salvo
                iteracaoAtual = 0;                      // Começa a reprise do zero
                iteracoesMax  = iteracoesParaReprise;  // O alvo da reprise é a iteração salva

                // Atualiza UI para refletir o início da reprise
                txtQtdCorposRestantes.Text = universo.QuantidadeCorpos.ToString();
                this.Invalidate();

                // O intervalo do timer já foi configurado no btCarregar_Click
                timer.Start();

                iteracoesParaReprise = 0; // Limpa a flag de reprise
            }

            // modo normal se não for reprise
            else
            {
                try
                {
                    int quantidade     = int.Parse(txtQtdCorpos.Text);
                    iteracoesMax       = long.Parse(txtQtdIteracoes.Text);
                    int intervaloTimer = int.Parse(txtTempoIteracoes.Text) / 10;
                    double massaMin    = double.Parse(txtMassaMin.Text, CultureInfo.InvariantCulture);
                    double massaMax    = double.Parse(txtMassaMax.Text, CultureInfo.InvariantCulture);

                    universo.InserirCorpos(quantidade, massaMin, massaMax, ClientSize.Width, ClientSize.Height);

                    // Salva o estado inicial para um futuro reprise
                    universoInicial      = universo.Clone();
                    iteracoesParaReprise = 0; // Garante que não está em modo reprise

                    txtQtdCorposRestantes.Text = universo.QuantidadeCorpos.ToString();

                    IniciarSimulacao(intervaloTimer);
                }
                catch (FormatException)
                {
                    MessageBox.Show("Por favor insira valores numéricos válidos.");
                }
            }
        }

        // Evento do botão Aleatório
        private void BtAleatorio_Click(object sender, EventArgs e)
        {
            try
            {
                int quantidade  = int.Parse(txtQtdCorpos.Text);
                double massaMin = double.Parse(txtMassaMin.Text, CultureInfo.InvariantCulture);
                double massaMax = double.Parse(txtMassaMax.Text, CultureInfo.InvariantCulture);

                universo.InserirCorpos(quantidade, massaMin, massaMax, ClientSize.Width, ClientSize.Height);

                // Salva o estado inicial
                universoInicial      = universo.Clone();
                iteracoesParaReprise = 0; // Limpa a flag de reprise

                iteracaoAtual              = 0;
                txtQtdCorposRestantes.Text = universo.QuantidadeCorpos.ToString();
                this.Invalidate();
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
            // para automaticamente ao atingir iteracoesMax
            if (iteracaoAtual >= iteracoesMax || universo.QuantidadeCorpos <= 1)
            {
                timer.Stop();
                return;
            }

            // cada tick = 1 unidade de tempo
            universo.SimularPasso();
            iteracaoAtual++;

            txtQtdCorposRestantes.Text = universo.QuantidadeCorpos.ToString();
            AjustarEscala();
            this.Invalidate();
        }

        // Método que inicia a simulação configurando o timer
        private void IniciarSimulacao(int intervalo)
        {
            iteracaoAtual  = 0;
            timer.Interval = intervalo;
            timer.Start();
            this.Invalidate();
        }

        // parar simulação
        private void btParar_Click(object sender, EventArgs e)
        {
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

            double minX = double.MaxValue, minY = double.MaxValue;
            double maxX = double.MinValue, maxY = double.MinValue;

            for (int i = 0; i < universo.QuantidadeCorpos; i++)
            {
                var corpo = universo.Corpos[i];
                if (corpo == null) continue;

                double corpoMinX = corpo.PosX - corpo.Raio;
                double corpoMinY = corpo.PosY - corpo.Raio;
                double corpoMaxX = corpo.PosX + corpo.Raio;
                double corpoMaxY = corpo.PosY + corpo.Raio;

                if (corpoMinX < minX) minX = corpoMinX;
                if (corpoMinY < minY) minY = corpoMinY;
                if (corpoMaxX > maxX) maxX = corpoMaxX;
                if (corpoMaxY > maxY) maxY = corpoMaxY;
            }

            double larguraOcupada = maxX - minX;
            double alturaOcupada  = maxY - minY;

            int larguraDisponivel = this.ClientSize.Width - panel1.Width;
            int alturaDisponivel  = this.ClientSize.Height;

            if (larguraOcupada == 0 || alturaOcupada == 0) return;

            float escalaX = (float)(larguraDisponivel / larguraOcupada);
            float escalaY = (float)(alturaDisponivel / alturaOcupada);

            escala = Math.Min(escalaX, escalaY);

            if (float.IsInfinity(escala) || float.IsNaN(escala))
                escala = 1f;

            txtEscala.Text = escala.ToString("F10");
        }

        // Evento de desenh, desenha o universo
        private void Form1_Paint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            g.Clear(Color.Black);

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

                g.TranslateTransform(-(float)minX * escala, -(float)minY * escala);
                g.ScaleTransform(escala, escala);
            }

            if (universo.Corpos != null)
            {
                for (int i = 0; i < universo.QuantidadeCorpos; i++)
                {
                    var corpo = universo.Corpos[i];
                    if (corpo == null) continue;

                    float raio = (float)(corpo.Raio > 0 ? corpo.Raio : 1);
                    float x = (float)(corpo.PosX - raio);
                    float y = (float)(corpo.PosY - raio);
                    float diametro = raio * 2;

                    using (Brush pincel = new SolidBrush(Color.White))
                    {
                        g.FillEllipse(pincel, x, y, diametro, diametro);
                    }
                }
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
        }

        // Botão Salvar, salva o estado INICIAL e o ATUAL com colisões
        private void btSalvar_Click(object sender, EventArgs e)
        {
            // Verifica se uma simulação foi iniciada e se universoInicial existe
            if (universoInicial == null)
            {
                MessageBox.Show("Você precisa iniciar uma simulação (com 'Gerar Aleatório' ou 'Iniciar') antes de poder salvar.");
                return;
            }

            SaveFileDialog sfd   = new SaveFileDialog();
            sfd.Filter           = "Arquivos TXT (*.txt)|*.txt";
            if (sfd.ShowDialog() == DialogResult.OK)
            {
                // Passamos 'universoInicial' para o reprise e 'universo' o estado ATUAL com colisões
                dao.Salvar(sfd.FileName, universoInicial, universo, (int)iteracaoAtual, timer.Interval);
                MessageBox.Show("A sua simulação foi salva com sucesso!");
            }
        }

        // Botão "Carregar", carrega e prepara a reprise
        private void btCarregar_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd   = new OpenFileDialog();
            ofd.Filter           = "Arquivos TXT (*.txt)|*.txt";
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                // Pára qualquer simulação em andamento
                timer.Stop();

                int iteracaoSalva, tempoIteracao;

                // Carrega o estado INICIAL e os parâmetros da reprise
                // O DAO agora sabe procurar o bloco ESTADO_INICIAL
                universo        = dao.Carregar(ofd.FileName, out iteracaoSalva, out tempoIteracao);
                universoInicial = universo.Clone(); // Armazena o estado inicial carregado

                iteracoesParaReprise = iteracaoSalva;          // Define o alvo da reprise
                iteracoesMax         = iteracaoSalva;         // O "total" de iterações agora é o ponto salvo
                timer.Interval       = tempoIteracao;
                iteracaoAtual        = 0;                    // Começamos do zero

                // Atualiza a UI para refletir o estado INICIAL
                txtQtdCorpos.Text          = universo.QuantidadeCorpos.ToString();
                txtQtdIteracoes.Text       = iteracoesMax.ToString(); // Mostra o total da reprise
                txtTempoIteracoes.Text     = timer.Interval.ToString();
                txtQtdCorposRestantes.Text = universo.QuantidadeCorpos.ToString();

                this.Invalidate(); // Desenha o estado inicial parado

                MessageBox.Show("Simulação carregada. Clique em 'Iniciar' para reprisar.");
            }
        }
    }
}