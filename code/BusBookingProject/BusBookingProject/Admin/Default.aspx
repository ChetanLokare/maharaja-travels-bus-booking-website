<%@ Page Title="Dashboard" Language="C#" MasterPageFile="~/Admin/Admin.Master" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="BusBookingProject.Admin.Default" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server"></asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <h1 class="h3 mb-4">Dashboard</h1>
    <div class="row g-3">
        <div class="col-6 col-xl-3" data-reveal><div class="feature-card"><div class="feature-icon"><i class="fa-solid fa-bus"></i></div><div class="stat-num"><asp:Literal ID="litBuses" runat="server" /></div><div class="text-body-secondary">Buses</div></div></div>
        <div class="col-6 col-xl-3" data-reveal><div class="feature-card"><div class="feature-icon"><i class="fa-solid fa-calendar-days"></i></div><div class="stat-num"><asp:Literal ID="litSchedules" runat="server" /></div><div class="text-body-secondary">Upcoming trips</div></div></div>
        <div class="col-6 col-xl-3" data-reveal><div class="feature-card"><div class="feature-icon"><i class="fa-solid fa-ticket"></i></div><div class="stat-num"><asp:Literal ID="litBookings" runat="server" /></div><div class="text-body-secondary">Confirmed bookings</div></div></div>
        <div class="col-6 col-xl-3" data-reveal><div class="feature-card"><div class="feature-icon"><i class="fa-solid fa-indian-rupee-sign"></i></div><div class="stat-num">&#8377;<asp:Literal ID="litRevenue" runat="server" /></div><div class="text-body-secondary">Revenue received</div></div></div>
    </div>
    <div class="row g-3 mt-1">
        <div class="col-md-6" data-reveal><div class="feature-card"><h2 class="h6">Payments needing attention</h2><div class="display-6 fw-bold"><asp:Literal ID="litRefunds" runat="server" /></div><div class="small text-body-secondary">Refunds pending after a seat conflict. Process these in the Razorpay dashboard.</div></div></div>
        <div class="col-md-6" data-reveal><div class="feature-card"><h2 class="h6">Failed sign-ins (24 h)</h2><div class="display-6 fw-bold"><asp:Literal ID="litFailed" runat="server" /></div><div class="small text-body-secondary">Customer and admin sign-in failures recorded in the audit log.</div></div></div>
    </div>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="scripts" runat="server"></asp:Content>
