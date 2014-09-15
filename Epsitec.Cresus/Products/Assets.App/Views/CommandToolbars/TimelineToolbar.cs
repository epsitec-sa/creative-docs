//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;

using Epsitec.Common.Widgets;
using Epsitec.Common.Drawing;
using Epsitec.Cresus.Assets.App.Helpers;
using Epsitec.Cresus.Assets.App.Widgets;
using Epsitec.Cresus.Assets.Server.BusinessLogic;
using Epsitec.Cresus.Assets.Server.SimpleEngine;

namespace Epsitec.Cresus.Assets.App.Views.CommandToolbars
{
	public class TimelineToolbar : AbstractCommandToolbar
	{
		public TimelineToolbar(DataAccessor accessor, CommandDispatcher commandDispatcher, CommandContext commandContext)
			: base (accessor, commandDispatcher, commandContext)
		{
		}

		protected override void CreateCommands()
		{
			this.SetCommandDescription (ToolbarCommand.Labels,      "Timeline.Labels",       Res.Strings.Toolbar.Timeline.Labels.ToString ());
			this.SetCommandDescription (ToolbarCommand.CompactAll,  "Timeline.Compacted",    Res.Strings.Toolbar.Timeline.Compacted.ToString ());
			this.SetCommandDescription (ToolbarCommand.ExpandAll,   "Timeline.Expanded",     Res.Strings.Toolbar.Timeline.Expanded.ToString ());
			this.SetCommandDescription (ToolbarCommand.WeeksOfYear, "Timeline.WeeksOfYear",  Res.Strings.Toolbar.Timeline.WeeksOfYear.ToString ());
			this.SetCommandDescription (ToolbarCommand.DaysOfWeek,  "Timeline.DaysOfWeek",   Res.Strings.Toolbar.Timeline.DaysOfWeek.ToString ());
			this.SetCommandDescription (ToolbarCommand.Graph,       "Timeline.Graph",        Res.Strings.Toolbar.Timeline.Graph.ToString ());
			this.SetCommandDescription (ToolbarCommand.First,       "Timeline.First",        Res.Strings.Toolbar.Timeline.First.ToString ());
			this.SetCommandDescription (ToolbarCommand.Prev,        "Timeline.Prev",         Res.Strings.Toolbar.Timeline.Prev.ToString ());
			this.SetCommandDescription (ToolbarCommand.Next,        "Timeline.Next",         Res.Strings.Toolbar.Timeline.Next.ToString ());
			this.SetCommandDescription (ToolbarCommand.Last,        "Timeline.Last",         Res.Strings.Toolbar.Timeline.Last.ToString ());
			this.SetCommandDescription (ToolbarCommand.Now,         "Timeline.Now",          Res.Strings.Toolbar.Timeline.Now.ToString ());
			this.SetCommandDescription (ToolbarCommand.Date,        "Timeline.Date",         Res.Strings.Toolbar.Timeline.Date.ToString ());
			this.SetCommandDescription (ToolbarCommand.New,         "TreeTable.New.Event",   Res.Strings.Toolbar.Timeline.New.ToString (), new Shortcut (KeyCode.AlphaE | KeyCode.ModifierControl));
			this.SetCommandDescription (ToolbarCommand.Delete ,     "Timeline.Delete",       Res.Strings.Toolbar.Timeline.Delete.ToString ());
			this.SetCommandDescription (ToolbarCommand.Deselect,    "Timeline.Deselect",     Res.Strings.Toolbar.Timeline.Deselect.ToString ());
			this.SetCommandDescription (ToolbarCommand.Copy,        "TreeTable.Copy.Event",  Res.Strings.Toolbar.Timeline.Copy.ToString ());
			this.SetCommandDescription (ToolbarCommand.Paste,       "TreeTable.Paste.Event", Res.Strings.Toolbar.Timeline.Paste.ToString ());
		}


		public TimelineMode						TimelineMode
		{
			get
			{
				return this.timelineMode;
			}
			set
			{
				if (this.timelineMode != value)
				{
					this.timelineMode = value;
					this.UpdateModeButtons ();
				}
			}
		}


		public override FrameBox CreateUI(Widget parent)
		{
			this.toolbar = new FrameBox
			{
				Parent          = parent,
				Dock            = DockStyle.Top,
				PreferredHeight = AbstractCommandToolbar.secondaryToolbarHeight,
				BackColor       = ColorManager.ToolbarBackgroundColor,
			};

			this.buttonLabels      = this.CreateModeButton    (TimelineMode.Labels,      ToolbarCommand.Labels);
								   							   					   
			this.buttonCompacted   = this.CreateModeButton    (TimelineMode.Compacted,   ToolbarCommand.CompactAll);
			this.buttonExpanded    = this.CreateModeButton    (TimelineMode.Expanded,    ToolbarCommand.ExpandAll);
														      
			this.buttonWeeksOfYear = this.CreateModeButton    (TimelineMode.WeeksOfYear, ToolbarCommand.WeeksOfYear);
			this.buttonDaysOfWeek  = this.CreateModeButton    (TimelineMode.DaysOfWeek,  ToolbarCommand.DaysOfWeek);
			this.buttonGraph       = this.CreateModeButton    (TimelineMode.Graph,       ToolbarCommand.Graph);

			this.buttonFirst       = this.CreateCommandButton (DockStyle.Left,           ToolbarCommand.First);
			this.buttonPrev        = this.CreateCommandButton (DockStyle.Left,           ToolbarCommand.Prev);
			this.buttonNext        = this.CreateCommandButton (DockStyle.Left,           ToolbarCommand.Next);
			this.buttonLast        = this.CreateCommandButton (DockStyle.Left,           ToolbarCommand.Last);
			this.buttonNow         = this.CreateCommandButton (DockStyle.Left,           ToolbarCommand.Now);
			this.buttonDate        = this.CreateCommandButton (DockStyle.Left,           ToolbarCommand.Date);
																				          
			this.CreateSeparator (DockStyle.Left);
																				          
			this.buttonNew         = this.CreateCommandButton (DockStyle.Left,           ToolbarCommand.New);
			this.buttonDelete      = this.CreateCommandButton (DockStyle.Left,           ToolbarCommand.Delete);
			this.buttonDeselect    = this.CreateCommandButton (DockStyle.Left,           ToolbarCommand.Deselect);

			this.CreateSeparator (DockStyle.Left);
																				          
			this.buttonCopy       = this.CreateCommandButton (DockStyle.Left,            ToolbarCommand.Copy);
			this.buttonPaste      = this.CreateCommandButton (DockStyle.Left,            ToolbarCommand.Paste);

			this.buttonCompacted  .Margins = new Margins ( 5, 0, 0, 0);
			this.buttonWeeksOfYear.Margins = new Margins ( 5, 0, 0, 0);
			this.buttonFirst      .Margins = new Margins (10, 0, 0, 0);

			this.UpdateModeButtons ();
			this.AttachShortcuts ();

			return this.toolbar;
		}


		private IconButton CreateModeButton(TimelineMode mode, ToolbarCommand command)
		{
			var button = this.CreateCommandButton (DockStyle.Left, command);
			button.ButtonStyle = ButtonStyle.ActivableIcon;

			button.Clicked += delegate
			{
				this.ChangeMode (mode);
				this.UpdateModeButtons ();
				this.OnModeChanged (this.timelineMode);
			};

			return button;
		}


		private void ChangeMode(TimelineMode mode)
		{
			if (mode == TimelineMode.Compacted)
			{
				this.timelineMode |=  TimelineMode.Compacted;
				this.timelineMode &= ~TimelineMode.Expanded;
			}
			else if (mode == TimelineMode.Expanded)
			{
				this.timelineMode |=  TimelineMode.Expanded;
				this.timelineMode &= ~TimelineMode.Compacted;
			}
			else
			{
				this.timelineMode ^= mode;
			}
		}

		private void UpdateModeButtons()
		{
			this.SetCommandActivate (ToolbarCommand.Labels,      (this.timelineMode & TimelineMode.Labels     ) != 0);
			this.SetCommandActivate (ToolbarCommand.CompactAll,  (this.timelineMode & TimelineMode.Compacted  ) != 0);
			this.SetCommandActivate (ToolbarCommand.ExpandAll,   (this.timelineMode & TimelineMode.Expanded   ) != 0);
			this.SetCommandActivate (ToolbarCommand.WeeksOfYear, (this.timelineMode & TimelineMode.WeeksOfYear) != 0);
			this.SetCommandActivate (ToolbarCommand.DaysOfWeek,  (this.timelineMode & TimelineMode.DaysOfWeek ) != 0);
			this.SetCommandActivate (ToolbarCommand.Graph,       (this.timelineMode & TimelineMode.Graph      ) != 0);
		}


		#region Events handler
		private void OnModeChanged(TimelineMode timelineMode)
		{
			this.ModeChanged.Raise (this, timelineMode);
		}

		public event EventHandler<TimelineMode> ModeChanged;
		#endregion


		private IconButton						buttonLabels;
		private IconButton						buttonCompacted;
		private IconButton						buttonExpanded;
		private IconButton						buttonWeeksOfYear;
		private IconButton						buttonDaysOfWeek;
		private IconButton						buttonGraph;

		private IconButton						buttonFirst;
		private IconButton						buttonPrev;
		private IconButton						buttonNext;
		private IconButton						buttonLast;
		private IconButton						buttonNow;
		private IconButton						buttonDate;
		private IconButton						buttonNew;
		private IconButton						buttonDelete;
		private IconButton						buttonDeselect;
		private IconButton						buttonCopy;
		private IconButton						buttonPaste;

		private TimelineMode					timelineMode;
	}
}
