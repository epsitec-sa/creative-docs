//	Copyright © 2004-2011, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Types;

using System.Collections.Generic;
using System.Reflection;

namespace Epsitec.Common.Support
{
	/// <summary>
	/// La classe AssemblyLoader permet de charger une "assembly" d'après son nom.
	/// </summary>
	public static class AssemblyLoader
	{
		/// <summary>
		/// Loads the specified assembly. The assembly is searched in the same directory
		/// as the Common.Support assembly.
		/// </summary>
		/// <param name="name">The assembly name (without the file path nor the file extension).</param>
		/// <param name="subfolder">The optional sub-folder.</param>
		/// <returns>
		/// The assembly if it could be found; otherwise, <c>null</c>.
		/// </returns>
		public static Assembly Load(string name, string subfolder = null)
		{
			string loadPath = AssemblyLoader.GetAssemblyLoadPath ();

			if (subfolder != null)
            {
				loadPath = System.IO.Path.Combine (loadPath, subfolder);
            }

			return AssemblyLoader.LoadFromPath (name, loadPath);
		}


		/// <summary>
		/// Loads the specified assembly from a specified path. This will probe both for the
		/// <c>dll</c> and <c>exe</c> files.
		/// </summary>
		/// <param name="name">The assembly name (without file extension).</param>
		/// <param name="loadPath">The assembly load path.</param>
		/// <param name="loadDependencies">If set to <c>true</c>, loads also all direct dependencies.</param>
		/// <returns>
		/// The assembly if it could be found; otherwise, <c>null</c>.
		/// </returns>
		public static Assembly LoadFromPath(string name, string loadPath, bool loadDependencies = false)
		{
			if (System.IO.Directory.Exists (loadPath))
			{
				foreach (string ext in new string[] { ".dll", ".exe" })
				{
					string fullName = System.IO.Path.Combine (loadPath, name + ext);

					if (System.IO.File.Exists (fullName))
					{
						try
						{
							var assembly = Assembly.LoadFrom (fullName);

							if (assembly != null)
							{
								if (loadDependencies)
								{
									//	Force the dependencies to be resolved...

									assembly.GetTypes ();
								}

								return assembly;
							}
						}
						catch (System.Exception ex)
						{
							System.Diagnostics.Debug.WriteLine ("Could not load assembly from file " + fullName + " : " + ex.Message);
						}
					}
				}
			}

			return null;
		}

		/// <summary>
		/// Loads all assemblies which match the specified pattern.
		/// </summary>
		/// <param name="pattern">The assembly name pattern.</param>
		/// <param name="searchOption">The search option.</param>
		/// <param name="loadMode">The load mode.</param>
		/// <param name="subfolder">The optional sub-folder.</param>
		/// <returns>
		/// The assemblies which could be found.
		/// </returns>
		public static IList<Assembly> LoadMatching(string pattern, System.IO.SearchOption searchOption, AssemblyLoadMode loadMode = AssemblyLoadMode.LoadOnlyEpsitecSigned, string subfolder = null)
		{
			string loadPath = AssemblyLoader.GetAssemblyLoadPath ();

			if (subfolder != null)
			{
				loadPath = System.IO.Path.Combine (loadPath, subfolder);
			}

			var assemblies  = new List<Assembly> ();

			if (System.IO.Directory.Exists (loadPath))
			{
				foreach (string ext in new string[] { ".dll", ".exe" })
				{
					string filePattern = string.Concat (pattern, ext);

					foreach (string path in System.IO.Directory.GetFiles (loadPath, filePattern, searchOption))
					{
						if (AssemblyLoader.CheckAssemblyName (path, loadMode))
						{
							assemblies.Add (Assembly.LoadFrom (path));
						}
					}
				}
			}			
			
			return assemblies;
		}


		/// <summary>
		/// Gets the assembly load path based on the Common.Support assembly path.
		/// </summary>
		/// <returns>The assembly load path.</returns>
		private static string GetAssemblyLoadPath()
		{
			string loadPath = Globals.Directories.Executable;

			string loadName = AssemblyLoader.CurrentAssembly.CodeBase;
			string filePrefix = "file:///";

			if (loadName.StartsWith (filePrefix))
			{
				loadName = loadName.Substring (8);
				loadName = loadName.Replace ('/', System.IO.Path.DirectorySeparatorChar);
				loadName = System.IO.Path.GetDirectoryName (loadName);

				if (loadPath != loadName)
				{
					if (AssemblyLoader.loadPathDifferences++ == 0)
					{
						System.Diagnostics.Debug.WriteLine (string.Format ("Assembly load path different from executable path\n" +
							/* */										   "  DLL load path: '{0}'\n" +
							/* */										   "  EXE load path: '{1}'", loadName, loadPath));
					}

					loadPath = loadName;
				}
			}
			else
			{
				throw new System.Exception (string.Format ("Support assembly not loaded from a file: CodeBase = '{0}'", loadName));
			}

			return loadPath;
		}


		/// <summary>
		/// Checks the assembly name to see if it is compatible with the load mode.
		/// </summary>
		/// <param name="path">The path to the assembly DLL.</param>
		/// <param name="loadMode">The load mode.</param>
		/// <returns><c>true</c> if the assembly is compatible; otherwise, <c>false</c>.</returns>
		private static bool CheckAssemblyName(string path, AssemblyLoadMode loadMode)
		{
			AssemblyName assemblyName = AssemblyName.GetAssemblyName (path);
			byte[] publicKey = assemblyName.GetPublicKey ();

			switch (loadMode)
			{
				case AssemblyLoadMode.LoadAny:
					return true;

				case AssemblyLoadMode.LoadOnlySigned:
					return publicKey != null;

				case AssemblyLoadMode.LoadOnlyEpsitecSigned:
					return Comparer.EqualValues (AssemblyLoader.EpsitecPublicKey, publicKey);

				default:
					throw new System.NotSupportedException (string.Format ("AssemblyLoadMode.{0} not supported", loadMode));
			}
		}
		
		
		private static int						loadPathDifferences;
		private static Assembly					CurrentAssembly  = typeof (AssemblyLoader).Assembly;
		private static readonly byte[]			EpsitecPublicKey = AssemblyLoader.CurrentAssembly.GetName ().GetPublicKey ();
	}
}