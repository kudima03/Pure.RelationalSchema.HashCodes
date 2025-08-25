using Pure.HashCodes;
using Pure.RelationalSchema.Abstractions.Index;
using Pure.RelationalSchema.Abstractions.Table;
using System.Collections;

namespace Pure.RelationalSchema.HashCodes;

public sealed record TableHash : IDeterminedHash
{
    private static readonly byte[] TypePrefix =
    [
        184,
        165,
        151,
        1,
        198,
        98,
        50,
        119,
        182,
        181,
        80,
        101,
        192,
        154,
        105,
        5,
    ];

    private readonly ITable _table;

    public TableHash(ITable table)
    {
        _table = table;
    }

    public IEnumerator<byte> GetEnumerator()
    {
        return new DeterminedHash(
            TypePrefix
                .Concat(new DeterminedHash(_table.Name))
                .Concat(new AggregatedHash(_table.Columns.Select(column => new ColumnHash(column))))
                .Concat(new AggregatedHash(_table.Indexes.Select(index => new IndexHash(index))))
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