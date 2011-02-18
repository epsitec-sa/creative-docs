//	Copyright © 2005-2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
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
