var chatHub = $.connection.Chat;
var currentChatUser = $('#chat-table').data('user');
var chatMessageTemplate = $('#chatMessageTemplate').html();
chatHub.client.OnlineCount = function (count, timestamp) {
	$('#chat-count').text(count);
	$('#chat-table > tbody tr').each(function () {
		$(this).find('.time').each(function () {
			$(this).html(timeSince(new Date(timestamp), new Date($(this).data('val'))));
		});
	});
};
chatHub.client.Messages = function (messages, timestamp) {
	$.each(messages, function (i, message) {
		addChatMessage(message, timestamp);
	});
};
chatHub.client.NewMessage = function (message, timestamp) {
	addChatMessage(message, timestamp);
};

chatHub.client.RemoveMessage = function (id) {
	$('#message-' + id).remove();
};

$.connection.hub.start().done(function () {
	chatHub.server.getOnlineCount();
	chatHub.server.getMessages();

	$('#chat-submit').on('click', function () {
		var input = $('#chat-input');
		var message = input.val();
		if (message) {
			chatHub.server.sendMessage(message);
			input.val('');
		}
	});

	setInterval(function () {
		if ($.connection.hub.state !== $.signalR.connectionState.disconnected) {
			chatHub.server.getOnlineCount();
		}
	}, 10000);
});


function addChatMessage(message, timestamp) {
	if (message) {
		var higlight = containsName(message.Message, currentChatUser) ? 'bg-success' : '';
		$('#chat-table > tbody').append(Mustache.render(chatMessageTemplate,
		{
			highlight: higlight,
			icon: message.Icon,
			user: message.UserName,
			time: message.Timestamp,
			since: timeSince(new Date(timestamp), new Date(message.Timestamp)),
			message: message.Message,
			id: message.Id
		}));
		$('#chat-table-container').scrollTop($('#chat-table-container')[0].scrollHeight);
	}
}

function containsName(message, name) {
	var username = '@' + currentChatUser;
	var t = message.indexOf(username + ' ');
	if (message.indexOf(username + ' ') != -1) {
		return true;
	}
	if (message.indexOf(username + ',') != -1) {
		return true;
	}
	if (message === username) {
		return true;
	}
	return false;
}

function quoteMessage(user) {
	var input = $('#chat-input');
	var username = '@' + user;
	var currentValue = input.val();
	if(currentValue){
		input.val(input.val() + username + ' ');
	}else{
		input.val(username + ',');
	}
}

function removeMessage(id) {
	confirmModal('Remove Post?', 'Are you sure you want to remove this post?', function () {
		chatHub.server.removeMessage(id);
	});
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