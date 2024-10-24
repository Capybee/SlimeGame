using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

public class DropControler : MonoBehaviour
{
    [SerializeField] private List<DropList> DropListInstance; 

    public void Drop(EntityTypes EntityType, Vector3 Position)
    {
        System.Random Rnd = new System.Random();

        foreach (DropList DropListItem in DropListInstance)
        {
            if(EntityType == DropListItem.EntityType)
            {
                int Value = Rnd.Next(0,100);

                foreach (var Item in DropListItem.Drops)
                {
                    if(Item.MinValue < Value && Value < Item.MaxValue)
                    {
                        GameObject ObjectInstance = Instantiate(Item.Object);
                        ObjectInstance.transform.position = Position;
                    }
                }
            }
        }
    }

}

[Serializable]
public struct DropAndChance
{
    public GameObject Object;
    public int MinValue;
    public int MaxValue;
}

[Serializable]
public struct DropList
{
    public EntityTypes EntityType;
    public List<DropAndChance> Drops;
}
