using EditorialUCR.Models.SolicitudPublicacion;
using EditorialUCR.Models.Central;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Net.Http;
using System.Text;

namespace EditorialUCR.Controllers
{
    public class SolicitudPublicacionController : Controller
    {
        private readonly HttpClient _httpClient;

        public SolicitudPublicacionController(IConfiguration config)
        {
            _httpClient = new HttpClient();
            _httpClient.BaseAddress = new Uri("https://localhost:7132");
        }

        // ============================
        // GET: Formulario
        // ============================
        // GET /SolicitudPublicacion
        public IActionResult Index()
        {
            // Apunta explícitamente a la vista que ya tienes
            return View("~/Views/Propuesta/SolicitudPublicacion.cshtml");
        }

        // ============================
        // POST: Enviar a la API
        // ============================
        [HttpPost]
        public async Task<IActionResult> EnviarSolicitud([FromBody] SolicitudPublicacionViewModel modelo)
        {
            if (!ModelState.IsValid)
                return BadRequest("Datos incompletos.");

            // Seguridad mínima
            modelo.Titulo = modelo.Titulo?.Trim();

            // ============================
            // 1) PROPOSICIÓN EDITORIAL
            // ============================
            var propuestaDto = new Propuesta_Editorial
            {
                // id_propuesta lo genera la API / BD
                fecha_creacion = DateTime.Now,
                titulo_obra = modelo.Titulo,
                subtitulo = modelo.Subtitulo,
                serie_coleccion = modelo.Serie,
                publico_meta = modelo.PublicoMeta,
                palabras_claves = modelo.PalabrasClave   // viene como "pal1,pal2,pal3..."
            };

            var jsonPropuesta = JsonConvert.SerializeObject(propuestaDto);
            var contentPropuesta = new StringContent(jsonPropuesta, Encoding.UTF8, "application/json");

            // POST api/Propuesta_Editorial
            var respPropuesta = await _httpClient.PostAsync("Propuesta_Editorial/GuardarDevolver", contentPropuesta);

            if (!respPropuesta.IsSuccessStatusCode)
            {
                var error = await respPropuesta.Content.ReadAsStringAsync();
                return BadRequest("Error al guardar Propuesta_Editorial en la API: " + error);
            }

            var propuestaCreadaJson = await respPropuesta.Content.ReadAsStringAsync();
            var propuestaCreada = JsonConvert.DeserializeObject<Propuesta_Editorial>(propuestaCreadaJson);

            if (propuestaCreada == null)
                return BadRequest("No se pudo leer la propuesta creada desde la API.");

            // ============================
            // 2) AUTOR
            // ============================
            var autorDto = new Autor
            {
                // id_autor lo genera la API / BD
                nombre_apellidos = $"{modelo.Nombre} {modelo.Apellidos}".Trim(),
                nacionalidad = modelo.Nacionalidad,
                tipo_cedula = modelo.TipoCedula,
                documento_identidad = modelo.NumeroCedula,
                estado_civil = modelo.EstadoCivil,
                profesion = modelo.Profesion,
                fecha_actualizacion = DateTime.Now,
                correo_electronico = modelo.Correo,
                direccion = modelo.Direccion,
                telefono_habitacion = modelo.TelHabitacion,
                telefono_celular = modelo.TelCelular,
                telefono_oficina = modelo.TelOficina
            };

            var jsonAutor = JsonConvert.SerializeObject(autorDto);
            var contentAutor = new StringContent(jsonAutor, Encoding.UTF8, "application/json");

            // POST api/Autores
            var respAutor = await _httpClient.PostAsync("api/Autor/Devolver", contentAutor);


            if (!respAutor.IsSuccessStatusCode)
            {
                var error = await respAutor.Content.ReadAsStringAsync();
                return BadRequest("Error al guardar Autor en la API: " + error);
            }

            var autorCreadoJson = await respAutor.Content.ReadAsStringAsync();
            var autorCreado = JsonConvert.DeserializeObject<Autor>(autorCreadoJson);

            if (autorCreado == null)
                return BadRequest("No se pudo leer el autor creado desde la API.");

            // ============================
            // 3) AUTOR_PROPUESTA (funciones)
            // ============================
            var funciones = new List<string>();

            if (modelo.Funciones != null)
                funciones.AddRange(modelo.Funciones);

            if (!string.IsNullOrWhiteSpace(modelo.OtraFuncion))
                funciones.Add(modelo.OtraFuncion);

            foreach (var funcion in funciones)
            {
                var autorPropuestaDto = new Autor_propuesta
                {
                    // Este valor lo recalcula la API, puede ir en 0
                    id_autor_propuesta = 0,

                    // CLAVES: estos sí tienen que ser válidos (>0)
                    id_propuesta = propuestaCreada.id_propuesta, // de la respuesta de Propuesta_Editorial
                    id_autor = autorCreado.id_autor,         // de la respuesta de Autor
                    funcion_en_obra = funcion                       // "Autor", "Coautor", etc.
                };

                var jsonAP = JsonConvert.SerializeObject(autorPropuestaDto);
                var contentAP = new StringContent(jsonAP, Encoding.UTF8, "application/json");

                var respAP = await _httpClient.PostAsync("api/Autor_Propuesta/devolverObjeto", contentAP);

                if (!respAP.IsSuccessStatusCode)
                {
                    var error = await respAP.Content.ReadAsStringAsync();
                    return BadRequest("Error al guardar funciones en Autor_Propuesta: " + error);
                }
            }

            // ============================
            // OK: devolvemos resumen
            // ============================
            return Ok(new
            {
                mensaje = "Solicitud registrada correctamente",
                idPropuesta = propuestaCreada.id_propuesta,
                idAutor = autorCreado.id_autor,
                funcionesCreadas = funciones.Count
            });
        }

    }
}