<bundle>
 <data name='class'>Window</data>
 <data name='name'>my_window</data>
 <data name='text'>Ressource: démo</data>
 <data name='size'>240;280</data>
 <data name='icon'>file:icon</data>
 <list name='widgets'>
  <bundle>									<!-- Définition du menu -->
   <data name='class'>HMenu</data>
   <data name='name'>main_menu</data>
   <data name='dock'>Top</data>
   <list name='items'>
    <bundle>
     <data name='class'>MenuItem</data>
     <data name='text'>Fichier</data>
     <bundle name='menu'>
      <data name='class'>VMenu</data>
      <list name='items'>
       <bundle>
        <data name='class'>MenuItem</data>
        <data name='text'>Ouvrir..</data>
        <data name='icon'>res:image#open</data>
        <data name='name'>open</data>
       </bundle>
       <bundle>
        <data name='class'>MenuItem</data>
        <data name='text'>Enregistrer..</data>
        <data name='icon'>res:image#save</data>
        <data name='name'>save</data>
       </bundle>
       <bundle>
        <data name='class'>MenuSeparator</data>
       </bundle>
       <bundle>
        <data name='class'>MenuItem</data>
        <data name='text'>Quitter</data>
        <data name='name'>quit</data>
       </bundle>
      </list>
     </bundle>
    </bundle>
   </list>
  </bundle>
  
  <bundle>									<!-- Définition de la barre d'icônes -->
   <data name='class'>HToolBar</data>
   <data name='dock'>Top</data>
   <list name='items'>
    <bundle>
     <data name='class'>IconButton</data>
     <data name='icon'>file:images/open.png</data>
    </bundle>
    <bundle>
     <data name='class'>IconButton</data>
     <data name='icon'>file:images/save.png</data>
    </bundle>
    <bundle>
     <data name='class'>IconSeparator</data>
    </bundle>
    <bundle>
     <data name='class'>TextFieldCombo</data>
     <data name='ro'>true</data>
     <data name='size'>80;*</data>
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
     <data name='icon'>file:images/cut.png</data>
    </bundle>
    <bundle>
     <data name='class'>IconButton</data>
     <data name='icon'>file:images/copy.png</data>
    </bundle>
    <bundle>
     <data name='class'>IconButton</data>
     <data name='icon'>file:images/paste.png</data>
    </bundle>
   </list>
  </bundle>
  
  <bundle>
   <data name='class'>Widget</data>
   <data name='dock'>Fill</data>
   <data name='size'>240;235</data>
   <list name='widgets'>
    <bundle>
     <data name='class'>CheckButton</data>
     <data name='text'><![CDATA[Pour rire, voici une icone&nbsp;:&nbsp;<img src="res:image#open"/>]]></data>
     <data name='name'>checkA</data>
     <data name='cmd'>on</data>
     <data name='size'>200;*</data>
     <data name='pos'>10;215</data>
     <data name='bind'>a;a#label</data>
    </bundle>
    <bundle>
     <data name='class'>Button</data>
     <data name='text'><![CDATA[<img src="res:file:image#back"/>]]></data>
     <data name='size'>40;40</data>
     <data name='pos'>10;165</data>
    </bundle>
    <bundle>
     <data name='class'>Button</data>
     <data name='text'><![CDATA[<img src="res:file:image#forward"/>]]></data>
     <data name='size'>40;40</data>
     <data name='pos'>50;165</data>
    </bundle>
    <bundle>
     <data name='class'>TextField</data>
     <data name='size'>80;*</data>
     <data name='pos'>100;190</data>
    </bundle>
    <bundle>
     <data name='class'>TextFieldCombo</data>
     <data name='ro'>true</data>
     <data name='size'>80;*</data>
     <data name='pos'>100;170</data>
     <bundle name='items'>
      <data name='00/SelectA'>Choix A</data>
      <data name='01/SelectB'>Choix B</data>
      <data name='02/BestSelectC'><![CDATA[Choix <b>C</b> <i>(meilleur de tous)</i>]]></data>
      <data name='03/SelectD'>Choix D</data>
     </bundle>
    </bundle>
    <bundle>
     <data name='class'>CheckButton</data>
     <data name='text'><![CDATA[Copie du premier bouton]]></data>
     <data name='name'>checkB</data>
     <data name='cmd'>on</data>
     <data name='size'>200;*</data>
     <data name='pos'>10;140</data>
     <data name='bind'>b;b#label</data>
    </bundle>
    <bundle>
     <data name='class'>GroupBox</data>
     <data name='anchor'>Left, Right, Top, Bottom</data>
     <data name='text'><![CDATA[Choix <i>cornelien</i>...]]></data>
     <data name='size'>215;85</data>
     <data name='pos'>10;50</data>
     <data name='dock_h'>no</data>
     <data name='dock_m'>5;5;20;5</data>
     <list name='widgets'>
      <bundle>
       <data name='class'>RadioButton</data>
       <data name='text'>Bouton B</data>
       <data name='dock'>Fill</data>
      </bundle>
      <bundle>
       <data name='class'>RadioButton</data>
       <data name='text'>Bouton C</data>
       <data name='dock'>Fill</data>
      </bundle>
      <bundle>
       <data name='class'>RadioButton</data>
       <data name='text'>Bouton D</data>
       <data name='dock'>Fill</data>
      </bundle>
     </list>
    </bundle>
    <bundle>
     <ref target='button.cancel'/>
     <data name='cmd'>on</data>
     <data name='pos'>10;10</data>
    </bundle>
   </list>
  </bundle>
 </list>
</bundle>
