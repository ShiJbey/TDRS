# Trait-Driven Relationship System (TDRS) for Unity

![Static Badge](https://img.shields.io/badge/Unity-2022.3-black)
![Static Badge](https://img.shields.io/badge/Project_Status-Unstable-red)
![Static Badge](https://img.shields.io/badge/Version-1.0.0_Unreleased-yellow)

**Unity-TDRS** is a toolkit for modeling dynamic character relationships for life sims, dating sims, visual novels, and adventure games in Unity. It enables game developers to track relationships between social entities (NPCs, players, factions, etc.). Designers can create and tag relationships and characters with various traits that modify how characters feel about each other. Also, they can dispatch various event types that change relationships and build interpersonal histories between characters. Game designers can leverage this package for NPC decision-making, customizing dialogue, and adding a semblance of social intelligence.

## Features

- ❤️ Model relationships between NPCs, Social Groups, and Player(s)
- 📊 Track various relationship stats like friendship, romance, trust, and reputation
- 🏷️ Tag characters and relationships with various traits to influence stats
- 📏 Associate traits with social rules that change how characters treat others
- 🎊 Uses an expressive event system to propagate information

## What this project is **NOT**

This project does not contain code for autonomous character decision-making or text generation. TDRS is a solution for modeling dynamic relationships and making the data available to other systems in a game. You can think of it like a physics engine. It exists to do one thing well and support the rest of the design.

## Project contents

This repository contains a Unity project with Unity-TDRS added as an embedded package. The package-specific code can be found in the `Packages/Unity-TDRS` directory. All other data found in the `Assets` folder belong to the samples and are not included in the final package distribution.

## Installation instructions

The following are instructions for installing `Unity-TDRS` into your Unity project. This project uses semantic versioning. So, major version numbers will have breaking changes. Minor version changes will mostly contain new features, but there is always a chance of a breaking change. Please check the [CHANGE LOG](./CHANGELOG.md) or release notes to see what changed between releases.

### Installing from tarball (Recommended)

You should download the latest release from the [Releases](https://github.com/ShiJbey/Unity-TDRS/releases) page, as the GitHub version might be unstable. Also, you can access previous releases if needed.

1. Find your desired release.
2. Download the `unity-tdrs_<VERSION>.tar.gz` from under the `Assets` dropdown (\<VERSION\> should be the release version you intend to download).
3. Open your project in Unity
4. Navigate to `Window > Package Manager` in the top menu.
5. Click the `+` icon in the top left and select `Add package from tarball...`.
6. Find and select the downloaded tarball

### Installing using GitHub link

This method installs the current version of the package as it is on the `main` repository branch. Please note that installing using a GitHub link is  **NOT** guaranteed to download the most recent/stable release. Use this method if no releases are available or you want to try experimental and unfinished features. However, if you would like to download a stable release, please follow the preceding `Installing from tarball` instructions.

1. Open your project in Unity
2. Navigate to `Window > Package Manager` in the top menu.
3. Click the `+` icon in the top left and select `Add package from git URL...`.
4. Paste the following URL `https://github.com/ShiJbey/Unity-TDRS.git`

## Dependencies

- Unity version 2022.3 or more recent

## Limitations

- Unity-TDRS is not optimized to handle a large number of NPCs. Some users might encounter performance slow-downs if they try to make something like Crusader Kings. More extensive performance profiling is required to investigate this.

## Documentation and Workflows

You can learn more about TDRS and how to get started by visiting the [Wiki](https://github.com/ShiJbey/Unity-TDRS/wiki). There are pages to walk you through common tasks and workflows.

## Frequently asked questions

### How do I persist data between scenes

Unity-TDRS does not offer a particular solution for persisting social data between scenes. This task is left to the discretion of the user. If you have ideas for a solution, please create a GitHub issue explaining your approach.

## To-Do List

- [ ] Write unit tests for adding and removing traits.
- [ ] Write unit tests for adding and removing social rules.
- [ ] Write unit tests for loading trait information from YAML.
- [ ] Write unit tests for TDRSManager public interface methods.
- [x] Add default implementations for basic effects
- [x] Implement a custom inspector for `TDRSEntity` components to display stat, trait, and relationship data.
- [ ] Implement social event system.
