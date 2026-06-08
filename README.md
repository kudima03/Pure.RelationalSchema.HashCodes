# Pure.RelationalSchema.HashCodes

Deterministic hash implementations for relational schema components in the **Pure** ecosystem — schemas, tables, columns, indexes, and foreign keys as composable `IDeterminedHash` byte sequences.

[![.NET build & test](https://github.com/kudima03/Pure.RelationalSchema.HashCodes/actions/workflows/build-and-test.yml/badge.svg?branch=main)](https://github.com/kudima03/Pure.RelationalSchema.HashCodes/actions/workflows/build-and-test.yml)
[![Build and Deploy](https://github.com/kudima03/Pure.RelationalSchema.HashCodes/actions/workflows/publish-nuget.yml/badge.svg?branch=main)](https://github.com/kudima03/Pure.RelationalSchema.HashCodes/actions/workflows/publish-nuget.yml)
[![NuGet](https://img.shields.io/nuget/v/Pure.RelationalSchema.HashCodes)](https://www.nuget.org/packages/Pure.RelationalSchema.HashCodes)
[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](LICENSE)

## Overview

`Pure.RelationalSchema.HashCodes` provides deterministic, SHA-256-based hash records for every component in a relational schema. Each type accepts either raw domain objects (from `Pure.RelationalSchema.Abstractions`) or pre-computed `IDeterminedHash` values, enabling incremental hashing and efficient reuse of previously computed sub-hashes.

## Hash Types

| Type | Inputs hashed |
|------|---------------|
| `SchemaHash` | name, tables collection, foreign keys collection |
| `TableHash` | name, columns collection, indexes collection |
| `ColumnHash` | name, column type |
| `ColumnTypeHash` | column type name |
| `IndexHash` | uniqueness flag, columns collection |
| `ForeignKeyHash` | referencing table, referencing columns, referenced table, referenced columns |

Every type is a `sealed record` implementing `IDeterminedHash` and `IEnumerable<byte>` — the underlying bytes of its SHA-256 digest. Calling `GetHashCode()` or `ToString()` throws `NotSupportedException` by design; use byte enumeration for equality checks.

Each type embeds a unique 16-byte domain-separation prefix before hashing, so hashes of structurally identical values from different types are never equal.

Collections (tables, columns, indexes, foreign keys) are hashed in an order-independent manner — reversing a collection produces the same hash.

## Design Principles

- **Deterministic** — identical schema structures always produce the same byte sequence.
- **Composable** — every constructor accepts either raw domain objects or pre-computed `IDeterminedHash` dependencies, so callers decide what to recompute vs. reuse.
- **Domain-separated** — each type prefixes a fixed 16-byte token before SHA-256, preventing cross-type collisions.

## Dependencies

- [`Pure.HashCodes`](https://github.com/kudima03/Pure.HashCodes/tree/2.1.0) — core `IDeterminedHash` abstraction and `DeterminedHash` implementation
- [`Pure.RelationalSchema.Abstractions`](https://github.com/kudima03/Pure.RelationalSchema.Abstractions/tree/1.2.0) — `ISchema`, `ITable`, `IColumn`, `IColumnType`, `IIndex`, `IForeignKey` interfaces

## Target Frameworks

- .NET 7
- .NET 8
- .NET 9
- .NET 10

## Installation

```bash
dotnet add package Pure.RelationalSchema.HashCodes
```

## Usage

```csharp
ISchema schema = /* ... */;

// Hash a full schema
byte[] hash = new SchemaHash(schema).ToArray();

// Reuse a pre-computed sub-hash
IDeterminedHash tableHash = new TableHash(table);
SchemaHash schemaHash = new SchemaHash(schemaName, tableHash, foreignKeyHash);

// Compare two schemas structurally
bool equal = new SchemaHash(schemaA).SequenceEqual(new SchemaHash(schemaB));
```
