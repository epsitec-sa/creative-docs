<bundle>
 <field name='class'>Window</field>
 <field name='name'>my_window</field>
 <field name='text'>Ressource: démo</field>
 <field name='size'>240;280</field>
 <field name='icon'>file:icon</field>
 <list name='widgets'>
  <bundle>
   <field name='class'>CheckButton</field>
   <field name='text'><![CDATA[Pour rire, voici une icone&nbsp;:&nbsp;<img src="res:image#open"/>]]></field>
   <field name='name'>checkA</field>
   <field name='cmd'>on</field>
   <field name='size'>200;*</field>
   <field name='pos'>10;210</field>
  </bundle>
  <bundle>
   <field name='class'>Button</field>
   <field name='text'><![CDATA[<img src="res:file:image#back"/>]]></field>
   <field name='size'>40;40</field>
   <field name='pos'>10;165</field>
  </bundle>
  <bundle>
   <field name='class'>Button</field>
   <field name='text'><![CDATA[<img src="res:file:image#forward"/>]]></field>
   <field name='size'>40;40</field>
   <field name='pos'>50;165</field>
  </bundle>
  <bundle>
   <field name='class'>GroupBox</field>
   <field name='anchor'>Left, Right, Top, Bottom</field>
   <field name='text'><![CDATA[Choix <i>cornelien</i>...]]></field>
   <field name='size'>215;85</field>
   <field name='pos'>10;60</field>
   <field name='dock_h'>no</field>
   <field name='dock_m'>5;5;20;5</field>
   <list name='widgets'>
    <bundle>
     <field name='class'>RadioButton</field>
     <field name='text'>Bouton B</field>
     <field name='dock'>Fill</field>
    </bundle>
    <bundle>
     <field name='class'>RadioButton</field>
     <field name='text'>Bouton C</field>
     <field name='dock'>Fill</field>
    </bundle>
    <bundle>
     <field name='class'>RadioButton</field>
     <field name='text'>Bouton D</field>
     <field name='dock'>Fill</field>
    </bundle>
   </list>
  </bundle>
  <bundle>
   <ref target='button.cancel'/>
   <field name='cmd'>on</field>
   <field name='pos'>10;10</field>
  </bundle>
 </list>
</bundle>
