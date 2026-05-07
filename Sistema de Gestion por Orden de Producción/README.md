# Sistema de Gestión por Orden de Producción

Este proyecto es una solución integral diseñada para optimizar y controlar el flujo de producción dentro de un entorno empresarial. Permite la trazabilidad completa desde la creación de una orden hasta su cumplimiento final.

## Tecnologías 

* **Lenguaje:** C# (.NET Core)
* **Base de Datos:** SQL Server 
* **Arquitectura:** (Model-View-Controller) para una separación clara entre la interfaz y la lógica.
* **Herramientas:** Entity Framework, Microsoft Visual Studio.

## Funcionalidades Clave

* **Gestión de Órdenes:** Creación, edición y seguimiento del estado de las órdenes de producción en tiempo real.
* **Control de Bitácora:** Registro automatizado de movimientos y cambios de estado para asegurar la auditoría de procesos.
* **Interfaz Intuitiva:** Diseño enfocado en la eficiencia del usuario operativo para reducir errores de entrada de datos.
* **Reportes:** Generación de resúmenes de producción para la toma de decisiones gerenciales.

## Detalles Técnicos

Para este sistema, se implementó una lógica de **validación de estados** que impide que una orden avance sin cumplir con los requisitos previos, asegurando la integridad del proceso de negocio. La base de datos fue normalizada hasta la **3ra Forma Normal** para garantizar un rendimiento óptimo y evitar redundancia.

---
> **Nota:** Este repositorio contiene únicamente la documentación técnica y capturas de pantalla del funcionamiento del sistema. El código fuente es privado por motivos de confidencialidad académica/profesional.
