//	Copyright � 2003, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Statut : OK/PA, 22/10/2003

namespace Epsitec.Cresus.Database
{
	/// <summary>
	/// L'�num�ration DbElementCat d�finit les cat�gories des �l�ments
	/// (table, colonne) de la base.
	/// </summary>
	public enum DbElementCat
	{
		Unsupported			= 0,	//	cat�gorie non support�e
		Unknown = Unsupported,		//	cat�gorie inconnue (= non support�e)
		
		Internal			= 1,	//	�l�ment � usage interne
		UserDataManaged		= 2,	//	�l�ment sous contr�le de l'utilisateur, g�r� par Cr�sus
		UserDataExternal	= 3,	//	�l�ment sous contr�le de l'utilisateur, non g�r� (source externe)
	}
}
