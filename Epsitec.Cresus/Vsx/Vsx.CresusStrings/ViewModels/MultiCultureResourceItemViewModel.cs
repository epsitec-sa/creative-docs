using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Epsitec.Windows;

namespace Epsitec.Cresus.Strings.ViewModels
{
	public class MultiCultureResourceItemViewModel
	{
		public MultiCultureResourceItemViewModel(string name)
		{
			this.name = name;
		}

		public string Name
		{
			get
			{
				return this.name;
			}
		}

		public ObservableCollection<ResourceItemViewModel> Items
		{
			get
			{
				return this.items;
			}
		}

		private readonly ObservableCollection<ResourceItemViewModel> items = new ObservableCollection<ResourceItemViewModel> ();
		private readonly string name;
	}
}
