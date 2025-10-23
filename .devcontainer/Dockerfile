ARG DOTNET_VERSION="9.0"

FROM mcr.microsoft.com/dotnet/sdk:${DOTNET_VERSION}

# Evitar prompts durante la instalación
ENV DEBIAN_FRONTEND=noninteractive
ENV TZ=UTC

# Instalar dependencias base
RUN apt-get update && apt-get install -y \
    curl \
    git \
    unzip \
    zip \
    sudo \
    ca-certificates \
    gnupg \
    lsb-release \
    software-properties-common \
    && rm -rf /var/lib/apt/lists/*

# Instalar herramientas adicionales
RUN apt-get update && apt-get install -y \
    htop \
    vim \
    nano \
    jq \
    && rm -rf /var/lib/apt/lists/*

# Crear usuario coder
RUN useradd -m -s /bin/bash coder && \
    echo "coder ALL=(ALL) NOPASSWD:ALL" >> /etc/sudoers.d/coder && \
    chmod 0440 /etc/sudoers.d/coder

# Configurar directorio de trabajo
WORKDIR /workspaces
RUN chown -R coder:coder /workspaces

# Cambiar al usuario coder
USER coder

# Instalar herramientas globales de .NET
RUN dotnet tool install -g dotnet-ef && \
    dotnet tool install -g dotnet-aspnet-codegenerator && \
    dotnet tool install -g dotnet-format && \
    dotnet tool install -g dotnet-outdated-tool && \
    dotnet tool install -g dotnet-trace && \
    dotnet tool install -g dotnet-counters && \
    dotnet tool install -g dotnet-dump

# Agregar herramientas a PATH
ENV PATH="${PATH}:/home/coder/.dotnet/tools"

# Verificar configuración de NuGet (la fuente por defecto ya está configurada)

# Configurar Git
RUN git config --global init.defaultBranch main && \
    git config --global core.editor "code --wait"

# Configurar el entorno para .NET
ENV ASPNETCORE_ENVIRONMENT=Development
ENV DOTNET_ENVIRONMENT=Development
ENV DOTNET_CLI_TELEMETRY_OPTOUT=1
ENV DOTNET_SKIP_FIRST_TIME_EXPERIENCE=1
ENV DOTNET_NOLOGO=1

# Exponer puertos comunes para .NET
EXPOSE 5000 5001 5002 5003 5004

# Comando por defecto
CMD ["/bin/bash"]