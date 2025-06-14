FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

COPY ["ZEN.Application/ZEN.Application.csproj", "ZEN.Application/"]
COPY ["ZEN.Contract/ZEN.Contract.csproj", "ZEN.Contract/"]
COPY ["ZEN.Controller/ZEN.Controller.csproj", "ZEN.Controller/"]
COPY ["ZEN.CoreLib/ZEN.CoreLib.csproj", "ZEN.CoreLib/"]
COPY ["ZEN.Domain/ZEN.Domain.csproj", "ZEN.Domain/"]
COPY ["ZEN.Infrastructure/ZEN.Infrastructure.csproj", "ZEN.Infrastructure/"]
COPY ["ZEN.Infrastructure.Mysql/ZEN.Infrastructure.Mysql.csproj", "ZEN.Infrastructure.Mysql/"]
COPY ["ZEN.Startup/ZEN.Startup.csproj", "ZEN.Startup/"]

RUN dotnet restore "ZEN.Startup/ZEN.Startup.csproj"

COPY . .
WORKDIR "/src/ZEN.Startup"
RUN dotnet publish -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:9.0
WORKDIR /app
COPY --from=build /app/publish .

COPY ZEN.Startup/appsettings.json /app/appsettings.json
COPY ZEN.Startup/appsettings.Development.json /app/appsettings.Development.json

ENTRYPOINT ["dotnet", "ZEN.Startup.dll"]
