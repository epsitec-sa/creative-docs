//	Copyright © 2005-2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Responsable: Michael Walz

namespace Epsitec.Common.Text.Exchange
{
	/// <summary>
	/// Mode de collage.
	/// </summary>
	public enum PasteMode
	{
		Unknown,						//	mode inconnu
		KeepSource,						//	insertion du texte avec tous les attributs de mise en forme
		MatchDestination,				//  insertion du texte en gardant juste quelques attributs comme gras, italique, souligné,... [A definir]
		KeepTextOnly,					//	insertion du texte brut sans formattage
	}
}
