FROM microsoft/dotnet:2.1-aspnetcore-runtime AS base
WORKDIR /app
EXPOSE 80

FROM microsoft/dotnet:2.1-sdk AS build
WORKDIR /src
COPY ["MessagingService.Service/NuGet.Config", "."]
COPY ["MessagingService.Service/MessagingService.Service.csproj", "MessagingService.Service/"]
COPY ["MessagingService.DataTransferObjects/MessagingService.DataTransferObjects.csproj", "MessagingService.DataTransferObjects/"]
RUN dotnet restore "MessagingService.Service/MessagingService.Service.csproj"
COPY . .
WORKDIR "/src/MessagingService.Service"
RUN dotnet build "MessagingService.Service.csproj" -c Release -o /app

FROM build AS publish
RUN dotnet publish "MessagingService.Service.csproj" -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "MessagingService.Service.dll"]