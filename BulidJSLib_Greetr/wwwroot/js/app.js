// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

(function ($) {

    $(document).ready(function () {
       

        $('#login').on('click', function () {

            var firstname = $('#firstname').val();
            var lastname = $('#lastname').val();
            var selectLang = $('#lang').val();
            var formal = $('#formal').prop('checked');
            //var greetingObj=
            var loginGrtr = G$(firstname, lastname);

            loginGrtr.setLang(selectLang).HTMLGreeting('#greeting', formal).log();

        });
    });

}(jQuery));

