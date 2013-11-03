//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Common.Drawing;
using Epsitec.Common.Widgets;
using Epsitec.Cresus.Assets.App.Views;
using Epsitec.Cresus.Assets.Server.SimpleEngine;

namespace Epsitec.Cresus.Assets.App.Popups
{
	public class NewEventPopup : AbstractPopup
	{
		public BaseType							BaseType;
		public System.DateTime					Date;

		protected override Size					DialogSize
		{
			get
			{
				return this.GetDialogSize (this.ButtonDescriptions.Count ());
			}
		}

		protected override void CreateUI()
		{
			int dx = NewEventPopup.buttonWidth;
			int dy = NewEventPopup.buttonHeight;
			int x = NewEventPopup.horizontalMargins;
			int y = (int) this.DialogSize.Height - NewEventPopup.verticalMargins - NewEventPopup.titleHeight - dy;

			this.CreateDateUI ();

			foreach (var desc in this.ButtonDescriptions)
			{
				this.CreateButton (x, y, dx, dy, desc.Type, desc.Text, desc.Tooltip);
				y -= dy+NewEventPopup.buttonGap;
			}

			this.CreateCloseButton ();
		}

		private void CreateDateUI()
		{
			var frame = this.CreateTitleFrame (NewEventPopup.titleHeight);

			this.dateController = new DateFieldController
			{
				Label      = "Crée un événement le",
				LabelWidth = 110,
				Value      = this.Date,
			};

			this.dateController.HideAdditionalButtons = true;
			this.dateController.CreateUI (frame);
			this.dateController.SetFocus ();

			this.dateController.ValueEdited += delegate
			{
				this.OnDateChanged (this.dateController.Value);
			};
		}

		private Button CreateButton(int x, int y, int dx, int dy, EventType type, string text, string tooltip = null)
		{
			string name = type.ToString();
			return this.CreateButton (x, y, dx, dy, name, text, tooltip);
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
				switch (this.BaseType)
				{
					case BaseType.Objects:
						yield return new ButtonDescription (EventType.Entrée,             "Entrée",                       "Entrée dans l'inventaire, acquisition");
						yield return new ButtonDescription (EventType.Modification,       "Modification",                 "Modification de diverses informations");
						yield return new ButtonDescription (EventType.Réorganisation,     "Réorganisation",               "Modification pour MCH2");
						yield return new ButtonDescription (EventType.Augmentation,       "Augmentation",                 "Revalorisation à la hausse de la valeur");
						yield return new ButtonDescription (EventType.Diminution,         "Diminution",                   "Réévaluation à la baisse de la valeur");
						yield return new ButtonDescription (EventType.AmortissementExtra, "Amortissement extraordinaire", "Amortissement manuel");
						yield return new ButtonDescription (EventType.Sortie,             "Sortie",                       "Sortie de l'inventaire, vente, vol, destruction, etc.");
						break;

					case BaseType.Categories:
						yield return new ButtonDescription (EventType.Entrée,       "Création",     "Création de la catégorie d'immobilisation");
						yield return new ButtonDescription (EventType.Modification, "Modification", "Modification de la catégorie d'immobilisation");
						yield return new ButtonDescription (EventType.Sortie,       "Suppression",  "Suppression de la catégorie d'immobilisation");
						break;
				}
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
			if (this.DateChanged != null)
			{
				this.DateChanged (this, dateTime);
			}
		}

		public delegate void DateChangedEventHandler(object sender, System.DateTime? dateTime);
		public event DateChangedEventHandler DateChanged;
		#endregion


		private static readonly int horizontalMargins = 40;
		private static readonly int verticalMargins   = 20;
		private static readonly int titleHeight       = 24;
		private static readonly int buttonWidth       = 180;
		private static readonly int buttonHeight      = 24;
		private static readonly int buttonGap         = 1;

		private DateFieldController dateController;
	}
}