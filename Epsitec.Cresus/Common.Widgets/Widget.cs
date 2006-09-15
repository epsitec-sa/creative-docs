//	Copyright © 2003-2006, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

using System.Collections.Generic;
using Epsitec.Common.Types;

[assembly: Epsitec.Common.Types.DependencyClass (typeof (Epsitec.Common.Widgets.Widget))]

namespace Epsitec.Common.Widgets
{
	
	
	public delegate bool WidgetWalkChildrenCallback(Widget widget);
	
	#region InternalState enum
	[System.Flags] public enum InternalState : uint
	{
		None				= 0,
		
		Disposing			= 0x00000001,
		Disposed			= 0x00000002,
		
		WasValid			= 0x00000004,
		
		Embedded			= 0x00000008,		//	=> widget appartient au parent (widgets composés)
		
		Focusable			= 0x00000010,
		Selectable			= 0x00000020,
		Engageable			= 0x00000040,		//	=> peut être enfoncé par une pression
		Frozen				= 0x00000080,		//	=> n'accepte aucun événement
		
		ExecCmdOnPressed	= 0x00001000,		//	=> exécute la commande quand on presse le widget
		
		AutoMnemonic		= 0x00100000,
		
		PossibleContainer	= 0x01000000,		//	widget peut être la cible d'un drag & drop en mode édition
		EditionEnabled		= 0x02000000,		//	widget peut être édité
		
		DebugActive			= 0x80000000		//	widget marqué pour le debug
	}
	#endregion
	
	
	/// <summary>
	/// La classe Widget implémente la classe de base dont dérivent tous les
	/// widgets de l'interface graphique ("controls" dans l'appellation Windows).
	/// </summary>
	public class Widget : Visual, Collections.IShortcutCollectionHost
	{
		public Widget()
		{
			if (Widget.DebugDispose)
			{
				System.Diagnostics.Debug.WriteLine (string.Format ("{1}+ Created {0}", this.GetType ().Name, this.VisualSerialId));
			}

			this.InternalState |= InternalState.WasValid;
			this.InternalState |= InternalState.AutoMnemonic;
			
			this.default_font_height = System.Math.Floor(this.DefaultFont.LineHeight*this.DefaultFontSize);
			
			lock (Widget.aliveWidgets)
			{
				Widget.aliveWidgets.Add (new System.WeakReference (this));
			}
		}

		public Widget(Widget embedder) : this()
		{
			this.SetEmbedder (embedder);
		}
		
		
		static Widget()
		{
			System.Diagnostics.Debug.WriteLine ("Initializing Widget infrastructure.");
			
			Helpers.FontPreviewer.Initialize ();
			
			Res.Initialise (typeof (Widget), "Common.Widgets");
			
			Support.ImageProvider.Initialize ();
			
			System.Threading.Thread          thread  = System.Threading.Thread.CurrentThread;
			System.Globalization.CultureInfo culture = thread.CurrentCulture;
			
			thread.CurrentUICulture = culture;
		}
		
		
		public static void Initialize()
		{
			//	En appelant cette méthode statique, on peut garantir que le constructeur
			//	statique de Widget a bien été exécuté.
		}
		
		
		#region IShortcutCollectionHost Members
		public void NotifyShortcutsChanged(Epsitec.Common.Widgets.Collections.ShortcutCollection collection)
		{
			if (collection.Count == 0)
			{
				this.shortcuts = null;
			}
			else
			{
				this.shortcuts = collection;
			}
			
			if (this.AutoMnemonic)
			{
				//	Supprime le flag 'auto mnemonic' sans altérer le raccourci,
				//	ce qui évite de générer un événement ShortcutChanged avant
				//	l'heure :
				
				this.internal_state &= ~InternalState.AutoMnemonic;
			}
			
			this.OnShortcutChanged ();
		}
		#endregion
		
		protected override void Dispose(bool disposing)
		{
			if (Widget.DebugDispose)
			{
				System.Diagnostics.Debug.WriteLine (string.Format ("{2}- {0} widget {1}", (disposing ? "Disposing" : "Collecting"), this.ToString (), this.VisualSerialId));
			}
			
			if (disposing)
			{
				this.internal_state |= InternalState.Disposing;
				
				if (this.HasChildren)
				{
					Widget[] widgets = this.Children.Widgets;
					
					for (int i = 0; i < widgets.Length; i++)
					{
						widgets[i].Dispose ();
						System.Diagnostics.Debug.Assert (widgets[i].Parent == null);
					}
					
					System.Diagnostics.Debug.Assert (this.Children.Count == 0);
				}
				
				this.SetParent (null);
				
				if (this.Disposed != null)
				{
					this.Disposed (this);
					this.Disposed = null;
				}
				
				this.internal_state |= InternalState.Disposed;
			}
		}
		
		
		#region Debugging Support
		public bool							DebugActive
		{
			get
			{
				return (this.internal_state & InternalState.DebugActive) != 0;
			}
			set
			{
				if (value)
				{
					this.internal_state |= InternalState.DebugActive;
				}
				else
				{
					this.internal_state &= ~ InternalState.DebugActive;
				}
			}
		}
		
		public static bool					DebugDispose
		{
			get
			{
				return Widget.debugDispose;
			}
			set
			{
				Widget.debugDispose = value;
			}
		}
		
		public static int					DebugAliveWidgetsCount
		{
			get
			{
				List<System.WeakReference> alive = new List<System.WeakReference> ();
				
				lock (Widget.aliveWidgets)
				{
					//	Passe en revue tous les widgets connus (même les décédés) et reconstruit
					//	une liste ne contenant que les widgets vivants :
					
					foreach (System.WeakReference weak_ref in Widget.aliveWidgets)
					{
						if (weak_ref.IsAlive)
						{
							alive.Add (weak_ref);
						}
					}
					
					//	Remplace la liste des widgets connus par la liste à jour qui vient d'être
					//	construite :
					
					Widget.aliveWidgets = alive;
				}
				
				return alive.Count;
			}
		}
		
		public static Widget[]				DebugAliveWidgets
		{
			get
			{
				System.Collections.ArrayList alive = new System.Collections.ArrayList ();
				
				lock (Widget.aliveWidgets)
				{
					foreach (System.WeakReference weak_ref in Widget.aliveWidgets)
					{
						if (weak_ref.IsAlive)
						{
							alive.Add (weak_ref.Target);
						}
					}
				}
				
				Widget[] widgets = new Widget[alive.Count];
				alive.CopyTo (widgets);
				
				return widgets;
			}
		}
		#endregion
		
		public MouseCursor							MouseCursor
		{
			get
			{
				return this.mouse_cursor == null ? MouseCursor.Default : this.mouse_cursor;
			}
			set
			{
				if (this.mouse_cursor != value)
				{
					this.mouse_cursor = value;
				}
				
				if (this.IsEntered)
				{
					this.Window.MouseCursor = this.MouseCursor;
				}
			}
		}
		
		public Drawing.Size							RealMinSize
		{
			get
			{
				double width  = this.MinWidth;
				double height = this.MinHeight;

				Layouts.LayoutMeasure measure_dx = Layouts.LayoutMeasure.GetWidth (this);
				Layouts.LayoutMeasure measure_dy = Layouts.LayoutMeasure.GetHeight (this);

				if (measure_dx != null)
				{
					width = System.Math.Max (width, measure_dx.Min);
				}
				if (measure_dy != null)
				{
					height = System.Math.Max (height, measure_dy.Min);
				}
				
				return new Drawing.Size (width, height);
			}
		}
		
		public Drawing.Size							RealMaxSize
		{
			get
			{
				double width  = this.MaxWidth;
				double height = this.MaxHeight;

				Layouts.LayoutMeasure measure_dx = Layouts.LayoutMeasure.GetWidth (this);
				Layouts.LayoutMeasure measure_dy = Layouts.LayoutMeasure.GetHeight (this);

				if (measure_dx != null)
				{
					width = System.Math.Min (width, measure_dx.Max);
				}
				if (measure_dy != null)
				{
					height = System.Math.Min (height, measure_dy.Max);
				}

				return new Drawing.Size (width, height);
			}
		}
		
		
		public virtual Drawing.Font					DefaultFont
		{
			get { return Drawing.Font.DefaultFont; }
		}
		
		public virtual double						DefaultFontSize
		{
			get { return Drawing.Font.DefaultFontSize; }
		}
		
		public static double						DefaultFontHeight
		{
			get { return 12.0; }
		}
		
		public virtual bool							IsFrozen
		{
			get
			{
				if ((this.internal_state & InternalState.Frozen) != 0)
				{
					return true;
				}
				if (this.Parent != null)
				{
					return this.Parent.IsFrozen;
				}
				
				return false;
			}
		}
		
		public bool									IsEmbedded
		{
			//	Un widget qui retourne IsEmbedded = true n'a pas besoin d'être sérialisé
			//	quand son parent est sérialisé, car il est construit et géré par le parent.
			//	Voir le constructeur Widget(Widget) et Widget.SetEmbedder.
			get
			{
				return (this.internal_state & InternalState.Embedded) != 0;
			}
		}
		
		public bool									IsEditionEnabled
		{
			get
			{
				if ((this.internal_state & InternalState.EditionEnabled) != 0)
				{
					return true;
				}
				
				if (this.Parent != null)
				{
					return this.Parent.IsEditionEnabled;
				}
				
				return false;
			}
			set
			{
				bool enabled = (this.internal_state & InternalState.EditionEnabled) != 0;
				
				if (enabled != value)
				{
					if (value)
					{
						this.internal_state |= InternalState.EditionEnabled;
					}
					else
					{
						this.internal_state &= ~InternalState.EditionEnabled;
					}
				}
			}
		}
		
		public bool									IsDisposing
		{
			get
			{
				return (this.internal_state & InternalState.Disposing) != 0;
			}
		}

		public bool									IsDisposed
		{
			get
			{
				return (this.internal_state & InternalState.Disposed) != 0;
			}
		}
		
		public bool									ExecuteCommandOnPressed
		{
			get
			{
				return (this.InternalState & InternalState.ExecCmdOnPressed) != 0;
			}
			set
			{
				if (value)
				{
					this.InternalState |= InternalState.ExecCmdOnPressed;
				}
				else
				{
					this.InternalState &= ~ InternalState.ExecCmdOnPressed;
				}
			}
		}
		
		
		public bool									AutoMnemonic
		{
			get
			{
				return (this.internal_state & InternalState.AutoMnemonic) != 0;
			}
			set
			{
				if (this.AutoMnemonic != value)
				{
					if (value)
					{
						this.internal_state |= InternalState.AutoMnemonic;
					}
					else
					{
						this.internal_state &= ~InternalState.AutoMnemonic;
					}
					
					this.ResetMnemonicShortcut ();
				}
			}
		}
		
		protected InternalState						InternalState
		{
			get
			{
				return this.internal_state;
			}
			set
			{
				this.internal_state = value;
			}
		}
		
		
		public WidgetPaintState						PaintState
		{
			get
			{
				WidgetPaintState state = WidgetPaintState.None;
				
				if (this.IsEntered)
				{
					state |= WidgetPaintState.Entered;
				}
				if (this.IsEngaged)
				{
					state |= WidgetPaintState.Engaged;
				}
				if (this.IsSelected)
				{
					state |= WidgetPaintState.Selected;
				}
				if (this.InError)
				{
					state |= WidgetPaintState.Error;
				}
				if (this.IsEnabled)
				{
					state |= WidgetPaintState.Enabled;
				}
				if (this.IsFocused)
				{
					state |= WidgetPaintState.Focused;
				}
				if (this.AcceptThreeState)
				{
					state |= WidgetPaintState.ThreeState;
				}
				switch (this.ActiveState)
				{
					case ActiveState.Yes:
						state |= WidgetPaintState.ActiveYes;
						break;

					case ActiveState.Maybe:
						state |= WidgetPaintState.ActiveMaybe;
						break;
				}
				
				return state;
			}
		}
		
		public int									ZOrder
		{
			get
			{
				//	Retourne la position dans la "pile" des widgets, en ne considérant que les
				//	frères et soeurs. 0 => widget sur le sommet de la pile des widgets.
				
				if (this.Parent == null)
				{
					return -1;
				}

				return this.Parent.Children.ZOrderOf (this);
			}
			set
			{
				if (this.Parent != null)
				{
					this.Parent.Children.ChangeZOrder (this, value);
				}
			}
		}
		
		
		public bool									CanSelect
		{
			get { return ((this.internal_state & InternalState.Selectable) != 0) && !this.IsFrozen; }
		}
		
		public bool									CanEngage
		{
			get { return ((this.internal_state & InternalState.Engageable) != 0) && this.IsEnabled && !this.IsFrozen; }
		}

		public virtual bool							AcceptsFocus
		{
			get
			{
				return ((this.internal_state & InternalState.Focusable) != 0)
					&& (!this.IsFrozen);
			}
		}
		public virtual bool							AcceptsDefocus
		{
			get
			{
				return true;
			}
		}
		
		public bool									PossibleContainer
		{
			get { return ((this.internal_state & InternalState.PossibleContainer) != 0) && !this.IsFrozen; }
		}
		
		
		public new Widget							Parent
		{
			get
			{
				return base.Parent as Widget;
			}
		}
		
		public Support.OpletQueue					OpletQueue
		{
			get
			{
				return Helpers.VisualTree.GetOpletQueue (this);
			}
			set
			{
				throw new System.InvalidOperationException ("Cannot set OpletQueue on Widget.");
			}
		}
		
		public virtual Support.ResourceManager		ResourceManager
		{
			get
			{
				if (this.resource_manager == null)
				{
					if (this.Parent != null)
					{
						return this.Parent.ResourceManager;
					}
					
//-					System.Diagnostics.Debug.WriteLine ("Falling back to default resource manager: " + this.ToString ());
					return Support.Resources.DefaultManager;
				}
				
				return this.resource_manager;
			}
			set
			{
				if (this.resource_manager != value)
				{
					this.resource_manager = value;
					
					if (this.text_layout != null)
					{
						this.text_layout.ResourceManager = value;
					}
					
					this.OnResourceManagerChanged ();
				}
			}
		}
		
		
		public Widget								RootParent
		{
			get
			{
				Widget widget = this;
				
				while (widget.Parent != null)
				{
					widget = widget.Parent;
				}
				
				return widget;
			}
		}
		
		public bool									IsEmpty
		{
			get { return this.HasChildren == false; }
		}
		
		public bool									HasParent
		{
			get { return this.Parent != null; }
		}
		
		public bool									HasSiblings
		{
			get
			{
				if ((this.Parent != null) &&
					(this.Parent.Children.Count > 1))
				{
					return true;
				}
				
				return false;
			}
		}
		
		public bool									HasDockedChildren
		{
			get
			{
				if (this.HasChildren)
				{
					return this.Children.DockLayoutCount > 0;
				}
				else
				{
					return false;
				}
			}
		}
		

		public string								FullPathName
		{
			get
			{
				System.Text.StringBuilder buffer = new System.Text.StringBuilder ();
				
				this.BuildFullPathName (buffer);
				
				return buffer.ToString ();
			}
		}
		
		public string								Text
		{
			get
			{
				return (string) this.GetValueBase (Widget.TextProperty) ?? "";
			}
			set
			{
				if (string.IsNullOrEmpty (value))
				{
					this.ClearValue (Widget.TextProperty);
					this.OnTextDefined ();
				}
				else
				{
					if (this.text_layout == null)
					{
						this.CreateTextLayout ();
					}
					
					this.ModifyText (value);
				}
			}
		}

		public virtual TextLayout					TextLayout
		{
			get { return this.text_layout; }
		}
		
		public Drawing.TextBreakMode				TextBreakMode
		{
			get
			{
				if (this.text_layout != null)
				{
					return this.text_layout.BreakMode;
				}
				
				return Drawing.TextBreakMode.None;
			}
			set
			{
				if (this.text_layout == null)
				{
					this.CreateTextLayout ();
				}
				
				if (this.text_layout.BreakMode != value)
				{
					this.text_layout.BreakMode = value;
					this.Invalidate ();
				}
			}
		}

		public string								IconName
		{
			get
			{
				return (string) this.GetValue (Widget.IconNameProperty);
			}
			set
			{
				if (string.IsNullOrEmpty (value))
				{
					this.ClearValue (Widget.IconNameProperty);
				}
				else
				{
					this.SetValue (Widget.IconNameProperty, value);
				}
			}
		}
		
		public char									Mnemonic
		{
			get
			{
				if (this.AutoMnemonic)
				{
					//	Le code mnémonique est encapsulé par des tags <m>..</m>.
					
					return TextLayout.ExtractMnemonic (this.Text);
				}
				
				return (char) 0;
			}
		}
		
		public int									TabIndex
		{
			get
			{
				return (int) this.GetValue (Widget.TabIndexProperty);
			}
			set
			{
				if (this.TabIndex != value)
				{
					this.SetValue (Widget.TabIndexProperty, value);
					
					if ((this.tab_navigation_mode == TabNavigationMode.Passive) &&
						(value > 0))
					{
						this.tab_navigation_mode = TabNavigationMode.ActivateOnTab;
					}
					else if ((this.tab_navigation_mode == TabNavigationMode.ActivateOnTab) &&
						/**/ (value <= 0))
					{
						this.tab_navigation_mode = TabNavigationMode.Passive;
					}
				}
			}
		}
		
		public virtual TabNavigationMode			TabNavigation
		{
			get
			{
				return this.tab_navigation_mode;
			}
			set
			{
				this.tab_navigation_mode = value;
			}
		}
		
		public Collections.ShortcutCollection		Shortcuts
		{
			get
			{
				if (this.shortcuts == null)
				{
					return new Collections.HostedShortcutCollection (this);
				}
				else
				{
					return this.shortcuts;
				}
			}
		}
		
		public string								Hypertext
		{
			get
			{
				if (this.hypertext == null)
				{
					return null;
				}
				
				return this.hypertext.Anchor;
			}
		}
		
		
		public virtual Drawing.Size GetBestFitSize()
		{
			Types.DependencyPropertyMetadata metadataDx = Visual.PreferredWidthProperty.GetMetadata (this);
			Types.DependencyPropertyMetadata metadataDy = Visual.PreferredHeightProperty.GetMetadata (this);

			return new Drawing.Size ((double) metadataDx.DefaultValue, (double) metadataDy.DefaultValue);
		}
		
		public virtual void Hide()
		{
			this.Visibility = false;
		}
		
		public virtual void Show()
		{
			this.Visibility = true;
		}
		
		public virtual void Toggle()
		{
			if ((this.AutoRadio) &&
				(this.ActiveState == ActiveState.Yes))
			{
				return;
			}
			
			if (this.AcceptThreeState)
			{
				switch (this.ActiveState)
				{
					case ActiveState.Yes:
						this.ActiveState = ActiveState.Maybe;
						break;
					case ActiveState.Maybe:
						this.ActiveState = ActiveState.No;
						break;
					case ActiveState.No:
						this.ActiveState = ActiveState.Yes;
						break;
				}
			}
			else
			{
				switch (this.ActiveState)
				{
					case ActiveState.Yes:
					case ActiveState.Maybe:
						this.ActiveState = ActiveState.No;
						break;
					case ActiveState.No:
						this.ActiveState = ActiveState.Yes;
						break;
				}
			}
		}

		
		internal void AsyncValidation()
		{
			Window window = this.Window;
			
			if (window != null)
			{
				window.AsyncValidation (this);
			}
		}
		
		public virtual void Validate()
		{
			bool oldValid = (this.internal_state & InternalState.WasValid) != 0;
			bool newValid = this.IsValid;

			if (oldValid != newValid)
			{
				if (newValid)
				{
					this.internal_state |= InternalState.WasValid;
				}
				else
				{
					this.internal_state &= ~InternalState.WasValid;
				}

				this.InvalidateProperty (Visual.IsValidProperty, oldValid, newValid);
				
				this.SetError (newValid == false);
				
				if (this.HasValidationGroups)
				{
					ValidationContext context = Helpers.VisualTree.GetValidationContext (this);

					if (context != null)
					{
						context.UpdateCommandEnableBasedOnVisualValidity (this);
					}
				}
			}
		}
		
		
		internal void AddValidator(IValidator value)
		{
			this.SetValue (Visual.ValidatorProperty, MulticastValidator.Combine (this.Validator, value));
			this.OnValidatorChanged ();
		}
		
		internal void RemoveValidator(IValidator value)
		{
			if (this.Validator != null)
			{
				IValidator validator = this.Validator;
				MulticastValidator mv = validator as MulticastValidator;
				
				if (mv != null)
				{
					validator = MulticastValidator.Remove (mv, value);
				}
				else if (validator == value)
				{
					validator = null;
				}

				if (validator == null)
				{
					this.ClearValue (Visual.ValidatorProperty);
				}
				else
				{
					this.SetValue (Visual.ValidatorProperty, validator);
				}
				
				this.OnValidatorChanged ();
			}
		}
		
		
		public void SetParent(Widget widget)
		{
			Widget oldParent = this.Parent;
			Widget newParent = widget;
			
			if (newParent != oldParent)
			{
				if (newParent == null)
				{
					oldParent.Children.Remove (this);
				}
				else
				{
					newParent.Children.Add (this);
				}
				
				System.Diagnostics.Debug.Assert (this.Parent == newParent);
			}
		}

		public virtual void SetFrozen(bool frozen)
		{
			if ((this.internal_state & InternalState.Frozen) == 0)
			{
				if (frozen)
				{
					this.internal_state |= InternalState.Frozen;
					this.Invalidate (InvalidateReason.FrozenChanged);
				}
			}
			else
			{
				if (!frozen)
				{
					this.internal_state &= ~ InternalState.Frozen;
					this.Invalidate (InvalidateReason.FrozenChanged);
				}
			}
		}
		
		public virtual void SetError(bool value)
		{
			if (this.InError != value)
			{
				this.SetValue (Visual.InErrorProperty, value);
			}
		}
		
		public void Focus()
		{
			Window window = this.Window;
			
			if (window != null)
			{
				window.FocusWidget (this);
			}
		}
		
		internal void SetFocused(bool focused)
		{
			//	Utiliser Focus() en lieu et place de SetFocused(true), pour
			//	avoir une gestion complète des conditions de focus.
			
			bool old_focus = this.KeyboardFocus;
			bool new_focus = focused;
			
			if (old_focus == new_focus)
			{
				return;
			}
			
			Window window = this.Window;

			if (new_focus)
			{
				if (window != null)
				{
					this.SetValue (Visual.KeyboardFocusProperty, true);
					window.FocusedWidget = this;
				}
				
				this.Invalidate (InvalidateReason.FocusedChanged);
			}
			else
			{
				this.SetValue (Visual.KeyboardFocusProperty, false);
				this.ClearValue (Visual.KeyboardFocusProperty);
				
				if (window != null)
				{
					window.FocusedWidget = null;
				}
				
				this.Invalidate (InvalidateReason.FocusedChanged);
			}
		}
		
		public void SetSelected(bool value)
		{
			if (this.IsSelected != value)
			{
				this.SetValue (Visual.SelectedProperty, value);
				
				if (value)
				{
					this.OnSelected ();
				}
				else
				{
					this.OnDeselected ();
				}
			}
		}
		
		public void SetEngaged(bool value)
		{
			Window window = this.Window;
			
			if (window == null)
			{
				return;
			}
			
			if ((this.internal_state & InternalState.Engageable) == 0)
			{
				return;
			}

			if (this.IsFrozen)
			{
				return;
			}
			
			if (this.IsEngaged != value)
			{
				this.SetValue (Visual.EngagedProperty, value);
				
				if (value)
				{
					window.EngagedWidget = this;
					this.OnEngaged ();
				}
				else
				{
					window.EngagedWidget = null;
					this.OnDisengaged ();
				}
			}
		}
		
		
		internal void FireStillEngaged()
		{
			if (this.IsEngaged)
			{
				this.OnStillEngaged ();
			}
		}
		
		internal void SimulatePressed()
		{
			if (this.IsEnabled)
			{
				this.OnPressed (null);
			}
		}
		
		internal void SimulateReleased()
		{
			if (this.IsEnabled)
			{
				this.OnReleased (null);
			}
		}
		
		internal void SimulateClicked()
		{
			if (this.IsEnabled)
			{
				this.OnClicked (null);
			}
		}
		
		
		public void SetEmbedder(Widget embedder)
		{
			this.SetParent (embedder);
			this.internal_state |= InternalState.Embedded;
		}
		
		
		public override string ToString()
		{
			System.Text.StringBuilder buffer = new System.Text.StringBuilder ();
			
			buffer.Append (this.VisualSerialId.ToString ());
			buffer.Append (":");
			buffer.Append (this.GetType ().Name);
			this.BuildFullPathName (buffer);
			buffer.Append (@".""");
			buffer.Append (this.Text);
			buffer.Append (@"""");
			
			return buffer.ToString ();
		}
		
		
		protected void SetEntered(bool value)
		{
			if (this.IsEntered != value)
			{
				Window window = this.Window;
				Message message = null;
				
				if (value)
				{
					Widget.ExitWidgetsNotParentOf (this);
					Widget.enteredWidgets.Add (this);

					this.SetValue (Visual.EnteredProperty, value);
					
					if ((this.Parent != null) &&
						(this.Parent.IsEntered == false) &&
						(!(this.Parent is WindowRoot)))
					{
						this.Parent.SetEntered (true);
					}
					
					message = Message.CreateDummyMessage (MessageType.MouseEnter);
					
					this.OnEntered (new MessageEventArgs (message, Message.CurrentState.LastPosition));
				}
				else
				{
					Widget.enteredWidgets.Remove (this);

					this.SetValue (Visual.EnteredProperty, value);
					
					//	Il faut aussi supprimer les éventuels enfants encore marqués comme 'entered'.
					//	Pour ce faire, on passe en revue tous les widgets à la recherche d'enfants
					//	directs.
					
					int i = 0;
					
					while (i < Widget.enteredWidgets.Count)
					{
						Widget candidate = Widget.enteredWidgets[i];
						
						if (candidate.Parent == this)
						{
							candidate.SetEntered (false);
							
							//	Note: le fait de changer l'état de l'enfant va modifier la liste des
							//	widgets sur laquelle on est en train d'itérer. On reprend donc, par
							//	précaution, l'itération au début...
							
							i = 0;
						}
						else
						{
							i++;
						}
					}

					message = Message.CreateDummyMessage (MessageType.MouseLeave);
					
					this.OnExited (new MessageEventArgs (message, Message.CurrentState.LastPosition));
				}

				this.MessageHandler (message);
				
				if (window != null)
				{
					window.PostProcessMessage (message);
				}
			}
		}
		
		protected static void ExitWidgetsNotParentOf(Widget widget)
		{
			int i = 0;
			
			while (i < Widget.enteredWidgets.Count)
			{
				Widget candidate = Widget.enteredWidgets[i];
				
				if (Helpers.VisualTree.IsAncestor (widget, candidate) == false)
				{
					//	Ce candidat n'est pas un ancêtre (parent direct ou indirect) du widget
					//	considéré; il faut donc changer son état Entered pour refléter le fait
					//	que le candidat n'a plus la souris :
					
					candidate.SetEntered (false);
					
					//	Note: le fait de changer l'état du candidat va modifier la liste des
					//	widgets sur laquelle on est en train d'itérer. On reprend donc, par
					//	précaution, l'itération au début...
					
					i = 0;
				}
				else
				{
					i++;
				}
			}
		}
		
		
		public static void UpdateEntered(Window window, Message message)
		{
			int index = Widget.enteredWidgets.Count;
			
			while (index > 0)
			{
				index--;
				
				if (index < Widget.enteredWidgets.Count)
				{
					Widget.UpdateEntered (window, Widget.enteredWidgets[index], message);
				}
			}
		}
		
		public static void UpdateEntered(Window window, Widget widget, Message message)
		{
			Drawing.Point point_in_widget = widget.MapRootToClient (message.Cursor);
			
			if ((widget.Window != window) ||
				(point_in_widget.X < 0) ||
				(point_in_widget.Y < 0) ||
				(point_in_widget.X >= widget.Client.Size.Width) ||
				(point_in_widget.Y >= widget.Client.Size.Height) ||
				(message.Type == MessageType.MouseLeave))
			{
				widget.SetEntered (false);
			}
			else
			{
				widget.SetEntered (true);
			}
		}
		
		
		public virtual bool HitTest(Drawing.Point point)
		{
			return this.ActualBounds.Contains (point);
		}
		
		
		public virtual Drawing.Rectangle GetClipBounds()
		{
			return this.GetShapeBounds ();
		}
		
		public virtual Drawing.Rectangle GetClipStackBounds()
		{
			//	Calcule le rectangle de clipping (relatif au widget) en tenant compte des
			//	rectangles de clipping de tous les parents.
			
			Drawing.Rectangle clip = this.GetClipBounds ();
			
			if (this.Parent != null)
			{
				clip = Drawing.Rectangle.Intersection (clip, this.MapParentToClient (this.Parent.GetClipStackBounds ()));
			}
			
			return clip;
		}
		
		public override void InvalidateRectangle(Drawing.Rectangle rect, bool sync)
		{
			if (this.Parent != null)
			{
				if (sync)
				{
					Window window = this.Window;
					
					if (window != null)
					{
						if (window.IsSyncPaintDisabled)
						{
							this.Parent.Invalidate (this.MapClientToParent (rect));
						}
						else
						{
							window.SynchronousRepaint ();
							
							window.PaintFilter = new Helpers.WidgetSyncPaintFilter (this);
							window.MarkForRepaint (this.MapClientToRoot (rect));
							window.SynchronousRepaint ();
							window.PaintFilter = null;
						}
					}
				}
				else
				{
					this.Parent.Invalidate (this.MapClientToParent (rect));
				}
			}
		}
		
		public virtual void Invalidate(InvalidateReason reason)
		{
			this.Invalidate ();
		}
		
		
		public virtual Drawing.Point MapParentToClient(Drawing.Point point)
		{
			if (this.IsDisposing)
			{
				return point;
			}
			else if (this.Parent == null)
			{
				return point;
			}
			else
			{
				Drawing.Point result = new Drawing.Point ();

				result.X = point.X - this.ActualLocation.X;
				result.Y = point.Y - this.ActualLocation.Y;

				return result;
			}
		}
		
		public virtual Drawing.Point MapClientToParent(Drawing.Point point)
		{
			if (this.IsDisposing)
			{
				return point;
			}
			else if (this.Parent == null)
			{
				return point;
			}
			else
			{
				Drawing.Point result = new Drawing.Point ();
				
				result.X = point.X + this.ActualLocation.X;
				result.Y = point.Y + this.ActualLocation.Y;

				return result;
			}
		}
		
		public virtual Drawing.Point MapRootToClient(Drawing.Point point)
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
		
		public virtual Drawing.Point MapClientToRoot(Drawing.Point point)
		{
			//	Transforme des coordonnées client d'un widget en coordonnées relatives à la
			//	racine de la fenêtre. Le point inférieur gauche d'un widget, en coordonnées
			//	client, est en principe [0;0].
			if (this.IsDisposing)
			{
				return point;
			}
			else
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
		}
		
		
		public virtual Drawing.Point MapScreenToClient(Drawing.Point point)
		{
			point = this.Window.MapScreenToWindow (point);
			point = this.MapRootToClient (point);
			return point;
		}
		
		public virtual Drawing.Point MapScreenToParent(Drawing.Point point)
		{
			point = this.Window.MapScreenToWindow (point);
			point = this.MapRootToClient (point);
			point = this.MapClientToParent (point);
			return point;
		}
		
		public virtual Drawing.Point MapClientToScreen(Drawing.Point point)
		{
			point = this.MapClientToRoot (point);
			Drawing.Point point_wdo = point;
			point = this.Window.MapWindowToScreen (point);
			return point;
		}
		
		public virtual Drawing.Point MapParentToScreen(Drawing.Point point)
		{
			point = this.MapParentToClient (point);
			point = this.MapClientToRoot (point);
			point = this.Window.MapWindowToScreen (point);
			return point;
		}
		
		
		public virtual Drawing.Rectangle MapRootToClient(Drawing.Rectangle rect)
		{
			bool flip_x = rect.Width < 0;
			bool flip_y = rect.Height < 0;
			
			Drawing.Point p1 = this.MapRootToClient (new Drawing.Point (rect.Left, rect.Bottom));
			Drawing.Point p2 = this.MapRootToClient (new Drawing.Point (rect.Right, rect.Top));
			
			rect.X = System.Math.Min (p1.X, p2.X);
			rect.Y = System.Math.Min (p1.Y, p2.Y);
			
			rect.Width  = System.Math.Abs (p1.X - p2.X);
			rect.Height = System.Math.Abs (p1.Y - p2.Y);
			
			if (flip_x) rect.FlipX ();
			if (flip_y) rect.FlipY ();
			
			return rect;
		}
		
		public virtual Drawing.Rectangle MapClientToRoot(Drawing.Rectangle rect)
		{
			//	Transforme des coordonnées client d'un widget en coordonnées relatives à la
			//	racine de la fenêtre. Le point inférieur gauche d'un widget, en coordonnées
			//	client, est en principe [0;0].
			//	Pour obtenir les "bounds" d'un widget, il faut donc convertir [0;0;width;height]
			//	comme ceci:
			//		widget.MapClientToRoot(widget.Client.Bounds);
			bool flip_x = rect.Width < 0;
			bool flip_y = rect.Height < 0;
			
			Drawing.Point p1 = this.MapClientToRoot (new Drawing.Point (rect.Left, rect.Bottom));
			Drawing.Point p2 = this.MapClientToRoot (new Drawing.Point (rect.Right, rect.Top));
			
			rect.X = System.Math.Min (p1.X, p2.X);
			rect.Y = System.Math.Min (p1.Y, p2.Y);
			
			rect.Width  = System.Math.Abs (p1.X - p2.X);
			rect.Height = System.Math.Abs (p1.Y - p2.Y);
			
			if (flip_x) rect.FlipX ();
			if (flip_y) rect.FlipY ();
			
			return rect;
		}
		
		
		public virtual Drawing.Rectangle MapParentToClient(Drawing.Rectangle rect)
		{
			bool flip_x = rect.Width < 0;
			bool flip_y = rect.Height < 0;
			
			Drawing.Point p1 = this.MapParentToClient (new Drawing.Point (rect.Left, rect.Bottom));
			Drawing.Point p2 = this.MapParentToClient (new Drawing.Point (rect.Right, rect.Top));
			
			rect.X = System.Math.Min (p1.X, p2.X);
			rect.Y = System.Math.Min (p1.Y, p2.Y);
			
			rect.Width  = System.Math.Abs (p1.X - p2.X);
			rect.Height = System.Math.Abs (p1.Y - p2.Y);
			
			if (flip_x) rect.FlipX ();
			if (flip_y) rect.FlipY ();
			
			return rect;
		}
		
		public virtual Drawing.Rectangle MapClientToParent(Drawing.Rectangle rect)
		{
			bool flip_x = rect.Width < 0;
			bool flip_y = rect.Height < 0;
			
			Drawing.Point p1 = this.MapClientToParent (new Drawing.Point (rect.Left, rect.Bottom));
			Drawing.Point p2 = this.MapClientToParent (new Drawing.Point (rect.Right, rect.Top));
			
			rect.X = System.Math.Min (p1.X, p2.X);
			rect.Y = System.Math.Min (p1.Y, p2.Y);
			
			rect.Width  = System.Math.Abs (p1.X - p2.X);
			rect.Height = System.Math.Abs (p1.Y - p2.Y);
			
			if (flip_x) rect.FlipX ();
			if (flip_y) rect.FlipY ();
			
			return rect;
		}

		public virtual Drawing.Rectangle MapScreenToClient(Drawing.Rectangle rect)
		{
			bool flip_x = rect.Width < 0;
			bool flip_y = rect.Height < 0;
			
			Drawing.Point p1 = this.MapScreenToClient (new Drawing.Point (rect.Left, rect.Bottom));
			Drawing.Point p2 = this.MapScreenToClient (new Drawing.Point (rect.Right, rect.Top));
			
			rect.X = System.Math.Min (p1.X, p2.X);
			rect.Y = System.Math.Min (p1.Y, p2.Y);
			
			rect.Width  = System.Math.Abs (p1.X - p2.X);
			rect.Height = System.Math.Abs (p1.Y - p2.Y);
			
			if (flip_x) rect.FlipX ();
			if (flip_y) rect.FlipY ();
			
			return rect;
		}
		
		public virtual Drawing.Rectangle MapClientToScreen(Drawing.Rectangle rect)
		{
			bool flip_x = rect.Width < 0;
			bool flip_y = rect.Height < 0;
			
			Drawing.Point p1 = this.MapClientToScreen (new Drawing.Point (rect.Left, rect.Bottom));
			Drawing.Point p2 = this.MapClientToScreen (new Drawing.Point (rect.Right, rect.Top));
			
			rect.X = System.Math.Min (p1.X, p2.X);
			rect.Y = System.Math.Min (p1.Y, p2.Y);
			
			rect.Width  = System.Math.Abs (p1.X - p2.X);
			rect.Height = System.Math.Abs (p1.Y - p2.Y);
			
			if (flip_x) rect.FlipX ();
			if (flip_y) rect.FlipY ();
			
			return rect;
		}

		
		public virtual Drawing.Size MapParentToClient(Drawing.Size size)
		{
			if (this.IsDisposing)
			{
				return size;
			}
			else
			{
				Drawing.Size result = new Drawing.Size ();

				result.Width  = size.Width;
				result.Height = size.Height;

				return result;
			}
		}
		
		public virtual Drawing.Size MapClientToParent(Drawing.Size size)
		{
			if (this.IsDisposing)
			{
				return size;
			}
			else
			{
				Drawing.Size result = new Drawing.Size ();

				result.Width  = size.Width;
				result.Height = size.Height;

				return result;
			}
		}


		public override Drawing.Point GetBaseLine(double width, double height, out double ascender, out double descender)
		{
			if (this.TextLayout != null)
			{
				Drawing.Point origin;
				Drawing.Size size;

				this.TextLayout.GetSingleLineGeometry (new Drawing.Size (width, height), this.ContentAlignment, out ascender, out descender, out origin, out size);

				double offset = this.GetBaseLineVerticalOffset ();

				origin.Y += offset;
				
				descender = -origin.Y;
				ascender  = height - origin.Y;
				
				return origin;
			}

			return base.GetBaseLine (width, height, out ascender, out descender);
		}

		protected virtual double GetBaseLineVerticalOffset()
		{
			return 0;
		}

		
		public static void ObsoleteBaseLineAlign(Widget model, Widget widget)
		{
			if ((model == null) ||
				(widget == null))
			{
				return;
			}
			
			double model_offset  = model.GetBaseLine ().Y;
			double widget_offset = widget.GetBaseLine ().Y;
			
			double y_bottom = model.ActualLocation.Y + model_offset - widget_offset;
			
			widget.SetManualBounds(new Drawing.Rectangle (widget.ActualLocation.X, y_bottom, widget.ActualWidth, widget.ActualHeight));
		}
		
		
		public Widget FindChild(Drawing.Point point)
		{
			return this.FindChild (point, null, ChildFindMode.SkipHidden);
		}

		public Widget FindChild(Drawing.Point point, IEnumerable<Widget> ignore)
		{
			return this.FindChild (point, ignore, ChildFindMode.SkipHidden);
		}

		public Widget FindChild(Drawing.Point point, ChildFindMode mode)
		{
			return this.FindChild (point, null, mode);
		}

		public virtual Widget FindChild(Drawing.Point point, IEnumerable<Widget> ignore, ChildFindMode mode)
		{
			if (this.HasChildren == false)
			{
				return null;
			}
			
			Widget[] children = this.Children.Widgets;
			int  children_num = children.Length;
			
			for (int i = 0; i < children_num; i++)
			{
				Widget widget = children[children_num-1 - i];
				
				System.Diagnostics.Debug.Assert (widget != null);
				
				if ((mode & ChildFindMode.SkipMask) != ChildFindMode.All)
				{
					if ((mode & ChildFindMode.SkipDisabled) != 0)
					{
						if (widget.IsEnabled == false)
						{
							continue;
						}
					}
					if ((mode & ChildFindMode.SkipHidden) != 0)
					{
						if (widget.Visibility == false)
						{
							continue;
						}
					}
					if ((mode & ChildFindMode.SkipNonContainer) != 0)
					{
						if (widget.PossibleContainer == false)
						{
							continue;
						}
					}
				}
				
				if (Widget.IsInIgnoreList (widget, ignore))
				{
					continue;
				}

				if (widget.HitTest (point))
				{
					if ((mode & ChildFindMode.SkipTransparent) != 0)
					{
						//	TODO: vérifier que le point en question n'est pas transparent
					}
					
					if ((mode & ChildFindMode.Deep) != 0)
					{
						//	Si on fait une recherche en profondeur, on regarde si le point correspond à
						//	un descendant du widget trouvé...
						
						Widget deep = widget.FindChild (widget.MapParentToClient (point), ignore, mode);
						
						//	Si oui, pas de test supplémentaire: on s'arrête et on retourne le widget
						//	terminal trouvé lors de la descente récursive :
						
						if (deep != null)
						{
							return deep;
						}
					}
					
					if ((mode & ChildFindMode.SkipEmbedded) != 0)
					{
						//	Si l'appelant a demandé de sauter les widgets spéciaux, marqués comme étant
						//	"embedded" dans un parent, on vérifie que l'on ne retourne pas un tel widget.
						//	Ce test doit se faire en dernier, parce qu'une descente récursive dans un
						//	widget "embedded" peut éventuellement donner des résultats positifs :
						
						if (widget.IsEmbedded)
						{
							continue;
						}
					}
					
					return widget;
				}
			}
			
			return null;
		}

		public Widget FindChild(Drawing.Rectangle rect, ChildFindMode mode)
		{
			return this.FindChild (rect, null, mode);
		}
		
		public virtual Widget FindChild(Drawing.Rectangle rect, IEnumerable<Widget> ignore, ChildFindMode mode)
		{
			if (this.HasChildren == false)
			{
				return null;
			}
			
			Widget[] children = this.Children.Widgets;
			int  children_num = children.Length;
			
			for (int i = 0; i < children_num; i++)
			{
				Widget widget = children[children_num-1 - i];
				
				System.Diagnostics.Debug.Assert (widget != null);
				
				if ((mode & ChildFindMode.SkipMask) != ChildFindMode.All)
				{
					if ((mode & ChildFindMode.SkipDisabled) != 0)
					{
						if (widget.IsEnabled == false)
						{
							continue;
						}
					}
					if ((mode & ChildFindMode.SkipHidden) != 0)
					{
						if (widget.Visibility == false)
						{
							continue;
						}
					}
					if ((mode & ChildFindMode.SkipNonContainer) != 0)
					{
						if (widget.PossibleContainer == false)
						{
							continue;
						}
					}
				}

				if (Widget.IsInIgnoreList (widget, ignore))
				{
					continue;
				}

				if (widget.HitTest (rect.BottomLeft) && widget.HitTest (rect.TopRight))
				{
					if ((mode & ChildFindMode.SkipTransparent) != 0)
					{
						//	TODO: vérifier que le point en question n'est pas transparent
					}
					
					if ((mode & ChildFindMode.Deep) != 0)
					{
						//	Si on fait une recherche en profondeur, on regarde si le point correspond à
						//	un descendant du widget trouvé...
						
						Widget deep = widget.FindChild (widget.MapParentToClient (rect), mode);
						
						//	Si oui, pas de test supplémentaire: on s'arrête et on retourne le widget
						//	terminal trouvé lors de la descente récursive :
						
						if (deep != null)
						{
							return deep;
						}
					}
					
					if ((mode & ChildFindMode.SkipEmbedded) != 0)
					{
						//	Si l'appelant a demandé de sauter les widgets spéciaux, marqués comme étant
						//	"embedded" dans un parent, on vérifie que l'on ne retourne pas un tel widget.
						//	Ce test doit se faire en dernier, parce qu'une descente récursive dans un
						//	widget "embedded" peut éventuellement donner des résultats positifs :
						
						if (widget.IsEmbedded)
						{
							continue;
						}
					}
					
					return widget;
				}
			}
			
			return null;
		}
		
		public Widget			FindChild(string name)
		{
			return this.FindChild (name, ChildFindMode.All);
		}
		
		public virtual Widget	FindChild(string name, ChildFindMode mode)
		{
			if (this.HasChildren == false)
			{
				return null;
			}
			
			Widget[] children = this.Children.Widgets;
			int  children_num = children.Length;
			
			for (int i = 0; i < children_num; i++)
			{
				Widget widget = children[i];
				
				System.Diagnostics.Debug.Assert (widget != null);
				
				if ((mode & ChildFindMode.SkipMask) != ChildFindMode.All)
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
						if (widget.Visibility == false)
						{
							continue;
						}
					}
					else if ((mode & ChildFindMode.SkipNonContainer) != 0)
					{
						if (widget.PossibleContainer == false)
						{
							continue;
						}
					}
				}
				
				if ((widget.Name == null) ||
					(widget.Name.Length == 0))
				{
					Widget child = widget.FindChild (name, mode);
					
					if (child != null)
					{
						return child;
					}
				}
				else if (widget.Name == name)
				{
					return widget;
				}
			}
			
			return null;
		}
		
		public virtual Widget	FindChildByPath(string[] names, int offset)
		{
			if (offset >= names.Length)
			{
				return this;
			}
			
			if (this.HasChildren == false)
			{
				return null;
			}
			
			Widget[] children = this.Children.Widgets;
			int  children_num = children.Length;
			
			for (int i = 0; i < children_num; i++)
			{
				Widget widget = children[i];
				
				System.Diagnostics.Debug.Assert (widget != null);
				
				if ((widget.Name == null) ||
					(widget.Name.Length == 0))
				{
					Widget child = widget.FindChildByPath (names, offset);
					
					if (child != null)
					{
						return child;
					}
				}
				else if (widget.Name == names[offset])
				{
					return widget.FindChildByPath (names, offset+1);
				}
			}
			
			return null;
		}
		
		public Widget			FindChildByPath(string path)
		{
			string[] names = path.Split ('.');
			
			if ((this.Name == null) ||
				(this.Name.Length == 0))
			{
				return this.FindChildByPath (names, 0);
			}
			if (this.Name == names[0])
			{
				return this.FindChildByPath (names, 1);
			}
			
			return null;
		}
		
		public Widget	        FindCommandWidget(string command)
		{
			//	Passe en revue tous les widgets de la descendance et retourne le
			//	premier qui correspond parfaitement.
			
			CommandWidgetFinder finder = new CommandWidgetFinder (command);
			
			this.WalkChildren (new WidgetWalkChildrenCallback (finder.Analyse));
			
			if (finder.Widgets.Length > 0)
			{
				return finder.Widgets[0];
			}
			
			return null;
		}
		
		public Widget[]         FindAllChildren()
		{
			System.Collections.ArrayList list = new System.Collections.ArrayList ();
			this.FindAllChildren (list);
			
			Widget[] widgets = new Widget[list.Count];
			list.CopyTo (widgets);
			
			return widgets;
		}
		
		public Widget			FindFocusedChild()
		{
			Window window = this.Window;
			Widget focused = (window == null) ? null : window.FocusedWidget;
			
			if (focused != null)
			{
				//	Il y a un widget avec le focus. Ca peut être nous, un de nos descendants
				//	ou un autre widget sans aucun lien.
				
				if (this.KeyboardFocus)
				{
					System.Diagnostics.Debug.Assert (this == focused);
					
					return this;
				}
				
				if (Helpers.VisualTree.IsDescendant (this, focused))
				{
					return focused;
				}
			}
			
			return null;
		}
		
		protected static bool IsInIgnoreList(Widget widget, IEnumerable<Widget> ignore)
		{
			if (ignore == null)
			{
				return false;
			}
			
			foreach (Widget item in ignore)
			{
				if (widget == item)
				{
					return true;
				}
			}
			
			return false;
		}
		
		protected virtual void FindAllChildren(System.Collections.ArrayList list)
		{
			foreach (Widget child in this.Children)
			{
				list.Add (child);
				child.FindAllChildren (list);
			}
		}
		
#if false //#fix
		public static Widget[]	FindAllCommandWidgets(string command)
		{
			return Widget.FindAllCommandWidgets (command, null);
		}
		
		public static Widget[]	FindAllCommandWidgets(System.Text.RegularExpressions.Regex regex)
		{
			return Widget.FindAllCommandWidgets (regex, null);
		}
		
		public static Widget[]	FindAllCommandWidgets(string command, CommandDispatcher dispatcher)
		{
			//	Passe en revue absolument tous les widgets qui existent et cherche ceux qui ont
			//	une commande qui correspond au critère spécifié.
			
			System.Collections.ArrayList list = new System.Collections.ArrayList ();
			System.Collections.ArrayList dead = new System.Collections.ArrayList ();
			
			lock (Widget.alive_widgets)
			{
				foreach (System.WeakReference weak_ref in Widget.alive_widgets)
				{
					//	On utilise la liste des widgets connus qui permet d'avoir accès immédiatement
					//	à tous les widgets sans nécessiter de descente récursive :
					
					if (weak_ref.IsAlive)
					{
						//	Le widget trouvé existe (encore) :
						
						Widget widget = weak_ref.Target as Widget;
						
						if ((widget != null) &&
							(widget.IsCommand))
						{
							if ((dispatcher == null) ||
								(widget.CommandDispatcher == dispatcher))
							{
								if (widget.CommandName == command)
								{
									list.Add (widget);
								}
							}
						}
					}
					else
					{
						dead.Add (weak_ref);
					}
				}
				
				//	Profite de l'occasion, puisqu'on vient de passer en revue tous les widgets,
				//	de supprimer ceux qui sont morts entre temps :
				
				foreach (System.WeakReference weak_ref in dead)
				{
					Widget.alive_widgets.Remove (weak_ref);
				}
			}
			
			Widget[] widgets = new Widget[list.Count];
			list.CopyTo (widgets);
			
			return widgets;
		}
		
		public static Widget[]	FindAllCommandWidgets(System.Text.RegularExpressions.Regex regex, CommandDispatcher dispatcher)
		{
			//	Passe en revue absolument tous les widgets qui existent et cherche ceux qui ont
			//	une commande qui correspond au critère spécifié.
			
			System.Collections.ArrayList list = new System.Collections.ArrayList ();
			System.Collections.ArrayList dead = new System.Collections.ArrayList ();
			
			lock (Widget.alive_widgets)
			{
				foreach (System.WeakReference weak_ref in Widget.alive_widgets)
				{
					//	On utilise la liste des widgets connus qui permet d'avoir accès immédiatement
					//	à tous les widgets sans nécessiter de descente récursive :
					
					if (weak_ref.IsAlive)
					{
						//	Le widget trouvé existe (encore) :
						
						Widget widget = weak_ref.Target as Widget;
						
						if ((widget != null) &&
							(widget.IsCommand))
						{
							if ((dispatcher == null) ||
								(widget.CommandDispatcher == dispatcher))
							{
								if (regex.IsMatch (widget.CommandName))
								{
									list.Add (widget);
								}
							}
						}
					}
					else
					{
						dead.Add (weak_ref);
					}
				}
				
				//	Profite de l'occasion, puisqu'on vient de passer en revue tous les widgets,
				//	de supprimer ceux qui sont morts entre temps :
				
				foreach (System.WeakReference weak_ref in dead)
				{
					Widget.alive_widgets.Remove (weak_ref);
				}
			}
			
			Widget[] widgets = new Widget[list.Count];
			list.CopyTo (widgets);
			
			return widgets;
		}
#endif
		
		public static Widget[]	FindAllFullPathWidgets(System.Text.RegularExpressions.Regex regex)
		{
			//	Passe en revue absolument tous les widgets qui existent et cherche ceux qui ont
			//	une commande qui correspond au critère spécifié.
			
			System.Collections.ArrayList list = new System.Collections.ArrayList ();
			System.Collections.ArrayList dead = new System.Collections.ArrayList ();
			
			lock (Widget.aliveWidgets)
			{
				foreach (System.WeakReference weak_ref in Widget.aliveWidgets)
				{
					//	On utilise la liste des widgets connus qui permet d'avoir accès immédiatement
					//	à tous les widgets sans nécessiter de descente récursive :
					
					if (weak_ref.IsAlive)
					{
						//	Le widget trouvé existe (encore) :
						
						Widget widget = weak_ref.Target as Widget;
						
						if ((widget != null) &&
							(widget.Name != ""))
						{
							if (regex.IsMatch (widget.FullPathName))
							{
								list.Add (widget);
							}
						}
					}
					else
					{
						dead.Add (weak_ref);
					}
				}
				
				//	Profite de l'occasion, puisqu'on vient de passer en revue tous les widgets,
				//	de supprimer ceux qui sont morts entre temps :
				
				foreach (System.WeakReference weak_ref in dead)
				{
					Widget.aliveWidgets.Remove (weak_ref);
				}
			}
			
			Widget[] widgets = new Widget[list.Count];
			list.CopyTo (widgets);
			
			return widgets;
		}
		
		
		#region CommandWidgetFinder class
		protected class CommandWidgetFinder
		{
			public CommandWidgetFinder()
			{
			}
			
			public CommandWidgetFinder(string filter)
			{
				this.filter = filter;
			}
			
			public CommandWidgetFinder(System.Text.RegularExpressions.Regex regex)
			{
				this.regex = regex;
			}
			
			
			public bool Analyse(Widget widget)
			{
				if (widget.HasCommand)
				{
					if (this.regex == null)
					{
						if (this.filter == null)
						{
							this.list.Add (widget);
						}
						else if (this.filter == widget.CommandName)
						{
							this.list.Add (widget);
						}
					}
					else
					{
						//	Une expression régulière a été définie pour filtrer les widgets en
						//	fonction de leur nom. On applique cette expression pour voir si le
						//	nom de la commande est conforme...
						
						System.Text.RegularExpressions.Match match = this.regex.Match (widget.CommandName);
						
						//	...en cas de succès, on prend note du widget, sinon on passe simplement
						//	au suivant.
						
						if (match.Success)
						{
							this.list.Add (widget);
						}
					}
				}
				
				return true;
			}
			
			
			public Widget[]					Widgets
			{
				get
				{
					Widget[] widgets = new Widget[this.list.Count];
					this.list.CopyTo (widgets);
					return widgets;
				}
			}
			
			
			System.Collections.ArrayList			list   = new System.Collections.ArrayList ();
			System.Text.RegularExpressions.Regex	regex  = null;
			string									filter = null;
		}
		#endregion
		
		public void SetFocusOnTabWidget()
		{
			Widget widget = this.FindTabWidget (TabNavigationDir.Forwards, TabNavigationMode.ActivateOnTab, false, true);
			
			if (widget == null)
			{
				widget = this;
			}

			widget.Focus ();
		}
		
		public Widget FindTabWidget(TabNavigationDir dir, TabNavigationMode mode)
		{
			return this.FindTabWidget (dir, mode, false, false);
		}
		
		
		protected virtual Widget FindTabWidget(TabNavigationDir dir, TabNavigationMode mode, bool disable_first_enter, bool accept_focus)
		{
			if (this.ProcessTab (dir, mode))
			{
				return this;
			}
			
			Widget find = null;
			
			if (this.AutoRadio)
			{
				if (mode == TabNavigationMode.ActivateOnCursorX)
				{
					Helpers.GroupController controller = Helpers.GroupController.GetGroupController (this);
					
					find = controller.FindXWidget (this, dir == TabNavigationDir.Backwards ? -1 : 1);
					
					if ((find == null) &&
						(controller.FindXWidget (this, dir == TabNavigationDir.Backwards ? 1 : -1) == null))
					{
						//	L'utilisateur demande un déplacement horizontal bien que la disposition
						//	soit purement verticale. On corrige pour lui :
						
						find = controller.FindYWidget (this, dir == TabNavigationDir.Backwards ? -1 : 1);
					}
					
					if (find != null)
					{
						find.ActiveState = ActiveState.Yes;
						return find;
					}
				}
				
				if (mode == TabNavigationMode.ActivateOnCursorY)
				{
					Helpers.GroupController controller = Helpers.GroupController.GetGroupController (this);
					
					find = controller.FindYWidget (this, dir == TabNavigationDir.Backwards ? -1 : 1);
					
					if ((find == null) &&
						(controller.FindYWidget (this, dir == TabNavigationDir.Backwards ? 1 : -1) == null))
					{
						//	L'utilisateur demande un déplacement vertical bien que la disposition
						//	soit purement horizontale. On corrige pour lui :
						
						find = controller.FindXWidget (this, dir == TabNavigationDir.Backwards ? -1 : 1);
					}
					
					if (find != null)
					{
						find.ActiveState = ActiveState.Yes;
						return find;
					}
				}
			}
			
			if ((!disable_first_enter) &&
				((this.tab_navigation_mode & TabNavigationMode.ForwardToChildren) != 0) &&
				(this.HasChildren))
			{
				//	Ce widget permet aux enfants d'entrer dans la liste accessible par la
				//	touche TAB.
				
				Widget[] candidates = this.Children.Widgets[0].FindTabWidgets (mode);
				
				if (candidates.Length > 0)
				{
					if (dir == TabNavigationDir.Forwards)
					{
						find = candidates[0].FindTabWidget (dir, mode, false, true);
					}
					else if (accept_focus)
					{
						int count = candidates.Length;
						find = candidates[count-1].FindTabWidget (dir, mode, false, true);
					}
					
					if (find != null)
					{
						return find;
					}
				}
			}
			
			if (accept_focus)
			{
				if ((this.tab_navigation_mode & mode) != 0)
				{
					return this;
				}
			}
			
			//	Cherche parmi les frères...
			
			Widget[] siblings = this.FindTabWidgets (mode);
			bool     search_z = true;
			
			for (int i = 0; i < siblings.Length; i++)
			{
				if (siblings[i] == this)
				{
					//	On vient de trouver notre position dans la liste des widgets activables
					//	par la touche TAB.
					
					search_z = false;
					
					switch (dir)
					{
						case TabNavigationDir.Backwards:
							
							find = this.GetTabFromSiblings (i, dir, siblings);
							
							if (find != null)
							{
								if (((find.tab_navigation_mode & TabNavigationMode.ForwardToChildren) != 0) &&
									(find.HasChildren))
								{
									//	Entre en marche arrière dans le widget...
									
									Widget[] candidates = find.Children.Widgets[0].FindTabWidgets (mode);
									
									if (candidates.Length > 0)
									{
										int    count = candidates.Length;
										Widget enter = candidates[count-1].FindTabWidget (dir, mode, false, true);
										
										if (enter != null)
										{
											find = enter;
										}
									}
								}
							}
							break;
						
						case TabNavigationDir.Forwards:
							
							find = this.GetTabFromSiblings (i, dir, siblings);
							
							if (find != null)
							{
								if (((find.tab_navigation_mode & TabNavigationMode.ForwardToChildren) != 0) &&
									((find.tab_navigation_mode & TabNavigationMode.ForwardOnly) != 0) &&
									(find.HasChildren))
								{
									//	Entre en marche avant dans le widget...
									
									Widget[] candidates = find.Children.Widgets[0].FindTabWidgets (mode);
									
									if (candidates.Length > 0)
									{
										Widget enter = candidates[0].FindTabWidget (dir, mode, false, true);
										
										if (enter != null)
										{
											find = enter;
										}
									}
								}
							}
							break;
					}
					
					break;
				}
			}
			
			if (search_z)
			{
				find = this;
				
				while (true)
				{
					if (dir == TabNavigationDir.Forwards)
					{
						find = this.Children.FindNext (find) as Widget;
					}
					else if (dir == TabNavigationDir.Backwards)
					{
						find = this.Children.FindPrevious (find) as Widget;
					}
					if ((find == null) ||
						((find.tab_navigation_mode & mode) != 0))
					{
						break;
					}
				}
			}
			
			if (find == null)
			{
				//	Toujours rien trouvé. On a demandé aux enfants et aux frères. Il ne nous
				//	reste plus qu'à transmettre au père.
				
				if (this.Parent != null)
				{
					if (this.Parent.ProcessTabChildrenExit (dir, mode, out find))
					{
						return find;
					}
					
					find = null;
					
					if ((this.Parent.tab_navigation_mode & TabNavigationMode.ForwardToChildren) != 0)
					{
						bool accept;
						
						switch (dir)
						{
							case TabNavigationDir.Backwards:
								accept = (this.Parent.tab_navigation_mode & TabNavigationMode.ForwardOnly) == 0;
								find   = this.Parent.FindTabWidget (dir, mode, true, accept);
								break;
							
							case TabNavigationDir.Forwards:
								accept = false;
								find = this.Parent.FindTabWidget (dir, mode, true, accept);
								break;
						}
					}
				}
				else if (this.HasChildren)
				{
					//	Il n'y a plus de parents au-dessus. C'est donc vraisemblablement WindowRoot et
					//	dans ce cas, il ne sert à rien de boucler. On va simplement tenter d'activer le
					//	premier descendant trouvé :
					
					Widget[] candidates = this.Children.Widgets[0].FindTabWidgets (mode);
					
					if (candidates.Length > 0)
					{
						if (dir == TabNavigationDir.Forwards)
						{
							find = candidates[0].FindTabWidget (dir, mode, false, true);
						}
						else if (accept_focus)
						{
							int count = candidates.Length;
							find = candidates[count-1].FindTabWidget (dir, mode, false, true);
						}
						
						if (find != null)
						{
							return find;
						}
					}
				}
			}
			
			if (find == null)
			{
				//	On ne peut plus avancer, donc on tente de boucler.
				
				if (siblings.Length > 1)
				{
					switch (dir)
					{
						case TabNavigationDir.Backwards:
							find = siblings[siblings.Length-1].FindTabWidget (dir, mode, false, true);
							break;
							
						case TabNavigationDir.Forwards:
							find = siblings[0].FindTabWidget (dir, mode, false, true);
							break;
					}
				}
			}
			
			return find;
		}
		
		protected Widget[] FindTabWidgets(TabNavigationMode mode)
		{
			System.Collections.ArrayList list = this.FindTabWidgetList (mode);
			
			Widget[] widgets = new Widget[list.Count];
			list.CopyTo (widgets);
			
			return widgets;
		}
		
		protected virtual System.Collections.ArrayList FindTabWidgetList(TabNavigationMode mode)
		{
			System.Collections.ArrayList list = new System.Collections.ArrayList ();
			
			Widget parent = this.Parent;
			
			if (parent != null)
			{
				Widget[] siblings = parent.Children.Widgets;
				
				for (int i = 0; i < siblings.Length; i++)
				{
					Widget sibling = siblings[i];
					
					if (((sibling.TabNavigation & mode) != 0) &&
						(sibling.IsEnabled) &&
						(sibling.Visibility))
					{
						if (((sibling.TabNavigation & TabNavigationMode.SkipIfReadOnly) != 0) &&
							(sibling is Types.IReadOnly))
						{
							//	Saute aussi les widgets qui déclarent être en lecture seule. Ils ne
							//	sont pas intéressants pour une navigation clavier :
							
							Types.IReadOnly read_only = sibling as Types.IReadOnly;
							
							if (read_only.IsReadOnly)
							{
								continue;
							}
						}
						
						list.Add (sibling);
					}
				}
			}
			
			list.Sort (new TabIndexComparer ());
			
			if ((mode == TabNavigationMode.ActivateOnTab) &&
				(this.AutoRadio))
			{
				//	On recherche les frères de ce widget, pour déterminer lequel devra être activé par la
				//	pression de la touche TAB. Pour bien faire, il faut supprimer les autres boutons radio
				//	qui appartiennent à notre groupe :
			
				System.Collections.ArrayList copy = new System.Collections.ArrayList ();
				
				string group = this.Group;
				
				foreach (Widget widget in list)
				{
					if ((widget != this) &&
						(widget.Group == group))
					{
						//	Saute les boutons du même groupe. Ils ne sont pas accessibles par la
						//	touche TAB.
					}
					else
					{
						copy.Add (widget);
					}
				}
				
				return copy;
			}
			else
			{
				return list;
			}
		}
		
		protected virtual Widget GetTabFromSiblings(int index, TabNavigationDir dir, Widget[] siblings)
		{
			switch (dir)
			{
				case TabNavigationDir.Backwards:
					if (index > 0)
					{
						return siblings[index-1];
					}
					break;
				
				case TabNavigationDir.Forwards:
					if (index < siblings.Length-1)
					{
						return siblings[index+1];
					}
					break;
			}
			
			return null;
		}
		
		
		#region TabIndexComparer class
		protected class TabIndexComparer : System.Collections.IComparer
		{
			public int Compare(object x, object y)
			{
				Widget wx = x as Widget;
				Widget wy = y as Widget;
				if (wx == wy) return 0;
				if (wx == null) return -1;
				if (wy == null) return 1;
				return (wx.TabIndex == wy.TabIndex) ? wx.Index - wy.Index : wx.TabIndex - wy.TabIndex;
			}
		}
		#endregion
		
		protected virtual bool ProcessTab(TabNavigationDir dir, TabNavigationMode mode)
		{
			//	Une classe qui désire gérer l'événement de déplacement de manière interne,
			//	par exemple dans le cas d'un widget gérant lui-même plusieurs zones sensibles,
			//	a la possibilité de le faire ici; si l'événement a été consommé de manière
			//	interne, il faut retourner 'true'.
			
			return false;
		}
		
		protected virtual bool ProcessTabChildrenExit(TabNavigationDir dir, TabNavigationMode mode, out Widget focus)
		{
			//	Une classe qui désire gérer l'événement de déplacement lorsque le focus quitte
			//	le dernier (ou le premier) des enfants peut le faire ici. Si cette méthode
			//	retourne 'true', c'est le widget retourné par 'focus' qui sera activé; dans
			//	le cas contraire, un algorithme de navigation par défaut sera utilisé.
			
			focus = null;
			
			return false;
		}
		
		
		internal bool InternalAboutToGetFocus(TabNavigationDir dir, TabNavigationMode mode, out Widget focus)
		{
			return this.AboutToGetFocus (dir, mode, out focus);
		}
		
		internal bool InternalAboutToLoseFocus(TabNavigationDir dir, TabNavigationMode mode)
		{
			return this.AboutToLoseFocus (dir, mode);
		}
		
		protected virtual bool AboutToGetFocus(TabNavigationDir dir, TabNavigationMode mode, out Widget focus)
		{
			if ((this.AutoRadio) &&
				(this.ActiveState != ActiveState.Yes) &&
				(mode == TabNavigationMode.ActivateOnTab))
			{
				//	Ce n'est pas ce bouton radio qui est allumé. TAB voudrait nous
				//	donner le focus, mais ce n'est pas adéquat; mieux vaut mettre
				//	le focus sur le frère qui est actuellement activé :
				
				Helpers.GroupController controller    = Helpers.GroupController.GetGroupController (this);
				Widget                  active_widget = controller.FindActiveWidget ();
				
				if (active_widget != null)
				{
					return active_widget.AboutToGetFocus (dir, mode, out focus);
				}
			}
			
			focus = this;
			return true;
		}
		
		protected virtual bool AboutToLoseFocus(TabNavigationDir dir, TabNavigationMode mode)
		{
			return true;
		}
		
		
		protected virtual void AboutToBecomeOrphan()
		{
			this.SetFocused (false);
			this.SetEngaged (false);
			this.SetEntered (false);
			
			if (this.HasChildren)
			{
				Widget[] children = this.Children.Widgets;
				int  children_num = children.Length;
				
				for (int i = 0; i < children_num; i++)
				{
					children[i].AboutToBecomeOrphan ();
				}
			}
		}
		
		
		public virtual bool WalkChildren(WidgetWalkChildrenCallback callback)
		{
			//	Retourne true si on a parcouru tous les enfants.
			
			foreach (Widget child in this.GetAllChildren ())
			{
				if (!callback (child))
				{
					return false;
				}
			}
			
			return true;
		}
		
		public virtual void ExecuteCommand()
		{
			if ((this.HasCommand) &&
				(this.IsEnabled))
			{
				Window window = this.Window;
				
				if (window != null)
				{
					Command commandObject = this.CommandObject;
					
					if (commandObject == null)
					{
						//	Command cannot be queued !
					}
					else
					{
						CommandState state = Helpers.VisualTree.GetCommandContext (this).GetCommandState (commandObject);
						this.QueueCommandForExecution (window, commandObject, state);
					}
				}
			}
		}

		protected virtual void QueueCommandForExecution(Window window, Command command, CommandState state)
		{
			window.QueueCommand (this, command);
		}
		
		internal void ExecuteCommand(string command)
		{
			Window window = this.Window;
			
			if (window != null)
			{
				window.QueueCommand (this, command);
			}
		}
		
		
		protected override void SetBoundsOverride(Drawing.Rectangle oldRect, Drawing.Rectangle newRect)
		{
			base.SetBoundsOverride(oldRect, newRect);

			this.InvalidateTextLayout ();
			this.UpdateClientGeometry ();
		}

		protected virtual void UpdateClientGeometry()
		{
		}
		
		protected virtual void UpdateTextLayout()
		{
			if (this.text_layout != null)
			{
				this.text_layout.Alignment  = this.ContentAlignment;
				this.text_layout.LayoutSize = this.GetTextLayoutSize ();
				
				this.UpdateCaption ();
			}
		}

		internal override void InvalidateTextLayout()
		{
			base.InvalidateTextLayout ();
			
			if (this.TextLayout != null)
			{
				this.UpdateTextLayout ();
			}
		}

		protected override void OnDisplayCaptionChanged()
		{
			base.OnDisplayCaptionChanged ();
			this.UpdateCaption ();
		}

		public void ForceCaptionUpdate()
		{
			if (this.text_layout == null)
			{
				this.CreateTextLayout ();
			}
			
			this.UpdateTextLayout ();
		}

		protected virtual void UpdateCaption()
		{
			if (this.text_layout == null)
			{
				//	Create the text layout object first. This will automatically call
				//	UpdateCaption back again, so we don't need to do anything else
				//	here :

				this.CreateTextLayout ();
			}
			else
			{
				Caption caption = this.GetDisplayCaption ();

				if (caption != null)
				{
					if (caption.HasLabels)
					{
						this.DefineTextFromCaption (TextLayout.SelectBestText (this.TextLayout, caption.SortedLabels, this.GetTextLayoutSize ()));
					}

					if (caption.HasDescription)
					{
						Collections.ShortcutCollection shortcuts = Shortcut.GetShortcuts (caption);
						string tip = Shortcut.AppendShortcutText (caption.Description, shortcuts);

						this.DefineToolTipFromCaption (tip);
					}

					if (caption.HasIcon)
					{
						this.DefineIconFromCaption (caption.Icon);
					}
				}
			}
		}

		protected virtual void DefineTextFromCaption(string text)
		{
			this.Text = text;
		}

		protected virtual void DefineToolTipFromCaption(string tip)
		{
			ToolTip.Default.SetToolTip (this, tip);
		}

		protected virtual void DefineIconFromCaption(string icon)
		{
			Drawing.Image image = Support.ImageProvider.Default.GetImage (icon, Support.Resources.DefaultManager);

			if (image == null)
			{
				this.IconName = null;
			}
			else
			{
				this.IconName = icon;
			}
		}

		protected virtual Drawing.Size GetTextLayoutSize()
		{
			return this.IsActualGeometryValid ? this.Client.Size : this.PreferredSize;
		}

		protected virtual Drawing.Point GetTextLayoutOffset()
		{
			return Drawing.Point.Zero;
		}

		internal void InternalNotifyTextLayoutTextChanged(string oldText, string newText)
		{
			this.NotifyTextLayoutTextChanged (oldText, newText);
		}

		protected virtual void NotifyTextLayoutTextChanged(string oldText, string newText)
		{
			this.SetValueBase (Widget.TextProperty, newText);
		}
		
		public virtual void PaintHandler(Drawing.Graphics graphics, Drawing.Rectangle repaint, IPaintFilter paint_filter)
		{
			if ((paint_filter != null) &&
				(paint_filter.IsWidgetFullyDiscarded (this)))
			{
				return;
			}
			
			this.OnPreparePaint ();
			
			long cycles = Drawing.Agg.Library.Cycles;
			
			if (this.DebugActive)
			{
				System.Diagnostics.Debug.WriteLine (string.Format ("{0}: clip {1}, widget {2}", this.ToString (), graphics.SaveClippingRectangle ().ToString (), this.MapClientToRoot (this.Client.Bounds).ToString ()));
			}
			
			if (this.PaintCheckClipping (repaint))
			{
				Drawing.Rectangle bounds = this.GetClipBounds ();
				
				bounds  = this.MapClientToRoot (bounds);
				repaint = this.MapParentToClient (repaint);
				
				Drawing.Rectangle original_clipping  = graphics.SaveClippingRectangle ();
				Drawing.Transform original_transform = graphics.Transform;
				Drawing.Transform graphics_transform = Drawing.Transform.FromTranslation (this.ActualLocation);
				
				graphics.SetClippingRectangle (bounds);
				
				if (graphics.TestForEmptyClippingRectangle ())
				{
					//	Optimisation du cas où la région de clipping devient vide: on restaure
					//	la région précédente et on ne fait rien de plus.
					
					graphics.RestoreClippingRectangle (original_clipping);
					return;
				}
				
				graphics_transform.MultiplyBy (original_transform);
				
				graphics.Transform = graphics_transform;
			
				try
				{
					if (this.hypertext_list != null)
					{
						this.hypertext_list.Clear ();
					}
					
					PaintEventArgs local_paint_args = new PaintEventArgs (graphics, repaint);
					
					//	Peint l'arrière-plan du widget. En principe, tout va dans l'arrière plan, sauf
					//	si l'on désire réaliser des effets de transparence par dessus le dessin des
					//	widgets enfants.
					
					if ((paint_filter == null) ||
						(paint_filter.IsWidgetPaintDiscarded (this) == false))
					{
						graphics.ResetLineStyle ();
						this.OnPaintBackground (local_paint_args);
						
						if (paint_filter != null)
						{
							paint_filter.NotifyAboutToProcessChildren ();
						}
					}
					
					//	Peint tous les widgets enfants, en commençant par le numéro 0, lequel se trouve
					//	derrière tous les autres, etc. On saute les widgets qui ne sont pas visibles.
					
					if (this.HasChildren)
					{
						Widget[] children = this.Children.Widgets;
						int  children_num = children.Length;
						
						for (int i = 0; i < children_num; i++)
						{
							Widget widget = children[i];
						
							System.Diagnostics.Debug.Assert (widget != null);
						
							if (widget.Visibility)
							{
								widget.PaintHandler (graphics, repaint, paint_filter);
							}
						}
					}
				
					//	Peint l'avant-plan du widget, à n'utiliser que pour faire un "effet" spécial
					//	après coup.
					
					if ((paint_filter == null) ||
						(paint_filter.IsWidgetPaintDiscarded (this) == false))
					{
						graphics.ResetLineStyle ();
						this.OnPaintForeground (local_paint_args);
						
						if (paint_filter != null)
						{
							paint_filter.NotifyChildrenProcessed ();
						}
					}
				}
				finally
				{
					graphics.Transform = original_transform;
					graphics.RestoreClippingRectangle (original_clipping);
				}
			}
#if false
			else
			{
				Drawing.Rectangle bounds = this.GetPaintBounds ();
				System.Diagnostics.Debug.WriteLine ("Clipped : repaint="+repaint+", bounds="+bounds+", parent="+this.MapClientToParent (bounds));
				System.Diagnostics.Debug.WriteLine ("          widget ="+this.Bounds);
			}
#endif
			
			if (this.DebugActive)
			{
				cycles = Drawing.Agg.Library.Cycles - cycles;
				System.Diagnostics.Debug.WriteLine (string.Format ("{0}: {1} us @ 1.7GHz", this.ToString (), cycles/1700));
			}
		}
		
		protected virtual void PaintBackgroundImplementation(Drawing.Graphics graphics, Drawing.Rectangle clip_rect)
		{
			//	Implémenter le dessin du fond dans cette méthode.
		}
		
		protected virtual void PaintForegroundImplementation(Drawing.Graphics graphics, Drawing.Rectangle clip_rect)
		{
			//	Implémenter le dessin des enjoliveurs additionnels dans cette méthode.
		}
		
		protected virtual bool PaintCheckClipping(Drawing.Rectangle repaint)
		{
			Drawing.Rectangle bounds = this.GetPaintBounds ();
			bounds = this.MapClientToParent (bounds);
			return repaint.IntersectsWithAligned (bounds);
		}
		
		
		public void DispatchDummyMouseMoveEvent()
		{
			Window window = this.Window;
			
			if (window != null)
			{
				window.DispatchMessage (Message.CreateDummyMouseMoveEvent ());
			}
		}
		
		public void DispatchDummyMouseUpEvent(MouseButtons button, Drawing.Point pos)
		{
			Window window = this.Window;
			
			if (window != null)
			{
				window.DispatchMessage (Message.CreateDummyMouseUpEvent (button, pos));
			}
		}
		
		public void MessageHandler(Message message)
		{
			Drawing.Point point;

			if (message.IsDummy)
			{
				point = Drawing.Point.Zero;
			}
			else
			{
				point = message.Cursor;
				point = this.MapRootToClient (point);
				point = this.MapClientToParent (point);
			}
			
			this.MessageHandler (message, point);
		}
		
		public virtual void MessageHandler(Message message, Drawing.Point pos)
		{
			Drawing.Point client_pos = message.IsDummy ? pos : this.MapParentToClient (pos);
			
			if (! this.PreProcessMessage (message, client_pos))
			{
				return;
			}
			
			//	En premier lieu, si le message peut être transmis aux descendants de ce widget, passe
			//	en revue ceux-ci dans l'ordre inverse de leur affichage (commence par le widget qui est
			//	visuellement au sommet).
			
			if ((message.FilterNoChildren == false) &&
				(message.Handled == false) &&
				(this.HasChildren))
			{
				Widget[] children = this.Children.Widgets;
				int  children_num = children.Length;

				WindowRoot root = message.WindowRoot ?? Helpers.VisualTree.GetWindowRoot (this);
				
				for (int i = 0; i < children_num; i++)
				{
					Widget widget         = children[children_num-1 - i];
					bool   contains_focus = (root == null) ? false : root.DoesVisualContainKeyboardFocus (widget);
					
					if ((widget.IsFrozen == false) &&
						((widget.Visibility) || (contains_focus && message.IsKeyType)) &&
						((message.FilterOnlyFocused == false) || (contains_focus)) &&
						((message.FilterOnlyOnHit == false) || (widget.HitTest (client_pos))))
					{
						if (widget.IsEnabled)
						{
							if (message.IsMouseType)
							{
								//	C'est un message souris. Vérifions d'abord si le widget contenait déjà
								//	la souris auparavant.
								
								if ((widget.IsEntered == false) &&
									(message.Type != MessageType.MouseLeave))
								{
									widget.SetEntered (true);
								}
							}
							
							widget.MessageHandler (message, client_pos);
							
							if (message.Handled)
							{
								break;
							}
						}
						else
						{
							break;
						}
					}
				}
			}
			else if ((message.Handled == false) &&
				/**/ (message.Captured) &&
				/**/ (message.IsMouseType))
			{
				if ((this.IsEntered == false) &&
					(message.InWidget == this))
				{
					this.SetEntered (true);
				}
			}
			
			if (message.Handled == false)
			{
				this.DispatchMessage (message, client_pos);
			}
			
			this.PostProcessMessage (message, client_pos);
		}
		
		
		protected virtual void DispatchMessage(Message message, Drawing.Point pos)
		{
			if (this.Visibility || message.IsKeyType)
			{
				bool is_entered = this.IsEntered;
				
				switch (message.Type)
				{
					case MessageType.MouseUp:
						
						if (Message.CurrentState.IsSameWindowAsButtonDown == false)
						{
							return;
						}
						
						if (! this.AutoDoubleClick)
						{
							Message.ResetButtonDownCounter ();
						}
						
						//	Le bouton a été relâché. Ceci génère l'événement 'Released' pour signaler
						//	ce relâchement, mais aussi un événement 'Clicked' ou 'DoubleClicked' en
						//	fonction du nombre de clics.
						
						this.OnReleased (new MessageEventArgs (message, pos));
						
						if (is_entered)
						{
							switch (message.ButtonDownCount)
							{
								case 1:	this.OnClicked (new MessageEventArgs (message, pos));		break;
								case 2:	this.OnDoubleClicked (new MessageEventArgs (message, pos));	break;
							}
						}
						break;
					
					case MessageType.MouseDown:
						this.OnPressed (new MessageEventArgs (message, pos));
						break;
				}
				
				
				this.ProcessMessage (message, pos);
			}
			else
			{
				System.Diagnostics.Debug.WriteLine ("Dispatching to invisible widget: "  + this.ToString ());
			}
		}
		
		protected virtual bool PreProcessMessage(Message message, Drawing.Point pos)
		{
			//	...appelé avant que l'événement ne soit traité...
			
			if (this.PreProcessing != null)
			{
				MessageEventArgs e = new MessageEventArgs (message, pos);
				this.PreProcessing (this, e);
				
				if (e.Suppress)
				{
					return false;
				}
			}
			
			if (message.IsMouseType)
			{
				bool reset = true;
				
				if (this.hypertext_list != null)
				{
					foreach (HypertextInfo info in this.hypertext_list)
					{
						if (info.Bounds.Contains (pos))
						{
							this.SetHypertext (info);
							reset = false;
							break;
						}
					}
				}
				
				if (reset)
				{
					this.SetHypertext (null);
				}
			}
			
			return true;
		}
		
		protected virtual void SetHypertext(HypertextInfo info)
		{
			if (this.hypertext == null)
			{
				if (info == null)
				{
					return;
				}
			}
			else if (this.hypertext.Equals (info))
			{
				return;
			}
			
			this.hypertext = info;
			this.OnHypertextHot ();
		}
		
		protected virtual void ProcessMessage(Message message, Drawing.Point pos)
		{
			//	...appelé pour traiter l'événement...
		}
		
		protected virtual bool PostProcessMessage(Message message, Drawing.Point pos)
		{
			//	...appelé après que l'événement ait été traité...
			
			if (this.PostProcessing != null)
			{
				MessageEventArgs e = new MessageEventArgs (message, pos);
				this.PostProcessing (this, e);
				
				if (e.Suppress)
				{
					return false;
				}
			}
			
			return true;
		}
		
		
		public virtual bool ShortcutHandler(Shortcut shortcut)
		{
			return this.ShortcutHandler (shortcut, true);
		}
		
		
		protected virtual bool ShortcutHandler(Shortcut shortcut, bool execute_focused)
		{
			Widget[] children = this.Children.Widgets;
			int  children_num = children.Length;
			
			if (execute_focused)
			{
				for (int i = 0; i < children_num; i++)
				{
					Widget widget = children[children_num-1 - i];
				
					if (widget.ContainsKeyboardFocus)
					{
						if (widget.ShortcutHandler (shortcut))
						{
							return true;
						}
					}
				}
			}
			
			if (this.ProcessShortcut (shortcut))
			{
				return true;
			}
			
			for (int i = 0; i < children_num; i++)
			{
				Widget widget = children[children_num-1 - i];
				
				if ((widget.IsEnabled) &&
					(widget.IsFrozen == false) &&
					(widget.Visibility))
				{
					if (widget.ShortcutHandler (shortcut, false))
					{
						return true;
					}
				}
			}
			
			return false;
		}
		
		protected virtual bool ProcessShortcut(Shortcut shortcut)
		{
			if ((this.shortcuts != null) &&
				(this.shortcuts.Contains (shortcut)))
			{
				this.OnShortcutPressed ();
				return true;
			}
			
			return false;
		}
		
		protected virtual void ResetMnemonicShortcut()
		{
			if (this.AutoMnemonic)
			{
				this.Shortcuts.Define (this.GetMnemonicShortcuts ());
			}
		}
		
		protected virtual Shortcut[] GetMnemonicShortcuts()
		{
			if (this.Mnemonic == '\0')
			{
				return new Shortcut[0];
			}
			else
			{
				Shortcut[] shortcuts = new Shortcut[2];
				
				shortcuts[0] = new Shortcut (this.Mnemonic, ModifierKeys.None);
				shortcuts[1] = new Shortcut (this.Mnemonic, ModifierKeys.Alt);
				
				return shortcuts;
			}
		}
		
		protected virtual void BuildFullPathName(System.Text.StringBuilder buffer)
		{
			if (this.Parent != null)
			{
				this.Parent.BuildFullPathName (buffer);
			}
			
			string name = this.Name;
			
			if (name != "")
			{
				//	Ne tient pas compte du nom si celui-ci est absent, sinon le chemin de la
				//	commande risque de contenir des suites de ".." inutiles.
				
				int length = buffer.Length;
				
				if ((length > 0) &&
					(buffer[length-1] != '.'))
				{
					buffer.Append (".");
				}
				
				buffer.Append (name);
			}
		}
		
		
		protected virtual void CreateTextLayout()
		{
			if (this.text_layout == null)
			{
				this.text_layout = new TextLayout ();

				this.text_layout.Embedder        = this;
				this.text_layout.DefaultFont     = this.DefaultFont;
				this.text_layout.DefaultFontSize = this.DefaultFontSize;
				this.text_layout.Anchor         += new AnchorEventHandler (this.HandleTextLayoutAnchor);
				this.text_layout.ResourceManager = this.ResourceManager;
				
				this.UpdateTextLayout ();
			}
		}
		
		protected virtual void ModifyText(string text)
		{
			System.Diagnostics.Debug.Assert (this.text_layout != null);
			
			this.SetValueBase (Widget.TextProperty, text);
			this.OnTextDefined ();
		}
		
		protected virtual void ModifyTextLayout(string text)
		{
			if (this.text_layout != null)
			{
				this.text_layout.Text = text;
			}
		}
		
		protected virtual void DisposeTextLayout()
		{
			if (this.text_layout != null)
			{
				this.text_layout.Anchor -= new AnchorEventHandler (this.HandleTextLayoutAnchor);
				this.text_layout = null;
			}
		}
		

		protected override void OnParentChanged(Epsitec.Common.Types.DependencyPropertyChangedEventArgs e)
		{
			base.OnParentChanged (e);
			
			Widget old_parent = e.OldValue as Widget;
			Widget new_parent = e.NewValue as Widget;
			
			if (new_parent == null)
			{
				this.AboutToBecomeOrphan ();
			}
			
			Window old_window = old_parent == null ? null : Helpers.VisualTree.GetWindow (old_parent);
			Window new_window = new_parent == null ? null : Helpers.VisualTree.GetWindow (new_parent);
			
			if (old_window != new_window)
			{
				this.NotifyWindowChanged (old_window, new_window);
			}
		}
		
		protected virtual void NotifyWindowChanged(Window old_window, Window new_window)
		{
			foreach (Widget widget in this.Children.Widgets)
			{
				widget.NotifyWindowChanged (old_window, new_window);
			}
		}
		


#if false //#fix
		protected void HandleParentChanged()
		{
			//	Cette méthode est appelée chaque fois qu'un widget change de parent.
			
			this.OnParentChanged ();
			
			if (this.Parent == null)
			{
				if (this.Visibility)
				{
					this.NotifyChangedToHidden ();
				}
			}
			else
			{
				if ((this.Visibility) &&
					(this.IsVisible))
				{
					this.NotifyChangedToVisible ();
				}
			}
		}
#endif
		
		protected void HandleAdornerChanged()
		{
			foreach (Widget child in this.Children)
			{
				child.HandleAdornerChanged ();
			}
			
			this.OnAdornerChanged ();
		}
		
		protected void HandleCultureChanged()
		{
			foreach (Widget child in this.Children)
			{
				child.HandleCultureChanged ();
			}
			
			this.OnCultureChanged ();
		}
		
		protected void HandleTextLayoutAnchor(object sender, AnchorEventArgs e)
		{
			System.Diagnostics.Debug.Assert (sender == this.text_layout);
			
			HypertextInfo info = new HypertextInfo (this.text_layout, e.Bounds, e.Index);
			
			if (this.hypertext_list == null)
			{
				this.hypertext_list = new System.Collections.ArrayList ();
			}
			
			this.hypertext_list.Add (info);
		}
		
		
		protected virtual void OnClientGeometryUpdated()
		{
			if (this.ClientGeometryUpdated != null)
			{
				this.ClientGeometryUpdated (this);
			}
		}
		
		protected virtual void OnPreparePaint()
		{
			if (this.PreparePaint != null)
			{
				this.PreparePaint (this);
			}
		}
		
		protected virtual void OnPaintBackground(PaintEventArgs e)
		{
			PaintEventHandler handler = this.GetUserEventHandler ("PaintBackground") as PaintEventHandler;
			
			if (handler != null)
			{
				e.Suppress = false;
				
				handler (this, e);
				
				if (e.Suppress)
				{
					return;
				}
			}
			
			this.PaintBackgroundImplementation (e.Graphics, e.ClipRectangle);
		}
		
		protected virtual void OnPaintForeground(PaintEventArgs e)
		{
			PaintEventHandler handler = this.GetUserEventHandler ("PaintForeground") as PaintEventHandler;
			
			if (handler != null)
			{
				e.Suppress = false;
				
				handler (this, e);
				
				if (e.Suppress)
				{
					return;
				}
			}
			
			this.PaintForegroundImplementation (e.Graphics, e.ClipRectangle);
		}
		
		protected virtual void OnAdornerChanged()
		{
			if (this.AdornerChanged != null)
			{
				this.AdornerChanged (this);
			}
		}
		
		protected virtual void OnCultureChanged()
		{
			if (this.CultureChanged != null)
			{
				this.CultureChanged (this);
			}
		}
		
		protected virtual void OnPressed(MessageEventArgs e)
		{
			if (this.Pressed != null)
			{
				if (e != null)
				{
					e.Message.Consumer = this;
				}
				
				this.Pressed (this, e);
			}
			
			if ((this.InternalState & InternalState.ExecCmdOnPressed) != 0)
			{
				this.ExecuteCommand ();
			}
		}
		
		protected virtual void OnReleased(MessageEventArgs e)
		{
			if (this.Released != null)
			{
				if (e != null)
				{
					e.Message.Consumer = this;
				}
				
				this.Released (this, e);
			}
		}
		
		protected virtual void OnClicked(MessageEventArgs e)
		{
			if (this.hypertext != null)
			{
				if (e != null)
				{
					this.OnHypertextClicked (e);
					
					if (e.Message.Consumer != null)
					{
						return;
					}
				}
			}
			
			if (this.Clicked != null)
			{
				if (e != null)
				{
					e.Message.Consumer = this;
				}
				
				this.Clicked (this, e);
			}

			if ((this.InternalState & InternalState.ExecCmdOnPressed) == 0)
			{
				this.ExecuteCommand ();
			}
		}
		
		protected virtual void OnDoubleClicked(MessageEventArgs e)
		{
			if (this.DoubleClicked != null)
			{
				e.Message.Consumer = this;
				this.DoubleClicked (this, e);
			}
		}
		
		protected virtual void OnEntered(MessageEventArgs e)
		{
			Window window = this.Window;
				
			if (window != null)
			{
				window.MouseCursor = this.MouseCursor;
				
				if (window.CapturingWidget == this)
				{
					e.Message.FilterNoChildren = true;
					e.Message.Captured         = true;
				}
			}
			
			if (this.Entered != null)
			{
				if (e != null)
				{
//-					e.Message.Consumer = this;
				}
				
				this.Entered (this, e);
			}
		}
		
		protected virtual void OnExited(MessageEventArgs e)
		{
			if (this.Parent != null)
			{
				Window window = this.Window;
				
				if (window != null)
				{
					if (window.CapturingWidget == null)
					{
						window.MouseCursor = this.Parent.MouseCursor;
					}
				}
			}
			
			if (this.Exited != null)
			{
				if (e != null)
				{
//-					e.Message.Consumer = this;
				}
				
				this.Exited (this, e);
			}
		}
		
		protected virtual void OnShortcutPressed()
		{
			if (this.ShortcutPressed != null)
			{
				this.ShortcutPressed (this);
			}
		}
		
		protected virtual void OnShortcutChanged()
		{
			if (this.ShortcutChanged != null)
			{
				this.ShortcutChanged (this);
			}
		}
		
		protected virtual void OnHypertextHot()
		{
			Window window = this.Window;
			if (window != null)
			{
				if (this.hypertext == null)
				{
					window.MouseCursor = this.MouseCursor;
				}
				else
				{
					window.MouseCursor = MouseCursor.AsHand;
				}
			}
			
			if (this.HypertextHot != null)
			{
				this.HypertextHot (this);
			}
		}
		
		protected virtual void OnHypertextClicked(MessageEventArgs e)
		{
			if (this.HypertextClicked != null)
			{
				e.Message.Consumer = this;
				this.HypertextClicked (this, e);
			}
		}
		
		protected virtual void OnNameChanged()
		{
			if (this.NameChanged != null)
			{
				this.NameChanged (this);
			}
		}
		
		protected virtual void OnTextDefined()
		{
			if (this.TextDefined != null)
			{
				this.TextDefined (this);
			}
		}
		
		protected virtual void OnTextChanged()
		{
			if (this.TextChanged != null)
			{
				this.TextChanged (this);
			}
			if (this.Validator != null)
			{
				this.Validator.MakeDirty (true);
			}
		}

		protected virtual void OnIconNameChanged(string oldIconName, string newIconName)
		{
		}

		
		
		
		protected virtual void OnSelected()
		{
			if (this.Selected != null)
			{
				this.Selected (this);
			}
		}
		
		protected virtual void OnDeselected()
		{
			if (this.Deselected != null)
			{
				this.Deselected (this);
			}
		}
		
		protected virtual void OnEngaged()
		{
			if (this.Engaged != null)
			{
				this.Engaged (this);
			}
		}
		
		protected virtual void OnDisengaged()
		{
			if (this.Disengaged != null)
			{
				this.Disengaged (this);
			}
		}
		
		protected virtual void OnStillEngaged()
		{
			if (this.StillEngaged != null)
			{
				this.StillEngaged (this);
			}
		}
		
		protected override void OnActiveStateChanged()
		{
			if ((this.ActiveState == ActiveState.Yes) &&
				(this.Group != null) &&
				(this.Group.Length > 0))
			{
				//	Eteint les autres boutons du groupe (s'il y en a) :
				
				Helpers.GroupController controller = Helpers.GroupController.GetGroupController (this);
				
				controller.TurnOffAllButOne (this);
				controller.SetActiveIndex (this.Index);
			}
			
			if (this.ActiveStateChanged != null)
			{
				this.ActiveStateChanged (this);
			}
		}
		
		protected virtual void OnValidatorChanged()
		{
			if (this.ValidatorChanged != null)
			{
				this.ValidatorChanged (this);
			}
		}
		
		protected virtual void OnResourceManagerChanged()
		{
			if (this.ResourceManagerChanged != null)
			{
				this.ResourceManagerChanged (this);
			}
		}
		
		public event PaintEventHandler				PaintBackground
		{
			add
			{
				this.AddUserEventHandler ("PaintBackground", value);
			}
			remove
			{
				this.RemoveUserEventHandler ("PaintBackground", value);
			}
		}
		
		public event PaintEventHandler				PaintForeground
		{
			add
			{
				this.AddUserEventHandler ("PaintForeground", value);
			}
			remove
			{
				this.RemoveUserEventHandler ("PaintForeground", value);
			}
		}
		
		#region Events
		public event Support.EventHandler			ClientGeometryUpdated;
		public event Support.EventHandler			PreparePaint;
		public event Support.EventHandler			AdornerChanged;
		public event Support.EventHandler			CultureChanged;
		
		public event MessageEventHandler			Pressed;
		public event MessageEventHandler			Released;
		public event MessageEventHandler			Clicked;
		public event MessageEventHandler			DoubleClicked;
		public event MessageEventHandler			Entered;
		public event MessageEventHandler			Exited;
		public event Support.EventHandler			ShortcutPressed;
		public event Support.EventHandler			ShortcutChanged;
		public event Support.EventHandler			HypertextHot;
		public event MessageEventHandler			HypertextClicked;
		public event Support.EventHandler			ValidatorChanged;
		public event Support.EventHandler			ResourceManagerChanged;
		
		public event MessageEventHandler			PreProcessing;
		public event MessageEventHandler			PostProcessing;
		
		public event Support.EventHandler			Selected;
		public event Support.EventHandler			Deselected;
		public event Support.EventHandler			Engaged;
		public event Support.EventHandler			StillEngaged;
		public event Support.EventHandler			Disengaged;
		public event Support.EventHandler			ActiveStateChanged;
		public event Support.EventHandler			Disposed;
		public event Support.EventHandler			TextDefined;
		public event Support.EventHandler			TextChanged;
		public event Support.EventHandler			NameChanged;
		#endregion
		
		#region Various enums
		public enum Setting : byte
		{
			None				= 0,
			IncludeChildren		= 1
		}
		
		[System.Flags] public enum TabNavigationMode
		{
			Passive				= 0,
			
			ActivateOnTab		= 0x00000001,
			ActivateOnCursorX	= 0x00000002,
			ActivateOnCursorY	= 0x00000004,
			ActivateOnCursor	= ActivateOnCursorX + ActivateOnCursorY,
			ActivateOnPage		= 0x00000008,
			
			ForwardToChildren	= 0x00010000,		//	transmet aux widgets enfants
			ForwardOnly			= 0x00020000,		//	utilisé avec ForwardToChilden: ne prend pas le focus soi-même
			SkipIfReadOnly		= 0x00040000,		//	saute si 'read-only'
			
			ForwardTabActive	= ActivateOnTab | ForwardToChildren,
			ForwardTabPassive	= ActivateOnTab | ForwardToChildren | ForwardOnly,
		}
		
		public enum TabNavigationDir
		{
			None				=  0,
			Forwards			=  1,
			Backwards			= -1
		}
		
		[System.Flags] public enum ChildFindMode
		{
			All					= 0,
			SkipHidden			= 0x00000001,
			SkipDisabled		= 0x00000002,
			SkipTransparent		= 0x00000004,
			SkipEmbedded		= 0x00000008,
			SkipNonContainer	= 0x00000010,
			SkipMask			= 0x000000ff,
			
			Deep				= 0x00010000
		}
		#endregion
		
		#region HypertextInfo class
		protected sealed class HypertextInfo : System.ICloneable, System.IComparable
		{
			internal HypertextInfo(TextLayout layout, Drawing.Rectangle bounds, int index)
			{
				this.layout = layout;
				this.bounds = bounds;
				this.index  = index;
			}
			
			
			#region ICloneable Members
			public object Clone()
			{
				return new HypertextInfo (this.layout, this.bounds, this.index);
			}
			#endregion

			#region IComparable Members
			public int CompareTo(object obj)
			{
				if (obj == null)
				{
					return 1;
				}
				
				HypertextInfo that = obj as HypertextInfo;
				
				if ((that == null) || (that.layout != this.layout))
				{
					throw new System.ArgumentException ("Invalid argument");
				}
				
				return this.index.CompareTo (that.index);
			}
			#endregion
			
			public override bool Equals(object obj)
			{
				return this.CompareTo (obj) == 0;
			}
		
			public override int GetHashCode()
			{
				return this.index;
			}
			
			
			public Drawing.Rectangle		Bounds
			{
				get { return this.bounds; }
			}
			
			public string					Anchor
			{
				get { return this.layout.FindAnchor (this.index); }
			}
			
			
			private TextLayout				layout;
			private Drawing.Rectangle		bounds;
			private int						index;
		}
		#endregion

		private static void SetTextValue(DependencyObject o, object value)
		{
			Widget that = o as Widget;
			that.Text = (string) value;
		}
		
		private static object GetTextValue(DependencyObject o)
		{
			Widget that = o as Widget;
			return that.Text;
		}

		private static void NotifyTextChanged(DependencyObject o, object oldValue, object newValue)
		{
			Widget that = o as Widget;
			string oldText = oldValue as string;
			string newText = newValue as string;

			if (string.IsNullOrEmpty (newText))
			{
				that.DisposeTextLayout ();
			}
			else
			{
				that.ModifyTextLayout (newText);
			}

			that.OnTextChanged ();
			that.Invalidate ();

			if (that.AutoMnemonic)
			{
				that.ResetMnemonicShortcut ();
			}
		}

		private static void NotifyIconNameChanged(DependencyObject o, object oldValue, object newValue)
		{
			Widget that = o as Widget;
			string oldIconName = oldValue as string;
			string newIconName = newValue as string;
			that.OnIconNameChanged (oldIconName, newIconName);
		}

		public static readonly DependencyProperty TextProperty = DependencyProperty.Register ("Text", typeof (string), typeof (Widget), new DependencyPropertyMetadata (Widget.GetTextValue, Widget.SetTextValue, Widget.NotifyTextChanged));
		public static readonly DependencyProperty TabIndexProperty = DependencyProperty.Register ("TabIndex", typeof (int), typeof (Widget), new DependencyPropertyMetadata (0));
		public static readonly DependencyProperty IconNameProperty = DependencyProperty.Register ("IconName", typeof (string), typeof (Widget), new Helpers.VisualPropertyMetadata (null, Widget.NotifyIconNameChanged, Helpers.VisualPropertyMetadataOptions.AffectsDisplay));
		
		private InternalState					internal_state;
		
		private System.Collections.ArrayList	hypertext_list;
		private HypertextInfo					hypertext;
		
		private TextLayout						text_layout;
		private TabNavigationMode				tab_navigation_mode;
		private Collections.ShortcutCollection	shortcuts;
		private double							default_font_height;
		private MouseCursor						mouse_cursor;
		private Support.ResourceManager			resource_manager;
		
		static List<Widget>						enteredWidgets = new List<Widget> ();
		static List<System.WeakReference>		aliveWidgets = new List<System.WeakReference> ();
		static bool								debugDispose;
	}
}
