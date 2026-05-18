FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

COPY ["DevBoard.Api/DevBoard.Api.csproj", "DevBoard.Api/"]
COPY ["DevBoard.Application/DevBoard.Application.csproj", "DevBoard.Application/"]
COPY ["DevBoard.Domain/DevBoard.Domain.csproj", "DevBoard.Domain/"]
COPY ["DevBoard.Infrastructure/DevBoard.Infrastructure.csproj", "DevBoard.Infrastructure/"]

RUN dotnet restore "DevBoard.Api/DevBoard.Api.csproj"

COPY . .

RUN dotnet build "DevBoard.Api/DevBoard.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "DevBoard.Api/DevBoard.Api.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "DevBoard.Api.dll"]