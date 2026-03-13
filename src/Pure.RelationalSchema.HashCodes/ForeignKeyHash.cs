using System.Collections;
using Pure.HashCodes;
using Pure.HashCodes.Abstractions;
using Pure.RelationalSchema.Abstractions.Column;
using Pure.RelationalSchema.Abstractions.ForeignKey;
using Pure.RelationalSchema.Abstractions.Table;

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

    public ForeignKeyHash(IForeignKey foreignKey) :
        this(
            foreignKey.ReferencingTable,
            foreignKey.ReferencingColumns,
            foreignKey.ReferencedTable,
            foreignKey.ReferencedColumns
            )
    { }

    public ForeignKeyHash(
        ITable referencingTable,
        IEnumerable<IColumn> referencingColumns,
        ITable referencedTable,
        IEnumerable<IColumn> referencedColumns)
        : this(
            new TableHash(referencingTable),
            referencingColumns,
            referencedTable,
            referencedColumns
        )
    { }

    public ForeignKeyHash(
        IDeterminedHash referencingTableHash,
        IEnumerable<IColumn> referencingColumns,
        ITable referencedTable,
        IEnumerable<IColumn> referencedColumns)
        : this(
            referencingTableHash,
            new DeterminedHash(referencingColumns.Select(c => new ColumnHash(c))),
            referencedTable,
            referencedColumns
              )
    { }

    public ForeignKeyHash(
        ITable referencingTable,
        IDeterminedHash referencingColumnsHash,
        ITable referencedTable,
        IEnumerable<IColumn> referencedColumns)
        : this(
            referencingTable,
            referencingColumnsHash,
            new TableHash(referencedTable),
            referencedColumns
        )
    { }

    public ForeignKeyHash(
        ITable referencingTable,
        IEnumerable<IColumn> referencingColumns,
        IDeterminedHash referencedTableHash,
        IEnumerable<IColumn> referencedColumns)
        : this(
            referencingTable,
            referencingColumns,
            referencedTableHash,
            new DeterminedHash(referencedColumns.Select(c => new ColumnHash(c)))
        )
    { }

    public ForeignKeyHash(
        ITable referencingTable,
        IEnumerable<IColumn> referencingColumns,
        ITable referencedTable,
        IDeterminedHash referencedColumnsHash)
        : this(
            new TableHash(referencingTable),
            referencingColumns,
            referencedTable,
            referencedColumnsHash
        )
    { }

    public ForeignKeyHash(
    IDeterminedHash referencingTableHash,
    IEnumerable<IColumn> referencingColumns,
    ITable referencedTable,
    IDeterminedHash referencedColumnsHash)
    : this(
        referencingTableHash,
        new DeterminedHash(referencingColumns.Select(c => new ColumnHash(c))),
        referencedTable,
        referencedColumnsHash
    )
    { }

    public ForeignKeyHash(
        IDeterminedHash referencingTableHash,
        IDeterminedHash referencingColumnsHash,
        ITable referencedTable,
        IEnumerable<IColumn> referencedColumns)
        : this(
            referencingTableHash,
            referencingColumnsHash,
            new TableHash(referencedTable),
            referencedColumns
        )
    { }

    public ForeignKeyHash(
        ITable referencingTable,
        IDeterminedHash referencingColumnsHash,
        ITable referencedTable,
        IDeterminedHash referencedColumnsHash)
        : this(
            referencingTable,
            referencingColumnsHash,
            new TableHash(referencedTable),
            referencedColumnsHash
        )
    { }

    public ForeignKeyHash(
        IDeterminedHash referencingTableHash,
        IEnumerable<IColumn> referencingColumns,
        IDeterminedHash referencedTableHash,
        IEnumerable<IColumn> referencedColumns)
        : this(
            referencingTableHash,
            referencingColumns,
            referencedTableHash,
            new DeterminedHash(referencedColumns.Select(c => new ColumnHash(c)))
        )
    { }

    public ForeignKeyHash(
        ITable referencingTable,
        IDeterminedHash referencingColumnsHash,
        IDeterminedHash referencedTableHash,
        IEnumerable<IColumn> referencedColumns)
        : this(
            new TableHash(referencingTable),
            referencingColumnsHash,
            referencedTableHash,
            referencedColumns
        )
    { }

    public ForeignKeyHash(
        ITable referencingTable,
        IEnumerable<IColumn> referencingColumns,
        IDeterminedHash referencedTableHash,
        IDeterminedHash referencedColumnsHash)
        : this(
            new TableHash(referencingTable),
            referencingColumns,
            referencedTableHash,
            referencedColumnsHash
        )
    { }

    public ForeignKeyHash(
        IDeterminedHash referencingTableHash,
        IDeterminedHash referencingColumnsHash,
        IDeterminedHash referencedTableHash,
        IEnumerable<IColumn> referencedColumns)
        : this(
            referencingTableHash,
            referencingColumnsHash,
            referencedTableHash,
            new DeterminedHash(referencedColumns.Select(c => new ColumnHash(c)))
        )
    { }

    public ForeignKeyHash(
        IDeterminedHash referencingTableHash,
        IDeterminedHash referencingColumnsHash,
        ITable referencedTable,
        IDeterminedHash referencedColumnsHash)
        : this(
            referencingTableHash,
            referencingColumnsHash,
            new TableHash(referencedTable),
            referencedColumnsHash
        )
    { }

    public ForeignKeyHash(
        IDeterminedHash referencingTableHash,
        IEnumerable<IColumn> referencingColumns,
        IDeterminedHash referencedTableHash,
        IDeterminedHash referencedColumnsHash)
        : this(
            referencingTableHash,
            new DeterminedHash(referencingColumns.Select(c => new ColumnHash(c))),
            referencedTableHash,
            referencedColumnsHash
        )
    { }

    public ForeignKeyHash(
        ITable referencingTable,
        IDeterminedHash referencingColumnsHash,
        IDeterminedHash referencedTableHash,
        IDeterminedHash referencedColumnsHash)
        : this(
            new TableHash(referencingTable),
            referencingColumnsHash,
            referencedTableHash,
            referencedColumnsHash
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
