# Sử dụng .NET SDK để build ứng dụng
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

# Copy toàn bộ solution vào container
COPY ["VaccineChildren.sln", "./"]
COPY ["VaccineChildren.API/VaccineChildren.API.csproj", "VaccineChildren.API/"]
COPY ["VaccineChildren.Application/VaccineChildren.Application.csproj", "VaccineChildren.Application/"]
COPY ["VaccineChildren.Core/VaccineChildren.Core.csproj", "VaccineChildren.Core/"]
COPY ["VaccineChildren.Domain/VaccineChildren.Domain.csproj", "VaccineChildren.Domain/"]
COPY ["VaccineChildren.Infrastructure/VaccineChildren.Infrastructure.csproj", "VaccineChildren.Infrastructure/"]

# Restore các package
RUN dotnet restore

# Copy toàn bộ source code
COPY . .

# Build ứng dụng
WORKDIR "/app/VaccineChildren.API"
RUN dotnet publish -c Release -o /out --no-restore

# Runtime Image
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app

# Cài đặt Docker CLI để chạy docker-compose
RUN apt-get update && apt-get install -y docker.io

# Copy từ build stage vào runtime
COPY --from=build /out .

# Copy file docker-compose.yml vào container
COPY docker-compose.yml /app/docker-compose.yml

# Thiết lập biến môi trường
ENV ASPNETCORE_URLS=http://+:5014
ENV DOTNET_RUNNING_IN_CONTAINER=true

# Expose cổng API
EXPOSE 5014

# Chạy docker-compose khi container khởi động
CMD ["sh", "-c", "docker-compose up -d && dotnet VaccineChildren.API.dll"]
