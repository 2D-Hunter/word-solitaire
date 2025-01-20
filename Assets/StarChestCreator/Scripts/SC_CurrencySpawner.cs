using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace StarChestCreator     
{
    public class SC_CurrencySpawner : MonoBehaviour
    {
        public static SC_CurrencySpawner instance;
        public static event Action OnAllCurrenciesMoved;
        public Camera UICamera;
        public bool directToTarget = false;
        public float curveStrength = 0.2f; // Bezier curve control point strength

        private void Awake()
        {
            if (instance == null)
            {
                instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }

        public enum ScatterMode
        {
            Around,
            Upwards
        }

        [System.Serializable]
        public class Currency
        {
            public string name;
            public RectTransform startPoint;
            public RectTransform endPoint;
            public GameObject currencyPrefab;
            public GameObject shadowPrefab;
            public GameObject currencyContainer;
            public GameObject shadowContainer;
            public int poolSizeOfCurrencies;
            public float delayBetweenSpawningCurrencies;
            public float scatterDistance;
            public float scatterDuration;
            public float moveDuration;
            public float delayBetweenMovingToTarget;
            public string particleTag;
            public ScatterMode scatterMode;

            [HideInInspector]
            public List<GameObject> currencyPool = new List<GameObject>();
            [HideInInspector]
            public Queue<GameObject> availableCurrencies = new Queue<GameObject>();
            [HideInInspector]
            public List<GameObject> shadowPool = new List<GameObject>();
            [HideInInspector]
            public Queue<GameObject> availableShadows = new Queue<GameObject>();
        }

        public List<Currency> currencies = new List<Currency>();

        private void Start()
        {
            if (currencies == null || currencies.Count == 0)
            {
                Debug.LogError("No currencies available for pooling.");
                return;
            }

            // Initialize pools
            foreach (var currency in currencies)
            {
                InitializePool(currency.currencyPool, currency.availableCurrencies, currency.currencyPrefab, currency.currencyContainer, Vector3.zero, currency.poolSizeOfCurrencies);
                InitializePool(currency.shadowPool, currency.availableShadows, currency.shadowPrefab, currency.shadowContainer, Vector3.one, currency.poolSizeOfCurrencies);
            }
        }


        private void InitializePool(List<GameObject> pool, Queue<GameObject> available, GameObject prefab, GameObject container, Vector3 initialScale, int count)
        {
            for (int i = 0; i < count; i++)
            {
                GameObject obj = Instantiate(prefab, container.transform);
                RectTransform rect = obj.GetComponent<RectTransform>();
                rect.localScale = initialScale;
                obj.AddComponent<CanvasGroup>().alpha = 0;
                obj.SetActive(false);
                pool.Add(obj);
                available.Enqueue(obj);
            }
        }

        public void StartCurrencySpawn(string currencyName, int amount)
        {
            Currency targetCurrency = currencies.Find(currency => currency.name == currencyName);

            if (targetCurrency == null)
            {
                Debug.LogError($"Currency '{currencyName}' not found.");
                return;
            }

            Debug.Log($"Spawning {amount} of currency '{currencyName}'.");

            targetCurrency.poolSizeOfCurrencies = amount;
            StartCoroutine(SpawnCurrency(targetCurrency));
        }


        private IEnumerator SpawnCurrency(Currency currency)
        {
            List<Coroutine> routines = new List<Coroutine>();

            for (int i = 0; i < currency.poolSizeOfCurrencies; i++)
            {
                if (currency.availableCurrencies.Count == 0 || currency.availableShadows.Count == 0)
                    yield break;

                GameObject coin = currency.availableCurrencies.Dequeue();
                GameObject shadow = currency.availableShadows.Dequeue();
                ActivateObject(coin, shadow, currency);

                if (!directToTarget)
                {
                    routines.Add(StartCoroutine(ScatterCurrency(coin.GetComponent<RectTransform>(), shadow.GetComponent<RectTransform>(), currency)));
                }
                else
                {
                    InitializeCurrency(coin.GetComponent<RectTransform>(), shadow.GetComponent<RectTransform>(), currency, true);
                    StartCoroutine(MoveCurrencyToTarget(coin.GetComponent<RectTransform>(), shadow.GetComponent<RectTransform>(), currency, true));
                }

                yield return new WaitForSeconds(UnityEngine.Random.Range(0f, currency.delayBetweenSpawningCurrencies));
            }

            foreach (var routine in routines)
            {
                yield return routine;
            }
        }

        private void ActivateObject(GameObject coin, GameObject shadow, Currency currency)
        {
            coin.SetActive(true);
            shadow.SetActive(true);
        }

        private void InitializeCurrency(RectTransform coinRect, RectTransform shadowRect, Currency currency, bool randomizePosition)
        {
            Vector2 startPoint = currency.startPoint.anchoredPosition;

            if (randomizePosition)
            {
                startPoint += UnityEngine.Random.insideUnitCircle * currency.scatterDistance;
            }

            coinRect.anchoredPosition = startPoint;
            shadowRect.anchoredPosition = startPoint + new Vector2(30, -90);
            coinRect.localScale = Vector3.one;
            shadowRect.localScale = Vector3.one * 0.85f;
        }

        IEnumerator ScatterCurrency(RectTransform currencyRect, RectTransform shadowRect, Currency data)
        {
            float time = 0;

            currencyRect.GetComponent<CanvasGroup>().alpha = 1;
            shadowRect.GetComponent<CanvasGroup>().alpha = 1;

            Vector2 currencyScatterTarget = CalculateScatterTarget(data, out Vector2 shadowScatterTarget);
            while (time < data.scatterDuration)
            {
                time += Time.deltaTime;
                float t = time / data.scatterDuration;
                SetPositionAndScale(currencyRect, data.startPoint.anchoredPosition, currencyScatterTarget, t, EaseOutBack(t));
                SetPositionAndScale(shadowRect, data.startPoint.anchoredPosition, shadowScatterTarget, t, Mathf.Lerp(0f, 0.85f, t));
                yield return null;
            }

            currencyRect.anchoredPosition = currencyScatterTarget;
            shadowRect.anchoredPosition = shadowScatterTarget;

            StartCoroutine(MoveCurrencyToTargetWithDelay(currencyRect, shadowRect, data, data.delayBetweenMovingToTarget));
        }

        private Vector2 CalculateScatterTarget(Currency data, out Vector2 shadowScatterTarget)
        {
            Vector2 scatterDirection = GetScatterDirection(data);
            float minScatterDistance = 50f;
            float ringScatterDistance = data.scatterDistance * (UnityEngine.Random.Range(0, data.poolSizeOfCurrencies) + 1) / (data.poolSizeOfCurrencies + 1) + minScatterDistance;
            Vector2 currencyScatterTarget = data.startPoint.anchoredPosition + scatterDirection * ringScatterDistance;
            shadowScatterTarget = currencyScatterTarget + new Vector2(30, -90);
            return currencyScatterTarget;
        }

        private Vector2 GetScatterDirection(Currency data)
        {
            if (data.scatterMode == ScatterMode.Around)
            {
                float angle = UnityEngine.Random.Range(0, 360) * Mathf.Deg2Rad;
                return new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));
            }
            else if (data.scatterMode == ScatterMode.Upwards)
            {
                float angle = UnityEngine.Random.Range(-45, 45) * Mathf.Deg2Rad;
                return new Vector2(Mathf.Sin(angle), Mathf.Cos(angle));
            }
            return Vector2.zero;
        }

        private void SetPositionAndScale(RectTransform transform, Vector2 initialPosition, Vector2 targetPosition, float t, float scale)
        {
            transform.anchoredPosition = Vector2.Lerp(initialPosition, targetPosition, t);
            transform.localScale = new Vector3(scale, scale, 1);
        }

        IEnumerator MoveCurrencyToTargetWithDelay(RectTransform currency, RectTransform shadow, Currency data, float delay)
        {
            yield return new WaitForSeconds(delay);
            yield return MoveCurrencyToTarget(currency, shadow, data, false);
        }

        IEnumerator MoveCurrencyToTarget(RectTransform currency, RectTransform shadow, Currency data, bool directToTarget)
        {
            float time = 0;
            Vector2 initialCurrencyPosition = directToTarget ? data.startPoint.anchoredPosition : currency.anchoredPosition;
            Vector2 initialShadowPosition = directToTarget ? data.startPoint.anchoredPosition : shadow.anchoredPosition;

            // Directly set alpha
            currency.GetComponent<CanvasGroup>().alpha = 1;
            shadow.GetComponent<CanvasGroup>().alpha = 1;

            Vector2 finalCurrencyPosition = GetFinalPosition(data, currency);
            Vector2 finalShadowPosition = finalCurrencyPosition;

            // Make the Bezier curve more pronounced based on direction
            Vector2 controlPointCurrency = GetDynamicControlPoint(initialCurrencyPosition, finalCurrencyPosition, curveStrength);
            Vector2 controlPointShadow = GetDynamicControlPoint(initialShadowPosition, finalShadowPosition, curveStrength);

            while (time < data.moveDuration)
            {
                time += Time.deltaTime;
                float t = time / data.moveDuration;
                currency.anchoredPosition = CalculateBezierPoint(t, initialCurrencyPosition, controlPointCurrency, finalCurrencyPosition);
                shadow.anchoredPosition = CalculateBezierPoint(t, initialShadowPosition, controlPointShadow, finalShadowPosition);
                yield return null;
            }

            currency.anchoredPosition = finalCurrencyPosition;
            shadow.anchoredPosition = finalShadowPosition;

            // Use the pooled particle system instead of instantiating new particles
            SC_CurrencyParticlePool.Instance.PlayParticle(data.particleTag, data.endPoint.position, Quaternion.identity);  // Ensure the particle plays with the correct rotation

            // Reset the currency and shadow objects
            ResetObject(currency, data.availableCurrencies);
            ResetObject(shadow, data.availableShadows);

            yield return new WaitForSeconds(0.3f);

            // After all currencies have moved, invoke the event
            OnAllCurrenciesMoved?.Invoke();
        }


        private Vector2 GetFinalPosition(Currency data, RectTransform currency)
        {
            Vector3 screenEndPoint = RectTransformUtility.WorldToScreenPoint(UICamera, data.endPoint.position);
            RectTransformUtility.ScreenPointToLocalPointInRectangle(currency.parent as RectTransform, screenEndPoint, UICamera, out Vector2 finalPosition);
            return finalPosition;
        }

        private Vector2 GetDynamicControlPoint(Vector2 initialPosition, Vector2 finalPosition, float strength)
        {
            // Generate a more pronounced control point based on the direction of the movement
            Vector2 direction = (finalPosition - initialPosition).normalized;
            Vector2 perpendicular = new Vector2(-direction.y, direction.x);
            float distance = Vector2.Distance(initialPosition, finalPosition);
            return initialPosition + (finalPosition - initialPosition) / 2 + perpendicular * distance * strength;
        }

        private Vector2 CalculateBezierPoint(float t, Vector2 p0, Vector2 p1, Vector2 p2)
        {
            float u = 1 - t;
            float tt = t * t;
            float uu = u * u;
            return uu * p0 + 2 * u * t * p1 + tt * p2;
        }

        private void ResetObject(RectTransform obj, Queue<GameObject> queue)
        {
            obj.anchoredPosition = instance.currencies[0].startPoint.anchoredPosition;
            obj.GetComponent<CanvasGroup>().alpha = 0;
            obj.gameObject.SetActive(false);
            queue.Enqueue(obj.gameObject);  
        }

        private float EaseOutBack(float x)
        {
            float c1 = 1.70158f;
            float c3 = c1 + 1;
            return 1 + c3 * Mathf.Pow(x - 1, 3) + c1 * Mathf.Pow(x - 1, 2);
        }
    }
}
