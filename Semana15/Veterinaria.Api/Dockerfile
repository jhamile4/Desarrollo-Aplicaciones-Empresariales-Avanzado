# Microservicio 1: Veterinaria API (Semana 14). Contexto de construcción: carpeta Semana15
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src
COPY . .
RUN dotnet publish Veterinaria.Api/Veterinaria.Api.csproj -c Release -o /app

FROM mcr.microsoft.com/dotnet/aspnet:10.0
WORKDIR /app
COPY --from=build /app .
# Render envía el tráfico al puerto 10000 (variable PORT)
ENV ASPNETCORE_HTTP_PORTS=10000
EXPOSE 10000
ENTRYPOINT ["dotnet", "Veterinaria.Api.dll"]
