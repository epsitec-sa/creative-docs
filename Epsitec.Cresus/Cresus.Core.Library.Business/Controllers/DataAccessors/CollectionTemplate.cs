//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support;
using Epsitec.Common.Support.EntityEngine;
using Epsitec.Common.Types;
using Epsitec.Common.Types.Converters;

using Epsitec.Cresus.DataLayer.Context;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Controllers.DataAccessors
{
	/// <summary>
	/// The <c>CollectionTemplate</c> class provides the basic functionality
	/// needed to create and delete items, related to a <see cref="CollectionAccessor"/>.
	/// </summary>
	public abstract class CollectionTemplate
	{
		protected CollectionTemplate(string name)
		{
			this.name = name;
		}

		public string NamePrefix
		{
			get
			{
				return this.name;
			}
		}

		public abstract void GenericDefine(CollectionTemplateProperty property, object value);

		public abstract bool IsCompatible(AbstractEntity entity);

		public abstract void BindTileData(TileDataItem data, AbstractEntity entity, Marshaler marshaler, ICollectionAccessor collectionAccessor);

		public abstract void BindCreateItem(TileDataItem data, ICollectionAccessor collectionAccessor);

		public static readonly FormattedText DefaultEmptyText = TextFormatter.FormatText ("<i>vide</i>");
		public static readonly FormattedText DefaultDefinitionInProgressText = TextFormatter.FormatText ("<i>définition en cours</i>");

		private readonly string name;
	}
}
