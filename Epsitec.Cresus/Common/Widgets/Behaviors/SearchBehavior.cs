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

			this.UpdateButtonEnableAndVisibility ();
		}


		private void UpdateButtonEnableAndVisibility()
		{
			if (this.buttonSearch != null)
			{
				this.buttonSearch.Enable = this.isSearchEnabled;
				this.buttonSearch.Visibility = this.isVisible;
			}
		}

		private void HandleButtonSearchClicked(object sender, MessageEventArgs e)
		{
			System.Diagnostics.Debug.Assert (sender == this.buttonSearch);
			this.hostSearchBox.NotifySearchClicked ();
		}



		private readonly Widget					host;
		private readonly ISearchBox				hostSearchBox;
		private readonly SearchBoxPolicy		hostPolicy;

		private GlyphButton						buttonSearch;
		private bool							isVisible;
		private bool							isSearchEnabled;
	}
}
