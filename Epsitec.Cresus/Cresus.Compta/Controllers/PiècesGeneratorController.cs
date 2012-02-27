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
	/// Ce contrôleur gère les générateurs de numéros de pièces de la comptabilité.
	/// </summary>
	public class PiècesGeneratorController : AbstractController
	{
		public PiècesGeneratorController(Application app, BusinessContext businessContext, MainWindowController mainWindowController)
			: base (app, businessContext, mainWindowController)
		{
			this.dataAccessor = new PiècesGeneratorDataAccessor (this);

			this.viewSettingsList = this.mainWindowController.GetViewSettingsList ("Présentation.PiècesGenerator.ViewSettings");
		}


		protected override void UpdateTitle()
		{
			this.SetTitle ("Générateurs de numéros de pièces");
		}


		public override bool AcceptPériodeChanged
		{
			get
			{
				return false;
			}
		}


		public override bool HasCreateCommand
		{
			get
			{
				return true;
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
			this.editorController = new PiècesGeneratorEditorController (this);
			this.editorController.CreateUI (parent, this.UpdateArrayContent);
			this.editorController.ShowInfoPanel = this.mainWindowController.ShowInfoPanel;
		}


		protected override IEnumerable<ColumnMapper> InitialColumnMappers
		{
			get
			{
				yield return new ColumnMapper (ColumnType.Nom,         1.00, ContentAlignment.MiddleLeft, "Nom",             "Nom du générateur de numéros de pièces");
				yield return new ColumnMapper (ColumnType.Format,      0.00, ContentAlignment.MiddleLeft, "Format",          "Format du numéro, avec '#' pour indiquer la position du numéro<br/>Exemples: \"#\", \"A##/d\", \"##-###.P\"");
				yield return new ColumnMapper (ColumnType.Numéro,      0.00, ContentAlignment.MiddleLeft, "Prochain numéro", "Prochain numéro généré");
				yield return new ColumnMapper (ColumnType.Incrément,   0.00, ContentAlignment.MiddleLeft, "Incrément",       "Valeur de l'incrément");
				yield return new ColumnMapper (ColumnType.Exemple,     1.50, ContentAlignment.MiddleLeft, "Exemples", edition: false);
				yield return new ColumnMapper (ColumnType.Résumé,      1.50, ContentAlignment.MiddleLeft, "Résumé",   edition: false);
			}
		}
	}
}
