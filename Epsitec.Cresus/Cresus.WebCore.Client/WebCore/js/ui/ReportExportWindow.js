// This class is a window that lets the user configure the details of an
// exportation of entities to pages that contains reports.

Ext.require([
  'Epsitec.cresus.webcore.tools.Enumeration',
  'Epsitec.cresus.webcore.tools.Texts',
  'Epsitec.cresus.webcore.tools.Tools'
],
function() {
  Ext.define('Epsitec.cresus.webcore.ui.ReportExportWindow', {
    extend: 'Ext.window.Window',
    alternateClassName: ['Epsitec.ReportExportWindow'],

    /* Configuration */

    width: 400,
    border: false,
    layout: {
      type: 'vbox',
      align: 'stretch'
    },
    modal: true,
    plain: true,
    title: Epsitec.Texts.getExportReportText(),

    /* Properties */

    exportUrl: null,
    textFactoryCombo: null,
    layoutCombo: null,

    /* Constructor */

    constructor: function(options) {
      var newOptions;
      this.layoutCombo = this.createLayoutCombo(options);

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

    /* Methods */

    createLayoutCombo: function(options) {
      return Ext.create('Ext.form.ComboBox', {
        labelWidth: 150,
        fieldLabel: Epsitec.Texts.getExportReportText(),
        store: Ext.create('Ext.data.Store', {
          fields: ['text'],
          data: options.reportExportDefinitions
        }),
        queryMode: 'local',
        displayField: 'text',
        valueField: 'text',
        value: options.reportExportDefinitions[0].text
      });
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
      var layoutId = this.layoutCombo.getValue();
      var url = this.exportUrl;
      url = Epsitec.Tools.addParameterToUrl(url, 'layout', layoutId);

      //window.open(url);
      Ext.Ajax.request({
          url: url,
          success: function (response) {
              //TODO
          }
      });
      this.close();
    }
  });
});
