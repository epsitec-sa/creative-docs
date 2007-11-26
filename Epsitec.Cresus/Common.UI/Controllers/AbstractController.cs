//	Copyright © 2006-2007, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

using Epsitec.Common.Types;
using Epsitec.Common.Widgets;
using Epsitec.Common.Widgets.Layouts;

using System.Collections.Generic;

namespace Epsitec.Common.UI.Controllers
{
	/// <summary>
	/// La classe AbstractController sert de base à tous les contrôleurs qui lient
	/// des données à des widgets créés dynamiquement dans un widget Placeholder.
	/// </summary>
	public abstract class AbstractController : Types.DependencyObject, IController, IGridPermeable
	{
		protected AbstractController(ControllerParameters parameters)
		{
			System.Diagnostics.Debug.Assert (parameters != null);

			this.widgets = new List<WidgetRecord> ();
			this.parameters = parameters;

			if (this.parameters != null)
			{
				this.parameters.Lock ();
			}
		}

		public Placeholder						Placeholder
		{
			get
			{
				return this.placeholder;
			}
		}

		public ControllerParameters				ControllerParameters
		{
			get
			{
				return this.parameters;
			}
		}

		#region IController Members

		Placeholder IController.Placeholder
		{
			get
			{
				return this.Placeholder;
			}
		}

		void IController.DefinePlaceholder(Placeholder value)
		{
			if (this.placeholder != value)
			{
				Placeholder oldValue = this.placeholder;
				Placeholder newValue = value;

				this.placeholder = value;

				this.DetachAllWidgets (oldValue);
				this.AttachAllWidgets (newValue);

				this.InvalidateProperty (AbstractController.PlaceholderProperty, oldValue, newValue);
			}
		}

		void IController.CreateUserInterface()
		{
			if (this.placeholder != null)
			{
				Caption caption = this.placeholder.ValueCaption;
				INamedType type = this.placeholder.ValueType;

				if (type == null)
				{
					type = this.placeholder.InternalUpdateValueType ();
				}
				if (caption == null)
				{
					string name = this.placeholder.ValueName;
					
					caption = new Caption ();
					caption.Name = name;
					caption.Labels.Add (name);
				}
				
				if (type != null)
				{
					this.CreateUserInterface (type, caption);
				}
				else
				{
					System.Diagnostics.Debug.WriteLine ("No type object found");
				}
			}
		}

		void IController.DisposeUserInterface()
		{
			this.PrepareUserInterfaceDisposal ();
			
			WidgetRecord[] copy = this.widgets.ToArray ();
			
			this.widgets.Clear ();

			for (int i = 0; i < copy.Length; i++)
			{
				copy[i].Widget.Dispose ();
			}
		}

		void IController.RefreshUserInterface(object oldValue, object newValue)
		{
			//	Avoid update loops :
			
			if (this.isRefreshingUserInterface == false)
			{
				try
				{
					this.isRefreshingUserInterface = true;
					this.RefreshUserInterface (oldValue, newValue);
				}
				finally
				{
					this.isRefreshingUserInterface = false;
				}
			}
		}

		IGridPermeable IController.GetGridPermeableLayoutHelper()
		{
			return this.GetGridPermeableLayoutHelper ();
		}

		#endregion

		public virtual object GetActualValue()
		{
			return UndefinedValue.Value;
		}


		protected virtual IGridPermeable GetGridPermeableLayoutHelper()
		{
			return this;
		}

		protected abstract void CreateUserInterface(INamedType namedType, Caption caption);
		
		protected abstract void RefreshUserInterface(object oldValue, object newValue);

		protected virtual void PrepareUserInterfaceDisposal()
		{
		}
		
		protected void AddWidget(Widget widget, WidgetType widgetType)
		{
			this.widgets.Add (new WidgetRecord (widget, widgetType));

			if ((widgetType == WidgetType.Label) &&
				(this.parameters.GetParameterValue (AbstractController.NoLabelsParameter) != null))
			{
				widget.Visibility = false;
			}
			
			if (this.placeholder != null)
			{
				widget.SetEmbedder (this.placeholder);

				if (this.placeholder.Parent != null)
				{
					//	Force arrange of parent: since the placeholder is IGridPermeable,
					//	it will require that the parent updates its own layout in order
					//	to update the contents of the placeholder.

					Widgets.Layouts.LayoutContext.AddToArrangeQueue (this.placeholder.Parent);
				}
			}
		}

		protected BindingExpression GetPlaceholderBindingExpression()
		{
			if (this.placeholder == null)
			{
				return null;
			}
			else
			{
				return this.placeholder.ValueBindingExpression;
			}
		}

		protected virtual void OnActualValueChanged()
		{
			Support.EventHandler handler = (Support.EventHandler) this.GetUserEventHandler ("ActualValueChanged");

			if (handler != null)
			{
				handler (this);
			}

			this.SetPlaceholderValue ();
		}

		protected virtual void SetPlaceholderValue()
		{
			object value = this.ConvertBackValue (this.GetActualValue ());

			if (value != InvalidValue.Value)
			{
				this.Placeholder.Value = value;
			}
		}

		public bool IsConvertibleValue(object value)
		{
			if (InvalidValue.IsInvalidValue (this.ConvertBackValue (value)))
			{
				return false;
			}
			else
			{
				return true;
			}
		}

		public bool IsValidValue(object value)
		{
			value = this.ConvertBackValue (value);

			if (InvalidValue.IsInvalidValue (value))
			{
				return false;
			}
			if (value == Binding.DoNothing)
			{
				return true;
			}

			return TypeRosetta.IsValidValue (value, this.Placeholder.ValueType);
		}

		protected object ConvertBackValue(object value)
		{
			BindingExpression expression;
			return this.ConvertBackValue (value, out expression);
		}

		protected object ConvertBackValue(object value, out BindingExpression expression)
		{
			if (UndefinedValue.IsUndefinedValue (value))
			{
				expression = null;
				return InvalidValue.Value;
			}
			if (InvalidValue.IsInvalidValue (value))
			{
				expression = null;
				return InvalidValue.Value;
			}
			
			expression = this.GetPlaceholderBindingExpression ();

			if (expression == null)
			{
				return InvalidValue.Value;
			}
			else if (expression.DataSourceType == DataSourceType.None)
			{
				return Binding.DoNothing;
			}
			else
			{
				return expression.ConvertBackValue (value);
			}
		}

		#region IGridPermeable Members

		public virtual IEnumerable<PermeableCell> GetChildren(int column, int row, int columnSpan, int rowSpan)
		{
			int count = this.CountVisibleWidgets ();
			
			if (columnSpan < count)
			{
				throw new System.ArgumentException (string.Format ("Not enough columns for content; got {0} but at least {1} needed", columnSpan, count));
			}

			int index = 0;

			foreach (WidgetRecord record in this.widgets)
			{
				if (record.Widget.Visibility)
				{
					if (index == count-1)
					{
						yield return new Widgets.Layouts.PermeableCell (record.Widget, column+index, row+0, columnSpan-index, 1);
					}
					else
					{
						yield return new Widgets.Layouts.PermeableCell (record.Widget, column+index, row+0, 1, 1);
					}

					index++;
				}
			}
		}

		public virtual bool UpdateGridSpan(ref int columnSpan, ref int rowSpan)
		{
			int count = this.CountVisibleWidgets ();

			columnSpan = System.Math.Max (columnSpan, count);
			rowSpan    = System.Math.Max (rowSpan, 1);

			return true;
		}

		#endregion

		private int CountVisibleWidgets()
		{
			int count = 0;

			foreach (WidgetRecord record in this.widgets)
			{
				if (record.Widget.Visibility)
				{
					count++;
				}
			}

			return count;
		}
		
		
		private void AttachAllWidgets(Placeholder view)
		{
			if (view != null)
			{
				foreach (WidgetRecord record in this.widgets)
				{
					record.Widget.SetEmbedder (view);
				}
			}
		}

		private void DetachAllWidgets(Placeholder view)
		{
			if (view != null)
			{
				foreach (WidgetRecord record in this.widgets)
				{
					view.Children.Remove (record.Widget);
				}
			}
		}
		
		#region Get/Set Overrides

		private static object GetPlaceholderValue(DependencyObject o)
		{
			IController that = o as IController;
			
			if (that != null)
			{
				return that.Placeholder;
			}
			else
			{
				return o.GetValueBase (AbstractController.PlaceholderProperty);
			}
		}

		private static void SetPlaceholderValue(DependencyObject o, object value)
		{
			IController that = o as IController;

			if (that != null)
			{
				that.DefinePlaceholder ((Placeholder) value);
			}
			else
			{
				o.SetValueBase (AbstractController.PlaceholderProperty, value);
			}
		}
		
		#endregion

		public event Support.EventHandler		ActualValueChanged
		{
			add
			{
				this.AddUserEventHandler ("ActualValueChanged", value);
			}
			remove
			{
				this.RemoveUserEventHandler ("ActualValueChanged", value);
			}
		}

		public static readonly DependencyProperty PlaceholderProperty = DependencyProperty.Register ("Placeholder", typeof (Placeholder), typeof (AbstractController), new DependencyPropertyMetadata (AbstractController.GetPlaceholderValue, AbstractController.SetPlaceholderValue));

		public const string						NoLabelsParameter = "NoLabels";

		private Placeholder						placeholder;
		private readonly List<WidgetRecord>		widgets;
		private readonly ControllerParameters	parameters;
		private bool							isRefreshingUserInterface;
	}
}
