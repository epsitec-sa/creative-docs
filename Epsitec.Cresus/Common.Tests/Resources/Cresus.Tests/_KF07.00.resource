<?xml version="1.0" encoding="utf-8"?>
<bundle name="_KF07" caption="Part.InvoiceArticle" type="Panel" culture="fr" rank="7">
  <data name="Panel">
    <xml>
      <panel xmlns:s="http://www.epsitec.ch/XNS/storage-structure-1" xmlns:f="http://www.epsitec.ch/XNS/storage-fields-1">
        <s:storage s:root="_1" s:n_ext="2" s:n_typ="8" s:n_obj="6">
          <s:external s:name="_DataSource" />
          <s:external s:name="_ResourceManager" />
          <s:type id="_1" s:name="Epsitec.Common.UI.Panel" />
          <s:type id="_2" s:name="Epsitec.Common.Support.ResourceManager" />
          <s:type id="_3" s:name="Epsitec.Common.Types.DataObject" />
          <s:type id="_4" s:name="Epsitec.Common.UI.PanelPlaceholder" />
          <s:type id="_5" s:name="Epsitec.Common.UI.Placeholder" />
          <s:type id="_6" s:name="Epsitec.Common.UI.DataSourceMetadata" />
          <s:type id="_7" s:name="Epsitec.Common.UI.Panels.EditPanel" />
          <s:type id="_8" s:name="Epsitec.Common.UI.Panels.SearchPanel" />
          <s:object id="_1" s:type="_1" />
          <s:object id="_2" s:type="_4" />
          <s:object id="_3" s:type="_5" />
          <s:object id="_4" s:type="_6" />
          <s:object id="_5" s:type="_7" />
          <s:object id="_6" s:type="_8" />
          <s:data id="_1" f:TabNavigationMode="ForwardTabPassive" f:ChildrenLayoutMode="Stacked" f:ContainerLayoutMode="HorizontalFlow" f:PreferredWidth="200" f:PreferredHeight="51" f:Anchor="BottomLeft" f:Padding="0;0;0;0" f:_2.ResourceManager="{ExtRef _ResourceManager}" f:_1.BundleId="[KF07]" f:_3.DataContext="\[Binding Mode=TwoWay\]" f:Children="{Collection {ObjRef _2}, {ObjRef _3}}" f:DataSource="{ExtRef _DataSource}" f:DataSourceMetadata="{ObjRef _4}" f:EditionPanel="{ObjRef _5}" f:SearchPanel="{ObjRef _6}" f:Text="" />
          <s:data id="_2" f:Value="{Binding Path=*.Article, Mode=TwoWay}" f:TabNavigationMode="ForwardToChildren, ForwardOnly" f:ChildrenLayoutMode="Stacked" f:ContainerLayoutMode="VerticalFlow" f:Text="&lt;i&gt;Panel&lt;/i&gt;" f:PreferredWidth="200" f:Margins="5;5;5;5" f:PreferredHeight="38" f:_2.ResourceManager="{ExtRef _ResourceManager}" f:Dock="StackBegin" f:Anchor="None" f:PanelId="[KF06]" />
          <s:data id="_3" f:Value="{Binding Path=*.Quantity, Mode=TwoWay}" f:TabNavigationMode="ForwardToChildren, ForwardOnly" f:Text="&lt;i&gt;TextField&lt;/i&gt;" f:Margins="5;5;5;5" f:Dock="StackBegin" f:Anchor="None" f:PreferredHeight="0" f:ContainerLayoutMode="HorizontalFlow" />
          <s:data id="_4" f:Fields="{Collection {Text '*;[KF001]'}}" />
          <s:data id="_5" f:TabNavigationMode="ForwardTabPassive" f:ChildrenLayoutMode="Stacked" f:ContainerLayoutMode="HorizontalFlow" f:PreferredWidth="200" f:PreferredHeight="51" f:Anchor="BottomLeft" f:Dock="None" f:Margins="0;0;0;0" f:Padding="0;0;0;0" f:_3.DataContext="\[Binding Mode=TwoWay\]" f:DataSource="{ExtRef _DataSource}" f:DataSourceMetadata="{ObjRef _4}" f:EditionPanel="{ObjRef _5}" f:Owner="{ObjRef _1}" f:SearchPanel="{ObjRef _6}" f:Text="" />
          <s:data id="_6" f:TabNavigationMode="ForwardTabPassive" f:ChildrenLayoutMode="Stacked" f:ContainerLayoutMode="HorizontalFlow" f:PreferredWidth="200" f:PreferredHeight="51" f:Anchor="BottomLeft" f:Dock="None" f:Margins="0;0;0;0" f:Padding="0;0;0;0" f:_3.DataContext="\[Binding Mode=TwoWay\]" f:DataSource="{ExtRef _DataSource}" f:DataSourceMetadata="{ObjRef _4}" f:EditionPanel="{ObjRef _5}" f:Owner="{ObjRef _1}" f:SearchPanel="{ObjRef _6}" f:Text="" />
        </s:storage>
      </panel>
    </xml>
  </data>
  <data name="DefaultSize">200;51</data>
  <data name="RootEntityId"></data>
</bundle>