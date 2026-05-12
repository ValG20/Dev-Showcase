using APIBitacoras.Model;
using Microsoft.EntityFrameworkCore;
using Sistema_Editorial.Model.Valeria.Impresion;
using Sistema_Editorial.Model.Valeria.Reportes;
using Sistema_EditorialS.Model.Valeria.Reportes;
using Sistema_Editorial.Model;
using APIEditorialUCR.Models;

namespace APIEditorialUCR.Model
{
    public class DbContextEditorial : DbContext
    {
        public DbContextEditorial(
          DbContextOptions<DbContextEditorial> options) : base(options)
        {
        }

        public DbSet<Propuesta> Propuestas { get; set; }
        public DbSet<Especificaciones_Tecnicas> Especificaciones_Tecnicas { get; set; }
        public DbSet<Historial_Cambios_Propuesta> Historial_Cambios_Propuesta { get; set; }
        public DbSet<Portada> Portadas { get; set; }
        public DbSet<Correccion_Retroalimentacion> Correccion_Retroalimentaciones { get; set; }
        public DbSet<Acta_Propuesta> Acta_Propuesta { get; set; }
        public DbSet<Actas_Sesion> Actas_Sesion { get; set; }
        public DbSet<Autor_Propuesta> Autor_Propuesta { get; set; }
        public DbSet<Propuesta_Editorial> Propuesta_Editoriales { get; set; }
        public DbSet<Pedido> Pedidos { get; set; }
        public DbSet<Diseno> Disenos { get; set; }
        public DbSet<Movimiento_inventario> Movimientos_inventarios { get; set; }
        public DbSet<Material> Materiales { get; set; }
        public DbSet<Servicio_impresion> Servicio_Impresion { get; set; }
        public DbSet<Archivo_manuscrito> Archivo_Manuscrito { get; set; }
        public DbSet<Archivo_disenno> Archivos_Disenos { get; set; }
        public DbSet<Manuscrito> Manuscritos { get; set; }
        public DbSet<Detalle_Costo> Detalles_Costos { get; set; }
        public DbSet<Acabado_Final> Acabados_Finales { get; set; }
        public DbSet<Contenido_Impresion> Contenido_Impresion { get; set; }
        public DbSet<Ejemplar_Costo> Ejemplares_Costos { get; set; }
        public DbSet<Detalle_Reporte> Detalles_Reportes { get; set; }
        public DbSet<Reporte> Reportes { get; set; }
        public DbSet<Archivo_adjunto> Archivos_Adjuntos_Propuesta { get; set; }
        public DbSet<Cuentas_Contables> Cuentas_Contables { get; set; }
        public DbSet<Detalle_Cuentas_Contables> Detalle_Cuentas_Contables { get; set; }
        public DbSet<Autor> Autores { get; set; }
        public DbSet<Portada> Portada { get; set; }
        public DbSet<Telefono_Autor> Telefono_Autor { get; set; }
        public DbSet<Direccion_Autor> Direccion_Autor { get; set; }
        public DbSet<Bitacora> Bitacora { get; set; }
        public DbSet<Historial_Precios> Historial_Precios { get; set; }
        public DbSet<Presupuestos> Presupuestos { get; set; }
        public DbSet<Propuesta_Editorial> Propuesta_Editorial { get; set; }
        public DbSet<Producto> Productos { get; set; }
    }
}
