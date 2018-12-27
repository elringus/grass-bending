using System.Collections;
using System.IO;
using System.Text;
using UnityEditor;

namespace UnityCommon
{
    public class CountLinesOfCode : Editor
    {
        private readonly struct StatFile
        {
            public readonly string Name;
            public readonly int LinesCount;

            public StatFile (string name, int linesCount)
            {
                Name = name;
                LinesCount = linesCount;
            }
        }

        [MenuItem("Help/Count Lines")]
        private static void CountLines ()
        {
            var strDir = Directory.GetCurrentDirectory();
            strDir += @"/Assets";
            var iLengthOfRootPath = strDir.Length;
            var stats = new ArrayList();
            ProcessDirectory(stats, strDir);

            var iTotalNbLines = 0;
            foreach (StatFile f in stats)
                iTotalNbLines += f.LinesCount;

            var strStats = new StringBuilder();
            strStats.Append("Number of Files: " + stats.Count + "\n");
            strStats.Append("Number of Lines: " + iTotalNbLines + "\n");

            EditorUtility.DisplayDialog("Statistics", strStats.ToString(), "Ok");
        }

        private static void ProcessDirectory (ArrayList stats, string dir)
        {
            var strArrFiles = Directory.GetFiles(dir, "*.cs");
            foreach (string strFileName in strArrFiles)
                ProcessFile(stats, strFileName);

            strArrFiles = Directory.GetFiles(dir, "*.cginc");
            foreach (string strFileName in strArrFiles)
                ProcessFile(stats, strFileName);

            strArrFiles = Directory.GetFiles(dir, "*.shader");
            foreach (string strFileName in strArrFiles)
                ProcessFile(stats, strFileName);

            var strArrSubDir = Directory.GetDirectories(dir);
            foreach (string strSubDir in strArrSubDir)
                ProcessDirectory(stats, strSubDir);
        }

        private static void ProcessFile (ArrayList stats, string filename)
        {
            var reader = File.OpenText(filename);
            var iLineCount = 0;
            while (reader.Peek() >= 0)
            {
                reader.ReadLine();
                ++iLineCount;
            }
            stats.Add(new StatFile(filename, iLineCount));
            reader.Close();
        }
    }
}
