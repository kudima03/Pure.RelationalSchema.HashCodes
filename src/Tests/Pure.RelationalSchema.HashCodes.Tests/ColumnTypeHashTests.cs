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
        byte[] typePrefix = [8, 157, 151, 1, 149, 98, 28, 119, 130, 158, 187, 34, 130, 255, 222, 135];

        IColumnType columnType = new StringColumnType();

        IDeterminedHash nameHash = new DeterminedHash(columnType.Name);

        IEnumerable expectedHash = SHA256.HashData(typePrefix.Concat(nameHash).ToArray());

        using IEnumerator<byte> actualHash = new ColumnTypeHash(columnType).GetEnumerator();

        bool different = false;

        foreach (object i in expectedHash)
        {
            actualHash.MoveNext();
            if (actualHash.Current != (byte)i)
            {
                different = true;
                break;
            }
        }

        Assert.False(different);
    }

    [Fact]
    public void ProduceCorrectHash()
    {
        byte[] typePrefix = [8, 157, 151, 1, 149, 98, 28, 119, 130, 158, 187, 34, 130, 255, 222, 135];

        IColumnType columnType = new StringColumnType();

        IDeterminedHash nameHash = new DeterminedHash(columnType.Name);

        IEnumerable<byte> hash = SHA256.HashData(typePrefix.Concat(nameHash).ToArray());

        Assert.Equal(hash, new ColumnTypeHash(columnType));
    }

    [Fact]
    public void ThrowsExceptionOnGetHashCode()
    {
        Assert.Throws<NotSupportedException>(() => new ColumnTypeHash(new DateColumnType()).GetHashCode());
    }

    [Fact]
    public void ThrowsExceptionOnToString()
    {
        Assert.Throws<NotSupportedException>(() => new ColumnTypeHash(new DateColumnType()).ToString());
    }
}