<?xml version="1.0" encoding="utf-8"?>
<bundle name="_KF01" caption="Form.Invoice" type="Panel" culture="fr" rank="0">
  <data name="Panel">
    <xml>
      <panel xmlns:s="http://www.epsitec.ch/XNS/storage-structure-1" xmlns:f="http://www.epsitec.ch/XNS/storage-fields-1">
  <s:storage s:root="_1" s:n_ext="2" s:n_typ="12" s:n_obj="17">
    <s:external s:name="_DataSource" />
    <s:external s:name="_ResourceManager" />
    <s:type id="_1" s:name="Epsitec.Common.UI.Panel" />
    <s:type id="_2" s:name="Epsitec.Common.Support.ResourceManager" />
    <s:type id="_3" s:name="Epsitec.Common.Types.DataObject" />
    <s:type id="_4" s:name="Epsitec.Common.UI.PanelPlaceholder" />
    <s:type id="_5" s:name="Epsitec.Common.Widgets.FrameBox" />
    <s:type id="_6" s:name="Epsitec.Common.Widgets.Layouts.LayoutEngine" />
    <s:type id="_7" s:name="Epsitec.Common.Widgets.Layouts.GridLayoutEngine" />
    <s:type id="_8" s:name="Epsitec.Common.Widgets.Layouts.RowDefinition" />
    <s:type id="_9" s:name="Epsitec.Common.Widgets.Layouts.ColumnDefinition" />
    <s:type id="_10" s:name="Epsitec.Common.UI.Placeholder" />
    <s:type id="_11" s:name="Epsitec.Common.UI.DataSourceMetadata" />
    <s:type id="_12" s:name="Epsitec.Common.UI.Panels.EditPanel" />
    <s:object id="_1" s:type="_1" />
    <s:object id="_2" s:type="_4" />
    <s:object id="_3" s:type="_4" />
    <s:object id="_4" s:type="_5" />
    <s:object id="_5" s:type="_7" />
    <s:object id="_6" s:type="_8" />
    <s:object id="_7" s:type="_8" />
    <s:object id="_8" s:type="_8" />
    <s:object id="_9" s:type="_8" />
    <s:object id="_10" s:type="_9" />
    <s:object id="_11" s:type="_9" />
    <s:object id="_12" s:type="_10" />
    <s:object id="_13" s:type="_10" />
    <s:object id="_14" s:type="_10" />
    <s:object id="_15" s:type="_10" />
    <s:object id="_16" s:type="_11" />
    <s:object id="_17" s:type="_12" />
    <s:data id="_1" f:TabNavigationMode="ForwardTabPassive" f:ChildrenLayoutMode="Anchored" f:ContainerLayoutMode="VerticalFlow" f:PreferredWidth="389" f:PreferredHeight="332" f:Anchor="BottomLeft" f:Padding="20;20;20;20" f:_2.ResourceManager="{ExtRef _ResourceManager}" f:_1.BundleId="[KF01]" f:_3.DataContext="\[Binding Mode=TwoWay\]" f:Children="{Collection {ObjRef _2}, {ObjRef _3}, {ObjRef _4}}" f:DataSource="{ExtRef _DataSource}" f:DataSourceMetadata="{ObjRef _16}" f:EditionPanel="{ObjRef _17}" f:SearchPanel="{Null}" f:Text="" />
    <s:data id="_2" f:TabNavigationMode="ForwardTabPassive" f:ChildrenLayoutMode="Anchored" f:ContainerLayoutMode="HorizontalFlow" f:Anchor="BottomLeft" f:Text="&lt;i&gt;Panel&lt;/i&gt;" f:PreferredWidth="137" f:PreferredHeight="56" f:MinWidth="50" f:MinHeight="50" f:_2.ResourceManager="{ExtRef _ResourceManager}" f:Dock="None" f:Margins="212;0;0;236" f:PanelId="[KF]" />
    <s:data id="_3" f:TabNavigationMode="ForwardTabPassive" f:ChildrenLayoutMode="Anchored" f:ContainerLayoutMode="HorizontalFlow" f:Anchor="BottomLeft" f:Text="&lt;i&gt;Panel&lt;/i&gt;" f:PreferredWidth="137" f:PreferredHeight="56" f:MinWidth="50" f:MinHeight="50" f:_2.ResourceManager="{ExtRef _ResourceManager}" f:Dock="None" f:Margins="212;0;0;170" f:PanelId="[KF]" f:TabIndex="1" />
    <s:data id="_4" f:TabNavigationMode="ForwardToChildren, ForwardOnly" f:ChildrenLayoutMode="Grid" f:ContainerLayoutMode="HorizontalFlow" f:Anchor="BottomLeft" f:PreferredWidth="162" f:PreferredHeight="88" f:MinWidth="50" f:MinHeight="50" f:Padding="0;0;0;0" f:Dock="None" f:Margins="0;0;0;193" f:_6.LayoutEngine="{ObjRef _5}" f:Children="{Collection {ObjRef _12}, {ObjRef _13}, {ObjRef _14}, {ObjRef _15}}" f:Text="" />
    <s:data id="_5" f:ColumnDefinitions="{Collection {ObjRef _6}, {ObjRef _7}, {ObjRef _8}, {ObjRef _9}}" f:RowDefinitions="{Collection {ObjRef _10}, {ObjRef _11}}" />
    <s:data id="_6" f:Height="Auto" f:MinHeight="20" f:BottomBorder="1" f:TopBorder="1" />
    <s:data id="_7" f:Height="Auto" f:MinHeight="20" f:BottomBorder="1" f:TopBorder="1" />
    <s:data id="_8" f:Height="Auto" f:MinHeight="20" f:BottomBorder="1" f:TopBorder="1" />
    <s:data id="_9" f:Height="Auto" f:MinHeight="20" f:BottomBorder="1" f:TopBorder="1" />
    <s:data id="_10" f:Width="Auto" f:MinWidth="20" />
    <s:data id="_11" f:Width="*100" f:MinWidth="20" />
    <s:data id="_12" f:Value="{Binding Path=*.FirstName, Mode=TwoWay}" f:TabNavigationMode="ForwardToChildren, ForwardOnly" f:Text="&lt;i&gt;TextField&lt;/i&gt;" f:MinWidth="20" f:Anchor="None" f:MinHeight="20" f:Dock="None" f:PreferredHeight="20" f:Margins="0;0;0;0" f:PreferredWidth="80" f:ContainerLayoutMode="HorizontalFlow" f:_7.Column="0" f:_7.Row="0" />
    <s:data id="_13" f:Value="{Binding Path=*.LastName, Mode=TwoWay}" f:TabNavigationMode="ForwardToChildren, ForwardOnly" f:Text="&lt;i&gt;TextField&lt;/i&gt;" f:MinWidth="20" f:Anchor="None" f:MinHeight="20" f:Dock="None" f:PreferredHeight="20" f:Margins="0;0;0;0" f:PreferredWidth="80" f:ContainerLayoutMode="HorizontalFlow" f:_7.Column="0" f:_7.Row="1" />
    <s:data id="_14" f:Value="{Binding Path=*.Address1, Mode=TwoWay}" f:TabNavigationMode="ForwardTabPassive" f:Text="&lt;i&gt;TextField&lt;/i&gt;" f:MinWidth="20" f:MinHeight="20" f:Anchor="None" f:Dock="None" f:_7.Column="0" f:_7.Row="2" f:ContainerLayoutMode="HorizontalFlow" />
    <s:data id="_15" f:Value="{Binding Path=*.City, Mode=TwoWay}" f:TabNavigationMode="ForwardTabPassive" f:Text="&lt;i&gt;TextField&lt;/i&gt;" f:MinWidth="20" f:MinHeight="20" f:Anchor="None" f:Dock="None" f:_7.Column="0" f:_7.Row="3" f:ContainerLayoutMode="HorizontalFlow" />
    <s:data id="_16" f:Fields="{Collection {Text '*;[KF0E]'}}" />
    <s:data id="_17" f:TabNavigationMode="ForwardTabPassive" f:ChildrenLayoutMode="Anchored" f:ContainerLayoutMode="VerticalFlow" f:PreferredWidth="389" f:PreferredHeight="332" f:Anchor="BottomLeft" f:Dock="None" f:Margins="0;0;0;0" f:Padding="20;20;20;20" f:DataSource="{Null}" f:DataSourceMetadata="{ObjRef _16}" f:EditionPanel="{ObjRef _17}" f:Owner="{ObjRef _1}" f:SearchPanel="{Null}" f:Text="" />
  </s:storage>
</panel>
    </xml>
  </data>
</bundle>