﻿//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using Epsitec.Common.Drawing;
using Epsitec.Common.Widgets;
using Epsitec.Common.Types;
using Epsitec.Common.Types.Converters;
using Epsitec.Common.Support;

using Epsitec.Cresus.Compta.Accessors;
using Epsitec.Cresus.Compta.Entities;
using Epsitec.Cresus.Compta.Widgets;
using Epsitec.Cresus.Compta.Helpers;
using Epsitec.Cresus.Compta.Controllers;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Compta.Fields.Controllers
{
	/// <summary>
	/// Contrôleur générique permettant l'édition d'un champ "liste".
	/// </summary>
	public class AutoCompleteFieldController : AbstractFieldController
	{
		public AutoCompleteFieldController(AbstractController controller, int line, ColumnMapper columnMapper, System.Action<int, ColumnType> setFocusAction = null, System.Action contentChangedAction = null)
			: base (controller, line, columnMapper, setFocusAction, contentChangedAction)
		{
		}


		public override void CreateUI(Widget parent)
		{
			this.CreateLabelUI (parent);

			this.box = new FrameBox
			{
				Parent          = parent,
				DrawFullFrame   = true,
				PreferredHeight = 20,
				Dock            = this.HasRightEditor ? DockStyle.Top : DockStyle.Left,
				Margins         = this.BoxMargins,
			};

			this.container = new FrameBox
			{
				Parent          = this.box,
				PreferredHeight = 20,
				Dock            = DockStyle.Fill,
				TabIndex        = 1,
			};

			AbstractTextField textField;
			UIBuilder.CreateAutoCompleteTextField (this.container, out this.container, out textField);
			this.editWidget = textField;

			this.editWidget.TextChanged += new EventHandler (this.HandleTextChanged);
			this.editWidget.IsFocusedChanged += new EventHandler<DependencyPropertyChangedEventArgs> (this.HandleIsFocusedChanged);
		}


		public List<FormattedText> PrimaryTexts
		{
			get
			{
				return this.InternalField.PrimaryTexts;
			}
		}

		public List<FormattedText> SecondaryTexts
		{
			get
			{
				return this.InternalField.SecondaryTexts;
			}
		}


		public override bool IsReadOnly
		{
			get
			{
				return this.InternalField.IsReadOnly;
			}
			protected set
			{
				this.InternalField.IsReadOnly = value;
				this.InternalField.Invalidate ();  // pour contourner un bug !
			}
		}

		public override void SetFocus()
		{
			this.InternalField.SelectAll ();
			this.InternalField.Focus ();
		}

		public override void EditionDataToWidget()
		{
			if (this.editionData != null)
			{
				using (this.ignoreChanges.Enter ())
				{
					this.InternalField.SetSilentFormattedText (this.editionData.Text);
					this.IsReadOnly = !this.editionData.Enable || !this.columnMapper.Enable;
				}
			}
		}

		public override void WidgetToEditionData()
		{
			if (this.editionData != null)
			{
				this.editionData.Text = this.InternalField.FormattedText;
				this.editionData.Validate ();
			}
		}

		public override void Validate()
		{
			if (this.editionData == null)
			{
				return;
			}

			this.WidgetToEditionData ();

			this.editWidget.SetError (this.editionData.HasError);

			if (this.editionData.HasError)
			{
				ToolTip.Default.SetToolTip (this.editWidget, this.editionData.Error);
			}
			else
			{
				if (this.columnMapper != null)
				{
					ToolTip.Default.SetToolTip (this.editWidget, this.columnMapper.Tooltip);
				}
			}
		}


		private void HandleTextChanged(object sender)
		{
			if (this.ignoreChanges.IsNotZero || this.editionData == null)
			{
				return;
			}

			this.Validate ();
			this.ContentChangedAction ();
		}

		private void HandleIsFocusedChanged(object sender, DependencyPropertyChangedEventArgs e)
		{
			if (this.editionData == null)
			{
				return;
			}

			this.hasFocus = (bool) e.NewValue;

			if (this.hasFocus)  // prise du focus ?
			{
				this.SetFocusAction ();
			}
			else  // perte du focus ?
			{
				//	La mise à jour du contenu lors de la perte du focus est déjà gérée par le widget lui-même.
			}
		}


		private AutoCompleteTextField InternalField
		{
			get
			{
				return this.editWidget as AutoCompleteTextField;
			}
		}
	}
}
