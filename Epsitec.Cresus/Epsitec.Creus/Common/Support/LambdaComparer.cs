using Epsitec.Common.Support.Extensions;

using System;
using System.Collections.Generic;


namespace Epsitec.Common.Support
{


	/// <summary>
	/// The <c>LambdaComparer</c> class makes it easy to build custom instances of
	/// <see cref="IEqualityComparer{T}"/> with lambda expressions.
	/// </summary>
	/// <typeparam name="T">The type of the elements that will be compared.</typeparam>
	public sealed class LambdaComparer<T> : EqualityComparer<T>
	{


		/// <summary>
		/// Builds a new instance of <c>LambdaComparer</c> that uses the default hashing function.
		/// </summary>
		/// <param name="compareFunction">The function used to compare two objects.</param>
		/// <exception cref="System.ArgumentNullException">If <paramref name="compareFunction"/> is <c>null</c>.</exception>
		public LambdaComparer(Func<T, T, bool> compareFunction)
			: this (compareFunction, EqualityComparer<T>.Default.GetHashCode)
		{
		}


		/// <summary>
		/// Builds a new instance of <c>LambdaComparer</c>.
		/// </summary>
		/// <param name="compareFunction">The function used to compare two objects.</param>
		/// <param name="hashFunction">The function used to get the hash code of an object.</param>
		/// <exception cref="System.ArgumentNullException">If <paramref name="compareFunction"/> is <c>null</c>.</exception>
		/// <exception cref="System.ArgumentNullException">If <paramref name="hashFunction"/> is <c>null</c>.</exception>
		public LambdaComparer(Func<T, T, bool> compareFunction, Func<T, int> hashFunction)
			: base ()
		{
			compareFunction.ThrowIfNull ("compareFunction");
			hashFunction.ThrowIfNull ("hashFunction");

			this.CompareFunction = compareFunction;
			this.HashFunction = hashFunction;
		}


		/// <summary>
		/// The function used to compare objects.
		/// </summary>
		public Func<T, T, bool> CompareFunction
		{
			get;
			private set;
		}


		/// <summary>
		/// The function used to get the hash of an object.
		/// </summary>
		public Func<T, int> HashFunction
		{
			get;
			private set;
		}

		
		/// <summary>
		/// Determines whether two objects of type <typeparamref name="T"/> are equal.
		/// </summary>
		/// <param name="x">The first object to compare.</param>
		/// <param name="y">The second object to compare.</param>
		/// <returns><c>true</c> if the objects are equal, <c>false</c> if they aren't.</returns>
		public override bool Equals(T x, T y)
		{
			return this.CompareFunction (x, y);
		}


		/// <summary>
		/// Gets the hash code for the given object.
		/// </summary>
		/// <param name="obj">The object whose hash code to get.</param>
		/// <returns>The hash code.</returns>
		/// <exception cref="System.ArgumentNullException">If <paramref name="obj"/> is <c>null</c>.</exception>
		public override int GetHashCode(T obj)
		{
			((object) obj).ThrowIfNull ("obj");
	
			return this.HashFunction (obj);
		}


	}


}
