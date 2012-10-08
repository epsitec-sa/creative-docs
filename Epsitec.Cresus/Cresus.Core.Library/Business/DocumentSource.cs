//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Types;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Business
{
	[DesignerVisible]
	public enum DocumentSource
	{
		None		= 0,

		Generated	= 1,
		Internal	= 2,
		External	= 3,
	}
}
