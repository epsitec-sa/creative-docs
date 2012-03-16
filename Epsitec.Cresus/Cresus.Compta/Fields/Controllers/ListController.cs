//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
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
	public class ListController : AbstractFieldController
	{
		public ListController(AbstractController controller, int line, ColumnMapper columnMapper, System.Action<int, ColumnType> clearFocusAction, System.Action<int, ColumnType> setFocusAction, System.Action<int, ColumnType> contentChangedAction)
			: base (controller, line, columnMapper, clearFocusAction, setFocusAction, contentChangedAction)
		{
		}


		public override void CreateUI(Widget parent)
		{
			this.CreateLabelUI (parent);
			this.CreateBoxUI (parent);

			this.container = new FrameBox
			{
				Parent          = this.box,
				PreferredHeight = 20,
				Dock            = DockStyle.Fill,
				TabIndex        = 1,
			};

			StaticText field;
			this.comboFrame = UIBuilder.CreatePseudoCombo (this.container, out field, out this.menuButton);
			this.comboFrame.Dock = DockStyle.Fill;
			this.comboFrame.Margins = new Margins (0);
			this.editWidget = field;

			this.editWidget.Clicked += delegate
			{
				this.ShowMenu (this.comboFrame);
			};

			this.menuButton.Clicked += delegate
			{
				this.ShowMenu (this.comboFrame);
			};
		}


		public override bool IsReadOnly
		{
			get
			{
				return !this.comboFrame.Enable;
			}
			protected set
			{
				this.comboFrame.Enable = !value;
			}
		}

		public override void SetFocus()
		{
			this.InternalField.Focus ();
		}

		public override void EditionDataToWidget()
		{
			if (this.editionData != null)
			{
				using (this.ignoreChanges.Enter ())
				{
					//	Il ne faut surtout pas utiliser this.InternalField.FormattedText directement !
					this.InternalField.FormattedText = this.GetSummary;
					this.IsReadOnly = !this.editionData.Enable || !this.columnMapper.Enable;
				}
			}
		}

		public override void WidgetToEditionData()
		{
			if (this.editionData != null)
			{
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

			UIBuilder.SetErrorPseudoCombo (this.InternalField, this.editionData.HasError);

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


		private void ShowMenu(Widget parentButton)
		{
			//	Affiche le menu permettant de choisir le mode.
			var menu = new VMenu ();

			foreach (var param in this.editionData.Parameters)
			{
				bool selected = this.editionData.Texts.Contains (param);

				var item = new MenuItem ()
				{
					IconUri       = UIBuilder.GetCheckStateIconUri (selected),
					FormattedText = param,
				};

				item.Clicked += delegate
				{
					if (this.editionData.Texts.Contains (item.FormattedText))
					{
						this.editionData.Texts.Remove (item.FormattedText);
					}
					else
					{
						this.editionData.Texts.Add (item.FormattedText);
						this.EditionDataSort ();
					}

					this.InternalField.FormattedText = this.GetSummary;
					this.Validate ();
					this.ContentChangedAction ();
				};

				menu.Items.Add (item);
			}

			if (menu.Items.Any ())
			{
				TextFieldCombo.AdjustComboSize (parentButton, menu, false);

				menu.Host = parentButton.Window;
				menu.ShowAsComboList (parentButton, Point.Zero, parentButton);
			}
		}

		private void EditionDataSort()
		{
			var sorted = new List<FormattedText> ();

			foreach (var parameter in this.editionData.Parameters)
			{
				if (this.editionData.Texts.Contains (parameter))
				{
					sorted.Add (parameter);
				}
			}

			this.editionData.Texts.Clear ();
			this.editionData.Texts.AddRange (sorted);
		}

		private FormattedText GetSummary
		{
			get
			{
				return string.Join (", ", this.editionData.Texts);				
			}
		}


		private StaticText InternalField
		{
			get
			{
				return this.editWidget as StaticText;
			}
		}


		private FrameBox				comboFrame;
		private GlyphButton				menuButton;
	}
}
