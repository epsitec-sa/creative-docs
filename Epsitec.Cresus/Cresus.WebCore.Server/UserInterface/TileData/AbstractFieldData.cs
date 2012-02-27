using Epsitec.Common.Support.EntityEngine;

using Epsitec.Common.Types;

using Epsitec.Cresus.WebCore.Server.UserInterface.PanelFieldAccessor;
using Epsitec.Cresus.WebCore.Server.UserInterface.Tile;

using System;

using System.Collections.Generic;

using System.Linq.Expressions;


namespace Epsitec.Cresus.WebCore.Server.UserInterface.TileData
{


	internal abstract class AbstractFieldData : AbstractEditionTilePartData
	{


		// TODO Add PickFromCollection, ReadOnly, Width, Height


		public FormattedText Title
		{
			get;
			set;
		}


		public LambdaExpression Lambda
		{
			get;
			set;
		}


		public bool IsReadOnly
		{
			get;
			set;
		}


		public override AbstractEditionTilePart ToAbstractEditionTilePart(AbstractEntity entity, Func<AbstractEntity, string> entityIdGetter, Func<Type, IEnumerable<AbstractEntity>> entitiesGetter, Func<LambdaExpression, AbstractPanelFieldAccessor> panelFieldAccessorGetter)
		{
			return this.ToAbstractField (entity, entityIdGetter, entitiesGetter, panelFieldAccessorGetter);	
		}


		public AbstractField ToAbstractField(AbstractEntity entity, Func<AbstractEntity, string> entityIdGetter, Func<Type, IEnumerable<AbstractEntity>> entitiesGetter, Func<LambdaExpression, AbstractPanelFieldAccessor> panelFieldAccessorGetter)
		{
			var panelFieldAccessor = panelFieldAccessorGetter (this.Lambda);

			return this.ToAbstractField (entity, entityIdGetter, entitiesGetter, panelFieldAccessor);
		}


		protected abstract AbstractField ToAbstractField(AbstractEntity entity, Func<AbstractEntity, string> entityIdGetter, Func<Type, IEnumerable<AbstractEntity>> entitiesGetter, AbstractPanelFieldAccessor panelFieldAccessor);


	}


}

