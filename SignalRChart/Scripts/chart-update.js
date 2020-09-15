$(function () {

    dps = [];
    var chart = new CanvasJS.Chart("chartContainer", {
        theme: "light2",
        title: {
            text: "Live Update"
        },
        axisX: {
            title: "60 seconds",
            valueFormatString: " "
        },
        data: [{
            type: "line",
            dataPoints: dps
        }]
    });
    chart.render();
    var xAxis = 0; 
    function update(data) {
        for (var i = 0; i < data.length; ++i) {
            dps.push({ x: xAxis, y: data[i] });
            xAxis++;
        }
        if (isTimeOut) {
            for (var j = 0; j < data.length; j++) {
                dps.shift();
            }
        }
        chart.render();
    }

    var isTimeOut = false;
    setTimeout(function () { isTimeOut = true }, 60000);

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