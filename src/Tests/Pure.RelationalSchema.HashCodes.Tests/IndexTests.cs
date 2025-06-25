using Pure.HashCodes;
using Pure.Primitives.Abstractions.Bool;
using Pure.Primitives.Abstractions.String;
using Pure.Primitives.Number;
using Pure.Primitives.Random.Bool;
using Pure.Primitives.Random.String;
using Pure.Primitives.String;
using Pure.RelationalSchema.Abstractions.Column;
using Pure.RelationalSchema.Abstractions.ColumnType;
using Pure.RelationalSchema.Abstractions.Index;
using Pure.RelationalSchema.ColumnType;
using System.Collections;
using System.Security.Cryptography;

namespace Pure.RelationalSchema.HashCodes.Tests;

using Index = Index.Index;
using Column = Column.Column;

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
    public void ProduceCorrectHash()
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
            new IndexHash(new Index(uniqueness, columns.Reverse())));
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