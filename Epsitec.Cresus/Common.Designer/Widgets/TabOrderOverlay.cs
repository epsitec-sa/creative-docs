//	Copyright © 2004, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Statut : en chantier/PA

using Epsitec.Common.Widgets;
using Epsitec.Common.Support;

namespace Epsitec.Common.Designer.Widgets
{
	using IWidgetCollectionHost = Epsitec.Common.Widgets.Helpers.IWidgetCollectionHost;
	using WidgetCollection      = Epsitec.Common.Widgets.Helpers.WidgetCollection;
	
	/// <summary>
	/// La classe TabOrderOverlay crée une surface qui a la même taille que
	/// la fenêtre sous-jacente; cette surface va abriter des pastilles
	/// représentant les numéros d'ordre de tabulation.
	/// </summary>
	public class TabOrderOverlay : Widget
	{
		public TabOrderOverlay()
		{
			this.black_list = new System.Collections.ArrayList ();
			
			this.Name = string.Format ("TabOrderOverlay{0}", TabOrderOverlay.overlay_id++);
			
			this.SetFrozen (true);
		}
		
		public TabOrderOverlay(Widget embedder) : this ()
		{
			this.SetEmbedder (embedder);
		}
		
		
		public Widget							RootWidget
		{
			get
			{
				return this.root_widget;
			}
			set
			{
				if (this.root_widget != value)
				{
					if (this.root_widget != null)
					{
						this.DetachWidget (this.root_widget);
					}
					
					this.root_widget = value;
					
					if (this.root_widget != null)
					{
						this.AttachWidget (this.root_widget);
					}
				}
			}
		}
		
		public bool								IsPickerActive
		{
			get
			{
				return this.is_picker_active;
			}
			set
			{
				if (this.is_picker_active != value)
				{
					this.is_picker_active = value;
					
					this.UpdateInteraction ();
				}
			}
		}
		
		public bool								IsSetterActive
		{
			get
			{
				return this.is_setter_active;
			}
			set
			{
				if (this.is_setter_active != value)
				{
					this.is_setter_active = value;
					
					this.UpdateInteraction ();
				}
			}
		}
		
		
		public void SetNextIndex(int index)
		{
			this.black_list.Clear ();
			
			if (this.internal_set == false)
			{
				this.parent_filter = null;
			}
			
			this.default_index = index;
			this.Invalidate ();
		}
		
		protected bool CheckDisabled(Widget widget)
		{
			if (this.black_list.Contains (widget))
			{
				return true;
			}
			
			if (this.parent_filter != null)
			{
				if (this.parent_filter != widget.Parent)
				{
					return true;
				}
			}
			
			return false;
		}
		
		
		protected virtual void UpdateInteraction()
		{
			bool is_interactive = false;
			
			MouseCursor cursor = MouseCursor.Default;
			
			if (this.is_picker_active)
			{
				cursor = MouseCursor.FromImage (Support.ImageProvider.Default.GetImage ("file:images/numpicker.icon"));
				
				is_interactive = true;
			}
			else if (this.is_setter_active)
			{
				if (this.hot_widget != null)
				{
					cursor = MouseCursor.AsHand;
				}
				
				is_interactive = true;
			}
			
			this.MouseCursor = cursor;
			
			if (is_interactive)
			{
				this.SetFrozen (false);
			}
			else
			{
				this.SetFrozen (true);
				this.hot_widget = null;
			}
			
			this.Invalidate ();
		}
		
		protected virtual void AttachWidget(Widget widget)
		{
			if (this.Parent == null)
			{
				this.Parent = widget.Window.Root;
				this.Bounds = this.Parent.Client.Bounds;
				this.Anchor = AnchorStyles.LeftAndRight | AnchorStyles.TopAndBottom;
			}
			else
			{
				System.Diagnostics.Debug.Assert (this.Parent == widget.Window.Root);
			}
		}
		
		protected virtual void DetachWidget(Widget widget)
		{
			this.Parent = null;
		}
		
		
		protected virtual int FindNextIndex(Widget widget)
		{
			Widget parent = widget.Parent;
			
			foreach (Widget item in this.black_list)
			{
				if (item.Parent == parent)
				{
					return item.TabIndex + 1;
				}
			}
			
			return this.default_index;
		}
		
		protected virtual void DefineTabIndex(Widget widget, int tab_index)
		{
			widget.TabIndex = tab_index;
			
			this.parent_filter = null;
			this.default_index = 1;
			
			if (widget is RadioButton)
			{
				RadioButton radio = widget as RadioButton;
				System.Collections.ArrayList list  = RadioButton.FindRadioChildren (radio.Parent, radio.Group);
				RadioButton[] radios = new RadioButton[list.Count];
				list.CopyTo (radios);
				
				for (int i = 0; i < radios.Length; i++)
				{
					if (radios[i] == radio)
					{
						continue;
					}
					
					if (this.black_list.Contains (radios[i]))
					{
						continue;
					}
					
					this.black_list.Insert (0, radios[i]);
					
					radios[i].TabIndex = tab_index;
				}
			}
			
			while (widget != this.root_widget)
			{
				if (this.black_list.Contains (widget))
				{
					break;
				}
				
				if (widget.TabNavigation != TabNavigationMode.Passive)
				{
					if (widget.TabIndex == 0)
					{
						//	Le parent du widget qui a reçu un nouveau numéro d'index
						//	n'a pas d'index défini; il faut donc lui en attribuer un :
						
						widget.TabIndex = this.FindNextIndex (widget);
					}
					
					this.black_list.Insert (0, widget);
				}
				
				widget = widget.Parent;
			}
		}
		
		
		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				//	TODO: compléter
			}
			
			base.Dispose (disposing);
		}
		
		
		protected override void ProcessMessage(Message message, Epsitec.Common.Drawing.Point pos)
		{
			if (this.IsFrozen)
			{
				message.Consumer = this;
				message.Swallowed = true;
				return;
			}
			
			if (message.IsMouseType)
			{
				Drawing.Rectangle clip = this.root_widget.GetClipStackBounds ();
				Widget            hot  = null;
				
				if (clip.Contains (pos))
				{
					Widget.ChildFindMode mode = Widget.ChildFindMode.SkipHidden
						/**/				  | Widget.ChildFindMode.SkipEmbedded
						/**/				  | Widget.ChildFindMode.Deep;
					
					hot = this.root_widget.FindChild (pos, mode);
				}
				
				if (hot != null)
				{
					if (this.CheckDisabled (hot))
					{
						hot = null;
					}
					else if (this.is_picker_active)
					{
						if (hot.TabNavigation == TabNavigationMode.Passive)
						{
							hot = null;
						}
					}
				}
				
				if (this.hot_widget != hot)
				{
					this.hot_widget = hot;
					this.Invalidate ();
				}
				
				if ((! this.is_picker_active) &&
					(this.is_setter_active))
				{
					if (this.hot_widget != null)
					{
						this.MouseCursor = MouseCursor.AsHand;
					}
					else
					{
						this.MouseCursor = null;
					}
				}
			}
			
			if (message.Type == MessageType.MouseDown)
			{
				if (this.hot_widget != null)
				{
					if (message.IsLeftButton)
					{
						if (this.is_picker_active)
						{
							int tab_index = this.hot_widget.TabIndex;
							
							if (tab_index > 0)
							{
								this.parent_filter = this.hot_widget.Parent;
								this.internal_set  = true;
								this.CommandDispatcher.Dispatch (string.Format ("StartTabIndexAtIndex({0})", tab_index), this);
								this.internal_set  = false;
							}
						}
						else if (this.is_setter_active)
						{
							this.DefineTabIndex (this.hot_widget, this.FindNextIndex (this.hot_widget));
							this.hot_widget = null;
							this.Invalidate ();
						}
					}
					else if (message.IsRightButton)
					{
						//	TODO: afficher un menu permettant de supprimer/modifier un élément
					}
				}
			}
			if (message.IsMouseType)
			{
				message.Consumer = this;
			}
		}

		protected override void PaintBackgroundImplementation(Drawing.Graphics graphics, Drawing.Rectangle clip_rect)
		{
			this.PaintTag (this.root_widget, null, graphics, clip_rect);
		}
		
		protected void PaintTag(Widget widget, string prefix, Drawing.Graphics graphics, Drawing.Rectangle clip_rect)
		{
			if (prefix == null)
			{
				//	On n'affiche pas de pastille pour la racine commune de tous les widgets.
				//	C'est forcément toujours l'unique...
				
				prefix = "";
			}
			else
			{
				Drawing.Rectangle bounds = widget.MapClientToRoot (widget.Client.Bounds);
				
				if (! clip_rect.IntersectsWith (bounds))
				{
					return;
				}
				
				TabNavigationMode mode = widget.TabNavigation;
			
				if (((mode != TabNavigationMode.Passive) && (widget.TabIndex > 0)) ||
					(this.hot_widget == widget))
				{
					string text = string.Format ("{0}{1}", prefix, widget.TabIndex);
					
					if ((widget == this.hot_widget) &&
						(! this.is_picker_active) &&
						(this.is_setter_active))
					{
						text = string.Format ("{0}", this.FindNextIndex (widget));
						
						Widget parent = widget.Parent;
						
						while (parent != this.root_widget)
						{
							if (parent == null) break;
							
							if (parent.TabNavigation != TabNavigationMode.Passive)
							{
								if (parent.TabIndex == 0)
								{
									text = string.Format ("{0}.{1}", this.FindNextIndex (parent), text);
								}
								else
								{
									text = string.Format ("{0}.{1}", parent.TabIndex, text);
								}
							}
							
							parent = parent.Parent;
						}
					}
					
					Drawing.Font font  = this.DefaultFont;
					double       size  = this.DefaultFontSize;
					double       below = System.Math.Ceiling (font.Descender * size);
					double       above = System.Math.Ceiling (font.Ascender * size);
					
					double x = bounds.Left + 3;
					double y = bounds.Top - above - 1;
					
					bool is_hot = widget == this.hot_widget;
					
					Drawing.Color color_1 = Drawing.Color.FromRGB (1, 1, 1);
					Drawing.Color color_2 = Drawing.Color.FromRGB (0, 0, 0.6);
					
					if (this.CheckDisabled (widget))
					{
						color_1 = Drawing.Color.FromRGB (0.8, 0.8, 0.8);
						color_2 = Drawing.Color.FromRGB (0.4, 0.4, 0.8);
					}
					
					graphics.Color = is_hot ? color_2 : color_1;
					
					double width = System.Math.Ceiling (graphics.PaintText (x+1, y-1, text, font, size));
					
					Drawing.Rectangle rect = new Drawing.Rectangle (x, y + below, width, above - below);
					
					rect.Inflate (2.5, 2.5, 0.5, 0.5);
					
					graphics.LineWidth = 1.0;
					graphics.Color = Drawing.Color.FromColor (is_hot ? color_2 : color_1, 0.8);
					graphics.PaintSurface (Drawing.Path.FromRectangle (rect));
					
					graphics.Color = is_hot ? color_1 : color_2;
					graphics.PaintOutline (Drawing.Path.FromRectangle (rect));
					graphics.PaintText (x, y, text, font, size);
				}
			
				if ((mode & TabNavigationMode.ForwardToChildren) != 0)
				{
					//	Les enfants de ce widget peuvent être atteints par une pression sur TAB.
					//	Il faut donc refléter cela au moyen d'un préfixe incluant le ID du widget
					//	actuel :
					
					prefix = string.Format ("{0}{1}.", prefix, widget.TabIndex);
				}
				else
				{
					prefix = "X.";
				}
			}
			
			if (widget.HasChildren)
			{
				foreach (Widget child in widget.Children)
				{
					this.PaintTag (child, prefix, graphics, clip_rect);
				}
			}
		}
		
		
		protected Widget						root_widget;
		protected Widget						hot_widget;
		protected Widget						parent_filter;
		protected System.Collections.ArrayList	black_list;
		protected bool							is_picker_active;
		protected bool							is_setter_active;
		protected bool							internal_set;
		protected int							default_index;
		
		static long								overlay_id;
	}
}
