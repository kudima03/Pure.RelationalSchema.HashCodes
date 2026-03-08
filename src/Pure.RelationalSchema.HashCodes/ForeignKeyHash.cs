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

    private readonly IDeterminedHash _referencingTableHash;
    private readonly IDeterminedHash _referencingColumnsHash;
    private readonly IDeterminedHash _referencedTableHash;
    private readonly IDeterminedHash _referencedColumnsHash;

    public ForeignKeyHash(IForeignKey foreignKey)
        : this(
            new TableHash(foreignKey.ReferencingTable),
            new DeterminedHash(foreignKey.ReferencingColumns.Select(column => new ColumnHash(column.Name, column.Type))),
            new TableHash(foreignKey.ReferencedTable),
            new DeterminedHash(foreignKey.ReferencedColumns.Select(column => new ColumnHash(column.Name, column.Type)))
        )
    { }

    public ForeignKeyHash(
        IDeterminedHash referencingTableHash,
        IDeterminedHash referencingColumnsHash,
        IDeterminedHash referencedTableHash,
        IDeterminedHash referencedColumnsHash)
    {
        _referencingTableHash = referencingTableHash;
        _referencingColumnsHash = referencingColumnsHash;
        _referencedTableHash = referencedTableHash;
        _referencedColumnsHash = referencedColumnsHash;
    }

    public IEnumerator<byte> GetEnumerator()
    {
        return new DeterminedHash(
            TypePrefix
                .Concat(_referencingTableHash)
                .Concat(_referencingColumnsHash)
                .Concat(_referencedTableHash)
                .Concat(_referencedColumnsHash)
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
