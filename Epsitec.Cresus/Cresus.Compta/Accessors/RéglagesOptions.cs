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

namespace Epsitec.Cresus.Compta.Accessors
{
	/// <summary>
	/// Cette classe décrit les options d'affichage des réglages de la comptabilité.
	/// </summary>
	public class RéglagesOptions : AbstractOptions
	{
		public override void Clear()
		{
			base.Clear ();
			this.SelectIndex = -1;
		}


		public int SelectIndex
		{
			get;
			set;
		}


		protected override void CreateEmpty()
		{
			this.emptyOptions = new RéglagesOptions ();
			this.emptyOptions.SetComptaEntity (this.comptaEntity);
			this.emptyOptions.Clear ();
		}

		public override bool CompareTo(AbstractOptions other)
		{
			if (!base.CompareTo (other))
			{
				return false;
			}

			var o = other as RéglagesOptions;

			return this.SelectIndex == o.SelectIndex;
		}
	}
}
