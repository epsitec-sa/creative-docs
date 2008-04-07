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
				this.AddValue(list, selectedObject, AbstractProxy.Type.PanelLeftMargin,   Res.Captions.Geometry.LeftMargin,   -1, 9999, 1, 1);
				this.AddValue(list, selectedObject, AbstractProxy.Type.PanelRightMargin,  Res.Captions.Geometry.RightMargin,  -1, 9999, 1, 1);
				this.AddValue(list, selectedObject, AbstractProxy.Type.PanelTopMargin,    Res.Captions.Geometry.TopMargin,    -1, 9999, 1, 1);
				this.AddValue(list, selectedObject, AbstractProxy.Type.PanelBottomMargin, Res.Captions.Geometry.BottomMargin, -1, 9999, 1, 1);
			}

			if (this.ObjectModifier.HasBounds(selectedObject))
			{
				if (this.ObjectModifier.HasBounds(selectedObject, PanelEditor.ObjectModifier.BoundsMode.OriginX))
				{
					this.AddValue(list, selectedObject, AbstractProxy.Type.PanelOriginX, Res.Captions.Geometry.OriginX, -9999, 9999, 1, 1);
				}

				if (this.ObjectModifier.HasBounds(selectedObject, PanelEditor.ObjectModifier.BoundsMode.OriginY))
				{
					this.AddValue(list, selectedObject, AbstractProxy.Type.PanelOriginY, Res.Captions.Geometry.OriginX, -9999, 9999, 1, 1);
				}

				if (this.ObjectModifier.HasBounds(selectedObject, PanelEditor.ObjectModifier.BoundsMode.Width))
				{
					this.AddValue(list, selectedObject, AbstractProxy.Type.PanelWidth, Res.Captions.Geometry.Width, 0, 9999, 1, 1);
				}

				if (this.ObjectModifier.HasBounds(selectedObject, PanelEditor.ObjectModifier.BoundsMode.Height))
				{
					this.AddValue(list, selectedObject, AbstractProxy.Type.PanelHeight, Res.Captions.Geometry.Height, 0, 9999, 1, 1);
				}
			}
			else
			{
				if (this.ObjectModifier.HasWidth(selectedObject))
				{
					this.AddValue(list, selectedObject, AbstractProxy.Type.PanelWidth, Res.Captions.Geometry.Width, 0, 9999, 1, 1);
				}

				if (this.ObjectModifier.HasHeight(selectedObject))
				{
					this.AddValue(list, selectedObject, AbstractProxy.Type.PanelHeight, Res.Captions.Geometry.Height, 0, 9999, 1, 1);
				}
			}

			return list;
		}

		public override bool IsEnable(AbstractValue value)
		{
			//	Indique si la valeur pour représenter un objet est enable.
			return true;
		}


		public override void SendObjectToValue(AbstractValue value)
		{
			//	Tous les objets ont la même valeur. Il suffit donc de s'occuper du premier objet.
			Widget selectedObject = value.SelectedObjects[0];

			switch (value.Type)
			{
				case AbstractProxy.Type.PanelLeftMargin:
					value.Value = this.ObjectModifier.GetMargins(selectedObject).Left;
					break;

				case AbstractProxy.Type.PanelRightMargin:
					value.Value = this.ObjectModifier.GetMargins(selectedObject).Right;
					break;

				case AbstractProxy.Type.PanelTopMargin:
					value.Value = this.ObjectModifier.GetMargins(selectedObject).Top;
					break;

				case AbstractProxy.Type.PanelBottomMargin:
					value.Value = this.ObjectModifier.GetMargins(selectedObject).Bottom;
					break;

				case AbstractProxy.Type.PanelOriginX:
					value.Value = this.ObjectModifier.GetBounds(selectedObject).Left;
					break;

				case AbstractProxy.Type.PanelOriginY:
					value.Value = this.ObjectModifier.GetBounds(selectedObject).Bottom;
					break;

				case AbstractProxy.Type.PanelWidth:
					if (this.ObjectModifier.HasBounds(selectedObject))
					{
						value.Value = this.ObjectModifier.GetBounds(selectedObject).Width;
					}
					else
					{
						value.Value = this.ObjectModifier.GetWidth(selectedObject);
					}
					break;

				case AbstractProxy.Type.PanelHeight:
					if (this.ObjectModifier.HasBounds(selectedObject))
					{
						value.Value = this.ObjectModifier.GetBounds(selectedObject).Height;
					}
					else
					{
						value.Value = this.ObjectModifier.GetHeight(selectedObject);
					}
					break;

			}
		}

		protected override void SendValueToObject(AbstractValue value)
		{
			//	Il faut envoyer la valeur à tous les objets sélectionnés.
			foreach (Widget selectedObject in value.SelectedObjects)
			{
				Margins m;
				Rectangle r;

				switch (value.Type)
				{
					case AbstractProxy.Type.PanelLeftMargin:
						m = this.ObjectModifier.GetMargins(selectedObject);
						m.Left = (double) value.Value;
						this.ObjectModifier.SetMargins(selectedObject, m);
						break;

					case AbstractProxy.Type.PanelRightMargin:
						m = this.ObjectModifier.GetMargins(selectedObject);
						m.Right = (double) value.Value;
						this.ObjectModifier.SetMargins(selectedObject, m);
						break;

					case AbstractProxy.Type.PanelTopMargin:
						m = this.ObjectModifier.GetMargins(selectedObject);
						m.Top = (double) value.Value;
						this.ObjectModifier.SetMargins(selectedObject, m);
						break;

					case AbstractProxy.Type.PanelBottomMargin:
						m = this.ObjectModifier.GetMargins(selectedObject);
						m.Bottom = (double) value.Value;
						this.ObjectModifier.SetMargins(selectedObject, m);
						break;

					case AbstractProxy.Type.PanelOriginX:
						r = this.ObjectModifier.GetBounds(selectedObject);
						r.Left = (double) value.Value;
						this.ObjectModifier.SetBounds(selectedObject, r);
						break;

					case AbstractProxy.Type.PanelOriginY:
						r = this.ObjectModifier.GetBounds(selectedObject);
						r.Bottom = (double) value.Value;
						this.ObjectModifier.SetBounds(selectedObject, r);
						break;

					case AbstractProxy.Type.PanelWidth:
						if (this.ObjectModifier.HasBounds(selectedObject))
						{
							r = this.ObjectModifier.GetBounds(selectedObject);
							r.Width = (double) value.Value;
							this.ObjectModifier.SetBounds(selectedObject, r);
						}
						else
						{
							this.ObjectModifier.SetWidth(selectedObject, (double) value.Value);
						}
						break;

					case AbstractProxy.Type.PanelHeight:
						if (this.ObjectModifier.HasBounds(selectedObject))
						{
							r = this.ObjectModifier.GetBounds(selectedObject);
							r.Height = (double) value.Value;
							this.ObjectModifier.SetBounds(selectedObject, r);
						}
						else
						{
							this.ObjectModifier.SetHeight(selectedObject, (double) value.Value);
						}
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
