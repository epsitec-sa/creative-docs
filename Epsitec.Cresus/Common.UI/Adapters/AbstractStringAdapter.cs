//	Copyright © 2004, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

using Epsitec.Common.Widgets;
using Epsitec.Common.Support;

namespace Epsitec.Common.UI.Adapters
{
	/// <summary>
	/// Summary description for StringAdapter.
	/// </summary>
	public abstract class AbstractStringAdapter : AbstractAdapter
	{
		public AbstractStringAdapter()
		{
		}
		
		public AbstractStringAdapter(Binders.IBinder binder) : this ()
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
				if ((this.value != value) ||
					(this.validity == false))
				{
					this.value    = value;
					this.Validity = true;
					this.OnValueChanged ();
				}
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
	}
}
