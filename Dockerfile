FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

# Tüm dosyaları kopyala
COPY . .

# Klasör yapısına göre dosyayı bulup derleyelim
# Eğer .csproj dosyası direkt karşındaysa bu satırı kullan:
RUN dotnet publish *.csproj -c Release -o out

FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build /app/out .
ENTRYPOINT ["dotnet", "TorosSolar.dll"]
