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
		public override void CreateUI(Widget parent)
		{
			this.CreateToolbar (parent, AbstractCommandToolbar.SecondaryToolbarHeight);
			this.UpdateCommandButtons ();
		}


		public bool HasTreeOperations
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


		protected override void UpdateCommandButtons()
		{
			this.UpdateCommandButton (this.buttonFirst,      ToolbarCommand.First);
			this.UpdateCommandButton (this.buttonPrev,       ToolbarCommand.Prev);
			this.UpdateCommandButton (this.buttonNext,       ToolbarCommand.Next);
			this.UpdateCommandButton (this.buttonLast,       ToolbarCommand.Last);
			this.UpdateCommandButton (this.buttonCompactAll, ToolbarCommand.CompactAll);
			this.UpdateCommandButton (this.buttonExpandAll,  ToolbarCommand.ExpandAll);
			this.UpdateCommandButton (this.buttonNew,        ToolbarCommand.New);
			this.UpdateCommandButton (this.buttonDelete,     ToolbarCommand.Delete);
			this.UpdateCommandButton (this.buttonDeselect,   ToolbarCommand.Deselect);
		}


		protected override void CreateToolbar(Widget parent, int size)
		{
			//	La toolbar s'adapte en fonction de la largeur disponible. Certains
			//	boutons non indispensables disparaissent s'il manque de la place.
			this.toolbar = new FrameBox
			{
				Parent          = parent,
				Dock            = DockStyle.Top,
				PreferredHeight = size,
				BackColor       = ColorManager.ToolbarBackgroundColor,
			};

			this.buttonFirst      = this.CreateCommandButton (this.toolbar, 0, ToolbarCommand.First,      "TreeTable.First",      "Retour sur la première ligne");
			this.buttonPrev       = this.CreateCommandButton (this.toolbar, 0, ToolbarCommand.Prev,       "TreeTable.Prev",       "Recule sur la ligne précédente");
			this.buttonNext       = this.CreateCommandButton (this.toolbar, 0, ToolbarCommand.Next,       "TreeTable.Next",       "Avance sur la ligne suivante");
			this.buttonLast       = this.CreateCommandButton (this.toolbar, 0, ToolbarCommand.Last,       "TreeTable.Last",       "Avance sur la dernière ligne");
			this.buttonCompactAll = this.CreateCommandButton (this.toolbar, 0, ToolbarCommand.CompactAll, "TreeTable.CompactAll", "Compacte tout");
			this.buttonExpandAll  = this.CreateCommandButton (this.toolbar, 0, ToolbarCommand.ExpandAll,  "TreeTable.ExpandAll",  "Etend tout");
			this.buttonNew        = this.CreateCommandButton (this.toolbar, 0, ToolbarCommand.New,        "TreeTable.New",        "Nouvel ligne");
			this.buttonDelete     = this.CreateCommandButton (this.toolbar, 0, ToolbarCommand.Delete,     "TreeTable.Delete",     "Supprimer la ligne");
			this.buttonDeselect   = this.CreateCommandButton (this.toolbar, 0, ToolbarCommand.Deselect,   "TreeTable.Deselect",   "Désélectionne la ligne");

			this.toolbar.SizeChanged += delegate
			{
				this.Adjust ();
			};
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
				bs.Button.Visibility = bs.Visibility;

				if (bs.Visibility)
				{
					x += bs.Offset;
					bs.Button.SetManualBounds (new Rectangle (x, 0, size, size));
					x += size;
				}
			}
		}

		private IEnumerable<ButtonState> GetButtons(double width, double size)
		{
			bool prevNext      = width > size*5 + 20;
			bool firstLast     = width > size*7 + 20;
			bool compactExpand = width > size*9 + 20 + 20 && this.hasTreeOperations;

			yield return new ButtonState (this.buttonFirst, firstLast);
			yield return new ButtonState (this.buttonPrev,  prevNext);
			yield return new ButtonState (this.buttonNext,  prevNext);
			yield return new ButtonState (this.buttonLast,  firstLast);

			//	L'offset à gauche du bouton New n'a de raison d'être que si les
			//	boutons First/Prev/Next/Last sont présents.
			yield return new ButtonState (this.buttonCompactAll, compactExpand, offset: firstLast || prevNext ? 20 : 0);
			yield return new ButtonState (this.buttonExpandAll,  compactExpand);

			//	L'offset à gauche du bouton New n'a de raison d'être que si les
			//	boutons First/Prev/Next/Last/CompactAll/ExpandAll sont présents.
			yield return new ButtonState (this.buttonNew, offset: firstLast || prevNext || compactExpand ? 20 : 0);
			yield return new ButtonState (this.buttonDelete);
			yield return new ButtonState (this.buttonDeselect);
		}

		private struct ButtonState
		{
			public ButtonState(IconButton button, bool visibility = true, double offset = 0)
			{
				this.Button     = button;
				this.Visibility = visibility;
				this.Offset     = offset;
			}

			public readonly IconButton	Button;
			public readonly bool		Visibility;
			public readonly double		Offset;
		}


		private FrameBox						toolbar;

		private IconButton						buttonFirst;
		private IconButton						buttonPrev;
		private IconButton						buttonNext;
		private IconButton						buttonLast;

		private IconButton						buttonCompactAll;
		private IconButton						buttonExpandAll;

		private IconButton						buttonNew;
		private IconButton						buttonDelete;
		private IconButton						buttonDeselect;

		private bool							hasTreeOperations;
	}
}
