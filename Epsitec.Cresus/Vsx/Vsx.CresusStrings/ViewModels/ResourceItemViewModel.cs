using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Epsitec.Cresus.ResourceManagement;
using Epsitec.Windows;

namespace Epsitec.Cresus.Strings.ViewModels
{
	public class ResourceItemViewModel : ViewModel
	{
		public ResourceItemViewModel(ResourceItem model, CultureInfo culture)
		{
			this.model = model;
			this.culture = culture;

			this.id = model.Id;
			this.name = model.Name;
			this.value = model.Value;
		}

		public ResourceItem Model
		{
			get
			{
				return this.model;
			}
		}

		public CultureInfo Culture
		{
			get
			{
				return this.culture;
			}
		}

		public string CultureName
		{
			get
			{
				return this.culture.Parent.DisplayName;
			}
		}

		public string Id
		{
			get
			{
				return this.id;
			}
			set
			{
				if (this.id != value)
				{
					this.id = value;
					this.RaiseEvent ("Id");
				}
			}
		}

		public string Name
		{
			get
			{
				return this.name;
			}
			set
			{
				if (this.name != value)
				{
					this.name = value;
					this.RaiseEvent ("Name");
				}
			}
		}

		public string Value
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
					this.RaiseEvent ("Value");
				}
			}
		}

		private readonly CultureInfo culture;
		private readonly ResourceItem model;
		private string id;
		private string name;
		private string value;
	}
}
