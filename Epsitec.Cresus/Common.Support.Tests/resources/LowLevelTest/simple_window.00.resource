<bundle>
 <data name='class'>Window</data>
 <data name='Name'>my_window</data>
 <data name='Text'><ref target='strings#title.SettingsWindow'/></data>
 <data name='Size'>240;288</data>
 <data name='Icon'>file:icon</data>
 <list name='widgets'>
  <bundle>									<!-- Définition du menu -->
   <data name='class'>HMenu</data>
   <data name='Name'>main_menu</data>
   <data name='Dock'>Top</data>
   <list name='items'>
    <bundle>
     <data name='class'>MenuItem</data>
     <data name='Text'>Fichier</data>
     <bundle name='Submenu'>
      <data name='class'>VMenu</data>
      <list name='items'>
       <bundle>
        <data name='class'>MenuItem</data>
        <data name='Text'>Ouvrir..</data>
        <data name='Icon'>res:image#open</data>
        <data name='Name'>open</data>
        <data name='Command'>OpenSimpleWindow</data>
       </bundle>
       <bundle>
        <data name='class'>MenuItem</data>
        <data name='Text'>Enregistrer..</data>
        <data name='Icon'>res:image#save</data>
        <data name='Name'>save</data>
        <data name='Command'>SaveSimpleWindow</data>
       </bundle>
       <bundle>
        <data name='class'>MenuSeparator</data>
       </bundle>
       <bundle>
        <data name='class'>MenuItem</data>
        <data name='Text'>Quitter</data>
        <data name='Name'>quit</data>
        <data name='Command'>QuitSimpleWindow</data>
       </bundle>
      </list>
     </bundle>
    </bundle>
   </list>
  </bundle>
  
  <bundle>									<!-- Définition de la barre d'icônes -->
   <data name='class'>HToolBar</data>
   <data name='Dock'>Top</data>
   <list name='items'>
    <bundle>
     <data name='class'>IconButton</data>
     <data name='Icon'>file:images/open.png</data>
    </bundle>
    <bundle>
     <data name='class'>IconButton</data>
     <data name='Icon'>file:images/save.png</data>
    </bundle>
    <bundle>
     <data name='class'>IconSeparator</data>
    </bundle>
    <bundle>
     <data name='class'>TextFieldCombo</data>
     <data name='IsReadOnly'>true</data>
     <data name='Size'>80;*</data>
     <bundle name='items'>
      <data name='00/Red'>Rouge</data>
      <data name='01/Green'>Vert</data>
      <data name='02/Blue'>Bleu</data>
     </bundle>
    </bundle>
    <bundle>
     <data name='class'>IconSeparator</data>
    </bundle>
    <bundle>
     <data name='class'>IconButton</data>
     <data name='Icon'>file:images/cut.png</data>
    </bundle>
    <bundle>
     <data name='class'>IconButton</data>
     <data name='Icon'>file:images/copy.png</data>
    </bundle>
    <bundle>
     <data name='class'>IconButton</data>
     <data name='Icon'>file:images/paste.png</data>
    </bundle>
   </list>
  </bundle>
  
  <bundle>
   <data name='class'>Widget</data>
   <data name='Dock'>Fill</data>
   <list name='widgets'>
    <bundle>
     <data name='class'>CheckButton</data>
     <data name='Text'><![CDATA[Pour rire, voici une icone&#160;:&#160;<img src="res:image#open"/>]]></data>
     <data name='Name'>A</data>
     <data name='Size'>200;*</data>
     <data name='Location'>10;215</data>
     <data name='bind'>a;a#label</data>
    </bundle>
    <bundle>
     <data name='class'>Button</data>
     <data name='Text'><![CDATA[<img src="res:file:image#back"/>]]></data>
     <data name='Size'>40;40</data>
     <data name='Location'>10;165</data>
    </bundle>
    <bundle>
     <data name='class'>Button</data>
     <data name='Text'><![CDATA[<img src="res:file:image#forward"/>]]></data>
     <data name='Size'>40;40</data>
     <data name='Location'>50;165</data>
    </bundle>
    <bundle>
     <data name='class'>TextField</data>
     <data name='Size'>80;*</data>
     <data name='Location'>100;184</data>
    </bundle>
    <bundle>
     <data name='class'>TextFieldCombo</data>
     <data name='IsReadOnly'>true</data>
     <data name='Size'>80;*</data>
     <data name='Location'>100;164</data>
     <bundle name='items'>
      <data name='00/SelectA'>Choix A</data>
      <data name='01/SelectB'>Choix B</data>
      <data name='02/BestSelectC'><![CDATA[Choix <b>C</b> <i>(meilleur de tous)</i>]]></data>
      <data name='03/SelectD'>Choix D</data>
     </bundle>
    </bundle>
    <bundle>
     <data name='class'>CheckButton</data>
     <data name='Text'><![CDATA[Copie du premier bouton]]></data>
     <data name='Name'>B</data>
     <data name='Size'>200;*</data>
     <data name='Location'>10;140</data>
     <data name='bind'>b;b#label</data>
    </bundle>
    <bundle>
     <data name='class'>GroupBox</data>
     <data name='Anchor'>Left, Right, Top, Bottom</data>
     <data name='Margins'>10;10;100;50</data>
     <data name='Text'><![CDATA[Choix <i>cornélien</i>...]]></data>
     <data name='DockH'>no</data>
     <data name='Padding'>5;5;5;5</data>
     <list name='widgets'>
      <bundle>
       <data name='class'>RadioButton</data>
       <data name='Text'>Bouton B</data>
       <data name='Dock'>Top</data>
      </bundle>
      <bundle>
       <data name='class'>RadioButton</data>
       <data name='Text'>Bouton C</data>
       <data name='Dock'>Top</data>
      </bundle>
      <bundle>
       <data name='class'>RadioButton</data>
       <data name='Text'>Bouton D</data>
       <data name='Dock'>Top</data>
      </bundle>
     </list>
    </bundle>
    <bundle>
     <ref target='button.cancel'/>
     <data name='Anchor'>Bottom, Left</data>
     <data name='Margins'>10;0;0;10</data>
     <data name='Command'>CancelSimpleWindow</data>
    </bundle>
   </list>
  </bundle>
 </list>
</bundle>
