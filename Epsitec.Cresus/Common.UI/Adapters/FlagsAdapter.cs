//	Copyright � 2004, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

using Epsitec.Common.Widgets;
using Epsitec.Common.Support;

namespace Epsitec.Common.UI.Adapters
{
	/// <summary>
	/// Summary description for FlagsAdapter.
	/// </summary>
	
	[Controller (1, typeof (Controllers.FlagsController))]
	
	public class FlagsAdapter : AbstractAdapter
	{
		public FlagsAdapter(Types.IEnum enum_type)
		{
			this.enum_type = enum_type;
		}
		
		public FlagsAdapter(Types.IEnum enum_type, Binders.IBinder binder) : this (enum_type)
		{
			this.Binder = binder;
			this.Binder.Adapter = this;
		}
		
		
		public string							Value
		{
			get
			{
				return this.value;
			}
			set
			{
				if (this.value != value)
				{
					this.value = value;
					this.OnValueChanged ();
				}
			}
		}
		
		public Types.IEnum						EnumType
		{
			get
			{
				return this.enum_type;
			}
		}
		
		
		protected override object ConvertToObject()
		{
			System.ComponentModel.TypeConverter converter = System.ComponentModel.TypeDescriptor.GetConverter (this.binder.GetDataType ());
			return converter.ConvertFromString (this.Value);
		}
		
		protected override bool ConvertFromObject(object data)
		{
			System.ComponentModel.TypeConverter converter = System.ComponentModel.TypeDescriptor.GetConverter (this.binder.GetDataType ());
			this.Value = converter.ConvertToString (data);
			return true;
		}
		
		
		
		private string							value;
		private Types.IEnum						enum_type;
	}
}
