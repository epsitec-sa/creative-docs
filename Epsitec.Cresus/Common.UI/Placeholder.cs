//	Copyright © 2006, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

using Epsitec.Common.Types;
using Epsitec.Common.UI;
using Epsitec.Common.Widgets;

using System.Collections.Generic;

[assembly: DependencyClass (typeof (Placeholder))]

namespace Epsitec.Common.UI
{
	/// <summary>
	/// La classe Placeholder représente un conteneur utilisé par des widgets
	/// intelligents, remplis par data binding.
	/// </summary>
	public class Placeholder : AbstractPlaceholder, Widgets.Layouts.IGridPermeable
	{
		public Placeholder()
		{
			Application.QueueAsyncCallback (this.CreateUserInterface);
		}
		
		public Placeholder(Widget embedder) : this ()
		{
			this.SetEmbedder (embedder);
		}


		public MiniPanel						EditPanel
		{
			get
			{
				if (this.editPanel == null)
				{
					this.editPanel = new MiniPanel ();
				}
				
				return this.editPanel;
			}
		}

		public string							Controller
		{
			get
			{
				return (string) this.GetValue (Placeholder.ControllerProperty);
			}
			set
			{
				this.SetValue (Placeholder.ControllerProperty, value);
			}
		}

		public string							ControllerParameter
		{
			get
			{
				return (string) this.GetValue (Placeholder.ControllerParameterProperty);
			}
			set
			{
				this.SetValue (Placeholder.ControllerParameterProperty, value);
			}
		}

		private Widgets.Layouts.IGridPermeable	ControllerIGridPermeable
		{
			get
			{
				Widgets.Layouts.IGridPermeable helper = null;

				if ((this.controller == null) &&
					(this.controllerName != null))
				{
					IController temp = Controllers.Factory.CreateController (this.controllerName, this.controllerParameter);

					if (temp != null)
					{
						helper = temp.GetGridPermeableLayoutHelper ();
					}
				}
				else if (this.controller != null)
				{
					helper = this.controller.GetGridPermeableLayoutHelper ();
				}
				if (helper == null)
				{
					helper = Placeholder.noOpGridPermeableHelper;
				}

				return helper;
			}
		}

		protected override void LayoutArrange(Widgets.Layouts.ILayoutEngine engine)
		{
			Widget parent = this.Parent;
			
			if ((parent != null) &&
				(Widgets.Layouts.GridLayoutEngine.GetColumn (this) >= 0) &&
				(Widgets.Layouts.GridLayoutEngine.GetRow (this) >= 0))
			{
				Widgets.Layouts.ILayoutEngine parentEngine = Widgets.Layouts.LayoutEngine.GetLayoutEngine (parent);

				if (parentEngine is Widgets.Layouts.GridLayoutEngine)
				{
					//	This placeholder is in a grid. No need to arrange the children;
					//	they get arranged by the grid itself !
					
					return;
				}
			}
			
			base.LayoutArrange (engine);
		}

		protected override void OnValueBindingChanged()
		{
			this.UpdateController ();
			base.OnValueBindingChanged ();
		}

		protected override void OnBoundsChanged(Drawing.Rectangle oldValue, Drawing.Rectangle newValue)
		{
			base.OnBoundsChanged (oldValue, newValue);

			if (this.editPanel != null)
			{
				PanelStack panelStack = PanelStack.GetPanelStack (this);

				this.editPanel.PanelStack = panelStack;
				this.editPanel.Aperture = Widgets.Helpers.VisualTree.MapVisualToAncestor (this, panelStack, this.Client.Bounds);
			}
		}

		private void UpdateController()
		{
			string oldControllerName = this.controllerName;
			string newControllerName = null;
			string oldControllerParameter = this.controllerParameter;
			string newControllerParameter = null;
			
			if (this.Controller == "*")
			{
				BindingExpression expression = this.ValueBindingExpression;
				
				if (expression != null)
				{
					Controllers.Factory.GetDefaultController (expression, out newControllerName, out newControllerParameter);
				}
			}
			else
			{
				newControllerName      = this.Controller;
				newControllerParameter = this.ControllerParameter;
			}
			
			if ((newControllerName != oldControllerName) ||
				(newControllerParameter != oldControllerParameter))
			{
				this.controllerName      = newControllerName;
				this.controllerParameter = newControllerParameter;
				
				if (this.controller == null)
				{
					if (this.controllerName != null)
					{
						Application.QueueAsyncCallback (this.CreateUserInterface);
					}
				}
				else
				{
					Application.QueueAsyncCallback (this.RecreateUserInterface);
				}
			}
		}

		private void DisposeUserInterface()
		{
			if (this.controller != null)
			{
				this.controller.DisposeUserInterface ();
				this.controller = null;
			}
		}

		private void CreateUserInterface()
		{
			if ((this.controller == null) &&
				(this.controllerName != null))
			{
				this.controller = Controllers.Factory.CreateController (this.controllerName, this.controllerParameter);

				if (this.controller != null)
				{
					this.controller.Placeholder = this;
					this.controller.CreateUserInterface ();

					object value = this.Value;

					if (value != UndefinedValue.Instance)
					{
						this.controller.RefreshUserInterface (UndefinedValue.Instance, value);
					}
				}
			}
		}

		private void RecreateUserInterface()
		{
			if (this.controller != null)
			{
				this.DisposeUserInterface ();
				this.CreateUserInterface ();
			}
		}

		protected override void UpdateValueType(object oldValueType, object newValueType)
		{
			this.UpdateController ();
			base.UpdateValueType (oldValueType, newValueType);
			
			if (this.controller != null)
			{
				Application.QueueAsyncCallback (this.RecreateUserInterface);
			}
		}

		protected override void UpdateValueName(string oldValueName, string newValueName)
		{
			base.UpdateValueName (oldValueName, newValueName);
			
			if (this.controller != null)
			{
				Application.QueueAsyncCallback (this.RecreateUserInterface);
			}
		}

		protected override void UpdateValue(object oldValue, object newValue)
		{
			base.UpdateValue (oldValue, newValue);
			
			if (this.controller != null)
			{
				Application.ExecuteAsyncCallbacks ();
				this.controller.RefreshUserInterface (oldValue, newValue);
			}
		}

		#region IGridPermeable Members

		IEnumerable<Widgets.Layouts.PermeableCell> Widgets.Layouts.IGridPermeable.GetChildren(int column, int row, int columnSpan, int rowSpan)
		{
			return this.ControllerIGridPermeable.GetChildren (column, row, columnSpan, rowSpan);
		}

		bool Widgets.Layouts.IGridPermeable.UpdateGridSpan(ref int columnSpan, ref int rowSpan)
		{
			return this.ControllerIGridPermeable.UpdateGridSpan (ref columnSpan, ref rowSpan);
		}

		#endregion

		#region NoOpGridPermeableHelper Class

		private class NoOpGridPermeableHelper : Widgets.Layouts.IGridPermeable
		{
			#region IGridPermeable Members

			public IEnumerable<Widgets.Layouts.PermeableCell> GetChildren(int column, int row, int columnSpan, int rowSpan)
			{
				yield break;
			}

			public bool UpdateGridSpan(ref int columnSpan, ref int rowSpan)
			{
				return false;
			}

			#endregion
		}
		
		#endregion

		static Placeholder()
		{
			Controllers.Factory.Setup ();
		}

		private static void NotifyControllerChanged(DependencyObject o, object oldValue, object newValue)
		{
			Placeholder that = (Placeholder) o;
			
			that.UpdateController ();
		}
		
		public static readonly DependencyProperty ControllerProperty = DependencyProperty.Register ("Controller", typeof (string), typeof (Placeholder), new DependencyPropertyMetadata (Placeholder.NotifyControllerChanged));
		public static readonly DependencyProperty ControllerParameterProperty = DependencyProperty.Register ("ControllerParameter", typeof (string), typeof (Placeholder), new DependencyPropertyMetadata (Placeholder.NotifyControllerChanged));

		static readonly NoOpGridPermeableHelper	noOpGridPermeableHelper = new NoOpGridPermeableHelper ();
		
		private IController						controller;
		private string							controllerName;
		private string							controllerParameter;
		private MiniPanel						editPanel;
	}
}
