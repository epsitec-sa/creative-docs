using Epsitec.Common.Support.EntityEngine;

using Epsitec.Cresus.Core.Controllers;
using Epsitec.Cresus.WebCore.Server.UserInterface.PropertyAutoCreator;
using Epsitec.Cresus.WebCore.Server.UserInterface.Tile;

using System;

using System.Collections.Generic;

using System.Linq;


namespace Epsitec.Cresus.WebCore.Server.UserInterface.TileData
{


	internal sealed class SummaryTileData : AbstractSummaryTileData, ITileData
	{


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


		public AutoCreator AutoCreator
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


		public IEnumerable<AbstractTile> ToTiles(AbstractEntity entity, Func<AbstractEntity, string> entityIdGetter, Func<Type, string, string> iconClassGetter, Func<Type, string> typeGetter, Func<AbstractEntity, IEnumerable<AbstractTile>> editionTileBuilder, Func<Type, IEnumerable<AbstractEntity>> entitiesGetter)
		{
			if (this.Template == null)
			{
				yield return this.ToSummaryTile (entity, entityIdGetter, iconClassGetter);
			}
			else
			{
				var targets = this.Template.EntitiesGetter (entity).ToList ();

				if (targets.Count > 0)
				{
					foreach (var target in targets)
					{
						yield return this.ToCollectionSummaryTile (target, entityIdGetter, iconClassGetter, typeGetter);
					}
				}
				else
				{
					yield return this.ToEmptySummaryTile (typeGetter);
				}
			}
		}


		#endregion


		public AbstractTile ToSummaryTile(AbstractEntity entity, Func<AbstractEntity, string> entityIdGetter, Func<Type, string, string> iconClassGetter)
		{
			return new SummaryTile ()
			{
				EntityId = entityIdGetter (this.EntityGetter(entity)),
				IconClass = iconClassGetter (this.EntityType, this.Icon),
				SubViewControllerMode = Tools.ViewControllerModeToString (this.SubViewControllerMode),
				SubViewControllerSubTypeId = Tools.ControllerSubTypeIdToString (this.SubViewControllerSubTypeId),
				Text = this.TextGetter (entity).ToString (),
				Title = this.TitleGetter (entity).ToString (),
				AutoCreatorId = this.GetAutoCreatorId ()
			};
		}


		public AbstractTile ToEmptySummaryTile(Func<Type, string> typeGetter)
		{
			return new EmptySummaryTile ()
			{
				EntityType = typeGetter (this.Template.EntityType),
				PropertyAccessorId = this.Template.PropertyAccessor.Id,
			};
		}


		public AbstractTile ToCollectionSummaryTile(AbstractEntity entity, Func<AbstractEntity, string> entityIdGetter, Func<Type, string, string> iconClassGetter, Func<Type, string> typeGetter)
		{
			CollectionTileData template = this.Template;

			return new CollectionSummaryTile ()
			{
				EntityId = entityIdGetter (entity),
				IconClass = iconClassGetter (template.EntityType, template.Icon),
				SubViewControllerMode = Tools.ViewControllerModeToString (this.SubViewControllerMode),
				SubViewControllerSubTypeId = Tools.ControllerSubTypeIdToString (this.SubViewControllerSubTypeId),
				Text = template.TextGetter (entity).ToString (),
				Title = template.TitleGetter (entity).ToString (),
				EntityType = typeGetter (template.EntityType),
				PropertyAccessorId = template.PropertyAccessor.Id,
				HideAddButton = this.HideAddButton,
				HideRemoveButton = this.HideRemoveButton,
			};
		}


		private string GetAutoCreatorId()
		{
			return this.AutoCreator == null
				? null
				: this.AutoCreator.Id;
		}


	}


}

