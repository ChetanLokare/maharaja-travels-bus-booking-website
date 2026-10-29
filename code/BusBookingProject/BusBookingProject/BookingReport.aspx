<%@ Page Title="My bookings" Language="C#" MasterPageFile="~/BusBookingMaster.Master" AutoEventWireup="true" CodeBehind="BookingReport.aspx.cs" Inherits="BusBookingProject.BookingReport" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server"></asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="container py-4">
        <div class="d-flex justify-content-between align-items-center mb-3">
            <h1 class="h3 mb-0">My bookings</h1>
            <a href="Home.aspx" class="btn btn-primary"><i class="fa-solid fa-plus me-1"></i>New booking</a>
        </div>

        <asp:Panel ID="pnlEmpty" runat="server" Visible="false" CssClass="card p-5 text-center">
            <i class="fa-solid fa-ticket display-4 text-body-secondary mb-3"></i>
            <h2 class="h5">No bookings yet</h2>
            <p class="text-body-secondary">When you book a trip, your tickets and receipts will show up here.</p>
            <div><a href="Home.aspx" class="btn btn-primary">Find a bus</a></div>
        </asp:Panel>

        <div class="vstack gap-3">
            <asp:Repeater ID="rptBookings" runat="server">
                <ItemTemplate>
                    <div class="bus-card p-3 p-md-4" data-reveal>
                        <div class="row g-3 align-items-center">
                            <div class="col-md-3"><div class="small text-body-secondary">PNR</div><div class="fs-5 fw-bold text-primary"><%#: Eval("PNRNo") %></div><span class="badge text-bg-success">Confirmed</span></div>
                            <div class="col-md-4">
                                <div class="fw-semibold"><%#: Eval("Origin") %> <i class="fa-solid fa-arrow-right mx-1 small"></i> <%#: Eval("Destination") %></div>
                                <div class="small text-body-secondary"><%#: Eval("BusName") %> &middot; <%#: Eval("TravelDate") %></div>
                            </div>
                            <div class="col-6 col-md-2"><div class="small text-body-secondary"><%#: Eval("TotalTickets") %> seat(s)</div><div class="fw-bold">&#8377;<%#: Convert.ToDecimal(Eval("TotalAmount")).ToString("N2") %></div></div>
                            <div class="col-6 col-md-3 text-end d-flex flex-wrap gap-2 justify-content-end">
                                <a class="btn btn-sm btn-outline-primary" href="BookingConfirmation.aspx?pnr=<%#: Eval("PNRNo") %>">View</a>
                                <a class="btn btn-sm btn-primary" data-no-overlay="1" href="Download.ashx?pnr=<%#: Eval("PNRNo") %>&amp;type=ticket" title="Download ticket"><i class="fa-solid fa-ticket"></i></a>
                                <a class="btn btn-sm btn-primary" data-no-overlay="1" href="Download.ashx?pnr=<%#: Eval("PNRNo") %>&amp;type=receipt" title="Download receipt"><i class="fa-solid fa-receipt"></i></a>
                            </div>
                        </div>
                    </div>
                </ItemTemplate>
            </asp:Repeater>
        </div>
    </div>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="scripts" runat="server"></asp:Content>
