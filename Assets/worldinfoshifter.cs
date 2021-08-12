using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class worldinfoshifter : MonoBehaviour
{
    [SerializeField] WorldInformation[] worldInfos;
    [SerializeField] WorldLoadInfo[] worldLoadInfos;

    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < worldInfos.Length; i++)
        {
            //worldInfos[i].boxes = worldLoadInfos[i].boxes;
        }
    }
}