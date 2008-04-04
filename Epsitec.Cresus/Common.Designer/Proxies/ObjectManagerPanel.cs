using System.Collections.Generic;
using Epsitec.Common.Drawing;
using Epsitec.Common.Support;
using Epsitec.Common.Widgets;

namespace Epsitec.Common.Designer.Proxies
{
	/// <summary>
	/// Cette classe gère les objets associés à un proxy de type Panel.
	/// </summary>
	public class ObjectManagerPanel : AbstractObjectManager
	{
		public ObjectManagerPanel(object objectModifier) : base(objectModifier)
		{
		}

		public override List<AbstractValue> GetValues(Widget selectedObject)
		{
			//	Retourne la liste des valeurs nécessaires pour représenter un objet.
			List<AbstractValue> list = new List<AbstractValue>();

			if (this.ObjectModifier.HasMargins(selectedObject))
			{
				this.AddValue(list, selectedObject, AbstractProxy.Type.PanelLeftMargin, Res.Captions.Form.ColumnsRequired, -1, 9999, 1, 1);
			}

			return list;
		}

		public override bool IsEnable(AbstractValue value)
		{
			//	Indique si la valeur pour représenter un objet est enable.
			return true;
		}


		protected override void SendObjectToValue(AbstractValue value)
		{
			//	Tous les objets ont la même valeur. Il suffit donc de s'occuper du premier objet.
			Widget selectedObject = value.SelectedObjects[0];

			switch (value.Type)
			{
				case AbstractProxy.Type.PanelLeftMargin:
					value.Value = this.ObjectModifier.GetMargins(selectedObject).Left;
					break;
			}
		}

		protected override void SendValueToObject(AbstractValue value)
		{
			//	Il faut envoyer la valeur à tous les objets sélectionnés.
			foreach (Widget selectedObject in value.SelectedObjects)
			{
				switch (value.Type)
				{
					case AbstractProxy.Type.PanelLeftMargin:
						Margins m = this.ObjectModifier.GetMargins(selectedObject);
						m.Left = (double) value.Value;
						this.ObjectModifier.SetMargins(selectedObject, m);
						break;
				}
			}

			this.Viewer.ProxyManager.UpdateInterface();
			Application.QueueAsyncCallback(this.ObjectModifier.PanelEditor.RegenerateDimensions);
			this.ObjectModifier.PanelEditor.Module.AccessPanels.SetLocalDirty();
		}


		protected Viewers.Panels Viewer
		{
			get
			{
				return this.ObjectModifier.PanelEditor.ViewersPanels;
			}
		}

		protected PanelEditor.ObjectModifier ObjectModifier
		{
			get
			{
				return this.objectModifier as PanelEditor.ObjectModifier;
			}
		}
	}
}
