using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Epsitec.Cresus.Strings.ViewModels
{
	public class MultiCultureResourceItemCollectionViewModel
	{
		public ObservableCollection<MultiCultureResourceItemViewModel> Items
		{
			get
			{
				return this.items;
			}
		}

		private readonly ObservableCollection<MultiCultureResourceItemViewModel> items = new ObservableCollection<MultiCultureResourceItemViewModel> ();
	}
}
