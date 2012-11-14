Public Class Form1

    Private Sub Button1_Click(sender As System.Object, e As System.EventArgs) Handles Button1.Click
        Dim client As New DropNetClient("API", "SECRET")

        client.Account_InfoAsync(AddressOf AccountInfo_Success, AddressOf AccountInfo_Fail)
    End Sub

    Private Sub AccountInfo_Success(accountInfo As DropNet.Models.AccountInfo)
        'Do something with accountInfo
    End Sub

    Private Sub AccountInfo_Fail(ex As DropNet.Exceptions.DropboxException)
        'Failed to get AccountInfo
    End Sub

End Class
