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
	/// Interaction logic for MultiCultureResourceItemView.xaml
	/// </summary>
	public partial class MultiCultureResourceItemView : UserControl
	{
		public MultiCultureResourceItemView (IReadOnlyDictionary<CultureInfo, ResourceItem> resources)
			: this ()
		{
			this.DataContext = new MultiCultureResourceItemViewModel (resources);
		}

		public MultiCultureResourceItemView()
		{
			this.InitializeComponent ();
		}
	}
}
