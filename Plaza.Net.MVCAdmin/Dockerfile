# 请参阅 https://aka.ms/customizecontainer 以了解如何自定义调试容器，以及 Visual Studio 如何使用此 Dockerfile 生成映像以更快地进行调试。

# 此阶段用于在快速模式(默认为调试配置)下从 VS 运行时
FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80


# 此阶段用于生成服务项目
FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["Plaza.Net.MVCAdmin/Plaza.Net.MVCAdmin.csproj", "Plaza.Net.MVCAdmin/"]
COPY ["Plaza.Net.Auth/Plaza.Net.Auth.csproj", "Plaza.Net.Auth/"]
COPY ["Plaza.Net.EmailService/Plaza.Net.EmailService.csproj", "Plaza.Net.EmailService/"]
COPY ["Plaza.Net.Model/Plaza.Net.Model.csproj", "Plaza.Net.Model/"]
COPY ["Plaza.Net.Configruation/Plaza.Net.Configruation.csproj", "Plaza.Net.Configruation/"]
COPY ["Plaza.Net.Utility/Plaza.Net.Utility.csproj", "Plaza.Net.Utility/"]
COPY ["Plaza.Net.Core/Plaza.Net.Core.csproj", "Plaza.Net.Core/"]
COPY ["Plaza.Net.Caching/Plaza.Net.Caching.csproj", "Plaza.Net.Caching/"]
COPY ["Plaza.Net.Repository/Plaza.Net.Repository.csproj", "Plaza.Net.Repository/"]
COPY ["Plaza.Net.IRepository/Plaza.Net.IRepository.csproj", "Plaza.Net.IRepository/"]
COPY ["Plaza.Net.Services/Plaza.Net.Services.csproj", "Plaza.Net.Services/"]
COPY ["Plaza.Net.IServices/Plaza.Net.IServices.csproj", "Plaza.Net.IServices/"]
COPY ["Plaza.Net.Mapping/Plaza.Net.Mapping.csproj", "Plaza.Net.Mapping/"]
RUN dotnet restore "./Plaza.Net.MVCAdmin/Plaza.Net.MVCAdmin.csproj"
COPY . .
WORKDIR "/src/Plaza.Net.MVCAdmin"
RUN dotnet build "./Plaza.Net.MVCAdmin.csproj" -c $BUILD_CONFIGURATION -o /app/build

# 此阶段用于发布要复制到最终阶段的服务项目
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./Plaza.Net.MVCAdmin.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# 此阶段在生产中使用，或在常规模式下从 VS 运行时使用(在不使用调试配置时为默认值)
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Plaza.Net.MVCAdmin.dll"]