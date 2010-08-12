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
	/// <summary>
	/// Ce contrôleur gère la définition des paramètres d'un article.
	/// </summary>
	public class ArticleParameterListController
	{
		public ArticleParameterListController(TileContainer tileContainer, EnumValueArticleParameterDefinitionEntity parameterEntity)
		{
			this.tileContainer = tileContainer;
			this.parameterEntity = parameterEntity;

			this.enumValues = new List<EnumValue> ();
		}


		public void CreateUI(FrameBox parent)
		{
			double buttonSize = UIBuilder.TinyButtonSize;

			//	Crée la toolbar.
			var toolbar = new FrameBox
			{
				Parent = parent,
				PreferredHeight = buttonSize,
				Dock = DockStyle.Top,
				Margins = new Margins (0, UIBuilder.RightMargin, 2, 1),
				TabIndex = 1,
			};

			this.createButton = new GlyphButton
			{
				Parent = toolbar,
				PreferredSize = new Size (buttonSize*2+1, buttonSize),
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

			//	Crée la liste.
			this.scrollList = new ScrollList
			{
				Parent = parent,
				PreferredHeight = 124,
				Dock = DockStyle.Top,
				Margins = new Margins (0, UIBuilder.RightMargin, 0, 0),
				TabIndex = 2,
			};

			//	Crée l'édition de la valeur.
			this.valueLabel = new StaticText
			{
				Parent = parent,
				Text = "Valeur :",
				TextBreakMode = Common.Drawing.TextBreakMode.Ellipsis | Common.Drawing.TextBreakMode.Split | Common.Drawing.TextBreakMode.SingleLine,
				Dock = DockStyle.Top,
				Margins = new Margins (0, UIBuilder.RightMargin, 0, UIBuilder.MarginUnderLabel),
			};

			this.valueField = new TextFieldEx
			{
				Parent = parent,
				PreferredHeight = 20,
				Dock = DockStyle.Top,
				Margins = new Margins (0, UIBuilder.RightMargin, 0, UIBuilder.MarginUnderTextField),
				TabIndex = 3,
				DefocusAction = DefocusAction.AcceptEdition,
			};

			//	Crée l'édition de la description courte.
			this.shortDescriptionLabel = new StaticText
			{
				Parent = parent,
				Text = "Description courte :",
				TextBreakMode = Common.Drawing.TextBreakMode.Ellipsis | Common.Drawing.TextBreakMode.Split | Common.Drawing.TextBreakMode.SingleLine,
				Dock = DockStyle.Top,
				Margins = new Margins (0, UIBuilder.RightMargin, 0, UIBuilder.MarginUnderLabel),
			};

			this.shortDescriptionField = new TextFieldEx
			{
				Parent = parent,
				PreferredHeight = 20,
				Dock = DockStyle.Top,
				Margins = new Margins (0, UIBuilder.RightMargin, 0, UIBuilder.MarginUnderTextField),
				TabIndex = 4,
				DefocusAction = DefocusAction.AcceptEdition,
			};

			//	Crée l'édition de la description longue.
			this.longDescriptionLabel = new StaticText
			{
				Parent = parent,
				Text = "Description longue :",
				TextBreakMode = Common.Drawing.TextBreakMode.Ellipsis | Common.Drawing.TextBreakMode.Split | Common.Drawing.TextBreakMode.SingleLine,
				Dock = DockStyle.Top,
				Margins = new Margins (0, UIBuilder.RightMargin, 0, UIBuilder.MarginUnderLabel),
			};

			this.longDescriptionField = new TextFieldMultiEx
			{
				Parent = parent,
				PreferredHeight = 78,
				Dock = DockStyle.Top,
				Margins = new Margins (0, UIBuilder.RightMargin, 0, UIBuilder.MarginUnderTextField),
				TabIndex = 5,
				DefocusAction = DefocusAction.AcceptEdition,
				ScrollerVisibility = false,
				PreferredLayout = TextFieldMultiExPreferredLayout.PreserveScrollerHeight,
			};

			//	Créer le bouton pour la valeur par défaut.
			this.defaultButton = new CheckButton
			{
				Parent = parent,
				Text = "Cette valeur est la valeur par défaut",
				AutoToggle = false,
				Dock = DockStyle.Top,
				Margins = new Margins (0, UIBuilder.RightMargin, 5, UIBuilder.MarginUnderTextField),
				TabIndex = 6,
			};

			//	Connecte tous les événements.
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

			this.valueField.AcceptingEdition += delegate
			{
				this.SelectedValue = this.valueField.Text;
			};

			this.shortDescriptionField.AcceptingEdition += delegate
			{
				this.SelectedShortDescription = this.shortDescriptionField.Text;
			};

			this.longDescriptionField.AcceptingEdition += delegate
			{
				this.SelectedLongDescription = this.longDescriptionField.Text;
			};

			this.defaultButton.Clicked += delegate
			{
				this.defaultButton.ActiveState = (this.defaultButton.ActiveState == ActiveState.Yes) ? ActiveState.No : ActiveState.Yes;
				this.SelectedDefaultValue = this.defaultButton.ActiveState == ActiveState.Yes;
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
			this.UpdateFields ();
			this.UpdateButtons ();
			this.tileContainer.UpdateAllWidgets ();

			this.valueField.Focus ();
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
			this.UpdateFields ();
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
			this.UpdateFields ();
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
			this.UpdateFields ();
			this.UpdateButtons ();
			this.tileContainer.UpdateAllWidgets ();
		}

		private void ActionSelectionChanged()
		{
			if (this.ignoreChange)
			{
				return;
			}

			this.UpdateFields ();
			this.UpdateButtons ();
			this.tileContainer.UpdateAllWidgets ();
		}


		private void UpdateFields()
		{
			this.valueField.Text            = this.SelectedValue;
			this.shortDescriptionField.Text = this.SelectedShortDescription;
			this.longDescriptionField.Text  = this.SelectedLongDescription;
			this.defaultButton.ActiveState  = this.SelectedDefaultValue ? ActiveState.Yes : ActiveState.No;
		}

		private void UpdateButtons()
		{
			int sel = this.SelectedIndex;

			this.createButton.Enable = true;
			this.deleteButton.Enable = sel != -1;
			this.upButton.Enable = sel > 0;
			this.downButton.Enable = sel != -1 && sel < this.enumValues.Count-1;

			this.valueLabel.Enable = sel != -1;
			this.shortDescriptionLabel.Enable = sel != -1;
			this.longDescriptionLabel.Enable = sel != -1;
			this.valueField.Enable = sel != -1;
			this.shortDescriptionField.Enable = sel != -1;
			this.longDescriptionField.Enable = sel != -1;
			this.defaultButton.Enable = sel != -1;
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
				string icon, text;

				if (enumValue.IsEmpty)
				{
					icon = "Button.RadioNo";
					text = "<i>Vide</i>";
				}
				else
				{
					icon = (enumValue.Value == this.parameterEntity.DefaultValue) ? "Button.RadioYes" : "Button.RadioNo";
					text = TextFormatter.FormatText (enumValue.Value, "(", enumValue.ShortDescription, ",~", Misc.FirstLine (enumValue.LongDescription), ")").ToSimpleText ();
				}

				text = string.Concat (Misc.GetResourceIconImageTag (icon, -4), " ", text);
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


		private string SelectedValue
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
					if (this.SelectedDefaultValue)  // est-on en train de changer la valeur utilisée comme valeur par défaut ?
					{
						this.parameterEntity.DefaultValue = value;
					}

					this.enumValues[this.SelectedIndex].Value = value;
					this.StoreEnumValuesList ();
					this.UpdateScrollList ();
				}
			}
		}

		private string SelectedShortDescription
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

		private string SelectedLongDescription
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

		private bool SelectedDefaultValue
		{
			get
			{
				return this.parameterEntity.DefaultValue == this.enumValues[this.SelectedIndex].Value;
			}
			set
			{
				this.parameterEntity.DefaultValue = value ? this.enumValues[this.SelectedIndex].Value : null;
				this.UpdateScrollList ();
			}
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
			var extract = this.enumValues.Where (x => !string.IsNullOrEmpty (x.Value));

			this.parameterEntity.Values            = string.Join (AbstractArticleParameterDefinitionEntity.Separator, extract.Select (x => x.Value));
			this.parameterEntity.ShortDescriptions = string.Join (AbstractArticleParameterDefinitionEntity.Separator, extract.Select (x => x.ShortDescription));
			this.parameterEntity.LongDescriptions  = string.Join (AbstractArticleParameterDefinitionEntity.Separator, extract.Select (x => x.LongDescription));
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
		private StaticText valueLabel;
		private StaticText shortDescriptionLabel;
		private StaticText longDescriptionLabel;
		private TextFieldEx valueField;
		private TextFieldEx shortDescriptionField;
		private TextFieldMultiEx longDescriptionField;
		private CheckButton defaultButton;

		private List<EnumValue> enumValues;
		private bool ignoreChange;
	}
}
