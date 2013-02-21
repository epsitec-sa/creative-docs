using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Epsitec.Cresus.ComptaNG.Common.Data;

namespace Epsitec.Cresus.ComptaNG.Common.RecordAccessor
{
	public abstract class AbstractRecord
	{
		public AbstractRecord(AbstractObjetComptable data)
		{
			this.data = data;
		}

		public virtual IEnumerable<FieldType> Fields
		{
			get
			{
				return null;
			}
		}

		public virtual AbstractField GetField(FieldType field)
		{
			return null;
		}

		public virtual void SetField(FieldType field, AbstractField content)
		{
		}


		protected AbstractObjetComptable data;
	}
}
