//	Copyright © 2007-2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

namespace Epsitec.Common.Support.EntityEngine
{
	/// <summary>
	/// The <c>EntityDataVersion</c> enumeration lists the different data versions
	/// possible for an <see cref="AbstractEntity"/>.
	/// </summary>
	public enum EntityDataVersion
	{
		/// <summary>
		/// The original data in the entity.
		/// </summary>
		Original,
		
		/// <summary>
		/// The modified data in the entity.
		/// </summary>
		Modified,
		//Proposed
	}
}
