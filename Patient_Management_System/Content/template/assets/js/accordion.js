! function(t) {
    "use strict";
    var e, n, i, o;

    function a(n, i) {
        var o;
        return o = Array.prototype.slice.call(arguments, 1), this.each(function() {
            var a, r;
            (r = (a = t(this)).data("vc.accordion")) || (r = new e(a, t.extend(!0, {}, i)), a.data("vc.accordion", r)), "string" == typeof n && r[n].apply(r, o)
        })
    }(e = function(t, e) {
        this.$element = t, this.activeClass = "active", this.animatingClass = "animating", this.useCacheFlag = void 0, this.$target = void 0, this.$targetContent = void 0, this.selector = void 0, this.$container = void 0, this.animationDuration = void 0, this.index = 0
    }).transitionEvent = function() {
        var t, e, n;
        for (t in n = document.createElement("vcFakeElement"), e = {
                transition: "transitionend",
                MSTransition: "msTransitionEnd",
                MozTransition: "transitionend",
                WebkitTransition: "webkitTransitionEnd"
            })
            if (void 0 !== n.style[t]) return e[t]
    }, e.emulateTransitionEnd = function(t, n) {
        var i;
        i = !1, n || (n = 250), t.one(e.transitionName, function() {
            i = !0
        }), setTimeout(function() {
            i || t.trigger(e.transitionName)
        }, n)
    }, e.DEFAULT_TYPE = "collapse", e.transitionName = e.transitionEvent(), e.prototype.controller = function(t) {
        var n;
        n = this.$element;
        var i = t;
        "string" != typeof i && (i = n.data("vcAction") || this.getContainer().data("vcAction")), void 0 === i && (i = e.DEFAULT_TYPE), "string" == typeof i && a.call(n, i, t)
    }, e.prototype.isCacheUsed = function() {
        var t, e;
        return t = function() {
            return !1 !== e.$element.data("vcUseCache")
        }, void 0 === (e = this).useCacheFlag && (this.useCacheFlag = t()), this.useCacheFlag
    }, e.prototype.getSelector = function() {
        var t, e;
        return e = this.$element, t = function() {
            var t;
            return (t = e.data("vcTarget")) || (t = e.attr("href")), t
        }, this.isCacheUsed() ? (void 0 === this.selector && (this.selector = t()), this.selector) : t()
    }, e.prototype.findContainer = function() {
        var e;
        return (e = this.$element.closest(this.$element.data("vcContainer"))).length || (e = t("body")), e
    }, e.prototype.getContainer = function() {
        return this.isCacheUsed() ? (void 0 === this.$container && (this.$container = this.findContainer()), this.$container) : this.findContainer()
    }, e.prototype.getTarget = function() {
        var t, e, n;
        return t = (e = this).getSelector(), n = function() {
            var n;
            return (n = e.getContainer().find(t)).length || (n = e.getContainer().filter(t)), n
        }, this.isCacheUsed() ? (void 0 === this.$target && (this.$target = n()), this.$target) : n()
    }, e.prototype.getTargetContent = function() {
        var t, e;
        return t = this.getTarget(), this.isCacheUsed() ? (void 0 === this.$targetContent && ((e = t).data("vcContent") && ((e = t.find(t.data("vcContent"))).length || (e = t)), this.$targetContent = e), this.$targetContent) : t.data("vcContent") && (e = t.find(t.data("vcContent"))).length ? e : t
    }, e.prototype.getTriggers = function() {
        var e;
        return e = 0, this.getContainer().find("[data-vc-accordion]").each(function() {
            var n, i;
            void 0 === (n = (i = t(this)).data("vc.accordion")) && (i.vcAccordion(), n = i.data("vc.accordion")), n && n.setIndex && n.setIndex(e++)
        })
    }, e.prototype.setIndex = function(t) {
        this.index = t
    }, e.prototype.getIndex = function() {
        return this.index
    }, e.prototype.triggerEvent = function(e, n) {
        var i;
        "string" == typeof e && (i = t.Event(e), this.$element.trigger(i, n))
    }, e.prototype.getActiveTriggers = function() {
        return this.getTriggers().filter(function() {
            var e;
            return (e = t(this).data("vc.accordion")).getTarget().hasClass(e.activeClass)
        })
    }, e.prototype.isActive = function() {
        return this.getTarget().hasClass(this.activeClass)
    }, e.prototype.getAnimationDuration = function() {
        var t, n;
        return t = function() {
            return void 0 === e.transitionName ? "0s" : n.getTargetContent().css("transition-duration").split(",")[0]
        }, (n = this).isCacheUsed() ? (void 0 === this.animationDuration && (this.animationDuration = t()), this.animationDuration) : t()
    }, e.prototype.getAnimationDurationMilliseconds = function() {
        var t;
        return "ms" === (t = this.getAnimationDuration()).substr(-2) ? parseInt(t) : "s" === t.substr(-1) ? Math.round(1e3 * parseFloat(t)) : void 0
    }, e.prototype.isAnimated = function() {
        return 0 < parseFloat(this.getAnimationDuration())
    }, e.prototype.show = function(t) {
        var n, i, o;
        n = (i = this).getTarget(), o = i.getTargetContent(), i.isActive() || (i.isAnimated() ? (i.triggerEvent("beforeShow.vc.accordion"), n.queue(function(a) {
            o.one(e.transitionName, function() {
                n.removeClass(i.animatingClass), o.attr("style", ""), i.triggerEvent("afterShow.vc.accordion", t)
            }), e.emulateTransitionEnd(o, i.getAnimationDurationMilliseconds() + 100), a()
        }).queue(function(t) {
            o.attr("style", ""), o.css({
                position: "absolute",
                visibility: "hidden",
                display: "block"
            });
            var e = o.height();
            o.data("vcHeight", e), o.attr("style", ""), t()
        }).queue(function(t) {
            o.height(0), o.css({
                "padding-top": 0,
                "padding-bottom": 0
            }), t()
        }).queue(function(e) {
            n.addClass(i.animatingClass), n.addClass(i.activeClass), i.triggerEvent("show.vc.accordion", t), e()
        }).queue(function(t) {
            var e = o.data("vcHeight");
            o.animate({
                height: e
            }, {
                duration: i.getAnimationDurationMilliseconds(),
                complete: function() {
                    o.data("events") || o.attr("style", "")
                }
            }), o.css({
                "padding-top": "",
                "padding-bottom": ""
            }), t()
        })) : (n.addClass(i.activeClass), i.triggerEvent("show.vc.accordion", t)))
    }, e.prototype.hide = function(t) {
        var n, i, o;
        n = (i = this).getTarget(), o = i.getTargetContent(), i.isActive() && (i.isAnimated() ? (i.triggerEvent("beforeHide.vc.accordion"), n.queue(function(a) {
            o.one(e.transitionName, function() {
                n.removeClass(i.animatingClass), o.attr("style", ""), i.triggerEvent("afterHide.vc.accordion", t)
            }), e.emulateTransitionEnd(o, i.getAnimationDurationMilliseconds() + 100), a()
        }).queue(function(e) {
            n.addClass(i.animatingClass), n.removeClass(i.activeClass), i.triggerEvent("hide.vc.accordion", t), e()
        }).queue(function(t) {
            var e = o.height();
            o.height(e), t()
        }).queue(function(t) {
            o.animate({
                height: 0
            }, i.getAnimationDurationMilliseconds()), o.css({
                "padding-top": 0,
                "padding-bottom": 0
            }), t()
        })) : (n.removeClass(i.activeClass), i.triggerEvent("hide.vc.accordion", t)))
    }, e.prototype.toggle = function(t) {
        var e;
        e = this.$element, this.isActive() ? a.call(e, "hide", t) : a.call(e, "show", t)
    }, e.prototype.dropdown = function(e) {
        var n;
        n = this.$element, this.isActive() ? a.call(n, "hide", e) : (a.call(n, "show", e), t(document).on("click.vc.accordion.data-api.dropdown", function(i) {
            a.call(n, "hide", e), t(document).off(i)
        }))
    }, e.prototype.collapse = function(t) {
        var e, n;
        e = this.$element, (n = this.getActiveTriggers().filter(function() {
            return e[0] !== this
        })).length && a.call(n, "hide", t), a.call(e, "show", t)
    }, e.prototype.collapseAll = function(t) {
        var e, n;
        e = this.$element, (n = this.getActiveTriggers().filter(function() {
            return e[0] !== this
        })).length && a.call(n, "hide", t), a.call(e, "toggle", t)
    }, e.prototype.showNext = function(t) {
        var e, n, i, o;
        (e = this.getTriggers(), n = this.getActiveTriggers(), e.length) && (n.length && (o = n.eq(n.length - 1).vcAccordion().data("vc.accordion")) && o.getIndex && (i = o.getIndex()), -1 < i && i + 1 < e.length ? a.call(e.eq(i + 1), "controller", t) : a.call(e.eq(0), "controller", t))
    }, e.prototype.showPrev = function(t) {
        var e, n, i, o;
        (e = this.getTriggers(), n = this.getActiveTriggers(), e.length) && (n.length && (o = n.eq(n.length - 1).vcAccordion().data("vc.accordion")) && o.getIndex && (i = o.getIndex()), a.call(-1 < i ? 0 <= i - 1 ? e.eq(i - 1) : e.eq(e.length - 1) : e.eq(0), "controller", t))
    }, e.prototype.showAt = function(t, e) {
        var n;
        (n = this.getTriggers()).length && t && t < n.length && a.call(n.eq(t), "controller", e)
    }, e.prototype.scrollToActive = function(e) {
        var n, i;
        (void 0 === e || void 0 === e.scrollTo || e.scrollTo) && (i = t((n = this).getTarget())).length && this.$element.length && setTimeout(function() {
            i.offset().top - t(window).scrollTop() - 1 * n.$element.outerHeight() < 0 && t("html, body").animate({
                scrollTop: i.offset().top - 1 * n.$element.outerHeight()
            }, 300)
        }, 300)
    }, i = t.fn.vcAccordion, t.fn.vcAccordion = a, t.fn.vcAccordion.Constructor = e, t.fn.vcAccordion.noConflict = function() {
        return t.fn.vcAccordion = i, this
    }, n = function(e) {
        var n;
        n = t(this), e.preventDefault(), a.call(n, "controller")
    }, o = function() {
        var e, n, i;
        (e = window.location.hash) && (n = t(e)).length && (i = n.find('[data-vc-accordion][href="' + e + '"],[data-vc-accordion][data-vc-target="' + e + '"]')).length && (setTimeout(function() {
            t("html, body").animate({
                scrollTop: n.offset().top - .2 * t(window).height()
            }, 0)
        }, 300), i.trigger("click"))
    }, t(window).on("hashchange.vc.accordion", o), t(document).on("click.vc.accordion.data-api", "[data-vc-accordion]", n), t(document).ready(o), t(document).on("afterShow.vc.accordion", function(e, n) {
        a.call(t(e.target), "scrollToActive", n)
    })
}(window.jQuery);