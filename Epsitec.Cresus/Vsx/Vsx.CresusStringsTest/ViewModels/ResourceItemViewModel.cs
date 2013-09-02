using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Epsitec.Cresus.Strings
{
	public class ResourceItemViewModel : INotifyPropertyChanged
	{
		public string Value
		{
			get
			{
				return this.value;
			}
			set
			{
				if (this.value != value)
				{
					this.value = value;
					this.RaiseEvent ("Value");
				}
			}
		}

		#region INotifyPropertyChanged Members

		public event PropertyChangedEventHandler PropertyChanged;

		#endregion


		private void RaiseEvent(string propertyName)
		{
			var handler = this.PropertyChanged;
			if (handler != null)
			{
				handler (this, new PropertyChangedEventArgs (propertyName));
			}
		}


		private string value;
	}
}
