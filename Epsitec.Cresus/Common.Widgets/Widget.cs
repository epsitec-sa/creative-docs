namespace Epsitec.Common.Widgets
{
	using Epsitec.Common.Drawing;
	
	/// <summary>
	/// 
	/// </summary>
	public class Widget
	{
		public Widget()
		{
		}
		
		public AnchorStyles					Anchor
		{
			get { return this.anchor; }
			set { this.anchor = value; }
		}
		
#if false
		public System.Drawing.Color			BackColor
		{
			get;
			set;
		}
		
		public System.Drawing.Color			ForeColor
		{
			get;
			set;
		}
#endif
		
		public float						Top
		{
			get { return this.y1; }
			set { this.SetBounds (this.x1, value, this.x2, this.y2); }
		}
		
		public float						Left
		{
			get { return this.x1; }
			set { this.SetBounds (value, this.y1, this.x2, this.y2); }
		}
		
		public float						Bottom
		{
			get { return this.y2; }
			set { this.SetBounds (this.x1, this.y1, this.x2, value); }
		}
		
		public float						Right
		{
			get { return this.x2; }
			set { this.SetBounds (this.x1, this.y1, value, this.y2); }
		}
		
		public System.Drawing.RectangleF	Bounds
		{
			get { return new System.Drawing.RectangleF (this.x1, this.y1, this.x2 - this.x1, this.y2 - this.y1); }
			set { this.SetBounds (value.X, value.Y, value.X + value.Width, value.Y + value.Height); }
		}
		
		public System.Drawing.PointF		Location
		{
			get { return new System.Drawing.PointF (this.x1, this.y1); }
			set { this.SetBounds (value.X, value.Y, value.X + this.x2 - this.x1, value.Y + this.y2 - this.y1); }
		}
		
		public System.Drawing.SizeF			Size
		{
			get { return new System.Drawing.SizeF (this.x2 - this.x1, this.y2 - this.y1); }
			set { this.SetBounds (this.x1, this.y1, this.x1 + value.Width, this.y1 + value.Height); }
		}
		
		public float						Width
		{
			get { return this.x2 - this.x1; }
			set { this.SetBounds (this.x1, this.y1, this.x1 + value, this.y2); }
		}
		
		public float						Height
		{
			get { return this.y2 - this.y1; }
			set { this.SetBounds (this.x1, this.y1, this.x2, this.y1 + value); }
		}
		
		
		public ClientInfo					Client
		{
			get { return this.client_info; }
		}
		
		
		public void SetClientAngle(int angle)
		{
			this.client_info.SetAngle (angle);
			this.UpdateClientGeometry ();
		}
		
		public void SetClientZoom(float zoom)
		{
			this.client_info.SetZoom (zoom);
			this.UpdateClientGeometry ();
		}
		
		
#if false
		public bool							CanFocus
		{
			get;
		}
		
		public bool							CanSelect
		{
			get;
		}
		
		public bool							CausesValidation
		{
			get;
			set;
		}
		
		public bool							ContainsFocus
		{
			get;
		}
#endif
		public bool							IsEnabled
		{
			get { return true; }	//	TODO:
		}
		
		public bool							IsFocused
		{
			get { return true; }	//	TODO:
		}
		
		public bool							IsVisible
		{
			get { return true; }	//	TODO:
		}

		
		public WidgetCollection				Children
		{
			get
			{
				if (this.children == null)
				{
					lock (this)
					{
						if (this.children == null)
						{
							this.children = this.CreateWidgetCollection ();
						}
					}
				}
				
				return this.children;
			}
		}
		
		public Widget						Parent
		{
			get { return this.parent; }
			
			set
			{
				if (value != this.parent)
				{
					if (value == null)
					{
						this.parent.Children.Remove (this);
					}
					else
					{
						value.Children.Add (this);
					}
				}
			}
		}
		
		public Widget						RootParent
		{
			get
			{
				Widget widget = this;
				
				while (widget.parent != null)
				{
					widget = widget.parent;
				}
				
				return widget;
			}
		}
		
		public bool							HasChildren
		{
			get { return (this.children != null) && (this.children.Count > 0); }
		}
		
		public bool							HasParent
		{
			get { return this.parent != null; }
		}
		
		public string						Name
		{
			get
			{
				if ((this.name == null) || (this.name.Length == 0))
				{
					return "";
				}
				
				return this.name;
			}
			
			set
			{
				if ((value == null) || (value.Length == 0))
				{
					this.name = null;
				}
				else
				{
					this.name = value;
				}
			}
		}

		public string						Text
		{
			get
			{
				if ((this.text == null) || (this.text.Length == 0))
				{
					return "";
				}
				
				return this.text;
			}
			
			set
			{
				if ((value == null) || (value.Length == 0))
				{
					this.text = null;
				}
				else
				{
					this.text = value;
				}
			}
		}
		
		public char							Mnemonic
		{
			get
			{
				string text = this.Text;
				
				if (text != null)
				{
					int max = text.Length - 1;
					for (int i = 0; i < max; i++)
					{
						if ((text[i] == '&') && (text[i+1] != '&'))
						{
							char mnemonic = text[i+1];
							mnemonic = System.Char.ToLower (mnemonic, System.Globalization.CultureInfo.CurrentCulture);
							return mnemonic;
						}
					}
				}
				
				return (char) 0;
			}
		}
		
		
		
		//	Cursor
		//	TabIndex, TabStop
		//	Text
		
		//	Focus/SetFocus
		//	Hide/Show/SetVisible
		//	FindNextWidget/FindPrevWidget
		//	Invalidate/Update/Refresh
		
		protected Widget FindChild(System.Drawing.PointF point)
		{
			return this.FindChild (point, ChildFindMode.All);
		}
		
		protected virtual Widget FindChild(System.Drawing.PointF point, ChildFindMode mode)
		{
			if (this.Children.Count == 0)
			{
				return null;
			}
			
			Widget[] children = this.Children.Widgets;
			int  children_num = children.Length;
			
			for (int i = 0; i < children_num; i++)
			{
				Widget widget = children[children_num-1 - i];
				
				System.Diagnostics.Debug.Assert (widget != null);
				
				if (mode != ChildFindMode.All)
				{
					if ((mode & ChildFindMode.SkipDisabled) != 0)
					{
						if (widget.IsEnabled == false)
						{
							continue;
						}
					}
					else if ((mode & ChildFindMode.SkipHidden) != 0)
					{
						if (widget.IsVisible == false)
						{
							continue;
						}
					}
				}
				
				if (widget.HitTest (point))
				{
					if ((mode & ChildFindMode.SkipTransparent) != 0)
					{
						//	TODO: vérifier que le point en question n'est pas transparent
					}
					
					return widget;
				}
			}
			
			return null;
		}
		
		
		public virtual bool HitTest(System.Drawing.PointF point)
		{
			if ((point.X >= this.x1) &&
				(point.X <  this.x2) &&
				(point.Y >= this.y1) &&
				(point.Y <  this.y2))
			{
				return true;
			}
			
			return false;
		}

		
		public virtual System.Drawing.PointF MapParentToClient(System.Drawing.PointF point)
		{
			float x = point.X - this.x1;
			float y = point.Y - this.y1;
			
			double angle = this.client_info.angle * System.Math.PI / 180.0;
			float  zoom  = this.client_info.zoom;
			
			System.Diagnostics.Debug.Assert (zoom > 0.0f);
			System.Diagnostics.Debug.Assert ((angle >= 0) && (angle < 360));
			
			if (angle != 0)
			{
				float sin = (float) System.Math.Sin (angle);
				float cos = (float) System.Math.Cos (angle);
				
				x -= (this.x2 - this.x1) / 2;
				y -= (this.y2 - this.y1) / 2;
				
				float xr = ( x * cos + y * sin) / zoom;
				float yr = (-x * sin + y * cos) / zoom;
				
				x = xr + this.client_info.width / 2;
				y = yr + this.client_info.height / 2;
			}
			else
			{
				x /= zoom;
				y /= zoom;
			}
			
			return new System.Drawing.PointF (x, y);
		}
		
		public virtual System.Drawing.PointF MapClientToParent(System.Drawing.PointF point)
		{
			float x = point.X;
			float y = point.Y;
			
			double angle = this.client_info.angle * System.Math.PI / 180.0;
			float  zoom  = this.client_info.zoom;
			
			System.Diagnostics.Debug.Assert (zoom > 0.0f);
			System.Diagnostics.Debug.Assert ((angle >= 0) && (angle < 360));
			
			if (angle != 0)
			{
				float sin = (float) System.Math.Sin (angle);
				float cos = (float) System.Math.Cos (angle);
				
				x -= this.client_info.width / 2;
				y -= this.client_info.height / 2;
				
				float xr = (x * cos - y * sin) * zoom;
				float yr = (x * sin + y * cos) * zoom;
				
				x = xr + (this.x2 - this.x1) / 2;
				y = yr + (this.y2 - this.y1) / 2;
			}
			else
			{
				x *= zoom;
				y *= zoom;
			}
			
			return new System.Drawing.PointF (x + this.x1, y + this.y1);
		}

		public virtual System.Drawing.PointF MapRootToClient(System.Drawing.PointF point)
		{
			Widget parent = this.Parent;
			
			//	Le plus simple est d'utiliser la récursion, afin de commencer la conversion depuis la
			//	racine, puis d'enfant en enfant jusqu'au widget final.
			
			if (parent != null)
			{
				point = parent.MapRootToClient (point);
			}
			
			return this.MapParentToClient (point);
		}
		
		public virtual System.Drawing.PointF MapClientToRoot(System.Drawing.PointF point)
		{
			Widget iter = this;
			
			//	On a le choix entre une solution récursive et une solution itérative. La version
			//	itérative devrait être un petit peu plus rapide ici.
			
			while (iter != null)
			{
				point = iter.MapClientToParent (point);
				iter = iter.Parent;
			}
			
			return point;
		}
		
		
		public virtual System.Drawing.RectangleF MapParentToClient(System.Drawing.RectangleF rect)
		{
			float x1 = rect.Left   - this.x1;
			float y1 = rect.Top    - this.y1;
			float x2 = rect.Right  - this.x1;
			float y2 = rect.Bottom - this.y1;
			
			double angle = this.client_info.angle * System.Math.PI / 180.0;
			float  zoom  = this.client_info.zoom;
			
			System.Diagnostics.Debug.Assert (zoom > 0.0f);
			System.Diagnostics.Debug.Assert ((angle >= 0) && (angle < 360));
			
			if (angle != 0)
			{
				float sin = (float) System.Math.Sin (angle);
				float cos = (float) System.Math.Cos (angle);
				
				float cx = (this.x2 - this.x1) / 2;
				float cy = (this.y2 - this.y1) / 2;
				
				x1 -= cx;		x2 -= cx;
				y1 -= cy;		y2 -= cy;
				
				float xr1 = ( x1 * cos + y1 * sin) / zoom;
				float yr1 = (-x1 * sin + y1 * cos) / zoom;
				float xr2 = ( x2 * cos + y2 * sin) / zoom;
				float yr2 = (-x2 * sin + y2 * cos) / zoom;
				
				cx = this.client_info.width / 2;
				cy = this.client_info.height / 2;
				
				x1 = xr1 + cx;		x2 = xr2 + cx;
				y1 = yr1 + cy;		y2 = yr2 + cy;
			}
			else
			{
				x1 /= zoom;		x2 /= zoom;
				y1 /= zoom;		y2 /= zoom;
			}
			
			rect.X = System.Math.Min (x1, x2);
			rect.Y = System.Math.Min (y1, y2);
			
			rect.Width  = System.Math.Abs (x2 - x1);
			rect.Height = System.Math.Abs (y2 - y1);
			
			return rect;
		}
		
		
		public virtual Epsitec.Common.Drawing.Transform GetRootToClientTransform()
		{
			Widget iter = this;
			
			Transform full_transform  = new Transform ();
			Transform local_transform = new Transform ();
			
			while (iter != null)
			{
				local_transform.Reset ();
				iter.MergeTransformToClient (local_transform);
				
				//	Les transformations de la racine au client doivent s'appliquer en commençant par
				//	la racine. Comme nous remontons la hiérarchie des widgets en sens inverse, il nous
				//	suffit d'utiliser la multiplication post-fixe pour arriver au même résultat :
				//
				//	 T = Tn * ... * T2 * T1 * T0, P' = T * P
				//
				//	avec Ti la transformation pour le widget 'i', où i=0 correspond à la racine,
				//	P le point en coordonnées racine et P' le point en coordonnées client.
				
				full_transform.MultiplyByPostfix (local_transform);
				iter = iter.Parent;
			}
			
			return full_transform;
		}
		
		public virtual Epsitec.Common.Drawing.Transform GetClientToRootTransform()
		{
			Widget iter = this;
			
			Transform full_transform  = new Transform ();
			Transform local_transform = new Transform ();
			
			while (iter != null)
			{
				local_transform.Reset ();
				iter.MergeTransformToParent (local_transform);
				
				//	Les transformations du client à la racine doivent s'appliquer en commençant par
				//	le client. Comme nous remontons la hiérarchie des widgets dans ce sens là, il nous
				//	suffit d'utiliser la multiplication normale pour arriver à ce résultat :
				//
				//	 T = T0 * T1 * T2 * ... * Tn, P' = T * P
				//
				//	avec Ti la transformation pour le widget 'i', où i=0 correspond à la racine.
				//	P le point en coordonnées client et P' le point en coordonnées racine.
				
				full_transform.MultiplyBy (local_transform);
				iter = iter.Parent;
			}
			
			return full_transform;
		}
		
		
		public virtual void MergeTransformToClient(Epsitec.Common.Drawing.Transform t)
		{
			float scale = 1 / this.client_info.zoom;
			
			t.Translate (- this.x1, - this.y1);
			t.Translate (- this.Width / 2, - this.Height / 2);
			t.Rotate (this.client_info.angle);
			t.Scale (scale);
			t.Translate (this.client_info.width / 2, this.client_info.height / 2);
			t.Round ();
		}
		
		public virtual void MergeTransformToParent(Epsitec.Common.Drawing.Transform t)
		{
			float scale = this.client_info.zoom;
			
			t.Translate (- this.client_info.width / 2, - this.client_info.height / 2);
			t.Scale (scale);
			t.Rotate (- this.client_info.angle);
			t.Translate (this.Width / 2, this.Height / 2);
			t.Translate (this.x1, this.y1);
			t.Round ();
		}
		
		
		protected virtual void SetBounds(float x1, float y1, float x2, float y2)
		{
			if ((x1 == this.x1) && (y1 == this.y1) && (x2 == this.x2) && (y2 == this.y2))
			{
				return;
			}
			
			this.x1 = x1;
			this.y1 = y1;
			this.x2 = x2;
			this.y2 = y2;
			
			this.UpdateClientGeometry ();
		}
		
		
		protected virtual void UpdateClientGeometry()
		{
			System.Diagnostics.Debug.Assert (this.layout_info == null);
			
			this.layout_info = new LayoutInfo (this.client_info.width, this.client_info.height);
			
			try
			{
				float zoom = this.client_info.zoom;
				
				float dx = (this.x2 - this.x1) / zoom;
				float dy = (this.y2 - this.y1) / zoom;
				
				switch (this.client_info.angle)
				{
					case 0:
					case 180:
						this.client_info.SetSize (dx, dy);
						break;
					
					case 90:
					case 270:
						this.client_info.SetSize (dy, dx);
						break;
					
					default:
						double angle = this.client_info.angle * System.Math.PI / 180.0;
						float cos = (float) System.Math.Cos (angle);
						float sin = (float) System.Math.Sin (angle);
						this.client_info.SetSize (cos*cos*dx + sin*sin*dy, sin*sin*dx + cos*cos*dy);
						break;
				}
				
				this.UpdateChildrenLayout ();
			}
			finally
			{
				this.layout_info = null;
			}
		}
		
		protected virtual void UpdateChildrenLayout()
		{
			System.Diagnostics.Debug.Assert (this.client_info != null);
			System.Diagnostics.Debug.Assert (this.layout_info != null);
			
			if (this.HasChildren)
			{
				float width_diff  = this.client_info.width  - this.layout_info.OriginalWidth;
				float height_diff = this.client_info.height - this.layout_info.OriginalHeight;
				
				foreach (Widget child in this.children)
				{
					AnchorStyles anchor_x = (AnchorStyles) child.Anchor & AnchorStyles.LeftAndRight;
					AnchorStyles anchor_y = (AnchorStyles) child.Anchor & AnchorStyles.TopAndBottom;
					
					float x1 = child.x1;
					float x2 = child.x2;
					float y1 = child.y1;
					float y2 = child.y2;
					
					switch (anchor_x)
					{
						case AnchorStyles.Left:							//	[x1] fixe à gauche
							break;
						case AnchorStyles.Right:						//	[x2] fixe à droite
							x1 += width_diff;
							x2 += width_diff;
							break;
						case AnchorStyles.None:							//	[x1] et [x2] mobiles (centré)
							x1 += width_diff / 2.0f;
							x2 += width_diff / 2.0f;
							break;
						case AnchorStyles.LeftAndRight:					//	[x1] fixe à gauche, [x2] fixe à droite
							x2 += width_diff;
							break;
					}
					
					switch (anchor_y)
					{
						case AnchorStyles.Top:							//	[y1] fixe en haut
							break;
						case AnchorStyles.Bottom:						//	[y2] fixe en bas
							y1 += height_diff;
							y2 += height_diff;
							break;
						case AnchorStyles.None:							//	[y1] et [y2] mobiles (centré)
							y1 += height_diff / 2.0f;
							y2 += height_diff / 2.0f;
							break;
						case AnchorStyles.TopAndBottom:					//	[y1] fixe en haut, [y2] fixe en bas
							y2 += height_diff;
							break;
					}
					
					child.SetBounds (x1, y1, x2, y2);
				}
			}
		}
		
		
		protected virtual bool PaintCheckClipping(PaintEventArgs e)
		{
			return e.ClipRectangle.IntersectsWith (this.Bounds);
		}
		
		protected virtual void PaintAlgorithm(PaintEventArgs e)
		{
			if (this.PaintCheckClipping (e))
			{
				Graphics                  graphics  = e.Graphics;
				System.Drawing.RectangleF clip_rect = this.MapParentToClient (e.ClipRectangle);
				
				Transform original_transform = graphics.Transform;
				Transform graphics_transform = new Transform (original_transform);
				
				this.MergeTransformToClient (graphics_transform);
				
				graphics.Transform = graphics_transform;
			
				try
				{
					PaintEventArgs local_paint_args = new PaintEventArgs (graphics, clip_rect);
					
					//	Peint l'arrière-plan du widget. En principe, tout va dans l'arrière plan, sauf
					//	si l'on désire réaliser des effets de transparence par dessus le dessin des
					//	widgets enfants.
					
					this.OnPaintBackground (local_paint_args);
					
					//	Peint tous les widgets enfants, en commençant par le numéro 0, lequel se trouve
					//	derrière tous les autres, etc. On saute les widgets qui ne sont pas visibles.
					
					if (this.Children.Count > 0)
					{
						Widget[] children = this.Children.Widgets;
						int  children_num = children.Length;
						
						for (int i = 0; i < children_num; i++)
						{
							Widget widget = children[i];
						
							System.Diagnostics.Debug.Assert (widget != null);
						
							if (widget.IsVisible)
							{
								widget.PaintAlgorithm (local_paint_args);
							}
						}
					}
				
					//	Peint l'avant-plan du widget, à n'utiliser que pour faire un "effet" spécial
					//	après coup.
					
					this.OnPaintForeground (local_paint_args);
				}
				finally
				{
					graphics.Transform = original_transform;
				}
			}
		}
		
		protected virtual void OnPaintBackground(PaintEventArgs e)
		{
		}
		
		protected virtual void OnPaintForeground(PaintEventArgs e)
		{
		}
		
		
		
		protected virtual WidgetCollection CreateWidgetCollection()
		{
			return new WidgetCollection (this);
		}
		
		
		
		[System.Flags] protected enum InternalState
		{
			None		= 0,
		}
		
		[System.Flags] public enum ChildFindMode
		{
			All				= 0,
			SkipHidden		= 1,
			SkipDisabled	= 2,
			SkipTransparent	= 4
		}
		
		[System.Flags] public enum AnchorStyles
		{
			None			= 0,
			Top				= 1,
			Bottom			= 2,
			Left			= 4,
			Right			= 8,
			
			LeftAndRight	= Left + Right,
			TopAndBottom	= Top + Bottom,
		}
		
		
		public class ClientInfo
		{
			internal ClientInfo()
			{
			}
			
			internal void SetSize(float width, float height)
			{
				this.width  = width;
				this.height = height;
			}
			
			internal void SetAngle(int angle)
			{
				angle = angle % 360;
				this.angle = (angle < 0) ? (short) (angle + 360) : (short) (angle);
			}
			
			internal void SetZoom(float zoom)
			{
				System.Diagnostics.Debug.Assert (zoom > 0.0f);
				this.zoom = zoom;
			}
			
			public float					Width
			{
				get { return this.width; }
			}
			
			public float					Height
			{
				get { return this.height; }
			}
			
			public System.Drawing.SizeF		Size
			{
				get { return new System.Drawing.SizeF (this.width, this.height); }
			}
			
			public int						Angle
			{
				get { return this.angle; }
			}
			
			public float					Zoom
			{
				get { return this.zoom; }
			}
			
			internal float					width	= 0.0f;
			internal float					height	= 0.0f;
			internal short					angle	= 0;
			internal float					zoom	= 1.0f;
		}
		
		public class WidgetCollection : System.Collections.IList
		{
			public WidgetCollection(Widget widget)
			{
				this.list   = new System.Collections.ArrayList ();
				this.widget = widget;
			}
			
			
			public Widget[]					Widgets
			{
				get
				{
					if (this.array == null)
					{
						this.array = new Widget[this.list.Count];
						this.list.CopyTo (this.array);
					}
					
					return this.array;
				}
			}
			
			private void PreInsert(object widget)
			{
				if (widget is Widget)
				{
					this.PreInsert (widget as Widget);
				}
				else
				{
					throw new System.ArgumentException ("Widget");
				}
			}
			
			private void PreInsert(Widget widget)
			{
				if (widget.parent != null)
				{
					Widget parent = widget.parent;
					parent.Children.Remove (widget);
					System.Diagnostics.Debug.Assert (widget.parent == null);
				}
				widget.parent = this.widget;
			}
			
			private void PreRemove(object widget)
			{
				if (widget is Widget)
				{
					this.PreRemove (widget as Widget);
				}
				else
				{
					throw new System.ArgumentException ("Widget");
				}
			}
			
			private void PreRemove(Widget widget)
			{
				System.Diagnostics.Debug.Assert (widget.parent == this.widget);
				widget.parent = null;
			}
			
			
			#region IList Members
			
			public bool IsReadOnly
			{
				get	{ return false; }
			}
			
			public object this[int index]
			{
				get	{ return this.list[index]; }
				set	{ throw new System.NotSupportedException ("Widget"); }
			}
			
			public void RemoveAt(int index)
			{
				System.Diagnostics.Debug.Assert (this.list[index] != null);
				this.PreRemove (this.list[index]);
				this.list.RemoveAt (index);
			}
			
			public void Insert(int index, object value)
			{
				throw new System.NotSupportedException ("Widget");
			}
			
			public void Remove(object value)
			{
				this.PreRemove (value);
				this.list.Remove (value);
			}
			
			public bool Contains(object value)
			{
				return this.list.Contains (value);
			}
			
			public void Clear()
			{
				while (this.Count > 0)
				{
					this.RemoveAt (this.Count - 1);
				}
			}
			
			public int IndexOf(object value)
			{
				return this.list.IndexOf (value);
			}
			
			public int Add(object value)
			{
				this.PreInsert (value);
				return this.list.Add (value);
			}
			
			public bool IsFixedSize
			{
				get	{ return false; }
			}

			#endregion

			#region ICollection Members

			public bool IsSynchronized
			{
				get { return false; }
			}
			
			public int Count
			{
				get	{ return this.list.Count; }
			}

			public void CopyTo(System.Array array, int index)
			{
				this.list.CopyTo (array, index);
			}
			
			public object SyncRoot
			{
				get { return this.list.SyncRoot; }
			}

			#endregion

			#region IEnumerable Members

			public System.Collections.IEnumerator GetEnumerator()
			{
				return this.list.GetEnumerator ();
			}
			
			#endregion
			
			System.Collections.ArrayList	list;
			Widget[]						array;
			Widget							widget;
		}
		
		
		protected class LayoutManager
		{
		}
		
		protected class LayoutInfo
		{
			internal LayoutInfo(float width, float height)
			{
				this.width  = width;
				this.height = height;
			}
			
			public float					OriginalWidth
			{
				get { return this.width; }
			}
			
			public float					OriginalHeight
			{
				get { return this.height; }
			}
			
			private float					width, height;
		}
		
		
		
		protected AnchorStyles				anchor;
		protected System.Drawing.Color		back_color;
		protected System.Drawing.Color		fore_color;
		protected float						x1, y1, x2, y2;
		protected ClientInfo				client_info = new ClientInfo ();
		
		protected WidgetCollection			children;
		protected Widget					parent;
		protected string					name;
		protected string					text;
		protected LayoutInfo				layout_info;
		protected InternalState				internal_state;
	}
}
