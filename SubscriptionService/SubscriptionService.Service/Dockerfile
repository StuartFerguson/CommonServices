FROM microsoft/dotnet:2.2-aspnetcore-runtime AS base
WORKDIR /app
EXPOSE 80

FROM microsoft/dotnet:2.2-sdk AS build
WORKDIR /src
COPY ["SubscriptionService.Service/NuGet.Config", "."]
COPY ["SubscriptionService.Service/SubscriptionService.Service.csproj", "SubscriptionService.Service/"]
COPY ["SubscriptionService.BusinessLogic/SubscriptionService.BusinessLogic.csproj", "SubscriptionService.BusinessLogic/"]
COPY ["SubscriptionService.Database/SubscriptionService.Database.csproj", "SubscriptionService.Database/"]
COPY ["SubscriptionService.DataTransferObjects/SubscriptionService.DataTransferObjects.csproj", "SubscriptionService.DataTransferObjects/"]
RUN dotnet restore "SubscriptionService.Service/SubscriptionService.Service.csproj"
COPY . .
WORKDIR "/src/SubscriptionService.Service"
RUN dotnet build "SubscriptionService.Service.csproj" -c Release -o /app

FROM build AS publish
RUN dotnet publish "SubscriptionService.Service.csproj" -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "SubscriptionService.Service.dll"]