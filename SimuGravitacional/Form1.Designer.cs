namespace SimuGravitacional
{
    partial class Form1
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Código gerado pelo Designer de Formulários do Windows
        private void InitializeComponent()
        {
            lblQtdCorpos = new Label();
            txtQtdCorpos = new TextBox();
            lblQtdIteracoes = new Label();
            txtQtdIteracoes = new TextBox();
            lblTempoIteracoes = new Label();
            txtTempoIteracoes = new TextBox();
            lblMassaMin = new Label();
            txtMassaMin = new TextBox();
            lblMassaMax = new Label();
            txtMassaMax = new TextBox();
            btcalcular = new Button();
            btAleatorio = new Button();
            btSalvar = new Button();
            btCarregar = new Button();
            btParar = new Button();
            panel1 = new Panel();
            lblQtdCorposRestantes = new Label();
            txtEscala = new TextBox();
            txtQtdCorposRestantes = new TextBox();
            lblEscala = new Label();
            panel1.SuspendLayout();
            SuspendLayout();
            // 
            // lblQtdCorpos
            // 
            lblQtdCorpos.AutoSize = true;
            lblQtdCorpos.ForeColor = SystemColors.ControlLightLight;
            lblQtdCorpos.Location = new Point(51, 75);
            lblQtdCorpos.Name = "lblQtdCorpos";
            lblQtdCorpos.Size = new Size(84, 15);
            lblQtdCorpos.TabIndex = 0;
            lblQtdCorpos.Text = "Qtd de Corpos";
            // 
            // txtQtdCorpos
            // 
            txtQtdCorpos.Location = new Point(41, 93);
            txtQtdCorpos.Name = "txtQtdCorpos";
            txtQtdCorpos.Size = new Size(105, 23);
            txtQtdCorpos.TabIndex = 1;
            // 
            // lblQtdIteracoes
            // 
            lblQtdIteracoes.AutoSize = true;
            lblQtdIteracoes.ForeColor = SystemColors.ControlLightLight;
            lblQtdIteracoes.Location = new Point(58, 119);
            lblQtdIteracoes.Name = "lblQtdIteracoes";
            lblQtdIteracoes.Size = new Size(77, 15);
            lblQtdIteracoes.TabIndex = 2;
            lblQtdIteracoes.Text = "Qtd Iterações";
            // 
            // txtQtdIteracoes
            // 
            txtQtdIteracoes.Location = new Point(41, 137);
            txtQtdIteracoes.Name = "txtQtdIteracoes";
            txtQtdIteracoes.Size = new Size(105, 23);
            txtQtdIteracoes.TabIndex = 3;
            // 
            // lblTempoIteracoes
            // 
            lblTempoIteracoes.AutoSize = true;
            lblTempoIteracoes.ForeColor = SystemColors.ControlLightLight;
            lblTempoIteracoes.Location = new Point(33, 163);
            lblTempoIteracoes.Name = "lblTempoIteracoes";
            lblTempoIteracoes.Size = new Size(123, 15);
            lblTempoIteracoes.TabIndex = 4;
            lblTempoIteracoes.Text = "Tempo entre Iterações";
            // 
            // txtTempoIteracoes
            // 
            txtTempoIteracoes.Location = new Point(41, 181);
            txtTempoIteracoes.Name = "txtTempoIteracoes";
            txtTempoIteracoes.Size = new Size(105, 23);
            txtTempoIteracoes.TabIndex = 5;
            // 
            // lblMassaMin
            // 
            lblMassaMin.AutoSize = true;
            lblMassaMin.ForeColor = SystemColors.ControlLightLight;
            lblMassaMin.Location = new Point(60, 207);
            lblMassaMin.Name = "lblMassaMin";
            lblMassaMin.Size = new Size(67, 15);
            lblMassaMin.TabIndex = 6;
            lblMassaMin.Text = "Massa Mín.";
            // 
            // txtMassaMin
            // 
            txtMassaMin.Location = new Point(41, 225);
            txtMassaMin.Name = "txtMassaMin";
            txtMassaMin.Size = new Size(105, 23);
            txtMassaMin.TabIndex = 7;
            // 
            // lblMassaMax
            // 
            lblMassaMax.AutoSize = true;
            lblMassaMax.ForeColor = SystemColors.ControlLightLight;
            lblMassaMax.Location = new Point(60, 251);
            lblMassaMax.Name = "lblMassaMax";
            lblMassaMax.Size = new Size(69, 15);
            lblMassaMax.TabIndex = 8;
            lblMassaMax.Text = "Massa Máx.";
            // 
            // txtMassaMax
            // 
            txtMassaMax.Location = new Point(41, 269);
            txtMassaMax.Name = "txtMassaMax";
            txtMassaMax.Size = new Size(105, 23);
            txtMassaMax.TabIndex = 9;
            // 
            // btcalcular
            // 
            btcalcular.Location = new Point(41, 507);
            btcalcular.Name = "btcalcular";
            btcalcular.Size = new Size(105, 23);
            btcalcular.TabIndex = 10;
            btcalcular.Text = "Iniciar";
            btcalcular.UseVisualStyleBackColor = true;
            btcalcular.Click += BtCalcular_Click;
            // 
            // btAleatorio
            // 
            btAleatorio.Location = new Point(41, 478);
            btAleatorio.Name = "btAleatorio";
            btAleatorio.Size = new Size(105, 23);
            btAleatorio.TabIndex = 11;
            btAleatorio.Text = "Gerar Aleatório";
            btAleatorio.UseVisualStyleBackColor = true;
            btAleatorio.Click += BtAleatorio_Click;
            // 
            // btSalvar
            // 
            btSalvar.Location = new Point(41, 536);
            btSalvar.Name = "btSalvar";
            btSalvar.Size = new Size(105, 23);
            btSalvar.TabIndex = 14;
            btSalvar.Text = "Salvar Simulação";
            btSalvar.UseVisualStyleBackColor = true;
            btSalvar.Click += btSalvar_Click;
            // 
            // btCarregar
            // 
            btCarregar.Location = new Point(41, 565);
            btCarregar.Name = "btCarregar";
            btCarregar.Size = new Size(105, 23);
            btCarregar.TabIndex = 15;
            btCarregar.Text = "Carregar Simulação";
            btCarregar.UseVisualStyleBackColor = true;
            btCarregar.Click += btCarregar_Click;
            // 
            // btParar
            // 
            btParar.Location = new Point(41, 594);
            btParar.Name = "btParar";
            btParar.Size = new Size(105, 23);
            btParar.TabIndex = 20;
            btParar.Text = "Parar Simulação";
            btParar.UseVisualStyleBackColor = true;
            btParar.Click += btParar_Click;
            // 
            // panel1
            // 
            panel1.BackColor = SystemColors.Desktop;
            panel1.Controls.Add(lblQtdCorposRestantes);
            panel1.Controls.Add(txtEscala);
            panel1.Controls.Add(txtQtdCorposRestantes);
            panel1.Controls.Add(lblEscala);
            panel1.Controls.Add(lblQtdCorpos);
            panel1.Controls.Add(btcalcular);
            panel1.Controls.Add(btAleatorio);
            panel1.Controls.Add(txtQtdCorpos);
            panel1.Controls.Add(txtMassaMax);
            panel1.Controls.Add(lblQtdIteracoes);
            panel1.Controls.Add(lblMassaMax);
            panel1.Controls.Add(txtQtdIteracoes);
            panel1.Controls.Add(txtMassaMin);
            panel1.Controls.Add(lblTempoIteracoes);
            panel1.Controls.Add(lblMassaMin);
            panel1.Controls.Add(txtTempoIteracoes);
            panel1.Controls.Add(btSalvar);
            panel1.Controls.Add(btCarregar);
            panel1.Controls.Add(btParar);
            panel1.Dock = DockStyle.Right;
            panel1.Location = new Point(1026, 0);
            panel1.Name = "panel1";
            panel1.Size = new Size(168, 636);
            panel1.TabIndex = 11;
            // 
            // lblQtdCorposRestantes
            // 
            lblQtdCorposRestantes.AutoSize = true;
            lblQtdCorposRestantes.ForeColor = SystemColors.ControlLightLight;
            lblQtdCorposRestantes.Location = new Point(48, 295);
            lblQtdCorposRestantes.Name = "lblQtdCorposRestantes";
            lblQtdCorposRestantes.Size = new Size(98, 15);
            lblQtdCorposRestantes.TabIndex = 18;
            lblQtdCorposRestantes.Text = "Corpos Restantes";
            // 
            // txtEscala
            // 
            txtEscala.Location = new Point(41, 357);
            txtEscala.Name = "txtEscala";
            txtEscala.ReadOnly = true;
            txtEscala.Size = new Size(105, 23);
            txtEscala.TabIndex = 17;
            // 
            // txtQtdCorposRestantes
            // 
            txtQtdCorposRestantes.Location = new Point(41, 313);
            txtQtdCorposRestantes.Name = "txtQtdCorposRestantes";
            txtQtdCorposRestantes.ReadOnly = true;
            txtQtdCorposRestantes.Size = new Size(105, 23);
            txtQtdCorposRestantes.TabIndex = 19;
            // 
            // lblEscala
            // 
            lblEscala.AutoSize = true;
            lblEscala.ForeColor = SystemColors.ControlLightLight;
            lblEscala.Location = new Point(74, 339);
            lblEscala.Name = "lblEscala";
            lblEscala.Size = new Size(39, 15);
            lblEscala.TabIndex = 16;
            lblEscala.Text = "Escala";
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = SystemColors.ActiveCaptionText;
            ClientSize = new Size(1194, 636);
            Controls.Add(panel1);
            Name = "Form1";
            Text = "Simulador Gravitacional by Grupo CTURB";
            Load += Form1_Load;
            panel1.ResumeLayout(false);
            panel1.PerformLayout();
            ResumeLayout(false);
        }
        #endregion

        private Label lblQtdCorpos;
        private TextBox txtQtdCorpos;
        private Label lblQtdIteracoes;
        private TextBox txtQtdIteracoes;
        private Label lblTempoIteracoes;
        private TextBox txtTempoIteracoes;
        private Label lblMassaMin;
        private TextBox txtMassaMin;
        private Label lblMassaMax;
        private TextBox txtMassaMax;
        private Button btcalcular;
        private Button btAleatorio;
        private Button btSalvar;
        private Button btCarregar;
        private Button btParar;
        private Panel panel1;
        private Label lblEscala;
        private TextBox txtEscala;
        private Label lblQtdCorposRestantes;
        private TextBox txtQtdCorposRestantes;
    }
}
