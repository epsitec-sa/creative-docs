//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Cresus.Assets.Server.BusinessLogic;

namespace Epsitec.Cresus.Assets.App.Views
{
	public static class EnumDictionaries
	{
		//	Ici, il est préférable de ne pas avoir de mécanisme automatique pour
		//	générer les dictionnaires à partir des enumérations C#. En effet, les
		//	énumérations peuvent évoluer au cours du temps, de nouvelles valeurs
		//	peuvent être introduites. Le dictionnaire est dans un ordre logique
		//	pour l'utilisateur, qui n'est pas forcément le même que celui de
		//	l'énumération C#.

		public static Dictionary<int, string> DictPériodicités
		{
			get
			{
				var dict = new Dictionary<int, string> ();

				dict.Add ((int) Périodicité.Annuel,      "Annuel");
				dict.Add ((int) Périodicité.Semestriel,  "Semestriel");
				dict.Add ((int) Périodicité.Trimestriel, "Trimestriel");
				dict.Add ((int) Périodicité.Mensuel,     "Mensuel");

				return dict;
			}
		}

		public static Dictionary<int, string> DictTypesAmortissements
		{
			get
			{
				var dict = new Dictionary<int, string> ();

				dict.Add ((int) TypeAmortissement.Linear,     "Linéaire");
				dict.Add ((int) TypeAmortissement.Degressive, "Dégressif");

				return dict;
			}
		}
	}
}
