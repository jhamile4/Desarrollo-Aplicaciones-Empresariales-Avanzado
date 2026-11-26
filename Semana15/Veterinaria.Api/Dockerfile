# Microservicio 1: Veterinaria API (Semana 14). Contexto de construcción: carpeta Semana15
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src
COPY . .
RUN dotnet publish Veterinaria.Api/Veterinaria.Api.csproj -c Release -o /app

FROM mcr.microsoft.com/dotnet/aspnet:10.0
WORKDIR /app
COPY --from=build /app .
# Escucha en el puerto que asigna Render (variable PORT; 10000 si no existe) y en todas las interfaces
ENV PORT=10000
EXPOSE 10000
ENTRYPOINT ["sh", "-c", "ASPNETCORE_URLS=http://0.0.0.0:${PORT} exec dotnet Veterinaria.Api.dll"]
