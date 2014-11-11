//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Common.Drawing;
using Epsitec.Common.Widgets;
using Epsitec.Cresus.Assets.App.Helpers;
using Epsitec.Cresus.Assets.App.Views.FieldControllers;
using Epsitec.Cresus.Assets.App.Widgets;
using Epsitec.Cresus.Assets.Server.SimpleEngine;

namespace Epsitec.Cresus.Assets.App.Views
{
	/// <summary>
	/// Contrôleur permettant de saisir une date avec plusieurs boutons pour faciliter
	/// la saisie, y compris un accès aux dates prédéfinies (début ou fin de l'année en
	/// cours par exemple) et au calendrier. Pour cela, on utilise les composants
	/// SimplePopup et CalendarPopup, affichés en tant que sous-popups.
	/// </summary>
	public class DateController
	{
		public DateController(DataAccessor accessor)
		{
			this.accessor = accessor;

			this.dateFieldController = new DateFieldController (accessor)
			{
				Label             = null,
				LabelWidth        = 0,
				DateRangeCategory = DateRangeCategory.Mandat,
			};

			this.DateDescription = Res.Strings.DateController.Description.ToString ();
			this.DateLabelWidth  = DateController.indent;
		}


		public string							DateDescription;
		public int								DateLabelWidth;
		public int								TabIndex;

		public DateRangeCategory				DateRangeCategory
		{
			get
			{
				return this.dateFieldController.DateRangeCategory;
			}
			set
			{
				this.dateFieldController.DateRangeCategory = value;
			}
		}

		public bool								HasError
		{
			get
			{
				return this.dateFieldController.HasError;
			}
		}

		public System.DateTime?					Date
		{
			get
			{
				return this.date;
			}
			set
			{
				if (this.date != value)
				{
					this.date = value;
					this.UpdateDate ();
					this.UpdateButtons ();
				}
			}
		}


		public void CreateUI(Widget parent)
		{
			this.CreateToolbar (parent);
			this.CreateController (parent);

			this.UpdateButtons ();
		}

		public void SetFocus()
		{
			this.dateFieldController.SetFocus ();
		}


		private void CreateToolbar(Widget parent)
		{
			//	Crée la ligne des boutons [J] [M] [A] permettant de sélectionner
			//	la partie correspondante dans le textede la date.
			//	Les boutons sont positionnés horizontalement de façon à s'aligner
			//	au mieux sur le texte de la date dans le TextField.
			var line = new FrameBox
			{
				Parent          = parent,
				Dock            = DockStyle.Top,
				PreferredHeight = DateController.lineHeight,
				Margins         = new Margins (this.DateLabelWidth+(this.DateLabelWidth == 0 ? 0 : 10), 0, 0, 0),
			};

			line.Entered += delegate
			{
				this.isMouseInside = true;
				this.dateFieldController.IsMouseInsideParent = this.isMouseInside;
				this.UpdateButtons ();
			};

			line.Exited += delegate
			{
				this.isMouseInside = false;
				this.dateFieldController.IsMouseInsideParent = this.isMouseInside;
				this.UpdateButtons ();
			};

			this.dayButton   = this.CreatePartButton (line, 5, 13);
			this.monthButton = this.CreatePartButton (line, 19, 13);
			this.yearButton  = this.CreatePartButton (line, 33, 23);

			ToolTip.Default.SetToolTip (this.dayButton,   Res.Strings.DateController.DaySelect.Tooltip.ToString ());
			ToolTip.Default.SetToolTip (this.monthButton, Res.Strings.DateController.MonthSelect.Tooltip.ToString ());
			ToolTip.Default.SetToolTip (this.yearButton,  Res.Strings.DateController.YearSelect.Tooltip.ToString ());

			this.info = new StaticText
			{
				Parent           = line,
				ContentAlignment = ContentAlignment.MiddleLeft,
				TextBreakMode    = TextBreakMode.Ellipsis | TextBreakMode.Split | TextBreakMode.SingleLine,
				Enable           = false,  // pour afficher en gris
				Dock             = DockStyle.Fill,
				Margins          = new Margins (72, 0, 0, 0),
			};

			this.dayButton.Clicked += delegate
			{
				int start, length;
				DateFieldController.GetDatePart (this.dateFieldController.TextField.Text, DateFieldController.Part.Day, out start, out length);
				if (length > 0)
				{
					this.SelectText (start, length);
				}
			};

			this.monthButton.Clicked += delegate
			{
				int start, length;
				DateFieldController.GetDatePart (this.dateFieldController.TextField.Text, DateFieldController.Part.Month, out start, out length);
				if (length > 0)
				{
					this.SelectText (start, length);
				}
			};

			this.yearButton.Clicked += delegate
			{
				int start, length;
				DateFieldController.GetDatePart (this.dateFieldController.TextField.Text, DateFieldController.Part.Year, out start, out length);
				if (length > 0)
				{
					this.SelectText (start, length);
				}
			};
		}

		private IconButton CreatePartButton(Widget parent, int x, int dx)
		{
			return new IconButton
			{
				Parent          = parent,
				AutoFocus       = false,
				PreferredWidth  = dx,
				PreferredHeight = DateController.lineHeight,
				Anchor          = AnchorStyles.BottomLeft,
				Margins         = new Margins (x, 0, 0, 0),
			};
		}

		public void CreateController(Widget parent)
		{
			var footer = new FrameBox
			{
				Parent          = parent,
				Dock            = DockStyle.Top,
				PreferredHeight = DateController.lineHeight,
			};

			if (this.DateLabelWidth > 0)
			{
				new StaticText
				{
					Parent           = footer,
					Text             = this.DateDescription,
					ContentAlignment = ContentAlignment.MiddleRight,
					PreferredWidth   = this.DateLabelWidth,
					Dock             = DockStyle.Left,
					Margins          = new Margins (0, 10, 0, 0),
				};
			}

			var dateFrame = new FrameBox
			{
				Parent    = footer,
				Dock      = DockStyle.Fill,
				BackColor = ColorManager.WindowBackgroundColor,
				Padding   = new Margins (2, 0, 0, 0),
			};

			this.dateFieldController.HideAdditionalButtons = true;
			this.dateFieldController.TabIndex = this.TabIndex;
			this.dateFieldController.CreateUI (dateFrame);
			this.TabIndex = this.dateFieldController.TabIndex;
			this.dateFieldController.SetFocus ();

			this.dateFieldController.ValueEdited += delegate
			{
				this.Date = this.dateFieldController.Value;
				this.OnDateChanged (this.date);
			};

			this.dateFieldController.CursorChanged += delegate
			{
				this.UpdateButtons ();
			};

			this.dateFieldController.MouseEnteredOrExited += delegate (object sender, bool isInside)
			{
				this.isMouseInsideChildren = isInside;
				this.UpdateButtons ();
			};
		}


		private void UpdateDate()
		{
			this.dateFieldController.Value = this.date;
		}

		private void UpdateButtons()
		{
			if (this.dayButton != null)
			{
				this.dayButton  .Visibility = this.AreButtonsVisible;
				this.monthButton.Visibility = this.AreButtonsVisible;
				this.yearButton .Visibility = this.AreButtonsVisible;

				var part = this.dateFieldController.SelectedPart;

				this.dayButton.Enable   = this.Date != null;
				this.monthButton.Enable = this.Date != null;
				this.yearButton.Enable  = this.Date != null;

				this.UpdateButtonPart (this.dayButton,   part == DateFieldController.Part.Day);
				this.UpdateButtonPart (this.monthButton, part == DateFieldController.Part.Month);
				this.UpdateButtonPart (this.yearButton,  part == DateFieldController.Part.Year);

				this.info.Text = this.date.ToFull ();
			}
		}

		private void UpdateButtonPart(IconButton button, bool activate)
		{
			if (activate)
			{
				button.IconUri = Misc.GetResourceIconUri ("Date.SelectedPart");
			}
			else
			{
				button.IconUri = Misc.GetResourceIconUri ("Date.UnselectedPart");
			}
		}

		private bool AreButtonsVisible
		{
			get
			{
				return this.isMouseInside || this.isMouseInsideChildren;
			}
		}


		private void SelectText(int start, int count)
		{
			this.dateFieldController.TextField.Focus ();
			this.dateFieldController.TextField.CursorFrom = start;
			this.dateFieldController.TextField.CursorTo   = start + count;
		}


		#region Events handler
		private void OnDateChanged(System.DateTime? dateTime)
		{
			this.DateChanged.Raise (this, dateTime);
		}

		public event EventHandler<System.DateTime?> DateChanged;
		#endregion


		public const int controllerWidth  = DateController.widgetWidth;  // = 235
		public const int controllerHeight = DateController.lineHeight*2 + 4;

		private const int indent          = 30;
		private const int widgetWidth     = DateFieldController.controllerWidth + (int) (AbstractFieldController.lineHeight*7.5) + 3;  // = 235
		private const int lineHeight      = AbstractFieldController.lineHeight;


		private readonly DataAccessor						accessor;
		private readonly DateFieldController				dateFieldController;

		private IconButton									dayButton;
		private IconButton									monthButton;
		private IconButton									yearButton;
		private StaticText									info;

		private System.DateTime?							date;
		private bool										isMouseInside;
		private bool										isMouseInsideChildren;
	}
}
