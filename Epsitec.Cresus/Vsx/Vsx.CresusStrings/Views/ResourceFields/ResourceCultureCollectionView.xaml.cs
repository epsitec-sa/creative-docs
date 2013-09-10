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
	/// Interaction logic for ResourceCultureCollectionView.xaml
	/// </summary>
	public partial class ResourceCultureCollectionView : UserControl
	{
		public static ResourceCultureCollectionView Create(IEnumerable<IReadOnlyDictionary<CultureInfo, ResourceField>> resources, double maxHeight = double.PositiveInfinity)
		{
			var view = new ResourceCultureCollectionView ()
			{
				MaxHeight = maxHeight
			};
			var viewModel = new ResourceCultureCollectionViewModel ();
			view.DataContext = viewModel;

			foreach (var field in resources.SelectMany (kv => kv.Values))
			{
				var fieldViewModel = new ResourceFieldViewModel (field);
				viewModel.Add (fieldViewModel);
			}

			return view;
		}

		public ResourceCultureCollectionView()
		{
			InitializeComponent ();
		}
	}
}
