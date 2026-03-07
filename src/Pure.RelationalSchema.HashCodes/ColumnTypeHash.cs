using System.Collections;
using Pure.HashCodes;
using Pure.HashCodes.Abstractions;
using Pure.RelationalSchema.Abstractions.ColumnType;

namespace Pure.RelationalSchema.HashCodes;

public sealed record ColumnTypeHash : IDeterminedHash
{
    private static readonly byte[] TypePrefix =
    [
        8,
        157,
        151,
        1,
        149,
        98,
        28,
        119,
        130,
        158,
        187,
        34,
        130,
        255,
        222,
        135,
    ];

    private readonly IColumnType? _columnType;
    private readonly IDeterminedHash? _hash;

    public ColumnTypeHash(IColumnType columnType)
    {
        _columnType = columnType ?? throw new ArgumentNullException(nameof(columnType));
    }

    public ColumnTypeHash(IDeterminedHash hash)
    {
        _hash = hash ?? throw new ArgumentNullException(nameof(hash));
    }

    public IEnumerator<byte> GetEnumerator()
    {
        return _hash is not null
            ? _hash.GetEnumerator()
            : new DeterminedHash(
            TypePrefix.Concat(new DeterminedHash(_columnType!.Name))
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
