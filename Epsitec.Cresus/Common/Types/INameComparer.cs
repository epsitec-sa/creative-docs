using System.Collections.Generic;


namespace Epsitec.Common.Types
{


	/// <summary>
	/// The <c>INameComparer</c> class implements comparison and hashing functions for objects
	/// deriving from the <see cref="IName"/> interface based on their name.
	/// </summary>
	public sealed class INameComparer : EqualityComparer<IName>
	{

		
		/// <summary>
		/// Builds a new instance of <c>INameComparer</c>.
		/// </summary>
		private INameComparer() : base ()
		{
		}


		/// <summary>
		/// Determines whether two objects of type <see cref="IName"/> have the same name.
		/// </summary>
		/// <param name="x">The first object to compare.</param>
		/// <param name="y">The second object to compare.</param>
		/// <returns><c>true</c> if the objects have the same name, <c>false</c> if they don't.</returns>
		public override bool Equals(IName x, IName y)
		{
			return x != null
				&& y != null
				&& x.Name != null
				&& y.Name != null
				&& string.Equals (x.Name, y.Name, System.StringComparison.InvariantCulture);
		}


		/// <summary>
		/// Gets the hash code for the given object.
		/// </summary>
		/// <param name="obj">The object whose hash code to get.</param>
		/// <returns>The hash code.</returns>
		public override int GetHashCode(IName obj)
		{
			int hash = 0;

			if (obj != null && obj.Name != null)
			{
				hash = obj.Name.GetHashCode ();
			}

			return hash;
		}


		/// <summary>
		/// Static constructor for the <c>INameComparer</c> class.
		/// </summary>
		static INameComparer()
		{
			INameComparer.instance = new INameComparer ();
		}


		/// <summary>
		/// Gets an instance of the <c>INameComparer</c> class.
		/// </summary>
		public static INameComparer Instance
		{
			get
			{
				return INameComparer.instance;
			}
		}


		/// <summary>
		/// An instance of the <c>INameComparer</c> class ready to be used.
		/// </summary>
		private static readonly INameComparer instance;


	}


}
