//	Copyright � 2005, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.Text.Properties
{
	/// <summary>
	/// L'�num�ration PropertyType d�finit les divers types de propri�t�s
	/// support�es. Ces types servent uniquement � classer en trois cat�gories
	/// les propri�t�s; cela n'a aucun rapport avec leur "affinit�".
	/// 
	/// Voir aussi Properties.PropertyAffinity.
	/// </summary>
	public enum PropertyType
	{
		Invalid			= 0,			//	d�finition non valide
		
		Style			= 1,			//	d�finition pour un style
		LocalSetting	= 2,			//	d�finition pour un r�glage local
		ExtraSetting	= 3,			//	d�finition pour un r�glage sp�cial
	}
}
