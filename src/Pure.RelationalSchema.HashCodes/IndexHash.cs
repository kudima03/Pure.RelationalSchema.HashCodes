using System.Collections;
using Pure.HashCodes;
using Pure.HashCodes.Abstractions;
using Pure.Primitives.Abstractions.Bool;
using Pure.RelationalSchema.Abstractions.Column;
using Pure.RelationalSchema.Abstractions.Index;

namespace Pure.RelationalSchema.HashCodes;

public sealed record IndexHash : IDeterminedHash
{
    private static readonly byte[] TypePrefix =
    [
        142,
        165,
        151,
        1,
        117,
        182,
        22,
        125,
        191,
        1,
        173,
        241,
        145,
        57,
        67,
        244,
    ];

    private readonly IDeterminedHash _isUniqueHash;
    private readonly IDeterminedHash _columnsHash;

    public IndexHash(IIndex index) :
        this(
            index.IsUnique,
            index.Columns
        )
    { }

    public IndexHash(IBool isUnique, IEnumerable<IColumn> columns)
        : this(
            new DeterminedHash(isUnique),
            columns
        )
    { }

    public IndexHash(IDeterminedHash isUniqueHash, IEnumerable<IColumn> columns)
        : this(
            isUniqueHash,
            new DeterminedHash(columns.Select(c => new ColumnHash(c)))
        )
    { }

    public IndexHash(IBool isUnique, IDeterminedHash columnsHash)
        : this(
              new DeterminedHash(isUnique),
              columnsHash)
    { }

    public IndexHash(IDeterminedHash isUniqueHash, IDeterminedHash columnsHash)
    {
        _isUniqueHash = isUniqueHash;
        _columnsHash = columnsHash;
    }

    public IEnumerator<byte> GetEnumerator()
    {
        return new DeterminedHash(
            TypePrefix
                .Concat(_isUniqueHash)
                .Concat(_columnsHash)
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
