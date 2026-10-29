<%@ Page Title="Verify code" Language="C#" MasterPageFile="~/BusBookingMaster.Master" AutoEventWireup="true" CodeBehind="OtpVerify.aspx.cs" Inherits="BusBookingProject.OtpVerify" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server"></asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="container auth-wrap py-4">
        <div class="card auth-card p-4 p-md-5 text-center">
            <span class="brand-mark mx-auto mb-3" style="width:56px;height:56px;font-size:1.5rem"><i class="fa-solid fa-envelope-open-text"></i></span>
            <h1 class="h3 mb-2">Enter your code</h1>
            <p class="text-body-secondary">We sent a 6-digit code to <strong><asp:Literal ID="litTarget" runat="server" /></strong>. It expires in 10 minutes.</p>

            <asp:Panel ID="pnlDev" runat="server" Visible="false" CssClass="alert alert-warning text-start small" role="status">
                <strong>Developer mode.</strong> SMTP is not configured and you are on localhost, so no e-mail was sent. Your code is
                <span class="d-block font-monospace fs-3 fw-bold text-center my-1"><asp:Literal ID="litDevCode" runat="server" /></span>
                <span class="text-body-secondary">This box never appears on a server with e-mail configured.</span>
            </asp:Panel>

            <asp:TextBox ID="txtCode" runat="server" CssClass="form-control form-control-lg otp-input mb-3" MaxLength="6" inputmode="numeric" autocomplete="one-time-code" placeholder="------" data-otp="1" required="required" pattern="[0-9]{6}"></asp:TextBox>
            <asp:Button ID="btnVerify" runat="server" Text="Verify" CssClass="btn btn-primary btn-lg w-100" OnClick="btnVerify_Click" />

            <div class="mt-3 small">
                <asp:LinkButton ID="btnResend" runat="server" OnClick="btnResend_Click" CausesValidation="false">Send a new code</asp:LinkButton>
                <div class="text-body-secondary mt-1" data-countdown="<%= ResendWait %>" data-countdown-target="<%= btnResend.ClientID %>"></div>
            </div>
            <div class="mt-3 small"><a href="Login.aspx" class="text-body-secondary"><i class="fa-solid fa-arrow-left me-1"></i>Back to sign in</a></div>
        </div>
    </div>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="scripts" runat="server"></asp:Content>
