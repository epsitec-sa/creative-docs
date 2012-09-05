//	Copyright © 2012, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support;
using Epsitec.Common.Support.Extensions;

using Epsitec.Cresus.Core.Metadata;

using System.Linq.Expressions;
using System.Xml.Linq;

[assembly: XmlNodeClass ("set", typeof (ColumnFilterSetExpression))]

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

		public override XElement Save(string xmlNodeName)
		{
			//	TODO: set serialization

			throw new System.NotImplementedException ();
		}

		public static new ColumnFilterSetExpression Restore(XElement xml)
		{
			return new ColumnFilterSetExpression ()
			{
				//	TODO: set initialisation
			};
		}
	}
}
