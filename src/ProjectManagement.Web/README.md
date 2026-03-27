# Assesment Técnico - Sistema de Gestión de Cursos

Este proyecto es una aplicación web full-stack para la gestión de cursos y lecciones, construida con .NET 8 y Blazor, siguiendo una arquitectura limpia y desacoplada.

## Descripción del Código

La solución implementa un sistema CRUD (Crear, Leer, Actualizar, Eliminar) para Cursos y Lecciones, aplicando una serie de reglas de negocio en el backend para garantizar la integridad de los datos.

### Características Principales
- **Gestión de Cursos**: Crear, editar, listar y eliminar (soft delete) cursos.
- **Gestión de Lecciones**: Crear, editar, eliminar (soft delete) y reordenar lecciones dentro de un curso.
- **Publicación**: Publicar y despublicar cursos, con reglas de negocio que impiden publicar un curso sin lecciones.
- **Autenticación**: Sistema de registro e inicio de sesión basado en tokens JWT (JSON Web Tokens).
- **API RESTful**: Un backend que expone todos los endpoints necesarios para la gestión de datos.
- **Documentación de API**: Interfaz de Swagger para visualizar y probar los endpoints de la API.

---

## Arquitectura y Frameworks

La solución está dividida en tres proyectos para promover el desacoplamiento, la escalabilidad y la mantenibilidad.

1.  **`Assesment_tecnico.Api`**:
    -   **Descripción**: Es el proyecto de backend, una **API RESTful** construida con **ASP.NET Core Web API**.
    -   **Responsabilidades**: Maneja toda la lógica de negocio, el acceso a la base de datos (a través de Entity Framework Core), la autenticación de usuarios y la generación de tokens JWT.
    -   **Frameworks/Librerías**: .NET 8, ASP.NET Core, Entity Framework Core, Npgsql (PostgreSQL Driver), Swashbuckle (Swagger), Microsoft.Identity.

2.  **`Assesment_tecnico.WebApp`**:
    -   **Descripción**: Es el proyecto de frontend, una aplicación web construida con **Blazor Server**.
    -   **Responsabilidades**: Renderiza la interfaz de usuario, gestiona la interacción con el usuario y consume los datos exclusivamente a través de llamadas HTTP a `Assesment_tecnico.Api`. No tiene acceso directo a la base de datos.
    -   **Frameworks/Librerías**: .NET 8, Blazor Server, Blazored.LocalStorage.

3.  **`Assesment_tecnico.Shared`**:
    -   **Descripción**: Es una **biblioteca de clases** compartida.
    -   **Responsabilidades**: Contiene los modelos de datos y los DTOs (Data Transfer Objects) que tanto la API como la WebApp utilizan para comunicarse. Sirve como un "contrato" de datos entre el frontend y el backend, evitando la duplicación de código.

---

## Guía de Instalación y Ejecución

### 1. Configurar la Base de Datos

-   **Requisito**: Tener una instancia de **PostgreSQL** en ejecución.
-   **Configuración**: La cadena de conexión se encuentra en el archivo `Assesment_tecnico.Api/appsettings.json`. La configuración por defecto es:
    ```json
    "ConnectionStrings": {
      "DefaultConnection": "Host=localhost;Port=5433;Database=coderdb;Username=Coder;Password=Qwe.123"
    }
    ```
-   Asegúrate de que la base de datos (`coderdb` por defecto) y el usuario (`Coder`) existan en tu instancia de PostgreSQL. Las tablas serán creadas automáticamente por las migraciones.

### 2. Ejecutar Migraciones

Las migraciones se aplican automáticamente cada vez que se inicia el proyecto de la API. Sin embargo, si necesitas crearlas o aplicarlas manualmente, sigue estos pasos:

```bash
# Navegar a la carpeta del proyecto de la API
cd Assesment_tecnico.Api

# (Opcional) Crear una nueva migración si has cambiado los modelos
dotnet ef migrations add <NombreDeLaMigracion>

# Aplicar las migraciones a la base de datos
dotnet ef database update
```

### 3. Levantar la API y el Frontend

Debes tener dos terminales abiertas para ejecutar ambos proyectos simultáneamente.

-   **Terminal 1: Iniciar la API**
    ```bash
    cd Assesment_tecnico.Api
    dotnet run
    ```
    La API se iniciará y escuchará en **`http://localhost:5001`**.
    Puedes acceder a la interfaz de Swagger en **`http://localhost:5001/swagger`**.

-   **Terminal 2: Iniciar el Frontend**
    ```bash
    cd Assesment_tecnico.WebApp
    dotnet run
    ```
    La aplicación web se iniciará y escuchará en **`http://localhost:5002`**.

-   **Acceso**: Abre tu navegador y navega a **`http://localhost:5002`** para usar la aplicación.

### 4. Credenciales del Usuario de Prueba

La base de datos se inicializa automáticamente con un usuario de prueba si no existe ninguno.

-   **Email / Username**: `admin@example.com`
-   **Password**: `Password123!`

---

## Testing

La solución incluye un proyecto de tests unitarios (`Assesment_tecnico.Tests`) para validar las reglas de negocio clave.

### Cómo Ejecutar los Tests

1.  Abre una terminal en la raíz del proyecto (la carpeta que contiene el archivo `.sln`).
2.  Ejecuta el siguiente comando:

    ```bash
    dotnet test
    ```

Este comando descubrirá, compilará y ejecutará todos los tests de la solución, mostrando los resultados en la terminal.

---

## Repositorio de GitHub

Puedes encontrar el código fuente de este proyecto en el siguiente repositorio:
[https://github.com/synfony/Cursos-online.git](https://github.com/synfony/Cursos-online.git)
