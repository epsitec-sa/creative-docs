Ext.define('Epsitec.cresus.webcore.EditionTile', {
  extend: 'Epsitec.cresus.webcore.Tile',
  alternateClassName: ['Epsitec.EditionTile'],
  alias: 'widget.editiontile',

  /* Config */

  border: false,
  frame: true,
  margin: '0 0 5 0',
  defaultType: 'textfield',
  defaults: {
    anchor: '100%'
  },
  fieldDefaults: {
    labelAlign: 'top',
    msgTarget: 'side'
  },
  buttons: [
    {
      text: 'Reset',
      handler: function() {
        this.up('form').getForm().reset();
      }
    },
    {
      text: 'Save',
      handler: function() {
        if (this.up('form').getForm().isValid()) {
          var form = this.up('form');
          form.setLoading();
          form.getForm().submit({
            success: function(form, action) {
              this.setLoading(false);
              this.entityPanel.columnManager.refreshColumns(
                  0, this.entityPanel.columnId - 1
              );
            },
            failure: function(form, action) {
              this.setLoading(false);
              try {
                var config = Ext.decode(action.response.responseText);
                this.getForm().markInvalid(config.errors);
              }
              catch (err) {
                return;
              }
            },
            scope: form
          });
        }
      }
    }
  ],

  /* Constructor */

  constructor: function(options) {
    options.url = 'proxy/entity/edit/' + options.entityId;
    this.callParent(arguments);
    return this;
  }
});
