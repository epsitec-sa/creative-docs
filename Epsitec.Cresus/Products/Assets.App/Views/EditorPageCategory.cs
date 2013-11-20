﻿//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Common.Widgets;
using Epsitec.Cresus.Assets.Server.BusinessLogic;
using Epsitec.Cresus.Assets.Server.SimpleEngine;

namespace Epsitec.Cresus.Assets.App.Views
{
	public class EditorPageCategory : AbstractEditorPage
	{
		public EditorPageCategory(DataAccessor accessor, BaseType baseType, bool isTimeless)
			: base (accessor, baseType, isTimeless)
		{
		}


		public override void CreateUI(Widget parent)
		{
			this.CreateStringController  (parent, ObjectField.Numéro, editWidth: 90);
			this.CreateStringController  (parent, ObjectField.Nom);
			this.CreateStringController  (parent, ObjectField.Description, lineCount: 5);
			this.CreateDecimalController (parent, ObjectField.TauxAmortissement, DecimalFormat.Rate);
			this.CreateStringController  (parent, ObjectField.TypeAmortissement, editWidth: 90);
			this.CreateStringController  (parent, ObjectField.Périodicité, editWidth: 90);
			this.CreateDecimalController (parent, ObjectField.ValeurRésiduelle, DecimalFormat.Amount);

			new FrameBox
			{
				Parent          = parent,
				Dock            = DockStyle.Top,
				PreferredHeight = 10,
			};

			this.CreateStringController (parent, ObjectField.Compte1);
			this.CreateStringController (parent, ObjectField.Compte2);
			this.CreateStringController (parent, ObjectField.Compte3);
			this.CreateStringController (parent, ObjectField.Compte4);
			this.CreateStringController (parent, ObjectField.Compte5);
			this.CreateStringController (parent, ObjectField.Compte6);
			this.CreateStringController (parent, ObjectField.Compte7);
			this.CreateStringController (parent, ObjectField.Compte8);
		}
	}
}
