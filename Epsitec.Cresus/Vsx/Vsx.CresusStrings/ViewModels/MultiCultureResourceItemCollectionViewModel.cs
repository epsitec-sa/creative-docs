using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Epsitec.Cresus.ResourceManagement;

namespace Epsitec.Cresus.Strings.ViewModels
{
	public class MultiCultureResourceItemCollectionViewModel : ObservableCollection<MultiCultureResourceItem>
	{
		public MultiCultureResourceItemCollectionViewModel(IEnumerable<MultiCultureResourceItem> items)
		{
			foreach (var item in items)
			{
				this.Add (item);
			}
		}
	}
}
