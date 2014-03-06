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
		public NewEventPopup(DataAccessor accessor)
		{
			this.accessor = accessor;
		}


		public BaseType							BaseType;
		public DataObject						DataObject;
		public Timestamp						Timestamp;

		protected override Size					DialogSize
		{
			get
			{
				return this.RequiredSize;
			}
		}

		public override void CreateUI()
		{
			this.CreateTitle ("Création d'un nouvel événement");

			var bh = this.ButtonsHeight;

			var dateFrame = this.CreateFrame
			(
				NewEventPopup.horizontalMargins,
				NewEventPopup.verticalMargins*2 + bh,
				NewEventPopup.buttonWidth,
				DateController.controllerHeight
			);

			this.buttonsFrame = this.CreateFrame
			(
				NewEventPopup.horizontalMargins,
				NewEventPopup.verticalMargins,
				NewEventPopup.buttonWidth,
				NewEventPopup.buttonGap + bh
			);

			this.CreateDate (dateFrame);
			this.CreateButtons ();
			this.CreateCloseButton ();
		}

		private void CreateDate(Widget parent)
		{
			var dateController = new DateController (this.accessor)
			{
				DateLabelWidth  = 0,
				DateDescription = null,
				TabIndex        = 1,
				Date            = this.Timestamp.Date,
			};

			dateController.CreateUI (parent);

			dateController.DateChanged += delegate (object sender, System.DateTime? date)
			{
				this.Timestamp = new Timestamp (dateController.Date.Value, 0);
				this.UpdateButtons ();
				this.OnDateChanged (date);
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
			int y = 0;

			foreach (var desc in this.ButtonDescriptions.Reverse ())
			{
				this.CreateButton (dx, dy, desc.Type, desc.Text, desc.Tooltip, desc.Enable);
				y += dy + NewEventPopup.buttonGap;
			}
		}

		private Button CreateButton(int dx, int dy, EventType type, string text, string tooltip, bool enable)
		{
			string name = type.ToString();

			var button = new Button
			{
				Parent        = this.buttonsFrame,
				Name          = name,
				Text          = text,
				ButtonStyle   = ButtonStyle.Icon,
				AutoFocus     = false,
				Enable        = enable,
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


		private Size RequiredSize
		{
			get
			{
				int dx = NewEventPopup.horizontalMargins*2 + NewEventPopup.buttonWidth;
				int dy = NewEventPopup.verticalMargins*3
					   + AbstractPopup.titleHeight
					   + DateController.controllerHeight
					   + this.ButtonsHeight;

				return new Size (dx, dy);
			}
		}

		private int ButtonsHeight
		{
			get
			{
				int buttonCount = this.ButtonDescriptions.Count ();

				return NewEventPopup.buttonHeight * buttonCount
					 + NewEventPopup.buttonGap * (buttonCount-1);
			}
		}


		private IEnumerable<ButtonDescription> ButtonDescriptions
		{
			get
			{
				foreach (var type in AssetCalculator.EventTypes)
				{
					yield return this.GetButtonDescription (type);
				}
			}
		}

		private ButtonDescription GetButtonDescription(EventType type)
		{
			var types = AssetCalculator.GetPlausibleEventTypes (this.DataObject, this.Timestamp);
			bool enable = types.ToArray ().Contains (type);

			switch (type)
			{
				case EventType.Input:
					return new ButtonDescription (type, "Entrée", "Entrée dans l'inventaire, acquisition", enable);

				case EventType.Modification:
					return new ButtonDescription (type, "Modification générale", "Modification de diverses informations", enable);

				case EventType.MainValue:
					return new ButtonDescription (type, "Modification de la valeur", "Modification de la valeur comptable<br/>(revalorisation, réévaluation, etc.)", enable);

				case EventType.AmortizationExtra:
					return new ButtonDescription (type, "Amortissement extraordinaire", "Amortissement de la valeur comptable", enable);

				case EventType.Output:
					return new ButtonDescription (type, "Sortie", "Sortie de l'inventaire, vente, vol, destruction, etc.", enable);
				
				default:
					return new ButtonDescription (type, null, null, false);
			}
		}


		private struct ButtonDescription
		{
			public ButtonDescription(EventType type, string text, string tooltip, bool enable)
			{
				this.Type    = type;
				this.Text    = text;
				this.Tooltip = tooltip;
				this.Enable  = enable;
			}

			public readonly EventType	Type;
			public readonly string		Text;
			public readonly string		Tooltip;
			public readonly bool		Enable;
		};


		#region Events handler
		private void OnDateChanged(System.DateTime? dateTime)
		{
			this.DateChanged.Raise (this, dateTime);
		}

		public event EventHandler<System.DateTime?> DateChanged;
		#endregion


		private const int horizontalMargins = 20;
		private const int verticalMargins   = 20;
		private const int buttonWidth       = DateController.controllerWidth;
		private const int buttonHeight      = 24;
		private const int buttonGap         = 1;

		private readonly DataAccessor					accessor;

		private FrameBox								buttonsFrame;
	}
}