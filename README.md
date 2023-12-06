# Trait-Driven Relationship System (TDRS) for Unity

![Static Badge](https://img.shields.io/badge/Unity-2022.3-black)
![Static Badge](https://img.shields.io/badge/Project_Status-Unstable-red)
![Static Badge](https://img.shields.io/badge/Version-1.0.0_Unreleased-yellow)

**Unity-TDRS** is a toolkit for modeling dynamic character relationships for life sims, dating sims, visual novels, and adventure games in Unity. It enables game developers to track relationships between social entities (NPCs, players, factions, etc.). Designers can create and tag relationships and characters with various traits that modify how characters feel about each other. Also they can dispatch various events that change relationships and build interpersonal histories between characters. Game designers can leverage this package for NPC decision-making, customizing dialogue, and adding a semblance of social intelligence.

## Features

- ❤️ Model relationships between NPC's, Social Groups, and Player(s)
- 📊 Track various relationship stats like friendship, romance, trust, and reputation
- 🏷️ Tag characters and relationships with various traits to influence stats
- 📏 Associate traits with social rules that change how characters treat others
- 🎊 Uses an expressive event system to propagate information

## What this project is **NOT**

This project does not contain code for autonomous character decision making or text generation. TDRS is a solution for modeling dynamic relationships and making the data available to other systems in a game. You can think of it like a physics engine. It exists to do one thing well and support the rest of the design.

## Project contents

This repository contains a Unity project with Unity-TDRS added as an embedded package. Package-specific code can be found in the `Packages/Unity-TDRS` directory. All other data found in the `Assets` folder belong to the samples and are not included in the final package distribution.

## Installation instructions

The following are instructions for installing `Unity-TDRS` into your Unity project. This project uses semantic versioning. So major version numbers will have breaking changes. Minor version changes will mostly contain new features, but there is always a chance of a breaking change. Please check the [CHANGE LOG](./CHANGELOG.md) or release notes to see what changed between releases.

### Installing from tarball (Recommended)

It's recommended that you download the latest release from the [Releases](https://github.com/ShiJbey/Unity-TDRS/releases) page, as the GitHub version might be unstable. Also, you can access previous releases, if needed.

1. Please find your desired release and under `Assets` download the `Source code (tar.gz)` option.
2. With Unity open, in the top toolbar navigate to `Window > Package Manager`.
3. Click the `+` icon in the top left and select `Add package from tarball...`.
4. Find and select the downloaded source code

### Installing from GitHub

This installs the current version of the package as it is on the main repository branch. This method is **NOT** guaranteed to download the most recent/stable release. Use this method if there are no releases available or you want to try out experimental and unfinished features. However, If you would like to download a stable release, please use the preceding `Installing from tarball` instructions.

1. With Unity open, in the top toolbar navigate to `Window > Package Manager`.
2. Click the `+` icon in the top left and select `Add package from git URL...`.
3. Paste the following link `https://github.com/ShiJbey/Unity-TDRS.git`.

## Dependencies

- Unity version 2022.3 or more recent

## Limitations

- Unity-TDRS is not optimized to handle large number of NPCs. So, some users might encounter performance slow-downs if they are trying to make something like Crusader Kings. More extensive performance profiling is required to investigate this.

## Documentation and Workflows

You can learn more about TDRS and how to get started using it by visiting the [Wiki](https://github.com/ShiJbey/Unity-TDRS/wiki). There are pages that will walk you through common tasks and workflows.

## Frequently asked questions

### How do I persist data between scenes

Currently, there is not a best method for persisting character opinion data between scenes. Taking advantage of JSON and Unity's JSON Utility is probably the best option. This package does not have classes available for translating to and from JSON. Since `JsonUtility` only serializes the public fields on a class, developers will need to create adapter-style classes for serializing and deserializing information from the opinion system's core code.

## To-Do List

- [ ] Create MonoBehaviour for configuring default character stats
- [ ] Outline workflow steps for defining relationship prefabs
- [ ] Add tests for trait addition and removal on characters and relationships
- [ ] Add default implementations for basic effects
