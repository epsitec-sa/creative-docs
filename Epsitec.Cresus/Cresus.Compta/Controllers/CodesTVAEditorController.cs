//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using Epsitec.Common.Drawing;
using Epsitec.Common.Widgets;
using Epsitec.Common.Types;
using Epsitec.Common.Types.Converters;

using Epsitec.Cresus.Compta.Accessors;
using Epsitec.Cresus.Compta.Entities;
using Epsitec.Cresus.Compta.Widgets;
using Epsitec.Cresus.Compta.Helpers;
using Epsitec.Cresus.Compta.Fields.Controllers;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Compta.Controllers
{
	/// <summary>
	/// Ce contrôleur gère le pied de page pour l'édition de la comptabilité.
	/// </summary>
	public class CodesTVAEditorController : AbstractEditorController
	{
		public CodesTVAEditorController(AbstractController controller)
			: base (controller)
		{
		}


		public override void CreateUI(FrameBox parent, System.Action updateArrayContentAction)
		{
			var band = this.CreateEditorUI (parent);
			this.CreateLineUI (band);

			base.CreateUI (parent, updateArrayContentAction);
		}

		private void CreateLineUI(Widget parent)
		{
			this.fieldControllers.Add (new List<AbstractFieldController> ());

			var editorFrame = new TabCatcherFrameBox
			{
				Parent  = parent,
				Dock    = DockStyle.Fill,
				Margins = new Margins (0, 0, 1, 0),
			};

			editorFrame.TabPressed += new TabCatcherFrameBox.TabPressedEventHandler (this.HandleLinesContainerTabPressed);

			this.linesFrames.Add (editorFrame);
			int line = this.linesFrames.Count - 1;
			int tabIndex = 0;

			editorFrame.TabIndex = line+1;

			foreach (var mapper in this.columnMappers.Where (x => x.Edition))
			{
				if (mapper.Column == ColumnType.Chiffre  ||
					mapper.Column == ColumnType.ParDéfaut)  // insère un gap ?
				{
					new FrameBox
					{
						Parent          = editorFrame,
						PreferredHeight = 20,
						Dock            = DockStyle.Top,
					};
				}

				AbstractFieldController field;

				if (mapper.Column == ColumnType.Compte)
				{
					field = new AutoCompleteFieldController (this.controller, line, mapper, this.HandleSetFocus, this.EditorTextChanged);
					field.CreateUI (editorFrame);

					var comptes = this.compta.PlanComptable.Where (x => x.Type == TypeDeCompte.Normal);
					UIBuilder.UpdateAutoCompleteTextField (field.EditWidget as AutoCompleteTextField, comptes);
				}
				else if (mapper.Column == ColumnType.ParDéfaut ||
						 mapper.Column == ColumnType.Désactivé)
				{
					field = new CheckButtonController (this.controller, line, mapper, this.HandleSetFocus, this.EditorTextChanged);
					field.CreateUI (editorFrame);
				}
				else
				{
					field = new TextFieldController (this.controller, line, mapper, this.HandleSetFocus, this.EditorTextChanged);
					field.CreateUI (editorFrame);
				}

				field.Box.TabIndex = ++tabIndex;

				this.fieldControllers[line].Add (field);
			}
		}

		protected override FormattedText GetOperationDescription(bool modify)
		{
			return modify ? "Modification d'un code TVA :" : "Création d'un code TVA :";
		}
	}
}
