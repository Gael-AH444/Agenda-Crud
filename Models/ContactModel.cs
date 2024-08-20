using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CRUD.Models
{
	public class ContactModel
	{
		public int id { get; set; }

		[Required(ErrorMessage = "El campo Nombre es obligatorio")]
		public string nombre { get; set; }

		[Required(ErrorMessage = "El campo Apellido Paterno es obligatorio")]
		public string apellido_paterno { get; set; }

		[Required(ErrorMessage = "El campo Apellido Materno es obligatorio")]
		public string apellido_materno { get; set; }

		[Display(Name = "Fecha de Nacimiento")]
		[DataType(DataType.Date)]
		public System.DateTime fecha_nacimiento { get; set; } = DateTime.Today;

		[Required(ErrorMessage = "El campo Alias es obligatorio")]
		public string alias { get; set; }

		[Required(ErrorMessage = "El campo Fotografía es obligatorio")]
		public string fotografiaRuta { get; set; } // Nombre de la imagen seleccionada

		[NotMapped]
		public byte[] fotografia { get; set; } // Bytes de la imagen

		// Listas
		public List<EmailModel> CorreosElectronicos { get; set; }
		public List<AddressModel> Direcciones { get; set; }
		public List<PhoneModel> Telefonos { get; set; }





		//Constructor
		public ContactModel()
		{
			Telefonos = new List<PhoneModel>();
			Direcciones = new List<AddressModel>();
			CorreosElectronicos = new List<EmailModel>();
		}

	}
}
