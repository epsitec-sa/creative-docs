namespace Epsitec.Common.Widgets
{
	using Keys = System.Windows.Forms.Keys;

	/// <summary>
	/// La classe TextFieldCombo implémente la ligne éditable avec bouton "v".
	/// </summary>
	public class TextFieldCombo : AbstractTextField
	{
		public TextFieldCombo() : base(TextFieldType.Combo)
		{
			this.items = new ObjectCollection (this);
			
			this.arrowDown = new ArrowButton();
			this.arrowDown.Direction = Direction.Down;
			this.arrowDown.ButtonStyle = ButtonStyle.Scroller;
			this.arrowDown.Pressed += new MessageEventHandler (this.HandleCombo);
			this.arrowDown.Parent = this;
		}
		protected override void UpdateClientGeometry()
		{
			base.UpdateClientGeometry ();
			Drawing.Rectangle rect = this.Bounds;
			double m = TextField.margin-1;
			this.rightMargin = rect.Height-m*2;
			if ( this.rightMargin > rect.Width/2 )  this.rightMargin = rect.Width/2;
			rect.Inflate(-m, -m);

			if ( this.arrowDown != null )
			{
				Drawing.Rectangle aRect = new Drawing.Rectangle(m+rect.Width-this.rightMargin, m, this.rightMargin, rect.Height);
				this.arrowDown.Bounds = aRect;
			}
		}

		
		protected override void ProcessKeyDown(Keys key, bool isShiftPressed, bool isCtrlPressed)
		{
			switch (key)
			{
				case Keys.Up:
					this.ComboExcavation(-1);
					break;
				case Keys.Down:
					this.ComboExcavation(1);
					break;
				default:
					base.ProcessKeyDown (key, isShiftPressed, isCtrlPressed);
					break;
			}
		}
		
		public ObjectCollection Items
		{
			get { return this.items; }
		}
		
		
		protected override void Dispose(bool disposing)
		{
			base.Dispose (disposing);
			
			if (disposing)
			{
				this.arrowDown.Pressed -= new MessageEventHandler (this.HandleCombo);
			}
		}

		// Cherche le nom suivant ou précédent dans la comboList, même si elle
		// n'est pas "déroulée".
		protected void ComboExcavation(int dir)
		{
			int		sel;
			bool	exact;

			if ( !this.ComboSearch(out sel, out exact) )
			{
				sel = 0;
			}
			else
			{
				if ( exact)  sel += dir;
			}
			sel = System.Math.Max(sel, 0);
			sel = System.Math.Min(sel, this.items.Count-1);
			this.Text = this.items[sel];
			this.SelectAll();
			this.SetFocused(true);
		}

		// Cherche à quelle ligne (dans comboList) correspond le mieux la ligne éditée.
		protected bool ComboSearch(out int rank, out bool exact)
		{
			string edit = this.Text.ToUpper();
			rank = 0;
			exact = false;
			foreach ( string text in this.items )
			{
				string maj = text.ToUpper();
				if ( maj == edit )
				{
					exact = true;
					return true;
				}

				if ( maj.StartsWith(edit) )
				{
					exact = false;
					return true;
				}

				rank ++;
			}
			return false;
		}

		// Conversion d'une coordonnée écran -> widget.
		protected Drawing.Point MapScreenToClient(Widget widget, Drawing.Point pos)
		{
			pos = widget.WindowFrame.MapScreenToWindow(pos);
			pos = widget.MapRootToClient(pos);
			return pos;
		}

		private void HandlerMessageFilter(object sender, Message message)
		{
			if ( this.scrollList == null )  return;
			WindowFrame window = sender as WindowFrame;

			switch ( message.Type )
			{
				case MessageType.MouseDown:
					Drawing.Point mouse = window.MapWindowToScreen(message.Cursor);
					Drawing.Point pos = this.MapScreenToClient(this.scrollList, mouse);
					if ( !this.scrollList.HitTest(pos) )
					{
						this.scrollList.SelectedIndexChanged -= new EventHandler(this.HandleScrollListSelectedIndexChanged);
						WindowFrame.MessageFilter -= new Epsitec.Common.Widgets.MessageHandler(this.HandlerMessageFilter);
						WindowFrame.ApplicationDeactivated -= new EventHandler(this.HandleApplicationDeactivated);
						this.scrollList.Dispose();
						this.scrollList = null;
						this.comboWindow.Dispose();
						this.comboWindow = null;

						if ( !message.NonClient )
						{
							message.Handled = true;
							message.Swallowed = true;
						}
					}
					break;
			}
		}

		private void HandleApplicationDeactivated(object sender)
		{
			this.scrollList.SelectedIndexChanged -= new EventHandler(this.HandleScrollListSelectedIndexChanged);
			WindowFrame.MessageFilter -= new Epsitec.Common.Widgets.MessageHandler(this.HandlerMessageFilter);
			WindowFrame.ApplicationDeactivated -= new EventHandler(this.HandleApplicationDeactivated);
			this.scrollList.Dispose();
			this.scrollList = null;
			this.comboWindow.Dispose();
			this.comboWindow = null;
		}

		private void HandleCombo(object sender, MessageEventArgs e)
		{
			this.scrollList = new ScrollList();
			this.scrollList.ScrollListStyle = ScrollListStyle.Simple;
			this.scrollList.ComboMode = true;
			Drawing.Point pos = new Drawing.Point(0, 0);
			this.scrollList.Location = pos;
			this.scrollList.Size = new Drawing.Size(this.Width, 200);

			int sel = -1;
			int i = 0;
			foreach ( string text in this.items )
			{
				this.scrollList.AddText(text);
				if ( text == this.Text )
				{
					sel = i;
				}
				i ++;
			}

			pos = this.MapClientToRoot(new Drawing.Point(0, 0));
			pos = this.WindowFrame.MapWindowToScreen(pos);
			ScreenInfo si = ScreenInfo.Find(pos);
			Drawing.Rectangle wa = si.WorkingArea;
			double hMax = pos.Y-wa.Bottom;
			this.scrollList.AdjustToContent(ScrollListAdjust.MoveUp, 40, hMax);

			this.scrollList.SelectedIndex = sel;
			this.scrollList.ShowSelect(ScrollListShow.Middle);
			this.scrollList.SelectedIndexChanged += new EventHandler(this.HandleScrollListSelectedIndexChanged);

			this.comboWindow = new WindowFrame();
			this.comboWindow.MakeFramelessWindow();
			pos = this.MapClientToRoot(new Drawing.Point(0, -this.scrollList.Height));
			pos = this.WindowFrame.MapWindowToScreen(pos);
			this.comboWindow.WindowBounds = new Drawing.Rectangle(pos.X, pos.Y, this.scrollList.Width, this.scrollList.Height);
			WindowFrame.MessageFilter += new Epsitec.Common.Widgets.MessageHandler(this.HandlerMessageFilter);
			WindowFrame.ApplicationDeactivated += new EventHandler(this.HandleApplicationDeactivated);
			this.comboWindow.Root.Children.Add(this.scrollList);
			this.comboWindow.AnimateShow(Animation.RollDown);

			this.SetFocused(false);
			this.scrollList.SetFocused(true);
		}
		
		// Gestion d'un événement lorsque la scroll-liste est sélectionnée.
		private void HandleScrollListSelectedIndexChanged(object sender)
		{
			int sel = this.scrollList.SelectedIndex;
			if ( sel == -1 )  return;
			this.Text = this.scrollList.GetText(sel);
			this.OnTextChanged();
			this.OnTextInserted();
			this.SelectAll();
			this.SetFocused(true);

			this.scrollList.SelectedIndexChanged -= new EventHandler(this.HandleScrollListSelectedIndexChanged);
			WindowFrame.MessageFilter -= new Epsitec.Common.Widgets.MessageHandler(this.HandlerMessageFilter);
			WindowFrame.ApplicationDeactivated -= new EventHandler(this.HandleApplicationDeactivated);
			this.scrollList.Dispose();
			this.scrollList = null;
			this.comboWindow.Dispose();
			this.comboWindow = null;
		}

		#region ObjectCollection Class
		public class ObjectCollection : System.Collections.IList, System.IDisposable
		{
			public ObjectCollection(TextFieldCombo combo)
			{
				this.combo = combo;
				this.list  = new System.Collections.ArrayList ();
			}
			
			public string this[int index]
			{
				get
				{
					return this.list[index].ToString ();
				}
			}
			
			
			public void Dispose()
			{
				System.Diagnostics.Debug.Assert (this.list.Count == 0);
				
				this.combo = null;
				this.list = null;
			}
			
			
			#region IList Members
			public bool IsReadOnly
			{
				get
				{
					return false;
				}
			}

			object System.Collections.IList.this[int index]
			{
				get
				{
					return this.list[index];
				}
				set
				{
					this.list[index] = value;
				}
			}

			public void RemoveAt(int index)
			{
				object item = this.list[index];
				this.HandleRemove (item);
				this.list.RemoveAt (index);
			}

			public void Insert(int index, object value)
			{
				this.list.Insert (index, value);
				this.HandleInsert (value);
			}

			public void Remove(object value)
			{
				this.HandleRemove (value);
				this.list.Remove (value);
			}

			public bool Contains(object value)
			{
				return this.Contains (value);
			}

			public void Clear()
			{
				foreach (object item in this.list)
				{
					this.HandleRemove (item);
				}
				this.list.Clear ();
			}

			public int IndexOf(object value)
			{
				return this.list.IndexOf (value);
			}

			public int Add(object value)
			{
				int index = this.list.Add (value);
				this.HandleInsert (value);
				return index;
			}

			public bool IsFixedSize
			{
				get
				{
					return this.list.IsFixedSize;
				}
			}
			#endregion
			
			#region ICollection Members
			public bool IsSynchronized
			{
				get
				{
					return this.list.IsSynchronized;
				}
			}

			public int Count
			{
				get
				{
					return this.list.Count;
				}
			}

			public void CopyTo(System.Array array, int index)
			{
				this.list.CopyTo (array, index);
			}

			public object SyncRoot
			{
				get
				{
					return this.list.SyncRoot;
				}
			}

			#endregion
			
			#region IEnumerable Members
			public System.Collections.IEnumerator GetEnumerator()
			{
				return this.list.GetEnumerator ();
			}
			#endregion
			
			protected void HandleInsert(object item)
			{
			}
			
			protected void HandleRemove(object item)
			{
			}
			
			
			
			private System.Collections.ArrayList	list;
			private TextFieldCombo					combo;
		}

		#endregion
		
		
//		protected ArrowButton					arrowDown;
		protected ObjectCollection					items;
		protected WindowFrame					comboWindow;
		protected ScrollList					scrollList;
	}
}
