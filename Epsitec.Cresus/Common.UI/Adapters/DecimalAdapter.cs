//	Copyright � 2004, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Statut : en chantier/PA, 27/04/2004

using Epsitec.Common.Widgets;
using Epsitec.Common.Support;

namespace Epsitec.Common.UI.Adapters
{
	/// <summary>
	/// Summary description for DecimalAdapter.
	/// </summary>
	
//	[Controller (1, typeof (Controllers.StringController))]
	
public class DecimalAdapter : AbstractAdapter
{
public DecimalAdapter()
{
}
		
public DecimalAdapter(Binders.IBinder binder) : this ()
{
this.Binder = binder;
this.Binder.Adapter = this;
}
		
		
public decimal							Value
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
this.Value = (decimal) data;
return true;
}
		
		
		
private decimal							value;
}
}
