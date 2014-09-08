//	Copyright � 2014, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Types;

namespace Epsitec.Aider.Enumerations
{
	[DesignerVisible]
	public enum OfficeType
	{
		None = 0,

		Parish			= 1,
		
		Region			= 2,					//	global - r�gion proprement dite
		RegionFA		= 3,					//	Formation & accompagnement
		RegionPS		= 4,					//	Pr�sence & solidarit�
		RegionCI		= 5,					//	Coordination & information
		
		CantonOffice	= 11,					//	4 offices cantonaux
		CantonService	= 12,					//	4 services (SFA, STN, SSAS, SVCC)
		CantonLieuPhare	= 13,					//	3 lieux-phares
		CantonMiCo		= 14,					//	11 missions communes
	}
}

