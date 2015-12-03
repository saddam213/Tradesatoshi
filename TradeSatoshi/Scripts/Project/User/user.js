$(function () {

	$('a[data-toggle="tab"]').on('shown.bs.tab', function (e) {
		var div = $(this).data('div');
		$(div + '-spin').show();
		if (e.relatedTarget) {
			var oldTarget = $(e.relatedTarget).data('div');
			if (oldTarget) {
				$(oldTarget).unbind();
				$(oldTarget).empty();
			}
		}
		getPartial(div, $(this).data('action'));
		var target = $(this).attr('href');
		$(target).css('left', $(window).width() + 'px');
		var left = $(target).offset().left;
		$(target).css({ left: left }).animate({ "left": "0px" }, "10", "swing", function () {
			$(div + '-spin').hide();
			location.hash = '#';
			location.hash = target;
		});
		
	});

	var hash = location.hash;
	if (!hash || hash == '#') {
		hash = "#Profile";
	}
	$('#tabcontrol-main a[href="' + hash + '"]').tab('show');

	
});