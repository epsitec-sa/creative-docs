//	Copyright � 2003-2004, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

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
