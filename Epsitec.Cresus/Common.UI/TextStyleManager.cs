//	Copyright © 2008, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Types;
using Epsitec.Common.UI;

using System.Collections.Generic;

[assembly: DependencyClass (typeof (TextStyleManager))]

namespace Epsitec.Common.UI
{
	public class TextStyleManager : DependencyObject
	{
		public TextStyleManager(Widgets.Widget root)
		{
			this.root = root;
			this.root.Measuring += this.HandleRootMeasuring;
			this.targets = new List<Epsitec.Common.Widgets.Widget> ();
		}


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

		public void Detach(Widgets.Widget targetWidget)
		{
			TextStyleManager manager = TextStyleManager.GetTextStyleManager (targetWidget);

			System.Diagnostics.Debug.Assert (this.targets.Contains (targetWidget));
			System.Diagnostics.Debug.Assert (manager == this);

			this.targets.Remove (targetWidget);
			TextStyleManager.SetTextStyleManager (targetWidget, null);
		}

		public void DefineStaticTextStyle(Drawing.TextStyle style)
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

			this.needsSync = true;
		}

		public void DefineTextFieldStyle(Drawing.TextStyle style)
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

			this.needsSync = true;
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
		
		private static void Refresh(Widgets.Widget root)
		{
			foreach (Widgets.Widget widget in root.Children)
			{
				TextStyleManager manager = TextStyleManager.GetTextStyleManager (widget);

				if (manager != null)
				{
					manager.staticTextStyleLive = manager.staticTextStyle;
					manager.textFieldStyleLive  = manager.textFieldStyle;

					manager.Synchronize (widget);
				}
				else if (widget.HasChildren)
				{
					TextStyleManager.Refresh (widget);
				}
			}
		}



		private void Synchronize(Widgets.Widget widget)
		{
			this.needsSync = false;

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

						this.Synchronize (child);
					}
					else
					{
						//	The child defines its own manager; update the manager's styles
						//	before we let it handle the synchronization :

						manager.lastWindowRootTreeChangeCounter = this.lastWindowRootTreeChangeCounter;
						manager.UpdateStyles (this);
						manager.Synchronize (child);
					}
				}
			}
		}

		private void HandleRootMeasuring(object sender)
		{
			if (this.needsSync == false)
			{
				Widgets.WindowRoot root = Widgets.Helpers.VisualTree.GetRoot (this.root) as Widgets.WindowRoot;
				
				if ((root != null) &&
					(root.TreeChangeCounter != this.lastWindowRootTreeChangeCounter))
				{
					this.lastWindowRootTreeChangeCounter = root.TreeChangeCounter;
					this.needsSync = true;
				}
			}

			if (this.needsSync)
			{
				Widgets.Widget root = Widgets.Helpers.VisualTree.GetRoot (this.root) as Widgets.Widget;

				if (root != null)
				{
					TextStyleManager.Refresh (root);
				}
			}
		}

		private void UpdateStyles(TextStyleManager parent)
		{
			this.staticTextStyleLive = TextStyleManager.MergeStyles (this.staticTextStyle, parent.staticTextStyleLive);
			this.textFieldStyleLive  = TextStyleManager.MergeStyles (this.textFieldStyle, parent.textFieldStyleLive);
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

		
		public static void SetTextStyleManager(Widgets.Widget obj, TextStyleManager manager)
		{
			if (manager == null)
			{
				obj.ClearValue (TextStyleManager.TextStyleManagerProperty);
			}
			else
			{
				manager.needsSync = true;
				obj.SetValue (TextStyleManager.TextStyleManagerProperty, manager);
			}
		}

		public static TextStyleManager GetTextStyleManager(Widgets.Widget obj)
		{
			return obj.GetValue (TextStyleManager.TextStyleManagerProperty) as TextStyleManager;
		}

		
		public static readonly DependencyProperty TextStyleManagerProperty = DependencyProperty.RegisterAttached ("TextStyleManager", typeof (TextStyleManager), typeof (TextStyleManager));

		private readonly Widgets.Widget root;
		private bool needsSync;
		private int lastWindowRootTreeChangeCounter;

		private readonly List<Widgets.Widget> targets;

		private Drawing.TextStyle staticTextStyle;
		private Drawing.TextStyle staticTextStyleLive;
		private Drawing.TextStyle textFieldStyle;
		private Drawing.TextStyle textFieldStyleLive;
	}
}
