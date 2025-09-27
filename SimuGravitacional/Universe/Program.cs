// Ponto de entrada da aplicaçao Windows Forms.
namespace SimuGravitacional
{
    internal static class Program
    {
        [STAThread]
        static void Main()
        {
            // Inicializa configuraçoes padrao da aplicaçao
            ApplicationConfiguration.Initialize();

            // Inicia o formulário principal da aplicação
            Application.Run(new Form1());
        }
    }
}
