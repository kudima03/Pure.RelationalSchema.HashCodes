using System.Collections;
using Pure.HashCodes;
using Pure.HashCodes.Abstractions;
using Pure.Primitives.Abstractions.String;
using Pure.RelationalSchema.Abstractions.Column;
using Pure.RelationalSchema.Abstractions.Index;
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

    public TableHash(ITable table) :
        this(
            table.Name,
            table.Columns,
            table.Indexes
            )
    { }

    public TableHash(IString name, IEnumerable<IColumn> columns, IEnumerable<IIndex> indexes)
        : this(
            new DeterminedHash(name),
            columns,
            indexes
        )
    { }

    public TableHash(IDeterminedHash nameHash, IEnumerable<IColumn> columns, IEnumerable<IIndex> indexes)
        : this(
            nameHash,
            new DeterminedHash(columns.Select(c => new ColumnHash(c))),
            indexes
        )
    { }

    public TableHash(IString name, IDeterminedHash columnsHash, IEnumerable<IIndex> indexes)
        : this(
            name,
            columnsHash,
            new DeterminedHash(indexes.Select(i => new IndexHash(i)))
        )
    { }

    public TableHash(IString name, IEnumerable<IColumn> columns, IDeterminedHash indexesHash)
        : this(
            new DeterminedHash(name),
            columns,
            indexesHash
        )
    { }

    public TableHash(IDeterminedHash nameHash, IDeterminedHash columnsHash, IEnumerable<IIndex> indexes)
        : this(
            nameHash,
            columnsHash,
            new DeterminedHash(indexes.Select(i => new IndexHash(i)))
        )
    { }

    public TableHash(IDeterminedHash nameHash, IEnumerable<IColumn> columns, IDeterminedHash indexesHash)
        : this(
            nameHash,
            new DeterminedHash(columns.Select(c => new ColumnHash(c))),
            indexesHash
        )
    { }

    public TableHash(IString name, IDeterminedHash columnsHash, IDeterminedHash indexesHash)
        : this(
            new DeterminedHash(name),
            columnsHash,
            indexesHash
        )
    { }

    public TableHash(IDeterminedHash nameHash, IDeterminedHash columnsHash, IDeterminedHash indexesHash)
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
