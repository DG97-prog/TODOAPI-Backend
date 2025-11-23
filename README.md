# Todo App – Backend

## Descripción General

La aplicación **Todo App** es un gestor de tareas que permite crear, actualizar y cambiar el estado de las tareas a:

- Pendiente  
- En Progreso  
- Completado  

Al asignar una tarea, el sistema envía automáticamente una **notificación por correo** al usuario.  
Los usuarios con rol **Supervisor** pueden generar **informes en Excel** con información detallada de las tareas.

La aplicación también incluye un módulo de **gestión de usuarios**, donde el Administrador puede crear, actualizar y eliminar usuarios con acceso al sistema.

---

## Características Principales

- ✔️ Creación, actualización y eliminación de tareas  
- ✔️ Notificación automática al asignar o actualizar tareas  
- ✔️ Actualización de estados: Pendiente, En Progreso y Completada  
- ✔️ Generación de informes Excel  
- ✔️ Sistema de autenticación (JWT)  
- ✔️ Logs y auditoría de cambios  
- ✔️ Documentación con Swagger  

---

##  Tecnologías Utilizadas

### 🔹 Backend  
- ASP.NET + C#  

### 🔹 Frontend  
- React + Vite  

### 🔹 Base de Datos  
- SQL Server  
- Supabase (Base de datos en la nube)

### 🔹 Otros  
- Notificaciones: SMTP  
- Autenticación: JSON Web Tokens (JWT)  
- Reportes: Excel (.xlsx)  
- Despliegue Frontend: Vercel  
- Despliegue Backend: Render  

---

## Arquitectura Utilizada

### Arquitectura Cliente–Servidor  
El frontend (cliente) realiza solicitudes HTTP al backend (servidor) para procesar y ejecutar acciones mediante las API.

### Arquitectura MVC (Model–View–Controller)
En el backend se emplea el patrón MVC:

- **Models:** representan los datos y entidades  
- **Views:** se encuentran en el frontend (basadas en componentes)  
- **Controllers:** gestionan las solicitudes HTTP  
- **DTOs:** encapsulan datos de entrada y salida  
- **Services:** contienen la lógica de negocio, JWT y notificación por correo  
- **API Services (frontend):** consumen las API expuestas  

---

## Patrones y Principios SOLID Aplicados

### 🔹 Patrones Utilizados
- **Singleton:** aplicado al contexto de acceso a la base de datos  
- **Inyección de Dependencias:** usada en controladores de tareas y cuentas  
- **Factory Method, Repository, Command:** no se implementaron por simplicidad, pero se recomiendan para futuras versiones

### 🔹 Principios SOLID

- **S – Single Responsibility:**  
  Servicios separados para notificaciones y generación de tokens.

- **I – Interface Segregation:**  
  Interfaces independientes para token y correo.

- **D – Dependency Inversion:**  
  Los controladores dependen de interfaces en lugar de implementaciones concretas.

---

## Instalación

### 🔹 1. Clonar el repositorio Backend


https://github.com/DG97-prog/TODOAPI-Backend.git


### 🔹 2. Abrir la solución en Visual Studio


### 🔹 3. Compilar el proyecto  
Esto instalará las dependencias requeridas.

### 🔹 4. Ejecutar la API  
Seleccionar un perfil de ejecución, por ejemplo:

- `http`  
- `TodoApp.http`

### 🔹 5. Acceder a Swagger  
Agregar `/swagger` a la URL del servidor.

### 🔹 6. Base de Datos  
Para probar debe:

- Tener acceso a la base de datos en la nube  
**o**
- Restaurar la base en SQL Server  

---

## Flujo del Sistema

### Administrador
- Crear usuarios  
- Actualizar usuarios  
- Eliminar usuarios  
- Crear sus propias tareas  

### Supervisor
- Crear tareas  
- Asignar tareas  
- Actualizar tareas  
- Eliminar tareas  
- Generar informes Excel:

Incluyen:
- Total de tareas por estado  
- Tareas por usuario  
- Fecha de creación  
- Fecha de vencimiento  

### Backend
- Envía correo automático cuando se crea o asigna una tarea  

---

## 📁 Estructura del Proyecto (Backend)

```plaintext
TodoApp.API/
│
├── Controllers/
│   ├── AccountController.cs
│   ├── AuthController.cs
│   └── TareasController.cs
│
├── data/
│   └── ApplicationDbContext.cs
│
├── DTOs/
│   ├── CreateTareaDto.cs
│   ├── LoginDto.cs
│   ├── RegisterDto.cs
│   ├── UpdateTareaDto.cs
│   └── UpdateUsuarioDto.cs
│
├── Interfaces/
│   ├── IAuthService.cs
│   └── IEmailService.cs
│
├── models/
│   ├── Categoria.cs
│   ├── Estado.cs
│   ├── Tarea.cs
│   └── Usuario.cs
│
├── Services/
│   ├── AuthService.cs
│   └── EmailService.cs
│
└── Properties/
