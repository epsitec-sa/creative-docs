//	Copyright � 2004, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Statut : en chantier/PA

using Epsitec.Common.Widgets;
using Epsitec.Common.Support;

namespace Epsitec.Common.UI.Adapters
{
	/// <summary>
	/// Summary description for TextRefAdapter.
	/// </summary>
	
	[Controller (1, typeof (Controllers.TextRefController))]
	
	public class TextRefAdapter : AbstractAdapter
	{
		public TextRefAdapter()
		{
		}
		
		public TextRefAdapter(Binders.IBinder binder) : this ()
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
		
		public string							XmlRefTarget
		{
			//	Acc�s � la d�finition de la r�f�rence XML; plus pr�cis�ment, cette propri�t� permet
			//	d'acc�der directement � la cible d�finie par le tag <ref target="cible"/>, � condition
			//	que le binder soit bien attach� � la propri�t� d'un objet supportant l'interface
			//	Data.IPropertyProvider.
			
			get
			{
				Binders.PropertyBinder binder = this.binder as Binders.PropertyBinder;
				
				if (binder != null)
				{
					object source = binder.Source;
					string name   = binder.PropertyName;
					string target;
					
					if (Support.ObjectBundler.FindXmlRef (source, name, out target))
					{
						return target;
					}
				}
				
				return null;
			}
			set
			{
				Binders.PropertyBinder binder = this.binder as Binders.PropertyBinder;
				
				if (binder != null)
				{
					object source = binder.Source;
					string name   = binder.PropertyName;
					
					Support.ObjectBundler.DefineXmlRef (source, name, value);
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
			
			Support.Data.IPropertyProvider pp = Support.Globals.Properties.GetProperty ("$resources$string data provider") as Support.Data.IPropertyProvider;
			
			string target = this.XmlRefTarget;
			
			if ((pp != null) &&
				(target != null) &&
				(pp.IsPropertyDefined (target)))
			{
				this.Value = pp.GetProperty (target) as string;
			}
			else
			{
				this.Value = converter.ConvertToString (data);
			}
			
			return true;
		}
		
		
		
		private string							value;
	}
}
