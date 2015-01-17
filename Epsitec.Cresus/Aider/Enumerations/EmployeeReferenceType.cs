//	Copyright © 2014, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Types;

namespace Epsitec.Aider.Enumerations
{
	[DesignerVisible]
	public enum EmployeeReferenceType
	{
		Other		 = 0,

		Childhood		= 1,
		Youth			= 2,
		Adults			= 3,

		TerreNouvelle	= 10,

		InfoCom			= 20,
		Coordinator		= 21,

		[Hidden]
		PublicReferee   = 99,
	}
}
