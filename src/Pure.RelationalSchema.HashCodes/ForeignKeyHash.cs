using System.Collections;
using Pure.HashCodes;
using Pure.HashCodes.Abstractions;
using Pure.RelationalSchema.Abstractions.ForeignKey;

namespace Pure.RelationalSchema.HashCodes;

public sealed record ForeignKeyHash : IDeterminedHash
{
    private static readonly byte[] TypePrefix =
    [
        196,
        165,
        151,
        1,
        153,
        27,
        106,
        112,
        143,
        29,
        159,
        81,
        46,
        52,
        46,
        148,
    ];

    private readonly IForeignKey? _foreignKey;
    private readonly IDeterminedHash? _hash;

    public ForeignKeyHash(IForeignKey foreignKey)
    {
        _foreignKey = foreignKey ?? throw new ArgumentNullException(nameof(foreignKey));
    }

    public ForeignKeyHash(IDeterminedHash hash)
    {
        _hash = hash ?? throw new ArgumentNullException(nameof(hash));
    }

    public IEnumerator<byte> GetEnumerator()
    {
        return _hash is not null
            ? _hash.GetEnumerator()
            : new DeterminedHash(
            TypePrefix
                .Concat(new TableHash(_foreignKey!.ReferencingTable))
                .Concat(
                    new DeterminedHash(
                        _foreignKey.ReferencingColumns.Select(x => new ColumnHash(x))
                    )
                )
                .Concat(new TableHash(_foreignKey.ReferencedTable))
                .Concat(
                    new DeterminedHash(
                        _foreignKey.ReferencedColumns.Select(x => new ColumnHash(x))
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
