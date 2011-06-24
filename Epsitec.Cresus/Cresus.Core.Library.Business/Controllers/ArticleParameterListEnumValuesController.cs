//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using Epsitec.Common.Drawing;
using Epsitec.Common.Widgets;
using Epsitec.Common.Types;

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
using Epsitec.Cresus.Core.Factories;

namespace Epsitec.Cresus.Core.Controllers.ArticleParameterControllers
{
	/// <summary>
	/// Ce contrôleur gère la définition des paramètres d'un article.
	/// </summary>
	public class ArticleParameterListEnumValuesController : IEntitySpecialController
	{
		public ArticleParameterListEnumValuesController(TileContainer tileContainer, EnumValueArticleParameterDefinitionEntity parameterEntity)
		{
			this.tileContainer = tileContainer;
			this.parameterEntity = parameterEntity;

			this.enumValues = new List<EnumValue> ();
		}


		public void CreateUI(Widget parent, bool isReadOnly)
		{
			this.isReadOnly = isReadOnly;

			this.InitialiseEnumValuesList ();

			//	Crée la liste.
			var listContainer = new FrameBox
			{
				Parent = parent,
				PreferredHeight = Library.UI.TinyButtonSize+3+124+2+TileArrow.Breadth,
				Dock = DockStyle.Top,
				Margins = new Margins (0, Library.UI.RightMargin, 2, 0),
			};

			this.listController = new ListController<EnumValue> (this.enumValues, this.ListControllerItemToText, this.ListControllerGetTextInfo, this.ListControllerCreateItem);
			this.listController.CreateUI (listContainer, Direction.Down, Library.UI.TinyButtonSize, isReadOnly);

			ToolTip.Default.SetToolTip (this.listController.AddButton,      "Ajoute une nouvelle valeur dans l'énumération");
			ToolTip.Default.SetToolTip (this.listController.RemoveButton,   "Supprime la valeur de l'énumération");
			ToolTip.Default.SetToolTip (this.listController.MoveUpButton,   "Montre la valeur dans la liste");
			ToolTip.Default.SetToolTip (this.listController.MoveDownButton, "Descend la valeur dans la liste");

			//	Crée l'édition de la valeur.
			this.valueLabel = new StaticText
			{
				Parent = parent,
				Text = "Valeur :",
				TextBreakMode = Common.Drawing.TextBreakMode.Ellipsis | Common.Drawing.TextBreakMode.Split | Common.Drawing.TextBreakMode.SingleLine,
				Dock = DockStyle.Top,
				Margins = new Margins (0, Library.UI.RightMargin, 0, Library.UI.MarginUnderLabel),
			};

			this.valueField = new TextFieldEx
			{
				Parent = parent,
				IsReadOnly = isReadOnly,
				PreferredHeight = 20,
				Dock = DockStyle.Top,
				Margins = new Margins (0, Library.UI.RightMargin, 0, Library.UI.MarginUnderTextField),
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
				Margins = new Margins (0, Library.UI.RightMargin, 0, Library.UI.MarginUnderLabel),
			};

			this.shortDescriptionField = new TextFieldEx
			{
				Parent = parent,
				IsReadOnly = isReadOnly,
				PreferredHeight = 20,
				Dock = DockStyle.Top,
				Margins = new Margins (0, Library.UI.RightMargin, 0, Library.UI.MarginUnderTextField),
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
				Margins = new Margins (0, Library.UI.RightMargin, 0, Library.UI.MarginUnderLabel),
			};

			this.longDescriptionField = new TextFieldMultiEx
			{
				Parent = parent,
				IsReadOnly = isReadOnly,
				PreferredHeight = 78,
				Dock = DockStyle.Top,
				Margins = new Margins (0, Library.UI.RightMargin, 0, Library.UI.MarginUnderTextField),
				TabIndex = 5,
				DefocusAction = DefocusAction.AcceptEdition,
				ScrollerVisibility = false,
				PreferredLayout = TextFieldMultiExPreferredLayout.PreserveScrollerHeight,
			};

			//	Créer le bouton pour la valeur par défaut.
			this.defaultButton = new CheckButton
			{
				Parent = parent,
				Enable = !isReadOnly,
				Text = "Cette valeur est la valeur par défaut",
				AutoToggle = false,
				Dock = DockStyle.Top,
				Margins = new Margins (0, Library.UI.RightMargin, 5, Library.UI.MarginUnderTextField),
				TabIndex = 6,
			};

			//	Connecte tous les événements.
			this.listController.SelectedItemChanged += delegate
			{
				this.ActionSelectedItemChanged ();
			};

			this.listController.ItemInserted += delegate
			{
				this.ActionItemInserted ();
			};

			this.valueField.EditionAccepted += delegate
			{
				this.SelectedValue = this.valueField.Text;
			};

			this.shortDescriptionField.EditionAccepted += delegate
			{
				this.SelectedShortDescription = this.shortDescriptionField.Text;
			};

			this.longDescriptionField.EditionAccepted += delegate
			{
				this.SelectedLongDescription = this.longDescriptionField.Text;
			};

			this.defaultButton.Clicked += delegate
			{
				this.defaultButton.ActiveState = (this.defaultButton.ActiveState == ActiveState.Yes) ? ActiveState.No : ActiveState.Yes;
				this.SelectedDefaultValue = this.defaultButton.ActiveState == ActiveState.Yes;
			};

			this.UpdateButtons ();
		}


		private void ActionSelectedItemChanged()
		{
			this.StoreEnumValuesList ();
			this.UpdateFields ();
			this.UpdateButtons ();
			this.tileContainer.UpdateAllWidgets ();
		}

		private void ActionItemInserted()
		{
			this.valueField.Focus ();
		}


		private void UpdateFields()
		{
			this.valueField.Text                     = this.SelectedValue;
			this.shortDescriptionField.FormattedText = this.SelectedShortDescription;
			this.longDescriptionField.FormattedText  = this.SelectedLongDescription;
			this.defaultButton.ActiveState           = this.SelectedDefaultValue ? ActiveState.Yes : ActiveState.No;
		}

		private void UpdateButtons()
		{
			int sel = this.listController.SelectedIndex;

			this.valueLabel.Enable            = sel != -1;
			this.shortDescriptionLabel.Enable = sel != -1;
			this.longDescriptionLabel.Enable  = sel != -1;
			this.valueField.Enable            = sel != -1;
			this.shortDescriptionField.Enable = sel != -1;
			this.longDescriptionField.Enable  = sel != -1;
			this.defaultButton.Enable         = !this.isReadOnly && sel != -1;
		}


		private string SelectedValue
		{
			get
			{
				if (this.listController.SelectedIndex == -1)
				{
					return null;
				}
				else
				{
					return this.enumValues[this.listController.SelectedIndex].Value;
				}
			}
			set
			{
				if (this.listController.SelectedIndex != -1 && this.enumValues[this.listController.SelectedIndex].Value != value)
				{
					if (this.SelectedDefaultValue)  // est-on en train de changer la valeur utilisée comme valeur par défaut ?
					{
						this.parameterEntity.DefaultValue = value;
					}

					this.enumValues[this.listController.SelectedIndex].Value = value;
					this.StoreEnumValuesList ();
					this.listController.UpdateList ();
				}
			}
		}

		private FormattedText SelectedShortDescription
		{
			get
			{
				if (this.listController.SelectedIndex == -1)
				{
					return FormattedText.Null;
				}
				else
				{
					return this.enumValues[this.listController.SelectedIndex].ShortDescription;
				}
			}
			set
			{
				if (this.listController.SelectedIndex != -1 && this.enumValues[this.listController.SelectedIndex].ShortDescription != value)
				{
					this.enumValues[this.listController.SelectedIndex].ShortDescription = value;
					this.StoreEnumValuesList ();
					this.listController.UpdateList ();
				}
			}
		}

		private FormattedText SelectedLongDescription
		{
			get
			{
				if (this.listController.SelectedIndex == -1)
				{
					return FormattedText.Null;
				}
				else
				{
					return this.enumValues[this.listController.SelectedIndex].LongDescription;
				}
			}
			set
			{
				if (this.listController.SelectedIndex != -1 && this.enumValues[this.listController.SelectedIndex].LongDescription != value)
				{
					this.enumValues[this.listController.SelectedIndex].LongDescription = value;
					this.StoreEnumValuesList ();
					this.listController.UpdateList ();
				}
			}
		}

		private bool SelectedDefaultValue
		{
			get
			{
				return this.listController.SelectedIndex < 0 ? false : this.parameterEntity.DefaultValue == this.enumValues[this.listController.SelectedIndex].Value;
			}
			set
			{
				this.parameterEntity.DefaultValue = value ? this.enumValues[this.listController.SelectedIndex].Value : null;
				this.listController.UpdateList ();
			}
		}


		private void InitialiseEnumValuesList()
		{
			//	parameterEntity -> enumValues
			this.enumValues.Clear ();

			string[] values = (this.parameterEntity.Values ?? "").Split (new string[] { AbstractArticleParameterDefinitionEntity.Separator }, System.StringSplitOptions.None);
			FormattedText[] shortDescriptions = FormattedText.Split (this.parameterEntity.ShortDescriptions, AbstractArticleParameterDefinitionEntity.Separator);
			FormattedText[] longDescriptions  = FormattedText.Split (this.parameterEntity.LongDescriptions,  AbstractArticleParameterDefinitionEntity.Separator);

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

			this.parameterEntity.Values = string.Join (AbstractArticleParameterDefinitionEntity.Separator, extract.Select (x => x.Value));
			this.parameterEntity.ShortDescriptions = FormattedText.Join (AbstractArticleParameterDefinitionEntity.Separator, extract.Select (x => x.ShortDescription).ToArray ());
			this.parameterEntity.LongDescriptions  = FormattedText.Join (AbstractArticleParameterDefinitionEntity.Separator, extract.Select (x => x.LongDescription ).ToArray ());
		}


		private class EnumValue
		{
			public string Value;
			public FormattedText ShortDescription;
			public FormattedText LongDescription;

			public bool IsEmpty
			{
				get
				{
					return string.IsNullOrEmpty (this.Value) &&
						   this.ShortDescription.IsNullOrEmpty &&
						   this.LongDescription.IsNullOrEmpty;
				}
			}
		}


		#region ListController callbacks
		private FormattedText ListControllerItemToText(EnumValue enumValue)
		{
			string icon;
			FormattedText text;

			if (enumValue.IsEmpty)
			{
				icon = "Button.RadioNo";
				text = TextFormatter.FormatText ("<i>Vide</i>");
			}
			else
			{
				icon = (enumValue.Value == this.parameterEntity.DefaultValue) ? "Button.RadioYes" : "Button.RadioNo";
				text = TextFormatter.FormatText (enumValue.Value, "(", enumValue.ShortDescription, ",~", enumValue.LongDescription.Lines.FirstOrDefault (), ")");
			}

			return string.Concat (Misc.GetResourceIconImageTag (icon, -4), " ", text);
		}

		private FormattedText ListControllerGetTextInfo(int count)
		{
			if (count == 0)
			{
				return "Aucune valeur";
			}
			else if (count == 1)
			{
				return string.Format ("{0} valeur", count.ToString ());
			}
			else
			{
				return string.Format ("{0} valeurs", count.ToString ());
			}
		}

		private EnumValue ListControllerCreateItem(int sel)
		{
			return new EnumValue ();
		}
		#endregion


		private class Factory : DefaultEntitySpecialControllerFactory<EnumValueArticleParameterDefinitionEntity>
		{
			protected override IEntitySpecialController Create(TileContainer container, EnumValueArticleParameterDefinitionEntity entity, int mode)
			{
				return new ArticleParameterListEnumValuesController (container, entity);
			}
		}


	
		private readonly TileContainer tileContainer;
		private readonly EnumValueArticleParameterDefinitionEntity parameterEntity;

		private ListController<EnumValue> listController;
		private StaticText valueLabel;
		private StaticText shortDescriptionLabel;
		private StaticText longDescriptionLabel;
		private TextFieldEx valueField;
		private TextFieldEx shortDescriptionField;
		private TextFieldMultiEx longDescriptionField;
		private CheckButton defaultButton;

		private List<EnumValue> enumValues;
		private bool isReadOnly;
	}
}
