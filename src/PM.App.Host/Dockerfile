FROM microsoft/dotnet:2.1-aspnetcore-runtime AS base
WORKDIR /app
EXPOSE 80

FROM microsoft/dotnet:2.1-sdk AS build
WORKDIR /src
COPY ["PM.App.Host/PM.App.Host.csproj", "PM.App.Host/"]
RUN dotnet restore "PM.App.Host/PM.App.Host.csproj"
COPY . .
WORKDIR "/src/PM.App.Host"
RUN dotnet build "PM.App.Host.csproj" -c Release -o /app

FROM build AS publish
RUN dotnet publish "PM.App.Host.csproj" -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "PM.App.Host.dll"]