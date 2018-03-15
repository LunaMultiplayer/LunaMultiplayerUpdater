FROM microsoft/dotnet:2.0-runtime AS base
WORKDIR /app

FROM microsoft/dotnet:2.0-sdk AS build
WORKDIR /src
COPY LunaMultiplayerUpdater.sln ./
COPY Manager/Manager.csproj Manager/
RUN dotnet restore -nowarn:msb3202,nu1503
COPY . .
WORKDIR /src/Manager
RUN dotnet build -c Release -o /app

FROM build AS publish
RUN dotnet publish -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "Manager.dll"]
