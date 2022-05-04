#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

#FROM mcr.microsoft.com/dotnet/aspnet:6.0-bullseye-slim-arm32v7 AS base
FROM mcr.microsoft.com/dotnet/aspnet:6.0-bullseye-slim AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

# Set the locale
RUN apt-get update
RUN apt-get install -y locales locales-all
ENV LANG en_GB.UTF-8  
ENV LANGUAGE en_GB:en  
ENV LC_ALL en_GB.UTF-8

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["Web Apps/PanacheSoftware.Identity/PanacheSoftware.Identity.csproj", "Web Apps/PanacheSoftware.Identity/"]
COPY ["Web/PanacheSoftware.UI.Core/PanacheSoftware.UI.Core.csproj", "Web/PanacheSoftware.UI.Core/"]
COPY ["Core/PanacheSoftware.Core/PanacheSoftware.Core.csproj", "Core/PanacheSoftware.Core/"]
COPY ["Core/PanacheSoftware.Http/PanacheSoftware.Http.csproj", "Core/PanacheSoftware.Http/"]
RUN dotnet restore "Web Apps/PanacheSoftware.Identity/PanacheSoftware.Identity.csproj"
COPY . .
WORKDIR "/src/Web Apps/PanacheSoftware.Identity"
RUN dotnet build "PanacheSoftware.Identity.csproj" -c Release -o /app/build #-r linux-arm

FROM build AS publish
RUN dotnet publish "PanacheSoftware.Identity.csproj" -c Release -o /app/publish --no-restore #-r linux-arm --self-contained false --no-restore

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "PanacheSoftware.Identity.dll"]
