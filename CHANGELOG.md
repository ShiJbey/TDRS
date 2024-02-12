# Changelog

All notable changes to this project will be documented in this file.

The format is based on <https://common-changelog.org/>, and this project adheres mostly to Semantic Versioning. However, all releases before 1.0.0 have breaking changes between minor-version updates.

## [2.0.0] - Unreleased

Version 2.0.0's architecture has been refactored from the previous major version to better support save/load and portability.

### Changed

- The `SocialEngine` inspector has been simplified
- Loading traits and social events from YAML files has been moved to supplementary loader classes
- Split `SocialEngine` in to `SocialEngineController` and `SocialEngine` classes to facilitate testing

### Added

- The new `SocialEngine` class to manage all the social state information rather than have it managed directly by a MonoBehaviour.
- ScriptableObject authoring interfaces for traits and social events
- Can now save and `SocialEngine` to/from YAML
- Added `Dont Destroy On Load` option to help  propagate the social engine across scenes
- Added `SocialEventFileLoader` class to load social events from StreamingAssets
- Added `TraitFileLoader` class to load traits from StreamingAssets
- Added `MockGameManager` to sample that facilitates initialization
- Added `MockSaveSystem` to sample to show how save load works

### Removed

- Removed need to have effect factories be MonoBehaviours

## [1.1.0] - 2024-01-25

### Changed

- **breaking:** Rename `SocialEventType` to `SocialEvent`
- **breaking:** Pluralized `stats` within the RePraxis database

### Added

- Users can create traits and social events using ScriptableObjects
- `DontDestroyOnLoad` option for `SocialEngine`

### Fixed

- Corrected `EffectName` property of `RemoveAgentTraitFactory` to `RemoveAgentTrait`

## [1.0.0] - 2024-01-11

_Initial release._

[1.0.0]: https://github.com/ShiJbey/TDRS/releases/tag/v1.0.0
[1.1.0]: https://github.com/ShiJbey/TDRS/releases/tag/v1.1.0
[2.0.0]: https://github.com/ShiJbey/TDRS/releases/tag/v2.0.0
