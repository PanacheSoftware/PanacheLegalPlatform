#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/core/aspnet:3.1-buster-slim AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

# Set the locale
RUN apt-get update
RUN apt-get install -y locales locales-all
ENV LANG en_GB.UTF-8  
ENV LANGUAGE en_GB:en  
ENV LC_ALL en_GB.UTF-8

FROM mcr.microsoft.com/dotnet/core/sdk:3.1-buster AS build
WORKDIR /src
COPY ["Web Apps/PanacheSoftware.UI.Client/PanacheSoftware.UI.Client.csproj", "Web Apps/PanacheSoftware.UI.Client/"]
COPY ["Web/PanacheSoftware.UI.Core/PanacheSoftware.UI.Core.csproj", "Web/PanacheSoftware.UI.Core/"]
COPY ["Core/PanacheSoftware.Core/PanacheSoftware.Core.csproj", "Core/PanacheSoftware.Core/"]
COPY ["Core/PanacheSoftware.Http/PanacheSoftware.Http.csproj", "Core/PanacheSoftware.Http/"]
RUN dotnet restore "Web Apps/PanacheSoftware.UI.Client/PanacheSoftware.UI.Client.csproj"
COPY . .
WORKDIR "/src/Web Apps/PanacheSoftware.UI.Client"
RUN dotnet build "PanacheSoftware.UI.Client.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "PanacheSoftware.UI.Client.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "PanacheSoftware.UI.Client.dll"]
