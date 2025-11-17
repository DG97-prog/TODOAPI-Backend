# TodoApp.API

API RESTful para la gestión de tareas con autenticación basada en JWT y almacenamiento en SQL Server, desarrollada con ASP.NET Core y Entity Framework Core.

---

## Descripción general

Este proyecto es una API que permite gestionar una lista de tareas (Todo App). Los usuarios pueden registrarse, autenticarse mediante JWT (JSON Web Tokens) y realizar operaciones CRUD (Crear, Leer, Actualizar, Eliminar) sobre tareas asociadas a su cuenta. 

La API implementa seguridad mediante autenticación JWT y validación de tokens para proteger los endpoints, garantizando que sólo usuarios autenticados puedan acceder y manipular sus tareas.

---

## Tecnologías y herramientas usadas

- **.NET 7** y **ASP.NET Core Web API**
- **Entity Framework Core** con SQL Server para la persistencia de datos
- **JWT (JSON Web Tokens)** para autenticación y autorización
- **Swagger / OpenAPI** para documentación y pruebas de la API
- **Visual Studio Code** como editor de código
- **Git y GitHub** para control de versiones y repositorio remoto

---

## Arquitectura del proyecto

El proyecto sigue una estructura limpia y organizada, dividiendo responsabilidades en capas y componentes:

- **Data (DataContext y Entidades)**: Clases que representan el modelo de datos y la configuración de la base de datos usando Entity Framework Core.
- **Interfaces**: Definición de contratos para los servicios (por ejemplo, `IAuthService`) que facilitan la inyección de dependencias y la abstracción.
- **Servicios**: Implementaciones de la lógica de negocio, como autenticación, creación y manejo de tareas.
- **Controladores (Controllers)**: Endpoints HTTP que exponen las funcionalidades a través de rutas RESTful.
- **Configuración de Seguridad**: Implementación de autenticación y autorización usando JWT.
- **Swagger**: Herramienta para documentar y probar fácilmente la API.

---

## Configuración

### Cadena de conexión a la base de datos

El archivo `appsettings.json` debe incluir la cadena de conexión a tu base de datos SQL Server:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=TU_SERVIDOR;Database=TodoAppDb;Trusted_Connection=True;"
  },
  "Jwt": {
    "Key": "TU_CLAVE_SECRETA_MUY_LARGA_Y_SEGURA",
    "Issuer": "TodoAppAPI",
    "Audience": "TodoAppClient"
  }
}
