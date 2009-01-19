//	Copyright © 2004-2009, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using System.Collections.Generic;

namespace Epsitec.Cresus.Requests
{
	/// <summary>
	/// The <c>RequestCollection</c> class groups logically related requests into a
	/// single meta request.
	/// </summary>
	
	[System.Serializable]
	
	public class RequestCollection : AbstractRequest, System.Runtime.Serialization.ISerializable, System.Runtime.Serialization.IDeserializationCallback, IEnumerable<AbstractRequest>, ICollection<AbstractRequest>
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="RequestCollection"/> class.
		/// </summary>
		public RequestCollection()
		{
		}


		/// <summary>
		/// Gets the <see cref="AbstractRequest"/> at the specified index.
		/// </summary>
		/// <value>The request.</value>
		public AbstractRequest					this[int index]
		{
			get
			{
				if ((this.requests != null) &&
					(this.requests.Count > index))
				{
					return this.requests[index];
				}
				
				throw new System.IndexOutOfRangeException ("Index invalid.");
			}
		}


		/// <summary>
		/// Adds the specified request to the collection.
		/// </summary>
		/// <param name="request">The request.</param>
		public void Add(AbstractRequest request)
		{
			if (request == null)
			{
				throw new System.ArgumentNullException ("request", "Null request provided");
			}
			
			if (this.requests == null)
			{
				this.requests = new List<AbstractRequest> ();
			}
			
			this.requests.Add (request);
		}

		/// <summary>
		/// Adds the specified range of requests to the collection.
		/// </summary>
		/// <param name="requests">The requests.</param>
		public void AddRange(IEnumerable<AbstractRequest> requests)
		{
			if (requests != null)
			{
				if (this.requests == null)
				{
					this.requests = new List<AbstractRequest> ();
				}
				
				this.requests.AddRange (requests);
			}
		}


		/// <summary>
		/// Executes sequentially all the requests using the specified execution
		/// engine.
		/// </summary>
		/// <param name="engine">The execution engine.</param>
		public override void Execute(ExecutionEngine engine)
		{
			foreach (AbstractRequest request in this)
			{
				request.Execute (engine);
			}
		}
		
		
		#region ISerializable Members
		
		protected RequestCollection(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context) : base (info, context)
		{
			AbstractRequest[] array = (AbstractRequest[]) info.GetValue (Strings.Array, typeof (AbstractRequest[]));
			
			System.Diagnostics.Debug.Assert ((array == null) || (array.Length > 0));
			
			//	The references to the requests have not yet been deserialized and we must
			//	therefore keep the reference to the array until the point where method
			//	OnDeserialization gets called :
			
			this.deserializationArray = array;
		}
		
		public override void GetObjectData(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
		{
			base.GetObjectData (info, context);
			
			AbstractRequest[] array = null;
			
			if ((this.requests != null) &&
				(this.requests.Count > 0))
			{
				array = this.requests.ToArray ();
			}

			info.AddValue (Strings.Array, array);
		}
		
		#endregion
		
		#region IDeserializationCallback Members
		
		void System.Runtime.Serialization.IDeserializationCallback.OnDeserialization(object sender)
		{
			if ((deserializationArray != null) &&
				(deserializationArray.Length > 0))
			{
				//	Instanciates the real collection :

				this.requests = new List<AbstractRequest> (this.deserializationArray);
				this.deserializationArray = null;
			}
		}
		
		#endregion

		#region IEnumerable<AbstractRequest> Members

		IEnumerator<AbstractRequest> IEnumerable<AbstractRequest>.GetEnumerator()
		{
			if (this.requests == null)
			{
				return Epsitec.Common.Support.EmptyEnumerator<AbstractRequest>.Instance;
			}
			else
			{
				return this.requests.GetEnumerator ();
			}
		}

		#endregion

		#region ICollection<AbstractRequest> Members

		public void Clear()
		{
			this.requests = null;
		}

		public bool Contains(AbstractRequest item)
		{
			if (this.requests != null)
			{
				return this.requests.Contains (item);
			}
			else
			{
				return false;
			}
		}

		public void CopyTo(AbstractRequest[] array, int arrayIndex)
		{
			if (this.requests != null)
			{
				this.requests.CopyTo (array, arrayIndex);
			}
		}

		public bool IsReadOnly
		{
			get
			{
				return false;
			}
		}

		public bool Remove(AbstractRequest item)
		{
			if (this.requests != null)
			{
				return this.requests.Remove (item);
			}
			else
			{
				return false;
			}
		}

		#endregion
		
		#region IEnumerable Members
		
		public System.Collections.IEnumerator GetEnumerator()
		{
			if (this.requests == null)
			{
				return Epsitec.Common.Support.EmptyEnumerator<AbstractRequest>.Instance;
			}
			else
			{
				System.Collections.IEnumerable enumerable = this.requests;
				return enumerable.GetEnumerator ();
			}
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
				if (this.requests == null)
				{
					return this;
				}
				else
				{
					return this.requests;
				}
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
				System.Collections.ICollection collection = this.requests;
				collection.CopyTo (array, index);
			}
		}
		
		#endregion

		#region Strings Class

		static class Strings
		{
			public const string Array = "Array";
		}

		#endregion

		List<AbstractRequest>					requests;
		AbstractRequest[]						deserializationArray;
	}
}
