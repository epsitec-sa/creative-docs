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
		public static FormattedText ArticleDescriptionReplaceTags(ArticleDocumentItemEntity articleDocumentItem, FormattedText description)
		{
			//	Remplace les tags <param code="..."/> par les valeurs réelles de l'article.
			if (description.IsNullOrEmpty)
			{
				return FormattedText.Null;
			}

			string value = description.ToString ();
			var dico = ArticleParameterHelper.GetArticleParametersValues (articleDocumentItem, useNameAsDictionaryKey: true);

			int index = 0;
			while (index < value.Length-ArticleParameterHelper.startParameterTag.Length)
			{
				index = value.IndexOf (ArticleParameterHelper.startParameterTag, index);
				if (index == -1)
				{
					break;
				}

				int nameIndex = index + ArticleParameterHelper.startParameterTag.Length;

				int end = value.IndexOf (ArticleParameterHelper.endParameterTag, nameIndex);
				if (end == -1)  // garde-fou: ne devrait jamais survenir !
				{
					break;
				}

				string name = value.Substring (nameIndex, end - nameIndex);
				string subst = "";

				if (dico.ContainsKey (name))
				{
					var parameter = ArticleParameterHelper.GetParameterFromName (articleDocumentItem, name);

					if (parameter == null)
					{
						subst = dico[name];
					}
					else
					{
						//	Un paramètre est remplacé par sa seule valeur, sans unité si elle est numérique, ou par les descriptions
						//	séparées par des virgules s'il s'agit d'une énumération.
						//	TODO: Il faudra faire mieux un jour...

						string twoLetterISOLanguageName = MultilingualText.GetTwoLetterISOLanguageName (value, index);
						subst = string.Join (", ", ArticleParameterHelper.GetEnumDescriptions (parameter as EnumValueArticleParameterDefinitionEntity, dico[name], twoLetterISOLanguageName));
					}
				}

				value = string.Concat (value.Substring (0, index), subst, value.Substring (end+ArticleParameterHelper.endParameterTag.Length));
				index += subst.Length;
			}

			return value;
		}

		private static IEnumerable<FormattedText> GetEnumDescriptions(EnumValueArticleParameterDefinitionEntity parameter, string values, string twoLetterISOLanguageName)
		{
			//	Si 'parameter' est une énumération, retourne la description la plus complète possible, dans une
			//	langue à choix, pour une série de valeurs données.
			string[] list = (values ?? "").Split (new string[] { AbstractArticleParameterDefinitionEntity.Separator }, System.StringSplitOptions.None);

			foreach (var value in list)
			{
				var description = ArticleParameterHelper.GetEnumDescription (parameter, value);
				yield return TextFormatter.GetMonolingualText (description, twoLetterISOLanguageName);
			}
		}

		private static FormattedText GetEnumDescription(EnumValueArticleParameterDefinitionEntity parameter, string value)
		{
			//	Si 'parameter' est une énumération, retourne la description la plus complète possible, pour une valeur donnée.
			if (parameter != null)
			{
				string[] values = (parameter.Values ?? "").Split (new string[] { AbstractArticleParameterDefinitionEntity.Separator }, System.StringSplitOptions.None);
				FormattedText[] shortDescriptions = FormattedText.Split (parameter.ShortDescriptions, AbstractArticleParameterDefinitionEntity.Separator);
				FormattedText[] longDescriptions  = FormattedText.Split (parameter.LongDescriptions,  AbstractArticleParameterDefinitionEntity.Separator);

				for (int i = 0; i < values.Length; i++)
				{
					if (value == values[i])
					{
						if (i < longDescriptions.Length && !longDescriptions[i].IsNullOrEmpty)
						{
							return longDescriptions[i];
						}

						if (i < shortDescriptions.Length && !shortDescriptions[i].IsNullOrEmpty)
						{
							return shortDescriptions[i];
						}
					}
				}
			}

			return value;
		}

		private static AbstractArticleParameterDefinitionEntity GetParameterFromName(ArticleDocumentItemEntity articleDocumentItem, string name)
		{
			foreach (var parameter in articleDocumentItem.ArticleDefinition.ArticleParameterDefinitions)
			{
				if (parameter.Name == name)
				{
					return parameter;
				}
			}

			return null;
		}


#if false
		public static string GetArticleParameterName(ArticleDocumentItemEntity articleDocumentItem, string code)
		{
			if (articleDocumentItem != null)
			{
				foreach (var parameter in articleDocumentItem.ArticleDefinition.ArticleParameterDefinitions)
				{
					if (parameter.Code == code)
					{
						return parameter.Name.ToString ();
					}
				}
			}

			return code;
		}
#endif

		public static Dictionary<string, string> GetArticleParametersValues(ArticleDocumentItemEntity articleDocumentItem, bool useNameAsDictionaryKey = false)
		{
			//	Retourne le dictionnaire des code/valeur (returnName = false) ou name/valeur (returnName = true) d'un article.
			var dico = new Dictionary<string, string> ();

			if (articleDocumentItem != null)
			{
				var defaultDico = ArticleParameterHelper.GetArticleParametersDefaultValues (articleDocumentItem);
				var localDico   = ArticleParameterHelper.GetArticleParametersLocalValues   (articleDocumentItem);

				foreach (var parameter in articleDocumentItem.ArticleDefinition.ArticleParameterDefinitions)
				{
					string key = parameter.Code;
					string value = null;

					if (!string.IsNullOrEmpty (key))
					{
						if (parameter is OptionValueArticleParameterDefinitionEntity)
						{
							//	La valeur obtenue est le code de l'article. Cela n'a donc pas de sens de la
							//	mettre telle quelle. Une recherche par l'exemple permettrait de trouver
							//	l'article, mais ce n'est pas fait pour l'instant.
						}
						else if (parameter is FreeTextValueArticleParameterDefinitionEntity)
						{
							//	La valeur obtenue est une texte fixe multilingue. Comme il est passé à TextLayout
							//	avec SetParameter, TextLayout ne peut pas extraire la bonne langue. Donc, j'ai
							//	préféré ne rien mettre pour l'instant.
						}
						else
						{
							if (localDico.ContainsKey (key))  // valeur définie localement ?
							{
								value = localDico[key];
							}
							else if (defaultDico.ContainsKey (key))  // valeur par défaut ?
							{
								value = defaultDico[key];
							}
						}

						if (!string.IsNullOrEmpty (value))
						{
							dico.Add (useNameAsDictionaryKey ? parameter.Name.ToString () : parameter.Code, value);
						}
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

			if (parameter is OptionValueArticleParameterDefinitionEntity)
			{
				var optionParameter = parameter as OptionValueArticleParameterDefinitionEntity;
				return null;  // TODO:
			}

			if (parameter is FreeTextValueArticleParameterDefinitionEntity)
			{
				var textParameter = parameter as FreeTextValueArticleParameterDefinitionEntity;
				return textParameter.ShortText.ToString ();
			}

			return null;
		}


		public static readonly string startParameterTag = "<param name=\"";  // <param name="X"/>
		public static readonly string endParameterTag   = "\"/>";

	}
}
