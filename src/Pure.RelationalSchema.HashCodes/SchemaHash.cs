using System.Collections;
using Pure.HashCodes;
using Pure.HashCodes.Abstractions;
using Pure.RelationalSchema.Abstractions.Schema;

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

    public SchemaHash(ISchema schema)
        : this(
            new DeterminedHash(schema.Name),
            new DeterminedHash(schema.Tables.Select(table => new TableHash(table))),
            new DeterminedHash(schema.ForeignKeys.Select(fk => new ForeignKeyHash(fk)))
        )
    { }

    public SchemaHash(
        IDeterminedHash nameHash,
        IDeterminedHash tablesHash,
        IDeterminedHash foreignKeysHash)
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
