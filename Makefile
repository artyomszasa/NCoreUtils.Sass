TEMP_IMAGE?=ncoreutils-sass-build
TEMP_CONTAINER?=ncoreutils-sass

build-linux-x64:
	docker build -t $(TEMP_IMAGE) -f misc/Dockerfile .
	docker create --name $(TEMP_CONTAINER) $(TEMP_IMAGE)
	mkdir -p ./NCoreUtils.Sass.MSBuild/tools/linux-x64
	docker cp $(TEMP_CONTAINER):/tmp/runner/NCoreUtils.Sass.MSBuild.Runner ./NCoreUtils.Sass.MSBuild/tools/linux-x64/
	docker rm -v $(TEMP_CONTAINER)
	docker rmi $(TEMP_IMAGE)

build-linux-arm64:
	docker buildx build --load --platform linux/arm64 --build-arg=RID=linux-arm64 -t $(TEMP_IMAGE) -f misc/arm64.Dockerfile .
	docker create --name $(TEMP_CONTAINER) $(TEMP_IMAGE)
	mkdir -p ./NCoreUtils.Sass.MSBuild/tools/linux-arm64
	docker cp $(TEMP_CONTAINER):/tmp/runner/NCoreUtils.Sass.MSBuild.Runner ./NCoreUtils.Sass.MSBuild/tools/linux-arm64/
	docker rm -v $(TEMP_CONTAINER)
	docker rmi $(TEMP_IMAGE)