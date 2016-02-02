$(function () {

	$('#table-tradepair > tbody > tr').on('click', function (e) {
		$('#table-tradepair > tbody > tr').removeClass('active');
		$(this).addClass('active');

		var marketUrl = $(this).data('marketurl');
		if (marketUrl) {
			History.pushState({}, "TradeSatoshi - " + marketUrl, "?market=" + marketUrl);
		}

		var targetUrl = $(this).data('action');
		if (targetUrl) {
			var targetName = '#tradepairTarget';
			var tradePairTarget = $(targetName);
			if (tradePairTarget) {
				tradePairTarget.unbind();
			}
			getPartial(targetName, targetUrl);
		}
	});
	
	var defaultMarket = $('#tradepairTarget').data('default');
	if (defaultMarket) {
		$('tr[data-marketurl="' + defaultMarket + '"]').trigger('click');
	} else {
		$('#table-tradepair tr:first').trigger('click');
	}
});