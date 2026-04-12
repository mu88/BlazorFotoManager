# BlazorFotoManager (FotoFlipper)

A Windows desktop application for managing photo exports. Load images, assign the number of copies per photo, and export all copies in one step — ideal for printing workflows. Built with Blazor Server hosted inside an Electron shell via ElectronNET.

![Combined CI / Release](https://github.com/mu88/BlazorFotoManager/actions/workflows/CI_CD.yml/badge.svg)
![Mutation Testing](https://github.com/mu88/BlazorFotoManager/actions/workflows/Mutation%20Testing.yml/badge.svg)
[![Reliability Rating](https://sonarcloud.io/api/project_badges/measure?project=mu88_BlazorFotoManager&metric=reliability_rating)](https://sonarcloud.io/summary/new_code?id=mu88_BlazorFotoManager)
[![Security Rating](https://sonarcloud.io/api/project_badges/measure?project=mu88_BlazorFotoManager&metric=security_rating)](https://sonarcloud.io/summary/new_code?id=mu88_BlazorFotoManager)
[![Maintainability Rating](https://sonarcloud.io/api/project_badges/measure?project=mu88_BlazorFotoManager&metric=sqale_rating)](https://sonarcloud.io/summary/new_code?id=mu88_BlazorFotoManager)
[![Bugs](https://sonarcloud.io/api/project_badges/measure?project=mu88_BlazorFotoManager&metric=bugs)](https://sonarcloud.io/summary/new_code?id=mu88_BlazorFotoManager)
[![Vulnerabilities](https://sonarcloud.io/api/project_badges/measure?project=mu88_BlazorFotoManager&metric=vulnerabilities)](https://sonarcloud.io/summary/new_code?id=mu88_BlazorFotoManager)
[![Code Smells](https://sonarcloud.io/api/project_badges/measure?project=mu88_BlazorFotoManager&metric=code_smells)](https://sonarcloud.io/summary/new_code?id=mu88_BlazorFotoManager)
[![Coverage](https://sonarcloud.io/api/project_badges/measure?project=mu88_BlazorFotoManager&metric=coverage)](https://sonarcloud.io/summary/new_code?id=mu88_BlazorFotoManager)
[![Mutation testing badge](https://img.shields.io/endpoint?style=flat&url=https%3A%2F%2Fbadge-api.stryker-mutator.io%2Fgithub.com%2Fmu88%2FBlazorFotoManager%2Fmain)](https://dashboard.stryker-mutator.io/reports/github.com/mu88/BlazorFotoManager/main)

## Features

- 📂 Load individual images or multi-select batches via native file dialogs
- 🔢 Set the number of copies per image using +/- controls
- ⬅️➡️ Navigate forward and backward through loaded images
- 💾 Save and load projects as JSON files for later continuation
- 📤 Export all copies to a target folder with a live progress bar
- 🌍 Localized UI (German)

## Architecture

The project follows an **Onion Architecture** (Clean Architecture) with two projects:

| Project | Layer | Responsibility |
|---------|-------|----------------|
| `FotoManagerLogic` | Domain / Business | Entities (`Project`, `Image`), interfaces, DTOs — no infrastructure dependencies |
| `FotoManager` | Application / UI | Blazor pages, `ProjectService`, ElectronNET integration |

Dependencies always point inward: `FotoManager` → `FotoManagerLogic`.

Infrastructure concerns (file I/O, Electron dialogs) are wrapped behind interfaces using the [Humble Object pattern](https://martinfowler.com/bliki/HumbleObject.html), keeping business logic fully unit-testable.

## Prerequisites

- [.NET SDK](https://dotnet.microsoft.com/download) (version pinned in `global.json`)
- [Node.js / npm](https://nodejs.org/) (required by ElectronNET CLI)
- [ElectronNET CLI](https://github.com/ElectronNET/Electron.NET): `dotnet tool install ElectronNET.CLI -g`

## Getting Started

```shell
cd FotoManager
electronize start
```

The first start takes a while as Electron downloads its binaries. Subsequent starts are faster.

## Running Tests

```shell
dotnet test --filter Category=Unit
dotnet test --filter Category=Integration
```

For mutation testing (requires the tool to be installed first via `dotnet tool restore`):

```shell
dotnet tool run dotnet-stryker
```

## Building a Release

The CI pipeline builds the Windows installer automatically via GitHub Actions on every push. For a local build:

```shell
dotnet publish FotoManager\FotoManager.csproj /p:PublishProfile=win-x64
```

The output EXE (`FotoFlipper_<version>.exe`) is placed under `FotoManager/publish/Release/`.

## Releasing

This project uses [Versionize](https://github.com/versionize/versionize) and [Conventional Commits](https://www.conventionalcommits.org/). A release is triggered automatically when a commit with the message `chore(release): X.Y.Z` is pushed to the default branch.

## License

[Do No Harm](LICENSE.md)


## Features

- 📂 Load individual images or multi-select batches via native file dialogs
- 🔢 Set the number of copies per image using +/- controls
- ⬅️➡️ Navigate forward and backward through loaded images
- 💾 Save and load projects as JSON files for later continuation
- 📤 Export all copies to a target folder with a live progress bar
- 🌍 Localized UI (German)

## Architecture

The project follows an **Onion Architecture** (Clean Architecture) with two projects:

| Project | Layer | Responsibility |
|---------|-------|----------------|
| `FotoManagerLogic` | Domain / Business | Entities (`Project`, `Image`), interfaces, DTOs — no infrastructure dependencies |
| `FotoManager` | Application / UI | Blazor pages, `ProjectService`, ElectronNET integration |

Dependencies always point inward: `FotoManager` → `FotoManagerLogic`.

Infrastructure concerns (file I/O, Electron dialogs) are wrapped behind interfaces using the [Humble Object pattern](https://martinfowler.com/bliki/HumbleObject.html), keeping business logic fully unit-testable.

## Prerequisites

- [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0) (version pinned in `global.json`)
- [Node.js / npm](https://nodejs.org/) (required by ElectronNET CLI)
- [ElectronNET CLI](https://github.com/ElectronNET/Electron.NET): `dotnet tool install ElectronNET.CLI -g`

## Getting Started

```shell
cd FotoManager
electronize start
```

The first start takes a while as Electron downloads its binaries. Subsequent starts are faster.

## Running Tests

```shell
dotnet test --filter Category=Unit
```

For mutation testing (requires the tool to be installed first via `dotnet tool restore`):

```shell
dotnet tool run dotnet-stryker
```

## Building a Release

The CI pipeline builds the Windows installer automatically via GitHub Actions on every push. For a local build:

```shell
dotnet publish FotoManager\FotoManager.csproj /p:PublishProfile=win-x64
```

The output EXE (`FotoFlipper_<version>.exe`) is placed under `FotoManager/publish/Release/`.

## Releasing

This project uses [Versionize](https://github.com/versionize/versionize) and [Conventional Commits](https://www.conventionalcommits.org/). A release is triggered automatically when a commit with the message `chore(release): X.Y.Z` is pushed to the default branch.

## License

[Do No Harm](LICENSE.md)
