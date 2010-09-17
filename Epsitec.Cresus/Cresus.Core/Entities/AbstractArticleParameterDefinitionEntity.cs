//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Types;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Entities
{
	public partial class AbstractArticleParameterDefinitionEntity
	{
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

		public FormattedText GetSummary()
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

	public partial class NumericValueArticleParameterDefinitionEntity
	{
		protected override void AppendSummary(TextBuilder builder)
		{
			base.AppendSummary (builder);
			builder.Append ("~: ");

			if (this.DefaultValue.HasValue ||
				this.MinValue.HasValue     ||
				this.MaxValue.HasValue)
			{
				builder.Append (this.DefaultValue);
				builder.Append (" (");
				builder.Append (this.MinValue);
				builder.Append ("..");
				builder.Append (this.MaxValue);
				builder.Append (")");
			}
			else
			{
				builder.Append (new FormattedText ("<i>Vide</i>"));
			}
		}
	}

	public partial class EnumValueArticleParameterDefinitionEntity
	{
		protected override void AppendSummary(TextBuilder builder)
		{
			base.AppendSummary (builder);
			builder.Append ("~: ");
			
			if (!string.IsNullOrWhiteSpace (this.DefaultValue) &&
				!string.IsNullOrWhiteSpace (this.Values))
			{
				builder.Append (this.DefaultValue);
				builder.Append (" (");
				builder.Append (Epsitec.Cresus.Core.Controllers.EditionControllers.Common.EnumInternalToSingleLine (this.Values));
				builder.Append (")");
			}
			else
			{
				builder.Append ("<i>Vide</i>");
			}
		}
	}
}