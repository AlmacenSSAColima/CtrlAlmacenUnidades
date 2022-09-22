// <reference path="upLoadDocument.js" />
var multiuploadFinished = false;
var cantidad_archivos = 0;
var referencia_seleccionada = "";
var observaciones_imagenes = "";
var button = document.getElementById('upload-button');
var wave = document.getElementById('wave');
var progress = document.getElementById('progress');
var self;
var inputId;
function multiUploader(config) {

    this.config = config;
    this.items = "";
    this.all = []
    self = this;

    multiUploader.prototype._init = function () {
        if (window.File &&
			window.FileReader &&
			window.FileList &&
			window.Blob) {
            inputId = $("#" + this.config.form).find("input[type='file']").eq(0).attr("id");
            document.getElementById(inputId).addEventListener("change", this._read, false);
            document.getElementById(this.config.dragArea).addEventListener("dragover", function (e) { e.stopPropagation(); e.preventDefault(); }, false);
            document.getElementById(this.config.dragArea).addEventListener("drop", this._dropFiles, false);
            document.getElementById(this.config.form).addEventListener("submit", this._submit, false);
        } else
            console.log("Browser supports failed");
    }

    multiUploader.prototype._submit = function (e) {
        e.stopPropagation(); e.preventDefault();
        self._startUpload();
    }

    multiUploader.prototype._preview = function (data) {
        this.items = data;
        if (this.items.length > 0) {
            var html = "";
            var uId = "";

            for (var i = 0; i < this.items.length; i++) {
                console.log(this.items[i].name);
                uId = this.items[i].name._unique();
                var sampleIcon = '<img src="../Content/MultiUploader/img/image.png" onclick="deleteImgPre(\'' + uId.trim() + '\');"/>'; //onclick="deleteImgPre(\'' + uId.trim() + '\');"
                var errorClass = "";

                if (typeof this.items[i] != undefined) {
                    if (self._validate(this.items[i].type) <= 0) {
                        sampleIcon = '<img src="../Content/MultiUploader/img/unknown.png" />';
                        errorClass = " invalid";
                    }
                    //html += '<div class="row dfiles' + errorClass + '" rel="' + uId + '"><div class="col-md-12"><h5>' + sampleIcon + this.items[i].name + '</h5><div id="' + uId + '" class="progress" style="display:none;"><img src="../../Content/asset/img/ajax-loader.gif" /></div></div><div class="col-md-12"><label>Comentario:</label><input type="text" class="form-control warning"/></div></div>';
                    html += '<div id="img_' + uId.trim() + '" class="dfiles ' + errorClass + '" rel="' + uId + '" style="height: 33px;">';
                    html += '   <div class="twelve columns">'
                    html += '       <h5>' + sampleIcon + this.items[i].name + '</h5>'
                    html += '       <div id="' + uId + '" class="progress" style="display: none; ">'
                    html += '           <img src="../Content/MultiUploader/img/ajax-loader.gif" />'
                    html += '       </div >'
                    html += '   </div >'
                    html += '   <div class"twelve columns" style = "display: none; " >'
                    html += '       <label> Comentario:</label > <input id="No_referencia" type="text" class="comment" />'
                    html += '   </div >'
                    html += '</div > ';
                }
            }
            cantidad_archivos = cantidad_archivos + this.items.length;
            $("#dragAndDropFiles").append(html);
            $("#ctdadFiles").html(cantidad_archivos);

        }
    }

    multiUploader.prototype._read = function (evt) {

        if (evt.target.files) {
            self._preview(evt.target.files);
            self.all.push(evt.target.files);
        } else
            console.log("Failed file reading");
    }

    multiUploader.prototype._validate = function (format) {
        var arr = this.config.support.split(",");
        if (this.config.noFilter)
            return 1;
        else
            return arr.indexOf(format);
    }

    multiUploader.prototype._dropFiles = function (e) {
        console.log(this.all);
        e.stopPropagation(); e.preventDefault();
        self._preview(e.dataTransfer.files);
        self.all.push(e.dataTransfer.files);
    }

    multiUploader.prototype._uploader = function (file, f) {

        if (typeof file[f] != undefined && self._validate(file[f].type) > 0) {
            var data = new FormData();
            var ids = file[f].name._unique();
            var comm = observaciones_imagenes;//$(".dfiles[rel='" + ids + "']").find(".comment").val();
            var referencia_no = referencia_seleccionada;
            //console.log(this.config.uploadUrl);
            data.append('file', file[f]);
            data.append('index', ids);
            data.append('reference', referencia_no);
            data.append('comment', comm);
            $(".dfiles[rel='" + ids + "']").find(".progress").show();
            $.ajax({
                type: "POST",
                url: this.config.uploadUrl,
                data: data,
                cache: false,
                contentType: false,
                processData: false,
                async: false,
                success: function (rponse) {
                    $("#" + ids).hide();
                    var obj = $(".dfiles").get();
                    console.log("OBJ UPLOADER: " + obj);
                    $.each(obj, function (k, fle) {
                       
                        if ($(fle).attr("rel").trim() == rponse.trim()) {
                            console.log($(fle).attr("rel").trim() + " = " + rponse.trim());
                            
                            $(fle).remove();
                            startUpload(wave, progress);

                            var porcEn = parseInt(100 / file.length);
                            SubirPorce(porcEn, file.length);
                        }
                    });
                    if (f + 1 < file.length) {
                        self._uploader(file, f + 1);
                    }

                },                
                error: function (ex) {
                    console.log(ex);
                }
            });
        } else
            console.log("Formato de archivo inválido - " + file[f].name);
    }

    multiUploader.prototype._deleteItem = function () {
        if (this.all.length > 0) {
            for (var k = 0; k < this.all.length; k++) {
                var file = this.all[k];
                console.log("DELETY...");
                this._uploader(file, 0);
            }
        }
    }

    multiUploader.prototype._startUpload = function () {
        //console.log(this.all);
        if (this.all.length > 0) {
            for (var k = 0; k < this.all.length; k++) {
                var file = this.all[k];
                this._uploader(file, 0);
            }
        }
    }

    String.prototype._unique = function () {
        return this.replace(/[a-zA-Z]/g, function (c) {
            return String.fromCharCode((c <= "Z" ? 90 : 122) >= (c = c.charCodeAt(0) + 13) ? c : c - 26);
        });
    }

    this._init();
}

function initMultiUploader() {
    new multiUploader(config);
}



function deleteImgPre(id_) {
    console.log("ELIMINAR: " + id_);
    console.log("SELF ALL: " + self.all);

    var obj = $(".dfiles").get();
    $.each(obj, function (k, fle) {
        console.log($(fle).attr("rel").trim() +"=="+ id_.trim());
        if ($(fle).attr("rel").trim() == id_.trim()) {
            $(fle).remove();
        }
    });

    console.log("INPUT FILE: " + inputId);

    self.all.push(e.dataTransfer.files);
    $("#demoFiler").find("input[type='file']").eq(0).forEach(function (file, index) {
        console.log("FILE: " + index + " NOMBRE " + file.name);
    });


    //$("#" + inputId)
    //self.all.files.forEach(function (file, index) {
    //    console.log("FILE: " + index + " NOMBRE " + file.name);
    //});

    //Array.from(self.all).forEach(file => {
    //   console.log("NOMBRE FILE: ");
    //});

    if (self.all.length > 0) {
        for (var k = 0; k < self.all.length; k++) {
            var file = self.all[k];
            //self.all.splice(k, 1);

            //console.log("QUEDAN: " + self.all.length);
            //console.log("FILE archivo: " + file);
        }
    }


    //$("#" + id_).remove();
}

function startUpload(wave, progress) {
    $("#submitHandler").click();
}

var porcienti = 0;
var totalFiles = 0;
function SubirPorce(porcentaje, lengFiles) {
    porcienti = porcienti + porcentaje;
    console.log("Porcentaje: " + porcienti);
    wave.style.height = porcienti+"%";
    progress.innerText = porcienti + "%";

    totalFiles = totalFiles + 1;
    $("#ctdadFiles").html(totalFiles + " de " + lengFiles);
    if (totalFiles == lengFiles) {
        setTimeout(function () {
            progress.innerText = "COMPLETO";
            wave.className = wave.className + " completed";
            show_exito("Imagenes cargadas correctamente");

            $("#buttonNuevaCarga").css("display","block");

            setTimeout(function () {
            //$("#upload-button").removeClass("hide");
            //$("#ctdadFiles").html(" 0 ");
            //$("#ref_selecto").html("- - - -");
            //$("#referencia_pics").val("");
            //$("#observaciones_pics").val("");
            //wave.style.height = "0%";
            //progress.innerText = "0%";
            }, 400);

        }, 500);
    }
}



//Start Upload
button.addEventListener("click", function () {
    var goTo = true;
    console.log("referencia " + $("#referencia_pics").val().length + ", observa " + $("#observaciones_pics").val().length);
    if ($("#referencia_pics").val().length == 0) {
        show_info("Por favor selecciona una referencia");
        goTo = false;
        return false;
    }
    if (cantidad_archivos == 0) {
        show_info("Debes cargar minimo un archivo tipo imagen");
        goTo = false;
        return false;
    }
    if ($("#observaciones_pics").val().length == 0) {
        show_info("Porfavor registra una observación");
        goTo = false;
        return false;
    }

    

    if (goTo == true) {
        referencia_seleccionada = $("#referencia_pics").val();
        observaciones_imagenes = $("#observaciones_pics").val();
        button.className = "hide";
        startUpload(wave, progress);
    }
    
});
