function contact_num_valid(e) {
    var r = e || window.event;
    if ("paste" === r.type) t = event.clipboardData.getData("text/plain");
    else {
        var t = r.keyCode || r.which;
        t = String.fromCharCode(t)
    }
    if ((e.target.value.match(/\+/g) || []).length < 2 && "+" == t) return e.target.value = e.target.value.replace(/\+/g, ""), e.target.value = "+" + e.target.value, r.returnValue = !1, r.preventDefault && r.preventDefault(), !1;
    /[+0-9]|\./.test(t) || (r.returnValue = !1, r.preventDefault && r.preventDefault())
}
jQuery("#Career").submit(function(e) {
    if (jQuery(this).find('input[type="password"],input[type="text"],input[type="number"],input[type="tel"],input[type="email"],textarea').each(function() {
            jQuery(this).val($.trim(jQuery(this).val()))
        }), 1 == function() {
            var e = document.querySelector("#Career #name"),
                r = document.querySelector("#Career #email"),
                t = document.querySelector("#Career #contact_no");
            if ("" == e.value) return document.querySelector("#Career #error_data").innerHTML = "* Please Enter Name.", e.style.borderColor = "red", e.focus(), !1;
            e.style.borderColor = "";
            var a = e.value;
            if (!/^[a-zA-Z-,]+(\s{0,1}[a-zA-Z-, ])+(\s{0,1}[a-zA-Z-, ])*$/.test(a)) return document.querySelector("#Career #error_data").innerHTML = "* Invalid Name: " + e.value, e.style.borderColor = "red", e.value = "", e.focus(), !1;
            if (e.style.borderColor = "", "" == r.value) return document.querySelector("#Career #error_data").innerHTML = "* Please Enter Email ID.", r.style.borderColor = "red", r.focus(), !1;
            r.style.borderColor = "";
            var o = r.value;
            if (0 == /^([A-Za-z0-9_\-\.])+\@([A-Za-z0-9_\-\.])+\.([A-Za-z]{2,4})$/.test(o)) return document.querySelector("#Career #error_data").innerHTML = "* Invalid Email ID: " + r.value, r.style.borderColor = "red", r.value = "", r.focus(), !1;
            if (r.style.borderColor = "", "" == t.value) return document.querySelector("#Career #error_data").innerHTML = "* Please Enter Contact No.", t.style.borderColor = "red", t.focus(), !1;
            t.style.borderColor = "";
            var l = document.querySelector("#Career #resume");
            if ("" == l.value) return document.querySelector("#Career #error_data").innerHTML = "* Please Upload your Resume.", l.style.borderColor = "red", l.focus(), !1;
            if (t.style.borderColor = "", !/([a-zA-Z0-9\s_\\.\-:])+(.doc|.docx|.pdf)$/.test(l.value)) return document.querySelector("#Career #error_data").innerHTML = "* Please Upload Resume only In doc | docx | pdf Format!", l.style.borderColor = "red", l.focus(), !1;
            t.style.borderColor = "";
            var u = t.value.replace(/\+/g, "");
            return /^(?!(\d)\1+\b|1234567890)\d{10,}$/.test(u) ? (t.style.borderColor = "", document.querySelector("#Career #error_data").innerHTML = "", !0) : (document.querySelector("#Career #error_data").innerHTML = "* Invalid Contact No.: " + t.value, t.style.borderColor = "red", t.value = "", t.focus(), !1)
        }()) {
        document.querySelector("#Career #form_process").style.visibility = "visible", jQuery(this).find('[type="submit"]').prop("disabled", !0);
        var r = jQuery("#Career").attr("action");
        $.ajax({
            type: "POST",
            url: r,
            enctype: "multipart/form-data",
            data: new FormData(this),
            contentType: !1,
            cache: !1,
            processData: !1,
            success: function(e) {
                jQuery("#Career").empty(), jQuery("#Career").html(e)
            },
            error: function(e) {
                jQuery("#Career").empty(), jQuery("#Career").html("<div class='alert alert-danger'>Sorry! Some Technical issue occured. Please try again after sometime.</div>")
            }
        }), e.preventDefault()
    } else e.preventDefault()
});