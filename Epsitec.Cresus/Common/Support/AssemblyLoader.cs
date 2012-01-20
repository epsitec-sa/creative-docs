//	Copyright © 2004-2012, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
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
		static AssemblyLoader()
		{
			System.AppDomain.CurrentDomain.AssemblyLoad += AssemblyLoader.HandleCurrentDomainAssemblyLoad;
		}


		/// <summary>
		/// Gets a value indicating whether the loader is currently loading an assembly.
		/// </summary>
		/// <value>
		/// 	<c>true</c> if an assembly is being loaded; otherwise, <c>false</c>.
		/// </value>
		public static bool						IsLoading
		{
			get
			{
				return AssemblyLoader.recursionCount.IsNotZero;
			}
		}


		/// <summary>
		/// Loads the specified assembly. The assembly is searched in the same directory
		/// as the Common.Support assembly.
		/// </summary>
		/// <param name="name">The assembly name (without the file path nor the file extension).</param>
		/// <param name="subfolder">The optional sub-folder.</param>
		/// <returns>
		/// The assembly if it could be found; otherwise, <c>null</c>.
		/// </returns>
		public static Assembly Load(string name, string subfolder = null, bool loadDependencies = false)
		{
			string loadPath = AssemblyLoader.GetAssemblyLoadPath ();

			if (string.IsNullOrEmpty (subfolder))
			{
				return AssemblyLoader.LoadFromPath (name, loadPath, loadDependencies);
			}
			else
			{
				loadPath = System.IO.Path.Combine (loadPath, subfolder);
				
				return AssemblyLoader.LoadFromPath (name, loadPath, loadDependencies);
			}
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
			lock (AssemblyLoader.exclusion)
			{
				Assembly assembly;

				if (AssemblyLoader.assemblies.TryGetValue (name, out assembly))
				{
					return assembly;
				}
			}

			using (AssemblyLoader.recursionCount.Enter ().AndFinally (AssemblyLoader.DispatchPendingEvents))
			{
				var assembly = Assembly.LoadWithPartialName (name);

				if (assembly != null)
				{
					lock (AssemblyLoader.exclusion)
					{
						AssemblyLoader.assemblies[name] = assembly;
					}

					if (loadDependencies)
					{
						AssemblyLoader.LoadDependencies (assembly);
					}

					return assembly;
				}
				
				if ((string.IsNullOrEmpty (loadPath) == false) &&
					(System.IO.Directory.Exists (loadPath)))
				{
					foreach (string ext in new string[] { ".dll", ".exe" })
					{
						string fullName = System.IO.Path.Combine (loadPath, name + ext);

						if (System.IO.File.Exists (fullName))
						{
							try
							{
								assembly = Assembly.LoadFrom (fullName);

								if (assembly != null)
								{
									lock (AssemblyLoader.exclusion)
									{
										AssemblyLoader.assemblies[name] = assembly;
									}

									if (loadDependencies)
									{
										AssemblyLoader.LoadDependencies (assembly, loadPath);
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
			}

			System.Diagnostics.Debug.WriteLine ("Failed to load assembly " + name);
			
			return null;
		}

		/// <summary>
		/// Loads the dependencies for the specified assembly.
		/// </summary>
		/// <param name="assembly">The assembly.</param>
		/// <param name="loadPath">The load path (which may be <c>null</c>).</param>
		private static void LoadDependencies(Assembly assembly, string loadPath = null)
		{
			var token1 = assembly.GetName ().GetPublicKeyToken ();
			var token2 = AssemblyLoader.CurrentAssembly.GetName ().GetPublicKeyToken ();

			foreach (var name in assembly.GetReferencedAssemblies ())
			{
				var token = name.GetPublicKeyToken ();

				//	Only load referenced assemblies which match either the assembly's public key,
				//	or the standard Epsitec public key; this avoids that we recursively load tons
				//	of assemblies from the .NET Framework.

				if ((Comparer.Equal (token, token1)) ||
					(Comparer.Equal (token, token2)))
				{
					if (AssemblyLoader.LoadFromPath (name.Name, loadPath, loadDependencies: true) == null)
					{
						System.Diagnostics.Debug.WriteLine (string.Format ("Failed to load assembly {0} referenced by {1}", name.FullName, assembly.FullName));
					}
				}
			}
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

		private static void HandleCurrentDomainAssemblyLoad(object sender, System.AssemblyLoadEventArgs args)
		{
			var handler = AssemblyLoader.AssemblyLoaded;

			if (handler != null)
			{
				if (AssemblyLoader.IsLoading)
				{
					lock (AssemblyLoader.exclusion)
					{
						AssemblyLoader.pendingEvents.Enqueue (() => handler (sender, args));
					}
				}
				else
				{
					handler (sender, args);
				}
			}
		}

		private static void DispatchPendingEvents()
		{
			while (AssemblyLoader.recursionCount.IsZero)
			{
				System.Action action = null;
				
				lock (AssemblyLoader.exclusion)
				{
					if (AssemblyLoader.pendingEvents.Count > 0)
					{
						action = AssemblyLoader.pendingEvents.Dequeue ();
					}
				}

				if (action == null)
				{
					break;
				}

				action ();
			}
		}


		/// <summary>
		/// Occurs when an assembly has been loaded. Prefer this event to the <see cref="System.AppDomain.CurrentDomain.AssemblyLoad"/>
		/// event, as it will fire only after all dependent assemblies were properly loaded too.
		/// </summary>
		public static event System.AssemblyLoadEventHandler AssemblyLoaded;

		private readonly static object							exclusion      = new object ();
		private readonly static Dictionary<string, Assembly>	assemblies     = new Dictionary<string, Assembly> ();
		private readonly static SafeCounter						recursionCount = new SafeCounter ();
		private readonly static Queue<System.Action>			pendingEvents  = new Queue<System.Action> ();
		
		private readonly static Assembly		CurrentAssembly	 = typeof (AssemblyLoader).Assembly;
		private readonly static byte[]			EpsitecPublicKey = AssemblyLoader.CurrentAssembly.GetName ().GetPublicKey ();
		
		private static int						loadPathDifferences;
	}
}