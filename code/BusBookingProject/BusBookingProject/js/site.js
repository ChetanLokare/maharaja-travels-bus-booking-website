/* Maharaja Travel's - shared behaviour (no jQuery needed) */
(function () {
    'use strict';
    var root = document.documentElement;
    var mq = window.matchMedia ? window.matchMedia('(prefers-color-scheme: dark)') : null;

    /* ---------- theme: light / dark / system ---------- */
    function stored() { try { return localStorage.getItem('mbt-theme') || 'system'; } catch (e) { return 'system'; } }
    function apply(choice) {
        var dark = choice === 'dark' || (choice === 'system' && mq && mq.matches);
        root.setAttribute('data-bs-theme', dark ? 'dark' : 'light');
        var meta = document.querySelector('meta[name="theme-color"]');
        if (meta) meta.setAttribute('content', dark ? '#0b1020' : '#4338ca');
        document.querySelectorAll('[data-theme-value]').forEach(function (b) {
            b.classList.toggle('is-active', b.getAttribute('data-theme-value') === choice);
        });
        document.querySelectorAll('[data-theme-icon]').forEach(function (i) {
            i.className = 'fa-solid ' + (choice === 'light' ? 'fa-sun' : choice === 'dark' ? 'fa-moon' : 'fa-circle-half-stroke');
        });
    }
    function setTheme(choice) {
        try { localStorage.setItem('mbt-theme', choice); } catch (e) { }
        apply(choice);
    }
    document.addEventListener('click', function (e) {
        var b = e.target.closest ? e.target.closest('[data-theme-value]') : null;
        if (b) { e.preventDefault(); setTheme(b.getAttribute('data-theme-value')); }
    });
    if (mq && mq.addEventListener) mq.addEventListener('change', function () { if (stored() === 'system') apply('system'); });
    apply(stored());

    /* ---------- toasts ---------- */
    /* minimal toast used only when Bootstrap's script is unavailable */
    function plainToast(message, type) {
        var colors = { success: '#198754', danger: '#dc3545', warning: '#ffc107', info: '#0d6efd' };
        var el = document.createElement('div');
        el.setAttribute('role', 'alert');
        el.style.cssText = 'position:fixed;top:16px;right:16px;z-index:5000;max-width:min(90vw,380px);padding:12px 16px;border-radius:10px;'
            + 'box-shadow:0 10px 30px rgba(0,0,0,.3);font:14px system-ui,sans-serif;color:' + (type === 'warning' ? '#000' : '#fff') + ';background:' + (colors[type] || colors.info);
        el.textContent = message;
        (document.body || document.documentElement).appendChild(el);
        setTimeout(function () { el.remove(); }, 7000);
    }

    var icons = { success: 'fa-circle-check', danger: 'fa-circle-exclamation', warning: 'fa-triangle-exclamation', info: 'fa-circle-info' };
    function toast(message, type) {
        type = icons[type] ? type : 'info';
        var host = document.getElementById('toastHost');
        if (!host || !window.bootstrap) { plainToast(message, type); return; }   // never lose a message, even if Bootstrap JS did not load
        var el = document.createElement('div');
        el.className = 'toast mbt-toast align-items-center text-bg-' + type;
        el.setAttribute('role', type === 'danger' ? 'alert' : 'status');
        var wrap = document.createElement('div'); wrap.className = 'd-flex';
        var body = document.createElement('div'); body.className = 'toast-body d-flex gap-2 align-items-start';
        var ic = document.createElement('i'); ic.className = 'fa-solid ' + icons[type] + ' mt-1';
        var tx = document.createElement('span'); tx.textContent = message;      // textContent: never interpret as HTML
        body.appendChild(ic); body.appendChild(tx);
        var close = document.createElement('button'); close.type = 'button'; close.className = 'btn-close btn-close-white me-2 m-auto';
        close.setAttribute('data-bs-dismiss', 'toast'); close.setAttribute('aria-label', 'Close');
        wrap.appendChild(body); wrap.appendChild(close); el.appendChild(wrap); host.appendChild(el);
        var t = new bootstrap.Toast(el, { delay: type === 'danger' ? 7000 : 4500 });
        el.addEventListener('hidden.bs.toast', function () { el.remove(); });
        t.show();
    }

    /* ---------- loading overlay + double-submit guard ---------- */
    var overlay = function () { return document.getElementById('mbtLoading'); };
    function showLoading() { var o = overlay(); if (o) o.hidden = false; }
    function hideLoading() { var o = overlay(); if (o) o.hidden = true; }
    document.addEventListener('submit', function (e) {
        var f = e.target;
        if (!f || f.hasAttribute('data-no-overlay')) return;
        if (f.checkValidity && !f.checkValidity()) return;
        showLoading();
        setTimeout(function () {                      // after the browser has collected the form data
            f.querySelectorAll('input[type=submit],button[type=submit]').forEach(function (b) { b.disabled = true; });
        }, 0);
    }, true);
    window.addEventListener('pageshow', function () {
        hideLoading();
        document.querySelectorAll('input[type=submit],button[type=submit]').forEach(function (b) { b.disabled = false; });
    });

    /* ---------- reveal on scroll ---------- */
    function initReveal() {
        var items = document.querySelectorAll('[data-reveal]');
        if (!('IntersectionObserver' in window)) { items.forEach(function (i) { i.classList.add('is-visible'); }); return; }
        var io = new IntersectionObserver(function (entries) {
            entries.forEach(function (en) { if (en.isIntersecting) { en.target.classList.add('is-visible'); io.unobserve(en.target); } });
        }, { threshold: 0.12 });
        items.forEach(function (i, idx) { if (!i.style.getPropertyValue('--d')) i.style.setProperty('--d', (idx % 6) * 70 + 'ms'); io.observe(i); });
    }

    /* ---------- animated counters ---------- */
    function initCounters() {
        var els = document.querySelectorAll('[data-count]');
        if (!els.length || !('IntersectionObserver' in window)) return;
        var io = new IntersectionObserver(function (entries) {
            entries.forEach(function (en) {
                if (!en.isIntersecting) return;
                io.unobserve(en.target);
                var el = en.target, end = parseFloat(el.getAttribute('data-count')), suffix = el.getAttribute('data-suffix') || '', start = null;
                function step(ts) {
                    if (start === null) start = ts;
                    var p = Math.min((ts - start) / 1400, 1), v = Math.round(end * (1 - Math.pow(1 - p, 3)));
                    el.textContent = v.toLocaleString('en-IN') + suffix;
                    if (p < 1) requestAnimationFrame(step);
                }
                requestAnimationFrame(step);
            });
        }, { threshold: 0.4 });
        els.forEach(function (el) { io.observe(el); });
    }

    /* ---------- password helpers ---------- */
    function score(pw) {
        var s = 0;
        if (pw.length >= 10) s++; if (pw.length >= 14) s++;
        if (/[a-z]/.test(pw) && /[A-Z]/.test(pw)) s++;
        if (/\d/.test(pw)) s++;
        if (/[^A-Za-z0-9]/.test(pw)) s++;
        return Math.min(s, 5);
    }
    function initPasswordUi() {
        document.querySelectorAll('[data-toggle-password]').forEach(function (btn) {
            btn.addEventListener('click', function () {
                var input = document.getElementById(btn.getAttribute('data-toggle-password'));
                if (!input) return;
                var show = input.type === 'password';
                input.type = show ? 'text' : 'password';
                var i = btn.querySelector('i'); if (i) i.className = 'fa-solid ' + (show ? 'fa-eye-slash' : 'fa-eye');
                btn.setAttribute('aria-pressed', show ? 'true' : 'false');
            });
        });
        document.querySelectorAll('[data-pw-meter]').forEach(function (bar) {
            var input = document.getElementById(bar.getAttribute('data-pw-meter'));
            var label = bar.parentNode.querySelector('[data-pw-label]');
            if (!input) return;
            var colors = ['#ef4444', '#ef4444', '#f59e0b', '#eab308', '#22c55e', '#16a34a'];
            var names = ['Too short', 'Weak', 'Fair', 'Good', 'Strong', 'Excellent'];
            input.addEventListener('input', function () {
                var s = input.value ? score(input.value) : 0, span = bar.firstElementChild;
                span.style.width = (input.value ? Math.max(s, 1) * 20 : 0) + '%'; span.style.background = colors[s];
                if (label) label.textContent = input.value ? names[s] : '';
            });
        });
    }

    /* ---------- OTP input + resend countdown ---------- */
    function initOtp() {
        document.querySelectorAll('[data-otp]').forEach(function (i) {
            i.addEventListener('input', function () { i.value = i.value.replace(/\D/g, '').slice(0, 6); });
        });
        document.querySelectorAll('[data-countdown]').forEach(function (el) {
            var left = parseInt(el.getAttribute('data-countdown'), 10) || 0;
            var target = document.getElementById(el.getAttribute('data-countdown-target'));
            function tick() {
                if (left > 0) { el.textContent = 'Resend available in ' + left + 's'; if (target) target.classList.add('disabled'); left--; setTimeout(tick, 1000); }
                else { el.textContent = ''; if (target) target.classList.remove('disabled'); }
            }
            tick();
        });
    }

    /* ---------- hold timer (passenger + payment pages) ---------- */
    function initHoldTimer() {
        var el = document.querySelector('[data-hold-until]');
        if (!el) return;
        var end = parseInt(el.getAttribute('data-hold-until'), 10);          // seconds remaining at page render
        var started = Date.now();
        var out = el.querySelector('[data-hold-text]') || el;
        (function tick() {
            var left = Math.max(0, end - Math.floor((Date.now() - started) / 1000));
            var m = Math.floor(left / 60), s = left % 60;
            out.textContent = (m < 10 ? '0' : '') + m + ':' + (s < 10 ? '0' : '') + s;
            el.classList.toggle('text-danger', left < 120);
            if (left <= 0) { el.classList.add('text-danger'); out.textContent = 'expired'; toast('Your seat hold expired. Please select seats again.', 'warning'); return; }
            setTimeout(tick, 1000);
        })();
    }

    /* ---------- bus result filters / sort ---------- */
    function initResultTools() {
        var list = document.getElementById('busList');
        if (!list) return;
        var sort = document.getElementById('sortBy'), type = document.getElementById('filterType');
        function run() {
            var cards = Array.prototype.slice.call(list.querySelectorAll('[data-bus]'));
            var t = type ? type.value : 'all';
            cards.forEach(function (c) { c.classList.toggle('d-none', t !== 'all' && c.getAttribute('data-type') !== t); });
            var key = sort ? sort.value : 'time';
            cards.sort(function (a, b) {
                if (key === 'fare') return parseFloat(a.dataset.fare) - parseFloat(b.dataset.fare);
                if (key === 'seats') return parseInt(b.dataset.seats, 10) - parseInt(a.dataset.seats, 10);
                return parseInt(a.dataset.order, 10) - parseInt(b.dataset.order, 10);
            }).forEach(function (c) { list.appendChild(c); });
            var empty = document.getElementById('noMatch');
            if (empty) empty.classList.toggle('d-none', list.querySelectorAll('[data-bus]:not(.d-none)').length > 0);
        }
        if (sort) sort.addEventListener('change', run);
        if (type) type.addEventListener('change', run);
    }

    /* ---------- swap origin / destination ---------- */
    function initSwap() {
        var b = document.getElementById('swapPlaces');
        if (!b) return;
        b.addEventListener('click', function () {
            var f = document.getElementById(b.getAttribute('data-a')), t = document.getElementById(b.getAttribute('data-b'));
            if (!f || !t) return;
            var v = f.value; f.value = t.value; t.value = v;
        });
    }

    /* messages queued by the server before this script loaded: window.mbtQ.push([text, type]) */
    function drainQueue() {
        var pending = window.mbtQ;
        window.mbtQ = { push: function (a) { toast(a[0], a[1]); } };           // later pushes show immediately
        if (pending && pending.length) pending.forEach(function (a) { toast(a[0], a[1]); });
    }

    function init() {
        apply(stored()); drainQueue(); initReveal(); initCounters(); initPasswordUi(); initOtp(); initHoldTimer(); initResultTools(); initSwap();
        if (document.querySelector('.summary-bar')) document.body.classList.add('has-summary-bar');
    }
    if (document.readyState === 'loading') document.addEventListener('DOMContentLoaded', init); else init();

    window.mbt = { toast: toast, showLoading: showLoading, hideLoading: hideLoading, setTheme: setTheme };
})();