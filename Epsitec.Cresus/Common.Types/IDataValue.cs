//	Copyright © 2004, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Statut : OK/PA, 26/04/2004

namespace Epsitec.Common.Types
{
	/// <summary>
	/// L'interface IDataValue donne accès à une donnée dans un IDataGraph.
	/// </summary>
	public interface IDataValue : IDataItem
	{
		object ReadValue();
		void WriteValue(object value);
	}
}

