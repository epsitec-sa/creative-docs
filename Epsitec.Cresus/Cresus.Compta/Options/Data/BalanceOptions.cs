//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using Epsitec.Common.Drawing;
using Epsitec.Common.Widgets;
using Epsitec.Common.Types;

using Epsitec.Cresus.Core;
using Epsitec.Cresus.Core.Entities;
using Epsitec.Cresus.Core.Controllers;

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

		public override bool CompareTo(AbstractOptions other)
		{
			if (!base.CompareTo (other))
			{
				return false;
			}

			var o = other as BalanceOptions;

			return this.HideZero == o.HideZero;
		}
	}
}
