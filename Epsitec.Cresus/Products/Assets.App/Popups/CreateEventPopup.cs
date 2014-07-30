//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Common.Drawing;
using Epsitec.Common.Widgets;
using Epsitec.Cresus.Assets.App.Helpers;
using Epsitec.Cresus.Assets.App.Views;
using Epsitec.Cresus.Assets.App.Widgets;
using Epsitec.Cresus.Assets.Data;
using Epsitec.Cresus.Assets.Server.BusinessLogic;
using Epsitec.Cresus.Assets.Server.SimpleEngine;

namespace Epsitec.Cresus.Assets.App.Popups
{
	/// <summary>
	/// Choix du type d'un nouvel événement à créer, et de sa date.
	/// </summary>
	public class CreateEventPopup : AbstractPopup
	{
		private CreateEventPopup(DataAccessor accessor)
		{
			this.accessor = accessor;
		}


		private BaseType						BaseType;
		private DataObject						DataObject;
		private Timestamp						Timestamp;

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
				CreateEventPopup.horizontalMargins,
				CreateEventPopup.verticalMargins*2 + bh,
				CreateEventPopup.buttonWidth,
				DateController.controllerHeight
			);

			this.buttonsFrame = this.CreateFrame
			(
				CreateEventPopup.horizontalMargins,
				CreateEventPopup.verticalMargins,
				CreateEventPopup.buttonWidth,
				CreateEventPopup.buttonGap + bh
			);

			this.CreateDate (dateFrame);
			this.CreateButtons ();
			this.CreateCloseButton ();
		}

		private void CreateDate(Widget parent)
		{
			var dateController = new DateController (this.accessor)
			{
				DateRangeCategory = DateRangeCategory.Mandat,
				DateLabelWidth    = 0,
				DateDescription   = null,
				TabIndex          = 1,
				Date              = this.Timestamp.Date,
			};

			dateController.CreateUI (parent);

			dateController.DateChanged += delegate (object sender, System.DateTime? date)
			{
				if (dateController.Date.HasValue)
				{
					this.Timestamp = new Timestamp (dateController.Date.Value, 0);
					this.UpdateButtons ();
					this.OnDateChanged (date);
				}
			};
		}


		private void UpdateButtons()
		{
			this.ChangeDialogSize (this.DialogSize);
			this.CreateButtons ();
		}

		private void CreateButtons()
		{
			//	Crée les boutons de bas en haut.
			this.buttonsFrame.Children.Clear ();

			int dx = CreateEventPopup.buttonWidth;
			int dy = CreateEventPopup.buttonHeight;
			int y = 0;

			int tabIndex = 100;
			foreach (var desc in this.ButtonDescriptions.Reverse ())
			{
				this.CreateButton (dx, dy, desc.Type, desc.Text, desc.Tooltip, desc.Enable, tabIndex--);
				y += dy + CreateEventPopup.buttonGap;
			}
		}

		private ColoredButton CreateButton(int dx, int dy, EventType type, string text, string tooltip, bool enable, int tabIndex)
		{
			string name = type.ToString();

			var button = new ColoredButton
			{
				Parent        = this.buttonsFrame,
				Name          = name,
				Text          = text,
				Enable        = enable,
				Dock          = DockStyle.Bottom,
				PreferredSize = new Size (dx, dy),
				Margins       = new Margins (0, 0, CreateEventPopup.buttonGap, 0),
				TabIndex      = tabIndex,
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
				int dx = CreateEventPopup.horizontalMargins*2 + CreateEventPopup.buttonWidth;
				int dy = CreateEventPopup.verticalMargins*3
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

				return CreateEventPopup.buttonHeight * buttonCount
					 + CreateEventPopup.buttonGap * (buttonCount-1);
			}
		}


		private IEnumerable<ButtonDescription> ButtonDescriptions
		{
			get
			{
				foreach (var type in CreateEventPopup.EventTypes)
				{
					yield return this.GetButtonDescription (type);
				}
			}
		}

		private static IEnumerable<EventType> EventTypes
		{
			get
			{
				yield return EventType.Input;
				yield return EventType.Modification;
				yield return EventType.Revaluation;
				yield return EventType.Revalorization;
				//?yield return EventType.MainValue;
				//?yield return EventType.AmortizationExtra;
				yield return EventType.Locked;
				yield return EventType.Output;
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
					return new ButtonDescription (type, "Modification générale", "Modification de diverses informations,<br/>sauf de la valeur comptable", enable);

				case EventType.Revaluation:
					return new ButtonDescription (type, "Réévaluation", "Réévaluation de la valeur comptable", enable);

				case EventType.Revalorization:
					return new ButtonDescription (type, "Revalorisation", "Revalorisation de la valeur comptable", enable);

				case EventType.MainValue:
					return new ButtonDescription (type, "Ajustement de la valeur comptable", "Ajustement extraordinaire de la valeur comptable", enable);

				case EventType.AmortizationExtra:
					return new ButtonDescription (type, "Amortissement extraordinaire", "Amortissement de la valeur comptable", enable);

				case EventType.Locked:
					return new ButtonDescription (type, "Verrou", "Verrouille tous les événements antérieurs", enable);

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


		#region Helpers
		public static void Show(Widget target, DataAccessor accessor,
			BaseType baseType, DataObject obj, Timestamp timestamp,
			System.Action<Timestamp?> timestampChanged,
			System.Action<System.DateTime, string> action)
		{
			if (target != null)
			{
				System.DateTime? createDate = timestamp.Date;

				var popup = new CreateEventPopup (accessor)
				{
					BaseType   = baseType,
					DataObject = obj,
					Timestamp  = timestamp,
				};

				popup.Create (target);

				popup.DateChanged += delegate (object sender, System.DateTime? dateTime)
				{
					if (dateTime.HasValue)
					{
						timestampChanged (new Timestamp (dateTime.Value, 0));
						createDate = dateTime.Value;
					}
					else
					{
						timestampChanged (null);
					}
				};

				popup.ButtonClicked += delegate (object sender, string name)
				{
					if (createDate.HasValue)
					{
						action (createDate.Value, name);
					}
				};
			}
		}
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