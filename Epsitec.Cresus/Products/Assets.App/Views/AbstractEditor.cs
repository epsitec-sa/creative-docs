//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Common.Support;
using Epsitec.Common.Widgets;
using Epsitec.Cresus.Assets.App.Helpers;
using Epsitec.Cresus.Assets.Server.SimpleEngine;

namespace Epsitec.Cresus.Assets.App.Views
{
	public abstract class AbstractEditor
	{
		public AbstractEditor(DataAccessor accessor, BaseType baseType, bool isTimeless)
		{
			this.accessor   = accessor;
			this.baseType   = baseType;
			this.isTimeless = isTimeless;
		}


		public bool EditionDirty
		{
			get
			{
				return this.editionDirty;
			}
		}

		public virtual void CreateUI(Widget parent)
		{
		}

		public virtual void OpenMainPage(EventType eventType)
		{
		}


		protected void StartEdition(Guid objectGuid, Timestamp? timestamp)
		{
			bool changed = this.accessor.EditionAccessor.SaveObjectEdition ();

			this.accessor.EditionAccessor.StartObjectEdition (this.baseType, objectGuid, timestamp);

			if (this.editionDirty)
			{
				this.editionDirty = false;
				this.OnValueChanged (ObjectField.Unknown);
			}

			if (changed)
			{
				this.OnUpdateData ();
			}
		}

		protected void SetEditionDirty(ObjectField field)
		{
			if (!this.editionDirty)
			{
				this.editionDirty = true;
				this.OnValueChanged (field);
			}
		}


		#region Events handler
		private void OnValueChanged(ObjectField field)
		{
			this.ValueChanged.Raise (this, field);
		}

		public event EventHandler<ObjectField> ValueChanged;


		private void OnUpdateData()
		{
			this.UpdateData.Raise (this);
		}

		public event EventHandler UpdateData;
		#endregion


		protected readonly DataAccessor			accessor;
		protected readonly BaseType				baseType;
		protected readonly bool					isTimeless;

		private bool							editionDirty;
	}
}
