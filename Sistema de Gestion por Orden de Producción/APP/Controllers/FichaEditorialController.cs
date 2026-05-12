using EditorialUCR.Models.Api;
using EditorialUCR.ViewModels;
using EditorialUCR.ViewModels.BusquedaDeFicha;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace EditorialUCR.Controllers
{
    public class FichaEditorialController : Controller
    {
        private readonly FichaCompletaController fichaCompletaController;


        public FichaEditorialController()
        {
            // Instanciamos FichaCompletaController para reutilizar sus métodos
            fichaCompletaController = new FichaCompletaController();
        }

        // =========================================================
        // 1. LISTAR FICHAS / BUSCADOR
        // =========================================================
        [HttpGet]
        public async Task<IActionResult> Index(string buscar)
        {
            // Llamamos directamente a la acción de listado de FichaCompletaController
            return await fichaCompletaController.GestionFichasPresupuesto();
        }

        // =========================================================
        // 2. CREAR NUEVA FICHA
        // =========================================================
        [HttpGet]
        public async Task<IActionResult> CrearFicha()
        {
            return await fichaCompletaController.CrearFicha();
        }


        // =========================================================
        // 3. VER / EDITAR FICHA
        // =========================================================
        [HttpGet]
        public async Task<IActionResult> DetallesFicha(int idPedido, string modo = "Ver")
        {
            return await fichaCompletaController.DetallesFicha(idPedido, modo);
        }

        // =========================================================
        // 4. GUARDAR FICHA (POST) - CREA O EDITA
        // =========================================================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> GuardarFicha(FichaCompletaViewModel modelo)
        {
            return await fichaCompletaController.GuardarFicha(modelo);
        }
    }
}
