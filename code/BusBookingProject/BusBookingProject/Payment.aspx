<%@ Page Title="Payment" Language="C#" MasterPageFile="~/BusBookingMaster.Master" AutoEventWireup="true" CodeBehind="Payment.aspx.cs" Inherits="BusBookingProject.Payment" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server"></asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="container py-4">
        <ol class="steps" aria-label="Booking progress">
            <li class="done">Search</li><li class="done">Seats</li><li class="done">Passengers</li><li class="current">Payment</li><li>Ticket</li>
        </ol>

        <div class="row justify-content-center">
            <div class="col-lg-7 col-xl-6">
                <div class="card p-4 p-md-5" data-reveal>
                    <div class="d-flex justify-content-between align-items-center mb-3">
                        <h1 class="h4 mb-0">Review &amp; pay</h1>
                        <asp:Panel ID="pnlTimer" runat="server" CssClass="badge text-bg-warning fs-6" data-hold-until="0"><i class="fa-regular fa-clock me-1"></i><span data-hold-text>--:--</span></asp:Panel>
                    </div>

                    <asp:Panel ID="pnlExpired" runat="server" Visible="false" CssClass="alert alert-warning">
                        Your seat hold has expired, so these seats may no longer be available. Please <a href="Home.aspx" class="alert-link">search again</a>.
                    </asp:Panel>

                    <div class="fw-semibold fs-5"><asp:Literal ID="litRoute" runat="server" /></div>
                    <div class="text-body-secondary mb-3"><asp:Literal ID="litTrip" runat="server" /></div>

                    <table class="table table-sm align-middle">
                        <thead><tr><th>Passenger</th><th class="text-center">Seat</th><th class="text-end">Fare</th></tr></thead>
                        <tbody>
                            <asp:Repeater ID="rptPax" runat="server">
                                <ItemTemplate>
                                    <tr><td><%#: Eval("FullName") %></td><td class="text-center"><%#: Eval("SeatNo") %></td><td class="text-end">&#8377;<%#: Convert.ToDecimal(Eval("Fare")).ToString("N2") %></td></tr>
                                </ItemTemplate>
                            </asp:Repeater>
                        </tbody>
                        <tfoot><tr class="fw-bold"><td colspan="2">Total payable</td><td class="text-end fs-5 text-primary">&#8377;<asp:Literal ID="litTotal" runat="server" /></td></tr></tfoot>
                    </table>

                    <div id="payBox" runat="server" class="d-grid gap-2 mt-2">
                        <button type="button" id="payNow" class="btn btn-primary btn-lg"><i class="fa-solid fa-lock me-2"></i>Pay securely</button>
                        <div class="text-center small text-body-secondary">UPI &middot; Debit/Credit cards &middot; Net banking &mdash; processed by Razorpay</div>
                    </div>

                    <asp:HiddenField ID="hdnPayment" runat="server" />
                    <asp:HiddenField ID="hdnOrder" runat="server" />
                    <asp:HiddenField ID="hdnSignature" runat="server" />
                    <asp:Button ID="btnVerify" runat="server" OnClick="btnVerify_Click" CssClass="d-none" Text="verify" UseSubmitBehavior="true" CausesValidation="false" />

                    <div class="text-center mt-3">
                        <asp:LinkButton ID="btnCancel" runat="server" CssClass="small text-body-secondary" OnClick="btnCancel_Click" CausesValidation="false">Cancel booking</asp:LinkButton>
                    </div>
                </div>
            </div>
        </div>
    </div>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="scripts" runat="server">
    <script src="https://checkout.razorpay.com/v1/checkout.js"></script>
    <script src="<%= ResolveUrl("~/js/payment.js") %>?v=<%= System.IO.File.GetLastWriteTimeUtc(Server.MapPath("~/js/payment.js")).Ticks %>"></script>
</asp:Content>
