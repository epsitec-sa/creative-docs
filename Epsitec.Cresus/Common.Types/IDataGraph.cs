//	Copyright © 2004, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Statut : OK/PA, 26/04/2004

namespace Epsitec.Common.Types
{
	/// <summary>
	/// L'interface IDataGraph donne accès à un graphe complet d'éléments
	/// IDataItem (c'est en général un arbre ou une table).
	/// </summary>
	public interface IDataGraph
	{
		IDataFolder		Root		{ get; }
		
		IDataItem Navigate(string path);
		IDataCollection Select(string query);
	}
}

