//	Copyright © 2015, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Types;

namespace Epsitec.Aider.Enumerations
{
	[DesignerVisible]
	public enum PersonMrMrsTitle
	{
		Auto = 0,

		ConseillerEtat_Monsieur	= 0x011,
		ConseillerEtat_Madame	= 0x012,
		Prefet_Monsieur			= 0x021,
		Prefet_Madame			= 0x022,
		ChefService_Monsieur	= 0x031,
		ChefService_Madame		= 0x032,
		Directeur_Monsieur		= 0x041,
		Directeur_Madame		= 0x042,
		Président_Monsieur		= 0x051,
		Président_Madame		= 0x052,

		Recteur_Monsieur		= 0x101,
		Recteur_Madame			= 0x102,
		Doyen_Monsieur			= 0x111,
		Doyen_Madame			= 0x112,

		Maitre					= 0x203,

		Procureur_Monsieur		= 0x211,
		Procureur_Madame		= 0x212,
		Juge_Monsieur			= 0x221,
		Juge_Madame				= 0x222,

		Pasteur_Monsieur		= 0x301,
		Pasteur_Madame			= 0x302,
		Diacre_Monsieur			= 0x311,
		Diacre_Madame			= 0x312,

		Eveque_Monsieur			= 0x401,
		Cure_Monsieur			= 0x411,
		Abbe_Monsieur			= 0x421,
		Pere_Monsieur			= 0x431,
		Reverend_Monsieur		= 0x441,
		ReverendPere_Monsieur	= 0x451,

		Frere_Monsieur			= 0x461,
		Soeur_Madame			= 0x462,

		Colonel_Monsieur		= 0x501,
		Brigadier_Monsieur		= 0x511,
		Divisionnaire_Monsieur	= 0x521,
		CdtCorps_Monsieur		= 0x531,
		ChefArmee_Monsieur		= 0x541,
	}
}

