//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Common.Drawing;
using Epsitec.Common.Widgets;
using Epsitec.Cresus.Assets.App.Views;
using Epsitec.Cresus.Assets.App.Widgets;
using Epsitec.Cresus.Assets.Server.SimpleEngine;

namespace Epsitec.Cresus.Assets.App.Popups
{
	/// <summary>
	/// Popup permettant la saisir des informations nécessaires à la création d'un
	/// nouveau mandat.
	/// </summary>
	public class CreateMandatPopup : AbstractPopup
	{
		public CreateMandatPopup(DataAccessor accessor)
		{
			this.accessor = accessor;
		}


		public string							MandatFactoryName;
		public string							MandatName;
		public System.DateTime					MandatStartDate;
		public bool								MandatWithSamples;


		protected override Size DialogSize
		{
			get
			{
				return new Size (CreateMandatPopup.popupWidth, 180+this.FactoriesCount*CreateMandatPopup.radioHeight);
			}
		}

		public override void CreateUI()
		{
			this.CreateTitle ("Création d'un nouveau mandat");

			var line1 = this.CreateFrame (CreateMandatPopup.margin, 140, CreateMandatPopup.popupWidth-CreateMandatPopup.margin*2, this.FactoriesCount*CreateMandatPopup.radioHeight);
			var line2 = this.CreateFrame (CreateMandatPopup.margin, 110, CreateMandatPopup.popupWidth-CreateMandatPopup.margin*2, CreateMandatPopup.lineHeight);
			var line3 = this.CreateFrame (CreateMandatPopup.margin,  80, CreateMandatPopup.popupWidth-CreateMandatPopup.margin*2, CreateMandatPopup.lineHeight);
			var line4 = this.CreateFrame (CreateMandatPopup.margin,  50, CreateMandatPopup.popupWidth-CreateMandatPopup.margin*2, CreateMandatPopup.lineHeight);

			this.CreateFactories (line1);
			this.CreateSamples   (line2);
			this.CreateName      (line3);
			this.CreateDate      (line4);
			this.CreateButtons   ();

			this.UpdateButtons ();
			this.textField.Focus ();
		}

		private void CreateFactories(Widget parent)
		{
			foreach (var factory in MandatFactory.Factories)
			{
				var radio = new RadioButton
				{
					Parent          = parent,
					Text            = factory.Name,
					PreferredHeight = CreateMandatPopup.radioHeight,
					AutoFocus       = false,
					ActiveState     = (this.MandatFactoryName == factory.Name) ? ActiveState.Yes : ActiveState.No,
					Dock            = DockStyle.Top,
					Margins         = new Margins (CreateMandatPopup.indent+10, 0, 0, 0),
				};

				radio.Clicked += delegate
				{
					this.MandatFactoryName = factory.Name;
				};
			}
		}

		private void CreateSamples(Widget parent)
		{
			var button = new CheckButton
			{
				Parent      = parent,
				Text        = "Avec les exemples",
				AutoFocus   = false,
				ActiveState = ActiveState.No,
				Dock        = DockStyle.Top,
				Margins     = new Margins (CreateMandatPopup.indent+10, 0, 0, 0),
			};

			button.ActiveStateChanged += delegate
			{
				this.MandatWithSamples = button.ActiveState == ActiveState.Yes;
			};
		}

		private void CreateName(Widget parent)
		{
			new StaticText
			{
				Parent           = parent,
				Text             = "Nom du mandat",
				ContentAlignment = ContentAlignment.MiddleRight,
				Dock             = DockStyle.Left,
				PreferredWidth   = CreateMandatPopup.indent,
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
				TabIndex         = ++this.tabIndex,
			};

			this.textField.TextChanged += delegate
			{
				this.MandatName = this.textField.Text;
				this.UpdateButtons ();
			};
		}

		private void CreateDate(Widget parent)
		{
			var line = new FrameBox
			{
				Parent          = parent,
				Dock            = DockStyle.Top,
				PreferredHeight = CreateMandatPopup.lineHeight,
			};

			new StaticText
			{
				Parent           = line,
				Text             = "Date de début",
				ContentAlignment = ContentAlignment.MiddleRight,
				PreferredWidth   = CreateMandatPopup.indent,
				Dock             = DockStyle.Left,
				Margins          = new Margins (0, 10, 0, 0),
			};

			var dateFrame = new FrameBox
			{
				Parent    = line,
				Dock      = DockStyle.Fill,
				BackColor = ColorManager.WindowBackgroundColor,
			};

			var controller = new DateFieldController (this.accessor)
			{
				Field                 = ObjectField.Unknown,
				Label                 = null,
				LabelWidth            = 0,
				HideAdditionalButtons = false,
				Value                 = this.MandatStartDate,
				TabIndex              = ++this.tabIndex,
			};

			controller.CreateUI (dateFrame);

			controller.ValueEdited += delegate (object sender, ObjectField of)
			{
				if (controller.Value.HasValue)
				{
					this.MandatStartDate = controller.Value.Value;
				}
			};
		}

		private void CreateButtons()
		{
			var footer = this.CreateFooter ();

			this.createButton = this.CreateFooterButton (footer, DockStyle.Left,  "create", "Créer");
			this.cancelButton = this.CreateFooterButton (footer, DockStyle.Right, "cancel", "Annuler");
		}


		private void UpdateButtons()
		{
			this.createButton.Enable = !string.IsNullOrEmpty (this.MandatName);
		}


		private int FactoriesCount
		{
			get
			{
				return MandatFactory.Factories.Count ();
			}
		}


		private const int lineHeight  = 2+AbstractFieldController.lineHeight+2;
		private const int radioHeight = 18;
		private const int indent      = 80;
		private const int popupWidth  = CreateMandatPopup.margin*2 + 10 + CreateMandatPopup.indent + DateController.controllerWidth;
		private const int margin      = 20;

		private readonly DataAccessor						accessor;

		private int											tabIndex;
		private TextField									textField;
		private Button										createButton;
		private Button										cancelButton;
	}
}