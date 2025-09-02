using Pure.HashCodes;
using Pure.RelationalSchema.Abstractions.ColumnType;
using Pure.RelationalSchema.Random;
using System.Collections;
using System.Security.Cryptography;

namespace Pure.RelationalSchema.HashCodes.Tests;

public sealed record ColumnTypeHashTests
{
    private readonly byte[] _typePrefix =
    [
        8,
        157,
        151,
        1,
        149,
        98,
        28,
        119,
        130,
        158,
        187,
        34,
        130,
        255,
        222,
        135,
    ];

    [Fact]
    public void EnumeratesAsUntyped()
    {
        IColumnType randomColumnType = new RandomColumnType();

        using IEnumerator<byte> expectedHash = SHA256
            .HashData(_typePrefix.Concat(new DeterminedHash(randomColumnType.Name)).ToArray())
            .AsEnumerable()
            .GetEnumerator();

        IEnumerable actualHash = new ColumnTypeHash(randomColumnType);

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
        IColumnType columnType = new RandomColumnType();

        IEnumerable<byte> expectedHash = SHA256.HashData(
            _typePrefix.Concat(new DeterminedHash(columnType.Name)).ToArray()
        );

        Assert.Equal(expectedHash, new ColumnTypeHash(columnType));
    }

    [Fact]
    public void ThrowsExceptionOnGetHashCode()
    {
        Assert.Throws<NotSupportedException>(() =>
            new ColumnTypeHash(new RandomColumnType()).GetHashCode()
        );
    }

    [Fact]
    public void ThrowsExceptionOnToString()
    {
        Assert.Throws<NotSupportedException>(() =>
            new ColumnTypeHash(new RandomColumnType()).ToString()
        );
    }
}