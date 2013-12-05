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
	public class TimelinesToolbar : AbstractCommandToolbar
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
			//	La toolbar s'adapte en fonction de la largeur disponible. Certains
			//	boutons non indispensables disparaissent s'il manque de la place.
			this.toolbar = new FrameBox
			{
				Parent          = parent,
				Dock            = DockStyle.Top,
				PreferredHeight = AbstractCommandToolbar.secondaryToolbarHeight,
				BackColor       = ColorManager.ToolbarBackgroundColor,
			};

			this.buttonCompacted = this.CreateModeButton   (TimelineMode.Compacted, ToolbarCommand.CompactAll, "Timeline.Single", "Affichage étroit");
			this.buttonExpended  = this.CreateModeButton   (TimelineMode.Expanded,  ToolbarCommand.ExpandAll,  "Timeline.Double", "Affichage large");

			this.buttonFirst    = this.CreateCommandButton (DockStyle.None, ToolbarCommand.First,    "Timeline.First",    "Retour sur le premier événement");
			this.buttonPrev     = this.CreateCommandButton (DockStyle.None, ToolbarCommand.Prev,     "Timeline.Prev",     "Recule sur l'événement précédent");
			this.buttonNext     = this.CreateCommandButton (DockStyle.None, ToolbarCommand.Next,     "Timeline.Next",     "Avance sur l'événement suivant");
			this.buttonLast     = this.CreateCommandButton (DockStyle.None, ToolbarCommand.Last,     "Timeline.Last",     "Avance sur le dernier événement");

			this.separator1     = this.CreateSeparator     (DockStyle.None);
			
			this.buttonNew      = this.CreateCommandButton (DockStyle.None, ToolbarCommand.New,      "Timeline.New",      "Nouvel événement");
			this.buttonDelete   = this.CreateCommandButton (DockStyle.None, ToolbarCommand.Delete,   "Timeline.Delete",   "Supprimer l'événement");
			this.buttonDeselect = this.CreateCommandButton (DockStyle.None, ToolbarCommand.Deselect, "Timeline.Deselect", "Désélectionne l'événement");

			this.toolbar.SizeChanged += delegate
			{
				this.Adjust ();
			};

			return this.toolbar;
		}


		private IconButton CreateModeButton(TimelineMode mode, ToolbarCommand command, string icon, string tooltip)
		{
			//	Utilise DockStyle.None, car le bouton est positionnée avec SetManualBounds.
			var button = this.CreateCommandButton (DockStyle.None, command, icon, tooltip);
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
			this.SetCommandActivate (ToolbarCommand.CompactAll, (this.timelineMode & TimelineMode.Compacted) != 0);
			this.SetCommandActivate (ToolbarCommand.ExpandAll,  (this.timelineMode & TimelineMode.Expanded ) != 0);
		}


		private void Adjust()
		{
			//	S'il manque de la place en largeur, on supprime des boutons avec
			//	cette priorité:
			//	- CompactAll/ExpandAll
			//	- First/Last
			//	- Prev/Next
			if (this.toolbar == null)
			{
				return;
			}

			double size = this.toolbar.ActualHeight;
			double x = 0;

			foreach (var bs in this.GetButtons (this.toolbar.ActualWidth, size))
			{
				bs.Widget.Visibility = bs.Visibility;

				if (bs.Visibility)
				{
					if (bs.Widget is IconButton)
					{
						bs.Widget.SetManualBounds (new Rectangle (x, 0, size, size));
						x += size;
					}
					else if (bs.Widget is FrameBox)
					{
						x += AbstractCommandToolbar.separatorWidth/2;
						bs.Widget.SetManualBounds (new Rectangle (x, 0, 1, size));
						x += AbstractCommandToolbar.separatorWidth/2;
					}
				}
			}
		}

		private IEnumerable<ButtonState> GetButtons(double width, double size)
		{
			bool prevNext  = width > size*7 + AbstractCommandToolbar.separatorWidth*2;
			bool firstLast = width > size*9 + AbstractCommandToolbar.separatorWidth;

			yield return new ButtonState (this.buttonCompacted);
			yield return new ButtonState (this.buttonExpended);

			yield return new ButtonState (this.separator1, firstLast || prevNext);

			yield return new ButtonState (this.buttonFirst, firstLast);
			yield return new ButtonState (this.buttonPrev,  prevNext);
			yield return new ButtonState (this.buttonNext,  prevNext);
			yield return new ButtonState (this.buttonLast,  firstLast);

			yield return new ButtonState (this.separator1, firstLast || prevNext);

			yield return new ButtonState (this.buttonNew);
			yield return new ButtonState (this.buttonDelete);
			yield return new ButtonState (this.buttonDeselect);
		}

		private struct ButtonState
		{
			public ButtonState(Widget widget, bool visibility = true)
			{
				this.Widget     = widget;
				this.Visibility = visibility;
			}

			public readonly Widget		Widget;
			public readonly bool		Visibility;
		}


		#region Events handler
		private void OnModeChanged(TimelineMode timelineMode)
		{
			this.ModeChanged.Raise (this, timelineMode);
		}

		public event EventHandler<TimelineMode> ModeChanged;
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

		private TimelineMode					timelineMode;
	}
}
