﻿using Pure.Primitives.Bool;
using Pure.Primitives.String;
using Pure.RelationalSchema.Abstractions.Column;
using Pure.RelationalSchema.Abstractions.ForeignKey;
using Pure.RelationalSchema.Abstractions.Index;
using Pure.RelationalSchema.Abstractions.Table;
using Pure.RelationalSchema.ColumnType;
using System.Collections;
using System.Security.Cryptography;
using String = Pure.Primitives.String.String;

namespace Pure.RelationalSchema.HashCodes.Tests;

using Column = Column.Column;
using ForeignKey = ForeignKey.ForeignKey;
using Index = Index.Index;
using Table = Table.Table;

public sealed record ForeignKeyHashTests
{
    [Fact]
    public void EnumeratesAsUntyped()
    {
        byte[] typePrefix = [196, 165, 151, 1, 153, 27, 106, 112, 143, 29, 159, 81, 46, 52, 46, 148];

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
            new Index(new True(), columns.Skip(2).Take(2))
        ];

        ITable referencingTable = new Table(new String("Sample name"), columns.Take(2), indexes.Take(1));
        ITable referencedTable = new Table(new String("Sample name1"), columns.Skip(2).Take(2), indexes.Skip(1).Take(1));

        using IEnumerator<byte> expectedHash = SHA256.HashData(typePrefix
                .Concat(new TableHash(referencingTable))
                .Concat(new ColumnHash(referencingTable.Columns.First()))
                .Concat(new TableHash(referencedTable))
                .Concat(new ColumnHash(referencedTable.Columns.First()))
                .ToArray())
            .AsEnumerable()
            .GetEnumerator();

        IEnumerable actualHash = new ForeignKeyHash(new ForeignKey(referencingTable,
            referencingTable.Columns.First(),
            referencedTable,
            referencedTable.Columns.First()));

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
        byte[] typePrefix = [196, 165, 151, 1, 153, 27, 106, 112, 143, 29, 159, 81, 46, 52, 46, 148];

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
            new Index(new True(), columns.Skip(2).Take(2))
        ];

        ITable referencingTable = new Table(new String("Sample name"), columns.Take(2), indexes.Take(1));
        ITable referencedTable = new Table(new String("Sample name1"), columns.Skip(2).Take(2), indexes.Skip(1).Take(1));

        Assert.Equal(
            SHA256.HashData(typePrefix
                .Concat(new TableHash(referencingTable))
                .Concat(new ColumnHash(referencingTable.Columns.First()))
                .Concat(new TableHash(referencedTable))
                .Concat(new ColumnHash(referencedTable.Columns.First()))
                .ToArray()),
            new ForeignKeyHash(new ForeignKey(referencingTable,
                referencingTable.Columns.First(),
                referencedTable,
                referencedTable.Columns.First())));
    }

    [Fact]
    public void Determined()
    {
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
            new Index(new True(), columns.Skip(2).Take(2))
        ];

        ITable referencingTable = new Table(new String("Sample name"), columns.Take(2), indexes.Take(1));
        ITable referencedTable = new Table(new String("Sample name1"), columns.Skip(2).Take(2), indexes.Skip(1).Take(1));

        IForeignKey foreignKey = new ForeignKey(referencingTable,
            referencingTable.Columns.First(),
            referencedTable,
            referencedTable.Columns.First());

        Assert.Equal("F43B8C97B9146B1392649302110A95D821EC085461CD463EE67CE9F7F65C5F1E",
            Convert.ToHexString(new ForeignKeyHash(foreignKey).ToArray()));
    }

    [Fact]
    public void ThrowsExceptionOnGetHashCode()
    {
        Assert.Throws<NotSupportedException>(() =>
            new ForeignKeyHash(
                new ForeignKey(
                    new Table(new EmptyString(), [], []),
                    new Column(new EmptyString(), new DateColumnType()),
                    new Table(new EmptyString(), [], []),
                    new Column(new EmptyString(), new IntColumnType())))
                .GetHashCode());
    }

    [Fact]
    public void ThrowsExceptionOnToString()
    {
        Assert.Throws<NotSupportedException>(() =>
            new ForeignKeyHash(
                    new ForeignKey(
                        new Table(new EmptyString(), [], []),
                        new Column(new EmptyString(), new DateColumnType()),
                        new Table(new EmptyString(), [], []),
                        new Column(new EmptyString(), new IntColumnType())))
                .ToString());
    }
}