
$(document).on("OnTradeHistoryUpdateGlobal", function (event, notification) {
	var last = $(".data-tradepair-last-" + notification.TradePairId);
	if (last && last.html()) {
		var lastPrice = (+last.html()).toFixed(8);
		var latestPrice = notification.Price.toFixed(8);
		var change = ((latestPrice - lastPrice) / lastPrice * 100.0);
		last.html(notification.Price.toFixed(8))
		$(".data-tradepair-val-" + notification.TradePairId).removeClass("text-danger text-success")
		$(".data-tradepair-change-" + notification.TradePairId).html(change.toFixed(1) + "%");
		if (lastPrice !== latestPrice) {
			$(".data-tradepair-val-" + notification.TradePairId).addClass(lastPrice > latestPrice ? "text-danger" : "text-success")
			highlightChange($(".tradepair-row-" + notification.TradePairId), lastPrice > latestPrice)
		}
	}
});

$(document).on("OnBalanceUpdateGlobal", function (event, notification) {
	var balance = $(".data-balance-" + notification.Symbol);
	if (balance) {
		var lastBalance = (+balance.html()).toFixed(8);
		var newBalance = notification.Avaliable.toFixed(8);
		balance.html(newBalance);
		if (lastBalance !== newBalance) {
			highlightChange($(".data-currency-" + notification.CurrencyId), lastBalance > newBalance)
		}
	}
});

$(".basemarket-btn").on("click", function () {
	var selectedBaseMarket = $(this)
	$(".basemarket-btn-selected").removeClass("basemarket-btn-selected");
	selectedBaseMarket.addClass("basemarket-btn-selected");
	$("#table-tradepair .tradepair-row").hide();
	$("#table-tradepair .tradepair-row-" + selectedBaseMarket.data("base").replace("$", "\\$")).show();

	$("#market-search").trigger("change");
});

$("#balance-search").on("keyup change paste", function () {
	var searchText = $(this).val();
	if (searchText) {
		$.each($("#table-balancemenu").find(".balance-row"), function () {
			if ($(this).data('search').toLowerCase().indexOf(searchText.toLowerCase()) == -1)
				$(this).hide();
			else
				$(this).show();
		});
	}
	else {
		$("#table-balancemenu .balance-row").show();
	}
});

$("#market-search").on("keyup change paste", function () {
	var searchText = $(this).val();
	var baseMarket = $(".basemarket-btn-selected").data("base") || "BTC";
	if (searchText) {
		$.each($("#table-tradepair").find(".tradepair-row-" + baseMarket.replace("$", "\\$")), function () {
			if ($(this).data('search').toLowerCase().indexOf(searchText.toLowerCase()) == -1)
				$(this).hide();
			else
				$(this).show();
		});
	}
	else {
		$("#table-tradepair .tradepair-row-" + baseMarket.replace("$", "\\$")).show();
	}
});

//var defaultMarket = $('#tradepairTarget').data('default');
//if (defaultMarket) {
//	var baseMarket = defaultMarket.split('_')[1];
//	$(".basemarket-btn-" + baseMarket).trigger('click');
//	$('.tradepair-row[data-marketurl="' + defaultMarket + '"]').trigger('click');
//} else {
//	var defaultBase = $(".basemarket-btn-BTC");
//	if (defaultBase.length > 0) {
//		defaultBase.trigger('click');
//	}
//	$('#table-tradepair .tradepair-row:first').trigger('click');
//}

