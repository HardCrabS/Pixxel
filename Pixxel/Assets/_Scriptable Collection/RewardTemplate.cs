using System.Text.RegularExpressions;
using UnityEngine;

[CreateAssetMenu(menuName = "Reward")]
public abstract class RewardTemplate : ScriptableObject 
{
    public LevelReward reward;

    public virtual Sprite GetRewardSprite()
    {
        return null;
    }
    public abstract string GetRewardId();

    public static string SplitCamelCase(string source)
    {
        var r = new Regex(@"
                (?<=[A-Z])(?=[A-Z][a-z]) |
                 (?<=[^A-Z])(?=[A-Z]) |
                 (?<=[A-Za-z])(?=[^A-Za-z])", RegexOptions.IgnorePatternWhitespace);

        return r.Replace(source, " ");
    }
}