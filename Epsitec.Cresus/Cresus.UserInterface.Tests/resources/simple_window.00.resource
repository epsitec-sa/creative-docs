<bundle>
 <data name='class'>Window</data>
 <data name='Name'>my_window</data>
 <data name='Text'>Ressource: démo</data>
 <data name='Size'>240;280</data>
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
       </bundle>
       <bundle>
        <data name='class'>MenuItem</data>
        <data name='Text'>Enregistrer..</data>
        <data name='Icon'>res:image#save</data>
        <data name='Name'>save</data>
       </bundle>
       <bundle>
        <data name='class'>MenuSeparator</data>
       </bundle>
       <bundle>
        <data name='class'>MenuItem</data>
        <data name='Text'>Quitter</data>
        <data name='Name'>quit</data>
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
   <data name='Size'>240;235</data>
   <list name='widgets'>
    <bundle>
     <data name='class'>CheckButton</data>
     <data name='Text'><![CDATA[Pour rire, voici une icone&#160;:&#160;<img src="res:image#open"/>]]></data>
     <data name='Name'>A</data>
     <data name='Command'>SimpleWindowChangeOption</data>
     <data name='Size'>200;*</data>
     <data name='Location'>10;215</data>
     <data name='bind'><![CDATA[x.0.a<setup mode="on-off"/>;x.*.a#capt]]></data>
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
     <data name='Location'>100;190</data>
    </bundle>
    <bundle>
     <data name='class'>TextFieldCombo</data>
     <data name='IsReadOnly'>true</data>
     <data name='Size'>80;*</data>
     <data name='Location'>100;170</data>
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
     <data name='Command'>SimpleWindowChangeOption</data>
     <data name='Size'>200;*</data>
     <data name='Location'>10;140</data>
     <data name='bind'>x.0.b;x.*.b#capt</data>
    </bundle>
    <bundle>
     <data name='class'>GroupBox</data>
     <data name='Anchor'>Left, Right, Top, Bottom</data>
     <data name='Text'><![CDATA[Choix <i>cornelien</i>...]]></data>
     <data name='Size'>215;85</data>
     <data name='Location'>10;50</data>
     <data name='DockH'>no</data>
     <data name='DockPadding'>5;5;5;5</data>
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
     <data name='class'>Button</data>
     <data name='Text'>Annule</data>
     <data name='Size'>80;*</data>
     <data name='Command'>SimpleWindowCancel</data>
     <data name='Location'>10;10</data>
     <data name='Anchor'>Bottom,Left</data>
    </bundle>
   </list>
  </bundle>
 </list>
</bundle>
