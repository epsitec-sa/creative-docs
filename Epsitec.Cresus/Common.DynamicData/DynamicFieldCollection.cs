//	Copyright © 2004, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.DynamicData
{
	/// <summary>
	/// La classe DynamicFieldCollection définit quels champs d'une table
	/// sont calculés de manière dynamique.
	/// </summary>
	public class DynamicFieldCollection : System.Collections.ICollection
	{
		public DynamicFieldCollection()
		{
			this.list = new System.Collections.ArrayList ();
		}
		
		
		public void Add(IDynamicField field)
		{
			this.list.Add (field);
			this.InvalidateCache ();
		}
		
		
		#region ICollection Members
		public int								Count
		{
			get
			{
				return this.list.Count;
			}
		}
		
		public bool								IsSynchronized
		{
			get
			{
				return false;
			}
		}
		
		public object							SyncRoot
		{
			get
			{
				return null;
			}
		}
		
		public void CopyTo(System.Array array, int index)
		{
			this.list.CopyTo (array, index);
		}
		#endregion
		
		#region IEnumerable Members
		public System.Collections.IEnumerator GetEnumerator()
		{
			return this.list.GetEnumerator ();
		}
		#endregion
		
		
		protected void UpdateCache()
		{
			if (this.cache == null)
			{
				this.cache = new System.Collections.Hashtable ();
				
				//	TODO: remplir le cache
			}
		}
		
		protected void InvalidateCache()
		{
			this.cache = null;
		}
		
		
		private System.Collections.ArrayList	list;
		private System.Collections.Hashtable	cache;
	}
}
