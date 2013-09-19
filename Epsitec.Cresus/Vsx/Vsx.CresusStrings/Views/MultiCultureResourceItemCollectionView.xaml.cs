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
		public MultiCultureResourceItemCollectionView (IEnumerable<IReadOnlyDictionary<CultureInfo, ResourceItem>> resources)
			: this ()
		{
			this.DataContext = new MultiCultureResourceItemCollectionViewModel (resources.OrderBy (map => map.SymbolName()));
		}

		public MultiCultureResourceItemCollectionView()
		{
			this.InitializeComponent ();
		}
	}
}
