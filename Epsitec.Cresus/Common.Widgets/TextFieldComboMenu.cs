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

			this.AddEventHandler (Visual.MaxWidthProperty, this.HandleMaxWidthChanged);
			this.AddEventHandler (Visual.MaxHeightProperty, this.HandleMaxHeightChanged);
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
						this.Padding = Widgets.Adorners.Factory.Active.GeometryMenuShadow;
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
				
				dx += this.Padding.Width;
				dy += this.Padding.Height;
				
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
				
				double width  = size.Width + this.Padding.Width;
				double height = size.Height + this.Padding.Height;

				width = System.Math.Max (width, this.MinWidth);
				height = System.Math.Max (height, this.MinHeight);
				
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


		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				this.RemoveEventHandler (Visual.MaxWidthProperty, this.HandleMaxWidthChanged);
				this.RemoveEventHandler (Visual.MaxHeightProperty, this.HandleMaxHeightChanged);
			}
			
			base.Dispose (disposing);
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

		private void HandleMaxWidthChanged(object sender, Types.DependencyPropertyChangedEventArgs e)
		{
			if (this.contents != null)
			{
				double width;
				
				width  = (double) e.NewValue;
				width -= this.Padding.Width;

				this.contents.MaxWidth = width;
			}
		}

		private void HandleMaxHeightChanged(object sender, Types.DependencyPropertyChangedEventArgs e)
		{
			if (this.contents != null)
			{
				double height;

				height  = (double) e.NewValue;
				height -= this.Padding.Height;

				this.contents.MaxHeight = height;
			}
		}
		
		
		private Widget							contents;
	}
}
