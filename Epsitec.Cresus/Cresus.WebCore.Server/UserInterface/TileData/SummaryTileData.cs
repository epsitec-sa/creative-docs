using Epsitec.Common.Support.EntityEngine;

using Epsitec.Common.Types;

using Epsitec.Cresus.Core.Controllers;

using Epsitec.Cresus.WebCore.Server.Core.PropertyAutoCreator;

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


		public IEnumerable<AbstractTile> ToTiles(PanelBuilder panelBuilder, AbstractEntity entity)
		{
			if (this.Template == null)
			{
				yield return this.ToSummaryTile (panelBuilder, entity);
			}
			else
			{
				var targets = this.Template.EntitiesGetter (entity).ToList ();

				if (targets.Count > 0)
				{
					foreach (var target in targets)
					{
						yield return this.ToCollectionSummaryTile (panelBuilder, target);
					}
				}
				else
				{
					yield return this.ToEmptySummaryTile (panelBuilder);
				}
			}
		}


		#endregion


		public AbstractTile ToSummaryTile(PanelBuilder panelBuilder, AbstractEntity entity)
		{
			return new SummaryTile ()
			{
				IsRoot = this.IsRoot,
				EntityId = panelBuilder.GetEntityId (this.EntityGetter(entity)),
				IconClass = panelBuilder.GetIconClass (this.EntityType, this.Icon),
				SubViewControllerMode = Tools.ViewControllerModeToString (this.SubViewControllerMode),
				SubViewControllerSubTypeId = Tools.ControllerSubTypeIdToString (this.SubViewControllerSubTypeId),
				Text = this.TextGetter (entity).ToString (),
				Title = this.TitleGetter (entity).ToString (),
				AutoCreatorId = this.GetAutoCreatorId ()
			};
		}


		public AbstractTile ToEmptySummaryTile(PanelBuilder panelBuilder)
		{
			return new EmptySummaryTile ()
			{
				EntityType = panelBuilder.GetTypeName (this.Template.EntityType),
				PropertyAccessorId = InvariantConverter.ToString (this.Template.PropertyAccessor.Id),
			};
		}


		public AbstractTile ToCollectionSummaryTile(PanelBuilder panelBuilder, AbstractEntity entity)
		{
			CollectionTileData template = this.Template;

			return new CollectionSummaryTile ()
			{
				EntityId = panelBuilder.GetEntityId (entity),
				IconClass = panelBuilder.GetIconClass (template.EntityType, template.Icon),
				SubViewControllerMode = Tools.ViewControllerModeToString (this.SubViewControllerMode),
				SubViewControllerSubTypeId = Tools.ControllerSubTypeIdToString (this.SubViewControllerSubTypeId),
				Text = template.TextGetter (entity).ToString (),
				Title = template.TitleGetter (entity).ToString (),
				EntityType = panelBuilder.GetTypeName (template.EntityType),
				PropertyAccessorId = InvariantConverter.ToString (template.PropertyAccessor.Id),
				HideAddButton = this.HideAddButton,
				HideRemoveButton = this.HideRemoveButton,
			};
		}


		private string GetAutoCreatorId()
		{
			return this.AutoCreator == null
				? null
				: InvariantConverter.ToString (this.AutoCreator.Id);
		}


	}


}

