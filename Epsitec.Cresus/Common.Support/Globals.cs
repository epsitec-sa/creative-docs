//	Copyright © 2004, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.Support
{
	/// <summary>
	/// La classe Globals permet de stocker des variables globales à une application.
	/// </summary>
	public sealed class Globals : Data.IPropertyProvider
	{
		private Globals()
		{
			this.property_hash = new System.Collections.Hashtable ();
		}
		
		static Globals()
		{
			Globals.properties = new Globals ();
		}
		
		
		public static Globals					Properties
		{
			get
			{
				return Globals.properties;
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
		
		
		#region IPropertyProvider Members
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
		#endregion
		
		private System.Collections.Hashtable	property_hash;
		private static Globals					properties;
	}
}
