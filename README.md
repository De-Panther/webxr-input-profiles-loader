# WebXR Input Profiles Loader

WebXR Input Profiles Loader in Unity. Based on [WebXR Input Profiles](https://immersive-web.github.io/webxr-input-profiles/).

You can [try the demo here](https://de-panther.github.io/webxr-input-profiles-loader/).

The package is intended to be used with WebXR exporters in general, and not only with [WebXR Export](https://github.com/De-Panther/unity-webxr-export).

The package uses [glTFast](https://github.com/atteneder/glTFast) to load the models, and Unity's package of [Newtonsoft Json.NET](https://docs.unity3d.com/Packages/com.unity.nuget.newtonsoft-json@latest) to parse the profiles.

## Downloads

You can get the package from [![openupm](https://img.shields.io/npm/v/com.de-panther.webxr-input-profiles-loader?label=openupm&registry_uri=https://package.openupm.com)](https://openupm.com/packages/com.de-panther.webxr-input-profiles-loader/).

### Setting package using OpenUPM

Set a `Scoped Registry` in `Project Settings > Package Manager` for OpenUPM.

```
Name: OpenUPM
URL: https://package.openupm.com
Scope(s): com.de-panther
          com.atteneder
```

Then in `Window > Package Manager` selecting `Packages: My Registries` and the WebXR Input Profiles Loader package would be available for install.

## Troubleshooting

You might need to add Shader Variants of the Input Profiles Models to the build for the shaders to work. More info about that can be found at the glTFast docs.

## Supported WebXR Exporters

- [WebXR Export](https://github.com/De-Panther/unity-webxr-export).
