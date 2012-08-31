//	Copyright © 2012, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Types;
using Epsitec.Common.Support.Extensions;

using System.Linq.Expressions;

namespace Epsitec.Cresus.Core.Metadata
{
	public class ColumnFilterSetExpression : ColumnFilterExpression
	{
		public override bool IsValid
		{
			get
			{
				throw new System.NotImplementedException ();
			}
		}

		public override Expression GetExpression(Expression parameter)
		{
			throw new System.NotImplementedException ();
		}
	}
}
