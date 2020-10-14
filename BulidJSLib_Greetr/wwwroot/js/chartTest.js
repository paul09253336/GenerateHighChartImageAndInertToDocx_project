// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

(function ($) {

    $(document).ready(function () {
        var option = {

            title: {
                text: 'Solar Employment Growth by Sector, 2010-2016'
            },

            subtitle: {
                text: 'Source: thesolarfoundation.com'
            },

            yAxis: {
                title: {
                    text: 'Number of Employees'
                }
            },

            xAxis: {
                accessibility: {
                    rangeDescription: 'Range: 2010 to 2017'
                }
            },

            legend: {
                layout: 'vertical',
                align: 'right',
                verticalAlign: 'middle'
            },

            plotOptions: {
                series: {
                    label: {
                        connectorAllowed: false
                    },
                    pointStart: 2010
                }
            },

            series: [{
                name: 'Installation',
                data: [43934, 52503, 57177, 69658, 97031, 119931, 137133, 154175]
            }, {
                name: 'Manufacturing',
                data: [24916, 24064, 29742, 29851, 32490, 30282, 38121, 40434]
            }, {
                name: 'Sales & Distribution',
                data: [11744, 17722, 16005, 19771, 20185, 24377, 32147, 39387]
            }, {
                name: 'Project Development',
                data: [null, null, 7988, 12169, 15112, 22452, 34400, 34227]
            }, {
                name: 'Other',
                data: [12908, 5948, 8105, 11248, 8989, 11816, 18274, 18111]
            }],

            responsive: {
                rules: [{
                    condition: {
                        maxWidth: 100
                    },
                    chartOptions: {
                        legend: {
                            layout: 'horizontal',
                            align: 'center',
                            verticalAlign: 'bottom'
                        }
                    }
                }]
            }

        };

        Highcharts.chart('chartTest', option);
        var imageData = {
            data: option,
            width: false,
            scale: false,
            constr: "Test",
            type: "image/png",
            async: true
        };
        var imageDataJson = JSON.stringify(imageData) + "";

        $('#Test').on('click', function () {
            var imageData = [];
            imageData.push(imageDataJson);
            $.ajax({
                type: 'POST',
                //timeout: 120000,
                beforeSend: function () { $.blockUI({ message: '<h1>檔案產生中...</h1>' }) },
                data: { imgDatas: imageData},
                url: '/Home/ExportDocx/',
                success: function (data) {

                },
                complete: function () { }
            }).done(function (data) {
                $.unblockUI();
                location.href = '/Home/HighChartDocxDownload?fileName=' + data.fileName;
            });

            //location.href = '/Home/ExportDocx';
        });
       

    });

}(jQuery));







