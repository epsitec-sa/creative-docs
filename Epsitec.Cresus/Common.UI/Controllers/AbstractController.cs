//	Copyright � 2006, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

using Epsitec.Common.Types;
using Epsitec.Common.Widgets;

using System.Collections.Generic;

namespace Epsitec.Common.UI.Controllers
{
	/// <summary>
	/// La classe AbstractController sert de base � tous les contr�leurs qui lient
	/// des donn�es � des widgets cr��s dynamiquement dans un widget Placeholder.
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

		Widgets.Layouts.IGridPermeable IController.GetGridPermeableLayoutHelper()
		{
			return this.GetGridPermeableLayoutHelper ();
		}

		#endregion

		public virtual object GetActualValue()
		{
			return UndefinedValue.Instance;
		}


		protected virtual Widgets.Layouts.IGridPermeable GetGridPermeableLayoutHelper()
		{
			return null;
		}

		protected abstract void CreateUserInterface(INamedType namedType, Caption caption);
		
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
			else if (expression.DataSourceType == DataSourceType.None)
			{
				return Binding.DoNothing;
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
					widget.SetEmbedder (view);
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
