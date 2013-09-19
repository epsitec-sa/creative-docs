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
	public class MultiCultureResourceItemCollectionViewModel : ObservableCollection<MultiCultureResourceItemViewModel>
	{
		public MultiCultureResourceItemCollectionViewModel (IEnumerable<IReadOnlyDictionary<CultureInfo, ResourceItem>> resources)
		{
			foreach (var cultureMap in resources)
			{
				this.Add (new MultiCultureResourceItemViewModel (cultureMap));
			}
		}
	}
}
