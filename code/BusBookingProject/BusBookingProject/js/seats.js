/* Seat map: builds the layout from data attributes and keeps the hidden field in sync. */
(function () {
  'use strict';
  var host = document.getElementById('seatMap');
  if (!host) return;

  var rows = parseInt(host.dataset.rows, 10);         // seats across the bus width (A..)
  var cols = parseInt(host.dataset.cols, 10);         // seat rows along the bus length
  var fare = parseFloat(host.dataset.fare);
  var max = parseInt(host.dataset.max, 10) || 3;
  var total = parseInt(host.dataset.total, 10);
  function list(name) { return (host.dataset[name] || '').split(',').filter(Boolean).map(Number); }
  var booked = new Set(list('booked')), held = new Set(list('held'));
  var selected = [];
  list('mine').forEach(function (n) { if (selected.length < max && !booked.has(n) && !held.has(n)) selected.push(n); });

  var hidden = document.getElementById(host.dataset.field);
  (hidden.value || '').split(',').filter(Boolean).map(Number).forEach(function (n) {
    if (selected.indexOf(n) < 0 && selected.length < max && !booked.has(n) && !held.has(n) && n >= 1 && n <= total) selected.push(n);
  });
  var chips = document.getElementById('seatChips'), totalEl = document.getElementById('seatTotal'),
      countEl = document.getElementById('seatCount'), btn = document.getElementById(host.dataset.button),
      empty = document.getElementById('seatEmpty');
  var money = new Intl.NumberFormat('en-IN', { style: 'currency', currency: 'INR', maximumFractionDigits: 0 });

  // numbering is unchanged from the original system: seat = row + column * rows + 1
  var leftCount = Math.ceil(rows / 2);
  var map = document.createElement('div');
  for (var j = 0; j < cols; j++) {
    var line = document.createElement('div'); line.className = 'seat-row';
    for (var i = 0; i < rows; i++) {
      var no = i + j * rows + 1;
      if (no > total) continue;
      if (rows > 1 && i === leftCount) { var a = document.createElement('span'); a.className = 'seat-aisle'; line.appendChild(a); }
      var s = document.createElement('button');
      s.type = 'button'; s.className = 'seat'; s.textContent = no; s.dataset.no = no;
      if (booked.has(no)) { s.classList.add('booked'); s.disabled = true; s.setAttribute('aria-label', 'Seat ' + no + ', booked'); s.title = 'Booked'; }
      else if (held.has(no)) { s.classList.add('held'); s.disabled = true; s.setAttribute('aria-label', 'Seat ' + no + ', held by another customer'); s.title = 'Being booked by someone else'; }
      else { s.setAttribute('aria-pressed', 'false'); s.setAttribute('aria-label', 'Seat ' + no + ', ' + money.format(fare)); s.title = 'Seat ' + no + ' - ' + money.format(fare); }
      line.appendChild(s);
    }
    map.appendChild(line);
  }
  host.appendChild(map);

  function render() {
    host.querySelectorAll('.seat:not(.booked):not(.held)').forEach(function (s) {
      var on = selected.indexOf(Number(s.dataset.no)) >= 0;
      s.classList.toggle('selected', on); s.setAttribute('aria-pressed', on ? 'true' : 'false');
    });
    chips.textContent = '';
    selected.slice().sort(function (a, b) { return a - b; }).forEach(function (n) {
      var c = document.createElement('span'); c.className = 'seat-chip'; c.textContent = 'Seat ' + n; chips.appendChild(c);
    });
    empty.classList.toggle('d-none', selected.length > 0);
    countEl.textContent = selected.length;
    totalEl.textContent = money.format(fare * selected.length);
    hidden.value = selected.slice().sort(function (a, b) { return a - b; }).join(',');
    btn.disabled = selected.length === 0;
  }

  host.addEventListener('click', function (e) {
    var s = e.target.closest('.seat');
    if (!s || s.disabled) return;
    var n = Number(s.dataset.no), idx = selected.indexOf(n);
    if (idx >= 0) selected.splice(idx, 1);
    else if (selected.length >= max) { window.mbt && mbt.toast('You can select up to ' + max + ' seats per booking.', 'warning'); return; }
    else selected.push(n);
    render();
  });
  render();
})();
