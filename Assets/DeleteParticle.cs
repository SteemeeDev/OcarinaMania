using UnityEngine;

public class DeleteParticle : MonoBehaviour
{
    [SerializeField] float spawnTime;
    float aliveTime = 0;

    private void Awake()
    {
        Debug.Log("GOOD HIT");
    }

    // Update is called once per frame
    void Update()
    {
        aliveTime += Time.deltaTime;
        if (aliveTime > spawnTime)
        {
            Destroy(gameObject);
        }
    }
}
