//	Copyright � 2005-2008, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.Text
{
	/// <summary>
	/// L'interface IContentsSignature donne acc�s � un m�canisme de v�rification
	/// du contenu d'un objet au moyen d'une signature (dans les faits, c'est une
	/// somme de contr�le ou CRC).
	/// </summary>
	public interface IContentsSignature
	{
		int GetContentsSignature();
	}
}
