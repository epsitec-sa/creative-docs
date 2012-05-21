using System.Collections.Generic;


namespace Epsitec.Common.Support
{


	/// <summary>
	/// The <c>ArrayEqualityComparer{T}</c> implements comparison and hashing functions for generic
	/// arrays.
	/// </summary>
	/// <typeparam name="T">The type of the arrays that will be dealt by this class.</typeparam>
	public sealed class ArrayEqualityComparer<T> : EqualityComparer<T[]>
	{


		/// <summary>
		/// Builds a new instance of <c>ArrayEqualityComparer{T}</c>.
		/// </summary>
		private ArrayEqualityComparer() : base ()
		{
		}


		/// <summary>
		/// Gets the hash code for the given object.
		/// </summary>
		/// <param name="obj">The object whose hash code to get.</param>
		/// <returns>The hash code.</returns>
		public override int GetHashCode(T[] obj)
		{
			int result = 0;

			if (obj != null)
			{
				result = 1000000007;

				result = 37 * result + obj.Length.GetHashCode ();

				for (int i = 0; i < obj.Length; i++)
				{
					int c = (obj[i] != null) ? obj[i].GetHashCode () : 0;

					result = 37 * result + c;
				}
			}

			return result;
		}


		/// <summary>
		/// Determines whether two objects of type <see cref="System.Array{T}"/> are equal.
		/// </summary>
		/// <param name="x">The first object to compare.</param>
		/// <param name="y">The second object to compare.</param>
		/// <returns><c>true</c> if the objects are equal, <c>false</c> if they don't.</returns>
		public override bool Equals(T[] x, T[] y)
		{
			bool different = x == null || y == null || x.Length != y.Length;

			for (int i = 0; !different && i < x.Length; i++)
			{
				different = !x[i].Equals (y[i]);
			}

			return !different;
		}


		/// <summary>
		/// Static constructor for the <c>ArrayEqualityComparer{T}</c> class.
		/// </summary>
		static ArrayEqualityComparer()
		{
			ArrayEqualityComparer<T>.instance = new ArrayEqualityComparer<T> ();
		}


		/// <summary>
		/// Gets an instance of the <c>ArrayEqualityComparer{T}</c> class.
		/// </summary>
		public static ArrayEqualityComparer<T> Instance
		{
			get
			{
				return ArrayEqualityComparer<T>.instance;
			}
		}


		/// <summary>
		/// An instance of the <c>ArrayEqualityComparer{T}</c> class ready to be used.
		/// </summary>
		private static readonly ArrayEqualityComparer<T> instance;


	}


}
