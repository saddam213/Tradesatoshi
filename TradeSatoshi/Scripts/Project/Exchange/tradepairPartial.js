$(function () {

	updateChart(null);


	var sum_buy = 0;
	$('[id^="table-buyOrders-"]').dataTable({
		"order": [[0, "desc"]],
		"lengthChange": false,
		"processing": true,
		"bServerSide": true,
		"searching": false,
		"scrollCollapse": false,
		"scrollY": "250px",
		"sPaginationType": "simple_numbers",
		"pageLength": "15",
		"sort": false,
		"paging": false,
		"info": false,
		"language": { "emptyTable": "No data avaliable." },
		"sAjaxSource": $('#buyOrders-host').data('action'),
		"sServerMethod": "POST",
		"fnServerParams": function (aoData) {
			aoData.push({ "name": "tradePairId", "value": $('#buyOrders-host').data('tradepair') });
			aoData.push({ "name": "tradeType", "value": $('#buyOrders-host').data('tradetype') });
		},
		"columnDefs": [{
			"targets": 2,
			"render": function (data, type, full, meta) {
				var total = (full[0] * full[1]);
				sum_buy += total;
				return total.toFixed(8);
			}
		},
		{
			"targets": 3,
			"visible": false,
			"render": function (data, type, full, meta) {
				return sum_buy.toFixed(8);
			}
		}],
		"footerCallback": function (row, data, start, end, display) {
			sum_buy = 0;
		},
		"rowCallback": function (row, data, index) {
			$(row).on('click', function () {
				$('#amount-Sell, #amount-Buy').val(data[1]);
				$('#rate-Sell, #rate-Buy').val(data[0]).trigger('change');
			});
		}
	});

	var sum_sell = 0;
	$('[id^="table-sellOrders-"]').dataTable({
		"order": [[0, "desc"]],
		"lengthChange": false,
		"processing": true,
		"bServerSide": true,
		"searching": false,
		"scrollCollapse": false,
		"scrollY": "250px",
		"sPaginationType": "simple_numbers",
		"pageLength": "15",
		"sort": false,
		"paging": false,
		"info": false,
		"language": { "emptyTable": "No data avaliable." },
		"sAjaxSource": $('#sellOrders-host').data('action'),
		"sServerMethod": "POST",
		"fnServerParams": function (aoData) {
			aoData.push({ "name": "tradePairId", "value": $('#sellOrders-host').data('tradepair') });
			aoData.push({ "name": "tradeType", "value": $('#sellOrders-host').data('tradetype') });
		},
		"columnDefs": [{
			"targets": 2,
			"render": function (data, type, full, meta) {
				var total = (full[0] * full[1]);
				sum_sell += total;
				return total.toFixed(8);
			}
		},
		{
			"targets": 3,
			"visible": false,
			"render": function (data, type, full, meta) {
				return sum_sell.toFixed(8);
			}
		}],
		"footerCallback": function (row, data, start, end, display) {
			sum_sell = 0;
		},
		"rowCallback": function (row, data, index) {
			$(row).on('click', function () {
				$('#amount-Sell, #amount-Buy').val(data[1]);
				$('#rate-Sell, #rate-Buy').val(data[0]).trigger('change');
			});
		}
	});


	$('[id^="table-tradehistory-"]').dataTable({
		"order": [[0, "desc"]],
		"lengthChange": false,
		"processing": false,
		"bServerSide": true,
		"searching": false,
		"scrollCollapse": false,
		"scrollY": "472px",
		"sort": false,
		"paging": false,
		"info": false,
		"language": { "emptyTable": "No data avaliable." },
		"sAjaxSource": $('#tradehistory-host').data('action'),
		"sServerMethod": "POST",
		"fnServerParams": function (aoData) {
			aoData.push({ "name": "tradePairId", "value": $('#tradehistory-host').data('tradepair') });
		},
		"columnDefs": [{
			"targets": 0,
			"render": function (data, type, full, meta) {
				return moment(new Date(data)).format('h:mm:ss a');
			}
		}],
		"rowCallback": function (row, data, index) {
			var textClass = data[1] === 'Buy' ? 'text-success' : 'text-danger';
			$(row).addClass(textClass);
		}
	});

	var chartAction = $('#chartdata').data('action');
	getJson(chartAction, {}, function (data) {
		updateChart(data);
	});

	$('#table-canceltradepair').on('click', function () {
		var action = $(this).data('action');
		var tradepair = $(this).data('tradepair');
		cancelTradePairOrders(tradepair, action);
	});
});

function cancelOrder(tradeId, action) {
	confirmModal("Cancel Order", "Are you sure you want to cancel order #" + tradeId + "?", function () {
		postJson(action, { id: tradeId, cancelType: 'Trade' });
	});
}

function cancelTradePairOrders(tradePair, action) {
	var count = $('[id^="table-useropenorders-"] > tbody > tr > td').length;
	if (count > 1) {
		confirmModal("Cancel Orders", "Are you sure you want to cancel ALL orders for this market?", function () {
			postJson(action, { id: tradePair, cancelType: 'TradePair' });
		});
	}
}

function updateChart(chartData) {
	var cdata = chartData ? chartData.Candle : [[0, 0, 0, 0, 0, 0]];
	var vdata = chartData ? chartData.Volume : [[0, 0]];
	var buydata = chartData ? chartData.BuyDepth : [[0, 0]];
	var selldata = chartData ? chartData.SellDepth : [[0, 0]];
	var baseSymbol = $('#chartdata').data('basesymbol')

	$('#chartdata').highcharts({
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
			type: "linear",
			name: "Price",
			labels: {
				format: '{value:.8f}',
				align: 'right',
				x: -3
			}
		},
		yAxis: {
			title: {
				text: 'Volume'
			},
			labels: {
				format: '{value:.8f}',
				align: 'right',
				x: -3
			}
		},
		credits: {
			enabled: false
		},
		tooltip:
			{
				changeDecimals: 8,
				valueDecimals: 8,
				followPointer: false,
				headerFormat: '<span>Price: <b>{point.key:.8f}</b> ' + baseSymbol + '</span><br/>',
				pointFormat: '<span>Volume: <b>{point.y:.8f}</b> ' + baseSymbol + '</span><br/>'
			},
		series: [{
			name: 'Sell',
			data: selldata,
			color: "#d9534f",
			fillOpacity: 0.5,
			marker: {
				enabled: false
			}
		},
		{
			name: 'Buy',
			color: "#5cb85c",
			fillOpacity: 0.5,
			data: buydata,
			marker: {
				enabled: false
			}
		}]
	});

	//$('#chartdata').highcharts('StockChart', {
	//	chart: {
	//		height: 278,
	//		//zoomType: 'xy',
	//		backgroundColor: 'transparent',

	//	},

	//	credits: {
	//		enabled: false
	//	},

	//	rangeSelector: {
	//		selected: 1
	//	},

	//	yAxis: [{
	//		labels: {
	//			format: '{value:.8f}',
	//			align: 'right',
	//			x: -3
	//		},
	//		title: {
	//			text: 'OHLC'
	//		},
	//		height: '75%',
	//		lineWidth: 2
	//	},
	//	{
	//		labels: {
	//			format: '{value:.8f}',
	//			align: 'right',
	//			x: -3
	//		},
	//		title: {
	//			text: 'Volume'
	//		},
	//		top: '75%',
	//		height: '25%',
	//		offset: 0,
	//		lineWidth: 2
	//	}],

	//	candlestick: {
	//		pointWidth: '4px'
	//	},

	//	series: [{
	//		type: 'candlestick',
	//		name: $('#chartdata').data('title'),
	//		data: cdata,
	//		color: '#ee5f5b',
	//		upColor: '#5cb85c',
	//	},
	//		{
	//			name: 'Mean',
	//			data: cdata,
	//			type: 'spline',
	//			color: 'rgba(0, 0, 0, 0.2)',
	//			tooltip: {
	//				valueDecimals: 8
	//			}
	//		},
	//		{
	//			type: 'column',
	//			color: '#666666',
	//			name: 'Volume',
	//			data: vdata,
	//			yAxis: 1,
	//		}],

	//	tooltip:
	//	{
	//		changeDecimals: 8,
	//		valueDecimals: 8,
	//		followPointer: false
	//	},

	//	rangeSelector: {
	//		allButtonsEnabled: true,
	//		buttons: [{
	//			type: 'day',
	//			count: 1,
	//			text: 'Day',
	//			dataGrouping: {
	//				forced: true,
	//				units: [['hour', [1]]]
	//			}
	//		},
	//		{
	//			type: 'week',
	//			count: 1,
	//			text: 'Week',
	//			dataGrouping: {
	//				forced: true,
	//				units: [['hour', [4]]]
	//			}
	//		},
	//		{
	//			type: 'week',
	//			text: 'Month',
	//			count: 4,
	//			dataGrouping: {
	//				forced: true,
	//				units: [['hour', [12]]]
	//			}
	//		},
	//		{
	//			type: 'week',
	//			text: '3 Month',
	//			count: 12,
	//			dataGrouping: {
	//				forced: true,
	//				units: [['hour', [48]]]
	//			}
	//		},
	//		{
	//			type: 'all',
	//			text: 'All',
	//			dataGrouping: {
	//				forced: true,
	//				units: [['day', [2]]]
	//			}
	//		}],

	//		buttonTheme: {
	//			width: 60
	//		},
	//		selected: 1
	//	},
	//});

}

function updateDepth(data) {
	alert(JSON.stringify(data))
	$('#chartdata').highcharts({
		chart: {
			type: 'area'
		},
		title: {
			text: 'Area chart with negative values'
		},
		xAxis: {
			type: "linear",

		},
		credits: {
			enabled: false
		},
		tooltip:
		{
			changeDecimals: 8,
			valueDecimals: 8,
			followPointer: false
		},
		series: [{
			name: 'Buy',
			data: [[0.001, 0], [0.001, 0], [0.001, 0], [0.001, 0], [0.001, 4000.00000000], [0.002, 3000.00000000], [0.003, 2000.00000000], [0.004, 1000.00000000], [0.0045, 100.00000000]]
		},
		{
			name: 'Sell',
			data: [[0.008, 4000.00000000], [0.007, 3000.00000000], [0.006, 2000.00000000], [0.005, 1000.00000000], [0.0045, 100.00000000], [0.005, 0], [0.005, 0], [0.005, 0], [0.005, 0]]
		}]
	});
}



[["0.00000015", "1000.00000000"], ["0.00000016", "2000.00000000"], ["0.00000020", "100.00000000"], ["12.00000000", "428.00000000"], ["124.00000000", "214.00000000"], ["125.00000000", "15.00000000"]]