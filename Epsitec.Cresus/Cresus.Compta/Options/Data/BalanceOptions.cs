//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using Epsitec.Common.Drawing;
using Epsitec.Common.Widgets;
using Epsitec.Common.Types;

using Epsitec.Cresus.Compta.Entities;
using Epsitec.Cresus.Compta.Graph;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Compta.Options.Data
{
	/// <summary>
	/// Cette classe décrit les options d'affichage de la balance de vérification de la comptabilité.
	/// </summary>
	public class BalanceOptions : AbstractOptions
	{
		public override void Clear()
		{
			base.Clear ();

			this.ZeroDisplayedInWhite = true;
			this.ComparisonShowed     = ComparisonShowed.Budget;

			this.graphOptions.Mode = GraphMode.SideBySide;
			this.graphOptions.TitleText = "Balance de vérification";
		}


		public bool ZeroDisplayedInWhite
		{
			//	Affiche en blanc les montants nuls ?
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
			this.emptyOptions = new BalanceOptions ();
			this.emptyOptions.SetComptaEntity (this.compta);
			this.emptyOptions.Clear ();
		}


		public override AbstractOptions CopyFrom()
		{
			var options = new BalanceOptions ();
			options.SetComptaEntity (this.compta);
			this.CopyTo (options);
			return options;
		}

		public override void CopyTo(AbstractOptions dst)
		{
			var d = dst as BalanceOptions;

			d.ZeroDisplayedInWhite = this.ZeroDisplayedInWhite;
			d.HasGraphics          = this.HasGraphics;

			base.CopyTo (dst);
		}

		public override bool CompareTo(AbstractOptions other)
		{
			if (!base.CompareTo (other))
			{
				return false;
			}

			var o = other as BalanceOptions;

			return this.ZeroDisplayedInWhite == o.ZeroDisplayedInWhite &&
				   this.HasGraphics          == o.HasGraphics;
		}


		public override FormattedText Summary
		{
			get
			{
				this.StartSummaryBuilder ();

				if (this.ViewGraph)
				{
					this.AppendSummaryBuilder (this.graphOptions.Summary);
				}
				else
				{
					if (this.ZeroDisplayedInWhite)
					{
						this.AppendSummaryBuilder ("Affiche en blanc les montants nuls");
					}

					if (this.HasGraphics)
					{
						this.AppendSummaryBuilder ("Graphique du solde");
					}

					this.AppendSummaryBuilder (this.ComparisonSummary);
				}

				return this.StopSummaryBuilder ();
			}
		}
	}
}
