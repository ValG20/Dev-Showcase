using APIEditorialUCR.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Sistema_Editorial.Model;

namespace Sistema_Editorial.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MaterialesController : ControllerBase
    {

       
        private readonly DbContextEditorial _context = null;

        
        public MaterialesController(DbContextEditorial pContext)
        {
            _context = pContext; 
        }

       
        [HttpGet("List")]
        public List<Material> List()
        {
           
            List<Material> temp = _context.Materiales.ToList();

            return temp; 
        }

        
        [HttpPost("Save")]
        public string Save(Material temp)
        {
            string msj = "Material guardado...";
            try
            {
                List<Material> lista = _context.Materiales.ToList();

                int maxId = 1;

                if (lista != null && lista.Count > 0)
                {
                    maxId = lista.Max(p => p.id_material);
                }

                temp.id_material = maxId + 1;
               
                _context.Materiales.Add(temp);

               
                _context.SaveChanges();
            }
            catch (Exception ex)
            {
               
                msj = ex.InnerException?.Message ?? ex.Message;
            }
            return msj;
        }

       
        [HttpGet("SearchById")]
        public ActionResult<Material> SearchById(int id_producto)
        {
            Material temp = _context.Materiales.FirstOrDefault(x => x.id_material == id_producto);

            if (temp == null)
            {
                return NotFound(new { Message = "No existe ningún producto con ese ID." });
            }

            return Ok(temp);
        }

       
        [HttpDelete("Delete")]
        public async Task<string> Delete(int id_producto)
        {
            string msj = "";
            try
            {
                var temp = _context.Materiales.FirstOrDefault(u => u.id_material == id_producto);

                if (temp == null)
                {
                    msj = $"No existe el Producto {id_producto}";
                }
                else
                {
                   
                    _context.Materiales.Remove(temp);

                   
                    await _context.SaveChangesAsync();

                    msj = "Delete";
                }
            }
            catch (Exception ex)
            {
                msj = ex.InnerException?.Message ?? ex.Message;
            }
            return msj; 
        }

       
        [HttpPut("Update")]
        public async Task<string> Update(Material pProducto)
        {
            string msj = $"Actualizando producto {pProducto.id_material}";

            try
            {
               
                var temp = _context.Materiales.FirstOrDefault(t => t.id_material == pProducto.id_material);

                if (temp != null)
                {
                    temp.nombre_material = pProducto.nombre_material;
                    temp.categoria = pProducto.categoria;
                    temp.unidad_medida = pProducto.unidad_medida;
                    temp.stock_actual = pProducto.stock_actual;
                    temp.stock_minimo = pProducto.stock_minimo;
                    temp.ubicacion = pProducto.ubicacion;
                    temp.costo_unitario = pProducto.costo_unitario;
                    temp.proveedor = pProducto.proveedor;
                    temp.estado = pProducto.estado;
                    temp.fecha_registro = pProducto.fecha_registro;

                    _context.Materiales.Update(temp);
                    await _context.SaveChangesAsync();

                    msj = $"El producto con ID {temp.id_material} fue actualizado correctamente.";
                }
                else
                {
                    msj = $"No existe un producto con el ID {pProducto.id_material}.";
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
