<%@ Page Title="Create account" Language="C#" MasterPageFile="~/BusBookingMaster.Master" AutoEventWireup="true" CodeBehind="UserRegistration.aspx.cs" Inherits="BusBookingProject.UserRegistration" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server"></asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="container auth-wrap py-4">
        <div class="card auth-card p-4 p-md-5" style="max-width:640px">
            <div class="text-center mb-4">
                <h1 class="h3 mb-1">Create your account</h1>
                <p class="text-body-secondary mb-0">We will e-mail a code to confirm your address.</p>
            </div>

            <% if (GoogleEnabled || MicrosoftEnabled) { %>
            <div class="row g-2 mb-3">
                <% if (GoogleEnabled) { %><div class="col"><a class="btn social-btn py-2" href="OAuthStart.ashx?provider=google" data-no-overlay="1"><i class="fa-brands fa-google" style="color:#ea4335"></i>Google</a></div><% } %>
                <% if (MicrosoftEnabled) { %><div class="col"><a class="btn social-btn py-2" href="OAuthStart.ashx?provider=microsoft" data-no-overlay="1"><i class="fa-brands fa-microsoft" style="color:#00a4ef"></i>Microsoft</a></div><% } %>
            </div>
            <div class="divider-text mb-3">or use e-mail</div>
            <% } %>

            <div class="row g-3">
                <div class="col-sm-6"><label class="form-label small">First name</label><asp:TextBox ID="txtFirstName" runat="server" CssClass="form-control" MaxLength="50" required="required" autocomplete="given-name"></asp:TextBox></div>
                <div class="col-sm-6"><label class="form-label small">Last name</label><asp:TextBox ID="txtLastName" runat="server" CssClass="form-control" MaxLength="50" required="required" autocomplete="family-name"></asp:TextBox></div>
                <div class="col-sm-7"><label class="form-label small">E-mail</label><asp:TextBox ID="txtEmail" runat="server" CssClass="form-control" TextMode="Email" MaxLength="150" required="required" autocomplete="email"></asp:TextBox></div>
                <div class="col-sm-5"><label class="form-label small">Mobile (10 digits)</label><asp:TextBox ID="txtMobile" runat="server" CssClass="form-control" TextMode="Phone" MaxLength="15" required="required" autocomplete="tel" pattern="[+0-9 ]{10,15}"></asp:TextBox></div>
                <div class="col-12">
                    <label class="form-label small">Password</label>
                    <div class="input-group">
                        <asp:TextBox ID="txtPassword" runat="server" CssClass="form-control" TextMode="Password" MaxLength="128" required="required" autocomplete="new-password"></asp:TextBox>
                        <button class="btn btn-outline-secondary" type="button" data-toggle-password="<%= txtPassword.ClientID %>" aria-label="Show password"><i class="fa-solid fa-eye"></i></button>
                    </div>
                    <div class="mt-2"><div class="pw-meter" data-pw-meter="<%= txtPassword.ClientID %>"><span></span></div><div class="small text-body-secondary mt-1" data-pw-label>&nbsp;</div></div>
                    <div class="form-text">At least 10 characters with upper-case, lower-case and a number.</div>
                </div>
                <div class="col-12"><label class="form-label small">Confirm password</label><asp:TextBox ID="txtConfirm" runat="server" CssClass="form-control" TextMode="Password" MaxLength="128" required="required" autocomplete="new-password"></asp:TextBox></div>
                <div class="col-12"><label class="form-label small">Address <span class="text-body-secondary">(optional)</span></label><asp:TextBox ID="txtAddress" runat="server" CssClass="form-control" MaxLength="200" autocomplete="street-address"></asp:TextBox></div>
                <div class="col-sm-7"><label class="form-label small">City <span class="text-body-secondary">(optional)</span></label><asp:TextBox ID="txtCity" runat="server" CssClass="form-control" MaxLength="50" autocomplete="address-level2"></asp:TextBox></div>
                <div class="col-sm-5"><label class="form-label small">Pincode <span class="text-body-secondary">(optional)</span></label><asp:TextBox ID="txtPincode" runat="server" CssClass="form-control" MaxLength="6" inputmode="numeric" pattern="[1-9][0-9]{5}" autocomplete="postal-code"></asp:TextBox></div>
                <div class="col-12 form-check ms-1">
                    <input type="checkbox" id="chkTerms" runat="server" class="form-check-input" />
                    <label class="form-check-label small" for="<%= chkTerms.ClientID %>">I agree to the booking terms and the privacy policy.</label>
                </div>
                <div class="col-12 d-grid"><asp:Button ID="btnRegister" runat="server" Text="Create account" CssClass="btn btn-primary btn-lg" OnClick="btnRegister_Click" /></div>
            </div>
            <p class="text-center mt-4 mb-0">Already registered? <a href="Login.aspx" class="fw-semibold">Sign in</a></p>
        </div>
    </div>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="scripts" runat="server"></asp:Content>
