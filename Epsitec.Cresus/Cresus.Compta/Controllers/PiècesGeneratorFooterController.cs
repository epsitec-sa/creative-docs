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
	public class PiècesGeneratorFooterController : AbstractFooterController
	{
		public PiècesGeneratorFooterController(AbstractController controller)
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

			var footerFrame = new TabCatcherFrameBox
			{
				Parent          = parent,
				PreferredHeight = 20,
				Dock            = DockStyle.Bottom,
				Margins         = new Margins (0, 0, 1, 0),
			};

			footerFrame.TabPressed += new TabCatcherFrameBox.TabPressedEventHandler (this.HandleLinesContainerTabPressed);

			this.linesFrames.Add (footerFrame);
			int line = this.linesFrames.Count - 1;
			int tabIndex = 0;

			footerFrame.TabIndex = line+1;

			foreach (var mapper in this.columnMappers.Where (x => x.Show))
			{
				AbstractFieldController field = new TextFieldController (this.controller, line, mapper, this.HandleSetFocus, this.FooterTextChanged);
				field.CreateUI (footerFrame);

				if (mapper.Column == ColumnType.Numéro   ||
					mapper.Column == ColumnType.Digits   ||
					mapper.Column == ColumnType.Incrément)
				{
					field.EditWidget.ContentAlignment = ContentAlignment.MiddleRight;
				}

				if (mapper.Column == ColumnType.Résumé)
				{
					field.IsReadOnly = true;
				}

				field.Box.TabIndex = ++tabIndex;

				this.fieldControllers[line].Add (field);
			}
		}

		protected override FormattedText GetOperationDescription(bool modify)
		{
			return modify ? "Modification d'un générateur de numéros de pièces :" : "Création d'un générateur de numéros de pièces :";
		}
	}
}
