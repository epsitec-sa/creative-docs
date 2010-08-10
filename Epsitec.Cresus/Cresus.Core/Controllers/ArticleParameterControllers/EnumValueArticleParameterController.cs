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
	/// Ce contrôleur permet de saisir une énumération pour un paramètre d'article, dans une ligne d'article
	/// d'une facture.
	/// </summary>
	public class EnumValueArticleParameterController : AbstractArticleParameterController
	{
		public EnumValueArticleParameterController(ArticleDocumentItemEntity article, int parameterIndex)
			: base (article, parameterIndex)
		{
		}


		public override void CreateUI(FrameBox parent)
		{
			var enumParameter = this.ParameterDefinition as EnumValueArticleParameterDefinitionEntity;
			double buttonWidth = 14;

			//	Ligne éditable.
			this.editor = new ItemPicketCombo
			{
				Parent = parent,
				MenuButtonWidth = buttonWidth,
				Cardinality = enumParameter.Cardinality,
				Dock = DockStyle.Fill,
				TabIndex = 1,
			};

			//	Initialise le menu des valeurs.
			string[] values            = (enumParameter.Values            ?? "").Split (new string[] { AbstractArticleParameterDefinitionEntity.Separator }, System.StringSplitOptions.None);
			string[] shortDescriptions = (enumParameter.ShortDescriptions ?? "").Split (new string[] { AbstractArticleParameterDefinitionEntity.Separator }, System.StringSplitOptions.None);

			for (int i = 0; i < System.Math.Min (values.Length, shortDescriptions.Length); i++)
			{
				this.editor.Items.Add (values[i], shortDescriptions[i]);
			}

			//	Initialise le contenu.
			string[] parameterValues = (this.ParameterValue ?? "").Split (new string[] { AbstractArticleParameterDefinitionEntity.Separator }, System.StringSplitOptions.None);

			foreach (var parameterValue in parameterValues)
			{
				int i = this.GetIndex (parameterValue);

				if (i != -1)
				{
					this.editor.AddSelection (new int[] { i });
				}
			}

			//	Initialise le contenu par défaut.
			if (this.editor.SelectionCount == 0 && !string.IsNullOrEmpty (enumParameter.DefaultValue))
			{
				int i = this.GetIndex (enumParameter.DefaultValue);

				if (i != -1)
				{
					this.editor.AddSelection (new int[] { i });
				}
			}

			this.editor.SelectedItemChanged += delegate
			{
				this.ParameterValue = this.SelectedParameterValues;
			};
		}


		private int GetIndex(string key)
		{
			//	Retourne l'index d'une clé.
			for (int i = 0; i < this.editor.Items.Count; i++)
			{
				if (key == this.editor.Items.GetKey (i))
				{
					return i;
				}
			}

			return -1;
		}

		private string SelectedParameterValues
		{
			//	Retourne la liste des paramètres choisis (les codes spéarés par des '∙').
			get
			{
				var list = new List<string> ();

				var sel = this.editor.GetSortedSelection ();
				foreach (int i in sel)
				{
					string key = this.editor.Items.GetKey (i);
					list.Add (key);
				}

				return string.Join (AbstractArticleParameterDefinitionEntity.Separator, list);
			}
		}


		private ItemPicketCombo editor;
	}
}
