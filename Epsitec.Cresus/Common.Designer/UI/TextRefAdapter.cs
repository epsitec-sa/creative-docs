//	Copyright � 2004, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Statut : en chantier/PA

using Epsitec.Common.Widgets;
using Epsitec.Common.Support;

namespace Epsitec.Common.Designer.UI
{
	/// <summary>
	/// Summary description for TextRefAdapter.
	/// </summary>
	
	[Common.UI.Adapters.Controller (1, typeof (TextRefController))]
	
	public class TextRefAdapter : Common.UI.Adapters.AbstractAdapter
	{
		public TextRefAdapter()
		{
		}
		
		public TextRefAdapter(Common.UI.Binders.IBinder binder) : this ()
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
				Common.UI.Binders.PropertyBinder binder = this.binder as Common.UI.Binders.PropertyBinder;
				
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
				
				return "";
			}
			set
			{
				Common.UI.Binders.PropertyBinder binder = this.binder as Common.UI.Binders.PropertyBinder;
				
				if (binder != null)
				{
					object source = binder.Source;
					string name   = binder.PropertyName;
					
					Support.ObjectBundler.DefineXmlRef (source, name, value);
					
					this.SyncFromBinder (Common.UI.SyncReason.AdapterChanged);
					this.OnValueChanged ();
				}
			}
		}
		
		
		public string							BundleName
		{
			get
			{
				string target = this.XmlRefTarget;
				
				if (target != null)
				{
					string bundle, field;
					
					if (Support.ResourceBundle.SplitTarget (target, out bundle, out field))
					{
						return bundle;
					}
				}
				
				return "";
			}
		}
		
		public string							FieldName
		{
			get
			{
				string target = this.XmlRefTarget;
				
				if (target != null)
				{
					string bundle, field;
					
					if (Support.ResourceBundle.SplitTarget (target, out bundle, out field))
					{
						return field;
					}
				}
				
				return "";
			}
		}
		
		
		public Support.Data.IPropertyProvider	StringProvider
		{
			get
			{
				if (this.string_provider == null)
				{
					this.string_provider = Support.Globals.Properties.GetProperty ("$resources$string provider") as Support.Data.IPropertyProvider;
				}
				
				return this.string_provider;
			}
		}
		
		public StringEditController				StringController
		{
			get
			{
				if (this.string_controller == null)
				{
					this.string_controller = Support.Globals.Properties.GetProperty ("$resources$string controller") as StringEditController;
				}
				
				return this.string_controller;
			}
		}
		
		
		public string GetFieldValue()
		{
			string target = this.XmlRefTarget;
			
			if ((target != null) &&
				(this.StringProvider != null))
			{
				string bundle;
				string field;
				
				if (Support.ResourceBundle.SplitTarget (target, out bundle, out field))
				{
					this.StringController.LoadStringBundle (bundle);
					
					if ((this.StringController.IsStringBundleLoaded (bundle)) &&
						(this.StringProvider.IsPropertyDefined (target)))
					{
						return this.StringProvider.GetProperty (target) as string;
					}
				}
			}
			
			return null;
		}
		
		
		protected override object ConvertToObject()
		{
			System.ComponentModel.TypeConverter converter = System.ComponentModel.TypeDescriptor.GetConverter (this.binder.GetDataType ());
			return converter.ConvertFromString (this.Value);
		}
		
		protected override bool ConvertFromObject(object data)
		{
			string field_value = this.GetFieldValue ();
			
			if (field_value != null)
			{
				this.Value = field_value;
				return true;
			}
			
			System.ComponentModel.TypeConverter converter = System.ComponentModel.TypeDescriptor.GetConverter (this.binder.GetDataType ());
			this.Value = converter.ConvertToString (data);
			
			return true;
		}
		
		
		private string							value;
		private Support.Data.IPropertyProvider	string_provider;
		private StringEditController			string_controller;
	}
}
