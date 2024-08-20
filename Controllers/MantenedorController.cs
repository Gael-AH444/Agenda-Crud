using CRUD.Datos;
using CRUD.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace CRUD.Controllers
{
	public class MantenedorController : Controller
	{
		ContactoDatos _contactoDatos = new ContactoDatos();

		public IActionResult Listar()
		{
			//La vista mostrara una lista de contactos
			var contactos = _contactoDatos.Listar();
			var todosContactos = new List<ContactModel>();

			//Recorrer el id de los registros
			foreach (var contacto in contactos)
			{
				var correos = _contactoDatos.ListarCorreos(contacto.id);
				var telefonos = _contactoDatos.ListarTelefonos(contacto.id);
				var direcciones = _contactoDatos.ListarDirecciones(contacto.id);

				todosContactos.Add(new ContactModel
				{
					id = contacto.id,
					nombre = contacto.nombre,
					apellido_paterno = contacto.apellido_paterno,
					apellido_materno = contacto.apellido_materno,
					alias = contacto.alias,
					fecha_nacimiento = contacto.fecha_nacimiento,
					CorreosElectronicos = correos,
					Telefonos = telefonos,
					Direcciones = direcciones
				});
			}

			return View(todosContactos);
		}


		public IActionResult Guardar()
		{
			//Solo devuelve la vista HTML 
			// Llenar el ViewBag con las imágenes disponibles
			var images = LlenarImagenesEnViewBagList();

			var model = new ContactModel
			{
				fotografiaRuta = images.FirstOrDefault(), // Obtener la primera imagen por defecto
				CorreosElectronicos = new List<EmailModel>(),
				Telefonos = new List<PhoneModel>(),
				Direcciones = new List<AddressModel>()
			};

			return View(model);
		}

		[HttpPost]
		public IActionResult Guardar(ContactModel oContacto)
		{
			//Validar que los campos obligatorios tengan informacion
			if (oContacto.nombre == null || oContacto.apellido_materno == null || oContacto.apellido_paterno == null ||
				oContacto.fotografiaRuta == null)
			{

				LlenarImagenesEnViewBag();
				return View(oContacto);
			}

			//Si los campos obligatorios tiene informacion, continua aqui....
			try
			{
				if (!string.IsNullOrEmpty(oContacto.fotografiaRuta))
				{
					var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images", oContacto.fotografiaRuta);
					oContacto.fotografia = System.IO.File.ReadAllBytes(filePath);
				}
				else
				{
					ModelState.AddModelError("fotografia", "Debe seleccionar una fotografía.");
					LlenarImagenesEnViewBag();
					return View(oContacto);
				}

				var contactoId = _contactoDatos.GuardarContacto(oContacto);
				if (contactoId > 0)
				{
					foreach (EmailModel correo in oContacto.CorreosElectronicos)
					{
						_contactoDatos.GuardarCorreo(contactoId, correo.correo);
					}

					foreach (PhoneModel tel in oContacto.Telefonos)
					{
						_contactoDatos.GuardarTelefono(contactoId, tel.telefono, tel.etiqueta);
					}

					foreach (AddressModel direccion in oContacto.Direcciones)
					{
						_contactoDatos.GuardarDireccion(contactoId, direccion.direccion);
					}

					return RedirectToAction("Listar");
				}
				else
				{
					ModelState.AddModelError("", "Ocurrió un error al intentar guardar el contacto.");
					LlenarImagenesEnViewBag();
					return View(oContacto);
				}
			}
			catch (Exception)
			{
				ModelState.AddModelError("", "Ocurrió un error al intentar guardar el contacto.");
				LlenarImagenesEnViewBag();
				return View(oContacto);
			}
		}

		private void LlenarImagenesEnViewBag()
		{
			//Obtner las imagenes del servidor
			var imagesPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images");
			var images = Directory.GetFiles(imagesPath)
								  .Select(Path.GetFileName)
								  .ToList();

			ViewBag.Images = new SelectList(images, images.First()); // Seleccionar la primera imagen por defecto
		}

		private List<string> LlenarImagenesEnViewBagList()
		{
			var imagesPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images");
			var images = Directory.GetFiles(imagesPath)
								  .Select(Path.GetFileName)
								  .ToList();

			ViewBag.Images = new SelectList(images, images.FirstOrDefault()); // Seleccionar la primera imagen por defecto

			return images;
		}


		public IActionResult Editar(int idContacto)
		{
			//Solo devuelve la vista HTML 

			// Obtener el contacto
			var oContacto = _contactoDatos.Obtener(idContacto);
			if (oContacto == null)
			{
				return NotFound();
			}

			// Obtener los correos electrónicos, telefonos y direcciones
			oContacto.CorreosElectronicos = _contactoDatos.ListarCorreos(idContacto);
			oContacto.Telefonos = _contactoDatos.ListarTelefonos(idContacto);
			oContacto.Direcciones = _contactoDatos.ListarDirecciones(idContacto);

			if (!string.IsNullOrEmpty(oContacto.fotografiaRuta))
			{
				var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images", oContacto.fotografiaRuta);
				oContacto.fotografia = System.IO.File.ReadAllBytes(filePath);
			}

			// Obtener la lista de imágenes del servidor
			var images = Directory.GetFiles(Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images"))
								  .Select(Path.GetFileName)
								  .ToList();

			ViewBag.Images = images;

			return View(oContacto);
		}

		[HttpPost]
		public IActionResult Editar(ContactModel oContacto)
		{
			//Validar que los campos obligatorios tengan informacion
			if (oContacto.nombre == null || oContacto.apellido_materno == null || oContacto.apellido_paterno == null ||
				oContacto.fotografiaRuta == null)
			{
				var oContact = _contactoDatos.Obtener(oContacto.id);
				ViewBag.Images = new List<string>();
				// Obtener la lista de imágenes del servidor
				var images = Directory.GetFiles(Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images"))
									  .Select(Path.GetFileName)
									  .ToList();

				ViewBag.Images = images;
				// Obtener los correos electrónicos, telefonos y direcciones
				oContact.CorreosElectronicos = _contactoDatos.ListarCorreos(oContacto.id);
				oContact.Telefonos = _contactoDatos.ListarTelefonos(oContacto.id);
				oContact.Direcciones = _contactoDatos.ListarDirecciones(oContacto.id);

				return View(oContact);
			}

			//Si todos los campos obligatorios estan llenos continia aqui....
			//Ruta de la imagen seleccionada
			if (!string.IsNullOrEmpty(oContacto.fotografiaRuta))
			{
				var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images", oContacto.fotografiaRuta);
				oContacto.fotografia = System.IO.File.ReadAllBytes(filePath);
			}
			else
			{
				ModelState.AddModelError("fotografia", "Debe seleccionar una fotografía.");
				return View(oContacto);
			}


			//Eliminar registros de Telefonos, Direcciones y Correos
			_contactoDatos.EliminarCorreos(oContacto.id);
			_contactoDatos.EliminarDirecciones(oContacto.id);
			_contactoDatos.EliminarTelefonos(oContacto.id);

			//Agregar nuevamente todos los registros de las tablas 
			foreach (var correo in oContacto.CorreosElectronicos)
			{
				_contactoDatos.GuardarCorreo(oContacto.id, correo.correo);
			}

			foreach (PhoneModel tel in oContacto.Telefonos)
			{
				_contactoDatos.GuardarTelefono(oContacto.id, tel.telefono, tel.etiqueta);
			}

			foreach (AddressModel direccion in oContacto.Direcciones)
			{
				_contactoDatos.GuardarDireccion(oContacto.id, direccion.direccion);
			}


			//El metodo recibe el objeto para guardarlo en la BD
			var respuesta = _contactoDatos.Editar(oContacto);



			if (respuesta)
				return RedirectToAction("Listar");
			else
				return View();
		}

		public IActionResult Eliminar(int idContacto)
		{
			//Solo devuelve la vista HTML 
			var oContacto = _contactoDatos.Obtener(idContacto);
			return View(oContacto);
		}

		[HttpPost]
		public IActionResult Eliminar(ContactModel oContacto)
		{
			//El metodo recibe el objeto para guardarlo en la BD
			var respuesta = _contactoDatos.Eliminar(oContacto.id);

			if (respuesta)
				return RedirectToAction("Listar");
			else
				return View();
		}

		public IActionResult VerInformacion(int idContacto)
		{
			// Obtener el contacto
			var oContacto = _contactoDatos.Obtener(idContacto);

			if (oContacto == null)
			{
				return NotFound();
			}

			// Obtener los correos electrónicos, teléfonos y direcciones
			oContacto.CorreosElectronicos = _contactoDatos.ListarCorreos(idContacto);
			oContacto.Telefonos = _contactoDatos.ListarTelefonos(idContacto);
			oContacto.Direcciones = _contactoDatos.ListarDirecciones(idContacto);

			// Obtener la ruta de la imagen seleccionada
			if (!string.IsNullOrEmpty(oContacto.fotografiaRuta))
			{
				var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images", oContacto.fotografiaRuta);
				oContacto.fotografia = System.IO.File.ReadAllBytes(filePath);
			}

			return View(oContacto);
		}
	}
}
