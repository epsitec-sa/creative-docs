//	Copyright © 2004, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Statut : en chantier/PA

using Epsitec.Common.Widgets;
using Epsitec.Common.Support;

namespace Epsitec.Common.Designer.UI
{
	using CultureInfo = System.Globalization.CultureInfo;

	/// <summary>
	/// Summary description for TextRefAdapter.
	/// </summary>
	
	[Common.UI.Adapters.Controller (1, typeof (TextRefController))]
	
	public class TextRefAdapter : Common.UI.Adapters.AbstractAdapter
	{
		public TextRefAdapter(Application application)
		{
			this.application = application;
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
			//	Accès à la définition de la référence XML; plus précisément, cette propriété permet
			//	d'accéder directement à la cible définie par le tag <ref target="cible"/>, à condition
			//	que le binder soit bien attaché à la propriété d'un objet supportant l'interface
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
		
		
		public Application						Application
		{
			get
			{
				return this.application;
			}
		}
		
		public Support.Data.IPropertyProvider	StringProvider
		{
			get
			{
				if (this.string_provider == null)
				{
					this.string_provider = this.application.StringEditController.Provider;
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
					this.string_controller = this.application.StringEditController;
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
		
		public bool DefineFieldValue(string bundle, string field, string value)
		{
			if (this.StringController.IsStringBundleLoaded (bundle))
			{
				StringEditController.Store store = this.StringController.FindStore (bundle);
				
				if (store != null)
				{
					ResourceLevel active_level   = store.ActiveBundle.ResourceLevel;
					CultureInfo   active_culture = store.ActiveBundle.Culture;
					
					store.SetActive (ResourceLevel.Default, active_culture);
					
					int row = store.GetRowCount ();
					
					if (store.CheckInsertRows (row, 1))
					{
						store.InsertRows (row, 1);
						store.SetCellText (row, 0, field);
						store.SetCellText (row, 1, value);
					}
					
					store.SetActive (active_level, active_culture);
				}
			}
			
			return false;
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
		private Application						application;
	}
}
