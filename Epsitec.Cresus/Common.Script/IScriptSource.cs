//	Copyright � 2004, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.Script
{
	/// <summary>
	/// L'interface IScriptSource d�finit les m�thodes de base permettant
	/// d'interagir avec un �diteur de scripts.
	/// </summary>
	public interface IScriptSource
	{
		Source		Source		{ get; set; }
		string		Name		{ get; set; }
	}
}
