using Epsitec.Common.Types;

using System.Collections.Generic;


namespace Epsitec.Cresus.DataLayer.Tests.Vs.Proxies
{


	internal sealed class TestStore : IValueStore
	{


		public TestStore()
		{
			this.data = new Dictionary<string, object> ();
		}


		#region IValueStore Members


		public object GetValue(string id)
		{
			if (this.data.ContainsKey (id))
			{
				return data[id];
			}
			else
			{
				return null;
			}
		}


		public void SetValue(string id, object value, ValueStoreSetMode mode)
		{
			this.data[id] = value;
		}


		#endregion


		private readonly Dictionary<string, object> data;


	}


}
