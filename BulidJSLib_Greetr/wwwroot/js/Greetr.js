// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

//這裡多放分號，用意是避免前面的js忘記用分號做結尾
;(function (global,$) {

    // Bulid new object
    var Greetr = function (firstname, lastname, language) {
        return new Greetr.init(firstname, lastname, language);
    }

    // hidden whthin the scope of the IIFE and never directly accessible
    var supportedlangs = ['en', 'es'];
    var greetings = {
        en: 'Hello',
        es: 'Hola'
    };

    var formalGreetings = {
        en: 'Greetings',
        es: 'Saludos'
    };

    var logMessages = {
        en: 'Logged in',
        es: 'Inicio sesion'
    };

    //建立prototype 內函數
    Greetr.prototype = {

        fullname: function () {
            return this.firstname + " " + this.lastname
        },

        validate: function () {
            if (supportedlangs.indexOf(this.language) === -1) {
                throw "Invlid language";
            }
        },

        greeting: function () {
            return greetings[this.language] + ' ' + this.firstname + '!';
        },

        formalGreeting: function () {

            return formalGreetings[this.language] + ' ' + this.fullname();
        },

        greet: function (formal) {
            var msg;

            if (formal) {
                msg = this.formalGreeting();
            }
            else {
                msg = this.greeting();
            }

            if (console) {
                console.log(msg);
            }

            return this;
        },

        log: function () {
            if (console) {
                console.log(logMessages[this.language] + ': ' + this.fullname());
            }
            return this;
        },

        //設定語言
        setLang: function (lang) {
            this.language = lang
           
            this.validate();

            return this;
        },

        //變更HTML標籤
        HTMLGreeting: function (selector,formal) {
            if (!$) {
                throw 'jQuery not loaded';
            }

            if (!selector) {
                throw 'Missing jQuery selector';
            }

            var msg;
            if (formal) {
                msg = this.formalGreeting();
            }
            else {
                msg = this.greeting();
            }

            $(selector).html(msg);

            return this;
        }
    };

    //建立init 函數 init一些參數
    Greetr.init = function (firstname, lastname, language) {

        var self = this;
        self.firstname = firstname || '';
        self.lastname = lastname || '';
        self.language = language || 'en';
    }

    //將Greetr.init的prototype設定與Greetr.prototype
    //這樣當調用Greetr.init時也能使用到Greetr.prototype的方法
    Greetr.init.prototype = Greetr.prototype;

    //新增window.Greetr物件，使我們可以直接調用它
    global.Greetr = global.G$ = Greetr;
}(window, jQuery));