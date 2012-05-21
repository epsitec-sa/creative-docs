//	Copyright © 2007-2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

namespace Epsitec.Common.Support.EntityEngine
{
	/// <summary>
	/// The <c>EntityDataState</c> enumeration describes the state of an
	/// <see cref="AbstractEntity"/>.
	/// </summary>
	public enum EntityDataState
	{
		/// <summary>
		/// The entity contains original data only.
		/// </summary>
		Unchanged,
		
		/// <summary>
		/// The entity contains modified data. This means that it was edited
		/// and is no longer in sync with the original data source.
		/// </summary>
		Modified
	}
}
