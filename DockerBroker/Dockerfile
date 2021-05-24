FROM mcr.microsoft.com/dotnet/sdk AS base
FROM base as publish
WORKDIR /src
COPY . DockerBroker/
COPY ../DockerCommon DockerCommon/
RUN dotnet publish "/src/DockerBroker.csproj" -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet AS final
WORKDIR /app
COPY --from=publish /app/publish .
EXPOSE 1235
EXPOSE 1236
EXPOSE 1237
ENTRYPOINT ["dotnet", "DockerBroker.dll"]
