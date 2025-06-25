using Pure.HashCodes;
using Pure.RelationalSchema.Abstractions.Schema;
using System.Collections;

namespace Pure.RelationalSchema.HashCodes;

public sealed record SchemaHash : IDeterminedHash
{
    private static readonly byte[] TypePrefix =
    [
        253, 165, 151, 1, 96, 51, 234, 121, 155, 41, 25, 146, 55, 243, 188, 110
    ];

    private readonly ISchema _schema;

    public SchemaHash(ISchema schema)
    {
        _schema = schema;
    }

    public IEnumerator<byte> GetEnumerator()
    {
        return new DeterminedHash(TypePrefix
                .Concat(new DeterminedHash(_schema.Name))
                .Concat(new AggregatedHash(_schema.Tables.Select(column => new TableHash(column))))
                .Concat(new AggregatedHash(_schema.ForeignKeys.Select(foreignKey => new ForeignKeyHash(foreignKey)))))
            .GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    public override int GetHashCode()
    {
        throw new NotSupportedException();
    }

    public override string ToString()
    {
        throw new NotSupportedException();
    }
}