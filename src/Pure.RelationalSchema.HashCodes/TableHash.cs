using System.Collections;
using Pure.HashCodes;
using Pure.HashCodes.Abstractions;
using Pure.RelationalSchema.Abstractions.Table;

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

    private readonly IDeterminedHash _nameHash;
    private readonly IDeterminedHash _columnsHash;
    private readonly IDeterminedHash _indexesHash;

    public TableHash(ITable table)
        : this(
            new DeterminedHash(table.Name),
            new DeterminedHash(table.Columns.Select(c => new ColumnHash(c.Name, c.Type))),
            new DeterminedHash(table.Indexes.Select(i => new IndexHash(i.IsUnique, i.Columns)))
        )
    { }

    public TableHash(
        IDeterminedHash nameHash,
        IDeterminedHash columnsHash,
        IDeterminedHash indexesHash)
    {
        _nameHash = nameHash;
        _columnsHash = columnsHash;
        _indexesHash = indexesHash;
    }

    public IEnumerator<byte> GetEnumerator()
    {
        return new DeterminedHash(
            TypePrefix
                .Concat(_nameHash)
                .Concat(_columnsHash)
                .Concat(_indexesHash)
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
