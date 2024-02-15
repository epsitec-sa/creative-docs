//	Copyright © 2012, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Xml.Linq;
using Epsitec.Common.Support;
using Epsitec.Common.Support.Extensions;
using Epsitec.Common.Types;

namespace Epsitec.Common.Support.EntityEngine
{
    /// <summary>
    /// The <c>EntityColumn</c> class is used to represent a column, related to
    /// an entity; the column is reached through an <see cref="EntityFieldPath"/>
    /// expressed as an expression.
    /// </summary>
    public class EntityColumn
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EntityColumn"/> class. This should
        /// not be called directly. Use <see cref="EntityMetadataRecorder.Column"/> instead.
        /// </summary>
        /// <param name="expression">The lambda expression (as an expression, not as compiled code).</param>
        /// <param name="title">The (optional) title associated with the column.</param>
        /// <param name="captionId">The (optional) caption id associated with the column.</param>
        /// <param name="canSort">If set to <c>true</c>, the column supports sorting.</param>
        /// <param name="canFilter">if set to <c>true</c>, the column supports filtering.</param>
        public EntityColumn(
            LambdaExpression expression,
            FormattedText title = default(FormattedText),
            Druid captionId = default(Druid),
            bool canSort = true,
            bool canFilter = true
        )
        {
            this.expression = expression;
            this.path = EntityFieldPath.CreateAbsolutePath(expression);

            this.captionId = captionId.IsEmpty ? this.GetLeafFieldId() : captionId;

            this.title = title;
            this.canSort = canSort;
            this.canFilter = canFilter;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="EntityColumn"/> class. This method
        /// is used when deserializing a column definition.
        /// </summary>
        /// <param name="data">The data.</param>
        protected EntityColumn(IDictionary<string, string> data)
            : this(
                EntityFieldPath.Parse(data[Strings.Path]).CreateLambda(),
                new FormattedText(data[Strings.Title]),
                Druid.Parse(data[Strings.CaptionId]),
                bool.Parse(data[Strings.CanSort] ?? "false"),
                bool.Parse(data[Strings.CanFilter] ?? "false")
            ) { }

        /// <summary>
        /// Gets the id of the column. This is guaranteed to be unique for a given entity.
        /// It is basically the encoding of the relative field path.
        /// </summary>
        public string Id
        {
            get { return this.path.FieldPath; }
        }

        public LambdaExpression Expression
        {
            get { return this.expression; }
        }

        public Druid EntityId
        {
            get { return this.Path.EntityId; }
        }

        public EntityFieldPath Path
        {
            get { return this.path; }
        }

        public Druid CaptionId
        {
            get { return this.captionId; }
        }

        public FormattedText Title
        {
            get { return this.title; }
        }

        public bool CanSort
        {
            get { return this.canSort; }
        }

        public bool CanFilter
        {
            get { return this.canFilter; }
        }

        /// <summary>
        /// Gets the title of the column.
        /// </summary>
        /// <returns>The name of the column.</returns>
        public FormattedText GetColumnTitle()
        {
            if (this.title.IsNotNull())
            {
                return this.title;
            }

            var captionId = this.captionId.IsEmpty
                ? EntityInfo.GetFieldCaptionId(this.expression)
                : this.captionId;

            return TextFormatter.GetCurrentCultureText(captionId);
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
            var fieldPath = ExpressionAnalyzer.ExplodeLambda(this.Expression, trimCount: 1);
            return EntityInfo.WalkEntityGraph(root, fieldPath, nullNodeAction);
        }

        /// <summary>
        /// Gets the ID of the leaf field.
        /// </summary>
        /// <returns>The ID of the leaf field.</returns>
        public Druid GetLeafFieldId()
        {
            return Druid.Parse(this.path.GetLeafFieldId());
        }

        /// <summary>
        /// Saves the column definition into an XML element witht the specified name.
        /// </summary>
        /// <param name="xmlNodeName">Name of the XML element.</param>
        /// <returns>The XML element.</returns>
        public XElement Save(string xmlNodeName)
        {
            var attributes = new List<XAttribute>();
            var elements = new List<XElement>();

            this.Serialize(attributes);
            this.Serialize(elements);

            return new XElement(
                xmlNodeName,
                attributes.Where(x => !string.IsNullOrEmpty(x.Value)),
                elements
            );
        }

        /// <summary>
        /// Restores the column definition from the specified XML element.
        /// </summary>
        /// <param name="xml">The XML element.</param>
        /// <returns></returns>
        public static EntityColumn Restore(XElement xml)
        {
            var typeName = TypeEnumerator.Instance.UnshrinkTypeName(
                xml.Attribute(Strings.Type).Value
            );
            var sysType = TypeEnumerator.Instance.FindType(typeName);
            var data = Xml.GetAttributeBag(xml);
            var instance = System.Activator.CreateInstance(sysType, data) as EntityColumn;

            instance.Deserialize(xml);

            return instance;
        }

        /// <summary>
        /// Serializes the specified attributes. When overridden, this method must call the base
        /// class to include its attributes too.
        /// </summary>
        /// <param name="attributes">The attributes.</param>
        protected virtual void Serialize(List<XAttribute> attributes)
        {
            attributes.Add(
                new XAttribute(
                    Strings.Type,
                    TypeEnumerator.Instance.ShrinkTypeName(this.GetType().FullName)
                )
            );
            attributes.Add(new XAttribute(Strings.Path, this.path.ToString()));
            attributes.Add(new XAttribute(Strings.CaptionId, this.captionId.ToCompactString()));
            attributes.Add(new XAttribute(Strings.Title, this.title.ToString()));
            attributes.Add(new XAttribute(Strings.CanSort, this.canSort.ToString()));
            attributes.Add(new XAttribute(Strings.CanFilter, this.canFilter.ToString()));
        }

        protected virtual void Serialize(List<XElement> elements) { }

        protected virtual void Deserialize(XElement xml) { }

        #region Strings Class

        private static class Strings
        {
            public static readonly string Type = "t";
            public static readonly string Path = "p";
            public static readonly string CaptionId = "cid";
            public static readonly string Title = "title";
            public static readonly string CanSort = "canSort";
            public static readonly string CanFilter = "canFilter";
        }

        #endregion

        private readonly LambdaExpression expression;
        private readonly EntityFieldPath path;
        private readonly Druid captionId;
        private readonly FormattedText title;
        private readonly bool canSort;
        private readonly bool canFilter;
    }
}
