using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Navigation;
using System.Xml.Linq;

namespace Epsitec.Windows
{
	public partial class XLayout
	{
		public static readonly DependencyProperty TextProperty =
			DependencyProperty.RegisterAttached ("Text", typeof (string), typeof (XLayout), new PropertyMetadata (string.Empty, XLayout.OnTextChanged));

		public static string GetText(DependencyObject obj)
		{
			return (string) obj.GetValue (TextProperty);
		}

		public static void SetText(DependencyObject obj, string value)
		{
			obj.SetValue (TextProperty, value);
		}


		private static void OnTextChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			var inlines = XLayout.GetInlines(d);
			if (inlines != null)
			{
				new XLayout.Builder (inlines).Append (e.NewValue as string);
			}
		}

		/// <summary>
		/// Convert HTML character entities to reserved characters <see cref="http://www.w3schools.com/HTML/html_entities.asp"/>
		/// </summary>
		/// <param name="escapedMarkup">The HTML markup encoded with character entities</param>
		/// <returns>A new markup with character entities replaced with special characters</returns>
		/// <example>
		/// <![CDATA[
		///		Builder.DecodeHtmlEntities("&lt;data /&gt;") == "<data/>";	// is true
		///	]]>
		///	</example>
		public static XElement UnescapeHtmlEntities(string escapedMarkup)
		{
			return Builder.UnescapeHtmlEntities (escapedMarkup);
		}

		private static InlineCollection GetInlines(DependencyObject d)
		{
			if (d is TextBlock)
			{
				return (d as TextBlock).Inlines;
			}
			else if (d is Paragraph)
			{
				return (d as Paragraph).Inlines;
			}
			else if (d is Span)
			{
				return (d as Span).Inlines;
			}
			return null;
		}
	}
}
