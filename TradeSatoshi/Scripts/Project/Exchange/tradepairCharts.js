setVisibleChart();
var tradePairId = $('#chart-container').data('tradepairid')
var charttitle = $('#chart-container').data('title')
var chartAction = $('#chart-container').data('action-chart');
var depthAction = $('#chart-container').data('action-depth');
var chartbaseSymbol = $('#chart-container').data('basesymbol');

$('#depthdata').highcharts({
	chart: {
		type: 'area',
		zoomType: 'xy',
		height: 260,
		backgroundColor: 'transparent'
	},
	title: {
		text: ''
	},
	legend: {
		enabled: false
	},
	xAxis: {
		type: "logarithmic",
		name: "Price",
		labels: {
			format: '{value:.8f}',
			align: 'right',
			x: -3
		},
		tickPosition: 'inside',
		tickInterval: 0.4,
		showFirstLabel: false,
		showLastLabel: false
	},
	yAxis: {
		type: "logarithmic",
		labels: {
			format: '{value:.8f}',
			align: 'right',
			x: -3,
			y: 0
		},
		title: {
			text: 'Volume ' + chartbaseSymbol
		},
		offset: 0,
		lineWidth: 2,
		tickPosition: 'inside',
		opposite: true,
		showFirstLabel: false,
		showLastLabel: false
	},
	credits: {
		enabled: false
	},
	tooltip:
		{
			changeDecimals: 8,
			valueDecimals: 8,
			followPointer: false,
			headerFormat: '<b style="font-size:14px">{series.name} Depth</b><br/><span>Price: <b>{point.key:.8f}</b> ' + chartbaseSymbol + '</span><br/>',
			pointFormat: '<span>Volume: <b>{point.y:.8f}</b> ' + chartbaseSymbol + '</span><br/>'
		},
	series: [{
		
		name: 'Buy',
		data: [],
		color: "#5cb85c",
		fillOpacity: 0.5,
		lineWidth:1,
		marker: {
			enabled: true,
			radius: 2,
			symbol: 'circle'
		},
		yAxis: 0
	},
	{
		name: 'Sell',
		color: "#d9534f",
		fillOpacity: 0.5,
		data: [],
		lineWidth: 1,
		marker: {
			enabled: true,
			radius: 2,
			symbol: 'circle'
		},
		yAxis: 0
	}]
});

$('#chartdata').highcharts('StockChart', {
	chart: {
		height: 274,
		zoomType: 'xy',
		backgroundColor: 'transparent',
	},

	credits: {
		enabled: false
	},

	rangeSelector: {
		selected: 1
	},
	navigator: {
		enabled: false
	},
	xAxis: {
		tickPosition: 'inside'
	},
	yAxis: [{
		labels: {
			format: '{value:.8f}',
			align: 'right',
			x: -3
		},
		title: {
			text: 'OHLC'
		},
		height: '70%',
		offset: 0,
		lineWidth: 2,
		tickPosition: 'inside'
	},
	{
		labels: {
			format: '{value:.8f}',
			align: 'right',
			x: -3
		},
		title: {
			text: 'Volume'
		},
		top: '72%',
		height: '28%',
		offset: 0,
		lineWidth: 2,
		tickPosition: 'inside'
	}],

	candlestick: {
		pointWidth: '4px'
	},

	series: [{
		type: 'candlestick',
		name: charttitle,
		color: '#ee5f5b',
		upColor: '#5cb85c',
	},
	{
		name: 'Mean',
		data: [[0, 0, 0, 0, 0, 0]],
		type: 'spline',
		color: 'rgba(0, 0, 0, 0.2)',
		tooltip: {
			valueDecimals: 8
		}
	},
	{
		type: 'column',
		color: '#666666',
		name: 'Volume',
		data: [[0, 0]],
		yAxis: 1,
	}],

	tooltip:
	{
		changeDecimals: 8,
		valueDecimals: 8,
		followPointer: false
	},

	rangeSelector: {
		inputEnabled: false,
		allButtonsEnabled: true,
		buttons: [{
			type: 'day',
			count: 1,
			text: 'Day',
			dataGrouping: {
				forced: true,
				units: [['hour', [1]]]
			}
		},
		{
			type: 'week',
			count: 1,
			text: 'Week',
			dataGrouping: {
				forced: true,
				units: [['hour', [4]]]
			}
		},
		{
			type: 'week',
			text: 'Month',
			count: 4,
			dataGrouping: {
				forced: true,
				units: [['hour', [12]]]
			}
		},
		{
			type: 'week',
			text: '3 Month',
			count: 12,
			dataGrouping: {
				forced: true,
				units: [['hour', [12]]]
			}
		},
		{
			type: 'all',
			text: 'All',
			dataGrouping: {
				forced: true,
				units: [['hour', [12]]]
			}
		}],

		buttonTheme: {
			width: 60
		},
		selected: 1
	},
});

if (isDepthChartSelected) {
	getDepthData();
}
else {
	getChartData();
}


$('.chart-option').on('click', function () {
	$('.chart-option').removeClass('active');
	$(this).addClass('active');
	var selectedChart = $(this).data('type');
	if (selectedChart === 'chart') {
		isDepthChartSelected = false;
		$('#depthdata').hide();
		$('#chartdata').show();
		getChartData();
	}
	else if (selectedChart === 'depth') {
		isDepthChartSelected = true;
		$('#chartdata').hide();
		$('#depthdata').show();
		getDepthData();
	}
});

$(document).off("chartdata-update").on("chartdata-update", function (event, tradePair) {
	if (tradePair == tradePairId && !isDepthChartSelected) {
	alert()
		getChartData();
	}
});

$(document).off("chartdata-depth-update").on("chartdata-depth-update", function (event, tradePair) {
	if (tradePair == tradePairId && isDepthChartSelected) {
		alert()
		getDepthData();
	}
});


function setVisibleChart() {
	if (isDepthChartSelected) {
		$('.chart-option').removeClass('active');
		$('.chart-option-depth').addClass('active');
		$('#depthdata').addClass('active').show();
		$('#chartdata').removeClass('active').hide();
	}
}

function getChartData() {
	getJson(chartAction, {}, function (data) {
		updateChart(data);
	});
}

function getDepthData() {
	getJson(depthAction, {}, function (data) {
		updateDepth(data);
	});
}

function updateChart(chartData) {
	setVisibleChart();
	var cdata = chartData ? chartData.Candle : [[0, 0, 0, 0, 0, 0]];
	var vdata = chartData ? chartData.Volume : [[0, 0]];
	var chart = $('#chartdata').highcharts();
	if (chart) {
		chart.showLoading();
		chart.series[0].setData(cdata)
		chart.series[1].setData(cdata)
		chart.series[2].setData(vdata)
		chart.xAxis[0].setExtremes();
		chart.rangeSelector.buttons[1].setState(2);
		chart.rangeSelector.clickButton(1, 1, true);
		chart.reflow();
		chart.hideLoading();
	}
}

function updateDepth(chartData) {
	setVisibleChart();
	var buydata = chartData ? chartData.BuyDepth : [[0, 0]];
	var selldata = chartData ? chartData.SellDepth : [[0, 0]];
	var depth = $('#depthdata').highcharts();
	if (depth) {
		depth.showLoading();
		depth.series[0].setData(buydata);
		depth.series[1].setData(selldata);
		depth.reflow();
		depth.hideLoading();
	}
}