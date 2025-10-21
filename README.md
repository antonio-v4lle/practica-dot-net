# Proyecto Base .NET Core 8.0

Este es un proyecto base mínimo para desarrollo con .NET Core 8.0 en Coder.

## Estructura del Proyecto

```
.
├── .devcontainer/          # Configuración del devcontainer
│   ├── Dockerfile
│   ├── devcontainer.json
│   ├── post-create.sh
│   └── startup.sh
├── src/                    # Código fuente de la aplicación
│   └── Program.cs
├── .gitignore              # Archivos a ignorar en Git
└── README.md               # Este archivo
```

## Inicio Rápido

### 1. Ejecutar la aplicación

```bash
dotnet run --project src/HelloWorld.csproj
```

### 2. Crear un nuevo proyecto

Puedes usar las funciones helper incluidas:

```bash
# Crear un proyecto Web API
create-webapi MiApi

# Crear un proyecto MVC
create-mvc MiWebApp

# Crear un proyecto Console
create-console MiConsola
```

### 3. Comandos útiles

```bash
# Restaurar dependencias
dotnet restore

# Compilar el proyecto
dotnet build

# Ejecutar tests (cuando los agregues)
dotnet test

# Desarrollo con hot reload
dotnet watch run --project src/HelloWorld.csproj
```

## Estructura Recomendada para Proyectos Grandes

```
.
├── src/
│   ├── MiApp.Api/              # Proyecto Web API
│   ├── MiApp.Core/             # Lógica de negocio
│   ├── MiApp.Infrastructure/   # Acceso a datos, servicios externos
│   └── MiApp.Shared/           # Modelos compartidos
├── tests/
│   ├── MiApp.Api.Tests/
│   ├── MiApp.Core.Tests/
│   └── MiApp.Integration.Tests/
├── docs/                       # Documentación
├── .devcontainer/
├── .gitignore
└── MiApp.sln                   # Solution file
```

## Crear una Solution

Para proyectos más complejos, es recomendable usar una solution:

```bash
# Crear solution
dotnet new sln -n MiApp

# Crear proyectos
dotnet new webapi -o src/MiApp.Api
dotnet new classlib -o src/MiApp.Core
dotnet new xunit -o tests/MiApp.Tests

# Agregar proyectos a la solution
dotnet sln add src/MiApp.Api/MiApp.Api.csproj
dotnet sln add src/MiApp.Core/MiApp.Core.csproj
dotnet sln add tests/MiApp.Tests/MiApp.Tests.csproj

# Agregar referencias entre proyectos
dotnet add src/MiApp.Api reference src/MiApp.Core
dotnet add tests/MiApp.Tests reference src/MiApp.Api
dotnet add tests/MiApp.Tests reference src/MiApp.Core
```

## Herramientas Disponibles

El workspace incluye las siguientes herramientas globales:

- **dotnet-ef** - Entity Framework Core CLI
- **dotnet-aspnet-codegenerator** - Generador de código ASP.NET
- **dotnet-format** - Formateador de código
- **dotnet-outdated** - Detector de paquetes desactualizados
- **dotnet-trace** - Herramienta de diagnóstico
- **dotnet-counters** - Monitoreo de performance
- **dotnet-dump** - Análisis de dumps

## Aliases Configurados

El ambiente incluye estos aliases útiles:

- `dn` → `dotnet`
- `dnb` → `dotnet build`
- `dnr` → `dotnet run`
- `dnt` → `dotnet test`
- `dnw` → `dotnet watch run`
- `dnc` → `dotnet clean`
- `dnrs` → `dotnet restore`
- `dnf` → `dotnet format`
- `dno` → `dotnet outdated`
- `dnef` → `dotnet ef`

## Siguientes Pasos

1. **Reemplazar este proyecto base** con tu código existente, o
2. **Crear un nuevo proyecto** usando los comandos dotnet new
3. **Configurar tu base de datos** si usas Entity Framework
4. **Agregar tests** para tu código

## Recursos

- [Documentación de .NET](https://learn.microsoft.com/en-us/dotnet/)
- [ASP.NET Core](https://learn.microsoft.com/en-us/aspnet/core/)
- [Entity Framework Core](https://learn.microsoft.com/en-us/ef/core/)
- [C# Language Reference](https://learn.microsoft.com/en-us/dotnet/csharp/)
