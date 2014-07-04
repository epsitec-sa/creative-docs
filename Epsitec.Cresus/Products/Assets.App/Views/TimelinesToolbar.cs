//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;

using Epsitec.Common.Widgets;
using Epsitec.Cresus.Assets.App.Helpers;
using Epsitec.Cresus.Assets.App.Widgets;
using Epsitec.Cresus.Assets.Server.BusinessLogic;

namespace Epsitec.Cresus.Assets.App.Views
{
	public class TimelinesToolbar : AbstractCommandToolbar
	{
		protected override void CreateCommands()
		{
			this.SetCommandDescription (ToolbarCommand.Narrow,                 "Timeline.Narrow",         "Affichage étroit");
			this.SetCommandDescription (ToolbarCommand.Wide,                   "Timeline.Wide",           "Affichage large");
			this.SetCommandDescription (ToolbarCommand.First,                  "Timeline.First",          "Retourner sur le premier événement");
			this.SetCommandDescription (ToolbarCommand.Prev,                   "Timeline.Prev",           "Reculer sur l'événement précédent");
			this.SetCommandDescription (ToolbarCommand.Next,                   "Timeline.Next",           "Avancer sur l'événement suivant");
			this.SetCommandDescription (ToolbarCommand.Last,                   "Timeline.Last",           "Avancer sur le dernier événement");
			this.SetCommandDescription (ToolbarCommand.New,                    "TreeTable.New.Event",     "Nouvel événement");
			this.SetCommandDescription (ToolbarCommand.Delete,                 "Timeline.Delete",         "Supprimer l'événement");
			this.SetCommandDescription (ToolbarCommand.AmortizationsPreview,   "Amortizations.Preview",   "Générer les préamortissements");
			this.SetCommandDescription (ToolbarCommand.AmortizationsFix,       "Amortizations.Fix",       "Fixer les préamortissements");
			this.SetCommandDescription (ToolbarCommand.AmortizationsToExtra,   "Amortizations.ToExtra",   "Transformer en amortissement extraordinaire");
			this.SetCommandDescription (ToolbarCommand.AmortizationsUnpreview, "Amortizations.Unpreview", "Supprimer les préamortissements");
			this.SetCommandDescription (ToolbarCommand.AmortizationsDelete,    "Amortizations.Delete",    "Supprimer des amortissements ordinaires");
			this.SetCommandDescription (ToolbarCommand.Deselect,               "Timeline.Deselect",       "Désélectionner l'événement");
			this.SetCommandDescription (ToolbarCommand.Copy,                   "TreeTable.Copy.Event",    "Copier l'événement");
			this.SetCommandDescription (ToolbarCommand.Paste,                  "TreeTable.Paste.Event",   "Coller l'événement");
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

			this.buttonCompacted = this.CreateModeButton   (TimelinesMode.Narrow, ToolbarCommand.Narrow);
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
