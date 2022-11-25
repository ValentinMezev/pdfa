# Aspose Prototype

## Prerequisites
If running on macos you need to install `mono-libgdiplus` from brew.  
Running on arm64 you have to ensure the dylib can be found at runtime.
```sh
brew install mono-libgdiplus
export DYLD_LIBRARY_PATH=/opt/homebrew/lib
```
Running on x86_64 *I think* the libs get linked into `/usr/local/lib` automatically which is already included in `DYLD_FALLBACK_LIBRARY_PATH`

## Conversion
```sh
dotnet run --project Aspose/Aspose.csproj convert --input Aspose/Samples/input.pdf
open Examples/output.pdf
```

## Licensing
License expires 24/12/2022 at which point a watermark will appear on the converted PDF.
