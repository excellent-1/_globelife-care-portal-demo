# Vue build
FROM node:22-alpine AS vue-build
WORKDIR /workspace/web
COPY src/globelife-web/package*.json ./
RUN npm install
COPY src/globelife-web/ ./
RUN npm run build

# API build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS api-build
WORKDIR /workspace
COPY src/GlobeLife.Api/GlobeLife.Api.csproj src/GlobeLife.Api/
RUN dotnet restore src/GlobeLife.Api/GlobeLife.Api.csproj
COPY src/GlobeLife.Api/ src/GlobeLife.Api/
RUN dotnet publish src/GlobeLife.Api/GlobeLife.Api.csproj -c Release -o /out

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080
COPY --from=api-build /out ./
COPY --from=vue-build /workspace/web/dist ./wwwroot
ENTRYPOINT ["dotnet", "GlobeLife.Api.dll"]
