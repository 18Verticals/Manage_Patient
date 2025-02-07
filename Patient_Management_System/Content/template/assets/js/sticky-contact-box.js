jQuery(document).ready(function(e) {
    var t = e(".cd-faq-group"),
        s = e(".cd-faq-trigger"),
        a = e(".cd-faq-items"),
        o = e(".cd-faq-categories"),
        r = o.find("a"),
        n = e(".cd-close-panel");

    function l(t) {
        t.preventDefault(), a.removeClass("slide-in").find("li").show(), n.removeClass("move-left"), e("body").removeClass("cd-overlay")
    }

    function i() {
        ! function() {
            var t = e(".cd-faq").offset().top,
                s = jQuery(".cd-faq").height() - jQuery(".cd-faq-categories").height();
            if (t - 130 <= e(window).scrollTop() && t - 130 + s > e(window).scrollTop()) {
                var a = o.offset().left;
                o.width(), o.addClass("is-fixed").css({
                    left: a,
                    top: 130,
                    "-moz-transform": "translateZ(0)",
                    "-webkit-transform": "translateZ(0)",
                    "-ms-transform": "translateZ(0)",
                    "-o-transform": "translateZ(0)",
                    transform: "translateZ(0)"
                })
            } else if (t - 130 + s <= e(window).scrollTop()) {
                var r = t - 130 + s - e(window).scrollTop();
                o.css({
                    "-moz-transform": "translateZ(0) translateY(" + r + "px)",
                    "-webkit-transform": "translateZ(0) translateY(" + r + "px)",
                    "-ms-transform": "translateZ(0) translateY(" + r + "px)",
                    "-o-transform": "translateZ(0) translateY(" + r + "px)",
                    transform: "translateZ(0) translateY(" + r + "px)"
                })
            } else o.removeClass("is-fixed").css({
                left: 0,
                top: 0
            })
        }(), t.each(function() {
            var t = e(this),
                s = parseInt(e(".slide-title").eq(1).css("marginTop").replace("px", "")),
                a = e('.cd-faq-categories a[href="#' + t.attr("id") + '"]');
            (a.parent("li").is(":first-child") ? 0 : Math.round(t.offset().top)) - 200 <= e(window).scrollTop() && Math.round(t.offset().top) + t.height() + s - 200 > e(window).scrollTop() ? a.addClass("selected") : a.removeClass("selected")
        })
    }
    r.on("click", function(t) {
        t.preventDefault();
        var s = e(this).attr("href"),
            o = e(s);
        e(window).width() < 768 ? (e(".cd-faq-items").find(".selected").removeClass("selected"), a.scrollTop(0).addClass("slide-in").children("ul").removeClass("selected").end().children(s).addClass("selected"), n.addClass("move-left"), e("body").addClass("cd-overlay")) : e("body,html").animate({
            scrollTop: o.offset().top - 130
        }, 200)
    }), e("body").bind("click touchstart", function(t) {
        (e(t.target).is("body.cd-overlay") || e(t.target).is(".cd-close-panel")) && l(t)
    }), a.on("swiperight", function(e) {
        l(e)
    }), s.on("click", function(t) {
        t.preventDefault(), e(this).next(".cd-faq-content").slideToggle(200).end().parent("li").toggleClass("content-visible")
    }), e(window).on("scroll", function() {
        e(window).width() > 1024 && (window.requestAnimationFrame ? window.requestAnimationFrame(i) : i())
    }), e(window).on("resize", function() {
        e(window).width() <= 1024 && o.removeClass("is-fixed").css({
            "-moz-transform": "translateY(0)",
            "-webkit-transform": "translateY(0)",
            "-ms-transform": "translateY(0)",
            "-o-transform": "translateY(0)",
            transform: "translateY(0)"
        }), o.hasClass("is-fixed") && o.css({
            left: a.offset().left
        })
    })
});