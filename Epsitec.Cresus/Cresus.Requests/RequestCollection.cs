//	Copyright © 2004-2009, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD


using System.Collections;
using System.Collections.Generic;

using System.Runtime.Serialization;


namespace Epsitec.Cresus.Requests
{
	
	
	/// <summary>
	/// The <c>RequestCollection</c> class groups logically related requests into a
	/// single meta request.
	/// </summary>
	[System.Serializable]
	public sealed class RequestCollection : AbstractRequest, IDeserializationCallback, IEnumerable<AbstractRequest>, ICollection<AbstractRequest>, ICollection, IEnumerable
	{
		
		
		/// <summary>
		/// Initializes a new instance of the <see cref="RequestCollection"/> class.
		/// </summary>
		public RequestCollection() : base()
		{
			this.requests = new List<AbstractRequest> ();
		}


		/// <summary>
		/// Gets the <see cref="AbstractRequest"/> at the specified index.
		/// </summary>
		/// <value>The request.</value>
		public AbstractRequest this[int index]
		{
			get
			{
				if (index < 0 || index >= this.requests.Count)
				{
					throw new System.ArgumentException ("Index invalid.");
				}

				return this.requests[index];
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


		RequestCollection(SerializationInfo info, StreamingContext context) : base (info, context)
		{
			AbstractRequest[] array = info.GetValue (SerializationKeys.Array, typeof (AbstractRequest[])) as AbstractRequest[];

			System.Diagnostics.Debug.Assert (array != null);

			//	The references to the requests have not yet been deserialized and we must
			//	therefore keep the reference to the array until the point where method
			//	OnDeserialization gets called :

			this.deserializationArray = array;
		}


		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData (info, context);

			info.AddValue (SerializationKeys.Array, this.requests.ToArray ());
		}

		
		#endregion


		#region Strings Class

		private static class SerializationKeys
		{
			public const string Array = "Array";
		}

		#endregion


		#region IDeserializationCallback Members
		

		void IDeserializationCallback.OnDeserialization(object sender)
		{
			if (this.deserializationArray != null && this.deserializationArray.Length > 0)
			{
				//	Instantiates the real collection :

				this.requests = new List<AbstractRequest> (this.deserializationArray);
				this.deserializationArray = null;
			}
		}
		

		#endregion


		#region IEnumerable<AbstractRequest> Members


		IEnumerator<AbstractRequest> IEnumerable<AbstractRequest>.GetEnumerator()
		{
			return this.requests.GetEnumerator ();
		}


		#endregion


		#region ICollection<AbstractRequest> Members


		public void Clear()
		{
			this.requests.Clear ();
		}


		public bool Contains(AbstractRequest item)
		{
			return this.requests.Contains (item);
		}


		public void CopyTo(AbstractRequest[] array, int arrayIndex)
		{
			this.requests.CopyTo (array, arrayIndex);
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
			return this.requests.Remove (item);
		}


		#endregion
		

		#region IEnumerable Members


		public IEnumerator GetEnumerator()
		{
			return this.requests.GetEnumerator ();
		}
		

		#endregion
		

		#region ICollection Members
		

		public bool IsSynchronized
		{
			get
			{
				return false;
			}
		}


		public object SyncRoot
		{
			get
			{
				return this.requests;
			}
		}
		

		public int Count
		{
			get
			{
				return this.requests.Count;
			}
		}

		
		public void CopyTo(System.Array array, int index)
		{
			(this.requests as ICollection).CopyTo (array, index);
		}
		
		#endregion


		private List<AbstractRequest> requests;


		private AbstractRequest[] deserializationArray;


	}


}
