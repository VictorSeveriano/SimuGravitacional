// Ponto de entrada da aplica�ao Windows Forms.
namespace SimuGravitacional
{
    internal static class Program
    {
        [STAThread]
        static void Main()
        {
            // Inicializa configura�oes padrao da aplica�ao
            ApplicationConfiguration.Initialize();

            // Inicia o formul�rio principal da aplica��o
            Application.Run(new Form1());
        }
    }
}
