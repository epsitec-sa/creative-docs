//	Copyright © 2006, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.Widgets
{
	/// <summary>
	/// La classe TextFieldComboMenu implémente le menu affiché par TextFieldCombo.
	/// </summary>
	public class TextFieldComboMenu : AbstractMenu
	{
		public TextFieldComboMenu()
		{
			this.InternalState |= InternalState.Focusable;
		}
		
		public TextFieldComboMenu(Widget embedder) : this()
		{
			this.SetEmbedder(embedder);
		}
		
		
		public Widget							Contents
		{
			get
			{
				return this.contents;
			}
			set
			{
				if (this.contents != value)
				{
					this.contents = value;
					
					if (this.contents != null)
					{
						this.DockPadding = Widgets.Adorners.Factory.Active.GeometryMenuShadow;
						this.contents.Dock = DockStyle.Fill;
						this.Children.Add (this.contents);
					}
				}
			}
		}
		
		public override MenuOrientation			MenuOrientation
		{
			get
			{
				return MenuOrientation.Vertical;
			}
		}
		
		
		public override Drawing.Size GetBestFitSize()
		{
			if (this.contents != null)
			{
				Drawing.Size size = this.contents.GetBestFitSize ();
				
				double dx = size.Width;
				double dy = size.Height;
				
				dx += this.DockPadding.Width;
				dy += this.DockPadding.Height;
				
				return new Drawing.Size (dx, dy);
			}
			else
			{
				return base.GetBestFitSize ();
			}
		}

		public override void AdjustSize()
		{
			if (this.contents != null)
			{
				Drawing.Size size = this.contents.GetBestFitSize ();
				
				double width  = size.Width + this.DockPadding.Width;
				double height = size.Height + this.DockPadding.Height;
				
				System.Diagnostics.Debug.WriteLine (string.Format ("AdjustSize from {0}:{1} to {2}:{3}", this.Width, this.Height, width, height));
				
				if ((this.Parent != null) &&
					(this.RootParent is WindowRoot))
				{
					this.RootParent.Size = new Drawing.Size (width, height);
				}
				else
				{
					this.Size = new Drawing.Size (width, height);
				}
			}
		}
		
		
		
		protected override bool AboutToGetFocus(Widget.TabNavigationDir dir, Widget.TabNavigationMode mode, out Widget focus)
		{
			if (this.contents != null)
			{
				return this.contents.InternalAboutToGetFocus (dir, mode, out focus);
			}
			else
			{
				return base.AboutToGetFocus (dir, mode, out focus);
			}
		}

		protected override void OnMaxSizeChanged(Types.DependencyPropertyChangedEventArgs e)
		{
			base.OnMaxSizeChanged (e);
			
			if (this.contents != null)
			{
				Drawing.Size size = (Drawing.Size) e.NewValue;
				
				double width  = size.Width - this.DockPadding.Width;
				double height = size.Height - this.DockPadding.Height;
				
				this.contents.MaxSize = size;
			}
		}
		
		
		
		private Widget							contents;
	}
}
