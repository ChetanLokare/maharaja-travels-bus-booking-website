<%@ Page Title="About us" Language="C#" MasterPageFile="~/BusBookingMaster.Master" AutoEventWireup="true" CodeBehind="AboutUs.aspx.cs" Inherits="BusBookingProject.AboutUs" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server"></asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="container py-4">
        <div class="row justify-content-center">
            <div class="col-lg-9">
                <div class="card p-4 p-md-5 mb-4" data-reveal>
                    <h1 class="h2 mb-3"><i class="fa-solid fa-bus text-primary me-2"></i>About Maharaja Travel's</h1>
                    <p class="lead mb-0">We are among the top 10 online bus-booking service providers in India, with branches in all major cities. Our service is fast, efficient and friendly, with round-the-clock support and every query resolved within a day.</p>
                </div>
                <div class="card p-4 p-md-5" data-reveal>
                    <h2 class="h4 mb-3">Contact us</h2>
                    <ul class="list-unstyled mb-0 vstack gap-3">
                        <li><i class="fa-solid fa-envelope text-primary fa-fw me-2"></i><a href="mailto:MaharajaTravels@hotmail.com">MaharajaTravels@hotmail.com</a></li>
                        <li><i class="fa-solid fa-phone text-primary fa-fw me-2"></i><a href="tel:08844567244">08844567244</a></li>
                        <li><i class="fa-solid fa-location-dot text-primary fa-fw me-2"></i>Maharaja Travels, Kolhapur Naka, Karad, Maharashtra, 415114</li>
                    </ul>
                </div>
            </div>
        </div>
    </div>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="scripts" runat="server"></asp:Content>
