<%@ Page Title="Booking confirmed" Language="C#" MasterPageFile="~/BusBookingMaster.Master" AutoEventWireup="true" CodeBehind="BookingConfirmation.aspx.cs" Inherits="BusBookingProject.BookingConfirmation" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server"></asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="container py-4">
        <ol class="steps" aria-label="Booking progress">
            <li class="done">Search</li><li class="done">Seats</li><li class="done">Passengers</li><li class="done">Payment</li><li class="current">Ticket</li>
        </ol>

        <div class="text-center mb-4">
            <svg class="tick" viewBox="0 0 84 84" aria-hidden="true"><circle cx="42" cy="42" r="30" /><path d="M28 43 l10 10 l19 -22" /></svg>
            <h1 class="h2 mt-2">Booking confirmed!</h1>
            <p class="text-body-secondary mb-0">Your e-ticket and receipt are on their way to <strong><asp:Literal ID="litEmail" runat="server" /></strong>.</p>
        </div>

        <div class="row justify-content-center">
            <div class="col-lg-8">
                <div class="ticket-card p-4 p-md-5" data-reveal>
                    <div class="d-flex flex-wrap justify-content-between gap-2 mb-3">
                        <div><div class="small text-body-secondary">PNR</div><div class="fs-3 fw-bold text-primary"><asp:Literal ID="litPnr" runat="server" /></div></div>
                        <div class="text-end"><div class="small text-body-secondary">Amount paid</div><div class="fs-3 fw-bold">&#8377;<asp:Literal ID="litAmount" runat="server" /></div></div>
                    </div>
                    <div class="fw-semibold fs-5"><asp:Literal ID="litRoute" runat="server" /></div>
                    <div class="text-body-secondary mb-3"><asp:Literal ID="litTrip" runat="server" /></div>
                    <div class="small mb-3"><i class="fa-solid fa-location-dot text-primary me-1"></i>Boarding: <asp:Literal ID="litBoarding" runat="server" /></div>
                    <div class="d-flex flex-wrap gap-2 mb-4">
                        <asp:Repeater ID="rptPax" runat="server">
                            <ItemTemplate><span class="seat-chip"><i class="fa-solid fa-user"></i><%#: Eval("FullName") %> &middot; Seat <%#: Eval("SeatNo") %></span></ItemTemplate>
                        </asp:Repeater>
                    </div>
                    <div class="d-flex flex-wrap gap-2">
                        <a id="lnkTicket" runat="server" class="btn btn-primary" data-no-overlay="1"><i class="fa-solid fa-ticket me-2"></i>Download ticket</a>
                        <a id="lnkReceipt" runat="server" class="btn btn-outline-primary" data-no-overlay="1"><i class="fa-solid fa-receipt me-2"></i>Download receipt</a>
                        <asp:LinkButton ID="btnResend" runat="server" CssClass="btn btn-link" OnClick="btnResend_Click" CausesValidation="false"><i class="fa-solid fa-envelope me-2"></i>E-mail again</asp:LinkButton>
                    </div>
                </div>
                <div class="text-center mt-4"><a href="BookingReport.aspx" class="me-3">My bookings</a><a href="Home.aspx">Book another trip</a></div>
            </div>
        </div>
    </div>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="scripts" runat="server"></asp:Content>
