using CRUD.Models;
using Dapper;
using System.Data;
using System.Data.SqlClient;

namespace CRUD.Datos
{
	public class ContactoDatos
	{
		private readonly Conexion _conexion;

		//Inicializa la variable _conexion dentro del contructor de la clase
		public ContactoDatos()
		{
			_conexion = new Conexion();
		}

		public List<ContactModel> Listar()
		{
			using (var conexion = new SqlConnection(_conexion.GetCadenaSQL()))
			{
				conexion.Open();
				var result = conexion.Query<ContactModel>(
					"sp_Listar",
					commandType: CommandType.StoredProcedure
				).ToList();
				return result;
			}
		}

		public List<PhoneModel> ListarTelefonos(int ContactoId)
		{
			using (var conexion = new SqlConnection(_conexion.GetCadenaSQL()))
			{
				conexion.Open();
				var parameters = new { ContactoId = ContactoId };
				var result = conexion.Query<PhoneModel>(
					"sp_ListarTelefonos",
					parameters,
					commandType: CommandType.StoredProcedure
				).ToList();
				return result;
			}
		}

		public List<AddressModel> ListarDirecciones(int ContactoId)
		{
			using (var conexion = new SqlConnection(_conexion.GetCadenaSQL()))
			{
				conexion.Open();
				var parameters = new { ContactoId = ContactoId };
				var result = conexion.Query<AddressModel>(
					"sp_ListarDirecciones",
					parameters,
					commandType: CommandType.StoredProcedure
				).ToList();
				return result;
			}
		}

		public List<EmailModel> ListarCorreos(int ContactoId)
		{
			using (var conexion = new SqlConnection(_conexion.GetCadenaSQL()))
			{
				conexion.Open();
				var parameters = new { ContactoId = ContactoId };
				var result = conexion.Query<EmailModel>(
					"sp_ListarCorreos",
					parameters,
					commandType: CommandType.StoredProcedure
				).ToList();
				return result;
			}
		}


		public ContactModel Obtener(int IdContacto)
		{
			using (var conexion = new SqlConnection(_conexion.GetCadenaSQL()))
			{
				conexion.Open();
				var parametros = new { IdContacto = IdContacto };
				var query = "sp_Obtener";

				// Utilizamos QuerySingleOrDefault para obtener un solo resultado o null si no existe
				var oContacto = conexion.QuerySingleOrDefault<ContactModel>(
					query,
					parametros,
					commandType: CommandType.StoredProcedure
				);

				return oContacto;
			}
		}


        public int GuardarContacto(ContactModel contacto)
		{
			try
			{
				using (var conexion = new SqlConnection(_conexion.GetCadenaSQL()))
				{
					conexion.Open();

					var query = "INSERT INTO Contactos (nombre, apellido_paterno, apellido_materno, alias, fecha_nacimiento, fotografia) " +
								"VALUES (@nombre, @apellido_paterno, @apellido_materno, @alias, @fecha_nacimiento, @fotografia); " +
								"SELECT SCOPE_IDENTITY();";

					var id = conexion.ExecuteScalar<int>(query, new
					{
						contacto.nombre,
						contacto.apellido_paterno,
						contacto.apellido_materno,
						contacto.alias,
						contacto.fecha_nacimiento,
						fotografia = contacto.fotografia
					});

					return id;
				}
			}
			catch (Exception)
			{
				return 0;
			}
		}

		public void GuardarCorreo(int contactoId, string correo)
		{
			using (var conexion = new SqlConnection(_conexion.GetCadenaSQL()))
			{
				conexion.Open();
				var parameters = new { ContactoId = contactoId, Correo = correo };
				conexion.Execute("sp_GuardarCorreo", parameters, commandType: CommandType.StoredProcedure);
			}
		}

		public void GuardarTelefono(int contactoId, string telefono, string etiqueta)
		{
			using (var conexion = new SqlConnection(_conexion.GetCadenaSQL()))
			{
				conexion.Open();
				var parameters = new { ContactoId = contactoId, Telefono = telefono, Etiqueta = etiqueta };
				conexion.Execute("sp_GuardarTelefono", parameters, commandType: CommandType.StoredProcedure);
			}
		}

		public void GuardarDireccion(int contactoId, string direccion)
		{
			using (var conexion = new SqlConnection(_conexion.GetCadenaSQL()))
			{
				conexion.Open();
				var parameters = new { ContactoId = contactoId, Direccion = direccion };
				conexion.Execute("sp_GuardarDireccion", parameters, commandType: CommandType.StoredProcedure);
			}
		}

		public bool Editar(ContactModel oContacto)
		{
			bool rpta;

			try
			{
				using (var conexion = new SqlConnection(_conexion.GetCadenaSQL()))
				{
					conexion.Open();
					var parametros = new
					{
						IdContacto = oContacto.id,
						Nombre = oContacto.nombre,
						ApellidoPaterno = oContacto.apellido_paterno,
						ApellidoMaterno = oContacto.apellido_materno,
						FechaNacimiento = oContacto.fecha_nacimiento,
						Alias = oContacto.alias,
						Fotografia = oContacto.fotografia 
					};

					var query = "sp_Editar";
					conexion.Execute(query, parametros, commandType: CommandType.StoredProcedure);
				}
				rpta = true;
			}
			catch (Exception ex)
			{
				string msg = ex.Message;
				rpta = false;
			}
			return rpta;
		}


		public bool Eliminar(int idContacto)
		{
			bool rpta;

			try
			{
				using (var conexion = new SqlConnection(_conexion.GetCadenaSQL()))
				{
					conexion.Open();
					var parametros = new
					{
						IdContacto = idContacto
					};

					var query = "sp_Eliminar";
					conexion.Execute(query, parametros, commandType: CommandType.StoredProcedure);
				}
				rpta = true;
			}
			catch (Exception ex)
			{
				string msg = ex.Message;
				rpta = false;
			}
			return rpta;
		}


		public void EliminarCorreos(int idContacto)
		{
			using (var conexion = new SqlConnection(_conexion.GetCadenaSQL()))
			{
				conexion.Open();
				var parametros = new
				{
					IdContacto = idContacto
				};

				var query = "sp_EliminarCorreos";
				conexion.Execute(query, parametros, commandType: CommandType.StoredProcedure);
			}
		}

		public void EliminarTelefonos(int idContacto)
		{
			using (var conexion = new SqlConnection(_conexion.GetCadenaSQL()))
			{
				conexion.Open();
				var parametros = new
				{
					IdContacto = idContacto
				};

				var query = "sp_EliminarTelefonos";
				conexion.Execute(query, parametros, commandType: CommandType.StoredProcedure);
			}
		}


		public void EliminarDirecciones(int idContacto)
		{
			using (var conexion = new SqlConnection(_conexion.GetCadenaSQL()))
			{
				conexion.Open();
				var parametros = new
				{
					IdContacto = idContacto
				};

				var query = "sp_EliminarDirecciones";
				conexion.Execute(query, parametros, commandType: CommandType.StoredProcedure);
			}
		}
	}
}
