var isDepthChartSelected = false;
$('#table-tradepair > .tradepair-row').on('click', function (e) {
	$('#table-tradepair > .tradepair-row').removeClass('active');
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
			tradePairTarget.off();
		}
		getPartial(targetName, targetUrl);
	}
});

var defaultMarket = $('#tradepairTarget').data('default');
if (!defaultMarket) {
	getPartial('#tradepairTarget', $('#tradepairTarget').data('summary'));
}
else {
	var baseMarket = defaultMarket.split('_')[1];
	$(".basemarket-btn-" + baseMarket).trigger('click');
	$('.tradepair-row[data-marketurl="' + defaultMarket + '"]').trigger('click');
}
