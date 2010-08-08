//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using Epsitec.Common.Support.EntityEngine;
using Epsitec.Common.Types;
using Epsitec.Common.Types.Converters;

using Epsitec.Cresus.Core;
using Epsitec.Cresus.Core.Entities;
using Epsitec.Cresus.Core.Controllers;
using Epsitec.Cresus.Core.Controllers.DataAccessors;
using Epsitec.Cresus.Core.Widgets;
using Epsitec.Cresus.Core.Widgets.Tiles;
using Epsitec.Cresus.Core.Helpers;

using Epsitec.Cresus.DataLayer.Context;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Controllers.EditionControllers
{
	public class EditionEnumValueArticleParameterDefinitionViewController : EditionViewController<Entities.EnumValueArticleParameterDefinitionEntity>
	{
		public EditionEnumValueArticleParameterDefinitionViewController(string name, Entities.EnumValueArticleParameterDefinitionEntity entity)
			: base (name, entity)
		{
		}

		protected override void CreateUI(TileContainer container)
		{
			this.tileContainer = container;

			using (var builder = new UIBuilder (container, this))
			{
				builder.CreateHeaderEditorTile ();
				builder.CreateEditionTitleTile ("Data.ArticleParameter", "Paramètre");

				this.CreateTabBook (builder);
				this.CreateUIMain (builder);

				builder.CreateFooterEditorTile ();
			}
		}


		private void CreateTabBook(UIBuilder builder)
		{
			var tile = builder.CreateEditionTile ();

			builder.CreateMargin (tile, horizontalSeparator: false);

			List<string> pagesDescription = new List<string> ();
			pagesDescription.Add ("Numeric.Valeur nunérique");
			pagesDescription.Add ("Enum.Enumération");
			this.tabBookContainer = builder.CreateTabBook (tile, pagesDescription, "Enum", this.HandleTabBookAction);
		}

		private void HandleTabBookAction(string tabPageName)
		{
			if (tabPageName == "Enum")
			{
				return;
			}

			Common.ChangeEditedParameterEntity (this.tileContainer, this.DataContext, this.Entity, tabPageName);
		}


		private void CreateUIMain(Epsitec.Cresus.Core.UIBuilder builder)
		{
			var tile = builder.CreateEditionTile ();

			builder.CreateTextField (tile, 80, "Code", Marshaler.Create (() => this.Entity.Code, x => this.Entity.Code = x));
			builder.CreateTextField (tile, 0, "Nom", Marshaler.Create (() => this.Entity.Name, x => this.Entity.Name = x));

			builder.CreateMargin (tile, horizontalSeparator: true);

			builder.CreateAutoCompleteTextField (tile, 0, "Cardinalité", Marshaler.Create (this.Entity, x => x.Cardinality, (x, v) => x.Cardinality = v), BusinessLogic.Enumerations.GetGetAllPossibleItemsValueCardinality (), x => UIBuilder.FormatText (x.Values[0]));
			builder.CreateTextFieldMulti (tile, 94, "Valeurs", Marshaler.Create (() => this.Values, x => this.Values = x));
			builder.CreateTextField (tile, 80, "Valeur par défaut", Marshaler.Create (() => this.Entity.DefaultValue, x => this.Entity.DefaultValue = x));

			builder.CreateMargin (tile, horizontalSeparator: true);

			builder.CreateTextFieldMulti (tile, 94, "Descriptions courtes", Marshaler.Create (() => this.ShortDescriptions, x => this.ShortDescriptions = x));
			builder.CreateTextFieldMulti (tile, 94, "Descriptions longues", Marshaler.Create (() => this.LongDescriptions, x => this.LongDescriptions = x));
		}


		private string Values
		{
			get
			{
				return EditionEnumValueArticleParameterDefinitionViewController.EnumInternalToMultiLine (this.Entity.Values);
			}
			set
			{
				this.Entity.Values = EditionEnumValueArticleParameterDefinitionViewController.EnumMultiLineToInternal (value);
			}
		}

		private string ShortDescriptions
		{
			get
			{
				return EditionEnumValueArticleParameterDefinitionViewController.EnumInternalToMultiLine (this.Entity.ShortDescriptions);
			}
			set
			{
				this.Entity.ShortDescriptions = EditionEnumValueArticleParameterDefinitionViewController.EnumMultiLineToInternal (value);
			}
		}

		private string LongDescriptions
		{
			get
			{
				return EditionEnumValueArticleParameterDefinitionViewController.EnumInternalToMultiLine (this.Entity.LongDescriptions);
			}
			set
			{
				this.Entity.LongDescriptions = EditionEnumValueArticleParameterDefinitionViewController.EnumMultiLineToInternal (value);
			}
		}


		public static string EnumInternalToMultiLine(string value)
		{
			var builder = new System.Text.StringBuilder ();

			string[] values = null;
			int count = 0;
			int max = 9;

			if (!string.IsNullOrEmpty(value))
			{
				values = value.Split (new string[] { AbstractArticleParameterDefinitionEntity.Separator }, System.StringSplitOptions.None);
				count = values.Length;
				max = System.Math.Max (9, values.Length);
			}

			for (int i = 0; i < max; i++)
			{
				builder.Append ((i+1).ToString ());
				builder.Append (": ");

				if (i < count)
				{
					builder.Append (values[i]);
				}

				builder.Append ("<br/>");
			}

			return builder.ToString ();
		}

		public static string EnumMultiLineToInternal(string value)
		{
			List<string> list = new List<string> ();
			string[] values = value.Split (new string[] { "<br/>" }, System.StringSplitOptions.None);

			for (int i = 0; i < values.Length; i++)
			{
				string text = values[i];

				int index = text.IndexOf (":");
				if (index != -1)
				{
					text = text.Substring (index+1);
				}

				text = text.Trim ();

				if (!string.IsNullOrEmpty (text))
				{
					list.Add (text);
				}
			}

			return string.Join (AbstractArticleParameterDefinitionEntity.Separator, list);
		}



		protected override EditionStatus GetEditionStatus()
		{
			return EditionStatus.Valid;
		}


		private TileContainer							tileContainer;
		private Epsitec.Common.Widgets.FrameBox			tabBookContainer;
	}
}
