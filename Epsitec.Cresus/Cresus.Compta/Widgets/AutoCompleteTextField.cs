//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using Epsitec.Common.Support;
using Epsitec.Common.Types;
using Epsitec.Common.Types.Converters;
using Epsitec.Common.Drawing;
using Epsitec.Common.Widgets;
using Epsitec.Common.Widgets.Helpers;

using Epsitec.Cresus.Core;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Compta.Widgets
{
	public sealed class AutoCompleteTextField : TextField
	{
		public AutoCompleteTextField()
		{
			this.MenuButtonWidth = Epsitec.Cresus.Compta.Controllers.UIBuilder.ComboButtonWidth;

			this.TextDisplayMode = TextFieldDisplayMode.ActiveHint;
			this.DefocusAction = Common.Widgets.DefocusAction.AcceptEdition;

			this.primaryTexts = new List<string> ();
			this.secondaryTexts = new List<string> ();

			this.ignoreChanges = new SafeCounter ();

			this.hintListIndex = new List<int> ();
			this.selectedHint = -1;

			this.IsFocusedChanged += this.HandleIsFocusedChanged;
		}


		public List<string> PrimaryTexts
		{
			get
			{
				return this.primaryTexts;
			}
		}

		public List<string> SecondaryTexts
		{
			get
			{
				return this.secondaryTexts;
			}
		}

		public double MenuButtonWidth
		{
			get;
			set;
		}


		protected override bool CanStartEdition
		{
			get
			{
				return true;
			}
		}
		

		private void UpdateHint(string typed)
		{
			this.UpdateHintList (typed);
			this.SelectDefaultHint ();
			this.UseSelectedHint (SelectedHintMode.Searching);
			this.UpdateComboMenuVisibility ();
		}

		private void UpdateComboMenuVisibility()
		{
			if (this.hintListIndex.Count <= 1)
			{
				//	Zero or one item in the list: no need to display a menu !
				this.CloseComboMenu ();
			}
			else
			{
				if (this.IsComboMenuOpen)
				{
					this.UpdateComboMenuContent ();
				}
				else
				{
					this.BaseOpenComboMenu ();
				}
			}
		}
		
		private void SelectDefaultHint()
		{
			if (this.hintListIndex.Count == 0)
			{
				this.selectedHint = -1;
			}
			else
			{
				this.selectedHint = 0;  // la première proposition
			}
		}
		
		private void UpdateHintList(string typed)
		{
			this.hintListIndex.Clear ();

			if (!string.IsNullOrEmpty (typed))
			{
				typed = typed.ToLower ();

				bool two = (this.primaryTexts.Count == this.secondaryTexts.Count);

				for (int i=0; i<this.primaryTexts.Count; i++)
				{
					string primary = this.primaryTexts[i].ToLower ();
					string secondary = two ? this.secondaryTexts[i].ToLower () : "";

					if (typed == primary || typed == secondary)
					{
						this.hintListIndex.Clear ();
						this.hintListIndex.Add (i);  // met une seule proposition, la bonne
						break;
					}

					if (primary.Contains (typed) || secondary.Contains (typed))
					{
						this.hintListIndex.Add (i);
					}
				}
			}
		}


		private void HandleIsFocusedChanged(object sender, DependencyPropertyChangedEventArgs e)
		{
			bool focused = (bool) e.NewValue;

			if (focused)
			{
			}
			else
			{
				this.UseSelectedHint (SelectedHintMode.AcceptEdition);
				this.CloseComboMenu ();
				base.OnTextChanged ();
			}
		}


		private void OnSelectedItemChanged()
		{
			var handler = (EventHandler) this.GetUserEventHandler (AutoCompleteTextField.SelectedItemChangedEvent);
			
			if (handler != null)
			{
				handler (this);
			}
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				this.IsFocusedChanged -= this.HandleIsFocusedChanged;

				this.CloseComboMenu ();
			}

			base.Dispose (disposing);
		}

		protected override bool AboutToGetFocus(TabNavigationDir dir, TabNavigationMode mode, out Widget focus)
		{
			// TODO: Ne fonctionne toujours pas.
			this.SelectAll ();
			return base.AboutToGetFocus (dir, mode, out focus);
		}

		protected override void OnTextChanged()
		{
			if (!this.ignoreChanges.IsZero)
			{
				return;
			}

			this.UpdateHint (this.Text);
			base.OnTextChanged ();
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

				case KeyCode.Escape:
					if (this.IsComboMenuOpen)
					{
						this.CloseComboMenu ();
					}
					break;
			}

			return base.ProcessKeyDown (message, pos);
		}

		private void Navigate(int dir)
		{
			//	Cherche le nom suivant ou précédent dans la comboList, même si elle
			//	n'est pas "déroulée".
			if (this.IsComboMenuOpen)
			{
				if (this.hintListIndex.Count != 0)
				{
					int sel = this.selectedHint + dir;

					if (sel < 0)
					{
						sel = this.hintListIndex.Count-1;
					}

					if (sel >= this.hintListIndex.Count)
					{
						sel = 0;
					}

					this.selectedHint = sel;

					this.UpdateComboMenuSelection ();
					this.UseSelectedHint (SelectedHintMode.StartEdition);
				}
			}
			else
			{
				int sel = 0;

				this.UpdateHintList (this.Text);

				if (this.hintListIndex.Count != 0)
				{
					sel = this.hintListIndex[0] + dir;
				}

				this.hintListIndex.Clear ();
				for (int i = 0; i < this.primaryTexts.Count; i++)
				{
					this.hintListIndex.Add (i);
				}

				if (sel < 0)
				{
					sel = this.hintListIndex.Count-1;
				}

				if (sel >= this.hintListIndex.Count)
				{
					sel = 0;
				}

				this.selectedHint = sel;
				this.UseSelectedHint (SelectedHintMode.StartEdition);

				this.hintListIndex.Clear ();
			}
		}


		private enum SelectedHintMode
		{
			Searching,
			StartEdition,
			AcceptEdition,
		}

		private void UseSelectedHint(SelectedHintMode selectedHintMode)
		{
			using (this.ignoreChanges.Enter ())
			{
				int i = -1;

				if (this.hintListIndex.Count != 0)
				{
					if (this.selectedHint >= 0 && this.selectedHint < this.hintListIndex.Count)
					{
						i = this.hintListIndex[this.selectedHint];
					}
				}
				else
				{
					i = this.selectedHint;
				}

				if (selectedHintMode == SelectedHintMode.AcceptEdition)
				{
					if (this.hintListIndex.Count != 0 || this.selectedHint != -1)  // pas un texte erroné ?
					{
						this.HintText = null;
						this.TextNavigator.TextLayout.Text = this.GetItemText (i, true);
						this.SelectAll ();
					}
				}
				else
				{
					this.HintText = this.AdjustHintText (this.GetItemText (i), this.Text);

					if (selectedHintMode == SelectedHintMode.StartEdition)
					{
						this.TextNavigator.TextLayout.Text = this.GetItemText (i, true);
						this.OnTextEdited ();
						this.SelectAll ();
					}
				}
			}
		}

		private string AdjustHintText(string hint, string typed)
		{
			//	Ajuste le 'HintText' en fonction des caractères réellement frappés, qui peuvent avoir
			//	une casse différente.
			if (!string.IsNullOrEmpty (hint) && !string.IsNullOrEmpty (typed))
			{
				int i = hint.ToLower ().IndexOf (typed.ToLower ());
				if (i != -1)
				{
					hint = hint.Substring (0, i) + typed + hint.Substring (i+typed.Length);
				}
			}

			return hint;
		}

		private string GetItemText(int index, bool onlyPrimary = false)
		{
			if (index < 0 || index >= this.primaryTexts.Count)
			{
				return null;
			}
			else
			{
				if (!onlyPrimary && this.primaryTexts.Count == this.secondaryTexts.Count)
				{
					return this.primaryTexts[index] + " " + this.secondaryTexts[index];
				}
				else
				{
					return this.primaryTexts[index];
				}
			}
		}


		private bool IsComboMenuOpen
		{
			get
			{
				return this.window != null;
			}
		}

		public void OpenComboMenu()
		{
			this.SelectAll ();
			this.Focus ();

			if (this.IsComboMenuOpen)
			{
				this.CloseComboMenu ();
			}
			else
			{
				this.UpdateHintListWithAllItems ();
				this.BaseOpenComboMenu ();
			}
		}

		private void BaseOpenComboMenu()
		{
			if (this.hintListIndex.Count == 0)
			{
				//	Don't open an empty menu:
				return;
			}

			this.CreateComboMenu ();
			this.UpdateComboMenuContent ();
			this.window.Show ();
		}

		private void CreateComboMenu()
		{
			this.window = new Common.Widgets.Window ();

			this.window.Owner = this.Window;
			this.window.MakeFramelessWindow ();
			this.window.MakeFloatingWindow ();
			this.window.DisableMouseActivation ();

			this.scrollList = new ScrollList ()
			{
				Parent = window.Root,
				ScrollListStyle = ScrollListStyle.Menu,
				AutomaticScrollEnable = false,
				Dock = DockStyle.Fill
			};

			this.Window.WindowPlacementChanged += this.HandleWindowPlacementChanged;
			this.scrollList.SelectionActivated += this.HandleScrollListSelectionActivated;
			Window.ApplicationDeactivated      += this.HandleApplicationDeactivated;
		}

		private void UpdateHintListWithAllItems()
		{
			this.hintListIndex.Clear ();

			for (int i = 0; i < this.primaryTexts.Count; i++)
			{
				this.hintListIndex.Add (i);
			}

			this.UpdateSelectedHint ();
		}

		private void UpdateSelectedHint()
		{
			if (this.hintListIndex.Count == 0)
			{
				this.selectedHint = -1;
			}
			else
			{
				this.selectedHint = 0;
			}
		}

		private void UpdateComboMenuContent()
		{
			if (!this.IsComboMenuOpen)
			{
				return;
			}

			this.UpdateScrollListContent ();

			Size bestSize = this.scrollList.GetBestFitSize ();
			Size size = new Size (this.ActualWidth, bestSize.Height);

			Point location;
			this.GetMenuDisposition (this.scrollList, ref size, out location);

			this.window.WindowLocation = location;
			this.window.WindowSize = size;
		}

		private void UpdateComboMenuSelection()
		{
			if (!this.IsComboMenuOpen)
			{
				return;
			}

			using (this.ignoreChanges.Enter ())
			{
				this.scrollList.SelectedItemIndex = this.selectedHint;
				this.scrollList.ShowSelected (ScrollShowMode.Center);
			}
		}

		private void CloseComboMenu()
		{
			if (!this.IsComboMenuOpen)
			{
				return;
			}

			this.scrollList.SelectionActivated -= this.HandleScrollListSelectionActivated;
			this.window.WindowPlacementChanged -= this.HandleWindowPlacementChanged;
			Window.ApplicationDeactivated      -= this.HandleApplicationDeactivated;

			this.window.Close ();
			this.window = null;

			this.scrollList = null;
		}


		private void UpdateScrollListContent()
		{
			this.scrollList.Items.Clear ();

			for (int index = 0; index < this.hintListIndex.Count; index++)
			{
				int i = this.hintListIndex[index];
				this.scrollList.Items.Add (this.GetItemText (i));
			}

			using (this.ignoreChanges.Enter ())
			{
				this.scrollList.SelectedItemIndex = this.selectedHint;
				this.scrollList.ShowSelected (ScrollShowMode.Center);
			}

			Size size = this.scrollList.GetBestFitSizeBasedOnContent ();
			this.scrollList.RowHeight      = size.Height;
			this.scrollList.PreferredWidth = size.Width;
		}


		private void GetMenuDisposition(ScrollList scrollList, ref Size size, out Point location)
		{
			Point pos = Common.Widgets.Helpers.VisualTree.MapVisualToScreen (this, new Point (0, 0));
			Point hot = Common.Widgets.Helpers.VisualTree.MapVisualToScreen (this, new Point (0, 0));
			ScreenInfo screenInfo  = ScreenInfo.Find (hot);
			Rectangle workingArea = screenInfo.WorkingArea;

			double maxHeight = pos.Y - workingArea.Bottom;

			if (maxHeight > size.Height || maxHeight > 100)
			{
				//	Il y a assez de place pour dérouler le menu vers le bas,
				//	mais il faudra peut-être le raccourcir un bout :
				scrollList.MaxSize = new Size (scrollList.MaxWidth, maxHeight);
				AutoCompleteTextField.AdjustSize (scrollList);

				size.Height = scrollList.PreferredHeight;
				size.Width  = System.Math.Max (size.Width+this.MenuButtonWidth, scrollList.PreferredWidth);

				location = new Point(pos.X, pos.Y+1);
			}
			else
			{
				//	Il faut dérouler le menu vers le haut.
				pos.Y += this.ActualHeight-1;

				maxHeight = workingArea.Top - pos.Y;

				scrollList.MaxSize = new Size (scrollList.MaxWidth, maxHeight);
				AutoCompleteTextField.AdjustSize (scrollList);

				pos.Y += scrollList.PreferredHeight;

				size.Height = scrollList.PreferredHeight;
				size.Width  = System.Math.Max (size.Width+this.MenuButtonWidth, scrollList.PreferredWidth);

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


		private void HandleWindowPlacementChanged(object sender)
		{
			this.CloseComboMenu ();
		}

		private void HandleApplicationDeactivated(object sender)
		{
			this.CloseComboMenu ();
		}

		private void HandleScrollListSelectionActivated(object sender)
		{
			if (!this.ignoreChanges.IsZero)
			{
				return;
			}

			this.selectedHint = this.scrollList.SelectedItemIndex;
			this.UseSelectedHint (SelectedHintMode.AcceptEdition);
			this.CloseComboMenu ();
			base.OnTextChanged ();
		}

		
		private const string SelectedItemChangedEvent = "SelectedItemChanged";


		private readonly List<string>		primaryTexts;
		private readonly List<string>		secondaryTexts;
		private readonly List<int>			hintListIndex;
		private readonly SafeCounter		ignoreChanges;
		
		private int							selectedHint;
		private Window						window;
		private ScrollList					scrollList;
	}
}
