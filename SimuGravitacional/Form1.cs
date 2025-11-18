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
        private Universo universo = new Universo();          // universo que contém os corpos e lógica
        private Timer timer = new Timer();                  // timer do Windows Forms para passos da simulação


        private GravitacaoDAO dao = new GravitacaoMySQLDAO();   // salvar e carregar no MySQL

        private long iteracoesMax = 0;       // número máximo de iterações a executar
        private long iteracaoAtual = 0;      // contador da iteração atual

        // variaveis da visualização
        private float escala = 1f;    // escala usada para desenhar
        private float offsetX = 0f;   // offset X para centralizar a visualização
        private float offsetY = 0f;   // offset Y para centralizar a visualização

        // variaveis para reprise
        private Universo universoInicial = null; // Armazena o estado inicial
        private long iteracoesParaReprise = 0;   // Se > 0, indica que estamos em modo reprise
        private bool modoRepriseAtivo = false;   // Flag para saber se um reprise está rodando

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
                universo = universoInicial.Clone(); // Reseta para o estado inicial salvo
                iteracaoAtual = 0;                      // Começa a reprise do zero
                iteracoesMax = iteracoesParaReprise;  // O alvo da reprise é a iteração salva

                // Atualiza UI para refletir o início da reprise
                txtQtdCorposRestantes.Text = universo.QuantidadeCorpos.ToString();

                AjustarEscala();
                this.Invalidate();

                // O intervalo do timer já foi configurado no btCarregar_Click
                modoRepriseAtivo = true; // ativa a flag de reprise
                timer.Start();

                iteracoesParaReprise = 0; // Limpa a flag de reprise
            }

            // modo normal se não for reprise
            else
            {
                try
                {
                    int quantidade = int.Parse(txtQtdCorpos.Text);
                    iteracoesMax = long.Parse(txtQtdIteracoes.Text);
                    int intervaloTimer = int.Parse(txtTempoIteracoes.Text) * 10;
                    double massaMin = double.Parse(txtMassaMin.Text, CultureInfo.InvariantCulture);
                    double massaMax = double.Parse(txtMassaMax.Text, CultureInfo.InvariantCulture);

                    universo.InserirCorpos(quantidade, massaMin, massaMax, ClientSize.Width, ClientSize.Height);

                    // Salva o estado inicial para um futuro reprise
                    universoInicial = universo.Clone();
                    iteracoesParaReprise = 0; // Garante que não está em modo reprise

                    modoRepriseAtivo = false; // desativa a flag, simulação normal

                    txtQtdCorposRestantes.Text = universo.QuantidadeCorpos.ToString();

                    AjustarEscala();

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
                int quantidade = int.Parse(txtQtdCorpos.Text);
                double massaMin = double.Parse(txtMassaMin.Text, CultureInfo.InvariantCulture);
                double massaMax = double.Parse(txtMassaMax.Text, CultureInfo.InvariantCulture);

                universo.InserirCorpos(quantidade, massaMin, massaMax, ClientSize.Width, ClientSize.Height);

                // Salva o estado inicial
                universoInicial = universo.Clone();
                iteracoesParaReprise = 0; // Limpa a flag de reprise
                modoRepriseAtivo = false; // Não é reprise

                iteracaoAtual = 0;
                txtQtdCorposRestantes.Text = universo.QuantidadeCorpos.ToString();

                AjustarEscala();

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

                // Verifica se a simulação que parou era um reprise
                if (modoRepriseAtivo)
                {
                    MessageBox.Show("Reprise da simulação concluído!");
                    modoRepriseAtivo = false; // Desarma a flag
                }

                return;
            }

            // cada tick = 1 unidade de tempo
            universo.SimularPasso();
            iteracaoAtual++;

            txtQtdCorposRestantes.Text = universo.QuantidadeCorpos.ToString();

            this.Invalidate();
        }

        // Método que inicia a simulação configurando o timer
        private void IniciarSimulacao(int intervalo)
        {
            iteracaoAtual = 0;
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
                modoRepriseAtivo = false; // Reseta a flag em parada manual
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
            double alturaOcupada = maxY - minY;

            int larguraDisponivel = this.ClientSize.Width - panel1.Width;
            int alturaDisponivel = this.ClientSize.Height;

            if (larguraOcupada == 0 || alturaOcupada == 0) return;

            float escalaX = (float)(larguraDisponivel / larguraOcupada);
            float escalaY = (float)(alturaDisponivel / alturaOcupada);

            escala = Math.Min(escalaX, escalaY);

            if (float.IsInfinity(escala) || float.IsNaN(escala))
                escala = 1f;

            //calculo para centralizar 
            double centroSimX = minX + (larguraOcupada / 2.0);
            double centroSimY = minY + (alturaOcupada / 2.0);
            float centroTelaX = (larguraDisponivel / 2.0f);
            float centroTelaY = (alturaDisponivel / 2.0f);
            offsetX = centroTelaX - (float)(centroSimX * escala);

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
                //  Aplica o PAN (calculado em AjustarEscala) para centralizar
                g.TranslateTransform(offsetX, offsetY);
                // Aplica o ZOOM (calculado em AjustarEscala)
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

        private void btSalvar_Click(object sender, EventArgs e)
        {
            // Verifica se uma simulação foi iniciada e se universoInicial existe
            if (universoInicial == null)
            {
                MessageBox.Show("Você precisa iniciar uma simulação antes de poder salvar.");
                return;
            }

            try
            {
                // O primeiro parâmetro é ignorado pelo MySQLDAO, então passamos null.
                // O timer.Interval é o tempo em milissegundos 
                dao.Salvar(null, universoInicial, universo, (int)iteracaoAtual, timer.Interval);
                MessageBox.Show("A sua simulação foi salva com sucesso no banco de dados!");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erro ao salvar no banco de dados: " + ex.Message);
            }
        }
        private void btCarregar_Click(object sender, EventArgs e)
        {
            // Pára qualquer simulação em andamento
            timer.Stop();
            modoRepriseAtivo = false;

            // Cria uma instância do novo formulário TelaBd
            // Passa a instância 'dao' existente para o construtor do TelaBd
            using (TelaBd telaSelecao = new TelaBd(this.dao))
            {
                // Mostra o formulário de seleção como um diálogo modal
                var resultado = telaSelecao.ShowDialog();

                // Verifica se o usuário clicou em "Carregar"
                if (resultado == DialogResult.OK)
                {
                    // Pega o ID que o usuário selecionou no TelaBd
                    string idParaCarregar = telaSelecao.IdSimulacaoSelecionada.ToString();

                    // Verifica se um ID válido foi selecionado
                    if (string.IsNullOrWhiteSpace(idParaCarregar) || idParaCarregar == "-1")
                    {
                        return; // Algo deu errado ou foi cancelado
                    }

                    try
                    {
                        // A lógica de carregamento restante é a mesma de antes
                        int iteracaoSalva, tempoIteracao;

                        // Usamos o ID selecionado (como string) no lugar do caminho do arquivo
                        universo = dao.Carregar(idParaCarregar, out iteracaoSalva, out tempoIteracao);
                        universoInicial = universo.Clone(); // Armazena o estado inicial carregado

                        iteracoesParaReprise = iteracaoSalva;          // Define o alvo da reprise
                        iteracoesMax = iteracaoSalva;         // O "total" de iterações agora é o ponto salvo
                        timer.Interval = tempoIteracao;     // tempoIteracao já vem em milissegSgundos
                        iteracaoAtual = 0;                    // Começamos do zero

                        // Atualiza a UI para refletir o estado INICIAL
                        txtQtdCorpos.Text = universo.QuantidadeCorpos.ToString();
                        txtQtdIteracoes.Text = iteracoesMax.ToString(); // Mostra o total da reprise
                        // Divide por 10 para mostrar o valor original no textbox
                        txtTempoIteracoes.Text = (timer.Interval / 10).ToString();
                        txtQtdCorposRestantes.Text = universo.QuantidadeCorpos.ToString();

                        AjustarEscala();
                        this.Invalidate(); // Desenha o estado inicial parado

                        MessageBox.Show($"Simulação {idParaCarregar} carregada. Clique em 'Iniciar' para reprisar.");
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Erro ao carregar simulação: " + ex.Message);
                    }
                }
                // Se o resultado for 'Cancel' (o usuário fechou o TelaBd), não fazemos nada.
            }
        }

        private void txtQtdIteracoes_TextChanged(object sender, EventArgs e)
        {

        }

        private void txtTempoIteracoes_TextChanged(object sender, EventArgs e)
        {

        }
    }
}