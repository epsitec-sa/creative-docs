//	Copyright � 2004, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Cresus.Database.Requests
{
	/// <summary>
	/// L'�num�ration Type d�finit le type des requ�tes courantes.
	/// </summary>
	public enum Type
	{
		Unknown = 0,
		
		Group,										//	groupe de requ�tes
		
		InsertStaticData,							//	requ�te INSERT, donn�es statiques
		
		UpdateStaticData,							//	requ�te UPDATE, donn�es statiques
		UpdateDynamicData,							//	requ�te UPDATE, donn�es dynamiques (calcul�es)
	}
}
