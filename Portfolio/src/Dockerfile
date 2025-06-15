FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

COPY ["src/ZEN.Application/ZEN.Application.csproj", "ZEN.Application/"]
COPY ["src/ZEN.Contract/ZEN.Contract.csproj", "ZEN.Contract/"]
COPY ["src/ZEN.Controller/ZEN.Controller.csproj", "ZEN.Controller/"]
COPY ["src/ZEN.CoreLib/ZEN.CoreLib.csproj", "ZEN.CoreLib/"]
COPY ["src/ZEN.Domain/ZEN.Domain.csproj", "ZEN.Domain/"]
COPY ["src/ZEN.Infrastructure/ZEN.Infrastructure.csproj", "ZEN.Infrastructure/"]
COPY ["src/ZEN.Infrastructure.Mysql/ZEN.Infrastructure.Mysql.csproj", "ZEN.Infrastructure.Mysql/"]
COPY ["src/ZEN.Startup/ZEN.Startup.csproj", "ZEN.Startup/"]

RUN dotnet restore "ZEN.Startup/ZEN.Startup.csproj"

COPY src/. .
WORKDIR "/src/ZEN.Startup"
RUN dotnet publish -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:9.0
WORKDIR /app
COPY --from=build /app/publish .

COPY src/ZEN.Startup/appsettings.json /app/appsettings.json
COPY src/ZEN.Startup/appsettings.Development.json /app/appsettings.Development.json

ENTRYPOINT ["dotnet", "ZEN.Startup.dll"]
