using System.Text;
using EditorialUCR.Models.Api;
using EditorialUCR.Models.Central;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using EditorialUCR.Models;


namespace EditorialUCR.Controllers
{
    public class PedidosController : Controller
    {
        private ApiEditorial client = null;
        private HttpClient api = null;

        public PedidosController()
        {
            client = new ApiEditorial();
            api = client.IniciarApi();
        }

        public async Task<IActionResult> Pedidos()
        {
            List<Pedido> pedidos = new List<Pedido>();

            try
            {
                HttpResponseMessage response = await api.GetAsync("Pedidos/Lista");

                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadAsStringAsync();
                    pedidos = JsonConvert.DeserializeObject<List<Pedido>>(result);
                }

                if (pedidos == null || pedidos.Count == 0)
                {
                    ViewBag.Mensaje = "No hay pedidos registrados actualmente.";
                    pedidos = new List<Pedido>();
                }
            }
            catch (Exception ex)
            {
                ViewBag.Mensaje = $"Error al obtener los pedidos: {ex.InnerException?.Message ?? ex.Message}";
            }

           return View("Pedidos", pedidos);
        }

        public async Task<IActionResult> PedidosInstitucional()
        {
            List<Pedido> pedidos = new List<Pedido>();

            try
            {
                HttpResponseMessage response = await api.GetAsync("Pedidos/Lista");

                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadAsStringAsync();
                    pedidos = JsonConvert.DeserializeObject<List<Pedido>>(result);
                }

                if (pedidos == null || pedidos.Count == 0)
                {
                    ViewBag.Mensaje = "No hay pedidos registrados actualmente.";
                    pedidos = new List<Pedido>();
                }
            }
            catch (Exception ex)
            {
                ViewBag.Mensaje = $"Error al obtener los pedidos: {ex.InnerException?.Message ?? ex.Message}";
            }

            return View("PedidosInstitucional", pedidos);
        }

        public async Task<IActionResult> PedidosPendientes()
        {
            List<Pedido> pedidos = new List<Pedido>();

            try
            {
                HttpResponseMessage response = await api.GetAsync("Pedidos/Lista");

                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadAsStringAsync();
                    pedidos = JsonConvert.DeserializeObject<List<Pedido>>(result);
                }

                if (pedidos == null || pedidos.Count == 0)
                {
                    ViewBag.Mensaje = "No hay pedidos registrados actualmente.";
                    pedidos = new List<Pedido>();
                }
            }
            catch (Exception ex)
            {
                ViewBag.Mensaje = $"Error al obtener los pedidos: {ex.InnerException?.Message ?? ex.Message}";
            }

           return View("PedidosPendientes", pedidos);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(Pedido pedido)
        {
            if (!ModelState.IsValid)
            {
                return View(pedido);
            }

            pedido.id_pedido = 0; 

            var jsonContent = new StringContent(JsonConvert.SerializeObject(pedido), Encoding.UTF8, "application/json");
            HttpResponseMessage response = await api.PostAsync("Pedidos/Guardar", jsonContent);

            if (response.IsSuccessStatusCode)
            {
                TempData["SuccessMessage"] = await response.Content.ReadAsStringAsync();
                return RedirectToAction("Index", "Home");
            }
            else
            {
                var error = await response.Content.ReadAsStringAsync();
                ModelState.AddModelError(string.Empty, $"Error al guardar: {error}");
                return View(pedido);
            }
        }



        public async Task<IActionResult> Detalles(int id)
        {
            Pedido cliente = null;
            HttpResponseMessage response = await api.GetAsync($"Pedidos/Detalles?id={id}");

            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                cliente = JsonConvert.DeserializeObject<Pedido>(json);
            }
            else
            {
                ViewBag.Mensaje = $"No se encontró ningún pedido con el ID {id}";
                return View("Detalles", null);
            }

            return View("Detalles", cliente);
        }

        [HttpPost]
        public async Task<IActionResult> Aprobar(int id)
        {
            var url = $"Pedidos/Aprobar?id={id}";
            var response = await api.PostAsync(url, null);

            if (!response.IsSuccessStatusCode)
            {
                ViewBag.Error = "Error al aprobar el pedido";
                return RedirectToAction("PedidosPendientes");
            }

            return RedirectToAction("Crear", "Actas_Sesion");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Rechazo(int id)
        {
            try
            {
                // Llama al endpoint REAL de la API
                var response = await api.PostAsync($"Pedidos/Rechazo?id={id}", null);

                if (!response.IsSuccessStatusCode)
                {
                    var error = await response.Content.ReadAsStringAsync();
                    TempData["Error"] = $"Error al rechazar el pedido {id}: {error}";
                    return RedirectToAction("PedidosPendientes");
                }

                TempData["Success"] = $"Pedido {id} rechazado correctamente";
                return RedirectToAction("Crear", "Actas_Sesion");
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
                return RedirectToAction("PedidosPendientes");
            }
        }


        //Metodos para imprimir PDF     
        [HttpGet]
        public async Task<IActionResult> Imprimir(int id)
        {
            var respuesta = await api.GetAsync($"Pedidos/Detalles?id={id}");


            if (!respuesta.IsSuccessStatusCode)
                return Content("Error obteniendo el pedido desde la API");

            var pedido = await respuesta.Content.ReadFromJsonAsync<Pedido>();

            if (pedido == null)
                return Content("No se encontró el pedido");

            // 2. Construir HTML con estilo UCR
            string html = $@"
<html>
<head>
    <meta charset='UTF-8'>
    <style>
        body {{
            font-family: Arial, sans-serif;
            background-color: #ffffff;
            padding: 0;
            margin: 0;
            font-size: 14px;
        }}

        /* Encabezado UCR */
        .header {{
            background-color: #005da4; /* Azul UCR */
            padding: 20px;
            color: white;
            text-align: center;
            font-size: 22px;
            font-weight: bold;
        }}

        /* Contenedor de tarjeta */
        .card {{
            background-color: #ffffff;
            border: 2px solid #e5e5e5; /* Gris suave */
            border-radius: 8px;
            padding: 25px;
            margin: 25px;
        }}

        h2 {{
            color: #005da4;
            margin-bottom: 15px;
        }}

        .campo {{
            margin-bottom: 12px;
            font-size: 15px;
        }}

        .label {{
            font-weight: bold;
            color: #005da4;
        }}

        /* Línea separadora */
        hr {{
            border: none;
            border-top: 2px solid #e5e5e5;
            margin: 20px 0;
        }}

        /* Estado estilo ficha */
        .estado {{
            display: inline-block;
            padding: 6px 12px;
            color: white;
            border-radius: 6px;
            font-weight: bold;
        }}

        .estado.Aprobado {{ background-color: #28a745; }}
        .estado.Rechazado {{ background-color: #dc3545; }}
        .estado.Borrador {{ background-color: #ffc107; color: black; }}

        /* Total */
        .total {{
            margin-top: 25px;
            font-size: 18px;
            color: #005da4;
            font-weight: bold;
        }}

    </style>
</head>

<body>

    <div class='header'>
        Sistema Editorial UCR – Pedido #{pedido.id_pedido}
    </div>

    <div class='card'>
        <h2>Detalles del Pedido</h2>

        <div class='campo'><span class='label'>Tipo:</span> {pedido.tipo_pedido}</div>
        <div class='campo'><span class='label'>Título:</span> {pedido.titulo_trabajo}</div>
        <div class='campo'><span class='label'>Dependencia:</span> {pedido.dependencia}</div>

        <div class='campo'><span class='label'>Funcionario:</span> {pedido.funcionario}</div>
        <div class='campo'><span class='label'>Correo:</span> {pedido.correo_funcionario}</div>
        <div class='campo'><span class='label'>Teléfono:</span> {pedido.telefono_funcionario}</div>

        <div class='campo'>
            <span class='label'>Estado:</span> 
            <span class='estado {pedido.estado}'>{pedido.estado}</span>
        </div>

        <div class='campo'><span class='label'>Fecha creación:</span> {pedido.fecha_creacion:dd/MM/yyyy}</div>
        <div class='campo'><span class='label'>Última actualización:</span> {pedido.fecha_actualizacion:dd/MM/yyyy}</div>

        <hr />

        <div class='campo'>
            <span class='label'>Observaciones:</span><br>
            {pedido.observaciones_generales}
        </div>

        <div class='total'>
            Total: ₡{pedido.total:N2}
        </div>

    </div>
</body>
</html>";

            // 3. Convertir HTML a PDF
            var renderer = new ChromePdfRenderer();
            var pdf = renderer.RenderHtmlAsPdf(html);

            // 4. Devolver PDF al navegador
            return File(
                pdf.BinaryData,
                "application/pdf",
                $"Pedido_{pedido.id_pedido}.pdf"
            );
        }




   



    }
}
