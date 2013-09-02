using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Epsitec.Windows
{
	public class ViewModel : INotifyPropertyChanged
	{
		#region INotifyPropertyChanged Members

		public event PropertyChangedEventHandler PropertyChanged;

		#endregion


		protected void RaiseEvent(string propertyName)
		{
			var handler = this.PropertyChanged;
			if (handler != null)
			{
				handler (this, new PropertyChangedEventArgs (propertyName));
			}
		}

	}
}
