namespace Epsitec.Cresus.Widgets
{
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
		
		public ClientInfo					Client
		{
			get { return this.client_info; }
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
		
		public bool							IsEnabled
		{
			get;
		}
		
		public bool							IsFocused
		{
			get;
		}
		
		public bool							IsVisible
		{
			get;
		}
#endif

		
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
					this.Text = value;
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
			foreach (Widget widget in this.Children)
			{
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
				float dx = this.x2 - this.x1;
				float dy = this.y2 - this.y1;
				
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
						float cos = (float) System.Math.Cos (this.client_info.angle);
						float sin = (float) System.Math.Sin (this.client_info.angle);
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
		
		
		
		protected virtual WidgetCollection CreateWidgetCollection()
		{
			return new WidgetCollection ();
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
			
			internal float					width	= 0.0f;
			internal float					height	= 0.0f;
			internal short					angle	= 0;
		}
		
		public class WidgetCollection : System.Collections.IList
		{
			#region IList Members

			public bool IsReadOnly
			{
				get
				{
					// TODO:  Add WidgetCollection.IsReadOnly getter implementation
					return false;
				}
			}

			public object this[int index]
			{
				get
				{
					// TODO:  Add WidgetCollection.this getter implementation
					return null;
				}
				set
				{
					// TODO:  Add WidgetCollection.this setter implementation
				}
			}

			public void RemoveAt(int index)
			{
				// TODO:  Add WidgetCollection.RemoveAt implementation
			}

			public void Insert(int index, object value)
			{
				// TODO:  Add WidgetCollection.Insert implementation
			}

			public void Remove(object value)
			{
				// TODO:  Add WidgetCollection.Remove implementation
			}

			public bool Contains(object value)
			{
				// TODO:  Add WidgetCollection.Contains implementation
				return false;
			}

			public void Clear()
			{
				// TODO:  Add WidgetCollection.Clear implementation
			}

			public int IndexOf(object value)
			{
				// TODO:  Add WidgetCollection.IndexOf implementation
				return 0;
			}

			public int Add(object value)
			{
				// TODO:  Add WidgetCollection.Add implementation
				return 0;
			}

			public bool IsFixedSize
			{
				get
				{
					// TODO:  Add WidgetCollection.IsFixedSize getter implementation
					return false;
				}
			}

			#endregion

			#region ICollection Members

			public bool IsSynchronized
			{
				get
				{
					// TODO:  Add WidgetCollection.IsSynchronized getter implementation
					return false;
				}
			}

			public int Count
			{
				get
				{
					// TODO:  Add WidgetCollection.Count getter implementation
					return 0;
				}
			}

			public void CopyTo(System.Array array, int index)
			{
				// TODO:  Add WidgetCollection.CopyTo implementation
			}

			public object SyncRoot
			{
				get
				{
					// TODO:  Add WidgetCollection.SyncRoot getter implementation
					return null;
				}
			}

			#endregion

			#region IEnumerable Members

			public System.Collections.IEnumerator GetEnumerator()
			{
				// TODO:  Add WidgetCollection.GetEnumerator implementation
				return null;
			}

			#endregion
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
