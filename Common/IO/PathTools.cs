/*
This file is part of CreativeDocs.

Copyright © 2003-2024, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland

CreativeDocs is free software: you can redistribute it and/or modify
it under the terms of the GNU General Public License as published by
the Free Software Foundation, either version 3 of the License, or
any later version.

CreativeDocs is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
GNU General Public License for more details.

You should have received a copy of the GNU General Public License
along with this program.  If not, see <https://www.gnu.org/licenses/>.
*/

﻿using System.IO;

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
