using Epsitec.Common.Drawing;
using System.Collections.Generic;

namespace Epsitec.Common.Pdf
{
	public enum ColorForce
	{
		Default,		// color space selon RichColor.ColorSpace
		Nothing,		// aucune commande de couleur
		Rgb,			// force le color space RGB
		Cmyk,			// force le color space CMYK
		Gray,			// force le color space Gray
	}
}
