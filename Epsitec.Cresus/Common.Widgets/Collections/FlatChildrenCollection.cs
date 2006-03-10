//	Copyright © 2006, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

using System.Collections.Generic;

namespace Epsitec.Common.Widgets.Collections
{
	/// <summary>
	/// La classe FlatChildrenCollection stocke un ensemble de widgets de manière
	/// ordonnée.
	/// </summary>
	public class FlatChildrenCollection : IList<Visual>
	{
		internal FlatChildrenCollection(Visual host)
		{
			this.host = host;
		}
		class Snapshot
		{
			private Snapshot()
			{
			}

			public void NotifyChanges()
			{
			}
			
			public static Snapshot RecordTree(params Visual[] visuals)
			{
				Snapshot snapshot = new Snapshot ();
				return snapshot;
			}
		}
		
		#region IList<Visual> Members

		public Visual							this[int index]
		{
			get
			{
				return this.visuals[index];
			}
			set
			{
				if (this.visuals[index] != value)
				{
					Snapshot snapshot = Snapshot.RecordTree (this.visuals[index], value);

					this.DetachVisual (this.visuals[index]);
					this.visuals[index] = value;
					this.AttachVisual (this.visuals[index]);

					snapshot.NotifyChanges ();
				}
			}
		}

		public int IndexOf(Visual item)
		{
			return this.visuals.IndexOf (item);
		}

		public void Insert(int index, Visual item)
		{
			throw new System.Exception ("The method or operation is not implemented.");
		}

		public void RemoveAt(int index)
		{
			throw new System.Exception ("The method or operation is not implemented.");
		}

		#endregion

		#region ICollection<Visual> Members

		public int								Count
		{
			get
			{
				return this.visuals.Count;
			}
		}
		public bool								IsReadOnly
		{
			get
			{
				return false;
			}
		}

		public void Add(Visual item)
		{
			throw new System.Exception ("The method or operation is not implemented.");
		}

		public bool Remove(Visual item)
		{
			throw new System.Exception ("The method or operation is not implemented.");
		}

		public void Clear()
		{
			throw new System.Exception ("The method or operation is not implemented.");
		}

		public bool Contains(Visual item)
		{
			return this.visuals.Contains (item);
		}

		public void CopyTo(Visual[] array, int index)
		{
			this.visuals.CopyTo (array, index);
		}

		#endregion

		#region IEnumerable<Visual> Members

		public IEnumerator<Visual> GetEnumerator()
		{
			return this.visuals.GetEnumerator ();
		}

		#endregion

		#region IEnumerable Members
		
		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			return this.GetEnumerator ();
		}
		
		#endregion

		private Visual							host;
		private List<Visual>					visuals = new List<Visual> ();
	}
}
