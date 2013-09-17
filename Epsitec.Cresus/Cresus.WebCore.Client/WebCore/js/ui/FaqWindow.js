Ext.require([
],
function() {
  Ext.define('Epsitec.cresus.webcore.ui.FaqWindow', {
    extend: 'Ext.Window',
    alternateClassName: ['Epsitec.FaqWindow'],

    /* Properties */

    /* Constructor */

    constructor: function() {
      var config;

      config = {
        title : 'FAQ Aider',
        width : 1100,
          height: 700,
          layout : 'fit',
          bodyStyle: {
              top: '25px',
              left: '80px'
          },
          frame: false,
          border: false,
          closable: false,
          items : [{
              frame: false,
              border: false,
              xtype : "component",
              autoEl : {
                  tag : "iframe",
                  src: "http://faq-aider.eerv.ch/"
              }
          }]
      };

      this.callParent([config]);

      return this;
    }
    /* Methods */
  });
});
