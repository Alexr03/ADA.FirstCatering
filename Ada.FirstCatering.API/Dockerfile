FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["Ada.FirstCatering.API/Ada.FirstCatering.API.csproj", "Ada.FirstCatering.API/"]
RUN dotnet restore "Ada.FirstCatering.API/Ada.FirstCatering.API.csproj"
COPY . .
WORKDIR "/src/Ada.FirstCatering.API"
RUN dotnet build "Ada.FirstCatering.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Ada.FirstCatering.API.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Ada.FirstCatering.API.dll"]
