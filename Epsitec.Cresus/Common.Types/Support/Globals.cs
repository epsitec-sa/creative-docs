//	Copyright © 2004-2006, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

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
			this.property_hash = new System.Collections.Hashtable ();
		}
		
		static Globals()
		{
			Globals.properties  = new Globals ();
			Globals.directories = new DirectoriesAccessor ();
			Globals.abort_event = new System.Threading.ManualResetEvent (false);
		}
		
		
		public static Globals					Properties
		{
			get
			{
				return Globals.properties;
			}
		}
		
		public static DirectoriesAccessor		Directories
		{
			get
			{
				return Globals.directories;
			}
		}
		
		public static string					ExecutableName
		{
			get
			{
				string name = System.Windows.Forms.Application.ExecutablePath;
				return System.IO.Path.GetFileName (name);
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
				return Globals.abort_event;
			}
		}
		
		
		public static void SignalAbort()
		{
			Globals.abort_event.Set ();
		}
		
		
		public string[] GetPropertyNames()
		{
			if (this.property_hash == null)
			{
				return new string[0];
			}
			
			string[] names = new string[this.property_hash.Count];
			this.property_hash.Keys.CopyTo (names, 0);
			System.Array.Sort (names);
			
			return names;
		}
		
		public void SetProperty(string key, object value)
		{
			lock (this)
			{
				this.property_hash[key] = value;
			}
		}
		
		public object GetProperty(string key)
		{
			lock (this)
			{
				return this.property_hash[key];
			}
		}
		
		public bool IsPropertyDefined(string key)
		{
			lock (this)
			{
				return this.property_hash.Contains (key);
			}
		}
		
		public void ClearProperty(string key)
		{
			lock (this)
			{
				this.property_hash.Remove (key);
			}
		}

		#region DirectoriesAccessor class
		public sealed class DirectoriesAccessor
		{
			public string						CommonAppData
			{
				get
				{
					return System.Windows.Forms.Application.CommonAppDataPath;
				}
			}
			
			public string						UserAppData
			{
				get
				{
					return System.Windows.Forms.Application.UserAppDataPath;
				}
			}
			
			public string						Executable
			{
				get
				{
					string name = System.Windows.Forms.Application.ExecutablePath;
					string root = System.IO.Path.GetPathRoot (name);
					string dir  = System.IO.Path.GetDirectoryName (name);
					string path = System.IO.Path.Combine (root, dir);
					return System.Windows.Forms.Application.StartupPath;
				}
			}
		}
		#endregion
		
		private System.Collections.Hashtable	property_hash;
		private static Globals					properties;
		private static DirectoriesAccessor		directories;
		private static ManualResetEvent			abort_event;
	}
}
