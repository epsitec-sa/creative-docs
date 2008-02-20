//	Copyright © 2006-2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

namespace Epsitec.Cresus.Database
{
	/// <summary>
	/// The <c>SqlAggregateFunction</c> enumeration defines the supported aggregate
	/// functions.
	/// </summary>
	public enum SqlAggregateFunction
	{
		/// <summary>
		/// Unknown function.
		/// </summary>
		Unknown,

		/// <summary>
		/// Count function.
		/// </summary>
		Count,
		
		/// <summary>
		/// Min function.
		/// </summary>
		Min,
		
		/// <summary>
		/// Max function.
		/// </summary>
		Max,
		
		/// <summary>
		/// Average function.
		/// </summary>
		Average,
		
		/// <summary>
		/// Sum function.
		/// </summary>
		Sum,
	}
}
