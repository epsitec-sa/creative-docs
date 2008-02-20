//	Copyright � 2004-2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.Types
{
	/// <summary>
	/// L'interface IDataGraph donne acc�s � un graphe complet d'�l�ments
	/// IDataItem (c'est en g�n�ral un arbre ou une table).
	/// </summary>
	public interface IDataGraph
	{
		IDataFolder		Root		{ get; }
		
		IDataItem Navigate(string path);
		IDataCollection Select(string query);
	}
}

