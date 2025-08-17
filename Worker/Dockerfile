
# This stage is used to build the service project
FROM mcr.microsoft.com/dotnet/sdk:9.0-alpine AS build
# Install clang/zlib1g-dev dependencies for publishing to native
RUN apk update \
    && apk add build-base zlib-dev
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["Worker.csproj", "."]
RUN dotnet restore "Worker.csproj"
COPY . .
WORKDIR "/src"
RUN dotnet build "./Worker.csproj" -c $BUILD_CONFIGURATION -o /app/build

# This stage is used to publish the service project to be copied to the final stage
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./Worker.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=true

FROM mcr.microsoft.com/dotnet/runtime-deps:9.0-alpine AS final
WORKDIR /app
EXPOSE 5000
COPY --from=publish /app/publish .
ENTRYPOINT ["./Worker"]