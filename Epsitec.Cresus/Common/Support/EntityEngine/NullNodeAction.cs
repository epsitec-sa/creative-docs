//	Copyright © 2012, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Common.Support.EntityEngine
{
	/// <summary>
	/// The <c>NullNodeAction</c> enumeration defines what should be done when a <c>null</c>
	/// node is reached while walking a graph.
	/// </summary>
	public enum NullNodeAction
	{
		ReturnNull,
		CreateMissing,
	}
}
