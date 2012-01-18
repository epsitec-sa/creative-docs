﻿//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using Epsitec.Common.Drawing;
using Epsitec.Common.Widgets;
using Epsitec.Common.Types;

using Epsitec.Cresus.Core;
using Epsitec.Cresus.Core.Entities;
using Epsitec.Cresus.Core.Controllers;
using Epsitec.Cresus.Core.Widgets;
using Epsitec.Cresus.Core.Widgets.Tiles;

using Epsitec.Cresus.Compta.Entities;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Compta
{
	/// <summary>
	/// Cette classe décrit les options d'affichage de la balance de vérification de la comptabilité.
	/// </summary>
	public class BalanceOptions : AbstractOptions
	{
		public BalanceOptions(ComptabilitéEntity comptabilitéEntity)
			: base (comptabilitéEntity)
		{
		}


		public bool ComptesNuls
		{
			//	Affiche les comptes dont le solde est nul ?
			get;
			set;
		}
	}
}
