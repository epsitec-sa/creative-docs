//	Copyright © 2012, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Business
{
	public class BusinessContextEventArgs : EventArgs
	{
		public BusinessContextEventArgs(BusinessContextOperation operation, BusinessContext context)
		{
			this.operation = operation;
			this.context = context;
		}

		
		public BusinessContextOperation			Operation
		{
			get
			{
				return this.operation;
			}
		}

		public BusinessContext					Context
		{
			get
			{
				return this.context;
			}
		}

		
		private readonly BusinessContextOperation	operation;
		private readonly BusinessContext			context;
	}
}
