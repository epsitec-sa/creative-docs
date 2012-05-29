//	Copyright © 2006-2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD


namespace Epsitec.Cresus.Database
{
	
	
	/// <summary>
	/// The <c>SqlJoinCode</c> enumeration defines all SQL joins.
	/// </summary>
	public enum SqlJoinCode
	{
		
		/// <summary>
		/// Inner join.
		/// </summary>
		Inner,
		
		
		/// <summary>
		/// Left outer join.
		/// </summary>
		OuterLeft,

		
		/// <summary>
		/// Right outer join.
		/// </summary>
		OuterRight,

	
		/// <summary>
		/// Full outer join.
		/// </summary>
		OuterFull,

		
		/// <summary>
		/// Cross join.
		/// </summary>
		Cross,
	
	
	}


}
