//	Copyright © 2005, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.Text.Properties
{
	/// <summary>
	/// L'énumération PropertyType définit les divers types de propriétés
	/// supportées.
	/// </summary>
	public enum PropertyType
	{
		Invalid			= 0,			//	définition non valide
		
		Style			= 1,			//	définition pour un style
		LocalSetting	= 2,			//	définition pour un réglage local
		ExtraSetting	= 3,			//	définition pour un réglage spécial
	}
}
