//	Copyright © 2012, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support.EntityEngine;

using System.Xml.Linq;
using System.Linq;
using System.Linq.Expressions;
using Epsitec.Common.Support;

namespace Epsitec.Cresus.Core.Metadata
{
	/// <summary>
	/// The <c>EntityColumnFilter</c> class defines the filtering conditions for an
	/// <see cref="EntityColumn"/>.
	/// </summary>
	public sealed class EntityColumnFilter : IFilter, IXmlNodeClass
	{
		public EntityColumnFilter(ColumnFilterExpression filterExpression = null)
		{
			this.filterExpression = filterExpression;
		}


		public ColumnFilterExpression			FilterExpression
		{
			get
			{
				return this.filterExpression;
			}
		}


		#region IFilter Members

		public bool IsValid
		{
			get
			{
				return this.filterExpression != null
					&& this.filterExpression.IsValid;
			}
		}

		public Expression GetExpression(Expression parameter)
		{
			if (this.filterExpression == null)
			{
				return null;
			}

			return this.filterExpression.GetExpression (parameter);
		}

		#endregion
		

		#region IXmlNodeClass Members
		
		public XElement Save(string xmlNodeName)
		{
			return new XElement (xmlNodeName, XmlNodeClassFactory.Save (this.filterExpression));
		}

		#endregion


		public static EntityColumnFilter Restore(XElement xml)
		{
			if (xml == null)
			{
				return null;
			}

			var element = xml.Elements ().First ();
			
			return new EntityColumnFilter (ColumnFilterExpression.Restore (element));
		}


		#region Strings Class

		private static class Strings
		{
		}

		#endregion


		private readonly ColumnFilterExpression	filterExpression;
	}
}