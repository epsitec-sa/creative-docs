//	Copyright © 2004, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Statut : en chantier/PA

using Epsitec.Common.Widgets;
using Epsitec.Common.Support;

namespace Epsitec.Common.UI.Adapters
{
	/// <summary>
	/// Summary description for LayoutAdapter.
	/// </summary>
	
	[Controller (1, typeof (Controllers.CaptionOnlyController))]
	[Controller (2, typeof (Controllers.LayoutController))]
	
	public class LayoutAdapter : AbstractAdapter
	{
		public LayoutAdapter()
		{
		}
		
		
		public LayoutStyles						Value
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
		
		
		protected override object ConvertToObject()
		{
			return this.Value;
		}
		
		protected override bool ConvertFromObject(object data)
		{
			this.Value = (LayoutStyles) (data);
			return true;
		}
		
		
		
		private LayoutStyles					value;
	}
}
