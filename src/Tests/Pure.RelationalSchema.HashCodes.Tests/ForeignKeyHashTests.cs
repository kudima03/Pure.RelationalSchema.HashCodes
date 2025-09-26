using System.Collections;
using System.Security.Cryptography;
using Pure.HashCodes;
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
                    .. new AggregatedHash(
                        randomForeignKey.ReferencingColumns.Select(x => new ColumnHash(x))
                    ),
                    .. new TableHash(randomForeignKey.ReferencedTable),
                    .. new AggregatedHash(
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
                .. new AggregatedHash(
                    randomForeignKey.ReferencingColumns.Select(x => new ColumnHash(x))
                ),
                .. new TableHash(randomForeignKey.ReferencedTable),
                .. new AggregatedHash(
                    randomForeignKey.ReferencedColumns.Select(x => new ColumnHash(x))
                ),
            ]
        );

        Assert.Equal(expectedHash, new ForeignKeyHash(randomForeignKey));
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
