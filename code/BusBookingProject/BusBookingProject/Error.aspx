<%@ Page Title="Something went wrong" Language="C#" MasterPageFile="~/BusBookingMaster.Master" AutoEventWireup="true" CodeBehind="Error.aspx.cs" Inherits="BusBookingProject.ErrorPage" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server"></asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="container py-5 text-center page-enter">
        <div class="display-1 fw-bold text-primary mb-2"><asp:Literal ID="litCode" runat="server" Text="Oops" /></div>
        <h1 class="h3 mb-3"><asp:Literal ID="litTitle" runat="server" /></h1>
        <p class="text-body-secondary mb-4"><asp:Literal ID="litMessage" runat="server" /></p>
        <a href="Default.aspx" class="btn btn-primary btn-lg"><i class="fa-solid fa-house me-2"></i>Back to home</a>
    </div>
</asp:Content>
