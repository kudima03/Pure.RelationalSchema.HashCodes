using System.Collections;
using Pure.HashCodes;
using Pure.HashCodes.Abstractions;
using Pure.RelationalSchema.Abstractions.Column;

namespace Pure.RelationalSchema.HashCodes;

public sealed record ColumnHash : IDeterminedHash
{
    private static readonly byte[] TypePrefix =
    [
        41,
        163,
        151,
        1,
        29,
        173,
        73,
        119,
        138,
        216,
        188,
        7,
        188,
        71,
        127,
        69,
    ];

    private readonly IColumn? _column;
    private readonly IDeterminedHash? _hash;

    public ColumnHash(IColumn column)
    {
        _column = column ?? throw new ArgumentNullException(nameof(column));
    }

    public ColumnHash(IDeterminedHash hash)
    {
        _hash = hash ?? throw new ArgumentNullException(nameof(hash));
    }

    public IEnumerator<byte> GetEnumerator()
    {
        return _hash is not null
            ? _hash.GetEnumerator()
            : new DeterminedHash(
            TypePrefix
                .Concat(new DeterminedHash(_column!.Name))
                .Concat(new ColumnTypeHash(_column.Type))
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
