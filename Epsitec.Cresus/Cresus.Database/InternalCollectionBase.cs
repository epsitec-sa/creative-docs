//	Copyright © 2003, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Statut : OK/PA, 07/10/2003

namespace Epsitec.Cresus.Database
{
	using System.Collections;
	
	/// <summary>
	/// La classe InternalCollectionBase sert de base aux collections propres à la
	/// base de données.
	/// </summary>
	public abstract class InternalCollectionBase : IEnumerable, ICollection
	{
		public InternalCollectionBase()
		{
		}
		
		
		public virtual ArrayList		List
		{
			get { return null; }
		}
		
		
		#region Interface IEnumerable
		public IEnumerator GetEnumerator()
		{
			return this.List.GetEnumerator ();
		}
		#endregion
		
		#region	Interface ICollection
		public int						Count
		{
			get { return this.List.Count; }
		}

		public bool						IsSynchronized
		{
			get { return this.List.IsSynchronized; }
		}

		public object					SyncRoot
		{
			get { return this.List.SyncRoot; }
		}
		
		
		public void CopyTo(System.Array array, int index)
		{
			this.List.CopyTo (array, index);
		}
		#endregion
	}
}
