//	Copyright © 2004, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

using Epsitec.Common.Widgets;
using Epsitec.Common.Support;

namespace Epsitec.Common.UI.Adapters
{
	/// <summary>
	/// Summary description for DataFolderAdapter.
	/// </summary>
	
	[Controller (1, typeof (Controllers.DataFolderController))]
	
	public class DataFolderAdapter : AbstractAdapter
	{
		public DataFolderAdapter()
		{
		}
		
		public DataFolderAdapter(Binders.IBinder binder) : this ()
		{
			this.Binder = binder;
			this.Binder.Adapter = this;
		}
		
		
		public Types.IDataItem[]				Value
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
			return this.Value;
		}
		
		protected override bool ConvertFromObject(object data)
		{
			Types.IDataItem[] items = data as Types.IDataItem[];
			
			if (items != null)
			{
				this.Value = items;
				return true;
			}
			
			return false;
		}
		
		
		
		private Types.IDataItem[]				value;
	}
}
