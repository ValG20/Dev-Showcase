using APIEditorialUCR.Model;
using EditorialUCR.Models;
using EditorialUCR.Models.Api;
using EditorialUCR.Models.Central;
using EditorialUCR.Models.Financieros;
using EditorialUCR.Models.Impresion;
using EditorialUCR.ViewModels.BusquedaDeFicha;
using Microsoft.AspNetCore.DataProtection.KeyManagement;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace EditorialUCR.Controllers
{
    // =========================================================
    // 1. INICIALIZACIÓN Y CONFIGURACIÓN
    // =========================================================
    public class FichaCompletaController : Controller
    {
        private readonly ApiEditorial client; // Asumo que ApiEditorial es tu clase de configuración de HttpClient
        private readonly HttpClient api;

        public FichaCompletaController()
        {
            client = new ApiEditorial();
            api = client.IniciarApi();
        }

        [HttpGet]
        public async Task<IActionResult> BusquedaFichas()
        {
            List<FichaCompletaViewModel> listadoFichas = new List<FichaCompletaViewModel>();

            try
            {
                HttpResponseMessage response = await api.GetAsync("Pedidos/ObtenerFichaCompleta");

                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadAsStringAsync();
                    listadoFichas = JsonConvert.DeserializeObject<List<FichaCompletaViewModel>>(result);
                }
                else
                {
                    ViewBag.Mensaje = "Error al obtener las fichas desde la API.";
                }
            }
            catch (Exception ex)
            {
                ViewBag.Mensaje = $"Error: {ex.Message}";
            }

            return View("~/Views/Editoriales/Fichas/BusquedaDeFicha.cshtml", listadoFichas);
        }



        // =========================================================
        // 2. ACCIÓN DE LISTADO (GESTIÓN DE FICHAS)
        // =========================================================
        [HttpGet]
        public async Task<IActionResult> GestionFichasPresupuesto()
        {
            List<FichaCompletaViewModel> listadoFichas = new List<FichaCompletaViewModel>();
            ViewBag.Mensaje = null;

            try
            {
                // RUTA API: GET Pedidos/ObtenerFichaCompleta
                HttpResponseMessage response = await api.GetAsync("Pedidos/ObtenerFichaCompleta");

                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadAsStringAsync();
                    listadoFichas = JsonConvert.DeserializeObject<List<FichaCompletaViewModel>>(result);
                }
                else
                {
                    ViewBag.Mensaje = $"Error al obtener el listado de fichas: {response.ReasonPhrase}";
                }

                if (listadoFichas == null || listadoFichas.Count == 0)
                {
                    ViewBag.Mensaje = "No hay solicitudes de publicación registradas actualmente.";
                    listadoFichas = new List<FichaCompletaViewModel>();
                }
            }
            catch (Exception ex)
            {
                ViewBag.Mensaje = $"Error de conexión al obtener las fichas: {ex.InnerException?.Message ?? ex.Message}";
            }

            return View("~/Views/Editoriales/Fichas/BusquedaDeFicha.cshtml",listadoFichas);
        }

        // =========================================================
        // 3. ACCIÓN DE CREACIÓN (Muestra el formulario vacío)
        // =========================================================

        [HttpGet]
        public async Task<IActionResult> CrearFicha()
        {
            ViewBag.Modo = "Crear";
            ViewData["Title"] = "Crear Nueva Ficha de Presupuesto";

            var model = new FichaCompletaViewModel();

            // === Cargar cuentas contables desde la API ===
            try
            {
                var resp = await api.GetAsync("Cuentas_Contables/List");

                if (resp.IsSuccessStatusCode)
                {
                    var json = await resp.Content.ReadAsStringAsync();
                    model.CuentasContables = JsonConvert.DeserializeObject<List<CuentaContable>>(json);
                }
                else
                {
                    model.CuentasContables = new List<CuentaContable>();
                }
            }
            catch
            {
                model.CuentasContables = new List<CuentaContable>();
            }

            // --- Inicializar detalles de cuentas en 0 si no hay datos ---
            if (model.CuentasContables.Any())
            {
                foreach (var cuenta in model.CuentasContables)
                {
                    if (!model.detallesCuentasContables.Any(d => d.id_cuenta == cuenta.id_cuenta))
                    {
                        model.detallesCuentasContables.Add(new Detalle_Cuentas_Contables
                        {
                            id_detalle = 0,
                            id_cuenta = cuenta.id_cuenta,
                            id_pedido = 0,
                            costo_directo = 0,
                            costo_indirecto = 0,
                            libro_digital = 0,
                            otros = 0
                        });
                    }
                }
            }

            // --- Inicializar 3 ejemplares vacíos ---
            for (int i = 0; i < 3; i++)
            {
                model.EjemplaresCostos.Add(new EjemplarCosto
                {
                    id_ejemplar = 0,
                    id_pedido = 0,
                    cantidad = 0,
                    costo = 0
                });
            }

            return View("~/Views/Editoriales/Fichas/FichaEditorial.cshtml", model);
        }


        // =========================================================
        // 2. DETALLES / VER / EDITAR FICHA
        // =========================================================
        [HttpGet]
        public async Task<IActionResult> DetallesFicha(int idPedido, string modo = "Ver")
        {
            FichaCompletaViewModel fichaCompleta = new FichaCompletaViewModel();
            ViewBag.Modo = modo;
            ViewData["Title"] = modo.Equals("Editar", StringComparison.OrdinalIgnoreCase)
                                ? "Editar Ficha de Presupuesto"
                                : "Ver Detalles de Ficha";

            try
            {
                if (idPedido > 0) // Ficha existente
                {
                    var response = await api.GetAsync($"Pedidos/BuscarFicha?idPedido={idPedido}");
                    if (!response.IsSuccessStatusCode)
                    {
                        TempData["MensajeError"] = $"Error de la API ({(int)response.StatusCode}): {await response.Content.ReadAsStringAsync()}";
                        return RedirectToAction(nameof(GestionFichasPresupuesto));
                    }

                    var result = await response.Content.ReadAsStringAsync();
                    fichaCompleta = JsonConvert.DeserializeObject<FichaCompletaViewModel>(result);

                    if (fichaCompleta == null || fichaCompleta.id_pedido == 0)
                    {
                        TempData["MensajeError"] = $"No se encontró la ficha con ID: {idPedido}.";
                        return RedirectToAction(nameof(GestionFichasPresupuesto));
                    }
                }
                else
                {
                    // Nueva ficha vacía
                    fichaCompleta.id_pedido = 0;
                    fichaCompleta.CuentasContables = new List<CuentaContable>();
                    fichaCompleta.detallesCuentasContables = new List<Detalle_Cuentas_Contables>();
                    fichaCompleta.EjemplaresCostos = new List<EjemplarCosto>();
                }

                // Inicializar listas no nulas
                fichaCompleta.CuentasContables ??= new List<CuentaContable>();
                fichaCompleta.detallesCuentasContables ??= new List<Detalle_Cuentas_Contables>();
                fichaCompleta.EjemplaresCostos ??= new List<EjemplarCosto>();
                fichaCompleta.TelefonoAutor ??= new List<Telefono_autor>();
                fichaCompleta.DireccionAutor ??= new List<Direccion_autor>();

                // --- Asegurar que cada cuenta tenga su detalle (solo agregar las que falten) ---
                foreach (var cuenta in fichaCompleta.CuentasContables)
                {
                    if (!fichaCompleta.detallesCuentasContables.Any(d => d.id_cuenta == cuenta.id_cuenta))
                    {
                        fichaCompleta.detallesCuentasContables.Add(new Detalle_Cuentas_Contables
                        {
                            id_detalle = 0,
                            id_cuenta = cuenta.id_cuenta,
                            id_pedido = fichaCompleta.id_pedido,
                            costo_directo = 0,
                            costo_indirecto = 0,
                            libro_digital = 0,
                            otros = 0
                        });
                    }
                }

                // --- Asegurar siempre 3 ejemplares ---
                while (fichaCompleta.EjemplaresCostos.Count < 3)
                {
                    fichaCompleta.EjemplaresCostos.Add(new EjemplarCosto
                    {
                        id_ejemplar = 0,
                        id_pedido = fichaCompleta.id_pedido,
                        cantidad = 0,
                        costo = 0
                    });
                }
            }
            catch (Exception ex)
            {
                TempData["MensajeError"] = $"Error inesperado: {ex.Message}";
                return RedirectToAction(nameof(GestionFichasPresupuesto));
            }

            return View("~/Views/Editoriales/Fichas/FichaEditorial.cshtml", fichaCompleta);
        }



        // =========================================================
        // ACCIÓN POST UNIFICADA (DELEGADOR)
        // =========================================================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> GuardarFicha(FichaCompletaViewModel modeloCompleto)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Mensaje = "Verifique los datos del formulario.";
                ViewBag.Modo = modeloCompleto.id_pedido > 0 ? "Editar" : "Crear";
                return View("~/Views/Editoriales/Fichas/FichaEditorial.cshtml", modeloCompleto);
            }

            try
            {
                bool esEdicion = modeloCompleto.id_pedido > 0;

                if (esEdicion)
                {
                    await ActualizarFichaCompleta(modeloCompleto);
                }
                else
                {
                    var resultadoCreacion = await CrearFichaCompleta(modeloCompleto, TempData);

                    if (resultadoCreacion != null)
                    {
                        return resultadoCreacion;
                    }
                }

                // Éxito: redirige a la búsqueda con mensaje
                TempData["MensajeExito"] = "¡La Solicitud de Publicación fue guardada exitosamente!";
                return RedirectToAction("Index", "BusquedaDeFicha");
            }
            catch (Exception ex)
            {
                // Error: también redirige a la búsqueda para mostrar mensaje
                TempData["MensajeError"] = $"Error al guardar la ficha: {ex.Message}";
                return RedirectToAction("Index", "BusquedaDeFicha");
            }
        }



        // =========================================================
        // 6. LÓGICA DE CREACIÓN (POST) - (Encadenamiento de 10 POSTs por entidad)
        // =========================================================
        private async Task<IActionResult> CrearFichaCompleta(FichaCompletaViewModel modeloCompleto, ITempDataDictionary tempData)
        {
            // Variables para almacenar las IDs generadas
            int idAutor = 0;
            int idPropuesta = 0;
            int idPedido = 0;
            int idTecnica = 0;
            int idCuenta = 0;

            // Configuración para el retorno en caso de error
            ViewBag.Modo = "Crear";
            ViewData["Title"] = "Crear Nueva Ficha de Presupuesto";

            // Función auxiliar para serialización
            StringContent CreateJsonContent(object data) => new StringContent(
                JsonConvert.SerializeObject(data),
                System.Text.Encoding.UTF8,
                "application/json"
            );

            // Función auxiliar para manejar errores - ahora usa tempData
            IActionResult HandleCreationError(string step, HttpResponseMessage response, string errorMessage)
            {
                string errorContent = "No se proporcionó contenido de error por parte del API.";

                if (response.Content != null)
                {
                    try
                    {
                        errorContent = response.Content.ReadAsStringAsync().Result;
                    }
                    catch (Exception ex)
                    {
                        errorContent = $"Error al leer el contenido: {ex.Message}";
                    }
                }

                tempData["MensajeError"] = $"Error al crear la ficha (Paso: **{step}**). Código HTTP: {(int)response.StatusCode}. Detalle API: {errorContent}. Mensaje: {errorMessage}";
                return View("~/Views/Editoriales/Fichas/FichaEditorial.cshtml", modeloCompleto);
            }

            // Función auxiliar para manejar excepciones - ahora usa tempData
            IActionResult HandleException(string step, Exception ex)
            {
                tempData["MensajeError"] = $"Error inesperado durante la creación (Paso: **{step}**): {ex.InnerException?.Message ?? ex.Message}";
                return View("~/Views/Editoriales/Fichas/FichaEditorial.cshtml", modeloCompleto);
            }

            try
            {
                // ======================================================
                // 1. OBTENER LISTA COMPLETA DE AUTORES
                // ======================================================
                HttpResponseMessage autoresResponse = await api.GetAsync("api/Autor");

                if (!autoresResponse.IsSuccessStatusCode)
                {
                    throw new Exception("No se pudo obtener la lista de autores desde la API.");
                }

                var listaAutores = await autoresResponse.Content.ReadFromJsonAsync<List<Autor>>();

                if (listaAutores == null)
                {
                    throw new Exception("La lista de autores vino vacía o no se pudo deserializar.");
                }

                // Buscar si ya existe un autor con esa cédula
                var autorExistente = listaAutores
                    .FirstOrDefault(a => a.documento_identidad == modeloCompleto.documento_identidad);

                // ======================================================
                // 2. SI EXISTE → ACTUALIZAR AUTOR (PUT api/Autor/{id})
                // ======================================================
                if (autorExistente != null)
                {
                    // Mapear datos actualizados
                    autorExistente.nombre_apellidos = modeloCompleto.nombre_apellidos;
                    autorExistente.nacionalidad = modeloCompleto.nacionalidad;
                    autorExistente.tipo_cedula = modeloCompleto.tipo_cedula;
                    autorExistente.estado_civil = modeloCompleto.estado_civil;
                    autorExistente.profesion = modeloCompleto.profesion;
                    autorExistente.correo_electronico = modeloCompleto.correo_electronico;

                    // Enviar PUT
                    HttpResponseMessage updateResponse =
                        await api.PutAsJsonAsync($"api/Autor/{autorExistente.id_autor}", autorExistente);

                    if (!updateResponse.IsSuccessStatusCode)
                    {
                        var error = await updateResponse.Content.ReadAsStringAsync();
                        throw new Exception($"Error al actualizar el autor: {error}");
                    }

                    idAutor = autorExistente.id_autor;
                }
                else
                {
                    // ======================================================
                    // 3. SI NO EXISTE → CREAR AUTOR (POST api/Autor)
                    // ======================================================
                    var autorData = new
                    {
                        nombre_apellidos = modeloCompleto.nombre_apellidos,
                        documento_identidad = modeloCompleto.documento_identidad,
                        nacionalidad = modeloCompleto.nacionalidad,
                        tipo_cedula = modeloCompleto.tipo_cedula,
                        estado_civil = modeloCompleto.estado_civil,
                        profesion = modeloCompleto.profesion,
                        correo_electronico = modeloCompleto.correo_electronico
                    };

                    HttpResponseMessage autorResponse =
                        await api.PostAsJsonAsync("api/Autor", autorData);

                    if (!autorResponse.IsSuccessStatusCode)
                    {
                        var error = await autorResponse.Content.ReadAsStringAsync();
                        throw new Exception($"Error al crear el autor: {error}");
                    }

                    // Leer ID generado por el POST
                    idAutor = await autorResponse.Content.ReadFromJsonAsync<int>();
                }

                // Guardar ID en el modelo
                modeloCompleto.id_autor = idAutor;


                // --- 2. GUARDAR TELEFONOS DE AUTOR ---
                if (modeloCompleto.TelefonoAutor != null && modeloCompleto.TelefonoAutor.Count > 0)
                {
                    foreach (var tel in modeloCompleto.TelefonoAutor)
                    {
                        var telefonoData = new
                        {
                            id_autor = idAutor,   // Asociar al autor recién creado
                            telefono = tel.telefono
                        };

                        HttpResponseMessage telResponse = await api.PostAsync("api/TelefonoAutor", CreateJsonContent(telefonoData));
                        if (!telResponse.IsSuccessStatusCode)
                            return HandleCreationError("2. Teléfonos", telResponse, "No se pudo guardar el teléfono del autor.");
                    }
                }

                // --- 3. GUARDAR DIRECCIONES DE AUTOR ---
                if (modeloCompleto.DireccionAutor != null && modeloCompleto.DireccionAutor.Count > 0)
                {
                    foreach (var dir in modeloCompleto.DireccionAutor)
                    {
                        var direccionData = new
                        {
                            id_autor = idAutor,  // Asociar al autor recién creado
                            direccion = dir.direccion
                        };

                        HttpResponseMessage dirResponse = await api.PostAsync("api/DireccionAutor", CreateJsonContent(direccionData));
                        if (!dirResponse.IsSuccessStatusCode)
                            return HandleCreationError("3. Direcciones", dirResponse, "No se pudo guardar la dirección del autor.");
                    }
                }

                // esto se quita y se cambia por el id del usuario cuando se implemente el guardar login
                const int idUsuarioTemporal = 1;

                // 1. Obtener la lista completa de catálogos desde la API
                var respuestaCatalogo = await api.GetAsync("Catalogo/Lista");

                if (!respuestaCatalogo.IsSuccessStatusCode)
                {
                    TempData["Error"] = "No se pudo obtener la lista de catálogos desde la API.";
                    return RedirectToAction("CrearFichaCompleta");
                }

                // 2. Leer el contenido como lista
                var listaCatalogos = await respuestaCatalogo.Content.ReadFromJsonAsync<List<Producto>>();

                if (listaCatalogos == null || listaCatalogos.Count == 0)
                {
                    TempData["Error"] = "La API no devolvió catálogos.";
                    return RedirectToAction("CrearFichaCompleta");
                }

                // 3. Buscar el catálogo con tipo EXACTO "Publicar Libro"
                var catalogoPublicarLibro = listaCatalogos
                    .FirstOrDefault(c => c.Tipo != null && c.Tipo.Trim() == "Publicar Libro");

                if (catalogoPublicarLibro == null)
                {
                    TempData["Error"] = "No existe un catálogo con el tipo 'Publicar Libro'.";
                    return RedirectToAction("CrearFichaCompleta");
                }

                // 4. Obtener el ID real desde la base de datos
                int idCatalogo = catalogoPublicarLibro.Id;


                // ---------------------------------------------------------------------
                // 5. Tu objeto propuestaGeneralData tal como me pediste
                // ---------------------------------------------------------------------

                var propuestaGeneralData = new
                {
                    id_usuario = idUsuarioTemporal,
                    id_producto = idCatalogo,               // ← AHORA usa el valor real
                    id_propuesta = 0,
                    fecha_envio = DateTime.Now,
                    estado = "Enviado",
                    observaciones_usuario = modeloCompleto.observaciones_generales ?? string.Empty,
                };


                // **Paso 1.1:** Llamada a la API para guardar la Propuesta General
                HttpResponseMessage generalResponse = await api.PostAsync("api/Propuesta", CreateJsonContent(propuestaGeneralData));

                if (!generalResponse.IsSuccessStatusCode)
                    return HandleCreationError("1. Propuesta General", generalResponse, "No se pudo guardar el registro padre en Propuestas.");

                // **Paso 1.2:** Obtener el ID generado por la Base de Datos
                string idString = await generalResponse.Content.ReadAsStringAsync();

                // Parsear a entero
                idPropuesta = int.Parse(idString.Trim());

                // Validar ID
                if (idPropuesta <= 0)
                    throw new Exception("La API de Propuesta no devolvió un ID válido.");

                // Guardarlo en el modelo
                modeloCompleto.id_propuesta = idPropuesta;

                // --- 2. GUARDAR PROPUESTA editorial ---
                var propuestaEditorialData = new
                {
                    id_propuesta = idPropuesta, //hay un problema, no puedo enlasarla a propuesta porque otro modulo genera propuestaEditorial y es posible que ya haya numeros de ids iguales al idPropuesta que le meto, hay que arreglarlo despues posiblemente tocando base de datos
                    subtitulo = modeloCompleto.subtitulo,
                    titulo_obra = modeloCompleto.titulo_obra,
                    fechaCreacion = DateTime.Now,
                    serie_coleccion = modeloCompleto.serie_coleccion, // Usamos nombre de propiedad correcto
                    publico_meta = modeloCompleto.publico_meta,
                    palabras_claves = modeloCompleto.palabras_claves,
                };
                HttpResponseMessage propuestaResponse = await api.PostAsync("Propuesta_Editorial/Guardar", CreateJsonContent(propuestaEditorialData));

                if (!propuestaResponse.IsSuccessStatusCode) return HandleCreationError("2. Propuesta editorial", propuestaResponse, "No se pudo guardar la Propuesta.");

                //idPropuesta = int.Parse(await propuestaResponse.Content.ReadAsStringAsync());
                //modeloCompleto.id_propuesta = idPropuesta;
                if (idPropuesta == 0) throw new Exception("La API de Propuesta no devolvió un ID válido.");


                var pedido = new Pedido
                {
                    id_propuesta = idPropuesta,
                    tipo_pedido = "Libro",
                    fecha_creacion = DateTime.Now,
                    orden_servicio = modeloCompleto.orden_servicio,
                    titulo_trabajo = modeloCompleto.titulo_trabajo,
                    dependencia = modeloCompleto.dependencia,
                    funcionario = modeloCompleto.funcionario,
                    estado = "Enviado",
                    correo_funcionario = modeloCompleto.correo_funcionario,
                    telefono_funcionario = modeloCompleto.telefono_funcionario,
                    fax = modeloCompleto.fax,
                    consentimiento_cliente = modeloCompleto.consentimiento_cliente,
                    sello = modeloCompleto.sello,
                    responsable_vb = modeloCompleto.responsable_vb,
                    observaciones_generales = modeloCompleto.observaciones_generales,
                    cant_cd = modeloCompleto.cant_cd,
                    campos_servicio_extra = "N/A",
                    total = modeloCompleto.total,
                    fecha_actualizacion = DateTime.Now
                };

                HttpResponseMessage pedidoResponse = await api.PostAsync("Pedidos/Guardar", CreateJsonContent(pedido));
                if (!pedidoResponse.IsSuccessStatusCode) return HandleCreationError("Pedido", pedidoResponse, "No se pudo guardar el Pedido.");

                idPedido = int.Parse(await pedidoResponse.Content.ReadAsStringAsync());
                modeloCompleto.id_pedido = idPedido;


                // --- 4. GUARDAR ESPECIFICACIONES TÉCNICAS ---
                var tecnica = new Especificaciones_Tecnicas
                {
                    id_pedido = idPedido,
                    num_paginas = modeloCompleto.num_paginas,
                    tipo_impreso = modeloCompleto.tipo_impreso,
                    no_pag_color = modeloCompleto.no_pag_color,
                    departamento_impresion = modeloCompleto.departamento_impresion,
                    contenido_portada = modeloCompleto.contenido_portada,
                    cts_num_impresiones = modeloCompleto.cts_num_impresiones,
                    cant_pliegos_1 = modeloCompleto.cant_pliegos_1,
                    cant_pliegos_2 = modeloCompleto.cant_pliegos_2,
                    cant_pliegos_3 = modeloCompleto.cant_pliegos_3
                };

                HttpResponseMessage tecnicaResponse = await api.PostAsync("api/Especificaciones_Tecnicas", CreateJsonContent(tecnica));
                if (!tecnicaResponse.IsSuccessStatusCode) return HandleCreationError("Especificaciones Técnicas", tecnicaResponse, "No se pudo guardar la Técnica.");

                idTecnica = int.Parse(await tecnicaResponse.Content.ReadAsStringAsync());
                modeloCompleto.id_tecnica = idTecnica;


                // --- 5. GUARDAR CONTENIDO IMPRESIÓN (Requiere idTecnica) ---
                var contenidoData = new
                {
                    id_pedido= idPedido,
                    tipo_impresion= modeloCompleto.tipo_impreso,
                    tipo_papel= modeloCompleto.tipo_papel,
                    tamano_corte= modeloCompleto.tamano_corte,
                    salen_tc= modeloCompleto.salen_tc
                };
                HttpResponseMessage contenidoResponse = await api.PostAsync("Contenido_Impresion/Guardar", CreateJsonContent(contenidoData));

                if (!contenidoResponse.IsSuccessStatusCode) return HandleCreationError("5. Contenido Impresión", contenidoResponse, "No se pudo guardar el Contenido de Impresión.");


                // --- 6. GUARDAR PORTADA (Requiere idTecnica) ---
                var portadaData = new
                {
                    id_pedido= idPedido,
                    tipo_impresion = modeloCompleto.tipo_impresion_portada,
                    tipo_papel = modeloCompleto.tipo_papel_portada,
                    tamano_corte= modeloCompleto.tamano_corte_portada,
                    salen_tc= modeloCompleto.salen_tc_portada,
                };
                HttpResponseMessage portadaResponse = await api.PostAsync("api/Portada", CreateJsonContent(portadaData));

                if (!portadaResponse.IsSuccessStatusCode) return HandleCreationError("6. Portada", portadaResponse, "No se pudo guardar la Portada.");


                // --- 7. GUARDAR ACABADOS FINALES (Requiere idPedido) ---
                var acabadoData = new
                {
                    id_pedido = idPedido, // FK del Pedido
                    cant_copias = modeloCompleto.cant_copias,
                    tipos = modeloCompleto.tipos,
                    otro = modeloCompleto.otro,
                    tecnologia_impresion_especial = modeloCompleto.tecnologia_impresion_especial,
                };
                HttpResponseMessage acabadoResponse = await api.PostAsync("Acabado_Final/Guardar", CreateJsonContent(acabadoData));

                if (!acabadoResponse.IsSuccessStatusCode) return HandleCreationError("7. Acabados Finales", acabadoResponse, "No se pudo guardar los Acabados Finales.");


                // --- GUARDAR CUENTAS CONTABLES ---
                bool cuentasOk = await GuardarCuentasContables(modeloCompleto);
                if (!cuentasOk)
                {
                    ViewBag.Mensaje = "Error al guardar las cuentas contables. Verifique los datos.";
                    return View("~/Views/Editoriales/Fichas/FichaEditorial.cshtml", modeloCompleto);
                }

                // --- GUARDAR EJEMPLARES DE COSTOS ---
                bool ejemplaresOk = await GuardarEjemplaresCostos(modeloCompleto);
                if (!ejemplaresOk)
                {
                    ViewBag.Mensaje = "Error al guardar los ejemplares de costos. Verifique los datos.";
                    return View("~/Views/Editoriales/Fichas/FichaEditorial.cshtml", modeloCompleto);
                }

                // --- TODO GUARDADO CORRECTO ---
                // Mostramos mensaje de éxito en la misma pantalla
                ViewBag.MensajeExito = "¡La Solicitud de Publicación fue guardada exitosamente!";
                ViewBag.Modo = "Ver"; // opcional: bloquear edición si quieres solo lectura
                return View("~/Views/Editoriales/Fichas/FichaEditorial.cshtml", modeloCompleto);

            }
            catch (Exception ex)
            {
                ViewBag.Mensaje = $"Error inesperado durante la creación: {ex.Message}";
                return View("~/Views/Editoriales/Fichas/FichaEditorial.cshtml", modeloCompleto);
            }
        }


        // =========================================================
        // 7. LÓGICA DE ACTUALIZACIÓN (PUT) - (Encadenamiento de 10 PUTs por entidad)
        // =========================================================
        private async Task<IActionResult> ActualizarFichaCompleta(FichaCompletaViewModel modeloCompleto)
        {
            // Función auxiliar para manejar errores y retornar la vista
            IActionResult HandleUpdateError(string step, HttpResponseMessage? response, Exception? ex = null)
            {
                string errorDetail = "";
                if (response != null)
                {
                    // Intenta leer el contenido de error de la API si hay respuesta HTTP
                    errorDetail = response.Content.ReadAsStringAsync().Result;
                    TempData["MensajeError"] = $"Error al actualizar la ficha (Paso: **{step}**). Código HTTP: {(int)response.StatusCode}. Detalle API: {errorDetail}.";
                }
                else if (ex != null)
                {
                    // Manejo de excepciones
                    TempData["MensajeError"] = $"Error inesperado durante la actualización (Paso: **{step}**): {ex.InnerException?.Message ?? ex.Message}";
                }

                // Configuración para retornar a la vista de edición
                ViewBag.Modo = "Editar";
                ViewData["Title"] = "Editar Ficha de Presupuesto";
                return View("~/Views/Editoriales/Fichas/FichaEditorial.cshtml", modeloCompleto);
            }

            try
            {
                // 0. Validación de IDs mínimos
                if (modeloCompleto.id_pedido == 0 || modeloCompleto.id_propuesta == 0 || modeloCompleto.id_autor == 0)
                {
                    throw new Exception("Error de edición: Faltan los IDs clave (Pedido, Propuesta o Autor) para actualizar.");
                }

                // =========================================================
                // 1. ACTUALIZAR AUTOR (PUT api/Autor/{id})
                // =========================================================
                var autorUpdateData = new
                {
                    id_autor = modeloCompleto.id_autor,
                    nombre_apellidos = modeloCompleto.nombre_apellidos,
                    documento_identidad = modeloCompleto.documento_identidad,
                    nacionalidad = modeloCompleto.nacionalidad,
                    tipo_cedula = modeloCompleto.tipo_cedula,
                    estado_civil = modeloCompleto.estado_civil,
                    profesion = modeloCompleto.profesion,
                    correo_electronico = modeloCompleto.correo_electronico,
                };

                var autorPut = await api.PutAsJsonAsync($"api/Autor/{autorUpdateData.id_autor}", autorUpdateData);

                if (!autorPut.IsSuccessStatusCode)
                {
                    var error = await autorPut.Content.ReadAsStringAsync();
                    return BadRequest($"No se pudo actualizar la información del Autor. {error}");
                }

                // =========================================================
                // 2. ACTUALIZAR TELEFONOS DE AUTOR
                // =========================================================
                if (modeloCompleto.TelefonoAutor != null)
                {
                    foreach (var tel in modeloCompleto.TelefonoAutor)
                    {
                        var telefonoUpdateData = new
                        {
                            id_autor = modeloCompleto.id_autor,
                            telefono = tel.telefono
                        };

                        var telResponse = await api.PostAsJsonAsync("api/TelefonoAutor", telefonoUpdateData);
                        if (!telResponse.IsSuccessStatusCode)
                        {
                            var error = await telResponse.Content.ReadAsStringAsync();
                            return BadRequest($"No se pudo actualizar el teléfono del autor. {error}");
                        }
                    }
                }

                // =========================================================
                // 3. ACTUALIZAR DIRECCIONES DE AUTOR
                // =========================================================
                if (modeloCompleto.DireccionAutor != null)
                {
                    foreach (var dir in modeloCompleto.DireccionAutor)
                    {
                        var direccionUpdateData = new
                        {
                            id_autor = modeloCompleto.id_autor,
                            direccion = dir.direccion
                        };

                        var dirResponse = await api.PostAsJsonAsync("api/DireccionAutor", direccionUpdateData);
                        if (!dirResponse.IsSuccessStatusCode)
                        {
                            var error = await dirResponse.Content.ReadAsStringAsync();
                            return BadRequest($"No se pudo actualizar la dirección del autor. {error}");
                        }
                    }
                }


                // =========================================================
                // 2. ACTUALIZAR PROPUESTA (PUT api/Propuesta/{id})
                // =========================================================
                var propuestaEditorialData = new
                {
                    idPropuesta = modeloCompleto.idPropuesta, // FK del Autor
                    subtitulo = modeloCompleto.subtitulo,
                    titulo_obra = modeloCompleto.titulo_obra,
                    fechaCreacion = modeloCompleto.fechaCreacion,
                    serie_coleccion = modeloCompleto.serie_coleccion, // Usamos nombre de propiedad correcto
                    publico_meta = modeloCompleto.publico_meta,
                    palabras_claves = modeloCompleto.palabras_claves,
                };
                HttpResponseMessage propuestaPut = await api.PutAsJsonAsync($"Propuesta_Editorial/Actualizar", propuestaEditorialData);
                if (!propuestaPut.IsSuccessStatusCode) return HandleUpdateError("2. Propuesta", propuestaPut);


                // =========================================================
                // 3. ACTUALIZAR PEDIDO (PUT Pedidos/Actualizar)
                // =========================================================
                var pedidoUpdateData = new
                {
                    id_pedido = modeloCompleto.id_pedido,
                    id_propuesta = modeloCompleto.id_propuesta,
                    tipo_pedido = modeloCompleto.tipo_pedido,
                    fecha_creacion = modeloCompleto.fecha_creacion,
                    orden_servicio = modeloCompleto.orden_servicio,
                    titulo_trabajo = modeloCompleto.titulo_trabajo,
                    dependencia = modeloCompleto.dependencia,
                    funcionario = modeloCompleto.funcionario,
                    estado_pedido = modeloCompleto.estado_pedido,
                    correo_funcionario = modeloCompleto.correo_funcionario,
                    telefono_funcionario = modeloCompleto.telefono_funcionario,
                    fax = modeloCompleto.fax,
                    consentimiento_cliente = modeloCompleto.consentimiento_cliente,
                    sello = modeloCompleto.sello,
                    responsable_vb = modeloCompleto.responsable_vb,
                    observaciones_generales = modeloCompleto.observaciones_generales,
                    cant_cd = modeloCompleto.cant_cd,
                    campos_servicio_extra = modeloCompleto.campos_servicio_extra,
                    total = modeloCompleto.total,
                    fecha_actualizacion_pedido = DateTime.Now // Actualiza la fecha
                };
                HttpResponseMessage pedidoPut = await api.PutAsJsonAsync("Pedidos/Actualizar", pedidoUpdateData);
                if (!pedidoPut.IsSuccessStatusCode) return HandleUpdateError("3. Pedido", pedidoPut);


                // =========================================================
                // 4. ACTUALIZAR ESPECIFICACIONES TÉCNICAS (PUT EspecificacionTecnica/Actualizar)
                // Asume que usa id_tecnica como ID
                // =========================================================
                var tecnicaUpdateData = new
                {
                    id_tecnica = modeloCompleto.id_tecnica, // ID de la tabla
                    id_pedido = modeloCompleto.id_pedido, // FK
                    num_paginas = modeloCompleto.num_paginas,
                    tipo_impreso = modeloCompleto.tipo_impreso,
                    no_pag_color = modeloCompleto.no_pag_color,
                    departamento_impresion = modeloCompleto.departamento_impresion,
                    contenido_portada = modeloCompleto.contenido_portada,
                    cts_num_impresiones = modeloCompleto.cts_num_impresiones,
                    cant_pliegos_1 = modeloCompleto.cant_pliegos_1,
                    cant_pliegos_2 = modeloCompleto.cant_pliegos_2,
                    cant_pliegos_3 = modeloCompleto.cant_pliegos_3,
                };
                HttpResponseMessage tecnicaPut = await api.PutAsJsonAsync("api/Especificaciones_Tecnicas/Actualizar", tecnicaUpdateData);
                if (!tecnicaPut.IsSuccessStatusCode) return HandleUpdateError("4. Especificaciones Técnicas", tecnicaPut);


                // =========================================================
                // 5. ACTUALIZAR CONTENIDO IMPRESIÓN (PUT ContenidoImpresion/Actualizar)
                // Asume que id_tecnica es la clave
                // =========================================================
                var contenidoUpdateData = new
                {
                    id_contenido = modeloCompleto.id_contenido, // Usamos el ID de la tabla ContenidoImpresion
                    id_tecnica = modeloCompleto.id_tecnica, // FK
                    tipo_impresion_contenido = modeloCompleto.tipo_impresion_contenido,
                    tipo_papel = modeloCompleto.tipo_papel,
                    tamano_corte = modeloCompleto.tamano_corte,
                    salen_tc = modeloCompleto.salen_tc,
                };
                HttpResponseMessage contenidoPut = await api.PutAsJsonAsync("ContenidoImpresion/Actualizar", contenidoUpdateData);
                if (!contenidoPut.IsSuccessStatusCode) return HandleUpdateError("5. Contenido Impresión", contenidoPut);


                // =========================================================
                // 6. ACTUALIZAR PORTADA (PUT Portada/Actualizar)
                // Asume que id_tecnica es la clave
                // =========================================================
                var portadaUpdateData = new
                {
                    id_portada = modeloCompleto.id_portada, // Usamos el ID de la tabla Portada
                    id_tecnica = modeloCompleto.id_tecnica, // FK
                    tipo_impresion_portada = modeloCompleto.tipo_impresion_portada,
                    tipo_papel_portada = modeloCompleto.tipo_papel_portada,
                    tamano_corte_portada = modeloCompleto.tamano_corte_portada,
                    salen_tc_portada = modeloCompleto.salen_tc_portada,
                };
                HttpResponseMessage portadaPut = await api.PutAsJsonAsync("Portada/Actualizar", portadaUpdateData);
                if (!portadaPut.IsSuccessStatusCode) return HandleUpdateError("6. Portada", portadaPut);


                // =========================================================
                // 7. ACTUALIZAR ACABADOS FINALES (PUT AcabadosFinales/Actualizar)
                // Asume que id_acabado es la clave
                // =========================================================
                var acabadoUpdateData = new
                {
                    id_acabado = modeloCompleto.id_acabado, // Usamos el ID de la tabla AcabadosFinales
                    id_pedido = modeloCompleto.id_pedido, // FK
                    cant_copias = modeloCompleto.cant_copias,
                    tipos = modeloCompleto.tipos,
                    otro = modeloCompleto.otro,
                    tecnologia_impresion_especial = modeloCompleto.tecnologia_impresion_especial,
                };
                HttpResponseMessage acabadoPut = await api.PutAsJsonAsync("Acabado_Final/Actualizar", acabadoUpdateData);
                if (!acabadoPut.IsSuccessStatusCode) return HandleUpdateError("7. Acabados Finales", acabadoPut);


                // --- 8 y 9. GUARDAR CUENTAS CONTABLES Y DETALLES ---
                // con un metodo que esta al final de este archivo
                bool cuentasOk = await GuardarCuentasContables(modeloCompleto);

                if (!cuentasOk)
                {
                    // Si hubo error guardando cuentas, regresamos al formulario con mensaje
                    ViewBag.Mensaje = "Error al guardar las cuentas contables. Verifique los datos.";
                    ViewBag.Modo = modeloCompleto.id_pedido > 0 ? "Editar" : "Crear";
                    return View("~/Views/Editoriales/Fichas/FichaEditorial.cshtml", modeloCompleto);
                }

                // --- 10. Guardar Ejemplares de Costos ---
                bool ejemplaresOk = await GuardarEjemplaresCostos(modeloCompleto);

                if (!ejemplaresOk)
                {
                    ViewBag.Modo = modeloCompleto.id_pedido > 0 ? "Editar" : "Crear";
                    return View("~/Views/Editoriales/Fichas/FichaEditorial.cshtml", modeloCompleto);
                }


                // Éxito:
                TempData["MensajeExito"] = $"¡La ficha de presupuesto ID {modeloCompleto.id_pedido} fue **actualizada** exitosamente! (Orquestación de 10 PUTs)";
                return RedirectToAction(nameof(GestionFichasPresupuesto));
            }
            catch (Exception ex)
            {
                return HandleUpdateError("General/Conexión", null, ex);
            }
        }

        // =========================================================
        // GUARDAR / ACTUALIZAR SOLO DETALLES DE CUENTAS CONTABLES
        // =========================================================
        private async Task<bool> GuardarCuentasContables(FichaCompletaViewModel modeloCompleto)
        {
            try
            {
                if (modeloCompleto.detallesCuentasContables == null ||
                    !modeloCompleto.detallesCuentasContables.Any())
                {
                    // Nada que guardar
                    return true;
                }

                foreach (var detalle in modeloCompleto.detallesCuentasContables)
                {
                    // Si el id_cuenta llega en 0 → significa que la vista no lo envió bien
                    if (detalle.id_cuenta <= 0)
                        throw new Exception("El detalle recibido no contiene un id_cuenta válido.");

                    var detalleData = new
                    {
                        id_detalle = detalle.id_detalle,
                        id_pedido = modeloCompleto.id_pedido,
                        id_cuenta = detalle.id_cuenta,
                        costo_directo = detalle.costo_directo,
                        costo_indirecto = detalle.costo_indirecto,
                        libro_digital = detalle.libro_digital,
                        otros = detalle.otros
                    };

                    HttpResponseMessage response;

                    if (detalle.id_detalle > 0)
                    {
                        // UPDATE
                        response = await api.PutAsJsonAsync(
                            "Detalle_Cuentas_Contables/Actualizar", detalleData);
                    }
                    else
                    {
                        // CREATE
                        response = await api.PostAsJsonAsync(
                            "Detalle_Cuentas_Contables/Guardar", detalleData);
                    }

                    if (!response.IsSuccessStatusCode)
                        throw new Exception("Error al guardar un detalle de cuenta contable.");
                }

                return true;
            }
            catch (Exception ex)
            {
                ViewBag.Mensaje = $"Error al guardar cuentas contables: {ex.Message}";
                return false;
            }
        }


        // =========================================================
        // MÉTODO: Guardar o actualizar todos los Ejemplares de costos
        // =========================================================
        private async Task<bool> GuardarEjemplaresCostos(FichaCompletaViewModel modeloCompleto)
        {
            try
            {
                if (modeloCompleto.EjemplaresCostos == null || !modeloCompleto.EjemplaresCostos.Any())
                    return true; // No hay nada que guardar

                foreach (var ejemplar in modeloCompleto.EjemplaresCostos)
                {
                    var data = new
                    {
                        id_ejemplar = ejemplar.id_ejemplar, // 0 o null para creación
                        id_pedido = modeloCompleto.id_pedido, // FK al pedido
                        cantidad = ejemplar.cantidad,
                        costo = ejemplar.costo
                    };

                    HttpResponseMessage response;

                    if (ejemplar.id_ejemplar > 0)
                    {
                        // Actualizar
                        response = await api.PutAsJsonAsync("Ejemplar_Costo/Actualizar", data);
                    }
                    else
                    {
                        // Crear
                        response = await api.PostAsJsonAsync("Ejemplar_Costo/Guardar", data);
                    }

                    if (!response.IsSuccessStatusCode)
                    {
                        // Opcional: leer contenido del error
                        var errorContent = await response.Content.ReadAsStringAsync();
                        throw new Exception($"Error al guardar EjemplarCosto (ID {ejemplar.id_ejemplar}): {errorContent}");
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                // Manejo de error global
                ViewBag.Mensaje = $"Error al guardar ejemplares de costos: {ex.Message}";
                return false;
            }
        }


    }
}