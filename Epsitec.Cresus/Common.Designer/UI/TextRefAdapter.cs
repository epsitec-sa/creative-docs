//	Copyright © 2004, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

using Epsitec.Common.Widgets;
using Epsitec.Common.Support;

namespace Epsitec.Common.Designer.UI
{
	using CultureInfo = System.Globalization.CultureInfo;

	/// <summary>
	/// Summary description for TextRefAdapter.
	/// </summary>
	
	[Common.UI.Adapters.Controller (1, typeof (TextRefController))]
	
	public class TextRefAdapter : Common.UI.Adapters.AbstractStringAdapter
	{
		public TextRefAdapter(Application application)
		{
			this.application = application;
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
				return this.application.StringEditController.Provider;
			}
		}
		
		public StringEditController				StringController
		{
			get
			{
				return this.application.StringEditController;
			}
		}
		
		
		
		private Application						application;
	}
}
