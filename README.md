# (WIP) Social AI - Trait-based Opinion System

## Overview

The *Trait-based Opinion System* provides non-player characters (NPCs) with a method of
determining their opinion of other characters based on their traits and the traits of
the other characters. Opinions are an integer score that represents how much a character
likes another character. Game designers can leverage this package for NPC  decision-
making and social intelligence.

This package is ideal for grand-strategy games, life/social simulation games, dating sims,
and any other game that needs a social component.

## Package contents

## Installation instructions

This package is installable via GitHub URL or from local disk.

### Installing from GitHub

This installs the most recent state of the package. It is not guaranteed to download
the most recent release. If you would like to download an official release, please use
the `Installing from tarball` instructions.

1. With Unity open, in the top toolbar navigate to `Window > Package Manager`.
2. Click the `+` icon in the top left and select `Add package from git URL...`.
3. Paste the following link `https://github.com/ShiJbey/TraitBasedOpinionSystem-Unity.git`.

### Installing from tarball

For this method, you will download a release of this package from GitHub. A list of all
available releases are listed at:
<https://github.com/ShiJbey/TraitBasedOpinionSystem-Unity/releases>.

1. Please find your desired release and under `Assets` download the `Source code (tar.gz)` option.
2. With Unity open, in the top toolbar navigate to `Window > Package Manager`.
3. Click the `+` icon in the top left and select `Add package from tarball...`.
4. Find and select the downloaded source code

## Requirements

There are no system-specific requirements.

## Limitations

Performance limitations may exist when using this package with a large number of NPCs.
However, more extensive performance profiling is required to identify quantitative
limitations.

## Workflows

If this is your first experience using this package, it is recommended that you start
by looking at the sample scenes in the `Samples` directory. By itself, the opinion-
system does not do anything. It is meant to be a tool to power decision-making and
conditional logic. For example, unlocking new content when the player reaches a certain
opinion-level with a given NPC.

The general workflow with this package is to:

1. Define new character traits using ScriptableObjects.
2. Create new NPC GameObjects and attach `OpinionAgent` script.
3. Attach a set of traits to the `OpinionAgent`.
4. Create an empty GameObject and attach a `OpinionSystemManager` script.
5. Within a separate script with your AI or story logic you reference the `OpinionAgent`
   script and call the `GetOpinionOf(...)` method to see how the agent feels about another.

## Advanced topics

* **Persisting Data Between Scenes**: Currently, there is not a best method for
persisting character opinion data between scenes. Taking advantage of JSON and Unity's
JSON Utility is probably the best option. This package does not have classes available
for translating to and from JSON. Since `JsonUtility` only serializes the public
fields on a class, developers will need to create adapter-style classes for serializing
and deserializing information from the opinion system's core code.

## Samples

I am working on providing good samples.

## Tutorials

I am working on a video that explains how this package was built and how to use it
