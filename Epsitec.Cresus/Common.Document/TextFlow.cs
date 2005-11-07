using Epsitec.Common.Support;
using Epsitec.Common.Widgets;
using Epsitec.Common.Text;
using System.Runtime.Serialization;

namespace Epsitec.Common.Document
{
	/// <summary>
	/// Flux de texte.
	/// </summary>
	[System.Serializable()]
	public class TextFlow : ISerializable
	{
		// Cr�e un nouveau flux pour un seul pav�.
		public TextFlow(Document document)
		{
			this.document = document;

			this.Initialise();
			this.objectsChain = new UndoableList(this.document, UndoableListType.ObjectsChain);
		}

		protected void Initialise()
		{
			if ( this.document.Modifier == null )
			{
				this.textStory = new Text.TextStory(this.document.TextContext);
			}
			else
			{
				this.textStory = new Text.TextStory(this.document.Modifier.OpletQueue, this.document.TextContext);
			}

			this.textFitter    = new Text.TextFitter(this.textStory);
			this.textNavigator = new Text.TextNavigator(this.textFitter);
			
			this.textStory.TextContext.IsDegradedLayoutEnabled = true;
			this.textStory.DebugDisableOpletQueue = true;
			this.textNavigator.Insert(Text.Unicode.Code.EndOfText);
			this.textNavigator.MoveTo(Text.TextNavigator.Target.TextStart, 0);
			this.textStory.DebugDisableOpletQueue = false;
		}


		public Text.TextStory TextStory
		{
			get { return this.textStory; }
		}

		public Text.TextFitter TextFitter
		{
			get { return this.textFitter; }
		}

		public Text.TextNavigator TextNavigator
		{
			get { return this.textNavigator; }
		}


		// Ajoute un pav� de texte dans la cha�ne de l'objet parent.
		// Si le pav� obj fait d�j� partie d'une autre cha�ne, toute sa cha�ne est ajout�e
		// dans la cha�ne de l'objet parent.
		public void Add(Objects.TextBox2 obj, Objects.TextBox2 parent, bool after)
		{
			if ( parent == null )
			{
				this.objectsChain.Add(obj);
				this.TextFitter.FrameList.Add(obj.TextFrame);
			}
			else
			{
				System.Diagnostics.Debug.Assert(parent.TextFlow == this);

				// Si l'objet � ajouter dans la cha�ne y est d�j�, mais ailleurs,
				// il faut d'abord l'enlever.
				if ( this.objectsChain.Contains(obj) )
				{
					obj.TextFlow.Remove(obj);
				}

				int index = this.objectsChain.IndexOf(parent);
				System.Diagnostics.Debug.Assert(index != -1);

				// Met dans la liste srcChain tous les objets faisant partie de la cha�ne
				// de l'objet � ajouter (il peut faire partie d'une autre cha�ne).
				TextFlow srcFlow = obj.TextFlow;
				System.Collections.ArrayList srcChain = new System.Collections.ArrayList();
				foreach ( Objects.TextBox2 listObj in srcFlow.Chain )
				{
					srcChain.Add(listObj);
				}

				foreach ( Objects.TextBox2 listObj in srcChain )
				{
					listObj.TextFlow.Remove(listObj);
				}

				this.MergeWith(srcFlow);  // cha�ne parent <- texte source
				this.document.TextFlows.Remove(srcFlow);

				// Ajoute toute la cha�ne initiale � la cha�ne de l'objet parent.
				if ( after )  index ++;
				foreach ( Objects.TextBox2 listObj in srcChain )
				{
					this.objectsChain.Insert(index, listObj);
					this.TextFitter.FrameList.InsertAt(index, listObj.TextFrame);

					listObj.TextFlow = this;

					index ++;
				}
			}
		}

		// Fusionne le texte d'un flux source � la fin du flux courant.
		// TODO: g�rer les tabulateurs, les styles, etc.
		public void MergeWith(TextFlow src)
		{
			src.textNavigator.MoveTo(Text.TextNavigator.Target.TextStart, 1);
			src.textNavigator.StartSelection();
			src.textNavigator.MoveTo(Text.TextNavigator.Target.TextEnd, 1);
			src.textNavigator.EndSelection();
			string[] texts = src.textNavigator.GetSelectedTexts();

			this.textNavigator.MoveTo(Text.TextNavigator.Target.TextEnd, 1);
			this.textNavigator.Insert(texts[0]);
		}

		// Supprime un pav� de texte de la cha�ne. Le pav� sera alors solitaire.
		// Le texte lui-m�me reste dans la cha�ne initiale.
		public void Remove(Objects.TextBox2 obj)
		{
			if ( this.objectsChain.Count == 1 )  return;

			this.objectsChain.Remove(obj);
			this.TextFitter.FrameList.Remove(obj.TextFrame);

			obj.UpdateTextLayout();
			obj.NotifyAreaFlow();

			obj.NewTextFlow();
		}

		// Nombre de pav�s de texte dans la cha�ne.
		public int Count
		{
			get
			{
				return this.objectsChain.Count;
			}
		}

		// Rang d'un pav� de texte dans la cha�ne.
		public int Rank(Objects.Abstract obj)
		{
			return this.objectsChain.IndexOf(obj);
		}

		// Donne la cha�ne des pav�s de texte.
		public UndoableList Chain
		{
			get
			{
				return this.objectsChain;
			}
		}


		#region Serialization
		// S�rialise l'objet.
		public virtual void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			info.AddValue("ObjectsChain", this.objectsChain);

			byte[] textStoryData = this.textStory.Serialize();
			info.AddValue("TextStoryData", textStoryData);
		}

		// Constructeur qui d�s�rialise l'objet.
		protected TextFlow(SerializationInfo info, StreamingContext context)
		{
			this.document = Document.ReadDocument;

			this.objectsChain = (UndoableList) info.GetValue("ObjectsChain", typeof(UndoableList));
			this.textStoryData = (byte[]) info.GetValue("TextStoryData", typeof(byte[]));
		}

		// Adapte l'objet apr�s une d�s�rialisation.
		public void ReadFinalize()
		{
			this.Initialise();
			this.textStory.Deserialize(this.textStoryData);
			this.textStoryData = null;
			
			System.Diagnostics.Debug.Assert(this.TextFitter.FrameList.Count == 0);
			foreach ( Objects.TextBox2 obj in this.objectsChain )
			{
				System.Diagnostics.Debug.Assert(obj.TextFrame != null);
				this.textFitter.FrameList.Add(obj.TextFrame);
				obj.ReadFinalizeFlowReady(this);
			}

			this.textFitter.ClearAllMarks();
			this.textFitter.GenerateAllMarks();
		}
		#endregion

		
		protected Document						document;
		protected byte[]						textStoryData;
		protected Text.TextStory				textStory;
		protected Text.TextFitter				textFitter;
		protected Text.TextNavigator			textNavigator;
		protected UndoableList					objectsChain;
	}
}
