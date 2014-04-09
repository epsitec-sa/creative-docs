using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Assets.Server.BusinessLogic
{
	[System.Flags]
	public enum DataCellFlags
	{
		None        = 0x0000,
		OutOfBounds = 0x0001,	// cellule hachurée
		Group       = 0x0002,	// plusieurs lignes compactées
		Locked      = 0x0004,	// antérieur à Locked (cadenas), cellule plus foncée
	}
}
