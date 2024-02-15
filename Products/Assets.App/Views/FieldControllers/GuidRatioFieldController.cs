//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Common.Drawing;
using Epsitec.Common.Types;
using Epsitec.Common.Widgets;
using Epsitec.Cresus.Assets.App.Popups;
using Epsitec.Cresus.Assets.App.Widgets;
using Epsitec.Cresus.Assets.Core.Helpers;
using Epsitec.Cresus.Assets.Data;
using Epsitec.Cresus.Assets.Server.BusinessLogic;
using Epsitec.Cresus.Assets.Server.SimpleEngine;

namespace Epsitec.Cresus.Assets.App.Views.FieldControllers
{
	public class GuidRatioFieldController : AbstractFieldController
	{
		public GuidRatioFieldController(DataAccessor accessor)
			: base (accessor)
		{
		}


		public DataAccessor						Accessor;

		public GuidRatio						Value
		{
			get
			{
				return this.value;
			}
			set
			{
				if (this.value != value)
				{
					this.value = value;

					if (this.button != null)
					{
						this.button.Text = this.GuidToString (this.value.Guid);
					}

					if (this.textField != null)
					{
						if (this.ignoreChanges.IsZero)
						{
							using (this.ignoreChanges.Enter ())
							{
								this.textField.Text = this.ConvDecimalToString (this.value.Ratio);
								this.textField.SelectAll ();
							}
						}
					}
				}
			}
		}

		private void UpdateValue()
		{
			using (this.ignoreChanges.Enter ())
			{
				this.textField.Text = this.ConvDecimalToString (this.value.Ratio);
				this.textField.SelectAll ();
			}
		}

		protected override void ClearValue()
		{
			this.Value = GuidRatio.Empty;
			this.OnValueEdited (this.Field);
		}

		protected override void UpdatePropertyState()
		{
			base.UpdatePropertyState ();

			var type = AbstractFieldController.GetFieldColorType (this.propertyState, isLocked: this.isReadOnly, isError: this.hasError);

			if (this.button != null)
			{
				AbstractFieldController.UpdateButton    (this.button,    type, this.isReadOnly);
				AbstractFieldController.UpdateTextField (this.textField, type, this.isReadOnly);
			}
		}


		public override void CreateUI(Widget parent)
		{
			base.CreateUI (parent);

			this.button = new ColoredButton
			{
				Parent           = this.frameBox,
				HoverColor       = ColorManager.HoverColor,
				ContentAlignment = ContentAlignment.MiddleLeft,
				Dock             = DockStyle.Left,
				PreferredWidth   = this.EditWidth - 50,
				PreferredHeight  = AbstractFieldController.lineHeight,
				Margins          = new Margins (0, 4, 0, 0),
				TabIndex         = ++this.TabIndex,
				Text             = this.GuidToString (this.value.Guid),
			};

			//	Petit triangle "v" par-dessus la droite du bouton principal, sans fond
			//	afin de prendre la couleur du bouton principal.
			var arrowButton = new GlyphButton
			{
				Parent           = this.button,
				GlyphShape       = GlyphShape.TriangleDown,
				ButtonStyle      = ButtonStyle.ToolItem,
				Anchor           = AnchorStyles.Right,
				PreferredWidth   = AbstractFieldController.lineHeight,
				PreferredHeight  = AbstractFieldController.lineHeight,
			};

			this.textField = new TextField
			{
				Parent          = this.frameBox,
				Dock            = DockStyle.Left,
				PreferredWidth  = 50,
				PreferredHeight = AbstractFieldController.lineHeight,
				Margins         = new Margins (0, 10, 0, 0),
				TabIndex        = ++this.TabIndex,
				Text            = this.ConvDecimalToString (this.value.Ratio),
			};

			this.UpdatePropertyState ();

			//	Connexion des événements.
			this.button.Clicked += delegate
			{
				this.ShowPopup ();
			};

			this.button.KeyboardFocusChanged += delegate (object sender, DependencyPropertyChangedEventArgs e)
			{
				bool focused = (bool) e.NewValue;

				if (focused)  // pris le focus ?
				{
					base.SetFocus ();
				}
				else  // perdu le focus ?
				{
				}
			};

			arrowButton.Clicked += delegate
			{
				this.ShowPopup ();
			};

			this.textField.TextChanged += delegate
			{
				if (this.ignoreChanges.IsZero)
				{
					using (this.ignoreChanges.Enter ())
					{
						var ratio = this.ConvStringToDecimal (this.textField.Text);
						this.Value = new GuidRatio (this.value.Guid, ratio);
						this.OnValueEdited (this.Field);
					}
				}
			};

			this.textField.KeyboardFocusChanged += delegate (object sender, DependencyPropertyChangedEventArgs e)
			{
				bool focused = (bool) e.NewValue;

				if (focused)  // pris le focus ?
				{
					this.SetFocus ();
				}
				else  // perdu le focus ?
				{
					this.UpdateValue ();
				}
			};
		}

		public override void SetFocus()
		{
			this.textField.SelectAll ();
			this.textField.Focus ();

			base.SetFocus ();
		}


		private void ShowPopup()
		{
			GroupsPopup.Show (this.button, this.accessor, BaseType.Groups, this.Value.Guid, delegate (Guid selectedGuid)
			{
				this.Value = new GuidRatio (selectedGuid, this.value.Ratio);
				this.OnValueEdited (this.Field);

				this.textField.Focus ();
			});
		}

		private string GuidToString(Guid guid)
		{
			return GroupsLogic.GetFullName (this.Accessor, guid);
		}


		private string ConvDecimalToString(decimal? value)
		{
			return TypeConverters.RateToString (value);
		}

		private decimal? ConvStringToDecimal(string text)
		{
			return TypeConverters.ParseRate (text);
		}


		private ColoredButton					button;
		private TextField						textField;

		private GuidRatio						value;
	}
}
