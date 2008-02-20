//	Copyright � 2004-2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Cresus.Requests
{
	/// <summary>
	/// L'�num�ration RequestType d�finit le type des requ�tes courantes.
	/// </summary>
	public enum RequestType
	{
		Unknown = 0,
		
		Group,										//	groupe de requ�tes
		
		InsertStaticData,							//	requ�te INSERT, donn�es statiques
		
		UpdateStaticData,							//	requ�te UPDATE, donn�es statiques
		UpdateDynamicData,							//	requ�te UPDATE, donn�es dynamiques (calcul�es)
	}
}
