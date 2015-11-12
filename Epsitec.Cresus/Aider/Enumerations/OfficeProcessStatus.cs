//	Copyright © 2014-2015, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Samuel LOUP, Maintainer: Samuel LOUP

using Epsitec.Common.Types;

namespace Epsitec.Aider.Enumerations
{
	[DesignerVisible]
	public enum OfficeProcessStatus
	{
		Started = 0,    // All tasks is not done

		InProgress = 1, // One of X tasks is done

		Canceled = 2,   // A new process must be instanciated

		Ended = 3,      // All tasks is done
	}
}

