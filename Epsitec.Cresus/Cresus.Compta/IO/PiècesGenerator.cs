//	Copyright � 2011-2012, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
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
	/// Cette classe s'occupe de g�n�rer les num�ros de pi�ces.
	/// </summary>
	public class Pi�cesGenerator
	{
		public Pi�cesGenerator(MainWindowController mainWindowController)
		{
			this.mainWindowController = mainWindowController;
		}


		public FormattedText GetProchainePi�ce(ComptaJournalEntity journal)
		{
			//	Retourne le g�n�rateur de num�ros de pi�ces � utiliser.
			return this.GetProchainePi�ce (this.mainWindowController.CurrentUser, this.mainWindowController.P�riode, journal);
		}

		private FormattedText GetProchainePi�ce(ComptaUtilisateurEntity utilisateur, ComptaP�riodeEntity p�riode, ComptaJournalEntity journal)
		{
			//	Retourne le g�n�rateur de num�ros de pi�ces � utiliser.
			var pi�ce = this.GetPi�ceEntity (utilisateur, p�riode, journal);

			if (pi�ce == null)
			{
				return FormattedText.Empty;
			}
			else
			{
				return this.GetProchainePi�ce (pi�ce);
			}
		}

		private ComptaPi�ceEntity GetPi�ceEntity(ComptaUtilisateurEntity utilisateur, ComptaP�riodeEntity p�riode, ComptaJournalEntity journal)
		{
			//	Retourne le g�n�rateur de num�ros de pi�ces � utiliser.
			//	TODO: Priorit�s � revoir �ventuellement ?
			if (journal != null && journal.G�n�rateurDePi�ces != null)
			{
				return journal.G�n�rateurDePi�ces;
			}

			if (p�riode != null && p�riode.G�n�rateurDePi�ces != null)
			{
				return p�riode.G�n�rateurDePi�ces;
			}

			if (utilisateur != null && utilisateur.G�n�rateurDePi�ces != null)
			{
				return utilisateur.G�n�rateurDePi�ces;
			}

			return this.mainWindowController.Compta.Pi�ces.FirstOrDefault ();
		}

		private FormattedText GetProchainePi�ce(ComptaPi�ceEntity pi�ce)
		{
			//	Retourne le prochain num�ro de pi�ce � utiliser.
			int n = this.GetPi�ceProchainNum�ro (pi�ce);
			string s = n.ToString (System.Globalization.CultureInfo.InvariantCulture);

			if (pi�ce.Digits != 0 && pi�ce.Digits > s.Length)
			{
				s = new string ('0', pi�ce.Digits - s.Length) + s;
			}

			if (!pi�ce.S�pMilliers.IsNullOrEmpty)
			{
				s = Strings.AddS�pMilliers (s, pi�ce.S�pMilliers.ToSimpleText ());
			}

			s = pi�ce.Pr�fixe + s + pi�ce.Suffixe;

			return s;
		}

		private int GetPi�ceProchainNum�ro(ComptaPi�ceEntity pi�ce)
		{
			//	Retourne le prochain num�ro de pi�ce � utiliser.
			//	TODO: Il faudra v�rifier que cette proc�dure fonctionne en multi-utilisateur !
			int n = pi�ce.Num�ro;
			pi�ce.Num�ro += pi�ce.Incr�ment;
			return n;
		}


		private readonly MainWindowController		mainWindowController;
	}
}
