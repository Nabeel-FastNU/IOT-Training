$(function () {

    dps = [];
    var chart = new CanvasJS.Chart("chartContainer", {
        theme: "light2",
        title: {
            text: "Live Update"
        },
        data: [{
            type: "spline",
            dataPoints: dps
        }]
    });
    chart.render();
    function update(data) {

        for (var i = 0; i < data.length; ++i) {
            dps = []
            dps.push({ y: data[i] });
            break;
        }
        dps.shift();
        chart.render();
        //chart = null;
    }

    //Create the proxy
    var chartData = $.connection.chartHub;

    function init() {
        chartData.server.initData();
    }

    chartData.client.updateData = function (data) {
        update(data);
    }

    // Start the connection
    $.connection.hub.start().done(function () {
        init()
    })
});