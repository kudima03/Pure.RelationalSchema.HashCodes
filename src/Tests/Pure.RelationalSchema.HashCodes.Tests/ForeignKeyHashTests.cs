using System.Collections;
using System.Security.Cryptography;
using Pure.HashCodes;
using Pure.HashCodes.Abstractions;
using Pure.Primitives.Bool;
using Pure.RelationalSchema.Abstractions.Column;
using Pure.RelationalSchema.Abstractions.ForeignKey;
using Pure.RelationalSchema.Abstractions.Index;
using Pure.RelationalSchema.Abstractions.Table;
using Pure.RelationalSchema.ColumnType;
using Pure.RelationalSchema.Random;
using String = Pure.Primitives.String.String;

namespace Pure.RelationalSchema.HashCodes.Tests;

using Column = Column.Column;
using ForeignKey = ForeignKey.ForeignKey;
using Index = Index.Index;
using Table = Table.Table;

public sealed record ForeignKeyHashTests
{
    private readonly byte[] _typePrefix =
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

    [Fact]
    public void EnumeratesAsUntyped()
    {
        IForeignKey randomForeignKey = new RandomForeignKey();

        using IEnumerator<byte> expectedHash = SHA256
            .HashData(
                [
                    .. _typePrefix,
                    .. new TableHash(randomForeignKey.ReferencingTable),
                    .. new SequenceHash(
                        randomForeignKey.ReferencingColumns.Select(x => new ColumnHash(x))
                    ),
                    .. new TableHash(randomForeignKey.ReferencedTable),
                    .. new SequenceHash(
                        randomForeignKey.ReferencedColumns.Select(x => new ColumnHash(x))
                    ),
                ]
            )
            .AsEnumerable()
            .GetEnumerator();

        IEnumerable actualHash = new ForeignKeyHash(randomForeignKey);

        bool equal = true;

        foreach (object item in actualHash)
        {
            _ = expectedHash.MoveNext();
            if ((byte)item != expectedHash.Current)
            {
                equal = false;
                break;
            }
        }

        Assert.True(equal);
    }

    [Fact]
    public void CorrectComputingSteps()
    {
        IForeignKey randomForeignKey = new RandomForeignKey();

        IEnumerable<byte> expectedHash = SHA256.HashData(
            [
                .. _typePrefix,
                .. new TableHash(randomForeignKey.ReferencingTable),
                .. new SequenceHash(
                    randomForeignKey.ReferencingColumns.Select(x => new ColumnHash(x))
                ),
                .. new TableHash(randomForeignKey.ReferencedTable),
                .. new SequenceHash(
                    randomForeignKey.ReferencedColumns.Select(x => new ColumnHash(x))
                ),
            ]
        );

        Assert.Equal(expectedHash, new ForeignKeyHash(randomForeignKey));
    }

    [Fact]
    public void ProducesCorrectHashWithTablesAndColumns()
    {
        IForeignKey fk = new RandomForeignKey();

        ITable referencingTable = fk.ReferencingTable;
        ITable referencedTable = fk.ReferencedTable;
        IEnumerable<IColumn> referencingColumns = fk.ReferencingColumns;
        IEnumerable<IColumn> referencedColumns = fk.ReferencedColumns;

        ForeignKeyHash expected = new ForeignKeyHash(fk);

        ForeignKeyHash actual = new ForeignKeyHash(
            referencingTable,
            referencingColumns,
            referencedTable,
            referencedColumns
        );

        Assert.True(expected.SequenceEqual(actual));
    }

    [Fact]
    public void ProducesCorrectHashWithTableHashAndColumns()
    {
        IForeignKey fk = new RandomForeignKey();

        TableHash referencingTableHash = new TableHash(fk.ReferencingTable);
        ITable referencedTable = fk.ReferencedTable;
        IEnumerable<IColumn> referencingColumns = fk.ReferencingColumns;
        IEnumerable<IColumn> referencedColumns = fk.ReferencedColumns;

        ForeignKeyHash expected = new ForeignKeyHash(fk);
        ForeignKeyHash actual = new ForeignKeyHash(
            referencingTableHash,
            referencingColumns,
            referencedTable,
            referencedColumns
        );

        Assert.True(expected.SequenceEqual(actual));
    }

    [Fact]
    public void ProducesCorrectHashWithTableAndColumnsHash()
    {
        IForeignKey fk = new RandomForeignKey();

        ITable referencingTable = fk.ReferencingTable;
        IDeterminedHash referencingColumnsHash = new SequenceHash(fk.ReferencingColumns.Select(c => new ColumnHash(c)));
        ITable referencedTable = fk.ReferencedTable;
        IEnumerable<IColumn> referencedColumns = fk.ReferencedColumns;

        ForeignKeyHash expected = new ForeignKeyHash(fk);
        ForeignKeyHash actual = new ForeignKeyHash(
            referencingTable,
            referencingColumnsHash,
            referencedTable,
            referencedColumns
        );

        Assert.True(expected.SequenceEqual(actual));
    }

    [Fact]
    public void ProducesCorrectHashWithTableAndReferencedTableHash()
    {
        IForeignKey fk = new RandomForeignKey();

        ITable referencingTable = fk.ReferencingTable;
        IEnumerable<IColumn> referencingColumns = fk.ReferencingColumns;
        TableHash referencedTableHash = new TableHash(fk.ReferencedTable);
        IEnumerable<IColumn> referencedColumns = fk.ReferencedColumns;

        ForeignKeyHash expected = new ForeignKeyHash(fk);
        ForeignKeyHash actual = new ForeignKeyHash(
            referencingTable,
            referencingColumns,
            referencedTableHash,
            referencedColumns
        );

        Assert.True(expected.SequenceEqual(actual));
    }

    [Fact]
    public void ProducesCorrectHashWithReferencedColumnsHash()
    {
        IForeignKey fk = new RandomForeignKey();

        ITable referencingTable = fk.ReferencingTable;
        IEnumerable<IColumn> referencingColumns = fk.ReferencingColumns;
        ITable referencedTable = fk.ReferencedTable;
        IDeterminedHash referencedColumnsHash = new SequenceHash(fk.ReferencedColumns.Select(c => new ColumnHash(c)));

        ForeignKeyHash expected = new ForeignKeyHash(fk);
        ForeignKeyHash actual = new ForeignKeyHash(
            referencingTable,
            referencingColumns,
            referencedTable,
            referencedColumnsHash
        );

        Assert.True(expected.SequenceEqual(actual));
    }

    [Fact]
    public void ProducesCorrectHashWithTableHashAndReferencedColumnsHash()
    {
        IForeignKey fk = new RandomForeignKey();

        TableHash referencingTableHash = new TableHash(fk.ReferencingTable);
        IEnumerable<IColumn> referencingColumns = fk.ReferencingColumns;
        ITable referencedTable = fk.ReferencedTable;
        IDeterminedHash referencedColumnsHash = new SequenceHash(fk.ReferencedColumns.Select(c => new ColumnHash(c)));

        ForeignKeyHash expected = new ForeignKeyHash(fk);
        ForeignKeyHash actual = new ForeignKeyHash(
            referencingTableHash,
            referencingColumns,
            referencedTable,
            referencedColumnsHash
        );

        Assert.True(expected.SequenceEqual(actual));
    }

    [Fact]
    public void ProducesCorrectHashWithTableAndColumnsHashAndReferencedTable()
    {
        IForeignKey fk = new RandomForeignKey();

        IDeterminedHash referencingTableHash = new TableHash(fk.ReferencingTable);
        IDeterminedHash referencingColumnsHash = new SequenceHash(
            fk.ReferencingColumns.Select(c => new ColumnHash(c))
        );
        ITable referencedTable = fk.ReferencedTable;
        IEnumerable<IColumn> referencedColumns = fk.ReferencedColumns;

        ForeignKeyHash expected = new ForeignKeyHash(fk);
        ForeignKeyHash actual = new ForeignKeyHash(
            referencingTableHash,
            referencingColumnsHash,
            referencedTable,
            referencedColumns
        );

        Assert.True(expected.SequenceEqual(actual));
    }

    [Fact]
    public void ProducesCorrectHashWithTableAndColumnsHashAndReferencedColumnsHash()
    {
        IForeignKey fk = new RandomForeignKey();

        ITable referencingTable = fk.ReferencingTable;
        IDeterminedHash referencingColumnsHash = new SequenceHash(
            fk.ReferencingColumns.Select(c => new ColumnHash(c))
        );
        ITable referencedTable = fk.ReferencedTable;
        IDeterminedHash referencedColumnsHash = new SequenceHash(
            fk.ReferencedColumns.Select(c => new ColumnHash(c))
        );

        ForeignKeyHash expected = new ForeignKeyHash(fk);
        ForeignKeyHash actual = new ForeignKeyHash(
            referencingTable,
            referencingColumnsHash,
            referencedTable,
            referencedColumnsHash
        );

        Assert.True(expected.SequenceEqual(actual));
    }

    [Fact]
    public void ProducesCorrectHashReferencingTableHashAndReferencedTableHash()
    {
        IForeignKey fk = new RandomForeignKey();

        IDeterminedHash referencingTableHash = new TableHash(fk.ReferencingTable);
        IEnumerable<IColumn> referencingColumns = fk.ReferencingColumns;
        IDeterminedHash referencedTableHash = new TableHash(fk.ReferencedTable);
        IEnumerable<IColumn> referencedColumns = fk.ReferencedColumns;

        ForeignKeyHash expected = new ForeignKeyHash(fk);
        ForeignKeyHash actual = new ForeignKeyHash(
            referencingTableHash,
            referencingColumns,
            referencedTableHash,
            referencedColumns
        );

        Assert.True(expected.SequenceEqual(actual));
    }

    [Fact]
    public void ProducesCorrectHashTableAndColumnsHashAndReferencedTableHash()
    {
        IForeignKey fk = new RandomForeignKey();

        ITable referencingTable = fk.ReferencingTable;
        IDeterminedHash referencingColumnsHash = new SequenceHash(
            fk.ReferencingColumns.Select(c => new ColumnHash(c))
        );
        IDeterminedHash referencedTableHash = new TableHash(fk.ReferencedTable);
        IEnumerable<IColumn> referencedColumns = fk.ReferencedColumns;

        ForeignKeyHash expected = new ForeignKeyHash(fk);
        ForeignKeyHash actual = new ForeignKeyHash(
            referencingTable,
            referencingColumnsHash,
            referencedTableHash,
            referencedColumns
        );

        Assert.True(expected.SequenceEqual(actual));
    }

    [Fact]
    public void ProducesCorrectHashTableAndColumnsHashAndReferencedHashes()
    {
        IForeignKey fk = new RandomForeignKey();

        ITable referencingTable = fk.ReferencingTable;
        IEnumerable<IColumn> referencingColumns = fk.ReferencingColumns;
        IDeterminedHash referencedTableHash = new TableHash(fk.ReferencedTable);
        IDeterminedHash referencedColumnsHash = new SequenceHash(
            fk.ReferencedColumns.Select(c => new ColumnHash(c))
        );

        ForeignKeyHash expected = new ForeignKeyHash(fk);
        ForeignKeyHash actual = new ForeignKeyHash(
            referencingTable,
            referencingColumns,
            referencedTableHash,
            referencedColumnsHash
        );

        Assert.True(expected.SequenceEqual(actual));
    }

    [Fact]
    public void ProducesCorrectHashWithReferencingHashesAndReferencedColumns()
    {
        IForeignKey fk = new RandomForeignKey();

        IDeterminedHash referencingTableHash = new TableHash(fk.ReferencingTable);
        IDeterminedHash referencingColumnsHash = new SequenceHash(
            fk.ReferencingColumns.Select(c => new ColumnHash(c))
        );
        IDeterminedHash referencedTableHash = new TableHash(fk.ReferencedTable);
        IEnumerable<IColumn> referencedColumns = fk.ReferencedColumns;

        ForeignKeyHash expected = new ForeignKeyHash(fk);
        ForeignKeyHash actual = new ForeignKeyHash(
            referencingTableHash,
            referencingColumnsHash,
            referencedTableHash,
            referencedColumns
        );

        Assert.True(expected.SequenceEqual(actual));
    }

    [Fact]
    public void ProducesCorrectHashWithReferencingHashesAndReferencedTableAndColumnsHash()
    {
        IForeignKey fk = new RandomForeignKey();

        IDeterminedHash referencingTableHash = new TableHash(fk.ReferencingTable);
        IDeterminedHash referencingColumnsHash = new SequenceHash(
            fk.ReferencingColumns.Select(c => new ColumnHash(c))
        );
        ITable referencedTable = fk.ReferencedTable;
        IDeterminedHash referencedColumnsHash = new SequenceHash(
            fk.ReferencedColumns.Select(c => new ColumnHash(c))
        );

        ForeignKeyHash expected = new ForeignKeyHash(fk);
        ForeignKeyHash actual = new ForeignKeyHash(
            referencingTableHash,
            referencingColumnsHash,
            referencedTable,
            referencedColumnsHash
        );

        Assert.True(expected.SequenceEqual(actual));
    }

    [Fact]
    public void ProducesCorrectHashWithReferencingTableHashAndColumnsAndReferencedHashes()
    {
        IForeignKey fk = new RandomForeignKey();

        IDeterminedHash referencingTableHash = new TableHash(fk.ReferencingTable);
        IEnumerable<IColumn> referencingColumns = fk.ReferencingColumns;
        IDeterminedHash referencedTableHash = new TableHash(fk.ReferencedTable);
        IDeterminedHash referencedColumnsHash = new SequenceHash(
            fk.ReferencedColumns.Select(c => new ColumnHash(c))
        );

        ForeignKeyHash expected = new ForeignKeyHash(fk);
        ForeignKeyHash actual = new ForeignKeyHash(
            referencingTableHash,
            referencingColumns,
            referencedTableHash,
            referencedColumnsHash
        );

        Assert.True(expected.SequenceEqual(actual));
    }

    [Fact]
    public void ProducesCorrectHashWithTableAndColumnsHashAndReferencedTableHashAndColumnsHash()
    {
        IForeignKey fk = new RandomForeignKey();

        ITable referencingTable = fk.ReferencingTable;
        IDeterminedHash referencingColumnsHash = new SequenceHash(
            fk.ReferencingColumns.Select(c => new ColumnHash(c))
        );
        IDeterminedHash referencedTableHash = new TableHash(fk.ReferencedTable);
        IDeterminedHash referencedColumnsHash = new SequenceHash(
            fk.ReferencedColumns.Select(c => new ColumnHash(c))
        );

        ForeignKeyHash expected = new ForeignKeyHash(fk);
        ForeignKeyHash actual = new ForeignKeyHash(
            referencingTable,
            referencingColumnsHash,
            referencedTableHash,
            referencedColumnsHash
        );

        Assert.True(expected.SequenceEqual(actual));
    }

    [Fact]
    public void ProducesCorrectHashWithAllHashes()
    {
        IForeignKey fk = new RandomForeignKey();

        IDeterminedHash referencingTableHash = new TableHash(fk.ReferencingTable);
        IDeterminedHash referencingColumnsHash = new SequenceHash(
            fk.ReferencingColumns.Select(c => new ColumnHash(c))
        );
        IDeterminedHash referencedTableHash = new TableHash(fk.ReferencedTable);
        IDeterminedHash referencedColumnsHash = new SequenceHash(
            fk.ReferencedColumns.Select(c => new ColumnHash(c))
        );

        ForeignKeyHash expected = new ForeignKeyHash(fk);
        ForeignKeyHash actual = new ForeignKeyHash(
            referencingTableHash,
            referencingColumnsHash,
            referencedTableHash,
            referencedColumnsHash
        );

        Assert.True(expected.SequenceEqual(actual));
    }

    [Fact]
    public void Determined()
    {
        IReadOnlyCollection<IColumn> columns =
        [
            new Column(new String("asd"), new DateColumnType()),
            new Column(new String("qwe"), new TimeColumnType()),
            new Column(new String("asd"), new UShortColumnType()),
            new Column(new String("zxc"), new LongColumnType()),
            new Column(new String("tyu"), new IntColumnType()),
        ];

        IReadOnlyCollection<IIndex> indexes =
        [
            new Index(new True(), columns.Take(2)),
            new Index(new True(), columns.Skip(2).Take(2)),
        ];

        ITable referencingTable = new Table(
            new String("Sample name"),
            columns.Take(2),
            indexes.Take(1)
        );
        ITable referencedTable = new Table(
            new String("Sample name1"),
            columns.Skip(2).Take(2),
            indexes.Skip(1).Take(1)
        );

        IForeignKey foreignKey = new ForeignKey(
            referencingTable,
            [referencingTable.Columns.First()],
            referencedTable,
            [referencedTable.Columns.First()]
        );

        Assert.Equal(
            "F54D204CACFBF76C5C4D56DC43CACB5FB79AAD92BA4381FA54B9D472328D141E",
            Convert.ToHexString(new ForeignKeyHash(foreignKey).ToArray())
        );
    }

    [Fact]
    public void DeterminedOnEmptyColumns()
    {
        IReadOnlyCollection<IColumn> columns =
        [
            new Column(new String("asd"), new DateColumnType()),
            new Column(new String("qwe"), new TimeColumnType()),
            new Column(new String("asd"), new UShortColumnType()),
            new Column(new String("zxc"), new LongColumnType()),
            new Column(new String("tyu"), new IntColumnType()),
        ];

        IReadOnlyCollection<IIndex> indexes =
        [
            new Index(new True(), columns.Take(2)),
            new Index(new True(), columns.Skip(2).Take(2)),
        ];

        ITable referencingTable = new Table(
            new String("Sample name"),
            columns.Take(2),
            indexes.Take(1)
        );
        ITable referencedTable = new Table(
            new String("Sample name1"),
            columns.Skip(2).Take(2),
            indexes.Skip(1).Take(1)
        );

        IForeignKey foreignKey = new ForeignKey(
            referencingTable,
            [],
            referencedTable,
            []
        );

        Assert.Equal(
            "3AD6E2E0F1118790D2D26F65B7C688A5AF6B959E363BC149D5A227BB215E192A",
            Convert.ToHexString(new ForeignKeyHash(foreignKey).ToArray())
        );
    }

    [Fact]
    public void ColumnsPairingMatters()
    {
        IReadOnlyCollection<IColumn> referencingColumns =
        [
            new Column(new String("a"), new IntColumnType()),
            new Column(new String("b"), new IntColumnType()),
        ];

        IReadOnlyCollection<IColumn> referencedColumns =
        [
            new Column(new String("x"), new IntColumnType()),
            new Column(new String("y"), new IntColumnType()),
        ];

        ITable referencingTable = new Table(new String("referencing"), referencingColumns, []);
        ITable referencedTable = new Table(new String("referenced"), referencedColumns, []);

        Assert.NotEqual(
            new ForeignKeyHash(
                new ForeignKey(
                    referencingTable,
                    referencingColumns,
                    referencedTable,
                    referencedColumns
                )
            ).AsEnumerable(),
            new ForeignKeyHash(
                new ForeignKey(
                    referencingTable,
                    referencingColumns,
                    referencedTable,
                    referencedColumns.Reverse()
                )
            ).AsEnumerable()
        );
    }

    [Fact]
    public void ThrowsExceptionOnGetHashCode()
    {
        _ = Assert.Throws<NotSupportedException>(() =>
            new ForeignKeyHash(new RandomForeignKey()).GetHashCode()
        );
    }

    [Fact]
    public void ThrowsExceptionOnToString()
    {
        _ = Assert.Throws<NotSupportedException>(() =>
            new ForeignKeyHash(new RandomForeignKey()).ToString()
        );
    }
}
