using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

using Object = UnityEngine.Object;

namespace UnityCommon
{
    public static class ObjectUtils
    {
        /// <summary>
        /// Wrapper over FindObjectsOfType to allow searching by any type and with predicate.
        /// Be aware this is slow and scales lineary with scene complexity.
        /// </summary>
        public static T FindObject<T> (Predicate<T> predicate = null) where T : class
        {
            return Object.FindObjectsOfType<Object>().FirstOrDefault(obj => obj is T &&
                (predicate == null || predicate(obj as T))) as T;
        }

        /// <summary>
        /// Wrapper over FindObjectsOfType to allow searching by any type and with predicate.
        /// Be aware this is slow and scales lineary with scene complexity.
        /// </summary>
        public static List<T> FindObjects<T> (Predicate<T> predicate = null) where T : class
        {
            return Object.FindObjectsOfType<Object>().Where(obj => obj is T &&
                (predicate == null || predicate(obj as T))).Cast<T>().ToList();
        }

        /// <summary>
        /// Asserts there is only one instance of the object instantiated on scene.
        /// </summary>
        public static void AssertSingleInstance (this Object unityObject)
        {
            var objectType = unityObject.GetType();
            Debug.Assert(Object.FindObjectsOfType(objectType).Length == 1,
               string.Format("More than one instance of {0} found on scene.", objectType.Name));
        }

        /// <summary>
        /// Asserts validity of all the required objects.
        /// </summary>
        /// <param name="unityObject"></param>
        /// <param name="requiredObjects">Objects to check for validity.</param>
        /// <returns>Whether all the required objects are valid.</returns>
        public static bool AssertRequiredObjects (this Object unityObject, params Object[] requiredObjects)
        {
            var assertFailed = false;
            for (int i = 0; i < requiredObjects.Length; ++i)
            {
                if (!requiredObjects[i])
                {
                    Debug.LogError(string.Format("Required object of type '{0}' is not valid for '{1}'", requiredObjects[i].GetType().Name, unityObject.name));
                    assertFailed = true;
                }
            }
            return !assertFailed;
        }
    }
}
