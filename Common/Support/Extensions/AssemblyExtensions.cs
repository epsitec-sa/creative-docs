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


using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Epsitec.Common.Types;

namespace Epsitec.Common.Support.Extensions
{
    /// <summary>
    /// The <c>AssemblyExtensions</c> class defines a set of extension methods for the
    /// <see cref="System.Reflection.Assembly"/> class.
    /// </summary>
    public static class AssemblyExtensions
    {
        /// <summary>
        /// Gets the version string for the assembly.
        /// </summary>
        /// <param name="assembly">The assembly.</param>
        /// <returns>The version string (such as <c>"2.6.0.1013"</c>).</returns>
        public static string GetVersionString(this Assembly assembly)
        {
            return assembly.FullName.Split(',')[1].Split('=')[1];
        }

        /// <summary>
        /// Gets the file path of the code base. If the <see cref="System.Assembly.CodeBase"/>
        /// property is not of the form <c>file:///</c>, then this method returns <c>null</c>.
        /// </summary>
        /// <param name="assembly">The assembly.</param>
        /// <returns>The file path or <c>null</c>.</returns>
        public static string GetCodeBaseFilePath(this Assembly assembly)
        {
            string codeBase = assembly.Location;

            if (codeBase.StartsWith("file:///"))
            {
                codeBase = codeBase.Substring(8);
                codeBase = codeBase.Replace('/', System.IO.Path.DirectorySeparatorChar);

                return codeBase;
            }
            else
            {
                return null;
            }
        }

        public static string GetResourceText(this Assembly assembly, string resourceName)
        {
            assembly.ThrowIfNull("assembly");
            resourceName.ThrowIfNullOrEmpty("resourceName");

            using (var stream = assembly.GetManifestResourceStream(resourceName))
            {
                using (var streamReader = new StreamReader(stream))
                {
                    return streamReader.ReadToEnd();
                }
            }
        }

        public static IEnumerable<T> GetCustomAttributes<T>(
            this Assembly assembly,
            bool inherit = false
        )
            where T : System.Attribute
        {
            return assembly.GetCustomAttributes(typeof(T), inherit).Cast<T>();
        }

        /// <summary>
        /// Gets the default namespace for the specified assembly. This information is only
        /// available if the assembly was decorated with the <see cref="NamespaceAttribute"/>
        /// at the <c>assembly:</c> level.
        /// </summary>
        /// <param name="assembly">The assembly.</param>
        /// <returns>The default namespace or <c>null</c>.</returns>
        public static string GetDefaultNamespace(this Assembly assembly)
        {
            var attribute = assembly.GetCustomAttributes<NamespaceAttribute>().FirstOrDefault();

            if (attribute == null)
            {
                return null;
            }

            return attribute.AssemblyNamespace;
        }
    }
}
