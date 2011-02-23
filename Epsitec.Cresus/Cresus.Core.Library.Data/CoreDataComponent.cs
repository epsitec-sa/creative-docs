//	Copyright © 2011, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core
{
	public abstract class CoreDataComponent
	{
		protected CoreDataComponent(CoreData data)
		{
			this.data = data;
		}


		public CoreData Data
		{
			get
			{
				return this.data;
			}
		}


		private readonly CoreData data;
	}
}
