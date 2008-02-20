//	Copyright � 2005-2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.Text.Wrappers
{
	/// <summary>
	/// Une classe d�riv�e AbstractState peut �tre mise en lecture seule ou
	/// supporter un mod�le Read & Write.
	/// </summary>
	public enum AccessMode
	{
		ReadOnly,								//	accessible en lecture seulement
		ReadWrite								//	lecture & �criture (modifiable)
	}
}
