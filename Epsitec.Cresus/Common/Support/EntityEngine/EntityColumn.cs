//	Copyright © 2012, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support;
using Epsitec.Common.Support.Extensions;
using Epsitec.Common.Types;

using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Xml.Linq;

namespace Epsitec.Common.Support.EntityEngine
{
	public class EntityColumn
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="EntityDataColumn"/> class. This should
		/// not be called directly. Use <see cref="EntityDataMetadataRecorder.Column"/> instead.
		/// </summary>
		/// <param name="expression">The lambda expression (as an expression, not as compiled code).</param>
		/// <param name="name">The name associated with the column.</param>
		public EntityColumn(LambdaExpression expression, FormattedText name)
		{
			this.expression = expression;
			this.path       = EntityFieldPath.CreateAbsolutePath (expression);
			this.name       = name;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="EntityColumn"/> class. This method
		/// is used when deserializing a column definition.
		/// </summary>
		/// <param name="data">The data.</param>
		public EntityColumn(IDictionary<string, string> data)
		{
			this.path       = EntityFieldPath.Parse (data["path"]);
			this.captionId  = Druid.Parse (data["cap"]);
			this.expression = this.path.CreateLambda () as LambdaExpression;
		}


		public LambdaExpression					Expression
		{
			get
			{
				return this.expression;
			}
		}

		public EntityFieldPath					Path
		{
			get
			{
				return this.path;
			}
		}


		/// <summary>
		/// Gets the name of the column.
		/// </summary>
		/// <returns>The name of the column.</returns>
		public FormattedText GetName()
		{
			if (this.name.IsNotNull ())
			{
				return this.name;
			}

			var captionId = this.captionId.IsEmpty
							? EntityInfo.GetFieldCaptionId (this.expression)
							: this.captionId;

			return TextFormatter.GetCurrentCultureText (captionId);
		}

		/// <summary>
		/// Gets the leaf entity, starting at the specified root and stopping before the last
		/// field gets resolved.
		/// </summary>
		/// <param name="root">The root.</param>
		/// <param name="nullNodeAction">The null node action.</param>
		/// <returns>The leaf entity.</returns>
		public AbstractEntity GetLeafEntity(AbstractEntity root, NullNodeAction nullNodeAction)
		{
			var fieldPath = ExpressionAnalyzer.ExplodeLambda (this.Expression, trimCount: 1);
			return EntityInfo.WalkEntityGraph (root, fieldPath, nullNodeAction);
		}

		/// <summary>
		/// Gets the ID of the leaf field.
		/// </summary>
		/// <returns>The ID of the leaf field.</returns>
		public Druid GetLeafFieldId()
		{
			return Druid.Parse (this.path.GetLeafFieldId ());
		}

		/// <summary>
		/// Saves the column definition into an XML element witht the specified name.
		/// </summary>
		/// <param name="xmlNodeName">Name of the XML element.</param>
		/// <returns>The XML element.</returns>
		public XElement Save(string xmlNodeName)
		{
			List<XAttribute> attributes = new List<XAttribute> ();

			this.Serialize (attributes);

			return new XElement (xmlNodeName, attributes);
		}

		/// <summary>
		/// Restores the column definition from the specified XML element.
		/// </summary>
		/// <param name="xml">The XML element.</param>
		/// <returns></returns>
		public static EntityColumn Restore(XElement xml)
		{
			var typeName = xml.Attribute ("type").Value;
			var sysType  = TypeEnumerator.Instance.FindType (typeName);
			var arg      = xml.Attributes ().ToDictionary (x => x.Name.LocalName, x => x.Value);
			var instance = System.Activator.CreateInstance (sysType, arg) as EntityColumn;
			
			return instance;
		}


		/// <summary>
		/// Serializes the specified attributes. When overridden, this method must call the base
		/// class to include its attributes too.
		/// </summary>
		/// <param name="attributes">The attributes.</param>
		protected virtual void Serialize(List<XAttribute> attributes)
		{
			attributes.Add (new XAttribute ("type", this.GetType ().FullName));
			attributes.Add (new XAttribute ("path", this.path.ToString ()));
			attributes.Add (new XAttribute ("cap", this.captionId.ToString ()));
		}


		private readonly LambdaExpression		expression;
		private readonly EntityFieldPath		path;
		private readonly Druid					captionId;
		private readonly FormattedText			name;
	}
}
