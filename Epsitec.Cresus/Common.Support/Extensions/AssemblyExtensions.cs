//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Common.Support.Extensions
{
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
	}
}
