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
	/// Ce contrôleur permet de saisir une valeur numérique pour un paramètre d'article, dans une ligne d'article
	/// d'une facture.
	/// </summary>
	public class OptionValueArticleParameterController : AbstractArticleParameterController
	{
		public OptionValueArticleParameterController(IArticleDefinitionParameters article, int parameterIndex)
			: base (article, parameterIndex)
		{
		}


		public override void CreateUI(FrameBox parent)
		{
			var optionParameter = this.ParameterDefinition as OptionValueArticleParameterDefinitionEntity;

			List<FormattedText> names     = optionParameter.Options.Select (x => x.Name).ToList ();
			List<FormattedText> summaries = optionParameter.Options.Select (x => x.GetSummary ()).ToList ();
			int count = names.Count;

			if (count == 0)
			{
				var label = new StaticText
				{
					Parent = parent,
					Text = "<i>Aucune option</i>",
					Dock = DockStyle.Fill,
				};
			}
			if (count == 1)
			{
				var button = new CheckButton
				{
					Parent = parent,
					FormattedText = summaries[0],
					Dock = DockStyle.Fill,
				};

				button.ActiveStateChanged += delegate
				{
					this.ParameterValue = (button.ActiveState == ActiveState.Yes) ? names[0].ToString () : "-";
				};
			}
			else
			{
				double buttonWidth = 14;

				//	Ligne éditable.
				this.editor = new ItemPicketCombo
				{
					Parent = parent,
					MenuButtonWidth = buttonWidth,
					Cardinality = optionParameter.Cardinality,
					Dock = DockStyle.Fill,
					TabIndex = 1,
				};

				//	Initialise le menu des valeurs.
				for (int i = 0; i < count; i++)
				{
					this.editor.Items.Add (names[i].ToString (), summaries[i].ToString ());
				}

				// TODO: finir...
#if false
				//	Initialise le contenu.
				var list = new List<int> ();

				foreach (var parameterValue in parameterValues)
				{
					int i = this.GetIndex (parameterValue);

					if (i != -1)
					{
						list.Add (i);
					}
				}

				this.editor.AddSelection (list);

				this.editor.SelectedItemChanged += delegate
				{
					this.ParameterValue = this.SelectedParameterValues;
				};
#endif
			}
		}


		private ItemPicketCombo editor;
	}
}
