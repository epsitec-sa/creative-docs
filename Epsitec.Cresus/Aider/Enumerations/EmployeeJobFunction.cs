//	Copyright � 2014-2019, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Types;

namespace Epsitec.Aider.Enumerations
{
	[DesignerVisible]
	public enum EmployeeJobFunction
	{
		Other		 = 0,

		Titulaire			= 1,
		Suffragant			= 2,
		Stagiaire			= 3,
		Vicaire				= 4,
		Rempla�ant			= 5,
		Auxiliaire			= 6,

		UtilisateurAIDER    = 20,
		GestionnaireAIDER   = 21,
		Suppl�antAIDER      = 22,

		[Hidden]
		PublicJob			= 99,
	}
}
