# Usa la imagen oficial del SDK de .NET 8 como entorno de construcción
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copia el archivo de proyecto y restaura las dependencias
COPY ["PostulacionIdeaCreativa.csproj", "./"]
RUN dotnet restore "./PostulacionIdeaCreativa.csproj"

# Copia todo el resto del código y transpila
COPY . .
RUN dotnet publish "PostulacionIdeaCreativa.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Cambia a la imagen en tiempo de ejecución de .NET
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 8080

# Copia la versión empaquetada
FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "PostulacionIdeaCreativa.dll"]
