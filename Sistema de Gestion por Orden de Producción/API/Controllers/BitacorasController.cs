using APIBitacoras.Model;
using APIEditorialUCR.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Sistema_Editorial.Model;

namespace APIBitacoras.Controllers
{

    [ApiController]
    [Route("[controller]")]
    public class BitacorasController : ControllerBase
    {


        private readonly DbContextEditorial _context = null;

        public BitacorasController (DbContextEditorial pContext)
        {
            _context = pContext; 
        }

        [HttpGet("List")]
        public List<Bitacora> List()
        {
            
            List<Bitacora> temp = _context.Bitacora.ToList();

            return temp; 
        }

        
        [HttpPost("Save")]
        public string Save(Bitacora temp)
        {
            string msj = "Bitacora guardada...";
            try
            {
               
                _context.Bitacora.Add(temp);

               
                _context.SaveChanges();
            }
            catch (Exception ex) 
            {
                
                msj = ex.InnerException?.Message ?? ex.Message;
            }
            return msj;
        }

      

        [HttpGet("SearchById")]
        public ActionResult<Bitacora> SearchById(int id_bitacora)
        {
            Bitacora temp = _context.Bitacora.FirstOrDefault(x => x.id_bitacora == id_bitacora);

            if (temp == null)
            {
                return NotFound(new { Message = "No existe ninguna bitácora con ese ID." });
            }

            return Ok(temp);
        }

       
        [HttpDelete("Delete")]
        public async Task<string> Delete(int id_bitacora)
        {
            string msj = "";
            try
            {
                var temp = _context.Bitacora.FirstOrDefault(u => u.id_bitacora == id_bitacora);
                if (temp == null)
                {
                    msj = $"No exists Book {id_bitacora}";
                }
                else
                {
                   
                    _context.Bitacora.Remove(temp);
                   
                    await _context.SaveChangesAsync();
                   
                    msj = $"Delete";
                }
            }
            catch (Exception ex)
            {

                msj = ex.InnerException.Message;
            }
            return msj; 
        }

        
        [HttpPut("Update")]
        public async Task<string> Update(Bitacora pBitacora)
        {
            string msj = $"Actualizando bitácora {pBitacora.id_bitacora}";

            try
            {
               
                var temp = _context.Bitacora.FirstOrDefault(t => t.id_bitacora == pBitacora.id_bitacora);

                if (temp != null)
                {
                   
                    temp.id_usuario = pBitacora.id_usuario;
                    temp.fecha = pBitacora.fecha;
                    temp.accion = pBitacora.accion;
                    temp.detalle = pBitacora.detalle;

                    _context.Bitacora.Update(temp);
                    await _context.SaveChangesAsync();

                    msj = $"La bitácora con ID {temp.id_bitacora} fue actualizada correctamente.";
                }
                else
                {
                    msj = $"No existe una bitácora con el ID {pBitacora.id_bitacora}.";
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
