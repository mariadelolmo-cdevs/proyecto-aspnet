# Proyecto base de ASP.NET

Brought to you by: La Grajilla 🐦‍⬛

## Cómo crear un proyecto desde cero

Los comandos se escriben en la terminal de VS Code (menú Terminal → New Terminal). Es PowerShell. Se ejecutan uno a uno, en orden.

### Extensiones de VS Code

Instala estas dos extensiones desde el panel de extensiones (Ctrl + Shift + X):

- **C# Dev Kit** de Microsoft: soporte de C# y .NET en VS Code.
- **SQLite Viewer** de Florian Klampfer: permite abrir y ver el contenido de la base de datos `.db`.

### 1. Situarse en la carpeta superior

```powershell
cd C:\dev
```

Nos situamos en la carpeta donde guardamos los proyectos.

### 2. Crear el proyecto

```powershell
dotnet new mvc -n proyecto
```

Crea la carpeta `C:\dev\proyecto` con una web MVC lista para usar. `-n proyecto` es el nombre del proyecto.

Este comando se ejecuta desde `C:\dev`, no desde dentro de `proyecto`. Si se ejecuta dentro, se crea una carpeta `proyecto\proyecto` por duplicado.

Trae de serie `Controllers`, `Models`, `Views`, `wwwroot` (con Bootstrap), `Program.cs` y `appsettings.json`.

### 3. Entrar en la carpeta

```powershell
cd proyecto
```

Desde aquí en adelante, todos los comandos se ejecutan dentro de `C:\dev\proyecto`.

### 4. Instalar Entity Framework Core

```powershell
dotnet add package Microsoft.EntityFrameworkCore.Sqlite
```

Instala EF Core, la librería que guarda y lee datos de la base de datos, y el proveedor de SQLite, que hace que la base de datos sea un archivo `.db`.

```powershell
dotnet add package Microsoft.EntityFrameworkCore.Design
```

Instala lo que EF Core necesita para crear las migraciones (los cambios de la base de datos).

### 5. Instalar dotnet-ef

```powershell
dotnet new tool-manifest
```

Crea un archivo `.config/dotnet-tools.json` que guarda la lista de herramientas que usa este proyecto.

```powershell
dotnet tool install dotnet-ef
```

Instala `dotnet-ef`, la herramienta que ejecuta los comandos `dotnet ef` (migraciones), y la apunta en ese archivo.

Para comprobar que está instalada:

```powershell
dotnet ef --version
```

Si sale un número de versión, está bien.

### 6. Probar que arranca

```powershell
dotnet run
```

Compila el proyecto y arranca el servidor. Saldrá algo como `Now listening on: http://localhost:5123`. Abre esa dirección en el navegador.

Para parar el servidor: `Ctrl + C` en la terminal.

### 7. Abrir en VS Code

```powershell
code .
```

Abre la carpeta del proyecto en VS Code. Si pregunta si confías en los autores, dale a que sí.

---

## Configurar el proyecto

La mayoría de estos pasos se hacen editando archivos.

### 8. Cadena de conexión

Crea la carpeta `App_Data` en la raíz del proyecto.

En `appsettings.json`, añade esto al principio, justo antes de `"Logging"`:

```json
  "ConnectionStrings": {
    "DefaultConnection": "Data Source=App_Data/database.db"
  },
```

Dice dónde está la base de datos. Se guarda en `App_Data/database.db`, dentro de la carpeta del proyecto. Si quieres, puedes cambiar el nombre del archivo `database.db`.

### 9. DbContext

Crea la carpeta `Data` y dentro el archivo `ApplicationDbContext.cs`:

```csharp
using Microsoft.EntityFrameworkCore;

namespace proyecto.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    // TABLAS
    
}
```

Es la clase que conecta el código con la base de datos. Las tablas se añadirán donde está el comentario `// TABLAS`.

### 10. DbInitializer

Crea `Data/DbInitializer.cs`:

```csharp
using Microsoft.EntityFrameworkCore;
using proyecto.Models;

namespace proyecto.Data;

public static class DbInitializer
{
    public static void Initialize(IServiceProvider services)
    {
        var context = services.GetRequiredService<ApplicationDbContext>();

        // aplica  migraciones pendientes al arrancar la app
        context.Database.Migrate();

        // DATOS DEMO
        
    }
}
```

Se ejecuta al arrancar la app. `Migrate()` crea o actualiza la base de datos con las migraciones. Los datos de prueba irán debajo de `// DATOS DEMO`.

### 11. Program.cs

Hay que hacer cuatro cambios en `Program.cs`.

**1)** Los `using` van arriba del todo:

```csharp
using System.Globalization;
using Microsoft.EntityFrameworkCore;
using proyecto.Data;
```

**2)** Justo después de `var builder = WebApplication.CreateBuilder(args);`:

```csharp
CultureInfo.DefaultThreadCurrentCulture = CultureInfo.InvariantCulture;
CultureInfo.DefaultThreadCurrentUICulture = CultureInfo.InvariantCulture;
```

Hace que los decimales usen punto en cualquier equipo, así no hay problemas con los precios.

**3)** Antes de `var app = builder.Build();`:

```csharp
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));
```

Registra el DbContext y le dice qué base de datos usar.

**4)** Antes de `app.Run();`, al final del archivo:

```csharp
using (var scope = app.Services.CreateScope())
{
    DbInitializer.Initialize(scope.ServiceProvider);
}
```

Hace que `DbInitializer` se ejecute cada vez que arranca la app.

### 12. .gitignore

Añade un archivo `.gitignore` en la raíz del proyecto para que no se suban a git las carpetas `bin` y `obj` ni los archivos de base de datos (el `.db`).

### 13. Comprobar la base de datos

```powershell
dotnet run
```

Al arrancar, se crea el archivo `App_Data/database.db`. Si aparece, todo está bien. Para verlo, ábrelo con la extensión SQLite Viewer.

Para parar el servidor: `Ctrl + C` en la terminal.

---

## Subir el proyecto a git

Cuando todo funcione, guarda el proyecto en git. El `.gitignore` del paso 12 evita subir `bin`, `obj` y las bases de datos.

```powershell
git init
git add .
git commit -m "proyecto base"
```

- `git init`: crea el repositorio en la carpeta del proyecto.
- `git add .`: añade los archivos al repositorio, menos los que ignora el `.gitignore`.
- `git commit`: guarda la primera versión del proyecto.
