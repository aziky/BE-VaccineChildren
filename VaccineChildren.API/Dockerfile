FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["VaccineChildren.API/VaccineChildren.API.csproj", "VaccineChildren.API/"]
COPY ["VaccineChildren.Application/VaccineChildren.Application.csproj", "VaccineChildren.Application/"]
COPY ["VaccineChildren.Core/VaccineChildren.Core.csproj", "VaccineChildren.Core/"]
COPY ["VaccineChildren.Domain/VaccineChildren.Domain.csproj", "VaccineChildren.Domain/"]
COPY ["VaccineChildren.Infrastructure/VaccineChildren.Infrastructure.csproj", "VaccineChildren.Infrastructure/"]
RUN dotnet restore "VaccineChildren.API/VaccineChildren.API.csproj"
COPY . .
WORKDIR "/src/VaccineChildren.API"
RUN dotnet build "VaccineChildren.API.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "VaccineChildren.API.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "VaccineChildren.API.dll"]
