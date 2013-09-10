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
	/// Interaction logic for ResourceSymbolCollectionView.xaml
	/// </summary>
	public partial class ResourceSymbolCollectionView : UserControl
	{
		public static ResourceSymbolCollectionView Create(IEnumerable<IReadOnlyDictionary<CultureInfo, ResourceField>> resources, double maxHeight = double.PositiveInfinity)
		{
			var view = new ResourceSymbolCollectionView ()
			{
				MaxHeight = maxHeight
			};
			var viewModel = new ResourceSymbolCollectionViewModel ();
			view.DataContext = viewModel;

			foreach (var cultureMap in resources)
			{
				foreach (var kv in cultureMap)
				{
					var symbolViewModel = new ResourceSymbolViewModel ();
					viewModel.Add (symbolViewModel);
				}
			}

			return view;
		}

		public ResourceSymbolCollectionView()
		{
			InitializeComponent ();
		}
	}
}
