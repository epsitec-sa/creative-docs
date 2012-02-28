//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using Epsitec.Common.Drawing;
using Epsitec.Common.Widgets;
using Epsitec.Common.Types;

using Epsitec.Cresus.Compta.Accessors;
using Epsitec.Cresus.Compta.Entities;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Compta.Options.Data
{
	/// <summary>
	/// Cette classe décrit les options d'affichage du bilan de la comptabilité.
	/// </summary>
	public class BilanOptions : DoubleOptions
	{
		public override void Clear()
		{
			base.Clear ();

			this.ComparisonShowed = ComparisonShowed.PériodePrécédente;
		}


		protected override void CreateEmpty()
		{
			this.emptyOptions = new BilanOptions ();
			this.emptyOptions.SetComptaEntity (this.compta);
			this.emptyOptions.Clear ();
		}

		public override AbstractOptions CopyFrom()
		{
			var options = new BilanOptions ();
			options.SetComptaEntity (this.compta);
			this.CopyTo (options);
			return options;
		}
	}
}
