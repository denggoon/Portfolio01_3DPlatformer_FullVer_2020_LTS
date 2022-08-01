using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using LitJson;
using Nvius.Util;
using UnityEngine.SceneManagement;

public class TriggerSpawnDataLoader : Singleton<TriggerSpawnDataLoader>
{
    public class TriggerSpawnData
    {
        public string markerName;
        public string prefabName;
        public string routeName;
    }

    public class SceneSpawnData
    {
        public string sceneName;
        public TriggerSpawnData[] spawnData;
    }

    private class SpawnData
    {
        public string prefabName;
        public string routeName;
    }

    private Dictionary<string /* sceneName */,
        Dictionary<string /* markeName */, SpawnData>> m_spawnData =
        new Dictionary<string, Dictionary<string, SpawnData>>();

    private bool isLoaded = false;

    public bool LoadScript(string jsonText)
    {
        var sceneSpawnDataList = LitJson.JsonMapper.ToObject<SceneSpawnData[]>(jsonText);
        foreach (var sceneSpawnData in sceneSpawnDataList)
        {
            var newSceneSpawnData = new Dictionary<string, SpawnData>();
            
            m_spawnData.Add(sceneSpawnData.sceneName, newSceneSpawnData);

            foreach (var triggerSpawnData in sceneSpawnData.spawnData)
            {
                var newSpawnData = new SpawnData();
                newSpawnData.prefabName = triggerSpawnData.prefabName;
                newSpawnData.routeName = triggerSpawnData.routeName;
                newSceneSpawnData.Add(triggerSpawnData.markerName, newSpawnData);
            }
        }

        isLoaded = true;
        return true;
    }

    public void Spawn(GameObject go)
    {
        //if (isLoaded == false)
        //{
        //    var testSpawnData = new SpawnData();
        //    testSpawnData.prefabName = "D_M_001_Mo";
        //    testSpawnData.routeName = "route_box";

        //    var sceneSpawnData = new Dictionary<string, SpawnData>();
        //    sceneSpawnData.Add("test", testSpawnData);
        //    m_spawnData.Add("alpha_1_1", sceneSpawnData);
        //}

        var spawnData = FindSpawnData(go.name);
        if (spawnData == null)
        {
            return;
        }

        var toBeSpawned = 
            (GameObject)ResourcesManager.instance.ResourcesLoadCached(spawnData.prefabName);
                
        if (toBeSpawned == null)
        {
            return;
        }

        var spawned = 
            (GameObject) GameObject.Instantiate(toBeSpawned, go.transform.position, go.transform.rotation);

        if (spawnData.routeName == null)
        {
            return;
        }
        else
        {
            PatrolRoute patrolRoute = spawned.GetComponentInChildren<PatrolRoute>();
            if (patrolRoute != null)
            {
                var route = GameObject.Find(spawnData.routeName);
                if (route != null && route.transform.childCount > 0)
                {
                    patrolRoute.goPatrolPattern = route;

                    patrolRoute.SendMessage("Awake");
                }
            }
        }
    }

    private SpawnData FindSpawnData(string markerName)
    {
        string currentSceneName = SceneManager.GetActiveScene().name;

        if (m_spawnData.ContainsKey(currentSceneName))
        {
            var sceneSpawnData = m_spawnData[currentSceneName];

            if (sceneSpawnData.ContainsKey(markerName))
            {
                return sceneSpawnData[markerName];
            }
        }

        return null;
    }
}
