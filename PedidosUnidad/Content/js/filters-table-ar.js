var filterTable = function (filter,table) {
    $("#" + filter).keyup(function (e) {
        _this = this;        
        $("#" + table + " tbody tr").each(function (i,e) {
            if ($(this).text().toLowerCase().indexOf(_this.value.toLowerCase()) === -1)
                $(this).hide();
            else
                $(this).show();
        });
    });
};

var filterDiv = function (filter, div) {
    $("#" + filter).keyup(function (e) {
        _this = this;
        $("#" + div + " .content-parent").each(function (i, e) {
            if ($(this).text().toLowerCase().indexOf(_this.value.toLowerCase()) === -1)
                $(this).hide();
            else
                $(this).show();
        });
    });
};

var filterDinamico = function (filter, condicion1, condicion2) {
    $("#" + filter).keyup(function (e) {
        _this = this;
        $(condicion1 + " " + condicion2).each(function (i, e) {
            if ($(this).text().toLowerCase().indexOf(_this.value.toLowerCase()) === -1)
                $(this).hide();
            else
                $(this).show();
        });
    });
};