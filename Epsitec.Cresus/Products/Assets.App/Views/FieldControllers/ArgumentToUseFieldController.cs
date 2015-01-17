//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Common.Drawing;
using Epsitec.Common.Types;
using Epsitec.Common.Widgets;
using Epsitec.Cresus.Assets.App.Popups;
using Epsitec.Cresus.Assets.App.Widgets;
using Epsitec.Cresus.Assets.Data;
using Epsitec.Cresus.Assets.Server.BusinessLogic;
using Epsitec.Cresus.Assets.Server.SimpleEngine;

namespace Epsitec.Cresus.Assets.App.Views.FieldControllers
{
	public class ArgumentToUseFieldController : AbstractFieldController
	{
		public ArgumentToUseFieldController(DataAccessor accessor)
			: base (accessor)
		{
		}


		public DataAccessor						Accessor;

		public Guid								Value
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
						this.button.Text = this.GuidToString (this.value);
					}
				}
			}
		}

		protected override void ClearValue()
		{
			this.Value = Guid.Empty;
			this.OnValueEdited (this.Field);
		}

		protected override void UpdatePropertyState()
		{
			base.UpdatePropertyState ();

			if (this.button != null)
			{
				AbstractFieldController.UpdateButton (this.button, FieldColorType.Editable, this.isReadOnly);
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
				PreferredWidth   = this.EditWidth,
				PreferredHeight  = AbstractFieldController.lineHeight,
				Margins          = new Margins (0, 4, 0, 0),
				TabIndex         = ++this.TabIndex,
				Text             = this.GuidToString (this.value),
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
		}

		public override void SetFocus()
		{
			base.SetFocus ();
		}


		private void ShowPopup()
		{
			ArgumentsPopup.Show (this.button, this.accessor, this.value, delegate (Guid guid)
			{
				this.Value = guid;

				var field = ArgumentsLogic.GetObjectField (this.accessor, guid);
				this.OnValueEdited (field);
			});
		}

		private string GuidToString(Guid guid)
		{
			return ArgumentsLogic.GetSummary (this.Accessor, guid);
		}


		private ColoredButton					button;
		private Guid							value;
	}
}
