//	Copyright © 2012, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support;

namespace Epsitec.Common.Widgets.Behaviors
{
	/// <summary>
	/// The <c>SearchBehavior</c> class handles a button to start searching in a
	/// text field.
	/// </summary>
	public sealed class SearchBehavior
	{
		public SearchBehavior(Widget host)
		{
			this.host = host;
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
					double height = this.host.ActualHeight;

					if (this.host is TextFieldMultiEx)
					{
						//	Si le widget est un TextFieldMultiEx, les boutons Accept/Reject seront en bas à droite.
						//	Il ne faut donc pas tenir compte de la hauteur totale du widget !
						height = System.Math.Min (height, TextFieldMultiEx.SingleLineDefaultHeight);
					}

					double width = System.Math.Floor ((height - 4) * 15.0 / 17.0);
					
					return width + width - 1;
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
			this.isVisible = visible;
			
			if (this.buttonSearch != null)
			{
				this.buttonSearch.Visibility = (this.isVisible);
			}
		}

		public void SetSearchEnabled(bool enableSearch)
		{
			this.isSearchEnabled = enableSearch;

			if (this.buttonSearch != null)
			{
				this.buttonSearch.Enable = this.isSearchEnabled;
			}
		}
		
		
		public void UpdateButtonGeometry()
		{
			AbstractTextField text = this.host as AbstractTextField;
			
			if ((text != null) &&
				(this.buttonSearch != null))
			{
				Drawing.Rectangle bounds = text.GetButtonBounds ();

				this.buttonSearch.SetManualBounds (bounds);
			}
		}


		public void CreateButtons()
		{
			System.Diagnostics.Debug.Assert (this.buttonSearch == null);

			this.buttonSearch = new GlyphButton (this.host);

			this.buttonSearch.Name        = "Search";
			this.buttonSearch.GlyphShape  = GlyphShape.Search;
			this.buttonSearch.Clicked    += this.HandleButtonSearchClicked;
			this.buttonSearch.ButtonStyle = ButtonStyle.ExListRight;

			this.SetVisible (this.isVisible);
			this.SetSearchEnabled (this.isSearchEnabled);
		}


		private void HandleButtonSearchClicked(object sender, MessageEventArgs e)
		{
			System.Diagnostics.Debug.Assert (sender == this.buttonSearch);
			this.OnSearchClicked ();
		}


		private void OnSearchClicked()
		{
			this.SearchClicked.Raise (this);
		}


		public event Support.EventHandler		SearchClicked;


		private readonly Widget					host;

		private GlyphButton						buttonSearch;

		private bool							isVisible;
		private bool							isSearchEnabled = true;
	}
}
