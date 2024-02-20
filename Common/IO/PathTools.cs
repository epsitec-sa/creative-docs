using System.IO;

namespace Epsitec.Common.IO
{
    /// <summary>
    /// Contains utilities to work with paths.
    /// </summary>
    public class PathTools
    {
        /// <summary>
        /// Remove the end of a path until the first occurence of a target directory name
        /// </summary>
        /// <param name="targetDirName">the name of the directory to look for</param>
        /// <param name="path">the path to search on</param>
        /// <returns>the first part of <paramref name="path"> until <paramref name="targetDirName"> (included)
        /// <c>null</c> if targetDirName is not found</returns>
        public static string RemoveUntilDir(string targetDirName, string path)
        {
            string outputPath = path;
            for (int i = 0; i < 100; i++)
            {
                try
                {
                    outputPath = Path.GetDirectoryName(outputPath);
                }
                catch (System.ArgumentException)
                {
                    outputPath = null;
                    break;
                }
                string directoryName = Path.GetFileName(outputPath);
                if (directoryName == targetDirName)
                {
                    break;
                }
            }
            return outputPath;
        }
    }
}
