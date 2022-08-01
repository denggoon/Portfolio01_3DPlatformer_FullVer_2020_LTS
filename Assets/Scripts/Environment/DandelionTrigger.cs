using UnityEngine;
using System.Collections;

public class DandelionTrigger : MonoBehaviour
{
    private BoxCollider boxCollider;

    public ParticleSystem effect;
    public GameObject initialEffect;

    public GameObject toBeSpawned;
    public float yPos;

    void Awake()
    {
        boxCollider = GetComponent<BoxCollider>();
    }

    // Use this for initialization
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
    }

    void OnTriggerEnter(Collider collider)
    {
        if (collider.gameObject.layer == LayerMask.NameToLayer("Player")) //감지한 컬라이더가 플레이어인 경우 
        {
            if (SoundBoard.instance != null)
            {
                SoundBoard.instance.PlayFromSoundBoard("SND_BGO_DANDELION", this.transform.position);
            }

            effect.Play();

            Destroy(initialEffect);
            Destroy(boxCollider);

            if (toBeSpawned != null)
            {
                Vector3 pos = gameObject.transform.position;
                pos.y += yPos;
                Instantiate(toBeSpawned, pos, gameObject.transform.rotation);
            }
        }
    }

}
