
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