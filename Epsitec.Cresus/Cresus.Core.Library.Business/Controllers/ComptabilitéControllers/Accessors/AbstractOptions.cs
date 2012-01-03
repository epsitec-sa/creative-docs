//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using Epsitec.Common.Drawing;
using Epsitec.Common.Widgets;
using Epsitec.Common.Types;

using Epsitec.Cresus.Core;
using Epsitec.Cresus.Core.Entities;
using Epsitec.Cresus.Core.Controllers;
using Epsitec.Cresus.Core.Widgets;
using Epsitec.Cresus.Core.Widgets.Tiles;
using Epsitec.Cresus.Core.Business.Finance.Comptabilité;

using Epsitec.Cresus.DataLayer.Context;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Controllers.ComptabilitéControllers
{
	/// <summary>
	/// Cette classe décrit les options d'affichage génériques de la comptabilité.
	/// </summary>
	public abstract class AbstractOptions
	{
		public AbstractOptions(ComptabilitéEntity comptabilitéEntity)
		{
			this.comptabilitéEntity = comptabilitéEntity;
		}


		public int? Profondeur
		{
			get;
			set;
		}

		public Date? DateDébut
		{
			get;
			set;
		}

		public Date? DateFin
		{
			get;
			set;
		}


		public bool DateInRange(Date? date)
		{
			if (date.HasValue)
			{
				if (this.DateDébut.HasValue && date.Value < this.DateDébut.Value)
				{
					return false;
				}

				if (this.DateFin.HasValue && date.Value > this.DateFin.Value)
				{
					return false;
				}
			}

			return true;
		}


		protected readonly ComptabilitéEntity		comptabilitéEntity;

		protected int?								profondeur;
		protected Date?								dateDébut;
		protected Date?								dateFin;
	}
}
