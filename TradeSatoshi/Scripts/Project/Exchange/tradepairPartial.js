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
			"visible":false,
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
	$('#chartdata').highcharts('StockChart', {
		chart: {
			height: 278,
			//zoomType: 'xy',
			backgroundColor: 'transparent',

		},

		credits: {
			enabled: false
		},

		rangeSelector: {
			selected: 1
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
			height: '75%',
			lineWidth: 2
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
			top: '75%',
			height: '25%',
			offset: 0,
			lineWidth: 2
		}],

		candlestick: {
			pointWidth: '4px'
		},

		series: [{
			type: 'candlestick',
			name: $('#chartdata').data('title'),
			data: cdata,
			color: '#ee5f5b',
			upColor: '#5cb85c',
		},
			{
				name: 'Mean',
				data: cdata,
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
				data: vdata,
				yAxis: 1,
			}],

		tooltip:
		{
			changeDecimals: 8,
			valueDecimals: 8,
			followPointer: false
		},

		rangeSelector: {
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
					units: [['hour', [48]]]
				}
			},
			{
				type: 'all',
				text: 'All',
				dataGrouping: {
					forced: true,
					units: [['day', [2]]]
				}
			}],

			buttonTheme: {
				width: 60
			},
			selected: 1
		},
	});
}

