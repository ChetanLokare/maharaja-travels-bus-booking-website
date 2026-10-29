<%@ Page Title="Passenger details" Language="C#" MasterPageFile="~/BusBookingMaster.Master" AutoEventWireup="true" CodeBehind="PassengerDetailsInfo.aspx.cs" Inherits="BusBookingProject.PassengerDetailsInfo" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server"></asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="container py-4">
        <ol class="steps" aria-label="Booking progress">
            <li class="done">Search</li><li class="done">Seats</li><li class="current">Passengers</li><li>Payment</li><li>Ticket</li>
        </ol>

        <div class="row g-4">
            <div class="col-lg-8">
                <div class="d-flex justify-content-between align-items-center mb-3">
                    <h1 class="h3 mb-0">Who is travelling?</h1>
                    <div class="badge text-bg-warning fs-6 fw-semibold" data-hold-until="<%= HoldSecondsLeft %>"><i class="fa-regular fa-clock me-1"></i>Seats held <span data-hold-text>--:--</span></div>
                </div>

                <asp:Repeater ID="rptPassengers" runat="server">
                    <ItemTemplate>
                        <div class="card p-3 p-md-4 mb-3" data-reveal>
                            <div class="d-flex justify-content-between align-items-center mb-3">
                                <h2 class="h6 mb-0"><i class="fa-solid fa-user me-2 text-primary"></i>Passenger <%# Container.ItemIndex + 1 %></h2>
                                <span class="seat-chip">Seat <%#: Eval("Seat") %></span>
                            </div>
                            <asp:HiddenField ID="hdnSeat" runat="server" Value='<%# Eval("Seat") %>' />
                            <div class="row g-3">
                                <div class="col-sm-6"><label class="form-label small">First name</label><asp:TextBox ID="txtFirst" runat="server" CssClass="form-control" MaxLength="50" required="required" autocomplete="given-name"></asp:TextBox></div>
                                <div class="col-sm-6"><label class="form-label small">Last name</label><asp:TextBox ID="txtLast" runat="server" CssClass="form-control" MaxLength="50" required="required" autocomplete="family-name"></asp:TextBox></div>
                                <div class="col-sm-6"><label class="form-label small">Age</label><asp:TextBox ID="txtAge" runat="server" CssClass="form-control" TextMode="Number" min="1" max="120" required="required"></asp:TextBox></div>
                                <div class="col-sm-6"><label class="form-label small">Gender</label>
                                    <asp:DropDownList ID="ddlGender" runat="server" CssClass="form-select" required="required">
                                        <asp:ListItem Value="" Text="Select"></asp:ListItem>
                                        <asp:ListItem Value="M" Text="Male"></asp:ListItem>
                                        <asp:ListItem Value="F" Text="Female"></asp:ListItem>
                                        <asp:ListItem Value="O" Text="Other"></asp:ListItem>
                                    </asp:DropDownList>
                                </div>
                            </div>
                        </div>
                    </ItemTemplate>
                </asp:Repeater>

                <div class="card p-3 p-md-4 mb-3" data-reveal>
                    <h2 class="h6 mb-1"><i class="fa-solid fa-envelope me-2 text-primary"></i>Where should we send the ticket?</h2>
                    <p class="small text-body-secondary">Your e-ticket and payment receipt are e-mailed here.</p>
                    <div class="row g-3">
                        <div class="col-sm-7"><label class="form-label small">E-mail</label><asp:TextBox ID="txtEmail" runat="server" CssClass="form-control" TextMode="Email" MaxLength="150" required="required" autocomplete="email"></asp:TextBox></div>
                        <div class="col-sm-5"><label class="form-label small">Mobile (10 digits)</label><asp:TextBox ID="txtMobile" runat="server" CssClass="form-control" TextMode="Phone" MaxLength="15" required="required" autocomplete="tel" pattern="[+0-9 ]{10,15}"></asp:TextBox></div>
                    </div>
                </div>
            </div>

            <div class="col-lg-4">
                <div class="card summary-sticky p-3 p-md-4">
                    <h2 class="h5">Trip summary</h2>
                    <div class="fw-semibold"><asp:Literal ID="litRoute" runat="server" /></div>
                    <div class="small text-body-secondary mb-3"><asp:Literal ID="litTrip" runat="server" /></div>
                    <dl class="row small mb-3">
                        <dt class="col-5 fw-normal text-body-secondary">Seats</dt><dd class="col-7 mb-1"><asp:Literal ID="litSeats" runat="server" /></dd>
                        <dt class="col-5 fw-normal text-body-secondary">Boarding</dt><dd class="col-7 mb-1"><asp:Literal ID="litBoarding" runat="server" /></dd>
                        <dt class="col-5 fw-normal text-body-secondary">Fare / seat</dt><dd class="col-7 mb-1">&#8377;<asp:Literal ID="litFare" runat="server" /></dd>
                    </dl>
                    <div class="d-flex justify-content-between align-items-center border-top pt-3 mb-3"><span>Total payable</span><span class="fs-4 fw-bold text-primary">&#8377;<asp:Literal ID="litTotal" runat="server" /></span></div>
                    <asp:Button ID="btnPay" runat="server" Text="Proceed to payment" CssClass="btn btn-primary btn-lg w-100" OnClick="btnPay_Click" />
                    <asp:LinkButton ID="btnCancel" runat="server" CssClass="btn btn-link w-100 text-body-secondary mt-1" OnClick="btnCancel_Click" CausesValidation="false" formnovalidate="formnovalidate">Cancel and release seats</asp:LinkButton>
                    <div class="small text-body-secondary mt-2 text-center"><i class="fa-solid fa-shield-halved me-1"></i>The fare is calculated on our server.</div>
                </div>
            </div>
        </div>
    </div>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="scripts" runat="server"></asp:Content>
