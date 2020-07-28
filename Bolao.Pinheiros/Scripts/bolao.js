var loading = $('.lds-container');
var dataDesejada = $('#datepicker');
var dadosDosJogos = $('.dados-jogos');
var gettingUpdates = false;

$(document).ready(function () {
    Notify.requestPermission(onPermissionGranted, onPermissionDenied);    

    dataDesejada.datepicker({
        dateFormat: "dd/mm/yy"
    });

    dataDesejada.on('change', function () {
        getGamesOnDate(this.value);
    });

    $(document).on('click', '.detalhar-estatisticas', function () {
        showStatistics(this);
    });

    $(document).on('click', '.flip-card-icon', function () {
        $(this).siblings('.card').toggleClass('is-flipped');
    });

    $(document).on('click', '.detalhar-apostas', function () {
        //var estatisticas = $(this).parent().find('.apostas');
        //if (estatisticas.is(':visible')) {
        //    estatisticas.slideUp();
        //} else {
        //    estatisticas.slideDown();
        //}
    });


    $(document).on('click', '.component-live', function () {
        showLiveGames();
    });

    $(document).on('click', '.update-icon', function () {
        getStatistics(this, false);
    });

    var loadTimeout = {};
    $(document).ajaxStart(function () {
        loadTimeout = setTimeout(function () {
            if (!gettingUpdates) {
                loading.show();
            }
        }, 300);
    });
    $(document).ajaxStop(function () {
        if (!$.active) {
            loading.hide();
            clearTimeout(loadTimeout);
        }
    });

    $(document).tooltip({
        classes: { "ui-tooltip": "tooltip-bolao" },
        position: { my: "left+10 top+5", at: "left bottom", collision: "flipfit" },
        track: true
    });

    setInterval(function () {
        $('.game-live .game-card-status-badge').toggleClass('badge-visible');
    }, 800);

    setInterval(function () {
        getGamesDataUpdate();
    }, 10000);

    setEvents();
});

var getGamesOnDate = function () {
    if (dataDesejada.val() === '') {
        return;
    }

    try {
        $.ajax({
            url: 'Home/GetGamesData',
            type: 'POST',
            dataType: 'html',
            cache: false,
            data: {
                date: dataDesejada.val()
            },
            beforeSend: function () {
                loading.show();
            }
        }).done(function (result) {
            dadosDosJogos.html(result);
            setEvents();
        }).fail(function (ex) {
            // TODO
        });
    } catch (error) {
        // TODO
    }
};

var getGamesDataUpdate = function () {
    if ($.active) {
        return;
    }

    try {
        gettingUpdates = true;
        $.ajax({
            url: 'Home/GetGamesDataUpdate',
            type: 'POST',
            dataType: 'html',
            cache: false,
            data: {
                date: dataDesejada.val()
            }
        }).done(function (result) {
            gettingUpdates = false;
            updateGamesPlaying(JSON.parse(result));
        }).fail(function (ex) {
            // TODO
        });
    } catch (error) {
        // TODO
    }
};

var getStatistics = function (element, show) {
    try {
        var gameId = $(element).parents('.container-jogo').attr('id');

        $.ajax({
            url: 'Home/GetStatistics',
            type: 'POST',
            dataType: 'html',
            cache: false,
            data: {
                gameId: gameId
            },
            beforeSend: function () {
                loading.show();
            }
        }).done(function (result) {
            var parent = $(element).parents('.container-jogo');
            if (show) {
                var statistics = parent.find('.detalhar-estatisticas');
                statistics.after(result);
                showStatistics(element);
            } else {
                parent.find('.container-estatisticas').html($(result).html());
            }
        }).fail(function (ex) {
            // TODO
        });
    } catch (error) {
        // TODO
    }
};

var setGamesVisible = function () {
    $('.games-count').html('(' + $('.container-jogo').filter(':visible').length + ')');
};

var setEvents = function () {
    $('.select2-component').select2();
    $('.select2-component').show();
    $('[name="competition-selector"]').on('change', function () {
        showCompetitionsSelected();
    });

    $('[name="games-selector"]').on('change', function () {
        showGamesSelected();
    });
};

var showCompetitionsSelected = function () {
    var competitions = $('.competition');
    competitions.hide();

    var competitionsSelected = $('[name="competition-selector"]').val();
    if (competitionsSelected === ''
        || competitionsSelected.length === 0) {
        competitions.show();
    } else {
        var selector = '';
        for (var i = 0; i < competitionsSelected.length; i++) {
            selector += '#' + competitionsSelected[i] + ',';
        }

        selector = selector.slice(0, -1);
        $(selector).show();
    }

    setGamesVisible();
};

var showGamesSelected = function () {
    var games = $('.container-jogo');

    var competitions = $('.competition');
    competitions.show();
    games.hide();

    var gamesSelected = gamesSelector.val();
    if (gamesSelected === ''
            || gamesSelected.length === 0) {
        games.show();
        competitions.show();
    } else {
        var selector = '';
        for (var i = 0; i < gamesSelected.length; i++) {
            selector += '#' + gamesSelected[i] + ',';
        }

        selector = selector.slice(0, -1);
        $(selector).show();

        showOrHideCompetitions();
    }

    setGamesVisible();
};

var showLiveGames = function () {
    var liveClass = 'live-active';
    var liveButton = $('.live-button').toggleClass(liveClass);
    var containerJogos = $('.container-jogo');
    containerJogos.show();
    if (liveButton.hasClass(liveClass)) {
        for (var i = 0; i < containerJogos.length; i++) {
            var isLive = $(containerJogos[i]).find('.game-playing').length;
            if (isLive) {
                $(containerJogos[i]).show();
            } else {
                $(containerJogos[i]).hide();
            }
        }
    } else {
        containerJogos.show();
    }

    setGamesVisible();
    showCompetitionsSelected();
    showOrHideCompetitions();
};

var showNotification = function (title, content) {

};

var showOrHideCompetitions = function () {
    var competitions = $('.competition');
    for (var c = 0; c < competitions.length; c++) {
        if ($(competitions[c]).find('.container-jogo:visible').length === 0) {
            $(competitions[c]).hide();
        }
    }
};

var showStatistics = function (element) {
    var statistics = $(element).parents('.container-jogo').find('.container-estatisticas');
    if (statistics.length) {
        if (statistics.is(':visible')) {
            statistics.slideUp();
        } else {
            statistics.slideDown();
        }
    } else {
        getStatistics(element, true);
    }
};

var updateGamesPlaying = function (result) {
    if (result === null
        || result.games === null) {
        return;
    }

    for (var i = 0; i < result.games.length; i++) {
        var gameId = result.games[i].id;
        var gameContainer = $('#' + gameId);
        if (gameContainer.length && gameContainer.is(':visible')) {
            var gameLive = result.games[i].gameTimeAndStatusDisplayType !== 1;
            var gamePlaying = result.games[i].gameTimeAndStatusDisplayType === 2;
            var gameStatus = gameContainer.find('.game-card-status-badge');
            if (gamePlaying) {
                if (!gameStatus.parent().hasClass('game-live')) {
                    gameStatus.parent().addClass('game-live');
                }

                gameStatus.html(result.games[i].gameTimeDisplay.replace('\'', ''));
                var score = gameContainer.find('.game-card-content-score');

                var home = result.games[i].homeCompetitor;
                var away = result.games[i].awayCompetitor;
                if (home.score >= 0) {
                    var scoreText = home.score + ' - ' + away.score;
                    if (!Notify.needsPermission && score.html().trim() !== scoreText) {
                        var gameText = home.name + ' - ' + away.name;
                        doNotification(gameText, scoreText);
                    }

                    score.html(scoreText);
                }
                score.html();
            } else {
                gameStatus.removeClass('game-live');
                if (gameLive) {
                    if (!gameStatus.hasClass('game-playing')) {
                        gameStatus.addClass('game-playing');
                    }
                } else {
                    gameStatus.removeClass('game-playing');
                }

                if (result.games[i].winDescription !== null
                        && result.games[i].winDescription !== '') {
                    gameStatus.html(result.games[i].winDescription);
                } else {
                    gameStatus.html(result.games[i].statusText);
                }                
            }
        }
    }
};

/* Notification */
function onShowNotification() {
    console.log('notification is shown!');
}

function onCloseNotification() {
    console.log('notification is closed!');
}

function onClickNotification() {
    console.log('notification was clicked!');
}

function onErrorNotification() {
    console.error('Error showing notification. You may need to request permission.');
}

function onPermissionGranted() {
    console.log('Permission has been granted by the user');
}

function onPermissionDenied() {
    console.warn('Permission has been denied by the user');
}

function doNotification(title, content) {
    var myNotification = new Notify(title, {
        body: content,
        notifyShow: onShowNotification,
        notifyClose: onCloseNotification,
        notifyClick: onClickNotification,
        notifyError: onErrorNotification,
        timeout: 4
    });

    myNotification.show();
}