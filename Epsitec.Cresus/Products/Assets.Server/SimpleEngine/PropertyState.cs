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
		InputValue,		// propriété pas encore définie, mais dont on donne la valeur lors de l'événement d'entrée
		Inherited,		// pas utilisé
		OneShot,		// propriété sans influence sur les événements futurs
		Timeless,		// propriété intemporelle
		Readonly,		// pas d'événement à la date choisie
	}
}