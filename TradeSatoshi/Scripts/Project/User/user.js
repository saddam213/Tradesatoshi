$(function () {

	$('#table-tradepair > .tradepair-row').on('click', function (e) {
		$('#table-tradepair > .tradepair-row').removeClass('active');
		$(this).addClass('active');
		var marketUrl = $(this).data('externallink');
		if (marketUrl) {
			window.location = marketUrl;
		}
	});

	$("#newApiKey").on("click", function () {
		var action = $(this).data("action");
		postJson(action, {}, function (data) {
			alert(data.Key)
			$("#Key").val(data.Key);
			$("#Secret").val(data.Secret);
		});
	});
});