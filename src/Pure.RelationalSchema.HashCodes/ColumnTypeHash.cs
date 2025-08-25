using System.Collections;
using Pure.HashCodes;
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

    private readonly IColumnType _columnType;

    public ColumnTypeHash(IColumnType columnType)
    {
        _columnType = columnType;
    }

    public IEnumerator<byte> GetEnumerator()
    {
        return new DeterminedHash(
            TypePrefix.Concat(new DeterminedHash(_columnType.Name))
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
