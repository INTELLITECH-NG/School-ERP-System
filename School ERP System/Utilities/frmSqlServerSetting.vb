Imports System.Data.SqlClient
Imports System.Security.Cryptography
Imports System.IO
Imports System.Text
Imports System.Data.Sql
Imports System.Data
Imports Microsoft.SqlServer.Management.Smo
Public Class frmSqlServerSetting
   
    Dim SqlConnStr As String
    Private Sub btnSave_Click(sender As System.Object, e As System.EventArgs) Handles btnSave.Click
        Try
            If cmbServerName.Text.Length <= 2 Then
                MsgBox("Please select Server Name", MsgBoxStyle.Information)
                cmbServerName.Focus()
                Exit Sub
            End If
            If cmbDatabase.Text = "" Then
                MsgBox("Please select database", MsgBoxStyle.Information)
                cmbDatabase.Focus()
                Exit Sub
            End If
            If cmbAuthentication.SelectedIndex = 1 Then
                If txtUserName.Text.Length = 0 Then
                    MsgBox("please enter user name", MsgBoxStyle.Information)
                    txtUserName.Focus()
                    Exit Sub
                End If
                If txtPassword.Text.Length = 0 Then
                    MsgBox("please enter password", MsgBoxStyle.Information)
                    txtPassword.Focus()
                    Exit Sub
                End If
            End If
            Cursor = Cursors.WaitCursor
            Timer1.Enabled = True
            Dim SqlConn As New SqlConnection

            If cmbAuthentication.SelectedIndex = 0 Then
                SqlConnStr = "Data Source=" & cmbServerName.Text.Trim & ";Initial Catalog=" & cmbDatabase.Text.Trim & ";Integrated Security=True"
            End If
            If cmbAuthentication.SelectedIndex = 1 Then
                SqlConnStr = "Data Source=" & cmbServerName.Text.Trim & ";Initial Catalog=" & cmbDatabase.Text.Trim & ";User ID=" & txtUserName.Text.Trim & ";Password=" & txtPassword.Text & ""
            End If
            If SqlConn.State = ConnectionState.Closed Then
                SqlConn.ConnectionString = SqlConnStr
                Try
                    SqlConn.Open()
                Catch ex As Exception
                    MessageBox.Show("Invalid DB SqlConnnection" + vbCrLf + Err.Description, "DB Connection Test", MessageBoxButtons.OK, MessageBoxIcon.Error)
                    Reset()
                    Exit Sub
                End Try

            End If
            If MsgBox("It will configure the sql server, Do you want to save?", MsgBoxStyle.YesNo + MsgBoxStyle.Information) = MsgBoxResult.Yes Then
                Cursor = Cursors.WaitCursor
                Timer1.Enabled = True
                Using sw As StreamWriter = New StreamWriter(Application.StartupPath & "\SQLSettings.dat")
                    If cmbAuthentication.SelectedIndex = 0 Then
                        sw.WriteLine("Data Source=" & cmbServerName.Text.Trim & ";Initial Catalog=" & cmbDatabase.Text.Trim & ";Integrated Security=True")
                        sw.Close()
                    End If
                    If cmbAuthentication.SelectedIndex = 1 Then
                        sw.WriteLine("Data Source=" & cmbServerName.Text.Trim & ";Initial Catalog=" & cmbDatabase.Text.Trim & ";User ID=" & txtUserName.Text.Trim & ";Password=" & txtPassword.Text & "")
                        sw.Close()
                    End If
                End Using
            Else
                End
            End If
            MessageBox.Show("SQL Server setting has been saved successfully..." & vbCrLf & "Application will be closed,Please start it again", "", MessageBoxButtons.OK, MessageBoxIcon.Information)
            End
        Catch ex As Exception
            MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    Private Sub btnClose_Click(sender As System.Object, e As System.EventArgs) Handles btnClose.Click
        If lblSet.Text = "Main Form" Then
            Me.Close()
        Else
            If MsgBox("Do you want to close the application....", MsgBoxStyle.YesNo + MsgBoxStyle.Information) = MsgBoxResult.Yes Then
                End
            End If
        End If
    End Sub
    Sub fillDatabase()
        Try
            Cursor = Cursors.WaitCursor
            Timer1.Enabled = True
            Dim CN As New SqlConnection("Data source=" & cmbServerName.Text & ";Initial Catalog=master;Integrated Security=True;")
            CN.Open()
            adp = New SqlDataAdapter()
            adp.SelectCommand = New SqlCommand("Select name from sys.databases", CN)
            ds = New DataSet("ds")
            adp.Fill(ds)
            Dim dtable As DataTable = ds.Tables(0)
            cmbDatabase.Items.Clear()
            For Each drow As DataRow In dtable.Rows
                cmbDatabase.Items.Add(drow(0).ToString())
            Next
        Catch ex As Exception
            MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub
    Private Sub LinkLabel1_LinkClicked(sender As System.Object, e As System.Windows.Forms.LinkLabelLinkClickedEventArgs) Handles LinkLabel1.LinkClicked
        Try
            Cursor = Cursors.WaitCursor
            Timer1.Enabled = True
            Dim dataTable As DataTable = SmoApplication.EnumAvailableSqlServers(True)
            cmbServerName.ValueMember = "Name"
            cmbServerName.DataSource = dataTable
            Dim serverName As String = cmbServerName.SelectedValue.ToString()
            Dim server As New Server(serverName)
        Catch ex As Exception
            MessageBox.Show("Sorry unable to find SQL Server instance" & vbCrLf & "If you have installed SQL Server then enter name of SQL Server instance manually", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information)
        End Try
    End Sub

    Private Sub cmbAuthentication_SelectedIndexChanged(sender As System.Object, e As System.EventArgs) Handles cmbAuthentication.SelectedIndexChanged
        If cmbAuthentication.SelectedIndex = 0 Then
            txtUserName.ReadOnly = True
            txtPassword.ReadOnly = True
            txtUserName.Text = ""
            txtPassword.Text = ""
        End If
        If cmbAuthentication.SelectedIndex = 1 Then
            txtUserName.ReadOnly = False
            txtPassword.ReadOnly = False
        End If
    End Sub

    Private Sub Timer1_Tick(sender As System.Object, e As System.EventArgs) Handles Timer1.Tick
        Cursor = Cursors.Default
        Timer1.Enabled = False
    End Sub


    Private Sub cmbServerName_SelectedIndexChanged(sender As System.Object, e As System.EventArgs) Handles cmbServerName.SelectedIndexChanged
        cmbDatabase.Enabled = True
        cmbAuthentication.Enabled = True
    End Sub

    Private Sub LinkLabel2_LinkClicked(sender As System.Object, e As System.Windows.Forms.LinkLabelLinkClickedEventArgs) Handles LinkLabel2.LinkClicked
        If cmbServerName.Text = "" Then
            MessageBox.Show("Please select server name", "", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            cmbServerName.Focus()
            Exit Sub
        End If
        fillDatabase()
    End Sub
    Sub Reset()
        txtPassword.Text = ""
        txtUserName.Text = ""
        cmbServerName.Text = ""
        cmbAuthentication.SelectedIndex = 0
        cmbDatabase.SelectedIndex = -1
    End Sub
    Private Sub frmSqlServerSetting_Load(sender As System.Object, e As System.EventArgs) Handles MyBase.Load

    End Sub
 
    Private Sub btnTestConnection_Click(sender As System.Object, e As System.EventArgs) Handles btnTestConnection.Click
        If cmbServerName.Text.Length <= 2 Then
            MsgBox("Please select Server Name", MsgBoxStyle.Information)
            cmbServerName.Focus()
            Exit Sub
        End If
        If cmbDatabase.Text = "" Then
            MsgBox("Please select database", MsgBoxStyle.Information)
            cmbDatabase.Focus()
            Exit Sub
        End If
        If cmbAuthentication.SelectedIndex = 1 Then
            If txtUserName.Text.Length = 0 Then
                MsgBox("please enter user name", MsgBoxStyle.Information)
                txtUserName.Focus()
                Exit Sub
            End If
            If txtPassword.Text.Length = 0 Then
                MsgBox("please enter password", MsgBoxStyle.Information)
                txtPassword.Focus()
                Exit Sub
            End If
        End If
        Cursor = Cursors.WaitCursor
        Timer1.Enabled = True
        Dim SqlConn As New SqlConnection

        If cmbAuthentication.SelectedIndex = 0 Then
            SqlConnStr = "Data Source=" & cmbServerName.Text.Trim & ";Initial Catalog=" & cmbDatabase.Text.Trim & ";Integrated Security=True"
        End If
        If cmbAuthentication.SelectedIndex = 1 Then
            SqlConnStr = "Data Source=" & cmbServerName.Text.Trim & ";Initial Catalog=" & cmbDatabase.Text.Trim & ";User ID=" & txtUserName.Text.Trim & ";Password=" & txtPassword.Text & ""
        End If
        If SqlConn.State = ConnectionState.Closed Then
            SqlConn.ConnectionString = SqlConnStr
            Try
                SqlConn.Open()
                MessageBox.Show("Succsessfull DB Connnection", "DB Connection Test", MessageBoxButtons.OK, MessageBoxIcon.Information)
            Catch ex As Exception
                MessageBox.Show("Invalid DB SqlConnnection" + vbCrLf + Err.Description, "DB Connection Test", MessageBoxButtons.OK, MessageBoxIcon.Error)
            End Try
        End If
    End Sub
End Class
