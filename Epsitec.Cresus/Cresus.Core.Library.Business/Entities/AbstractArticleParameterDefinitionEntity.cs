//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Types;

using Epsitec.Cresus.Core.Controllers.TabIds;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Entities
{
	public partial class AbstractArticleParameterDefinitionEntity
	{
		public virtual ArticleParameterTabId TabId
		{
			get
			{
				return ArticleParameterTabId.None;
			}
		}


		/// <summary>
		/// Some article parameter definitions store several values in a single
		/// text field; the values are separated by the special '∙' character.
		/// </summary>
		public const char				SeparatorChar	= (char) 0x2219;					// '∙'
		public static readonly string	Separator		= SeparatorChar.ToString ();		// "∙"

		/// <summary>
		/// Splits the specified text using the specific '∙' separator.
		/// </summary>
		/// <param name="text">The text.</param>
		/// <returns>The values.</returns>
		public static string[] Split(string text)
		{
			if (string.IsNullOrEmpty (text))
			{
				return new string[0];
			}
			else
			{
				return text.Split (AbstractArticleParameterDefinitionEntity.SeparatorChar);
			}
		}

		/// <summary>
		/// Joins the specified values using the specific '∙' separator.
		/// </summary>
		/// <param name="values">The values.</param>
		/// <returns>The text.</returns>
		public static string Join(params string[] values)
		{
			return string.Join (AbstractArticleParameterDefinitionEntity.Separator, values);
		}

		public override FormattedText GetSummary()
		{
			var builder = new TextBuilder ();
			
			this.AppendSummary (builder);

			return builder.ToFormattedText ();
		}

		protected virtual void AppendSummary(TextBuilder builder)
		{
			if (!this.Name.IsNullOrEmpty)
			{
				builder.Append (this.Name);
			}
		}
	}
}