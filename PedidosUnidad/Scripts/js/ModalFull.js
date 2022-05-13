var getFilesURL = '';
function createShowModal(Urling) {
    getFilesURL=Urling;
    var htmlin = "";
    htmlin += '<div id="mdal_scr" class="ex_full_modal" data-mdCreate="true">';
    htmlin += '   <div class="close-modal" data-mdDimiss="true">';
    htmlin += '        <div class="lr">';
    htmlin += '            <div class="rl"></div>';
    htmlin += '        </div>';
    htmlin += '    </div>';
    htmlin += '    <div id="ctn_full">';
    htmlin += '    </div>';
    htmlin += '</div>';

    $("body").append(htmlin);

    this.init();
}

var _show = 'animated bounceInUp';
var _create = 'animated fadeIn';
var _hide = 'animated bounceOutDown';
var _destroy = 'animated fadeOut';



    function openini(who, link) {
        if (link) {
            //$("*").find('.full-modal').load(noCache(link), function () { });
            $("*").find('.full-modal').load(link, function () { });
            $(who).addClass('full-modal' + _show).show();
        }
        if (!who)
            $("*").find('.full-modal').show().addClass(_show);
        $('body').css('overflow', 'hidden');
        $("*").find(who).show().addClass(_show);

        $("#ctn_full").html('<img src="../Content/img/loading_cir.gif" style="margin-left: -50px;"/>');

        //URL que mostrara la modal
        var url = getFilesURL;
        $.ajax({
            type: 'GET',
            url: url,
            success: function (data) {
                $("#ctn_full").html(data);
            },
            error: function (ex) {
                console.log(ex);
            }
        });

    }

    function close(callback) {
        $('body').css('overflow', 'auto');
        $("*").find('.full-modal').removeClass(_show).addClass(_hide);
        if (callback)
            callback();
        setTimeout(function () {
            $("*").find('.full-modal').hide();
            $('#mdal_scr').remove();
        }, 1000);
    }

    function append(who, where, link) {
        if (!link)
            $("*").find(where).append(who);
        $(who).addClass('full-modalAppend ' + _create).show();
        $("*").find('.full-modal').load(link, function () { });
        $(who).addClass('full-modalAppend ' + _create).show();
    }

    function disappend(callback) {
        $("*").find('.full-modalAppend').removeClass(_create).addClass(_destroy);
        setTimeout(function () {
            if (callback) {
                callback();
            }
            $("*").find('.full-modalAppend').hide();
        }, 1000);
}

    function init() {
        // Create full modal in mdCreate tag
        $('body').append($("*").find("[data-mdCreate='true']"));
        $("*").find("[data-mdCreate='true']").addClass('full-modal');
        $("*").find('.full-modal').hide();

        // Close modal to click in dimiss tag
        $("*").find("[data-mdDimiss='true']").on('click', function () {
            $('body').css('overflow', 'auto');
            $(this).parent().removeClass(_show).addClass(_hide);
            setTimeout(function () {
                $("*").find('.full-modal').hide();
                $('#mdal_scr').remove();
            }, 1000);
        });
        // Close append data
        $("*").find("[data-mdCloseAppend='true']").on('click', function () {
            $(this).parent().removeClass(_create).addClass(_destroy);
            setTimeout(function () {
                $("*").find('.full-modalAppend').hide();
                $('#mdal_scr').remove();
            }, 1000);
        });

        this.openini();
}

