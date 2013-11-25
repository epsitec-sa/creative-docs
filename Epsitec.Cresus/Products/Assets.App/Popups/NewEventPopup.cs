//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Common.Drawing;
using Epsitec.Common.Widgets;
using Epsitec.Cresus.Assets.App.Helpers;
using Epsitec.Cresus.Assets.App.Views;
using Epsitec.Cresus.Assets.App.Widgets;
using Epsitec.Cresus.Assets.Server.BusinessLogic;
using Epsitec.Cresus.Assets.Server.SimpleEngine;

namespace Epsitec.Cresus.Assets.App.Popups
{
	public class NewEventPopup : AbstractPopup
	{
		public BaseType							BaseType;
		public DataObject						DataObject;
		public Timestamp						Timestamp;

		protected override Size					DialogSize
		{
			get
			{
				return this.GetDialogSize (this.ButtonDescriptions.Count ());
			}
		}

		public override void CreateUI()
		{
			this.CreateDateUI ();

			this.buttonsFrame = new FrameBox
			{
				Parent  = this.mainFrameBox,
				Anchor  = AnchorStyles.All,
				Margins = new Margins (NewEventPopup.horizontalMargins, NewEventPopup.horizontalMargins, 0, NewEventPopup.verticalMargins),
			};

			this.CreateButtons ();
			this.CreateCloseButton ();
		}

		private void CreateDateUI()
		{
			var frame = new FrameBox
			{
				Parent          = this.mainFrameBox,
				Dock            = DockStyle.Top,
				PreferredHeight = NewEventPopup.titleHeight,
				BackColor       = ColorManager.TreeTableBackgroundColor,
				Padding         = new Margins (0, 0, 1, 0),
			};

			this.dateController = new DateFieldController
			{
				Label      = "Crée un événement le",
				LabelWidth = 110,
				Value      = this.Timestamp.Date,
			};

			this.dateController.HideAdditionalButtons = true;
			this.dateController.CreateUI (frame);
			this.dateController.SetFocus ();

			this.dateController.ValueEdited += delegate
			{
				this.Timestamp = new Timestamp (this.dateController.Value.Value, 0);
				this.OnDateChanged (this.dateController.Value);
				this.UpdateButtons ();
			};
		}

		private void UpdateButtons()
		{
			this.ChangeDialogSize (this.DialogSize);
			this.CreateButtons ();
		}

		private void CreateButtons()
		{
			this.buttonsFrame.Children.Clear ();

			int dx = NewEventPopup.buttonWidth;
			int dy = NewEventPopup.buttonHeight;
			int x = 0;
			int y = 0;

			foreach (var desc in this.ButtonDescriptions.Reverse ())
			{
				this.CreateButton (x, y, dx, dy, desc.Type, desc.Text, desc.Tooltip);
				y += dy + NewEventPopup.buttonGap;
			}
		}

		private Button CreateButton(int x, int y, int dx, int dy, EventType type, string text, string tooltip = null)
		{
			string name = type.ToString();

			var button = new Button
			{
				Parent        = this.buttonsFrame,
				Name          = name,
				Text          = text,
				ButtonStyle   = ButtonStyle.Icon,
				Dock          = DockStyle.Bottom,
				PreferredSize = new Size (dx, dy),
				Margins       = new Margins (0, 0, NewEventPopup.buttonGap, 0),
			};

			if (!string.IsNullOrEmpty (tooltip))
			{
				ToolTip.Default.SetToolTip (button, tooltip);
			}

			button.Clicked += delegate
			{
				this.ClosePopup ();
				this.OnButtonClicked (button.Name);
			};

			return button;
		}


		private Size GetDialogSize(int buttonCount)
		{
			int dx = NewEventPopup.horizontalMargins*2 + NewEventPopup.buttonWidth;
			int dy = NewEventPopup.verticalMargins*2 + NewEventPopup.titleHeight + NewEventPopup.buttonHeight*buttonCount + NewEventPopup.buttonGap*(buttonCount-1);

			return new Size (dx, dy);
		}


		private IEnumerable<ButtonDescription> ButtonDescriptions
		{
			get
			{
				foreach (var type in ObjectCalculator.GetPlausibleEventTypes (this.BaseType, this.DataObject, this.Timestamp))
				{
					yield return this.GetButtonDescription (type);
				}
			}
		}


		private ButtonDescription GetButtonDescription(EventType type)
		{
			switch (this.BaseType)
			{
				case BaseType.Objects:
					return this.GetObjectButtonDescription (type);

				case BaseType.Categories:
					return this.GetCategoryButtonDescription (type);

				case BaseType.Groups:
					return this.GetGroupButtonDescription (type);

				default:
					return new ButtonDescription (EventType.Unknown, null, null);
			}
		}

		private ButtonDescription GetObjectButtonDescription(EventType type)
		{
			switch (type)
			{
				case EventType.Entrée:
					return new ButtonDescription (EventType.Entrée, "Entrée", "Entrée dans l'inventaire, acquisition");

				case EventType.Modification:
					return new ButtonDescription (EventType.Modification, "Modification", "Modification de diverses informations");

				case EventType.Réorganisation:
					return new ButtonDescription (EventType.Réorganisation, "Réorganisation", "Modification pour MCH2");
				
				case EventType.Augmentation:
					return new ButtonDescription (EventType.Augmentation, "Augmentation", "Revalorisation à la hausse de la valeur");
				
				case EventType.Diminution:
					return new ButtonDescription (EventType.Diminution, "Diminution", "Réévaluation à la baisse de la valeur");
				
				case EventType.AmortissementExtra:
					return new ButtonDescription (EventType.AmortissementExtra, "Amortissement extraordinaire", "Amortissement manuel");
				
				case EventType.Sortie:
					return new ButtonDescription (EventType.Sortie, "Sortie", "Sortie de l'inventaire, vente, vol, destruction, etc.");
				
				default:
					return new ButtonDescription (EventType.Unknown, null, null);
			}
		}

		private ButtonDescription GetCategoryButtonDescription(EventType type)
		{
			switch (type)
			{
				case EventType.Entrée:
					return new ButtonDescription (EventType.Entrée, "Création", "Création de la catégorie d'immobilisation");
				
				case EventType.Modification:
					return new ButtonDescription (EventType.Modification, "Modification", "Modification de la catégorie d'immobilisation");
				
				case EventType.Sortie:
					return new ButtonDescription (EventType.Sortie, "Suppression", "Suppression de la catégorie d'immobilisation");
				
				default:
					return new ButtonDescription (EventType.Unknown, null, null);
			}
		}

		private ButtonDescription GetGroupButtonDescription(EventType type)
		{
			switch (type)
			{
				case EventType.Entrée:
					return new ButtonDescription (EventType.Entrée, "Création", "Création du goupe");

				case EventType.Modification:
					return new ButtonDescription (EventType.Modification, "Modification", "Modification du groupe");

				case EventType.Sortie:
					return new ButtonDescription (EventType.Sortie, "Suppression", "Suppression du groupe");

				default:
					return new ButtonDescription (EventType.Unknown, null, null);
			}
		}


		private struct ButtonDescription
		{
			public ButtonDescription(EventType type, string text, string tooltip)
			{
				this.Type    = type;
				this.Text    = text;
				this.Tooltip = tooltip;
			}

			public readonly EventType	Type;
			public readonly string		Text;
			public readonly string		Tooltip;
		};


		#region Events handler
		private void OnDateChanged(System.DateTime? dateTime)
		{
			this.DateChanged.Raise (this, dateTime);
		}

		public event EventHandler<System.DateTime?> DateChanged;
		#endregion


		private static readonly int horizontalMargins = 40;
		private static readonly int verticalMargins   = 20;
		private static readonly int titleHeight       = AbstractPopup.TitleHeight;
		private static readonly int buttonWidth       = 180;
		private static readonly int buttonHeight      = 24;
		private static readonly int buttonGap         = 1;

		private DateFieldController dateController;
		private FrameBox buttonsFrame;
	}
}