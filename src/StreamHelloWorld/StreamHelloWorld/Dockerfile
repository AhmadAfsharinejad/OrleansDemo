FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["StreamHelloWorld2/StreamHelloWorld2.csproj", "StreamHelloWorld2/"]
RUN dotnet restore "StreamHelloWorld2/StreamHelloWorld2.csproj"
COPY . .
WORKDIR "/src/StreamHelloWorld2"
RUN dotnet build "StreamHelloWorld2.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "StreamHelloWorld2.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "StreamHelloWorld2.dll"]
