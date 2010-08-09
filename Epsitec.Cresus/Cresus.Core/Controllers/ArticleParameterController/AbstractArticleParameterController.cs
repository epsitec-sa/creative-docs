//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using Epsitec.Common.Drawing;
using Epsitec.Common.Widgets;

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

namespace Epsitec.Cresus.Core.Controllers.ArticleParameterControllers
{
	public class AbstractArticleParameterController
	{
		public AbstractArticleParameterController(ArticleDocumentItemEntity article, int parameterIndex)
		{
			this.article = article;
			this.parameterIndex = parameterIndex;

			this.ImportDictionary ();
		}


		public void CreateUI(FrameBox parent)
		{
			AbstractArticleParameterDefinitionEntity parameter = this.article.ArticleDefinition.ArticleParameters[this.parameterIndex];

			if (parameter is NumericValueArticleParameterDefinitionEntity)
			{
				var field = new TextFieldEx
				{
					Parent = parent,
					Dock = DockStyle.Fill,
					Text = this.ParameterValue,
				};

				field.AcceptingEdition += delegate
				{
					this.ParameterValue = field.Text;
				};
			}

			if (parameter is EnumValueArticleParameterDefinitionEntity)
			{
				var field = new TextFieldEx
				{
					Parent = parent,
					Dock = DockStyle.Fill,
					Text = this.ParameterValue,
				};

				field.AcceptingEdition += delegate
				{
					this.ParameterValue = field.Text;
				};
			}
		}


		private string GetParameterValue()
		{
			return this.ParameterValue;
		}

		private void SetParameterValue(string value)
		{
			this.ParameterValue = value;
		}

		private string ParameterValue
		{
			get
			{
				string value;
				if (this.parameters.TryGetValue (this.Code, out value))
				{
					return value;
				}

				return null;
			}
			set
			{
				this.parameters[this.Code] = value;

				this.ExportDictionary ();
			}
		}

		private void ImportDictionary()
		{
			this.parameters = new Dictionary<string, string> ();

			string[] values = (this.article.ArticleParameters ?? "").Split (new string[] { AbstractArticleParameterDefinitionEntity.Separator }, System.StringSplitOptions.None);
			for (int i = 0; i < values.Length-1; i+=2)
			{
				string key  = values[i+0];
				string data = values[i+1];

				this.parameters.Add (key, data);
			}
		}

		private void ExportDictionary()
		{
			var list = new List<string> ();

			foreach (var pair in this.parameters)
			{
				list.Add (pair.Key);
				list.Add (pair.Value);
			}

			this.article.ArticleParameters = string.Join (AbstractArticleParameterDefinitionEntity.Separator, list);
		}

		private string Code
		{
			get
			{
				return this.article.ArticleDefinition.ArticleParameters[this.parameterIndex].Code;
			}
		}


		private readonly ArticleDocumentItemEntity article;
		private readonly int parameterIndex;

		private Dictionary<string, string> parameters;
	}
}
