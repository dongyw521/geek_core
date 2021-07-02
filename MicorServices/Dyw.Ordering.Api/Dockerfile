#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/core/aspnet:3.1-buster-slim AS base
WORKDIR /app
EXPOSE 80
EXPOSE 81
EXPOSE 443

FROM mcr.microsoft.com/dotnet/core/sdk:3.1-buster AS build
WORKDIR /src
COPY ["Dyw.Ordering.Api/Dyw.Ordering.Api.csproj", "Dyw.Ordering.Api/"]
COPY ["Dyw.Ordering.Infrastructure/Dyw.Ordering.Infrastructure.csproj", "Dyw.Ordering.Infrastructure/"]
COPY ["Dyw.Ordering.Domain/Dyw.Ordering.Domain.csproj", "Dyw.Ordering.Domain/"]
COPY ["Dyw.Domain.Abstractions/Dyw.Domain.Abstractions.csproj", "Dyw.Domain.Abstractions/"]
COPY ["Dyw.Infrastructure.Core/Dyw.Infrastructure.Core.csproj", "Dyw.Infrastructure.Core/"]
RUN dotnet restore "Dyw.Ordering.Api/Dyw.Ordering.Api.csproj"
COPY . .
WORKDIR "/src/Dyw.Ordering.Api"
RUN dotnet build "Dyw.Ordering.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Dyw.Ordering.Api.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Dyw.Ordering.Api.dll"]