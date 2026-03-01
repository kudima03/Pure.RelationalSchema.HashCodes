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

    private readonly ISchema? _schema;
    private readonly IDeterminedHash? _hash;

    public SchemaHash(ISchema schema)
    {
        _schema = schema ?? throw new ArgumentNullException(nameof(schema));
    }

    public SchemaHash(IDeterminedHash hash)
    {
        _hash = hash ?? throw new ArgumentNullException(nameof(hash));
    }

    public IEnumerator<byte> GetEnumerator()
    {
        return _hash is not null
            ? _hash.GetEnumerator()
            : new DeterminedHash(
            TypePrefix
                .Concat(new DeterminedHash(_schema!.Name))
                .Concat(
                    new DeterminedHash(
                        _schema.Tables.Select(column => new TableHash(column))
                    )
                )
                .Concat(
                    new DeterminedHash(
                        _schema.ForeignKeys.Select(foreignKey => new ForeignKeyHash(
                            foreignKey
                        ))
                    )
                )
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
