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
COPY ["Services/PanacheSoftware.Service.Client/PanacheSoftware.Service.Client.csproj", "Services/PanacheSoftware.Service.Client/"]
COPY ["Core/PanacheSoftware.Database/PanacheSoftware.Database.csproj", "Core/PanacheSoftware.Database/"]
COPY ["Core/PanacheSoftware.Core/PanacheSoftware.Core.csproj", "Core/PanacheSoftware.Core/"]
COPY ["Core/PanacheSoftware.Http/PanacheSoftware.Http.csproj", "Core/PanacheSoftware.Http/"]
RUN dotnet restore "Services/PanacheSoftware.Service.Client/PanacheSoftware.Service.Client.csproj"
COPY . .
WORKDIR "/src/Services/PanacheSoftware.Service.Client"
RUN dotnet build "PanacheSoftware.Service.Client.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "PanacheSoftware.Service.Client.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "PanacheSoftware.Service.Client.dll"]
