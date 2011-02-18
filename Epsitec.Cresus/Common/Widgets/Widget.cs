//	Copyright © 2003-2010, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Types;
using Epsitec.Common.Widgets.Helpers;

using System.Collections.Generic;
using System.Linq;

[assembly: Epsitec.Common.Types.DependencyClass (typeof (Epsitec.Common.Widgets.Widget))]

namespace Epsitec.Common.Widgets
{
	/// <summary>
	/// La classe Widget implémente la classe de base dont dérivent tous les
	/// widgets de l'interface graphique ("controls" dans l'appellation Windows).
	/// </summary>
	public class Widget : Visual, Collections.IShortcutCollectionHost, Support.IIsDisposed
	{
		public Widget()
		{
			if (Widget.DebugDispose)
			{
				System.Diagnostics.Debug.WriteLine (string.Format ("{1}+ Created {0}", this.GetType ().Name, this.GetVisualSerialId ()));
			}

			this.InternalState |= WidgetInternalState.WasValid;
			this.InternalState |= WidgetInternalState.AutoMnemonic;

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
			//	This static constructor can either be called implicitely by running the
			//	code, or by the dependency class manager when it is analysing a freshly
			//	loaded assembly.

			//	If we get called through the dependency class manager, we may not yet
			//	initialize the resources, as the other Widget classes are not yet known
			//	to the dependency object (de)serializer. The safe-guard is implemented
			//	by delegating the initialization to the dependency class manager :

			Epsitec.Common.Types.Serialization.DependencyClassManager.ExecuteInitializationCode (
				delegate ()
				{
					FontPreviewer.Initialize ();

					Platform.Window.Initialize ();
					Res.Initialize ();
					ApplicationCommands.Initialize ();

					Support.ImageProvider.Initialize ();
				});
			
			System.Threading.Thread thread = System.Threading.Thread.CurrentThread;
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
				
				this.internalState &= ~WidgetInternalState.AutoMnemonic;
			}
			
			this.OnShortcutChanged ();
		}
		#endregion
		
		protected override void Dispose(bool disposing)
		{
			if (Widget.DebugDispose)
			{
				System.Diagnostics.Debug.WriteLine (string.Format ("{2}- {0} widget {1}", (disposing ? "Disposing" : "Collecting"), this.ToString (), this.GetVisualSerialId ()));
			}
			
			if (disposing)
			{
				this.internalState |= WidgetInternalState.Disposing;
				
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
				
				this.internalState |= WidgetInternalState.Disposed;
			}
		}
		
		
		#region Debugging Support
		public bool							DebugActive
		{
			get
			{
				return (this.internalState & WidgetInternalState.DebugActive) != 0;
			}
			set
			{
				if (value)
				{
					this.internalState |= WidgetInternalState.DebugActive;
				}
				else
				{
					this.internalState &= ~ WidgetInternalState.DebugActive;
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
					
					foreach (System.WeakReference weakRef in Widget.aliveWidgets)
					{
						if (weakRef.IsAlive)
						{
							alive.Add (weakRef);
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
					foreach (System.WeakReference weakRef in Widget.aliveWidgets)
					{
						if (weakRef.IsAlive)
						{
							alive.Add (weakRef.Target);
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
				return this.mouseCursor == null ? MouseCursor.Default : this.mouseCursor;
			}
			set
			{
				if (this.mouseCursor != value)
				{
					this.mouseCursor = value;
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

				Layouts.LayoutMeasure measureDx = Layouts.LayoutMeasure.GetWidth (this);
				Layouts.LayoutMeasure measureDy = Layouts.LayoutMeasure.GetHeight (this);

				if (measureDx != null)
				{
					width = System.Math.Max (width, measureDx.Min);
				}
				if (measureDy != null)
				{
					height = System.Math.Max (height, measureDy.Min);
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

				Layouts.LayoutMeasure measureDx = Layouts.LayoutMeasure.GetWidth (this);
				Layouts.LayoutMeasure measureDy = Layouts.LayoutMeasure.GetHeight (this);

				if (measureDx != null)
				{
					width = System.Math.Min (width, measureDx.Max);
				}
				if (measureDy != null)
				{
					height = System.Math.Min (height, measureDy.Max);
				}

				return new Drawing.Size (width, height);
			}
		}
		
		
		public static double						DefaultFontHeight
		{
			get
			{
				return System.Math.Round (Drawing.TextStyle.Default.FontSize * 1.2 + 1);
			}
		}
		
		public virtual bool							IsFrozen
		{
			get
			{
				if ((this.internalState & WidgetInternalState.Frozen) != 0)
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
				return (this.internalState & WidgetInternalState.Embedded) != 0;
			}
		}
		
		public bool									IsEditionEnabled
		{
			get
			{
				if ((this.internalState & WidgetInternalState.EditionEnabled) != 0)
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
				bool enabled = (this.internalState & WidgetInternalState.EditionEnabled) != 0;
				
				if (enabled != value)
				{
					if (value)
					{
						this.internalState |= WidgetInternalState.EditionEnabled;
					}
					else
					{
						this.internalState &= ~WidgetInternalState.EditionEnabled;
					}
				}
			}
		}
		
		public bool									IsDisposing
		{
			get
			{
				return (this.internalState & WidgetInternalState.Disposing) != 0;
			}
		}

		#region IIsDisposed Members

		public bool IsDisposed
		{
			get
			{
				return (this.internalState & WidgetInternalState.Disposed) != 0;
			}
		}

		#endregion

		public bool									ExecuteCommandOnPressed
		{
			get
			{
				return (this.InternalState & WidgetInternalState.ExecCmdOnPressed) != 0;
			}
			set
			{
				if (value)
				{
					this.InternalState |= WidgetInternalState.ExecCmdOnPressed;
				}
				else
				{
					this.InternalState &= ~ WidgetInternalState.ExecCmdOnPressed;
				}
			}
		}


		public bool									AutoMnemonic
		{
			get
			{
				return (this.internalState & WidgetInternalState.AutoMnemonic) != 0;
			}
			set
			{
				if (this.AutoMnemonic != value)
				{
					if (value)
					{
						this.internalState |= WidgetInternalState.AutoMnemonic;
					}
					else
					{
						this.internalState &= ~WidgetInternalState.AutoMnemonic;
					}

					this.ResetMnemonicShortcut ();
				}
			}
		}

		public bool									AutoFitWidth
		{
			get
			{
				return (this.internalState & WidgetInternalState.AutoFitWidth) != 0;
			}
			set
			{
				if (this.AutoFitWidth != value)
				{
					if (value)
					{
						this.internalState |= WidgetInternalState.AutoFitWidth;
					}
					else
					{
						this.internalState &= ~WidgetInternalState.AutoFitWidth;
					}
				}
			}
		}

		protected WidgetInternalState				InternalState
		{
			get
			{
				return this.internalState;
			}
			set
			{
				this.internalState = value;
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
				if (this.UndefinedLanguage)
				{
					state |= WidgetPaintState.UndefinedLanguage;
				}
				if (this.IsEnabled)
				{
					state |= WidgetPaintState.Enabled;
				}
				if (this.IsFocused)
				{
					state |= WidgetPaintState.InheritedFocus;
				}
				if (this.KeyboardFocus)
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
			get { return ((this.internalState & WidgetInternalState.Selectable) != 0) && !this.IsFrozen; }
		}
		
		public bool									CanEngage
		{
			get { return ((this.internalState & WidgetInternalState.Engageable) != 0) && this.IsEnabled && !this.IsFrozen; }
		}

		public virtual bool							AcceptsFocus
		{
			get
			{
				return ((this.internalState & WidgetInternalState.Focusable) != 0)
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
			get { return ((this.internalState & WidgetInternalState.PossibleContainer) != 0) && !this.IsFrozen; }
		}
		
		
		public new virtual Widget					Parent
		{
			get
			{
				return base.Parent as Widget;
			}
			set
			{
				this.SetParent (value);
			}
		}
		
		public Support.OpletQueue					OpletQueue
		{
			get
			{
				return VisualTree.GetOpletQueue (this);
			}
			set
			{
				throw new System.InvalidOperationException ("Cannot set OpletQueue on Widget.");
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
		
		public virtual bool							HasTextLabel
		{
			get
			{
				return true;
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

		public FormattedText						FormattedText
		{
			get
			{
				return new FormattedText (this.Text);
			}
			set
			{
				this.Text = value == null ? null : value.ToString ();
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
					if (this.textLayout == null)
					{
						this.CreateTextLayout ();
					}
					
					this.ModifyText (value);
				}
			}
		}

		public virtual TextLayout					TextLayout
		{
			get
			{
				return this.textLayout;
			}
		}
		
		public Drawing.TextBreakMode				TextBreakMode
		{
			get
			{
				if (this.textLayout != null)
				{
					return this.textLayout.BreakMode;
				}
				
				return Drawing.TextBreakMode.None;
			}
			set
			{
				if (this.textLayout == null)
				{
					this.CreateTextLayout ();
				}
				
				if (this.textLayout.BreakMode != value)
				{
					this.textLayout.BreakMode = value;
					this.Invalidate ();
				}
			}
		}

		public string								IconUri
		{
			get
			{
				return (string) this.GetValue (Widget.IconUriProperty);
			}
			set
			{
				if (string.IsNullOrEmpty (value))
				{
					this.ClearValue (Widget.IconUriProperty);
				}
				else
				{
					this.SetValue (Widget.IconUriProperty, value);
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
					
					if ((this.TabNavigationMode == TabNavigationMode.None) &&
						(value > 0))
					{
						this.TabNavigationMode = TabNavigationMode.ActivateOnTab;
					}
					else if ((this.TabNavigationMode == TabNavigationMode.ActivateOnTab) &&
						/**/ (value <= 0))
					{
						this.TabNavigationMode = TabNavigationMode.None;
					}
				}
			}
		}
		
		public virtual TabNavigationMode			TabNavigationMode
		{
			get
			{
				return (TabNavigationMode) this.GetValue (Widget.TabNavigationModeProperty);
			}
			set
			{
				this.SetValue (Widget.TabNavigationModeProperty, value);
			}
		}

		public Widget								ForwardTabOverride
		{
			get
			{
				return (Widget) this.GetValue (Widget.ForwardTabOverrideProperty);
			}
			set
			{
				if (value == null)
				{
					this.ClearValue (Widget.ForwardTabOverrideProperty);
				}
				else
				{
					this.SetValue (Widget.ForwardTabOverrideProperty, value);
				}
			}
		}

		public Widget								BackwardTabOverride
		{
			get
			{
				return (Widget) this.GetValue (Widget.BackwardTabOverrideProperty);
			}
			set
			{
				if (value == null)
				{
					this.ClearValue (Widget.BackwardTabOverrideProperty);
				}
				else
				{
					this.SetValue (Widget.BackwardTabOverrideProperty, value);
				}
			}
		}

		public Widget								ForwardEnterTabOverride
		{
			get
			{
				return (Widget) this.GetValue (Widget.ForwardEnterTabOverrideProperty);
			}
			set
			{
				if (value == null)
				{
					this.ClearValue (Widget.ForwardEnterTabOverrideProperty);
				}
				else
				{
					this.SetValue (Widget.ForwardEnterTabOverrideProperty, value);
				}
			}
		}

		public Widget								BackwardEnterTabOverride
		{
			get
			{
				return (Widget) this.GetValue (Widget.BackwardEnterTabOverrideProperty);
			}
			set
			{
				if (value == null)
				{
					this.ClearValue (Widget.BackwardEnterTabOverrideProperty);
				}
				else
				{
					this.SetValue (Widget.BackwardEnterTabOverrideProperty, value);
				}
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


		public virtual double						AutoEngageDelay
		{
			get
			{
				return SystemInformation.InitialKeyboardDelay;
			}
		}

		public virtual double						AutoEngageRepeatPeriod
		{
			get
			{
				return SystemInformation.KeyboardRepeatPeriod;
			}
		}

		public Widget								Embedder
		{
			get
			{
				if ((this.internalState & WidgetInternalState.Embedded) == 0)
				{
					return null;
				}
				else
				{
					return this.Parent;
				}
			}
			set
			{
				this.SetEmbedder (value);
			}
		}
		

		public static readonly IComparer<Widget>	TabIndexComparer = new TabIndexComparerImplementation ();



		
		public virtual Drawing.Size GetBestFitSize()
		{
			Types.DependencyPropertyMetadata metadataDx = Visual.PreferredWidthProperty.GetMetadata (this);
			Types.DependencyPropertyMetadata metadataDy = Visual.PreferredHeightProperty.GetMetadata (this);

			return new Drawing.Size ((double) metadataDx.DefaultValue, (double) metadataDy.DefaultValue);
		}

		public void UpdatePreferredSize()
		{
			this.PreferredSize = this.GetBestFitSize ();
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
			bool oldValid = (this.internalState & WidgetInternalState.WasValid) != 0;
			bool newValid;

			using (ValidationContext.ValidationInProgress (this))
			{
				newValid = this.IsValid;
			}

			if (oldValid != newValid)
			{
				if (newValid)
				{
					this.internalState |= WidgetInternalState.WasValid;
				}
				else
				{
					this.internalState &= ~WidgetInternalState.WasValid;
				}

				this.InvalidateProperty (Visual.IsValidProperty, oldValid, newValid);
				
				this.SetError (newValid == false);

				string validationGroups = VisualTree.GetValidationGroups (this);
				
				if (validationGroups != null)
				{
					VisualTree.UpdateCommandEnable (this);
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
			if ((this.internalState & WidgetInternalState.Frozen) == 0)
			{
				if (frozen)
				{
					this.internalState |= WidgetInternalState.Frozen;
					this.Invalidate (InvalidateReason.FrozenChanged);
				}
			}
			else
			{
				if (!frozen)
				{
					this.internalState &= ~ WidgetInternalState.Frozen;
					this.Invalidate (InvalidateReason.FrozenChanged);
				}
			}
		}

		public virtual void SetError(bool value)
		{
			if (this.InError != value)
			{
				this.SetValue (Visual.InErrorProperty, value);
				
				this.UpdateCaption (updateIconUri: false);

				if ((value) &&
					(this.Validator != null))
				{
					FormattedText error = this.Validator.ErrorMessage;

					if (! error.IsNullOrWhiteSpace)
					{
						ToolTip.SetToolTipColor (this, Widgets.Adorners.Factory.Active.ColorError);
						ToolTip.Default.SetToolTip (this, error);
					}
				}
			}
		}

		public virtual void SetUndefinedLanguage(bool value)
		{
			if (this.UndefinedLanguage != value)
			{
				this.SetValue (Visual.UndefinedLanguageProperty, value);
			}
		}

		public bool Focus()
		{
			Window window = this.Window;

			if (window != null)
			{
				return window.FocusWidget (this);
			}
			else
			{
				return false;
			}
		}

		public void ClearFocus()
		{
			if (this.KeyboardFocus)
			{
				Window window = this.Window;

				if ((window != null) &&
					(window.FocusedWidget == this))
				{
					window.ClearFocusedWidget ();
				}
				else
				{
					this.SetFocused (false);
				}
			}
		}
		
		internal void SetFocused(bool focused)
		{
			//	Utiliser Focus() en lieu et place de SetFocused(true), pour
			//	avoir une gestion complète des conditions de focus.
			
			bool oldFocus = this.KeyboardFocus;
			bool newFocus = focused;
			
			if (oldFocus == newFocus)
			{
				return;
			}
			
			Window window = this.Window;

			if (newFocus)
			{
				if (window != null)
				{
					this.SetValue (Visual.KeyboardFocusProperty, true);
//-					window.FocusedWidget = this;
				}
			}
			else
			{
				this.SetValue (Visual.KeyboardFocusProperty, false);
				this.ClearValue (Visual.KeyboardFocusProperty);
				
//-				if (window != null)
//-				{
//-					window.FocusedWidget = null;
//-				}
			}

			this.Invalidate (InvalidateReason.FocusedChanged);
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
			
			if ((this.internalState & WidgetInternalState.Engageable) == 0)
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

		
		public static WidgetPaintState ConstrainPaintState(WidgetPaintState state)
		{
			if ((state & WidgetPaintState.Enabled) == 0)
			{
				state &= ~WidgetPaintState.Focused;
				state &= ~WidgetPaintState.Entered;
				state &= ~WidgetPaintState.Engaged;
			}
			
			return state;
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
		
		public void SimulateClicked()
		{
			if (this.IsEnabled)
			{
				this.OnClicked (null);
			}
		}

		
		public void SetEmbedder(Widget embedder)
		{
			this.SetParent (embedder);
			this.internalState |= WidgetInternalState.Embedded;
		}
		
		
		public override string ToString()
		{
			System.Text.StringBuilder buffer = new System.Text.StringBuilder ();
			
			buffer.Append (this.GetVisualSerialId ().ToString ());
			buffer.Append (":");
			buffer.Append (this.GetType ().Name);
			this.BuildFullPathName (buffer);
			buffer.Append (@".""");
			buffer.Append (this.Text);
			buffer.Append (@"""");
			
			return buffer.ToString ();
		}


		internal void InternalSetEntered()
		{
			this.SetEntered (true);
		}
		
		protected void SetEntered(bool value)
		{
			if (this.IsEntered != value)
			{
				Window window = this.Window;
				Message message = null;
				
				if (value)
				{
					if ((this.Parent != null) &&
						(this.Parent.IsEntered == false) &&
						(!(this.Parent is WindowRoot)))
					{
						this.Parent.SetEntered (true);
					}
					
					Widget.ExitWidgetsNotParentOf (this);
					Widget.enteredWidgets.Add (this);

					this.SetValue (Visual.EnteredProperty, value);
					
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
				this.Invalidate ();
				
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
				
				if (VisualTree.IsAncestor (widget, candidate) == false)
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


		internal static void ClearEntered(Window window)
		{
			while (Widget.enteredWidgets.Count > 0)
			{
				Widget widget = Widget.enteredWidgets[0];
				widget.SetEntered (false);
			}
		}

		internal static void UpdateEntered(Window window, Message message)
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
		
		internal static void UpdateEntered(Window window, Widget widget, Message message)
		{
			if (widget == null)
			{
				return;
			}

			Drawing.Point pointInWidget = widget.MapRootToClient (message.Cursor);
			
			if ((widget.Window != window) ||
				(pointInWidget.X < 0) ||
				(pointInWidget.Y < 0) ||
				(pointInWidget.X >= widget.Client.Size.Width) ||
				(pointInWidget.Y >= widget.Client.Size.Height) ||
				(message.MessageType == MessageType.MouseLeave))
			{
				widget.SetEntered (false);

				if ((window.EngagedWidget == widget) &&
					(window.CapturingWidget != widget))
				{
					window.EngagedWidget = null;
				}
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

							using (window.PushPaintFilter (new WidgetSyncPaintFilter (this, window.PaintFilter)))
							{
								window.MarkForRepaint (this.MapClientToRoot (rect));
								window.SynchronousRepaint ();
							}
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
			Drawing.Point pointWdo = point;
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
			bool flipX = rect.Width < 0;
			bool flipY = rect.Height < 0;
			
			Drawing.Point p1 = this.MapRootToClient (new Drawing.Point (rect.Left, rect.Bottom));
			Drawing.Point p2 = this.MapRootToClient (new Drawing.Point (rect.Right, rect.Top));
			
			rect.X = System.Math.Min (p1.X, p2.X);
			rect.Y = System.Math.Min (p1.Y, p2.Y);
			
			rect.Width  = System.Math.Abs (p1.X - p2.X);
			rect.Height = System.Math.Abs (p1.Y - p2.Y);
			
			if (flipX) rect.FlipX ();
			if (flipY) rect.FlipY ();
			
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
			bool flipX = rect.Width < 0;
			bool flipY = rect.Height < 0;
			
			Drawing.Point p1 = this.MapClientToRoot (new Drawing.Point (rect.Left, rect.Bottom));
			Drawing.Point p2 = this.MapClientToRoot (new Drawing.Point (rect.Right, rect.Top));
			
			rect.X = System.Math.Min (p1.X, p2.X);
			rect.Y = System.Math.Min (p1.Y, p2.Y);
			
			rect.Width  = System.Math.Abs (p1.X - p2.X);
			rect.Height = System.Math.Abs (p1.Y - p2.Y);
			
			if (flipX) rect.FlipX ();
			if (flipY) rect.FlipY ();
			
			return rect;
		}
		
		
		public virtual Drawing.Rectangle MapParentToClient(Drawing.Rectangle rect)
		{
			bool flipX = rect.Width < 0;
			bool flipY = rect.Height < 0;
			
			Drawing.Point p1 = this.MapParentToClient (new Drawing.Point (rect.Left, rect.Bottom));
			Drawing.Point p2 = this.MapParentToClient (new Drawing.Point (rect.Right, rect.Top));
			
			rect.X = System.Math.Min (p1.X, p2.X);
			rect.Y = System.Math.Min (p1.Y, p2.Y);
			
			rect.Width  = System.Math.Abs (p1.X - p2.X);
			rect.Height = System.Math.Abs (p1.Y - p2.Y);
			
			if (flipX) rect.FlipX ();
			if (flipY) rect.FlipY ();
			
			return rect;
		}
		
		public virtual Drawing.Rectangle MapClientToParent(Drawing.Rectangle rect)
		{
			bool flipX = rect.Width < 0;
			bool flipY = rect.Height < 0;
			
			Drawing.Point p1 = this.MapClientToParent (new Drawing.Point (rect.Left, rect.Bottom));
			Drawing.Point p2 = this.MapClientToParent (new Drawing.Point (rect.Right, rect.Top));
			
			rect.X = System.Math.Min (p1.X, p2.X);
			rect.Y = System.Math.Min (p1.Y, p2.Y);
			
			rect.Width  = System.Math.Abs (p1.X - p2.X);
			rect.Height = System.Math.Abs (p1.Y - p2.Y);
			
			if (flipX) rect.FlipX ();
			if (flipY) rect.FlipY ();
			
			return rect;
		}

		public virtual Drawing.Rectangle MapScreenToClient(Drawing.Rectangle rect)
		{
			bool flipX = rect.Width < 0;
			bool flipY = rect.Height < 0;
			
			Drawing.Point p1 = this.MapScreenToClient (new Drawing.Point (rect.Left, rect.Bottom));
			Drawing.Point p2 = this.MapScreenToClient (new Drawing.Point (rect.Right, rect.Top));
			
			rect.X = System.Math.Min (p1.X, p2.X);
			rect.Y = System.Math.Min (p1.Y, p2.Y);
			
			rect.Width  = System.Math.Abs (p1.X - p2.X);
			rect.Height = System.Math.Abs (p1.Y - p2.Y);
			
			if (flipX) rect.FlipX ();
			if (flipY) rect.FlipY ();
			
			return rect;
		}
		
		public virtual Drawing.Rectangle MapClientToScreen(Drawing.Rectangle rect)
		{
			bool flipX = rect.Width < 0;
			bool flipY = rect.Height < 0;
			
			Drawing.Point p1 = this.MapClientToScreen (new Drawing.Point (rect.Left, rect.Bottom));
			Drawing.Point p2 = this.MapClientToScreen (new Drawing.Point (rect.Right, rect.Top));
			
			rect.X = System.Math.Min (p1.X, p2.X);
			rect.Y = System.Math.Min (p1.Y, p2.Y);
			
			rect.Width  = System.Math.Abs (p1.X - p2.X);
			rect.Height = System.Math.Abs (p1.Y - p2.Y);
			
			if (flipX) rect.FlipX ();
			if (flipY) rect.FlipY ();
			
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
			
			double modelOffset  = model.GetBaseLine ().Y;
			double widgetOffset = widget.GetBaseLine ().Y;
			
			double yBottom = model.ActualLocation.Y + modelOffset - widgetOffset;
			
			widget.SetManualBounds(new Drawing.Rectangle (widget.ActualLocation.X, yBottom, widget.ActualWidth, widget.ActualHeight));
		}
		
		
		public Widget FindChild(Drawing.Point point)
		{
			return this.FindChild (point, null, WidgetChildFindMode.SkipHidden);
		}

		public Widget FindChild(Drawing.Point point, IEnumerable<Widget> ignore)
		{
			return this.FindChild (point, ignore, WidgetChildFindMode.SkipHidden);
		}

		public Widget FindChild(Drawing.Point point, WidgetChildFindMode mode)
		{
			return this.FindChild (point, null, mode);
		}

		public virtual Widget FindChild(Drawing.Point point, IEnumerable<Widget> ignore, WidgetChildFindMode mode)
		{
			if (this.HasChildren == false)
			{
				return null;
			}
			
			Widget[] children = this.Children.Widgets;
			int  childrenNum = children.Length;
			
			for (int i = 0; i < childrenNum; i++)
			{
				Widget widget = children[childrenNum-1 - i];
				
				System.Diagnostics.Debug.Assert (widget != null);
				
				if ((mode & WidgetChildFindMode.SkipMask) != WidgetChildFindMode.All)
				{
					if ((mode & WidgetChildFindMode.SkipDisabled) != 0)
					{
						if (widget.IsEnabled == false)
						{
							continue;
						}
					}
					if ((mode & WidgetChildFindMode.SkipHidden) != 0)
					{
						if (widget.Visibility == false)
						{
							continue;
						}
					}
					if ((mode & WidgetChildFindMode.SkipNonContainer) != 0)
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
					if ((mode & WidgetChildFindMode.SkipTransparent) != 0)
					{
						//	TODO: vérifier que le point en question n'est pas transparent
					}
					
					if ((mode & WidgetChildFindMode.Deep) != 0)
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
					
					if ((mode & WidgetChildFindMode.SkipEmbedded) != 0)
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

		public Widget FindChild(Drawing.Rectangle rect, WidgetChildFindMode mode)
		{
			return this.FindChild (rect, null, mode);
		}
		
		public virtual Widget FindChild(Drawing.Rectangle rect, IEnumerable<Widget> ignore, WidgetChildFindMode mode)
		{
			if (this.HasChildren == false)
			{
				return null;
			}
			
			Widget[] children = this.Children.Widgets;
			int  childrenNum = children.Length;
			
			for (int i = 0; i < childrenNum; i++)
			{
				Widget widget = children[childrenNum-1 - i];
				
				System.Diagnostics.Debug.Assert (widget != null);
				
				if ((mode & WidgetChildFindMode.SkipMask) != WidgetChildFindMode.All)
				{
					if ((mode & WidgetChildFindMode.SkipDisabled) != 0)
					{
						if (widget.IsEnabled == false)
						{
							continue;
						}
					}
					if ((mode & WidgetChildFindMode.SkipHidden) != 0)
					{
						if (widget.Visibility == false)
						{
							continue;
						}
					}
					if ((mode & WidgetChildFindMode.SkipNonContainer) != 0)
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
					if ((mode & WidgetChildFindMode.SkipTransparent) != 0)
					{
						//	TODO: vérifier que le point en question n'est pas transparent
					}
					
					if ((mode & WidgetChildFindMode.Deep) != 0)
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
					
					if ((mode & WidgetChildFindMode.SkipEmbedded) != 0)
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
			return this.FindChild (name, WidgetChildFindMode.All);
		}
		
		public virtual Widget	FindChild(string name, WidgetChildFindMode mode)
		{
			if (this.HasChildren == false)
			{
				return null;
			}
			
			Widget[] children = this.Children.Widgets;
			int  childrenNum = children.Length;
			
			for (int i = 0; i < childrenNum; i++)
			{
				Widget widget = children[i];
				
				System.Diagnostics.Debug.Assert (widget != null);
				
				if ((mode & WidgetChildFindMode.SkipMask) != WidgetChildFindMode.All)
				{
					if ((mode & WidgetChildFindMode.SkipDisabled) != 0)
					{
						if (widget.IsEnabled == false)
						{
							continue;
						}
					}
					else if ((mode & WidgetChildFindMode.SkipHidden) != 0)
					{
						if (widget.Visibility == false)
						{
							continue;
						}
					}
					else if ((mode & WidgetChildFindMode.SkipNonContainer) != 0)
					{
						if (widget.PossibleContainer == false)
						{
							continue;
						}
					}
				}

				if (widget.Name == name)
				{
					return widget;
				}
				
				if ((widget.Name == null) ||
					(widget.Name.Length == 0) ||
					((mode & WidgetChildFindMode.Deep) != 0))
				{
					Widget child = widget.FindChild (name, mode);
					
					if (child != null)
					{
						return child;
					}
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
			int  childrenNum = children.Length;
			
			for (int i = 0; i < childrenNum; i++)
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
			
			this.WalkChildren (finder.Analyze);
			
			if (finder.Widgets.Length > 0)
			{
				return finder.Widgets[0];
			}
			
			return null;
		}

		public Widget	        FindCommandWidget(Command command)
		{
			//	Passe en revue tous les widgets de la descendance et retourne le
			//	premier qui correspond parfaitement.
			
			CommandWidgetFinder finder = new CommandWidgetFinder (command);
			
			this.WalkChildren (finder.Analyze);
			
			if (finder.Widgets.Length > 0)
			{
				return finder.Widgets[0];
			}
			
			return null;
		}


		public IEnumerable<Widget> FindAll()
		{
			List<Widget> list = new List<Widget> ();
			list.Add (this);
			this.FindAllChildren (list, null);
			return list;
		}
		
		public IEnumerable<Widget> FindAllChildren()
		{
			List<Widget> list = new List<Widget> ();
			this.FindAllChildren (list, null);
			return list;
		}

		public IEnumerable<Widget> FindAllChildren(System.Predicate<Widget> predicate)
		{
			List<Widget> list = new List<Widget> ();
			this.FindAllChildren (list, predicate);
			return list;
		}

		public Widget FindFocusedChild()
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
				
				if (VisualTree.IsDescendant (this, focused))
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
		
		private void FindAllChildren(List<Widget> list, System.Predicate<Widget> predicate)
		{
			if (this.HasChildren)
			{
				foreach (Widget child in this.Children)
				{
					if ((predicate == null) ||
						(predicate (child)))
					{
						list.Add (child);
					}
					
					child.FindAllChildren (list, predicate);
				}
			}
		}
		
		
		public static Widget[]	FindAllFullPathWidgets(System.Text.RegularExpressions.Regex regex)
		{
			//	Passe en revue absolument tous les widgets qui existent et cherche ceux qui ont
			//	une commande qui correspond au critère spécifié.
			
			System.Collections.ArrayList list = new System.Collections.ArrayList ();
			System.Collections.ArrayList dead = new System.Collections.ArrayList ();
			
			lock (Widget.aliveWidgets)
			{
				foreach (System.WeakReference weakRef in Widget.aliveWidgets)
				{
					//	On utilise la liste des widgets connus qui permet d'avoir accès immédiatement
					//	à tous les widgets sans nécessiter de descente récursive :
					
					if (weakRef.IsAlive)
					{
						//	Le widget trouvé existe (encore) :
						
						Widget widget = weakRef.Target as Widget;
						
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
						dead.Add (weakRef);
					}
				}
				
				//	Profite de l'occasion, puisqu'on vient de passer en revue tous les widgets,
				//	de supprimer ceux qui sont morts entre temps :
				
				foreach (System.WeakReference weakRef in dead)
				{
					Widget.aliveWidgets.Remove (weakRef);
				}
			}
			
			Widget[] widgets = new Widget[list.Count];
			list.CopyTo (widgets);
			
			return widgets;
		}
		
		
		public void SetFocusOnTabWidget()
		{
			int iterations = 0;
			Window window  = this.Window;
			Widget focused = window == null ? null : window.FocusedWidget;
			Widget focus   = this.FindTabWidget (TabNavigationDir.Forwards, TabNavigationMode.ActivateOnTab, false, true, ref iterations);
			
			if (focus == null)
			{
				focus = this;
			}

			if ((focused == null) ||
				(focused.InternalAboutToLoseFocus (TabNavigationDir.Forwards, TabNavigationMode.ActivateOnTab)))
			{
				if (focus.InternalAboutToGetFocus (TabNavigationDir.Forwards, TabNavigationMode.ActivateOnTab, out focus))
				{
					focus.Focus ();
				}
			}
		}
		
		public Widget FindTabWidget(TabNavigationDir dir, TabNavigationMode mode)
		{
			int iterations = 0;
			return this.FindTabWidget (dir, mode, false, false, ref iterations);
		}


		private Widget FindTabWidget(TabNavigationDir dir, TabNavigationMode mode, bool disableFirstEnter, bool acceptFocus, ref int iterations)
		{
			if (iterations > 2)
			{
				return null;
			}

			if (this.ProcessTab (dir, mode))
			{
				return this;
			}

			Widget find = null;
			
			find = this.FindTabWidgetAutoRadio (dir, mode)
				?? this.FindTabWidgetEnterChildren (dir, mode, disableFirstEnter, acceptFocus, ref iterations);

			if (find != null)
			{
				return find;
			}
			
			if (acceptFocus)
			{
				if ((this.TabNavigationMode & mode) != 0)
				{
					if ((this.TabNavigationMode & TabNavigationMode.ForwardOnly) != 0)
					{
						if (dir != TabNavigationDir.Backwards)
						{
							return null;
						}
					}
					else
					{
						return this;
					}
				}
			}

			//	Cherche parmi les frères...

			System.Diagnostics.Debug.Assert (find == null);

			find = this.FindTabWidgetOverride (dir, mode, ref iterations);

			if (find != null)
			{
				return find;
			}

			var tabSiblings = this.GetTabNavigationSiblings (dir, mode).ToArray ();
			int index = this.FindIndex (tabSiblings);

			find = ((index == -1) ? this.FindTabWidgetInChildren (dir, mode)
				/* */             : this.FindTabWidgetInSiblings (dir, mode, ref iterations, tabSiblings, index))
				?? this.FindTabWidgetInParent (dir, mode, ref iterations)
				?? this.FindTabWidgetWarp (dir, mode, ref iterations, tabSiblings);
			
			if (find != null)
			{
				System.Diagnostics.Debug.Assert ((find.TabNavigationMode & TabNavigationMode.ForwardOnly) == 0);
			}

			return find;
		}

		private int FindIndex(Widget[] tabSiblings)
		{
			for (int i = 0; i < tabSiblings.Length; i++)
			{
				if (tabSiblings[i] == this)
				{
					return i;
				}
			}

			return -1;
		}

		private Widget FindTabWidgetEnterChildren(TabNavigationDir dir, TabNavigationMode mode, bool disableFirstEnter, bool acceptFocus, ref int iterations)
		{
			if ((!disableFirstEnter) &&
				((this.TabNavigationMode & TabNavigationMode.ForwardToChildren) != 0) &&
				(this.HasChildren))
			{
				//	Ce widget permet aux enfants d'entrer dans la liste accessible par la
				//	touche TAB.

				Widget find = Widget.TabNavigateEnterOverride (this, dir, mode, ref iterations);

				if (find != null)
				{
					return find;
				}

				Widget[] candidates = this.Children.Widgets[0].GetTabNavigationSiblings (dir, mode).ToArray ();

				for (int i = 0; i < candidates.Length; i++)
				{
					if (dir == TabNavigationDir.Forwards)
					{
						find = candidates[i].FindTabWidget (dir, mode, false, true, ref iterations);
					}
					else if (acceptFocus)
					{
						int count = candidates.Length;
						find = candidates[count-i-1].FindTabWidget (dir, mode, false, true, ref iterations);
					}

					if (find != null)
					{
						return find;
					}
				}
			}
			
			return null;
		}
		private Widget FindTabWidgetAutoRadio(TabNavigationDir dir, TabNavigationMode mode)
		{
			if (this.AutoRadio == false)
			{
				return null;
			}
			
			Widget find = null;

			if (mode == TabNavigationMode.ActivateOnCursorX)
			{
				GroupController controller = GroupController.GetGroupController (this);

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
				GroupController controller = GroupController.GetGroupController (this);

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
			return null;
		}
		
		private Widget FindTabWidgetWarp(TabNavigationDir dir, TabNavigationMode mode, ref int iterations, Widget[] tabSiblings)
		{
			if (tabSiblings.Length > 1)
			{
				switch (dir)
				{
					case TabNavigationDir.Forwards:
						return tabSiblings[0].FindTabWidget (dir, mode, false, true, ref iterations);

					case TabNavigationDir.Backwards:
						return tabSiblings[tabSiblings.Length-1].FindTabWidget (dir, mode, false, true, ref iterations);
				}
			}

			return null;
		}

		private Widget FindTabWidgetInParent(TabNavigationDir dir, TabNavigationMode mode, ref int iterations)
		{
			Widget find   = null;
			Widget parent = this.Parent;

			if (parent != null)
			{
				if (parent.ProcessTabChildrenExit (dir, mode, out find))
				{
					return find;
				}

				if ((parent.TabNavigationMode & TabNavigationMode.ForwardToChildren) != 0)
				{
					bool accept;

					switch (dir)
					{
						case TabNavigationDir.Backwards:
							accept = (parent.TabNavigationMode & TabNavigationMode.ForwardOnly) == 0;
							return parent.FindTabWidget (dir, mode, true, accept, ref iterations);

						case TabNavigationDir.Forwards:
							accept = false;
							return parent.FindTabWidget (dir, mode, true, accept, ref iterations);
					}
				}
				else
				{
					return parent.FindTabWidget (dir, mode, true, false, ref iterations);
				}
			}
			else if (this.HasChildren)
			{
				//	Il n'y a plus de parents au-dessus. C'est donc vraisemblablement WindowRoot et
				//	dans ce cas, il ne sert à rien de boucler. On va simplement tenter d'activer le
				//	premier descendant trouvé :

				Widget[] candidates = this.Children.Widgets[0].GetTabNavigationSiblings (dir, mode).ToArray ();
				iterations++;

				for (int i = 0; i < candidates.Length; i++)
				{
					if (dir == TabNavigationDir.Forwards)
					{
						find = candidates[i].FindTabWidget (dir, mode, false, true, ref iterations);
					}
					else
					{
						int count = candidates.Length;
						find = candidates[count-1-i].FindTabWidget (dir, mode, false, true, ref iterations);
					}

					if (find != null)
					{
						return find;
					}
				}
			}
			
			return null;
		}
		
		private Widget FindTabWidgetOverride(TabNavigationDir dir, TabNavigationMode mode, ref int iterations)
		{
			Widget find = null;

			switch (dir)
			{
				case TabNavigationDir.Forwards:
					find = this.ForwardTabOverride;
					break;

				case TabNavigationDir.Backwards:
					find = this.BackwardTabOverride;
					break;
			}

			if (find != null)
			{
				find = Widget.TabNavigateSibling (find, dir, mode, ref iterations);
			}

			return find;
		}

		private Widget FindTabWidgetInChildren(TabNavigationDir dir, TabNavigationMode mode)
		{
			Widget find = this;

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

				if (find == null)
				{
					return null;
				}

				if ((find.TabNavigationMode & mode) != 0)
				{
					return find;
				}
			}
		}

		private Widget FindTabWidgetInSiblings(TabNavigationDir dir, TabNavigationMode mode, ref int iterations, Widget[] tabSiblings, int i)
		{
			Widget find = null;

			while (true)
			{
				switch (dir)
				{
					case TabNavigationDir.Backwards:
						find = this.TabNavigate (i--, dir, tabSiblings);
						break;

					case TabNavigationDir.Forwards:
						find = this.TabNavigate (i++, dir, tabSiblings);
						break;
				}

				if (find == null)
				{
					return find;
				}

				find = Widget.TabNavigateSibling (find, dir, mode, ref iterations);

				if (find != null)
				{
					return find;
				}
			}
		}

		
		/// <summary>
		/// Finds the definitive widget based on an initial result obtained by
		/// navigating to a sibling. This method will enter groups, if needed.
		/// </summary>
		/// <param name="sibling">The sibling.</param>
		/// <param name="dir">The tab navigation direction.</param>
		/// <param name="mode">The tab navigation mode.</param>
		/// <param name="iterations">The iteration counter.</param>
		/// <returns>The definitive widget.</returns>
		private static Widget TabNavigateSibling(Widget sibling, TabNavigationDir dir, TabNavigationMode mode, ref int iterations)
		{
			if (sibling != null)
			{
				if (dir == TabNavigationDir.Backwards)
				{
					if ((sibling.TabNavigationMode & TabNavigationMode.ForwardToChildren) != 0)
					{
						//	Entre en marche arrière dans le widget...

						if (sibling.HasChildren)
						{
							Widget[] candidates = sibling.Children.Widgets[0].GetTabNavigationSiblings (dir, mode).ToArray ();
							Widget   widget     = Widget.TabNavigateEnterOverride (sibling, dir, mode, ref iterations);

							if (widget != null)
							{
								return widget;
							}

							if (candidates.Length > 0)
							{
								int count = candidates.Length;
								sibling = candidates[count-1].FindTabWidget (dir, mode, false, true, ref iterations);
							}
							else if ((sibling.TabNavigationMode & TabNavigationMode.ForwardOnly) != 0)
							{
								sibling = null;
							}
						}
						else if ((sibling.TabNavigationMode & TabNavigationMode.ForwardOnly) != 0)
						{
							sibling = null;
						}
					}
				}
				else if (dir == TabNavigationDir.Forwards)
				{
					if (((sibling.TabNavigationMode & TabNavigationMode.ForwardToChildren) != 0) &&
						((sibling.TabNavigationMode & TabNavigationMode.ForwardOnly) != 0))
					{
						if (sibling.HasChildren)
						{
							//	Entre en marche avant dans le widget...

							Widget[] candidates = sibling.Children.Widgets[0].GetTabNavigationSiblings (dir, mode).ToArray ();
							Widget   widget     = Widget.TabNavigateEnterOverride (sibling, dir, mode, ref iterations);
							
							if (widget != null)
							{
								return widget;
							}

							sibling = null;

							foreach (Widget candidate in candidates)
							{
								sibling = candidate.FindTabWidget (dir, mode, false, true, ref iterations);

								if (sibling != null)
								{
									break;
								}
							}
						}
						else
						{
							sibling = null;
						}
					}
				}
			}

			return sibling;
		}

		/// <summary>
		/// Navigates to the child specified by the enter override property.
		/// </summary>
		/// <param name="widget">The widget.</param>
		/// <param name="dir">The tab navigation direction.</param>
		/// <param name="mode">The tab navigation mode.</param>
		/// <param name="iterations">The iteration counter.</param>
		/// <returns>
		/// The override, or <c>null</c> if the default navigation should be used.
		/// </returns>
		private static Widget TabNavigateEnterOverride(Widget widget, TabNavigationDir dir, TabNavigationMode mode, ref int iterations)
		{
			System.Diagnostics.Debug.Assert (widget != null);

			Widget find = null;
			
			switch (dir)
			{
				case TabNavigationDir.Forwards:
					find = widget.ForwardEnterTabOverride;
					break;

				case TabNavigationDir.Backwards:
					find = widget.BackwardEnterTabOverride;
					break;
			}

			if (find != null)
			{
				find = find.FindTabWidget (dir, mode, false, true, ref iterations);
			}

			return find;
		}

		protected virtual Widget TabNavigate(int index, TabNavigationDir dir, Widget[] siblings)
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
		
		protected virtual List<Widget> GetTabNavigationSiblings(TabNavigationDir dir, TabNavigationMode mode)
		{
			List<Widget> list = new List<Widget> ();
			
			Widget parent = this.Parent;
			
			if (parent != null)
			{
				Widget[] siblings = parent.Children.Widgets;
				
				for (int i = 0; i < siblings.Length; i++)
				{
					Widget sibling = siblings[i];
					
					if (((sibling.TabNavigationMode & mode) != 0) &&
						(sibling.IsEnabled) &&
						(sibling.Visibility) &&
						(sibling.AcceptsFocus))
					{
						if (((sibling.TabNavigationMode & TabNavigationMode.SkipIfReadOnly) != 0) &&
							(sibling is Types.IReadOnly))
						{
							//	Saute aussi les widgets qui déclarent être en lecture seule. Ils ne
							//	sont pas intéressants pour une navigation clavier :
							
							Types.IReadOnly readOnly = sibling as Types.IReadOnly;
							
							if (readOnly.IsReadOnly)
							{
								continue;
							}
						}

						if (sibling != this)
						{
							//	Skip widgets which have an overridden tab order, since they would
							//	interfere with the logical flow :

							if (dir == TabNavigationDir.Forwards)
							{
								if (sibling.BackwardTabOverride != null)
								{
									continue;
								}
							}
							if (dir == TabNavigationDir.Backwards)
							{
								if (sibling.ForwardTabOverride != null)
								{
									continue;
								}
							}
						}
						
						list.Add (sibling);
					}
				}
			}
			
			list.Sort (Widget.TabIndexComparer);
			
			if ((mode == TabNavigationMode.ActivateOnTab) &&
				(this.AutoRadio))
			{
				//	On recherche les frères de ce widget, pour déterminer lequel devra être activé par la
				//	pression de la touche TAB. Pour bien faire, il faut supprimer les autres boutons radio
				//	qui appartiennent à notre groupe :

				List<Widget> copy = new List<Widget> ();
				
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


		#region TabIndexComparerImplementation class
		
		private class TabIndexComparerImplementation : IComparer<Widget>
		{
			public int Compare(Widget x, Widget y)
			{
				if (x == y) return 0;
				if (x == null) return -1;
				if (y == null) return 1;
				return (x.TabIndex == y.TabIndex) ? x.Index - y.Index : x.TabIndex - y.TabIndex;
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

		/// <summary>
		/// Processes the exit of the first or last child when navigating with the
		/// TAB key. A class which would like to handle TAB navigation in a custom
		/// manner can override this method.
		/// </summary>
		/// <param name="dir">The tab navigation direction.</param>
		/// <param name="mode">The tab navigation mode.</param>
		/// <param name="focus">The widget with should get the focus.</param>
		/// <returns><c>true</c> if the method has a focus suggestion; otherwise, <c>false</c>.</returns>
		protected virtual bool ProcessTabChildrenExit(TabNavigationDir dir, TabNavigationMode mode, out Widget focus)
		{
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
				
				GroupController controller    = GroupController.GetGroupController (this);
				Widget          activeWidget = controller.FindActiveWidget ();
				
				if (activeWidget != null)
				{
					return activeWidget.AboutToGetFocus (dir, mode, out focus);
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
			this.ClearFocus ();
			this.SetEngaged (false);
			this.SetEntered (false);
			
			if (this.HasChildren)
			{
				Widget[] children = this.Children.Widgets;
				int  childrenNum = children.Length;
				
				for (int i = 0; i < childrenNum; i++)
				{
					children[i].AboutToBecomeOrphan ();
				}
			}
		}
		
		
		public virtual bool WalkChildren(System.Predicate<Widget> callback)
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
		
		public virtual bool ExecuteCommand()
		{
			if ((this.HasCommand) &&
				(this.IsEnabled))
			{
				return this.ExecuteCommand (this.CommandObject);
			}
			else
			{
				return false;
			}
		}

		public bool ExecuteCommand(Command command)
		{
			Window window = this.Window;

			if (window != null)
			{
				if (command == null)
				{
					//	Command cannot be queued !
				}
				else
				{
					CommandContextChain chain = CommandContextChain.BuildChain (this);

					if (chain != null)
					{
						CommandState state = chain.GetCommandState (command);

						if (chain.GetLocalEnable (command))
						{
							this.QueueCommandForExecution (command, state);
							return true;
						}
					}
				}
			}

			return false;
		}

		protected virtual void QueueCommandForExecution(Command command, CommandState state)
		{
			Window window = this.Window;

			if (window != null)
			{
				window.QueueCommand (this, command);
			}
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
			if (this.textLayout != null)
			{
				this.textLayout.Alignment  = this.ContentAlignment;
				this.textLayout.LayoutSize = this.GetTextLayoutSize ();

				this.UpdateCaption (updateIconUri: this.IconUri == null);
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
			this.UpdateCaption (updateIconUri: true);
		}

		public void ForceCaptionUpdate()
		{
			if (this.textLayout == null)
			{
				this.CreateTextLayout ();
			}
			
			this.UpdateTextLayout ();
		}

		protected virtual void UpdateCaption(bool updateIconUri)
		{
			if (this.textLayout == null)
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
					string textLabel = null;

					if (string.IsNullOrEmpty (caption.Name) == false)
					{
						if ((UndefinedValue.IsUndefinedValue (this.GetLocalValue (Visual.NameProperty))) ||
							(Types.Serialization.BlackList.Contains (this, Visual.NameProperty)))
						{
							this.DefineNameFromCaption (caption);
						}
					}

					if (caption.HasLabels)
					{
						textLabel = TextLayout.SelectBestText (this.TextLayout, caption.SortedLabels, this.GetTextLayoutSize ());
						this.DefineTextFromCaption (textLabel);
					}

					string description = this.GetCaptionDescription (caption);

					ToolTip.ClearToolTipColor (this);

					if (!string.IsNullOrEmpty (description))
					{
						Collections.ShortcutCollection shortcuts = Shortcut.GetShortcuts (caption);
						string tip = Shortcut.AppendShortcutText (description, shortcuts);

						//	If the widget does not display a text label or if the tool tip
						//	is different from the label, define which tool tip to use.

						if ((this.HasTextLabel == false) ||
							(textLabel != tip))
						{
							this.DefineToolTipFromCaption (tip);
						}
						else
						{
							this.DefineToolTipFromCaption (null);
						}
					}
					else
					{
						this.DefineToolTipFromCaption (null);
					}

					if ((caption.HasIcon) &&
						(updateIconUri))
					{
						this.DefineIconFromCaption (caption.Icon);
					}
				}
			}
		}

		protected virtual string GetCaptionDescription(Caption caption)
		{
			if (caption.HasDescription)
			{
				return caption.Description;
			}
			else
			{
				return null;
			}
		}

		protected virtual void DefineNameFromCaption(Caption caption)
		{
			this.Name = caption.Name;
			Types.Serialization.BlackList.Add (this, Visual.NameProperty);
		}

		protected virtual void DefineTextFromCaption(string text)
		{
			this.Text = text;
			Types.Serialization.BlackList.Add (this, Widget.TextProperty);
		}

		protected virtual void DefineToolTipFromCaption(string tip)
		{
			ToolTip.Default.SetToolTip (this, tip);
			Types.Serialization.BlackList.Add (this, ToolTip.ToolTipTextProperty);
		}

		protected virtual void DefineIconFromCaption(string icon)
		{
			Drawing.Image image = Support.ImageProvider.Default.GetImage (icon, Support.Resources.DefaultManager);

			if (image == null)
			{
				this.IconUri = null;
			}
			else
			{
				this.IconUri = icon;
				Types.Serialization.BlackList.Add (this, Widget.IconUriProperty);
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

		internal void InternalNotifyTextLayoutAnchorEvent(object sender, AnchorEventArgs e)
		{
			System.Diagnostics.Debug.Assert (sender == this.textLayout);

			HypertextInfo info = new HypertextInfo (this.textLayout, e.Bounds, e.Index);

			if (this.hypertextList == null)
			{
				this.hypertextList = new List<HypertextInfo> ();
			}

			this.hypertextList.Add (info);
		}

		internal void InternalNotifyTextLayoutTextChanged(string oldText, string newText)
		{
			this.NotifyTextLayoutTextChanged (oldText, newText);
		}

		protected virtual void NotifyTextLayoutTextChanged(string oldText, string newText)
		{
			this.SetValueBase (Widget.TextProperty, newText);
		}
		
		public virtual void PaintHandler(Drawing.Graphics graphics, Drawing.Rectangle repaint, IPaintFilter paintFilter)
		{
			if ((paintFilter != null) &&
				(paintFilter.IsWidgetFullyDiscarded (this)))
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
				
				Drawing.Rectangle originalClipping  = graphics.SaveClippingRectangle ();
				Drawing.Transform originalTransform = graphics.Transform;
				Drawing.Transform graphicsTransform = Drawing.Transform.CreateTranslationTransform (this.ActualLocation);
				
				graphics.SetClippingRectangle (bounds);
				
				if (graphics.HasEmptyClippingRectangle)
				{
					//	Optimisation du cas où la région de clipping devient vide: on restaure
					//	la région précédente et on ne fait rien de plus.
					
					graphics.RestoreClippingRectangle (originalClipping);
					return;
				}
				
				graphics.Transform = graphicsTransform.MultiplyBy (originalTransform);
			
				try
				{
					if (this.hypertextList != null)
					{
						this.hypertextList.Clear ();
					}
					
					PaintEventArgs localPaintArgs = new PaintEventArgs (graphics, repaint);
					
					//	Peint l'arrière-plan du widget. En principe, tout va dans l'arrière plan, sauf
					//	si l'on désire réaliser des effets de transparence par dessus le dessin des
					//	widgets enfants.
					
					if ((paintFilter == null) ||
						(paintFilter.IsWidgetPaintDiscarded (this) == false))
					{
						graphics.ResetLineStyle ();
						this.OnPaintBackground (localPaintArgs);
						
						if (paintFilter != null)
						{
							paintFilter.NotifyAboutToProcessChildren (this, localPaintArgs);
						}
					}
					
					//	Peint tous les widgets enfants, en commençant par le numéro 0, lequel se trouve
					//	derrière tous les autres, etc. On saute les widgets qui ne sont pas visibles.
					
					if (this.HasChildren)
					{
						Widget[] children = this.Children.Widgets;
						int  childrenNum = children.Length;
						
						for (int i = 0; i < childrenNum; i++)
						{
							Widget widget = children[i];
						
							System.Diagnostics.Debug.Assert (widget != null);
						
							if (widget.Visibility)
							{
								widget.PaintHandler (graphics, repaint, paintFilter);
							}
						}
					}
				
					//	Peint l'avant-plan du widget, à n'utiliser que pour faire un "effet" spécial
					//	après coup.
					
					if ((paintFilter == null) ||
						(paintFilter.IsWidgetPaintDiscarded (this) == false))
					{
						graphics.ResetLineStyle ();
						this.OnPaintForeground (localPaintArgs);
						
						if (paintFilter != null)
						{
							paintFilter.NotifyChildrenProcessed (this, localPaintArgs);
						}
					}
				}
				finally
				{
					graphics.Transform = originalTransform;
					graphics.RestoreClippingRectangle (originalClipping);
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

		protected virtual WidgetPaintState GetPaintState()
		{
			return this.PaintState;
		}
		
		protected virtual void PaintBackgroundImplementation(Drawing.Graphics graphics, Drawing.Rectangle clipRect)
		{
			//	Implémenter le dessin du fond dans cette méthode.
		}
		
		protected virtual void PaintForegroundImplementation(Drawing.Graphics graphics, Drawing.Rectangle clipRect)
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
			Drawing.Point clientPos = message.IsDummy ? pos : this.MapParentToClient (pos);
			
			if (! this.PreProcessMessage (message, clientPos))
			{
				return;
			}
			
			//	En premier lieu, si le message peut être transmis aux descendants de ce widget, passe
			//	en revue ceux-ci dans l'ordre inverse de leur affichage (commence par le widget qui est
			//	visuellement au sommet).
			
			if ((message.FilterNoChildren == false) &&
				(message.Handled == false) &&
				(this.HasChildren) &&
				(message.IsDummy == false))
			{
				Widget[] children = this.Children.Widgets;
				int  childrenNum = children.Length;

				WindowRoot root = message.WindowRoot ?? VisualTree.GetWindowRoot (this);
				bool childEntered = false;
				
				for (int i = 0; i < childrenNum; i++)
				{
					Widget widget         = children[childrenNum-1 - i];
					bool   containsFocus = (root == null) ? false : root.DoesVisualContainKeyboardFocus (widget);
					
					if ((widget.IsFrozen == false) &&
						((widget.Visibility) || (containsFocus && message.IsKeyType)) &&
						((message.FilterOnlyFocused == false) || (containsFocus)) &&
						((message.FilterOnlyOnHit == false) || (widget.HitTest (clientPos))))
					{
						if (widget.IsEnabled)
						{
							if (message.IsMouseType)
							{
								//	C'est un message souris. Vérifions d'abord si le widget contenait déjà
								//	la souris auparavant.

								if ((!childEntered) &&
									(message.MessageType != MessageType.MouseLeave))
								{
									if ((this.IsEntered) &&
										(widget.IsEntered == false))
									{
										widget.SetEntered (true);
									}
									
									childEntered = widget.IsEntered;
								}
							}
							
							widget.MessageHandler (message, clientPos);
							
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
				this.DispatchMessage (message, clientPos);
			}
			
			this.PostProcessMessage (message, clientPos);
		}
		
		
		protected virtual void DispatchMessage(Message message, Drawing.Point pos)
		{
			if (this.Visibility || message.IsKeyType)
			{
				bool isEntered = this.IsEntered;
				
				switch (message.MessageType)
				{
					case MessageType.MouseUp:
						if (Message.CurrentState.IsSameWindowAsButtonDown == false)
						{
							return;
						}
						
						//	Le bouton a été relâché. Ceci génère l'événement 'Released' pour signaler
						//	ce relâchement, mais aussi un événement 'Clicked' ou 'DoubleClicked' en
						//	fonction du nombre de clics.
						
						this.OnReleased (new MessageEventArgs (message, pos));
						
						if (isEntered)
						{
							switch (message.ButtonDownCount)
							{
								case 1:	this.OnClicked (new MessageEventArgs (message, pos));		break;
								case 2:	this.OnDoubleClicked (new MessageEventArgs (message, pos));	break;
							}
						}

						if (message.Handled)
						{
							if (!this.AutoDoubleClick)
							{
								Message.ResetButtonDownCounter ();
							}
						}
						break;
					
					case MessageType.MouseDown:
						this.OnPressed (new MessageEventArgs (message, pos));
						break;

					case MessageType.MouseMove:
						this.OnMouseMove (new MessageEventArgs (message, pos));
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
				
				if (e.Cancel)
				{
					return false;
				}
			}
			
			if (message.IsMouseType)
			{
				bool reset = true;
				
				if (this.hypertextList != null)
				{
					foreach (HypertextInfo info in this.hypertextList)
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
				
				if (e.Cancel)
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
		
		
		protected virtual bool ShortcutHandler(Shortcut shortcut, bool executeFocused)
		{
			Widget[] children = this.Children.Widgets;
			int  childrenNum = children.Length;
			
			if (executeFocused)
			{
				for (int i = 0; i < childrenNum; i++)
				{
					Widget widget = children[childrenNum-1 - i];
				
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
			
			for (int i = 0; i < childrenNum; i++)
			{
				Widget widget = children[childrenNum-1 - i];
				
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
			Widget parent = this.Parent;
			
			if (parent != null)
			{
				parent.BuildFullPathName (buffer);
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
			if (this.textLayout == null)
			{
				this.textLayout = new TextLayout ();
				this.textLayout.SetEmbedder (this);
				
				this.UpdateTextLayout ();
			}
		}
		
		protected virtual void ModifyText(string text)
		{
			System.Diagnostics.Debug.Assert (this.textLayout != null);
			
			this.SetValueBase (Widget.TextProperty, text);
			this.OnTextDefined ();
		}
		
		protected virtual void ModifyTextLayout(string text)
		{
			if (this.textLayout != null)
			{
				this.textLayout.Text = text;
			}
		}
		
		protected virtual void DisposeTextLayout()
		{
			if (this.textLayout != null)
			{
				this.textLayout = null;
			}
		}
		

		protected override void OnParentChanged(Epsitec.Common.Types.DependencyPropertyChangedEventArgs e)
		{
			base.OnParentChanged (e);
			
			Widget oldParent = e.OldValue as Widget;
			Widget newParent = e.NewValue as Widget;
			
			if (newParent == null)
			{
				this.AboutToBecomeOrphan ();
			}
			
			Window oldWindow = oldParent == null ? null : VisualTree.GetWindow (oldParent);
			Window newWindow = newParent == null ? null : VisualTree.GetWindow (newParent);
			
			if (oldWindow != newWindow)
			{
				this.NotifyWindowChanged (oldWindow, newWindow);
			}
		}
		
		protected virtual void NotifyWindowChanged(Window oldWindow, Window newWindow)
		{
			foreach (Widget widget in this.Children.Widgets)
			{
				widget.NotifyWindowChanged (oldWindow, newWindow);
			}

			if ((newWindow != null) &&
				(this.Validator != null))
			{
				newWindow.AsyncValidation (this);
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
			var handler = this.GetUserEventHandler<PaintEventArgs> ("PaintBackground");
			
			if (handler != null)
			{
				e.Cancel = false;
				
				handler (this, e);
				
				if (e.Cancel)
				{
					return;
				}
			}
			
			this.PaintBackgroundImplementation (e.Graphics, e.ClipRectangle);
		}
		
		protected virtual void OnPaintForeground(PaintEventArgs e)
		{
			var handler = this.GetUserEventHandler<PaintEventArgs> ("PaintForeground");
			
			if (handler != null)
			{
				e.Cancel = false;
				
				handler (this, e);
				
				if (e.Cancel)
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

		protected virtual void OnMouseMove(MessageEventArgs e)
		{
			var handler = this.GetUserEventHandler<MessageEventArgs> (Widget.EventNames.MouseMoveEvent);

			if (handler != null)
			{
				handler (this, e);
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
			
			if ((this.InternalState & WidgetInternalState.ExecCmdOnPressed) != 0)
			{
				if (this.ExecuteCommand ())
				{
					if (e != null)
					{
						e.Message.Consumer = this;
					}
				}
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

			if ((this.InternalState & WidgetInternalState.ExecCmdOnPressed) == 0)
			{
				if (this.ExecuteCommand ())
				{
					if (e != null)
					{
						e.Message.Consumer = this;
					}
				}
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
			else
			{
				string hypertext = this.Hypertext;

				if (!string.IsNullOrEmpty (hypertext))
				{
					if (hypertext.StartsWith ("http:"))
					{
						e.Message.Consumer = this;
						System.Diagnostics.Process.Start (hypertext);
					}
					if (hypertext.StartsWith ("file:///"))
					{
						string path = hypertext.Substring (8);

						if (hypertext.EndsWith (".txt") || hypertext.EndsWith (".log"))
                        {
							System.Diagnostics.Process.Start ("notepad.exe", path);
							e.Message.Consumer = this;
						}
						else if (hypertext.EndsWith (@"\"))
						{
							System.Diagnostics.Process.Start ("explorer.exe", "/n," + path);
							e.Message.Consumer = this;
						}
					}
				}
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

		protected virtual void OnIconUriChanged(string oldIconUri, string newIconUri)
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
				
				GroupController controller = GroupController.GetGroupController (this);
				
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
			var handler = this.ValidatorChanged;

			if (handler != null)
			{
				this.ValidatorChanged (this);
			}
			
			if (this.Validator != null)
			{
				this.Validator.MakeDirty (true);
			}
		}
		
		public event Support.EventHandler<PaintEventArgs> PaintBackground
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

		public event Support.EventHandler<PaintEventArgs> PaintForeground
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

		public event Support.EventHandler<MessageEventArgs> MouseMove
		{
			add
			{
				this.AddUserEventHandler (Widget.EventNames.MouseMoveEvent, value);
			}
			remove
			{
				this.RemoveUserEventHandler (Widget.EventNames.MouseMoveEvent, value);
			}
		}

		public event Support.EventHandler<MessageEventArgs>			Pressed;
		public event Support.EventHandler<MessageEventArgs>			Released;
		public event Support.EventHandler<MessageEventArgs>			Clicked;
		public event Support.EventHandler<MessageEventArgs>			DoubleClicked;
		public event Support.EventHandler<MessageEventArgs>			Entered;
		public event Support.EventHandler<MessageEventArgs>			Exited;
		public event Support.EventHandler			ShortcutPressed;
		public event Support.EventHandler			ShortcutChanged;
		public event Support.EventHandler			HypertextHot;
		public event Support.EventHandler<MessageEventArgs>			HypertextClicked;
		public event Support.EventHandler			ValidatorChanged;

		public event Support.EventHandler<MessageEventArgs>			PreProcessing;
		public event Support.EventHandler<MessageEventArgs>			PostProcessing;
		
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

		public static class EventNames
		{
			public const string MouseMoveEvent = "MouseMove";
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

		private static void NotifyIconUriChanged(DependencyObject o, object oldValue, object newValue)
		{
			Widget that = o as Widget;
			string oldIconUri = oldValue as string;
			string newIconUri = newValue as string;
			that.OnIconUriChanged (oldIconUri, newIconUri);
		}

		public static readonly DependencyProperty TextProperty = DependencyProperty.Register ("Text", typeof (string), typeof (Widget), new DependencyPropertyMetadata (Widget.GetTextValue, Widget.SetTextValue, Widget.NotifyTextChanged));
		public static readonly DependencyProperty TabIndexProperty = DependencyProperty.Register ("TabIndex", typeof (int), typeof (Widget), new DependencyPropertyMetadata (0));
		public static readonly DependencyProperty IconUriProperty = DependencyProperty.Register ("IconUri", typeof (string), typeof (Widget), new VisualPropertyMetadata (null, Widget.NotifyIconUriChanged, VisualPropertyMetadataOptions.AffectsDisplay));
		public static readonly DependencyProperty TabNavigationModeProperty = DependencyProperty.Register ("TabNavigationMode", typeof (TabNavigationMode), typeof (Widget), new DependencyPropertyMetadata (TabNavigationMode.None));

		public static readonly DependencyProperty ForwardTabOverrideProperty  = DependencyProperty.Register ("ForwardTabOverride", typeof (Widget), typeof (Widget));
		public static readonly DependencyProperty BackwardTabOverrideProperty = DependencyProperty.Register ("BackwardTabOverride", typeof (Widget), typeof (Widget));
		public static readonly DependencyProperty ForwardEnterTabOverrideProperty = DependencyProperty.Register ("ForwardEnterTabOverride", typeof (Widget), typeof (Widget));
		public static readonly DependencyProperty BackwardEnterTabOverrideProperty = DependencyProperty.Register ("BackwardEnterTabOverride", typeof (Widget), typeof (Widget));


		static readonly List<Widget>			enteredWidgets = new List<Widget> ();
		static List<System.WeakReference>		aliveWidgets   = new List<System.WeakReference> ();
		static bool								debugDispose;

		private WidgetInternalState				internalState;
		
		private List<HypertextInfo>				hypertextList;
		private HypertextInfo					hypertext;
		
		private TextLayout						textLayout;
		private Collections.ShortcutCollection	shortcuts;
		private MouseCursor						mouseCursor;
	}
}
