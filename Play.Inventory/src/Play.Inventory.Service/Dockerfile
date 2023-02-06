FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY Play.Inventory.Service.csproj .
# had to add packages folder in Play.Inventory.Services in order for this copy to work
COPY solving-nuget/packages ./packages/
COPY solving-nuget/NuGet.Config /root/.nuget/NuGet/NuGet.Config
RUN dotnet restore
COPY . .
RUN dotnet publish -c release -o /app

FROM mcr.microsoft.com/dotnet/aspnet:6.0
WORKDIR /app
COPY --from=build /app .
ENTRYPOINT ["dotnet", "Play.Inventory.Service.dll"]