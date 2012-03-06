//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using Epsitec.Common.Drawing;
using Epsitec.Common.Widgets;
using Epsitec.Common.Types;
using Epsitec.Common.Types.Converters;

using Epsitec.Cresus.Core.Business;

using Epsitec.Cresus.Compta.Accessors;
using Epsitec.Cresus.Compta.Entities;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Compta.Controllers
{
	/// <summary>
	/// Ce contrôleur gère les taux de TVA de la comptabilité.
	/// </summary>
	public class TauxTVAController : AbstractController
	{
		public TauxTVAController(Application app, BusinessContext businessContext, MainWindowController mainWindowController)
			: base (app, businessContext, mainWindowController)
		{
			this.dataAccessor = new TauxTVADataAccessor (this);

			this.viewSettingsList = this.mainWindowController.GetViewSettingsList ("Présentation.TauxTVA.ViewSettings");
		}


		protected override void UpdateTitle()
		{
			this.SetTitle ("Taux de TVA");
		}


		public override bool AcceptPériodeChanged
		{
			get
			{
				return false;
			}
		}


		public override bool HasRightEditor
		{
			get
			{
				return true;
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


		protected override void CreateEditor(FrameBox parent)
		{
			this.editorController = new TauxTVAEditorController (this);
			this.editorController.CreateUI (parent, this.UpdateArrayContent);
			this.editorController.ShowInfoPanel = this.mainWindowController.ShowInfoPanel;
		}


		protected override IEnumerable<ColumnMapper> InitialColumnMappers
		{
			get
			{
				yield return new ColumnMapper (ColumnType.Nom,       2.00, ContentAlignment.MiddleLeft,  "Nom court",                "Nom court unique");
				yield return new ColumnMapper (ColumnType.DateDébut, 1.00, ContentAlignment.MiddleLeft,  "Dates de début et de fin", "Date de début de validité (inclue)");
				yield return new ColumnMapper (ColumnType.DateFin,   1.00, ContentAlignment.MiddleLeft,  "",                         "Date de fin de validité (inclue)");
				yield return new ColumnMapper (ColumnType.Taux,      1.00, ContentAlignment.MiddleRight, "Taux",                     "Taux de TVA");
			}
		}
	}
}
