! function (e) {
    function t(t) {
        let n = window.location.href.split("/").reverse()[0];
        t.find("li").each(function () {
            let t = e(this).find("a");
            e(t).attr("href") == n && e(this).addClass("current")
        }), t.children("li").each(function () {
            e(this).find(".current").length && e(this).addClass("current")
        }), "" == n && t.find("li").eq(0).addClass("current")
    }
    if (e("body").append('<div class="right-side-sticker"><div class="xs-display-none book_appoinment"><a class="" href="https://kd.doctor9.com/default.htm" target="_blank"></a></div><div class="left-side-social-icons"><div class="we-social xs-display-none"></div><div class="kd-facebook xs-display-none"></div><div class="kd-twitter xs-display-none"></div><div class="kd-instagram xs-display-none"></div><div class="kd-linkedin xs-display-none"></div><div class="kd-blog xs-display-none"></div></div>'), e(".curved-circle").length && e(".curved-circle").circleType({
        position: "absolute",
        dir: .95,
        radius: 85,
        forceHeight: !1,
        forceWidth: !0
    }), e(".main-nav__main-navigation li.dropdown ul").length && e(".main-nav__main-navigation li.dropdown").append('<button class="dropdown-btn"><i class="fa fa-angle-right"></i></button>'), e(".main-nav__main-navigation").length) {
        let n = e(".mobile-nav__container"),
            i = e(".main-nav__main-navigation").html();
        n.append(function () {
            return i
        }), n.find("li.dropdown .dropdown-btn").on("click", function () {
            e(this).toggleClass("open"), e(this).prev("ul").slideToggle(500)
        });
        let a = e(".main-nav__main-navigation").find(".main-nav__navigation-box"),
            o = n.find(".main-nav__navigation-box");
        t(a), t(o)
    }
    if (e(".mc-form").length) {
        var n = e(".mc-form").data("url");
        e(".mc-form").ajaxChimp({
            url: n,
            callback: function (t) {
                e(".mc-form__response").append(function () {
                    return '<p class="mc-message">' + t.msg + "</p>"
                }), "success" === t.result && (e(".mc-form").removeClass("errored").addClass("successed"), e(".mc-form__response").removeClass("errored").addClass("successed"), e(".mc-form").find("input").val(""), e(".mc-form__response p").fadeOut(1e4)), "error" === t.result && (e(".mc-form").removeClass("successed").addClass("errored"), e(".mc-form__response").removeClass("successed").addClass("errored"), e(".mc-form").find("input").val(""), e(".mc-form__response p").fadeOut(1e4))
            }
        })
    }
    if (e(".datepicker").length && e(".datepicker").datepicker(), e(".plan-visit__tab-links").length) {
        var i = e(".plan-visit__tab-links").find(".nav-link");
        i.on("click", function (t) {
            var n = e(this).attr("data-target");
            return e("html, body").animate({
                scrollTop: e(n).offset().top - 50
            }, 1e3), i.removeClass("active"), e(this).addClass("active"), !1
        })
    }
    if (e(".contact-form-validated").length && e(".contact-form-validated").validate({
        rules: {
            fname: {
                required: !0
            },
            lname: {
                required: !0
            },
            name: {
                required: !0
            },
            email: {
                required: !0,
                email: !0
            },
            service: {
                required: !0
            },
            message: {
                required: !0
            },
            subject: {
                required: !0
            }
        },
        submitHandler: function (t) {
            return e.post(e(t).attr("action"), e(t).serialize(), function (n) {
                e(t).parent().find(".result").append(n), e(t).find('input[type="text"]').val(""), e(t).find('input[type="email"]').val(""), e(t).find("textarea").val("")
            }), !1
        }
    }), e(".counter").length && e(".counter").counterUp({
        delay: 10,
        time: 3e3
    }), e(".img-popup").length) {
        var a = {};
        e(".img-popup").each(function () {
            var t = parseInt(e(this).attr("data-group"), 10);
            a[t] || (a[t] = []), a[t].push(this)
        }), e.each(a, function () {
            e(this).magnificPopup({
                type: "image",
                closeOnContentClick: !0,
                closeBtnInside: !1,
                gallery: {
                    enabled: !0
                }
            })
        })
    }
    e(".img-popup1-new").length && (a = {}, e(".img-popup1-new").each(function () {
        var t = parseInt(e(this).attr("data-group"), 10);
        a[t] || (a[t] = []), a[t].push(this)
    }), e.each(a, function () {
        e(this).magnificPopup({
            type: "image",
            closeOnContentClick: !0,
            closeBtnInside: !1,
            gallery: {
                enabled: !0
            }
        })
    })), e(".img-popup4-new").length && (a = {}, e(".img-popup4-new").each(function () {
        var t = parseInt(e(this).attr("data-group"), 10);
        a[t] || (a[t] = []), a[t].push(this)
    }), e.each(a, function () {
        e(this).magnificPopup({
            type: "image",
            closeOnContentClick: !0,
            closeBtnInside: !1,
            gallery: {
                enabled: !0
            }
        })
    })), e(".img-popup5-new").length && (a = {}, e(".img-popup5-new").each(function () {
        var t = parseInt(e(this).attr("data-group"), 10);
        a[t] || (a[t] = []), a[t].push(this)
    }), e.each(a, function () {
        e(this).magnificPopup({
            type: "image",
            closeOnContentClick: !0,
            closeBtnInside: !1,
            gallery: {
                enabled: !0
            }
        })
    })), e(".img-popup3-new").length && (a = {}, e(".img-popup3-new").each(function () {
        var t = parseInt(e(this).attr("data-group"), 10);
        a[t] || (a[t] = []), a[t].push(this)
    }), e.each(a, function () {
        e(this).magnificPopup({
            type: "image",
            closeOnContentClick: !0,
            closeBtnInside: !1,
            gallery: {
                enabled: !0
            }
        })
    })), e(".img-popup4-new1").length && (a = {}, e(".img-popup4-new1").each(function () {
        var t = parseInt(e(this).attr("data-group"), 10);
        a[t] || (a[t] = []), a[t].push(this)
    }), e.each(a, function () {
        e(this).magnificPopup({
            type: "image",
            closeOnContentClick: !0,
            closeBtnInside: !1,
            gallery: {
                enabled: !0
            }
        })
    })), e(".img-popup2-new").length && (a = {}, e(".img-popup2-new").each(function () {
        var t = parseInt(e(this).attr("data-group"), 10);
        a[t] || (a[t] = []), a[t].push(this)
    }), e.each(a, function () {
        e(this).magnificPopup({
            type: "image",
            closeOnContentClick: !0,
            closeBtnInside: !1,
            gallery: {
                enabled: !0
            }
        })
    })), e(".img-popup1").length && (a = {}, e(".img-popup1").each(function () {
        var t = parseInt(e(this).attr("data-group"), 10);
        a[t] || (a[t] = []), a[t].push(this)
    }), e.each(a, function () {
        e(this).magnificPopup({
            type: "image",
            closeOnContentClick: !0,
            closeBtnInside: !1,
            gallery: {
                enabled: !0
            }
        })
    }), e(".img-popup2").length && (a = {}, e(".img-popup2").each(function () {
        var t = parseInt(e(this).attr("data-group"), 10);
        a[t] || (a[t] = []), a[t].push(this)
    }), e.each(a, function () {
        e(this).magnificPopup({
            type: "image",
            closeOnContentClick: !0,
            closeBtnInside: !1,
            gallery: {
                enabled: !0
            }
        })
    })), e(".img-popup3").length && (a = {}, e(".img-popup3").each(function () {
        var t = parseInt(e(this).attr("data-group"), 10);
        a[t] || (a[t] = []), a[t].push(this)
    }), e.each(a, function () {
        e(this).magnificPopup({
            type: "image",
            closeOnContentClick: !0,
            closeBtnInside: !1,
            gallery: {
                enabled: !0
            }
        })
    })), e(".img-popup4").length && (a = {}, e(".img-popup4").each(function () {
        var t = parseInt(e(this).attr("data-group"), 10);
        a[t] || (a[t] = []), a[t].push(this)
    }), e.each(a, function () {
        e(this).magnificPopup({
            type: "image",
            closeOnContentClick: !0,
            closeBtnInside: !1,
            gallery: {
                enabled: !0
            }
        })
    })), e(".img-popup5").length && (a = {}, e(".img-popup5").each(function () {
        var t = parseInt(e(this).attr("data-group"), 10);
        a[t] || (a[t] = []), a[t].push(this)
    }), e.each(a, function () {
        e(this).magnificPopup({
            type: "image",
            closeOnContentClick: !0,
            closeBtnInside: !1,
            gallery: {
                enabled: !0
            }
        })
    })), e(".img-popup6").length && (a = {}, e(".img-popup6").each(function () {
        var t = parseInt(e(this).attr("data-group"), 10);
        a[t] || (a[t] = []), a[t].push(this)
    }), e.each(a, function () {
        e(this).magnificPopup({
            type: "image",
            closeOnContentClick: !0,
            closeBtnInside: !1,
            gallery: {
                enabled: !0
            }
        })
    })), e(".img-popup7").length && (a = {}, e(".img-popup7").each(function () {
        var t = parseInt(e(this).attr("data-group"), 10);
        a[t] || (a[t] = []), a[t].push(this)
    }), e.each(a, function () {
        e(this).magnificPopup({
            type: "image",
            closeOnContentClick: !0,
            closeBtnInside: !1,
            gallery: {
                enabled: !0
            }
        })
    })), e(".img-popup8").length && (a = {}, e(".img-popup8").each(function () {
        var t = parseInt(e(this).attr("data-group"), 10);
        a[t] || (a[t] = []), a[t].push(this)
    }), e.each(a, function () {
        e(this).magnificPopup({
            type: "image",
            closeOnContentClick: !0,
            closeBtnInside: !1,
            gallery: {
                enabled: !0
            }
        })
    })), e(".img-popup9").length && (a = {}, e(".img-popup9").each(function () {
        var t = parseInt(e(this).attr("data-group"), 10);
        a[t] || (a[t] = []), a[t].push(this)
    }), e.each(a, function () {
        e(this).magnificPopup({
            type: "image",
            closeOnContentClick: !0,
            closeBtnInside: !1,
            gallery: {
                enabled: !0
            }
        })
    })), e(".img-popup10").length && (a = {}, e(".img-popup10").each(function () {
        var t = parseInt(e(this).attr("data-group"), 10);
        a[t] || (a[t] = []), a[t].push(this)
    }), e.each(a, function () {
        e(this).magnificPopup({
            type: "image",
            closeOnContentClick: !0,
            closeBtnInside: !1,
            gallery: {
                enabled: !0
            }
        })
    })), e(".img-popup11").length && (a = {}, e(".img-popup11").each(function () {
        var t = parseInt(e(this).attr("data-group"), 10);
        a[t] || (a[t] = []), a[t].push(this)
    }), e.each(a, function () {
        e(this).magnificPopup({
            type: "image",
            closeOnContentClick: !0,
            closeBtnInside: !1,
            gallery: {
                enabled: !0
            }
        })
    })), e(".img-popup12").length && (a = {}, e(".img-popup12").each(function () {
        var t = parseInt(e(this).attr("data-group"), 10);
        a[t] || (a[t] = []), a[t].push(this)
    }), e.each(a, function () {
        e(this).magnificPopup({
            type: "image",
            closeOnContentClick: !0,
            closeBtnInside: !1,
            gallery: {
                enabled: !0
            }
        })
    })), e(".img-popup13").length && (a = {}, e(".img-popup13").each(function () {
        var t = parseInt(e(this).attr("data-group"), 10);
        a[t] || (a[t] = []), a[t].push(this)
    }), e.each(a, function () {
        e(this).magnificPopup({
            type: "image",
            closeOnContentClick: !0,
            closeBtnInside: !1,
            gallery: {
                enabled: !0
            }
        })
    })), e(".img-popup14").length && (a = {}, e(".img-popup14").each(function () {
        var t = parseInt(e(this).attr("data-group"), 10);
        a[t] || (a[t] = []), a[t].push(this)
    }), e.each(a, function () {
        e(this).magnificPopup({
            type: "image",
            closeOnContentClick: !0,
            closeBtnInside: !1,
            gallery: {
                enabled: !0
            }
        })
    })), e(".img-popup15").length && (a = {}, e(".img-popup15").each(function () {
        var t = parseInt(e(this).attr("data-group"), 10);
        a[t] || (a[t] = []), a[t].push(this)
    }), e.each(a, function () {
        e(this).magnificPopup({
            type: "image",
            closeOnContentClick: !0,
            closeBtnInside: !1,
            gallery: {
                enabled: !0
            }
        })
    })), e(".img-popup16").length && (a = {}, e(".img-popup16").each(function () {
        var t = parseInt(e(this).attr("data-group"), 10);
        a[t] || (a[t] = []), a[t].push(this)
    }), e.each(a, function () {
        e(this).magnificPopup({
            type: "image",
            closeOnContentClick: !0,
            closeBtnInside: !1,
            gallery: {
                enabled: !0
            }
        })
    })), e(".img-popup17").length && (a = {}, e(".img-popup17").each(function () {
        var t = parseInt(e(this).attr("data-group"), 10);
        a[t] || (a[t] = []), a[t].push(this)
    }), e.each(a, function () {
        e(this).magnificPopup({
            type: "image",
            closeOnContentClick: !0,
            closeBtnInside: !1,
            gallery: {
                enabled: !0
            }
        })
    })), e(".img-popup18").length && (a = {}, e(".img-popup18").each(function () {
        var t = parseInt(e(this).attr("data-group"), 10);
        a[t] || (a[t] = []), a[t].push(this)
    }), e.each(a, function () {
        e(this).magnificPopup({
            type: "image",
            closeOnContentClick: !0,
            closeBtnInside: !1,
            gallery: {
                enabled: !0
            }
        })
    })), e(".img-popup19").length && (a = {}, e(".img-popup19").each(function () {
        var t = parseInt(e(this).attr("data-group"), 10);
        a[t] || (a[t] = []), a[t].push(this)
    }), e.each(a, function () {
        e(this).magnificPopup({
            type: "image",
            closeOnContentClick: !0,
            closeBtnInside: !1,
            gallery: {
                enabled: !0
            }
        })
    })), e(".img-popup20").length && (a = {}, e(".img-popup20").each(function () {
        var t = parseInt(e(this).attr("data-group"), 10);
        a[t] || (a[t] = []), a[t].push(this)
    }), e.each(a, function () {
        e(this).magnificPopup({
            type: "image",
            closeOnContentClick: !0,
            closeBtnInside: !1,
            gallery: {
                enabled: !0
            }
        })
    })), e(".img-popup21").length && (a = {}, e(".img-popup21").each(function () {
        var t = parseInt(e(this).attr("data-group"), 10);
        a[t] || (a[t] = []), a[t].push(this)
    }), e.each(a, function () {
        e(this).magnificPopup({
            type: "image",
            closeOnContentClick: !0,
            closeBtnInside: !1,
            gallery: {
                enabled: !0
            }
        })
    })), e(".img-popup22").length && (a = {}, e(".img-popup22").each(function () {
        var t = parseInt(e(this).attr("data-group"), 10);
        a[t] || (a[t] = []), a[t].push(this)
    }), e.each(a, function () {
        e(this).magnificPopup({
            type: "image",
            closeOnContentClick: !0,
            closeBtnInside: !1,
            gallery: {
                enabled: !0
            }
        })
    })), e(".img-popup23").length && (a = {}, e(".img-popup23").each(function () {
        var t = parseInt(e(this).attr("data-group"), 10);
        a[t] || (a[t] = []), a[t].push(this)
    }), e.each(a, function () {
        e(this).magnificPopup({
            type: "image",
            closeOnContentClick: !0,
            closeBtnInside: !1,
            gallery: {
                enabled: !0
            }
        })
    })), e(".img-popup24").length && (a = {}, e(".img-popup24").each(function () {
        var t = parseInt(e(this).attr("data-group"), 10);
        a[t] || (a[t] = []), a[t].push(this)
    }), e.each(a, function () {
        e(this).magnificPopup({
            type: "image",
            closeOnContentClick: !0,
            closeBtnInside: !1,
            gallery: {
                enabled: !0
            }
        })
    })), e(".img-popup25").length && (a = {}, e(".img-popup25").each(function () {
        var t = parseInt(e(this).attr("data-group"), 10);
        a[t] || (a[t] = []), a[t].push(this)
    }), e.each(a, function () {
        e(this).magnificPopup({
            type: "image",
            closeOnContentClick: !0,
            closeBtnInside: !1,
            gallery: {
                enabled: !0
            }
        })
    })), e(".img-popup26").length && (a = {}, e(".img-popup26").each(function () {
        var t = parseInt(e(this).attr("data-group"), 10);
        a[t] || (a[t] = []), a[t].push(this)
    }), e.each(a, function () {
        e(this).magnificPopup({
            type: "image",
            closeOnContentClick: !0,
            closeBtnInside: !1,
            gallery: {
                enabled: !0
            }
        })
    })), e(".img-popup27").length && (a = {}, e(".img-popup27").each(function () {
        var t = parseInt(e(this).attr("data-group"), 10);
        a[t] || (a[t] = []), a[t].push(this)
    }), e.each(a, function () {
        e(this).magnificPopup({
            type: "image",
            closeOnContentClick: !0,
            closeBtnInside: !1,
            gallery: {
                enabled: !0
            }
        })
    })), e(".img-popup28").length && (a = {}, e(".img-popup28").each(function () {
        var t = parseInt(e(this).attr("data-group"), 10);
        a[t] || (a[t] = []), a[t].push(this)
    }), e.each(a, function () {
        e(this).magnificPopup({
            type: "image",
            closeOnContentClick: !0,
            closeBtnInside: !1,
            gallery: {
                enabled: !0
            }
        })
    })), e(".img-popup29").length && (a = {}, e(".img-popup29").each(function () {
        var t = parseInt(e(this).attr("data-group"), 10);
        a[t] || (a[t] = []), a[t].push(this)
    }), e.each(a, function () {
        e(this).magnificPopup({
            type: "image",
            closeOnContentClick: !0,
            closeBtnInside: !1,
            gallery: {
                enabled: !0
            }
        })
    })), e(".img-popup30").length && (a = {}, e(".img-popup30").each(function () {
        var t = parseInt(e(this).attr("data-group"), 10);
        a[t] || (a[t] = []), a[t].push(this)
    }), e.each(a, function () {
        e(this).magnificPopup({
            type: "image",
            closeOnContentClick: !0,
            closeBtnInside: !1,
            gallery: {
                enabled: !0
            }
        })
    })), e(".img-popup31").length && (a = {}, e(".img-popup31").each(function () {
        var t = parseInt(e(this).attr("data-group"), 10);
        a[t] || (a[t] = []), a[t].push(this)
    }), e.each(a, function () {
        e(this).magnificPopup({
            type: "image",
            closeOnContentClick: !0,
            closeBtnInside: !1,
            gallery: {
                enabled: !0
            }
        })
    })), e(".img-popup32").length && (a = {}, e(".img-popup32").each(function () {
        var t = parseInt(e(this).attr("data-group"), 10);
        a[t] || (a[t] = []), a[t].push(this)
    }), e.each(a, function () {
        e(this).magnificPopup({
            type: "image",
            closeOnContentClick: !0,
            closeBtnInside: !1,
            gallery: {
                enabled: !0
            }
        })
    })), e(".img-popup33").length && (a = {}, e(".img-popup33").each(function () {
        var t = parseInt(e(this).attr("data-group"), 10);
        a[t] || (a[t] = []), a[t].push(this)
    }), e.each(a, function () {
        e(this).magnificPopup({
            type: "image",
            closeOnContentClick: !0,
            closeBtnInside: !1,
            gallery: {
                enabled: !0
            }
        })
    })), e(".img-popup34").length && (a = {}, e(".img-popup34").each(function () {
        var t = parseInt(e(this).attr("data-group"), 10);
        a[t] || (a[t] = []), a[t].push(this)
    }), e.each(a, function () {
        e(this).magnificPopup({
            type: "image",
            closeOnContentClick: !0,
            closeBtnInside: !1,
            gallery: {
                enabled: !0
            }
        })
    })), e(".img-popup35").length && (a = {}, e(".img-popup35").each(function () {
        var t = parseInt(e(this).attr("data-group"), 10);
        a[t] || (a[t] = []), a[t].push(this)
    }), e.each(a, function () {
        e(this).magnificPopup({
            type: "image",
            closeOnContentClick: !0,
            closeBtnInside: !1,
            gallery: {
                enabled: !0
            }
        })
    })), e(".img-popup36").length && (a = {}, e(".img-popup36").each(function () {
        var t = parseInt(e(this).attr("data-group"), 10);
        a[t] || (a[t] = []), a[t].push(this)
    }), e.each(a, function () {
        e(this).magnificPopup({
            type: "image",
            closeOnContentClick: !0,
            closeBtnInside: !1,
            gallery: {
                enabled: !0
            }
        })
    })), e(".img-popup37").length && (a = {}, e(".img-popup37").each(function () {
        var t = parseInt(e(this).attr("data-group"), 10);
        a[t] || (a[t] = []), a[t].push(this)
    }), e.each(a, function () {
        e(this).magnificPopup({
            type: "image",
            closeOnContentClick: !0,
            closeBtnInside: !1,
            gallery: {
                enabled: !0
            }
        })
    })), e(".img-popup38").length && (a = {}, e(".img-popup38").each(function () {
        var t = parseInt(e(this).attr("data-group"), 10);
        a[t] || (a[t] = []), a[t].push(this)
    }), e.each(a, function () {
        e(this).magnificPopup({
            type: "image",
            closeOnContentClick: !0,
            closeBtnInside: !1,
            gallery: {
                enabled: !0
            }
        })
    })), e(".img-popup39").length && (a = {}, e(".img-popup39").each(function () {
        var t = parseInt(e(this).attr("data-group"), 10);
        a[t] || (a[t] = []), a[t].push(this)
    }), e.each(a, function () {
        e(this).magnificPopup({
            type: "image",
            closeOnContentClick: !0,
            closeBtnInside: !1,
            gallery: {
                enabled: !0
            }
        })
    })), e(".img-popup40").length && (a = {}, e(".img-popup40").each(function () {
        var t = parseInt(e(this).attr("data-group"), 10);
        a[t] || (a[t] = []), a[t].push(this)
    }), e.each(a, function () {
        e(this).magnificPopup({
            type: "image",
            closeOnContentClick: !0,
            closeBtnInside: !1,
            gallery: {
                enabled: !0
            }
        })
    })), e(".img-popup41").length && (a = {}, e(".img-popup41").each(function () {
        var t = parseInt(e(this).attr("data-group"), 10);
        a[t] || (a[t] = []), a[t].push(this)
    }), e.each(a, function () {
        e(this).magnificPopup({
            type: "image",
            closeOnContentClick: !0,
            closeBtnInside: !1,
            gallery: {
                enabled: !0
            }
        })
    })), e(".img-popup42").length && (a = {}, e(".img-popup42").each(function () {
        var t = parseInt(e(this).attr("data-group"), 10);
        a[t] || (a[t] = []), a[t].push(this)
    }), e.each(a, function () {
        e(this).magnificPopup({
            type: "image",
            closeOnContentClick: !0,
            closeBtnInside: !1,
            gallery: {
                enabled: !0
            }
        })
    })), e(".img-popup43").length && (a = {}, e(".img-popup43").each(function () {
        var t = parseInt(e(this).attr("data-group"), 10);
        a[t] || (a[t] = []), a[t].push(this)
    }), e.each(a, function () {
        e(this).magnificPopup({
            type: "image",
            closeOnContentClick: !0,
            closeBtnInside: !1,
            gallery: {
                enabled: !0
            }
        })
    })), e(".img-popup44").length && (a = {}, e(".img-popup44").each(function () {
        var t = parseInt(e(this).attr("data-group"), 10);
        a[t] || (a[t] = []), a[t].push(this)
    }), e.each(a, function () {
        e(this).magnificPopup({
            type: "image",
            closeOnContentClick: !0,
            closeBtnInside: !1,
            gallery: {
                enabled: !0
            }
        })
    })), e(".img-popup45").length && (a = {}, e(".img-popup45").each(function () {
        var t = parseInt(e(this).attr("data-group"), 10);
        a[t] || (a[t] = []), a[t].push(this)
    }), e.each(a, function () {
        e(this).magnificPopup({
            type: "image",
            closeOnContentClick: !0,
            closeBtnInside: !1,
            gallery: {
                enabled: !0
            }
        })
    })), e(".img-popup46").length && (a = {}, e(".img-popup46").each(function () {
        var t = parseInt(e(this).attr("data-group"), 10);
        a[t] || (a[t] = []), a[t].push(this)
    }), e.each(a, function () {
        e(this).magnificPopup({
            type: "image",
            closeOnContentClick: !0,
            closeBtnInside: !1,
            gallery: {
                enabled: !0
            }
        })
    })), e(".img-popup47").length && (a = {}, e(".img-popup47").each(function () {
        var t = parseInt(e(this).attr("data-group"), 10);
        a[t] || (a[t] = []), a[t].push(this)
    }), e.each(a, function () {
        e(this).magnificPopup({
            type: "image",
            closeOnContentClick: !0,
            closeBtnInside: !1,
            gallery: {
                enabled: !0
            }
        })
    })), e(".img-popup48").length && (a = {}, e(".img-popup48").each(function () {
        var t = parseInt(e(this).attr("data-group"), 10);
        a[t] || (a[t] = []), a[t].push(this)
    }), e.each(a, function () {
        e(this).magnificPopup({
            type: "image",
            closeOnContentClick: !0,
            closeBtnInside: !1,
            gallery: {
                enabled: !0
            }
        })
    })), e(".img-popup49").length && (a = {}, e(".img-popup49").each(function () {
        var t = parseInt(e(this).attr("data-group"), 10);
        a[t] || (a[t] = []), a[t].push(this)
    }), e.each(a, function () {
        e(this).magnificPopup({
            type: "image",
            closeOnContentClick: !0,
            closeBtnInside: !1,
            gallery: {
                enabled: !0
            }
        })
    })), e(".img-popup50").length && (a = {}, e(".img-popup50").each(function () {
        var t = parseInt(e(this).attr("data-group"), 10);
        a[t] || (a[t] = []), a[t].push(this)
    }), e.each(a, function () {
        e(this).magnificPopup({
            type: "image",
            closeOnContentClick: !0,
            closeBtnInside: !1,
            gallery: {
                enabled: !0
            }
        })
    })), e(".img-popup51").length && (a = {}, e(".img-popup51").each(function () {
        var t = parseInt(e(this).attr("data-group"), 10);
        a[t] || (a[t] = []), a[t].push(this)
    }), e.each(a, function () {
        e(this).magnificPopup({
            type: "image",
            closeOnContentClick: !0,
            closeBtnInside: !1,
            gallery: {
                enabled: !0
            }
        })
    })), e(".img-popup52").length && (a = {}, e(".img-popup52").each(function () {
        var t = parseInt(e(this).attr("data-group"), 10);
        a[t] || (a[t] = []), a[t].push(this)
    }), e.each(a, function () {
        e(this).magnificPopup({
            type: "image",
            closeOnContentClick: !0,
            closeBtnInside: !1,
            gallery: {
                enabled: !0
            }
        })
    })), e(".img-popup53").length && (a = {}, e(".img-popup53").each(function () {
        var t = parseInt(e(this).attr("data-group"), 10);
        a[t] || (a[t] = []), a[t].push(this)
    }), e.each(a, function () {
        e(this).magnificPopup({
            type: "image",
            closeOnContentClick: !0,
            closeBtnInside: !1,
            gallery: {
                enabled: !0
            }
        })
    })), e(".img-popup54").length && (a = {}, e(".img-popup54").each(function () {
        var t = parseInt(e(this).attr("data-group"), 10);
        a[t] || (a[t] = []), a[t].push(this)
    }), e.each(a, function () {
        e(this).magnificPopup({
            type: "image",
            closeOnContentClick: !0,
            closeBtnInside: !1,
            gallery: {
                enabled: !0
            }
        })
    })), e(".img-popup55").length && (a = {}, e(".img-popup55").each(function () {
        var t = parseInt(e(this).attr("data-group"), 10);
        a[t] || (a[t] = []), a[t].push(this)
    }), e.each(a, function () {
        e(this).magnificPopup({
            type: "image",
            closeOnContentClick: !0,
            closeBtnInside: !1,
            gallery: {
                enabled: !0
            }
        })
    }))), e(".wow").length && new WOW({
        boxClass: "wow",
        animateClass: "animated",
        mobile: !0,
        live: !0
    }).init(), e(".video-popup").length && e(".video-popup").magnificPopup({
        disableOn: 700,
        type: "iframe",
        mainClass: "mfp-fade",
        removalDelay: 160,
        preloader: !0,
        fixedContentPos: !1
    }), e('[data-toggle="tooltip"]').length && e('[data-toggle="tooltip"]').tooltip(), e(".stricky").length && e(".stricky").addClass("original").clone(!0).insertAfter(".stricky").addClass("stricked-menu").removeClass("original"), e(".scroll-to-target").length && e(".scroll-to-target").on("click", function () {
        var t = e(this).attr("data-target");
        return e("html, body").animate({
            scrollTop: e(t).offset().top
        }, 1e3), !1
    }), e(".side-menu__toggler").length && e(".side-menu__toggler").on("click", function (t) {
        e(".side-menu__block").toggleClass("active"), t.preventDefault()
    }), e(".side-menu__block-overlay").length && e(".side-menu__block-overlay").on("click", function (t) {
        e(".side-menu__block").removeClass("active"), t.preventDefault()
    }), e(".side-content__toggler").length && e(".side-content__toggler").on("click", function (t) {
        e(".side-content__block").toggleClass("active"), t.preventDefault()
    }), e(".side-content__block-overlay").length && e(".side-content__block-overlay").on("click", function (t) {
        e(".side-content__block").removeClass("active"), t.preventDefault()
    }), e(".search-popup__toggler").length && e(".search-popup__toggler").on("click", function (t) {
        e(".search-popup").addClass("active"), t.preventDefault()
    }), e(".search-popup__overlay").length && e(".search-popup__overlay").on("click", function (t) {
        e(".search-popup").removeClass("active"), t.preventDefault()
    }), e(window).on("scroll", function () {
        if (e(".scroll-to-top").length && (e(window).scrollTop() > 100 ? e(".scroll-to-top").fadeIn(500) : e(this).scrollTop() <= 100 && e(".scroll-to-top").fadeOut(500)), e(".stricked-menu").length) {
            var t = e(".stricked-menu");
            e(window).scrollTop() > 100 ? t.addClass("stricky-fixed") : e(this).scrollTop() <= 100 && t.removeClass("stricky-fixed")
        }
    }), e(".accrodion-grp").length && e(".accrodion-grp").each(function () {
        var t = e(this).data("grp-name"),
            n = e(this),
            i = n.find(".accrodion");
        n.addClass(t), n.find(".accrodion .accrodion-content").hide(), n.find(".accrodion.active").find(".accrodion-content").show(), i.each(function () {
            e(this).find(".accrodion-title").on("click", function () {
                !1 === e(this).parent().hasClass("active") && (e(".accrodion-grp." + t).find(".accrodion").removeClass("active"), e(".accrodion-grp." + t).find(".accrodion").find(".accrodion-content").slideUp(), e(this).parent().addClass("active"), e(this).parent().find(".accrodion-content").slideDown())
            })
        })
    }), e(".thm__owl-carousel").length && e(".thm__owl-carousel").each(function () {
        var t = e(this),
            n = t.data("options"),
            i = t.data("carousel-prev-btn"),
            a = t.data("carousel-next-btn"),
            o = t.data("carousel-dots-container"),
            p = t.owlCarousel(n);
        void 0 !== i && e(i).on("click", function () {
            return p.trigger("prev.owl.carousel", [1e3]), !1
        }), void 0 !== a && e(a).on("click", function () {
            return p.trigger("next.owl.carousel", [1e3]), !1
        }), void 0 !== o && e(o).find(".owl-dot").on("click", function () {
            var t = e(this).index();
            p.trigger("to.owl.carousel", [t, 1e3])
        })
    }), e(window).on("load", function () {
        if (e(".preloader").length && e(".preloader").fadeOut("fast"), e(".side-menu__block-inner").length && e(".side-menu__block-inner").mCustomScrollbar({
            axis: "y",
            theme: "dark"
        }), e(".side-content__block-inner").length && e(".side-content__block-inner").mCustomScrollbar({
            axis: "y",
            theme: "dark"
        }), e(".custom-cursor__overlay").length) {
            var t = e(".custom-cursor__overlay .cursor"),
                n = e(".custom-cursor__overlay .cursor-follower"),
                i = 0,
                a = 0,
                o = 0,
                p = 0;
            TweenMax.to({}, .016, {
                repeat: -1,
                onRepeat: function () {
                    i += (o - i) / 9, a += (p - a) / 9, TweenMax.set(n, {
                        css: {
                            left: i - 22,
                            top: a - 22
                        }
                    }), TweenMax.set(t, {
                        css: {
                            left: o,
                            top: p
                        }
                    })
                }
            }), e(document).on("mousemove", function (e) {
                var t = window.pageYOffset || document.documentElement.scrollTop;
                o = e.pageX, p = e.pageY - t
            }), e("button, a").on("mouseenter", function () {
                t.addClass("active"), n.addClass("active")
            }), e("button, a").on("mouseleave", function () {
                t.removeClass("active"), n.removeClass("active")
            }), e(".custom-cursor__overlay").on("mouseenter", function () {
                t.addClass("close-cursor"), n.addClass("close-cursor")
            }), e(".custom-cursor__overlay").on("mouseleave", function () {
                t.removeClass("close-cursor"), n.removeClass("close-cursor")
            })
        }
        if (e(".masonary-layout").length && e(".masonary-layout").isotope({
            layoutMode: "masonry",
            itemSelector: ".masonary-item"
        }), e(".post-filter").length) {
            var s = e(".post-filter li");
            e(".filter-layout").isotope({
                filter: ".filter-item",
                animationOptions: {
                    duration: 500,
                    easing: "linear",
                    queue: !1
                }
            }), s.children("span").on("click", function () {
                var t = e(this),
                    n = t.parent().attr("data-filter");
                return s.children("span").parent().removeClass("active"), t.parent().addClass("active"), e(".filter-layout").isotope({
                    filter: n,
                    animationOptions: {
                        duration: 500,
                        easing: "linear",
                        queue: !1
                    }
                }), !1
            })
        }
        if (e(".post-filter.has-dynamic-filter-counter").length && e(".post-filter.has-dynamic-filter-counter").find("li").each(function () {
            var t = e(this).data("filter"),
                n = e(".gallery-content").find(t).length;
            e(this).children("span").append('<span class="count"><b>' + n + "</b></span>")
        }), e(".testimonials-two__thumb-carousel").length) var c = new Swiper(".testimonials-two__thumb-carousel", {
            slidesPerView: 3,
            spaceBetween: 0,
            mousewheel: !0,
            speed: 1400,
            watchSlidesVisibility: !0,
            watchSlidesProgress: !0,
            loop: !0,
            autoplay: {
                delay: 5e3
            }
        });
        e(".testimonials-two__carousel").length && new Swiper(".testimonials-two__carousel", {
            observer: !0,
            observeParents: !0,
            speed: 1400,
            mousewheel: !1,
            autoplay: {
                delay: 5e3
            },
            thumbs: {
                swiper: c
            }
        })
    })
}(jQuery);