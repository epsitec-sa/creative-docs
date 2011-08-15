using System.Linq;
using System.Linq.Expressions;
using Epsitec.Common.Support.EntityEngine;
using Epsitec.Common.Types;
using Epsitec.Cresus.Bricks;
using Epsitec.Cresus.Core.Bricks;
using Epsitec.Cresus.Core.Controllers;
using Epsitec.Cresus.Core.Controllers.DataAccessors;

namespace Epsitec.Cresus.Core.Server
{
	class BrickProcessor
	{
		internal static Brick ProcessBrick(Brick brick, WebDataItem item)
		{
			Brick oldBrick;

			do
			{

				if (Brick.ContainsProperty (brick, BrickPropertyKey.OfType))
				{

				}
				else if (Brick.ContainsProperty (brick, BrickPropertyKey.Template))
				{
					//	Don't produce default text properties for bricks which contain AsType
					//	or Template bricks. Instead, specify the default empty text.

					var templateBrick = Brick.GetProperty (brick, BrickPropertyKey.Template).Brick;

					System.Diagnostics.Debug.Assert (templateBrick != null);

					if ((!Brick.ContainsProperty (templateBrick, BrickPropertyKey.Title)) &&
					(!Brick.ContainsProperty (templateBrick, BrickPropertyKey.TitleCompact)))
					{
						BrickProcessor.CreateDefaultTitleProperties (templateBrick);
					}

					if ((!Brick.ContainsProperty (templateBrick, BrickPropertyKey.Text)) &&
					(!Brick.ContainsProperty (templateBrick, BrickPropertyKey.TextCompact)))
					{
						BrickProcessor.CreateDefaultTextProperties (templateBrick);
					}

					if (!Brick.ContainsProperty (brick, BrickPropertyKey.Text))
					{
						Brick.AddProperty (brick, new BrickProperty (BrickPropertyKey.Text, CollectionTemplate.DefaultEmptyText));
					}
				}
				else
				{
					BrickProcessor.CreateDefaultTextProperties (brick);
				}

				BrickProcessor.ProcessProperty (brick, BrickPropertyKey.Name, x => item.Name = x);
				BrickProcessor.ProcessProperty (brick, BrickPropertyKey.Icon, x => item.IconUri = x);

				BrickProcessor.ProcessProperty (brick, BrickPropertyKey.Title, x => item.Title = x);
				BrickProcessor.ProcessProperty (brick, BrickPropertyKey.TitleCompact, x => item.CompactTitle = x);
				BrickProcessor.ProcessProperty (brick, BrickPropertyKey.Text, x => item.Text = x);
				BrickProcessor.ProcessProperty (brick, BrickPropertyKey.TextCompact, x => item.CompactText = x);

				BrickProcessor.ProcessProperty (brick, BrickPropertyKey.Attribute, x => BrickProcessor.ProcessAttribute (item, x));

				if ((!item.Title.IsNullOrEmpty) && (item.CompactTitle.IsNull))
				{
					item.CompactTitle = item.Title;
				}

				BrickProcessor.ProcessProperty (brick, BrickPropertyKey.Title, x => item.TitleAccessor = x);
				BrickProcessor.ProcessProperty (brick, BrickPropertyKey.TitleCompact, x => item.CompactTitleAccessor = x);
				BrickProcessor.ProcessProperty (brick, BrickPropertyKey.Text, x => item.TextAccessor = x);
				BrickProcessor.ProcessProperty (brick, BrickPropertyKey.TextCompact, x => item.CompactTextAccessor = x);

				AddSpecificData (brick, item);

				oldBrick = brick;
				brick = Brick.GetProperty (brick, BrickPropertyKey.OfType).Brick;
			} while (brick != null);

			return oldBrick;
		}

		private static void AddSpecificData(Brick brick, WebDataItem item)
		{
			if (Brick.ContainsProperty (brick, BrickPropertyKey.CollectionAnnotation))
			{
				item.DataType = TileDataType.CollectionItem;
			}
		}

		private static void CreateDefaultTextProperties(Brick brick)
		{
			if (!Brick.ContainsProperty (brick, BrickPropertyKey.Text))
			{
				Expression<System.Func<AbstractEntity, FormattedText>> expression = x => x.GetSummary ();
				Brick.AddProperty (brick, new BrickProperty (BrickPropertyKey.Text, expression));
			}

			if (!Brick.ContainsProperty (brick, BrickPropertyKey.TextCompact))
			{
				Expression<System.Func<AbstractEntity, FormattedText>> expression = x => x.GetCompactSummary ();
				Brick.AddProperty (brick, new BrickProperty (BrickPropertyKey.TextCompact, expression));
			}
		}

		protected static void CreateDefaultTitleProperties(Brick brick)
		{
			if (Brick.ContainsProperty (brick, BrickPropertyKey.Title))
			{
				return;
			}

			var fieldType = brick.GetFieldType ();
			var methodInfo = fieldType.GetMethod ("GetTitle");

			Expression<System.Func<AbstractEntity, FormattedText>> expression = x => (FormattedText) methodInfo.Invoke (x, new object[0]);
			Brick.AddProperty (brick, new BrickProperty (BrickPropertyKey.Title, expression));
		}

        private static void ProcessProperty(Brick brick, BrickPropertyKey key, System.Action<bool> setter)
		{
			if (Brick.ContainsProperty (brick, key))
			{
				setter (true);
			}
		}

		private static void ProcessProperty(Brick brick, BrickPropertyKey key, System.Action<string> setter)
		{
			var value = Brick.GetProperty (brick, key).StringValue;

			if (value != null)
			{
				setter (value);
			}
		}

		private static void ProcessProperty(Brick brick, BrickPropertyKey key, System.Action<BrickMode> setter)
		{
			foreach (var attributeValue in Brick.GetProperties (brick, key).Select (x => x.AttributeValue))
			{
				if ((attributeValue != null) &&
					(attributeValue.ContainsValue<BrickMode> ()))
				{
					setter (attributeValue.GetValue<BrickMode> ());
				}
			}
		}

		private static void ProcessProperty(Brick brick, BrickPropertyKey key, System.Action<Accessor<FormattedText>> setter)
		{
			//var formatter = this.ToAccessor (brick, Brick.GetProperty (brick, key));
			Accessor<FormattedText> formatter = null;

			if (formatter != null)
			{
				setter (formatter);
			}
		}

		private static void ProcessProperty(Brick brick, BrickPropertyKey key, System.Action<Expression> setter)
		{
			var value = Brick.GetProperty (brick, key).ExpressionValue;

			if (value != null)
			{
				setter (value);
			}
		}

		private static void ProcessAttribute(WebDataItem item, BrickMode value)
		{
			switch (value)
			{
				case BrickMode.AutoGroup:
					item.AutoGroup = true;
					break;

				case BrickMode.DefaultToSummarySubview:
					item.DefaultMode = ViewControllerMode.Summary;
					break;

				case BrickMode.HideAddButton:
					item.HideAddButton = true;
					break;

				case BrickMode.FullHeightStretch:
					item.FullHeightStretch = true;
					break;

				case BrickMode.HideRemoveButton:
					item.HideRemoveButton = true;
					break;
			}
		}
	}
}
