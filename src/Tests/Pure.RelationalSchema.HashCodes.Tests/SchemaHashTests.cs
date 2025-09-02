using System.Collections;
using System.Security.Cryptography;
using Pure.HashCodes;
using Pure.Primitives.Abstractions.String;
using Pure.Primitives.Bool;
using Pure.Primitives.Number;
using Pure.Primitives.Random.Bool;
using Pure.Primitives.Random.String;
using Pure.Primitives.String;
using Pure.RelationalSchema.Abstractions.Column;
using Pure.RelationalSchema.Abstractions.ForeignKey;
using Pure.RelationalSchema.Abstractions.Index;
using Pure.RelationalSchema.Abstractions.Table;
using Pure.RelationalSchema.ColumnType;
using String = Pure.Primitives.String.String;

namespace Pure.RelationalSchema.HashCodes.Tests;

public sealed record SchemaHashTests
{
    [Fact]
    public void EnumeratesAsUntyped()
    {
        byte[] typePrefix =
        [
            253,
            165,
            151,
            1,
            96,
            51,
            234,
            121,
            155,
            41,
            25,
            146,
            55,
            243,
            188,
            110,
        ];

        IString name = new RandomString(new UShort(10));

        IReadOnlyCollection<IColumn> columns =
        [
            new Column(new RandomString(new UShort(10)), new DateColumnType()),
            new Column(new RandomString(new UShort(10)), new TimeColumnType()),
            new Column(new RandomString(new UShort(10)), new UShortColumnType()),
            new Column(new RandomString(new UShort(10)), new LongColumnType()),
            new Column(new RandomString(new UShort(10)), new IntColumnType()),
        ];

        IReadOnlyCollection<IIndex> indexes =
        [
            new Index(new RandomBool(), columns.Take(2)),
            new Index(new RandomBool(), columns.Skip(2).Take(2)),
            new Index(new RandomBool(), columns.Take(1)),
            new Index(new RandomBool(), columns.Skip(2).Take(1)),
        ];

        IReadOnlyCollection<ITable> tables =
        [
            new Table(new RandomString(new UShort(10)), columns, indexes.Take(2)),
            new Table(new RandomString(new UShort(10)), columns, indexes.Skip(2).Take(2)),
            new Table(new RandomString(new UShort(10)), columns, indexes.Skip(3).Take(1)),
            new Table(new RandomString(new UShort(10)), columns, indexes.Skip(1).Take(1)),
        ];

        IReadOnlyCollection<IForeignKey> foreignKeys =
        [
            new ForeignKey(
                tables.First(),
                tables.First().Columns.First(),
                tables.Skip(1).First(),
                tables.Skip(1).First().Columns.First()
            ),
            new ForeignKey(
                tables.Skip(2).First(),
                tables.Skip(2).First().Columns.First(),
                tables.Skip(3).First(),
                tables.Skip(3).First().Columns.First()
            ),
        ];

        using IEnumerator<byte> expectedHash = SHA256
            .HashData(
                [
                    .. typePrefix,
                    .. new DeterminedHash(name),
                    .. new AggregatedHash(tables.Select(x => new TableHash(x))),
                    .. new AggregatedHash(foreignKeys.Select(x => new ForeignKeyHash(x))),
                ]
            )
            .AsEnumerable()
            .GetEnumerator();

        IEnumerable actualHash = new SchemaHash(new Schema(name, tables, foreignKeys));

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
        byte[] typePrefix =
        [
            253,
            165,
            151,
            1,
            96,
            51,
            234,
            121,
            155,
            41,
            25,
            146,
            55,
            243,
            188,
            110,
        ];

        IString name = new RandomString(new UShort(10));

        IReadOnlyCollection<IColumn> columns =
        [
            new Column(new RandomString(new UShort(10)), new DateColumnType()),
            new Column(new RandomString(new UShort(10)), new TimeColumnType()),
            new Column(new RandomString(new UShort(10)), new UShortColumnType()),
            new Column(new RandomString(new UShort(10)), new LongColumnType()),
            new Column(new RandomString(new UShort(10)), new IntColumnType()),
        ];

        IReadOnlyCollection<IIndex> indexes =
        [
            new Index(new RandomBool(), columns.Take(2)),
            new Index(new RandomBool(), columns.Skip(2).Take(2)),
            new Index(new RandomBool(), columns.Take(1)),
            new Index(new RandomBool(), columns.Skip(2).Take(1)),
        ];

        IReadOnlyCollection<ITable> tables =
        [
            new Table(new RandomString(new UShort(10)), columns, indexes.Take(2)),
            new Table(new RandomString(new UShort(10)), columns, indexes.Skip(2).Take(2)),
            new Table(new RandomString(new UShort(10)), columns, indexes.Skip(3).Take(1)),
            new Table(new RandomString(new UShort(10)), columns, indexes.Skip(1).Take(1)),
        ];

        IReadOnlyCollection<IForeignKey> foreignKeys =
        [
            new ForeignKey(
                tables.First(),
                tables.First().Columns.First(),
                tables.Skip(1).First(),
                tables.Skip(1).First().Columns.First()
            ),
            new ForeignKey(
                tables.Skip(2).First(),
                tables.Skip(2).First().Columns.First(),
                tables.Skip(3).First(),
                tables.Skip(3).First().Columns.First()
            ),
        ];

        Assert.Equal(
            SHA256.HashData(
                [
                    .. typePrefix,
                    .. new DeterminedHash(name),
                    .. new AggregatedHash(tables.Select(x => new TableHash(x))),
                    .. new AggregatedHash(foreignKeys.Select(x => new ForeignKeyHash(x))),
                ]
            ),
            new SchemaHash(new Schema(name, tables, foreignKeys))
        );
    }

    [Fact]
    public void TablesOrderNotMatter()
    {
        IString name = new String("fdjbhvn");

        IReadOnlyCollection<IColumn> columns =
        [
            new Column(new String("tyghdntygrhhgrty"), new DateColumnType()),
            new Column(new String("srfgbzERWGerg"), new TimeColumnType()),
            new Column(new String("fdjbWAREREWGRGhvn"), new UShortColumnType()),
            new Column(new String("fdjbhyufjkjyhukvn"), new LongColumnType()),
            new Column(new String("fdjb3q45t4rhvn"), new IntColumnType()),
        ];

        IReadOnlyCollection<IIndex> indexes =
        [
            new Index(new True(), columns.Take(2)),
            new Index(new True(), columns.Skip(2).Take(2)),
            new Index(new True(), columns.Take(1)),
            new Index(new True(), columns.Skip(2).Take(1)),
        ];

        IReadOnlyCollection<ITable> tables =
        [
            new Table(new String("inerbgfhbnig"), columns, indexes.Take(2)),
            new Table(new String("pu9jWQPHJUI9"), columns, indexes.Skip(2).Take(2)),
            new Table(new String("DAFNdsflkgmvkjo"), columns, indexes.Skip(3).Take(1)),
            new Table(new String("EWQOIJRPOIJUe"), columns, indexes.Skip(1).Take(1)),
        ];

        IReadOnlyCollection<IForeignKey> foreignKeys =
        [
            new ForeignKey(
                tables.First(),
                tables.First().Columns.First(),
                tables.Skip(1).First(),
                tables.Skip(1).First().Columns.First()
            ),
            new ForeignKey(
                tables.Skip(2).First(),
                tables.Skip(2).First().Columns.First(),
                tables.Skip(3).First(),
                tables.Skip(3).First().Columns.First()
            ),
        ];

        Assert.Equal(
            "4E3A969A5633DAAA88ED5BD30FFFA35C0679C7C2B0C38CD52567445C4433186B",
            Convert.ToHexString(
                new SchemaHash(new Schema(name, tables.Reverse(), foreignKeys)).ToArray()
            )
        );
    }

    [Fact]
    public void ForeignKeysOrderNotMatter()
    {
        IString name = new String("fdjbhvn");

        IReadOnlyCollection<IColumn> columns =
        [
            new Column(new String("tyghdntygrhhgrty"), new DateColumnType()),
            new Column(new String("srfgbzERWGerg"), new TimeColumnType()),
            new Column(new String("fdjbWAREREWGRGhvn"), new UShortColumnType()),
            new Column(new String("fdjbhyufjkjyhukvn"), new LongColumnType()),
            new Column(new String("fdjb3q45t4rhvn"), new IntColumnType()),
        ];

        IReadOnlyCollection<IIndex> indexes =
        [
            new Index(new True(), columns.Take(2)),
            new Index(new True(), columns.Skip(2).Take(2)),
            new Index(new True(), columns.Take(1)),
            new Index(new True(), columns.Skip(2).Take(1)),
        ];

        IReadOnlyCollection<ITable> tables =
        [
            new Table(new String("inerbgfhbnig"), columns, indexes.Take(2)),
            new Table(new String("pu9jWQPHJUI9"), columns, indexes.Skip(2).Take(2)),
            new Table(new String("DAFNdsflkgmvkjo"), columns, indexes.Skip(3).Take(1)),
            new Table(new String("EWQOIJRPOIJUe"), columns, indexes.Skip(1).Take(1)),
        ];

        IReadOnlyCollection<IForeignKey> foreignKeys =
        [
            new ForeignKey(
                tables.First(),
                tables.First().Columns.First(),
                tables.Skip(1).First(),
                tables.Skip(1).First().Columns.First()
            ),
            new ForeignKey(
                tables.Skip(2).First(),
                tables.Skip(2).First().Columns.First(),
                tables.Skip(3).First(),
                tables.Skip(3).First().Columns.First()
            ),
        ];

        Assert.Equal(
            "4E3A969A5633DAAA88ED5BD30FFFA35C0679C7C2B0C38CD52567445C4433186B",
            Convert.ToHexString(
                new SchemaHash(new Schema(name, tables, foreignKeys.Reverse())).ToArray()
            )
        );
    }

    [Fact]
    public void Determined()
    {
        IString name = new String("fdjbhvn");

        IReadOnlyCollection<IColumn> columns =
        [
            new Column(new String("tyghdntygrhhgrty"), new DateColumnType()),
            new Column(new String("srfgbzERWGerg"), new TimeColumnType()),
            new Column(new String("fdjbWAREREWGRGhvn"), new UShortColumnType()),
            new Column(new String("fdjbhyufjkjyhukvn"), new LongColumnType()),
            new Column(new String("fdjb3q45t4rhvn"), new IntColumnType()),
        ];

        IReadOnlyCollection<IIndex> indexes =
        [
            new Index(new True(), columns.Take(2)),
            new Index(new True(), columns.Skip(2).Take(2)),
            new Index(new True(), columns.Take(1)),
            new Index(new True(), columns.Skip(2).Take(1)),
        ];

        IReadOnlyCollection<ITable> tables =
        [
            new Table(new String("inerbgfhbnig"), columns, indexes.Take(2)),
            new Table(new String("pu9jWQPHJUI9"), columns, indexes.Skip(2).Take(2)),
            new Table(new String("DAFNdsflkgmvkjo"), columns, indexes.Skip(3).Take(1)),
            new Table(new String("EWQOIJRPOIJUe"), columns, indexes.Skip(1).Take(1)),
        ];

        IReadOnlyCollection<IForeignKey> foreignKeys =
        [
            new ForeignKey(
                tables.First(),
                tables.First().Columns.First(),
                tables.Skip(1).First(),
                tables.Skip(1).First().Columns.First()
            ),
            new ForeignKey(
                tables.Skip(2).First(),
                tables.Skip(2).First().Columns.First(),
                tables.Skip(3).First(),
                tables.Skip(3).First().Columns.First()
            ),
        ];

        Assert.Equal(
            "4E3A969A5633DAAA88ED5BD30FFFA35C0679C7C2B0C38CD52567445C4433186B",
            Convert.ToHexString(
                new SchemaHash(new Schema(name, tables, foreignKeys)).ToArray()
            )
        );
    }

    [Fact]
    public void ThrowsExceptionOnGetHashCode()
    {
        Assert.Throws<NotSupportedException>(() =>
            new SchemaHash(new Schema(new EmptyString(), [], [])).GetHashCode()
        );
    }

    [Fact]
    public void ThrowsExceptionOnToString()
    {
        Assert.Throws<NotSupportedException>(() =>
            new SchemaHash(new Schema(new EmptyString(), [], [])).ToString()
        );
    }
}
