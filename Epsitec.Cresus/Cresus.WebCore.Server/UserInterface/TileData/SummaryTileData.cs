using Epsitec.Common.Support.EntityEngine;

using Epsitec.Cresus.Core.Controllers;

using Epsitec.Cresus.WebCore.Server.UserInterface.Tile;

using System;

using System.Collections.Generic;

using System.Linq;
using System.Linq.Expressions;


namespace Epsitec.Cresus.WebCore.Server.UserInterface.TileData
{


	internal sealed class SummaryTileData : AbstractSummaryTileData, ITileData
	{


		// TODO Add Separator, GlobalWarning ?


		public Func<AbstractEntity, AbstractEntity> EntityGetter
		{
			get;
			set;
		}


		public ViewControllerMode SubViewControllerMode
		{
			get;
			set;
		}


		public int? SubViewControllerSubTypeId
		{
			get;
			set;
		}


		public bool HideAddButton
		{
			get;
			set;
		}


		public bool HideRemoveButton
		{
			get;
			set;
		}


		public CollectionTileData Template
		{
			get;
			set;
		}


		#region ITileData Members


		public IEnumerable<AbstractTile> ToTiles(AbstractEntity entity, Func<AbstractEntity, string> entityIdGetter, Func<string, string> iconClassGetter, Func<LambdaExpression, string> lambdaIdGetter, Func<Type, string> typeGetter, Func<AbstractEntity, IEnumerable<AbstractTile>> editionTileBuilder, Func<Type, IEnumerable<AbstractEntity>> entitiesGetter, Func<LambdaExpression, PanelFieldAccessor> panelFieldAccessorGetter)
		{
			if (this.Template == null)
			{
				yield return this.ToSummaryTile (entity, entityIdGetter, iconClassGetter);
			}
			else
			{
				var targets = this.Template.CollectionGetter (entity).ToList ();

				if (targets.Count > 0)
				{
					foreach (var target in targets)
					{
						yield return this.ToCollectionSummaryTile (target, entityIdGetter, iconClassGetter, lambdaIdGetter, typeGetter);
					}
				}
				else
				{
					yield return this.ToEmptySummaryTile (lambdaIdGetter, typeGetter);
				}
			}
		}


		#endregion


		public AbstractTile ToSummaryTile(AbstractEntity entity, Func<AbstractEntity, string> entityIdGetter, Func<string, string> iconClassGetter)
		{
			return new SummaryTile ()
			{
				EntityId = entityIdGetter (this.EntityGetter(entity)),
				IconClass = iconClassGetter (this.Icon),
				SubViewControllerMode = Tools.ViewControllerModeToString (this.SubViewControllerMode),
				SubViewControllerSubTypeId = Tools.ControllerSubTypeIdToString (this.SubViewControllerSubTypeId),
				Text = this.TextGetter (entity).ToString (),
				Title = this.TitleGetter (entity).ToString (),
			};
		}


		public AbstractTile ToEmptySummaryTile(Func<LambdaExpression, string> lambdaIdGetter, Func<Type, string> typeGetter)
		{
			return new EmptySummaryTile ()
			{
				EntityType = typeGetter (this.Template.EntityType),
				LambdaId = lambdaIdGetter (this.Template.Lambda),
			};
		}


		public AbstractTile ToCollectionSummaryTile(AbstractEntity entity, Func<AbstractEntity, string> entityIdGetter, Func<string, string> iconClassGetter, Func<LambdaExpression, string> lambdaIdGetter, Func<Type, string> typeGetter)
		{
			return new CollectionSummaryTile ()
			{
				EntityId = entityIdGetter (entity),
				IconClass = iconClassGetter (this.Template.Icon),
				SubViewControllerMode = Tools.ViewControllerModeToString (this.SubViewControllerMode),
				SubViewControllerSubTypeId = Tools.ControllerSubTypeIdToString (this.SubViewControllerSubTypeId),
				Text = this.Template.TextGetter (entity).ToString (),
				Title = this.Template.TitleGetter (entity).ToString (),
				EntityType = typeGetter (this.Template.EntityType),
				LambdaId = lambdaIdGetter (this.Template.Lambda),
				HideAddButton = this.HideAddButton,
				HideRemoveButton = this.HideRemoveButton,
			};
		}


	}


}

