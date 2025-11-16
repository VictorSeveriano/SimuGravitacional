namespace SimuGravitacional
{
    partial class TelaBd
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.dgvSimulacoes = new System.Windows.Forms.DataGridView();
            this.btnCarregarSelecionada = new System.Windows.Forms.Button();
            this.btnCancelar = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.dgvSimulacoes)).BeginInit();
            this.SuspendLayout();
            // 
            // dgvSimulacoes
            // 
            this.dgvSimulacoes.AllowUserToAddRows = false;
            this.dgvSimulacoes.AllowUserToDeleteRows = false;
            this.dgvSimulacoes.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
            | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dgvSimulacoes.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvSimulacoes.Location = new System.Drawing.Point(12, 12);
            this.dgvSimulacoes.MultiSelect = false;
            this.dgvSimulacoes.Name = "dgvSimulacoes";
            this.dgvSimulacoes.ReadOnly = true;
            this.dgvSimulacoes.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvSimulacoes.Size = new System.Drawing.Size(560, 288);
            this.dgvSimulacoes.TabIndex = 0;
            // 
            // btnCarregarSelecionada
            // 
            this.btnCarregarSelecionada.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCarregarSelecionada.Location = new System.Drawing.Point(419, 306);
            this.btnCarregarSelecionada.Name = "btnCarregarSelecionada";
            this.btnCarregarSelecionada.Size = new System.Drawing.Size(153, 23);
            this.btnCarregarSelecionada.TabIndex = 1;
            this.btnCarregarSelecionada.Text = "Carregar Selecionada";
            this.btnCarregarSelecionada.UseVisualStyleBackColor = true;
            this.btnCarregarSelecionada.Click += new System.EventHandler(this.btnCarregarSelecionada_Click);
            // 
            // btnCancelar
            // 
            this.btnCancelar.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancelar.Location = new System.Drawing.Point(338, 306);
            this.btnCancelar.Name = "btnCancelar";
            this.btnCancelar.Size = new System.Drawing.Size(75, 23);
            this.btnCancelar.TabIndex = 2;
            this.btnCancelar.Text = "Cancelar";
            this.btnCancelar.UseVisualStyleBackColor = true;
            this.btnCancelar.Click += new System.EventHandler(this.btnCancelar_Click);
            // 
            // TelaBd
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(584, 341);
            this.Controls.Add(this.btnCancelar);
            this.Controls.Add(this.btnCarregarSelecionada);
            this.Controls.Add(this.dgvSimulacoes);
            this.Name = "TelaBd";
            this.Text = "Carregar Simulação do Banco";
            this.Load += new System.EventHandler(this.TelaBd_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dgvSimulacoes)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.DataGridView dgvSimulacoes;
        private System.Windows.Forms.Button btnCarregarSelecionada;
        private System.Windows.Forms.Button btnCancelar;
    }
}