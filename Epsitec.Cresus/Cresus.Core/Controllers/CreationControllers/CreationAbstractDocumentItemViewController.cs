//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Types;
using Epsitec.Common.Support.EntityEngine;
using Epsitec.Common.Widgets;

using Epsitec.Cresus.Core.Entities;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Controllers.CreationControllers
{
	public class CreationAbstractDocumentItemViewController : CreationViewController<AbstractDocumentItemEntity>
	{
		public CreationAbstractDocumentItemViewController(string name, Entities.AbstractDocumentItemEntity entity)
			: base (name, entity)
		{
		}

		protected override void CreateUI(Widgets.TileContainer container)
		{
			using (var builder = new UIBuilder (container, this))
			{
				builder.CreatePanelTitleTile ("Data.ArticleDefinition", "Ligne à créer pour la facture...");

				this.CreateUINewTextDocumentButton (builder);
				this.CreateUINewArticleDocumentButton (builder);
				this.CreateUINewPriceDocumentButton (builder);
				
				builder.EndPanelTitleTile ();
			}
		}

		private void CreateUINewTextDocumentButton(UIBuilder builder)
		{
			var button = new ConfirmationButton
			{
				Text = ConfirmationButton.FormatContent ("Texte", "Crée une ligne de texte"),
				PreferredHeight = 52,
			};

			button.Clicked +=
				delegate
				{
					this.CreateRealEntity (
						(context, documentItem) =>
						{
							//?documentItem.FirstContactDate = Date.Today;
							//?documentItem.Person = context.CreateEmptyEntity<TextDocumentItemEntity> ();
						});
				};

			builder.Add (button);
		}

		private void CreateUINewArticleDocumentButton(UIBuilder builder)
		{
			var button = new ConfirmationButton
			{
				Text = ConfirmationButton.FormatContent ("Article", "Crée un article à facturer"),
				PreferredHeight = 52,
			};

			button.Clicked +=
				delegate
				{
					this.CreateRealEntity (
						(context, documentItem) =>
						{
							//?documentItem.FirstContactDate = Date.Today;
							//?documentItem.Person = context.CreateEmptyEntity<ArticleDocumentItemEntity> ();
						});
				};

			builder.Add (button);
		}

		private void CreateUINewPriceDocumentButton(UIBuilder builder)
		{
			var button = new ConfirmationButton
			{
				Text = ConfirmationButton.FormatContent ("Prix", "Crée un rabais ou un prix"),
				PreferredHeight = 52,
			};

			button.Clicked +=
				delegate
				{
					this.CreateRealEntity (
						(context, documentItem) =>
						{
							//?documentItem.FirstContactDate = Date.Today;
							//?documentItem.Person = context.CreateEmptyEntity<PriceDocumentItemEntity> ();
						});
				};

			builder.Add (button);
		}

		
		protected override EditionStatus GetEditionStatus()
		{
#if false
			if (this.Entity.Person.UnwrapNullEntity () == null)
			{
				return EditionStatus.Empty;
			}
			else
			{
				return EditionStatus.Valid;
			}
#else
			return EditionStatus.Valid;
#endif
		}
	}
}
