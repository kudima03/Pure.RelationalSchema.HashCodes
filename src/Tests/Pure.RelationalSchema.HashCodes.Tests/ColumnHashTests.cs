using Pure.HashCodes;
using Pure.RelationalSchema.Abstractions.Column;
using Pure.RelationalSchema.Random;
using System.Collections;
using System.Security.Cryptography;

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
                _typePrefix
                    .Concat(new DeterminedHash(randomColumn.Name))
                    .Concat(new ColumnTypeHash(randomColumn.Type))
                    .ToArray()
            )
            .AsEnumerable()
            .GetEnumerator();

        IEnumerable actualHash = new ColumnHash(randomColumn);

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
        IColumn randomColumn = new RandomColumn();
        IDeterminedHash nameHash = new DeterminedHash(randomColumn.Name);
        IDeterminedHash columnTypeHash = new ColumnTypeHash(randomColumn.Type);

        Assert.Equal(
            SHA256.HashData(_typePrefix.Concat(nameHash).Concat(columnTypeHash).ToArray()),
            new ColumnHash(randomColumn)
        );
    }

    [Fact]
    public void ThrowsExceptionOnGetHashCode()
    {
        Assert.Throws<NotSupportedException>(() =>
            new ColumnHash(new RandomColumn()).GetHashCode()
        );
    }

    [Fact]
    public void ThrowsExceptionOnToString()
    {
        Assert.Throws<NotSupportedException>(() => new ColumnHash(new RandomColumn()).ToString());
    }
}