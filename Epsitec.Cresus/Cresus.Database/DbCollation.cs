namespace Epsitec.Cresus.Database
{
	
	
	public enum DbCollation : byte
	{

		/// <summary>
		/// This collation sorts in ascii byte order and considers that every characters are
		/// different.
		/// </summary>
		Ascii = 1,
		
		
		/// <summary>
		/// This collation sorts in unicode code-point order and considers that every characters are
		/// different.
		/// </summary>
		UcsBasic = 2,


		/// <summary>
		/// This collation uses the Unicode Collation Algorithm.
		/// </summary>
		Unicode = 3,


		/// <summary>
		/// This collation is like Unicode but case insensitive.
		/// </summary>
		UnicodeCi = 4,


		/// <summary>
		/// This collation is like Unicode but case insensitive and accent isensitive.
		/// </summary>
		UnicodeCiAi = 5,


	}


}