using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Epsitec.Cresus.ResourceManagement;
using Epsitec.Windows;

namespace Epsitec.Cresus.Strings.ViewModels
{
	public class MultiCultureResourceItemViewModel : ObservableCollection<ResourceItem>
	{
		public MultiCultureResourceItemViewModel (IReadOnlyDictionary<CultureInfo, ResourceItem> resources)
		{
			foreach (var item in resources.Values)
			{
				this.Add (item);
			}
		}

		public string SymbolName
		{
			get
			{
				var item = this.FirstOrDefault ();
				return item == null ? null : item.SymbolName;
			}
		}
	}
}
