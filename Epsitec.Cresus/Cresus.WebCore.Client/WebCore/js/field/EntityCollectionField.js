Ext.require([
  'Epsitec.cresus.webcore.field.EntityCollectionHiddenField',
  'Epsitec.cresus.webcore.field.EntityFieldList'
],
function() {
  Ext.define('Epsitec.cresus.webcore.field.EntityCollectionField', {
    extend: 'Ext.form.FieldContainer',
    alternateClassName: ['Epsitec.EntityCollectionField'],
    alias: 'widget.epsitec.entitycollectionfield',

    /* Config */

    layout: 'vbox',

    /* Properties */

    entityFieldList: null,
    hiddenField: null,
    originalValues: null,

    /* Constructor */

    constructor: function(options) {
      var newOptions;

      this.entityFieldList = this.createEntityFieldList(options);
      this.hiddenField = this.createHiddenField(options);

      newOptions = {
        fieldLabel: options.fieldLabel,
        items: [this.entityFieldList, this.hiddenField],
        originalValues: options.value
      };

      this.callParent([newOptions]);
      return this;
    },

    /* Additional methods */

    createEntityFieldList: function(options) {
      var value = options.value,
          readOnly = options.readOnly;

      return Ext.create('Epsitec.EntityFieldList', value, readOnly, {
        width: '100%',
        minHeight: '50',
        maxHeight: '150',
        databaseName: options.databaseName
      });
    },

    createHiddenField: function(options) {
      var me = this;
      return Ext.create('Epsitec.EntityCollectionHiddenField', {
        name: options.name,
        submitValueGetter: function() { return me.getSubmitValue(); },
        onReset: function() { me.resetEntityFieldList(); }
      });
    },

    getSubmitValue: function() {
      return this.entityFieldList
          .getItems()
          .map(function(item) { return item.id; })
          .join(';');
    },

    resetEntityFieldList: function() {
      this.entityFieldList.resetContent();
    }
  });
});
