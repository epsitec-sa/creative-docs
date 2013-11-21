//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Common.Drawing;
using Epsitec.Common.Widgets;
using Epsitec.Cresus.Assets.App.Widgets;
using Epsitec.Cresus.Assets.Server.SimpleEngine;

namespace Epsitec.Cresus.Assets.App.Popups
{
	public class CreateCategoryPopup : AbstractPopup
	{
		public CreateCategoryPopup(DataAccessor accessor, BaseType baseType, Guid selectedGuid)
		{
			this.accessor = accessor;
			this.baseType = baseType;
		}


		public string							ObjectName;


		protected override Size DialogSize
		{
			get
			{
				return new Size (CreateCategoryPopup.PopupWidth, CreateCategoryPopup.PopupHeight);
			}
		}

		public override void CreateUI()
		{
			this.CreateTitle (this.mainFrameBox);
			this.CreateCloseButton ();

			var line1 = this.CreateFrame (CreateCategoryPopup.Margin, 67, CreateCategoryPopup.PopupWidth-CreateCategoryPopup.Margin*2, CreateCategoryPopup.LineHeight);
			var line2 = this.CreateFrame (CreateCategoryPopup.Margin, 20, CreateCategoryPopup.PopupWidth-CreateCategoryPopup.Margin*2, 24);

			this.CreateName    (line1);
			this.CreateButtons (line2);

			this.UpdateButtons ();
			this.textField.Focus ();
		}

		private void CreateTitle(Widget parent)
		{
			new StaticText
			{
				Parent           = parent,
				Text             = "Création d'une nouvelle catégorie",
				ContentAlignment = ContentAlignment.MiddleCenter,
				Dock             = DockStyle.Top,
				PreferredHeight  = CreateCategoryPopup.TitleHeight - 4,
				BackColor        = ColorManager.SelectionColor,
			};

			new StaticText
			{
				Parent           = parent,
				Dock             = DockStyle.Top,
				PreferredHeight  = 4,
				BackColor        = ColorManager.SelectionColor,
			};
		}

		private void CreateName(Widget parent)
		{
			new StaticText
			{
				Parent           = parent,
				Text             = "Nom",
				ContentAlignment = ContentAlignment.MiddleRight,
				Dock             = DockStyle.Left,
				PreferredWidth   = CreateCategoryPopup.Indent,
				Margins          = new Margins (0, 10, 0, 0),
			};

			var frame = new FrameBox
			{
				Parent           = parent,
				Dock             = DockStyle.Fill,
				BackColor        = ColorManager.WindowBackgroundColor,
				Padding          = new Margins (2),
			};

			this.textField = new TextField
			{
				Parent           = frame,
				Dock             = DockStyle.Fill,
			};

			this.textField.TextChanged += delegate
			{
				this.ObjectName = this.textField.Text;
				this.UpdateButtons ();
			};
		}

		private void CreateButtons(Widget parent)
		{
			this.createButton = new Button
			{
				Parent        = parent,
				Name          = "create",
				Text          = "Créer",
				ButtonStyle   = ButtonStyle.Icon,
				AutoFocus     = false,
				Dock          = DockStyle.Left,
				PreferredSize = new Size (CreateCategoryPopup.PopupWidth/2 - CreateCategoryPopup.Margin - 5, parent.PreferredHeight),
				Margins       = new Margins (0, 5, 0, 0),
			};

			this.cancelButton = new Button
			{
				Parent        = parent,
				Name          = "cancel",
				Text          = "Annuler",
				ButtonStyle   = ButtonStyle.Icon,
				AutoFocus     = false,
				Dock          = DockStyle.Left,
				PreferredSize = new Size (CreateCategoryPopup.PopupWidth/2 - CreateCategoryPopup.Margin - 5, parent.PreferredHeight),
				Margins       = new Margins (5, 0, 0, 0),
			};

			this.createButton.Clicked += this.HandleButtonClicked;
			this.cancelButton.Clicked += this.HandleButtonClicked;
		}

		private void HandleButtonClicked(object sender, MessageEventArgs e)
		{
			var button = sender as Button;

			this.ClosePopup ();
			this.OnButtonClicked (button.Name);
		}


		private void UpdateButtons()
		{
			this.createButton.Enable = !string.IsNullOrEmpty (this.ObjectName);
		}


		private static readonly int TitleHeight = 24;
		private static readonly int LineHeight  = 2+17+2;
		private static readonly int Indent      = 40;
		private static readonly int PopupWidth  = 250;
		private static readonly int PopupHeight = 130;
		private static readonly int Margin      = 20;

		private readonly DataAccessor					accessor;
		private readonly BaseType						baseType;

		private TextField								textField;
		private Button									createButton;
		private Button									cancelButton;
	}
}