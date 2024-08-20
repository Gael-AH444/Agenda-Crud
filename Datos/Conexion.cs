using System.Data.SqlClient;

namespace CRUD.Datos
{
    public class Conexion
    {
        //Propiedad que almacena la cadena de conexion
        private string cadenaSQL = string.Empty;

        //Constructor
        public Conexion()
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .Build();

            cadenaSQL = builder.GetSection("ConnectionStrings:CadenaSQL").Value;
        }

        //Metodo para obtener la conexion
        public string GetCadenaSQL()
        {
            return cadenaSQL;
        }
    }
}
