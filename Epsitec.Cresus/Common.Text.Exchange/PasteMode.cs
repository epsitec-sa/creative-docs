//	Copyright © 2005, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Michael Walz

namespace Epsitec.Common.Text.Exchange
{
	/// <summary>
	/// Mode de collage.
	/// </summary>
	public enum PasteMode
	{
		Unknown,						//	mode inconnu

		InsertRaw,						//	insertion du texte brut sans formattage
		InsertAll,						//	insertion du texte avec tous les attributs de mise en forme
		InsertNoStyle,					//  insertion du texte avec tous les attributs de mise en forme locaux (sans les styles)
	}
}
