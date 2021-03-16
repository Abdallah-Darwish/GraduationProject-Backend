#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:5.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /src
COPY ["GradProjectServer/GradProjectServer.csproj", "GradProjectServer/"]
RUN dotnet restore "GradProjectServer/GradProjectServer.csproj"
COPY . .
WORKDIR "/src/GradProjectServer"
RUN dotnet build "GradProjectServer.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "GradProjectServer.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "GradProjectServer.dll"]