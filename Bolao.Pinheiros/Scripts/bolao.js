var loading = $('.lds-container');
var dataDesejada = $('#datepicker');
var dadosDosJogos = $('.dados-jogos');
var jogos = [];

$(document).ready(function () {
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

    $(document).ajaxStart(function () {
        loading.show();
    });
    $(document).ajaxStop(function () {
        if (!$.active) {
            loading.hide();
        }
    });

    $(document).tooltip({
        classes: {
            "ui-tooltip": "tooltip-bolao"
        }
    });
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
            }
        }).done(function (result) {
            dadosDosJogos.html(result);
        }).fail(function (ex) {
            // TODO
        });
    } catch (error) {
        // TODO
    }
};

var getStatistics = function (element) {
    try {
        var gameId = $(element).parents('.container-jogo').attr('id');

        $.ajax({
            url: 'Home/GetStatistics',
            type: 'POST',
            dataType: 'html',
            cache: false,
            data: {
                gameId: gameId
            }
        }).done(function (result) {
            var statistics = $(element).parent().find('.detalhar-estatisticas');
            statistics.after(result);

            showStatistics(element);
        }).fail(function (ex) {
            // TODO
        });
    } catch (error) {
        // TODO
    }
};

var showStatistics = function (element) {
    var statistics = $(element).parent().find('.container-estatisticas');
    if (statistics.length) {
        if (statistics.is(':visible')) {
            statistics.slideUp();
        } else {
            statistics.slideDown();
        }
    } else {
        getStatistics(element);
    }
};