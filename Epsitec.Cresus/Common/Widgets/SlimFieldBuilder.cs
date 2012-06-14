//	Copyright � 2012, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support.Extensions;
using Epsitec.Common.Types;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Common.Widgets
{
	public static class SlimFieldBuilder
	{
		public static SlimField CreateSlimField(Caption caption)
		{
			var slimField = new SlimField ()
			{
			};

			return slimField;
		}

		public static void SetFieldTexts(SlimField slimField, Caption caption)
		{
			var displayMode = slimField.DisplayMode;

			slimField.FieldLabel = SlimFieldBuilder.GetLabelText (caption);
			
			switch (displayMode)
			{
				case SlimFieldDisplayMode.Label:
					break;

				case SlimFieldDisplayMode.TextEdition:
					SlimFieldBuilder.SetFieldTexts (slimField, SlimFieldBuilder.GetEditText (caption));
					break;

				case SlimFieldDisplayMode.Text:
				case SlimFieldDisplayMode.Menu:
					SlimFieldBuilder.SetFieldTexts (slimField, SlimFieldBuilder.GetValueText (caption));
					break;

				default:
					throw displayMode.NotSupportedException ();
			}
		}

		private static void SetFieldTexts(SlimField slimField, string captionText)
		{
			if (string.IsNullOrEmpty (captionText))
			{
				slimField.FieldPrefix = null;
				slimField.FieldSuffix = null;
			}
			else
			{
				if (captionText[0] == '[')
				{
					int end = captionText.IndexOf (']', 1);

					if (end > 0)
					{
						while (++end < captionText.Length)
						{
							if (captionText[end] != ' ')
							{
								break;
							}
						}

						captionText = captionText.Substring (end);
					}
				}

				captionText = FormattedText.Unescape (captionText);

				int pos = captionText.IndexOf ('*');

				if (pos < 0)
				{
					pos = captionText.Length;
				}

				slimField.FieldPrefix = captionText.Substring (0, pos++);
				slimField.FieldSuffix = pos < captionText.Length ? captionText.Substring (pos) : null;
			}
		}


		private static string GetLabelText(Caption caption)
		{
			return caption.Labels.FirstOrDefault (x => x.StartsWith (String.Label))
				?? caption.Labels.FirstOrDefault (x => x.Length > 0 && x[0] != '[' && !x.Contains ('*'));
		}

		private static string GetValueText(Caption caption)
		{
			return caption.Labels.FirstOrDefault (x => x.StartsWith (String.Value))
				?? caption.Labels.FirstOrDefault (x => x.Length > 0 && x[0] != '[' && x.Contains ('*'));
		}

		private static string GetEditText(Caption caption)
		{
			return caption.Labels.FirstOrDefault (x => x.StartsWith (String.Edit));
		}


		public static class String
		{
			public const string					Label			= "[label]";
			public const string					Value			= "[value]";
			public const string					Edit			= "[edit]";
			public const string					ValueSingular	= "[value-1]";
			public const string					ValuePlural		= "[value-2]";
		}
	}
}
