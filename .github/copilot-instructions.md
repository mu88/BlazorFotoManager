# BlazorFotoManager — Repo Context

## Artifact Name
- The published Windows executable is named **FotoFlipper** (not FotoManager). CI artifact path: `FotoFlipper_*.exe`. GitHub Release assets are also named `FotoFlipper_*.exe`.

## Existing Humble Objects — Never Bypass
Four humble objects already exist; never call the underlying APIs directly:
- `IElectronHelper` / `ElectronHelper` — wraps ElectronNET API
- `IFileSystem` / `FileSystem` — wraps `System.IO`
- `IFileHandler` / `JsonFileHandler` — wraps JSON persistence
- `ITranslator` / `Translator` — wraps localization

## Architecture Invariant
- `GetCurrentImageUrl()` belongs in `ProjectService` (Application layer, `FotoManager/`), **not** in `IProject`/`Project` (Domain layer, `FotoManagerLogic/`). URL construction is an application-level concern.

## CI: Lock Files Disabled
- `dotnet-cache-dependencies-via-lock-file: false` in `CI_CD.yml` — ElectronNET is incompatible with NuGet lock files. Do **not** introduce `packages.lock.json`.

## bUnit Tests
- Use `BunitContext` (new bunit 2.x API) — not `TestContext` (deprecated). Import alias: `using BunitContext = Bunit.BunitContext;`
- NSubstitute 5.x auto-mocks interface properties — but `IProject.CurrentImage` requires an explicit `.Returns(default(IImage))` to avoid null reference exceptions in tests.
