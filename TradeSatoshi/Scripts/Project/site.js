$.ajaxPrefilter(function (options, originalOptions) {
	if (options.type.toUpperCase() == "POST") {
		options.data = $.param($.extend(originalOptions.data, { __RequestVerificationToken: $('input[name="__RequestVerificationToken"]').val() }));
	}
});
$.ajaxSetup({ cache: false });

var notificationHub = $.connection.Notification;
notificationHub.client.OnNotification = function (notification) {
	console.log(notification.Message)
	showNotificationPopup(notification);
};
notificationHub.client.OnBalanceUpdate = function (notification) {
	console.log("OnBalanceUpdate")
	$(document).trigger("OnBalanceUpdate", notification);
	$(document).trigger("OnBalanceUpdateGlobal", notification);
};
notificationHub.client.OnOrderBookUpdate = function (notification) {
	console.log("OnOrderBookUpdate" + notification.TradePairId)
	$(document).trigger("OnOrderBookUpdate", notification);
	$(document).trigger("OnOrderBookUpdateGlobal", notification);
};
notificationHub.client.OnTradeHistoryUpdate = function (notification) {
	console.log("OnTradeHistoryUpdate" + notification.TradePairId)
	$(document).trigger("OnTradeHistoryUpdate", notification);
	$(document).trigger("OnTradeHistoryUpdateGlobal", notification);
};
notificationHub.client.OnOpenOrderUserUpdate = function (notification) {
	console.log("OnOpenOrderUserUpdate")
	$(document).trigger("OnOpenOrderUserUpdate", notification);
};
//notificationHub.client.OnTradeUserHistoryUpdate = function (notification) {
//	console.log("OnTradeUserHistoryUpdate")
//	$(document).trigger("OnTradeUserHistoryUpdate", notification);
//};

$.connection.hub.start().done(function () { });


function showNotificationPopup(notification) {
	var icon = 'fa-info';
	var type = 'alert-info';
	if (notification.Type == 1) {
		icon = 'fa-check';
		type = 'alert-success';
	}
	if (notification.Type == 2) {
		icon = 'fa-exclamation-triangle';
		type = 'alert-warning';
	}
	if (notification.Type == 3) {
		icon = 'fa-times-circle';
		type = 'alert-danger';
	}
	$.jGrowl(htmlEncode(notification.Message), { position: "bottom-left", header: htmlEncode(notification.Title), icon: icon, type: type });
}


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

function postJson(url, vars, callback, errorCallback) {
	$.ajax({
		url: url,
		cache: false,
		async: true,
		type: "POST",
		dataType: 'json',
		data: vars,
		success: function (response, textStatus, jqXHR) {
			if (callback) {
				callback(response);
			}
		},
		error: function (jqXHR, textStatus, errorThrown) {
			if (errorCallback) {
				errorCallback(jqXHR, textStatus, errorThrown);
			}
		}
	});
}

function getJson(url, vars, callback, errorCallback) {
	$.ajax({
		url: url,
		cache: false,
		async: true,
		type: "GET",
		dataType: 'json',
		data: vars,
		success: function (response, textStatus, jqXHR) {
			if (callback) {
				callback(response);
			}
		},
		error: function (jqXHR, textStatus, errorThrown) {
			if (errorCallback) {
				errorCallback(jqXHR, textStatus, errorThrown);
			}
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
				onClose: function (dialog, result) {
					dialog.container.fadeOut(50, function () {
						dialog.overlay.fadeOut(200, function () {
							$.modal.close();
							if ($.isFunction(callback)) {
								callback(result);
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
	var data = '<div style="width:100% !important;height:100%"><div class="modal-dialog"><div class="modal-content"><div class="modal-header"><button class="close simplemodal-close" aria-hidden="true" type="button">×</button><h4 class="modal-title header"></h4></div><div class="modal-body"><span  class="message" ></span></div><div class="modal-footer"><button style="width:70px" class="btn btn-info yes " type="button">Yes</button><button style="width:70px" class="btn btn-info simplemodal-close no" type="button">No</button></div></div></div></div>';
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
	var data = '<div style="min-width:300px"  class="modal-dialog"><div class="modal-content"><div class="modal-header"><button class="close simplemodal-close" aria-hidden="true" type="button">×</button><h4 class="modal-title header"></h4></div><div class="modal-body text-center"><span class="message" ></span></div><div class="modal-footer"><button style="width:70px" class="btn btn-info ok" type="button">Ok</button></div></div></div>';
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


function htmlEncode(val) {
	return $('<div/>').text(val).html();
}

function showAlertResult(data) {
	if (data) {
		$("#alert-result").hide();
		var type = data.AlertType || "Info";
		var title = data.Title || type;
		var message = data.Message;
		var alertType = "#alert-result";
		var alertObj = $(alertType);
		alertObj.removeClass("alert-success alert-info alert-warning alert-danger").addClass("alert-" + type.toLowerCase())
		alertObj.find('h4').text(title);
		alertObj.find('p').text(message);
		alertObj.slideDown(200);
		alertObj.delay(4000).slideUp(200, function () {
			alertObj.hide();
		});
	}
}

function timeSince(time1, time2) {
	var seconds = Math.floor((time1 - time2) / 1000);

	if (seconds < 60) {
		var val = Math.floor(seconds);
		if (val > 3) {
			return '(' + Math.floor(seconds) + ' seconds ago)';
		}
		return '(just now)';
	}
	if (seconds < 3600) {
		var val = Math.floor((seconds / 60));
		if (val > 2) {
			return '(' + val + ' minutes ago)';
		}
		return '(a minute ago)';
	}
	if (seconds < 86400) {
		var val = Math.floor(((seconds / 60) / 60));
		if (val > 1) {
			return '(' + val + ' hours ago)';
		}
		return '(an hour ago)';
	}
	return '(ages ago...)';
}


function highlightChange(element, red) {
	var item = $(element);
	if (item.hasClass("greenhighlight") || item.hasClass("redhighlight")) {
		item.removeClass("greenhighlight redhighlight").addClass(red ? "redhighlight2" : "greenhighlight2");
	} else {
		item.removeClass("greenhighlight2 redhighlight2").addClass(red ? "redhighlight" : "greenhighlight")
	}
}

function truncateDecimal(decimal) {
	var value = decimal.toString();
	if (value.indexOf(".") > -1) {
		var arr = value.split('.');
		if (arr[1].length > 8) {
			arr[1] = arr[1].substring(0, 8);
			return (+(arr[0] + "." + arr[1])).toFixed(8)
		}
	}
	return (+value).toFixed(8);
}