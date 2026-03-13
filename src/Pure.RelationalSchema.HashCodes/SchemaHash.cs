using System.Collections;
using Pure.HashCodes;
using Pure.HashCodes.Abstractions;
using Pure.Primitives.Abstractions.String;
using Pure.RelationalSchema.Abstractions.ForeignKey;
using Pure.RelationalSchema.Abstractions.Schema;
using Pure.RelationalSchema.Abstractions.Table;

namespace Pure.RelationalSchema.HashCodes;

public sealed record SchemaHash : IDeterminedHash
{
    private static readonly byte[] TypePrefix =
    [
        253,
        165,
        151,
        1,
        96,
        51,
        234,
        121,
        155,
        41,
        25,
        146,
        55,
        243,
        188,
        110,
    ];

    private readonly IDeterminedHash _nameHash;
    private readonly IDeterminedHash _tablesHash;
    private readonly IDeterminedHash _foreignKeysHash;

    public SchemaHash(ISchema schema) :
        this(
            schema.Name,
            schema.Tables,
            schema.ForeignKeys
        )
    { }

    public SchemaHash(IString name, IEnumerable<ITable> tables, IEnumerable<IForeignKey> foreignKeys)
        : this(
              new DeterminedHash(name),
              tables,
              foreignKeys
        )
    { }

    public SchemaHash(IDeterminedHash nameHash, IEnumerable<ITable> tables, IEnumerable<IForeignKey> foreignKeys)
        : this(
              nameHash,
              new DeterminedHash(tables.Select(t => new TableHash(t))),
              foreignKeys
        )
    { }

    public SchemaHash(IString name, IDeterminedHash tablesHash, IEnumerable<IForeignKey> foreignKeys)
        : this(
              name,
              tablesHash,
              new DeterminedHash(foreignKeys.Select(fk => new ForeignKeyHash(fk)))
        )
    { }

    public SchemaHash(IString name, IEnumerable<ITable> tables, IDeterminedHash foreignKeysHash)
        : this(
              new DeterminedHash(name),
              tables,
              foreignKeysHash
        )
    { }

    public SchemaHash(IDeterminedHash nameHash, IDeterminedHash tablesHash, IEnumerable<IForeignKey> foreignKeys)
        : this(
              nameHash,
              tablesHash,
              new DeterminedHash(foreignKeys.Select(fk => new ForeignKeyHash(fk)))
        )
    { }

    public SchemaHash(IDeterminedHash nameHash, IEnumerable<ITable> tables, IDeterminedHash foreignKeysHash)
        : this(
              nameHash,
              new DeterminedHash(tables.Select(t => new TableHash(t))),
              foreignKeysHash
        )
    { }

    public SchemaHash(IString name, IDeterminedHash tablesHash, IDeterminedHash foreignKeysHash)
        : this(
              new DeterminedHash(name),
              tablesHash,
              foreignKeysHash
        )
    { }

    public SchemaHash(IDeterminedHash nameHash, IDeterminedHash tablesHash, IDeterminedHash foreignKeysHash)
    {
        _nameHash = nameHash;
        _tablesHash = tablesHash;
        _foreignKeysHash = foreignKeysHash;
    }

    public IEnumerator<byte> GetEnumerator()
    {
        return new DeterminedHash(
            TypePrefix
                .Concat(_nameHash)
                .Concat(_tablesHash)
                .Concat(_foreignKeysHash)
        ).GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    public override int GetHashCode()
    {
        throw new NotSupportedException();
    }

    public override string ToString()
    {
        throw new NotSupportedException();
    }
}
