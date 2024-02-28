# Changelog

All notable changes to this project will be documented in this file.

The format is based on <https://common-changelog.org/>, and this project adheres mostly to Semantic Versioning. However, all releases before 1.0.0 have breaking changes between minor-version updates.

## [2.0.0] - 2024-02-28

Version 2.0.0's architecture has been heavily refactored on the back-end to simplify testing, reliability, and serialization (save/load). Users will find a similar experience within the inspector, but some core content authoring features have been moved around.

Below is a non-exhaustive list of changes from 1.0 to 2.0. Many file and class names have changed to simplify the project. Please visit the wiki for examples of how to utilize the latest TDRS version.

### Changed

- The `SocialEngine` inspector has been simplified
- Loading traits and social events from YAML files has been moved to supplementary loader classes
- `SocialRules` are no longer tied to traits and are now authored as separate content.
- Split `SocialEngine` into `SocialEngineController` and `SocialEngine` classes to facilitate testing
- The effect system is only utilized by social events.
- Social rule, trait, and social event YAML formats have changed to conform to serialization expectations.
- `AgentConfig` and `RelationshipConfig` have been renamed `AgentSchema` and `RelationshipSchema`.

### Added

- The new `SocialEngine` class to manage all the social state information rather than have it managed directly by a MonoBehaviour.
- ScriptableObject authoring interfaces for traits and social events
- Can now save and `SocialEngine` to/from YAML
- Added `Dont Destroy On Load` option to help  propagate the social engine across scenes
- Added `SocialEventFileLoader` class to load social events from StreamingAssets
- Added `TraitFileLoader` class to load traits from StreamingAssets
- Added `MockGameManager` to sample that facilitates initialization
- Added `MockSaveSystem` to sample to show how save load works
- Events from `TraitManager` and `StatManager` now bubble up to and can be directly subscribed to on the agent and relationship instances.
- Added support for a mutually-exclusive `RelationshipType` property on relationship instances.
- Trait instance descriptions can be overwritten when added to the agent/relationship.
- Unit tests for most systems.

### Removed

- Removed need to have effect factories be MonoBehaviours
- Removed references to "node" and "edges" within the codebase

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
