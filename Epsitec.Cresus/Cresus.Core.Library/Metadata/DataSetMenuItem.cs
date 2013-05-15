using Epsitec.Common.Support;
using Epsitec.Common.Support.EntityEngine;
using Epsitec.Common.Types;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Xml.Linq;

namespace Epsitec.Cresus.Core.Metadata
{
	public sealed class DataSetMenuItem
	{
		public DataSetMenuItem(Druid dataSetId, FormattedText title, LambdaExpression expression)
		{
			this.dataSetId = dataSetId;
			this.title = title;
			this.expression = expression;
			this.path = EntityFieldPath.CreateAbsolutePath (expression);
		}

		public Druid							DataSetId
		{
			get
			{
				return this.dataSetId;
			}
		}

		public FormattedText					Title
		{
			get
			{
				return this.title;
			}
		}

		public EntityFieldPath					Path
		{
			get
			{
				return this.path;
			}
		}

		public LambdaExpression					Expression
		{
			get
			{
				return this.expression;
			}
		}

		public XElement Save()
		{
			var attributes = new List<XAttribute> ();

			attributes.Add (new XAttribute (Strings.DataSetId, this.dataSetId.ToCompactString ()));
			attributes.Add (new XAttribute (Strings.Title, this.title.ToString ()));
			attributes.Add (new XAttribute (Strings.Path, this.path.ToString ()));

			return new XElement (Strings.MenuItem, attributes);
		}

		public static DataSetMenuItem Restore(XElement xml)
		{
			if (xml.Name.LocalName != Strings.MenuItem)
			{
				throw new ArgumentException ("Invalid xml element name.");
			}

			var data = Xml.GetAttributeBag (xml);

			var dataSetId = Druid.Parse (data[Strings.DataSetId]);
			var title = new FormattedText (data[Strings.Title]);
			var expression = EntityFieldPath.Parse (data[Strings.Path]).CreateLambda ();

			return new DataSetMenuItem (dataSetId, title, expression);
		}

		#region Strings Class

		private static class Strings
		{
			public static readonly string		MenuItem = "m";
			public static readonly string		DataSetId = "d";
			public static readonly string		Title = "t";
			public static readonly string		Path = "p";
		}

		#endregion

		private readonly Druid					dataSetId;
		private readonly FormattedText			title;
		private readonly EntityFieldPath		path;
		private readonly LambdaExpression		expression;
	}
}
