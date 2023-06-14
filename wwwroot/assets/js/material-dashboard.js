// =========================================================
// Material Dashboard 2 - v3.1.0
// =========================================================

// Product Page: https://www.creative-tim.com/product/material-dashboard
// Copyright 2023 Creative Tim (https://www.creative-tim.com)
// Licensed under MIT (https://github.com/creativetimofficial/material-dashboard/blob/master/LICENSE.md)

// Coded by www.creative-tim.com

// =========================================================

// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.

"use strict";
(function() {
    var isWindows = navigator.platform.indexOf('Win') > -1 ? true : false;

    if (isWindows) {
        // if we are on windows OS we activate the perfectScrollbar function
        if (document.getElementsByClassName('main-content')[0]) {
            var mainpanel = document.querySelector('.main-content');
            var ps = new PerfectScrollbar(mainpanel);
        };

        if (document.getElementsByClassName('sidenav')[0]) {
            var sidebar = document.querySelector('.sidenav');
            var ps1 = new PerfectScrollbar(sidebar);
        };

        if (document.getElementsByClassName('navbar-collapse')[0]) {
            var fixedplugin = document.querySelector('.navbar:not(.navbar-expand-lg) .navbar-collapse');
            var ps2 = new PerfectScrollbar(fixedplugin);
        };

        if (document.getElementsByClassName('fixed-plugin')[0]) {
            var fixedplugin = document.querySelector('.fixed-plugin');
            var ps3 = new PerfectScrollbar(fixedplugin);
        };
    };
})();