//	Copyright © 2004, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Cresus.Database.Requests
{
	/// <summary>
	/// La requête Group permet de regrouper un ensemble de requêtes qui sont
	/// liées logiquement.
	/// </summary>
	
	[System.Serializable]
	
	public class Group : Base, System.Runtime.Serialization.ISerializable, System.Collections.IEnumerable, System.Collections.ICollection
	{
		public Group()
		{
		}
		

		public Requests.Base					this[int index]
		{
			get
			{
				if (this.Count > 0)
				{
					return this.requests[index] as Requests.Base;
				}
				
				throw new System.IndexOutOfRangeException ("Index invalid.");
			}
		}
		
		
		public void Add(Requests.Base request)
		{
			if (request == null)
			{
				throw new System.ArgumentNullException ("request", "Null request provided");
			}
			
			if (this.requests == null)
			{
				this.requests = new System.Collections.ArrayList ();
			}
			
			this.requests.Add (request);
		}
		
		public void AddRange(System.Collections.ICollection requests)
		{
			if ((requests != null) &&
				(requests.Count > 0))
			{
				if (this.requests == null)
				{
					this.requests = new System.Collections.ArrayList ();
				}
				
				this.requests.AddRange (requests);
			}
		}
		
		
		#region ISerializable Members
		protected Group(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context) : base (info, context)
		{
			System.Array array = info.GetValue ("RequestArray", typeof (System.Array)) as System.Array;
			
			if ((array != null) &&
				(array.Length > 0))
			{
				this.requests = new System.Collections.ArrayList ();
				this.requests.AddRange (array);
			}
		}
		
		public override void GetObjectData(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
		{
			System.Array array = null;
			int n = this.Count;
			
			if (n > 0)
			{
				array = new Requests.Base[n];
				this.CopyTo (array, 0);
			}
			
			info.AddValue ("RequestArray", array);
			
			base.GetObjectData (info, context);
		}
		#endregion
		
		#region IEnumerable Members
		public System.Collections.IEnumerator GetEnumerator()
		{
			if (this.requests == null)
			{
				return new Epsitec.Common.Support.EmptyEnumerator ();
			}
			
			return this.requests.GetEnumerator ();
		}
		#endregion
		
		#region ICollection Members
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
		
		public int								Count
		{
			get
			{
				return (this.requests == null) ? 0 : this.requests.Count;
			}
		}

		
		public void CopyTo(System.Array array, int index)
		{
			if (this.requests != null)
			{
				this.requests.CopyTo (array, index);
			}
		}
		#endregion
		
		
		protected System.Collections.ArrayList	requests;
	}
}
