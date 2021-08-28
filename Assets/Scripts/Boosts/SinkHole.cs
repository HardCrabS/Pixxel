using System.Collections;
using UnityEngine;
using DG.Tweening;

public class SinkHole : BoostBase
{
    AudioClip sinkShakeSFX, sinkDipSFX;
    public override void ExecuteBonus()
    {
        base.ExecuteBonus();
        GridA.Instance.currState = GameState.wait; //disallow block movement

        if (sinkShakeSFX == null)
        {
            sinkShakeSFX = Resources.Load<AudioClip>(RESOURCES_FOLDER + "Sinkhole/SFX-boost-sinkhole-shake");
            sinkDipSFX = Resources.Load<AudioClip>(RESOURCES_FOLDER + "Sinkhole/sfx-boost-sinkhole-dip");
        }
        if (LevelSettingsKeeper.settingsKeeper)
        {

            StartCoroutine(SinkAllBlocks());
        }
    }
    IEnumerator SinkAllBlocks()
    {
        audioSource.PlayOneShot(sinkShakeSFX, 0.5f);
        Camera.main.GetComponent<CameraShake>().ShakeCam(1f, 0.2f);
        yield return new WaitForSeconds(0.8f);

        var boxes = LevelSettingsKeeper.settingsKeeper.worldInformation.Boxes;
        string tag = boxes[Random.Range(0, boxes.Length)].tag;
        SinkBlocks(tag);//sink all blocks
        yield return new WaitForSeconds(1.1f);//wait while blocks are sinking and being destroyed
        StartCoroutine(GridA.Instance.MoveBoxesDown());//move new blocks down
        GridA.Instance.currState = GameState.move;
        finished = true;
    }
    void SinkBlocks(string tag)
    {
        audioSource.PlayOneShot(sinkDipSFX);
        var grid = GridA.Instance;
        foreach (GameObject go in grid.allBoxes)
        {
            if (go != null && go.CompareTag(tag))
            {
                StartCoroutine(BlockSinkAndDestroy(go));
            }
        }
    }
    IEnumerator BlockSinkAndDestroy(GameObject block)
    {
        Box boxComp = block.GetComponent<Box>();
        boxComp.enabled = false;//turn off snapping to it's pos

        //scale down to 0
        block.transform.DOScale(0, 1);
        //move block down a bit
        Vector3 destination = block.transform.localPosition + Vector3.down * 1f;
        yield return block.transform.DOLocalMove(destination, 1f).WaitForCompletion();

        GridA.Instance.DestroyBlockAtPosition(boxComp.row, boxComp.column, useDestructionFX:false);//destroy block
    }
}