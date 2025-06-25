using Pure.HashCodes;
using Pure.RelationalSchema.Abstractions.Column;
using Pure.RelationalSchema.Abstractions.Index;
using System.Collections;

namespace Pure.RelationalSchema.HashCodes;

public sealed record IndexHash : IDeterminedHash
{
    private static readonly byte[] TypePrefix =
    [
        142, 165, 151, 1, 117, 182, 22, 125, 191, 1, 173, 241, 145, 57, 67, 244
    ];

    private readonly IIndex _index;

    public IndexHash(IIndex index)
    {
        _index = index;
    }

    public IEnumerator<byte> GetEnumerator()
    {
        return new DeterminedHash(TypePrefix
                .Concat(new DeterminedHash(_index.IsUnique))
                .Concat(new AggregatedHash(_index.Columns.Select(column => new ColumnHash(column)))))
            .GetEnumerator();
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