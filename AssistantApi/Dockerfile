FROM mcr.microsoft.com/dotnet/aspnet:5.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /src
COPY ["AssistantApi/AssistantApi.csproj", "AssistantApi/"]
RUN dotnet restore "AssistantApi/AssistantApi.csproj"
COPY . .
WORKDIR "/src/AssistantApi"
RUN dotnet build "AssistantApi.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "AssistantApi.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "AssistantApi.dll"]
