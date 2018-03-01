(function ($) {
    $('span.error:first').qtip("hide");
   
    //$('span.error:first').qtip(
    //   {
    //       content: 'You left some fields blank',
    //       position:
    //         {
    //             corner: { target: 'topright', tooltip: 'bottomright' },
    //             adjust: { x: -11 }
    //         },
    //       style: { name: 'red', tip: 'bottomRight' },
    //       show:
    //         {
    //             effect: { length: 500 },
    //             ready: true // Show when ready (page load)
    //         },
    //       hide: {
    //           effect: { length: 500 }
    //       },
    //       api: {
    //           onRender: function () {
    //               setTimout(this.hide, 3000); // Hide it after 3 seconds
    //           }
    //       }
    //   });

}(jQuery));