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
		KeepTextOnly,					//	insertion du texte brut sans formattage
		KeepSource,						//	insertion du texte avec tous les attributs de mise en forme
		MatchDestination,				//  insertion du texte en gardant juste quelques attributs comme gras, italique, souligné,... [A definir]
	}
}
