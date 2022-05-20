FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build-env
WORKDIR /app

# Copy everything
COPY . ./

# Restore as distinct layers
RUN dotnet restore

# Build and publish a release
RUN dotnet publish -c Release -r linux-x64 -p:PublishSingleFile=true --self-contained true -o out

# Build runtime image
FROM hasura/graphql-engine
WORKDIR /hasura-stdout-wrapper
COPY --from=build-env /app/out .
COPY --from=build-env /app/entryPoint.sh .

CMD [ "./entryPoint.sh"]
