//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Types;

namespace Epsitec.Cresus.Core.BusinessLogic
{
	[DesignerVisible]
	public enum EnumValueCardinality
	{
		ZeroOrOne	= 0,
		ExactlyOne	= 1,
		AtLeastOne	= 2,
		Any			= 3,
	}
}
