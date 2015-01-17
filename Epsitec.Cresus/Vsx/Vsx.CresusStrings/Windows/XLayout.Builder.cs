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
		public class Builder
		{
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
				var markup = string.Format ("<root{0}>{1}</root>",
					string.Join (string.Empty, Builder.namespacePrefixMap.Select (kv => string.Format (" xmlns:{0}=\"{1}\"", kv.Value, kv.Key))),
					escapedMarkup);

				return XElement.Parse (markup);
			}

			public Builder(InlineCollection inlines, bool indentAttributes = false)
			{
				this.indentAttributes = indentAttributes;
				this.inlines = inlines;
			}

			public bool IndentAttributes
			{
				get
				{
					return this.indentAttributes;
				}
			}

			public Builder Append(string escapedMarkup)
			{
				return this.Append (Builder.ToXNodesSafe (escapedMarkup.Trim ()), 0);
			}

			#region Helpers

			private static IEnumerable<XNode> ToXNodesSafe(string escapedMarkup)
			{
				try
				{
					return Builder.UnescapeHtmlEntities(escapedMarkup).Nodes ();
				}
				catch
				{
					return new XText (escapedMarkup).AsSequence ();
				}
			}

			private static string GetShortNamespacePrefix(XName name)
			{
				return name.ToShortPrefixName (Builder.namespacePrefixMap);
			}

			private static Inline EndOfLine()
			{
				return new InlineUIContainer (
					new Border ()
					{
						CornerRadius = new CornerRadius (3),
						Padding = new Thickness (3, 0, 3, 0),
						Background = Brushes.Gray,
						Child = new TextBlock (
							new Run (Builder.EndOfLineSymbol)
							{
								FontFamily = Builder.Wingdings3,
								Foreground = Brushes.White,
							}
						)
					});
			}

			private static UIElement DecorateAttributeValue(TextBlock value, bool multiNodes)
			{
				if (multiNodes)
				{
					return new Border ()
					{
						Padding = new Thickness (3, 0, 3, 0),
						Child = value
					};
				}
				else
				{
					foreach (var inline in value.Inlines)
					{
						inline.Foreground = Brushes.Blue;
					}
					return value;
				}
			}


			private Builder Append(IEnumerable<XNode> nodes, int indent)
			{
				foreach (var node in nodes)
				{
					if (node is XText)
					{
						var text = node as XText;
						this.Append (text, indent);
					}
					else if (node is XElement)
					{
						var element = node as XElement;
						var name = element.Name.LocalName;

						Action<Builder, XElement, int> factory;
						if (Builder.factories.TryGetValue (name, out factory))
						{
							factory (this, element, indent);
						}
						else
						{
							this.NewLine (indent);
							this.Append (element, indent);
							
							if (node.NextNode != null)
							{
								this.inlines.GotoStartOfLine ();
							}
						}
					}
				}
				return this;
			}

			private Builder Append(XText text, int indent)
			{
				var value = text.Value;
				if (this.inlines.AtStartOfLine ())
				{
					value = value.TrimStart ();
				}
				if (text.NextNode == null)
				{
					value = value.TrimEnd ();
				}
				this.inlines.Add (new Run (value));
				return this;
			}

			private Builder Append(XElement element, int indent)
			{
				// <tag
				this.inlines.Add (new Run ("<" + Builder.GetShortNamespacePrefix (element.Name))
				{
					Foreground = Brushes.Brown,
				});

				// attr1="value1" attr2="value2" ...
				this.Append (element.Attributes (), indent + 1);

				var nodes = element.Nodes ();
				if (nodes.Any ())
				{
					// >
					this.inlines.Add (new Run (">")
					{
						Foreground = Brushes.Brown,
					});

					// start indented scope
					this.NewLine (indent + 1);

					var textBlock = new TextBlock();
					new Builder(textBlock.Inlines, this.indentAttributes).Append (nodes, 0);
					this.inlines.Add (new InlineUIContainer(textBlock));

					// end indented scope
					this.NewLine (indent);

					// </tag>
					this.inlines.Add (new Run ("</" + Builder.GetShortNamespacePrefix (element.Name) + ">")
					{
						Foreground = Brushes.Brown,
					});
				}
				else
				{
					// />
					this.inlines.Add (new Run ("/>")
					{
						Foreground = Brushes.Brown,
					});
				}
				return this;
			}

			private Builder Append(IEnumerable<XAttribute> attributes, int indent)
			{
				foreach (var attribute in attributes)
				{
					if (this.IndentAttributes)
					{
						this.NewLine (indent);
					}
					this.Append (attribute, indent);
				}
				return this;
			}

			private Builder Append(XAttribute attribute, int indent)
			{
				var xnodes = Builder.ToXNodesSafe (attribute.Value.Trim ());

				var textBlock = new TextBlock ();
				new Builder (textBlock.Inlines, this.indentAttributes).Append (xnodes, 0);

				this.DecorateAttribute (attribute, textBlock, xnodes.Count () > 1, indent);

				return this;
			}

			private Builder Add(Inline inline, int indent)
			{
				var previous = inline.PreviousInline;
				if (previous != null && previous is LineBreak && indent > 0)
				{
					this.inlines.Add (new Run (new string ('\t', indent)));
				}
				this.inlines.Add (inline);
				return this;
			}

			private void DecorateAttribute(XAttribute attribute, TextBlock value, bool multiNodes, int indent)
			{
				this.inlines.Add (new Run (this.inlines.GetLeftMargin (indent, " ") + Builder.GetShortNamespacePrefix (attribute.Name))
				{
					Foreground = Brushes.Red
				});
				this.inlines.Add (new Run ("=\"")
				{
					Foreground = Brushes.Blue
				});

				this.inlines.Add (new InlineUIContainer (Builder.DecorateAttributeValue (value, multiNodes))
				{
					BaselineAlignment = BaselineAlignment.Top
				});

				this.inlines.Add (new Run ("\"")
				{
					Foreground = Brushes.Blue
				});
			}

			private void NewLine(int indent)
			{
				this.inlines.GotoStartOfLine ();
				if (indent > 0)
				{
					this.inlines.Add (new Run (new string (' ', 4 * indent)));
				}
			}

			private void ProcessMargin(int indent)
			{
				if (indent > 0 && this.inlines.AtStartOfLine ())
				{
					this.inlines.Add (new Run (new string (' ', 4 * indent)));
				}
			}

			#endregion

			#region Constants

			private static readonly string EndOfLineSymbol = '\u00C9'.ToString ();
			private static readonly FontFamily  Wingdings3 = new FontFamily ("Wingdings 3");

			private static readonly Dictionary<string, string> namespacePrefixMap = new Dictionary<string, string> ()
			{
				{"http://epsitec/cresus/s", "s"},
				{"http://epsitec/cresus/f", "f"},
			};

			private static readonly Dictionary<string, Action<Builder, XElement, int>> factories = new Dictionary<string, Action<Builder, XElement, int>> ()
			{
				{
					"a",
					(builder, element, indent) =>
					{
						var hyperlink = new Hyperlink ();
						hyperlink.NavigateUri = new Uri (element.Attribute ("href").Value);
						hyperlink.RequestNavigate += (sender, e) =>
						{
							Process.Start (new ProcessStartInfo (e.Uri.AbsoluteUri));
							e.Handled = true;
						};
						new Builder (hyperlink.Inlines, builder.indentAttributes).Append (element.Nodes (), 0);
						builder.inlines.Add (hyperlink);
					}
				},
				{
					"b",
					(builder, element, indent) =>
					{
						var span = new Bold ();
						new Builder (span.Inlines, builder.indentAttributes).Append (element.Nodes (), indent);
						builder.inlines.Add (span);
					}
				},
				{
					"br",
					(builder, element, indent) =>
					{
						if (builder.inlines.AtStartOfLine())
						{
							builder.inlines.Add (Builder.EndOfLine ());
						}
						builder.inlines.Add (new LineBreak ());
					}
				},
				{
					"i",
					(builder, element, indent) =>
					{
						var span = new Italic ();
						new Builder (span.Inlines, builder.indentAttributes).Append (element.Nodes (), indent);
						builder.inlines.Add (span);
					}
				},
				{
					"u",
					(builder, element, indent) =>
					{
						var span = new Underline ();
						new Builder (span.Inlines, builder.indentAttributes).Append (element.Nodes (), indent);
						builder.inlines.Add (span);
					}
				},
			};
			#endregion


			private readonly InlineCollection inlines;
			private readonly bool indentAttributes;
		}
	}
}
