//	Copyright © 2004, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

using Epsitec.Common.Widgets;
using Epsitec.Common.Support;

namespace Epsitec.Common.UI.Adapters
{
	/// <summary>
	/// Summary description for EnumAdapter.
	/// </summary>
	public abstract class AbstractEnumAdapter : AbstractAdapter
	{
		public AbstractEnumAdapter(Types.IEnumType enum_type)
		{
			this.enum_type = enum_type;
		}
		
		public AbstractEnumAdapter(Types.IEnumType enum_type, Binders.IBinder binder) : this (enum_type)
		{
			this.Binder = binder;
			this.Binder.Adapter = this;
		}
		
		
		public System.Enum						Value
		{
			get
			{
				return this.value;
			}
			set
			{
				if ((this.value != value) ||
					(this.validity == false))
				{
					this.value    = value;
					this.Validity = true;
					this.OnValueChanged ();
				}
			}
		}
		
		public Types.IEnumType					EnumType
		{
			get
			{
				return this.enum_type;
			}
		}
		
		
		protected override object ConvertToObject()
		{
			System.ComponentModel.TypeConverter converter = System.ComponentModel.TypeDescriptor.GetConverter (this.binder.GetDataType ());
			return converter.ConvertFromString (this.Value.ToString ());
		}
		
		protected override bool ConvertFromObject(object data)
		{
			System.ComponentModel.TypeConverter converter = System.ComponentModel.TypeDescriptor.GetConverter (this.binder.GetDataType ());
			System.Enum value;
			
			Types.Converter.Convert (data, this.enum_type.SystemType, out value);
			
			this.Value = value;
			return true;
		}
		
		
		
		private System.Enum						value;
		private Types.IEnumType					enum_type;
	}
}
