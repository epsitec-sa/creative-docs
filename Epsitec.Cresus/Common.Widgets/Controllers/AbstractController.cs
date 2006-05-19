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

		#region IController Members

		public Placeholder Placeholder
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
		
		public void CreateUserInterface()
		{
			if (this.placeholder != null)
			{
				this.CreateUserInterface (this.placeholder.Value);
			}
		}

		public void DisposeUserInterface()
		{
			Widget[] copy = this.widgets.ToArray ();
			
			this.widgets.Clear ();

			for (int i = 0; i < copy.Length; i++)
			{
				copy[i].Dispose ();
			}
		}

		#endregion

		protected abstract void CreateUserInterface(object value);
		
		protected void AddWidget(Widget widget)
		{
			System.Diagnostics.Debug.Assert (this.widgets.Contains (widget) == false);
			
			this.widgets.Add (widget);

			if (this.placeholder != null)
			{
				this.placeholder.Children.Add (widget);
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

		public static readonly DependencyProperty PlaceholderProperty = DependencyProperty.Register ("Placeholder", typeof (Placeholder), typeof (AbstractController), new DependencyPropertyMetadata (AbstractController.GetPlaceholderValue, AbstractController.SetPlaceholderValue));

		private Placeholder						placeholder;
		private List<Widget>					widgets = new List<Widget> ();
	}
}
