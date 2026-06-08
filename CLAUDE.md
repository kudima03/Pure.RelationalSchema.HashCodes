# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Commands

All `dotnet` commands must be run from the `./src` directory.

```bash
dotnet restore
dotnet build --no-restore -warnaserror /p:RunAnalyzers=true
dotnet format --verify-no-changes             # check code style (CI enforces this)
dotnet format                                  # auto-fix code style
dotnet test --no-build --verbosity normal --logger trx --collect:"XPlat Code Coverage"
dotnet pack --configuration Release -p:PackageVersion=<version> --output .
```

## Architecture

This is a **hash library** — six `sealed record` types implementing `IDeterminedHash` from `Pure.HashCodes.Abstractions`, each computing a deterministic SHA-256 digest for a different relational schema component.

**Type hierarchy by schema layer:**
- `SchemaHash` — hashes name + tables + foreign keys
- `TableHash` — hashes name + columns + indexes
- `ColumnHash` — hashes name + column type
- `ColumnTypeHash` — hashes column type name only
- `IndexHash` — hashes uniqueness flag + columns
- `ForeignKeyHash` — hashes referencing table + referencing columns + referenced table + referenced columns

All types accept either raw domain objects (`ISchema`, `ITable`, etc. from `Pure.RelationalSchema.Abstractions`) or pre-computed `IDeterminedHash` values. Each constructor variant accepts a different mix, allowing callers to reuse partial hash results.

Collections are hashed order-independently via `DeterminedHash(IEnumerable<IDeterminedHash>)`.

Each type prefixes a unique 16-byte constant before SHA-256 to prevent cross-type hash collisions.

`GetHashCode()` and `ToString()` throw `NotSupportedException` — use `IEnumerable<byte>` / `.SequenceEqual()` for comparisons.

**Multi-targeting:** net7.0, net8.0, net9.0, net10.0. `IsAotCompatible = true`.

**Package validation:** `EnablePackageValidation = true`, baseline version 3.3.0. Breaking API changes fail the build.

**Publishing:** triggered by pushing a semver tag (e.g. `3.4.0`). The tag name becomes `PackageVersion`.

## Code Style

Enforced via `.editorconfig` and `dotnet format --verify-no-changes` in CI:
- No `var` — always use explicit types
- No expression-bodied methods or constructors — use block bodies
- Expression-bodied properties and accessors are required
- Private fields: `_camelCase` prefix
- File-scoped namespaces required
- All analyzers run as warnings; `-warnaserror` in CI promotes them to errors

## Commit Messages

Do not mention Claude or AI assistance in commit messages.
