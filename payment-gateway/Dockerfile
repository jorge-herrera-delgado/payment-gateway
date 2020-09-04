#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/core/aspnet:3.1-buster-slim AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/core/sdk:3.1-buster AS build
WORKDIR /src
COPY ["payment-gateway/payment-gateway.csproj", "payment-gateway/"]
COPY ["payment-gateway-core/payment-gateway-core.csproj", "payment-gateway-core/"]
COPY ["payment-gateway-repository/payment-gateway-repository.csproj", "payment-gateway-repository/"]
RUN dotnet restore "payment-gateway/payment-gateway.csproj"
COPY . .
WORKDIR "/src/payment-gateway"
RUN dotnet build "payment-gateway.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "payment-gateway.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "payment-gateway.dll"]