# Changelog

All notable changes to Pure.RelationalSchema.HashCodes are documented here.

Format follows [Keep a Changelog](https://keepachangelog.com/en/1.1.0/).

---

## [3.3.0] — 2026-03-16

### Added

- **`ColumnTypeHash`**, **`ColumnHash`**, **`IndexHash`**, **`TableHash`**,
  **`ForeignKeyHash`**, and **`SchemaHash`** now expose a full ladder of
  constructor overloads: each component of the hashed value (name, columns,
  indexes, tables, foreign keys, referencing/referenced sides, etc.) can be
  supplied either as the underlying domain object(s) or as an already
  computed `IDeterminedHash`. This allows composing a hash from
  partially pre-computed pieces instead of always rebuilding it from the
  full domain graph.
- Package validation enabled (`EnablePackageValidation`) with baseline
  version `3.2.0` to guard against unintentional breaking API changes.

---

## [3.2.0] — 2025-12-06

### Added

- Multi-targeting expanded from `net9.0` only to `net7.0`, `net8.0`,
  `net9.0`, and `net10.0`.

### Changed

- Consolidated onto `Pure.HashCodes` 2.1.0; the separate
  `Pure.HashCodes.Abstractions` and `Microsoft.NET.ILLink.Tasks` package
  references are no longer required.

---

## [3.1.0] — 2025-11-05

- Maintenance release: dependency updates only.

---

## [3.0.0] — 2025-11-04

### Changed

- **Breaking:** Hash composition for collections (a table's columns and
  indexes, a schema's tables and foreign keys) now uses `DeterminedHash`
  from `Pure.HashCodes.Abstractions` instead of `AggregatedHash` from
  `Pure.HashCodes`, changing the computed hash values for any schema,
  table, or foreign key with more than one nested item.

### Added

- Package marked AOT-compatible (`IsAotCompatible`).

---

## [2.0.0] — 2025-09-26

- Promotes the `2.0.0-preview.0.1.0` release to a stable version. No
  further hash-computation changes.

---

## [2.0.0-preview.0.1.0] — 2025-09-26

### Changed

- **Breaking:** `ForeignKeyHash` now hashes `ReferencingColumns` and
  `ReferencedColumns` (multiple columns per side) instead of a single
  `ReferencingColumn`/`ReferencedColumn`, matching the upstream move to
  composite foreign keys. This changes the computed hash for every
  foreign key.

---

## [1.0.0] — 2025-08-25

- Maintenance release: code formatting and dependency updates; no
  behavioral changes.

---

## [0.2.0] — 2025-08-21

- Maintenance release: dependency and build updates.

---

## [0.1.0] — 2025-06-25

### Added

- **`ColumnTypeHash`** — deterministic hash of an `IColumnType`'s name.
- **`ColumnHash`** — deterministic hash of an `IColumn`, combining its
  name and `ColumnTypeHash`.
- **`IndexHash`** — deterministic hash of an `IIndex`, combining its
  uniqueness flag and the hashes of its columns.
- **`TableHash`** — deterministic hash of an `ITable`, combining its
  name, column hashes, and index hashes.
- **`ForeignKeyHash`** — deterministic hash of an `IForeignKey`,
  combining the referencing and referenced table and column hashes.
- **`SchemaHash`** — deterministic hash of an `ISchema`, combining its
  name, table hashes, and foreign key hashes.

All hash types implement `IDeterminedHash` from `Pure.HashCodes` and
operate over the `Pure.RelationalSchema.Abstractions` model types.
