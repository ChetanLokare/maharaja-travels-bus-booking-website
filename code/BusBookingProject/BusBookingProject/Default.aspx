<%@ Page Title="Maharaja Travel's - Book bus tickets online" Language="C#" MasterPageFile="~/BusBookingMaster.Master" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="BusBookingProject.Default" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server"></asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <section class="hero pb-5">
        <div class="container py-5">
            <div class="row align-items-center g-4 py-lg-4">
                <div class="col-lg-7">
                    <span class="badge rounded-pill text-bg-light mb-3 px-3 py-2"><i class="fa-solid fa-shield-heart me-1 text-primary"></i>Secure UPI, card &amp; net banking payments</span>
                    <h1 class="hero-title mb-3">Your next journey,<br /><span class="accent">booked in a minute.</span></h1>
                    <p class="lead opacity-75 mb-4">Pick your bus, choose the exact seat you like and get your e-ticket on e-mail instantly.</p>
                    <div class="d-flex flex-wrap gap-2">
                        <a href="Home.aspx" class="btn btn-warning btn-lg fw-semibold px-4"><i class="fa-solid fa-magnifying-glass me-2"></i>Search buses</a>
                        <a href="BookingReport.aspx" class="btn btn-outline-light btn-lg px-4"><i class="fa-solid fa-ticket me-2"></i>My bookings</a>
                    </div>
                </div>
                <div class="col-lg-5 d-none d-lg-block text-center">
                    <svg class="float-y" width="100%" style="max-width:380px" viewBox="0 0 320 190" role="img" aria-label="Illustration of a bus">
                        <rect x="10" y="30" rx="26" width="300" height="120" fill="#ffffff" opacity=".96" />
                        <rect x="10" y="108" width="300" height="14" fill="#f59e0b" />
                        <g fill="#c7d2fe"><rect x="28" y="48" width="48" height="38" rx="8" /><rect x="86" y="48" width="48" height="38" rx="8" /><rect x="144" y="48" width="48" height="38" rx="8" /><rect x="202" y="48" width="48" height="38" rx="8" /></g>
                        <rect x="262" y="48" width="38" height="52" rx="8" fill="#93c5fd" />
                        <circle cx="80" cy="152" r="20" fill="#1f2937" /><circle cx="80" cy="152" r="9" fill="#9ca3af" />
                        <circle cx="240" cy="152" r="20" fill="#1f2937" /><circle cx="240" cy="152" r="9" fill="#9ca3af" />
                        <rect x="300" y="120" width="10" height="10" rx="3" fill="#fde68a" />
                    </svg>
                </div>
            </div>
        </div>
        <div class="hero-road" aria-hidden="true">
            <svg class="hero-bus" viewBox="0 0 130 56"><rect x="2" y="6" rx="10" width="124" height="38" fill="#fff" /><rect x="2" y="30" width="124" height="5" fill="#f59e0b" /><g fill="#c7d2fe"><rect x="10" y="12" width="20" height="14" rx="3" /><rect x="36" y="12" width="20" height="14" rx="3" /><rect x="62" y="12" width="20" height="14" rx="3" /><rect x="88" y="12" width="20" height="14" rx="3" /></g><circle cx="30" cy="46" r="8" fill="#1f2937" /><circle cx="98" cy="46" r="8" fill="#1f2937" /></svg>
        </div>
    </section>

    <section class="container py-5">
        <div class="text-center mb-5" data-reveal>
            <h2 class="fw-bold">Why travellers choose us</h2>
            <p class="text-body-secondary mb-0">Everything you need for a smooth ride.</p>
        </div>
        <div class="row g-4">
            <div class="col-sm-6 col-lg-3" data-reveal><div class="feature-card"><div class="feature-icon"><i class="fa-solid fa-couch"></i></div><h3 class="h5">Pick your seat</h3><p class="text-body-secondary mb-0">A clear seat map shows what is free, taken or being held by someone else, live.</p></div></div>
            <div class="col-sm-6 col-lg-3" data-reveal><div class="feature-card"><div class="feature-icon"><i class="fa-solid fa-lock"></i></div><h3 class="h5">Safe payments</h3><p class="text-body-secondary mb-0">Pay by UPI, card or net banking on Razorpay. We never see or store your card details.</p></div></div>
            <div class="col-sm-6 col-lg-3" data-reveal><div class="feature-card"><div class="feature-icon"><i class="fa-solid fa-envelope-circle-check"></i></div><h3 class="h5">Instant e-ticket</h3><p class="text-body-secondary mb-0">Ticket and receipt PDFs land in your inbox the moment payment succeeds.</p></div></div>
            <div class="col-sm-6 col-lg-3" data-reveal><div class="feature-card"><div class="feature-icon"><i class="fa-solid fa-mobile-screen-button"></i></div><h3 class="h5">Works on any screen</h3><p class="text-body-secondary mb-0">Phone, tablet or desktop, in light or dark mode - the layout adapts.</p></div></div>
        </div>
    </section>

    <section class="container pb-4">
        <div class="row g-4 text-center">
            <div class="col-6 col-md-3" data-reveal><div class="stat-num" data-count="40" data-suffix="+">0</div><div class="text-body-secondary">Cities connected</div></div>
            <div class="col-6 col-md-3" data-reveal><div class="stat-num" data-count="120" data-suffix="+">0</div><div class="text-body-secondary">Daily departures</div></div>
            <div class="col-6 col-md-3" data-reveal><div class="stat-num" data-count="25000" data-suffix="+">0</div><div class="text-body-secondary">Happy passengers</div></div>
            <div class="col-6 col-md-3" data-reveal><div class="stat-num" data-count="24" data-suffix="x7">0</div><div class="text-body-secondary">Support</div></div>
        </div>
    </section>

    <section class="container py-5">
        <div id="gallery" class="carousel slide carousel-fade rounded-4 overflow-hidden shadow" data-bs-ride="carousel" data-reveal>
            <div class="carousel-indicators">
                <button type="button" data-bs-target="#gallery" data-bs-slide-to="0" class="active" aria-current="true" aria-label="Slide 1"></button>
                <button type="button" data-bs-target="#gallery" data-bs-slide-to="1" aria-label="Slide 2"></button>
                <button type="button" data-bs-target="#gallery" data-bs-slide-to="2" aria-label="Slide 3"></button>
            </div>
            <div class="carousel-inner">
                <div class="carousel-item active"><img src="img/Bus2.jpg" class="d-block gallery-img" alt="Luxury coach on the highway" loading="lazy" decoding="async" /></div>
                <div class="carousel-item"><img src="img/Bus3.jpg" class="d-block gallery-img" alt="Comfortable sleeper coach" loading="lazy" decoding="async" /></div>
                <div class="carousel-item"><img src="img/Bus4.jpg" class="d-block gallery-img" alt="Our fleet" loading="lazy" decoding="async" /></div>
            </div>
            <button class="carousel-control-prev" type="button" data-bs-target="#gallery" data-bs-slide="prev"><span class="carousel-control-prev-icon" aria-hidden="true"></span><span class="visually-hidden">Previous</span></button>
            <button class="carousel-control-next" type="button" data-bs-target="#gallery" data-bs-slide="next"><span class="carousel-control-next-icon" aria-hidden="true"></span><span class="visually-hidden">Next</span></button>
        </div>
    </section>

    <section class="container pb-4" data-reveal>
        <div class="card border-0 text-center p-5 hero text-white">
            <h2 class="fw-bold">Ready to hit the road?</h2>
            <p class="opacity-75">Create a free account and book your first ticket in minutes.</p>
            <div><a href="Home.aspx" class="btn btn-warning btn-lg fw-semibold px-5">Find my bus</a></div>
        </div>
    </section>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="scripts" runat="server"></asp:Content>
