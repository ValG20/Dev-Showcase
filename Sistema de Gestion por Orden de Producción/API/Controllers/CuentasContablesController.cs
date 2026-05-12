using APIBitacoras.Model;
using APIEditorialUCR.Model;
using Microsoft.AspNetCore.Mvc;
using Sistema_Editorial.Model;

namespace APIBitacoras.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class Cuentas_ContablesController : ControllerBase
    {
       
        private readonly DbContextEditorial _context = null;

       
        public Cuentas_ContablesController (DbContextEditorial pContext)
        {
            _context = pContext; 
        }

        
        [HttpGet("List")]
        public List<Cuentas_Contables> List()
        {
            List<Cuentas_Contables> temp = _context.Cuentas_Contables.ToList();
            return temp;
        }

        
        [HttpPost("Save")]
        public string Save(Cuentas_Contables temp)
        {
            string msj = "Cuenta contable guardada...";

            try
            {
                int nuevoId = 1;
                if (_context.Cuentas_Contables.Any())
                    nuevoId = _context.Cuentas_Contables.Max(c => c.id_cuenta) + 1; 
                temp.id_cuenta = nuevoId;

                _context.Cuentas_Contables.Add(temp);
                _context.SaveChanges();
            }
            catch (Exception ex)
            {
                msj = ex.InnerException?.Message ?? ex.Message;
            }

            return msj;
        }


       
        [HttpGet("SearchById")]
        public ActionResult<Cuentas_Contables> SearchById(int id_cuenta)
        {
            Cuentas_Contables temp = _context.Cuentas_Contables.FirstOrDefault(x => x.id_cuenta == id_cuenta);

            if (temp == null)
            {
                return NotFound(new { Message = "No existe ninguna cuenta contable con ese ID." });
            }

            return Ok(temp);
        }

       
        [HttpDelete("Delete")]
        public async Task<string> Delete(int id_cuenta)
        {
            string msj = "";
            try
            {
                var temp = _context.Cuentas_Contables.FirstOrDefault(u => u.id_cuenta == id_cuenta);
                if (temp == null)
                {
                    msj = $"No existe la cuenta contable con ID {id_cuenta}";
                }
                else
                {
                    _context.Cuentas_Contables.Remove(temp);
                    await _context.SaveChangesAsync();
                    msj = $"Cuenta contable con ID {id_cuenta} eliminada correctamente.";
                }
            }
            catch (Exception ex)
            {
                msj = ex.InnerException?.Message ?? ex.Message;
            }
            return msj;
        }

        // Método encargado de modificar los datos de una cuenta contable
        [HttpPut("Update")]
        public async Task<string> Update(Cuentas_Contables pCuenta)
        {
            string msj = $"Actualizando cuenta contable {pCuenta.id_cuenta}";

            try
            {
                var temp = _context.Cuentas_Contables.FirstOrDefault(t => t.id_cuenta == pCuenta.id_cuenta);

                if (temp != null)
                {
                   

                    temp.id_cuenta = pCuenta.id_cuenta;
                    temp.codigo_cuenta = pCuenta.codigo_cuenta;
                    temp.nombre_cuenta = pCuenta.nombre_cuenta;
                   

                    _context.Cuentas_Contables.Update(temp);
                    await _context.SaveChangesAsync();

                    msj = $"La cuenta contable con ID {temp.id_cuenta} fue actualizada correctamente.";
                }
                else
                {
                    msj = $"No existe una cuenta contable con el ID {pCuenta.id_cuenta}.";
                }
            }
            catch (Exception ex)
            {
                msj = ex.InnerException?.Message ?? ex.Message;
            }

            return msj;
        }

    }
}
