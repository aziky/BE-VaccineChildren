# Sử dụng .NET SDK để build ứng dụng
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

# Copy toàn bộ solution vào container
COPY ["VaccineChildren.sln", "./"]
COPY ["VaccineChildren.API/VaccineChildren.API.csproj", "VaccineChildren.API/"]
COPY ["VaccineChildren.API/private_key.rsa", "VaccineChildren.API/private_key.rsa"]
COPY ["VaccineChildren.API/public_key.rsa", "VaccineChildren.API/public_key.rsa"]
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

# Copy key files to output directory
RUN cp private_key.rsa /out/private_key.rsa
RUN cp public_key.rsa /out/public_key.rsa

# Sử dụng runtime để chạy ứng dụng
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app

# Copy từ build stage vào runtime
COPY --from=build /out .

# Thiết lập biến môi trường
ENV ASPNETCORE_URLS=http://+:5014
ENV DOTNET_RUNNING_IN_CONTAINER=true

# Expose cổng ứng dụng
EXPOSE 5014

# Chạy ứng dụng
ENTRYPOINT ["dotnet", "VaccineChildren.API.dll", "--urls", "http://0.0.0.0:5014"]