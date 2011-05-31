//	Copyright © 2010-2011, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using System.Collections.Generic;
using System.Linq;

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
		public static string GetVersionString(this System.Reflection.Assembly assembly)
		{
			return assembly.FullName.Split (',')[1].Split ('=')[1];
		}

		/// <summary>
		/// Gets the file path of the code base. If the <see cref="System.Assembly.CodeBase"/>
		/// property is not of the form <c>file:///</c>, then this method returns <c>null</c>.
		/// </summary>
		/// <param name="assembly">The assembly.</param>
		/// <returns>The file path or <c>null</c>.</returns>
		public static string GetCodeBaseFilePath(this System.Reflection.Assembly assembly)
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
	}
}
