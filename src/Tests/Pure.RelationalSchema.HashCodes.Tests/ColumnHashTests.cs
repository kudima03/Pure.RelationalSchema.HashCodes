using System.Collections;
using System.Security.Cryptography;
using Pure.HashCodes;
using Pure.HashCodes.Abstractions;
using Pure.RelationalSchema.Abstractions.Column;
using Pure.RelationalSchema.Random;

namespace Pure.RelationalSchema.HashCodes.Tests;

public sealed record ColumnHashTests
{
    private readonly byte[] _typePrefix =
    [
        41,
        163,
        151,
        1,
        29,
        173,
        73,
        119,
        138,
        216,
        188,
        7,
        188,
        71,
        127,
        69,
    ];

    [Fact]
    public void EnumeratesAsUntyped()
    {
        IColumn randomColumn = new RandomColumn();

        using IEnumerator<byte> expectedHash = SHA256
            .HashData(
                [
                    .. _typePrefix,
                    .. new DeterminedHash(randomColumn.Name),
                    .. new ColumnTypeHash(randomColumn.Type),
                ]
            )
            .AsEnumerable()
            .GetEnumerator();

        IEnumerable actualHash = new ColumnHash(randomColumn);

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
    public void ProduceCorrectHash()
    {
        IColumn randomColumn = new RandomColumn();
        IDeterminedHash nameHash = new DeterminedHash(randomColumn.Name);
        IDeterminedHash columnTypeHash = new ColumnTypeHash(randomColumn.Type);

        Assert.Equal(
            SHA256.HashData([.. _typePrefix, .. nameHash, .. columnTypeHash]),
            new ColumnHash(randomColumn)
        );
    }

    [Fact]
    public void ProduceCorrectHashFromNameAndType()
    {
        IColumn randomColumn = new RandomColumn();

        Assert.Equal(
            new ColumnHash(randomColumn),
            new ColumnHash(randomColumn.Name, randomColumn.Type)
        );
    }

    [Fact]
    public void ProduceCorrectHashFromNameHashAndType()
    {
        IColumn randomColumn = new RandomColumn();
        IDeterminedHash nameHash = new DeterminedHash(randomColumn.Name);

        Assert.Equal(
            new ColumnHash(randomColumn),
            new ColumnHash(nameHash, randomColumn.Type)
        );
    }

    [Fact]
    public void ProduceCorrectHashFromNameAndTypeHash()
    {
        IColumn randomColumn = new RandomColumn();
        IDeterminedHash typeHash = new ColumnTypeHash(randomColumn.Type);

        Assert.Equal(
            new ColumnHash(randomColumn),
            new ColumnHash(randomColumn.Name, typeHash)
        );
    }

    [Fact]
    public void ProduceCorrectHashFromHashes()
    {
        IColumn randomColumn = new RandomColumn();

        IDeterminedHash nameHash = new DeterminedHash(randomColumn.Name);
        IDeterminedHash typeHash = new ColumnTypeHash(randomColumn.Type);

        Assert.Equal(
            new ColumnHash(randomColumn),
            new ColumnHash(nameHash, typeHash)
        );
    }

    [Fact]
    public void ThrowsExceptionOnGetHashCode()
    {
        _ = Assert.Throws<NotSupportedException>(() =>
            new ColumnHash(new RandomColumn()).GetHashCode()
        );
    }

    [Fact]
    public void ThrowsExceptionOnToString()
    {
        _ = Assert.Throws<NotSupportedException>(() =>
            new ColumnHash(new RandomColumn()).ToString()
        );
    }
}
