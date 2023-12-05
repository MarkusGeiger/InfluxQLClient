FROM mcr.microsoft.com/dotnet/runtime:8.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["InfluxQl.GrafanaProxy/InfluxQl.GrafanaProxy.csproj", "InfluxQl.GrafanaProxy/"]
RUN dotnet restore "InfluxQl.GrafanaProxy/InfluxQl.GrafanaProxy.csproj"
COPY . .
WORKDIR "/src/InfluxQl.GrafanaProxy"
RUN dotnet build "InfluxQl.GrafanaProxy.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "InfluxQl.GrafanaProxy.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "InfluxQl.GrafanaProxy.dll"]
