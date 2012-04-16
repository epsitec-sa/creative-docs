//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using Epsitec.Common.Types;

using Epsitec.Cresus.Compta.Controllers;
using Epsitec.Cresus.Compta.Entities;
using Epsitec.Cresus.Compta.Helpers;
using Epsitec.Cresus.Compta.Search.Data;
using Epsitec.Cresus.Compta.Options.Data;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Compta.Accessors
{
	/// <summary>
	/// Gère l'accès aux données du résumé périodique de la comptabilité.
	/// </summary>
	public class RésuméPériodiqueDataAccessor : AbstractDataAccessor
	{
		public RésuméPériodiqueDataAccessor(AbstractController controller)
			: base (controller)
		{
			this.options    = this.mainWindowController.GetSettingsOptions<RésuméPériodiqueOptions> ("Présentation.RésuméPériodique.Options", this.compta);
			this.searchData = this.mainWindowController.GetSettingsSearchData ("Présentation.RésuméPériodique.Search");
			this.filterData = this.mainWindowController.GetSettingsSearchData ("Présentation.RésuméPériodique.Filter");

			this.UpdateAfterOptionsChanged ();
		}


		public override void UpdateFilter()
		{
			this.UpdateReadonlyAllData ();
		}

		public override void UpdateAfterOptionsChanged()
		{
			this.UpdateReadonlyAllData ();
		}

		private void UpdateReadonlyAllData()
		{
			this.readonlyAllData.Clear ();

			this.filterData.GetBeginnerDates (out this.lastBeginDate, out this.lastEndDate);
			this.soldesJournalManager.Initialize (this.période.Journal, this.lastBeginDate, this.lastEndDate);

			var soldesManagers = new List<SoldesJournalManager> ();
			var dateDébut = this.période.DateDébut;
			var finPériode = Dates.AddDays (this.période.DateFin, 1);  // normalement, 1er janvier de l'année suivante
			var dateFin = dateDébut;

			do
			{
				dateFin = Dates.AddMonths (dateDébut, this.Options.NumberOfMonths);

				if (dateFin > finPériode)
				{
					dateFin = finPériode;
				}

				var soldesManager = new SoldesJournalManager (this.compta);
				soldesManager.Initialize (this.période.Journal, dateDébut, Dates.AddDays (dateFin, -1));
				soldesManagers.Add (soldesManager);

				if (!this.Options.Cumul)
				{
					dateDébut = dateFin;
				}
			}
			while (dateFin < finPériode);

			foreach (var compte in this.compta.PlanComptable)
			{
				if (compte.Catégorie == CatégorieDeCompte.Inconnu)
				{
					continue;
				}

				var data = new RésuméPériodiqueData ();

				data.Numéro = compte.Numéro;
				data.Titre  = compte.Titre;

				for (int i = 0; i < soldesManagers.Count; i++)
				{
					data.SetSolde (i, soldesManagers[i].GetSolde (compte));
				}

				this.readonlyAllData.Add (data);
			}



			base.UpdateFilter ();
		}


		public override FormattedText GetText(int row, ColumnType column, bool all = false)
		{
			var data = this.GetReadOnlyData (row, all) as RésuméPériodiqueData;

			if (data == null)
			{
				return FormattedText.Null;
			}

			if (column >= ColumnType.Solde1 && column <= ColumnType.Solde12)
			{
				int rank = column - ColumnType.Solde1;
				var solde = data.GetSolde (rank);
				return Converters.MontantToString (solde, this.compta.Monnaies[0]);
			}

			switch (column)
			{
				case ColumnType.Numéro:
					return data.Numéro;

				case ColumnType.Titre:
					return data.Titre;

				default:
					return FormattedText.Null;
			}
		}


		private new RésuméPériodiqueOptions Options
		{
			get
			{
				return this.options as RésuméPériodiqueOptions;
			}
		}
	}
}