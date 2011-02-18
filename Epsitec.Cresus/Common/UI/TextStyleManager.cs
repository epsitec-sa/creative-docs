//	Copyright © 2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Types;
using Epsitec.Common.UI;

using System.Collections.Generic;

[assembly: DependencyClass (typeof (TextStyleManager))]

namespace Epsitec.Common.UI
{
	/// <summary>
	/// The <c>TextStyleManager</c> class propagates text styles to a visual
	/// tree. It can be used to globally define the appearance of labels or
	/// text fields in a user interface.
	/// </summary>
	public sealed class TextStyleManager : DependencyObject
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="TextStyleManager"/> class.
		/// This constructor creates the primary text style manager for a visual
		/// tree.
		/// </summary>
		/// <param name="root">The root of the visual tree.</param>
		public TextStyleManager(Widgets.Widget root)
		{
			this.root = root;
			this.root.Measuring += this.HandleRootMeasuring;
			this.targets = new List<Epsitec.Common.Widgets.Widget> ();
		}


		/// <summary>
		/// Initializes a new instance of the <see cref="TextStyleManager"/> class.
		/// Use this constructor only for secondary text styles; the visual tree
		/// must have a primary text style manager.
		/// </summary>
		public TextStyleManager()
		{
			this.targets = new List<Epsitec.Common.Widgets.Widget> ();
		}


		/// <summary>
		/// Gets or sets the static text style.
		/// </summary>
		/// <value>The static text style.</value>
		public Drawing.TextStyle StaticTextStyle
		{
			get
			{
				return this.staticTextStyle;
			}
			set
			{
				if (this.staticTextStyle != value)
				{
					this.DefineStaticTextStyle (value);
				}
			}
		}

		/// <summary>
		/// Gets or sets the text field style.
		/// </summary>
		/// <value>The text field style.</value>
		public Drawing.TextStyle TextFieldStyle
		{
			get
			{
				return this.textFieldStyle;
			}
			set
			{
				if (this.textFieldStyle != value)
				{
					this.DefineTextFieldStyle (value);
				}
			}
		}


		/// <summary>
		/// Attaches the text style manager to the specified target widget.
		/// </summary>
		/// <param name="targetWidget">The target widget.</param>
		public void Attach(Widgets.Widget targetWidget)
		{
			TextStyleManager manager = TextStyleManager.GetTextStyleManager (targetWidget);

			if (manager != null)
			{
				manager.Detach (targetWidget);
			}

			this.targets.Add (targetWidget);
			TextStyleManager.SetTextStyleManager (targetWidget, this);
		}

		/// <summary>
		/// Detaches the text style manager from the specified target widget.
		/// </summary>
		/// <param name="targetWidget">The target widget.</param>
		public void Detach(Widgets.Widget targetWidget)
		{
			TextStyleManager manager = TextStyleManager.GetTextStyleManager (targetWidget);

			System.Diagnostics.Debug.Assert (this.targets.Contains (targetWidget));
			System.Diagnostics.Debug.Assert (manager == this);

			this.targets.Remove (targetWidget);
			TextStyleManager.SetTextStyleManager (targetWidget, null);
		}

		
		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				foreach (Widgets.Widget target in this.targets.ToArray ())
				{
					this.Detach (target);
				}

				System.Diagnostics.Debug.Assert (this.targets.Count == 0);
			}

			base.Dispose (disposing);
		}


		/// <summary>
		/// Defines the static text style.
		/// </summary>
		/// <param name="style">The style.</param>
		private void DefineStaticTextStyle(Drawing.TextStyle style)
		{
			if (style == null)
			{
				this.staticTextStyle = null;
			}
			else
			{
				if (style.IsReadOnly == false)
				{
					style = style.Clone ();
					style.Lock ();
				}
				
				this.staticTextStyle = style;
			}

			this.lastChangeCounter = 0;

			if (this.root != null)
			{
				Widgets.Layouts.LayoutContext.AddToMeasureQueue (this.root);
			}
		}

		/// <summary>
		/// Defines the text field style.
		/// </summary>
		/// <param name="style">The style.</param>
		private void DefineTextFieldStyle(Drawing.TextStyle style)
		{
			if (style == null)
			{
				this.textFieldStyle = null;
			}
			else
			{
				if (style.IsReadOnly == false)
				{
					style = style.Clone ();
					style.Lock ();
				}
				
				this.textFieldStyle = style;
			}

			this.lastChangeCounter = 0;

			if (this.root != null)
			{
				Widgets.Layouts.LayoutContext.AddToMeasureQueue (this.root);
			}
		}


		/// <summary>
		/// Refreshes text style information starting at the specified root.
		/// </summary>
		/// <param name="root">The root of the visual tree.</param>
		/// <param name="currentCounter">The current tree change counter.</param>
		private static void Refresh(Widgets.Widget root, int currentCounter)
		{
			if (root == null)
			{
				return;
			}

			foreach (Widgets.Widget widget in root.Children)
			{
				TextStyleManager manager = TextStyleManager.GetTextStyleManager (widget);

				if (manager != null)
				{
					manager.lastChangeCounter = currentCounter;
					manager.ResetLiveStyles ();
					manager.ApplyStyles (widget);
				}
				else if (widget.HasChildren)
				{
					TextStyleManager.Refresh (widget, currentCounter);
				}
			}
		}

		/// <summary>
		/// Resets the live styles.
		/// </summary>
		private void ResetLiveStyles()
		{
			this.staticTextStyleLive = this.staticTextStyle;
			this.textFieldStyleLive  = this.textFieldStyle;
		}

		/// <summary>
		/// Updates the live styles.
		/// </summary>
		/// <param name="parent">The parent.</param>
		private void UpdateLiveStyles(TextStyleManager parent)
		{
			this.staticTextStyleLive = TextStyleManager.MergeStyles (this.staticTextStyle, parent.staticTextStyleLive);
			this.textFieldStyleLive  = TextStyleManager.MergeStyles (this.textFieldStyle, parent.textFieldStyleLive);
		}



		/// <summary>
		/// Applies the styles for the specified widget and all of its children,
		/// recursively.
		/// </summary>
		/// <param name="widget">The widget.</param>
		private void ApplyStyles(Widgets.Widget widget)
		{
			if ((widget.TextLayout != null) &&
				(widget.TextLayout.Style.IsReadOnly))
			{
				if (widget is Widgets.AbstractTextField)
				{
					if (this.textFieldStyleLive != null)
					{
						widget.TextLayout.Style = this.textFieldStyleLive;
					}
				}
				else
				{
					if (this.staticTextStyleLive != null)
					{
						widget.TextLayout.Style = this.staticTextStyleLive;
					}
				}
			}

			if (widget.HasChildren)
			{
				foreach (Widgets.Widget child in widget.Children)
				{
					TextStyleManager manager = TextStyleManager.GetTextStyleManager (child);

					if (manager == null)
					{
						//	Use same manager for child as for the current widget.

						this.ApplyStyles (child);
					}
					else
					{
						//	The child defines its own manager; update the manager's styles
						//	before we let it handle the synchronization :

						manager.lastChangeCounter = this.lastChangeCounter;
						manager.UpdateLiveStyles (this);
						manager.ApplyStyles (child);
					}
				}
			}
		}

		/// <summary>
		/// Handles the root measuring event. When this happens, check if we
		/// need to refresh the associated text styles.
		/// </summary>
		/// <param name="sender">The sender.</param>
		private void HandleRootMeasuring(object sender)
		{
			System.Diagnostics.Debug.Assert (this.root != null);

			int currentChangeCounter = this.GetVisualTreeChangeId (this.root);
			
			if (this.lastChangeCounter == currentChangeCounter)
			{
				//	Nothing to do... Either we have already executed the refresh
				//	previously, or another text style manager has done the job
				//	for us.
			}
			else
			{
				this.lastChangeCounter = currentChangeCounter;
				
				Widgets.Widget root = Widgets.Helpers.VisualTree.GetRoot (this.root) as Widgets.Widget;

				if (root != null)
				{
					TextStyleManager.Refresh (root, currentChangeCounter);
				}
			}
		}

		private static Drawing.TextStyle MergeStyles(Drawing.TextStyle localStyle, Drawing.TextStyle parentStyle)
		{
			if (localStyle == null)
			{
				return parentStyle;
			}
			else
			{
				localStyle.RedefineParent (parentStyle);
				return localStyle;
			}
		}

		private int GetVisualTreeChangeId(Widgets.Widget widget)
		{
			Widgets.WindowRoot root = Widgets.Helpers.VisualTree.GetRoot (widget) as Widgets.WindowRoot;

			if (root == null)
			{
				return 0;
			}
			else
			{
				return root.TreeChangeCounter;
			}
		}

		
		internal static void SetTextStyleManager(Widgets.Widget obj, TextStyleManager manager)
		{
			if (manager == null)
			{
				obj.ClearValue (TextStyleManager.TextStyleManagerProperty);
			}
			else
			{
				obj.SetValue (TextStyleManager.TextStyleManagerProperty, manager);
			}
		}

		internal static TextStyleManager GetTextStyleManager(Widgets.Widget obj)
		{
			return obj.GetValue (TextStyleManager.TextStyleManagerProperty) as TextStyleManager;
		}

		
		public static readonly DependencyProperty TextStyleManagerProperty = DependencyProperty.RegisterAttached ("TextStyleManager", typeof (TextStyleManager), typeof (TextStyleManager));

		
		private readonly Widgets.Widget			root;
		private readonly List<Widgets.Widget>	targets;
		
		private int								lastChangeCounter;

		private Drawing.TextStyle				staticTextStyle;
		private Drawing.TextStyle				staticTextStyleLive;
		private Drawing.TextStyle				textFieldStyle;
		private Drawing.TextStyle				textFieldStyleLive;
	}
}
