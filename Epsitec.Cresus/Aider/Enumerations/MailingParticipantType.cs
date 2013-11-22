//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
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

		Contact         = 1,
		Household       = 2,
		Group           = 3,
		GroupExtraction = 4,
	}
}

