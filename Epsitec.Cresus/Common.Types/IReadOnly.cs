//	Copyright © 2004-2006, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.Types
{
	/// <summary>
	/// L'inteface IReadOnly permet de déterminer si un objet est accessible en
	/// lecture seule, ou non.
	/// </summary>
	public interface IReadOnly
	{
		bool IsReadOnly
		{
			get;
		}
	}
}
