//	Copyright © 2012, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support;
using Epsitec.Common.Support.Extensions;

using System.Collections.Generic;
using System.Linq;
using Epsitec.Common.Drawing;

namespace Epsitec.Common.Widgets.Behaviors
{
	/// <summary>
	/// The <c>SearchBehavior</c> class handles a button to start searching in a
	/// text field.
	/// </summary>
	public sealed class SearchBehavior
	{
		public SearchBehavior(ISearchBox host)
		{
			this.host = host as Widget;
			this.hostSearchBox = host;
			this.hostPolicy = this.hostSearchBox == null ? null : this.hostSearchBox.Policy;

			this.isSearchEnabled = true;
			this.isVisible       = true;
		}
		
		
		public Widget							Host
		{
			get
			{
				return this.host;
			}
		}
		
		public double							DefaultWidth
		{
			get
			{
				if (this.IsVisible)
				{
					this.CreateButtons ();
					double width = 0;

					foreach (var button in this.GetButtons ().Where (x => x.Visibility))
					{
						width += button.PreferredWidth - 1;
					}

					if (width > 0)
					{
						return width + 1;
					}
				}
				
				return 0;
			}
		}
		
		public bool								IsVisible
		{
			get
			{
				return this.isVisible;
			}
		}
		
		public bool								IsSearchEnabled
		{
			get
			{
				return this.isSearchEnabled;
			}
		}
		
		
		public void SetVisible(bool visible)
		{
			if (this.isVisible != visible)
			{
				this.isVisible = visible;
				this.UpdateButtonEnableAndVisibility ();
			}
		}

		public void SetSearchEnabled(bool enableSearch)
		{
			if (this.isSearchEnabled != enableSearch)
			{
				this.isSearchEnabled = enableSearch;
				this.UpdateButtonEnableAndVisibility ();
			}
		}
		
		public void UpdateButtonGeometry()
		{
			AbstractTextField text = this.host as AbstractTextField;
			
			if (text != null)
			{
				this.CreateButtons ();

				var sequence = new ButtonStyleSequence (ButtonStyle.ExListRight, ButtonStyle.ExListMiddle);
				var bounds = text.GetButtonBounds ();

				bounds = Rectangle.Offset (bounds, bounds.Width-1, 0);

				foreach (var button in this.GetButtons ().Where (x => x.Visibility).Reverse ())
				{
					sequence.ApplyStyle (button);
					
					bounds = new Rectangle (bounds.Left + 1 - button.PreferredWidth, bounds.Bottom, button.PreferredWidth, bounds.Height);
					
					button.SetManualBounds (bounds);
				}
			}
		}

		private void CreateButtons()
		{
			if (this.buttonSearch != null)
			{
				return;
			}

			this.buttonSearch = new GlyphButton (this.host)
			{
				Name = "Search",
				GlyphShape = GlyphShape.Search,
				PreferredWidth = 21,
			};

			this.buttonShowNext = new GlyphButton (this.host)
			{
				Name = "Next",
				GlyphShape = GlyphShape.ArrowDown,
				PreferredWidth = 15,
			};

			this.buttonShowPrev = new GlyphButton (this.host)
			{
				Name = "Prev",
				GlyphShape = GlyphShape.ArrowUp,
				PreferredWidth = 15,
			};

			this.buttonSearch.Clicked   += this.HandleButtonSearchClicked;
			this.buttonShowNext.Clicked += this.HandleButtonNextClicked;
			this.buttonShowPrev.Clicked += this.HandleButtonPrevClicked;
			
			this.UpdateButtonEnableAndVisibility ();
		}


		private void UpdateButtonEnableAndVisibility()
		{
			if (this.buttonSearch != null)
			{
				this.buttonSearch.Enable = this.isSearchEnabled;
				this.buttonSearch.Visibility = this.isVisible && this.hostPolicy.DisplaySearchButton;
			}
		}

		private IEnumerable<GlyphButton> GetButtons()
		{
			if (this.buttonShowPrev != null)
			{
				yield return this.buttonShowPrev;
			}
			if (this.buttonShowNext != null)
			{
				yield return this.buttonShowNext;
			}
			if (this.buttonSearch != null)
			{
				yield return this.buttonSearch;
			}
		}

		private sealed class ButtonStyleSequence
		{
			public ButtonStyleSequence(ButtonStyle first, ButtonStyle next)
			{
				this.current = first;
				this.next    = next;
			}

			public void ApplyStyle(GlyphButton button)
			{
				button.ButtonStyle = this.current;
				this.current = this.next;
			}

			private ButtonStyle					current;
			private readonly ButtonStyle		next;
		}



		private void HandleButtonSearchClicked(object sender, MessageEventArgs e)
		{
			System.Diagnostics.Debug.Assert (sender == this.buttonSearch);
			this.hostSearchBox.NotifySearchClicked ();
		}

		private void HandleButtonNextClicked(object sender, MessageEventArgs e)
		{
			System.Diagnostics.Debug.Assert (sender == this.buttonShowNext);
			this.hostSearchBox.NotifyShowNextClicked ();
		}

		private void HandleButtonPrevClicked(object sender, MessageEventArgs e)
		{
			System.Diagnostics.Debug.Assert (sender == this.buttonShowPrev);
			this.hostSearchBox.NotifyShowPrevClicked ();
		}



		private readonly Widget					host;
		private readonly ISearchBox				hostSearchBox;
		private readonly SearchBoxPolicy		hostPolicy;

		private GlyphButton						buttonSearch;
		private GlyphButton						buttonShowNext;
		private GlyphButton						buttonShowPrev;
		
		private bool							isVisible;
		private bool							isSearchEnabled;
	}
}
