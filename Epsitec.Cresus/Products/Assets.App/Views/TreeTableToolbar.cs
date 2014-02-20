//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;

using Epsitec.Common.Widgets;
using Epsitec.Common.Drawing;
using Epsitec.Cresus.Assets.App.Widgets;

namespace Epsitec.Cresus.Assets.App.Views
{
	public class TreeTableToolbar : AbstractCommandToolbar
	{
		public bool								HasFilter
		{
			get
			{
				return this.hasFilter;
			}
			set
			{
				if (this.hasFilter != value)
				{
					this.hasFilter = value;
					this.Adjust ();
				}
			}
		}

		public bool								HasTreeOperations
		{
			get
			{
				return this.hasTreeOperations;
			}
			set
			{
				if (this.hasTreeOperations != value)
				{
					this.hasTreeOperations = value;
					this.Adjust ();
				}
			}
		}

		public bool								HasMoveOperations
		{
			get
			{
				return this.hasMoveOperations;
			}
			set
			{
				if (this.hasMoveOperations != value)
				{
					this.hasMoveOperations = value;
					this.Adjust ();
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

			this.buttonFilter     = this.CreateCommandButton (DockStyle.None, ToolbarCommand.Filter,     "TreeTable.Filter",     "Filtre");

			this.buttonFirst      = this.CreateCommandButton (DockStyle.None, ToolbarCommand.First,      "TreeTable.First",      "Retour sur la première ligne");
			this.buttonPrev       = this.CreateCommandButton (DockStyle.None, ToolbarCommand.Prev,       "TreeTable.Prev",       "Recule sur la ligne précédente");
			this.buttonNext       = this.CreateCommandButton (DockStyle.None, ToolbarCommand.Next,       "TreeTable.Next",       "Avance sur la ligne suivante");
			this.buttonLast       = this.CreateCommandButton (DockStyle.None, ToolbarCommand.Last,       "TreeTable.Last",       "Avance sur la dernière ligne");

			this.separator1       = this.CreateSeparator     (DockStyle.None);
			
			this.buttonCompactAll = this.CreateCommandButton (DockStyle.None, ToolbarCommand.CompactAll, "TreeTable.CompactAll", "Compacte tout");
			this.buttonExpandAll  = this.CreateCommandButton (DockStyle.None, ToolbarCommand.ExpandAll,  "TreeTable.ExpandAll",  "Etend tout");
			
			this.separator2       = this.CreateSeparator     (DockStyle.None);
			
			this.buttonMoveTop    = this.CreateCommandButton (DockStyle.None, ToolbarCommand.MoveTop,    "TreeTable.MoveTop",    "Déplace la ligne au sommet");
			this.buttonMoveUp     = this.CreateCommandButton (DockStyle.None, ToolbarCommand.MoveUp,     "TreeTable.MoveUp",     "Monte la ligne");
			this.buttonMoveDown   = this.CreateCommandButton (DockStyle.None, ToolbarCommand.MoveDown,   "TreeTable.MoveDown",   "Descend la ligne");
			this.buttonMoveBottom = this.CreateCommandButton (DockStyle.None, ToolbarCommand.MoveBottom, "TreeTable.MoveBottom", "Déplace la ligne à la fin");

			this.separator3       = this.CreateSeparator     (DockStyle.None);
			
			this.buttonNew        = this.CreateCommandButton (DockStyle.None, ToolbarCommand.New,        "TreeTable.New",        "Nouvel ligne");
			this.buttonDelete     = this.CreateCommandButton (DockStyle.None, ToolbarCommand.Delete,     "TreeTable.Delete",     "Supprimer la ligne");
			this.buttonDeselect   = this.CreateCommandButton (DockStyle.None, ToolbarCommand.Deselect,   "TreeTable.Deselect",   "Désélectionne la ligne");

			this.buttonFilter.ButtonStyle = ButtonStyle.ActivableIcon;

			this.toolbar.SizeChanged += delegate
			{
				this.Adjust ();
			};

			return this.toolbar;
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
			int f = this.hasFilter ? 1:0;

			bool prevNext      = width > size*(f+5) + AbstractCommandToolbar.separatorWidth*1;
			bool firstLast     = width > size*(f+7) + AbstractCommandToolbar.separatorWidth*2;
			bool compactExpand = width > size*(f+9) + AbstractCommandToolbar.separatorWidth*3 && this.hasTreeOperations;
			bool move          = this.hasMoveOperations;

			yield return new ButtonState (this.buttonFilter, this.hasFilter);
			yield return new ButtonState (this.separator1, this.hasFilter);

			yield return new ButtonState (this.buttonFirst, firstLast);
			yield return new ButtonState (this.buttonPrev,  prevNext);
			yield return new ButtonState (this.buttonNext,  prevNext);
			yield return new ButtonState (this.buttonLast,  firstLast);

			yield return new ButtonState (this.separator1, firstLast || prevNext);

			yield return new ButtonState (this.buttonCompactAll, compactExpand);
			yield return new ButtonState (this.buttonExpandAll,  compactExpand);

			yield return new ButtonState (this.separator2, compactExpand);

			yield return new ButtonState (this.buttonMoveTop,    move);
			yield return new ButtonState (this.buttonMoveUp,     move);
			yield return new ButtonState (this.buttonMoveDown,   move);
			yield return new ButtonState (this.buttonMoveBottom, move);

			yield return new ButtonState (this.separator3, move);

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


		private IconButton						buttonFilter;

		private IconButton						buttonFirst;
		private IconButton						buttonPrev;
		private IconButton						buttonNext;
		private IconButton						buttonLast;

		private FrameBox						separator1;
		
		private IconButton						buttonCompactAll;
		private IconButton						buttonExpandAll;

		private FrameBox						separator2;

		private IconButton						buttonMoveTop;
		private IconButton						buttonMoveUp;
		private IconButton						buttonMoveDown;
		private IconButton						buttonMoveBottom;

		private FrameBox						separator3;

		private IconButton						buttonNew;
		private IconButton						buttonDelete;
		private IconButton						buttonDeselect;

		private bool							hasFilter;
		private bool							hasTreeOperations;
		private bool							hasMoveOperations;
	}
}
