namespace Epsitec.Common.Widgets
{
	/// <summary>
	/// Liste les types de poignées connues.
	/// </summary>
	public enum GripType
	{
		None,
		Vertex,								//	coin (poignée standard)
		Edge,								//	côté
		Center								//	centre de gravité
	}
	
	/// <summary>
	/// La classe Grip implémente une poignée avec à la fois le support pour son
	/// dessin et une aide pour le dragging de celle-ci (génère un DragEvent qui
	/// décrit le déplacement désiré).
	/// </summary>
	public class Grip : Widget, Helpers.IDragBehaviorHost
	{
		public Grip()
		{
			this.grip_type  = GripType.None;
			this.grip_color = Design.HiliteAdorner.FrameColor;
			
			this.drag_behavior = new Helpers.DragBehavior (this);
			
			//	Une poignée ne peut jamais obtenir le focus clavier.
			
			this.InternalState &= ~ InternalState.Focusable;
		}
		
		public Grip(Widget embedder) : this()
		{
			this.SetEmbedder(embedder);
		}
		
		
		public GripType						GripType
		{
			get { return this.grip_type; }
			set
			{
				if (this.grip_type != value)
				{
					Drawing.Point offset = this.GripOffset;
					
					this.grip_type = value;
					
					this.Bounds = new Drawing.Rectangle (this.Location + offset - this.GripOffset, this.GripSize);
				}
			}
		}
		
		public Drawing.Point				GripLocation
		{
			get { return this.Location + this.GripOffset; }
			set { this.Location = value - this.GripOffset; }
		}
		
		public Drawing.Point				GripOffset
		{
			get
			{
				switch (this.grip_type)
				{
					case GripType.None:		return new Drawing.Point (0, 0);
					case GripType.Vertex:	return new Drawing.Point (3, 3);
					case GripType.Edge:		return new Drawing.Point (3, 3);
					case GripType.Center:	return new Drawing.Point (3, 3);
				}
				
				return Drawing.Point.Empty;
			}
		}
		
		public Drawing.Size					GripSize
		{
			get
			{
				switch (this.grip_type)
				{
					case GripType.None:		return new Drawing.Size (0, 0);
					case GripType.Vertex:	return new Drawing.Size (6, 6);
					case GripType.Edge:		return new Drawing.Size (6, 6);
					case GripType.Center:	return new Drawing.Size (6, 6);
				}
				
				return Drawing.Size.Empty;
			}
		}
		
		
		public Drawing.Point				DragLocation
		{
			get { return this.GripLocation; }
		}
		
		
		protected override void PaintBackgroundImplementation(Drawing.Graphics graphics, Drawing.Rectangle clip_rect)
		{
			switch (this.grip_type)
			{
				case GripType.Vertex:
				case GripType.Edge:
				case GripType.Center:
					using (Drawing.Path path = new Drawing.Path ())
					{
						this.DefineGradientShape (graphics);
						this.DefineGradientOffset (graphics, 2, 4, 3);
						
						path.AppendCircle (3, 3, 3);
						graphics.Rasterizer.AddSurface (path);
						graphics.RenderGradient ();
					}
					break;
			}
		}
		
		protected override void ProcessMessage(Message message, Drawing.Point pos)
		{
			if (this.drag_behavior.ProcessMessage (message, pos))
			{
				base.ProcessMessage (message, pos);
			}
		}
		
		
		protected void DefineGradientShape(Drawing.Graphics graphics)
		{
			double[] r = new double[256];
			double[] g = new double[256];
			double[] b = new double[256];
			double[] a = new double[256];
			
			Drawing.Color color = Design.HiliteAdorner.FrameColor;
			Drawing.Color spot  = Drawing.Color.FromRGB (1.0, 1.0, 1.0);
			
			for (int i = 0; i < 256; i++)
			{
				double mix1 = (i/255.0); // * 0.5 + 0.5;
				double mix2 = 1.0 - mix1;
				
				r[i] = mix1*color.R + mix2*spot.R;
				g[i] = mix1*color.G + mix2*spot.G;
				b[i] = mix1*color.B + mix2*spot.B;
				a[i] = 1.0;
			}
			
			graphics.GradientRenderer.SetParameters (0, 100);
			graphics.GradientRenderer.SetColors (r, g, b, a);
			graphics.GradientRenderer.Fill = Drawing.GradientFill.Circle;
		}
		
		protected void DefineGradientOffset(Drawing.Graphics graphics, double cx, double cy, double r)
		{
			Drawing.Transform t = new Drawing.Transform ();
			t.Scale (r / 100.0, r / 100.0);
			t.Translate (cx, cy);
			
			graphics.GradientRenderer.Transform = t;
		}
		
		void Helpers.IDragBehaviorHost.OnDragBegin()
		{
			if (this.DragBegin != null)
			{
				this.DragBegin (this);
			}
		}
		
		void Helpers.IDragBehaviorHost.OnDragging(DragEventArgs e)
		{
			if (this.Dragging != null)
			{
				this.Dragging (this, e);
			}
		}
		
		void Helpers.IDragBehaviorHost.OnDragEnd()
		{
			if (this.DragEnd != null)
			{
				this.DragEnd (this);
			}
		}
		
		
		public event EventHandler			DragBegin;
		public event EventHandler			DragEnd;
		public event DragEventHandler		Dragging;
		
		protected GripType					grip_type;
		protected Drawing.Color				grip_color;
		protected Helpers.DragBehavior		drag_behavior;
	}
}
