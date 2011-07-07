//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using Epsitec.Common.Drawing;
using Epsitec.Common.Widgets;
using Epsitec.Common.Types;
using Epsitec.Common.Types.Converters;

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
	public class ArticleParameterListPreferredValuesController
	{
		public ArticleParameterListPreferredValuesController(TileContainer tileContainer, NumericValueArticleParameterDefinitionEntity parameterEntity)
		{
			this.tileContainer = tileContainer;
			this.parameterEntity = parameterEntity;

			this.values = new List<decimal> ();
		}


		public void CreateUI(FrameBox parent, bool isReadOnly)
		{
			this.InitialiseEnumValuesList ();

			//	Crée la liste.
			var listContainer = new FrameBox
			{
				Parent = parent,
				PreferredHeight = Library.UI.Constants.TinyButtonSize+3+124+2+TileArrow.Breadth,
				Dock = DockStyle.Top,
				Margins = new Margins (0, Library.UI.Constants.RightMargin, 2, 0),
			};

			this.listController = new ListController<decimal> (this.values, this.ListControllerItemToText, this.ListControllerGetTextInfo, this.ListControllerCreateItem);
			this.listController.CreateUI (listContainer, Direction.Down, Library.UI.Constants.TinyButtonSize, isReadOnly);

			ToolTip.Default.SetToolTip (this.listController.AddButton,      "Ajoute une nouvelle valeur préférentielle");
			ToolTip.Default.SetToolTip (this.listController.RemoveButton,   "Supprime la valeur préférentielle");
			ToolTip.Default.SetToolTip (this.listController.MoveUpButton,   "Montre la valeur dans la liste");
			ToolTip.Default.SetToolTip (this.listController.MoveDownButton, "Descend la valeur dans la liste");

			//	Crée l'édition de la valeur.
			this.valueLabel = new StaticText
			{
				Parent = parent,
				Text = "Valeur préférentielle :",
				TextBreakMode = Common.Drawing.TextBreakMode.Ellipsis | Common.Drawing.TextBreakMode.Split | Common.Drawing.TextBreakMode.SingleLine,
				Dock = DockStyle.Top,
				Margins = new Margins (0, Library.UI.Constants.RightMargin, 0, Library.UI.Constants.MarginUnderLabel),
			};

			this.valueField = new TextFieldEx
			{
				Parent = parent,
				IsReadOnly = isReadOnly,
				PreferredHeight = 20,
				Dock = DockStyle.Top,
				Margins = new Margins (0, Library.UI.Constants.RightMargin, 0, Library.UI.Constants.MarginUnderTextField),
				TabIndex = 3,
				DefocusAction = DefocusAction.AcceptEdition,
			};

			var marshaler = Marshaler.Create (() => this.SelectedValue, x => this.SelectedValue = x);
			var valueController = new TextValueController (marshaler);
			valueController.Attach (this.valueField);

			//	Connecte tous les événements.
			this.listController.SelectedItemChanged += delegate
			{
				this.ActionSelectedItemChanged ();
			};

			this.listController.ItemInserted += delegate
			{
				this.ActionItemInserted ();
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
			this.valueField.SelectAll ();
			this.valueField.Focus ();
		}


		private void UpdateFields()
		{
			this.valueField.Text = this.SelectedValue.ToString ();
		}

		private void UpdateButtons()
		{
			int sel = this.listController.SelectedIndex;

			this.valueLabel.Enable = sel != -1;
			this.valueField.Enable = sel != -1;
		}


		private decimal SelectedValue
		{
			get
			{
				if (this.listController.SelectedIndex == -1)
				{
					return 0;
				}
				else
				{
					return this.values[this.listController.SelectedIndex];
				}
			}
			set
			{
				if (this.listController.SelectedIndex != -1)
				{
					this.values[this.listController.SelectedIndex] = value;
					this.StoreEnumValuesList ();
					this.listController.UpdateList ();
				}
			}
		}


		private void InitialiseEnumValuesList()
		{
			//	parameterEntity -> values
			this.values.Clear ();

			string[] values = (this.parameterEntity.PreferredValues ?? "").Split (new string[] { AbstractArticleParameterDefinitionEntity.Separator }, System.StringSplitOptions.None);

			foreach (var value in values)
			{
				if (!string.IsNullOrWhiteSpace (value))
				{
					decimal d;

					if (decimal.TryParse (value, out d))
					{
						this.values.Add (d);
					}
				}
			}
		}

		private void StoreEnumValuesList()
		{
			//	values -> parameterEntity
			var v = this.values.Select (x => x.ToString (System.Globalization.CultureInfo.InvariantCulture));

			this.parameterEntity.PreferredValues = string.Join (AbstractArticleParameterDefinitionEntity.Separator, v);
		}


		#region ListController callbacks
		private FormattedText ListControllerItemToText(decimal value)
		{
			return value.ToString ();
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

		private decimal ListControllerCreateItem(int sel)
		{
			return this.parameterEntity.MinValue.GetValueOrDefault (0);
		}
		#endregion

	
		private readonly TileContainer tileContainer;
		private readonly NumericValueArticleParameterDefinitionEntity parameterEntity;

		private ListController<decimal> listController;
		private StaticText valueLabel;
		private TextFieldEx valueField;

		private List<decimal> values;
	}
}
