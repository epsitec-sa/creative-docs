//	Copyright © 2012, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support.EntityEngine;
using Epsitec.Common.Support.Extensions;
using Epsitec.Common.Types;
using Epsitec.Cresus.DataLayer.Expressions;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Xml.Linq;
using Epsitec.Common.Support;

namespace Epsitec.Cresus.Core.Metadata
{
	/// <summary>
	/// The <c>EntityColumnSort</c> class defines the sorting for an <see cref="EntityColumn"/>.
	/// </summary>
	public class EntityColumnSort : IXmlNodeClass
	{
		public EntityColumnSort()
		{
			this.SortOrder = ColumnSortOrder.None;
		}
		
		
		public ColumnSortOrder					SortOrder
		{
			get;
			set;
		}

		
		public SortClause ToSortClause(EntityColumn column, AbstractEntity example)
		{
			if (this.SortOrder == ColumnSortOrder.None)
			{
				return null;
			}

			var fieldEntity = column.GetLeafEntity (example, NullNodeAction.CreateMissing);
			var fieldId     = column.GetLeafFieldId ();

			var fieldNode = new ValueField (fieldEntity, fieldId);
			var sortOrder = EntityColumnSort.Convert (this.SortOrder);

			return new SortClause (fieldNode, sortOrder);
		}

		#region IXmlNodeClass Members

		public XElement Save(string xmlNodeName)
		{
			return new XElement (xmlNodeName,
				new XAttribute (Xml.SortOrder, this.SortOrder.ToString ()));
		}

		#endregion

		public static EntityColumnSort Restore(XElement xml)
		{
			if (xml == null)
			{
				return null;
			}

			return new EntityColumnSort
			{
				SortOrder = InvariantConverter.ToEnum (xml.Attribute (Xml.SortOrder), ColumnSortOrder.None)
			};
		}


		#region Xml Class

		private static class Xml
		{
			public static readonly string		SortOrder = "o";
		}

		#endregion
		
		
		public static ColumnSortOrder Convert(SortOrder value)
		{
			switch (value)
			{
				case Epsitec.Cresus.DataLayer.Expressions.SortOrder.Ascending:
					return ColumnSortOrder.Ascending;
				case Epsitec.Cresus.DataLayer.Expressions.SortOrder.Descending:
					return ColumnSortOrder.Descending;
			}

			throw new System.NotImplementedException (string.Format ("{0} not implemented", value.GetQualifiedName ()));
		}

		public static SortOrder Convert(ColumnSortOrder value)
		{
			switch (value)
			{
				case ColumnSortOrder.Ascending:
					return Epsitec.Cresus.DataLayer.Expressions.SortOrder.Ascending;
				case ColumnSortOrder.Descending:
					return Epsitec.Cresus.DataLayer.Expressions.SortOrder.Descending;
			}

			throw new System.NotImplementedException (string.Format ("{0} not implemented", value.GetQualifiedName ()));
		}
	}
}
