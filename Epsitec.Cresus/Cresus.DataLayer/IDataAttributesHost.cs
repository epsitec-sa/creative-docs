//	Copyright � 2003, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Statut : OK/PA, 13/11/2003

namespace Epsitec.Cresus.DataLayer
{
	/// <summary>
	/// L'interface IDataAttributesHost d�core toutes les classes qui offrent des
	/// informations sous la forme d'attributs.
	/// </summary>
	public interface IDataAttributesHost
	{
		DataAttributes		Attributes			{ get; }
	}
}
