//	Copyright © 2010-2011, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Types;

namespace Epsitec.Cresus.Core.Business
{
	/// <summary>
	/// The <c>WorkflowState</c> enumeration defines the states in which a workflow can be.
	/// </summary>
	[DesignerVisible]
	public enum WorkflowState
	{
		None			= 0,

		Pending			= 1,
		Active			= 2,
		Done			= 3,

		Cancelled		= 4,
		TimedOut		= 5,

		Restricted		= 6,
	}
}
