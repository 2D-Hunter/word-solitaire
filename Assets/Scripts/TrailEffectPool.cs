using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrailEffectPool : MonoBehaviour
{
    public static TrailEffectPool Instance;

    [SerializeField] private GameObject trailPrefab;
    [SerializeField] private int initialSize = 10;

    private Queue<GameObject> pool = new Queue<GameObject>();

    private void Awake()
    {
        Instance = this;
        for (int i = 0; i < initialSize; i++)
        {
            GameObject obj = Instantiate(trailPrefab);
            obj.SetActive(false);
            pool.Enqueue(obj);
        }
    }

    public GameObject GetTrailEffect(Vector3 position)
    {
        GameObject trail = pool.Count > 0 ? pool.Dequeue() : Instantiate(trailPrefab);
        trail.transform.position = position;
        trail.SetActive(true);
        return trail;
    }

    public void ReturnTrailEffect(GameObject trail)
    {
        trail.SetActive(false);
        pool.Enqueue(trail);
    }
}
