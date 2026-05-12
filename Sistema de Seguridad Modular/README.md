# Sistema de Seguridad Modular

Este proyecto consiste en una solución de seguridad desacoplada diseñada para ser integrada en múltiples aplicaciones de forma modular. Proporciona un control exhaustivo de identidades, accesos y permisos sin depender de la base de datos principal de la aplicación cliente.

## Características Principales

* **Arquitectura Modular:** Diseñado como un componente independiente que se puede acoplar a diversos proyectos de software, permitiendo una integración rápida de capas de seguridad.
* **Gestión Centralizada :** Implementación de control de acceso basado en roles (Role-Based Access Control) para administrar usuarios, roles y permisos específicos.
* **Control de Interfaz:** Capacidad de gestionar dinámicamente el acceso a pantallas y funcionalidades del frontend según el perfil del usuario.
* **Independencia de Datos:** Utiliza una base de datos **Oracle** dedicada, separando la información de seguridad de la lógica de negocio de las aplicaciones conectadas.

## Tecnologías

* **Backend:** ASP.NET Core (API RESTful).
* **Base de Datos:** Oracle Database.
* **Persistencia de Datos (Oracle Database):** 
    La conexión con el motor de base de datos **Oracle** se gestiona de manera eficiente para asegurar la integridad de los perfiles de usuario y las reglas de acceso centralizadas.
    * *Infraestructura:* Conexión mediante protocolos TCP estandarizados.
  
## Seguridad y Autenticación

El sistema implementa un esquema de seguridad basado en **JWT (JSON Web Tokens)**, lo que permite:
* **Autenticación Stateless:** El servidor no necesita guardar sesiones, mejorando el rendimiento y permitiendo que la API sea consumida por múltiples clientes (Web, Móvil, Desktop).
* **Autorización por Claims:** Uso de roles y permisos en el token para el control de acceso a pantallas y endpoints.

## Ventajas del Diseño

Al mantener la seguridad en una base de datos Oracle independiente, el sistema garantiza:
1.  **Escalabilidad:** Se pueden conectar múltiples aplicaciones al mismo motor de seguridad.
2.  **Seguridad:** Aislamiento de las credenciales de usuario de los datos transaccionales.
3.  **Mantenibilidad:** Los cambios en las reglas de acceso se gestionan desde un solo lugar para todo el ecosistema de la empresa.

---
> **Nota:** Este repositorio es de carácter informativo. El código fuente permanece privado por motivos de seguridad y confidencialidad. Se incluyen diagramas de arquitectura en la carpeta `/docs`.
