//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;

using Epsitec.Common.Widgets;
using Epsitec.Cresus.Assets.App.Helpers;
using Epsitec.Cresus.Assets.App.Widgets;
using Epsitec.Cresus.Assets.Server.BusinessLogic;
using Epsitec.Cresus.Assets.Server.SimpleEngine;

namespace Epsitec.Cresus.Assets.App.Views.CommandToolbars
{
	public class TimelinesToolbar : AbstractCommandToolbar
	{
		public TimelinesToolbar(DataAccessor accessor, CommandContext commandContext)
			: base (accessor, commandContext)
		{
		}

		protected override void CreateCommands()
		{
			this.SetCommandDescription (ToolbarCommand.Narrow,                 "Timeline.Narrow",         Res.Strings.Toolbar.Timelines.Narrow.ToString ());
			this.SetCommandDescription (ToolbarCommand.Wide,                   "Timeline.Wide",           Res.Strings.Toolbar.Timelines.Wide.ToString ());
			this.SetCommandDescription (ToolbarCommand.First,                  "Timeline.First",          Res.Strings.Toolbar.Timelines.First.ToString ());
			this.SetCommandDescription (ToolbarCommand.Prev,                   "Timeline.Prev",           Res.Strings.Toolbar.Timelines.Prev.ToString ());
			this.SetCommandDescription (ToolbarCommand.Next,                   "Timeline.Next",           Res.Strings.Toolbar.Timelines.Next.ToString ());
			this.SetCommandDescription (ToolbarCommand.Last,                   "Timeline.Last",           Res.Strings.Toolbar.Timelines.Last.ToString ());
			this.SetCommandDescription (ToolbarCommand.New,                    "TreeTable.New.Event",     Res.Strings.Toolbar.Timelines.NewEvent.ToString (), new Shortcut (KeyCode.AlphaE | KeyCode.ModifierControl));
			this.SetCommandDescription (ToolbarCommand.Delete,                 "Timeline.Delete",         Res.Strings.Toolbar.Timelines.DeleteEvent.ToString ());
			this.SetCommandDescription (ToolbarCommand.AmortizationsPreview,   "Amortizations.Preview",   Res.Strings.Popup.Amortizations.Preview.Title.ToString ());
			this.SetCommandDescription (ToolbarCommand.AmortizationsFix,       "Amortizations.Fix",       Res.Strings.Popup.Amortizations.Fix.Title.ToString ());
			this.SetCommandDescription (ToolbarCommand.AmortizationsToExtra,   "Amortizations.ToExtra",   Res.Strings.Popup.Amortizations.ToExtra.Title.ToString ());
			this.SetCommandDescription (ToolbarCommand.AmortizationsUnpreview, "Amortizations.Unpreview", Res.Strings.Popup.Amortizations.Unpreview.Title.ToString ());
			this.SetCommandDescription (ToolbarCommand.AmortizationsDelete,    "Amortizations.Delete",    Res.Strings.Popup.Amortizations.Delete.Title.ToString ());
			this.SetCommandDescription (ToolbarCommand.Deselect,               "Timeline.Deselect",       Res.Strings.Toolbar.Timelines.DeselectEvent.ToString ());
			this.SetCommandDescription (ToolbarCommand.Copy,                   "TreeTable.Copy.Event",    Res.Strings.Toolbar.Timelines.CopyEvent.ToString ());
			this.SetCommandDescription (ToolbarCommand.Paste,                  "TreeTable.Paste.Event",   Res.Strings.Toolbar.Timelines.PasteEvent.ToString ());
		}


		public TimelinesMode					TimelinesMode
		{
			get
			{
				return this.timelinesMode;
			}
			set
			{
				if (this.timelinesMode != value)
				{
					this.timelinesMode = value;
					this.UpdateModeButtons ();
				}
			}
		}


		public override FrameBox CreateUI(Widget parent)
		{
			//	La toolbar s'adapte en fonction de la largeur disponible. Certains
			//	boutons non indispensables disparaissent s'il manque de la place.
			this.toolbar = new FrameBox
			{
				Parent          = parent,
				Dock            = DockStyle.Top,
				PreferredHeight = AbstractCommandToolbar.secondaryToolbarHeight,
				BackColor       = ColorManager.ToolbarBackgroundColor,
			};

			CommandDispatcher.SetDispatcher (this.toolbar, this.commandDispatcher);

			this.buttonCompacted = this.CreateModeButton (TimelinesMode.Narrow, ToolbarCommand.Narrow);
			this.buttonExpended  = this.CreateModeButton   (TimelinesMode.Wide,   ToolbarCommand.Wide);

			this.buttonFirst = this.CreateCommandButton (DockStyle.Left, ToolbarCommand.First);
			this.buttonPrev  = this.CreateCommandButton (DockStyle.Left, ToolbarCommand.Prev);
			this.buttonNext  = this.CreateCommandButton (DockStyle.Left, ToolbarCommand.Next);
			this.buttonLast  = this.CreateCommandButton (DockStyle.Left, ToolbarCommand.Last);

			this.separator1 = this.CreateSeparator (DockStyle.Left);
			
			this.buttonNew                    = this.CreateCommandButton (DockStyle.Left, ToolbarCommand.New);
			this.buttonDelete                 = this.CreateCommandButton (DockStyle.Left, ToolbarCommand.Delete);
			this.buttonAmortizationsPreview   = this.CreateCommandButton (DockStyle.Left, ToolbarCommand.AmortizationsPreview);
			this.buttonAmortizationsFix       = this.CreateCommandButton (DockStyle.Left, ToolbarCommand.AmortizationsFix);
			this.buttonAmortizationsToExtra   = this.CreateCommandButton (DockStyle.Left, ToolbarCommand.AmortizationsToExtra);
			this.buttonAmortizationsUnpreview = this.CreateCommandButton (DockStyle.Left, ToolbarCommand.AmortizationsUnpreview);
			this.buttonAmortizationsDelete    = this.CreateCommandButton (DockStyle.Left, ToolbarCommand.AmortizationsDelete);
			this.buttonDeselect               = this.CreateCommandButton (DockStyle.Left, ToolbarCommand.Deselect);

			this.separator2 = this.CreateSeparator (DockStyle.Left);
			
			this.buttonCopy  = this.CreateCommandButton (DockStyle.Left, ToolbarCommand.Copy);
			this.buttonPaste = this.CreateCommandButton (DockStyle.Left, ToolbarCommand.Paste);

			this.AttachShortcuts ();

			return this.toolbar;
		}


		private IconButton CreateModeButton(TimelinesMode mode, ToolbarCommand command)
		{
			//	Utilise DockStyle.None, car le bouton est positionnée avec SetManualBounds.
			var button = this.CreateCommandButton (DockStyle.Left, command);
			button.ButtonStyle = ButtonStyle.ActivableIcon;

			button.Clicked += delegate
			{
				this.ChangeMode (mode);
				this.UpdateModeButtons ();
				this.OnModeChanged (this.timelinesMode);
			};

			return button;
		}


		private void ChangeMode(TimelinesMode mode)
		{
			if (mode == TimelinesMode.Narrow)
			{
				this.timelinesMode |=  TimelinesMode.Narrow;
				this.timelinesMode &= ~TimelinesMode.Wide;
			}
			else if (mode == TimelinesMode.Wide)
			{
				this.timelinesMode |=  TimelinesMode.Wide;
				this.timelinesMode &= ~TimelinesMode.Narrow;
			}
			else
			{
				this.timelinesMode ^= mode;
			}
		}

		private void UpdateModeButtons()
		{
			this.SetCommandActivate (ToolbarCommand.Narrow, (this.timelinesMode & TimelinesMode.Narrow) != 0);
			this.SetCommandActivate (ToolbarCommand.Wide,   (this.timelinesMode & TimelinesMode.Wide  ) != 0);
		}


		#region Events handler
		private void OnModeChanged(TimelinesMode timelinesMode)
		{
			this.ModeChanged.Raise (this, timelinesMode);
		}

		public event EventHandler<TimelinesMode> ModeChanged;
		#endregion


		private IconButton						buttonCompacted;
		private IconButton						buttonExpended;

		private IconButton						buttonFirst;
		private IconButton						buttonPrev;
		private IconButton						buttonNext;
		private IconButton						buttonLast;

		private FrameBox						separator1;
		
		private IconButton						buttonNew;
		private IconButton						buttonDelete;
		private IconButton						buttonDeselect;

		private IconButton						buttonAmortizationsPreview;
		private IconButton						buttonAmortizationsFix;
		private IconButton						buttonAmortizationsToExtra;
		private IconButton						buttonAmortizationsUnpreview;
		private IconButton						buttonAmortizationsDelete;

		private FrameBox						separator2;

		private IconButton						buttonCopy;
		private IconButton						buttonPaste;

		private TimelinesMode					timelinesMode;
	}
}
