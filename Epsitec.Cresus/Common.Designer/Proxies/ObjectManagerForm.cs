﻿using System.Collections.Generic;
using Epsitec.Common.Drawing;
using Epsitec.Common.Support;
using Epsitec.Common.Widgets;

namespace Epsitec.Common.Designer.Proxies
{
	/// <summary>
	/// Cette classe gère les objets associés à un proxy de type Form.
	/// </summary>
	public class ObjectManagerForm : AbstractObjectManager
	{
		public ObjectManagerForm(object objectModifier) : base(objectModifier)
		{
		}

		public override List<AbstractValue> GetValues(Widget selectedObject)
		{
			//	Retourne la liste des valeurs nécessaires pour représenter un objet.
			List<AbstractValue> list = new List<AbstractValue>();

			if (this.ObjectModifier.IsField(selectedObject) ||
				this.ObjectModifier.IsCommand(selectedObject) ||
				this.ObjectModifier.IsBox(selectedObject) ||
				this.ObjectModifier.IsGlue(selectedObject))
			{
				this.AddValue(list, selectedObject, AbstractProxy.Type.FormColumnsRequired, Res.Captions.Form.ColumnsRequired.Id, 1, 10, 1, 1);
			}

			if (this.ObjectModifier.IsField(selectedObject) ||
				this.ObjectModifier.IsBox(selectedObject))
			{
				this.AddValue(list, selectedObject, AbstractProxy.Type.FormRowsRequired, Res.Captions.Form.RowsRequired.Id, 1, 10, 1, 1);
			}

			if (this.ObjectModifier.IsField(selectedObject) ||
				this.ObjectModifier.IsCommand(selectedObject) ||
				this.ObjectModifier.IsBox(selectedObject) ||
				this.ObjectModifier.IsGlue(selectedObject) ||
				this.ObjectModifier.IsTitle(selectedObject) ||
				this.ObjectModifier.IsLine(selectedObject))
			{
				this.AddValue(list, selectedObject, AbstractProxy.Type.FormPreferredWidth, Res.Captions.Form.PreferredWidth.Id, 1, 1000, 1, 1);
			}

			if (this.ObjectModifier.IsField(selectedObject) ||
				this.ObjectModifier.IsCommand(selectedObject) ||
				this.ObjectModifier.IsBox(selectedObject))
			{
				this.AddValue(list, selectedObject, AbstractProxy.Type.FormSeparatorBottom, Res.Captions.Form.SeparatorBottom.Id, Res.Types.FieldDescription.SeparatorType);
			}

			if (this.ObjectModifier.IsField(selectedObject) ||
				this.ObjectModifier.IsBox(selectedObject) ||
				this.ObjectModifier.IsGlue(selectedObject))
			{
				this.AddValue(list, selectedObject, AbstractProxy.Type.FormBackColor, Res.Captions.Form.BackColor.Id, Res.Types.FieldDescription.BackColorType);
			}

			if (this.ObjectModifier.IsField(selectedObject) ||
				this.ObjectModifier.IsBox(selectedObject) ||
				this.ObjectModifier.IsTitle(selectedObject) ||
				this.ObjectModifier.IsCommand(selectedObject))
			{
				this.AddValue(list, selectedObject, AbstractProxy.Type.FormLabelFontColor, Res.Captions.Form.FontColor.Id, Res.Types.FieldDescription.FontColorType);
				this.AddValue(list, selectedObject, AbstractProxy.Type.FormLabelFontFace, Res.Captions.Form.FontFace.Id, Res.Types.FieldDescription.FontFaceType);
				this.AddValue(list, selectedObject, AbstractProxy.Type.FormLabelFontStyle, Res.Captions.Form.FontStyle.Id, Res.Types.FieldDescription.FontStyleType);
				this.AddValue(list, selectedObject, AbstractProxy.Type.FormLabelFontSize, Res.Captions.Form.FontSize.Id, Res.Types.FieldDescription.FontSizeType);
			}

			if (this.ObjectModifier.IsField(selectedObject) ||
				this.ObjectModifier.IsBox(selectedObject))
			{
				this.AddValue(list, selectedObject, AbstractProxy.Type.FormFieldFontColor, Res.Captions.Form.FontColor.Id, Res.Types.FieldDescription.FontColorType);
				this.AddValue(list, selectedObject, AbstractProxy.Type.FormFieldFontFace, Res.Captions.Form.FontFace.Id, Res.Types.FieldDescription.FontFaceType);
				this.AddValue(list, selectedObject, AbstractProxy.Type.FormFieldFontStyle, Res.Captions.Form.FontStyle.Id, Res.Types.FieldDescription.FontStyleType);
				this.AddValue(list, selectedObject, AbstractProxy.Type.FormFieldFontSize, Res.Captions.Form.FontSize.Id, Res.Types.FieldDescription.FontSizeType);
			}

			if (this.ObjectModifier.IsBox(selectedObject))
			{
				this.AddValue(list, selectedObject, AbstractProxy.Type.FormBoxLayout, Res.Captions.Form.BoxLayout.Id, Res.Types.FieldDescription.BoxLayoutType);
				this.AddValue(list, selectedObject, AbstractProxy.Type.FormBoxPadding, Res.Captions.Form.BoxPadding.Id, Res.Types.FieldDescription.BoxPaddingType);
				this.AddValue(list, selectedObject, AbstractProxy.Type.FormBoxFrameState, Res.Captions.Form.BoxFrameState.Id, Res.Types.FieldDescription.FrameState);
				this.AddValue(list, selectedObject, AbstractProxy.Type.FormBoxFrameWidth, Res.Captions.Form.BoxFrameWidth.Id, 0, 5, 1, 1);
			}

			if (this.ObjectModifier.IsCommand(selectedObject))
			{
				this.AddValue(list, selectedObject, AbstractProxy.Type.FormButtonClass, Res.Captions.Form.CommandButtonClass.Id, Res.Types.FieldDescription.CommandButtonClass);
			}

			return list;
		}

		public override bool IsEnable(AbstractValue value)
		{
			//	Indique si la valeur pour représenter un objet est enable.
			if (value.Type == AbstractProxy.Type.FormPreferredWidth)
			{
				foreach (Widget widget in value.SelectedObjects)
				{
					//	En cas de sélection multiple, il suffit qu'un seul objet ne soit pas dans une grille
					//	pour que la valeur FormPreferredWidth soit enable.
					if (!(widget.Parent is UI.Panel))
					{
						return true;
					}
				}
				return false;
			}

			return true;
		}


		protected void AddValue(List<AbstractValue> list, Widget selectedObject, AbstractProxy.Type type, Druid captionId, double min, double max, double step, double resolution)
		{
			this.SuspendChanges();

			try
			{
				ValueNumeric value = new ValueNumeric(min, max, step, resolution);
				value.SelectedObjects.Add(selectedObject);
				value.Type = type;
				value.CaptionId = captionId;
				value.ValueChanged += new EventHandler(this.HandleValueChanged);
				this.SendObjectToValue(value);

				list.Add(value);
			}
			finally
			{
				this.ResumeChanges();
			}
		}

		protected void AddValue(List<AbstractValue> list, Widget selectedObject, AbstractProxy.Type type, Druid captionId, Types.EnumType enumType)
		{
			this.SuspendChanges();

			try
			{
				ValueEnum value = new ValueEnum(enumType);
				value.SelectedObjects.Add(selectedObject);
				value.Type = type;
				value.CaptionId = captionId;
				value.ValueChanged += new EventHandler(this.HandleValueChanged);
				this.SendObjectToValue(value);

				list.Add(value);
			}
			finally
			{
				this.ResumeChanges();
			}
		}

		private void HandleValueChanged(object sender)
		{
			if (this.IsNotSuspended)
			{
				AbstractValue value = sender as AbstractValue;
				this.SendValueToObject(value);
			}
		}


		protected void SendObjectToValue(AbstractValue value)
		{
			//	Tous les objets ont la même valeur. Il suffit donc de s'occuper du premier objet.
			Widget selectedObject = value.SelectedObjects[0];

			switch (value.Type)
			{
				case AbstractProxy.Type.FormColumnsRequired:
					value.Value = this.ObjectModifier.GetColumnsRequired(selectedObject);
					break;

				case AbstractProxy.Type.FormRowsRequired:
					value.Value = this.ObjectModifier.GetRowsRequired(selectedObject);
					break;

				case AbstractProxy.Type.FormPreferredWidth:
					value.Value = this.ObjectModifier.GetPreferredWidth(selectedObject);
					break;

				case AbstractProxy.Type.FormSeparatorBottom:
					value.Value = this.ObjectModifier.GetSeparatorBottom(selectedObject);
					break;

				case AbstractProxy.Type.FormBackColor:
					value.Value = this.ObjectModifier.GetBackColor(selectedObject);
					break;

				case AbstractProxy.Type.FormLabelFontColor:
					value.Value = this.ObjectModifier.GetLabelFontColor(selectedObject);
					break;

				case AbstractProxy.Type.FormFieldFontColor:
					value.Value = this.ObjectModifier.GetFieldFontColor(selectedObject);
					break;

				case AbstractProxy.Type.FormLabelFontFace:
					value.Value = this.ObjectModifier.GetLabelFontFace(selectedObject);
					break;

				case AbstractProxy.Type.FormFieldFontFace:
					value.Value = this.ObjectModifier.GetFieldFontFace(selectedObject);
					break;

				case AbstractProxy.Type.FormLabelFontStyle:
					value.Value = this.ObjectModifier.GetLabelFontStyle(selectedObject);
					break;

				case AbstractProxy.Type.FormFieldFontStyle:
					value.Value = this.ObjectModifier.GetFieldFontStyle(selectedObject);
					break;

				case AbstractProxy.Type.FormLabelFontSize:
					value.Value = this.ObjectModifier.GetLabelFontSize(selectedObject);
					break;

				case AbstractProxy.Type.FormFieldFontSize:
					value.Value = this.ObjectModifier.GetFieldFontSize(selectedObject);
					break;

				case AbstractProxy.Type.FormBoxLayout:
					value.Value = this.ObjectModifier.GetBoxLayout(selectedObject);
					break;

				case AbstractProxy.Type.FormBoxPadding:
					value.Value = this.ObjectModifier.GetBoxPadding(selectedObject);
					break;

				case AbstractProxy.Type.FormBoxFrameState:
					value.Value = this.ObjectModifier.GetBoxFrameState(selectedObject);
					break;

				case AbstractProxy.Type.FormBoxFrameWidth:
					value.Value = this.ObjectModifier.GetBoxFrameWidth(selectedObject);
					break;

				case AbstractProxy.Type.FormButtonClass:
					value.Value = this.ObjectModifier.GetCommandButtonClass(selectedObject);
					break;
			}
		}

		protected void SendValueToObject(AbstractValue value)
		{
			//	Il faut envoyer la valeur à tous les objets sélectionnés.
			foreach (Widget selectedObject in value.SelectedObjects)
			{
				switch (value.Type)
				{
					case AbstractProxy.Type.FormColumnsRequired:
						this.ObjectModifier.SetColumnsRequired(selectedObject, (int) value.Value);
						break;

					case AbstractProxy.Type.FormRowsRequired:
						this.ObjectModifier.SetRowsRequired(selectedObject, (int) value.Value);
						break;

					case AbstractProxy.Type.FormPreferredWidth:
						this.ObjectModifier.SetPreferredWidth(selectedObject, (double) value.Value);
						break;

					case AbstractProxy.Type.FormSeparatorBottom:
						this.ObjectModifier.SetSeparatorBottom(selectedObject, (FormEngine.FieldDescription.SeparatorType) value.Value);
						break;

					case AbstractProxy.Type.FormBackColor:
						this.ObjectModifier.SetBackColor(selectedObject, (FormEngine.FieldDescription.BackColorType) value.Value);
						break;

					case AbstractProxy.Type.FormLabelFontColor:
						this.ObjectModifier.SetLabelFontColor(selectedObject, (FormEngine.FieldDescription.FontColorType) value.Value);
						break;

					case AbstractProxy.Type.FormFieldFontColor:
						this.ObjectModifier.SetFieldFontColor(selectedObject, (FormEngine.FieldDescription.FontColorType) value.Value);
						break;

					case AbstractProxy.Type.FormLabelFontFace:
						this.ObjectModifier.SetLabelFontFace(selectedObject, (FormEngine.FieldDescription.FontFaceType) value.Value);
						break;

					case AbstractProxy.Type.FormFieldFontFace:
						this.ObjectModifier.SetFieldFontFace(selectedObject, (FormEngine.FieldDescription.FontFaceType) value.Value);
						break;

					case AbstractProxy.Type.FormLabelFontStyle:
						this.ObjectModifier.SetLabelFontStyle(selectedObject, (FormEngine.FieldDescription.FontStyleType) value.Value);
						break;

					case AbstractProxy.Type.FormFieldFontStyle:
						this.ObjectModifier.SetFieldFontStyle(selectedObject, (FormEngine.FieldDescription.FontStyleType) value.Value);
						break;

					case AbstractProxy.Type.FormLabelFontSize:
						this.ObjectModifier.SetLabelFontSize(selectedObject, (FormEngine.FieldDescription.FontSizeType) value.Value);
						break;

					case AbstractProxy.Type.FormFieldFontSize:
						this.ObjectModifier.SetFieldFontSize(selectedObject, (FormEngine.FieldDescription.FontSizeType) value.Value);
						break;

					case AbstractProxy.Type.FormBoxLayout:
						this.ObjectModifier.SetBoxLayout(selectedObject, (FormEngine.FieldDescription.BoxLayoutType) value.Value);
						break;

					case AbstractProxy.Type.FormBoxPadding:
						this.ObjectModifier.SetBoxPadding(selectedObject, (FormEngine.FieldDescription.BoxPaddingType) value.Value);
						break;

					case AbstractProxy.Type.FormBoxFrameState:
						this.ObjectModifier.SetBoxFrameState(selectedObject, (FrameState) value.Value);
						break;

					case AbstractProxy.Type.FormBoxFrameWidth:
						this.ObjectModifier.SetBoxFrameWidth(selectedObject, (double) value.Value);
						break;

					case AbstractProxy.Type.FormButtonClass:
						this.ObjectModifier.SetCommandButtonClass(selectedObject, (FormEngine.FieldDescription.CommandButtonClass) value.Value);
						break;
				}
			}

			this.Viewer.ProxyManager.UpdateInterface();
			Application.QueueAsyncCallback(this.ObjectModifier.FormEditor.RegenerateForm);
			this.ObjectModifier.FormEditor.Module.AccessForms.SetLocalDirty();
		}


		protected Viewers.Forms Viewer
		{
			get
			{
				return this.ObjectModifier.FormEditor.ViewersForms;
			}
		}

		protected FormEditor.ObjectModifier ObjectModifier
		{
			get
			{
				return this.objectModifier as FormEditor.ObjectModifier;
			}
		}
	}
}
