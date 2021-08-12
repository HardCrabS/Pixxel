using System;
using System.Collections;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public class WorldLoader : MonoBehaviour
{
    private AsyncOperationHandle _currentSkyboxMaterialOperationHandle;

    const string WORLD_LOAD_INFO_PATH = "Assets/_Scriptable Collection/Worlds Info/LoadInfo/";

    public WorldLoadInfo loadedWorldInfo { get; private set; }

    public void LoadWorldInfoAsync(string worldId, Action<WorldLoadInfo> action)
    {
        StartCoroutine(LoadWorldInfoInternal(worldId, action));
    }

    IEnumerator LoadWorldInfoInternal(string worldId, Action<WorldLoadInfo> action)
    {
        if(_currentSkyboxMaterialOperationHandle.IsValid())
        {
            Addressables.Release(_currentSkyboxMaterialOperationHandle);
        }

        string worldPath = WORLD_LOAD_INFO_PATH + worldId;
        _currentSkyboxMaterialOperationHandle = Addressables.LoadAssetAsync<WorldLoadInfo>(worldPath);
        yield return _currentSkyboxMaterialOperationHandle;
        WorldLoadInfo info = _currentSkyboxMaterialOperationHandle.Result as WorldLoadInfo;
        action.Invoke(info);
    }
    public IEnumerator LoadWorldInfoInternal(string worldId)
    {
        if (_currentSkyboxMaterialOperationHandle.IsValid())
        {
            Addressables.Release(_currentSkyboxMaterialOperationHandle);
        }

        string worldPath = WORLD_LOAD_INFO_PATH + worldId;
        _currentSkyboxMaterialOperationHandle = Addressables.LoadAssetAsync<WorldLoadInfo>(worldPath);
        yield return _currentSkyboxMaterialOperationHandle;
        loadedWorldInfo = _currentSkyboxMaterialOperationHandle.Result as WorldLoadInfo;
    }
}