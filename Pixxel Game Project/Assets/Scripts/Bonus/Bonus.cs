using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

[Serializable]
public class Bonus
{
    public List<SerializableBoost> boosts;

    public Bonus()
    {
        boosts = new List<SerializableBoost>();
    }
    public void SaveBonus(SerializableBoost newBoost)
    {
        for (int i = 0; i < boosts.Count; i++)
        {
            if (boosts[i].stringType == newBoost.stringType)
            {
                boosts[i] = newBoost;
                break;
            }
        }
    }
    public void SetAllBonusDefault()
    {
         foreach (Type mytype in System.Reflection.Assembly.GetExecutingAssembly().GetTypes()
                  .Where(mytype => mytype.GetInterfaces().Contains(typeof(IConcreteBonus))))
         {
             SerializableBoost boost = new SerializableBoost(mytype.Name, 1);
             boosts.Add(boost);
         }
    }

    public int GetLevel(Type type)
    {
        int level = 1;
        for (int i = 0; i < boosts.Count; i++)
        {
            if (boosts[i].stringType == type.Name)
            {
                level = boosts[i].level;
                break;
            }
        }
        return level;
    }
}

[Serializable]
public class SerializableBoost
{
    public string stringType;
    public int level;

    public SerializableBoost(string type, int lvl)
    {
        this.stringType = type;
        level = lvl;
    }
}