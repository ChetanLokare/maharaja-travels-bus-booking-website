<%@ Page Title="" Language="C#" MasterPageFile="~/Admin/Admin.Master" AutoEventWireup="true" CodeBehind="FeedbackReport.aspx.cs" Inherits="BusBookingProject.Admin.FeedbackReport" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="container" style="margin-top:6%">
     <asp:SqlDataSource ID="SqlDataSourceFeedback" runat="server" ConnectionString="<%$ ConnectionStrings:OnlineBusBookingConnectionString %>"
    SelectCommand="SELECT TOP 1000 [FeedbackID], [UserName], [Email], [ContactNo], [Rating], [FeedbackText], [ComplaintOrSuggestion], [FeedbackDate] FROM [Feedback]">
</asp:SqlDataSource>

<div class="table-responsive"><asp:GridView ID="GridViewFeedback" runat="server" EmptyDataText="No Record Found...." AutoGenerateColumns="False" AllowPaging="true" PageSize="20"
    CssClass="table table-hover table-bordered" Width="100%" Font-Size="12" DataSourceID="SqlDataSourceFeedback">
    <Columns>
        <asp:BoundField DataField="FeedbackID" HeaderText="Feedback ID" />
        <asp:BoundField DataField="UserName" HeaderText="User Name" />
        <asp:BoundField DataField="Email" HeaderText="Email" />
        <asp:BoundField DataField="ContactNo" HeaderText="Contact No" />
        <asp:BoundField DataField="Rating" HeaderText="Rating" />
        <asp:BoundField DataField="FeedbackText" HeaderText="Feedback Text" />
        <asp:BoundField DataField="ComplaintOrSuggestion" HeaderText="Complaint/Suggestion" />
        <asp:BoundField DataField="FeedbackDate" HeaderText="Feedback Date" DataFormatString="{0:yyyy-MM-dd HH:mm:ss}" />
       
    </Columns>
</asp:GridView></div>

    </div>
</asp:Content>
