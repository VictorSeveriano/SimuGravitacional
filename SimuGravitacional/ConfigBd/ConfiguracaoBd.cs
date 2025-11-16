using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

// define a conexão para o banco de dados MySQL usado pelo simulador, permite acesso interno ao projeto e não pode ser alterada

namespace SimuGravitacional.ConfigBd
{
    internal class ConfiguracaoBd
    {
        internal static readonly string ConnectionString = "Server=localhost;Port=3306;Database=simulador_gravitacional;Uid=root;Pwd=teste123";
    }
}