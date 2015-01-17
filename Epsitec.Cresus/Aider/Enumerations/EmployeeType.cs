//	Copyright © 2014, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Types;

namespace Epsitec.Aider.Enumerations
{
	[DesignerVisible]
	public enum EmployeeType
	{
		None		= 0,

		Pasteur				= 1,
		Diacre				= 2,
		Employee			= 3,
		
		AnimateurEglise		= 4,
		AnimateurParoisse	= 5,

		BenevoleAIDER       = 90,
		Other				= 99,
		
		[Hidden]
		PublicEmployee		= 999,
	}
}
