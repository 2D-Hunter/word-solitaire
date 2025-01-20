using System.Collections.Generic;
using UnityEngine;
using System.Collections;

namespace StarChestCreator
{
    public class SC_CurrencyParticlePool : MonoBehaviour
    {
        public static SC_CurrencyParticlePool Instance { get; private set; }

        [System.Serializable]
        public class Pool
        {
            public string tag;
            public GameObject prefab;
            public int size;
            public Transform parent; // Optional parent transform for organization
        }

        public List<Pool> pools;
        private Dictionary<string, Queue<GameObject>> poolDictionary;

        private void Awake()
        {
            // Singleton pattern to ensure only one instance exists
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Destroy(gameObject);
                return;
            }

            poolDictionary = new Dictionary<string, Queue<GameObject>>();

            // Initialize pools
            foreach (Pool pool in pools)
            {
                CreatePool(pool);
            }
        }

        private void CreatePool(Pool pool)
        {
            Queue<GameObject> objectPool = new Queue<GameObject>();

            for (int i = 0; i < pool.size; i++)
            {
                GameObject obj = Instantiate(pool.prefab, pool.parent);
                obj.SetActive(false);

                // Add PooledParticle component to store the pool tag
                SC_PooledParticle pooledParticle = obj.GetComponent<SC_PooledParticle>();
                if (pooledParticle == null)
                {
                    pooledParticle = obj.AddComponent<SC_PooledParticle>();
                }
                pooledParticle.poolTag = pool.tag;

                objectPool.Enqueue(obj);
            }

            poolDictionary.Add(pool.tag, objectPool);
        }

        public GameObject SpawnFromPool(string tag, Vector3 position, Quaternion rotation)
        {
            if (!poolDictionary.ContainsKey(tag))
            {
                Debug.LogWarning($"Pool with tag {tag} doesn't exist.");
                return null;
            }

            if (poolDictionary[tag].Count == 0)
            {
                ExtendPool(tag);
            }

            GameObject objectToSpawn = poolDictionary[tag].Dequeue();

            if (objectToSpawn.activeInHierarchy)
            {
                // If the object is still active, extend the pool and try again
                ExtendPool(tag);
                objectToSpawn = poolDictionary[tag].Dequeue();
            }

            objectToSpawn.SetActive(true);
            objectToSpawn.transform.position = position;
            objectToSpawn.transform.rotation = rotation;

            return objectToSpawn;
        }

        public void ReturnToPool(string tag, GameObject objectToReturn)
        {
            if (!poolDictionary.ContainsKey(tag))
            {
                Debug.LogWarning($"Pool with tag {tag} doesn't exist.");
                return;
            }

            // Reset scale
            objectToReturn.transform.localScale = Vector3.one;

            objectToReturn.SetActive(false);
            poolDictionary[tag].Enqueue(objectToReturn);
        }

        private void ExtendPool(string tag)
        {
            Pool pool = pools.Find(p => p.tag == tag);
            if (pool == null)
            {
                Debug.LogWarning($"No pool configuration found for tag {tag}.");
                return;
            }

            for (int i = 0; i < pool.size; i++)
            {
                GameObject obj = Instantiate(pool.prefab, pool.parent);
                obj.SetActive(false);

                // Add PooledParticle component to store the pool tag
                SC_PooledParticle pooledParticle = obj.GetComponent<SC_PooledParticle>();
                if (pooledParticle == null)
                {
                    pooledParticle = obj.AddComponent<SC_PooledParticle>();
                }
                pooledParticle.poolTag = pool.tag;

                poolDictionary[tag].Enqueue(obj);
            }

            Debug.Log($"Pool with tag {tag} has been extended by {pool.size} objects.");
        }

        // Method to play particle and return to pool automatically
        public void PlayParticle(string tag, Vector3 position, Quaternion rotation)
        {
            GameObject particleObj = SpawnFromPool(tag, position, rotation);
            if (particleObj == null)
                return;

            ParticleSystem particleSystem = particleObj.GetComponent<ParticleSystem>();
            if (particleSystem != null)
            {
                particleSystem.Clear();  // Ensure the particle system is cleared before playing
                particleSystem.Play();   // Play the particle effect

                // Return to pool after duration
                StartCoroutine(ReturnParticleToPoolAfterDuration(particleObj, particleSystem.main.duration));
            }
        }

        // Overload to play particle without specifying rotation
        public void PlayParticle(string tag, Vector3 position)
        {
            PlayParticle(tag, position, Quaternion.identity);
        }

        private IEnumerator ReturnParticleToPoolAfterDuration(GameObject particleObj, float duration)
        {
            yield return new WaitForSeconds(duration + 1f);

            // Reset scale
            particleObj.transform.localScale = Vector3.one;

            // Get the poolTag from the particleObj
            SC_PooledParticle pooledParticle = particleObj.GetComponent<SC_PooledParticle>();
            if (pooledParticle != null)
            {
                ReturnToPool(pooledParticle.poolTag, particleObj);
            }
            else
            {
                Debug.LogWarning("PooledParticle component not found on particleObj.");
                particleObj.SetActive(false);
            }
        }
    }
}