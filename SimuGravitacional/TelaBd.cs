using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
// Adiciona os namespaces necessários
using SimuGravitacional.Abstrato;

namespace SimuGravitacional
{
    public partial class TelaBd : Form
    {
        // Armazena a instância do DAO passada pelo Form1
        private GravitacaoDAO dao;

        // Propriedade pública para o Form1 ler o ID selecionado
        public int IdSimulacaoSelecionada { get; private set; } = -1;

        // Construtor modificado para receber a instância do DAO
        public TelaBd(GravitacaoDAO dao)
        {
            InitializeComponent();
            this.dao = dao;
            // Define o resultado padrão como Cancelar
            this.DialogResult = DialogResult.Cancel;
        }

        // Evento que dispara quando o formulário é carregado
        private void TelaBd_Load(object sender, EventArgs e)
        {
            CarregarListaSimulacoes();
        }

        // Método para buscar os dados do DAO e popular o DataGridView
        private void CarregarListaSimulacoes()
        {
            try
            {
                // Busca a lista de resumos do banco de dados
                var simulacoes = dao.ListarSimulacoes();

                // Define a lista como a fonte de dados do grid
                dgvSimulacoes.DataSource = simulacoes;

                // Configura as colunas para melhor visualização
                if (dgvSimulacoes.Columns["IdSimulacao"] != null)
                {
                    dgvSimulacoes.Columns["IdSimulacao"].HeaderText = "ID";
                    dgvSimulacoes.Columns["IdSimulacao"].Width = 50;
                }
                if (dgvSimulacoes.Columns["DataSimulacao"] != null)
                {
                    dgvSimulacoes.Columns["DataSimulacao"].HeaderText = "Data da Simulação";
                    dgvSimulacoes.Columns["DataSimulacao"].DefaultCellStyle.Format = "g"; // Formato de data e hora
                    dgvSimulacoes.Columns["DataSimulacao"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                }
                if (dgvSimulacoes.Columns["QtdCorposInicial"] != null)
                {
                    dgvSimulacoes.Columns["QtdCorposInicial"].HeaderText = "Corpos Iniciais";
                    dgvSimulacoes.Columns["QtdCorposInicial"].Width = 120;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erro ao carregar lista de simulações: " + ex.Message);
                this.Close(); // Fecha a tela se não conseguir carregar
            }
        }

        // Evento do botão "Carregar Selecionada"
        private void btnCarregarSelecionada_Click(object sender, EventArgs e)
        {
            // Verifica se alguma linha está selecionada
            if (dgvSimulacoes.SelectedRows.Count > 0)
            {
                // Pega o objeto ResumoSimulacao da linha selecionada
                var resumo = (ResumoSimulacao)dgvSimulacoes.SelectedRows[0].DataBoundItem;

                // Define a propriedade pública com o ID
                this.IdSimulacaoSelecionada = resumo.IdSimulacao;

                // Define o resultado do diálogo como OK
                this.DialogResult = DialogResult.OK;

                // Fecha o formulário
                this.Close();
            }
            else
            {
                MessageBox.Show("Por favor, selecione uma simulação na lista.");
            }
        }

        // Evento do botão "Cancelar"
        private void btnCancelar_Click(object sender, EventArgs e)
        {
            // O DialogResult já é "Cancel" por padrão, apenas fecha o form
            this.Close();
        }
    }
}