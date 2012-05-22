//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using Epsitec.Common.Drawing;
using Epsitec.Common.Widgets;
using Epsitec.Common.Types;
using Epsitec.Common.Types.Converters;

using Epsitec.Cresus.Core.Business;

using Epsitec.Cresus.Compta.Accessors;
using Epsitec.Cresus.Compta.Entities;
using Epsitec.Cresus.Compta.Helpers;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Compta.Controllers
{
	/// <summary>
	/// Ce contrôleur gère les journaux de la comptabilité.
	/// </summary>
	public class JournauxController : AbstractController
	{
		public JournauxController(ComptaApplication app, BusinessContext businessContext, MainWindowController mainWindowController)
			: base (app, businessContext, mainWindowController)
		{
			this.dataAccessor = new JournauxDataAccessor (this);
		}


		protected override void UpdateTitle()
		{
			this.SetGroupTitle (Présentations.GetGroupName (ControllerType.Journaux));
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

		public override bool HasSearchPanel
		{
			get
			{
				return true;
			}
		}

		public override bool HasFilterPanel
		{
			get
			{
				return false;
			}
		}

		public override bool HasOptionsPanel
		{
			get
			{
				return false;
			}
		}

		public override bool HasInfoPanel
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
			this.editorController = new JournauxEditorController (this);
			this.editorController.CreateUI (parent, this.UpdateArrayContent);
			this.editorController.ShowInfoPanel = this.mainWindowController.ShowInfoPanel;
		}


		protected override IEnumerable<ColumnMapper> InitialColumnMappers
		{
			get
			{
				yield return new ColumnMapper (ColumnType.Titre,   1.00, ContentAlignment.MiddleLeft, "Nom",                  "Nom court du journal");
				yield return new ColumnMapper (ColumnType.Libellé, 0.00, ContentAlignment.MiddleLeft, "Description",          "Description détaillée du journal");
				yield return new ColumnMapper (ColumnType.Pièce,   0.50, ContentAlignment.MiddleLeft, "Générateur de pièces", "Générateur pour les numéros de pièces (facultatif)");
				yield return new ColumnMapper (ColumnType.Résumé,  1.00, ContentAlignment.MiddleLeft, "Résumé", edition: false);
			}
		}
	}
}
