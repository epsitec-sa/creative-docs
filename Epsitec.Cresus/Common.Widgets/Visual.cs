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
		
		public static readonly Property NameProperty = Property.Register ("Name", typeof (string), typeof (Visual));
		public static readonly Property LayerProperty = Property.Register ("Layer", typeof (string), typeof (Visual));

		public static readonly Property ParentProperty = Property.RegisterReadOnly ("Parent", typeof (Visual), typeof (Visual));
		public static readonly Property ChildrenProperty = Property.Register ("Children", typeof (Helpers.VisualCollection), typeof (Visual));
	}
}
