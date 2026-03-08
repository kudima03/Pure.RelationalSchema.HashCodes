using System.Collections;
using Pure.HashCodes;
using Pure.HashCodes.Abstractions;
using Pure.Primitives.Abstractions.String;
using Pure.RelationalSchema.Abstractions.ColumnType;

namespace Pure.RelationalSchema.HashCodes;

public sealed record ColumnHash : IDeterminedHash
{
    private static readonly byte[] TypePrefix =
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

    private readonly IDeterminedHash _nameHash;
    private readonly IDeterminedHash _typeHash;

    public ColumnHash(IString name, IColumnType type)
       : this(new DeterminedHash(name), new ColumnTypeHash(type.Name)) { }

    public ColumnHash(IDeterminedHash nameHash, IColumnType type)
        : this(nameHash, new ColumnTypeHash(type.Name)) { }

    public ColumnHash(IString name, IDeterminedHash typeHash)
        : this(new DeterminedHash(name), typeHash) { }

    public ColumnHash(IDeterminedHash nameHash, IDeterminedHash typeHash)
    {
        _nameHash = nameHash;
        _typeHash = typeHash;
    }

    public IEnumerator<byte> GetEnumerator()
    {
        return new DeterminedHash(
            TypePrefix.Concat(_nameHash).Concat(_typeHash)
        ).GetEnumerator();
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
