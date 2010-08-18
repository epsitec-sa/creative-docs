//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Types;
using Epsitec.Common.Support.EntityEngine;

using Epsitec.Cresus.Core.Entities;
using Epsitec.Cresus.Core.Controllers;
using Epsitec.Cresus.Core.Controllers.DataAccessors;
using Epsitec.Cresus.Core.Widgets;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Controllers.SummaryControllers
{
	public class SummaryArticleDefinitionViewController : SummaryViewController<Entities.ArticleDefinitionEntity>
	{
		public SummaryArticleDefinitionViewController(string name, Entities.ArticleDefinitionEntity entity)
			: base (name, entity)
		{
		}


		protected override void CreateUI()
		{
			using (var data = TileContainerController.Setup (this))
			{
				this.CreateUIArticleDefinition (data);
				this.CreateUIGroup             (data);
				this.CreateUIParameters        (data);
				this.CreateUIComments          (data);
			}
		}
		

		private void CreateUIArticleDefinition(SummaryDataItems data)
		{
			data.Add (
				new SummaryData
				{
					Name				= "ArticleDefinition",
					IconUri				= "Data.ArticleDefinition",
					Title				= TextFormatter.FormatText ("Article"),
					CompactTitle		= TextFormatter.FormatText ("Article"),
					TextAccessor		= Accessor.Create (this.EntityGetter, x => TextFormatter.FormatText ("N°", x.IdA, "\n", x.ShortDescription)),
					CompactTextAccessor = Accessor.Create (this.EntityGetter, x => TextFormatter.FormatText ("N°", x.IdA, "\n", x.LongDescription)),
					EntityMarshaler		= this.EntityMarshaler,
				});
		}

		private void CreateUIGroup(SummaryDataItems data)
		{
			data.Add (
				new SummaryData
				{
					AutoGroup    = true,
					Name		 = "ArticleGroup",
					IconUri		 = "Data.ArticleGroup",
					Title		 = TextFormatter.FormatText ("Groupes d'articles"),
					CompactTitle = TextFormatter.FormatText ("Groupes"),
					Text		 = CollectionTemplate.DefaultEmptyText
				});

			var template = new CollectionTemplate<ArticleGroupEntity> ("ArticleGroup", data.Controller, this.DataContext);

			template.DefineText        (x => TextFormatter.FormatText (x.Name));
			template.DefineCompactText (x => TextFormatter.FormatText (x.Name));

			data.Add (CollectionAccessor.Create (this.EntityGetter, x => x.ArticleGroups, template));
		}

		private void CreateUIParameters(SummaryDataItems data)
		{
			data.Add (
				new SummaryData
				{
					AutoGroup    = true,
					Name		 = "ArticleParameterDefinition",
					IconUri		 = "Data.ArticleParameter",
					Title		 = TextFormatter.FormatText ("Paramètres"),
					CompactTitle = TextFormatter.FormatText ("Paramètres"),
					Text		 = CollectionTemplate.DefaultEmptyText,
				});

			var template = new CollectionTemplate<AbstractArticleParameterDefinitionEntity> ("ArticleParameterDefinition", data.Controller, this.DataContext);

			template.DefineText        (x => TextFormatter.FormatText (GetParameterSummary (x)));
			template.DefineCompactText (x => TextFormatter.FormatText (GetParameterSummary (x)));
			template.DefineCreateItem (this.CreateParameter);  // le bouton [+] crée une ligne d'article

			data.Add (CollectionAccessor.Create (this.EntityGetter, x => x.ArticleParameterDefinitions, template));
		}

		private void CreateUIComments(SummaryDataItems data)
		{
			SummaryControllers.Common.CreateUIComments (this.DataContext, data, this.EntityGetter, x => x.Comments);
		}


		private static string GetParameterSummary(AbstractArticleParameterDefinitionEntity parameter)
		{
			var builder = new System.Text.StringBuilder ();

			if (!string.IsNullOrEmpty (parameter.Name))
			{
				builder.Append (parameter.Name);
				builder.Append (": ");
			}

			if (parameter is NumericValueArticleParameterDefinitionEntity)
			{
				var value = parameter as NumericValueArticleParameterDefinitionEntity;

				if (value.DefaultValue.HasValue ||
					value.MinValue.HasValue     ||
					value.MaxValue.HasValue)
				{
					builder.Append (value.DefaultValue.ToString ());
					builder.Append (" (");
					builder.Append (value.MinValue.ToString ());
					builder.Append ("..");
					builder.Append (value.MaxValue.ToString ());
					builder.Append (")");
				}
				else
				{
					builder.Append ("<i>Vide</i>");
				}
			}

			if (parameter is EnumValueArticleParameterDefinitionEntity)
			{
				var value = parameter as EnumValueArticleParameterDefinitionEntity;

				if (!string.IsNullOrWhiteSpace (value.DefaultValue) ||
					!string.IsNullOrWhiteSpace (value.Values))
				{
					builder.Append (value.DefaultValue);
					builder.Append (" (");
					builder.Append (EditionControllers.Common.EnumInternalToSingleLine (value.Values));
					builder.Append (")");
				}
				else
				{
					builder.Append ("<i>Vide</i>");
				}
			}

			return builder.ToString ();
			;
		}

		private NumericValueArticleParameterDefinitionEntity CreateParameter()
		{
			//	Crée un nouvelle ligne dans la facture du type le plus courant, c'est-à-dire ArticleDocumentItemEntity.
			return this.DataContext.CreateEmptyEntity<NumericValueArticleParameterDefinitionEntity> ();
		}


		protected override EditionStatus GetEditionStatus()
		{
			var entity = this.Entity;
			return entity.IsEmpty () ? EditionStatus.Empty : EditionStatus.Valid;
		}
	}
}
