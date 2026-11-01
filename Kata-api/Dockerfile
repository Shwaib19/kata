# Build and publish in the SDK image, then ship only the output: the runtime image carries no
# compiler, no sources and no test projects.
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# The whole solution is copied because restoring it needs every project file it references.
COPY Kata.sln ./
COPY src/ ./src/

RUN dotnet restore Kata.sln
RUN dotnet publish src/Kata.Api/Kata.Api.csproj \
      --configuration Release \
      --no-restore \
      --output /publish

FROM mcr.microsoft.com/dotnet/aspnet:10.0
WORKDIR /app

COPY --from=build /publish ./

ENV ASPNETCORE_HTTP_PORTS=8080
EXPOSE 8080

ENTRYPOINT ["dotnet", "Kata.Api.dll"]
