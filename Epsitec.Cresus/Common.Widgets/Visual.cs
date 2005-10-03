//	Copyright © 2005, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

using Epsitec.Common.Types;

namespace Epsitec.Common.Widgets
{
	/// <summary>
	/// Visual.
	/// </summary>
	public class Visual : Types.Object
	{
		public Visual()
		{
		}
		
		
		public string							Name
		{
			get
			{
				return (string) this.GetValue (Visual.NameProperty);
			}
			set
			{
				this.SetValue (Visual.NameProperty, value);
			}
		}
		
		public Visual							Parent
		{
			get
			{
				return (Visual) this.GetValue (Visual.ParentProperty);
			}
		}
		
		
		
		
		
		internal void NotifyVisualInsertion(Layouts.Layer layer, Visual visual)
		{
		}
		
		internal void NotifyVisualRemoval(Layouts.Layer layer, Visual visual)
		{
		}
		
		
		public static readonly Property NameProperty = Property.Register ("Name", typeof (string), typeof (Visual));
		public static readonly Property LayerProperty = Property.Register ("Layer", typeof (string), typeof (Visual));

		public static readonly Property ParentProperty = Property.RegisterReadOnly ("Parent", typeof (Visual), typeof (Visual));
		public static readonly Property ChildrenProperty = Property.Register ("Children", typeof (Collections.VisualCollection), typeof (Visual));
	}
}
