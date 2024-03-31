Imports DotSpatial.Controls
Imports DotSpatial.Data
Imports DotSpatial.Symbology
Imports DotSpatial.Topology

Public Class FormUtama

    Public AppPath As String = Application.ExecutablePath
    Public ResourcesPath As String = AppPath.ToUpper.Replace("\SISTEM INFORMASI SPASIAL.EXE", "\Resources")
    Public lyrJalan As MapLineLayer
    Public Pendidikan As MapPointLayer
    Public lyrAdmin As MapPolygonLayer
    Public iselect(,) As String
    Public iselectnumd As Integer = 0
    Public totalselected As Integer
    Public selectnext As String = "salah"
    Public fullextentclick As String = "salah"
    Public sedangload As Boolean = False
    Public pointLayerTemplate As MapPointLayer
    Public pointFeatureTemplate As New FeatureSet(FeatureType.Point)
    Private lyrPendidikan As Object

    Private Sub FormUtama_Load(sender As Object, e As EventArgs) Handles MyBase.Load

        sedangload = True

        'PENAMBAHAN LAYER BATAS ADMMIN (AREA)
        lyrAdmin = Map1.Layers.Add(ResourcesPath & "\Spatial\ADMINISTRASIDESA_AR_25K.shp")
        lyrAdmin.LegendText = "Batas Administrasi"
        lyrAdmin.FeatureSet.AddFid()
        lyrAdmin.FeatureSet.Save()
        lyrAdmin.SelectionEnabled = False

        'Dim symbolAdmin As New PolygonSymbolizier(Color.FromArgb(255, Color.White), Color.Transparent, 0.5
        'lyrAdmin.Symbolizer=symbolAdmin

        'SIMBOLOGI LAYER BATAS ADMIN (AREA)
        Dim schemeAdmin As New PolygonScheme
        schemeAdmin.EditorSettings.ClassificationType = ClassificationType.UniqueValues
        schemeAdmin.EditorSettings.UseGradient = False
        schemeAdmin.EditorSettings.FieldName = "NAMOBJ"
        schemeAdmin.CreateCategories(lyrAdmin.DataSet.DataTable)

        For Each ifc As IFeatureCategory In schemeAdmin.GetCategories
            ifc.SetColor(Color.FromArgb(255, ifc.GetColor))
        Next

        lyrAdmin.Symbology = schemeAdmin

        'PENAMBAHAN LAYER JARINGAN JALAN (GARIS)
        lyrJalan = Map1.Layers.Add(ResourcesPath & "\Spatial\JALAN_LN_25K.shp")
        lyrJalan.LegendText = "Jaringan Jalan"
        lyrJalan.FeatureSet.AddFid()
        lyrJalan.FeatureSet.Save()
        lyrJalan.SelectionEnabled = False

        Dim schemeJalan As New LineScheme
        schemeJalan.ClearCategories()

        'SIMBOLOGI LAYER JARINGAN JALAN (GARIS)
        Dim symbolizerJalanArteri As New LineSymbolizer(Color.FromArgb(150, 0, 0), Color.Gray, 3, Drawing2D.DashStyle.Solid, Drawing2D.LineCap.Flat)
        symbolizerJalanArteri.ScaleMode = ScaleMode.Simple
        Dim categoryJalanArteri As New LineCategory(symbolizerJalanArteri)
        categoryJalanArteri.FilterExpression = "[REMARK] = 'Jalan Arteri'"
        categoryJalanArteri.LegendText = "Jalan Arteri"
        schemeJalan.AddCategory(categoryJalanArteri)

        Dim symbolizerJalanKolektor As New LineSymbolizer(Color.FromArgb(230, 0, 0), Color.Transparent, 2.5, Drawing2D.DashStyle.Solid, Drawing2D.LineCap.Flat)
        symbolizerJalanKolektor.ScaleMode = ScaleMode.Simple
        Dim categoryJalanKolektor As New LineCategory(symbolizerJalanKolektor)
        categoryJalanKolektor.FilterExpression = "[REMARK] = 'Jalan Kolektor'"
        categoryJalanKolektor.LegendText = "Jalan Kolektor"
        schemeJalan.AddCategory(categoryJalanKolektor)

        Dim symbolizerJalanLokal As New LineSymbolizer(Color.FromArgb(245, 53, 53), Color.Transparent, 2, Drawing2D.DashStyle.Solid, Drawing2D.LineCap.Flat)
        symbolizerJalanLokal.ScaleMode = ScaleMode.Simple
        Dim categoryJalanLokal As New LineCategory(symbolizerJalanLokal)
        categoryJalanLokal.FilterExpression = "[REMARK] = 'Jalan Lokal'"
        categoryJalanLokal.LegendText = "Jalan Lokal"
        schemeJalan.AddCategory(categoryJalanLokal)

        Dim symbolizerJalanLain As New LineSymbolizer(Color.FromArgb(210, 105, 0), Color.Transparent, 1.5, Drawing2D.DashStyle.Solid, Drawing2D.LineCap.Flat)
        symbolizerJalanLain.ScaleMode = ScaleMode.Simple
        Dim categoryJalanLain As New LineCategory(symbolizerJalanLain)
        categoryJalanLain.FilterExpression = "[REMARK] = 'Jalan Lain'"
        categoryJalanLain.LegendText = "Jalan Lain"
        schemeJalan.AddCategory(categoryJalanLain)

        Dim symbolizerJalanSetapak As New LineSymbolizer(Color.FromArgb(235, 120, 0), Color.Transparent, 1, Drawing2D.DashStyle.Solid, Drawing2D.LineCap.Flat)
        symbolizerJalanSetapak.ScaleMode = ScaleMode.Simple
        Dim categoryJalanSetapak As New LineCategory(symbolizerJalanSetapak)
        categoryJalanSetapak.FilterExpression = "[REMARK] = 'Jalan Setapak'"
        categoryJalanSetapak.LegendText = "Jalan Setapak"
        schemeJalan.AddCategory(categoryJalanSetapak)

        For Each ifc As IFeatureCategory In schemeJalan.GetCategories
            ifc.SetColor(Color.FromArgb(255, ifc.GetColor))
        Next

        lyrJalan.Symbology = schemeJalan

        'PENAMBAHAN LAYER PENDIDIKAN (TITIK)
        lyrPendidikan = Map1.Layers.Add(ResourcesPath & "\Spatial\GABUNGAN.shp")
        lyrPendidikan.LegendText = "Pendidikan"
        lyrPendidikan.FeatureSet.AddFid()
        lyrPendidikan.FeatureSet.Save()

        Dim schemePendidikan As New PointScheme
        schemePendidikan.ClearCategories()

        'Dim symbolizersd As New PointSymbolizer (Color.White, DotSpatial.Symbology.PointShape.Hexagon, 10)
        Dim sd As Image = Image.FromFile(ResourcesPath & "\NonSpatial\Icon\school1.png", False)
        Dim symbolizersd As New PointSymbolizer(sd, 20)
        symbolizersd.ScaleMode = ScaleMode.Simple
        Dim categorysd As New PointCategory(symbolizersd)
        categorysd.FilterExpression = "[bentuk_p_1] = 'SD'"
        categorysd.LegendText = "sd"
        schemePendidikan.AddCategory(categorysd)

        'Dim symbolizersmp As New PointSymbolizer (Color.White, DotSpatial.Symbology.PointShape.Hexagon, 10)
        Dim smp As Image = Image.FromFile(ResourcesPath & "\NonSpatial\Icon\school2.png", False)
        Dim symbolizersmp As New PointSymbolizer(smp, 20)
        symbolizersmp.ScaleMode = ScaleMode.Simple
        Dim categorysmp As New PointCategory(symbolizersmp)
        categorysmp.FilterExpression = "[bentuk_p_1] = 'SMP'"
        categorysmp.LegendText = "smp"
        schemePendidikan.AddCategory(categorysmp)

        'Dim symbolizersma As New PointSymbolizer (Color.White, DotSpatial.Symbology.PointShape.Hexagon, 10)
        Dim sma As Image = Image.FromFile(ResourcesPath & "\NonSpatial\Icon\school3.png", False)
        Dim symbolizersma As New PointSymbolizer(sma, 20)
        symbolizersma.ScaleMode = ScaleMode.Simple
        Dim categorysma As New PointCategory(symbolizersma)
        categorysma.FilterExpression = "[bentuk_p_1] = 'SMA'"
        categorysma.LegendText = "sma"
        schemePendidikan.AddCategory(categorysma)

        lyrPendidikan.Symbology = schemePendidikan

        'ADD LAYER TEMPLATE
        pointLayerTemplate = Map1.Layers.Add(pointFeatureTemplate)
        Dim pointttsymbol As New PointSymbolizer(Color.FromArgb(175, 75, 230, 0), DotSpatial.Symbology.PointShape.Diamond, 12)
        pointLayerTemplate.Symbolizer = pointttsymbol
        pointLayerTemplate.LegendText = “point template”
        pointLayerTemplate.LegendItemVisible = False

        'LOAD ATTRIBUTE
        Dim dt As DataTable
        dt = lyrPendidikan.DataSet.DataTable
        DataGridView1.DataSource = dt

        'LOAD DATA QUERY
        lyrAdmin.SelectAll()

        Dim ls1 As List(Of IFeature) = New List(Of IFeature)
        Dim il1 As ISelection = lyrAdmin.Selection

        ls1 = il1.ToFeatureList

        KryptonRibbonGroupComboBoxQueryKecamatan.Items.Clear()
        Dim i As Integer = 0
        Do While (i < il1.Count)
            Dim Name As String = (ls1(i).DataRow.ItemArray.GetValue(4).ToString)
            KryptonRibbonGroupComboBoxQueryKecamatan.Items.Insert(i, Name)
            i = (i + 1)
        Loop

        KryptonRibbonGroupComboBoxQueryKecamatan.Sorted = True
        Dim cboNumber As Integer = KryptonRibbonGroupComboBoxQueryKecamatan.Items.Count - 1
        Try
            For j = 1 To cboNumber
                If j > (KryptonRibbonGroupComboBoxQueryKecamatan.Items.Count - 1) Then Exit For
                If KryptonRibbonGroupComboBoxQueryKecamatan.Items(j) = KryptonRibbonGroupComboBoxQueryKecamatan.Items(j - i) Then
                    KryptonRibbonGroupComboBoxQueryKecamatan.Items.RemoveAt(j)
                    j = j - 1
                    cboNumber = cboNumber - 1
                End If
            Next
        Catch ex As Exception
        End Try

        KryptonRibbonGroupComboBoxQueryKecamatan.Sorted = True

        lyrAdmin.UnSelectAll()

        sedangload = False
    End Sub

    Friend Shared Function Pendidikan1() As Object
        Throw New NotImplementedException()
    End Function

    Private Sub Map1_SelectionChanged(sender As Object, e As EventArgs) Handles Map1.SelectionChanged
        Try
            If sedangload = True Then Exit Sub
            If KryptonRibbonGroupButton_Identify.Checked = True Then
                If lyrPendidikan.Selection.Count = 0 Then
                    'FormPopUp.Map1.ClearLayers()
                    Call RemoveSelection()
                    Exit Sub
                Else
                    FormPopUp.Show()
                    Call ShowPhoto()
                    FormPopUp.BringToFront()
                    FormPopUp.Activate()
                End If
            End If
        Catch ex As Exception
            MsgBox(ex.ToString)
        End Try
    End Sub

    Private Sub Map1_MouseMove(sender As Object, e As MouseEventArgs) Handles Map1.MouseMove
        Try
            Dim coord As Coordinate = Map1.PixelToProj(e.Location)
            lblXY.Text = String.Format(“X: {0} Y: {1}", coord.X, coord.Y)
        Catch ex As Exception
        End Try
    End Sub

    Private Sub Map1_MouseUp(sender As Object, e As MouseEventArgs) Handles Map1.MouseUp
        If KryptonRibbonGroupButton_AddPoint.Checked = True And Map1.Cursor = Cursors.Cross Then
            If FormAddPoint.rdoTitik_Cursor.Checked = True Then
                If e.Button = MouseButtons.Left Then
                    sedangload = True
                    pointLayerTemplate.SelectAll()
                    pointLayerTemplate.RemoveSelectedFeatures()
                    Dim coord As Coordinate = Map1.PixelToProj(e.Location)
                    Dim point As New Point(coord)
                    pointFeatureTemplate.AddFeature(point)
                    FormAddPoint.txtTitikX.Text = coord.X
                    FormAddPoint.txtTitikY.Text = coord.Y
                    sedangload = False
                End If
                pointFeatureTemplate.InitializeVertices()
                pointLayerTemplate.DataSet.InitializeVertices()
                pointLayerTemplate.AssignFastDrawnStates()
                pointFeatureTemplate.UpdateExtent()
                pointLayerTemplate.DataSet.UpdateExtent()
                Map1.Refresh()
                Map1.ResetBuffer()
            End If
        End If
    End Sub
    '---------------------------------------------------------------------------------------------------------------------------------
    'Mode Kursor
    '---------------------------------------------------------------------------------------------------------------------------------

    Private Sub KryptonRibbonGroupButton_NormalMode_Click(sender As Object, e As EventArgs) Handles KryptonRibbonGroupButton_NormalMode.Click
        If KryptonRibbonGroupButton_NormalMode.Checked = True Then
            Map1.FunctionMode = DotSpatial.Controls.FunctionMode.None
            'KryptonRibbonGroupButton_NormalMode.Checked = False
            KryptonRibbonGroupButton_ZoomInMode.Checked = False
            KryptonRibbonGroupButton_ZoomOutMode.Checked = False
            KryptonRibbonGroupButton_PanMode.Checked = False
            KryptonRibbonGroupButton_AddPoint.Checked = False
            KryptonRibbonGroupButton_MeasureLength.Checked = False
            KryptonRibbonGroupButton_MeasureArea.Checked = False
            KryptonRibbonGroupButton_Identify.Checked = False
        Else
            KryptonRibbonGroupButton_NormalMode.Checked = True
            Map1.FunctionMode = DotSpatial.Controls.FunctionMode.None
        End If
    End Sub

    Private Sub KryptonRibbonGroupButton_ZoomInMode_Click(sender As Object, e As EventArgs) Handles KryptonRibbonGroupButton_ZoomInMode.Click
        If KryptonRibbonGroupButton_ZoomInMode.Checked = True Then
            Map1.FunctionMode = DotSpatial.Controls.FunctionMode.ZoomIn
            KryptonRibbonGroupButton_NormalMode.Checked = False
            'KryptonRibbonGroupButton_ZoomInMode.Checked = False
            KryptonRibbonGroupButton_ZoomOutMode.Checked = False
            KryptonRibbonGroupButton_PanMode.Checked = False
            KryptonRibbonGroupButton_MeasureLength.Checked = False
            KryptonRibbonGroupButton_MeasureArea.Checked = False
            KryptonRibbonGroupButton_AddPoint.Checked = False
            KryptonRibbonGroupButton_Identify.Checked = False
        Else
            KryptonRibbonGroupButton_NormalMode.Checked = True
            Map1.FunctionMode = DotSpatial.Controls.FunctionMode.None
        End If
    End Sub

    Private Sub KryptonRibbonGroupButton_ZoomOutMode_Click(sender As Object, e As EventArgs) Handles KryptonRibbonGroupButton_ZoomOutMode.Click
        If KryptonRibbonGroupButton_ZoomOutMode.Checked = True Then
            Map1.FunctionMode = DotSpatial.Controls.FunctionMode.ZoomOut
            KryptonRibbonGroupButton_NormalMode.Checked = False
            KryptonRibbonGroupButton_ZoomInMode.Checked = False
            'KryptonRibbonGroupButton_ZoomOutMode.Checked = False
            KryptonRibbonGroupButton_PanMode.Checked = False
            KryptonRibbonGroupButton_MeasureLength.Checked = False
            KryptonRibbonGroupButton_MeasureArea.Checked = False
            KryptonRibbonGroupButton_AddPoint.Checked = False
            KryptonRibbonGroupButton_Identify.Checked = False
        Else
            KryptonRibbonGroupButton_NormalMode.Checked = True
            Map1.FunctionMode = DotSpatial.Controls.FunctionMode.None
        End If
    End Sub

    Private Sub KryptonRibbonGroupButton_PanMode_Click(sender As Object, e As EventArgs) Handles KryptonRibbonGroupButton_PanMode.Click
        If KryptonRibbonGroupButton_PanMode.Checked = True Then
            Map1.FunctionMode = DotSpatial.Controls.FunctionMode.Pan
            KryptonRibbonGroupButton_NormalMode.Checked = False
            KryptonRibbonGroupButton_ZoomInMode.Checked = False
            KryptonRibbonGroupButton_ZoomOutMode.Checked = False
            'KryptonRibbonGroupButton_PanMode.Checked = False
            KryptonRibbonGroupButton_MeasureLength.Checked = False
            KryptonRibbonGroupButton_MeasureArea.Checked = False
            KryptonRibbonGroupButton_AddPoint.Checked = False
            KryptonRibbonGroupButton_Identify.Checked = False
        Else
            KryptonRibbonGroupButton_NormalMode.Checked = True
            Map1.FunctionMode = DotSpatial.Controls.FunctionMode.None
        End If
    End Sub

    Private Sub KryptonRibbonGroupButton_Identify_Click(sender As Object, e As EventArgs) Handles KryptonRibbonGroupButton_Identify.Click
        KryptonRibbonGroupButton_Identify.Checked = True
        Map1.Cursor = Cursors.Cross
        FormPopUp.Show()
        FormPopUp.BringToFront()
        FormPopUp.Activate()
        KryptonRibbonGroupButton_NormalMode.Checked = False
        KryptonRibbonGroupButton_ZoomInMode.Checked = False
        KryptonRibbonGroupButton_ZoomOutMode.Checked = False
        KryptonRibbonGroupButton_PanMode.Checked = False
        KryptonRibbonGroupButton_MeasureLength.Checked = False
        KryptonRibbonGroupButton_MeasureArea.Checked = False
        KryptonRibbonGroupButton_AddPoint.Checked = False
        'KryptonRibbonGroupButton_Identify.Checked = False

        KryptonRibbonGroupButton_NormalMode.Checked = True
        Map1.FunctionMode = DotSpatial.Controls.FunctionMode.None

    End Sub

    Private Sub KryptonRibbonGroupButton_AddPoint_Click(sender As Object, e As EventArgs) Handles KryptonRibbonGroupButton_AddPoint.Click
        KryptonRibbonGroupButton_AddPoint.Checked = True
        Map1.Cursor = Cursors.Cross
        FormAddPoint.Show()
        FormAddPoint.BringToFront()
        FormAddPoint.Activate()
        KryptonRibbonGroupButton_NormalMode.Checked = False
        KryptonRibbonGroupButton_ZoomInMode.Checked = False
        KryptonRibbonGroupButton_ZoomOutMode.Checked = False
        KryptonRibbonGroupButton_PanMode.Checked = False
        KryptonRibbonGroupButton_MeasureLength.Checked = False
        KryptonRibbonGroupButton_MeasureArea.Checked = False
        'KryptonRibbonGroupButton_AddPoint.Checked = False
        KryptonRibbonGroupButton_Identify.Checked = False

        KryptonRibbonGroupButton_NormalMode.Checked = True
        Map1.FunctionMode = DotSpatial.Controls.FunctionMode.None

    End Sub


    '---------------------------------------------------------------------------------------------------------------------------------
    'Zooming
    '---------------------------------------------------------------------------------------------------------------------------------
    Private Sub KryptonRibbonGroupButton_ZoomIn_Click(sender As Object, e As EventArgs) Handles KryptonRibbonGroupButton_ZoomIn.Click
        Map1.ZoomIn()
    End Sub

    Private Sub KryptonRibbonGroupButton_ZoomOut_Click(sender As Object, e As EventArgs) Handles KryptonRibbonGroupButton_ZoomOut.Click
        Map1.ZoomOut()
    End Sub

    Private Sub KryptonRibbonGroupButton_FullExtent_Click(sender As Object, e As EventArgs) Handles KryptonRibbonGroupButton_FullExtent.Click
        Map1.ZoomToMaxExtent()
    End Sub

    Private Sub KryptonRibbonGroupButton_ZoomToPrevious_Click(sender As Object, e As EventArgs) Handles KryptonRibbonGroupButton_ZoomToPrevious.Click
        Map1.ZoomToPrevious()
    End Sub

    Private Sub KryptonRibbonGroupButton_ZoomToNext_Click(sender As Object, e As EventArgs) Handles KryptonRibbonGroupButton_ZoomToNext.Click
        Map1.ZoomToNext()
    End Sub

    '---------------------------------------------------------------------------------------------------------------------------------
    'Query
    '---------------------------------------------------------------------------------------------------------------------------------

    Private Sub KryptonRibbonGroupComboBoxQueryKecamatan_SelectedIndexChanged(sender As Object, e As EventArgs) Handles KryptonRibbonGroupComboBoxQueryKecamatan.SelectedIndexChanged
        If KryptonRibbonGroupComboBoxQueryKecamatan.Text = "Cari Kecamatan..." Then Exit Sub

        sedangload = True
        Dim StrKecamatan As String = KryptonRibbonGroupComboBoxQueryKecamatan.Text
        lyrAdmin.SelectByAttribute("[NAMOBJ]='" & StrKecamatan & "'")
        lyrAdmin.ZoomToSelectedFeatures(0.01, 0.01)
        Map1.Refresh()

        Dim ls1 As List(Of IFeature) = New List(Of IFeature)
        Dim il1 As ISelection = lyrAdmin.Selection

        ls1 = il1.ToFeatureList

        KryptonRibbonGroupComboBoxQueryDesa.Items.Clear()
        Dim i As Integer = 0
        Do While (i < il1.Count)
            Dim Name As String = (ls1(i).DataRow.ItemArray.GetValue(8).ToString)
            KryptonRibbonGroupComboBoxQueryDesa.Items.Insert(i, Name)
            i = (i + 1)
        Loop

        KryptonRibbonGroupComboBoxQueryDesa.Sorted = True
        Dim cboNumber As Integer = KryptonRibbonGroupComboBoxQueryDesa.Items.Count - 1
        Try
            For j = 1 To cboNumber
                If j > (KryptonRibbonGroupComboBoxQueryDesa.Items.Count - 1) Then Exit For
                If KryptonRibbonGroupComboBoxQueryDesa.Items(j) = KryptonRibbonGroupComboBoxQueryDesa.Items(j - 1) Then
                    KryptonRibbonGroupComboBoxQueryDesa.Items.RemoveAt(j)
                    j = j - 1
                    cboNumber = cboNumber - 1
                End If
            Next
        Catch ex As Exception

        End Try
        KryptonRibbonGroupComboBoxQueryDesa.Sorted = True

        'lyrAdmin

        sedangload = False
    End Sub

    Private Sub KryptonRibbonGroupComboBoxQueryDesa_SelectedIndexChanged(sender As Object, e As EventArgs) Handles KryptonRibbonGroupComboBoxQueryDesa.SelectedIndexChanged
        If KryptonRibbonGroupComboBoxQueryKecamatan.Text = "Cari Kecamatan..." Then Exit Sub
        If KryptonRibbonGroupComboBoxQueryDesa.Text = "Cari Desa..." Then Exit Sub

        sedangload = True
        Dim StrKecamatan As String = KryptonRibbonGroupComboBoxQueryKecamatan.Text
        Dim StrDesa As String = KryptonRibbonGroupComboBoxQueryDesa.Text
        lyrAdmin.SelectByAttribute("[WADMKC]='" & StrKecamatan & "' AND [NAMOBJ]='" & StrDesa & "'")
        lyrAdmin.ZoomToSelectedFeatures(0.01, 0.01)
        Map1.Refresh()

        lyrPendidikan.SelectByAttribute("[WADMKC]='" & StrKecamatan & "' AND [NAMOBJ]='" & StrDesa & "'")
        Dim ls1 As List(Of IFeature) = New List(Of IFeature)
        Dim il1 As ISelection = lyrAdmin.Selection

        ls1 = il1.ToFeatureList

        KryptonRibbonGroupComboBoxQueryFasilitas.Items.Clear()
        Dim i As Integer = 0
        Do While (i < il1.Count)
            Dim Name As String = (ls1(i).DataRow.ItemArray.GetValue(4).ToString)
            KryptonRibbonGroupComboBoxQueryFasilitas.Items.Insert(i, Name)
            i = (i + 1)
        Loop
        KryptonRibbonGroupComboBoxQueryFasilitas.Sorted = True
        Dim cboNumber As Integer = KryptonRibbonGroupComboBoxQueryFasilitas.Items.Count - 1
        Try
            For j = 1 To cboNumber
                If j > (KryptonRibbonGroupComboBoxQueryFasilitas.Items.Count - 1) Then Exit For
                If KryptonRibbonGroupComboBoxQueryFasilitas.Items(j) = KryptonRibbonGroupComboBoxQueryFasilitas.Items(j - 1) Then
                    KryptonRibbonGroupComboBoxQueryFasilitas.Items.RemoveAt(j)
                    j = j - 1
                    cboNumber = cboNumber - 1
                End If
            Next
        Catch ex As Exception

        End Try
        KryptonRibbonGroupComboBoxQueryFasilitas.Sorted = True

        'lyrAdmin.UnselectAll()
        sedangload = False
    End Sub

    Private Sub KryptonRibbonGroupComboBoxQueryFasilitas_SelectedIndexChanged(sender As Object, e As EventArgs) Handles KryptonRibbonGroupComboBoxQueryFasilitas.SelectedIndexChanged
        If KryptonRibbonGroupComboBoxQueryKecamatan.Text = "Cari Kecamatan..." Then Exit Sub
        If KryptonRibbonGroupComboBoxQueryDesa.Text = "Cari Desa..." Then Exit Sub
        If KryptonRibbonGroupComboBoxQueryFasilitas.Text = "Cari Sarana..." Then Exit Sub

        sedangload = True

        Dim StrKecamatan As String = KryptonRibbonGroupComboBoxQueryKecamatan.Text
        Dim StrDesa As String = KryptonRibbonGroupComboBoxQueryDesa.Text
        Dim StrPen As String = KryptonRibbonGroupComboBoxQueryFasilitas.Text
        lyrPendidikan.SelectByAttribute("[WADMKC]='" & StrKecamatan & "' AND [NAMOBJ]='" & StrDesa & "' AND [JENIS] = '" & StrPen & "'")
        lyrPendidikan.ZoomToSelectedFeatures(0.01, 0.01)
        Map1.Refresh()

        lyrAdmin.UnSelectAll()

        sedangload = False
    End Sub

    '---------------------------------------------------------------------------------------------------------------------------------
    'Attribute
    '---------------------------------------------------------------------------------------------------------------------------------

    Private Sub DataGridView1_RowHeaderMouseDoubleClick(sender As Object, e As DataGridViewCellMouseEventArgs) Handles DataGridView1.RowHeaderMouseDoubleClick
        sedangload = True
        If DataGridView1.SelectedRows.Count = 0 Then Exit Sub
        Map1.ClearSelection()
        lyrPendidikan.Select(CInt(DataGridView1.SelectedRows.Item(0).Cells.Item("FID").Value))
        lyrPendidikan.ZoomToSelectedFeatures(0.01, 0.01)
        Map1.Refresh()
        sedangload = False
    End Sub

    Private Sub DataGridView1_SelectionChanged(sender As Object, e As EventArgs) Handles DataGridView1.SelectionChanged
        sedangload = True
        If DataGridView1.SelectedRows.Count = 0 Then Exit Sub
        Map1.ClearSelection()
        For i = 0 To DataGridView1.SelectedRows.Count - 1
            'lyrAset.SelectByAttribute("[FID]=" & DataGridView1.SelectedRows.Item(i).Cells.Item("FID").Value)
            lyrPendidikan.Select(CInt(DataGridView1.SelectedRows.Item(i).Cells.Item("FID").Value))
        Next
        lyrPendidikan.ZoomToSelectedFeatures(0.01, 0.01)
        Map1.Refresh()
        sedangload = True
    End Sub

    '---------------------------------------------------------------------------------------------------------------------------------
    'Pop Up
    '---------------------------------------------------------------------------------------------------------------------------------
    Public Sub ShowPhoto()
        Try
            Dim ls1 As List(Of IFeature) = New List(Of IFeature)
            Dim il1 As ISelection = lyrPendidikan.Selection

            Dim dt As DataTable
            dt = lyrPendidikan.DataSet.DataTable

            Dim SaranaPendidikan As String = “”
            Dim NamaSekolah As String = “"
            Dim Foto As String = “”
            Dim shapeIndex As Integer = 0

            ls1 = il1.ToFeatureList

            SaranaPendidikan = (ls1(0).DataRow.ItemArray.GetValue(2).ToString)
            NamaSekolah = (ls1(0).DataRow.ItemArray.GetValue(3).ToString)
            Foto = (ls1(0).DataRow.ItemArray.GetValue(5).ToString)
            shapeIndex = (ls1(0).DataRow.ItemArray.GetValue(dt.Columns.Count - 1))

            FormPopUp.txtSaranaPendidikan.Text = SaranaPendidikan
            FormPopUp.txtNama.Text = NamaSekolah
            FormPopUp.txtFoto.Text = Foto
            FormPopUp.txtShapeIndex.Text = shapeIndex

            Dim alamatfoto As String = ResourcesPath & “\Database\ Non-Spasial\ Foto” & Foto
            FormPopUp.Map1.AddLayer(alamatfoto)
            If SaranaPendidikan = “” Then
                Call RemoveSelection()
                Exit Sub
            End If

            Map1.Refresh()
            Me.Refresh()
            FormPopUp.Refresh()
        Catch ex As Exception
            MsgBox(ex.ToString)
        End Try
    End Sub

    Public Sub RemoveSelection()
        Try
            sedangload = True

            FormPopUp.txtSaranaPendidikan.Text = “”
            FormPopUp.txtNama.Text = “”
            FormPopUp.txtFoto.Text = “.....”
            FormPopUp.txtShapeIndex.Text = “”

            lyrAdmin.UnSelectAll()
            lyrPendidikan.UnSelectAll()

            FormPopUp.Map1.ClearLayers()

            Me.Refresh()
            FormPopUp.Refresh()

            sedangload = False
        Catch ex As Exception
            MsgBox(ex.ToString)
        End Try
    End Sub

    Private Sub KryptonRibbon1_SelectedTabChanged(sender As Object, e As EventArgs) Handles KryptonRibbon1.SelectedTabChanged

    End Sub

    Private Sub KryptonRibbonGroupButton_MeasureLength_Click(sender As Object, e As EventArgs) Handles KryptonRibbonGroupButton_MeasureLength.Click
        If KryptonRibbonGroupButton_MeasureLength.Checked = True Then
            Map1.FunctionMode = DotSpatial.Controls.FunctionMode.Select
            KryptonRibbonGroupButton_NormalMode.Checked = False
            KryptonRibbonGroupButton_ZoomInMode.Checked = False
            KryptonRibbonGroupButton_ZoomOutMode.Checked = False
            KryptonRibbonGroupButton_PanMode.Checked = False
            'KryptonRibbonGroupButton_MeasureLength.Checked = False
            KryptonRibbonGroupButton_MeasureArea.Checked = False
            KryptonRibbonGroupButton_AddPoint.Checked = False
            KryptonRibbonGroupButton_Identify.Checked = False
        Else
            KryptonRibbonGroupButton_NormalMode.Checked = True
            Map1.FunctionMode = DotSpatial.Controls.FunctionMode.None
        End If
    End Sub

    Private Sub KryptonRibbonGroupButton_MeasureArea_Click(sender As Object, e As EventArgs) Handles KryptonRibbonGroupButton_MeasureArea.Click
        If KryptonRibbonGroupButton_MeasureArea.Checked = True Then
            Map1.FunctionMode = DotSpatial.Controls.FunctionMode.Select
            KryptonRibbonGroupButton_NormalMode.Checked = False
            KryptonRibbonGroupButton_ZoomInMode.Checked = False
            KryptonRibbonGroupButton_ZoomOutMode.Checked = False
            KryptonRibbonGroupButton_PanMode.Checked = False
            KryptonRibbonGroupButton_MeasureLength.Checked = False
            'KryptonRibbonGroupButton_MeasureArea.Checked = False
            KryptonRibbonGroupButton_AddPoint.Checked = False
            KryptonRibbonGroupButton_Identify.Checked = False
        Else
            KryptonRibbonGroupButton_NormalMode.Checked = True
            Map1.FunctionMode = DotSpatial.Controls.FunctionMode.None
        End If
    End Sub


    Private Sub KryptonRibbonGroupButton_About_Click(sender As Object, e As EventArgs) Handles KryptonRibbonGroupButton_About.Click

    End Sub
End Class
