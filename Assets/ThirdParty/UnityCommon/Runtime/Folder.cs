using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace UnityCommon
{
    /// <summary>
    /// Represents a directory in the project assets.
    /// </summary>
    [System.Serializable]
    public class Folder
    {
        public string Path { get => path; private set => path = value; }
        public string Name => Path.Contains("/") ? Path.GetAfter("/") : Path;

        [SerializeField] string path = null;

        public Folder (string path)
        {
            this.path = path;
        }
    }
}
