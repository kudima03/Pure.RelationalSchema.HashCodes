using Pure.HashCodes;
using Pure.RelationalSchema.Abstractions.ColumnType;
using Pure.RelationalSchema.ColumnType;
using System.Collections;
using System.Security.Cryptography;

namespace Pure.RelationalSchema.HashCodes.Tests;

public sealed record ColumnTypeHashTests
{
    [Fact]
    public void EnumeratesAsUntyped()
    {
        byte[] typePrefix =
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

        IColumnType columnType = new StringColumnType();

        IDeterminedHash nameHash = new DeterminedHash(columnType.Name);

        using IEnumerator<byte> expectedHash = SHA256
            .HashData(typePrefix.Concat(nameHash).ToArray())
            .AsEnumerable()
            .GetEnumerator();

        IEnumerable actualHash = new ColumnTypeHash(columnType);

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
        byte[] typePrefix =
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

        IColumnType columnType = new StringColumnType();

        IDeterminedHash nameHash = new DeterminedHash(columnType.Name);

        IEnumerable<byte> hash = SHA256.HashData(typePrefix.Concat(nameHash).ToArray());

        Assert.Equal(hash, new ColumnTypeHash(columnType));
    }

    [Fact]
    public void ThrowsExceptionOnGetHashCode()
    {
        Assert.Throws<NotSupportedException>(() =>
            new ColumnTypeHash(new DateColumnType()).GetHashCode()
        );
    }

    [Fact]
    public void ThrowsExceptionOnToString()
    {
        Assert.Throws<NotSupportedException>(() =>
            new ColumnTypeHash(new DateColumnType()).ToString()
        );
    }
}