FROM mcr.microsoft.com/dotnet/runtime:7.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["KafkaSample/KafkaSample.csproj", "KafkaSample/"]
RUN dotnet restore "KafkaSample/KafkaSample.csproj"
COPY . .
WORKDIR "/src/KafkaSample"
RUN dotnet build "KafkaSample.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "KafkaSample.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "KafkaSample.dll"]
