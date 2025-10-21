#!/bin/bash
set -e

echo "🚀 Iniciando ambiente de desarrollo .NET..."

# Configurar PATH para herramientas .NET
export PATH=/home/coder/.dotnet/tools:/usr/local/bin:$PATH

# Mostrar estado del ambiente
echo ""
echo "📦 Herramientas disponibles:"
command -v dotnet >/dev/null 2>&1 && echo "  ✅ .NET SDK: $(dotnet --version)" || echo "  ❌ .NET SDK no disponible"
command -v git >/dev/null 2>&1 && echo "  ✅ Git: $(git --version)" || echo "  ❌ Git no disponible"

# Detectar y mostrar información del proyecto .NET
if ls *.csproj >/dev/null 2>&1 || ls *.fsproj >/dev/null 2>&1 || ls *.sln >/dev/null 2>&1 || \
   ls src/*.csproj >/dev/null 2>&1 || ls src/*/*.csproj >/dev/null 2>&1; then
    echo ""
    echo "📋 Proyecto .NET detectado"
    echo "💡 Comandos disponibles:"
    echo "  dotnet restore  # Restaurar dependencias"
    echo "  dotnet build    # Compilar proyecto"
    echo "  dotnet run      # Ejecutar proyecto"
    echo "  dotnet test     # Ejecutar pruebas"
    echo "  dotnet watch    # Desarrollo con recarga automática"

    # Verificar si es una aplicación web
    if find . -name "*.csproj" -not -path "*/obj/*" -not -path "*/bin/*" -exec grep -q "Microsoft.AspNetCore.App" {} \; 2>/dev/null; then
        echo ""
        echo "🌐 Aplicación Web ASP.NET Core detectada"
        echo "💡 La aplicación estará disponible en:"
        echo "  http://localhost:5000"
        echo "  https://localhost:5001"
    fi

    # Mostrar ubicación del proyecto si está en src/
    if ls src/*.csproj >/dev/null 2>&1; then
        echo ""
        echo "📁 Proyecto ubicado en: src/"
        PROJECT_NAME=$(ls src/*.csproj | head -n 1 | xargs basename -s .csproj)
        echo "💡 Ejecutar: dotnet run --project src/$PROJECT_NAME.csproj"
    fi
fi

echo ""
echo "✨ Ambiente .NET listo para usar"
echo "📁 Directorio actual: $(pwd)"