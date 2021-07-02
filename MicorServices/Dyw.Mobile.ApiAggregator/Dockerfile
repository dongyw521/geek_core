#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/core/aspnet:3.1-buster-slim AS base
WORKDIR /app
EXPOSE 80
EXPOSE 81
EXPOSE 443

FROM mcr.microsoft.com/dotnet/core/sdk:3.1-buster AS build
WORKDIR /src
COPY ["Dyw.Mobile.ApiAggregator/Dyw.Mobile.ApiAggregator.csproj", "Dyw.Mobile.ApiAggregator/"]
RUN dotnet restore "Dyw.Mobile.ApiAggregator/Dyw.Mobile.ApiAggregator.csproj"
COPY . .
WORKDIR "/src/Dyw.Mobile.ApiAggregator"
RUN dotnet build "Dyw.Mobile.ApiAggregator.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Dyw.Mobile.ApiAggregator.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Dyw.Mobile.ApiAggregator.dll"]