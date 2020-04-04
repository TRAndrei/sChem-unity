using System;

public sealed class RuleKey
{
    private string firstType;
    private string secondType;

    public RuleKey(string firstType, string secondType)
    {
        this.firstType = firstType;
        this.secondType = secondType;
    }

    public override bool Equals(object obj)
    {
        return obj is RuleKey key &&
              firstType == key.firstType &&
               secondType == key.secondType;
    }

    public override int GetHashCode()
    {
        return firstType.GetHashCode() ^ secondType.GetHashCode();
    }
}
