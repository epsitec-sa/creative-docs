//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using Epsitec.Common.Types;

using Epsitec.Cresus.Core;
using Epsitec.Cresus.Core.Entities;
using Epsitec.Cresus.Core.Business;

using Epsitec.Cresus.Compta.Controllers;
using Epsitec.Cresus.Compta.Entities;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Compta.Accessors
{
	/// <summary>
	/// Gère l'accès aux données de la balance de vérification de la comptabilité.
	/// </summary>
	public class BalanceDataAccessor : AbstractDataAccessor
	{
		public BalanceDataAccessor(BusinessContext businessContext, ComptaEntity comptaEntity, MainWindowController mainWindowController)
			: base (businessContext, comptaEntity, mainWindowController)
		{
			this.options = this.mainWindowController.GetSettingsOptions<BalanceOptions> ("Présentation.BalanceOptions", this.comptaEntity);

			this.UpdateAfterOptionsChanged ();
		}


		public override void UpdateAfterOptionsChanged()
		{
			this.readonlyAllData.Clear ();

			ComptaCompteEntity lastCompte = null;
			decimal totalDébit  = 0;
			decimal totalCrédit = 0;
			decimal totalSoldeD = 0;
			decimal totalSoldeC = 0;

			this.comptaEntity.PlanComptableUpdate (this.options.DateDébut, this.options.DateFin);

			foreach (var compte in this.comptaEntity.PlanComptable.OrderBy (x => x.Numéro))
			{
				if (compte.Catégorie == CatégorieDeCompte.Inconnu ||
					compte.Catégorie == CatégorieDeCompte.Exploitation)
				{
					continue;
				}

				if (this.Options.Profondeur.HasValue && compte.Niveau >= this.Options.Profondeur.Value)
				{
					continue;
				}

				var soldeDébit  = this.comptaEntity.GetSoldeCompteDébit  (compte);
				var soldeCrédit = this.comptaEntity.GetSoldeCompteCrédit (compte);
				var différence = soldeCrédit.GetValueOrDefault () - soldeDébit.GetValueOrDefault ();

				if (!this.Options.ComptesNuls && soldeDébit.GetValueOrDefault () == 0 && soldeCrédit.GetValueOrDefault () == 0)
				{
					continue;
				}

				var data = new BalanceData ();

				data.Numéro = compte.Numéro;
				data.Titre  = compte.Titre;
				data.Niveau = compte.Niveau;
				data.Débit  = soldeDébit .GetValueOrDefault () == 0 ? null : soldeDébit;
				data.Crédit = soldeCrédit.GetValueOrDefault () == 0 ? null : soldeCrédit;

				if (différence < 0)
				{
					data.SoldeDébit = -différence;
				}
				if (différence > 0)
				{
					data.SoldeCrédit = différence;
				}

				if (compte.Type == TypeDeCompte.Normal)
				{
					totalDébit  += soldeDébit.GetValueOrDefault ();
					totalCrédit += soldeCrédit.GetValueOrDefault ();

					if (différence < 0)
					{
						totalSoldeD -= différence;
					}
					if (différence > 0)
					{
						totalSoldeC += différence;
					}
				}

				if (lastCompte == null || lastCompte.Catégorie != compte.Catégorie)  // changement de catégorie ?
				{
					data.IsBold = true;
					this.SetBottomSeparatorToPreviousLine ();
				}

				lastCompte = compte;

				this.readonlyAllData.Add (data);
			}

			this.SetBottomSeparatorToPreviousLine ();

			// Génère la dernière ligne.
			{
				var data = new BalanceData ();

				data.Titre         = "Mouvement";
				data.Débit         = totalDébit;
				data.Crédit        = totalCrédit;
				data.SoldeDébit    = totalSoldeD;
				data.SoldeCrédit   = totalSoldeC;
				data.IsItalic      = true;
				data.NeverFiltered = true;

				this.readonlyAllData.Add (data);
			}

			this.FilterUpdate ();
		}


		public override FormattedText GetText(int row, ColumnType column, bool all = false)
		{
			var data = this.GetReadOnlyData (row, all) as BalanceData;

			if (data == null)
			{
				return FormattedText.Null;
			}

			switch (column)
			{
				case ColumnType.Numéro:
					return data.Numéro;

				case ColumnType.Titre:
					return data.Titre;

				case ColumnType.Débit:
					return AbstractDataAccessor.GetMontant (data.Débit);

				case ColumnType.Crédit:
					return AbstractDataAccessor.GetMontant (data.Crédit);

				case ColumnType.SoldeDébit:
					return AbstractDataAccessor.GetMontant (data.SoldeDébit);

				case ColumnType.SoldeCrédit:
					return AbstractDataAccessor.GetMontant (data.SoldeCrédit);

				case ColumnType.Budget:
					return AbstractDataAccessor.GetMontant (data.Budget);

				default:
					return FormattedText.Null;
			}
		}


		private BalanceOptions Options
		{
			get
			{
				return this.options as BalanceOptions;
			}
		}
	}
}