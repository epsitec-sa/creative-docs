//	Copyright © 2004, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Cresus.Database.Requests
{
	/// <summary>
	/// La requête Group permet de regrouper un ensemble de requêtes qui sont
	/// liées logiquement.
	/// </summary>
	
	[System.Serializable]
	
	public class Group : Base, System.Runtime.Serialization.ISerializable, System.Runtime.Serialization.IDeserializationCallback, System.Collections.IEnumerable, System.Collections.ICollection
	{
		public Group() : base (Type.Group)
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
			this.SetupType (Type.Group);
			
			object[] array = info.GetValue ("RequestArray", typeof (object[])) as object[];
			
			System.Diagnostics.Debug.Assert ((array == null) || (array.Length > 0));
			
			//	Les références aux divers objets n'ont pas encore été désérialisées
			//	et ne peuvent donc pas (encore) être insérées dans this.requests.
			//	Voir aussi la méthode OnDeserialization.
			
			this.deserialization_array = array;
		}
		
		public override void GetObjectData(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
		{
			object[] array = null;
			int n = this.Count;
			
			if (n > 0)
			{
				array = new object[n];
				this.CopyTo (array, 0);
			}
			
			info.AddValue ("RequestArray", array);
			
			base.GetObjectData (info, context);
		}
		#endregion
		
		#region IDeserializationCallback Members
		public void OnDeserialization(object sender)
		{
			if ((deserialization_array != null) &&
				(deserialization_array.Length > 0))
			{
				this.requests = new System.Collections.ArrayList ();
				this.requests.AddRange (this.deserialization_array);
			}
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
		protected object[]						deserialization_array;
	}
}
