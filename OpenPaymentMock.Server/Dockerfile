#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER app
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM node:20-slim AS node
WORKDIR /src
COPY ./OpenPaymentMock.Frontend /src/OpenPaymentMock.Frontend
WORKDIR /src/OpenPaymentMock.Frontend
RUN npm i
RUN npm run build

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["./OpenPaymentMock.Server/OpenPaymentMock.Server.csproj", "./OpenPaymentMock.Server/"]
RUN dotnet restore "./OpenPaymentMock.Server/OpenPaymentMock.Server.csproj"
COPY . .
WORKDIR "/src/OpenPaymentMock.Server"
RUN dotnet build "./OpenPaymentMock.Server.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./OpenPaymentMock.Server.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
COPY --from=node /src/OpenPaymentMock.Frontend/dist ./wwwroot
ENTRYPOINT ["dotnet", "OpenPaymentMock.Server.dll"]