using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]

public abstract class Item
{
    public string name;
    public int price;
    public override bool Equals(object obj)
    {
        if (obj is Item otherItem)
        {
            return this.name == otherItem.name;
        }
        return false;
    }

    public override int GetHashCode()
    {
        return name != null ? name.GetHashCode() : 0;
    }
}
