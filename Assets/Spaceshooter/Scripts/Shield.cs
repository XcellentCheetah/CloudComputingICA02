using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]

public class Shield : Item
{
    public float duration;
    public int health; // number of hits the shield can sustain before getting damaged

    public Shield(string name, float duration, int health, int price)
    {
        this.name = name;
        this.price = price;
        this.duration = duration;
        this.health = health;
    }
}