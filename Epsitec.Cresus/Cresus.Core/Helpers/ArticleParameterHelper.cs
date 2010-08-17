//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using Epsitec.Common.Support;
using Epsitec.Common.Support.EntityEngine;
using Epsitec.Common.Types;
using Epsitec.Common.Drawing;
using Epsitec.Common.Widgets;

using Epsitec.Cresus.Core.Entities;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Helpers
{
	public static class ArticleParameterHelper
	{
		public static string ArticleDescriptionReplaceTags(ArticleDocumentItemEntity articleDocumentItem, string description)
		{
			//	Remplace les tags <param code="..."/> par les valeurs réelles de l'article.
			if (string.IsNullOrEmpty (description))
			{
				return null;
			}

			var dico = ArticleParameterHelper.GetArticleParametersValues (articleDocumentItem);

			int index = 0;
			while (index < description.Length-ArticleParameterHelper.startParameterTag.Length)
			{
				index = description.IndexOf (ArticleParameterHelper.startParameterTag, index);
				if (index == -1)
				{
					break;
				}

				int codeIndex = index + ArticleParameterHelper.startParameterTag.Length;

				int end = description.IndexOf (ArticleParameterHelper.endParameterTag, codeIndex);
				if (end == -1)  // garde-fou: ne devrait jamais survenir !
				{
					break;
				}

				string code = description.Substring (codeIndex, end - codeIndex);
				string subst = "";

				if (dico.ContainsKey (code))
				{
					var parameter = ArticleParameterHelper.GetParameter (articleDocumentItem, code);

					if (parameter == null)
					{
						subst = dico[code];
					}
					else
					{
						subst = string.Join (", ", ArticleParameterHelper.GetEnumDescriptions (parameter as EnumValueArticleParameterDefinitionEntity, dico[code]));
					}
				}

				description = string.Concat (description.Substring (0, index), subst, description.Substring (end+ArticleParameterHelper.endParameterTag.Length));
				index += subst.Length;
			}

			return description;
		}

		private static IEnumerable<string> GetEnumDescriptions(EnumValueArticleParameterDefinitionEntity parameter, string values)
		{
			//	Si 'parameter' est une énumération, retourne la description la plus complète possible, pour une série
			//	de valeurs données.
			string[] list = (values ?? "").Split (new string[] { AbstractArticleParameterDefinitionEntity.Separator }, System.StringSplitOptions.None);

			foreach (var value in list)
			{
				yield return ArticleParameterHelper.GetEnumDescription (parameter, value);
			}
		}

		private static string GetEnumDescription(EnumValueArticleParameterDefinitionEntity parameter, string value)
		{
			//	Si 'parameter' est une énumération, retourne la description la plus complète possible, pour une valeur donnée.
			if (parameter != null)
			{
				string[] values            = (parameter.Values            ?? "").Split (new string[] { AbstractArticleParameterDefinitionEntity.Separator }, System.StringSplitOptions.None);
				string[] shortDescriptions = (parameter.ShortDescriptions ?? "").Split (new string[] { AbstractArticleParameterDefinitionEntity.Separator }, System.StringSplitOptions.None);
				string[] longDescriptions  = (parameter.LongDescriptions  ?? "").Split (new string[] { AbstractArticleParameterDefinitionEntity.Separator }, System.StringSplitOptions.None);

				for (int i = 0; i < values.Length; i++)
				{
					if (value == values[i])
					{
						if (i < longDescriptions.Length && !string.IsNullOrEmpty (longDescriptions[i]))
						{
							return longDescriptions[i];
						}

						if (i < shortDescriptions.Length && !string.IsNullOrEmpty (shortDescriptions[i]))
						{
							return shortDescriptions[i];
						}
					}
				}
			}

			return value;
		}

		private static AbstractArticleParameterDefinitionEntity GetParameter(ArticleDocumentItemEntity articleDocumentItem, string code)
		{
			foreach (var parameter in articleDocumentItem.ArticleDefinition.ArticleParameterDefinitions)
			{
				if (parameter.Code == code)
				{
					return parameter;
				}
			}

			return null;
		}


		public static Dictionary<string, string> GetArticleParametersValues(ArticleDocumentItemEntity articleDocumentItem)
		{
			//	Retourne le dictionnaire des code/valeur d'un article.
			var dico = new Dictionary<string, string> ();

			if (articleDocumentItem != null)
			{
				var defaultDico = ArticleParameterHelper.GetArticleParametersDefaultValues (articleDocumentItem);
				var localDico   = ArticleParameterHelper.GetArticleParametersLocalValues   (articleDocumentItem);

				foreach (var parameter in articleDocumentItem.ArticleDefinition.ArticleParameterDefinitions)
				{
					string key = parameter.Code;
					string value = null;

					if (localDico.ContainsKey (key))  // valeur définie localement ?
					{
						value = localDico[key];
					}
					else if (defaultDico.ContainsKey (key))  // valeur par défaut ?
					{
						value = defaultDico[key];
					}

					if (!string.IsNullOrEmpty (value))
					{
						dico.Add (key, value);
					}
				}
			}

			return dico;
		}

		private static Dictionary<string, string> GetArticleParametersDefaultValues(ArticleDocumentItemEntity articleDocumentItem)
		{
			//	Retourne le dictionnaire des code/valeur par défaut d'un article.
			var dico = new Dictionary<string, string> ();

			if (articleDocumentItem != null)
			{
				foreach (var parameter in articleDocumentItem.ArticleDefinition.ArticleParameterDefinitions)
				{
					string value = ArticleParameterHelper.GetParameterDefaultValue (parameter);

					if (!string.IsNullOrEmpty (value))
					{
						dico.Add (parameter.Code, value);
					}
				}
			}

			return dico;
		}

		private static Dictionary<string, string> GetArticleParametersLocalValues(ArticleDocumentItemEntity articleDocumentItem)
		{
			//	Retourne le dictionnaire des code/valeur spécifiques d'un article.
			var dico = new Dictionary<string, string> ();

			if (articleDocumentItem != null)
			{
				string[] values = (articleDocumentItem.ArticleParameters ?? "").Split (new string[] { Controllers.ArticleParameterControllers.AbstractArticleParameterController.Separator }, System.StringSplitOptions.None);

				for (int i = 0; i < values.Length-1; i+=2)
				{
					string key  = values[i+0];
					string data = values[i+1];

					dico.Add (key, data);
				}
			}

			return dico;
		}

		private static string GetParameterDefaultValue(AbstractArticleParameterDefinitionEntity parameter)
		{
			if (parameter is NumericValueArticleParameterDefinitionEntity)
			{
				var numericParameter = parameter as NumericValueArticleParameterDefinitionEntity;
				return numericParameter.DefaultValue.ToString ();
			}

			if (parameter is EnumValueArticleParameterDefinitionEntity)
			{
				var enumParameter = parameter as EnumValueArticleParameterDefinitionEntity;
				return enumParameter.DefaultValue;
			}

			return null;
		}


		public static readonly string startParameterTag = "<param code=\"";  // <param code="X"/>
		public static readonly string endParameterTag   = "\"/>";

	}
}
