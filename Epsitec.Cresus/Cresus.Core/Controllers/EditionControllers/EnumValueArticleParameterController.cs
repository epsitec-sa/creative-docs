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

namespace Epsitec.Cresus.Core.Controllers.EditionControllers
{
	public class EnumValueArticleParameterController
	{
		public EnumValueArticleParameterController(TileContainer tileContainer, EnumValueArticleParameterDefinitionEntity parameterEntity)
		{
			this.tileContainer = tileContainer;
			this.parameterEntity = parameterEntity;

			this.enumValues = new List<EnumValue> ();
		}


		public string SelectedValue
		{
			get
			{
				if (this.SelectedIndex == -1)
				{
					return null;
				}
				else
				{
					return this.enumValues[this.SelectedIndex].Value;
				}
			}
			set
			{
				if (this.SelectedIndex != -1 && this.enumValues[this.SelectedIndex].Value != value)
				{
					this.enumValues[this.SelectedIndex].Value = value;
					this.StoreEnumValuesList ();
					this.UpdateScrollList ();
				}
			}
		}

		public string SelectedShortDescription
		{
			get
			{
				if (this.SelectedIndex == -1)
				{
					return null;
				}
				else
				{
					return this.enumValues[this.SelectedIndex].ShortDescription;
				}
			}
			set
			{
				if (this.SelectedIndex != -1 && this.enumValues[this.SelectedIndex].ShortDescription != value)
				{
					this.enumValues[this.SelectedIndex].ShortDescription = value;
					this.StoreEnumValuesList ();
					this.UpdateScrollList ();
				}
			}
		}

		public string SelectedLongDescription
		{
			get
			{
				if (this.SelectedIndex == -1)
				{
					return null;
				}
				else
				{
					return this.enumValues[this.SelectedIndex].LongDescription;
				}
			}
			set
			{
				if (this.SelectedIndex != -1 && this.enumValues[this.SelectedIndex].LongDescription != value)
				{
					this.enumValues[this.SelectedIndex].LongDescription = value;
					this.StoreEnumValuesList ();
					this.UpdateScrollList ();
				}
			}
		}


		public void CreateUI(FrameBox parent)
		{
			int buttonSize = 23;

			var toolbar = new FrameBox
			{
				Parent = parent,
				PreferredHeight = buttonSize,
				Dock = DockStyle.Top,
				Margins = new Margins (0, 10, 2, 2),
			};

			this.createButton = new GlyphButton
			{
				Parent = toolbar,
				PreferredSize = new Size (buttonSize*2, buttonSize),
				GlyphShape = GlyphShape.Plus,
				Margins = new Margins (0, 0, 0, 0),
				Dock = DockStyle.Left,
			};

			this.deleteButton = new GlyphButton
			{
				Parent = toolbar,
				PreferredSize = new Size (buttonSize, buttonSize),
				GlyphShape = GlyphShape.Minus,
				Margins = new Margins (1, 0, 0, 0),
				Dock = DockStyle.Left,
			};

			this.upButton = new GlyphButton
			{
				Parent = toolbar,
				PreferredSize = new Size (buttonSize, buttonSize),
				GlyphShape = GlyphShape.ArrowUp,
				Margins = new Margins (10, 0, 0, 0),
				Dock = DockStyle.Left,
			};

			this.downButton = new GlyphButton
			{
				Parent = toolbar,
				PreferredSize = new Size (buttonSize, buttonSize),
				GlyphShape = GlyphShape.ArrowDown,
				Margins = new Margins (1, 0, 0, 0),
				Dock = DockStyle.Left,
			};

			this.labelCount = new StaticText
			{
				Parent = toolbar,
				ContentAlignment = ContentAlignment.MiddleRight,
				Dock = DockStyle.Fill,
			};

			this.scrollList = new ScrollList
			{
				Parent = parent,
				PreferredHeight = 124,
				Dock = DockStyle.Top,
				Margins = new Margins (0, 10, 0, 0),
			};

			this.createButton.Clicked += delegate
			{
				this.ActionCreate ();
			};

			this.deleteButton.Clicked += delegate
			{
				this.ActionDelete ();
			};

			this.upButton.Clicked += delegate
			{
				this.ActionMoveUp ();
			};

			this.downButton.Clicked += delegate
			{
				this.ActionMoveDown ();
			};

			this.scrollList.SelectedItemChanged += delegate
			{
				this.ActionSelectionChanged ();
			};

			this.InitialiseEnumValuesList ();
			this.UpdateScrollList ();
			this.UpdateButtons ();
		}


		private void ActionCreate()
		{
			int sel = this.SelectedIndex;

			if (sel == -1)
			{
				sel = this.enumValues.Count;  // insère à la fin
			}
			else
			{
				sel++;  // insère après la ligne sélectionnée
			}

			this.enumValues.Insert (sel, new EnumValue ());

			this.StoreEnumValuesList ();
			this.UpdateScrollList (sel);
			this.UpdateButtons ();
			this.tileContainer.UpdateAllWidgets ();
		}

		private void ActionDelete()
		{
			int sel = this.SelectedIndex;

			this.enumValues.RemoveAt (sel);

			if (sel >= this.enumValues.Count)
			{
				sel = this.enumValues.Count-1;
			}

			this.StoreEnumValuesList ();
			this.UpdateScrollList (sel);
			this.UpdateButtons ();
			this.tileContainer.UpdateAllWidgets ();
		}

		private void ActionMoveUp()
		{
			int sel = this.SelectedIndex;

			var t = this.enumValues[sel];
			this.enumValues.RemoveAt (sel);
			this.enumValues.Insert (sel-1, t);

			this.StoreEnumValuesList ();
			this.UpdateScrollList (sel-1);
			this.UpdateButtons ();
			this.tileContainer.UpdateAllWidgets ();
		}

		private void ActionMoveDown()
		{
			int sel = this.SelectedIndex;

			var t = this.enumValues[sel];
			this.enumValues.RemoveAt (sel);
			this.enumValues.Insert (sel+1, t);

			this.StoreEnumValuesList ();
			this.UpdateScrollList (sel+1);
			this.UpdateButtons ();
			this.tileContainer.UpdateAllWidgets ();
		}

		private void ActionSelectionChanged()
		{
			if (this.ignoreChange)
			{
				return;
			}

			this.UpdateButtons ();
			this.tileContainer.UpdateAllWidgets ();
		}


		private void UpdateButtons()
		{
			int sel = this.SelectedIndex;

			this.createButton.Enable = true;
			this.deleteButton.Enable = sel != -1;
			this.upButton.Enable = sel > 0;
			this.downButton.Enable = sel != -1 && sel < this.enumValues.Count-1;
		}

		private void UpdateScrollList(int? sel=null)
		{
			if (!sel.HasValue)
			{
				sel = this.SelectedIndex;
			}

			this.ignoreChange = true;

			this.scrollList.Items.Clear ();

			foreach (var enumValue in this.enumValues)
			{
				string text;

				if (enumValue.IsEmpty)
				{
					text = "<i>Vide</i>";
				}
				else
				{
					text = UIBuilder.FormatText (enumValue.Value, "(", enumValue.ShortDescription, ",~", Misc.FirstLine (enumValue.LongDescription), ")").ToSimpleText ();
				}

				this.scrollList.Items.Add (text);
			}

			this.ignoreChange = false;

			if (sel.HasValue)
			{
				this.SelectedIndex = sel.Value;
				this.scrollList.ShowSelected (ScrollShowMode.Extremity);
			}

			this.UpdateLabelCount ();
		}

		private void UpdateLabelCount()
		{
			string text;

			if (this.enumValues.Count == 0)
			{
				text = "Aucune valeur";
			}
			else if (this.enumValues.Count == 1)
			{
				text = "1 valeur";
			}
			else
			{
				text = string.Format ("{0} valeurs", this.enumValues.Count.ToString ());
			}

			this.labelCount.Text = text;
		}

		private int SelectedIndex
		{
			get
			{
				return this.scrollList.SelectedItemIndex;
			}
			set
			{
				this.ignoreChange = true;
				this.scrollList.SelectedItemIndex = value;
				this.ignoreChange = false;
			}
		}



		private void InitialiseEnumValuesList()
		{
			//	parameterEntity -> enumValues
			this.enumValues.Clear ();

			string[] values            = (this.parameterEntity.Values            ?? "").Split (new string[] { AbstractArticleParameterDefinitionEntity.Separator }, System.StringSplitOptions.None);
			string[] shortDescriptions = (this.parameterEntity.ShortDescriptions ?? "").Split (new string[] { AbstractArticleParameterDefinitionEntity.Separator }, System.StringSplitOptions.None);
			string[] longDescriptions  = (this.parameterEntity.LongDescriptions  ?? "").Split (new string[] { AbstractArticleParameterDefinitionEntity.Separator }, System.StringSplitOptions.None);

			int max = System.Math.Max (values.Length, System.Math.Max (shortDescriptions.Length, longDescriptions.Length));
			for (int i = 0; i < max; i++)
			{
				var enumValue = new EnumValue ();

				if (i < values.Length)
				{
					enumValue.Value = values[i];
				}

				if (i < shortDescriptions.Length)
				{
					enumValue.ShortDescription = shortDescriptions[i];
				}

				if (i < longDescriptions.Length)
				{
					enumValue.LongDescription = longDescriptions[i];
				}

				if (!enumValue.IsEmpty)
				{
					this.enumValues.Add (enumValue);
				}
			}
		}

		private void StoreEnumValuesList()
		{
			//	enumValues -> parameterEntity
			//	Génial, cet opérateur Select !
			this.parameterEntity.Values            = string.Join (AbstractArticleParameterDefinitionEntity.Separator, this.enumValues.Select (x => x.Value));
			this.parameterEntity.ShortDescriptions = string.Join (AbstractArticleParameterDefinitionEntity.Separator, this.enumValues.Select (x => x.ShortDescription));
			this.parameterEntity.LongDescriptions  = string.Join (AbstractArticleParameterDefinitionEntity.Separator, this.enumValues.Select (x => x.LongDescription));
		}


		private class EnumValue
		{
			public string Value;
			public string ShortDescription;
			public string LongDescription;

			public bool IsEmpty
			{
				get
				{
					return string.IsNullOrEmpty (this.Value)            &&
						   string.IsNullOrEmpty (this.ShortDescription) &&
						   string.IsNullOrEmpty (this.LongDescription);
				}
			}
		}


		private readonly TileContainer tileContainer;
		private readonly EnumValueArticleParameterDefinitionEntity parameterEntity;

		private GlyphButton createButton;
		private GlyphButton deleteButton;
		private GlyphButton upButton;
		private GlyphButton downButton;
		private StaticText labelCount;
		private ScrollList scrollList;

		private List<EnumValue> enumValues;
		private bool ignoreChange;
	}
}
