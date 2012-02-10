//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using Epsitec.Common.Drawing;
using Epsitec.Common.Widgets;
using Epsitec.Common.Types;
using Epsitec.Common.Types.Converters;

using Epsitec.Cresus.Core;
using Epsitec.Cresus.Core.Entities;
using Epsitec.Cresus.Core.Controllers;
using Epsitec.Cresus.Core.Library;
using Epsitec.Cresus.Core.Business;

using Epsitec.Cresus.Compta.Accessors;
using Epsitec.Cresus.Compta.Entities;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Compta.Controllers
{
	/// <summary>
	/// Ce contrôleur gère les journaux de la comptabilité.
	/// </summary>
	public class JournauxController : AbstractController
	{
		public JournauxController(Application app, BusinessContext businessContext, MainWindowController mainWindowController)
			: base (app, businessContext, mainWindowController)
		{
			this.dataAccessor = new JournauxDataAccessor (this);
		}


		protected override void UpdateTitle()
		{
			this.SetTitle ("Journaux d'écritures");
		}


		public override bool AcceptPériodeChanged
		{
			get
			{
				return false;
			}
		}


		public override bool HasShowSearchPanel
		{
			get
			{
				return true;
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
			this.footerController = new JournauxFooterController (this);
			this.footerController.CreateUI (parent, this.UpdateArrayContent);
			this.footerController.ShowInfoPanel = this.ShowInfoPanel;
		}


		protected override IEnumerable<ColumnMapper> InitialColumnMappers
		{
			get
			{
				yield return new ColumnMapper (ColumnType.Titre,   0.50, ContentAlignment.MiddleLeft, "Nom",         "Nom court du journal");
				yield return new ColumnMapper (ColumnType.Libellé, 1.00, ContentAlignment.MiddleLeft, "Description", "Description détaillée du journal");
				yield return new ColumnMapper (ColumnType.Résumé,  0.50, ContentAlignment.MiddleLeft, "Résumé");
			}
		}
	}
}
