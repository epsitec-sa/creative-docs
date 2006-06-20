//	Copyright © 2006, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

using Epsitec.Common.Types;
using System.Collections.Generic;

namespace Epsitec.Common.Widgets.Controllers
{
	/// <summary>
	/// La classe AbstractController sert de base à tous les contrôleurs qui lient
	/// des données à des widgets créés dynamiquement dans un widget Placeholder.
	/// </summary>
	public abstract class AbstractController : Types.DependencyObject, IController
	{
		protected AbstractController()
		{
		}

		public Placeholder						Placeholder
		{
			get
			{
				return this.placeholder;
			}
			set
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
		}

		#region IController Members

		Placeholder IController.Placeholder
		{
			get
			{
				return this.Placeholder;
			}
			set
			{
				this.Placeholder = value;
			}
		}

		void IController.CreateUserInterface()
		{
			if (this.placeholder != null)
			{
				this.CreateUserInterface (this.placeholder.ValueType, this.placeholder.ValueName);
			}
		}

		void IController.DisposeUserInterface()
		{
			this.PrepareUserInterfaceDisposal ();
			
			Widget[] copy = this.widgets.ToArray ();
			
			this.widgets.Clear ();

			for (int i = 0; i < copy.Length; i++)
			{
				copy[i].Dispose ();
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

		Layouts.IGridPermeable IController.GetGridPermeableLayoutHelper()
		{
			return this.GetGridPermeableLayoutHelper ();
		}

		#endregion

		public virtual object GetActualValue()
		{
			return UndefinedValue.Instance;
		}
		
		
		protected virtual Layouts.IGridPermeable GetGridPermeableLayoutHelper()
		{
			return null;
		}

		protected abstract void CreateUserInterface(INamedType namedType, string valueName);
		
		protected abstract void RefreshUserInterface(object oldValue, object newValue);

		protected virtual void PrepareUserInterfaceDisposal()
		{
		}
		
		protected void AddWidget(Widget widget)
		{
			System.Diagnostics.Debug.Assert (this.widgets.Contains (widget) == false);
			
			this.widgets.Add (widget);

			if (this.placeholder != null)
			{
				this.placeholder.Children.Add (widget);
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

			if (value != InvalidValue.Instance)
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
				return InvalidValue.Instance;
			}
			if (InvalidValue.IsInvalidValue (value))
			{
				expression = null;
				return InvalidValue.Instance;
			}
			
			expression = this.GetPlaceholderBindingExpression ();

			if (expression == null)
			{
				return InvalidValue.Instance;
			}
			else
			{
				return expression.ConvertBackValue (value);
			}
		}
		
		
		private void AttachAllWidgets(Placeholder view)
		{
			if (view != null)
			{
				foreach (Widget widget in this.widgets)
				{
					view.Children.Add (widget);
				}
			}
		}

		private void DetachAllWidgets(Placeholder view)
		{
			if (view != null)
			{
				foreach (Widget widget in this.widgets)
				{
					view.Children.Remove (widget);
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
				that.Placeholder = (Placeholder) value;
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

		private Placeholder						placeholder;
		private List<Widget>					widgets = new List<Widget> ();
		private bool							isRefreshingUserInterface;
	}
}
