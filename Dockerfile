FROM mcr.microsoft.com/dotnet/aspnet:6.0-alpine AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:6.0-alpine AS build
WORKDIR /src
COPY . .
WORKDIR /src/UnitTests
RUN dotnet test || exit 1
WORKDIR /src/Server
RUN dotnet build "PasswordWallet.Server.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "PasswordWallet.Server.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "PasswordWallet.Server.dll"]