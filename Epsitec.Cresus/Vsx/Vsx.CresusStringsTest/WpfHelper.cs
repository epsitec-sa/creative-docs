using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Epsitec.Cresus.Strings
{
	public class WpfHelper
	{
		public static Window CreateWindow()
		{
			var window = new Window ();
			var resourceDictionary = new ResourceDictionary ();
			resourceDictionary.Source = new Uri ("pack://application:,,,/Vsx.CresusStrings;component/CresusStrings.xaml");
			window.Resources.MergedDictionaries.Add (resourceDictionary);
			return window;
		}
	}
}
