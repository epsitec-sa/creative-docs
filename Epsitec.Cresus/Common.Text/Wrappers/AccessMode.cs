//	Copyright � 2005, EPSITEC SA, CH-1092 BELMONT, Switzerland
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
