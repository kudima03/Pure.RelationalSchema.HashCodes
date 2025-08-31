using Pure.HashCodes;
using Pure.Primitives.Abstractions.Bool;
using Pure.Primitives.Bool;
using Pure.RelationalSchema.Abstractions.Column;
using Pure.RelationalSchema.Abstractions.Index;
using Pure.RelationalSchema.ColumnType;
using Pure.RelationalSchema.Random;
using System.Collections;
using System.Security.Cryptography;
using String = Pure.Primitives.String.String;

namespace Pure.RelationalSchema.HashCodes.Tests;

using Column = Column.Column;
using Index = Index.Index;

public sealed record IndexTests
{
    private readonly byte[] _typePrefix =
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

    [Fact]
    public void EnumeratesAsUntyped()
    {
        IIndex randomIndex = new RandomIndex();

        using IEnumerator<byte> expectedHash = SHA256
            .HashData(
                _typePrefix
                    .Concat(new DeterminedHash(randomIndex.IsUnique))
                    .Concat(new AggregatedHash(randomIndex.Columns.Select(x => new ColumnHash(x))))
                    .ToArray()
            )
            .AsEnumerable()
            .GetEnumerator();

        IEnumerable actualHash = new IndexHash(randomIndex);

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
        IIndex randomIndex = new RandomIndex();

        IEnumerable<byte> expectedHash = SHA256.HashData(
            _typePrefix
                .Concat(new DeterminedHash(randomIndex.IsUnique))
                .Concat(new AggregatedHash(randomIndex.Columns.Select(x => new ColumnHash(x))))
                .ToArray()
        );

        Assert.Equal(expectedHash, new IndexHash(randomIndex));
    }

    [Fact]
    public void ColumnsOrderNotMatter()
    {
        IIndex randomIndex = new RandomIndex();

        IIndex indexWithReversedColumns = new Index(
            randomIndex.IsUnique,
            randomIndex.Columns.Reverse()
        );

        Assert.Equal(
            new IndexHash(randomIndex).AsEnumerable(),
            new IndexHash(indexWithReversedColumns).AsEnumerable()
        );
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

        Assert.Equal(
            "975E8091A716FB7C61915C355449EDD67E98863259CC592D892F9639051C557E",
            Convert.ToHexString(new IndexHash(new Index(uniqueness, columns)).ToArray())
        );
    }

    [Fact]
    public void ThrowsExceptionOnGetHashCode()
    {
        Assert.Throws<NotSupportedException>(() => new IndexHash(new RandomIndex()).GetHashCode());
    }

    [Fact]
    public void ThrowsExceptionOnToString()
    {
        Assert.Throws<NotSupportedException>(() => new IndexHash(new RandomIndex()).ToString());
    }
}