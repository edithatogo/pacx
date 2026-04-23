ARG SDK_IMAGE=mcr.microsoft.com/dotnet/sdk:10.0
FROM ${SDK_IMAGE} AS build

WORKDIR /src
COPY . .

RUN dotnet restore Greg.Xrm.Command/Greg.Xrm.Command.sln
RUN dotnet build Greg.Xrm.Command/Greg.Xrm.Command.sln -c Release --no-restore

# Optional publish output for containerized execution or packaging.
RUN dotnet publish Greg.Xrm.Command/Greg.Xrm.Command.csproj -c Release -o /out/publish --no-restore
