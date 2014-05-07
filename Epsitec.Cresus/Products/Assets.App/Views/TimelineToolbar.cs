//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;

using Epsitec.Common.Widgets;
using Epsitec.Common.Drawing;
using Epsitec.Cresus.Assets.App.Helpers;
using Epsitec.Cresus.Assets.App.Widgets;
using Epsitec.Cresus.Assets.Server.BusinessLogic;

namespace Epsitec.Cresus.Assets.App.Views
{
	public class TimelineToolbar : AbstractCommandToolbar
	{
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

			this.buttonLabels      = this.CreateModeButton    (TimelineMode.Labels,      ToolbarCommand.Labels,      "Timeline.Labels",      "Affiche les noms des lignes");
								   							   					   
			this.buttonCompacted   = this.CreateModeButton    (TimelineMode.Compacted,   ToolbarCommand.CompactAll,  "Timeline.Compacted",   "Affichage compact");
			this.buttonExpanded    = this.CreateModeButton    (TimelineMode.Expanded,    ToolbarCommand.ExpandAll,   "Timeline.Expanded",    "Affichage étendu");
														      
			this.buttonWeeksOfYear = this.CreateModeButton    (TimelineMode.WeeksOfYear, ToolbarCommand.WeeksOfYear, "Timeline.WeeksOfYear", "Affiche les numéros des semaines");
			this.buttonDaysOfWeek  = this.CreateModeButton    (TimelineMode.DaysOfWeek,  ToolbarCommand.DaysOfWeek,  "Timeline.DaysOfWeek",  "Affiche les jours de la semaine");
			this.buttonGraph       = this.CreateModeButton    (TimelineMode.Graph,       ToolbarCommand.Graph,       "Timeline.Graph",       "Affiche les graphique des valeurs");

			this.buttonFirst       = this.CreateCommandButton (DockStyle.Left,           ToolbarCommand.First,       "Timeline.First",       "Retour sur le premier événement");
			this.buttonPrev        = this.CreateCommandButton (DockStyle.Left,           ToolbarCommand.Prev,        "Timeline.Prev",        "Recule sur l'événement précédent");
			this.buttonNext        = this.CreateCommandButton (DockStyle.Left,           ToolbarCommand.Next,        "Timeline.Next",        "Avance sur l'événement suivant");
			this.buttonLast        = this.CreateCommandButton (DockStyle.Left,           ToolbarCommand.Last,        "Timeline.Last",        "Avance sur le dernier événement");
			this.buttonNow         = this.CreateCommandButton (DockStyle.Left,           ToolbarCommand.Now,         "Timeline.Now",         "Va à la date du jour");
			this.buttonDate        = this.CreateCommandButton (DockStyle.Left,           ToolbarCommand.Date,        "Timeline.Date",        "Va à une date à choix");
																				          
			this.CreateSeparator (DockStyle.Left);
																				          
			this.buttonNew         = this.CreateCommandButton (DockStyle.Left,           ToolbarCommand.New,         "TreeTable.New.Event",  "Nouvel événement");
			this.buttonDelete      = this.CreateCommandButton (DockStyle.Left,           ToolbarCommand.Delete ,     "Timeline.Delete",      "Supprime l'événement");
			this.buttonDeselect    = this.CreateCommandButton (DockStyle.Left,           ToolbarCommand.Deselect,    "Timeline.Deselect",    "Désélectionne l'événement");

			this.buttonCompacted.Margins   = new Margins (5, 0, 0, 0);
			this.buttonWeeksOfYear.Margins = new Margins (5, 0, 0, 0);
			this.buttonFirst.Margins       = new Margins (10, 0, 0, 0);

			this.UpdateModeButtons ();

			return this.toolbar;
		}


		private IconButton CreateModeButton(TimelineMode mode, ToolbarCommand command, string icon, string tooltip)
		{
			var button = this.CreateCommandButton (DockStyle.Left, command, icon, tooltip);
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

		private TimelineMode					timelineMode;
	}
}
