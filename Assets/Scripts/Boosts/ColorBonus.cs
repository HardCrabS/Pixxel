using System.Collections;

public class ColorBonus : BoostBase
{
    public override void ExecuteBonus()
    {
        if (LevelSettingsKeeper.settingsKeeper)
        {
            StartCoroutine(DestroyAllSameColor());
        }
    }
    IEnumerator DestroyAllSameColor()
    {
        var boxes = LevelSettingsKeeper.settingsKeeper.worldInfo.Boxes;
        string tag = boxes[UnityEngine.Random.Range(0, boxes.Length)].tag;
        yield return StartCoroutine(GridA.Instance.DestroyAllSameColor(tag));
        finished = true;
    }
}