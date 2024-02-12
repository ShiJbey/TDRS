# Trait-Driven Relationship System (TDRS) for Unity

![Static Badge](https://img.shields.io/badge/Unity-2022.3-black)
![Static Badge](https://img.shields.io/badge/Version-2.0.0-green)

**TDRS** (Trait-Driven Relationship System) is a Unity package for modeling dynamic character relationships in RPGs, simulation games, visual novels, and adventure games. Designers can model character personalities, emotions, affinities, and relationship statuses. They can also define various social events that NPCs can respond to, given their relationships with the party(s) involved. TDRS aims to empower designers to create engaging, relationship-driven gameplay where the immediate and second-order effects of various social interactions drive NPC decision-making and narrative progression.

Designers can model NPC personalities, emotions, relationship statuses, and interpersonal affinities using combinations of numerical stats and traits. Traits are the driving force of TDRS, as its name implies. They are tags of information attached to NPCs and relationships that provide additional semantic information and may apply various effects or social rules that change how a character treats another.

## Features

- ❤️ Model relationships between Agents (NPCs, Groups, and Player(s))
- 📊 Track various agent and relationship stat values like sociability, confidence, friendship, romance, trust, and reputation.
- 🏷️ Tag agents and relationships with various traits to influence stats
- 📏 Associate traits with social rules that change how characters treat others
- 🎊 Dispatch custom social events that propagate through the social network and change relationships

## What this project is **NOT**

This project does not contain code for autonomous character decision-making or text generation. TDRS is a solution for modeling dynamic relationships and making the data available to other systems in a game. You can think of it like a physics engine. It exists to do one thing well and support the rest of the design.

## Project contents

This repository contains a Unity project with TDRS added as an embedded package. The package-specific code can be found in the `Packages/TDRS` directory. All other data found in the `Assets` folder belong to the samples.

## Installation instructions

The following are instructions for installing `TDRS` into your Unity project. This project uses semantic versioning. So, major version numbers will have breaking changes. Minor version changes will mostly contain new features, but there is always a chance of a breaking change. Please check the [CHANGE LOG](./CHANGELOG.md) or release notes to see what changed between releases.

To add TDRS to your Unity project, you must download the latest version of the package from GitHub. TDRS is not available in the Unity Asset store. All releases are on the [TDRS GitHub Releases page](https://github.com/ShiJbey/TDRS/releases). Please follow the steps below.

1. Find your desired release.
2. Download the `tdrs_<VERSION>.tar.gz` from under the `Assets` dropdown (\<VERSION\> should be the release version you intend to download).
3. Open your project in Unity
4. Navigate to `Window > Package Manager` in the top menu.
5. Click the `+` icon in the top left and select `Add package from tarball...`.
6. Find and select the downloaded tarball
7. You should now see TDRS appear in the Unity Package Manager window with a version number matching your downloaded version.
8. Close the Package Manager window

## Dependencies

- Unity version 2022.3 or more recent

## Limitations

- TDRS is not optimized to handle a large number of NPCs. Some users might encounter performance slow-downs if they try to make something like Crusader Kings. More extensive performance profiling is required to investigate this.

## Documentation and Workflows

You can learn more about TDRS and how to get started by visiting the [Wiki](https://github.com/ShiJbey/TDRS/wiki). There are pages to walk you through common tasks and workflows.
