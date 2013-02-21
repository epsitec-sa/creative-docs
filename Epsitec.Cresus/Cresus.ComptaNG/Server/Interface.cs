using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Epsitec.Cresus.ComptaNG.Data;

namespace Epsitec.Cresus.ComptaNG.Server
{
	public class Interface
	{
		public Guid OpenView(View view)
		{
			return System.Guid.NewGuid ();
		}

		public int GetCount(Guid guid)
		{
			return 0;
		}

		public List<AbstractObjetComptable> GetData(Guid guid, int firstLine, int count)
		{
			return null;
		}

		public void SetData(Guid guid, AbstractObjetComptable data)
		{
		}

		public void CloseView(Guid guid)
		{
		}
	}
}
