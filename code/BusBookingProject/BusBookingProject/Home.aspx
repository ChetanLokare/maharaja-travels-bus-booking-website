<%@ Page Title="Search buses" Language="C#" MasterPageFile="~/BusBookingMaster.Master" AutoEventWireup="true" CodeBehind="Home.aspx.cs" Inherits="BusBookingProject.Home" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server"></asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <section class="hero pb-5">
        <div class="container py-5">
            <h1 class="hero-title h1 mb-2">Where are you <span class="accent">headed?</span></h1>
            <p class="opacity-75 mb-0">Search live schedules and pick your seat.</p>
        </div>
        <div class="hero-road" aria-hidden="true"></div>
    </section>

    <div class="container" style="margin-top:-3.2rem; position:relative; z-index:2">
        <div class="card search-card p-3 p-md-4">
            <asp:Panel ID="pnlError" runat="server" Visible="false" CssClass="alert alert-danger" role="alert"><asp:Literal ID="litError" runat="server" /></asp:Panel>
            <div class="row g-3 align-items-end">
                <div class="col-md">
                    <label for="<%= ddlOrigin.ClientID %>" class="form-label fw-semibold"><i class="fa-solid fa-location-dot text-primary me-1"></i>From</label>
                    <asp:DropDownList ID="ddlOrigin" runat="server" CssClass="form-select form-select-lg" required="required"></asp:DropDownList>
                </div>
                <div class="col-md-auto text-center">
                    <button type="button" id="swapPlaces" class="btn btn-outline-primary rounded-circle swap-btn" data-a="<%= ddlOrigin.ClientID %>" data-b="<%= ddlDestination.ClientID %>" aria-label="Swap origin and destination"><i class="fa-solid fa-right-left"></i></button>
                </div>
                <div class="col-md">
                    <label for="<%= ddlDestination.ClientID %>" class="form-label fw-semibold"><i class="fa-solid fa-flag-checkered text-primary me-1"></i>To</label>
                    <asp:DropDownList ID="ddlDestination" runat="server" CssClass="form-select form-select-lg" required="required"></asp:DropDownList>
                </div>
                <div class="col-md-3">
                    <label for="<%= txtJourneyDate.ClientID %>" class="form-label fw-semibold"><i class="fa-regular fa-calendar text-primary me-1"></i>Journey date</label>
                    <asp:TextBox ID="txtJourneyDate" runat="server" TextMode="Date" CssClass="form-control form-control-lg" required="required"></asp:TextBox>
                </div>
                <div class="col-md-auto d-grid">
                    <asp:Button ID="btnSearch" runat="server" Text="Search" CssClass="btn btn-primary btn-lg px-4" OnClick="btnSearch_Click" />
                </div>
            </div>
        </div>

        <div class="row g-4 mt-2 text-center">
            <div class="col-md-4" data-reveal><i class="fa-solid fa-bolt text-primary fs-3 mb-2"></i><h2 class="h6">Live availability</h2><p class="small text-body-secondary mb-0">Seat counts update as people book.</p></div>
            <div class="col-md-4" data-reveal><i class="fa-solid fa-clock text-primary fs-3 mb-2"></i><h2 class="h6">10-minute seat hold</h2><p class="small text-body-secondary mb-0">Chosen seats are reserved while you pay.</p></div>
            <div class="col-md-4" data-reveal><i class="fa-solid fa-file-pdf text-primary fs-3 mb-2"></i><h2 class="h6">Ticket &amp; receipt by e-mail</h2><p class="small text-body-secondary mb-0">Both arrive as PDFs after payment.</p></div>
        </div>
    </div>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="scripts" runat="server"></asp:Content>
