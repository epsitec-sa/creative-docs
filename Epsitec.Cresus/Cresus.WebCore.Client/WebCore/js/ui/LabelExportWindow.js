Ext.require([
  'Epsitec.cresus.webcore.tools.Enumeration',
  'Epsitec.cresus.webcore.tools.Texts',
  'Epsitec.cresus.webcore.tools.Tools'
],
function() {
  Ext.define('Epsitec.cresus.webcore.ui.LabelExportWindow', {
    extend: 'Ext.window.Window',
    alternateClassName: ['Epsitec.LabelExportWindow'],

    /* Config */

    width: 400,
    border: false,
    layout: {
      type: 'vbox',
      align: 'stretch'
    },
    modal: true,
    plain: true,
    title: Epsitec.Texts.getExportTitle(),

    /* Properties */

    exportUrl: null,
    textFactoryCombo: null,
    layoutCombo: null,

    /* Constructor */

    constructor: function(options) {
      var newOptions;

      this.textFactoryCombo = this.createTextFactoryCombo(options);
      this.layoutCombo = this.createLayoutCombo();

      newOptions = {
        items: [this.textFactoryCombo, this.layoutCombo],
        buttons: [
          this.createOkButton(),
          this.createCancelButton()
        ]
      };
      Ext.applyIf(newOptions, options);

      this.callParent([newOptions]);
      return this;
    },

    /* Additional methods */

    createTextFactoryCombo: function(options) {
      return Ext.create('Ext.form.ComboBox', {
        labelWidth: 150,
        fieldLabel: Epsitec.Texts.getExportLabelText(),
        store: Ext.create('Ext.data.Store', {
          fields: ['id', 'text'],
          data: options.labelExportDefinitions
        }),
        queryMode: 'local',
        displayField: 'text',
        valueField: 'id',
        value: options.labelExportDefinitions[0].id
      });
    },

    createLayoutCombo: function() {
      var combo, store, storeName, storeUrl;

      storeName = 'labellayouts';
      storeUrl = 'proxy/print/labellayouts';
      store = Epsitec.Enumeration.getEnumerationStore(storeName, storeUrl);

      combo = Ext.create('Ext.form.ComboBox', {
        labelWidth: 150,
        fieldLabel: Epsitec.Texts.getExportLabelLayout(),
        store: store,
        queryMode: 'local',
        displayField: 'text',
        valueField: 'id'
      });

      if (store.isLoaded)
      {
        combo.setValue(store.first().get('id'));
      }
      else
      {
        store.on(
            'load',
            function() { combo.setValue(store.first().get('id')); },
            this
        );
      }

      return combo;
    },

    createCancelButton: function() {
      return Ext.create('Ext.Button', {
        text: Epsitec.Texts.getCancelLabel(),
        handler: this.onCancelClick,
        scope: this
      });
    },

    createOkButton: function() {
      return Ext.create('Ext.Button', {
        text: Epsitec.Texts.getOkLabel(),
        handler: this.onOkClick,
        scope: this
      });
    },

    onCancelClick: function() {
      this.close();
    },

    onOkClick: function() {
      var url, layoutId, textId;

      layoutId = this.layoutCombo.getValue();
      textId = this.textFactoryCombo.getValue();

      url = this.exportUrl;
      url = Epsitec.Tools.addParameterToUrl(url, 'type', 'label');
      url = Epsitec.Tools.addParameterToUrl(url, 'layout', layoutId);
      url = Epsitec.Tools.addParameterToUrl(url, 'text', textId);

      window.open(url);
      this.close();
    }
  });
});
