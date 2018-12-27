using System.Collections.Generic;
using UnityEngine;

namespace UnityCommon
{
    public enum StartupPoolMode
    {
        Awake,
        Start,
        CallManually
    };

    [System.Serializable]
    public class StartupPool
    {
        public int Size;
        public GameObject Prefab;
    }

    public class ObjectPool : MonoBehaviour
    {
        public StartupPoolMode StartupPoolMode { get { return startupPoolMode; } private set { startupPoolMode = value; } }
        public StartupPool[] StartupPools { get { return startupPools; } private set { startupPools = value; } }

        private Dictionary<GameObject, List<GameObject>> pooledObjects = new Dictionary<GameObject, List<GameObject>>();
        private Dictionary<GameObject, GameObject> spawnedObjects = new Dictionary<GameObject, GameObject>();
        private List<GameObject> tempList = new List<GameObject>();
        private bool startupPoolsCreated;

        [SerializeField] private StartupPoolMode startupPoolMode;
        [SerializeField] private StartupPool[] startupPools;

        private void Awake ()
        {
            if (StartupPoolMode == StartupPoolMode.Awake)
                CreateStartupPools();
        }

        private void Start ()
        {
            if (StartupPoolMode == StartupPoolMode.Start)
                CreateStartupPools();
        }

        public void CreateStartupPools ()
        {
            if (!startupPoolsCreated)
            {
                startupPoolsCreated = true;
                var pools = StartupPools;
                if (pools != null && pools.Length > 0)
                    for (int i = 0; i < pools.Length; ++i)
                        CreatePool(pools[i].Prefab, pools[i].Size);
            }
        }

        public bool IsSpawned (GameObject obj)
        {
            return spawnedObjects.ContainsKey(obj);
        }

        #region Pool creation

        public void CreatePool<T> (T prefab, int initialPoolSize) where T : Component
        {
            CreatePool(prefab.gameObject, initialPoolSize);
        }

        public void CreatePool (GameObject prefab, int initialPoolSize)
        {
            if (prefab != null && !pooledObjects.ContainsKey(prefab))
            {
                var list = new List<GameObject>();
                pooledObjects.Add(prefab, list);

                if (initialPoolSize > 0)
                {
                    bool active = prefab.activeSelf;
                    prefab.SetActive(false);
                    Transform parent = transform;
                    while (list.Count < initialPoolSize)
                    {
                        var obj = (GameObject)Object.Instantiate(prefab);
                        obj.transform.parent = parent;
                        list.Add(obj);
                    }
                    prefab.SetActive(active);
                }
            }
        }

        #endregion

        #region Spawning

        public T Spawn<T> (T prefab, Transform parent, Vector3 position, Quaternion rotation) where T : Component
        {
            return Spawn(prefab.gameObject, parent, position, rotation).GetComponent<T>();
        }

        public T Spawn<T> (T prefab, Vector3 position, Quaternion rotation) where T : Component
        {
            return Spawn(prefab.gameObject, null, position, rotation).GetComponent<T>();
        }

        public T Spawn<T> (T prefab, Transform parent, Vector3 position) where T : Component
        {
            return Spawn(prefab.gameObject, parent, position, Quaternion.identity).GetComponent<T>();
        }

        public T Spawn<T> (T prefab, Vector3 position) where T : Component
        {
            return Spawn(prefab.gameObject, null, position, Quaternion.identity).GetComponent<T>();
        }

        public T Spawn<T> (T prefab, Transform parent) where T : Component
        {
            return Spawn(prefab.gameObject, parent, Vector3.zero, Quaternion.identity).GetComponent<T>();
        }

        public T Spawn<T> (T prefab) where T : Component
        {
            return Spawn(prefab.gameObject, null, Vector3.zero, Quaternion.identity).GetComponent<T>();
        }

        public GameObject Spawn (GameObject prefab, Transform parent, Vector3 position, Quaternion rotation)
        {
            List<GameObject> list;
            Transform trans;
            GameObject obj = null;
            CreatePool(prefab.gameObject, 0);
            if (pooledObjects.TryGetValue(prefab, out list))
            {
                obj = null;
                if (list.Count > 0)
                {
                    while (obj == null && list.Count > 0)
                    {
                        obj = list[0];
                        list.RemoveAt(0);
                    }
                    if (obj != null)
                    {
                        trans = obj.transform;
                        if (parent != null)
                        {
                            bool worldPositionStays = (parent.GetComponent<RectTransform>() == null);
                            trans.SetParent(parent, worldPositionStays);
                        }
                        trans.position = position;
                        trans.rotation = rotation;
                        obj.SetActive(true);
                        spawnedObjects.Add(obj, prefab);
                        return obj;
                    }
                }
                obj = (GameObject)Object.Instantiate(prefab);
                trans = obj.transform;
                if (parent != null)
                {
                    bool worldPositionStays = (parent.GetComponent<RectTransform>() == null);
                    trans.SetParent(parent, worldPositionStays);
                }
                trans.position = position;
                trans.rotation = rotation;
                spawnedObjects.Add(obj, prefab);
            }
            return obj;
        }

        public GameObject Spawn (GameObject prefab, Transform parent, Vector3 position)
        {
            return Spawn(prefab, parent, position, Quaternion.identity);
        }

        public GameObject Spawn (GameObject prefab, Vector3 position, Quaternion rotation)
        {
            return Spawn(prefab, null, position, rotation);
        }

        public GameObject Spawn (GameObject prefab, Transform parent)
        {
            return Spawn(prefab, parent, Vector3.zero, Quaternion.identity);
        }

        public GameObject Spawn (GameObject prefab, Vector3 position)
        {
            return Spawn(prefab, null, position, Quaternion.identity);
        }

        public GameObject Spawn (GameObject prefab)
        {
            return Spawn(prefab, null, Vector3.zero, Quaternion.identity);
        }

        #endregion

        #region Recycling

        public void Recycle<T> (T obj) where T : Component
        {
            Recycle(obj.gameObject);
        }

        public void Recycle (GameObject obj)
        {
            GameObject prefab;
            if (spawnedObjects.TryGetValue(obj, out prefab))
            {
                Recycle(obj, prefab);
            }
            else
            {
                Debug.LogWarning(obj.name + "can not be recycled because it was never pooled. It will be destroyed instead.");
                Object.Destroy(obj);
            }
        }

        private void Recycle (GameObject obj, GameObject prefab)
        {
            pooledObjects[prefab].Add(obj);
            spawnedObjects.Remove(obj);
            obj.transform.SetParent(transform, false);
            obj.SetActive(false);
        }

        public void RecycleAll<T> (T prefab) where T : Component
        {
            RecycleAll(prefab.gameObject);
        }

        public void RecycleAll (GameObject prefab)
        {
            foreach (var item in spawnedObjects)
                if (item.Value == prefab)
                    tempList.Add(item.Key);
            for (int i = 0; i < tempList.Count; ++i)
                Recycle(tempList[i]);
            tempList.Clear();
        }

        public void RecycleAll ()
        {
            tempList.AddRange(spawnedObjects.Keys);
            for (int i = 0; i < tempList.Count; ++i)
                Recycle(tempList[i]);
            tempList.Clear();
        }

        #endregion

        #region Counters

        public int CountPooled<T> (T prefab) where T : Component
        {
            return CountPooled(prefab.gameObject);
        }

        public int CountPooled (GameObject prefab)
        {
            List<GameObject> list;
            if (pooledObjects.TryGetValue(prefab, out list))
                return list.Count;
            return 0;
        }

        public int CountSpawned<T> (T prefab) where T : Component
        {
            return CountSpawned(prefab.gameObject);
        }

        public int CountSpawned (GameObject prefab)
        {
            int count = 0;
            foreach (var instancePrefab in spawnedObjects.Values)
                if (prefab == instancePrefab)
                    ++count;
            return count;
        }

        public int CountAllPooled ()
        {
            int count = 0;
            foreach (var list in pooledObjects.Values)
                count += list.Count;
            return count;
        }

        [ContextMenu("Log All Pooled")]
        public void PringAllPooled ()
        {
            Debug.Log(string.Format("ObjectPool: {0} object pooled", CountAllPooled()));
        }

        #endregion

        #region Getters

        public List<GameObject> GetPooled (GameObject prefab, List<GameObject> list, bool appendList)
        {
            if (list == null)
                list = new List<GameObject>();
            if (!appendList)
                list.Clear();
            List<GameObject> pooled;
            if (pooledObjects.TryGetValue(prefab, out pooled))
                list.AddRange(pooled);
            return list;
        }
        public List<T> GetPooled<T> (T prefab, List<T> list, bool appendList) where T : Component
        {
            if (list == null)
                list = new List<T>();
            if (!appendList)
                list.Clear();
            List<GameObject> pooled;
            if (pooledObjects.TryGetValue(prefab.gameObject, out pooled))
                for (int i = 0; i < pooled.Count; ++i)
                    list.Add(pooled[i].GetComponent<T>());
            return list;
        }

        public List<GameObject> GetSpawned (GameObject prefab, List<GameObject> list, bool appendList)
        {
            if (list == null)
                list = new List<GameObject>();
            if (!appendList)
                list.Clear();
            foreach (var item in spawnedObjects)
                if (item.Value == prefab)
                    list.Add(item.Key);
            return list;
        }
        public List<T> GetSpawned<T> (T prefab, List<T> list, bool appendList) where T : Component
        {
            if (list == null)
                list = new List<T>();
            if (!appendList)
                list.Clear();
            var prefabObj = prefab.gameObject;
            foreach (var item in spawnedObjects)
                if (item.Value == prefabObj)
                    list.Add(item.Key.GetComponent<T>());
            return list;
        }

        #endregion

        #region Destroying

        public void DestroyPooled (GameObject prefab)
        {
            List<GameObject> pooled;
            if (pooledObjects.TryGetValue(prefab, out pooled))
            {
                for (int i = 0; i < pooled.Count; ++i)
                    GameObject.Destroy(pooled[i]);
                pooled.Clear();
            }
        }

        public void DestroyPooled<T> (T prefab) where T : Component
        {
            DestroyPooled(prefab.gameObject);
        }

        public void DestroyAll (GameObject prefab)
        {
            RecycleAll(prefab);
            DestroyPooled(prefab);
        }

        public void DestroyAll<T> (T prefab) where T : Component
        {
            DestroyAll(prefab.gameObject);
        }

        #endregion
    }
}
