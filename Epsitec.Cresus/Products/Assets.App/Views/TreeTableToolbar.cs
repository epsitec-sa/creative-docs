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
		public string							NewIcon;
		public string							NewTooltip;
		public string							DeleteIcon;
		public string							DeleteTooltip;
		public string							DeselectIcon;
		public string							DeselectTooltip;

		public bool								HasGraphic
		{
			get
			{
				return this.hasGraphic;
			}
			set
			{
				if (this.hasGraphic != value)
				{
					this.hasGraphic = value;
					this.Adjust ();
				}
			}
		}

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

		public bool								HasGraphicOperations
		{
			get
			{
				return this.hasGraphicOperations;
			}
			set
			{
				if (this.hasGraphicOperations != value)
				{
					this.hasGraphicOperations = value;
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

			this.buttonFilter     = this.CreateCommandButton (DockStyle.None, ToolbarCommand.Filter,     "TreeTable.Filter",     "Grouper les objets selon");
			this.buttonGraphic    = this.CreateCommandButton (DockStyle.None, ToolbarCommand.Graphic,    "TreeTable.Graphic",    "Représentation graphique");

			this.buttonFirst      = this.CreateCommandButton (DockStyle.None, ToolbarCommand.First,      "TreeTable.First",      "Retourner sur la première ligne");
			this.buttonPrev       = this.CreateCommandButton (DockStyle.None, ToolbarCommand.Prev,       "TreeTable.Prev",       "Reculer sur la ligne précédente");
			this.buttonNext       = this.CreateCommandButton (DockStyle.None, ToolbarCommand.Next,       "TreeTable.Next",       "Avancer sur la ligne suivante");
			this.buttonLast       = this.CreateCommandButton (DockStyle.None, ToolbarCommand.Last,       "TreeTable.Last",       "Avancer sur la dernière ligne");

			this.separator1       = this.CreateSeparator     (DockStyle.None);
			
			this.buttonCompactAll = this.CreateCommandButton (DockStyle.None, ToolbarCommand.CompactAll, "TreeTable.CompactAll", "Compacter tout");
			this.buttonCompactOne = this.CreateCommandButton (DockStyle.None, ToolbarCommand.CompactOne, "TreeTable.CompactOne", "Compacter un niveau");
			this.buttonExpandOne  = this.CreateCommandButton (DockStyle.None, ToolbarCommand.ExpandOne,  "TreeTable.ExpandOne",  "Etendre un niveau");
			this.buttonExpandAll  = this.CreateCommandButton (DockStyle.None, ToolbarCommand.ExpandAll,  "TreeTable.ExpandAll",  "Etendre tout");
			
			this.separator2       = this.CreateSeparator     (DockStyle.None);
			
			this.buttonMoveTop    = this.CreateCommandButton (DockStyle.None, ToolbarCommand.MoveTop,    "TreeTable.MoveTop",    "Déplacer la ligne au sommet");
			this.buttonMoveUp     = this.CreateCommandButton (DockStyle.None, ToolbarCommand.MoveUp,     "TreeTable.MoveUp",     "Monter la ligne");
			this.buttonMoveDown   = this.CreateCommandButton (DockStyle.None, ToolbarCommand.MoveDown,   "TreeTable.MoveDown",   "Descendre la ligne");
			this.buttonMoveBottom = this.CreateCommandButton (DockStyle.None, ToolbarCommand.MoveBottom, "TreeTable.MoveBottom", "Déplacer la ligne à la fin");

			this.separator3       = this.CreateSeparator     (DockStyle.None);
			
			this.buttonNew        = this.CreateCommandButton (DockStyle.None, ToolbarCommand.New,        this.NewIcon      ?? "TreeTable.New",        this.NewTooltip      ?? "Nouvelle ligne");
			this.buttonDelete     = this.CreateCommandButton (DockStyle.None, ToolbarCommand.Delete,     this.DeleteIcon   ?? "TreeTable.Delete",     this.DeleteTooltip   ?? "Supprimer la ligne");
			this.buttonDeselect   = this.CreateCommandButton (DockStyle.None, ToolbarCommand.Deselect,   this.DeselectIcon ?? "TreeTable.Deselect",   this.DeselectTooltip ?? "Désélectionner la ligne");

			this.separator4       = this.CreateSeparator     (DockStyle.None);

			this.buttonCopy       = this.CreateCommandButton (DockStyle.None, ToolbarCommand.Copy,       "TreeTable.Copy",       "Copier");
			this.buttonPaste      = this.CreateCommandButton (DockStyle.None, ToolbarCommand.Paste,      "TreeTable.Paste",      "Coller");
			this.buttonExport     = this.CreateCommandButton (DockStyle.None, ToolbarCommand.Export,     "TreeTable.Export",     "Exporter");

			this.buttonGraphic.ButtonStyle = ButtonStyle.ActivableIcon;
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
			bool prevNext      = false;
			bool firstLast     = false;
			bool compactExpand = false;
			bool moveLimit     = false;
			bool moveStep      = false;
			bool copyPaste     = false;
			bool sep1          = false;
			bool sep2          = false;
			bool sep3          = false;
			bool sep4          = false;

			double used = size*3;  // place pour New/Delete/Deselect

			if (this.hasGraphic || this.hasFilter)
			{
				used += this.hasGraphic ? size : 0;  // place pour Graphic
				used += this.hasFilter  ? size : 0;  // place pour Filter
				used += AbstractCommandToolbar.separatorWidth;
			}

			if (this.hasMoveOperations)
			{
				used += AbstractCommandToolbar.separatorWidth;
				sep3 = true;

				if (width > used + size*2)
				{
					used += size*2;
					moveStep = true;
				}

				if (width > used + size*2)
				{
					used += size*2;
					moveLimit = true;
				}
			}

			if (width > used + size*2 + AbstractCommandToolbar.separatorWidth)
			{
				used += size*2 + AbstractCommandToolbar.separatorWidth;
				prevNext = true;
				sep1 = true;

				if (width > used + size*2)
				{
					used += size*2;
					firstLast = true;
				}
			}

			if (this.hasTreeOperations)
			{
				if (width > used + size*4 + AbstractCommandToolbar.separatorWidth)
				{
					used += size*4 + AbstractCommandToolbar.separatorWidth;
					compactExpand = true;
					sep2 = true;
				}
			}

			if (width > used + size*3 + AbstractCommandToolbar.separatorWidth)
			{
				used += size*3 + AbstractCommandToolbar.separatorWidth;
				copyPaste = true;
				sep4 = true;
			}

			yield return new ButtonState (this.buttonFilter,  this.hasFilter);
			yield return new ButtonState (this.buttonGraphic, this.hasGraphic);
			yield return new ButtonState (this.separator1,    this.hasGraphic | this.hasFilter);

			yield return new ButtonState (this.buttonFirst, firstLast);
			yield return new ButtonState (this.buttonPrev,  prevNext);
			yield return new ButtonState (this.buttonNext,  prevNext);
			yield return new ButtonState (this.buttonLast,  firstLast);

			yield return new ButtonState (this.separator1, sep1);

			yield return new ButtonState (this.buttonCompactAll, compactExpand);
			yield return new ButtonState (this.buttonCompactOne, compactExpand);
			yield return new ButtonState (this.buttonExpandOne,  compactExpand);
			yield return new ButtonState (this.buttonExpandAll,  compactExpand);

			yield return new ButtonState (this.separator2, sep2);

			yield return new ButtonState (this.buttonMoveTop,    moveLimit);
			yield return new ButtonState (this.buttonMoveUp,     moveStep);
			yield return new ButtonState (this.buttonMoveDown,   moveStep);
			yield return new ButtonState (this.buttonMoveBottom, moveLimit);

			yield return new ButtonState (this.separator3, sep3);

			yield return new ButtonState (this.buttonNew);
			yield return new ButtonState (this.buttonDelete);
			yield return new ButtonState (this.buttonDeselect);

			yield return new ButtonState (this.separator4, sep4);

			yield return new ButtonState (this.buttonCopy,   copyPaste);
			yield return new ButtonState (this.buttonPaste,  copyPaste);
			yield return new ButtonState (this.buttonExport, copyPaste);
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
		private IconButton						buttonGraphic;

		private IconButton						buttonFirst;
		private IconButton						buttonPrev;
		private IconButton						buttonNext;
		private IconButton						buttonLast;

		private FrameBox						separator1;

		private IconButton						buttonCompactAll;
		private IconButton						buttonCompactOne;
		private IconButton						buttonExpandOne;
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

		private FrameBox						separator4;

		private IconButton						buttonCopy;
		private IconButton						buttonPaste;
		private IconButton						buttonExport;

		private bool							hasGraphic;
		private bool							hasFilter;
		private bool							hasGraphicOperations;
		private bool							hasTreeOperations;
		private bool							hasMoveOperations;
	}
}
