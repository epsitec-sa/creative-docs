using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Epsitec.Cresus.ResourceManagement;
using Epsitec.Cresus.Strings.ViewModels;

namespace Epsitec.Cresus.Strings.Views
{
	/// <summary>
	/// Interaction logic for MultiCultureResourceItemCollectionView.xaml
	/// </summary>
	public partial class MultiCultureResourceItemCollectionView : UserControl
	{
		public static MultiCultureResourceItemCollectionView Create(CompositeDictionary symbolFirstMap)
		{
			var view = new MultiCultureResourceItemCollectionView ();
			var viewModel = new MultiCultureResourceItemCollectionViewModel ();
			view.DataContext = viewModel;

			var symbolKeys = symbolFirstMap.FirstLevelKeys;
			foreach (var symbolKey in symbolKeys)
			{
				var symbolName = symbolKey.Values.Single () as string;
				var multiCulturalViewModel = new MultiCultureResourceItemViewModel (symbolName);
				viewModel.Items.Add (multiCulturalViewModel);

				var cultureMap = CompositeDictionary.Create (symbolFirstMap[symbolKey]);
				var cultureKeys = cultureMap.Keys;
				foreach (var cultureKey in cultureKeys)
				{
					var culture = cultureKey.Values.Single () as CultureInfo;
					var resourceItem = cultureMap[cultureKey] as ResourceItem;
					multiCulturalViewModel.Items.Add (new ResourceItemViewModel (resourceItem, culture));
				}
			}
			return view;
		}

		public MultiCultureResourceItemCollectionView()
		{
			InitializeComponent ();
		}
	}
}
