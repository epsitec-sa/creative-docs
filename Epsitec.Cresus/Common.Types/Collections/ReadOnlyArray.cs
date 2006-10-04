//	Copyright © 2006, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

using System.Collections.Generic;

namespace Epsitec.Common.Types.Collections
{
	/// <summary>
	/// La classe ReadOnlyArray se comporte exactement comme un Array,
	/// si ce n'est que l'on ne peut pas en modifier le contenu. C'est
	/// utile lorsqu'une méthode désire retourner un array pour consultation
	/// uniquement.
	/// </summary>
	/// <typeparam name="T">Le type d'un élément</typeparam>
	public struct ReadOnlyArray<T> : ICollection<T>, IList<T>
	{
		public ReadOnlyArray(T[] array)
		{
			this.array = array;
		}

		public int								Length
		{
			get
			{
				return this.array == null ? 0 : this.array.Length;
			}
		}
		public int								Rank
		{
			get
			{
				return 1;
			}
		}
		public bool								IsFixedSize
		{
			get
			{
				return true;
			}
		}
		public bool								IsSynchronized
		{
			get
			{
				return this.array.IsSynchronized;
			}
		}
		public bool								IsNull
		{
			get
			{
				return this.array == null;
			}
		}
		
		public T[] ToArray()
		{
			return Copier.CopyArray (this.array);
		}
		
		#region ICollection<T> Members
		void ICollection<T>.Add(T item)
		{
			throw new System.InvalidOperationException ();
		}
		bool ICollection<T>.Remove(T item)
		{
			throw new System.InvalidOperationException ();
		}
		void ICollection<T>.Clear()
		{
			throw new System.InvalidOperationException ();
		}
		
		public bool Contains(T item)
		{
			if (this.array != null)
			{
				for (int i = 0; i < this.array.Length; i++)
				{
					if (object.ReferenceEquals (this.array[i], item))
					{
						return true;
					}
				}
			}
			
			return false;
		}
		public void CopyTo(T[] array, int index)
		{
			this.array.CopyTo (array, index);
		}

		public int								Count
		{
			get
			{
				return this.array == null ? 0 : this.array.Length;
			}
		}
		public bool								IsReadOnly
		{
			get
			{
				return true;
			}
		}
		#endregion

		#region IList<T> Members
		public T								this[int index]
		{
			get
			{
				return this.array[index];
			}
			set
			{
				throw new System.InvalidOperationException ();
			}
		}
		
		public int IndexOf(T item)
		{
			if (this.array != null)
			{
				for (int i = 0; i < this.array.Length; i++)
				{
					if (object.ReferenceEquals (this.array[i], item))
					{
						return i;
					}
				}
			}

			return -1;
		}

		void IList<T>.Insert(int index, T item)
		{
			throw new System.InvalidOperationException ();
		}
		void IList<T>.RemoveAt(int index)
		{
			throw new System.InvalidOperationException ();
		}
		#endregion

		#region IEnumerable<T> Members
		public IEnumerator<T> GetEnumerator()
		{
			return new Enumerator (this.array);
		}
		#endregion

		#region IEnumerable Members
		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			return this.GetEnumerator ();
		}
		#endregion

		#region Private Enumerator Class
		private struct Enumerator : IEnumerator<T>
		{
			public Enumerator(T[] array)
			{
				this.index = -1;
				this.array = array;
			}
			
			#region IEnumerator<T> Members
			public T Current
			{
				get
				{
					return this.array[this.index];
				}
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

			public bool MoveNext()
			{
				if (this.index+1 < this.array.Length)
				{
					this.index++;
					return true;
				}
				else
				{
					return false;
				}
			}

			public void Reset()
			{
				this.index = -1;
			}
			#endregion

			private int index;
			private T[] array;
		}
		#endregion

		private T[]								array;
	}
}
