using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Assets.Server.BusinessLogic
{
	[System.Flags]
	public enum DataCellFlags
	{
		None        = 0x0000,
		OutOfBounds = 0x0001,	// cellule hachur�e
		Group       = 0x0002,	// plusieurs lignes compact�es
		Locked      = 0x0004,	// ant�rieur � Locked (cadenas), cellule plus fonc�e
	}
}
