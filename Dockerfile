# 1. SDK aşaması (Derleme için .NET 9.0 kullanıyoruz)
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /app

# Dosyaları kopyala
COPY . .

# Projeyi Release modunda derle
RUN dotnet publish *.csproj -c Release -o out

# 2. Çalışma aşaması (Runtime için de .NET 9.0 kullanıyoruz)
FROM mcr.microsoft.com/dotnet/aspnet:9.0
WORKDIR /app
COPY --from=build /app/out .

# Uygulamayı başlat
ENTRYPOINT ["dotnet", "TorosSolar.dll"]
