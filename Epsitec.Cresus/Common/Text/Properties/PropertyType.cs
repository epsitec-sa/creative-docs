//	Copyright © 2005-2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.Text.Properties
{
	/// <summary>
	/// L'énumération PropertyType définit les divers types de propriétés
	/// supportées. Ces types servent uniquement à classer en trois catégories
	/// les propriétés; cela n'a aucun rapport avec leur "affinité".
	/// 
	/// Voir aussi Properties.PropertyAffinity.
	/// </summary>
	public enum PropertyType
	{
		Invalid			= 0,			//	définition non valide
		
		CoreSetting		= 1,			//	définition de base
		LocalSetting	= 2,			//	définition pour un réglage local
		ExtraSetting	= 3,			//	définition pour un réglage spécial
		
		Polymorph		= 4,			//	définition soit Style, soit Extra; utilisé par StylesProperty
	}
}
