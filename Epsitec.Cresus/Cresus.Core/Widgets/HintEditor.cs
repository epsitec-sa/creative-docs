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

	public enum HintComparerResult
	{
		NoMatch,
		SecondaryMatch,
		PrimaryMatch,	// le "meilleur" en dernier
	}


	public class HintEditor : TextFieldEx, Common.Widgets.Collections.IStringCollectionHost, Common.Support.Data.IKeyedStringSelection
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

			this.hintWordSeparators = new List<string> ();
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


		/// <summary>
		/// Méthode de conversion d'un objet stocké dans Items.Value en une chaîne à afficher.
		/// </summary>
		/// <value>The value converter.</value>
		public System.Func<object, string> ValueToDescriptionConverter
		{
			get;
			set;
		}

		/// <summary>
		/// Méthode de comparaison d'un objet stocké dans Items.Value avec une chaîne partielle entrée par l'utilisateur.
		/// </summary>
		/// <value>The hint comparer.</value>
		public System.Func<object, string, HintComparerResult> HintComparer
		{
			get;
			set;
		}


		public static HintComparerResult Compare(string text, string typed)
		{
			int index = text.IndexOf (typed);

			if (index == -1)
			{
				return HintComparerResult.NoMatch;
			}
			else if (index == 0)
			{
				return HintComparerResult.PrimaryMatch;
			}
			else
			{
				return HintComparerResult.SecondaryMatch;
			}
		}

		public static HintComparerResult Bestof(HintComparerResult result1, HintComparerResult result2)
		{
			if (result1 > result2)
			{
				return result1;
			}
			else
			{
				return result2;
			}
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

		#region IKeyedStringSelection Members

		public string SelectedKey
		{
			get
			{
				if (this.selectedRow == -1)
				{
					return null;
				}

				return this.items.GetKey (this.selectedRow);
			}
			set
			{
				if (this.SelectedKey != value)
				{
					int index = -1;

					if (value != null)
					{
						index = this.items.FindIndexByKey (value);

						if (index < 0)
						{
							throw new System.ArgumentException (string.Format ("No element named '{0}' in list", value));
						}
					}

					this.SelectedItemIndex = index;
				}
			}
		}

		#endregion

		#region IStringSelection Members

		public int SelectedItemIndex
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

					this.ignoreChange = true;
					this.Text = this.GetItemText (this.selectedRow);
					this.ignoreChange = false;

					this.OnSelectedItemChanged ();
				}
			}
		}

		public string SelectedItem
		{
			get
			{
				int index = this.SelectedItemIndex;
				
				if (index < 0)
				{
					return null;
				}

				return this.Items[index];
			}
			set
			{
				this.SelectedItemIndex = this.Items.IndexOf (value);
			}
		}

		public event EventHandler SelectedItemChanged
		{
			add
			{
				this.AddUserEventHandler(HintEditor.SelectedItemChangedEvent, value);
			}
			remove
			{
				this.RemoveUserEventHandler (HintEditor.SelectedItemChangedEvent, value);
			}
		}

		#endregion


		private void HintSearching(string typed)
		{
			if (this.ValueToDescriptionConverter == null || this.HintComparer == null)
			{
				return;
			}

			this.hintListIndex.Clear ();

			string original = typed;
			if (this.HintComparisonConverter != null)
			{
				typed = this.HintComparisonConverter (typed);
			}

			if (!string.IsNullOrEmpty (typed))
			{
				List<int> list1 = new List<int> ();
				List<int> list2 = new List<int> ();

				for (int i=0; i<this.items.Count; i++)
				{
					var value = this.items.GetValue (i);

					string full = this.ValueToDescriptionConverter (value);
					if (full == original)  // trouvé exactement ?
					{
						list1.Clear ();
						list2.Clear ();

						list1.Add (i);  // met une seule proposition, la bonne
						break;
					}

					var result = this.HintComparer (value, typed);

					if (result == HintComparerResult.PrimaryMatch)
					{
						list1.Add (i);
					}

					if (result == HintComparerResult.SecondaryMatch)
					{
						list2.Add (i);
					}
				}

				this.hintListIndex.AddRange (list1);
				this.hintListIndex.AddRange (list2);
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


		protected virtual void OnSelectedItemChanged()
		{
			//	Génère un événement pour dire que la sélection dans la liste a changé.
			EventHandler handler = (EventHandler) this.GetUserEventHandler (HintEditor.SelectedItemChangedEvent);
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
			if (this.ignoreChange)
			{
				return;
			}

			this.HintSearching (this.Text);
			base.OnTextChanged ();
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

				this.SetSilentSelectedItemIndex (i);  // (*)
				this.HintText = this.GetItemText (i);
				this.SetError (false);
			}
			else
			{
				this.SetSilentSelectedItemIndex (-1);  // (*)
				this.HintText = null;
				this.SetError (!string.IsNullOrEmpty (this.Text));
			}
		}

		// (*)	Il ne faut surtout pas utiliser SelectedItemIndex = i, car il ne faut surtout
		//		pas modifier this.Text !

		private void SetSilentSelectedItemIndex(int index)
		{
			if (this.selectedRow != index)
			{
				this.selectedRow = index;
				this.OnSelectedItemChanged ();
			}
		}

		private string GetItemText(int index)
		{
			if (this.ValueToDescriptionConverter == null)
			{
				return null;
			}
			else
			{
				object value = this.items.GetValue (index);
				return this.ValueToDescriptionConverter (value);
			}
		}


		#region Combo menu
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
			this.scrollList.SelectedItemIndex = this.hintSelected;
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

				string key = this.items.GetKey (i);
				string text = this.GetItemText (i);

				this.scrollList.Items.Add (key, text);
			}

			this.ignoreChange = true;
			this.scrollList.SelectedItemIndex = this.hintSelected;
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

			this.hintSelected = this.scrollList.SelectedItemIndex;
			this.UseSelectedHint ();
			this.Text = this.HintText;
			this.SelectAll ();
			this.OnEditionAccepted ();

			this.CloseComboMenu ();
		}
		#endregion

		private const string SelectedItemChangedEvent = "SelectedItemChanged";


		private readonly Common.Widgets.Collections.StringCollection items;
		private int selectedRow;

		private readonly List<int> hintListIndex;
		private int hintSelected;

		private List<string> hintWordSeparators;

		private Window window;
		private ScrollList scrollList;

		private bool ignoreChange;
	}
}
