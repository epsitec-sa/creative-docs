//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using Epsitec.Common.Drawing;
using Epsitec.Common.Widgets;
using Epsitec.Common.Types;

using Epsitec.Cresus.Compta.Entities;

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
			this.HideZero = true;
		}


		public bool HideZero
		{
			//	Affiche en blanc les montants nuls ?
			get;
			set;
		}


		protected override void CreateEmpty()
		{
			this.emptyOptions = new BalanceOptions ();
			this.emptyOptions.SetComptaEntity (this.comptaEntity);
			this.emptyOptions.Clear ();
		}


		public override AbstractOptions CopyFrom()
		{
			var options = new BalanceOptions ();
			options.SetComptaEntity (this.comptaEntity);
			this.CopyTo (options);
			return options;
		}

		public override void CopyTo(AbstractOptions dst)
		{
			var d = dst as BalanceOptions;
			d.HideZero = this.HideZero;

			base.CopyTo (dst);
		}

		public override bool CompareTo(AbstractOptions other)
		{
			if (!base.CompareTo (other))
			{
				return false;
			}

			var o = other as BalanceOptions;

			return this.HideZero == o.HideZero;
		}


		public override FormattedText Summary
		{
			get
			{
				this.StartSummaryBuilder ();

				if (this.HideZero)
				{
					this.AppendSummaryBuilder ("Affiche en blanc les montants nuls");
				}

				return this.StopSummaryBuilder ();
			}
		}
	}
}
