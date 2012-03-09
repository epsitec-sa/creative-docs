//	Copyright � 2011, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using Epsitec.Common.Types;
using Epsitec.Common.Widgets;

using Epsitec.Cresus.Compta.Entities;
using Epsitec.Cresus.Compta.Accessors;
using Epsitec.Cresus.Compta.Controllers;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Compta.Helpers
{
	public static class TVA
	{
		public static FormattedText GetShortError(IEnumerable<ComptaTauxTVAEntity> tauxEntities)
		{
			//	Retourne l'erreur courte �ventuelle li�e � une liste de taux.
			var error = TVA.GetError (tauxEntities);

			if (error.IsNullOrEmpty)
			{
				return FormattedText.Empty;  // ok
			}
			else
			{
				return FormattedText.Format ("Erreur").ApplyBold ();
			}
		}

		public static FormattedText GetError(IEnumerable<ComptaTauxTVAEntity> tauxEntities)
		{
			//	Retourne l'erreur �ventuelle li�e � une liste de taux.
			if (tauxEntities.Count () == 0)
			{
				return FormattedText.Format ("Aucun taux").ApplyBold ();
			}

			//	V�rifie si un et un seul taux a l'option "par d�faut".
			int defauntCount = tauxEntities.Where (x => x.ParD�faut).Count ();
			if (defauntCount == 0)
			{
				return FormattedText.Format ("Pas de taux par d�faut").ApplyBold ();
			}
			if (defauntCount > 1)
			{
				return FormattedText.Format ("Plusieurs taux par d�faut").ApplyBold ();
			}

			//	V�rifie s'il y a des collisions de dates entre les taux.
			foreach (var tauxEntity1 in tauxEntities)
			{
				foreach (var tauxEntity2 in tauxEntities)
				{
					if (tauxEntity1 != tauxEntity2)
					{
						if (tauxEntity1.DateD�but.HasValue)
						{
							if (Dates.DateInRange (tauxEntity1.DateD�but, tauxEntity2.DateD�but, tauxEntity2.DateFin))
							{
								return FormattedText.Format ("Collision de dates").ApplyBold ();
							}
						}

						if (tauxEntity1.DateFin.HasValue)
						{
							if (Dates.DateInRange (tauxEntity1.DateFin, tauxEntity2.DateD�but, tauxEntity2.DateFin))
							{
								return FormattedText.Format ("Collision de dates").ApplyBold ();
							}
						}
					}
				}
			}

			return FormattedText.Empty;  // ok
		}
	}
}
