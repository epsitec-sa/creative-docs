//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using Epsitec.Common.Support;
using Epsitec.Common.Types;
using Epsitec.Common.Drawing;
using Epsitec.Common.Widgets;
using Epsitec.Common.Widgets.Helpers;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Widgets
{
	public enum HintEditorComboMenu
	{
		Never,
		IfReasonable,
		Always,
	}


	public class HintEditor : TextFieldEx, Common.Widgets.Collections.IStringCollectionHost, Common.Support.Data.INamedStringSelection
	{
		public HintEditor()
		{
			this.HintEditorComboMenu = Widgets.HintEditorComboMenu.IfReasonable;
			this.ComboMenuReasonableItemsLimit = 100;

			this.TextDisplayMode = TextFieldDisplayMode.ActiveHint;
			this.DefocusAction = Common.Widgets.DefocusAction.AcceptEdition;

			this.items = new Common.Widgets.Collections.StringCollection (this);
			this.selectedRow = -1;

			this.hintListIndex = new List<int> ();
			this.hintSelected = -1;
		}

		public HintEditor(Widget embedder)
			: this ()
		{
			this.SetEmbedder (embedder);
		}


		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				this.CloseComboMenu ();
			}

			base.Dispose (disposing);
		}


		public HintEditorComboMenu HintEditorComboMenu
		{
			get;
			set;
		}

		/// <summary>
		/// Lorsque HintEditorComboMenu = IfReasonable, détermine à partir de combien le combo-menu est caché.
		/// </summary>
		/// <value>The combo menu items limit.</value>
		public int ComboMenuReasonableItemsLimit
		{
			get;
			set;
		}

		public System.Func<string, string> HintConverter
		{
			get;
			set;
		}


		#region IStringCollectionHost Members

		public void NotifyStringCollectionChanged()
		{
		}

		public Common.Widgets.Collections.StringCollection Items
		{
			get
			{
				return this.items;
			}
		}

		#endregion

		#region INamedStringSelection Members

		public string SelectedName
		{
			get
			{
				if (this.selectedRow == -1)
				{
					return null;
				}

				return this.items.GetName (this.selectedRow);
			}
			set
			{
				if (this.SelectedName != value)
				{
					int index = -1;

					if (value != null)
					{
						index = this.items.FindIndexByName (value);

						if (index < 0)
						{
							throw new System.ArgumentException (string.Format ("No element named '{0}' in list", value));
						}
					}

					this.SelectedIndex = index;
				}
			}
		}

		#endregion

		#region IStringSelection Members

		public int SelectedIndex
		{
			get
			{
				return this.selectedRow;
			}
			set
			{
				if (value != -1)
				{
					value = System.Math.Max (value, 0);
					value = System.Math.Min (value, this.items.Count-1);
				}
				if (value != this.selectedRow)
				{
					this.selectedRow = value;
					this.OnSelectedIndexChanged ();
				}
			}
		}

		public string SelectedItem
		{
			get
			{
				int index = this.SelectedIndex;
				
				if (index < 0)
				{
					return null;
				}

				return this.Items[index];
			}
			set
			{
				this.SelectedIndex = this.Items.IndexOf (value);
			}
		}

		public event EventHandler  SelectedIndexChanged
		{
			add
			{
				this.AddUserEventHandler("SelectedIndexChanged", value);
			}
			remove
			{
				this.RemoveUserEventHandler("SelectedIndexChanged", value);
			}
		}

		#endregion


		private void HintSearching(string typed)
		{
			this.hintListIndex.Clear ();

			typed = this.HintConvert (typed);

			if (!string.IsNullOrEmpty (typed))
			{
				List<int>[] lists = new List<int>[2];
				lists[0] = new List<int> ();
				lists[1] = new List<int> ();

				for (int i=0; i<this.items.Count; i++)
				{
					var item = this.items[i];

					string name = this.HintConvert (item.ToString ());
					int result = HintEditor.HintWordSearching (typed, name);

					if (result == 0 || result == 1)
					{
						lists[result].Add (i);
					}
				}

				this.hintListIndex.AddRange (lists[0]);
				this.hintListIndex.AddRange (lists[1]);
			}

			if (this.hintListIndex.Count == 0)
			{
				this.hintSelected = -1;
			}
			else
			{
				this.hintSelected = 0;  // la première proposition
			}

			this.UseSelectedHint ();

			//	Gère le combo menu.
			if (this.hintListIndex.Count <= 1)
			{
				if (this.IsComboMenuOpen)
				{
					this.CloseComboMenu ();
				}
			}
			else
			{
				if (this.IsComboMenuOpen)
				{
					this.UpdateComboMenuContent ();
				}
				else
				{
					this.OpenComboMenu ();
				}
			}
		}

		private static int HintWordSearching(string typed, string name)
		{
			//	Cherche si 'typed' fait partie de 'name'.
			//	Retourne -1 si ce n'est pas le cas.
			//	Retourne 0 si on est au début d'un mot.
			//	Retourne 1 si on est ailleurs.
			int i = name.IndexOf (typed);

			if (i == -1 || i == 0)  // pas trouvé ou trouvé au début ?
			{
				return i;
			}

			if (name[i-1] == ' ')  // trouvé au début d'un mot ?
			{
				return 0;
			}
			else
			{
				return 1;
			}
		}

		private string HintConvert(string text)
		{
			if (this.HintConverter == null)
			{
				return text;
			}
			else
			{
				return this.HintConverter (text);
			}
		}


		protected virtual void OnSelectedIndexChanged()
		{
			//	Génère un événement pour dire que la sélection dans la liste a changé.
			EventHandler handler = (EventHandler) this.GetUserEventHandler ("SelectedIndexChanged");
			if (handler != null)
			{
				handler (this);
			}
		}

		protected override bool AboutToGetFocus(TabNavigationDir dir, TabNavigationMode mode, out Widget focus)
		{
			// TODO: Ne fonctionne toujours pas.
			this.SelectAll ();
			return base.AboutToGetFocus (dir, mode, out focus);
		}

		protected override void OnTextChanged()
		{
			base.OnTextChanged ();

			this.HintSearching (this.Text);
		}

		protected override void OnEditionAccepted()
		{
			base.OnEditionAccepted ();

			this.Text = this.HintText;
			this.CloseComboMenu ();
		}

		protected override void OnEditionRejected()
		{
			base.OnEditionRejected ();

			this.CloseComboMenu ();
		}



		protected override bool ProcessKeyDown(Message message, Point pos)
		{
			//	Gère les pressions de touches (en particulier les flèches haut
			//	et bas qui permettent de cycler le contenu).
			switch (message.KeyCode)
			{
				case KeyCode.ArrowUp:
					this.Navigate (-1);
					return true;

				case KeyCode.ArrowDown:
					this.Navigate (1);
					return true;
			}

			return base.ProcessKeyDown (message, pos);
		}

		private void Navigate(int dir)
		{
			//	Cherche le nom suivant ou précédent dans la comboList, même si elle
			//	n'est pas "déroulée".
			if (this.hintListIndex.Count != 0)
			{
				int sel = this.hintSelected + dir;

				if (sel < 0)
				{
					sel = this.hintListIndex.Count-1;
				}

				if (sel >= this.hintListIndex.Count)
				{
					sel = 0;
				}

				this.hintSelected = sel;

				this.UpdateComboMenuSelection ();
				this.UseSelectedHint ();
			}
		}

		private void UseSelectedHint()
		{
			if (this.hintSelected >= 0 && this.hintSelected < this.hintListIndex.Count)
			{
				int i = this.hintListIndex[this.hintSelected];

				this.SelectedIndex = i;
				this.HintText = this.items[i];
				this.SetError (false);
			}
			else
			{
				this.HintText = null;
				this.SetError (!string.IsNullOrEmpty (this.Text));
			}
		}



		private bool IsComboMenuOpen
		{
			get
			{
				return this.window != null;
			}
		}

		private void OpenComboMenu()
		{
			if (this.IsComboMenuOpen)
			{
				return;
			}

			if (this.HintEditorComboMenu == Widgets.HintEditorComboMenu.Never)
			{
				return;
			}

			if (this.HintEditorComboMenu == Widgets.HintEditorComboMenu.IfReasonable &&
				this.hintListIndex.Count >= this.ComboMenuReasonableItemsLimit)
			{
				return;
			}

			this.window = new Common.Widgets.Window ();

			this.window.Owner = this.Window;
			this.window.MakeFramelessWindow ();
			this.window.MakeFloatingWindow ();
			this.window.DisableMouseActivation ();

			this.scrollList = new ScrollList ();
			this.scrollList.Parent = window.Root;
			this.scrollList.ScrollListStyle = ScrollListStyle.Menu;
			this.scrollList.Dock = DockStyle.Fill;
			this.scrollList.SelectionActivated += new EventHandler (this.HandleScrollListSelectionActivated);

			this.UpdateComboMenuContent ();

			this.window.Show ();
		}

		private void UpdateComboMenuContent()
		{
			if (!this.IsComboMenuOpen)
			{
				return;
			}

			if (this.HintEditorComboMenu == Widgets.HintEditorComboMenu.IfReasonable &&
				this.hintListIndex.Count >= this.ComboMenuReasonableItemsLimit)
			{
				this.CloseComboMenu ();
				return;
			}

			this.UpdateScrollListContent ();

			Size bestSize = this.scrollList.GetBestFitSize ();
			Size size = new Size (this.ActualWidth, bestSize.Height);

			//?Point location = this.MapClientToScreen (new Point (0, 0));
			//?location = new Point (location.X, location.Y-size.Height);

			Point location;
			HintEditor.GetMenuDisposition (this, this.scrollList, ref size, out location);

			this.window.WindowLocation = location;
			this.window.WindowSize = size;
		}

		private void UpdateComboMenuSelection()
		{
			if (!this.IsComboMenuOpen)
			{
				return;
			}

			this.ignoreChange = true;
			this.scrollList.SelectedIndex = this.hintSelected;
			this.scrollList.ShowSelected (ScrollShowMode.Center);
			this.ignoreChange = false;
		}

		private void CloseComboMenu()
		{
			// TODO: Le menu n'est pas fermé lorsqu'on déplace la fenêtre !
			if (!this.IsComboMenuOpen)
			{
				return;
			}

			this.window.Close ();
			this.window = null;

			this.scrollList.SelectionActivated -= new EventHandler (this.HandleScrollListSelectionActivated);
			this.scrollList = null;
		}


		private void UpdateScrollListContent()
		{
			this.scrollList.Items.Clear ();

			for (int index = 0; index < this.hintListIndex.Count; index++)
			{
				int i = this.hintListIndex[index];

				string name = this.items.GetName (i);
				string text = this.items[i];

				this.scrollList.Items.Add (name, text);
			}

			this.ignoreChange = true;
			this.scrollList.SelectedIndex = this.hintSelected;
			this.scrollList.ShowSelected (ScrollShowMode.Center);
			this.ignoreChange = false;

			Size size = this.scrollList.GetBestFitSizeBasedOnContent ();
			this.scrollList.RowHeight      = size.Height;
			this.scrollList.PreferredWidth = size.Width;
		}


		private static void GetMenuDisposition(Widget item, ScrollList scrollList, ref Size size, out Point location)
		{
			Point pos = Common.Widgets.Helpers.VisualTree.MapVisualToScreen (item, new Point (0, 0));
			Point hot = Common.Widgets.Helpers.VisualTree.MapVisualToScreen (item, new Point (0, 0));
			ScreenInfo screenInfo  = ScreenInfo.Find (hot);
			Rectangle workingArea = screenInfo.WorkingArea;

			double maxHeight = pos.Y - workingArea.Bottom;

			if (maxHeight > size.Height || maxHeight > 100)
			{
				//	Il y a assez de place pour dérouler le menu vers le bas,
				//	mais il faudra peut-être le raccourcir un bout :
				scrollList.MaxSize = new Size (scrollList.MaxWidth, maxHeight);
				HintEditor.AdjustSize (scrollList);

				size = scrollList.PreferredSize;
				location = new Point(pos.X, pos.Y+1);
			}
			else
			{
				//	Il faut dérouler le menu vers le haut.
				pos.Y += item.ActualHeight-1;

				maxHeight = workingArea.Top - pos.Y;

				scrollList.MaxSize = new Size (scrollList.MaxWidth, maxHeight);
				HintEditor.AdjustSize (scrollList);

				pos.Y += scrollList.PreferredHeight;

				size = scrollList.PreferredSize;
				location = pos;
			}

			location.Y -= size.Height;

			if (location.X + size.Width > workingArea.Right)
			{
				location.X = workingArea.Right - size.Width;
			}
		}

		private static void AdjustSize(ScrollList scrollList)
		{
			scrollList.PreferredSize = scrollList.GetBestFitSize ();
			Common.Widgets.Layouts.LayoutContext.SyncArrange (scrollList);
		}


		private void HandleScrollListSelectionActivated(object sender)
		{
			if (this.ignoreChange)
			{
				return;
			}

			this.hintSelected = this.scrollList.SelectedIndex;
			this.UseSelectedHint ();
			this.Text = this.HintText;
			this.SelectAll ();
			this.OnEditionAccepted ();

			this.CloseComboMenu ();
		}

		
		private readonly Common.Widgets.Collections.StringCollection items;
		private int selectedRow;

		private readonly List<int> hintListIndex;
		private int hintSelected;

		private Window window;
		private ScrollList scrollList;

		private bool ignoreChange;
	}
}
