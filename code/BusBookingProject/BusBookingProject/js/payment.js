/* Opens Razorpay Checkout. Values come from server-rendered data attributes; the result is verified server-side. */
(function () {
    'use strict';
    // payBox is a server control, so ASP.NET renames its id (e.g. ContentPlaceHolder1_payBox) - find it by its data attributes instead
    var box = document.querySelector('[data-order][data-key]');
    var btn = document.getElementById('payNow');
    if (!box || !btn) { if (window.console) console.warn('payment.js: pay box or button not found'); return; }
    var f = function (id) { return document.getElementById(box.dataset[id]); };

    function open() {
        if (typeof Razorpay === 'undefined') { mbt.toast('Could not load the payment window. Check your connection and try again.', 'danger'); return; }
        var settled = false;
        var rzp = new Razorpay({
            key: box.dataset.key,
            order_id: box.dataset.order,
            amount: box.dataset.amount,
            currency: 'INR',
            name: "Maharaja Travel's",
            description: 'Bus ticket ' + box.dataset.pnr,
            prefill: { name: box.dataset.name, email: box.dataset.email, contact: box.dataset.contact },
            theme: { color: '#4338ca' },
            modal: { ondismiss: function () { if (!settled) { btn.disabled = false; mbt.toast('Payment was cancelled. Your seats stay held until the timer ends.', 'warning'); } }, confirm_close: true },
            handler: function (resp) {
                settled = true;
                f('fPayment').value = resp.razorpay_payment_id || '';
                f('fOrder').value = resp.razorpay_order_id || '';
                f('fSignature').value = resp.razorpay_signature || '';
                mbt.showLoading();
                document.getElementById(box.dataset.verify).click();
            }
        });
        rzp.on('payment.failed', function (r) {
            btn.disabled = false;
            mbt.toast((r && r.error && r.error.description) || 'Payment failed. You can try again.', 'danger');
        });
        btn.disabled = true;
        rzp.open();
    }
    btn.addEventListener('click', function (e) { e.preventDefault(); open(); });
})();
