//	Copyright © 2004, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Statut : OK/PA, 29/04/2004

namespace Epsitec.Common.Types
{
	/// <summary>
	/// L'inteface IReadOnly permet de déterminer si un objet est accessible en
	/// lecture seule, ou non.
	/// </summary>
	public interface IReadOnly
	{
		bool	IsReadOnly		{ get; }
	}
}
