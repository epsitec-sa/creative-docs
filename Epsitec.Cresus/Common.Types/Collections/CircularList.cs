//	Copyright © 2008, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using System.Collections.Generic;

namespace Epsitec.Common.Types.Collections
{
	/// <summary>
	/// The <c>CircularList</c> class implements a circular list where the
	/// index wraps around at the end and the beginning of the list.
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public class CircularList<T> : IList<T>
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="CircularList&lt;T&gt;"/> class.
		/// </summary>
		public CircularList()
		{
			this.list = new List<T> ();
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="CircularList&lt;T&gt;"/> class.
		/// </summary>
		/// <param name="collection">The collection.</param>
		public CircularList(IEnumerable<T> collection)
			: this ()
		{
			this.list.AddRange (collection);
		}


		/// <summary>
		/// Adds the collection to the list.
		/// </summary>
		/// <param name="collection">The collection.</param>
		public void AddRange(IEnumerable<T> collection)
		{
			foreach (T item in collection)
			{
				this.Add (item);
			}
		}

		/// <summary>
		/// Rotates the list by the specified amplitude. A positive distance
		/// moves the origin to the right in the list, a negative distance
		/// moves the origin to the list in the list.
		/// </summary>
		/// <remarks>This is an O(1) operation.</remarks>
		/// <param name="amplitude">The amplitude of the rotation.</param>
		public void Rotate(int amplitude)
		{
			
			if ((amplitude != 0) &&
				(this.list.Count > 0))
			{
				if (this.reverseDirection)
				{
					amplitude = -amplitude;
				}

				this.headIndex = this.ClipInternalIndex (this.headIndex + amplitude);
			}
		}

		/// <summary>
		/// Reverses the list.
		/// </summary>
		/// <remarks>This is an O(1) operation.</remarks>
		public void Reverse()
		{
			int n = this.list.Count;

			if (this.reverseDirection)
			{
				this.reverseDirection = false;
				this.headIndex++;

				if (this.headIndex >= n)
				{
					this.headIndex = 0;
				}
			}
			else
			{
				this.reverseDirection = true;

				if (this.headIndex == 0)
				{
					this.headIndex = System.Math.Max (n-1, 0);
				}
				else
				{
					this.headIndex--;
				}
			}
		}


		/// <summary>
		/// Gets the collection in reverse order.
		/// </summary>
		/// <param name="collection">The collection.</param>
		/// <returns>The collection in reverse order.</returns>
		public static IEnumerable<T> Reverse(CircularList<T> collection)
		{
			int n    = collection.list.Count;
			int head = collection.headIndex;

			if (collection.reverseDirection)
			{
				head = head >= n-1 ? 0 : head+1;
				return new Enumerator (collection.list, head, false);
			}
			else
			{
				head = (head == 0) ? System.Math.Max (n-1, 0) : head-1;
				return new Enumerator (collection.list, head, true);
			}
		}


		#region IList<T> Members

		public int IndexOf(T item)
		{
			return this.ToExternalIndex (this.list.IndexOf (item));
		}

		public void Insert(int index, T item)
		{
			if (this.list.Count == 0)
			{
				this.list.Add (item);
			}
			else if (index == 0)
			{
				//	Special case : insert at the head.

				if (this.reverseDirection)
				{
					this.headIndex++;
					this.list.Insert (this.headIndex, item);
				}
				else
				{
					this.list.Insert (this.headIndex, item);
				}
			}
			else
			{
				index = this.ToInternalIndex (index);

				System.Diagnostics.Debug.Assert (index >= 0);
				System.Diagnostics.Debug.Assert (index < this.list.Count);

				if (this.reverseDirection)
				{
					index++;
				}
				if (index <= this.headIndex)
				{
					this.headIndex++;
				}

				this.list.Insert (index, item);
			}
		}

		public void RemoveAt(int index)
		{
			index = this.ToInternalIndex (index);

			this.list.RemoveAt (index);

			if (index < this.headIndex)
			{
				this.headIndex--;
			}
		}

		public T this[int index]
		{
			get
			{
				return this.list[this.ToInternalIndex (index)];
			}
			set
			{
				this.list[this.ToInternalIndex (index)] = value;
			}
		}

		#endregion

		#region ICollection<T> Members

		public void Add(T item)
		{
			this.Insert (this.list.Count, item);
		}

		public void Clear()
		{
			this.list.Clear ();
			this.reverseDirection = false;
			this.headIndex = 0;
		}

		public bool Contains(T item)
		{
			return this.list.Contains (item);
		}

		public void CopyTo(T[] array, int arrayIndex)
		{
			int index = this.ToInternalIndex (arrayIndex);
			int count = this.list.Count - arrayIndex;
			int pos   = 0;

			if (this.reverseDirection)
			{
				while (count-- > 0)
				{
					array[pos++] = this.list[this.ClipInternalIndex (index--)];
				}
			}
			else
			{
				while (count-- > 0)
				{
					array[pos++] = this.list[this.ClipInternalIndex (index++)];
				}
			}
		}

		public int Count
		{
			get
			{
				return this.list.Count;
			}
		}

		public bool IsReadOnly
		{
			get
			{
				return false;
			}
		}

		public bool Remove(T item)
		{
			int index = this.list.IndexOf (item);

			if (index < 0)
			{
				return false;
			}
			else
			{
				if (index < this.headIndex)
				{
					this.headIndex--;
				}
				
				this.list.RemoveAt (index);
				
				return true;
			}
		}

		#endregion

		#region IEnumerable<T> Members

		public IEnumerator<T> GetEnumerator()
		{
			return new Enumerator (this.list, this.headIndex, this.reverseDirection);
		}

		#endregion

		#region IEnumerable Members

		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			return new Enumerator (this.list, this.headIndex, this.reverseDirection);
		}

		#endregion

		#region Enumerator Class

		/// <summary>
		/// The <c>Enumerator</c> class implements the enumerator for the
		/// circular list.
		/// </summary>
		private class Enumerator : IEnumerator<T>, System.Collections.IEnumerator, IEnumerable<T>
		{
			public Enumerator(IList<T> list, int headIndex, bool reverseDirection)
			{
				this.list = list;
				this.headIndex = headIndex;
				this.reverseDirection = reverseDirection;
				this.Reset ();
			}

			#region IEnumerator<T> Members

			public T Current
			{
				get
				{
					return this.list[this.index];
				}
			}

			public bool MoveNext()
			{
				if (this.count == 0)
				{
					return false;
				}

				int index = this.index;

				if (this.reverseDirection)
				{
					index--;
					
					if (index < 0)
					{
						index = this.list.Count-1;
					}
				}
				else
				{
					index++;

					if (index >= this.list.Count)
					{
						index = 0;
					}
				}

				this.index = index;
				this.count--;

				return true;
			}

			public void Reset()
			{
				this.index = this.reverseDirection ? this.headIndex+1 : this.headIndex-1;
				this.count = this.list.Count;
			}

			#endregion

			#region IDisposable Members

			public void Dispose()
			{
			}

			#endregion

			#region IEnumerator Members

			object System.Collections.IEnumerator.Current
			{
				get
				{
					return this.Current;
				}
			}

			bool System.Collections.IEnumerator.MoveNext()
			{
				return this.MoveNext ();
			}

			void System.Collections.IEnumerator.Reset()
			{
				this.Reset ();
			}

			#endregion

			#region IEnumerable<T> Members

			public IEnumerator<T> GetEnumerator()
			{
				return this;
			}

			#endregion

			#region IEnumerable Members

			System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
			{
				return this;
			}

			#endregion
			
			private readonly IList<T> list;
			private readonly bool reverseDirection;
			private readonly int headIndex;
			private int index;
			private int count;
		}

		#endregion

		private int ToInternalIndex(int index)
		{
			int n = this.list.Count;

			if (n == 0)
			{
				return 0;
			}
			
			index = (this.reverseDirection) ? this.headIndex - index : this.headIndex + index;
			index = index % n;
			index = (index < 0) ? index + n : index;

			return index;
		}

		private int ToExternalIndex(int index)
		{
			if (index < 0)
			{
				return index;
			}

			int n = this.list.Count;
			
			index = (this.reverseDirection) ? this.headIndex - index : index - this.headIndex;

			return (index < 0) ? index + n : index;
		}
		
		private int ClipInternalIndex(int index)
		{
			int n = this.list.Count;

			if (n == 0)
			{
				return 0;
			}

			index = index % n;
			index = (index < 0) ? index + n : index;

			return index;
		}

		
		private readonly List<T> list;
		private int headIndex;
		private bool reverseDirection;
	}
}
