Imports DotSpatial.Controls
Imports DotSpatial.Data
Imports DotSpatial.Topology
Imports System.IO

Public Class FormAddPoint
    Private txtFoto As Object
    Public AppPath As String = Application.ExecutablePath
    Public ResourcesPath As String = AppPath.ToUpper.Replace("\SISTEM INFORMASI SPASIAL.EXE", "\Resources")
    Private lyrAdmin As MapPolygonLayer

    Private Sub cmdBrowse_Click(sender As Object, e As EventArgs) Handles cmdBrowse.Click
        Dim ofd As OpenFileDialog = New OpenFileDialog()
        ofd.Title = “Browse Photo”
        ofd.InitialDirectory = “C:\”
        ofd.Filter = “JPG (*.jpg)|*.jpg|JPEG (*.jpeg)|*.jpeg|PNG (*.png)|*.png|All files (*.*)|*.*”
        ofd.FilterIndex = 1
        ofd.RestoreDirectory = True

        If (ofd.ShowDialog() = DialogResult.OK) Then
            Dim fileName As String = Path.GetFileName(ofd.FileName)
            Dim sourcePath As String = Path.GetDirectoryName(ofd.FileName)
            Dim targetPath As String = Path.Combine(FormUtama.ResourcesPath, “Database/Non-Spasial/Foto”)
            Dim sourceFile As String = Path.Combine(sourcePath, fileName)
            Dim destFile As String = Path.Combine(targetPath, fileName)
            File.Copy(sourceFile, destFile, True)
            txtFoto.Text = fileName
            Map1.ClearLayers()
            Map1.AddLayer(destFile)
        Else
            MessageBox.Show(“YUK PILIH DULU FOTONY <3”, “Report”, MessageBoxButtons.OK)
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

    Private Sub cmdSave_Click(sender As Object, e As EventArgs) Handles cmdSave.Click
        Dim PendidikanFeatureSet As FeatureSet = FormUtama.Pendidikan.FeatureSet
        Dim PendidikanPoint As New Point(CDbl(txtTitikX.Text), CDbl(txtTitikY.Text))
        Dim featureInserted As IFeature = PendidikanFeatureSet.AddFeature(PendidikanPoint)
        featureInserted.DataRow.BeginEdit()
        featureInserted.DataRow(“Pendidikan”) = txtSaranaPendidikan.Text
        featureInserted.DataRow(“Gambar”) = txtFoto.Text
        featureInserted.DataRow.EndEdit()

        PendidikanFeatureSet.InitializeVertices()
        PendidikanFeatureSet.UpdateExtent()
        PendidikanFeatureSet.Save()

        FormUtama.Pendidikan.DataSet.InitializeVertices()
        FormUtama.Pendidikan.AssignFastDrawnStates()
        FormUtama.Pendidikan.DataSet.UpdateExtent()

        Dim dt As DataTable
        dt = FormUtama.Pendidikan.DataSet.DataTable
        dt.Columns.RemoveAt((dt.Columns.Count - 1))
        dt.AcceptChanges()
        FormUtama.Pendidikan.DataSet.Save()
        FormUtama.Pendidikan.FeatureSet.AddFid()
        FormUtama.Pendidikan.FeatureSet.Save()

        FormUtama.pointLayerTemplate.SelectAll()
        FormUtama.pointLayerTemplate.RemoveSelectedFeatures()

        FormUtama.pointFeatureTemplate.InitializeVertices()
        FormUtama.pointFeatureTemplate.UpdateExtent()
        FormUtama.pointLayerTemplate.DataSet.InitializeVertices()
        FormUtama.pointLayerTemplate.AssignFastDrawnStates()
        FormUtama.pointLayerTemplate.DataSet.UpdateExtent()

        Map1.Refresh()
        Map1.ResetBuffer()

        MessageBox.Show(“Data sudah tersimpan!”)
    End Sub

    Private Sub cmdCancel_Click(sender As Object, e As EventArgs) Handles cmdCancel.Click
        Me.Close()
    End Sub


    Private Sub FormAddPoint_Load(sender As Object, e As EventArgs) Handles MyBase.Load

    End Sub
End Class