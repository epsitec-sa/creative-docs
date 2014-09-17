//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;

using Epsitec.Common.Widgets;
using Epsitec.Common.Drawing;
using Epsitec.Cresus.Assets.Server.SimpleEngine;

namespace Epsitec.Cresus.Assets.App.Views.CommandToolbars
{
	/// <summary>
	/// La toolbar s'adapte en fonction de la largeur disponible. Certains
	/// boutons non indispensables disparaissent s'il manque de la place.
	/// </summary>
	public abstract class AbstractTreeTableToolbar : AbstractCommandToolbar
	{
		public AbstractTreeTableToolbar(DataAccessor accessor, CommandContext commandContext)
			: base (accessor, commandContext)
		{
		}


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

		public bool								HasDateRange
		{
			get
			{
				return this.hasDateRange;
			}
			set
			{
				if (this.hasDateRange != value)
				{
					this.hasDateRange = value;
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


		public override void CreateUI(Widget parent)
		{
			base.CreateUI (parent);

			this.toolbar.SizeChanged += delegate
			{
				this.Adjust ();
			};
		}

#if false
		public override void CreateUI(Widget parent)
		{
			//	La toolbar s'adapte en fonction de la largeur disponible. Certains
			//	boutons non indispensables disparaissent s'il manque de la place.
			base.CreateUI (parent);

			this.buttonFilter     = this.CreateCommandButton (DockStyle.None, Res.Commands.TreeTable.Filter);
			this.buttonDateRange  = this.CreateCommandButton (DockStyle.None, Res.Commands.TreeTable.DateRange);
			this.buttonGraphic    = this.CreateCommandButton (DockStyle.None, Res.Commands.TreeTable.Graphic);

			this.buttonFirst      = this.CreateCommandButton (DockStyle.None, Res.Commands.TreeTable.First);
			this.buttonPrev       = this.CreateCommandButton (DockStyle.None, Res.Commands.TreeTable.Prev);
			this.buttonNext       = this.CreateCommandButton (DockStyle.None, Res.Commands.TreeTable.Next);
			this.buttonLast       = this.CreateCommandButton (DockStyle.None, Res.Commands.TreeTable.Last);

			this.separator1       = this.CreateSeparator     (DockStyle.None);
			
			this.buttonCompactAll = this.CreateCommandButton (DockStyle.None, Res.Commands.TreeTable.CompactAll);
			this.buttonCompactOne = this.CreateCommandButton (DockStyle.None, Res.Commands.TreeTable.CompactOne);
			this.buttonExpandOne  = this.CreateCommandButton (DockStyle.None, Res.Commands.TreeTable.ExpandOne);
			this.buttonExpandAll  = this.CreateCommandButton (DockStyle.None, Res.Commands.TreeTable.ExpandAll);
			
			this.separator2       = this.CreateSeparator     (DockStyle.None);
			
			this.buttonMoveTop    = this.CreateCommandButton (DockStyle.None, Res.Commands.TreeTable.MoveTop);
			this.buttonMoveUp     = this.CreateCommandButton (DockStyle.None, Res.Commands.TreeTable.MoveUp);
			this.buttonMoveDown   = this.CreateCommandButton (DockStyle.None, Res.Commands.TreeTable.MoveDown);
			this.buttonMoveBottom = this.CreateCommandButton (DockStyle.None, Res.Commands.TreeTable.MoveBottom);

			this.separator3       = this.CreateSeparator     (DockStyle.None);
			
			this.buttonNew        = this.CreateCommandButton (DockStyle.None, Res.Commands.TreeTable.New);
			this.buttonDelete     = this.CreateCommandButton (DockStyle.None, Res.Commands.TreeTable.Delete);
			this.buttonDeselect   = this.CreateCommandButton (DockStyle.None, Res.Commands.TreeTable.Deselect);

			this.separator4       = this.CreateSeparator     (DockStyle.None);

			this.buttonCopy       = this.CreateCommandButton (DockStyle.None, Res.Commands.TreeTable.Copy);
			this.buttonPaste      = this.CreateCommandButton (DockStyle.None, Res.Commands.TreeTable.Paste);
			this.buttonImport     = this.CreateCommandButton (DockStyle.None, Res.Commands.TreeTable.Import);
			this.buttonExport     = this.CreateCommandButton (DockStyle.None, Res.Commands.TreeTable.Export);
			this.buttonGoto       = this.CreateCommandButton (DockStyle.None, Res.Commands.TreeTable.Goto);

			this.toolbar.SizeChanged += delegate
			{
				this.Adjust ();
			};

			this.AttachShortcuts ();
		}
#endif


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
				if (bs.Widget == null)
				{
					continue;
				}

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

			if (this.hasGraphic || this.hasFilter || this.hasDateRange)
			{
				used += this.hasGraphic   ? size : 0;  // place pour Graphic
				used += this.hasFilter    ? size : 0;  // place pour Filter
				used += this.hasDateRange ? size : 0;  // place pour DateRange
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
				used += size*this.CopyPasteGroupCount + AbstractCommandToolbar.separatorWidth;
				copyPaste = true;
				sep4 = true;
			}

			yield return new ButtonState (this.buttonFilter,    this.hasFilter);
			yield return new ButtonState (this.buttonDateRange, this.hasDateRange);
			yield return new ButtonState (this.buttonGraphic,   this.hasGraphic);
			yield return new ButtonState (this.separator1,      this.hasGraphic | this.hasFilter | this.hasDateRange);

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

			if (this.buttonCopy != null)
			{
				yield return new ButtonState (this.buttonCopy, copyPaste);
			}

			if (this.buttonPaste != null)
			{
				yield return new ButtonState (this.buttonPaste, copyPaste);
			}

			if (this.buttonImport != null)
			{
				yield return new ButtonState (this.buttonImport, copyPaste);
			}

			if (this.buttonPaste != null)
			{
				yield return new ButtonState (this.buttonExport, copyPaste);
			}

			if (this.buttonGoto != null)
			{
				yield return new ButtonState (this.buttonGoto, copyPaste);
			}
		}

		private int CopyPasteGroupCount
		{
			get
			{
				return this.CopyPasteGroup.Where (x => x != null).Count ();
			}
		}

		private IEnumerable<IconButton> CopyPasteGroup
		{
			get
			{
				yield return this.buttonCopy;
				yield return this.buttonPaste;
				yield return this.buttonExport;
				yield return this.buttonImport;
			}
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


		protected IconButton					buttonFilter;
		protected IconButton					buttonDateRange;
		protected IconButton					buttonGraphic;

		protected IconButton					buttonFirst;
		protected IconButton					buttonPrev;
		protected IconButton					buttonNext;
		protected IconButton					buttonLast;

		protected FrameBox						separator1;

		protected IconButton					buttonCompactAll;
		protected IconButton					buttonCompactOne;
		protected IconButton					buttonExpandOne;
		protected IconButton					buttonExpandAll;

		protected FrameBox						separator2;

		protected IconButton					buttonMoveTop;
		protected IconButton					buttonMoveUp;
		protected IconButton					buttonMoveDown;
		protected IconButton					buttonMoveBottom;

		protected FrameBox						separator3;

		protected IconButton					buttonNew;
		protected IconButton					buttonDelete;
		protected IconButton					buttonDeselect;

		protected FrameBox						separator4;

		protected IconButton					buttonCopy;
		protected IconButton					buttonPaste;
		protected IconButton					buttonExport;
		protected IconButton					buttonImport;
		protected IconButton					buttonGoto;

		protected bool							hasGraphic;
		protected bool							hasFilter;
		protected bool							hasDateRange;
		protected bool							hasGraphicOperations;
		protected bool							hasTreeOperations;
		protected bool							hasMoveOperations;
	}
}
