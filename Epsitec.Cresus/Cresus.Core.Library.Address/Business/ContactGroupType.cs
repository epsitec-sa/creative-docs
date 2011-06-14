//	Copyright © 2011, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Types;

namespace Epsitec.Cresus.Core.Business
{
	[DesignerVisible]
	public enum ContactGroupType
	{
		Unknown						= -1,
		None						=  0,

		Billing						= 10,
		Shipping					= 20,
		Return						= 30,
	}
}
