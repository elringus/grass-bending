using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace UnityCommon
{
    /// <summary>
    /// The object of class will be auto-registered in <see cref="Context"/> on scene load (before Awake calls).
    /// Only works for <see cref="MonoBehaviour"/> objects already placed on scene.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class RegisterInContext : Attribute
    {
        public readonly bool AssertSingleInstance;

        public RegisterInContext (bool assertSingleInstance = false)
        {
            AssertSingleInstance = assertSingleInstance;
        }
    }

    /// <summary>
    /// When resolving in context, if not registered, an instance of class object will be constructed and registered. 
    /// Only works for classes with a default contructor and classes derived from <see cref="Component"/>. 
    /// When used with <see cref="Component"/>, component will be added to a new <see cref="GameObject"/> instantiated on the active scene.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class ConstructOnContextResolve : Attribute
    {
        public readonly HideFlags HideFlags;
        public readonly bool DontDestroyOnLoad;

        public ConstructOnContextResolve (HideFlags hideFlags = HideFlags.None, bool dontDestroyOnLoad = false)
        {
            HideFlags = hideFlags;
            DontDestroyOnLoad = dontDestroyOnLoad;
        }
    }

    /// <summary>
    /// Keeps weak references to registered objects.
    /// </summary>
    public class Context : MonoBehaviour
    {
        private const float gcInterval = 60;
        private static Context instance;
        private Dictionary<Type, List<WeakReference>> references;

        private void Start ()
        {
            StartCoroutine(RemoveDeadReferences());
        }

        private void OnDestroy ()
        {
            if (references != null)
                references.Clear();
            instance = null;
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void Initialize ()
        {
            var gameobject = new GameObject("Context");
            gameobject.hideFlags = HideFlags.HideAndDontSave;
            DontDestroyOnLoad(gameobject);
            instance = gameobject.AddComponent<Context>();
            instance.references = new Dictionary<Type, List<WeakReference>>();
        }

        // [BeforeSceneAwakeCalls]
        public static void RegisterSceneObjects ()
        {
            foreach (var monoBehaviour in FindObjectsOfType<MonoBehaviour>())
            {
                if (IsRegistered(monoBehaviour)) continue;

                var attribute = monoBehaviour.GetType().GetCustomAttributes(typeof(RegisterInContext), true).FirstOrDefault() as RegisterInContext;
                if (attribute == null) continue;

                Register(monoBehaviour, attribute.AssertSingleInstance);
            }
        }

        public static T Resolve<T> (Predicate<T> predicate = null, bool strictType = true, bool assertResult = false) where T : class
        {
            T result = null;
            var resolvingType = typeof(T);
            if (resolvingType.IsInterface || resolvingType.IsAbstract)
                strictType = false;

            var refsOfType = GetReferencesOfType(resolvingType, strictType);
            if (refsOfType != null && refsOfType.Count > 0)
            {
                var weakRef = refsOfType.FirstOrDefault(r => IsWeakRefValid(r) &&
                    (predicate == null || predicate(r.Target as T)));
                if (weakRef != null) result = weakRef.Target as T;
            }

            if (result == null && ShouldAutoConstruct(resolvingType))
                return ConstructAndRegister(resolvingType) as T;

            if (result == null && assertResult)
                Debug.LogError(string.Format("Failed to resolve object of type '{0}'", resolvingType.Name));

            return result;
        }

        public static List<T> ResolveAll<T> (Predicate<T> predicate = null, bool strictType = true, bool assertResult = false) where T : class
        {
            var resolvingType = typeof(T);
            if (resolvingType.IsInterface || resolvingType.IsAbstract)
                strictType = false;

            var refsOfType = GetReferencesOfType(resolvingType, strictType);
            if (refsOfType == null || refsOfType.Count == 0)
            {
                if (assertResult)
                    Debug.LogError(string.Format("Failed to resolve objects of type '{0}'", resolvingType.Name));
                return new List<T>();
            }

            return refsOfType
                .Where(r => IsWeakRefValid(r) && (predicate == null || predicate(r.Target as T)))
                .Select(r => r.Target)
                .Cast<T>()
                .ToList();
        }

        public static bool IsRegistered (object obj, bool strictType = true)
        {
            var refsOfType = GetReferencesOfType(obj.GetType(), strictType);
            if (refsOfType == null || refsOfType.Count == 0)
                return false;

            return refsOfType.Exists(r => IsWeakRefValid(r) && r.Target == obj);
        }

        public static void Register (object obj, bool assertSingleInstance = false)
        {
            if (obj == null)
            {
                Debug.LogWarning("Attempted to register a null object to Context.");
                return;
            }

            if (IsRegistered(obj))
            {
                Debug.LogWarning("Attempted to re-register the same object to Context.");
                return;
            }

            var objType = obj.GetType();
            var reference = new WeakReference(obj);

            if (instance.references.ContainsKey(objType))
            {
                Debug.Assert(!assertSingleInstance, string.Format("Attempted to register multiple objects with type '{0}' while asserting single instance.", objType.Name));
                instance.references[objType].Add(reference);
            }
            else instance.references.Add(objType, new List<WeakReference>() { reference });
        }

        [ContextMenu("Log Reference Count")]
        public void LogReferenceCount ()
        {
            var total = references.Values.Select(refList => refList.Count).Sum();
            var valid = references.Values.Select(refList => refList.Count(r => IsWeakRefValid(r))).Sum();
            Debug.Log(string.Format("Context: {0} total and {1} valid references", total, valid));
        }

        private static List<WeakReference> GetReferencesOfType (Type type, bool strictType = true)
        {
            if (strictType)
            {
                List<WeakReference> result;
                instance.references.TryGetValue(type, out result);
                return result;
            }
            else return instance.references
                    .Where(kv => type.IsAssignableFrom(kv.Key))
                    .SelectMany(kv => kv.Value)
                    .ToList();
        }

        private static object ConstructAndRegister (Type type, HideFlags hideFlags = HideFlags.None, bool dontDestroyOnLoad = false)
        {
            if (!type.IsSubclassOf(typeof(Component)))
            {
                var obj = Activator.CreateInstance(type);
                Register(obj);
                return obj;
            }

            if (type.IsDefined(typeof(ConstructOnContextResolve), true))
            {
                var attrs = (ConstructOnContextResolve[])type
                    .GetCustomAttributes(typeof(ConstructOnContextResolve), true);
                if (attrs.Length > 0)
                {
                    var attr = attrs[0];
                    hideFlags = attr.HideFlags;
                    dontDestroyOnLoad = attr.DontDestroyOnLoad;
                }
            }

            var containerObject = new GameObject(type.Name);
            containerObject.hideFlags = hideFlags;
            if (dontDestroyOnLoad) DontDestroyOnLoad(containerObject);
            var component = containerObject.AddComponent(type);
            Register(component);
            return component;
        }

        private static bool ShouldAutoConstruct (Type type)
        {
            return type.IsDefined(typeof(ConstructOnContextResolve), true) &&
                (type.GetConstructor(Type.EmptyTypes) != null || type.IsSubclassOf(typeof(Component)));
        }

        private static bool IsWeakRefValid (WeakReference weakRef)
        {
            if (weakRef == null || !weakRef.IsAlive) return false;

            var targetCopy = weakRef.Target; // To prevent race conditions.
            if (targetCopy == null) return false;

            // Check Unity objects internal (C++) state.
            if (targetCopy is UnityEngine.Object)
            {
                var unityObject = targetCopy as UnityEngine.Object;
                if (unityObject == null || !unityObject) return false;
            }

            return true;
        }

        private IEnumerator RemoveDeadReferences ()
        {
            var waitForGcInterval = new WaitForSeconds(gcInterval);

            while (true)
            {
                references.Values.ToList()
                    .ForEach(refList => refList.RemoveAll(r => !IsWeakRefValid(r)));

                yield return waitForGcInterval;
            }
        }
    }
}
