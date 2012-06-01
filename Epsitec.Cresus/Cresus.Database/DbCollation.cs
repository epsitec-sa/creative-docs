namespace Epsitec.Cresus.Database
{
	
	
	public enum DbCollation
	{

		/// <summary>
		/// This collation sorts in ascii byte order and considers that every characters are
		/// different.
		/// </summary>
		Ascii,
		
		
		/// <summary>
		/// This collation sorts in unicode code-point order and considers that every characters are
		/// different.
		/// </summary>
		UcsBasic,


		/// <summary>
		/// This collation uses the Unicode Collation Algorithm.
		/// </summary>
		Unicode,


		/// <summary>
		/// This collation is like Unicode but case insensitive.
		/// </summary>
		UnicodeCi,


		/// <summary>
		/// This collation is like Unicode but case insensitive and accent isensitive.
		/// </summary>
		UnicodeCiAi,


	}


}