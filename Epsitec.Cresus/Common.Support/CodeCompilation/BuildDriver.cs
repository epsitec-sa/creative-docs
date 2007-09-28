//	Copyright © 2007, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support;
using Epsitec.Common.Support.CodeCompilation;
using Epsitec.Common.Support.CodeGeneration;

using System.Collections.Generic;

namespace Epsitec.Common.Support.CodeCompilation
{
	public class BuildDriver
	{
		public BuildDriver()
		{
		}

		public bool IsValidInstallation
		{
			get
			{
				if (!this.isValidInstallation.HasValue)
				{
					this.isValidInstallation = BuildDriver.VerifyBuildInstallation ();
				}

				return this.isValidInstallation.Value;
			}
		}


		/// <summary>
		/// Finds the reference assembly path.
		/// </summary>
		/// <param name="assemblyName">Name of the assembly.</param>
		/// <returns>The path to the reference assembly or <c>null</c> if it
		/// cannot be found.</returns>
		public static string FindReferenceAssemblyPath(string assemblyName)
		{
			if (assemblyName == "System")
			{
				assemblyName = "mscorlib";
			}
			
			string assemblyFileName = BuildDriver.GetAssemblyFileName (assemblyName);

			foreach (string path in BuildDriver.ReferenceAssembliesPaths)
			{
				string filePath = System.IO.Path.Combine (path, assemblyFileName);

				if (System.IO.File.Exists (filePath))
				{
					return filePath;
				}
			}

			return null;
		}

		private static bool VerifyBuildInstallation()
		{
			if ((System.IO.Directory.Exists (Paths.V20_Framework)) &&
				(System.IO.Directory.Exists (Paths.V30_Framework)) &&
				(System.IO.Directory.Exists (Paths.V35_Framework)) &&
				(System.IO.Directory.Exists (Paths.V30_ReferenceAssemblies)) &&
				(System.IO.Directory.Exists (Paths.V35_ReferenceAssemblies)))
			{
				return true;
			}
			else
			{
				return false;
			}
		}

		private static string GetAssemblyFileName(string assemblyName)
		{
			return string.Concat (assemblyName, ".dll");
		}

		private static IEnumerable<string> ReferenceAssembliesPaths
		{
			get
			{
				yield return Paths.V35_ReferenceAssemblies;
				yield return Paths.V30_ReferenceAssemblies;
				yield return Paths.V20_Framework;
			}
		}

		#region Paths Static Class

		internal static class Paths
		{
			static Paths()
			{
				Paths.v30_ReferenceAssemblies = System.IO.Path.Combine (Paths.ReferenceAssemblies, "v3.0");
				Paths.v35_ReferenceAssemblies = System.IO.Path.Combine (Paths.ReferenceAssemblies, "v3.5");
				Paths.v35_Framework = System.IO.Path.Combine (Paths.Framework, "v3.5");
				Paths.v30_Framework = System.IO.Path.Combine (Paths.Framework, "v3.0");
				Paths.v20_Framework = System.IO.Path.Combine (Paths.Framework, "v2.0.50727");
			}

			public static string V35_ReferenceAssemblies
			{
				get
				{
					return Paths.v35_ReferenceAssemblies;
				}
			}

			public static string V35_Framework
			{
				get
				{
					return Paths.v35_Framework;
				}
			}

			public static string V30_ReferenceAssemblies
			{
				get
				{
					return Paths.v30_ReferenceAssemblies;
				}
			}

			public static string V30_Framework
			{
				get
				{
					return Paths.v30_Framework;
				}
			}

			public static string V20_Framework
			{
				get
				{
					return Paths.v20_Framework;
				}
			}

			private static readonly string ReferenceAssemblies	= System.IO.Path.Combine (Globals.Directories.ProgramFiles, @"Reference Assemblies\Microsoft\Framework");
			private static readonly string Framework			= System.IO.Path.Combine (Globals.Directories.Windows, @"Microsoft.Net\Framework");

			private static readonly string v30_ReferenceAssemblies;
			private static readonly string v35_ReferenceAssemblies;
			private static readonly string v35_Framework;
			private static readonly string v30_Framework;
			private static readonly string v20_Framework;
		}

		#endregion

		private bool? isValidInstallation;
	}
}
