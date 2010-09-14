﻿//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support.EntityEngine;
using Epsitec.Common.Types;

using Epsitec.Cresus.Core.Entities;
using Epsitec.Cresus.Core.Controllers.TabIds;
using Epsitec.Cresus.Core.Widgets;

using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Epsitec.Cresus.DataLayer.Context;

namespace Epsitec.Cresus.Core.Controllers.EditionControllers
{
	internal static class Common
	{
		public static string EnumInternalToSingleLine(string value)
		{
			if (string.IsNullOrEmpty (value))
			{
				return null;
			}
			else
			{
				return value.Replace (AbstractArticleParameterDefinitionEntity.Separator, ", ");
			}
		}

		public static string EnumInternalToMultiLine(string value)
		{
			if (string.IsNullOrEmpty (value))
			{
				return null;
			}
			else
			{
				return value.Replace (AbstractArticleParameterDefinitionEntity.Separator, FormattedText.HtmlBreak);
			}
		}

		public static string EnumSingleLineToInternal(string value)
		{
			if (string.IsNullOrEmpty (value))
			{
				return null;
			}
			else
			{
				return value.Replace (", ", AbstractArticleParameterDefinitionEntity.Separator);
			}
		}

		public static string EnumMultiLineToInternal(string value)
		{
			if (string.IsNullOrEmpty (value))
			{
				return null;
			}
			else
			{
				return value.Replace (FormattedText.HtmlBreak, AbstractArticleParameterDefinitionEntity.Separator);
			}
		}

		public static void CreateDocumentItemTabBook<T>(UIBuilder builder, EntityViewController<T> controller, DocumentItemTabId defaultId)
			where T : AbstractDocumentItemEntity, new ()
		{
			var tile = builder.CreateEditionTile ();

			builder.CreateMargin (tile, horizontalSeparator: false);

			var book = builder.CreateTabBook (
				TabPageDef.Create (DocumentItemTabId.Text,    "Texte",      id => Common.ChangeEditedLineEntity (controller, id)),
				TabPageDef.Create (DocumentItemTabId.Article, "Article",    id => Common.ChangeEditedLineEntity (controller, id)),
				TabPageDef.Create (DocumentItemTabId.Price,   "Sous-total", id => Common.ChangeEditedLineEntity (controller, id)));

			book.SelectTabPage (defaultId);
		}


		public static void CreateAbstractArticleParameterTabBook<T>(UIBuilder builder, EntityViewController<T> controller, ArticleParameterTabId defaultId)
			where T : AbstractArticleParameterDefinitionEntity, new ()
		{
			var tile = builder.CreateEditionTile ();

			builder.CreateMargin (tile, horizontalSeparator: false);

			var book = builder.CreateTabBook (
				TabPageDef.Create (ArticleParameterTabId.Numeric, "Valeur numérique", id => Common.ChangeEditedParameterEntity (controller, id)),
				TabPageDef.Create (ArticleParameterTabId.Enum,    "Énumération",      id => Common.ChangeEditedParameterEntity (controller, id)));

			book.SelectTabPage (defaultId);
		}


		private static void ChangeEditedLineEntity<T>(EntityViewController<T> controller, DocumentItemTabId id)
			where T : AbstractDocumentItemEntity, new ()
		{
			var entity       = controller.Entity;
			var dataContext  = controller.DataContext;
			var orchestrator = controller.Orchestrator;

			if (entity != null && entity.TabId == id)
			{
				return;
			}

			var invoiceDocument = dataContext.GetEntitiesOfType<GenericArticleDocumentEntity> (x => x.Lines.Contains (entity)).Single ();
			var navigator       = controller.Navigator;

			navigator.PreserveNavigation (
				delegate
				{
					//	Cherche l'index de la ligne dans la collection.
					int index = invoiceDocument.Lines.IndexOf (entity);

					System.Diagnostics.Debug.Assert (index >= 0);

					//	Ferme la tuile.
					orchestrator.CloseView (controller);

					//	Crée la nouvelle entité.
					AbstractDocumentItemEntity newEntity = null;

					if (id == DocumentItemTabId.Text)
					{
						newEntity = dataContext.CreateEmptyEntity<TextDocumentItemEntity> ();
					}
					else if (id == DocumentItemTabId.Article)
					{
						newEntity = dataContext.CreateEmptyEntity<ArticleDocumentItemEntity> ();

						var article = newEntity as ArticleDocumentItemEntity;
						article.BeginDate = invoiceDocument.CreationDate;
						article.EndDate   = invoiceDocument.CreationDate;
					}
					else if (id == DocumentItemTabId.Price)
					{
						newEntity = dataContext.CreateEmptyEntity<PriceDocumentItemEntity> ();
					}

					System.Diagnostics.Debug.Assert (newEntity != null);
					newEntity.Visibility = true;

					//	Remplace l'entité dans la db.
					invoiceDocument.Lines[index] = newEntity;
					dataContext.DeleteEntity (entity);  // supprime dans le DataContext de la ligne
				});
//-			parentController.TileContainerController.ShowSubView (index, "DocumentItem");
		}

		private static void ChangeEditedParameterEntity<T>(EntityViewController<T> controller, ArticleParameterTabId id)
			where T : AbstractArticleParameterDefinitionEntity, new ()
		{
			var entity       = controller.Entity;
			var dataContext  = controller.DataContext;
			var orchestrator = controller.Orchestrator;

			if (entity != null && entity.TabId == id)
			{
				return;
			}

			var articleDefinition = dataContext.GetEntitiesOfType<ArticleDefinitionEntity> (x => x.ArticleParameterDefinitions.Contains (entity)).Single ();
			var navigator       = controller.Navigator;

			navigator.PreserveNavigation (
				delegate
				{
					//	Cherche l'index de la ligne dans la collection.
					int index = articleDefinition.ArticleParameterDefinitions.IndexOf (entity);

					System.Diagnostics.Debug.Assert (index >= 0);

					//	Ferme la tuile.
					orchestrator.CloseView (controller);

					//	Crée la nouvelle entité.
					AbstractArticleParameterDefinitionEntity newEntity = null;

					if (id == ArticleParameterTabId.Numeric)
					{
						newEntity = dataContext.CreateEmptyEntity<NumericValueArticleParameterDefinitionEntity> ();
					}
					else if (id == ArticleParameterTabId.Enum)
					{
						newEntity = dataContext.CreateEmptyEntity<EnumValueArticleParameterDefinitionEntity> ();
					}

					System.Diagnostics.Debug.Assert (newEntity != null);

					//	Remplace l'entité dans la db.
					articleDefinition.ArticleParameterDefinitions[index] = newEntity;
					dataContext.DeleteEntity (entity);  // supprime dans le DataContext de la ligne
				});
//-			parentController.TileContainerController.ShowSubView (index, "DocumentItem");
		}


		/// <summary>
		/// Cette méthode "magique" est capable de retrouver l'entité parent à partir du container servant
		/// à éditer une entité fille.
		/// </summary>
		/// <param name="container"></param>
		/// <returns></returns>
		public static AbstractEntity GetParentEntity(TileContainer container)
		{
			//	TODO: supprimer ce code
			var parentController = Common.GetParentController (container);

			if (parentController != null)
			{
				return parentController.GetEntity ();
			}

			return null;
		}

		/// <summary>
		/// Cette méthode "magique" est capable de retrouver le contrôleur parent à partir du container servant
		/// à éditer une entité fille.
		/// </summary>
		/// <param name="container"></param>
		/// <returns></returns>
		public static EntityViewController GetParentController(TileContainer container)
		{
			//	TODO: supprimer ce code
			// TODO: Il faudra supprimer cette méthode. Un *ViewController devrait connaître les entités parent !
			var controllers = container.Controller.Orchestrator.DataViewController.GetAllSubControllers ();

			if (controllers.Count () > 1)
			{
				return controllers.ElementAt (1) as EntityViewController;
			}

			return null;
		}
	}
}