//	Copyright © 2005, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.Text
{
	/// <summary>
	/// L'interface IContentsSignatureUpdater permet de calculer un CRC sur le
	/// contenu d'une classe.
	/// </summary>
	public interface IContentsSignatureUpdater
	{
		void UpdateContentsSignature(IO.IChecksum checksum);
	}
}
