//	Copyright � 2004-2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Cresus.Requests
{
	/// <summary>
	/// La requ�te Group permet de regrouper un ensemble de requ�tes qui sont
	/// li�es logiquement.
	/// </summary>
	
	[System.Serializable]
	
	public class Group : AbstractRequest, System.Runtime.Serialization.ISerializable, System.Runtime.Serialization.IDeserializationCallback, System.Collections.IEnumerable, System.Collections.ICollection
	{
		public Group() : base (RequestType.Group)
		{
		}
		
		
		public AbstractRequest					this[int index]
		{
			get
			{
				if (this.Count > 0)
				{
					return this.requests[index] as AbstractRequest;
				}
				
				throw new System.IndexOutOfRangeException ("Index invalid.");
			}
		}
		
		
		public void Add(AbstractRequest request)
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
		
		
		public override void Execute(ExecutionEngine engine)
		{
			if (this.Count > 0)
			{
				foreach (AbstractRequest request in this.requests)
				{
					request.Execute (engine);
				}
			}
		}
		
		
		#region ISerializable Members
		protected Group(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context) : base (info, context)
		{
			this.SetupRequestType (RequestType.Group);
			
			object[] array = info.GetValue ("RequestArray", typeof (object[])) as object[];
			
			System.Diagnostics.Debug.Assert ((array == null) || (array.Length > 0));
			
			//	Les r�f�rences aux divers objets n'ont pas encore �t� d�s�rialis�es
			//	et ne peuvent donc pas (encore) �tre ins�r�es dans this.requests.
			//	Voir aussi la m�thode OnDeserialization.
			
			this.deserialization_array = array;
		}
		
		public override void GetObjectData(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
		{
			base.GetObjectData (info, context);
			
			object[] array = null;
			int n = this.Count;
			
			if (n > 0)
			{
				array = new object[n];
				this.CopyTo (array, 0);
			}
			
			info.AddValue ("RequestArray", array);
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
