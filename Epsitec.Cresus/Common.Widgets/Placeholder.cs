//	Copyright © 2006, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

using Epsitec.Common.Types;
using System.Collections.Generic;

namespace Epsitec.Common.Widgets
{
	/// <summary>
	/// La classe Placeholder représente un conteneur
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
		
		
		public static DependencyProperty ValueProperty = DependencyProperty.Register ("Value", typeof (object), typeof (Placeholder));
	}
}
