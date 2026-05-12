using APIEditorialUCR.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Sistema_Editorial.Model;

namespace APIEditorialUCR.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class Servicio_impresionController : Controller
    {
       
        private readonly DbContextEditorial _context = null;

        public Servicio_impresionController(DbContextEditorial pContext)
        {
            _context = pContext;
        }

        [HttpGet("Lista")]
        public List<Servicio_impresion> Lista()
        {
            List<Servicio_impresion> temp = _context.Servicio_Impresion.ToList();

            return temp;
        }

        [HttpGet("Buscar")]
        public IActionResult Buscar(int id)
        {
            var temp = _context.Servicio_Impresion.FirstOrDefault(x => x.id_servicio == id);

            if (temp == null)
            {
                return NotFound($"No existe un diseño con el identificador {id}.");
            }

            return Ok(temp);
        }

        [HttpPost("Guardar")]
        public string Guardar(Servicio_impresion temp)
        {
            string msj = "Servicio de impresión guardado correctamente.";

            try
            {
                int maxId = _context.Servicio_Impresion.Any()
                                ? _context.Servicio_Impresion.Max(s => s.id_servicio)
                                : 0;
                temp.id_servicio = maxId + 1;

                // 2️⃣ Sanitizar strings para evitar nulls
                temp.tamaño = temp.tamaño ?? "";
                temp.portada_tipo_cartulina = temp.portada_tipo_cartulina ?? "";
                temp.contenido_origen = temp.contenido_origen ?? "";
                temp.colores_separacion = temp.colores_separacion ?? "";
                temp.impresion_tipo = temp.impresion_tipo ?? "";
                temp.sello_unidad = temp.sello_unidad ?? "";
                temp.tipo_papel = temp.tipo_papel ?? "";
                temp.opciones_impresion = temp.opciones_impresion ?? "";
                temp.encolado = temp.encolado ?? "";
                temp.fuente_financiamiento = temp.fuente_financiamiento ?? "";
                temp.cargado_a = temp.cargado_a ?? "";
                temp.codigo_presupuestario = temp.codigo_presupuestario ?? "";
                temp.datos_contacto = temp.datos_contacto ?? "";
                temp.autorizacion_firma = temp.autorizacion_firma ?? "";
                temp.sello_recibido_por_siedin = temp.sello_recibido_por_siedin ?? "";
                temp.retiro_firma = temp.retiro_firma ?? "";
                temp.recepcionista = temp.recepcionista ?? "";

                // 3️⃣ Asegurar fechas válidas
                temp.fecha_solicitud = temp.fecha_solicitud == default ? DateTime.Now : temp.fecha_solicitud;
                temp.fecha_hora_recepcion = temp.fecha_hora_recepcion == default ? DateTime.Now : temp.fecha_hora_recepcion;
                temp.fecha_retiro = temp.fecha_retiro == default ? DateTime.Now : temp.fecha_retiro;

                // 4️⃣ Agregar y guardar en la BD
                _context.Servicio_Impresion.Add(temp);
                _context.SaveChanges();
            }
            catch (Exception ex)
            {
                msj = ex.InnerException?.Message ?? ex.Message;
            }

            return msj;
        }



        [HttpDelete("Eliminar")]
        public string Eliminar(int id)
        {
            string msj = "Eliminando Servicio impresion...";

            try
            {
                var temp = _context.Servicio_Impresion.FirstOrDefault(r => r.id_servicio == id);

                if (temp == null)
                {
                    msj = "No existe el Servicio impresion.";
                }
                else
                {

                    _context.Servicio_Impresion.Remove(temp);
                    _context.SaveChanges();

                }
            }
            catch (Exception ex)
            {
                msj = ex.InnerException?.Message ?? ex.Message;
            }

            return msj;
        }

        [HttpPut("Actualizar")]
        public string Actualizar(Servicio_impresion temp)
        {
            string msj = "Actualizando servicio de impresión...";

            try
            {
                var objServicio = _context.Servicio_Impresion.FirstOrDefault(r => r.id_servicio == temp.id_servicio);

                if (objServicio == null)
                {
                    msj = "No existe el servicio de impresión.";
                }
                else
                {
                    objServicio.id_pedido = temp.id_pedido;
                    objServicio.fecha_solicitud = temp.fecha_solicitud;
                    objServicio.tamaño = temp.tamaño;
                    objServicio.portada_tipo_cartulina = temp.portada_tipo_cartulina;
                    objServicio.contenido_origen = temp.contenido_origen;
                    objServicio.colores_separacion = temp.colores_separacion;
                    objServicio.impresion_tipo = temp.impresion_tipo;
                    objServicio.sello_unidad = temp.sello_unidad;
                    objServicio.cantidad_paginas = temp.cantidad_paginas;
                    objServicio.tipo_papel = temp.tipo_papel;
                    objServicio.opciones_impresion = temp.opciones_impresion;
                    objServicio.cd_adjunto = temp.cd_adjunto;
                    objServicio.textos_digitados = temp.textos_digitados;
                    objServicio.archivo_enviado_email = temp.archivo_enviado_email;
                    objServicio.original_y_copias = temp.original_y_copias;
                    objServicio.numeracion_desde = temp.numeracion_desde;
                    objServicio.numeracion_hasta = temp.numeracion_hasta;
                    objServicio.en_blocks_ejemplares = temp.en_blocks_ejemplares;
                    objServicio.encolado = temp.encolado;
                    objServicio.fuente_financiamiento = temp.fuente_financiamiento;
                    objServicio.cargado_a = temp.cargado_a;
                    objServicio.codigo_presupuestario = temp.codigo_presupuestario;
                    objServicio.datos_contacto = temp.datos_contacto;
                    objServicio.autorizacion_firma = temp.autorizacion_firma;
                    objServicio.sello_recibido_por_siedin = temp.sello_recibido_por_siedin;
                    objServicio.retiro_firma = temp.retiro_firma;
                    objServicio.fecha_retiro = temp.fecha_retiro;
                    objServicio.recepcionista = temp.recepcionista;
                    objServicio.fecha_hora_recepcion = temp.fecha_hora_recepcion;

                    _context.Servicio_Impresion.Update(objServicio);
                    _context.SaveChanges();

                    msj = "Servicio de impresión actualizado correctamente.";
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
