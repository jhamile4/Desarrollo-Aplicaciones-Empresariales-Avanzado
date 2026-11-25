# Microservicio 2: Vacunas. Contexto de construcción: carpeta Semana15
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src
COPY . .
RUN dotnet publish Veterinaria.Vacunas/Veterinaria.Vacunas.csproj -c Release -o /app

FROM mcr.microsoft.com/dotnet/aspnet:10.0
WORKDIR /app
COPY --from=build /app .
ENV ASPNETCORE_HTTP_PORTS=10000
EXPOSE 10000
ENTRYPOINT ["dotnet", "Veterinaria.Vacunas.dll"]
