using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GemsAudio : MonoBehaviour 
{
    [SerializeField] AudioScaleParameters[] scaleParameters;
    Box box;
    AudioSyncScale syncScale;
	// Use this for initialization
	void Start () 
    {
        //box = GetComponent<Box>();
        syncScale = GetComponent<AudioSyncScale>();
        //int column = box.column;

        int index = (int)ToEnum<BlockTags>(gameObject.tag);
        
        syncScale.bias = scaleParameters[index].bias;
        syncScale.timeStep = scaleParameters[index].timeStep;
        syncScale.timeToBeat = scaleParameters[index].timeToBeat;
        syncScale.restSmoothTime = scaleParameters[index].restTime;
        syncScale.beatScale = scaleParameters[index].beatScale;
    }
    public static T ToEnum<T>(string value)
    {
        return (T)System.Enum.Parse(typeof(T), value, true);
    }
}

[System.Serializable]
class AudioScaleParameters
{
    public float bias;
    public float timeStep;
    public float timeToBeat;
    public float restTime;
    public Vector2 beatScale;
}