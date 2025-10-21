#!/bin/bash
set -e

echo "🔧 Ejecutando configuración post-creación para .NET Core/8.0..."

# Configurar Git si no está configurado
if [ -z "$(git config --global user.email)" ]; then
    echo "📝 Configurando Git..."
    git config --global user.email "${GIT_AUTHOR_EMAIL:-coder@example.com}"
    git config --global user.name "${GIT_AUTHOR_NAME:-Coder}"
    git config --global init.defaultBranch main
    git config --global pull.rebase false
fi

# Verificar si es un proyecto existente
if ls *.sln >/dev/null 2>&1; then
    echo "📦 Solution .NET detectada, ejecutando restore..."
    dotnet restore
elif ls *.csproj >/dev/null 2>&1; then
    echo "📦 Proyecto .NET detectado en raíz, ejecutando restore..."
    dotnet restore
elif ls src/*.csproj >/dev/null 2>&1 || ls src/*/*.csproj >/dev/null 2>&1; then
    echo "📦 Proyecto .NET detectado en src/, ejecutando restore..."
    find . -name "*.csproj" -not -path "*/obj/*" -not -path "*/bin/*" -exec dotnet restore {} \; 2>/dev/null || true
else
    echo "ℹ️  No se detectó proyecto .NET existente (este es el proyecto base de ejemplo)"
fi

# Configurar alias y variables de entorno en .bashrc (al final para que estén disponibles en shells nuevos)
if [ -f "$HOME/.bashrc" ]; then
    SHELL_RC="$HOME/.bashrc"
else
    SHELL_RC="$HOME/.profile"
fi

# Verificar si los alias ya existen para evitar duplicados
if ! grep -q "# .NET environment variables" "$SHELL_RC" 2>/dev/null; then
    echo "📝 Configurando aliases y funciones en $SHELL_RC..."
    cat >> "$SHELL_RC" << 'EOF'

# .NET environment variables
export ASPNETCORE_ENVIRONMENT=Development
export DOTNET_ENVIRONMENT=Development
export DOTNET_CLI_TELEMETRY_OPTOUT=1
export DOTNET_SKIP_FIRST_TIME_EXPERIENCE=1
export DOTNET_NOLOGO=1
export PATH="$PATH:$HOME/.dotnet/tools"

# .NET aliases
alias dn='dotnet'
alias dnb='dotnet build'
alias dnr='dotnet run'
alias dnt='dotnet test'
alias dnw='dotnet watch run'
alias dnc='dotnet clean'
alias dnrs='dotnet restore'
alias dnf='dotnet format'
alias dno='dotnet outdated'
alias dnef='dotnet ef'

# Git aliases
alias gs='git status'
alias ga='git add'
alias gc='git commit'
alias gp='git push'
alias gl='git log --oneline'

# Función para crear nuevo proyecto Web API
create-webapi() {
    if [ -z "$1" ]; then
        echo "Uso: create-webapi <nombre-proyecto>"
        return 1
    fi
    dotnet new webapi -n "$1" -o "$1"
    cd "$1"
    echo "✅ Proyecto Web API creado. Ejecuta 'dotnet run' para iniciar"
}

# Función para crear nuevo proyecto MVC
create-mvc() {
    if [ -z "$1" ]; then
        echo "Uso: create-mvc <nombre-proyecto>"
        return 1
    fi
    dotnet new mvc -n "$1" -o "$1"
    cd "$1"
    echo "✅ Proyecto MVC creado. Ejecuta 'dotnet run' para iniciar"
}

# Función para crear nuevo proyecto Console
create-console() {
    if [ -z "$1" ]; then
        echo "Uso: create-console <nombre-proyecto>"
        return 1
    fi
    dotnet new console -n "$1" -o "$1"
    cd "$1"
    echo "✅ Proyecto Console creado. Ejecuta 'dotnet run' para iniciar"
}
EOF
else
    echo "ℹ️  Los aliases ya están configurados en $SHELL_RC"
fi

echo "✅ Configuración post-creación completada"
