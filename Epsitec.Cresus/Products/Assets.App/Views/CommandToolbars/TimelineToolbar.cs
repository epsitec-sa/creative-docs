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
		public TimelineToolbar(DataAccessor accessor)
			: base (accessor)
		{
		}

		protected override void CreateCommands()
		{
			this.SetCommandDescription (ToolbarCommand.Labels,      "Timeline.Labels",       "Afficher les noms des lignes");
			this.SetCommandDescription (ToolbarCommand.CompactAll,  "Timeline.Compacted",    "Affichage compact");
			this.SetCommandDescription (ToolbarCommand.ExpandAll,   "Timeline.Expanded",     "Affichage étendu");
			this.SetCommandDescription (ToolbarCommand.WeeksOfYear, "Timeline.WeeksOfYear",  "Afficher les numéros des semaines");
			this.SetCommandDescription (ToolbarCommand.DaysOfWeek,  "Timeline.DaysOfWeek",   "Afficher les jours de la semaine");
			this.SetCommandDescription (ToolbarCommand.Graph,       "Timeline.Graph",        "Afficher les graphique des valeurs");
			this.SetCommandDescription (ToolbarCommand.First,       "Timeline.First",        "Retourner sur le premier événement");
			this.SetCommandDescription (ToolbarCommand.Prev,        "Timeline.Prev",         "Reculer sur l'événement précédent");
			this.SetCommandDescription (ToolbarCommand.Next,        "Timeline.Next",         "Avancer sur l'événement suivant");
			this.SetCommandDescription (ToolbarCommand.Last,        "Timeline.Last",         "Avancer sur le dernier événement");
			this.SetCommandDescription (ToolbarCommand.Now,         "Timeline.Now",          "Aller à la date du jour");
			this.SetCommandDescription (ToolbarCommand.Date,        "Timeline.Date",         "Aller à une date à choix");
			this.SetCommandDescription (ToolbarCommand.New,         "TreeTable.New.Event",   "Nouvel événement");
			this.SetCommandDescription (ToolbarCommand.Delete ,     "Timeline.Delete",       "Supprimer l'événement");
			this.SetCommandDescription (ToolbarCommand.Deselect,    "Timeline.Deselect",     "Désélectionner l'événement");
			this.SetCommandDescription (ToolbarCommand.Copy,        "TreeTable.Copy.Event",  "Copier l'événement");
			this.SetCommandDescription (ToolbarCommand.Paste,       "TreeTable.Paste.Event", "Coller l'événement");
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
