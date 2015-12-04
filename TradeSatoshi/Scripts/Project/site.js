(function () {
	var notificationHub = $.connection.Notification;
	notificationHub.client.SendNotification = function (notification) {
		$.jGrowl(notification.Message, { position: "bottom-left", header: notification.Title });
	};
	$.connection.hub.start();
})();

function getPartial(div, url, callback) {
	$.ajax({
		url: url,
		cache: false,
		async: true,
		type: "GET",
		success: function (response, textStatus, jqXHR) {
			$(div).html(response);
			if (callback) {
				callback(response);
			}
		},
		error: function (jqXHR, textStatus, errorThrown) {
		}
	});
}

function postJson(url, vars, callback) {
	$.ajax({
		url: url,
		cache: false,
		async: true,
		type: "post",
		dataType: 'json',
		data: vars,
		success: function (response, textStatus, jqXHR) {
			if (callback) {
				callback(response);
			}
		},
		error: function (jqXHR, textStatus, errorThrown) {
		}
	});
}

function openModal(url, data, callback) {
	$.ajax({
		type: "GET",
		url: url,
		data: data,
		cache: false,
		success: function (data, textStatus, jqXHR) {
			$.modal(data, {
				onClose: function (dialog) {
					dialog.container.fadeOut(50, function () {
						dialog.overlay.fadeOut(200, function () {
							$.modal.close();
							if ($.isFunction(callback)) {
								callback(data);
							}
						});
					});
				},
				onOpen: function (dialog) {
					dialog.overlay.fadeIn(200, function () {
						dialog.container.fadeIn(50, function () {
							dialog.data.fadeIn(50);
						});
					});
				}
			});
		},
		error: function (jqXHR, textStatus, errorThrown) {
		}
	});

}

function confirmModal(header, message, callback) {
	var data = '<div style="width:100%;height:100%"><div class="modal-dialog"><div class="modal-content"><div class="modal-header"><button class="close simplemodal-close" aria-hidden="true" type="button">×</button><h4 class="modal-title header"></h4></div><div class="modal-body"><span class="message" ></span></div><div class="modal-footer"><button style="width:70px" class="btn btn-info yes " type="button">Yes</button><button style="width:70px" class="btn btn-info simplemodal-close no" type="button">No</button></div></div></div></div>';
	$.modal(data, {
		onShow: function (dialog) {
			var modal = this;

			$('.header', dialog.data[0]).append(header);
			$('.message', dialog.data[0]).append(message);

			// if the user clicks "yes"
			$('.yes', dialog.data[0]).click(function () {
				// call the callback
				if ($.isFunction(callback)) {
					callback.apply();
				}
				// close the dialog
				modal.close(); // or $.modal.close();
			});
		},
		onClose: function (dialog) {
			dialog.container.fadeOut(200, function () {
				dialog.overlay.fadeOut(150, function () {
					$.modal.close();
				});
			})
		},
		onOpen: function (dialog) {
			dialog.overlay.fadeIn(150, function () {
				dialog.container.fadeIn(200);
				dialog.data.fadeIn(200);
			});
		}
	});
}

function notifyModal(header, message, callback) {
	var data = '<div class="modal-dialog"><div class="modal-content"><div class="modal-header"><button class="close simplemodal-close" aria-hidden="true" type="button">×</button><h4 class="modal-title header"></h4></div><div class="modal-body"><span class="message" ></span></div><div class="modal-footer"><button style="width:70px" class="btn btn-info ok" type="button">Ok</button></div></div></div>';
	$.modal(data, {
		onShow: function (dialog) {
			var modal = this;

			$('.header', dialog.data[0]).append(header);
			$('.message', dialog.data[0]).append(message);

			// if the user clicks "ok"
			$('.ok', dialog.data[0]).click(function () {
				// call the callback
				if ($.isFunction(callback)) {
					callback.apply();
				}
				// close the dialog
				modal.close();
			});
		},
		onClose: function (dialog) {
			dialog.container.fadeOut(200, function () {
				dialog.overlay.fadeOut(150, function () {
					$.modal.close();
				});
			})
		},
		onOpen: function (dialog) {
			dialog.overlay.fadeIn(150, function () {
				dialog.container.fadeIn(200);
				dialog.data.fadeIn(200);
			});
		}
	});
}