# Stage 1: Build
FROM mcr.microsoft.com/dotnet/sdk:11.0 AS build
WORKDIR /src

# Layer caching: copy project files first, restore, then copy source
COPY Greg.Xrm.Command/Directory.Build.props .
COPY Greg.Xrm.Command/Greg.Xrm.Command.sln .
COPY Greg.Xrm.Command/Greg.Xrm.Command/Greg.Xrm.Command.csproj Greg.Xrm.Command/
COPY Greg.Xrm.Command/Greg.Xrm.Command.Interfaces/Greg.Xrm.Command.Interfaces.csproj Greg.Xrm.Command.Interfaces/
COPY Greg.Xrm.Command/Greg.Xrm.Command.Core/Greg.Xrm.Command.Core.csproj Greg.Xrm.Command.Core/
COPY Greg.Xrm.Command/Greg.Xrm.Command.Core.TestSuite/Greg.Xrm.Command.Core.TestSuite.csproj Greg.Xrm.Command.Core.TestSuite/
COPY Greg.Xrm.Command/Greg.Xrm.Command.TestSamplePlugin/Greg.Xrm.Command.TestSamplePlugin.csproj Greg.Xrm.Command.TestSamplePlugin/

RUN dotnet restore Greg.Xrm.Command.sln

COPY Greg.Xrm.Command/ .

RUN dotnet publish Greg.Xrm.Command/Greg.Xrm.Command.csproj \
    -c Release \
    -o /app \
    --no-restore \
    --framework net11.0

# Stage 2: Runtime
FROM mcr.microsoft.com/dotnet/runtime-deps:11.0-jammy AS runtime

# Create non-root user
RUN addgroup --system --gid 1001 pacx \
    && adduser --system --uid 1001 --ingroup pacx pacx

WORKDIR /app
COPY --from=build --chown=pacx:pacx /app .

ENV DOTNET_SYSTEM_GLOBALIZATION_INVARIANT=1

# OCI labels
LABEL org.opencontainers.image.title="PACX" \
      org.opencontainers.image.description="PACX - PowerApps CLI Extensions" \
      org.opencontainers.image.source="https://github.com/edithatogo/pacx" \
      org.opencontainers.image.licenses="MIT"

HEALTHCHECK --interval=30s --timeout=3s --start-period=5s --retries=3 \
    CMD ["dotnet", "pacx.dll", "--version"] || exit 1

USER pacx:pacx
ENTRYPOINT ["dotnet", "pacx.dll"]
