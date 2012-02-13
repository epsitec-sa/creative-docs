//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using Epsitec.Common.Types;

using Epsitec.Cresus.Compta.Accessors;
using Epsitec.Cresus.Compta.Search.Data;
using Epsitec.Cresus.Compta.Options.Data;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Compta.Memory.Data
{
	public class MemoryList : ISettingsData
	{
		public MemoryList()
		{
			this.list = new List<MemoryData> ();
		}


		public List<MemoryData> List
		{
			get
			{
				return this.list;
			}
		}


		private readonly List<MemoryData>	list;
	}
}