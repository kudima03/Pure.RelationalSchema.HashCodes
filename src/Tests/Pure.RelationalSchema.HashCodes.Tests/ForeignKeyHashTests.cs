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
                    .. new DeterminedHash(
                        randomForeignKey.ReferencingColumns.Select(x => new ColumnHash(x))
                    ),
                    .. new TableHash(randomForeignKey.ReferencedTable),
                    .. new DeterminedHash(
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
                .. new DeterminedHash(
                    randomForeignKey.ReferencingColumns.Select(x => new ColumnHash(x))
                ),
                .. new TableHash(randomForeignKey.ReferencedTable),
                .. new DeterminedHash(
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
        DeterminedHash referencingColumnsHash = new DeterminedHash(fk.ReferencingColumns.Select(c => new ColumnHash(c)));
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
        DeterminedHash referencedColumnsHash = new DeterminedHash(fk.ReferencedColumns.Select(c => new ColumnHash(c)));

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
    public void ProducesCorrectHashWithTableHashAndReferencedColumnsHashProduces()
    {
        IForeignKey fk = new RandomForeignKey();

        TableHash referencingTableHash = new TableHash(fk.ReferencingTable);
        IEnumerable<IColumn> referencingColumns = fk.ReferencingColumns;
        ITable referencedTable = fk.ReferencedTable;
        DeterminedHash referencedColumnsHash = new DeterminedHash(fk.ReferencedColumns.Select(c => new ColumnHash(c)));

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
        IDeterminedHash referencingColumnsHash = new DeterminedHash(
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
        IDeterminedHash referencingColumnsHash = new DeterminedHash(
            fk.ReferencingColumns.Select(c => new ColumnHash(c))
        );
        ITable referencedTable = fk.ReferencedTable;
        IDeterminedHash referencedColumnsHash = new DeterminedHash(
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
        IDeterminedHash referencingColumnsHash = new DeterminedHash(
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
        IDeterminedHash referencedColumnsHash = new DeterminedHash(
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
        IDeterminedHash referencingColumnsHash = new DeterminedHash(
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
        IDeterminedHash referencingColumnsHash = new DeterminedHash(
            fk.ReferencingColumns.Select(c => new ColumnHash(c))
        );
        ITable referencedTable = fk.ReferencedTable;
        IDeterminedHash referencedColumnsHash = new DeterminedHash(
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
        IDeterminedHash referencedColumnsHash = new DeterminedHash(
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
        IDeterminedHash referencingColumnsHash = new DeterminedHash(
            fk.ReferencingColumns.Select(c => new ColumnHash(c))
        );
        IDeterminedHash referencedTableHash = new TableHash(fk.ReferencedTable);
        IDeterminedHash referencedColumnsHash = new DeterminedHash(
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
        IDeterminedHash referencingColumnsHash = new DeterminedHash(
            fk.ReferencingColumns.Select(c => new ColumnHash(c))
        );
        IDeterminedHash referencedTableHash = new TableHash(fk.ReferencedTable);
        IDeterminedHash referencedColumnsHash = new DeterminedHash(
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
            "92683CB2463C911400D18121CBFBE36AF89C4B67926228E308BF78BC1590438D",
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
            "54216FBD7A1DA51D3DA696527ED790ADB3B0A229BBEEE7DCDC8652E40F459879",
            Convert.ToHexString(new ForeignKeyHash(foreignKey).ToArray())
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
