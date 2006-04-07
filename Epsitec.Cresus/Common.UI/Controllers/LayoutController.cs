//	Copyright © 2004, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

using Epsitec.Common.Widgets;
using Epsitec.Common.Support;

namespace Epsitec.Common.UI.Controllers
{
	using GroupController = Epsitec.Common.Widgets.Helpers.GroupController;
	
	/// <summary>
	/// Summary description for LayoutController.
	/// </summary>
	public class LayoutController : AbstractController
	{
		public LayoutController()
		{
		}
		
		
		public override void CreateUI(Widget panel)
		{
			this.caption_label = new StaticText (panel);
			this.radio_manual  = new RadioButton (panel);
			this.radio_anchor  = new RadioButton (panel);
			this.radio_dock    = new RadioButton (panel);
			this.edge_select   = new EdgeSelectWidget (panel, this);
			
			panel.Height = System.Math.Max (panel.Height, 80 + 6);
			
			this.caption_label.Width         = 80;
			this.caption_label.Anchor        = AnchorStyles.TopLeft;
			this.caption_label.Margins = new Drawing.Margins (0, 0, 8, 0);
			
			this.radio_manual.Text           = "manual";
			this.radio_manual.Width          = this.caption_label.Width;
			this.radio_manual.Anchor         = this.caption_label.Anchor;
			this.radio_manual.Margins  = new Drawing.Margins (0, 0, 8+20, 0);
			this.radio_manual.Group          = "mode";
			this.radio_manual.Index          = (int) ShowMode.Manual;
			this.radio_manual.TabIndex       = 10;
			this.radio_manual.ActiveStateChanged += new EventHandler (this.HandleRadioActiveStateChanged);
			this.radio_manual.Name           = "Value";
			
			this.radio_anchor.Text           = "anchor";
			this.radio_anchor.Width          = this.caption_label.Width;
			this.radio_anchor.Anchor         = this.caption_label.Anchor;
			this.radio_anchor.Margins  = new Drawing.Margins (0, 0, 8+20+16, 0);
			this.radio_anchor.Group          = "mode";
			this.radio_anchor.Index          = (int) ShowMode.Anchor;
			this.radio_anchor.TabIndex       = 10;
			this.radio_anchor.ActiveStateChanged += new EventHandler (this.HandleRadioActiveStateChanged);
			this.radio_anchor.Name           = "Value";
			
			this.radio_dock.Text             = "dock";
			this.radio_dock.Width            = this.caption_label.Width;
			this.radio_dock.Anchor           = this.caption_label.Anchor;
			this.radio_dock.Margins    = new Drawing.Margins (0, 0, 8+20+16*2, 0);
			this.radio_dock.Group            = "mode";
			this.radio_dock.Index            = (int) ShowMode.Dock;
			this.radio_dock.TabIndex         = 10;
			this.radio_dock.ActiveStateChanged += new EventHandler (this.HandleRadioActiveStateChanged);
			this.radio_dock.Name             = "Value";
			
			this.edge_select.Anchor          = AnchorStyles.All;
			this.edge_select.Margins   = new Drawing.Margins (80, 0, 4, 4);
			
			this.OnCaptionChanged ();
			
			this.SyncFromAdapter (SyncReason.Initialisation);
		}
		
		public override void SyncFromAdapter(SyncReason reason)
		{
#if false			
			Adapters.LayoutAdapter adapter = this.Adapter as Adapters.LayoutAdapter;
			if (adapter != null)
			{
				if (this.layout != adapter.Value)
				{
					this.layout = adapter.Value;
					
					if (this.edge_select != null)
					{
						this.edge_select.Invalidate ();
					}
					
					switch (this.layout)
					{
						case LayoutStyles.Manual:
							this.SetShowMode (ShowMode.Manual);
							break;
						
						case LayoutStyles.DockLeft:
						case LayoutStyles.DockRight:
						case LayoutStyles.DockBottom:
						case LayoutStyles.DockTop:
							this.SetShowMode (ShowMode.Dock);
							break;
						
						default:
							this.SetShowMode (ShowMode.Anchor);
							break;
					}
				}
			}
#endif
		}
		
		public override void SyncFromUI()
		{
#if false
			Adapters.LayoutAdapter adapter = this.Adapter as Adapters.LayoutAdapter;
			
			if (adapter != null)
			{
				adapter.Value = this.layout;
			}
#endif
		}
		
		
		protected void SetShowMode(ShowMode mode)
		{
			if (this.show_mode != mode)
			{
				this.show_mode = mode;
				
				if ((this.edge_select != null) &&
					(this.radio_manual != null))
				{
					GroupController controller = GroupController.GetGroupController (this.radio_manual);
					
					controller.ActiveIndex = (int) this.show_mode;
					
					this.edge_select.Invalidate ();
				}
				
				if (this.show_mode == ShowMode.Manual)
				{
//@					this.layout = LayoutStyles.Manual;
					this.OnUIDataChanged ();
				}
			}
		}
		
		
		private void HandleRadioActiveStateChanged(object sender)
		{
			RadioButton radio = sender as RadioButton;
			
			if ((radio.ActiveState == ActiveState.Yes) &&
				(radio.Group == "mode"))
			{
				this.SetShowMode ((ShowMode) radio.Index);
			}
		}
		
		
		protected enum ShowMode
		{
			Manual, Anchor, Dock
		}
		
		[SuppressBundleSupport]
		internal class EdgeSelectWidget : Widgets.PropPane.FatWidget
		{
			public EdgeSelectWidget(Widget embedder, LayoutController host) : base (embedder)
			{
				this.host = host;
			}
			
			
			protected override void ProcessMessage(Message message, Epsitec.Common.Drawing.Point pos)
			{
				if (message.IsMouseType)
				{
					Drawing.Rectangle hilite = this.hilite;
					
					if (this.host.show_mode == ShowMode.Dock)
					{
						double dx = this.Client.Size.Width;
						double dy = this.Client.Size.Height;
						double dd = 25;
						
						switch (message.Type)
						{
							case MessageType.MouseLeave:
								hilite = Drawing.Rectangle.Empty;
								break;
							
							case MessageType.MouseMove:
								if (! this.Client.Bounds.Contains (pos))
								{
									hilite = Drawing.Rectangle.Empty;
								}
								else if (pos.X < dd)
								{
									if (pos.Y < pos.X)
									{
										hilite = new Drawing.Rectangle (0, 0, dx, dd);
//@										this.candidate = LayoutStyles.DockBottom;
									}
									else if ((dy - pos.Y) < pos.X)
									{
										hilite = new Drawing.Rectangle (0, dy-dd, dx, dd);
//@										this.candidate = LayoutStyles.DockTop;
									}
									else
									{
										hilite = new Drawing.Rectangle (0, 0, dd, dy);
//@										this.candidate = LayoutStyles.DockLeft;
									}
								}
								else if ((dx - pos.X) < dd)
								{
									if ((dy - pos.Y) < (dx - pos.X))
									{
										hilite = new Drawing.Rectangle (0, dy-dd, dx, dd);
//@										this.candidate = LayoutStyles.DockTop;
									}
									else if (pos.Y < (dx - pos.X))
									{
										hilite = new Drawing.Rectangle (0, 0, dx, dd);
//@										this.candidate = LayoutStyles.DockBottom;
									}
									else
									{
										hilite = new Drawing.Rectangle (dx-dd, 0, dd, dy);
//@										this.candidate = LayoutStyles.DockRight;
									}
								}
								else if (pos.Y < dd)
								{
									hilite = new Drawing.Rectangle (0, 0, dx, dd);
//@									this.candidate = LayoutStyles.DockBottom;
								}
								else if ((dy - pos.Y) < dd)
								{
									hilite = new Drawing.Rectangle (0, dy-dd, dx, dd);
//@									this.candidate = LayoutStyles.DockTop;
								}
								else
								{
									hilite = new Drawing.Rectangle (0, 0, dx, dy);
//@									this.candidate = LayoutStyles.DockFill;
								}
								break;
						}
					}
					else if (this.host.show_mode == ShowMode.Anchor)
					{
						double dx = this.Client.Size.Width;
						double dy = this.Client.Size.Height;
						double dd = 25;
						
						switch (message.Type)
						{
							case MessageType.MouseLeave:
								hilite = Drawing.Rectangle.Empty;
								break;
							
							case MessageType.MouseMove:
								if (! this.Client.Bounds.Contains (pos))
								{
									hilite = Drawing.Rectangle.Empty;
								}
								else if ((pos.X < dd) &&
									/**/ (pos.Y > dy/2-5) &&
									/**/ (pos.Y < dy/2+5))
								{
									hilite = new Drawing.Rectangle (0, 0, dd, dy);
//@									this.candidate = LayoutStyles.AnchorLeft;
								}
								else if (((dx - pos.X) < dd) &&
									/**/ (pos.Y > dy/2-5) &&
									/**/ (pos.Y < dy/2+5))
								{
									hilite = new Drawing.Rectangle (dx-dd, 0, dd, dy);
//@									this.candidate = LayoutStyles.AnchorRight;
								}
								else if ((pos.Y < dd) &&
									/**/ (pos.X > dx/2-5) &&
									/**/ (pos.X < dx/2+5))
								{
									hilite = new Drawing.Rectangle (0, 0, dx, dd);
//@									this.candidate = LayoutStyles.AnchorBottom;
								}
								else if (((dy - pos.Y) < dd) &&
									/**/ (pos.X > dx/2-5) &&
									/**/ (pos.X < dx/2+5))
								{
									hilite = new Drawing.Rectangle (0, dy-dd, dx, dd);
//@									this.candidate = LayoutStyles.AnchorTop;
								}
								else
								{
									hilite = Drawing.Rectangle.Empty;
								}
								break;
						}
					}
					else
					{
						this.hilite = Drawing.Rectangle.Empty;
					}
					
					if ((message.Type == MessageType.MouseDown) &&
						(message.IsLeftButton) &&
						(this.hilite.IsValid))
					{
						if (this.host.show_mode == ShowMode.Dock)
						{
#if false
							if (this.host.layout == this.candidate)
							{
//@								this.host.layout = LayoutStyles.Manual;
							}
							else
							{
								this.host.layout = this.candidate;
							}
#endif
						}
						else if (this.host.show_mode == ShowMode.Anchor)
						{
//@							this.host.layout &= LayoutStyles.MaskAnchor;
//@							this.host.layout ^= this.candidate;
						}
						
						this.host.SyncFromUI ();
						this.Invalidate ();
					}
					
					if (this.hilite != hilite)
					{
						this.hilite = hilite;
						this.Invalidate ();
					}
					
					message.Consumer = this;
					return;
				}
				
				base.ProcessMessage (message, pos);
			}

			
			protected override void PaintBackgroundImplementation(Drawing.Graphics graphics, Drawing.Rectangle clip_rect)
			{
				IAdorner adorner = Epsitec.Common.Widgets.Adorners.Factory.Active;
				
				Drawing.Rectangle rect_1 = this.Client.Bounds;
				Drawing.Rectangle rect_2 = this.Client.Bounds;
				
				rect_1.Deflate ( 0.5,  0.5,  0.5,  0.5);
				rect_2.Deflate (25.5, 25.5, 20.5, 20.5);
				
				double cx = System.Math.Floor (this.Client.Size.Width / 2) + 0.5;
				double cy = System.Math.Floor (this.Client.Size.Height / 2) + 0.5;
				
				Drawing.Path surface = new Drawing.Path ();
				Drawing.Path outline = new Drawing.Path ();
				Drawing.Path picture = new Drawing.Path ();
				
				surface.AppendRectangle (rect_1);
				
				graphics.Rasterizer.AddSurface (surface);
				graphics.RenderSolid (Drawing.Color.FromAlphaRgb (0.8, 1, 1, 1));
				
				surface.Clear ();
				
				outline.AppendRectangle (rect_1);
				
				switch (this.host.show_mode)
				{
					case ShowMode.Manual:
						break;
					
					case ShowMode.Anchor:
						outline.AppendRectangle (rect_2);
						
						this.AppendLink (rect_1.Left,  cy,  25, picture, Direction.Left);
						this.AppendLink (rect_1.Right, cy,  25, picture, Direction.Right);
						this.AppendLink (cx, rect_1.Top,    20, picture, Direction.Up);
						this.AppendLink (cx, rect_1.Bottom, 20, picture, Direction.Down);
						
//						if ((this.host.layout & LayoutStyles.AnchorLeft) != 0)		this.AppendLink (rect_1.Left,  cy,  25, surface, Direction.Left);
//						if ((this.host.layout & LayoutStyles.AnchorRight) != 0)		this.AppendLink (rect_1.Right, cy,  25, surface, Direction.Right);
//						if ((this.host.layout & LayoutStyles.AnchorTop) != 0)		this.AppendLink (cx, rect_1.Top,    20, surface, Direction.Up);
//						if ((this.host.layout & LayoutStyles.AnchorBottom) != 0)	this.AppendLink (cx, rect_1.Bottom, 20, surface, Direction.Down);
						
						break;
					
					case ShowMode.Dock:
						this.AppendArrow (rect_1.Left  + 1, cy, 16, picture, Direction.Left);
						this.AppendArrow (rect_1.Right - 1, cy, 16, picture, Direction.Right);
						this.AppendArrow (cx, rect_1.Top    - 1, 16, picture, Direction.Up);
						this.AppendArrow (cx, rect_1.Bottom + 1, 16, picture, Direction.Down);
						
//						switch (this.host.layout)
//						{
//							case LayoutStyles.DockLeft:		this.AppendArrow (rect_1.Left  + 1, cy, 16, surface, Direction.Left);	break;
//							case LayoutStyles.DockRight:	this.AppendArrow (rect_1.Right - 1, cy, 16, surface, Direction.Right);	break;
//							case LayoutStyles.DockTop:		this.AppendArrow (cx, rect_1.Top    - 1, 16, surface, Direction.Up);	break;
//							case LayoutStyles.DockBottom:	this.AppendArrow (cx, rect_1.Bottom + 1, 16, surface, Direction.Down);	break;
//							
//							case LayoutStyles.DockFill:
//								this.AppendArrow (rect_1.Left  + 1, cy, 16, surface, Direction.Left);
//								this.AppendArrow (rect_1.Right - 1, cy, 16, surface, Direction.Right);
//								this.AppendArrow (cx, rect_1.Top    - 1, 16, surface, Direction.Up);
//								this.AppendArrow (cx, rect_1.Bottom + 1, 16, surface, Direction.Down);
//								break;
//						}
						
						break;
				}
				
				if (outline.IsValid)
				{
					graphics.Rasterizer.AddOutline (outline, 1.0, Drawing.CapStyle.Square, Drawing.JoinStyle.Miter);
					graphics.RenderSolid (adorner.ColorBorder);
				}
				
				if (surface.IsValid)
				{
					graphics.Rasterizer.AddSurface (surface);
					graphics.RenderSolid (Drawing.Color.FromRgb (0, 0, 0.8));
				}
				
				if ((this.hilite.IsValid) &&
					(picture.IsValid))
				{
					Drawing.Rectangle clip = graphics.SaveClippingRectangle ();
					graphics.SetClippingRectangle (this.MapClientToRoot (this.hilite));
					graphics.Rasterizer.AddSurface (picture);
					graphics.RenderSolid (Drawing.Color.FromAlphaRgb (0.5, 0, 0, 1));
					graphics.RestoreClippingRectangle (clip);
				}
				
				if (picture.IsValid)
				{
					graphics.Rasterizer.AddOutline (picture, 1.0, Drawing.CapStyle.Square, Drawing.JoinStyle.Miter);
					graphics.RenderSolid (adorner.ColorBorder);
				}
				
				surface.Dispose ();
				outline.Dispose ();
				picture.Dispose ();
			}
			
			
			protected void AppendArrow(double x, double y, double s, Drawing.Path path, Direction direction)
			{
				switch (direction)
				{
					case Direction.Up:
						path.MoveTo (x, y);
						path.LineTo (x-0.50*s, y-0.50*s);
						path.LineTo (x-0.25*s, y-0.50*s);
						path.LineTo (x-0.25*s, y-1.00*s);
						path.LineTo (x+0.25*s, y-1.00*s);
						path.LineTo (x+0.25*s, y-0.50*s);
						path.LineTo (x+0.50*s, y-0.50*s);
						path.LineTo (x, y);
						path.Close ();
						break;
					
					case Direction.Down:
						path.MoveTo (x, y);
						path.LineTo (x-0.50*s, y+0.50*s);
						path.LineTo (x-0.25*s, y+0.50*s);
						path.LineTo (x-0.25*s, y+1.00*s);
						path.LineTo (x+0.25*s, y+1.00*s);
						path.LineTo (x+0.25*s, y+0.50*s);
						path.LineTo (x+0.50*s, y+0.50*s);
						path.LineTo (x, y);
						path.Close ();
						break;
					
					case Direction.Left:
						path.MoveTo (x, y);
						path.LineTo (x+0.50*s, y-0.50*s);
						path.LineTo (x+0.50*s, y-0.25*s);
						path.LineTo (x+1.00*s, y-0.25*s);
						path.LineTo (x+1.00*s, y+0.25*s);
						path.LineTo (x+0.50*s, y+0.25*s);
						path.LineTo (x+0.50*s, y+0.50*s);
						path.LineTo (x, y);
						path.Close ();
						break;
					
					case Direction.Right:
						path.MoveTo (x, y);
						path.LineTo (x-0.50*s, y-0.50*s);
						path.LineTo (x-0.50*s, y-0.25*s);
						path.LineTo (x-1.00*s, y-0.25*s);
						path.LineTo (x-1.00*s, y+0.25*s);
						path.LineTo (x-0.50*s, y+0.25*s);
						path.LineTo (x-0.50*s, y+0.50*s);
						path.LineTo (x, y);
						path.Close ();
						break;
				}
			}
			
			protected void AppendLink(double x, double y, double s, Drawing.Path path, Direction direction)
			{
				switch (direction)
				{
					case Direction.Up:
						path.AppendCircle (x, y, 3);
						path.MoveTo (x, y-3);
						path.LineTo (x, y-s+3);
						path.AppendCircle (x, y-s, 3);
						break;
					
					case Direction.Down:
						path.AppendCircle (x, y, 3);
						path.MoveTo (x, y+3);
						path.LineTo (x, y+s-3);
						path.AppendCircle (x, y+s, 3);
						break;
					
					case Direction.Left:
						path.AppendCircle (x, y, 3);
						path.MoveTo (x+3, y);
						path.LineTo (x+s-3, y);
						path.AppendCircle (x+s, y, 3);
						break;
					
					case Direction.Right:
						path.AppendCircle (x, y, 3);
						path.MoveTo (x-3, y);
						path.LineTo (x-s+3, y);
						path.AppendCircle (x-s, y, 3);
						break;
				}
			}
			
			
			private Drawing.Rectangle			hilite;
			private LayoutController			host;
//@			private LayoutStyles				candidate;
		}
		
		
		private RadioButton						radio_manual;
		private RadioButton						radio_anchor;
		private RadioButton						radio_dock;
		
		private EdgeSelectWidget				edge_select;
		
//@		private LayoutStyles					layout;
		private ShowMode						show_mode;
	}
}
