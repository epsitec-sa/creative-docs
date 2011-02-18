using System.Collections.Generic;


namespace Epsitec.Common.Types.Collections
{


	/// <summary>
	/// The <c>CircularBuffer</c> class implements a fixed size queue which provide read-only access
	/// to all its elements in addition to the add/remove operations.
	/// </summary>
	/// <remarks>Each operation on this class has a complexity of O(1), except the constructor and the
	/// <see cref="Clear"/> method.</remarks>
	/// <typeparam name="T">The type of the elements held by the buffer.</typeparam>
	public sealed class CircularBuffer<T>
	{


		/// <summary>
		/// Creates a new instance of <c>CircularBuffer</c> with the given size.
		/// </summary>
		/// <param name="size">The size of the buffer.</param>
		/// <exception cref="System.ArgumentException">If <paramref name="size"/> is lower than zero.</exception>
		public CircularBuffer(int size)
		{
			if (size < 0)
			{
				throw new System.ArgumentException ("size cannot be smaller than zero");
			}

			this.Count = 0;
			this.head = 0;
			this.tail = 0;

			this.elements = new T[size];
		}


		/// <summary>
		/// Gets the size of the current instance, that is the maximum number of elements that it
		/// can hold.
		/// </summary>
		public int Size
		{
			get
			{
				return this.elements.Length;
			}
		}


		/// <summary>
		/// Gets the number of elements currently held in the current instance.
		/// </summary>
		public int Count
		{
			get;
			private set;
		}


		/// <summary>
		/// Gets the element at the given position within the current instance.
		/// </summary>
		/// <param name="index">The index of the element to get.</param>
		/// <returns>The element.</returns>
		/// <exception cref="System.ArgumentOutOfRangeException">If <paramref name="index"/> is too small or too large.</exception>
		public T this[int index]
		{
			get
			{
				if (index < 0 || index >= this.Count)
				{
					throw new System.ArgumentOutOfRangeException ("Index out of bounds.");
				}

				return this.elements[(this.tail + index) % this.Size];
			}
		}


		/// <summary>
		/// Adds a new element at the end of the current instance.
		/// </summary>
		/// <param name="element">The element to add.</param>
		/// <exception cref="System.InvalidOperationException">If the buffer is full.</exception>
		public void Add(T element)
		{
			if (this.Count >= this.Size)
			{
				throw new System.InvalidOperationException ("Full buffer.");
			}

			this.elements[this.head] = element;

			this.head = (this.head + 1) % this.Size;
			this.Count = this.Count + 1;
		}


		/// <summary>
		/// Removes the element at the start of the current instance.
		/// </summary>
		/// <exception cref="System.InvalidOperationException">If the buffer is empty.</exception>
		public void Remove()
		{
			if (this.Count == 0)
			{
				throw new System.InvalidOperationException ("Empty buffer.");
			}

			this.elements[this.tail] = default (T);

			this.tail = (this.tail + 1) % this.Size;
			this.Count = this.Count - 1;
		}


		/// <summary>
		/// Removes all the elements in the current instance.
		/// </summary>
		public void Clear()
		{
			this.Count = 0;

			this.head = 0;
			this.tail = 0;

			for (int i = 0; i < this.elements.Length; i++)
			{
				this.elements[i] = default (T);
			}
		}


		/// <summary>
		/// The index in the internal array at which the next element will be inserted.
		/// </summary>
		private int head;


		/// <summary>
		/// The index in the internal array at which the next element will be removed.
		/// </summary>
		private int tail;
		

		/// <summary>
		/// The internal array which holds the elements of the current instance.
		/// </summary>
		private readonly T[] elements;


	}


}
