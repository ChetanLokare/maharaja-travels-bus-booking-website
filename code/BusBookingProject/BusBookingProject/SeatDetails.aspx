<%@ Page Title="Select seats" Language="C#" MasterPageFile="~/BusBookingMaster.Master" AutoEventWireup="true" CodeBehind="SeatDetails.aspx.cs" Inherits="BusBookingProject.SeatDetails" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server"></asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="container py-4">
        <ol class="steps" aria-label="Booking progress">
            <li class="done">Search</li><li class="current">Seats</li><li>Passengers</li><li>Payment</li><li>Ticket</li>
        </ol>

        <div class="d-flex flex-wrap justify-content-between align-items-start gap-2 mb-3">
            <div>
                <h1 class="h3 mb-1"><asp:Literal ID="litRoute" runat="server" /></h1>
                <div class="text-body-secondary"><asp:Literal ID="litMeta" runat="server" /></div>
            </div>
            <a href="javascript:history.back()" class="btn btn-outline-primary btn-sm"><i class="fa-solid fa-arrow-left me-1"></i>Back to results</a>
        </div>

        <div class="row g-4">
            <div class="col-lg-7 col-xl-8">
                <div class="card p-3 p-md-4">
                    <div class="d-flex flex-wrap gap-3 justify-content-center small mb-4" aria-label="Seat legend">
                        <span><span class="legend-dot legend-free"></span>Available</span>
                        <span><span class="legend-dot legend-sel"></span>Selected</span>
                        <span><span class="legend-dot legend-held"></span>Held by someone</span>
                        <span><span class="legend-dot legend-booked"></span>Booked</span>
                    </div>
                    <div class="bus-shell">
                        <div class="bus-front"><span><i class="fa-solid fa-arrow-up me-1"></i>Front</span><i class="fa-solid fa-circle-notch steering" title="Driver"></i></div>
                        <div id="seatMap"
                             data-rows="<%= SeatRows %>" data-cols="<%= SeatCols %>" data-total="<%= TotalSeats %>" data-fare="<%= FareInvariant %>" data-max="<%= MaxSeats %>"
                             data-booked="<%= BookedCsv %>" data-held="<%= HeldCsv %>" data-mine="<%= MineCsv %>"
                             data-field="<%= hdnSeats.ClientID %>" data-button="<%= btnContinue.ClientID %>"></div>
                    </div>
                </div>
            </div>

            <div class="col-lg-5 col-xl-4">
                <div class="card summary-sticky summary-bar p-3 p-md-4">
                    <h2 class="h5 mb-3">Your selection</h2>
                    <div class="mb-2 small text-body-secondary" id="seatEmpty">Tap a green seat to select it (up to <%= MaxSeats %>).</div>
                    <div id="seatChips" class="d-flex flex-wrap gap-2 mb-3" aria-live="polite"></div>

                    <label class="form-label small fw-semibold mb-1" for="<%= ddlBoarding.ClientID %>"><i class="fa-solid fa-location-dot text-primary me-1"></i>Boarding point</label>
                    <asp:DropDownList ID="ddlBoarding" runat="server" CssClass="form-select mb-3" required="required"></asp:DropDownList>

                    <div class="d-flex justify-content-between align-items-center border-top pt-3 mb-3">
                        <div><span class="text-body-secondary small">Total (<span id="seatCount">0</span> seat<span class="d-none d-sm-inline">s</span>)</span></div>
                        <div class="fs-4 fw-bold text-primary" id="seatTotal">&#8377;0</div>
                    </div>
                    <asp:HiddenField ID="hdnSeats" runat="server" />
                    <asp:Button ID="btnContinue" runat="server" Text="Continue" CssClass="btn btn-primary btn-lg w-100" OnClick="btnContinue_Click" />
                    <div class="small text-body-secondary mt-2 text-center"><i class="fa-solid fa-lock me-1"></i>Seats are held for <%= HoldMinutes %> minutes once you continue.</div>
                </div>
            </div>
        </div>
    </div>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="scripts" runat="server">
    <script src="<%= ResolveUrl("~/js/seats.js") %>?v=2"></script>
</asp:Content>
