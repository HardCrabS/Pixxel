public class ColorBonus : BoostBase
{
    public override void ExecuteBonus()
    {
        if (LevelSettingsKeeper.settingsKeeper)
        {
            var boxes = LevelSettingsKeeper.settingsKeeper.worldInfo.Boxes;
            string tag = boxes[UnityEngine.Random.Range(0, boxes.Length)].tag;
            StartCoroutine(GridA.Instance.DestroyAllSameColor(tag));
        }
    }
}