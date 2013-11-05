//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Epsitec.Cresus.Assets.Server.SimpleEngine
{
	public enum PropertyState
	{
		Undefined,
		Single,			// propriété définie directement dans l'événement
		Synthetic,		// propriété définie dans un événement précédent
		Inherited,		// pas utilisé
		Singleton,		// propriété sans influence sur les événements futurs
		Readonly,		// pas d'événement à la date choisie
		//?Locked,			// hors des périodes définies par les événements d'entrée/sortie
	}
}