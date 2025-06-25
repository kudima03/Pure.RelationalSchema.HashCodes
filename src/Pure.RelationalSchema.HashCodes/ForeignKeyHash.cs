using Pure.HashCodes;
using Pure.RelationalSchema.Abstractions.ForeignKey;
using System.Collections;

namespace Pure.RelationalSchema.HashCodes;

public sealed record ForeignKeyHash : IDeterminedHash
{
    private static readonly byte[] TypePrefix =
    [
        196, 165, 151, 1, 153, 27, 106, 112, 143, 29, 159, 81, 46, 52, 46, 148
    ];

    private readonly IForeignKey _foreignKey;

    public ForeignKeyHash(IForeignKey foreignKey)
    {
        _foreignKey = foreignKey;
    }

    public IEnumerator<byte> GetEnumerator()
    {
        return new DeterminedHash(TypePrefix
                .Concat(new TableHash(_foreignKey.ReferencingTable))
                .Concat(new ColumnHash(_foreignKey.ReferencingColumn))
                .Concat(new TableHash(_foreignKey.ReferencedTable))
                .Concat(new ColumnHash(_foreignKey.ReferencedColumn)))
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