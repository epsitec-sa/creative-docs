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
			get
			{
				return this.anchor;
			}
			set
			{
				if (this.anchor != value)
				{
					this.anchor = value;
				}
			}
		}
		
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
		
		
		public float						Top
		{
			get { return this.y; }
			set { this.SetBounds (0, value, 0, 0, BoundsSpecified.Y); }
		}
		
		public float						Left
		{
			get { return this.x; }
			set { this.SetBounds (value, 0, 0, 0, BoundsSpecified.X); }
		}
		
		public float						Bottom
		{
			get { return this.y + this.height; }
			set { this.SetBounds (0, 0, 0, value - this.y, BoundsSpecified.Height); }
		}
		
		public float						Right
		{
			get { return this.x + this.width; }
			set { this.SetBounds (0, 0, value - this.x, 0, BoundsSpecified.Width); }
		}
		
		public System.Drawing.RectangleF	Bounds
		{
			get { return new System.Drawing.RectangleF (this.x, this.y, this.width, this.height); }
			set { this.SetBounds (value.X, value.Y, value.Width, value.Height, BoundsSpecified.All); }
		}
		
		public System.Drawing.PointF		Location
		{
			get { return new System.Drawing.PointF (this.x, this.y); }
			set { this.SetBounds (value.X, value.Y, 0, 0, BoundsSpecified.Location); }
		}
		
		public System.Drawing.SizeF			Size
		{
			get { return new System.Drawing.SizeF (this.width, this.height); }
			set { this.SetBounds (0, 0, value.Width, value.Height, BoundsSpecified.Size); }
		}
		
		
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
							this.children = new WidgetCollection ();
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
				
				return 0;
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
					if (mode & ChildFindMode.SkipDisabled)
					{
						if (widget.IsEnabled == false)
						{
							continue;
						}
					}
					else if (mode & ChildFindMode.SkipHidden)
					{
						if (widget.IsVisible == false)
						{
							continue;
						}
					}
				}
				
				if (widget.HitTest (point))
				{
					if (mode & ChildFindMode.SkipTransparent)
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
			if ((point.X >= this.x) &&
				(point.X <  this.x + this.width) &&
				(point.Y >= this.y) &&
				(point.Y <  this.y + this.height))
			{
				return true;
			}
			
			return false;
		}
		
		
		
		protected virtual void SetBounds(float x, float y, float width, float height, BoundsSpecified bounds)
		{
			if (bounds != BoundsSpecified.All)
			{
				if ((bounds & BoundsSpecified.X) == 0)
				{
					x = this.x;
				}
				if ((bounds & BoundsSpecified.Y) == 0)
				{
					y = this.y;
				}
				if ((bounds & BoundsSpecified.Width) == 0)
				{
					width = this.width;
				}
				if ((bounds & BoundsSpecified.Height) == 0)
				{
					height = this.height;
				}
			}
			if ((x == this.x) && (y == this.y) && (width == this.width) && (height == this.height))
			{
				return;
			}
			this.SetBoundsAndPerformLayout (x, y, width, height);
		}
		
		protected virtual void SetBoundsAndPerformLayout(float x, float y, float width, float height)
		{
			LayoutInfo layout_info = new LayoutInfo (this.x, this.y, this.width, this.height);
			
			this.x = x;
			this.y = y;
			
			this.width  = width;
			this.height = height;
			
			this.PerformLayout (layout_info);
		}
		
		
		protected virtual void PerformLayout(LayoutInfo layout_info)
		{
			lock (this)
			{
				if (this.layout_info == null)
				{
					this.layout_info = layout_info;
				}
				
				if (this.layout_suspended > 0)
				{
					this.internal_state |= InternalState.LayoutDirty;
					return;
				}
				
				try
				{
					//	TODO: gère le layout automatique du contenu...
				}
				finally
				{
					this.layout_info = null;
					this.internal_state &= ~ InternalState.LayoutDirty;
				}
			}
		}
		
		protected virtual void SuspendLayout()
		{
			lock (this)
			{
				this.layout_suspended++;
			}
		}
		
		protected virtual void ResumeLayout()
		{
			this.ResumeLayout (true);
		}
		
		protected virtual void ResumeLayout(bool perform_layout)
		{
			lock (this)
			{
				if (this.layout_suspended > 0)
				{
					this.layout_suspended--;
					
					if ((this.layout_suspended == 0) &&
						(this.internal_state & InternalState.LayoutDirty) &&
						(perform_layout))
					{
						System.Diagnostics.Debug.Assert (this.layout_info != null);
						this.PerformLayout (this.layout_info);
					}
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
			LayoutDirty	= 1,
		}
		
		[System.Flags] protected enum BoundsSpecified
		{
			None		= 0,
			X			= 1,
			Y			= 2,
			Width		= 4,
			Height		= 8,
			
			Location	= X+Y,
			Size		= Width+Height,
			All			= Location+Size
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
			Right			= 8
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
			public LayoutInfo(float x, float y, float width, float height)
			{
				this.x1 = x1;
				this.y1 = y1;
				this.x2 = x1 + width;
				this.y2 = y1 + height;
			}
			
			public float					OriginalX1
			{
				get { return this.x; }
				set { this.x = value; }
			}
			
			public float					OriginalY1
			{
				get { return this.y; }
				set { this.y = value; }
			}
			
			public float					OriginalX2
			{
				get { return this.width; }
				set { this.width = value; }
			}
			
			public float					OriginalY2
			{
				get { return this.height; }
				set { this.height = value; }
			}
			
			private float					x1, y1, x2, y2;
		}
		
		
		
		protected AnchorStyles				anchor;
		protected System.Drawing.Color		back_color;
		protected System.Drawing.Color		fore_color;
		protected float						x, y, width, height;
		protected WidgetCollection			children;
		protected Widget					parent;
		protected string					name;
		protected string					text;
		protected LayoutInfo				layout_info;
		protected int						layout_suspended;
		protected InternalState				internal_state;
	}
}
