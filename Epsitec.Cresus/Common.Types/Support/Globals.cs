//	Copyright © 2004-2009, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Responsable: Pierre ARNAUD

using System.Collections.Generic;

namespace Epsitec.Common.Support
{
	using WaitHandle = System.Threading.WaitHandle;
	using ManualResetEvent = System.Threading.ManualResetEvent;
	
	/// <summary>
	/// La classe Globals permet de stocker des variables globales à une application.
	/// </summary>
	public sealed class Globals
	{
		private Globals()
		{
			this.propertyHash = new Dictionary<string, object> ();
		}
		
		static Globals()
		{
			Globals.properties  = new Globals ();
			Globals.abortEvent = new System.Threading.ManualResetEvent (false);
		}
		
		
		public static Globals					Properties
		{
			get
			{
				return Globals.properties;
			}
		}

		public static string ExecutableName
		{
			get
			{
				string name = System.Windows.Forms.Application.ExecutablePath;
				return System.IO.Path.GetFileName (name);
			}
		}
		
		public static string ExecutablePath
		{
			get
			{
				string name = System.Windows.Forms.Application.ExecutablePath;
				return name;
			}
		}
		
		public object							this[string key]
		{
			//	On peut accéder aux propriétés globales très simplement au moyen de l'opérateur [],
			//	mais contrairement à GetProperty, une exception est levée si la propriété demandée
			//	n'existe pas.
			
			get
			{
				if (this.IsPropertyDefined (key))
				{
					return this.GetProperty (key);
				}
				
				throw new System.ArgumentOutOfRangeException ("key", key, string.Format ("Cannot find the global property named '{0}'.", key));
			}
			set
			{
				this.SetProperty (key, value);
			}
		}
		
		public static WaitHandle				AbortEvent
		{
			//	Cet événement peut être utilisé par toutes les classes qui désirent réaliser
			//	une attente qui soit interruptible par l'arrêt de l'application; pour cela,
			//	on réalise un appel à WaitHandle.WaitAny en spécifiant AbortEvent comme l'un
			//	des "événements" sur lesquels attendre.
			
			get
			{
				return Globals.abortEvent;
			}
		}

		public static bool						IsDebugBuild
		{
			get
			{
				if (Globals.isDebugBuildInitialized == false)
				{
					Globals.isDebugBuild = typeof (Globals).Assembly.Location.Contains ("Debug") || Globals.Directories.ExecutableRoot.StartsWith (@"S:\Epsitec.Cresus");
					Globals.isDebugBuildInitialized = true;
				}

				return Globals.isDebugBuild;
			}
		}
		
		public static void SignalAbort()
		{
			Globals.abortEvent.Set ();
		}
		
		
		public string[] GetPropertyNames()
		{
			string[] names = new string[this.propertyHash.Count];
			this.propertyHash.Keys.CopyTo (names, 0);
			System.Array.Sort (names);
			
			return names;
		}
		
		public void SetProperty(string key, object value)
		{
			lock (this)
			{
				if (Types.UndefinedValue.IsUndefinedValue (value))
				{
					this.propertyHash.Remove (key);
				}
				else
				{
					this.propertyHash[key] = value;
				}
			}
		}
		
		public object GetProperty(string key)
		{
			lock (this)
			{
				object value;

				if (this.propertyHash.TryGetValue (key, out value))
				{
					return value;
				}
			}

			return Types.UndefinedValue.Value;
		}

		public T GetProperty<T>(string key)
		{
			object value = this.GetProperty (key);

			if (Types.UndefinedValue.IsUndefinedValue (value))
			{
				return default (T);
			}
			else
			{
				return (T) value;
			}
		}

		public T GetProperty<T>(string key, T defaultValue)
		{
			object value = this.GetProperty (key);

			if (Types.UndefinedValue.IsUndefinedValue (value))
			{
				return defaultValue;
			}
			else
			{
				return (T) value;
			}
		}
		
		public bool IsPropertyDefined(string key)
		{
			lock (this)
			{
				return this.propertyHash.ContainsKey (key);
			}
		}
		
		public void ClearProperty(string key)
		{
			lock (this)
			{
				this.propertyHash.Remove (key);
			}
		}

		#region Directories Class

		public static class Directories
		{
			public static string				CommonAppDataRevision
			{
				get
				{
					return System.Windows.Forms.Application.CommonAppDataPath;
				}
			}

			public static string				CommonAppData
			{
				get
				{
					string path = Directories.CommonAppDataRevision;
					return System.IO.Path.GetDirectoryName (path);
				}
			}

			public static string				UserAppDataRevision
			{
				get
				{
					return System.Windows.Forms.Application.UserAppDataPath;
				}
			}

			public static string				UserAppData
			{
				get
				{
					string path = Directories.UserAppDataRevision;
					return System.IO.Path.GetDirectoryName (path);
				}
			}
			
			public static string				Executable
			{
				get
				{
					return System.Windows.Forms.Application.StartupPath;
				}
			}

			public static string				ExecutableRoot
			{
				get
				{
					string path = Directories.Executable;

					string debugSuffix   = @"\bin\Debug";
					string releaseSuffix = @"\bin\Release";
					string nunitSuffix   = @"\nunit";

					if (path.EndsWith (debugSuffix))
					{
						path = path.Substring (0, path.Length - debugSuffix.Length);
					}
					else if (path.EndsWith (releaseSuffix))
					{
						path = path.Substring (0, path.Length - releaseSuffix.Length);
					}
					else if (path.Contains (nunitSuffix))
					{
						path = path.Substring (0, path.IndexOf (nunitSuffix));
					}

					return path;
				}
			}

			public static string				InitialDirectory
			{
				get
				{
					return Directories.initialDirectory;
				}
			}

			public static string				ProgramFiles
			{
				get
				{
					return System.Environment.GetFolderPath (System.Environment.SpecialFolder.ProgramFiles);
				}
			}

			public static string				Windows
			{
				get
				{
					return System.IO.Path.GetDirectoryName (System.Environment.GetFolderPath (System.Environment.SpecialFolder.System));
				}
			}


			private static readonly string initialDirectory = System.IO.Directory.GetCurrentDirectory ();
		}
		
		#endregion
		
		private Dictionary<string, object>		propertyHash;
		private static Globals					properties;
		private static ManualResetEvent			abortEvent;
		private static bool						isDebugBuild;
		private static bool						isDebugBuildInitialized;
	}
}
