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
		
		
		public ScrollList						ScrollList
		{
			get
			{
				if (this.list == null)
				{
					this.CreateScrollList ();
				}
				
				return this.list;
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
			Drawing.Size size = this.ScrollList.GetBestFitSize ();
			
			double dx = size.Width;
			double dy = size.Height;
			
			dx += this.DockPadding.Width;
			dy += this.DockPadding.Height;
			
			return new Drawing.Size (dx, dy);
		}

		public override void AdjustSize()
		{
			Drawing.Size size = this.ScrollList.GetBestFitSize ();
			
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
		
		
		protected override bool AboutToGetFocus(Widget.TabNavigationDir dir, Widget.TabNavigationMode mode, out Widget focus)
		{
			if (this.list != null)
			{
				return this.list.InternalAboutToGetFocus (dir, mode, out focus);
			}
			else
			{
				return base.AboutToGetFocus (dir, mode, out focus);
			}
		}

		protected override void OnMaxSizeChanged(Types.PropertyChangedEventArgs e)
		{
			base.OnMaxSizeChanged (e);
			
			if (this.list != null)
			{
				Drawing.Size size = (Drawing.Size) e.NewValue;
				
				double width  = size.Width - this.DockPadding.Width;
				double height = size.Height - this.DockPadding.Height;
				
				this.list.MaxSize = size;
			}
		}
		
		protected virtual void CreateScrollList()
		{
			this.list = new ScrollList (this);
			this.list.ScrollListStyle = ScrollListStyle.Menu;
			this.list.Dock = DockStyle.Fill;
			
			IAdorner adorner = Widgets.Adorners.Factory.Active;
			
			this.DockPadding = adorner.GeometryMenuShadow;
		}
		
		
		private ScrollList						list;
	}
}
