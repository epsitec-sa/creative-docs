//	Copyright � 2003, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Statut : en chantier/PA

namespace Epsitec.Cresus.DataLayer
{
	/// <summary>
	/// L'interface IDataAttributesHost d�core toutes les classes qui offrent des
	/// informations sous la forme d'attributs.
	/// </summary>
	public interface IDataAttributesHost
	{
		DataAttributes		DataAttributes		{ get; }
	}
}
