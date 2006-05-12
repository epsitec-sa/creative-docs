//	Copyright © 2006, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

using Epsitec.Common.Types;
using System.Collections.Generic;

namespace Epsitec.Common.Widgets
{
	/// <summary>
	/// La classe Placeholder représente un conteneur utilisé par des widgets
	/// intelligents, remplis par data binding.
	/// </summary>
	public class Placeholder : AbstractGroup
	{
		public Placeholder()
		{
		}
		
		public Placeholder(Widget embedder)
		{
			this.SetEmbedder (embedder);
		}


		public Binding ValueBinding
		{
			get
			{
				return this.GetBinding (Placeholder.ValueProperty);
			}
		}

		public object ValueTypeObject
		{
			get
			{
				//	TODO: améliorer ceci pour fonctionner aussi quand la valeur est null
				
				return TypeRosetta.GetTypeObjectFromValue (this.Value);
			}
		}
		
		
		public object Value
		{
			get
			{
				return this.GetValue (Placeholder.ValueProperty);
			}
			set
			{
				this.SetValue (Placeholder.ValueProperty, value);
			}
		}

		public string Controller
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
			if (this.controller == null)
			{
				this.controller = Controllers.Factory.CreateController (this.Controller);

				if (this.controller != null)
				{
					this.controller.Placeholder = this;
					this.controller.CreateUserInterface ();
				}
			}
		}
		
		
		public static DependencyProperty ValueProperty = DependencyProperty.Register ("Value", typeof (object), typeof (Placeholder));
		public static DependencyProperty ControllerProperty = DependencyProperty.Register ("Controller", typeof (string), typeof (Placeholder));


		private IController controller;
	}
}
