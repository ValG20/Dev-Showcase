using System.Text;
using EditorialUCR.Models.Api;
using EditorialUCR.Models.Central;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Hosting; 
using System.IO; 
using Microsoft.AspNetCore.Http;
using EditorialUCR.Models;
using AppWebEditorial.Models;

namespace EditorialUCR.Controllers
{
    public class Servicio_impresionController : Controller
    {
        private AppWebEditorial.Models.ApiEditorial client = null;
        private HttpClient api = null;
        private readonly IWebHostEnvironment _hostEnvironment;

        public Servicio_impresionController(IWebHostEnvironment hostEnvironment)
        {
            _hostEnvironment = hostEnvironment;
            client = new AppWebEditorial.Models.ApiEditorial();
            api = client.IniciarApi();
        }

        public async Task<IActionResult> ListaServicios_Impresion()
        {
            List<Servicio_impresion> servicio_Impresions = new List<Servicio_impresion>();

            try
            {
                HttpResponseMessage response = await api.GetAsync("Servicio_impresion/Lista");

                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadAsStringAsync();
                    servicio_Impresions = JsonConvert.DeserializeObject<List<Servicio_impresion>>(result);
                }

                if (servicio_Impresions == null || servicio_Impresions.Count == 0)
                {
                    ViewBag.Mensaje = "No hay impresiones registrados actualmente.";
                    servicio_Impresions = new List<Servicio_impresion>();
                }
            }
            catch (Exception ex)
            {
                ViewBag.Mensaje = $"Error al obtener los servicios: {ex.InnerException?.Message ?? ex.Message}";
            }

            return View("ListaServicios_Impresion", servicio_Impresions);
        }
        public IActionResult Crear(int Id)
        {
            ViewBag.Id = Id;
            return View();
        }



        [HttpPost]
        public async Task<IActionResult> Create(
    Servicio_impresion servicio,
    IFormFile autorizacion_firma,
    IFormFile sello_archivo,
    IFormFile archivo_extra,
    int Id
)
        {
            
            var respuestaProducto = await api.GetAsync($"Catalogo/BuscarEnCatalogo?id={Id}");
            if (!respuestaProducto.IsSuccessStatusCode)
            {
                ModelState.AddModelError("", "Error obteniendo información del producto.");
                return View("Crear", servicio);
            }

            var jsonProducto = await respuestaProducto.Content.ReadAsStringAsync();
            var producto = JsonConvert.DeserializeObject<Producto>(jsonProducto);

            

            // ===============================
            // 1. Guardar archivo de autorización (firma)
            // ===============================
            if (autorizacion_firma != null && autorizacion_firma.Length > 0)
            {
                string uploadsFolder = Path.Combine(_hostEnvironment.WebRootPath, "uploads", "autorizaciones");
                if (!Directory.Exists(uploadsFolder))
                    Directory.CreateDirectory(uploadsFolder);

                string uniqueFileName = Guid.NewGuid().ToString() + "_" + Path.GetFileName(autorizacion_firma.FileName);
                string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                using (var fileStream = new FileStream(filePath, FileMode.Create))
                    await autorizacion_firma.CopyToAsync(fileStream);

                servicio.autorizacion_firma = uniqueFileName;
            }
            else
            {
                servicio.autorizacion_firma = "";
            }

            if (sello_archivo != null && sello_archivo.Length > 0)
            {
                string uploadsFolder = Path.Combine(_hostEnvironment.WebRootPath, "uploads", "sellos");
                if (!Directory.Exists(uploadsFolder))
                    Directory.CreateDirectory(uploadsFolder);

                string uniqueFileName = Guid.NewGuid().ToString() + "_" + Path.GetFileName(sello_archivo.FileName);
                string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                using (var fileStream = new FileStream(filePath, FileMode.Create))
                    await sello_archivo.CopyToAsync(fileStream);

                servicio.sello_unidad = uniqueFileName;
            }
            else
            {
                servicio.sello_unidad = "";
            }

            if (archivo_extra != null && archivo_extra.Length > 0)
            {
                string uploadsFolder = Path.Combine(_hostEnvironment.WebRootPath, "uploads", "extra");
                if (!Directory.Exists(uploadsFolder))
                    Directory.CreateDirectory(uploadsFolder);

                string uniqueFileName = Guid.NewGuid().ToString() + "_" + Path.GetFileName(archivo_extra.FileName);
                string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                using (var fileStream = new FileStream(filePath, FileMode.Create))
                    await archivo_extra.CopyToAsync(fileStream);
            }

            var propuestaNueva = new Propuesta
            {
                id_propuesta = 1,
                id_usuario = 1,
                id_producto = Id,   // ← ANTES 9999
                fecha_envio = DateTime.Now,
                estado = "En Revisión",
                observaciones_usuario = "GENERADO_AUTOMATICO"
            };

            var propuestaJson = new StringContent(
                JsonConvert.SerializeObject(propuestaNueva),
                Encoding.UTF8,
                "application/json"
            );

            var respuestaPropuesta = await api.PostAsync("api/Propuesta", propuestaJson);

            if (!respuestaPropuesta.IsSuccessStatusCode)
            {
                ModelState.AddModelError("", "Error creando Propuesta");
                return View("Crear", servicio);
            }

            int idPropuestaCreada = int.Parse(await respuestaPropuesta.Content.ReadAsStringAsync());

           
            var pedidoNuevo = new Pedido
            {
                tipo_pedido = producto.Tipo,     
                id_propuesta = idPropuestaCreada,
                orden_servicio = "AUTO",
                titulo_trabajo = producto.Tipo,
                dependencia = "N/A",
                funcionario = "N/A",
                estado = "En Revisión",
                correo_funcionario = "ejemplo@ucr.ac.cr",
                telefono_funcionario = "0000-0000",
                fax = "",
                consentimiento_cliente = "",
                sello = "",
                responsable_vb = "",
                observaciones_generales = "",
                cant_cd = 0,
                campos_servicio_extra = "",
                total = 0
            };

            var pedidoJson = new StringContent(
                JsonConvert.SerializeObject(pedidoNuevo),
                Encoding.UTF8,
                "application/json"
            );

            var respuestaPedido = await api.PostAsync("Pedidos/Guardar", pedidoJson);

            if (!respuestaPedido.IsSuccessStatusCode)
            {
                ModelState.AddModelError("", "Error creando Pedido");
                return View("Crear", servicio);
            }

            int idPedidoCreado = int.Parse(await respuestaPedido.Content.ReadAsStringAsync());

            // ===============================
            // Mantengo TODO tu código igual
            // ===============================

            ModelState.Remove(nameof(servicio.id_servicio));
            ModelState.Remove(nameof(servicio.id_pedido));
            ModelState.Remove(nameof(servicio.fecha_retiro));
            ModelState.Remove(nameof(servicio.fecha_hora_recepcion));

            ModelState.Remove(nameof(servicio.sello_recibido_por_siedin));
            ModelState.Remove(nameof(servicio.retiro_firma));
            ModelState.Remove(nameof(servicio.recepcionista));

            servicio.id_servicio = 0;
            servicio.id_pedido = idPedidoCreado;
            servicio.fecha_solicitud = DateTime.Now;
            servicio.fecha_hora_recepcion = DateTime.Now;

            servicio.sello_recibido_por_siedin = "PENDIENTE";
            servicio.retiro_firma = "PENDIENTE";
            servicio.fecha_retiro = DateTime.Now;
            servicio.recepcionista = "SISTEMA";

            var servicioJson = new StringContent(
                JsonConvert.SerializeObject(servicio),
                Encoding.UTF8,
                "application/json"
            );

            var respuestaServicio = await api.PostAsync("Servicio_impresion/Guardar", servicioJson);

            if (respuestaServicio.IsSuccessStatusCode)
            {
                TempData["SuccessMessage"] = "Servicio creado correctamente.";
                return RedirectToAction("Crear");
            }
            else
            {
                var error = await respuestaServicio.Content.ReadAsStringAsync();
                ModelState.AddModelError(string.Empty, $"Error al guardar servicio: {error}");
                return View("Crear", servicio);
            }
        }




    }
}
