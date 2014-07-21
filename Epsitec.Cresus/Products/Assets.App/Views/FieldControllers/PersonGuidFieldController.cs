﻿//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Common.Drawing;
using Epsitec.Common.Widgets;
using Epsitec.Cresus.Assets.App.Popups;
using Epsitec.Cresus.Assets.App.Widgets;
using Epsitec.Cresus.Assets.Data;
using Epsitec.Cresus.Assets.Server.BusinessLogic;
using Epsitec.Cresus.Assets.Server.SimpleEngine;

namespace Epsitec.Cresus.Assets.App.Views.FieldControllers
{
	public class PersonGuidFieldController : AbstractFieldController
	{
		public PersonGuidFieldController(DataAccessor accessor)
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

					this.UpdateButtons ();
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
				AbstractFieldController.UpdateButton (this.button, this.PropertyState, this.isReadOnly);

				this.UpdateButtons ();
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
				PreferredWidth   = this.EditWidth-AbstractFieldController.lineHeight,
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

			this.CreateGotoPersonButton ();
			this.UpdatePropertyState ();

			//	Connexion des événements.
			this.button.Clicked += delegate
			{
				this.ShowPopup ();
			};

			arrowButton.Clicked += delegate
			{
				this.ShowPopup ();
			};
		}

		private void CreateGotoPersonButton()
		{
			this.gotoButton = this.CreateGotoButton ();

			ToolTip.Default.SetToolTip (this.gotoButton, "Montrer les détails du contact");

			this.gotoButton.Clicked += delegate
			{
				if (!this.value.IsEmpty)
				{
					var viewState = PersonsView.GetViewState (this.value, ObjectField.Unknown);
					this.OnGoto (viewState);
				}
			};
		}

		private void UpdateButtons()
		{
			if (this.button != null)
			{
				var text = PersonsLogic.GetFullDescription (this.Accessor, this.value);
				ToolTip.Default.SetToolTip (this.button, text);
			}

			if (this.gotoButton != null)
			{
				this.gotoButton.Visibility = !this.value.IsEmpty;
			}
		}


		private void ShowPopup()
		{
			var popup = new PersonsPopup (this.Accessor, this.Value);

			popup.Create (this.button, leftOrRight: false);

			popup.Navigate += delegate (object sender, Guid guid)
			{
				this.Value = guid;
				this.OnValueEdited (this.Field);
			};
		}

		private string GuidToString(Guid guid)
		{
			return PersonsLogic.GetSummary (this.Accessor, guid);
		}


		private ColoredButton					button;
		private IconButton						gotoButton;
		private Guid							value;
	}
}
