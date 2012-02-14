//	Copyright © 2011-2012, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Types;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Compta.Entities
{
	public partial class ComptaEntity
	{
		public decimal? GetMontantBudget(ComptaPériodeEntity période, int offset, ComptaCompteEntity compte)
		{
			var budget = this.GetBudget (période, offset, compte);

			if (budget == null)
			{
				return null;
			}
			else
			{
				return budget.Montant;
			}
		}

		public ComptaBudgetEntity GetBudget(ComptaPériodeEntity période, int offset, ComptaCompteEntity compte)
		{
			période = this.GetPériode (période, offset);

			if (période == null)
			{
				return null;
			}
			else
			{
				return compte.GetBudget (période);
			}
		}

		public ComptaPériodeEntity GetPériode(ComptaPériodeEntity période, int offset)
		{
			//	Retourne la période précédente (offset = -1) ou suivante (offset = 1).
			if (offset == 0)
			{
				return période;
			}

			int i = this.Périodes.IndexOf (période);

			if (i == -1 || i+offset < 0 || i+offset >= this.Périodes.Count)
			{
				return null;
			}

			return this.Périodes[i+offset];
		}


		public IEnumerable<FormattedText> GetLibellésDescriptions(ComptaPériodeEntity période)
		{
			//	Retourne la liste des libellés usuels.
			return this.Libellés.Select (x => x.Libellé);
		}

		public void AddLibellé(ComptaPériodeEntity période, FormattedText libellé)
		{
			//	Insère un nouveau libellé volatile. S'il est déjà dans la liste, on le remet au sommet.
			if (libellé.IsNullOrEmpty)
			{
				return;
			}

			var exist = this.Libellés.Where (x => x.Libellé == libellé).FirstOrDefault ();

			if (exist == null)
			{
				int index = 0;

				var firstPermanant = this.Libellés.Where (x => x.Permanant).LastOrDefault ();
				if (firstPermanant != null)
				{
					//	On insère un libellé volatile après le dernier libellé volatile.
					index = this.Libellés.IndexOf (firstPermanant) + 1;
				}

				var nouveau = new ComptaLibelléEntity ()
				{
					Libellé   = libellé,
					Permanant = false,
				};

				this.Libellés.Insert (index, nouveau);

				//?this.PurgeVolatileLibellés (20);
			}
			else
			{
				this.Libellés.Remove (exist);
				this.Libellés.Insert (0, exist);  // déplace au sommet
			}
		}

		private void PurgeVolatileLibellés(int limit)
		{
			int count = this.Libellés.Where (x => !x.Permanant).Count ();

			while (count > limit)
			{
				var last = this.Libellés.Where (x => !x.Permanant).LastOrDefault ();
				this.Libellés.Remove (last);
			}
		}


		public FormattedText JournalRésumé(ComptaJournalEntity journal)
		{
			//	Retourne le résumé d'un journal.
			int totalEcritures = 0;
			int totalPériodes = 0;

			foreach (var période in this.Périodes)
			{
				int total = période.Journal.Where (x => x.Journal == journal).Count ();

				if (total != 0)
				{
					totalEcritures += total;
					totalPériodes++;
				}
			}

			string écrituresRésumé, périodesRésumé;

			if (totalEcritures == 0)
			{
				return "Vide";
			}
			else if (totalEcritures == 1)
			{
				écrituresRésumé = "1 écriture";
			}
			else
			{
				écrituresRésumé = string.Format ("{0} écritures", totalEcritures.ToString ());
			}

			if (totalPériodes == 0)
			{
				périodesRésumé = "aucune période";
			}
			else if (totalPériodes == 1)
			{
				périodesRésumé = "1 période";
			}
			else
			{
				périodesRésumé = string.Format ("{0} périodes", totalPériodes.ToString ());
			}

			return écrituresRésumé + " dans " + périodesRésumé;
		}


		public FormattedText ProchainePièce
		{
			//	A partir de "AB102", retourne "AB103" (par exemple).
			get
			{
				var pièce = this.DernièrePièce;

				if (!pièce.IsNullOrEmpty)
				{
					string p = pièce.ToSimpleText ();
					int i = pièce.Length-1;
					while (i >= 0)
					{
						if (p[i] >= '0' && p[i] <= '9')
						{
							i--;
						}
						else
						{
							break;
						}
					}

					if (i < pièce.Length-1)
					{
						int n;
						if (int.TryParse (p.Substring (i+1), out n))
						{
							return p.Substring (0, i+1) + (n+1).ToString ();
						}
					}
				}

				return pièce;
			}
		}


		#region Niveaux d'imbrications
		public void UpdateNiveauCompte()
		{
			//	Met à jour le niveau d'imbrication (0..n) de tous les comptes.
			foreach (var compte in this.PlanComptable)
			{
				this.UpdateNiveauCompte (compte);
			}
		}

		private void UpdateNiveauCompte(ComptaCompteEntity compte)
		{
			//	Met à jour le niveau d'imbrication (0..n) d'un compte.
			var c = compte;
			int niveau = 0;

			while (c != null && c.Groupe != null && !c.Groupe.Numéro.IsNullOrEmpty)
			{
				c = this.PlanComptable.Where (x => x.Numéro == c.Groupe.Numéro).FirstOrDefault ();

				if (c == null)
				{
					break;
				}

				niveau++;
			}

			if (compte.Niveau != niveau)
			{
				compte.Niveau = niveau;
			}
		}
		#endregion
	}
}
