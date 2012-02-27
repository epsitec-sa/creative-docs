//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using Epsitec.Common.Drawing;
using Epsitec.Common.Widgets;
using Epsitec.Common.Types;
using Epsitec.Common.Types.Converters;

using Epsitec.Cresus.Compta.Accessors;
using Epsitec.Cresus.Compta.Entities;
using Epsitec.Cresus.Compta.Widgets;
using Epsitec.Cresus.Compta.Fields.Controllers;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Compta.Controllers
{
	/// <summary>
	/// Ce contrôleur gère le pied de page pour l'édition de la comptabilité.
	/// </summary>
	public class BudgetsEditorController : AbstractEditorController
	{
		public BudgetsEditorController(AbstractController controller)
			: base (controller)
		{
		}


		public override void CreateUI(FrameBox parent, System.Action updateArrayContentAction)
		{
			this.fieldControllers.Clear ();

			this.CreateLineUI (parent);

			base.CreateUI (parent, updateArrayContentAction);
		}

		private void CreateLineUI(Widget parent)
		{
			this.fieldControllers.Add (new List<AbstractFieldController> ());

			var editorFrame = new TabCatcherFrameBox
			{
				Parent          = parent,
				PreferredHeight = 20,
				Dock            = DockStyle.Bottom,
				Margins         = new Margins (0, 0, 1, 0),
			};

			editorFrame.TabPressed += new TabCatcherFrameBox.TabPressedEventHandler (this.HandleLinesContainerTabPressed);

			this.linesFrames.Add (editorFrame);
			int line = this.linesFrames.Count - 1;
			int tabIndex = 0;

			editorFrame.TabIndex = line+1;

			foreach (var mapper in this.columnMappers.Where (x => x.Show))
			{
				AbstractFieldController field = new TextFieldController (this.controller, line, mapper, this.HandleSetFocus, this.EditorTextChanged);
				field.CreateUI (editorFrame);

				if (mapper.Column == ColumnType.Numéro ||
					mapper.Column == ColumnType.Titre  ||
					mapper.Column == ColumnType.Solde)
				{
					field.IsReadOnly = true;
				}

				if (mapper.Column == ColumnType.Solde           ||
					mapper.Column == ColumnType.BudgetPrécédent ||
					mapper.Column == ColumnType.Budget          ||
					mapper.Column == ColumnType.BudgetFutur     )
				{
					field.EditWidget.ContentAlignment = ContentAlignment.MiddleRight;
				}

				field.Box.TabIndex = ++tabIndex;

				this.fieldControllers[line].Add (field);
			}
		}

		protected override FormattedText GetOperationDescription(bool modify)
		{
			return modify ? "Modification du budget d'un compte :" : "";
		}


		public override void UpdateEditorContent()
		{
			foreach (var field in this.fieldControllers[0])
			{
				field.EditWidget.Visibility = this.dataAccessor.IsModification;
			}

			base.UpdateEditorContent ();
		}
	}
}
