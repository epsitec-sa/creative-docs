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
		public override FrameBox CreateUI(Widget parent)
		{
			var toolbar = this.CreateToolbar (parent, AbstractCommandToolbar.SecondaryToolbarHeight);
			this.UpdateCommandButtons ();

			return toolbar;
		}


		public TimelineMode TimelineMode
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


		protected override void UpdateCommandButtons()
		{
			this.UpdateCommandButton (this.buttonFirst,    ToolbarCommand.First);
			this.UpdateCommandButton (this.buttonPrev,     ToolbarCommand.Prev);
			this.UpdateCommandButton (this.buttonNext,     ToolbarCommand.Next);
			this.UpdateCommandButton (this.buttonLast,     ToolbarCommand.Last);
			this.UpdateCommandButton (this.buttonNow,      ToolbarCommand.Now);
			this.UpdateCommandButton (this.buttonNew,      ToolbarCommand.New);
			this.UpdateCommandButton (this.buttonDelete,   ToolbarCommand.Delete);
			this.UpdateCommandButton (this.buttonDeselect, ToolbarCommand.Deselect);
		}


		protected override FrameBox CreateToolbar(Widget parent, int size)
		{
			var toolbar = new FrameBox
			{
				Parent          = parent,
				Dock            = DockStyle.Top,
				PreferredHeight = size,
				BackColor       = ColorManager.ToolbarBackgroundColor,
			};

			this.buttonLabels      = this.CreateModeButton (toolbar, TimelineMode.Labels,      "Timeline.Labels",      "Affiche les noms des lignes");
								   															   
			this.buttonCompacted   = this.CreateModeButton (toolbar, TimelineMode.Compacted,   "Timeline.Compacted",   "Affichage compact");
			this.buttonExpended    = this.CreateModeButton (toolbar, TimelineMode.Expanded,    "Timeline.Expanded",    "Affichage étendu");

			this.buttonWeeksOfYear = this.CreateModeButton (toolbar, TimelineMode.WeeksOfYear, "Timeline.WeeksOfYear", "Affiche les numéros des semaines");
			this.buttonDaysOfWeek  = this.CreateModeButton (toolbar, TimelineMode.DaysOfWeek,  "Timeline.DaysOfWeek",  "Affiche les jours de la semaine");
			this.buttonGraph       = this.CreateModeButton (toolbar, TimelineMode.Graph,       "Timeline.Graph",       "Affiche les graphique des valeurs");

			this.buttonFirst       = this.CreateCommandButton (toolbar, DockStyle.Left, ToolbarCommand.First,    "Timeline.First",    "Retour sur le premier événement");
			this.buttonPrev        = this.CreateCommandButton (toolbar, DockStyle.Left, ToolbarCommand.Prev,     "Timeline.Prev",     "Recule sur l'événement précédent");
			this.buttonNext        = this.CreateCommandButton (toolbar, DockStyle.Left, ToolbarCommand.Next,     "Timeline.Next",     "Avance sur l'événement suivant");
			this.buttonLast        = this.CreateCommandButton (toolbar, DockStyle.Left, ToolbarCommand.Last,     "Timeline.Last",     "Avance sur le dernier événement");
			this.buttonNow         = this.CreateCommandButton (toolbar, DockStyle.Left, ToolbarCommand.Now,      "Timeline.Now",      "Va à la date du jour");

			this.CreateSeparator (toolbar, DockStyle.Left);
			
			this.buttonNew         = this.CreateCommandButton (toolbar, DockStyle.Left, ToolbarCommand.New,      "Timeline.New",      "Nouvel événement");
			this.buttonDelete      = this.CreateCommandButton (toolbar, DockStyle.Left, ToolbarCommand.Delete ,  "Timeline.Delete",   "Supprimer l'événement");
			this.buttonDeselect    = this.CreateCommandButton (toolbar, DockStyle.Left, ToolbarCommand.Deselect, "Timeline.Deselect", "Désélectionne l'événement");

			this.buttonCompacted.Margins   = new Margins (5, 0, 0, 0);
			this.buttonWeeksOfYear.Margins = new Margins (5, 0, 0, 0);
			this.buttonFirst.Margins       = new Margins (10, 0, 0, 0);

			this.UpdateModeButtons ();

			return toolbar;
		}

		private IconButton CreateModeButton(FrameBox toolbar, TimelineMode mode, string icon, string tooltip)
		{
			var size = toolbar.PreferredHeight;

			var button = new IconButton
			{
				Parent        = toolbar,
				ButtonStyle   = ButtonStyle.ActivableIcon,
				AutoFocus     = false,
				Dock          = DockStyle.Left,
				IconUri       = MainToolbar.GetResourceIconUri (icon),
				PreferredSize = new Size (size, size),
			};

			ToolTip.Default.SetToolTip (button, tooltip);

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
			this.SetActiveState (this.buttonLabels,      (this.timelineMode & TimelineMode.Labels     ) != 0);
			this.SetActiveState (this.buttonCompacted,   (this.timelineMode & TimelineMode.Compacted  ) != 0);
			this.SetActiveState (this.buttonExpended,    (this.timelineMode & TimelineMode.Expanded   ) != 0);
			this.SetActiveState (this.buttonWeeksOfYear, (this.timelineMode & TimelineMode.WeeksOfYear) != 0);
			this.SetActiveState (this.buttonDaysOfWeek,  (this.timelineMode & TimelineMode.DaysOfWeek ) != 0);
			this.SetActiveState (this.buttonGraph,       (this.timelineMode & TimelineMode.Graph      ) != 0);
		}


		#region Events handler
		private void OnModeChanged(TimelineMode timelineMode)
		{
			this.ModeChanged.Raise (this, timelineMode);
		}

		public event EventHandler<TimelineMode> ModeChanged;
		#endregion


		private IconButton buttonLabels;
		private IconButton buttonCompacted;
		private IconButton buttonExpended;
		private IconButton buttonWeeksOfYear;
		private IconButton buttonDaysOfWeek;
		private IconButton buttonGraph;

		private IconButton buttonFirst;
		private IconButton buttonPrev;
		private IconButton buttonNext;
		private IconButton buttonLast;
		private IconButton buttonNow;
		private IconButton buttonNew;
		private IconButton buttonDelete;
		private IconButton buttonDeselect;

		private TimelineMode timelineMode;
	}
}
