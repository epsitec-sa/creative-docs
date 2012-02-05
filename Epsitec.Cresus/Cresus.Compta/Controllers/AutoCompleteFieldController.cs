//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using Epsitec.Common.Drawing;
using Epsitec.Common.Widgets;
using Epsitec.Common.Types;
using Epsitec.Common.Types.Converters;
using Epsitec.Common.Support;

using Epsitec.Cresus.Core;
using Epsitec.Cresus.Core.Entities;
using Epsitec.Cresus.Core.Controllers;
using Epsitec.Cresus.Core.Library;
using Epsitec.Cresus.Core.Business;

using Epsitec.Cresus.Compta.Accessors;
using Epsitec.Cresus.Compta.Entities;
using Epsitec.Cresus.Compta.Widgets;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Compta.Controllers
{
	/// <summary>
	/// Contrôleur générique permettant l'édition d'un champ "liste".
	/// </summary>
	public class AutoCompleteFieldController : AbstractFieldController
	{
		public AutoCompleteFieldController(AbstractController controller, EditionData editionData, FormattedText description, System.Action contentChangedAction = null, System.Action<EditionData> validateAction = null, System.Func<FormattedText, FormattedText, string> adjustHintFunction = null)
			: base (controller, editionData, description, contentChangedAction, validateAction, adjustHintFunction)
		{
		}


		public override void CreateUI(Widget parent)
		{
			this.box = new FrameBox
			{
				Parent        = parent,
				DrawFullFrame = true,
				Dock          = DockStyle.Left,
				Margins       = new Margins (0, 1, 0, 0),
			};

			this.container = new FrameBox
			{
				Parent   = this.box,
				Dock     = DockStyle.Fill,
				TabIndex = 1,
			};

			AbstractTextField textField;
			UIBuilder.CreateAutoCompleteTextField (this.box, out this.container, out textField);
			this.field = textField;

			this.field.TextChanged += new EventHandler (this.HandleTextChanged);
			this.field.IsFocusedChanged += new EventHandler<DependencyPropertyChangedEventArgs> (this.HandleIsFocusedChanged);
		}

		private void HandleTextChanged(object sender)
		{
			this.ValidateAction ();

			this.field.SetError (this.editionData.HasError);

			if (this.editionData.HasError)
			{
				ToolTip.Default.SetToolTip (this.field, this.editionData.Error);
			}
			else
			{
				ToolTip.Default.SetToolTip (this.field, this.description);
			}

			this.ContentChangedAction ();
		}

		private void HandleIsFocusedChanged(object sender, DependencyPropertyChangedEventArgs e)
		{
		}


		private AutoCompleteTextField InternalField
		{
			get
			{
				return this.field as AutoCompleteTextField;
			}
		}
	}
}
