FROM --platform=$BUILDPLATFORM europe-west1-docker.pkg.dev/hosting-666/dotnet/sdk:8.0.203-bullseye-slim-amd64
RUN apt update && apt install -y build-essential clang git zlib1g-dev
RUN git clone https://github.com/sass/libsass.git /usr/src/libsass

WORKDIR /usr/src/libsass
RUN git checkout tags/3.6.6
ENV BUILD=static
RUN make -j4

WORKDIR /build
COPY ./NuGet.Config ./
COPY ./NCoreUtils.Sass ./NCoreUtils.Sass/
COPY ./NCoreUtils.Sass.MSBuild.Runner ./NCoreUtils.Sass.MSBuild.Runner/
ARG RID=linux-x64
RUN dotnet restore -r $RID ./NCoreUtils.Sass.MSBuild.Runner/NCoreUtils.Sass.MSBuild.Runner.csproj
RUN dotnet publish --no-restore -c Release -o /tmp/runner -r $RID --self-contained -p LibSassStaticLibPath=/usr/src/libsass/lib/libsass.a ./NCoreUtils.Sass.MSBuild.Runner/NCoreUtils.Sass.MSBuild.Runner.csproj