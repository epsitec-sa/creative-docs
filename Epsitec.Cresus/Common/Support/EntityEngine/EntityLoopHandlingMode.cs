//	Copyright © 2007-2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

namespace Epsitec.Common.Support.EntityEngine
{
	/// <summary>
	/// The <c>EntityLoopHandlingMode</c> enumeration specifies how <see cref="EntityContext"/>
	/// handles loops when creating an entity graph.
	/// </summary>
	public enum EntityLoopHandlingMode
	{
		/// <summary>
		/// Throws an exception if a loop is encountered.
		/// </summary>
		Throw,
		
		/// <summary>
		/// Skips fields which would lead to a loop.
		/// </summary>
		Skip,
	}
}
