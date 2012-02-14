//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using Epsitec.Common.Drawing;
using Epsitec.Common.Widgets;
using Epsitec.Common.Types;

using Epsitec.Cresus.Compta.Entities;
using Epsitec.Cresus.Compta.Helpers;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Compta.Options.Data
{
	/// <summary>
	/// Cette classe décrit les options d'affichage d'un extrait de compte de la comptabilité.
	/// </summary>
	public class ExtraitDeCompteOptions : AbstractOptions
	{
		public override void SetComptaEntity(ComptaEntity compta)
		{
			base.SetComptaEntity (compta);

			//	Utilise le premier compte normal par défaut.
			var compte = this.comptaEntity.PlanComptable.Where (x => x.Type == TypeDeCompte.Normal).FirstOrDefault ();

			if (compte != null)
			{
				this.NuméroCompte = compte.Numéro;
			}

			this.Clear ();
		}


		public override void Clear()
		{
			base.Clear ();

			this.CatégorieMontrée             = CatégorieDeCompte.Tous;
			this.MontreComptesVides           = true;
			this.MontreComptesCentralisateurs = false;
			this.HasGraphics                  = true;
		}


		public FormattedText NuméroCompte
		{
			//	Numéro du compte dont on affiche l'extrait.
			get;
			set;
		}

		public CatégorieDeCompte CatégorieMontrée
		{
			get;
			set;
		}

		public bool MontreComptesVides
		{
			get;
			set;
		}

		public bool MontreComptesCentralisateurs
		{
			get;
			set;
		}

		public bool HasGraphics
		{
			get;
			set;
		}


		protected override void CreateEmpty()
		{
			this.emptyOptions = new ExtraitDeCompteOptions ();
			this.emptyOptions.SetComptaEntity (this.comptaEntity);
			this.emptyOptions.Clear ();
		}


		public override AbstractOptions CopyFrom()
		{
			var options = new ExtraitDeCompteOptions ();
			options.SetComptaEntity (this.comptaEntity);
			this.CopyTo (options);
			return options;
		}

		public override void CopyTo(AbstractOptions dst)
		{
			var d = dst as ExtraitDeCompteOptions;

			d.CatégorieMontrée             = this.CatégorieMontrée;
			d.MontreComptesVides           = this.MontreComptesVides;
			d.MontreComptesCentralisateurs = this.MontreComptesCentralisateurs;
			d.HasGraphics                  = this.HasGraphics;

			base.CopyTo (dst);
		}

		public override bool CompareTo(AbstractOptions other)
		{
			if (!base.CompareTo (other))
			{
				return false;
			}

			var o = other as ExtraitDeCompteOptions;

			return this.CatégorieMontrée             == o.CatégorieMontrée             &&
				   this.MontreComptesVides           == o.MontreComptesVides           &&
				   this.MontreComptesCentralisateurs == o.MontreComptesCentralisateurs &&
				   this.HasGraphics                  == o.HasGraphics;
		}


		public override FormattedText Summary
		{
			get
			{
				this.StartSummaryBuilder ();

				if (this.CatégorieMontrée != CatégorieDeCompte.Tous)
				{
					this.AppendSummaryBuilder ("Montre " + Converters.CatégoriesToString (this.CatégorieMontrée));
				}

				if (this.MontreComptesVides)
				{
					this.AppendSummaryBuilder ("Comptes vides");
				}

				if (this.MontreComptesCentralisateurs)
				{
					this.AppendSummaryBuilder ("Comptes centralisateurs");
				}

				if (this.HasGraphics)
				{
					this.AppendSummaryBuilder ("Graphique du solde");
				}

				return this.StopSummaryBuilder ();
			}
		}
	}
}
