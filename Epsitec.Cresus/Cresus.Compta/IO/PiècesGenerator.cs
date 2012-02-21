//	Copyright © 2011-2012, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using Epsitec.Common.Types;
using Epsitec.Common.Support;

using Epsitec.Cresus.Compta.Controllers;
using Epsitec.Cresus.Compta.Entities;
using Epsitec.Cresus.Compta.Helpers;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Compta.IO
{
	/// <summary>
	/// Cette classe s'occupe de générer les numéros de pièces.
	/// </summary>
	public class PiècesGenerator
	{
		public PiècesGenerator(MainWindowController mainWindowController)
		{
			this.mainWindowController = mainWindowController;
		}


		public FormattedText GetProchainePièce(ComptaJournalEntity journal)
		{
			//	Retourne le générateur de numéros de pièces à utiliser.
			return this.GetProchainePièce (this.mainWindowController.CurrentUser, this.mainWindowController.Période, journal);
		}

		private FormattedText GetProchainePièce(ComptaUtilisateurEntity utilisateur, ComptaPériodeEntity période, ComptaJournalEntity journal)
		{
			//	Retourne le générateur de numéros de pièces à utiliser.
			var pièce = this.GetPièceEntity (utilisateur, période, journal);

			if (pièce == null)
			{
				return FormattedText.Empty;
			}
			else
			{
				return this.GetProchainePièce (pièce);
			}
		}

		private ComptaPièceEntity GetPièceEntity(ComptaUtilisateurEntity utilisateur, ComptaPériodeEntity période, ComptaJournalEntity journal)
		{
			//	Retourne le générateur de numéros de pièces à utiliser.
			//	TODO: Priorités à revoir éventuellement ?
			if (journal != null && journal.GénérateurDePièces != null)
			{
				return journal.GénérateurDePièces;
			}

			if (période != null && période.GénérateurDePièces != null)
			{
				return période.GénérateurDePièces;
			}

			if (utilisateur != null && utilisateur.GénérateurDePièces != null)
			{
				return utilisateur.GénérateurDePièces;
			}

			return this.mainWindowController.Compta.Pièces.FirstOrDefault ();
		}

		private FormattedText GetProchainePièce(ComptaPièceEntity pièce)
		{
			//	Retourne le prochain numéro de pièce à utiliser.
			int n = this.GetPièceProchainNuméro (pièce);
			string s = n.ToString (System.Globalization.CultureInfo.InvariantCulture);

			if (pièce.Digits != 0 && pièce.Digits > s.Length)
			{
				s = new string ('0', pièce.Digits - s.Length) + s;
			}

			if (!pièce.SépMilliers.IsNullOrEmpty)
			{
				s = Strings.AddSépMilliers (s, pièce.SépMilliers.ToSimpleText ());
			}

			s = pièce.Préfixe + s + pièce.Suffixe;

			return s;
		}

		private int GetPièceProchainNuméro(ComptaPièceEntity pièce)
		{
			//	Retourne le prochain numéro de pièce à utiliser.
			//	TODO: Il faudra vérifier que cette procédure fonctionne en multi-utilisateur !
			int n = pièce.Numéro;
			pièce.Numéro += pièce.Incrément;
			return n;
		}


		private readonly MainWindowController		mainWindowController;
	}
}
