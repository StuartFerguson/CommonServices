FROM microsoft/dotnet:2.1-aspnetcore-runtime AS base
WORKDIR /app
EXPOSE 80

FROM microsoft/dotnet:2.1-sdk AS build
WORKDIR /src
COPY ["SubscriptionService.Configuration.Service/NuGet.Config", "."]
COPY ["SubscriptionService.Configuration.Service/SubscriptionService.Configuration.Service.csproj", "SubscriptionService.Configuration.Service/"]
COPY ["SubscriptionService.Configuration.Service/NuGet.Config", "SubscriptionService.Configuration.Service/"]
COPY ["SubscriptionService.BusinessLogic/SubscriptionService.BusinessLogic.csproj", "SubscriptionService.BusinessLogic/"]
COPY ["SubscriptionService.Database/SubscriptionService.Database.csproj", "SubscriptionService.Database/"]
COPY ["SubscriptionService.DataTransferObjects/SubscriptionService.DataTransferObjects.csproj", "SubscriptionService.DataTransferObjects/"]
RUN dotnet restore "SubscriptionService.Configuration.Service/SubscriptionService.Configuration.Service.csproj"
COPY . .
WORKDIR "/src/SubscriptionService.Configuration.Service"
RUN dotnet build "SubscriptionService.Configuration.Service.csproj" -c Release -o /app

FROM build AS publish
RUN dotnet publish "SubscriptionService.Configuration.Service.csproj" -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "SubscriptionService.Configuration.Service.dll"]