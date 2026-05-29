FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY ["CasaDaRosa.API/CasaDaRosa.API.csproj", "CasaDaRosa.API/"]
COPY ["CasaDaRosa.Application/CasaDaRosa.Application.csproj", "CasaDaRosa.Application/"]
COPY ["CasaDaRosa.Domain/CasaDaRosa.Domain.csproj", "CasaDaRosa.Domain/"]
COPY ["CasaDaRosa.Infrastructure/CasaDaRosa.Infrastructure.csproj", "CasaDaRosa.Infrastructure/"]
RUN dotnet restore "CasaDaRosa.API/CasaDaRosa.API.csproj"

COPY . .
WORKDIR "/src/CasaDaRosa.API"
RUN dotnet publish "CasaDaRosa.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=build /app/publish .

ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080

ENTRYPOINT ["dotnet", "CasaDaRosa.API.dll"]
