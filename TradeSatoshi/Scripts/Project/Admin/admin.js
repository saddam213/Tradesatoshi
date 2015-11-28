(function () {

	$('a[data-toggle="tab"]').on('shown.bs.tab', function (e) {
		var div = $(this).data('div');
		$(div + '-spin').show();
		if (e.relatedTarget) {
			var oldTarget = $(e.relatedTarget).data('div');
			$(oldTarget).empty();
		}
		getPartial(div, $(this).data('action'));
		var target = $(this).attr('href');
		$(target).css('left', $(window).width() + 'px');
		var left = $(target).offset().left;
		$(target).css({ left: left }).animate({ "left": "0px" }, "10", "swing", function () { $(div + '-spin').hide(); });
	});
	$('#tabcontrol-main a[href="#tabcontrol-main-tab1"]').tab('show');
}());