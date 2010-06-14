//	Copyright © 2004-2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

namespace Epsitec.Cresus.Database
{
	/// <summary>
	/// The <c>DbCompareCombiner</c> enumeration specifies how comparisons
	/// are combined together (logical and/logical or).
	/// </summary>
	public enum DbConditionCombinerOperator : byte
	{
		/// <summary>
		/// Combine using logical and.
		/// </summary>
		And,
		
		/// <summary>
		/// Combine using logical or.
		/// </summary>
		Or,


	}
}
