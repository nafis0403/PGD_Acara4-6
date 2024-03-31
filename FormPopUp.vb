Imports DotSpatial.Controls
Imports DotSpatial.Data
Imports DotSpatial.Symbology
Imports System.IO

Public Class FormPopUp
    Public AppPath As String = Application.ExecutablePath
    Public ResourcesPath As String = AppPath.ToUpper.Replace("\SISTEM INFORMASI SPASIAL.EXE", "\Resources")
    Private lyrAdmin As MapPolygonLayer

    Private Sub cmdEdit_Click(sender As Object, e As EventArgs) Handles cmdEdit.Click
        Try

            FormUtama.sedangload = True

            If cmdEdit.Text = “Edit” Then

                Dim input As String = Microsoft.VisualBasic.Interaction.InputBox(
                        “Please enter your password...”, “Password”, “”, -1, -1)

                If input = “HALO” Then
                    txtSaranaPendidikan.ReadOnly = False
                    cmdBrowse.Enabled = True
                    cmdDelete.Visible = True
                    cmdEdit.Text = “Save”
                Else
                    txtSaranaPendidikan.ReadOnly = True
                    cmdBrowse.Enabled = False
                    cmdDelete.Visible = False
                    cmdEdit.Text = “Edit”
                End If

            ElseIf cmdEdit.Text = “Save” Then
                Dim featureEdited As IFeature = FormUtama.Pendidikan.FeatureSet.GetFeature(CInt(txtShapeIndex.Text))
                featureEdited.DataRow.BeginEdit()
                featureEdited.DataRow(“Pendidikan”) = txtSaranaPendidikan.Text
                featureEdited.DataRow.EndEdit()

                cmdEdit.Text = “Edit”
                txtSaranaPendidikan.ReadOnly = True
                cmdBrowse.Enabled = False
                Map1.Refresh()
                Me.Hide()
                MessageBox.Show(“Data tersimpan baik”)
            End If

            FormUtama.sedangload = False

        Catch ex As Exception
            MsgBox(ex.ToString)
        End Try
    End Sub

    Private Sub cmdBrowse_Click(sender As Object, e As EventArgs) Handles cmdBrowse.Click
        Dim ofd As OpenFileDialog = New OpenFileDialog()
        ofd.InitialDirectory = "E:\"
        ofd.Title = "Browse Photo"
        ofd.Filter = "JPG (*.jpg)|*.jpg|JPEG (*.jpeg)|*.jpeg|PNG (*.png)|*.png|All files (*.*)|*.*"
        ofd.FilterIndex = 1
        ofd.RestoreDirectory = True

        If (ofd.ShowDialog() = DialogResult.OK) Then
            Dim fileName As String = Path.GetFileName(ofd.FileName)
            Dim sourcePath As String = Path.GetDirectoryName(ofd.FileName)
            Dim targetPath As String = Path.Combine(FormUtama.ResourcesPath, "Database/Non spasial/Foto")
            Dim sourceFile As String = Path.Combine(sourcePath, fileName)
            Dim destFile As String = Path.Combine(targetPath, fileName)
            File.Copy(sourceFile, destFile, True)
            txtFoto.Text = fileName
            Map1.ClearLayers()
            Map1.AddLayer(destFile)

        Else
            MessageBox.Show("MILIH FOTO DULU YAAA", "Report", MessageBoxButtons.OK)
        End If


    End Sub

    Private Sub cmdZoomIn_Click(sender As Object, e As EventArgs) Handles cmdZoomIn.Click
        Map1.FunctionMode = DotSpatial.Controls.FunctionMode.ZoomIn
    End Sub

    Private Sub cmdZoomOut_Click(sender As Object, e As EventArgs) Handles cmdZoomOut.Click
        Map1.FunctionMode = DotSpatial.Controls.FunctionMode.ZoomOut
    End Sub

    Private Sub cmdPan_Click(sender As Object, e As EventArgs) Handles cmdPan.Click
        Map1.FunctionMode = DotSpatial.Controls.FunctionMode.Pan
    End Sub

    Private Sub cmdFullExtent_Click(sender As Object, e As EventArgs) Handles cmdFullExtent.Click
        Map1.ZoomToMaxExtent()
    End Sub

    Private Sub cmdDelete_Click(sender As Object, e As EventArgs) Handles cmdDelete.Click
        FormUtama.sedangload = True
        FormUtama.Pendidikan.ClearSelection()
        FormUtama.Pendidikan.Select(CInt(txtShapeIndex.Text))
        FormUtama.Pendidikan.RemoveSelectedFeatures()
        FormUtama.sedangload = False
        FormUtama.Map1.Refresh()
        Me.Close()
        MessageBox.Show(“Data sudah terhapus!”)
    End Sub

    Private Sub cmdCancel_Click(sender As Object, e As EventArgs) Handles cmdCancel.Click

    End Sub

    Private Sub FormPopUp_Load(sender As Object, e As EventArgs) Handles MyBase.Load

    End Sub



    Private Sub Label3_Click(sender As Object, e As EventArgs) Handles Label3.Click

    End Sub
End Class