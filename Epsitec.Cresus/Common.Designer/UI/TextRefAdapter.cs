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
		
		
		public string							BundleName
		{
			get
			{
				string text = this.Value;
				
				if (Resources.IsTextRef (text))
				{
					string target = Resources.ExtractTextRefTarget (text);
					string bundle;
					string field;
					
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
				string text = this.Value;
				
				if (Resources.IsTextRef (text))
				{
					string target = Resources.ExtractTextRefTarget (text);
					string bundle;
					string field;
					
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
		private Support.Data.IPropertyProvider	string_provider;
		private StringEditController			string_controller;
		private Application						application;
	}
}
