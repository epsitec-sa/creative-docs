//	Copyright � 2003, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Statut : OK/PA, 19/11/2003

namespace Epsitec.Cresus.Database
{
	/// <summary>
	/// L'interface IDbAttributesHost d�core toutes les classes qui offrent des
	/// informations sous la forme d'attributs.
	/// </summary>
	public interface IDbAttributesHost
	{
		DbAttributes		Attributes			{ get; }
	}
}
