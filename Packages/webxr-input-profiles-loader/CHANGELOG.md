# Changelog
All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [Unreleased]

## [0.6.2] - 2024-01-17
### Fixed
- Pose error after latest glTFast updates.

## [0.6.1] - 2024-01-17
### Added
- glTFShaderVariantsURP and glTFShaderVariantsLegacyURP to the ShaderVariants folder.

### Changed
- Renamed GLTFShaderVariants to glTFShaderVariantsBuiltInRP.

## [0.6.0] - 2024-01-15
### Changed
- Use glTFast package from Unity registry.
- Updated glTFast package to 5.2.0.

## [0.5.0] - 2024-01-11
### Changed
- Updated glTFast package to 5.1.0.

## [0.4.0] - 2021-12-12
### Changed
- Updated glTFast package to 4.4.8.
- Minimum Unity version 2019.4.33f1.

## [0.3.7] - 2021-06-19
### Changed
- Updated glTFast package to 4.0.1.

## [0.3.6] - 2021-05-06
### Added
- GetChildTransform to InputProfileModel for WebXR Hands API support.

### Changed
- Updated glTFast package to 3.2.1.

## [0.3.5] - 2021-02-13
### Fixed
- Warning in DeferAgent file.

### Changed
- Updated glTFast package to 3.0.2.
- TimeBudgetDeferAgent to WebGLDeferAgent.
- Sample scene lighting settings.

## [0.3.4] - 2021-02-07
### Fixed
- More issues with loading models.

## [0.3.3] - 2021-02-06
### Fixed
- Issues with loading models.

## [0.3.2] - 2021-02-06
### Changed
- Updated glTFast package to 3.0.1.

## [0.3.1] - 2020-12-19
### Changed
- Increment version due to error.

## [0.3.0] - 2020-12-19
### Changed
- InputProfileLoader - Can set profilesUrl only on LoadProfilesList.
- Moved ShaderVariants folder to package Runtime folder from Samples folder.
- Use custom DeferAgent for loading gLTF models, for better loading performance.
- Make sure to download a profile only once.
- Updated glTFast package to 2.5.0.

## [0.2.0] - 2020-12-06
### Added
- Support profilesList.json file.

## [0.1.0] - 2020-11-21
### Added
- This package.
