# See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

# This stage is used when running from VS in fast mode (Default for Debug configuration)
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081


# This stage is used to build the service project
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["eCommerce.OrdersService.API/eCommerce.OrdersService.API.csproj", "eCommerce.OrdersService.API/"]
RUN dotnet restore "./eCommerce.OrdersService.API/eCommerce.OrdersService.API.csproj"
COPY . .
WORKDIR "/src/eCommerce.OrdersService.API"
RUN dotnet build "./eCommerce.OrdersService.API.csproj" -c $BUILD_CONFIGURATION -o /app/build

# This stage is used to publish the service project to be copied to the final stage
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./eCommerce.OrdersService.API.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# This stage is used in production or when running from VS in regular mode (Default when not using the Debug configuration)
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENV MONGO_USER=appuser
ENV MONGO_PASS=password
ENV MONGO_HOST=localhost
ENV MONGO_PORT=27017
ENV MONGO_DATABASE=orders
ENV MONGO_COLLECTION_ORDERS=orders
ENV USERSERVICE_HOST=localhost
ENV USERSERVICE_PORT=9090
ENTRYPOINT ["dotnet", "eCommerce.OrdersService.API.dll"]