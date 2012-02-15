//	Copyright © 2011, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using Epsitec.Common.Types;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Compta.Memory.Data
{
	public enum ShowPanelMode
	{
		DoesNotExist,		// le panneaux correspondant n'existe pas
		Nop,
		Hide,
		ShowBeginner,
		ShowSpecialist,
	}
}
