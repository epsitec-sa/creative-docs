//	Copyright © 2004, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.Types
{
	/// <summary>
	/// L'inteface INullable permet de déterminer si un objet (ValueType) est
	/// nul (NULL) ou non.
	/// </summary>
	public interface INullable
	{
		bool	IsNull		{ get; }
	}
}
