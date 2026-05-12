using EditorialUCR.Models.Api;
using EditorialUCR.Models.Central;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using EditorialUCR.Models;
using AppWebEditorial.Models;

namespace EditorialUCR.Controllers
{
    public class MaterialesController : Controller
    {
        private AppWebEditorial.Models.ApiEditorial _client = null;
        private HttpClient _api = null;

        public MaterialesController()
        {
            _client = new AppWebEditorial.Models.ApiEditorial();
            _api = _client.IniciarApi();
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            List<Material> listado = new List<Material>();

            HttpResponseMessage response = await _api.GetAsync("api/Materiales/List");

            if (response.IsSuccessStatusCode)
            {
                var result = response.Content.ReadAsStringAsync().Result;

                listado = JsonConvert.DeserializeObject<List<Material>>(result);
            }
            return View("~/Views/Materiales/Index.cshtml",listado);
        }//Llenamos con la lista de productos

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create([Bind] Material pProducto)
        {

            

            if (pProducto.fecha_registro > DateTime.Now)
            {
                ModelState.AddModelError("fecha_registro", "La fecha no puede ser futura");
            }

            if (!ModelState.IsValid)
            {
                return View(pProducto);
            }

            var agregar = await _api.PostAsJsonAsync("Api/Materiales/Save", pProducto);

            //Validar respuesta de la API
            if (agregar.IsSuccessStatusCode)
            {
                return RedirectToAction("Index");
            }
            else
            {
                TempData["Error"] = "Error, no se logró almacenar el material.";
                return View(pProducto);
            }
        }//Fin de crear

       
        //Método encargado de eliminar un producto
        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            //Se utiliza el método publico en la Web API se envia el ID del producto a eliminar
            HttpResponseMessage delete = await _api.DeleteAsync($"Api/Materiales/Delete?id_producto={id}");

            if (delete.IsSuccessStatusCode)
            { 
                return RedirectToAction("Index");
            }
            else
            {  //Se muestra un error en caso que no se logra eliminar
                return NotFound();
            }
        }//fin de eliminar

        //Métodos encargados de realizar el proceso de editar
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            //Variable para almacenar los datos del libro a eliminar
            Material temp = new Material();

            //Se busca el Producto a modificar por medio del ID
            HttpResponseMessage buscar = await _api.GetAsync($"Api/Materiales/SearchById?id_producto={id}");

            if (buscar.IsSuccessStatusCode)
            {
                //Se realiza lectura de los datos en formato JSON
                var result = buscar.Content.ReadAsStringAsync().Result;
                //Se convierte los datos a un Object producto
                temp = JsonConvert.DeserializeObject<Material>(result);
            }
            return View(temp);
        }//fin de editar

        [HttpPost]
        public async Task<IActionResult> Edit([Bind] Material temp)
        {  //Se envian los datos del producto a modificar

            HttpResponseMessage modificar = await _api.PutAsJsonAsync<Material>("Api/Materiales/Update", temp);
            //Se valida si la ejecución es correcta
            if (modificar.IsSuccessStatusCode)
            {  //se ubica al usuario dentro del listado Productos
                return RedirectToAction("Index");
            }
            else
            { //En caso de error, se ubica al usuario dentro del formulario editar para que revise
                //los datos
                return View(temp);
            }
        }//fin de editar

        //Para ver los productos individualmente
        //Método encargado de mostrar el detalle de datos para un libro
        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            Material temp = new Material();

            //Se ejecuta el método buscar public en la web api
            HttpResponseMessage buscar = await _api.GetAsync($"Api/Materiales/SearchById?id_producto={id}");

            if (buscar.IsSuccessStatusCode)
            {//se realiza la lectura de datos en formato JSON
                var result = buscar.Content.ReadAsStringAsync().Result;
                //Se convierte los datos en un objeto
                temp = JsonConvert.DeserializeObject<Material>(result);
            }

            return View(temp);
        }

        public async Task<JsonResult> SearchByName(string nombre)
        {
            // Forzar que nunca sea null
            nombre = nombre ?? "";

            HttpResponseMessage resp = await _api.GetAsync($"Api/Materiales/SearchByName?nombre={nombre}");

            if (!resp.IsSuccessStatusCode)
                return Json(new List<Material>());

            string json = await resp.Content.ReadAsStringAsync();
            var lista = JsonConvert.DeserializeObject<List<Material>>(json);

            return Json(lista);
        }


        [HttpGet]
        public async Task<JsonResult> FilterByCategory(string categoria)
        {
            HttpResponseMessage response =
                await _api.GetAsync($"Api/Materiales/FilterByCategory?categoria={categoria}");

            if (!response.IsSuccessStatusCode)
                return Json(new List<Material>());

            string json = await response.Content.ReadAsStringAsync();
            var lista = JsonConvert.DeserializeObject<List<Material>>(json);

            return Json(lista);
        }




    }
}
