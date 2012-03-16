//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using Epsitec.Common.Drawing;
using Epsitec.Common.Widgets;
using Epsitec.Common.Types;
using Epsitec.Common.Types.Converters;
using Epsitec.Common.Support;

using Epsitec.Cresus.Compta.Accessors;
using Epsitec.Cresus.Compta.Entities;
using Epsitec.Cresus.Compta.Controllers;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Compta.Fields.Controllers
{
	/// <summary>
	/// Contrôleur permettant l'édition d'un champ booléen.
	/// </summary>
	public class CheckButtonController : AbstractFieldController
	{
		public CheckButtonController(AbstractController controller, int line, ColumnMapper columnMapper, System.Action<int, ColumnType> clearFocusAction, System.Action<int, ColumnType> setFocusAction, System.Action<int, ColumnType> contentChangedAction)
			: base (controller, line, columnMapper, clearFocusAction, setFocusAction, contentChangedAction)
		{
		}


		public override void CreateUI(Widget parent)
		{
			this.CreateBoxUI (parent);
			this.box.DrawFullFrame = false;
			this.box.Margins = this.HasRightEditor ? new Margins (0, 0, 0, 0) : new Margins (0, 1, 0, 0);

			this.container = new FrameBox
			{
				Parent          = this.box,
				PreferredHeight = 20,
				Dock            = DockStyle.Fill,
				TabIndex        = 1,
			};

			this.editWidget = new CheckButton
			{
				Parent          = this.container,
				FormattedText   = this.columnMapper.Description,
				PreferredHeight = 20,
				Dock            = DockStyle.Fill,
				TabIndex        = 1,
				Margins         = new Margins (this.HasRightEditor ? 0:10, 0, 0, 0),
			};

			this.InternalButton.ActiveStateChanged += new EventHandler (this.HandleActiveStateChanged);
			this.editWidget.IsFocusedChanged += new EventHandler<DependencyPropertyChangedEventArgs> (this.HandleIsFocusedChanged);
		}


		public override bool IsReadOnly
		{
			get
			{
				return !this.InternalButton.Enable;
			}
			protected set
			{
				this.InternalButton.Enable = !value;
			}
		}

		public override void SetFocus()
		{
			this.InternalButton.Focus ();
		}

		public override void EditionDataToWidget()
		{
			if (this.editionData != null)
			{
				using (this.ignoreChanges.Enter ())
				{
					this.InternalButton.ActiveState = (this.editionData.Text == "1") ? ActiveState.Yes : ActiveState.No;
					this.IsReadOnly = !this.editionData.Enable || !this.columnMapper.Enable;
				}
			}
		}

		public override void WidgetToEditionData()
		{
			if (this.editionData != null)
			{
				this.editionData.Text = (this.InternalButton.ActiveState == ActiveState.Yes) ? "1":"0";
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


		private void HandleActiveStateChanged(object sender)
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
			else
			{
				this.ClearFocusAction ();
			}
		}


		private CheckButton InternalButton
		{
			get
			{
				return this.editWidget as CheckButton;
			}
		}
	}
}
