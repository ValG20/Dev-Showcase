using APIEditorial.ViewModels;
using APIEditorialUCR.Model;
using Microsoft.AspNetCore.Mvc;
using Sistema_Editorial.Model;

namespace APIEditorialUCR.Controllers
{
    [ApiController]
    [Route("[controller]")]

    public class PedidosController : Controller
    {
        private readonly DbContextEditorial _context = null;

        public PedidosController(DbContextEditorial pContext)
        {
            _context = pContext;
        }

        [HttpGet("Lista")]
        public List<Pedido> Lista()
        {

            List<Pedido> temp = _context.Pedidos.ToList();

            return temp;
        }

        [HttpGet("Detalles")]
        public ActionResult<Pedido> Detalles(int id)
        {
            try
            {
                var pedido = _context.Pedidos.FirstOrDefault(p => p.id_pedido == id);

                if (pedido == null)
                {
                    return NotFound($"No se encontró el pedido con ID {id}.");
                }

                return Ok(pedido);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.InnerException?.Message ?? ex.Message);
            }
        }


        [HttpPost("Guardar")]
        public IActionResult Guardar(Pedido temp)
        {
            if (temp == null)
                return BadRequest("El cuerpo de la solicitud no puede estar vacío.");

            try
            {
              
                int nuevoId = 1;
                if (_context.Pedidos.Any())
                    nuevoId = _context.Pedidos.Max(p => p.id_pedido) + 1;
                temp.id_pedido = nuevoId;

                temp.fecha_actualizacion = DateTime.Now;
                temp.fecha_creacion = DateTime.Now;

                _context.Pedidos.Add(temp);
                _context.SaveChanges();

               
                return Ok(temp.id_pedido);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.InnerException?.Message ?? ex.Message);
            }
        }



        [HttpDelete("Eliminar")]
        public string Eliminar(int id)
        {
            string msj = "Eliminando pedido...";

            try
            {
                var temp = _context.Pedidos.FirstOrDefault(r => r.id_pedido == id);

                if (temp == null)
                {
                    msj = "No existe el archivo.";
                }
                else
                {

                    _context.Pedidos.Remove(temp);
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
        public string Actualizar(Pedido temp)
        {
            string msj = "Actualizando pedido...";

            try
            {
                var objPedido = _context.Pedidos.FirstOrDefault(r => r.id_pedido == temp.id_pedido);

                if (objPedido == null)
                {
                    msj = "No existe el pedido.";
                }
                else
                {
                    objPedido.tipo_pedido = temp.tipo_pedido;
                    objPedido.id_propuesta = temp.id_propuesta;
                    objPedido.fecha_creacion = temp.fecha_creacion;
                    objPedido.orden_servicio = temp.orden_servicio;
                    objPedido.titulo_trabajo = temp.titulo_trabajo;
                    objPedido.dependencia = temp.dependencia;
                    objPedido.funcionario = temp.funcionario;
                    objPedido.estado = temp.estado;
                    objPedido.correo_funcionario = temp.correo_funcionario;
                    objPedido.telefono_funcionario = temp.telefono_funcionario;
                    objPedido.fax = temp.fax;
                    objPedido.consentimiento_cliente = temp.consentimiento_cliente;
                    objPedido.sello = temp.sello;
                    objPedido.responsable_vb = temp.responsable_vb;
                    objPedido.observaciones_generales = temp.observaciones_generales;
                    objPedido.cant_cd = temp.cant_cd;
                    objPedido.fecha_actualizacion = temp.fecha_actualizacion;
                    objPedido.campos_servicio_extra = temp.campos_servicio_extra;

                    _context.Pedidos.Update(objPedido);
                    _context.SaveChanges();

                    msj = "Pedido actualizado correctamente.";
                }
            }
            catch (Exception ex)
            {
                msj = ex.InnerException?.Message ?? ex.Message;
            }

            return msj;
        }

        [HttpPost("Aprobar")]
        public async Task<IActionResult> Aprobar(int id)
        {
            try
            {
                var pedido = await _context.Pedidos.FindAsync(id);

                if (pedido == null)
                {
                    return NotFound("No existe el pedido");
                }

                pedido.estado = "Aprobado";

                await _context.SaveChangesAsync();

                return Ok(new { mensaje = "Pedido aprobado correctamente" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        [HttpPost("Rechazo")]
        public async Task<IActionResult> Rechazo(int id)
        {
            try
            {
                var pedido = await _context.Pedidos.FindAsync(id);

                if (pedido == null)
                {
                    return NotFound("No existe el pedido");
                }

                pedido.estado = "Rechazado";

                await _context.SaveChangesAsync();

                return Ok(new { mensaje = "Pedido rechazado correctamente" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }


        // =========================================================
        // MÉTODO DE BÚSQUEDA UNIFICADA (SERVER-SIDE JOIN)
        // =========================================================

        [HttpGet("ObtenerFichaCompleta")]
        public ActionResult<List<FichaCompletaViewModel>> ObtenerFichaCompleta()
        {
            try
            {
                // 1. Consulta LINQ: Une todas las tablas y proyecta como un tipo anónimo.
                var query = from p in _context.Pedidos

                                // JOIN a la propuesta general (solo info de pedido)
                            join prop in _context.Propuestas on p.id_propuesta equals prop.id_propuesta

                            // JOIN a la propuesta editorial (nueva tabla)
                            join propEd in _context.Propuesta_Editoriales
                                on p.id_propuesta equals propEd.id_propuesta into propEdGroup
                            from propEditorial in propEdGroup.DefaultIfEmpty()

                            join aut in _context.Autores on prop.id_usuario equals aut.id_autor

                            // LEFT JOIN Teléfono Autor
                            join telAut in _context.Telefono_Autor
                                on aut.id_autor equals telAut.id_autor into telGroup
                            from tel in telGroup.DefaultIfEmpty()

                                // LEFT JOIN Dirección Autor
                            join dirAut in _context.Direccion_Autor
                                on aut.id_autor equals dirAut.id_autor into dirGroup
                            from dir in dirGroup.DefaultIfEmpty()

                                // LEFT JOIN Detalle_Cuentas_Contables
                            join detCta in _context.Detalle_Cuentas_Contables
                                on p.id_pedido equals detCta.id_pedido into detGroup
                            from det in detGroup.DefaultIfEmpty()

                                // LEFT JOIN Cuentas_Contables
                            join ctaEntidad in _context.Cuentas_Contables
                                on det.id_cuenta equals ctaEntidad.id_cuenta into ctaGroup
                            from cta in ctaGroup.DefaultIfEmpty()

                                // LEFT JOIN Especificaciones_Tecnicas
                            join espEntidad in _context.Especificaciones_Tecnicas
                                on p.id_pedido equals espEntidad.id_pedido into espGroup
                            from esp in espGroup.DefaultIfEmpty()

                                // LEFT JOIN Contenido_Impresion
                            join contEntidad in _context.Contenido_Impresion
                                on p.id_pedido equals contEntidad.id_pedido into contGroup
                            from cont in contGroup.DefaultIfEmpty()

                                // LEFT JOIN Portada
                            join portEntidad in _context.Portada
                                on p.id_pedido equals portEntidad.id_pedido into portGroup
                            from port in portGroup.DefaultIfEmpty()

                                // LEFT JOIN Acabados_Finales
                            join acabEntidad in _context.Acabados_Finales
                                on p.id_pedido equals acabEntidad.id_pedido into acabGroup
                            from acab in acabGroup.DefaultIfEmpty()

                                // LEFT JOIN Ejemplares_Costos
                            join ejem in _context.Ejemplares_Costos
                                on p.id_pedido equals ejem.id_pedido into ejemGroup
                            from ejem in ejemGroup.DefaultIfEmpty()

                            select new
                            {
                                p,
                                prop,
                                propEditorial, // <-- NUEVO: propuesta editorial
                                aut,
                                tel,
                                dir,
                                det,
                                cta,
                                esp,
                                cont,
                                port,
                                acab,
                                ejem
                            };




                // 2. Ejecutar la consulta SQL y pasar a evaluación en cliente.
                var resultadosBrutos = query.AsEnumerable().ToList();

                var resultados = resultadosBrutos
                    .GroupBy(x => x.p.id_pedido)
                    .Select(g =>
                    {
                        var primero = g.First();
                        return new FichaCompletaViewModel
                        {
                            // PEDIDO
                            id_pedido = primero.p.id_pedido,
                            tipo_pedido = primero.p.tipo_pedido,
                            fecha_creacion = primero.p.fecha_creacion,
                            orden_servicio = primero.p.orden_servicio,
                            titulo_trabajo = primero.p.titulo_trabajo,
                            dependencia = primero.p.dependencia,
                            funcionario = primero.p.funcionario,
                            estado_pedido = primero.p.estado,
                            correo_funcionario = primero.p.correo_funcionario,
                            telefono_funcionario = primero.p.telefono_funcionario,
                            fax = primero.p.fax,
                            consentimiento_cliente = primero.p.consentimiento_cliente,
                            sello = primero.p.sello,
                            responsable_vb = primero.p.responsable_vb,
                            observaciones_generales = primero.p.observaciones_generales,
                            cant_cd = primero.p.cant_cd,

                            // AUTOR
                            id_autor = primero.aut.id_autor,
                            nombre_apellidos = primero.aut.nombre_apellidos,
                            nacionalidad = primero.aut.nacionalidad,
                            tipo_cedula = primero.aut.tipo_cedula,
                            documento_identidad = primero.aut.documento_identidad,
                            estado_civil = primero.aut.estado_civil,
                            profesion = primero.aut.profesion,
                            correo_electronico = primero.aut.correo_electronico,

                            // TELEFONOS Y DIRECCIONES
                            TelefonoAutor = g.Where(x => x.tel != null).Select(x => x.tel).Distinct().ToList(),
                            DireccionAutor = g.Where(x => x.dir != null).Select(x => x.dir).Distinct().ToList(),

                            // PROPUESTA
                            id_propuesta = primero.prop.id_propuesta,
                            estado_propuesta = primero.prop.estado,
                            observaciones_usuario = primero.prop.observaciones_usuario,

                            // PROPUESTA EDITORIAL
                            id_propuesta_editorial = primero.propEditorial?.id_propuesta ?? 0,
                            fecha_creacion_editorial = primero.propEditorial?.fecha_creacion ?? DateTime.MinValue,
                            titulo_obra = primero.propEditorial?.titulo_obra,
                            subtitulo = primero.propEditorial?.subtitulo,
                            serie_coleccion = primero.propEditorial?.serie_coleccion,
                            publico_meta = primero.propEditorial?.publico_meta,
                            palabras_claves = primero.propEditorial?.palabras_claves,


                            // CUENTAS
                            CuentasContables = g.Where(x => x.cta != null).Select(x => x.cta).Distinct().ToList(),
                            detallesCuentasContables = g.Where(x => x.det != null).Select(x => x.det).Distinct().ToList(),

                            // EJEMPLARES
                            EjemplaresCostos = g.Where(x => x.ejem != null).Select(x => x.ejem).Distinct().ToList(),

                            // ESPECIFICACIONES
                            id_tecnica = primero.esp?.id_tecnica ?? 0,
                            num_paginas = primero.esp?.num_paginas ?? 0,
                            tipo_impreso = primero.esp?.tipo_impreso,
                            no_pag_color = primero.esp?.no_pag_color ?? 0,
                            departamento_impresion = primero.esp?.departamento_impresion,
                            contenido_portada = primero.esp?.contenido_portada,
                            cts_num_impresiones = primero.esp?.cts_num_impresiones ?? 0,
                            cant_pliegos_1 = primero.esp?.cant_pliegos_1 ?? 0,
                            cant_pliegos_2 = primero.esp?.cant_pliegos_2 ?? 0,
                            cant_pliegos_3 = primero.esp?.cant_pliegos_3 ?? 0,

                            // CONTENIDO IMPRESION
                            id_contenido = primero.cont?.id_contenido ?? 0,
                            tipo_impresion_contenido = primero.cont?.tipo_impresion,
                            tipo_papel = primero.cont?.tipo_papel,
                            tamano_corte = primero.cont?.tamano_corte,
                            salen_tc = primero.cont?.salen_tc,

                            // PORTADA
                            id_portada = primero.port?.id_portada ?? 0,
                            tipo_impresion_portada = primero.port?.tipo_impresion,
                            tipo_papel_portada = primero.port?.tipo_papel,
                            tamano_corte_portada = primero.port?.tamano_corte,
                            salen_tc_portada = primero.port?.salen_tc,

                            // ACABADOS
                            id_acabado = primero.acab?.id_acabado ?? 0,
                            cant_copias = primero.acab?.cant_copias ?? 0,
                            tipos = primero.acab?.tipos,
                            otro = primero.acab?.otro,
                            tecnologia_impresion_especial = primero.acab?.tecnologia_impresion_especial
                        };
                    })
                    .ToList();


                return Ok(resultados);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.InnerException?.Message ?? ex.Message);
            }
        }


        // =========================================================
        // MÉTODO DE BÚSQUEDA UNIFICADA (SERVER-SIDE JOIN)
        // =========================================================

        // APIEditorialUCR.Controllers/PedidosController.cs

        // APIEditorialUCR.Controllers/PedidosController.cs
        [HttpGet("BuscarFicha")]
        public ActionResult<FichaCompletaViewModel> BuscarFicha(int idPedido)
        {
            try
            {
                // 1. Consulta LINQ: Une y filtra por idPedido. Proyecta como tipo anónimo.
                var query = from p in _context.Pedidos
                            join prop in _context.Propuestas on p.id_propuesta equals prop.id_propuesta
                            join aut in _context.Autores on prop.id_usuario equals aut.id_autor

                            // LEFT JOIN a Detalle_Cuentas_Contables
                            join detCta in _context.Detalle_Cuentas_Contables
                                on p.id_pedido equals detCta.id_pedido into detGroup
                            from det in detGroup.DefaultIfEmpty()

                                // LEFT JOIN a CuentasContables
                            join ctaEntidad in _context.Cuentas_Contables
                                on det.id_cuenta equals ctaEntidad.id_cuenta into ctaGroup
                            from cta in ctaGroup.DefaultIfEmpty()

                                // LEFT JOIN a Especificaciones_Tecnicas
                            join espEntidad in _context.Especificaciones_Tecnicas
                                on p.id_pedido equals espEntidad.id_pedido into espGroup
                            from esp in espGroup.DefaultIfEmpty() // **Línea corregida**

                                // LEFT JOIN a Contenidos_Impresion
                            join contEntidad in _context.Contenido_Impresion
                                on p.id_pedido equals contEntidad.id_pedido into contGroup
                            from cont in contGroup.DefaultIfEmpty() // **Línea corregida**

                                // LEFT JOIN a Portada
                            join portEntidad in _context.Portada
                                on p.id_pedido equals portEntidad.id_pedido into portGroup
                            from port in portGroup.DefaultIfEmpty() // **Línea corregida**

                                // LEFT JOIN a Acabados_Finales
                            join acabEntidad in _context.Acabados_Finales
                                on p.id_pedido equals acabEntidad.id_pedido into acabGroup
                            from acab in acabGroup.DefaultIfEmpty() // **Línea corregida**

                                // LEFT JOIN a Ejemplares_Costos
                            join ejem in _context.Ejemplares_Costos
                                on p.id_pedido equals ejem.id_pedido into ejemGroup
                            from ejem in ejemGroup.DefaultIfEmpty()

                            where p.id_pedido == idPedido

                            // Proyección de tipo anónimo con todas las entidades
                            select new { p, prop, aut, det, cta, esp, cont, port, acab, ejem };

                // 2. Ejecutar la consulta SQL y pasar a evaluación en cliente.
                var resultadosBrutos = query.AsEnumerable().ToList();

                // 3. Agrupar y seleccionar el ViewModel.
                var ficha = resultadosBrutos
                    .GroupBy(x => x.p.id_pedido)
                    .Select(g =>
                    {
                        var primero = g.First();
                        return new FichaCompletaViewModel
                        {
                            // =====================
                            // PEDIDO
                            // =====================
                            id_pedido = primero.p.id_pedido,
                            tipo_pedido = primero.p.tipo_pedido,
                            fecha_creacion = primero.p.fecha_creacion,
                            orden_servicio = primero.p.orden_servicio ?? "N/A",
                            titulo_trabajo = primero.p.titulo_trabajo,
                            dependencia = primero.p.dependencia,
                            funcionario = primero.p.funcionario,
                            estado_pedido = primero.p.estado,
                            correo_funcionario = primero.p.correo_funcionario,
                            telefono_funcionario = primero.p.telefono_funcionario,
                            fax = primero.p.fax,
                            consentimiento_cliente = primero.p.consentimiento_cliente,
                            sello = primero.p.sello,
                            responsable_vb = primero.p.responsable_vb,
                            observaciones_generales = primero.p.observaciones_generales,
                            cant_cd = primero.p.cant_cd,

                            // =====================
                            // AUTOR
                            // =====================
                            id_autor = primero.aut.id_autor,
                            nombre_apellidos = primero.aut.nombre_apellidos,
                            nacionalidad = primero.aut.nacionalidad,
                            tipo_cedula = primero.aut.tipo_cedula,
                            documento_identidad = primero.aut.documento_identidad,
                            estado_civil = primero.aut.estado_civil,
                            profesion = primero.aut.profesion,
                            correo_electronico = primero.aut.correo_electronico,
                            TelefonoAutor = _context.Telefono_Autor.Where(t => t.id_autor == primero.aut.id_autor).ToList(),
                            DireccionAutor = _context.Direccion_Autor.Where(d => d.id_autor == primero.aut.id_autor).ToList(),

                            // =====================
                            // PROPUESTA
                            // =====================
                            id_propuesta = primero.prop.id_propuesta,
                            estado_propuesta = primero.prop.estado,
                            observaciones_usuario = primero.prop.observaciones_usuario,

                            // =====================
                            // CUENTAS CONTABLES
                            // =====================
                            CuentasContables = g.Where(x => x.cta != null).Select(x => x.cta).Distinct().ToList(),
                            detallesCuentasContables = g.Where(x => x.det != null).Select(x => x.det).Distinct().ToList(),

                            // =====================
                            // EJEMPLARES Y COSTOS
                            // =====================
                            EjemplaresCostos = g.Where(x => x.ejem != null).Select(x => x.ejem).Distinct().ToList(),

                            // =====================
                            // ESPECIFICACIONES TÉCNICAS
                            // =====================
                            id_tecnica = primero.esp != null ? primero.esp.id_tecnica : 0,
                            num_paginas = primero.esp != null ? primero.esp.num_paginas : 0,
                            tipo_impreso = primero.esp != null ? primero.esp.tipo_impreso : null,
                            no_pag_color = primero.esp != null ? primero.esp.no_pag_color : 0,
                            departamento_impresion = primero.esp != null ? primero.esp.departamento_impresion : null,
                            contenido_portada = primero.esp != null ? primero.esp.contenido_portada : null,
                            cts_num_impresiones = primero.esp != null ? primero.esp.cts_num_impresiones : 0,
                            cant_pliegos_1 = primero.esp != null ? primero.esp.cant_pliegos_1 : 0,
                            cant_pliegos_2 = primero.esp != null ? primero.esp.cant_pliegos_2 : 0,
                            cant_pliegos_3 = primero.esp != null ? primero.esp.cant_pliegos_3 : 0,

                            // =====================
                            // CONTENIDO IMPRESION
                            // =====================
                            id_contenido = primero.cont != null ? primero.cont.id_contenido : 0,
                            tipo_impresion_contenido = primero.cont != null ? primero.cont.tipo_impresion : null,
                            tipo_papel = primero.cont != null ? primero.cont.tipo_papel : null,
                            tamano_corte = primero.cont != null ? primero.cont.tamano_corte : null,
                            salen_tc = primero.cont != null ? primero.cont.salen_tc : null,

                            // =====================
                            // PORTADA
                            // =====================
                            id_portada = primero.port != null ? primero.port.id_portada : 0,
                            tipo_impresion_portada = primero.port != null ? primero.port.tipo_impresion : null,
                            tipo_papel_portada = primero.port != null ? primero.port.tipo_papel : null,
                            tamano_corte_portada = primero.port != null ? primero.port.tamano_corte : null,
                            salen_tc_portada = primero.port != null ? primero.port.salen_tc : null,

                            // =====================
                            // ACABADOS FINALES
                            // =====================
                            id_acabado = primero.acab != null ? primero.acab.id_acabado : 0,
                            cant_copias = primero.acab != null ? primero.acab.cant_copias : 0,
                            tipos = primero.acab != null ? primero.acab.tipos : null,
                            otro = primero.acab != null ? primero.acab.otro : null,
                            tecnologia_impresion_especial = primero.acab != null ? primero.acab.tecnologia_impresion_especial : null
                        };
                    })
                    .FirstOrDefault(); // Toma el primer (y único) resultado.

                if (ficha == null)
                    return NotFound($"No se encontró la ficha con el ID {idPedido}.");

                return Ok(ficha);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.InnerException?.Message ?? ex.Message);
            }
        }

    }
}
