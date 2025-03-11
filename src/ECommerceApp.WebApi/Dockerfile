FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["ECommerceApp.WebApi/ECommerceApp.WebApi.csproj", "ECommerceApp.WebApi/"]
COPY ["ECommerceApp.Application/ECommerceApp.Application.csproj", "ECommerceApp.Application/"]
COPY ["ECommerceApp.Domain/ECommerceApp.Domain.csproj", "ECommerceApp.Domain/"]
COPY ["ECommerceApp.Infrastructure/ECommerceApp.Infrastructure.csproj", "ECommerceApp.Infrastructure/"]
RUN dotnet restore "ECommerceApp.WebApi/ECommerceApp.WebApi.csproj"
COPY . .
WORKDIR "/src/ECommerceApp.WebApi"
RUN dotnet build "ECommerceApp.WebApi.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "ECommerceApp.WebApi.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTR