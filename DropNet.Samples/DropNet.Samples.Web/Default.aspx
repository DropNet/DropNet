<%@ Page Title="Home Page" Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true"
    CodeBehind="Default.aspx.cs" Inherits="DropNet.Samples.Web._Default" %>

<asp:Content ID="HeaderContent" runat="server" ContentPlaceHolderID="HeadContent">
</asp:Content>
<asp:Content ID="BodyContent" runat="server" ContentPlaceHolderID="MainContent">
    <h2>
        Click the button to start.
    </h2>
    <asp:Literal ID="litOutput" runat="server"></asp:Literal>
    <asp:Button ID="btnStart" runat="server" Text="Start" OnClick="btnStart_Click" />
</asp:Content>
