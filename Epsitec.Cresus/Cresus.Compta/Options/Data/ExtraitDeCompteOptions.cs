//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using Epsitec.Common.Drawing;
using Epsitec.Common.Widgets;
using Epsitec.Common.Types;

using Epsitec.Cresus.Compta.Entities;
using Epsitec.Cresus.Compta.Helpers;
using Epsitec.Cresus.Compta.Graph;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Compta.Options.Data
{
	/// <summary>
	/// Cette classe décrit les options d'affichage d'un extrait de compte de la comptabilité.
	/// </summary>
	public class ExtraitDeCompteOptions : AbstractOptions
	{
		public override void Clear()
		{
			base.Clear ();

			this.HasGraphicColumn = true;
			this.graphOptions.Mode = GraphMode.Lines;
			this.graphOptions.HasLegend = false;
			this.graphOptions.TitleText = "Extrait de compte";
		}


		public FormattedText NuméroCompte
		{
			get;
			set;
		}


		protected override void CreateEmpty()
		{
			this.emptyOptions = new ExtraitDeCompteOptions ();
			this.emptyOptions.SetComptaEntity (this.compta);
			this.emptyOptions.Clear ();
		}


		public override AbstractOptions CopyFrom()
		{
			var options = new ExtraitDeCompteOptions ();
			options.SetComptaEntity (this.compta);
			this.CopyTo (options);
			return options;
		}

		public override void CopyTo(AbstractOptions dst)
		{
			if (dst == this)
			{
				return;
			}

			var d = dst as ExtraitDeCompteOptions;

			d.NuméroCompte = this.NuméroCompte;

			base.CopyTo (dst);
		}

		public override bool CompareTo(AbstractOptions other)
		{
			if (!base.CompareTo (other))
			{
				return false;
			}

			var o = other as ExtraitDeCompteOptions;

			return this.NuméroCompte == o.NuméroCompte;
		}


		public override FormattedText Summary
		{
			get
			{
				this.StartSummaryBuilder ();

				if (!this.NuméroCompte.IsNullOrEmpty)
				{
					this.AppendSummaryBuilder (FormattedText.Concat ("Compte ", this.NuméroCompte));
				}

				if (this.ViewGraph)
				{
					this.AppendSummaryBuilder (this.graphOptions.Summary);
				}
				else
				{
					if (this.HasGraphicColumn)
					{
						this.AppendSummaryBuilder ("Graphique du solde");
					}
				}

				return this.StopSummaryBuilder ();
			}
		}
	}
}
