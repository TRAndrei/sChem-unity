﻿using System;

public sealed class LinkKey
{
    private string first;
    private string second;

    public LinkKey(string first, string second)
    {
        this.first = first;
        this.second = second;
    }

    public string GetOther(string element)
    {
        if (first == element)
        {
            return second;
        } else
        {
            return first;
        }
    }

    public override bool Equals(object obj)
    {
        return obj is LinkKey key &&
               (first == key.first &&
               second == key.second || first == key.second &&
               second == key.first);
    }

    public override int GetHashCode()
    {
        return first.GetHashCode() ^ second.GetHashCode();
    }
}
