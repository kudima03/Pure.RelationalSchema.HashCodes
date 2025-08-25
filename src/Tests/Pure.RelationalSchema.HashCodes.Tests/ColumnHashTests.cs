using System.Collections;
using System.Security.Cryptography;
using Pure.HashCodes;
using Pure.Primitives.Abstractions.String;
using Pure.Primitives.Number;
using Pure.Primitives.Random.String;
using Pure.Primitives.String;
using Pure.RelationalSchema.Abstractions.ColumnType;
using Pure.RelationalSchema.ColumnType;

using Column = Column.Column;

namespace Pure.RelationalSchema.HashCodes.Tests;
public sealed record ColumnHashTests
{
    [Fact]
    public void EnumeratesAsUntyped()
    {
        byte[] typePrefix =
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

        IString name = new RandomString(new UShort(10));
        IColumnType columnType = new StringColumnType();
        IDeterminedHash nameHash = new DeterminedHash(name);
        IDeterminedHash columnTypeHash = new ColumnTypeHash(columnType);

        using IEnumerator<byte> expectedHash = SHA256
            .HashData([.. typePrefix, .. nameHash, .. columnTypeHash])
            .AsEnumerable()
            .GetEnumerator();

        IEnumerable actualHash = new ColumnHash(new Column(name, columnType));

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
        byte[] typePrefix =
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

        IString name = new RandomString(new UShort(10));
        IColumnType columnType = new StringColumnType();
        IDeterminedHash nameHash = new DeterminedHash(name);
        IDeterminedHash columnTypeHash = new ColumnTypeHash(columnType);

        Assert.Equal(
            SHA256.HashData([.. typePrefix, .. nameHash, .. columnTypeHash]),
            new ColumnHash(new Column(name, columnType))
        );
    }

    [Fact]
    public void ThrowsExceptionOnGetHashCode()
    {
        Assert.Throws<NotSupportedException>(() =>
            new ColumnHash(
                new Column(new EmptyString(), new UIntColumnType())
            ).GetHashCode()
        );
    }

    [Fact]
    public void ThrowsExceptionOnToString()
    {
        Assert.Throws<NotSupportedException>(() =>
            new ColumnHash(new Column(new EmptyString(), new UIntColumnType())).ToString()
        );
    }
}
