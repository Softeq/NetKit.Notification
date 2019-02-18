FROM microsoft/aspnetcore:2.0 AS base
WORKDIR /app
EXPOSE 80

FROM microsoft/aspnetcore-build:2.0 AS build
COPY Softeq.NetKit.Notifications.Web/Softeq.NetKit.Notifications.Web.csproj Softeq.NetKit.Notifications.Web/
RUN dotnet restore Softeq.NetKit.Notifications.Web/Softeq.NetKit.Notifications.Web.csproj
COPY . .
WORKDIR /Softeq.NetKit.Notifications.Web
RUN dotnet build Softeq.NetKit.Notifications.Web.csproj -c Debug -o /app

FROM build AS publish
RUN dotnet publish Softeq.NetKit.Notifications.Web.csproj -c Debug -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "Softeq.NetKit.Notifications.Web.dll"]
