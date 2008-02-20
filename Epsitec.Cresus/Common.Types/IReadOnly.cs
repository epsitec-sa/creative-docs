//	Copyright © 2004-2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
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
