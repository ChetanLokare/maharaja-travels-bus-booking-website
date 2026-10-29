<%@ Page Title="Available buses" Language="C#" MasterPageFile="~/BusBookingMaster.Master" AutoEventWireup="true" CodeBehind="BusBookingSearchDetails.aspx.cs" Inherits="BusBookingProject.BusBookingSearchDetails" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server"></asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="container py-4">
        <ol class="steps" aria-label="Booking progress">
            <li class="current">Search</li><li>Seats</li><li>Passengers</li><li>Payment</li><li>Ticket</li>
        </ol>

        <div class="d-flex flex-wrap align-items-center justify-content-between gap-3 mb-3">
            <div>
                <h1 class="h3 mb-1"><asp:Literal ID="litRoute" runat="server" /></h1>
                <div class="text-body-secondary"><i class="fa-regular fa-calendar me-1"></i><asp:Literal ID="litDate" runat="server" /> &middot; <asp:Literal ID="litCount" runat="server" /></div>
            </div>
            <div class="d-flex gap-2 flex-wrap">
                <select id="filterType" class="form-select w-auto" aria-label="Filter by bus type"><option value="all">All types</option><option value="ac">AC</option><option value="nonac">Non-AC</option></select>
                <select id="sortBy" class="form-select w-auto" aria-label="Sort buses"><option value="time">Earliest departure</option><option value="fare">Lowest fare</option><option value="seats">Most seats left</option></select>
                <a href="Home.aspx" class="btn btn-outline-primary"><i class="fa-solid fa-pen me-1"></i>Modify</a>
            </div>
        </div>

        <asp:Panel ID="pnlEmpty" runat="server" Visible="false" CssClass="card p-5 text-center">
            <i class="fa-solid fa-bus-simple display-4 text-body-secondary mb-3"></i>
            <h2 class="h4">No buses found</h2>
            <p class="text-body-secondary"><asp:Literal ID="litEmpty" runat="server" /></p>
            <div><a href="Home.aspx" class="btn btn-primary">Search again</a></div>
        </asp:Panel>

        <div id="busList" class="vstack gap-3">
            <asp:Repeater ID="rptBuses" runat="server">
                <ItemTemplate>
                    <div class="bus-card p-3 p-md-4" data-bus data-reveal data-fare="<%#: Eval("Fare") %>" data-seats="<%#: Eval("AvailableSeats") %>" data-order="<%# Container.ItemIndex %>" data-type="<%#: TypeKey(Eval("BustType")) %>">
                        <div class="row g-3 align-items-center">
                            <div class="col-lg-3">
                                <div class="fw-bold fs-5"><%#: Eval("BusName") %></div>
                                <div class="small text-body-secondary"><%#: Eval("BusNo") %></div>
                                <span class="badge badge-soft mt-1"><%#: Eval("BustType") %></span>
                            </div>
                            <div class="col-lg-5">
                                <div class="d-flex align-items-center">
                                    <div class="text-center"><div class="bus-time"><%#: Eval("DepartureTime") %></div><div class="small text-body-secondary"><%#: Eval("Origin") %></div></div>
                                    <div class="bus-line"></div>
                                    <div class="text-center"><div class="bus-time"><%#: Eval("ArivalTime") %></div><div class="small text-body-secondary"><%#: Eval("Destination") %></div></div>
                                </div>
                                <div class="text-center small text-body-secondary mt-2"><i class="fa-regular fa-clock me-1"></i><%#: Eval("EstimatedTime") %></div>
                            </div>
                            <div class="col-6 col-lg-2">
                                <div class="small text-body-secondary mb-1"><%#: Eval("AvailableSeats") %> of <%#: Eval("TotalSeat") %> seats left</div>
                                <div class="progress" style="height:6px" role="progressbar" aria-label="Seat availability" aria-valuenow="<%#: Eval("AvailableSeats") %>" aria-valuemin="0" aria-valuemax="<%#: Eval("TotalSeat") %>">
                                    <div class="progress-bar" style="width:<%#: SeatPercent(Eval("AvailableSeats"), Eval("TotalSeat")) %>%"></div>
                                </div>
                            </div>
                            <div class="col-6 col-lg-2 text-end">
                                <div class="fare">&#8377;<%#: Convert.ToDecimal(Eval("Fare")).ToString("N0") %></div>
                                <div class="small text-body-secondary mb-2">per seat</div>
                                <a class="btn btn-primary w-100 <%# Convert.ToInt32(Eval("AvailableSeats")) <= 0 ? "disabled" : "" %>" href="SeatDetails.aspx?sid=<%#: Eval("ScheduleId") %>">
                                    <%# Convert.ToInt32(Eval("AvailableSeats")) <= 0 ? "Sold out" : "Select seats" %>
                                </a>
                            </div>
                        </div>
                    </div>
                </ItemTemplate>
            </asp:Repeater>
        </div>
        <div id="noMatch" class="alert alert-info mt-3 d-none">No buses match that filter.</div>
    </div>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="scripts" runat="server"></asp:Content>
