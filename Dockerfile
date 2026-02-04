# 1. SDK aşaması
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /app

# Tüm dosyaları kopyala
COPY . .

# Projeyi derle
RUN dotnet publish *.csproj -c Release -o out

# 2. Çalışma aşaması
FROM mcr.microsoft.com/dotnet/aspnet:9.0
WORKDIR /app

# Derleme çıktılarını kopyala
COPY --from=build /app/out .

# KRİTİK SATIR: Veritabanı dosyasını çalışma klasörüne zorla kopyalıyoruz
COPY TorosSolar.db . 

# Uygulamayı başlat
ENTRYPOINT ["dotnet", "TorosSolar.dll"]
