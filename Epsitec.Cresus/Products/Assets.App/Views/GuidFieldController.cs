//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Common.Drawing;
using Epsitec.Common.Widgets;
using Epsitec.Cresus.Assets.App.Popups;
using Epsitec.Cresus.Assets.App.Widgets;
using Epsitec.Cresus.Assets.Server.BusinessLogic;
using Epsitec.Cresus.Assets.Server.SimpleEngine;

namespace Epsitec.Cresus.Assets.App.Views
{
	public class GuidFieldController : AbstractFieldController
	{
		public DataAccessor						Accessor;
		public BaseType							BaseType;

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
			this.OnValueEdited ();
		}

		protected override void UpdatePropertyState()
		{
			base.UpdatePropertyState ();

			if (this.button != null)
			{
				if (this.propertyState == PropertyState.Readonly)
				{
					this.button.NormalColor = ColorManager.ReadonlyFieldColor;
				}
				else
				{
					if (this.BackgroundColor.IsVisible)
					{
						this.button.NormalColor = this.BackgroundColor;
					}
					else
					{
						this.button.NormalColor = ColorManager.NormalFieldColor;
					}
				}
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
				Margins          = new Margins (0, 10, 0, 0),
				TabIndex         = this.TabIndex,
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

			this.button.Clicked += delegate
			{
				this.ShowPopup ();
			};

			arrowButton.Clicked += delegate
			{
				this.ShowPopup ();
			};
		}


		private void ShowPopup()
		{
			var popup = new GroupsPopup (this.Accessor, this.Value);

			popup.Create (this.button, leftOrRight: false);

			popup.Navigate += delegate (object sender, Guid guid)
			{
				this.Value = guid;
				this.OnValueEdited ();
			};
		}

		private string GuidToString(Guid guid)
		{
			return GroupsLogic.GetFullName (this.Accessor, guid);
		}


		private ColoredButton					button;
		private Guid							value;
	}
}
