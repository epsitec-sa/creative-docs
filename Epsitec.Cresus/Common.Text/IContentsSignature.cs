//	Copyright © 2005, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.Text
{
	/// <summary>
	/// L'interface IContentsSignature donne accès à un mécanisme de vérification
	/// du contenu d'un objet au moyen d'une signature (dans les faits, c'est une
	/// somme de contrôle ou CRC).
	/// </summary>
	public interface IContentsSignature
	{
		int GetContentsSignature();
	}
}
