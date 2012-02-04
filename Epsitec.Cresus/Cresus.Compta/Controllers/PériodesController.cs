//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using Epsitec.Common.Drawing;
using Epsitec.Common.Widgets;
using Epsitec.Common.Types;
using Epsitec.Common.Types.Converters;

using Epsitec.Cresus.Core;
using Epsitec.Cresus.Core.Entities;
using Epsitec.Cresus.Core.Controllers;
using Epsitec.Cresus.Core.Widgets;
using Epsitec.Cresus.Core.Library;
using Epsitec.Cresus.Core.Business;

using Epsitec.Cresus.Compta.Accessors;
using Epsitec.Cresus.Compta.Entities;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Compta.Controllers
{
	/// <summary>
	/// Ce contrôleur gère les périodes comptables de la comptabilité.
	/// </summary>
	public class PériodesController : AbstractController
	{
		public PériodesController(Application app, BusinessContext businessContext, MainWindowController mainWindowController)
			: base (app, businessContext, mainWindowController)
		{
			this.dataAccessor = new PériodesDataAccessor (this);
		}


		protected override void UpdateTitle()
		{
			this.SetTitle ("Périodes comptables");
		}


		public override bool HasShowSearchPanel
		{
			get
			{
				return false;
			}
		}

		public override bool HasShowFilterPanel
		{
			get
			{
				return false;
			}
		}

		public override bool HasShowOptionsPanel
		{
			get
			{
				return false;
			}
		}

		public override bool HasShowInfoPanel
		{
			get
			{
				return true;
			}
		}


		protected override int ArrayLineHeight
		{
			get
			{
				return 20;
			}
		}

		protected override FormattedText GetArrayText(int row, ColumnType columnType)
		{
			//	Retourne le texte contenu dans une cellule.
			return this.dataAccessor.GetText (row, columnType);
		}


		protected override void CreateFooter(FrameBox parent)
		{
			this.footerController = new PériodeFooterController (this);
			this.footerController.CreateUI (parent, this.UpdateArrayContent);
			this.footerController.ShowInfoPanel = this.ShowInfoPanel;
		}


		protected override IEnumerable<ColumnMapper> InitialColumnMappers
		{
			get
			{
				yield return new ColumnMapper (ColumnType.Utilise,   0.20, ContentAlignment.MiddleCenter, "En cours",    "Détermine la période comptable en cours");
				yield return new ColumnMapper (ColumnType.DateDébut, 0.20, ContentAlignment.MiddleLeft,   "Date début",  "Date de début de la période");
				yield return new ColumnMapper (ColumnType.DateFin,   0.20, ContentAlignment.MiddleLeft,   "Date fin",    "Date de fin de la période");
				yield return new ColumnMapper (ColumnType.Titre,     0.80, ContentAlignment.MiddleLeft,   "Commentaire", "Commentaire affiché entre parenthèses après la période");
				yield return new ColumnMapper (ColumnType.Résumé,    0.60, ContentAlignment.MiddleLeft,   "Résumé");
			}
		}
	}
}
