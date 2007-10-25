using Epsitec.Common.Widgets;
using Epsitec.Common.Support;
using Epsitec.Common.Drawing;
using Epsitec.Common.Types;

namespace Epsitec.Common.Designer.MyWidgets
{
	/// <summary>
	/// Widget permettant d'éditer un Caption.Type.
	/// </summary>
	public class TypeEditorNative : AbstractTypeEditor
	{
		public TypeEditorNative(Module module)
		{
			this.module = module;

			this.CreateStringLabeled(Res.Strings.Viewers.Types.Native.Type, this, out this.groupSystemType, out this.fieldSystemType);
			this.groupSystemType.Dock = DockStyle.StackBegin;
			this.groupSystemType.Margins = new Margins(0, 0, 0, 0);
			this.groupSystemType.ResetButton.Name = "SystemType";
			this.groupSystemType.ResetButton.Clicked += new MessageEventHandler(this.HandleResetButtonClicked);
			this.fieldSystemType.PreferredWidth = 400;
			this.fieldSystemType.EditionAccepted += new EventHandler(this.HandleTextFieldChanged);
		}

		
		protected override void Dispose(bool disposing)
		{
			if ( disposing )
			{
				this.groupSystemType.ResetButton.Clicked -= new MessageEventHandler(this.HandleResetButtonClicked);
				this.fieldSystemType.EditionAccepted -= new EventHandler(this.HandleTextFieldChanged);
			}
			
			base.Dispose(disposing);
		}


		public override string GetSummary()
		{
			//	Retourne le texte du résumé.
			System.Text.StringBuilder builder = new System.Text.StringBuilder();

			this.PutSummaryInitialise();
			this.PutSummaryValue(builder, Res.Strings.Viewers.Types.Summary.Native, this.TypeName);

			return builder.ToString();
		}


		public override void UpdateContent()
		{
			//	Met à jour le contenu de l'éditeur.
			this.ignoreChange = true;
			bool usesOriginalData;

			this.fieldSystemType.Text = this.TypeName;
			this.structuredData.GetValue(Support.Res.Fields.ResourceOtherType.SystemType, out usesOriginalData);
			this.ColorizeResetBox(this.groupSystemType, usesOriginalData);

			this.ignoreChange = false;
		}


		protected string TypeName
		{
			get
			{
				System.Type type = this.structuredData.GetValue(Support.Res.Fields.ResourceOtherType.SystemType) as System.Type;
				return AbstractType.GetSystemTypeNameFromSystemType(type);
			}

			set
			{
				System.Type type;
				try
				{
					type = AbstractType.GetSystemTypeFromSystemTypeName(value);
				}
				catch
				{
					type = null;
				}

				if (type == null)
				{
					this.module.DesignerApplication.DialogError(Res.Strings.Error.Native.Incorrect);
				}
				else
				{
					this.structuredData.SetValue(Support.Res.Fields.ResourceOtherType.SystemType, type);
				}
			}
		}


		private void HandleTextFieldChanged(object sender)
		{
			if (this.ignoreChange)
			{
				return;
			}

			if (sender == this.fieldSystemType)
			{
				this.TypeName = this.fieldSystemType.Text;
			}

			this.OnContentChanged();
			this.UpdateContent();
			this.module.AccessTypes.SetLocalDirty();
		}

		private void HandleResetButtonClicked(object sender, MessageEventArgs e)
		{
			AbstractButton button = sender as AbstractButton;

			if (button.Name == "SystemType")
			{
				this.ResetToOriginalValue(Support.Res.Fields.ResourceOtherType.SystemType);
			}

			this.OnContentChanged();
			this.UpdateContent();
			this.module.AccessTypes.SetLocalDirty();
		}


		protected ResetBox						groupSystemType;
		protected TextFieldEx					fieldSystemType;
	}
}
