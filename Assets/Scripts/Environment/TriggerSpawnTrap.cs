using UnityEngine;
using System.Collections;

public class TriggerSpawnTrap : Trap
{

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (isTriggered == false)
        {
            return;
        }
        else 
        {
            TriggerSpawnDataLoader.Instance.Spawn(gameObject);
            Destroy(gameObject);
        }
    }
}
