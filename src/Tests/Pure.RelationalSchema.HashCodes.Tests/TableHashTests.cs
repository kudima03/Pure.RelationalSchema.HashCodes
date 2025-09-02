using System.Collections;
using System.Security.Cryptography;
using Pure.HashCodes;
using Pure.Primitives.Abstractions.String;
using Pure.Primitives.Bool;
using Pure.Primitives.Number;
using Pure.Primitives.Random.String;
using Pure.Primitives.String;
using Pure.RelationalSchema.Abstractions.Column;
using Pure.RelationalSchema.Abstractions.Index;
using Pure.RelationalSchema.ColumnType;
using String = Pure.Primitives.String.String;

namespace Pure.RelationalSchema.HashCodes.Tests;

public sealed record TableHashTests
{
    [Fact]
    public void EnumeratesAsUntyped()
    {
        byte[] typePrefix =
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
            new Index(new True(), columns.Take(2)),
            new Index(new True(), columns.Skip(2).Take(2)),
        ];

        IDeterminedHash nameHash = new DeterminedHash(name);
        IDeterminedHash columnsHash = new AggregatedHash(
            columns.Select(x => new ColumnHash(x))
        );
        IDeterminedHash indexesHash = new AggregatedHash(
            indexes.Select(x => new IndexHash(x))
        );

        using IEnumerator<byte> expectedHash = SHA256
            .HashData([.. typePrefix, .. nameHash, .. columnsHash, .. indexesHash])
            .AsEnumerable()
            .GetEnumerator();

        IEnumerable actualHash = new TableHash(new Table(name, columns, indexes));

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
            new Index(new True(), columns.Take(2)),
            new Index(new True(), columns.Skip(2).Take(2)),
        ];

        IDeterminedHash nameHash = new DeterminedHash(name);
        IDeterminedHash columnsHash = new AggregatedHash(
            columns.Select(x => new ColumnHash(x))
        );
        IDeterminedHash indexesHash = new AggregatedHash(
            indexes.Select(x => new IndexHash(x))
        );

        Assert.Equal(
            SHA256.HashData([.. typePrefix, .. nameHash, .. columnsHash, .. indexesHash]),
            new TableHash(new Table(name, columns, indexes))
        );
    }

    [Fact]
    public void ColumnsOrderNotMatter()
    {
        IString name = new String("Sample name");

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

        Assert.Equal(
            "BEF6FF6D2D6180367A589DDF981080F3EAC06FBE2221321CAAEDDEF87B401245",
            Convert.ToHexString(
                new TableHash(new Table(name, columns.Reverse(), indexes)).ToArray()
            )
        );
    }

    [Fact]
    public void IndexesOrderNotMatter()
    {
        IString name = new String("Sample name");

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

        Assert.Equal(
            "BEF6FF6D2D6180367A589DDF981080F3EAC06FBE2221321CAAEDDEF87B401245",
            Convert.ToHexString(
                new TableHash(new Table(name, columns, indexes.Reverse())).ToArray()
            )
        );
    }

    [Fact]
    public void Determined()
    {
        IString name = new String("Sample name");

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

        Assert.Equal(
            "BEF6FF6D2D6180367A589DDF981080F3EAC06FBE2221321CAAEDDEF87B401245",
            Convert.ToHexString(
                new TableHash(new Table(name, columns, indexes)).ToArray()
            )
        );
    }

    [Fact]
    public void ThrowsExceptionOnGetHashCode()
    {
        Assert.Throws<NotSupportedException>(() =>
            new TableHash(new Table(new EmptyString(), [], [])).GetHashCode()
        );
    }

    [Fact]
    public void ThrowsExceptionOnToString()
    {
        Assert.Throws<NotSupportedException>(() =>
            new TableHash(new Table(new EmptyString(), [], [])).ToString()
        );
    }
}
