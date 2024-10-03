FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app
COPY TrainBot.csproj ./
RUN dotnet restore
COPY . ./
RUN dotnet publish -c Release -o TrainBot

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY --from=build /app/TrainBot ./
EXPOSE 8080
EXPOSE 8081
ENTRYPOINT ["dotnet", "TrainBot.dll"]
