# C# .NET Core 8.0 DevContainer

Configuración de devcontainer para workspaces de C# .NET Core 8.0 en Coder.

## Archivos Incluidos

- `Dockerfile` - Imagen base con .NET SDK 8.0 y herramientas preinstaladas
- `devcontainer.json` - Configuración de VS Code con extensiones para C#
- `post-create.sh` - Script de inicialización que configura Git y aliases
- `startup.sh` - Script de inicio que muestra el estado del ambiente

## Herramientas Preinstaladas

**Herramientas Globales .NET:**
- dotnet-ef (Entity Framework CLI)
- dotnet-aspnet-codegenerator
- dotnet-format
- dotnet-outdated-tool
- dotnet-trace, dotnet-counters, dotnet-dump

**Sistema:**
- Git
- curl, wget, unzip
- vim, nano, jq
- htop
- sudo

## Extensiones VSCode

- ms-dotnettools.csharp
- ms-dotnettools.vscode-dotnet-runtime
- formulahendry.dotnet-test-explorer
- fernandoescolar.vscode-solution-explorer
- jmrog.vscode-nuget-package-manager
- GitHub.copilot
- kreativ-software.csharpextensions

## Aliases Configurados

Después del post-create, estarán disponibles:
- `dn`, `dnb`, `dnr`, `dnt`, `dnw`, `dnc`, `dnrs`, `dnf`, `dno`, `dnef`
- Funciones: `create-webapi`, `create-mvc`, `create-console`

## Uso

Este devcontainer se usa automáticamente cuando creas un workspace con la plantilla `csharp-dotnet-core` en Coder.