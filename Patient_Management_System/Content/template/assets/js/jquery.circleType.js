$.fn.circleType = function(t) {
    var e = {
        dir: 1,
        position: "relative"
    };
    if ("function" == typeof $.fn.lettering) return this.each(function() {
        t && $.extend(e, t);
        var i, n, o = this,
            r = 180 / Math.PI,
            s = parseInt($(o).css("line-height"), 10),
            f = parseInt($(o).css("font-size"), 10),
            a = o.innerHTML.replace(/^\s+|\s+$/g, "").replace(/\s/g, "&nbsp;");
        o.innerHTML = a, $(o).lettering(), o.style.position = e.position, i = o.getElementsByTagName("span"), n = Math.floor(i.length / 2);
        var l = function() {
                var t, n, a, l, d, h, c, u = 0,
                    p = 0;
                for (t = 0; t < i.length; t++) u += i[t].offsetWidth;
                for (n = u / Math.PI / 2 + s, e.fluid && !e.fitText ? e.radius = Math.max(o.offsetWidth / 2, n) : e.radius || (e.radius = n), a = -1 === e.dir ? "center " + (-e.radius + s) / f + "em" : "center " + e.radius / f + "em", l = e.radius - s, t = 0; t < i.length; t++) p += (d = i[t]).offsetWidth / 2 / l * r, d.rot = p, p += d.offsetWidth / 2 / l * r;
                for (t = 0; t < i.length; t++) h = (d = i[t]).style, c = "rotate(" + (-p * e.dir / 2 + d.rot * e.dir) + "deg)", h.position = "absolute", h.left = "50%", h.marginLeft = -d.offsetWidth / 2 / f + "em", h.webkitTransform = c, h.MozTransform = c, h.OTransform = c, h.msTransform = c, h.transform = c, h.webkitTransformOrigin = a, h.MozTransformOrigin = a, h.OTransformOrigin = a, h.msTransformOrigin = a, h.transformOrigin = a, -1 === e.dir && (h.bottom = 0);
                e.fitText && ("function" != typeof $.fn.fitText ? console.log("FitText.js is required when using the fitText option") : ($(o).fitText(), $(window).resize(function() {
                    g()
                }))), g()
            },
            d = function(t) {
                var e = document.documentElement,
                    i = t.getBoundingClientRect();
                return {
                    top: i.top + window.pageYOffset - e.clientTop,
                    left: i.left + window.pageXOffset - e.clientLeft,
                    height: i.height
                }
            },
            g = function() {
                var t, e = d(i[n]),
                    r = d(i[0]);
                t = e.top < r.top ? r.top - e.top + r.height : e.top - r.top + r.height, o.style.height = t + "px"
            };
        e.fluid && !e.fitText && $(window).resize(function() {
            l()
        }), "complete" !== document.readyState ? (o.style.visibility = "hidden", $(window).load(function() {
            o.style.visibility = "visible", l()
        })) : l()
    });
    console.log("Lettering.js is required")
};