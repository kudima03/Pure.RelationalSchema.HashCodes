using System.Collections;
using System.Security.Cryptography;
using Pure.HashCodes;
using Pure.HashCodes.Abstractions;
using Pure.Primitives.Abstractions.String;
using Pure.Primitives.Bool;
using Pure.RelationalSchema.Abstractions.Column;
using Pure.RelationalSchema.Abstractions.ForeignKey;
using Pure.RelationalSchema.Abstractions.Index;
using Pure.RelationalSchema.Abstractions.Schema;
using Pure.RelationalSchema.Abstractions.Table;
using Pure.RelationalSchema.ColumnType;
using Pure.RelationalSchema.Random;
using String = Pure.Primitives.String.String;

namespace Pure.RelationalSchema.HashCodes.Tests;

using Column = Column.Column;
using ForeignKey = ForeignKey.ForeignKey;
using Index = Index.Index;
using Schema = Schema.Schema;
using Table = Table.Table;

public sealed record SchemaHashTests
{
    private readonly byte[] _typePrefix =
    [
        253,
        165,
        151,
        1,
        96,
        51,
        234,
        121,
        155,
        41,
        25,
        146,
        55,
        243,
        188,
        110,
    ];

    [Fact]
    public void EnumeratesAsUntyped()
    {
        ISchema randomSchema = new RandomSchema();

        using IEnumerator<byte> expectedHash = SHA256
            .HashData(
                [
                    .. _typePrefix,
                    .. new DeterminedHash(randomSchema.Name),
                    .. new DeterminedHash(
                        randomSchema.Tables.Select(x => new TableHash(x))
                    ),
                    .. new DeterminedHash(
                        randomSchema.ForeignKeys.Select(x => new ForeignKeyHash(x))
                    ),
                ]
            )
            .AsEnumerable()
            .GetEnumerator();

        IEnumerable actualHash = new SchemaHash(randomSchema);

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
    public void CorrectComputingSteps()
    {
        ISchema randomSchema = new RandomSchema();

        IEnumerable<byte> expectedHash = SHA256.HashData(
            [
                .. _typePrefix,
                .. new DeterminedHash(randomSchema.Name),
                .. new DeterminedHash(randomSchema.Tables.Select(x => new TableHash(x))),
                .. new DeterminedHash(
                    randomSchema.ForeignKeys.Select(x => new ForeignKeyHash(x))
                ),
            ]
        );

        Assert.Equal(expectedHash, new SchemaHash(randomSchema));
    }

    [Fact]
    public void TablesOrderNotMatter()
    {
        ISchema randomSchema = new RandomSchema();

        ISchema schemaWithReversedTables = new Schema(
            randomSchema.Name,
            randomSchema.Tables.Reverse(),
            randomSchema.ForeignKeys
        );

        Assert.Equal(
            new SchemaHash(randomSchema).AsEnumerable(),
            new SchemaHash(schemaWithReversedTables).AsEnumerable()
        );
    }

    [Fact]
    public void ForeignKeysOrderNotMatter()
    {
        ISchema randomSchema = new RandomSchema();

        ISchema schemaWithReversedTables = new Schema(
            randomSchema.Name,
            randomSchema.Tables,
            randomSchema.ForeignKeys.Reverse()
        );

        Assert.Equal(
            new SchemaHash(randomSchema).AsEnumerable(),
            new SchemaHash(schemaWithReversedTables).AsEnumerable()
        );
    }

    [Fact]
    public void ProduceCorrectHashFromNameAndTablesAndForeignKeys()
    {
        ITable table = new Table(
            new String("Table1"),
            [new Column(new String("Col1"), new IntColumnType())],
            []
        );

        IForeignKey fk = new ForeignKey(
            table,
            table.Columns,
            table,
            table.Columns
        );

        IString name = new String("Schema1");
        IEnumerable<ITable> tables = [table];
        IEnumerable<IForeignKey> foreignKeys = [fk];

        SchemaHash expected = new SchemaHash(name, tables, foreignKeys);
        SchemaHash actual = new SchemaHash(name, tables, foreignKeys);

        Assert.True(expected.SequenceEqual(actual));
    }

    [Fact]
    public void ProduceCorrectHashFromNameHashAndTablesAndForeignKeys()
    {
        ITable table = new Table(
            new String("Table1"),
            [new Column(new String("Col1"), new IntColumnType())],
            []
        );

        IForeignKey fk = new ForeignKey(
            table,
            table.Columns,
            table,
            table.Columns
        );

        IDeterminedHash nameHash = new DeterminedHash(new String("Schema1"));
        IEnumerable<ITable> tables = [table];
        IEnumerable<IForeignKey> foreignKeys = [fk];

        SchemaHash expected = new SchemaHash(new String("Schema1"), tables, foreignKeys);
        SchemaHash actual = new SchemaHash(nameHash, tables, foreignKeys);

        Assert.True(expected.SequenceEqual(actual));
    }

    [Fact]
    public void ProduceCorrectHashFromNameAndTablesHashAndForeignKeys()
    {
        ITable table = new Table(
            new String("Table1"),
            [new Column(new String("Col1"), new IntColumnType())],
            []
        );

        IForeignKey fk = new ForeignKey(
            table,
            table.Columns,
            table,
            table.Columns
        );

        IString name = new String("Schema1");
        IEnumerable<ITable> tables = [table];
        IDeterminedHash tablesHash = new DeterminedHash([new TableHash(table)]);
        IEnumerable<IForeignKey> foreignKeys = [fk];

        SchemaHash expected = new SchemaHash(name, tables, foreignKeys);
        SchemaHash actual = new SchemaHash(name, tablesHash, foreignKeys);

        Assert.True(expected.SequenceEqual(actual));
    }

    [Fact]
    public void ProduceCorrectHashFromNameAndTablesAndForeignKeysHash()
    {
        ITable table = new Table(
            new String("Table1"),
            [new Column(new String("Col1"), new IntColumnType())],
            []
        );

        IForeignKey fk = new ForeignKey(
            table,
            table.Columns,
            table,
            table.Columns
        );

        IString name = new String("Schema1");
        IEnumerable<ITable> tables = [table];
        IDeterminedHash foreignKeysHash = new DeterminedHash([new ForeignKeyHash(fk)]);
        IEnumerable<IForeignKey> foreignKeys = [fk];

        SchemaHash expected = new SchemaHash(name, tables, foreignKeys);
        SchemaHash actual = new SchemaHash(name, tables, foreignKeysHash);

        Assert.True(expected.SequenceEqual(actual));
    }

    [Fact]
    public void ProduceCorrectHashFromNameHashTablesHashAndForeignKeys()
    {
        ITable table = new Table(
            new String("Table1"),
            [new Column(new String("Col1"), new IntColumnType())],
            []
        );

        IForeignKey fk = new ForeignKey(
            table,
            table.Columns,
            table,
            table.Columns
        );

        IDeterminedHash nameHash = new DeterminedHash(new String("Schema1"));
        IDeterminedHash tablesHash = new DeterminedHash([new TableHash(table)]);
        IEnumerable<IForeignKey> foreignKeys = [fk];

        SchemaHash expected = new SchemaHash(new String("Schema1"), [table], foreignKeys);
        SchemaHash actual = new SchemaHash(nameHash, tablesHash, foreignKeys);

        Assert.True(expected.SequenceEqual(actual));
    }

    [Fact]
    public void ProduceCorrectHashFromNameHashTablesAndForeignKeysHash()
    {
        ITable table = new Table(
            new String("Table1"),
            [new Column(new String("Col1"), new IntColumnType())],
            []
        );

        IForeignKey fk = new ForeignKey(
            table,
            table.Columns,
            table,
            table.Columns
        );

        IDeterminedHash nameHash = new DeterminedHash(new String("Schema1"));
        IEnumerable<ITable> tables = [table];
        IDeterminedHash foreignKeysHash = new DeterminedHash([new ForeignKeyHash(fk)]);

        SchemaHash expected = new SchemaHash(new String("Schema1"), tables, [fk]);
        SchemaHash actual = new SchemaHash(nameHash, tables, foreignKeysHash);

        Assert.True(expected.SequenceEqual(actual));
    }

    [Fact]
    public void ProduceCorrectHashFromNameTablesHashAndForeignKeysHash()
    {
        ITable table = new Table(
            new String("Table1"),
            [new Column(new String("Col1"), new IntColumnType())],
            []
        );

        IForeignKey fk = new ForeignKey(
            table,
            table.Columns,
            table,
            table.Columns
        );

        IString name = new String("Schema1");
        IDeterminedHash tablesHash = new DeterminedHash([new TableHash(table)]);
        IDeterminedHash foreignKeysHash = new DeterminedHash([new ForeignKeyHash(fk)]);

        SchemaHash expected = new SchemaHash(name, [table], [fk]);
        SchemaHash actual = new SchemaHash(name, tablesHash, foreignKeysHash);

        Assert.True(expected.SequenceEqual(actual));
    }

    [Fact]
    public void ProduceCorrectHashFromAllHashes()
    {
        ITable table = new Table(
            new String("Table1"),
            [new Column(new String("Col1"), new IntColumnType())],
            []
        );

        IForeignKey fk = new ForeignKey(
            table,
            table.Columns,
            table,
            table.Columns
        );

        IDeterminedHash nameHash = new DeterminedHash(new String("Schema1"));
        IDeterminedHash tablesHash = new DeterminedHash([new TableHash(table)]);
        IDeterminedHash foreignKeysHash = new DeterminedHash([new ForeignKeyHash(fk)]);

        SchemaHash expected = new SchemaHash(new String("Schema1"), [table], [fk]);
        SchemaHash actual = new SchemaHash(nameHash, tablesHash, foreignKeysHash);

        Assert.True(expected.SequenceEqual(actual));
    }

    [Fact]
    public void Determined()
    {
        IString name = new String("fdjbhvn");

        IReadOnlyCollection<IColumn> columns =
        [
            new Column(new String("tyghdntygrhhgrty"), new DateColumnType()),
            new Column(new String("srfgbzERWGerg"), new TimeColumnType()),
            new Column(new String("fdjbWAREREWGRGhvn"), new UShortColumnType()),
            new Column(new String("fdjbhyufjkjyhukvn"), new LongColumnType()),
            new Column(new String("fdjb3q45t4rhvn"), new IntColumnType()),
        ];

        IReadOnlyCollection<IIndex> indexes =
        [
            new Index(new True(), columns.Take(2)),
            new Index(new True(), columns.Skip(2).Take(2)),
            new Index(new True(), columns.Take(1)),
            new Index(new True(), columns.Skip(2).Take(1)),
        ];

        IReadOnlyCollection<ITable> tables =
        [
            new Table(new String("inerbgfhbnig"), columns, indexes.Take(2)),
            new Table(new String("pu9jWQPHJUI9"), columns, indexes.Skip(2).Take(2)),
            new Table(new String("DAFNdsflkgmvkjo"), columns, indexes.Skip(3).Take(1)),
            new Table(new String("EWQOIJRPOIJUe"), columns, indexes.Skip(1).Take(1)),
        ];

        IReadOnlyCollection<IForeignKey> foreignKeys =
        [
            new ForeignKey(
                tables.First(),
                [tables.First().Columns.First()],
                tables.Skip(1).First(),
                [tables.Skip(1).First().Columns.First()]
            ),
            new ForeignKey(
                tables.Skip(2).First(),
                [tables.Skip(2).First().Columns.First()],
                tables.Skip(3).First(),
                [tables.Skip(3).First().Columns.First()]
            ),
        ];

        Assert.Equal(
            "CA2280C217ACD33B5FBD7E32CE7B9BCEA3FAE6356BFADFB3D6FCCFBDF9AEDC2C",
            Convert.ToHexString(
                new SchemaHash(new Schema(name, tables, foreignKeys)).ToArray()
            )
        );
    }

    [Fact]
    public void ThrowsExceptionOnGetHashCode()
    {
        _ = Assert.Throws<NotSupportedException>(() =>
            new SchemaHash(new RandomSchema()).GetHashCode()
        );
    }

    [Fact]
    public void ThrowsExceptionOnToString()
    {
        _ = Assert.Throws<NotSupportedException>(() =>
            new SchemaHash(new RandomSchema()).ToString()
        );
    }
}
