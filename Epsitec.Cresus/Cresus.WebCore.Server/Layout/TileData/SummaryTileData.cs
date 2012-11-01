using Epsitec.Common.Support.EntityEngine;

using Epsitec.Common.Types;

using Epsitec.Cresus.Core.Controllers;

using Epsitec.Cresus.WebCore.Server.Core.PropertyAutoCreator;

using Epsitec.Cresus.WebCore.Server.Layout.Tile;

using System;

using System.Collections.Generic;

using System.Linq;


namespace Epsitec.Cresus.WebCore.Server.Layout.TileData
{


	internal sealed class SummaryTileData : AbstractSummaryTileData, ITileData
	{


		public Func<AbstractEntity, AbstractEntity> EntityGetter
		{
			get;
			set;
		}


		public ViewControllerMode SubViewMode
		{
			get;
			set;
		}


		public int? SubViewId
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


		public bool IsRoot
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


		public IEnumerable<AbstractTile> ToTiles(LayoutBuilder layoutBuilder, AbstractEntity entity)
		{
			if (this.Template == null)
			{
				yield return this.ToSummaryTile (layoutBuilder, entity);
			}
			else
			{
				var targets = this.Template.EntitiesGetter (entity).ToList ();

				if (targets.Count > 0)
				{
					foreach (var target in targets)
					{
						yield return this.ToCollectionSummaryTile (layoutBuilder, target);
					}
				}
				else
				{
					yield return this.ToEmptySummaryTile (layoutBuilder);
				}
			}
		}


		#endregion


		public AbstractTile ToSummaryTile(LayoutBuilder layoutBuilder, AbstractEntity entity)
		{
			return new SummaryTile ()
			{
				IsRoot = this.IsRoot,
				EntityId = layoutBuilder.GetEntityId (this.EntityGetter(entity)),
				IconClass = layoutBuilder.GetIconClass (this.EntityType, this.Icon),
				SubViewMode = Tools.ViewModeToString (this.SubViewMode),
				SubViewId = Tools.ViewIdToString (this.SubViewId),
				Text = this.TextGetter (entity).ToString (),
				Title = this.TitleGetter (entity).ToString (),
				AutoCreatorId = this.GetAutoCreatorId ()
			};
		}


		public AbstractTile ToEmptySummaryTile(LayoutBuilder layoutBuilder)
		{
			return new EmptySummaryTile ()
			{
				EntityId = null,
				IconClass = layoutBuilder.GetIconClass (this.Template.EntityType, this.Template.Icon),
				SubViewMode = Tools.ViewModeToString (this.SubViewMode),
				SubViewId = Tools.ViewIdToString (this.SubViewId),
				Text = null,
				Title = null,
				EntityType = layoutBuilder.GetTypeName (this.Template.EntityType),
				PropertyAccessorId = this.Template.PropertyAccessor.Id,
				HideAddButton = this.HideAddButton,
				HideRemoveButton = this.HideRemoveButton,	
			};
		}


		public AbstractTile ToCollectionSummaryTile(LayoutBuilder layoutBuilder, AbstractEntity entity)
		{
			return new CollectionSummaryTile ()
			{
				EntityId = layoutBuilder.GetEntityId (entity),
				IconClass = layoutBuilder.GetIconClass (this.Template.EntityType, this.Template.Icon),
				SubViewMode = Tools.ViewModeToString (this.SubViewMode),
				SubViewId = Tools.ViewIdToString (this.SubViewId),
				Text = this.Template.TextGetter (entity).ToString (),
				Title = this.Template.TitleGetter (entity).ToString (),
				EntityType = layoutBuilder.GetTypeName (this.Template.EntityType),
				PropertyAccessorId = this.Template.PropertyAccessor.Id,
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

