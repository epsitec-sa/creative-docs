namespace Epsitec.Common.Widgets
{
	/// <summary>
	/// Liste les types de poignées connues.
	/// </summary>
	public enum GripType
	{
		None,
		Vertex,								//	coin (poignée standard)
		Center								//	centre de gravité
	}
	
	/// <summary>
	/// La classe Grip implémente une poignée avec à la fois le support pour son
	/// dessin et une aide pour le dragging de celle-ci (génère un DragEvent qui
	/// décrit le déplacement désiré).
	/// </summary>
	public class Grip : Widget
	{
		public Grip()
		{
			this.grip_type  = GripType.None;
			this.grip_color = Drawing.Color.FromRGB (1.0, 0.0, 0.0);
			
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
					case GripType.Vertex:	return new Drawing.Point (2, 2);
					case GripType.Center:	return new Drawing.Point (2, 2);
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
					case GripType.Vertex:	return new Drawing.Size (5, 5);
					case GripType.Center:	return new Drawing.Size (5, 5);
				}
				
				return Drawing.Size.Empty;
			}
		}
		
		
		protected override void PaintBackgroundImplementation(Drawing.Graphics graphics, Drawing.Rectangle clip_rect)
		{
			switch (this.grip_type)
			{
				case GripType.Vertex:
				case GripType.Center:
					using (Drawing.Path path = new Drawing.Path ())
					{
						path.AppendCircle (2.5, 2.5, 2.5);
						graphics.Rasterizer.AddSurface (path);
						graphics.RenderSolid (this.grip_color);
					}
					using (Drawing.Path path = new Drawing.Path ())
					{
						path.AppendCircle (2.5, 2.5, 1.5);
						graphics.Rasterizer.AddSurface (path);
						graphics.RenderSolid (Drawing.Color.FromBrightness (1.0));
					}
					break;
			}
		}
		
		protected override void ProcessMessage(Message message, Drawing.Point pos)
		{
			base.ProcessMessage (message, pos);
			
			if (this.IsEnabled)
			{
				switch (message.Type)
				{
					case MessageType.MouseDown:
						if ((message.Button == MouseButtons.Left) &&
							(message.ButtonDownCount == 1))
						{
							this.StartDragging (message, pos);
						}
						break;
					
					case MessageType.MouseUp:
						if (message.Button == MouseButtons.Left)
						{
							this.StopDragging (message, pos);
						}
						break;
					
					case MessageType.MouseMove:
						if (Message.State.Buttons == MouseButtons.Left)
						{
							this.HandleDragging (message, pos);
						}
						break;
				}
			}
		}
		
		
		protected virtual void StartDragging(Message message, Drawing.Point pos)
		{
			message.Consumer = this;
			this.is_dragging = true;
			
			this.drag_mouse_offset = message.Cursor - this.GripLocation;
		}
		
		protected virtual void StopDragging(Message message, Drawing.Point pos)
		{
			if (this.is_dragging)
			{
				message.Consumer = this;
				this.is_dragging = false;
			}
		}
		
		protected virtual void HandleDragging(Message message, Drawing.Point pos)
		{
			if (this.is_dragging)
			{
				System.Diagnostics.Debug.WriteLine ("Pos: " + pos.ToString () + ", message.Cursor: " + message.Cursor.ToString ());
				
				message.Consumer = this;
				
				Drawing.Point old_pos = this.GripLocation;
				Drawing.Point new_pos = message.Cursor - drag_mouse_offset;
				
				this.OnDragging (new DragEventArgs (old_pos, new_pos));
			}
		}
		
		
		protected virtual void OnDragging(DragEventArgs e)
		{
			if (this.Dragging != null)
			{
				this.Dragging (this, e);
			}
		}
		
		
		public event DragEventHandler		Dragging;
		
		protected GripType					grip_type;
		protected Drawing.Color				grip_color;
		
		protected bool						is_dragging;
		protected Drawing.Point				drag_mouse_offset;
	}
}
