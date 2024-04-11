FROM --platform=$TARGETPLATFORM debian:10.13-slim AS libsass-build
RUN apt update && apt install -y build-essential git
RUN git clone https://github.com/sass/libsass.git /usr/src/libsass
WORKDIR /usr/src/libsass
RUN git checkout tags/3.6.6
ENV BUILD=static
RUN make -j4

FROM --platform=$BUILDPLATFORM europe-west1-docker.pkg.dev/hosting-666/dotnet/sdk:8.0.203-bullseye-slim
RUN apt update && apt install -y build-essential clang git zlib1g-dev
# RUN git clone https://github.com/sass/libsass.git /usr/src/libsass

# WORKDIR /usr/src/libsass
# RUN git checkout tags/3.6.6
# ENV BUILD=static
# RUN make -j4
COPY --from=libsass-build /usr/src/libsass/lib/libsass.a /usr/src/libsass/lib/libsass.a

WORKDIR /build
COPY ./NuGet.Config ./
ARG TARGETARCH
RUN apt install -y binutils-aarch64-linux-gnu gcc-aarch64-linux-gnu g++-aarch64-linux-gnu
RUN apt install libstdc++6-arm64-cross
RUN dpkg --add-architecture arm64
RUN apt update && apt install -y zlib1g-dev:arm64 libstdc++-10-dev:arm64
COPY ./NCoreUtils.Sass ./NCoreUtils.Sass/
COPY ./NCoreUtils.Sass.MSBuild.Runner ./NCoreUtils.Sass.MSBuild.Runner/
RUN dotnet restore -a ${TARGETARCH} -r linux-arm64 ./NCoreUtils.Sass.MSBuild.Runner/NCoreUtils.Sass.MSBuild.Runner.csproj
RUN dotnet publish --no-restore -c Release -o /tmp/runner -a ${TARGETARCH} --self-contained -p:ObjCopyName=aarch64-linux-gnu-objcopy -p LibSassStaticLibPath=/usr/src/libsass/lib/libsass.a ./NCoreUtils.Sass.MSBuild.Runner/NCoreUtils.Sass.MSBuild.Runner.csproj