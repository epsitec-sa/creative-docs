//	Copyright © 2013-2014, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Types;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Aider.Enumerations
{
	[DesignerVisible]
	public enum MailingParticipantType
	{
		[Hidden]
		None = 0,

		Contact         = 1,		//	C
		Household       = 2,		//	H
		Group           = 3,		//	G
		GroupExtraction = 4,		//	T --> "Transversal"
	}
}

