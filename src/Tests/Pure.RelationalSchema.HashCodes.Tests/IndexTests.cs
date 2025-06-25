using Pure.HashCodes;
using Pure.Primitives.Abstractions.Bool;
using Pure.Primitives.Bool;
using Pure.Primitives.Number;
using Pure.Primitives.Random.Bool;
using Pure.Primitives.Random.String;
using Pure.RelationalSchema.Abstractions.Column;
using Pure.RelationalSchema.ColumnType;
using System.Collections;
using System.Security.Cryptography;
using String = Pure.Primitives.String.String;

namespace Pure.RelationalSchema.HashCodes.Tests;

using Column = Column.Column;
using Index = Index.Index;

public sealed record IndexTests
{
    [Fact]
    public void EnumeratesAsUntyped()
    {
        byte[] typePrefix = [142, 165, 151, 1, 117, 182, 22, 125, 191, 1, 173, 241, 145, 57, 67, 244];

        IBool uniqueness = new RandomBool();

        IReadOnlyCollection<IColumn> columns =
        [
            new Column(new RandomString(new UShort(10)), new DateColumnType()),
            new Column(new RandomString(new UShort(10)), new TimeColumnType()),
            new Column(new RandomString(new UShort(10)), new UShortColumnType()),
            new Column(new RandomString(new UShort(10)), new LongColumnType()),
            new Column(new RandomString(new UShort(10)), new IntColumnType()),
        ];

        IDeterminedHash uniquenessHash = new DeterminedHash(uniqueness);

        IDeterminedHash columnsHash = new AggregatedHash(columns.Select(x => new ColumnHash(x)));

        using IEnumerator<byte> expectedHash = SHA256.HashData(typePrefix
            .Concat(uniquenessHash)
            .Concat(columnsHash)
            .ToArray())
            .AsEnumerable()
            .GetEnumerator();

        IEnumerable actualHash = new IndexHash(new Index(uniqueness, columns));

        bool equal = true;

        foreach (object item in actualHash)
        {
            expectedHash.MoveNext();
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
        byte[] typePrefix = [142, 165, 151, 1, 117, 182, 22, 125, 191, 1, 173, 241, 145, 57, 67, 244];

        IBool uniqueness = new RandomBool();

        IReadOnlyCollection<IColumn> columns =
        [
            new Column(new RandomString(new UShort(10)), new DateColumnType()),
            new Column(new RandomString(new UShort(10)), new TimeColumnType()),
            new Column(new RandomString(new UShort(10)), new UShortColumnType()),
            new Column(new RandomString(new UShort(10)), new LongColumnType()),
            new Column(new RandomString(new UShort(10)), new IntColumnType()),
        ];

        IDeterminedHash uniquenessHash = new DeterminedHash(uniqueness);

        IDeterminedHash columnsHash = new AggregatedHash(columns.Select(x => new ColumnHash(x)));

        Assert.Equal(
            SHA256.HashData(typePrefix
                .Concat(uniquenessHash)
                .Concat(columnsHash)
                .ToArray()),
            new IndexHash(new Index(uniqueness, columns)));
    }

    [Fact]
    public void ColumnsOrderNotMatter()
    {
        IBool uniqueness = new False();

        IReadOnlyCollection<IColumn> columns =
        [
            new Column(new String("asd"), new DateColumnType()),
            new Column(new String("qwe"), new TimeColumnType()),
            new Column(new String("asd"), new UShortColumnType()),
            new Column(new String("zxc"), new LongColumnType()),
            new Column(new String("tyu"), new IntColumnType()),
        ];

        Assert.Equal("975E8091A716FB7C61915C355449EDD67E98863259CC592D892F9639051C557E",
            Convert.ToHexString(new IndexHash(new Index(uniqueness, columns.Reverse())).ToArray()));
    }

    [Fact]
    public void Determined()
    {
        IBool uniqueness = new False();

        IReadOnlyCollection<IColumn> columns =
        [
            new Column(new String("asd"), new DateColumnType()),
            new Column(new String("qwe"), new TimeColumnType()),
            new Column(new String("asd"), new UShortColumnType()),
            new Column(new String("zxc"), new LongColumnType()),
            new Column(new String("tyu"), new IntColumnType()),
        ];

        Assert.Equal("975E8091A716FB7C61915C355449EDD67E98863259CC592D892F9639051C557E",
            Convert.ToHexString(new IndexHash(new Index(uniqueness, columns)).ToArray()));
    }

    [Fact]
    public void ThrowsExceptionOnGetHashCode()
    {
        Assert.Throws<NotSupportedException>(() => new IndexHash(new Index(new RandomBool(), [])).ToString());
    }

    [Fact]
    public void ThrowsExceptionOnToString()
    {
        Assert.Throws<NotSupportedException>(() => new IndexHash(new Index(new RandomBool(), [])).ToString());
    }
}