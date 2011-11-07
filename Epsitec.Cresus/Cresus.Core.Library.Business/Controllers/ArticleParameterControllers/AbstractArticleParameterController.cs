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
	/// Classe de base pour la saisie d'une valeur numérique ou d'une énumération pour un paramètre d'article,
	/// dans une ligne d'article d'une facture.
	/// </summary>
	public abstract class AbstractArticleParameterController
	{
		public AbstractArticleParameterController(IArticleDefinitionParameters article, int parameterIndex, TileContainer tileContainer)
		{
			this.article        = article;
			this.parameterIndex = parameterIndex;
			this.tileContainer  = tileContainer;

			this.ImportDictionary ();
		}


		public System.Action<AbstractArticleParameterDefinitionEntity> CallbackParameterChanged
		{
			//	Définition de la méthode qui sera appelée lorsque le paramètre sera changé.
			get;
			set;
		}


		public virtual void CreateUI(FrameBox parent)
		{
		}


		protected string ParameterValue
		{
			get
			{
				var parameters = this.ImportDictionary ();

				if (!string.IsNullOrEmpty (this.Code))
				{
					string value;
					if (parameters.TryGetValue (this.Code, out value))
					{
						return value;
					}
				}

				return null;
			}
			set
			{
				var parameters = this.ImportDictionary ();

				parameters[this.Code] = value;

				this.ExportDictionary (parameters);

				//	Appel du callback pour informer le propriétaire du changement du paramètre.
				if (this.CallbackParameterChanged != null)
				{
					this.CallbackParameterChanged (this.ParameterDefinition);
				}
			}
		}

		private Dictionary<string, string> ImportDictionary()
		{
			//	Ce dictionnaiore ne doit pas être conservé, car plusieurs contrôleurs travaillent simultanément.
			var parameters = new Dictionary<string, string> ();

			string[] values = (this.article.ArticleParameters ?? "").Split (new string[] { AbstractArticleParameterController.Separator }, System.StringSplitOptions.None);
			for (int i = 0; i < values.Length-1; i+=2)
			{
				string key  = values[i+0];
				string data = values[i+1];

				parameters.Add (key, data);
			}

			return parameters;
		}

		private void ExportDictionary(Dictionary<string, string> parameters)
		{
			var list = new List<string> ();

			foreach (var pair in parameters)
			{
				list.Add (pair.Key);
				list.Add (pair.Value);
			}

			this.article.ArticleParameters = string.Join (AbstractArticleParameterController.Separator, list);
		}

		private string Code
		{
			get
			{
				return this.ParameterDefinition.Code;
			}
		}

		protected AbstractArticleParameterDefinitionEntity ParameterDefinition
		{
			get
			{
				return this.article.ArticleDefinition.ArticleParameterDefinitions[this.parameterIndex];
			}
		}


		public const char				SeparatorChar	= (char) 0x25CA;					// '◊'
		public static readonly string	Separator		= SeparatorChar.ToString ();		// "◊"

		private readonly IArticleDefinitionParameters	article;
		private readonly int							parameterIndex;
		protected readonly TileContainer				tileContainer;
	}
}
