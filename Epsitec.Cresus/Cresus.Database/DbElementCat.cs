//	Copyright � 2003-2004, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Cresus.Database
{
	/// <summary>
	/// L'�num�ration DbElementCat d�finit les cat�gories des �l�ments
	/// (table, colonne) de la base.
	/// </summary>
	public enum DbElementCat : byte
	{
		Unsupported			= 0,		//	cat�gorie non support�e
		Unknown = Unsupported,			//	cat�gorie inconnue (= non support�e)
		
		Internal			= 1,		//	�l�ment � usage interne
		UserDataManaged		= 2,		//	�l�ment sous contr�le de l'utilisateur, g�r� par Cr�sus
		UserDataExternal	= 3,		//	�l�ment sous contr�le de l'utilisateur, non g�r� (source externe)
		
		Synthetic			= 4,		//	�l�ment synth�tique (n'existe pas en tant que tel dans la base)
		Any					= 5,		//	n'importe (utilisable uniquement dans les crit�res d'extraction)
	}
}
