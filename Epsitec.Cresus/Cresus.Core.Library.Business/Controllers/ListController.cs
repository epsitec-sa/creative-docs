//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using Epsitec.Common.Drawing;
using Epsitec.Common.Widgets;
using Epsitec.Common.Types;
using Epsitec.Common.Support;
using Epsitec.Common.Support.Extensions;

using Epsitec.Cresus.Core.Widgets;
using Epsitec.Cresus.Core.Widgets.Tiles;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Controllers
{
	/// <summary>
	/// Ce contrôleur gère une liste, représentée par des textes, surmontée des boutons [+] [-] [^] [v].
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public class ListController<T>
	{
		public ListController(IList<T> collection, System.Func<T, FormattedText> convertItemToText, System.Func<int, FormattedText> getTextInfo, System.Func<int, T> createItem)
		{
			//	FormattedText convertItemToText(T value)
			//		Converti un élément de la collection en un texte qui sera affiché dans la liste.
			//		Obligatoire.
			//
			//	FormattedText getTextInfo(int count)
			//		Retourne le texte affiché en haut à droite, indiquant le nombre d'éléments contenus.
			//		Facultatif.
			//
			//	T createItem(int index)
			//		Crée un nouvelle valeur qui sera insérée dans la collection.
			//		Obligatoire.

			System.Diagnostics.Debug.Assert (collection != null);
			System.Diagnostics.Debug.Assert (convertItemToText != null);
			System.Diagnostics.Debug.Assert (createItem != null);

			this.collection        = collection;
			this.convertItemToText = convertItemToText;
			this.getTextInfo       = getTextInfo;
			this.createItem        = createItem;
		}


		public void CreateUI(Widget parent, Direction arrowDirection, double? buttonSize = null, bool isReadOnly = false)
		{
			//	Les valeurs préférentielles pour buttonSize sont 19 et 23, pour obtenir un dessin optimal des GlyphButton.
			this.isReadOnly = isReadOnly;

			if (buttonSize == null)
			{
				buttonSize = Library.UI.TinyButtonSize;
			}

			this.toolbar = UIBuilder.CreateMiniToolbar (parent, buttonSize.Value);
			this.toolbar.Dock = DockStyle.Top;
			this.toolbar.Margins = new Margins (0, arrowDirection == Direction.Right ? Widgets.Tiles.TileArrow.Breadth : 0, 0, -1);
			this.toolbar.TabIndex = 1;

			this.addButton = new GlyphButton
			{
				Parent = toolbar,
				PreferredSize = new Size (buttonSize.Value*2+1, buttonSize.Value),
				GlyphShape = GlyphShape.Plus,
				Margins = new Margins (0, 0, 0, 0),
				Dock = DockStyle.Left,
			};

			this.removeButton = new GlyphButton
			{
				Parent = toolbar,
				PreferredSize = new Size (buttonSize.Value, buttonSize.Value),
				GlyphShape = GlyphShape.Minus,
				Margins = new Margins (1, 0, 0, 0),
				Dock = DockStyle.Left,
			};

			this.moveUpButton = new GlyphButton
			{
				Parent = toolbar,
				PreferredSize = new Size (buttonSize.Value, buttonSize.Value),
				GlyphShape = GlyphShape.ArrowUp,
				Margins = new Margins (System.Math.Floor (buttonSize.Value*0.55), 0, 0, 0),
				Dock = DockStyle.Left,
			};

			this.moveDownButton = new GlyphButton
			{
				Parent = toolbar,
				PreferredSize = new Size (buttonSize.Value, buttonSize.Value),
				GlyphShape = GlyphShape.ArrowDown,
				Margins = new Margins (1, 0, 0, 0),
				Dock = DockStyle.Left,
			};

			this.labelInfo = new StaticText
			{
				Parent = toolbar,
				ContentAlignment = ContentAlignment.MiddleRight,
				Dock = DockStyle.Fill,
				Margins = new Margins (0, 5, 0, 0),
			};

			//	Crée le cadre avec une flèche.
			var tile = new ArrowedTileFrame (arrowDirection)
			{
				Parent = parent,
				Dock = DockStyle.Fill,
				TabIndex = 2,
			};

			tile.Padding = tile.ContainerPadding + new Margins (1);

			//	Crée la liste.
			this.scrollList = new ScrollList
			{
				Parent = tile,
				ScrollListStyle = Common.Widgets.ScrollListStyle.FrameLess,
				Dock = DockStyle.Fill,
				Margins = new Margins (0, 0, 0, 0),
				TabIndex = 2,
			};

			//	Connecte tous les événements.
			this.addButton.Clicked += delegate
			{
				this.ActionAdd ();
			};

			this.removeButton.Clicked += delegate
			{
				this.ActionRemove ();
			};

			this.moveUpButton.Clicked += delegate
			{
				this.ActionMoveUp ();
			};

			this.moveDownButton.Clicked += delegate
			{
				this.ActionMoveDown ();
			};

			this.scrollList.SelectedItemChanged += delegate
			{
				this.ActionSelectionChanged ();
			};

			this.UpdateList ();
			this.UpdateButtons ();
		}


		public Button AddButton
		{
			get
			{
				return this.addButton;
			}
		}

		public Button RemoveButton
		{
			get
			{
				return this.removeButton;
			}
		}

		public Button MoveUpButton
		{
			get
			{
				return this.moveUpButton;
			}
		}

		public Button MoveDownButton
		{
			get
			{
				return this.moveDownButton;
			}
		}


		public void UpdateList(int? sel=null)
		{
			//	Met à jour toute la liste en fonction de la collection.
			if (!sel.HasValue)
			{
				sel = this.SelectedIndex;
			}

			this.ignoreChange = true;

			this.scrollList.Items.Clear ();

			foreach (var value in this.collection)
			{
				FormattedText text = this.convertItemToText (value);
				this.scrollList.Items.Add (text);
			}

			this.ignoreChange = false;

			if (sel.HasValue)
			{
				this.SelectedIndex = sel.Value;
				this.scrollList.ShowSelected (ScrollShowMode.Extremity);
			}

			if (this.getTextInfo != null)
			{
				FormattedText info = this.getTextInfo (this.collection.Count);
				this.labelInfo.FormattedText = info;
			}
		}

		public int SelectedIndex
		{
			//	Index de la ligne sélectionnée dans la liste.
			get
			{
				return this.scrollList.SelectedItemIndex;
			}
			set
			{
				this.ignoreChange = true;
				this.scrollList.SelectedItemIndex = value;
				this.ignoreChange = false;
			}
		}


		private void ActionAdd()
		{
			int sel = this.SelectedIndex;

			if (sel == -1)
			{
				sel = this.collection.Count;  // insère à la fin
			}
			else
			{
				sel++;  // insère après la ligne sélectionnée
			}

			var newItem = this.createItem (sel);
			this.collection.Insert (sel, newItem);

			this.UpdateList (sel);
			this.UpdateButtons ();

			this.OnSelectedItemChanged ();
			this.OnItemInserted ();  // doit venir après OnSelectedItemChanged !
		}

		private void ActionRemove()
		{
			int sel = this.SelectedIndex;

			this.collection.RemoveAt (sel);

			if (sel >= this.collection.Count)
			{
				sel = this.collection.Count-1;
			}

			this.UpdateList (sel);
			this.UpdateButtons ();

			this.OnSelectedItemChanged ();
		}

		private void ActionMoveUp()
		{
			int sel = this.SelectedIndex;

			var t = this.collection[sel];
			this.collection.RemoveAt (sel);
			this.collection.Insert (sel-1, t);

			this.UpdateList (sel-1);
			this.UpdateButtons ();

			this.OnSelectedItemChanged ();
		}

		private void ActionMoveDown()
		{
			int sel = this.SelectedIndex;

			var t = this.collection[sel];
			this.collection.RemoveAt (sel);
			this.collection.Insert (sel+1, t);

			this.UpdateList (sel+1);
			this.UpdateButtons ();

			this.OnSelectedItemChanged ();
		}

		private void ActionSelectionChanged()
		{
			if (this.ignoreChange)
			{
				return;
			}

			this.UpdateButtons ();

			this.OnSelectedItemChanged ();
		}


		private void UpdateButtons()
		{
			int sel = this.SelectedIndex;

			this.addButton.Enable      = !this.isReadOnly;
			this.removeButton.Enable   = !this.isReadOnly && sel != -1;
			this.moveUpButton.Enable   = !this.isReadOnly && sel > 0;
			this.moveDownButton.Enable = !this.isReadOnly && sel != -1 && sel < this.collection.Count-1;
		}




		#region Event manager
		private void OnItemInserted()
		{
			var handler = this.ItemInserted;
			if (handler != null)
			{
				handler (this);
			}
		}

		private void OnSelectedItemChanged()
		{
			var handler = this.SelectedItemChanged;
			if (handler != null)
			{
				handler (this);
			}
		}

		public event EventHandler ItemInserted;
		public event EventHandler SelectedItemChanged;
		#endregion


		private readonly IList<T>							collection;
		private readonly System.Func<T, FormattedText>		convertItemToText;
		private readonly System.Func<int, FormattedText>	getTextInfo;
		private readonly System.Func<int, T>				createItem;

		private FrameBox									toolbar;
		private GlyphButton									addButton;
		private GlyphButton									removeButton;
		private GlyphButton									moveUpButton;
		private GlyphButton									moveDownButton;
		private StaticText									labelInfo;
		private ScrollList									scrollList;
		private bool										isReadOnly;
		private bool										ignoreChange;
	}
}
