//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using Epsitec.Common.Types;
using Epsitec.Common.Support.EntityEngine;

using Epsitec.Cresus.Compta.Controllers;
using Epsitec.Cresus.Compta.Entities;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Compta.Accessors
{
	/// <summary>
	/// Données éditables génériques d'un champ de la comptabilité.
	/// </summary>
	public class EditionData
	{
		public EditionData(AbstractController controller, System.Action<EditionData> validateAction = null)
		{
			this.controller     = controller;
			this.validateAction = validateAction;

			this.compta  = this.controller.ComptaEntity;
			this.période = this.controller.PériodeEntity;

			this.parameters = new List<FormattedText> ();
			this.texts = new List<FormattedText> ();

			this.Enable = true;
		}


		public void Validate()
		{
			//	Valide le contenu, en l'adaptant éventuellement.
			if (this.validateAction != null)
			{
				this.validateAction (this);
			}
		}


		public bool Enable
		{
			set;
			get;
		}


		public bool HasText
		{
			get
			{
				return !this.text.IsNullOrEmpty;
			}
		}

		public FormattedText Text
		{
			get
			{
				return this.text;
			}
			set
			{
				this.text = value;
			}
		}

		public List<FormattedText> Parameters
		{
			get
			{
				return this.parameters;
			}
		}

		public List<FormattedText> Texts
		{
			get
			{
				return this.texts;
			}
		}


		public void ClearError()
		{
			this.error = FormattedText.Null;
		}

		public bool HasError
		{
			get
			{
				return !this.error.IsNullOrEmpty;
			}
		}

		public FormattedText Error
		{
			get
			{
				return this.error;
			}
			set
			{
				this.error = value;
			}
		}


		private readonly AbstractController					controller;
		private readonly System.Action<EditionData>			validateAction;
		private readonly ComptaEntity						compta;
		private readonly ComptaPériodeEntity				période;
		private readonly List<FormattedText>				parameters;
		private readonly List<FormattedText>				texts;

		private FormattedText								text;
		private FormattedText								error;
	}
}