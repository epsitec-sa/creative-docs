//	Copyright © 2006, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

using System.Collections.Generic;

[assembly: Epsitec.Common.Types.DependencyClass (typeof (Epsitec.Common.Types.StructuredTypeField))]

namespace Epsitec.Common.Types
{
	public class StructuredTypeField : DependencyObject
	{
		public StructuredTypeField()
		{
		}

		public StructuredTypeField(string name, INamedType type)
		{
			this.Name = name;
			this.Type = type;
		}

		public string Name
		{
			get
			{
				return (string) this.GetValue (StructuredTypeField.NameProperty);
			}
			set
			{
				this.SetValue (StructuredTypeField.NameProperty, value);
			}
		}

		public INamedType Type
		{
			get
			{
				return (INamedType) this.GetValue (StructuredTypeField.TypeProperty);
			}
			set
			{
				this.SetValue (StructuredTypeField.TypeProperty, value);
			}
		}

		public static DependencyProperty NameProperty = DependencyProperty.Register ("Name", typeof (string), typeof (StructuredTypeField), new DependencyPropertyMetadata ());
		public static DependencyProperty TypeProperty = DependencyProperty.Register ("Type", typeof (INamedType), typeof (StructuredTypeField), new DependencyPropertyMetadata ());
	}
}
