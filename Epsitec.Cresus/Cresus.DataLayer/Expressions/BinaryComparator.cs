namespace Epsitec.Cresus.DataLayer.Expressions
{


	/// <summary>
	/// The <c>BinaryComparator</c> enumeration defines the possible arithmetic and string comparison
	/// that can be made to compare two values.
	/// </summary>
	public enum BinaryComparator
	{

		
		/// <summary>
		/// The arithmetic equality comparison.
		/// </summary>
		IsEqual,


		/// <summary>
		///  The arithmetic inequality comparison.
		/// </summary>
		IsNotEqual,

		
		/// <summary>
		/// The arithmetic lower comparison.
		/// </summary>
		IsLower,

		
		/// <summary>
		/// The arithmetic lower or equal comparison.
		/// </summary>
		IsLowerOrEqual,

		
		/// <summary>
		/// The arithmetic greater comparison.
		/// </summary>
		IsGreater,
		
	
		/// <summary>
		/// The arithmetic greater or equal comparison.
		/// </summary>
		IsGreaterOrEqual,


		/// <summary>
		/// The comparison that checks if a string is like another one, including the wildcards.
		/// </summary>
		IsLike,


		/// <summary>
		/// The comparison that checks if a string is not like another one, including the wildcards.
		/// </summary>
		IsNotLike,


		/// <summary>
		/// The comparison that checks if a string is like another one, including the wildcards, but
		/// escaping all special chars before.
		/// </summary>
		IsLikeEscape,


		/// <summary>
		/// The comparison that checks if a string is not like another one, including the wildcards,
		/// but escaping all special chars before.
		/// </summary>
		IsNotLikeEscape,


	}


}
