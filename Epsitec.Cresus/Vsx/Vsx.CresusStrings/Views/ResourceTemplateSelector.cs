using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using Epsitec.Cresus.Strings.ViewModels;

namespace Epsitec.Cresus.Strings.Views
{
	public class ResourceTemplateSelector : DataTemplateSelector
	{
		public DataTemplate Compact
		{
			get;
			set;
		}
		public DataTemplate Default
		{
			get;
			set;
		}

		public override DataTemplate SelectTemplate(object item, DependencyObject container)
		{
			if (item is MultiCultureResourceItemViewModel)
			{
				var viewModel = item as MultiCultureResourceItemViewModel;
				if (viewModel.Count == 1)
				{
					return this.Compact;
				}
			}
			return Default;
		}
	}
}
