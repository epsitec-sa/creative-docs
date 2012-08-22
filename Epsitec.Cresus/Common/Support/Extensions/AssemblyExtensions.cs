//	Copyright © 2010-2012, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Types;

using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

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
			return assembly.FullName.Split (',')[1].Split ('=')[1];
		}

		/// <summary>
		/// Gets the file path of the code base. If the <see cref="System.Assembly.CodeBase"/>
		/// property is not of the form <c>file:///</c>, then this method returns <c>null</c>.
		/// </summary>
		/// <param name="assembly">The assembly.</param>
		/// <returns>The file path or <c>null</c>.</returns>
		public static string GetCodeBaseFilePath(this Assembly assembly)
		{
			string codeBase = assembly.CodeBase;

			if (codeBase.StartsWith ("file:///"))
			{
				codeBase = codeBase.Substring (8);
				codeBase = codeBase.Replace ('/', System.IO.Path.DirectorySeparatorChar);

				return codeBase;
			}
			else
			{
				return null;
			}
		}

		public static string GetResourceText(this Assembly assembly, string resourceName)
		{
			assembly.ThrowIfNull ("assembly");
			resourceName.ThrowIfNullOrEmpty ("resourceName");

			using (var stream = assembly.GetManifestResourceStream (resourceName))
			{
				using (var streamReader = new StreamReader (stream))
				{
					return streamReader.ReadToEnd ();
				}
			}
		}
		
		public static IEnumerable<T> GetCustomAttributes<T>(this Assembly assembly, bool inherit = false)
			where T : System.Attribute
		{
			return assembly.GetCustomAttributes (typeof (T), inherit).Cast<T> ();
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
			var attribute = assembly.GetCustomAttributes<NamespaceAttribute> ().FirstOrDefault ();

			if (attribute == null)
			{
				return null;
			}

			return attribute.AssemblyNamespace;
		}
	}
}
