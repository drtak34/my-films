Imports System.Threading
Imports System.Xml
Imports System.ComponentModel

Public Class AntProcessor

    Private Shared WithEvents bgwFolderScanUpdate As New System.ComponentModel.BackgroundWorker
    Private Shared WithEvents bgwManualUpdate As New System.ComponentModel.BackgroundWorker

    Private Shared ds As DataSet

    Public Shared XMLDoc As Xml.XmlDocument = New Xml.XmlDocument

    Private Shared _CountXMLRecords As Integer
    Private Shared _CountOrphanFiles As Integer = 0
    Private Shared _CountOrphanRecords As Integer = 0
    Private Shared _CountRecordsAdded As Integer = 0
    Private Shared _CountRecordsDeleted As Integer = 0
    Private Shared _CountMultiPartFiles As Integer = 0
    Private Shared _CountMultiPartFilesMerged As Integer = 0

    Private Shared _IsMultiFolderScan As Boolean
    Private Shared _InteractiveMode As Boolean = True

    Private Shared _ManualFieldName As String
    Private Shared _ManualFieldValue As String
    Private Shared _ManualFieldOldValue As String
    Private Shared _ManualOperation As String
    Private Shared _ManualParameterField1 As String
    Private Shared _ManualParameterOperator1 As String
    Private Shared _ManualParameterValue1 As String
    Private Shared _ManualParameterField2 As String
    Private Shared _ManualParameterOperator2 As String
    Private Shared _ManualParameterValue2 As String
    Private Shared _ManualParameterAndOr As String
    Private Shared _ManualParameterMatchAll As Boolean
    Private Shared _ManualXMLPath As String = String.Empty
    Private Shared _ManualParserPath As String = String.Empty
    Private Shared _ManualExcludedMoviesPath As String = String.Empty
    Private Shared _ManualInternetLookupAlwaysPrompt As Boolean = False
    Private Shared _ManualMissingFanartDownload As Boolean = True
    Private Shared _ManualMissingTrailer As Boolean = True
    Private Shared _TempXMLBackupFile As String

    Private Shared _OperationCancelled As Boolean = False

    Public ReadOnly Property CountXMLRecords() As Integer
        Get
            _CountXMLRecords = 0
            _CountXMLRecords = ds.Tables("tblXML").Rows.Count
            Return _CountXMLRecords
        End Get
    End Property
    Public ReadOnly Property CountFilesFound() As Integer
        Get
            Dim FileCount As Integer = 0
            If ds.Tables("tblFoundMediaFiles") IsNot Nothing Then
                FileCount += ds.Tables("tblFoundMediaFiles").Rows.Count
            End If
            If ds.Tables("tblFoundNonMediaFiles") IsNot Nothing Then
                FileCount += ds.Tables("tblFoundNonMediaFiles").Rows.Count
            End If
            Return FileCount
        End Get
    End Property
    Public ReadOnly Property CountOrphanFiles() As Integer
        Get
            _CountOrphanFiles = 0
            If ds.Tables("tblOrphanedMediaFiles") IsNot Nothing Then
                _CountOrphanFiles += ds.Tables("tblOrphanedMediaFiles").Rows.Count
            End If
            If ds.Tables("tblOrphanedNonMediaFiles") IsNot Nothing Then
                _CountOrphanFiles += ds.Tables("tblOrphanedNonMediaFiles").Rows.Count
            End If

            Return _CountOrphanFiles
        End Get
    End Property
    Public ReadOnly Property CountOrphanRecords() As Integer
        Get
            _CountOrphanRecords = 0
            If ds.Tables("tblOrphanedAntRecords").Rows.Count > 0 Then
                _CountOrphanRecords = ds.Tables("tblOrphanedAntRecords").Rows.Count
            End If

            Return _CountOrphanRecords
        End Get
    End Property
    Public ReadOnly Property CountRecordsAdded() As Integer
        Get
            Return _CountRecordsAdded
        End Get
    End Property
    Public ReadOnly Property CountRecordsDeleted() As Integer
        Get
            Return _CountRecordsDeleted
        End Get
    End Property
    Public ReadOnly Property CountMultiPartFiles() As Integer
        Get
            If ds.Tables("tblMultiPartFiles") IsNot Nothing Then
                _CountMultiPartFiles = ds.Tables("tblMultiPartFiles").Rows.Count
            End If

            Return _CountMultiPartFiles
        End Get
    End Property
    Public ReadOnly Property CountMultiPartFilesMerged() As Integer
        Get
            Return _CountMultiPartFilesMerged
        End Get
    End Property
    Public ReadOnly Property GetAntFieldNames() As DataTable
        Get
            Dim dt As DataTable = ds.Tables("tblAntFields")
            Return dt
        End Get
    End Property


    Public Property ManualFieldName() As String
        Get
            Return _ManualFieldName
        End Get
        Set(ByVal value As String)
            _ManualFieldName = value
        End Set
    End Property
    Public Property ManualFieldValue() As String
        Get
            Return _ManualFieldValue
        End Get
        Set(ByVal value As String)
            _ManualFieldValue = value
        End Set
    End Property
    Public Property ManualFieldOldValue() As String
        Get
            Return _ManualFieldOldValue
        End Get
        Set(ByVal value As String)
            _ManualFieldOldValue = value
        End Set
    End Property
    Public Property ManualOperation() As String
        Get
            Return _ManualOperation
        End Get
        Set(ByVal value As String)
            _ManualOperation = value
        End Set
    End Property
    Public Property ManualParameterField1() As String
        Get
            Return _ManualParameterField1
        End Get
        Set(ByVal value As String)
            _ManualParameterField1 = value
        End Set
    End Property
    Public Property ManualParameterOperator1() As String
        Get
            Return _ManualParameterOperator1
        End Get
        Set(ByVal value As String)
            _ManualParameterOperator1 = value
        End Set
    End Property
    Public Property ManualParameterValue1() As String
        Get
            Return _ManualParameterValue1
        End Get
        Set(ByVal value As String)
            _ManualParameterValue1 = value
        End Set
    End Property
    Public Property ManualParameterField2() As String
        Get
            Return _ManualParameterField2
        End Get
        Set(ByVal value As String)
            _ManualParameterField2 = value
        End Set
    End Property
    Public Property ManualParameterOperator2() As String
        Get
            Return _ManualParameterOperator2
        End Get
        Set(ByVal value As String)
            _ManualParameterOperator2 = value
        End Set
    End Property
    Public Property cbManualParameterAndOr() As String
        Get
            Return _ManualParameterAndOr
        End Get
        Set(ByVal value As String)
            _ManualParameterAndOr = value
        End Set
    End Property
    Public Property ManualParameterValue2() As String
        Get
            Return _ManualParameterValue2
        End Get
        Set(ByVal value As String)
            _ManualParameterValue2 = value
        End Set
    End Property

    Public Property ManualParameterMatchAll() As Boolean
        Get
            Return _ManualParameterMatchAll
        End Get
        Set(ByVal value As Boolean)
            _ManualParameterMatchAll = value
        End Set
    End Property
    Public Property ManualMissingFanartDownload() As Boolean
        Get
            Return _ManualMissingFanartDownload
        End Get
        Set(ByVal value As Boolean)
            _ManualMissingFanartDownload = value
        End Set
    End Property
    Public Property ManualMissingTrailer() As Boolean
        Get
            Return _ManualMissingTrailer
        End Get
        Set(ByVal value As Boolean)
            _ManualMissingTrailer = value
        End Set
    End Property
    Public Property ManualXMLPath() As String
        Get
            Return _ManualXMLPath
        End Get
        Set(ByVal value As String)
            _ManualXMLPath = value
        End Set
    End Property
    Public Property ManualParserPath() As String
        Get
            Return _ManualParserPath
        End Get
        Set(ByVal value As String)
            _ManualParserPath = value
        End Set
    End Property
    Public Property ManualExcludedMoviesPath()
        Get
            Return _ManualExcludedMoviesPath
        End Get
        Set(ByVal value)
            _ManualExcludedMoviesPath = value
        End Set
    End Property
    Public Property ManualInternetLookupAlwaysPrompt() As Boolean
        Get
            Return _ManualInternetLookupAlwaysPrompt
        End Get
        Set(ByVal value As Boolean)
            _ManualInternetLookupAlwaysPrompt = value
        End Set
    End Property
    Public ReadOnly Property ManualTestResultCount() As Integer
        Get
            If ds.Tables("tblNodesToProcess") Is Nothing Then
                Return 0
            Else
                Return ds.Tables("tblNodesToProcess").Rows.Count
            End If
        End Get
    End Property

    Public Property InteractiveMode() As Boolean
        Get
            Return _InteractiveMode
        End Get
        Set(ByVal value As Boolean)
            _InteractiveMode = value
        End Set
    End Property

    Public Sub New()
        BuildTables()
        bgwFolderScanUpdate.WorkerReportsProgress = True
        bgwFolderScanUpdate.WorkerSupportsCancellation = True
        bgwManualUpdate.WorkerReportsProgress = True
        bgwManualUpdate.WorkerSupportsCancellation = True
    End Sub

#Region "Manual Updater"


    Public Function ManualControlRecord(ByVal ManualParameterField As String, ByVal ManualParameterOperator As String, ByVal ManualParameterValue As String, ByRef CurrentNode As XmlNode) As Integer
        Select Case ManualParameterOperator
            Case "LIKE"
                If CurrentNode.Attributes(ManualParameterField) IsNot Nothing Then
                    Dim str1 As String = CurrentNode.Attributes(ManualParameterField).Value.ToString
                    Dim str2 As String = ManualParameterValue

                    If str1.ToLower.IndexOf(str2.ToLower) >= 0 Then
                        Return 1
                    Else
                        Return 0
                    End If
                Else
                    Return 0
                End If
            Case "NOT LIKE"
                If CurrentNode.Attributes(ManualParameterField) IsNot Nothing Then
                    Dim str1 As String = CurrentNode.Attributes(ManualParameterField).Value.ToString
                    Dim str2 As String = ManualParameterValue

                    If str1.ToLower.IndexOf(str2.ToLower) >= 0 Then
                        Return 0
                    Else
                        Return 1
                    End If
                Else
                    Return 1
                End If
            Case "="
                If CurrentNode.Attributes(ManualParameterField) IsNot Nothing Then
                    If CurrentNode.Attributes(ManualParameterField).Value.ToString = ManualParameterValue Then
                        Return 1
                    Else
                        Return 0
                    End If
                Else
                    Return 0
                End If
            Case "!="
                If CurrentNode.Attributes(ManualParameterField) IsNot Nothing Then
                    'Attribute exists, check it's not a match:
                    If CurrentNode.Attributes(ManualParameterField).Value.ToString <> ManualParameterValue Then
                        Return 1
                    Else
                        Return 0
                    End If
                Else
                    'Not there, so obviously not equal to our parameter!
                    Return 1
                End If
            Case ">"
                If CurrentNode.Attributes(ManualParameterField) IsNot Nothing Then
                    If CurrentNode.Attributes(ManualParameterField).Value.ToString > ManualParameterValue Then
                        Return 1
                    Else
                        Return 0
                    End If
                Else
                    Return 0
                End If
            Case "<"
                If CurrentNode.Attributes(ManualParameterField) IsNot Nothing Then
                    'Attribute exists, check it's not a match:
                    If CurrentNode.Attributes(ManualParameterField).Value.ToString < ManualParameterValue Then
                        Return 1
                    Else
                        Return 0
                    End If
                End If
            Case "EXISTS"
                If CurrentNode.Attributes(ManualParameterField) IsNot Nothing Then
                    Return 1
                Else
                    Return 0
                End If
            Case "NOT EXISTS"
                If CurrentNode.Attributes(ManualParameterField) Is Nothing Then
                    Return 1
                Else
                    Return 0
                End If
        End Select

    End Function
    Public Sub ManualTestOperation()

        '_ManualOperation = 'Update Value', 'Delete Record', 'Delete Value', 'Update Record'
        '_ManualFieldName = For Update or Delete Value
        '_ManualFieldValue = as above
        '_ManualParameterField = ant field to check
        '_ManualParameterOperator = '=', '!=', 'EXISTS', 'NOT EXISTS'
        '_ManualParameterValue = value to set.
        '_ManualParameterMatchAll = ignore params and match all

        'Dim NodesToProcess As New ArrayList()
        Dim TextReader As Xml.XmlTextReader
        TextReader = New Xml.XmlTextReader(_ManualXMLPath)
        Dim CurrentNode As Xml.XmlNode

        'Update a value = check if attribute exists and update it -
        'Delete a record - check the parameter and delete if match. - 
        'Delete Value = check if attribute exists and delete it - 

        LogEvent("Manual Database Operation Starting", EventLogLevel.ImportantEvent)
        LogEvent(" - Operation : " & _ManualOperation.ToString, EventLogLevel.ImportantEvent)
        If (_ManualOperation.ToString = "Update Value") Then
            LogEvent(" - Field to Update : " & _ManualFieldName.ToString, EventLogLevel.ImportantEvent)
            LogEvent(" - Value to Set : " & _ManualFieldValue.ToString, EventLogLevel.ImportantEvent)
        End If
        If (_ManualOperation.ToString = "Update Value - Replace String") Then
            LogEvent(" - Field to Update : " & _ManualFieldName.ToString, EventLogLevel.ImportantEvent)
            LogEvent(" - Value to be replaced : " & _ManualFieldOldValue.ToString, EventLogLevel.ImportantEvent)
            LogEvent(" - Value to replace with : " & _ManualFieldValue.ToString, EventLogLevel.ImportantEvent)
        End If
        If (_ManualOperation.ToString = "Update Value - Add String") Then
            LogEvent(" - Field to Update : " & _ManualFieldName.ToString, EventLogLevel.ImportantEvent)
            LogEvent(" - Value to add to existing value : " & _ManualFieldValue.ToString, EventLogLevel.ImportantEvent)
        End If
        If (_ManualOperation.ToString = "Update Value - Remove String") Then
            LogEvent(" - Field to Update : " & _ManualFieldName.ToString, EventLogLevel.ImportantEvent)
            LogEvent(" - Value to remove from existing value : " & _ManualFieldValue.ToString, EventLogLevel.ImportantEvent)
        End If
        If _ManualParameterMatchAll = True Then
            LogEvent(" - Parameters - Match All Records : True", EventLogLevel.ImportantEvent)
        Else
            LogEvent(" - Parameters - Field to Check : " & _ManualParameterField1.ToString, EventLogLevel.ImportantEvent)
            LogEvent(" - Parameters - Operator : " & _ManualParameterOperator1.ToString, EventLogLevel.ImportantEvent)
            LogEvent(" - Parameters - Value to Find : " & _ManualParameterValue1.ToString, EventLogLevel.ImportantEvent)
            If _ManualParameterAndOr = "and" Or _ManualParameterAndOr = "or" Then
                LogEvent(" - Parameters - More : " & _ManualParameterAndOr.ToString, EventLogLevel.ImportantEvent)
                LogEvent(" - Parameters - Field to Check : " & _ManualParameterField2.ToString, EventLogLevel.ImportantEvent)
                LogEvent(" - Parameters - Operator : " & _ManualParameterOperator2.ToString, EventLogLevel.ImportantEvent)
                LogEvent(" - Parameters - Value to Find : " & _ManualParameterValue2.ToString, EventLogLevel.ImportantEvent)
            End If
            If _ManualParserPath <> "" Then
                LogEvent(" - Parameters - Internet Parser : " & _ManualParserPath.ToString, EventLogLevel.ImportantEvent)
                LogEvent(" - Parameters - Excluded Movies File : " & _ManualExcludedMoviesPath.ToString, EventLogLevel.ImportantEvent)
                LogEvent(" - Parameters - Internet Lookup Always Prompt : " & _ManualInternetLookupAlwaysPrompt.ToString, EventLogLevel.ImportantEvent)
            End If
        End If
        If (_ManualOperation.ToString = "DownLoad Fanart") Then
            LogEvent(" - Download only Missing Fanart : " & _ManualMissingFanartDownload.ToString, EventLogLevel.ImportantEvent)
        End If
        If (_ManualOperation.ToString = "Register Trailer") Then
            LogEvent(" - Register only Missing Trailer : " & _ManualMissingTrailer.ToString, EventLogLevel.ImportantEvent)
        End If

        'Dim XmlDoc As New XmlDocument
        XMLDoc = New XmlDocument
        XMLDoc.Load(_ManualXMLPath)
        Dim CurrentMovieNumber As Integer

        While TextReader.Read()
            If TextReader.Name = "Movie" Then
                CurrentMovieNumber = TextReader.GetAttribute("Number")
                CurrentNode = XMLDoc.SelectSingleNode("//AntMovieCatalog/Catalog/Contents/Movie[@Number='" & CurrentMovieNumber.ToString & "']")
                If CurrentNode IsNot Nothing Then
                    Dim wtitle As String
                    If (Not IsNothing(CurrentNode.Attributes("TranslatedTitle"))) Then
                        If (CurrentNode.Attributes("TranslatedTitle").Value.Length <> 0) Then
                            wtitle = CurrentNode.Attributes("TranslatedTitle").Value
                        Else
                            wtitle = CurrentNode.Attributes("OriginalTitle").Value
                        End If
                    Else
                        wtitle = CurrentNode.Attributes("OriginalTitle").Value
                    End If
                    'Dim wotitle As String = wtitle
                    'If (Not IsNothing(CurrentNode.Attributes("OriginalTitle"))) Then
                    '    If (CurrentNode.Attributes("OriginalTitle").Value.Length <> 0) Then
                    '        wotitle = Grabber.GrabUtil.normalizeTitle(CurrentNode.Attributes("OriginalTitle").Value)
                    '    End If
                    'End If

                    If (((Not _ManualMissingFanartDownload) Or (_ManualMissingFanartDownload And ManualTestMissingFanart(wtitle))) And (_ManualOperation = "Download Fanart")) Or (Not _ManualOperation = "Download Fanart") Then
                        If _ManualParameterMatchAll = True Then
                            'We're matching all records - proceed with editing
                            Dim wyear As String = ""
                            If (Not IsNothing(CurrentNode.Attributes("Year"))) Then
                                wyear = CurrentNode.Attributes("Year").Value
                            End If
                            If (Not IsNothing(CurrentNode.Attributes("Director"))) Then
                                ds.Tables("tblNodesToProcess").Rows.Add(New Object() {CurrentMovieNumber, wtitle, CurrentNode.Attributes("Director").Value, wyear})
                            Else
                                ds.Tables("tblNodesToProcess").Rows.Add(New Object() {CurrentMovieNumber, wtitle, "", wyear})
                            End If
                            LogEvent(" - Entry to process : " & CurrentMovieNumber.ToString & " | " & wtitle, EventLogLevel.Informational)
                        Else
                            'Parameters in use - check first then proceed
                            If CurrentNode IsNot Nothing Then
                                Dim wrecord As Integer
                                wrecord = ManualControlRecord(_ManualParameterField1, _ManualParameterOperator1, _ManualParameterValue1, CurrentNode)
                                wrecord = wrecord + ManualControlRecord(_ManualParameterField2, _ManualParameterOperator2, _ManualParameterValue2, CurrentNode)
                                If (_ManualParameterAndOr <> "and" And wrecord > 0) Then
                                    ds.Tables("tblNodesToProcess").Rows.Add(New Object() {CurrentMovieNumber, wtitle})
                                    LogEvent(" - Entry to process : " & CurrentMovieNumber.ToString & " | " & wtitle, EventLogLevel.Informational)
                                Else
                                    If (_ManualParameterAndOr = "and" And wrecord = 2) Then
                                        ds.Tables("tblNodesToProcess").Rows.Add(New Object() {CurrentMovieNumber, wtitle})
                                        LogEvent(" - Entry to process : " & CurrentMovieNumber.ToString & " | " & wtitle, EventLogLevel.Informational)
                                    End If
                                End If
                            End If
                        End If
                    End If
                End If
            End If
        End While

        If ds.Tables("tblNodesToProcess").Rows.Count > 0 Then
            LogEvent("Ready to Proceed. " & ds.Tables("tblNodesToProcess").Rows.Count.ToString & " Records Matched.", EventLogLevel.ImportantEvent)
            Form1.btnManualApplyChanges.Enabled = True
        Else
            LogEvent("Nothing to process - " & ds.Tables("tblNodesToProcess").Rows.Count.ToString & " Records Matched.", EventLogLevel.ImportantEvent)
            Form1.btnManualApplyChanges.Enabled = False
        End If
        LogEvent("===================================================================================================", EventLogLevel.Informational)


        If Not (TextReader Is Nothing) Then
            TextReader.Close()
        End If



    End Sub

    Public Sub ManualRunOperation()

        _OperationCancelled = False

        ' backup the xml file before updating 
        If CurrentSettings.Backup_XML_First = True Then
            LogEvent("Backing up xml file.", EventLogLevel.ImportantEvent)
            Dim NewFileName As String = Replace(CurrentSettings.Manual_XML_File, ".xml", " - " + My.Computer.Clock.LocalTime.ToString.Replace(":", "-") + ".xml")
            NewFileName = NewFileName.Replace("/", "-")

            Try
                My.Computer.FileSystem.CopyFile(CurrentSettings.Manual_XML_File, NewFileName, True)
                LogEvent("Backing up xml file - done.", EventLogLevel.ImportantEvent)
            Catch ex As Exception
                LogEvent("ERROR : Cannot back up xml file : " & ex.Message, EventLogLevel.ErrorOrSimilar)
            End Try
        End If

        If ds.Tables("tblNodesToProcess") IsNot Nothing Then
            LogEvent("Performing Manual Update Process", EventLogLevel.ImportantEvent)
            Form1.ToolStripProgressBar.Minimum = 0
            Form1.ToolStripProgressBar.Maximum = ds.Tables("tblNodesToProcess").Rows.Count + 1
            Form1.ToolStripProgressBar.Value = 0
            Form1.btnManualCancel.Enabled = True
            Form1.btnManualDoTest.Enabled = False
            Form1.btnManualApplyChanges.Enabled = False
            Dim muc As New ManualUpdateClass
            muc.XmlDoc = XMLDoc

            'TO SWITCH FROM SINGLE-THREAD to MULTI-THREAD, CHANGE HERE!
            bgwManualUpdate.RunWorkerAsync(muc)
            'muc.RunUpdate()
            'bgwManualUpdate_PostProcessing()
        End If
    End Sub

    Private Class ManualUpdateClass

        Public XmlDoc As XmlDocument
        'Public ParserPath As String
        'Public ExcludeFile As String
        'Public InteractiveMode As Boolean
        'Public InternetLookupAlwaysPrompt As Boolean

        Public Sub RunUpdate()

            Dim CurrentNode As Xml.XmlNode
            Dim newAttr As Xml.XmlAttribute
            Dim MovieRootNode As Xml.XmlNode = XmlDoc.SelectSingleNode("//AntMovieCatalog/Catalog/Contents")
            Dim ProcessCounter As Integer = 0
            Dim DoScan As Boolean = True

            For Each row As DataRow In ds.Tables("tblNodesToProcess").Rows
                CurrentNode = XmlDoc.SelectSingleNode("//AntMovieCatalog/Catalog/Contents/Movie[@Number='" & row("AntID") & "']")
                If bgwManualUpdate.CancellationPending = True Then
                    Exit Sub
                End If

                Select Case _ManualOperation
                    'Update Value
                    'Delete Value
                    'Update Record
                    'Delete Record
                    'Download Fanart

                    Case "Update Value"
                        If CurrentNode.Attributes(_ManualFieldName) Is Nothing Then
                            newAttr = XmlDoc.CreateAttribute(_ManualFieldName)
                            newAttr.Value = _ManualFieldValue
                            CurrentNode.Attributes.Append(newAttr)
                            'LogEvent("Value Updated (Added too) : " & CurrentNode.Attributes("Number").Value & " | " & row("AntTitle").ToString, EventLogLevel.Informational)
                            bgwManualUpdate.ReportProgress(ProcessCounter, "Value Updated (and added) : " & CurrentNode.Attributes("Number").Value & " | " & row("AntTitle").ToString)
                        Else
                            CurrentNode.Attributes(_ManualFieldName).Value = _ManualFieldValue
                            'LogEvent("Value Updated : " & CurrentNode.Attributes("Number").Value & " | " & row("AntTitle").ToString, EventLogLevel.Informational)
                            bgwManualUpdate.ReportProgress(ProcessCounter, "Value Updated : " & CurrentNode.Attributes("Number").Value & " | " & row("AntTitle").ToString)
                        End If
                    Case "Update Value - Replace String"
                        If CurrentNode.Attributes(_ManualFieldName) Is Nothing Then
                            ' Do nothing, as old value to be replaced is not contained !
                            'newAttr = XmlDoc.CreateAttribute(_ManualFieldName)
                            'newAttr.Value = ""
                            'CurrentNode.Attributes.Append(newAttr)
                            'LogEvent("Value Updated (Added too) : " & CurrentNode.Attributes("Number").Value & " | " & row("AntTitle").ToString, EventLogLevel.Informational)
                            bgwManualUpdate.ReportProgress(ProcessCounter, "Value not Updated (No old Value present) : " & CurrentNode.Attributes("Number").Value & " | " & row("AntTitle").ToString)
                        Else
                            If CurrentNode.Attributes(_ManualFieldName).Value.Contains(_ManualFieldOldValue) = True Then
                                CurrentNode.Attributes(_ManualFieldName).Value = CurrentNode.Attributes(_ManualFieldName).Value.Replace(_ManualFieldOldValue, _ManualFieldValue)
                                'LogEvent("Value Updated : " & CurrentNode.Attributes("Number").Value & " | " & row("AntTitle").ToString, EventLogLevel.Informational)
                                bgwManualUpdate.ReportProgress(ProcessCounter, "Value Updated (Replaced String): " & CurrentNode.Attributes("Number").Value & " | " & row("AntTitle").ToString)
                            Else
                                bgwManualUpdate.ReportProgress(ProcessCounter, "Value not updated (Replace String not found): " & CurrentNode.Attributes("Number").Value & " | " & row("AntTitle").ToString)
                            End If
                        End If
                    Case "Update Value - Add String"
                        If CurrentNode.Attributes(_ManualFieldName) Is Nothing Then
                            newAttr = XmlDoc.CreateAttribute(_ManualFieldName)
                            newAttr.Value = _ManualFieldValue
                            CurrentNode.Attributes.Append(newAttr)
                            'LogEvent("Value Updated (Added too) : " & CurrentNode.Attributes("Number").Value & " | " & row("AntTitle").ToString, EventLogLevel.Informational)
                            bgwManualUpdate.ReportProgress(ProcessCounter, "Value Updated (Added Field and String) : " & CurrentNode.Attributes("Number").Value & " | " & row("AntTitle").ToString)
                        Else
                            CurrentNode.Attributes(_ManualFieldName).Value = CurrentNode.Attributes(_ManualFieldName).Value & _ManualFieldValue
                            'LogEvent("Value Updated : " & CurrentNode.Attributes("Number").Value & " | " & row("AntTitle").ToString, EventLogLevel.Informational)
                            bgwManualUpdate.ReportProgress(ProcessCounter, "Value Updated (Added String): " & CurrentNode.Attributes("Number").Value & " | " & row("AntTitle").ToString)
                        End If
                    Case "Update Value - Remove String"
                        If CurrentNode.Attributes(_ManualFieldName) Is Nothing Then
                            ' Do nothing
                            'LogEvent("Value Updated (Added too) : " & CurrentNode.Attributes("Number").Value & " | " & row("AntTitle").ToString, EventLogLevel.Informational)
                            bgwManualUpdate.ReportProgress(ProcessCounter, "Value not Removed - (No old Value present) : " & CurrentNode.Attributes("Number").Value & " | " & row("AntTitle").ToString)
                        Else
                            If CurrentNode.Attributes(_ManualFieldName).Value.Contains(_ManualFieldValue) = True Then
                                CurrentNode.Attributes(_ManualFieldName).Value = CurrentNode.Attributes(_ManualFieldName).Value.Replace(_ManualFieldValue, "")
                                'LogEvent("Value Updated : " & CurrentNode.Attributes("Number").Value & " | " & row("AntTitle").ToString, EventLogLevel.Informational)
                                bgwManualUpdate.ReportProgress(ProcessCounter, "Value Updated (Removed String): " & CurrentNode.Attributes("Number").Value & " | " & row("AntTitle").ToString)
                            Else
                                bgwManualUpdate.ReportProgress(ProcessCounter, "Value not Updated (Remove String not found): " & CurrentNode.Attributes("Number").Value & " | " & row("AntTitle").ToString)
                            End If
                        End If
                    Case "Delete Record"
                        If Not CurrentNode Is Nothing Then
                            MovieRootNode.RemoveChild(CurrentNode)
                            'LogEvent("Record Deleted : " & CurrentNode.Attributes("Number").Value & " | " & row("AntTitle").ToString, EventLogLevel.Informational)
                            bgwManualUpdate.ReportProgress(ProcessCounter, "Record Deleted : " & CurrentNode.Attributes("Number").Value & " | " & row("AntTitle").ToString)
                        End If
                    Case "Delete Value"
                        If Not CurrentNode.Attributes(_ManualFieldName) Is Nothing Then
                            CurrentNode.Attributes.Remove(CurrentNode.Attributes.GetNamedItem(_ManualFieldName))
                            'LogEvent("Value Deleted : " & CurrentNode.Attributes("Number").Value & " | " & row("AntTitle").ToString, EventLogLevel.Informational)
                            bgwManualUpdate.ReportProgress(ProcessCounter, "Value Deleted : " & CurrentNode.Attributes("Number").Value & " | " & row("AntTitle").ToString)
                        End If
                    Case "Update Record"
                        Dim FileToScan As String = String.Empty
                        Dim AllFilesPath As String = String.Empty
                        If Not IsNothing(CurrentNode.Attributes(CurrentSettings.Ant_Database_Source_Field)) Then
                            FileToScan = CurrentNode.Attributes(CurrentSettings.Ant_Database_Source_Field).Value
                        End If

                        If FileToScan.IndexOf(";") > 0 Then
                            'In case of multi-part files take the first part to internet scan; record the full list in AllFilesPath
                            AllFilesPath = FileToScan
                            FileToScan = FileToScan.Substring(0, FileToScan.IndexOf(";"))
                        End If

                        Dim wDirector As String = ""
                        Dim wYear As String = ""
                        Dim wIMDB_Id As String = ""
                        If row("AntTitle").ToString.Length > 0 Then
                            If (row("AntDirector").ToString.Length > 0) Then
                                If (row("AntDirector").ToString.IndexOf(",") > 0) Then
                                    wDirector = row("AntDirector").ToString.Substring(0, row("AntDirector").ToString.IndexOf(","))
                                Else
                                    wDirector = row("AntDirector").ToString
                                End If
                            End If
                            If (row("AntYear").ToString.Length > 0) Then
                                wYear = row("AntYear").ToString
                            End If
                            If (row("AntIMDB_Id").ToString.Length > 0) Then
                                wIMDB_Id = row("AntIMDB_Id").ToString
                            End If
                        End If

                        DoScan = True
                        ' search Filename
                        Dim wtitle As String = row("AntTitle").ToString
                        If IsFileNeeded() = True Then
                            'If My.Computer.FileSystem.FileExists(FileToScan) = False Then
                            If (CurrentSettings.Folder_Name_Is_Group_Name) Then
                                If (CurrentSettings.Group_Name_Applies_To = "Original Title" Or CurrentSettings.Group_Name_Applies_To = "Both Titles") Then
                                    wtitle = "OriginalTitle"
                                Else
                                    If (CurrentSettings.Group_Name_Applies_To = "Translated Title") Then
                                        wtitle = "TranslatedTitle"
                                    End If
                                End If
                                wtitle = CurrentNode.Attributes(wtitle).Value
                            End If
                            SearchFile(FileToScan, CurrentSettings.Movie_Scan_Path, CurrentNode.Attributes("Number").Value, wtitle)
                            If Not FileToScan.Length > 0 Then
                                bgwManualUpdate.ReportProgress(ProcessCounter, "File Not Scanned (File Not Found) : " & CurrentNode.Attributes("Number").Value & " | " & row("AntTitle").ToString)
                                'DoScan = False
                            End If
                        End If

                        'If FileToScan.Length > 0 Then
                        If DoScan = True Then
                            Dim Ant As New AntRecord()
                            With Ant
                                .XMLDoc = XmlDoc
                                .XMLElement = CurrentNode
                                .XMLFilePath = CurrentSettings.Manual_XML_File
                                .FilePath = FileToScan
                                .AllFilesPath = AllFilesPath
                                If AllFilesPath.Length > 0 Then
                                    .OverridePath = AllFilesPath
                                Else
                                    .OverridePath = FileToScan
                                End If
                                If wtitle.Length > 0 Then
                                    If wtitle.Contains("\") = True Then
                                        .GroupName = wtitle.Substring(0, wtitle.LastIndexOf("\"))
                                        'Console.WriteLine("-" & .GroupName.ToString & "-")
                                    End If
                                End If
                                'If CurrentNode.Attributes("MovieNumber") Then
                                .ExcludeFile = CurrentSettings.Manual_Excluded_Movies_File
                                .InternetLookupAlwaysPrompt = CurrentSettings.Manual_Internet_Lookup_Always_Prompt
                                .DateHandling = CurrentSettings.Date_Handling
                                .MovieTitleHandling = CurrentSettings.Movie_Title_Handling
                                .MasterTitle = CurrentSettings.Master_Title
                                .InteractiveMode = True
                                .ImagePath = CurrentSettings.Manual_XML_File.Substring(0, CurrentSettings.Manual_XML_File.LastIndexOf("\"))

                                If (CurrentSettings.Image_Download_Filename_Prefix.Length > 0) Then
                                    If CurrentSettings.Image_Download_Filename_Prefix.LastIndexOf("\") > -1 Then
                                        .ImagePath = .ImagePath & "\\" & CurrentSettings.Image_Download_Filename_Prefix.Substring(0, CurrentSettings.Image_Download_Filename_Prefix.LastIndexOf("\"))
                                    Else
                                        .ImagePath = .ImagePath & "\\" & CurrentSettings.Image_Download_Filename_Prefix
                                    End If
                                End If

                                .InternetSearchHint = wDirector
                                .InternetSearchHintYear = wYear
                                .InternetSearchHintIMDB_Id = wIMDB_Id
                                .ParserPath = CurrentSettings.Manual_Internet_Parser_Path
                                .ProcessFile(AntRecord.Process_Mode_Names.Update)
                                .SaveProgress()
                            End With



                            If Ant.LastOutputMessage.StartsWith("ERROR") = True Then
                                bgwManualUpdate.ReportProgress(ProcessCounter, Ant.LastOutputMessage)
                            Else
                                If (CurrentNode.Attributes("Number") Is Nothing) Then
                                    bgwManualUpdate.ReportProgress(ProcessCounter, "File Scanned for Data : " & row(0).ToString & " | " & row("AntTitle").ToString)
                                Else
                                    bgwManualUpdate.ReportProgress(ProcessCounter, "File Scanned for Data : " & CurrentNode.Attributes("Number").Value & " | " & row("AntTitle").ToString)
                                End If
                            End If


                        End If
                    Case "Download Fanart"
                        If Not CurrentNode Is Nothing Then
                            Dim fanart As List(Of Grabber.DBMovieInfo)
                            Dim year As Int16 = 0
                            If CurrentNode.Attributes("Year") IsNot Nothing Then
                                Try
                                    year = CInt(CurrentNode.Attributes("Year").Value)
                                Catch
                                End Try
                            End If
                            Dim director As String = ""
                            If CurrentNode.Attributes("Director") IsNot Nothing Then
                                director = CurrentNode.Attributes("Director").Value
                            End If

                            Dim Gb As Grabber.Grabber_URLClass = New Grabber.Grabber_URLClass
                            Dim wtitle As String = String.Empty
                            Dim wotitle As String = String.Empty
                            If CurrentNode.Attributes("OriginalTitle") IsNot Nothing Then
                                wotitle = CurrentNode.Attributes("OriginalTitle").Value
                            Else
                                If CurrentNode.Attributes("TranslatedTitle") IsNot Nothing Then
                                    wotitle = CurrentNode.Attributes("TranslatedTitle").Value
                                End If
                            End If
                            wtitle = wotitle
                            If CurrentNode.Attributes("TranslatedTitle") IsNot Nothing Then
                                wtitle = CurrentNode.Attributes("TranslatedTitle").Value
                            End If
                            If wtitle.Contains("\") Then
                                wtitle = wtitle.Substring(wtitle.LastIndexOf("\") + 1)
                            End If
                            If wotitle.Contains("\") Then
                                wotitle = wotitle.Substring(wtitle.LastIndexOf("\") + 1)
                            End If
                            If wtitle.Length > 0 Then
                                'If CurrentNode.Attributes("OriginalTitle").Value.ToString.Contains("\") = True Then
                                '    Dim Title As String = CurrentNode.Attributes("OriginalTitle").Value.ToString.Substring(0, CurrentNode.Attributes("OriginalTitle").Value.ToString.IndexOf("\"))
                                '    'Console.WriteLine("-" & .GroupName.ToString & "-")

                                '    fanart = Gb.GetFanart(Title, year, director, CurrentSettings.Movie_Fanart_Path, True)
                                'Else
                                fanart = Gb.GetFanart(wotitle, wtitle, year, director, CurrentSettings.Movie_Fanart_Path, True, False, CurrentSettings.Master_Title)
                                'End If
                                If (fanart.Count > 0) Then
                                    If (fanart(0).Name = "already") Then
                                        bgwManualUpdate.ReportProgress(ProcessCounter, "Fanart already downloaded : " & CurrentNode.Attributes("Number").Value & " | " & row("AntTitle").ToString)
                                    ElseIf (fanart(0).Name = "added") Then
                                        bgwManualUpdate.ReportProgress(ProcessCounter, "Fanart added : " & CurrentNode.Attributes("Number").Value & " | " & row("AntTitle").ToString)
                                    ElseIf (fanart(0).Name.StartsWith("(toomany)")) Then
                                        bgwManualUpdate.ReportProgress(ProcessCounter, "Too many Movies found : " & CurrentNode.Attributes("Number").Value & " | " & row("AntTitle").ToString)
                                    Else
                                        If (fanart.Count = 1 And fanart(0).Backdrops.Count > 0) Then
                                            bgwManualUpdate.ReportProgress(ProcessCounter, "Fanart downloaded : " & CurrentNode.Attributes("Number").Value & " | " & row("AntTitle").ToString)
                                        Else
                                            bgwManualUpdate.ReportProgress(ProcessCounter, "Fanart not Found : " & CurrentNode.Attributes("Number").Value & " | " & row("AntTitle").ToString)
                                        End If
                                    End If
                                Else
                                    wtitle = Grabber.GrabUtil.CreateFilename(wtitle.ToLower().Trim.Replace(" ", "."))
                                    If Not (System.IO.Directory.Exists(CurrentSettings.Movie_Fanart_Path & "\\{" & wtitle & "}")) Then
                                        System.IO.Directory.CreateDirectory(CurrentSettings.Movie_Fanart_Path & "\\{" & wtitle & "}")
                                    End If
                                    bgwManualUpdate.ReportProgress(ProcessCounter, "Fanart not Found : " & CurrentNode.Attributes("Number").Value & " | " & row("AntTitle").ToString)
                                End If
                            End If
                        End If

                End Select
                ProcessCounter += 1
            Next

        End Sub

        Public Sub New()

        End Sub
    End Class

    Private Shared Sub bgwManualUpdate_DoWork(ByVal sender As Object, ByVal e As System.ComponentModel.DoWorkEventArgs) Handles bgwManualUpdate.DoWork
        Dim ManualUpdateObject As ManualUpdateClass = CType(e.Argument, ManualUpdateClass)
        'e.Result = ManualUpdateObject.RunUpdate()
        ManualUpdateObject.RunUpdate()
    End Sub

    Private Shared Sub bgwManualUpdate_RunWorkerCompleted(ByVal sender As Object, ByVal e As System.ComponentModel.RunWorkerCompletedEventArgs) Handles bgwManualUpdate.RunWorkerCompleted


        If e.Error IsNot Nothing Then
            'If e.Error.Message.ToString <> "" Then
            LogEvent("ERROR : " & e.Error.Message.ToLower, EventLogLevel.ErrorOrSimilar)
            'End If
        End If


        If _OperationCancelled = True Then
            If MsgBox("Save work done so far?", MsgBoxStyle.YesNo) = MsgBoxResult.No Then
                LogEvent("Operation Cancelled.", EventLogLevel.ErrorOrSimilar)
                Form1.ToolStripProgressBar.Value = Form1.ToolStripProgressBar.Maximum
                Try
                    My.Computer.FileSystem.CopyFile(_TempXMLBackupFile, CurrentSettings.Manual_XML_File, True)
                    My.Computer.FileSystem.DeleteFile(_TempXMLBackupFile, FileIO.UIOption.OnlyErrorDialogs, FileIO.RecycleOption.SendToRecycleBin)
                Catch ex As Exception

                End Try
                Exit Sub
            Else
                LogEvent("Operation Cancelled - Save Continuing", EventLogLevel.ErrorOrSimilar)
            End If
        End If
        ' added to force refresh of View Collection Tab
        Form1.txtConfigFilePath_TextChanged(sender, e)

        'XMLDoc.Save(_ManualXMLPath)
        'LogEvent("Manual Update Process Complete.", EventLogLevel.ImportantEvent)
        'LogEvent("===================================================================================================", EventLogLevel.Informational)
        'MsgBox("Process Complete", MsgBoxStyle.Information)
        bgwManualUpdate_PostProcessing()

    End Sub

    Private Shared Sub bgwManualUpdate_PostProcessing()
        XMLDoc.Save(_ManualXMLPath)
        Form1.btnManualCancel.Enabled = False
        Form1.btnManualApplyChanges.Enabled = False
        Form1.btnManualDoTest.Enabled = True
        Form1.ToolStripProgressBar.Value = Form1.ToolStripProgressBar.Maximum
        LogEvent("Manual Update Process Complete.", EventLogLevel.ImportantEvent)
        LogEvent("===================================================================================================", EventLogLevel.Informational)

        Dim dtFiles As New DataTable
        dtFiles.Columns.Add("FileString", System.Type.GetType("System.String"))
        If CurrentSettings.Excluded_File_Strings.Length > 0 Then
            Dim ExcludeFiles() As String
            ExcludeFiles = CurrentSettings.Excluded_File_Strings.Split("|")
            If ExcludeFiles.Length > 0 Then
                For i As Integer = 0 To ExcludeFiles.Length - 1
                    dtFiles.Rows.Add(ExcludeFiles(i))
                Next
            End If
        End If
        Form1.dgExcludedFileStrings.DataSource = dtFiles
        Form1.dgExcludedFileStrings.Columns(0).Width = Form1.dgExcludedFileStrings.Width - Form1.dgExcludedFileStrings.RowHeadersWidth - 20


        If CurrentSettings.Execute_Program = False Or CurrentSettings.Execute_Program_Path = "" Then
            MsgBox("Process Complete", MsgBoxStyle.Information)
        ElseIf CurrentSettings.Execute_Program = True And CurrentSettings.Execute_Program_Path <> "" Then
            If CurrentSettings.Execute_Only_For_Orphans = False Then
                Try
                    System.Diagnostics.Process.Start(CurrentSettings.Execute_Program_Path)
                Catch ex As Exception

                End Try
            Else
                If _CountRecordsAdded + _CountRecordsDeleted > 0 Then
                    Try
                        System.Diagnostics.Process.Start(CurrentSettings.Execute_Program_Path)
                    Catch ex As Exception

                    End Try

                End If
            End If
        End If

        'MsgBox("Process Complete", MsgBoxStyle.Information)


    End Sub

    Private Shared Sub bgwManualUpdate_ProgressChanged(ByVal sender As Object, ByVal e As ProgressChangedEventArgs) Handles bgwManualUpdate.ProgressChanged

        'Console.WriteLine(e.UserState)
        'Console.WriteLine("Min: " & Form1.ToolStripProgressBar.Minimum.ToString & ", Max : " & Form1.ToolStripProgressBar.Maximum.ToString & ", Value : " & Form1.ToolStripProgressBar.Value.ToString)
        If Form1.ToolStripProgressBar.Value < Form1.ToolStripProgressBar.Maximum Then
            Form1.ToolStripProgressBar.Value += 1
            Form1.ToolStripProgressMessage.Text = "status: " & Form1.ToolStripProgressBar.Value.ToString & " of " & Form1.ToolStripProgressBar.Maximum.ToString & " total movie(s)"
        End If
        'The e.UserState includes messages from the background process.  In this case, it's the 'File Imported - Filename' message.
        LogEvent(e.UserState, EventLogLevel.Informational)

    End Sub
    Public Sub bgwManualUpdate_Cancel()
        bgwManualUpdate.CancelAsync()
        Form1.btnManualDoTest.Enabled = True
        Form1.btnManualApplyChanges.Enabled = True
        Form1.btnManualCancel.Enabled = False

        _OperationCancelled = True

        LogEvent("Halting Operation by user request - Please Wait.", EventLogLevel.ErrorOrSimilar)
    End Sub

#End Region

#Region "Main Import Run"
    Public Sub ProcessXML(Optional ByVal XMLFilePath As String = "")
        Dim textReader As Xml.XmlTextReader
        Dim CurrentMovieTitle As String = ""
        Dim CurrentMovieNumber As Integer
        Dim objReader As System.IO.StreamReader
        Dim PlaylistEntry As String = String.Empty

        Try
            If ds.Tables("tblXML") IsNot Nothing Then
                ds.Tables("tblXML").Clear()
            End If

            LogEvent("---------------------------------------------------------------------------------------------------", EventLogLevel.Informational)
            LogEvent("Parsing XML File", EventLogLevel.ImportantEvent)
            LogEvent("===================================================================================================", EventLogLevel.Informational)


            'If XMLFilePath = "" Then
            textReader = New Xml.XmlTextReader(CurrentSettings.XML_File)
            'Else
            'textReader = New Xml.XmlTextReader(XMLFilePath)
            'End If
            While textReader.Read()
                If textReader.Name = "Movie" Then
                    CurrentMovieTitle = textReader.GetAttribute(CurrentSettings.Ant_Database_Source_Field)
                    CurrentMovieNumber = textReader.GetAttribute("Number")
                    If Not CurrentMovieTitle Is Nothing Then
                        If CurrentMovieTitle.ToLower.EndsWith(".m3u") = True Then
                            If File.Exists(CurrentMovieTitle) Then
                                objReader = New System.IO.StreamReader(CurrentMovieTitle)
                                Do While objReader.Peek() <> -1
                                    PlaylistEntry = objReader.ReadLine()
                                    If PlaylistEntry <> String.Empty Then
                                        '
                                    End If
                                Loop

                            Else
                                'Referenced .m3u file doesn't exist or not present.
                            End If
                        Else
                            'Check for multi-valued entries:
                            'If CurrentMovieTitle.IndexOf(";") > -1 Then
                            Dim TitleList() As String
                            TitleList = CurrentMovieTitle.Split(";")
                            Dim FolderList() As String
                            FolderList = CurrentSettings.Movie_Scan_Path.Split(";")
                            Dim myEnumerator As System.Collections.IEnumerator = TitleList.GetEnumerator()
                            Dim shortitle As String = String.Empty
                            While myEnumerator.MoveNext()
                                shortitle = myEnumerator.Current.ToString
                                For Each folder As String In FolderList
                                    shortitle = StripPathFromFile(myEnumerator.Current.ToString, folder, CurrentSettings.Override_Path)
                                    If Not shortitle = myEnumerator.Current.ToString Then
                                        Exit For
                                    End If
                                Next

                                ds.Tables("tblXML").Rows.Add(New Object() {CurrentMovieNumber, myEnumerator.Current.ToString, shortitle})
                                LogEvent("  Entry Found - " & myEnumerator.Current.ToString, EventLogLevel.Informational)
                            End While
                            TitleList = Nothing
                        End If
                    End If
                End If
            End While

            If Not (textReader Is Nothing) Then
                textReader.Close()
            End If

            LogEvent("---------------------------------------------------------------------------------------------------", EventLogLevel.Informational)
            LogEvent("Parsing XML File - Complete - " & ds.Tables("tblXML").Rows.Count.ToString & " entries found.", EventLogLevel.ImportantEvent)
            LogEvent("===================================================================================================", EventLogLevel.Informational)
        Catch ex As Exception
            LogEvent("ERROR : Cannot parse XML file", EventLogLevel.ErrorOrSimilar)
        End Try

    End Sub

    Public Sub ProcessMovieFolder()
        'Sub to enumerate all files in the given MoviePath location.

        If ds.Tables("tblFoundNonMediaFiles") IsNot Nothing Then
            ds.Tables("tblFoundNonMediaFiles").Clear()
        End If
        If ds.Tables("tblFoundMediaFiles") IsNot Nothing Then
            ds.Tables("tblFoundMediaFiles").Clear()
        End If

        Dim XMLExclTable As New Hashtable
        If (IO.File.Exists(CurrentSettings.Excluded_Movies_File)) Then
            Dim sr As StreamReader = File.OpenText(CurrentSettings.Excluded_Movies_File)
            Dim i As Integer = 0
            Do While sr.Peek() >= 0
                XMLExclTable.Add(i, sr.ReadLine)
                i += 1
            Loop
        End If

        Dim FoundFileName As String
        Dim ValidMediaExtensions As String() = CurrentSettings.File_Types_Media.Split(";")
        Dim ValidNonMediaExtensions As String() = CurrentSettings.File_Types_Non_Media.Split(";")
        Dim row As DataRow

        Dim CurrentMoviePath As String
        For Each CurrentMoviePath In CurrentSettings.Movie_Scan_Path.Split(";")

            Dim dir As New DirectoryInfo(CurrentMoviePath)
            If Not dir.Exists Then
                LogEvent("ERROR : Cannot access folder '" + CurrentMoviePath.ToString + "'", EventLogLevel.ErrorOrSimilar)
            Else
                If Not CurrentMoviePath.EndsWith("\") = True Then
                    CurrentMoviePath = CurrentMoviePath & "\"
                End If

                LogEvent("Processing Movie Folder : " & CurrentMoviePath.ToString, EventLogLevel.ImportantEvent)
                LogEvent("---------------------------------------------------------------------------------------------------", EventLogLevel.Informational)

                Dim blah As New FileFolderEnum()
                'blah.Root = CurrentMoviePath
                blah.ExcludedFiles = CurrentSettings.Excluded_File_Strings
                blah.ExcludedFolders = CurrentSettings.Excluded_Folder_Strings
                blah.GetFiles(CurrentMoviePath)
                For Each foundFile As String In blah.Files

                    If (XMLExclTable.ContainsValue(foundFile.ToLower) = False) Then
                        'Check for match against movie file types:
                        Try
                            'I took out the Override path here so every file gets loaded into tblFoundMovieFiles properly.  Override path handling moved to ProcessOr
                            FoundFileName = StripPathFromFile(foundFile, CurrentMoviePath)

                            'File Handling - compare extension to known media filetypes
                            If Array.IndexOf(ValidMediaExtensions, foundFile.Substring(InStrRev(foundFile, "."))) >= 0 Then
                                LogEvent("  File Found - " & FoundFileName, EventLogLevel.Informational)

                                row = ds.Tables("tblFoundMediaFiles").NewRow()
                                row("FileName") = FoundFileName
                                row("FilePath") = CurrentMoviePath
                                ds.Tables("tblFoundMediaFiles").Rows.Add(row)

                            ElseIf Array.IndexOf(ValidNonMediaExtensions, foundFile.Substring(InStrRev(foundFile, "."))) >= 0 Then
                                'Check for match against non-movie file types:
                                LogEvent("  File Found - " & FoundFileName, EventLogLevel.Informational)

                                row = ds.Tables("tblFoundNonMediaFiles").NewRow()
                                row("FileName") = FoundFileName
                                row("FilePath") = CurrentMoviePath
                                ds.Tables("tblFoundNonMediaFiles").Rows.Add(row)

                            ElseIf CurrentSettings.Scan_For_DVD_Folders = True Then
                                'Finally special handling to check for DVD images in folders.
                                'If FoundFileName.ToLower.Contains("video_ts") Then
                                If Right(FoundFileName, 12).ToLower = "video_ts.ifo" Then
                                    LogEvent("  File Found - " & FoundFileName, EventLogLevel.Informational)

                                    row = ds.Tables("tblFoundNonMediaFiles").NewRow()
                                    row("FileName") = FoundFileName
                                    row("FilePath") = CurrentMoviePath
                                    ds.Tables("tblFoundNonMediaFiles").Rows.Add(row)
                                End If
                            End If
                        Catch ex As Exception
                            LogEvent("ERROR : " & ex.Message, EventLogLevel.ErrorOrSimilar)
                        End Try
                    Else
                        LogEvent("  File Excluded - **********  " & foundFile & "  *****", EventLogLevel.Informational)
                    End If

                Next

            End If
        Next
        LogEvent("---------------------------------------------------------------------------------------------------", EventLogLevel.Informational)
        LogEvent("Processing Movie Folder - Done - " & CountFilesFound.ToString & " files found.", EventLogLevel.ImportantEvent)
        LogEvent("===================================================================================================", EventLogLevel.Informational)
    End Sub

    Public Sub ProcessOrphanFiles()

        Dim MovedFilesCount As Integer = 0

        'Whenever this is called, ensure the OrphanedMovies tables are cleared out first.
        If ds.Tables("tblOrphanedMediaFiles").Rows.Count > 0 Then
            ds.Tables("tblOrphanedMediaFiles").Clear()
        End If
        If ds.Tables("tblOrphanedNonMediaFiles").Rows.Count > 0 Then
            ds.Tables("tblOrphanedNonMediaFiles").Clear()
        End If
        If ds.Tables("tblMultiPartFiles").Rows.Count > 0 Then
            ds.Tables("tblMultiPartFiles").Clear()
        End If

        LogEvent("Finding Orphaned Movies (not in XML file)", EventLogLevel.ImportantEvent)
        LogEvent("---------------------------------------------------------------------------------------------------", EventLogLevel.Informational)

        '_NextAntID = fnGetNextMovieID(_XMLFilePath)

        Dim dvXML As New DataView
        dvXML = ds.Tables("tblXML").DefaultView
        dvXML.Sort = "AntPath"

        Dim dvOrphanedMediaFiles As New DataView
        dvOrphanedMediaFiles = ds.Tables("tblOrphanedMediaFiles").DefaultView
        dvOrphanedMediaFiles.Sort = "FileName"

        Dim SplitText As New Regex(CurrentSettings.RegEx_Check_For_MultiPart_Files)
        Dim m As Match

        Dim row As DataRow
        Dim CutText As New Regex(CurrentSettings.RegEx_Check_For_MultiPart_Files)
        Dim SearchName As String = String.Empty

        If ds.Tables("tblFoundMediaFiles") IsNot Nothing Then
            For Each row In ds.Tables("tblFoundMediaFiles").Rows
                m = SplitText.Match(row("FileName"))
                Dim wfile As String = String.Empty
                If CurrentSettings.Store_Short_Names_Only Then
                    wfile = row("FileName")
                    If wfile.Contains("\") Then
                        wfile = wfile.Substring(wfile.LastIndexOf("\") + 1)
                    End If
                Else
                    If CurrentSettings.Override_Path <> "" Then
                        wfile = CurrentSettings.Override_Path + row("FileName")
                    Else
                        wfile = row("FilePath") + row("FileName")
                    End If
                End If
                Dim FileMoved As Boolean = False

                If dvXML.Find(wfile) = -1 Then 'This is case-sensitive!
                    'File not already in XML - add it.
                    If dvOrphanedMediaFiles.Find(row("FileName")) = -1 Then
                        dvXML.Sort = "AntShortPath"
                        If dvXML.Find(row("FileName")) = -1 Then
                            ' Shortname not found - might be new or might have moved.
                            For Each row2 As DataRow In ds.Tables("tblXML").Rows
                                'take just the filename without any path:
                                SearchName = row2("AntPath").ToString.Substring(row2("AntPath").ToString.LastIndexOf("\") + 1)
                                If SearchName = wfile.Substring(wfile.LastIndexOf("\") + 1) Then
                                    'Match found - file moved:
                                    ds.Tables("tblOrphanedMediaFiles").Rows.Add(New Object() {row2("AntID"), row("FilePath"), CurrentSettings.Override_Path, row("FileName"), True})
                                    LogEvent("     Moved File : " + row("FileName"), EventLogLevel.Informational)
                                    MovedFilesCount += 1
                                    FileMoved = True
                                    Exit For
                                End If
                            Next
                            If FileMoved = False Then
                                'If dvXML.Find(SearchName) > -1 Then
                                'Match found for short name - file moved.
                                'ds.Tables("tblOrphanedMediaFiles").Rows.Add(New Object() {dvXML.FindRows(row("FileName")).GetValue(0).Item(0), row("FilePath"), CurrentSettings.Override_Path, row("FileName"), True})
                                'LogEvent("     Moved File : " + row("FileName"), EventLogLevel.Informational)
                                'MovedFilesCount += 1
                                'Else
                                ds.Tables("tblOrphanedMediaFiles").Rows.Add(New Object() {0, row("FilePath"), CurrentSettings.Override_Path, row("FileName"), False})
                                LogEvent("  Orphaned File : " + row("FileName"), EventLogLevel.Informational)
                            End If
                            FileMoved = False
                        Else
                            ' short name found file moved
                            ds.Tables("tblOrphanedMediaFiles").Rows.Add(New Object() {dvXML.FindRows(row("FileName")).GetValue(0).Item(0), row("FilePath"), CurrentSettings.Override_Path, row("FileName"), True})
                            LogEvent("     Moved File : " + row("FileName"), EventLogLevel.Informational)
                            MovedFilesCount += 1
                        End If
                        If m.Success Then
                            ds.Tables("tblMultiPartFiles").Rows.Add(New Object() {row("FilePath"), CurrentSettings.Override_Path, row("FileName"), CutText.Replace(row("FileName"), "")})
                        End If
                        dvXML.Sort = "AntPath"
                    End If
                End If
            Next row
        End If

        Dim dvOrphanedNonMediaFiles As New DataView
        dvOrphanedNonMediaFiles = ds.Tables("tblOrphanedNonMediaFiles").DefaultView
        dvOrphanedNonMediaFiles.Sort = "FileName"
        dvXML.Sort = "AntPath"

        'Repeat for non-movie types.
        If ds.Tables("tblFoundNonMediaFiles") IsNot Nothing Then
            For Each row In ds.Tables("tblFoundNonMediaFiles").Rows
                m = SplitText.Match(row("FileName"))
                Dim wfile As String = String.Empty
                If CurrentSettings.Store_Short_Names_Only Then
                    wfile = row("FileName")
                    If wfile.Contains("\") Then
                        wfile = wfile.Substring(wfile.LastIndexOf("\") + 1)
                    End If
                Else
                    If CurrentSettings.Override_Path <> "" Then
                        wfile = CurrentSettings.Override_Path + row("FileName")
                    Else
                        wfile = row("FilePath") + row("FileName")
                    End If
                End If
                If dvXML.Find(wfile) = -1 Then
                    If dvOrphanedNonMediaFiles.Find(row("FileName")) = -1 Then
                        dvXML.Sort = "AntShortPath"
                        If dvXML.Find(row("FileName")) = -1 Then
                            ' Shortname not found new file
                            ds.Tables("tblOrphanedNonMediaFiles").Rows.Add(New Object() {0, row("FilePath"), CurrentSettings.Override_Path, row("FileName"), False})
                            'ds.Tables("tblOrphanedNonMediaFiles").Rows.Add(New Object() {row("FilePath"), _OverridePath, row("FileName"), False})
                            '_NextAntID += 1
                            LogEvent("  Orphaned File : " + row("FileName"), EventLogLevel.Informational)
                        Else
                            ' short name found file moved
                            MovedFilesCount += 1
                            ds.Tables("tblOrphanedNonMediaFiles").Rows.Add(New Object() {dvXML.FindRows(row("FileName")).GetValue(0).Item(0), row("FilePath"), CurrentSettings.Override_Path, row("FileName"), True})
                            'ds.Tables("tblOrphanedNonMediaFiles").Rows.Add(New Object() {row("FilePath"), _OverridePath, row("FileName"), True})
                            LogEvent("     Moved File : " + row("FileName"), EventLogLevel.Informational)
                        End If
                        If m.Success Then
                            'ds.Tables("tblMultiPartFiles").Rows.Add(New Object() {_NextAntID, row("FilePath"), _OverridePath, row("FileName"), CutText.Replace(row("FileName"), "")})
                            ds.Tables("tblMultiPartFiles").Rows.Add(New Object() {row("FilePath"), CurrentSettings.Override_Path, row("FileName"), CutText.Replace(row("FileName"), "")})
                        End If
                        dvXML.Sort = "AntPath"
                    End If
                End If
            Next row
        End If

        If CurrentSettings.Folder_Name_Is_Group_Name = True Then
            'Scan through the new tables and check for group names (multiple movies in a folder together)
            Dim FolderNames As New Hashtable
            Dim GroupNames As New Hashtable
            Dim CurrentFoldername As String

            If ds.Tables("tblOrphanedMediaFiles") IsNot Nothing Then
                Dim IsMultiPart As Boolean = False
                For Each row In ds.Tables("tblOrphanedMediaFiles").Rows
                    If row("FileName").ToString.Contains("\") Then
                        CurrentFoldername = GetGroupName(row("FileName"))
                        'If FolderNames.ContainsValue(CurrentFoldername) Then
                        '    'Already there - this is a group name, unless it's a multi-part file:
                        '    m = SplitText.Match(row("FileName"))
                        '    If Not m.Success Then
                        'It's not a multi-part, so it's a group name.  Add it.
                        GroupNames.Add(row("FileName"), CurrentFoldername)
                        'End If
                        '    Else
                        ''Not there - add to foldernames:
                        'FolderNames.Add(row("FileName"), CurrentFoldername)
                        '    End If
        End If
                Next
                'Repeat to catch the missing ones (bad technique but it should work!)
                'For Each row In ds.Tables("tblOrphanedMediaFiles").Rows
                '    If row("FileName").ToString.Contains("\") Then
                '        CurrentFoldername = GetGroupName(row("FileName"))
                '        If GroupNames.ContainsValue(CurrentFoldername) Then
                '            'Make sure this entry is in there too!
                '            If Not GroupNames.ContainsKey(row("FileName")) Then
                '                GroupNames.Add(row("FileName"), CurrentFoldername)
                '            End If
                '        End If
                '    End If
                'Next
                If GroupNames.Count > 0 Then
                    'Need to update the shared datatable with the group names:
                    For Each row In ds.Tables("tblOrphanedMediaFiles").Rows
                        If row("FileName").ToString.Contains("\") Then
                            CurrentFoldername = GetGroupName(row("FileName"))
                            'CurrentFoldername = row("FileName").ToString
                            'CurrentFoldername = CurrentFoldername.Substring(0, CurrentFoldername.IndexOf("\"))
                            If GroupNames.ContainsValue(CurrentFoldername) Then
                                row("GroupName") = CurrentFoldername
                            End If
                        End If
                    Next
                End If
            End If

            FolderNames = New Hashtable
            GroupNames = New Hashtable

            If ds.Tables("tblOrphanedNonMediaFiles") IsNot Nothing Then
                For Each row In ds.Tables("tblOrphanedNonMediaFiles").Rows
                    If row("FileName").ToString.Contains("\") Then
                        CurrentFoldername = GetGroupName(row("FileName"))
                        If FolderNames.ContainsValue(CurrentFoldername) Then
                            'Already there - this is a group name, unless it's a multi-part file:
                            m = SplitText.Match(row("FileName"))
                            If Not m.Success Then
                                'It's not a multi-part, so it's a group name.  Add it.
                                GroupNames.Add(row("FileName"), CurrentFoldername)
                            End If
                        Else
                            'Not there - add to foldernames:
                            FolderNames.Add(row("FileName"), CurrentFoldername)
                        End If
                    End If
                Next
                'Repeat to catch the missing ones (bad technique but it should work!)
                For Each row In ds.Tables("tblOrphanedNonMediaFiles").Rows
                    If row("FileName").ToString.Contains("\") Then
                        CurrentFoldername = GetGroupName(row("FileName"))
                        If GroupNames.ContainsValue(CurrentFoldername) Then
                            'Make sure this entry is in there too!
                            If Not GroupNames.ContainsKey(row("FileName")) Then
                                GroupNames.Add(row("FileName"), CurrentFoldername)
                            End If
                        End If
                    End If
                Next
                'Need to update the shared datatable with the group names:
                If GroupNames.Count > 0 Then
                    For Each row In ds.Tables("tblOrphanedNonMediaFiles").Rows
                        If row("FileName").ToString.Contains("\") Then
                            CurrentFoldername = GetGroupName(row("FileName"))
                            'CurrentFoldername = row("FileName").ToString
                            'CurrentFoldername = CurrentFoldername.Substring(0, CurrentFoldername.IndexOf("\"))
                            If GroupNames.ContainsValue(CurrentFoldername) Then
                                row("GroupName") = CurrentFoldername
                            End If
                        End If
                    Next
                End If
            End If
        End If

        LogEvent("---------------------------------------------------------------------------------------------------", EventLogLevel.Informational)
        LogEvent("Processing Complete - Found " & CountMultiPartFiles.ToString & " Multi Part Files Entries.", EventLogLevel.ImportantEvent)
        LogEvent("Processing Complete - Found " & MovedFilesCount.ToString & " to be Updated Entries.", EventLogLevel.ImportantEvent)
        LogEvent("Processing Complete - Found " & (CountOrphanFiles - MovedFilesCount).ToString & " Missing Entries.", EventLogLevel.ImportantEvent)
        LogEvent("===================================================================================================", EventLogLevel.Informational)

        If CurrentSettings.Purge_Missing_Files = True Then
            ProcessOrphanRecords()
        End If

    End Sub

    Public Sub ProcessOrphanRecords()

        LogEvent("Finding Missing Files (In XML file, not on hard disk)", EventLogLevel.ImportantEvent)

        Dim row As DataRow
        ds.Tables("tblOrphanedAntRecords").Clear()

        Dim dvFoundMediaFiles As New DataView
        dvFoundMediaFiles = ds.Tables("tblFoundMediaFiles").DefaultView
        dvFoundMediaFiles.Sort = "FileName"

        Dim dvFoundNonMediaFiles As New DataView
        dvFoundNonMediaFiles = ds.Tables("tblFoundNonMediaFiles").DefaultView
        dvFoundNonMediaFiles.Sort = "FileName"

        Dim Path() As String
        Dim iTemp As Integer
        Dim strTemp, strTemp2 As String

        For Each row In ds.Tables("tblXML").Rows
            If dvFoundMediaFiles.Find(row("AntShortPath")) = -1 Then
                If dvFoundNonMediaFiles.Find(row("AntShortPath")) = -1 Then
                    ''And if not there, it's probably an orphan - check to see if the path is different:

                    Path = CurrentSettings.Movie_Scan_Path.Split(";")
                    For i As Integer = 0 To Path.Length - 1
                        If CurrentSettings.Override_Path = "" Then
                            strTemp = row("AntPath").ToString.ToLower
                            strTemp2 = Path(i).ToLower
                            iTemp = row("AntPath").ToString.ToLower.IndexOf(Path(i).ToLower)
                            If row("AntPath").ToString.ToLower.IndexOf(Path(i).ToLower) > -1 Then
                                'Match - we're scanning the path that this entry refers to - must be orphaned.
                                ds.Tables("tblOrphanedAntRecords").Rows.Add(New Object() {row("AntID"), row("AntPath"), row("AntShortPath")})
                                LogEvent(" - Orphaned Ant Record : " & row("AntPath"), EventLogLevel.Informational)
                            End If
                        Else
                            'Match - the ant records refers to the location we're using as override path.
                            strTemp = row("AntPath").ToString.ToLower
                            strTemp2 = CurrentSettings.Override_Path.ToLower
                            iTemp = row("AntPath").ToString.ToLower.IndexOf(CurrentSettings.Override_Path.ToLower)
                            If row("AntPath").ToString.ToLower.IndexOf(CurrentSettings.Override_Path.ToLower) > -1 Then
                                'Match - we're scanning the path that this entry refers to - must be orphaned.
                                ds.Tables("tblOrphanedAntRecords").Rows.Add(New Object() {row("AntID"), row("AntPath"), row("AntShortPath")})
                                LogEvent(" - Orphaned Ant Record : " & row("AntPath"), EventLogLevel.Informational)
                            End If
                        End If

                    Next
                End If
            End If
        Next row
        LogEvent("---------------------------------------------------------------------------------------------------", EventLogLevel.Informational)
        LogEvent("Processing Complete - Found " & CountOrphanRecords.ToString & " Missing Files.", EventLogLevel.ImportantEvent)
        LogEvent("===================================================================================================", EventLogLevel.Informational)

    End Sub

    Public Sub UpdateXMLFile()

        If (Not (System.IO.File.Exists(CurrentSettings.Internet_Parser_Path.ToString))) Then
            MsgBox("Error : Cannot find Parser Configuration file : " & CurrentSettings.Internet_Parser_Path.ToString, MsgBoxStyle.Critical, "Missing File")
            SetCheckButtonStatus(ButtonStatus.ReadyToDoImport)
            Exit Sub
        End If
        _OperationCancelled = False

        LogEvent("Starting XML update process.", EventLogLevel.ImportantEvent)
        LogEvent("  FilePath : " + CurrentSettings.XML_File.ToString, EventLogLevel.ImportantEvent)
        LogEvent("  ParserPath : " + CurrentSettings.Internet_Parser_Path.ToString, EventLogLevel.ImportantEvent)
        LogEvent("  ExcludePath : " + CurrentSettings.Excluded_Movies_File.ToString, EventLogLevel.ImportantEvent)
        LogEvent("  MoviePath : " + CurrentSettings.Movie_Scan_Path.ToString, EventLogLevel.ImportantEvent)
        LogEvent("  OverridePath : " + CurrentSettings.Override_Path.ToString, EventLogLevel.ImportantEvent)
        LogEvent("  FanartPath : " + CurrentSettings.Movie_Fanart_Path.ToString, EventLogLevel.ImportantEvent)
        LogEvent("  Store Short Names : " + CurrentSettings.Store_Short_Names_Only.ToString, EventLogLevel.ImportantEvent)
        LogEvent("  OverwriteFlag : " + CurrentSettings.Overwrite_XML_File.ToString, EventLogLevel.ImportantEvent)
        LogEvent("  BackupFlag : " + CurrentSettings.Backup_XML_First.ToString, EventLogLevel.ImportantEvent)
        LogEvent("  MediaType : " + CurrentSettings.Ant_Media_Type.ToString, EventLogLevel.ImportantEvent)
        LogEvent("  MediaLabel : " + CurrentSettings.Ant_Media_Label.ToString, EventLogLevel.ImportantEvent)
        LogEvent("  SourceField : " + CurrentSettings.Ant_Database_Source_Field.ToString, EventLogLevel.ImportantEvent)
        LogEvent("  PurgeMissing : " + CurrentSettings.Purge_Missing_Files.ToString, EventLogLevel.ImportantEvent)
        LogEvent("  Import on Internet Lookup Failure : " + CurrentSettings.Import_File_On_Internet_Lookup_Failure.ToString, EventLogLevel.ImportantEvent)
        LogEvent("  Prohibit Internet Lookup : " + CurrentSettings.Prohibit_Internet_Lookup.ToString, EventLogLevel.ImportantEvent)
        LogEvent("  Use XBMC nfo : " + CurrentSettings.Use_XBMC_nfo.ToString, EventLogLevel.ImportantEvent)
        LogEvent("  Use Page Grabber : " + CurrentSettings.Use_Page_Grabber.ToString, EventLogLevel.ImportantEvent)
        LogEvent("Starting file analysis and import - " & _CountOrphanFiles.ToString & " orphaned files found.", EventLogLevel.ImportantEvent)

        XMLDoc.Load(CurrentSettings.XML_File)

        If _InteractiveMode = True Then
            Form1.ToolStripProgressBar.Minimum = 0
            Form1.ToolStripProgressBar.Maximum = _CountOrphanFiles + _CountOrphanRecords
            Form1.ToolStripProgressBar.Value = 0
            Form1.btnCancelProcessing.Enabled = True
        End If

        ' Clear the multipart Files Processed Table
        ds.Tables("tblMultiPartProcessedFiles").Clear()

        Dim XMLUpdateObject As New XMLUpdateClass
        With XMLUpdateObject
            .xmldoc = XMLDoc
            .ds = ds
            .InteractiveMode = _InteractiveMode
            .objSettings = CurrentSettings
        End With

        Dim DoProcess As Boolean = True

        Dim NewFileName As String

        If CurrentSettings.Backup_XML_First = True Then
            LogEvent("Backing up xml file.", EventLogLevel.ImportantEvent)

            NewFileName = Replace(CurrentSettings.XML_File, ".xml", " - " + My.Computer.Clock.LocalTime.ToString.Replace(":", "-") + ".xml")
            NewFileName = NewFileName.Replace("/", "-")

            Try
                My.Computer.FileSystem.CopyFile(CurrentSettings.XML_File, NewFileName, True)
                LogEvent("Backing up xml file - done.", EventLogLevel.ImportantEvent)
            Catch ex As Exception
                DoProcess = False
                LogEvent("ERROR : Cannot back up xml file : " & ex.Message, EventLogLevel.ErrorOrSimilar)
            End Try
        End If

        'Take an additional backup regardless and store in the application directory.
        ' _TempXMLBackupFile = My.Application.Info.DirectoryPath & "\AntCatalogAutoBackup_" & My.Computer.Clock.LocalTime.ToString.Replace(":", "-") & ".xml"
        _TempXMLBackupFile = MediaPortal.Configuration.Config.Dir.Thumbs & "\MyFilms\AMCupdaterData\AntCatalogAutoBackup_" & My.Computer.Clock.LocalTime.ToString.Replace(":", "-") & ".xml"
        _TempXMLBackupFile = _TempXMLBackupFile.Replace("/", "-")
        My.Computer.FileSystem.CopyFile(CurrentSettings.XML_File, _TempXMLBackupFile)

        If DoProcess = True Then
            LogEvent("---------------------------------------------------------------------------------------------------", EventLogLevel.Informational)
            LogEvent("Processing Files.", EventLogLevel.ImportantEvent)
            If (_InteractiveMode) Then
                bgwFolderScanUpdate.RunWorkerAsync(XMLUpdateObject)
                'XMLUpdateObject.UpdateXML()
                'UpdateXMLFile_PostProcessing()
            Else
                XMLUpdateObject.UpdateXML()
                UpdateXMLFile_PostProcessing()
            End If
            LogEvent("===================================================================================================", EventLogLevel.Informational)
        End If

    End Sub

    Private Shared Sub bgwFolderScanUpdate_ProgressChanged(ByVal sender As Object, ByVal e As ProgressChangedEventArgs) Handles bgwFolderScanUpdate.ProgressChanged

        If _InteractiveMode = True Then
            Form1.ToolStripProgressBar.Value += 1
            Form1.ToolStripProgressMessage.Text = "status: " & Form1.ToolStripProgressBar.Value.ToString & " of " & Form1.ToolStripProgressBar.Maximum.ToString & " total movie(s)"
        End If

        'The e.UserState includes messages from the background process.  In this case, it's the 'File Imported - Filename' message.
        LogEvent(e.UserState, EventLogLevel.Informational)

    End Sub

    Private Shared Sub bgwFolderScanUpdate_DoWork(ByVal sender As Object, ByVal e As System.ComponentModel.DoWorkEventArgs) Handles bgwFolderScanUpdate.DoWork

        Dim XMLUpdateObject As XMLUpdateClass = CType(e.Argument, XMLUpdateClass)
        ' Return the value through the Result property.
        'Try-Catch removed - causes problems as this section is still within the 2nd thread and can't log properly to the window.  OH 07Sep2008.
        'Try
        e.Result = XMLUpdateObject.UpdateXML()
        'Catch ex As Exception
        'LogEvent("ERROR : " & ex.Message, EventLogLevel.ErrorOrSimilar)
        'End Try

    End Sub

    Public Sub bgwFolderScanUpdate_Cancel()
        bgwFolderScanUpdate.CancelAsync()
        If _InteractiveMode = True Then
            Form1.btnCancelProcessing.Enabled = False
        End If
        _OperationCancelled = True
        LogEvent("Halting Operation by user request - Please Wait.", EventLogLevel.ErrorOrSimilar)
    End Sub

    Private Class XMLUpdateClass
        Public xmldoc As XmlDocument
        Public ds As DataSet
        Public InteractiveMode As Boolean
        Public objSettings As AntSettings

        Function UpdateXML() As Integer

            Dim FilePath As String
            Dim ReplacementPath As String
            Dim FileName As String
            Dim ShortName As String
            Dim CutText As New Regex(objSettings.RegEx_Check_For_MultiPart_Files)
            Dim NewAntID As Integer = GetNextMovieID(objSettings.XML_File)
            Dim ImagePath As String = String.Empty
            Dim XMLPath As String = objSettings.XML_File.Substring(0, objSettings.XML_File.LastIndexOf("\"))
            Dim ImagePrefix As String = objSettings.Image_Download_Filename_Prefix.ToString
            ImagePath = XMLPath
            If objSettings.Use_Folder_Dot_Jpg = False And ImagePrefix.ToString <> String.Empty Then
                If ImagePrefix.ToString.Contains("\") = True Then
                    'Example : "foldername\" or "foldername\prefix_"
                    Dim PictureFolder As String = ImagePrefix.Substring(0, ImagePrefix.IndexOf("\"))
                    ImagePath = ImagePath.ToString & "\" & PictureFolder.ToString
                    If Not My.Computer.FileSystem.DirectoryExists(ImagePath) Then
                        My.Computer.FileSystem.CreateDirectory(ImagePath)
                    End If
                End If
            End If

            Dim MovieRootNode As Xml.XmlNode
            MovieRootNode = xmldoc.SelectSingleNode("//AntMovieCatalog/Catalog/Contents")
            Dim row As DataRow

            Dim wtime As String
            wtime = My.Computer.Clock.LocalTime.Year.ToString() + My.Computer.Clock.LocalTime.Month.ToString() + My.Computer.Clock.LocalTime.Day.ToString() + "-" + My.Computer.Clock.LocalTime.Hour.ToString() + My.Computer.Clock.LocalTime.Minute.ToString()

            Dim f As New IO.FileInfo(objSettings.Internet_Parser_Path)
            If Not (Directory.Exists(f.DirectoryName + "\log")) Then
                Directory.CreateDirectory(f.DirectoryName + "\log")
            End If

            Dim dvMultiPartFiles As New DataView
            dvMultiPartFiles = ds.Tables("tblMultiPartFiles").DefaultView
            'dvMultiPartFiles.
            dvMultiPartFiles.Sort = "FileName"

            Dim dvMultiPartProcessedFiles As New DataView
            dvMultiPartProcessedFiles = ds.Tables("tblMultiPartProcessedFiles").DefaultView
            dvMultiPartProcessedFiles.Sort = "FileName"

            Dim AllFilesPath As String = String.Empty
            If ds.Tables("tblOrphanedMediaFiles") IsNot Nothing Then
                For Each row In ds.Tables("tblOrphanedMediaFiles").Rows
                    If bgwFolderScanUpdate.CancellationPending = True Then
                        Exit Function
                    End If

                    If dvMultiPartProcessedFiles.Find(row("FileName")) = -1 Then
                        FileName = row("FileName")
                        FilePath = row("PhysicalPath") & FileName
                        ShortName = CutText.Replace(row("FileName"), "")
                        AllFilesPath = ""
                        ReplacementPath = ""

                        ' search if it's a part of a multipart movie. In that case Replacement contains all movie files and AllFilesPath complete path for all movies
                        If (dvMultiPartFiles.Find(row("FileName")) <> -1) Then
                            For Each row2 As DataRow In ds.Tables("tblMultiPartFiles").Rows
                                If (row2("ShortName") = ShortName) Then
                                    ds.Tables("tblMultiPartProcessedFiles").Rows.Add(row2("FileName"))
                                    Dim wpath As String = String.Empty
                                    If (objSettings.Store_Short_Names_Only = True) Then
                                        wpath = row2("FileName")
                                        If wpath.Contains("\") Then
                                            wpath = wpath.Substring(wpath.LastIndexOf("\") + 1)
                                        End If
                                    Else
                                        If row2("VirtualPath") = "" Then
                                            wpath = row2("PhysicalPath") & row2("FileName")
                                        Else
                                            wpath = row2("VirtualPath") & row2("FileName")
                                        End If
                                    End If
                                    If ReplacementPath.Length = 0 Then
                                        ReplacementPath = wpath
                                    Else
                                        ReplacementPath = ReplacementPath + ";" + wpath
                                    End If
                                    If AllFilesPath.Length = 0 Then
                                        AllFilesPath = FilePath
                                    Else
                                        AllFilesPath = AllFilesPath + ";" + FilePath
                                    End If
                                End If
                            Next
                        Else
                            'Not a multi-part entry, so just set the ReplacementPath value and continue:
                            If (objSettings.Store_Short_Names_Only = True) Then
                                ReplacementPath = FileName
                                If ReplacementPath.Contains("\") Then
                                    ReplacementPath = ReplacementPath.Substring(ReplacementPath.LastIndexOf("\") + 1)
                                End If
                            Else
                                If row("VirtualPath") = "" Then
                                    ReplacementPath = FilePath
                                Else
                                    ReplacementPath = row("VirtualPath") & FileName
                                End If
                            End If
                        End If

                        Dim Ant As New AntRecord()
                        With Ant
                            .FileName = FileName
                            .FilePath = FilePath
                            .AllFilesPath = AllFilesPath
                            .MediaLabel = objSettings.Ant_Media_Label
                            .MediaType = objSettings.Ant_Media_Type
                            .SourceField = objSettings.Ant_Database_Source_Field
                            .OverridePath = ReplacementPath
                            .MovieNumber = NewAntID
                            .XMLDoc = xmldoc
                            .ParserPath = objSettings.Internet_Parser_Path
                            .InteractiveMode = InteractiveMode
                            .ExcludeFile = objSettings.Excluded_Movies_File
                            '.ImagePath = objSettings.XML_File.Substring(0, objSettings.XML_File.LastIndexOf("\"))
                            .ImagePath = ImagePath
                            .InternetLookupAlwaysPrompt = objSettings.Internet_Lookup_Always_Prompt
                            .DateHandling = objSettings.Date_Handling
                            .Read_DVD_Label = objSettings.Read_DVD_Label
                            .XMLFilePath = objSettings.XML_File
                            .MovieTitleHandling = objSettings.Movie_Title_Handling
                            .GroupName = row("GroupName").ToString
                        End With

                        If (row("Moved")) Then
                            Ant.MovieNumber = row("AntID")
                            ' filename exist but with wrong path (may be moved. Entry don't be created but updated    
                            Ant.UpdateElement()
                            Ant.ProhibitInternetLookup = True
                            Ant.ProcessFile(AntRecord.Process_Mode_Names.Import)
                            Ant.SaveProgress()
                            If Ant.LastOutputMessage.StartsWith("ERROR") = True Then
                                bgwFolderScanUpdate.ReportProgress(_CountRecordsAdded, Ant.LastOutputMessage)
                            Else
                                bgwFolderScanUpdate.ReportProgress(_CountRecordsAdded, " File  Updated - " & ReplacementPath)
                            End If
                        Else
                            'need to create a new entry:
                            Ant.CreateElement()
                            Ant.ProcessFile(AntRecord.Process_Mode_Names.Import)
                            Ant.SaveProgress()

                            If Ant.LastOutputMessage.StartsWith("ERROR") = True Then
                                bgwFolderScanUpdate.ReportProgress(_CountRecordsAdded, Ant.LastOutputMessage)
                                'LogEvent("ERROR : " & blah.LastOutputMessage, EventLogLevel.ErrorOrSimilar)
                            Else
                                If CurrentSettings.Import_File_On_Internet_Lookup_Failure = True Then
                                    'Doesn't matter if the Internet loookup worked; just load the entry:
                                    MovieRootNode.AppendChild(Ant.XMLElement)
                                    _CountRecordsAdded += 1
                                    Dim OutputMessage As String = String.Empty
                                    If ReplacementPath.IndexOf(";") >= 0 Then
                                        OutputMessage = " Files Imported - " & ReplacementPath
                                    Else
                                        OutputMessage = " File  Imported - " & ReplacementPath
                                    End If
                                    If Ant.InternetLookupOK = False Then
                                        OutputMessage += " (Internet Lookup Failed)"
                                    End If
                                    bgwFolderScanUpdate.ReportProgress(_CountRecordsAdded, OutputMessage)
                                    NewAntID += 1
                                Else
                                    'First check if the Internet Lookup works:
                                    If Ant.InternetLookupOK = True Then
                                        MovieRootNode.AppendChild(Ant.XMLElement)
                                        _CountRecordsAdded += 1
                                        If ReplacementPath.IndexOf(";") >= 0 Then
                                            bgwFolderScanUpdate.ReportProgress(_CountRecordsAdded, " Files Imported - " & ReplacementPath)
                                        Else
                                            bgwFolderScanUpdate.ReportProgress(_CountRecordsAdded, " File  Imported - " & ReplacementPath)
                                        End If
                                        NewAntID += 1
                                    Else
                                        'Mark as Ignored - do not import.
                                        If ReplacementPath.IndexOf(";") >= 0 Then
                                            bgwFolderScanUpdate.ReportProgress(_CountRecordsAdded, " Files Ignored - **********  " & ReplacementPath & "  *****")
                                        Else
                                            bgwFolderScanUpdate.ReportProgress(_CountRecordsAdded, " File  Ignored - **********  " & ReplacementPath & "  *****")
                                        End If
                                    End If
                                End If
                            End If
                        End If
                    End If
                Next row
            End If



            If ds.Tables("tblOrphanedNonMediaFiles") IsNot Nothing Then
                For Each row In ds.Tables("tblOrphanedNonMediaFiles").Rows

                    If bgwFolderScanUpdate.CancellationPending = True Then
                        Exit Function
                    End If
                    If dvMultiPartProcessedFiles.Find(row("FileName")) = -1 Then
                        FileName = row("FileName")
                        FilePath = row("PhysicalPath") & FileName
                        ShortName = CutText.Replace(row("FileName"), "")
                        AllFilesPath = ""
                        ReplacementPath = ""
                        ' search if it's a part of a multipart movie. In that case Replacement contains all movie files and AllFilesPath complete path for all movies
                        If (dvMultiPartFiles.Find(row("FileName")) <> -1) Then
                            For Each row2 As DataRow In ds.Tables("tblMultiPartFiles").Rows
                                If (row2("ShortName") = ShortName) Then
                                    ds.Tables("tblMultiPartProcessedFiles").Rows.Add(row2("FileName"))
                                    Dim wpath As String = String.Empty
                                    If (objSettings.Store_Short_Names_Only = True) Then
                                        wpath = row2("FileName")
                                        If wpath.Contains("\") Then
                                            wpath = wpath.Substring(wpath.LastIndexOf("\") + 1)
                                        End If
                                    Else
                                        If row("VirtualPath") = "" Then
                                            wpath = row2("PhysicalPath") & row2("FileName")
                                        Else
                                            wpath = row2("VirtualPath") & row2("FileName")
                                        End If
                                    End If
                                    If ReplacementPath.Length = 0 Then
                                        ReplacementPath = wpath
                                    Else
                                        ReplacementPath = ReplacementPath + ";" + wpath
                                    End If
                                    If AllFilesPath.Length = 0 Then
                                        AllFilesPath = FilePath
                                    Else
                                        AllFilesPath = AllFilesPath + ";" + FilePath
                                    End If
                                End If
                            Next
                        Else
                            If (objSettings.Store_Short_Names_Only = True) Then
                                ReplacementPath = FileName
                                If ReplacementPath.Contains("\") Then
                                    ReplacementPath = ReplacementPath.Substring(ReplacementPath.LastIndexOf("\") + 1)
                                End If
                            Else
                                If row("VirtualPath") = "" Then
                                    ReplacementPath = FilePath
                                Else
                                    ReplacementPath = row("VirtualPath") & FileName
                                End If
                            End If
                        End If

                        Dim Ant As New AntRecord()
                        With Ant
                            .FileName = FileName
                            .FilePath = FilePath
                            .AllFilesPath = AllFilesPath
                            .MediaLabel = objSettings.Ant_Media_Label
                            .MediaType = objSettings.Ant_Media_Type
                            .SourceField = objSettings.Ant_Database_Source_Field
                            .OverridePath = ReplacementPath
                            .MovieNumber = NewAntID
                            .XMLDoc = xmldoc
                            .ParserPath = objSettings.Internet_Parser_Path
                            .InteractiveMode = InteractiveMode
                            .ExcludeFile = objSettings.Excluded_Movies_File
                            '.ImagePath = objSettings.XML_File.Substring(0, objSettings.XML_File.LastIndexOf("\"))
                            .ImagePath = ImagePath
                            .InternetLookupAlwaysPrompt = objSettings.Internet_Lookup_Always_Prompt
                            .DateHandling = objSettings.Date_Handling
                            .Read_DVD_Label = objSettings.Read_DVD_Label
                            .XMLFilePath = objSettings.XML_File
                            .MovieTitleHandling = objSettings.Movie_Title_Handling
                            .GroupName = row("GroupName").ToString
                        End With
                        If (row("Moved")) Then
                            ' filename exist but with wrong path (maybe moved. Entry don't be created but updated  
                            Ant.UpdateElement()
                            Ant.ProhibitInternetLookup = True
                            Ant.ProcessFile(AntRecord.Process_Mode_Names.Import)
                            Ant.SaveProgress()
                            If Ant.LastOutputMessage.StartsWith("ERROR") = True Then
                                bgwFolderScanUpdate.ReportProgress(_CountRecordsAdded, Ant.LastOutputMessage)
                            Else
                                bgwFolderScanUpdate.ReportProgress(_CountRecordsAdded, " File  Updated - " & ReplacementPath)
                            End If
                        Else
                            'Try
                            Ant.CreateElement()
                            Ant.ProcessFile(AntRecord.Process_Mode_Names.Import)
                            Ant.SaveProgress()
                            'Catch ex As Exception
                            'LogEvent("ERROR : " & ex.Message.ToString, EventLogLevel.ErrorOrSimilar)
                            'End Try


                            If Ant.LastOutputMessage.StartsWith("ERROR") = True Then
                                bgwFolderScanUpdate.ReportProgress(_CountRecordsAdded, Ant.LastOutputMessage)
                                'LogEvent("ERROR : " & blah.LastOutputMessage, EventLogLevel.ErrorOrSimilar)
                            Else
                                'Check to see whether to save record:
                                If CurrentSettings.Import_File_On_Internet_Lookup_Failure = True Then
                                    'Doesn't matter if the Internet loookup worked; just load the entry:
                                    MovieRootNode.AppendChild(Ant.XMLElement)
                                    _CountRecordsAdded += 1
                                    If ReplacementPath.IndexOf(";") >= 0 Then
                                        bgwFolderScanUpdate.ReportProgress(_CountRecordsAdded, " Files Imported - " & ReplacementPath)
                                    Else
                                        bgwFolderScanUpdate.ReportProgress(_CountRecordsAdded, " File  Imported - " & ReplacementPath)
                                    End If
                                    NewAntID += 1
                                Else
                                    'First check if the Internet Lookup works:
                                    If Ant.InternetLookupOK = True Then
                                        MovieRootNode.AppendChild(Ant.XMLElement)
                                        _CountRecordsAdded += 1
                                        If ReplacementPath.IndexOf(";") >= 0 Then
                                            bgwFolderScanUpdate.ReportProgress(_CountRecordsAdded, " Files Imported - " & ReplacementPath)
                                        Else
                                            bgwFolderScanUpdate.ReportProgress(_CountRecordsAdded, " File  Imported - " & ReplacementPath)
                                        End If
                                        NewAntID += 1
                                    Else
                                        'Mark as Ignored - do not import.
                                        If ReplacementPath.IndexOf(";") >= 0 Then
                                            bgwFolderScanUpdate.ReportProgress(_CountRecordsAdded, " Files Ignored - **********  " & ReplacementPath & "  *****")
                                        Else
                                            bgwFolderScanUpdate.ReportProgress(_CountRecordsAdded, " File  Ignored - **********  " & ReplacementPath & "  *****")
                                        End If
                                    End If
                                End If
                            End If
                        End If
                    End If
                Next row
            End If
        End Function
    End Class

    Private Shared Sub bgwFolderScanUpdate_RunWorkerCompleted(ByVal sender As Object, ByVal e As System.ComponentModel.RunWorkerCompletedEventArgs) Handles bgwFolderScanUpdate.RunWorkerCompleted
        'Now new XML document is in memory; and we have an arraylist of movies with multiple parts:

        If e.Error IsNot Nothing Then
            'If e.Error.Message.ToString <> "" Then
            LogEvent("ERROR : " & e.Error.Message.ToLower, EventLogLevel.ErrorOrSimilar)
            'End If
        End If

        'Check to see if the cancel button was clicked; if so prompt on how to continue
        If _OperationCancelled = True Then
            If MsgBox("Save work done so far?", MsgBoxStyle.YesNo) = MsgBoxResult.No Then
                LogEvent("Operation Cancelled.", EventLogLevel.ErrorOrSimilar)
                'If _InteractiveMode = True Then
                Form1.ToolStripProgressBar.Value = Form1.ToolStripProgressBar.Maximum
                Form1.btnCancelProcessing.Enabled = False
                SetCheckButtonStatus(ButtonStatus.ReadyToParseXML)
                Try
                    My.Computer.FileSystem.CopyFile(_TempXMLBackupFile, CurrentSettings.XML_File, True)
                    My.Computer.FileSystem.DeleteFile(_TempXMLBackupFile, FileIO.UIOption.OnlyErrorDialogs, FileIO.RecycleOption.SendToRecycleBin)
                Catch ex As Exception

                End Try
                Exit Sub
            Else
                LogEvent("Operation Cancelled - Save Continuing", EventLogLevel.ErrorOrSimilar)
            End If
        End If


        UpdateXMLFile_PostProcessing()

    End Sub

    Private Shared Sub UpdateXMLFile_PostProcessing()
        LogEvent("---------------------------------------------------------------------------------------------------", EventLogLevel.Informational)
        LogEvent("File Import Complete - " & _CountRecordsAdded.ToString & " files loaded.", EventLogLevel.ImportantEvent)
        LogEvent("===================================================================================================", EventLogLevel.Informational)

        'Purge database entries for missing files (optional)
        If CurrentSettings.Purge_Missing_Files = True Then
            Dim MovieRootNode As Xml.XmlNode = XMLDoc.SelectSingleNode("//AntMovieCatalog/Catalog/Contents")
            Dim NodeToDelete As Xml.XmlNode
            Dim row As DataRow

            LogEvent("Removing Missing Files from Database", EventLogLevel.ImportantEvent)

            For Each row In ds.Tables("tblOrphanedAntRecords").Rows

                NodeToDelete = XMLDoc.SelectSingleNode("//AntMovieCatalog/Catalog/Contents/Movie[@Number='" & row("AntID") & "']")
                If Not NodeToDelete Is Nothing Then
                    _CountRecordsDeleted += 1
                    MovieRootNode.RemoveChild(NodeToDelete)
                    LogEvent(" Entry Removed : " & row("FileName"), EventLogLevel.Informational)
                End If
            Next row
        End If


        LogEvent("Writing changes to XML file", EventLogLevel.ImportantEvent)
        LogEvent("===================================================================================================", EventLogLevel.Informational)

        Try
            If CurrentSettings.Overwrite_XML_File = True Then
                XMLDoc.Save(CurrentSettings.XML_File)
            Else
                XMLDoc.Save(CurrentSettings.XML_File.Replace(".xml", "-NEW.xml"))
            End If
            My.Computer.FileSystem.DeleteFile(_TempXMLBackupFile, FileIO.UIOption.OnlyErrorDialogs, FileIO.RecycleOption.SendToRecycleBin)
        Catch ex As Exception
            LogEvent("ERROR SAVING CHANGES TO XML FILE.", EventLogLevel.ErrorOrSimilar)
        End Try


        LogEvent("XML update process complete.", EventLogLevel.ImportantEvent)
        LogEvent("===================================================================================================", EventLogLevel.Informational)

        If _InteractiveMode = True Then
            SetCheckButtonStatus(ButtonStatus.ReadyToParseXML)
            Form1.ToolStripProgressBar.Value = Form1.ToolStripProgressBar.Maximum
            Form1.btnCancelProcessing.Enabled = False

            If CurrentSettings.Execute_Program = False Or CurrentSettings.Execute_Program_Path = "" Then
                MsgBox("Process Complete", MsgBoxStyle.Information)
            ElseIf CurrentSettings.Execute_Program = True And CurrentSettings.Execute_Program_Path <> "" Then
                If CurrentSettings.Execute_Only_For_Orphans = False Then
                    Try
                        System.Diagnostics.Process.Start(CurrentSettings.Execute_Program_Path)
                    Catch ex As Exception
                    End Try
                Else
                    If _CountRecordsAdded + _CountRecordsDeleted > 0 Then
                        Try
                            System.Diagnostics.Process.Start(CurrentSettings.Execute_Program_Path)
                        Catch ex As Exception
                        End Try
                    End If
                End If
            End If
        Else
            If CurrentSettings.Execute_Program = True And CurrentSettings.Execute_Program_Path <> "" Then
                If CurrentSettings.Execute_Only_For_Orphans = False Then
                    Try
                        System.Diagnostics.Process.Start(CurrentSettings.Execute_Program_Path)
                    Catch ex As Exception

                    End Try
                Else
                    If _CountRecordsAdded + _CountRecordsDeleted > 0 Then
                        Try
                            System.Diagnostics.Process.Start(CurrentSettings.Execute_Program_Path)
                        Catch ex As Exception

                        End Try
                    End If
                End If
            End If
        End If
    End Sub

#End Region




    Public Sub Reset()
        If ds.Tables("tblXML") IsNot Nothing Then
            ds.Tables("tblXML").Clear()
        End If
        If ds.Tables("tblFoundMediaFiles") IsNot Nothing Then
            ds.Tables("tblFoundMediaFiles").Clear()
        End If
        If ds.Tables("tblFoundNonMediaFiles") IsNot Nothing Then
            ds.Tables("tblFoundNonMediaFiles").Clear()
        End If
        If ds.Tables("tblOrphanedMediaFiles") IsNot Nothing Then
            ds.Tables("tblOrphanedMediaFiles").Clear()
        End If
        If ds.Tables("tblOrphanedNonMediaFiles") IsNot Nothing Then
            ds.Tables("tblOrphanedNonMediaFiles").Clear()
        End If
        If ds.Tables("tblOrphanedAntRecords") IsNot Nothing Then
            ds.Tables("tblOrphanedAntRecords").Clear()
        End If
        If ds.Tables("tblMultiPartFiles") IsNot Nothing Then
            ds.Tables("tblMultiPartFiles").Clear()
        End If
    End Sub

    Public Sub TEST_ListTable(ByVal TableName As String)
        Dim ColumnNames As String = String.Empty
        Dim RowText As String = String.Empty

        If ds.Tables(TableName) IsNot Nothing Then
            LogEvent(TableName & " : " & ds.Tables(TableName).Rows.Count.ToString & " rows.", EventLogLevel.ImportantEvent)
            If ds.Tables(TableName).Rows.Count > 0 Then
                For Each col As DataColumn In ds.Tables(TableName).Columns
                    ColumnNames += col.ColumnName & " | "
                Next
                LogEvent(ColumnNames.Substring(0, ColumnNames.Length - 1), EventLogLevel.ImportantEvent)
                For Each row As DataRow In ds.Tables(TableName).Rows
                    For i As Integer = 0 To ds.Tables(TableName).Columns.Count - 1
                        RowText += row(i).ToString & " | "
                    Next
                    LogEvent(RowText.Substring(0, RowText.Length - 1), EventLogLevel.ImportantEvent)
                    RowText = String.Empty
                Next
                LogEvent("End of table dump", EventLogLevel.ImportantEvent)
            End If
        End If
    End Sub

    Public Shared Sub SearchFile(ByRef FileToScan As String, ByVal FoldersToScan As String, ByVal Number As String, ByRef wtitle As String)
        'NOTE : cannot call LogEvent from here as this will be running on a separate thread and cannot check the form1.visible property!
        Try
            If IO.File.Exists(FileToScan) Then
                Return
            End If
        Catch ex As Exception
            'LogEvent("File ignored (invalid filename): " & Number & " | " & FileToScan, EventLogLevel.Informational)
            FileToScan = String.Empty
            Return
        End Try

        If (FileToScan.IndexOf("\\") > 0 Or FileToScan.IndexOf("//") > 0 Or FileToScan.IndexOf(":") > 0) Then
            ' File Moved, need to update the xml first
            'LogEvent("File Moved, Please Update the XML first : " & Number & " | " & FileToScan, EventLogLevel.ImportantEvent)
            FileToScan = String.Empty
            Return
        End If
        Dim subfolder As String = String.Empty
        If (wtitle.IndexOf("\") > 0) Then
            subfolder = wtitle.Substring(0, wtitle.LastIndexOf("\"))
        End If
        Dim FolderList() As String
        FolderList = FoldersToScan.Split(";")
        Dim myEnumerator As System.Collections.IEnumerator = FolderList.GetEnumerator()
        While myEnumerator.MoveNext()
            For Each folder As String In FolderList
                If IO.File.Exists(folder + FileToScan) Then
                    FileToScan = folder + FileToScan
                    Return
                Else
                    If IO.File.Exists(folder + subfolder + "\" + FileToScan) Then
                        FileToScan = folder + subfolder + "\" + FileToScan
                        Return
                    End If
                End If
                Dim dirInfo As New DirectoryInfo(folder)
                Dim filelist As List(Of FileInfo) = Grabber.GrabUtil.GetFilesRecursive(dirInfo)
                For Each file As FileInfo In filelist
                    If (file.Name = FileToScan) Then
                        FileToScan = file.FullName
                        wtitle = file.FullName.Substring(folder.Length)
                        Return
                    End If
                Next

            Next
        End While
        FileToScan = String.Empty
    End Sub

    Private Sub BuildTables()

        Dim table As DataTable
        Dim column As DataColumn
        ds = New DataSet

        'XML Table
        table = New DataTable("tblXML")
        column = New DataColumn()
        column.DataType = System.Type.GetType("System.Int32")
        column.ColumnName = "AntID"
        table.Columns.Add(column)
        column = New DataColumn()
        column.DataType = System.Type.GetType("System.String")
        column.ColumnName = "AntPath"
        table.Columns.Add(column)
        column = New DataColumn()
        column.DataType = System.Type.GetType("System.String")
        column.ColumnName = "AntShortPath"
        table.Columns.Add(column)
        table.CaseSensitive = False
        ds.Tables.Add(table)

        'Found Media Files Table
        table = New DataTable("tblFoundMediaFiles")
        column = New DataColumn()
        column.DataType = System.Type.GetType("System.String")
        column.ColumnName = "FileName"
        table.Columns.Add(column)
        column = New DataColumn()
        column.DataType = System.Type.GetType("System.String")
        column.ColumnName = "FilePath"
        table.Columns.Add(column)
        table.CaseSensitive = False
        ds.Tables.Add(table)

        'Found Non-Media Files Table
        table = New DataTable("tblFoundNonMediaFiles")
        column = New DataColumn()
        column.DataType = System.Type.GetType("System.String")
        column.ColumnName = "FileName"
        table.Columns.Add(column)
        column = New DataColumn()
        column.DataType = System.Type.GetType("System.String")
        column.ColumnName = "FilePath"
        table.Columns.Add(column)
        table.CaseSensitive = False
        ds.Tables.Add(table)

        table = New DataTable("tblOrphanedMediaFiles")
        column = New DataColumn()
        column.DataType = System.Type.GetType("System.Int32")
        column.ColumnName = "AntID"
        table.Columns.Add(column)
        column = New DataColumn()
        column.DataType = System.Type.GetType("System.String")
        column.ColumnName = "PhysicalPath"
        table.Columns.Add(column)
        column = New DataColumn()
        column.DataType = System.Type.GetType("System.String")
        column.ColumnName = "VirtualPath"
        table.Columns.Add(column)
        column = New DataColumn()
        column.DataType = System.Type.GetType("System.String")
        column.ColumnName = "FileName"
        table.Columns.Add(column)
        column = New DataColumn()
        column.DataType = System.Type.GetType("System.Boolean")
        column.ColumnName = "Moved"
        table.Columns.Add(column)
        column = New DataColumn()
        column.DataType = System.Type.GetType("System.String")
        column.ColumnName = "GroupName"
        table.Columns.Add(column)
        table.CaseSensitive = False
        ds.Tables.Add(table)

        table = New DataTable("tblOrphanedNonMediaFiles")
        column = New DataColumn()
        column.DataType = System.Type.GetType("System.Int32")
        column.ColumnName = "AntID"
        table.Columns.Add(column)
        column = New DataColumn()
        column.DataType = System.Type.GetType("System.String")
        column.ColumnName = "PhysicalPath"
        table.Columns.Add(column)
        column = New DataColumn()
        column.DataType = System.Type.GetType("System.String")
        column.ColumnName = "VirtualPath"
        table.Columns.Add(column)
        column = New DataColumn()
        column.DataType = System.Type.GetType("System.String")
        column.ColumnName = "FileName"
        table.Columns.Add(column)
        column = New DataColumn()
        column.DataType = System.Type.GetType("System.Boolean")
        column.ColumnName = "Moved"
        table.Columns.Add(column)
        column = New DataColumn()
        column.DataType = System.Type.GetType("System.String")
        column.ColumnName = "GroupName"
        table.Columns.Add(column)
        table.CaseSensitive = False
        ds.Tables.Add(table)

        table = New DataTable("tblOrphanedAntRecords")
        column = New DataColumn()
        column.DataType = System.Type.GetType("System.Int32")
        column.ColumnName = "AntID"
        table.Columns.Add(column)
        column = New DataColumn()
        column.DataType = System.Type.GetType("System.String")
        column.ColumnName = "PhysicalPath"
        table.Columns.Add(column)
        column = New DataColumn()
        column.DataType = System.Type.GetType("System.String")
        column.ColumnName = "FileName"
        table.Columns.Add(column)
        table.CaseSensitive = False
        ds.Tables.Add(table)

        table = New DataTable("tblMultiPartFiles")
        'column = New DataColumn()
        'column.DataType = System.Type.GetType("System.Int32")
        'column.ColumnName = "AntID"
        'table.Columns.Add(column)
        column = New DataColumn()
        column.DataType = System.Type.GetType("System.String")
        column.ColumnName = "PhysicalPath"
        table.Columns.Add(column)
        column = New DataColumn()
        column.DataType = System.Type.GetType("System.String")
        column.ColumnName = "VirtualPath"
        table.Columns.Add(column)
        column = New DataColumn()
        column.DataType = System.Type.GetType("System.String")
        column.ColumnName = "FileName"
        table.Columns.Add(column)
        column = New DataColumn()
        column.DataType = System.Type.GetType("System.String")
        column.ColumnName = "ShortName"
        table.Columns.Add(column)
        table.CaseSensitive = False
        ds.Tables.Add(table)

        table = New DataTable("tblMultiPartProcessedFiles")
        column = New DataColumn()
        column.DataType = System.Type.GetType("System.String")
        column.ColumnName = "FileName"
        table.Columns.Add(column)
        table.CaseSensitive = False
        ds.Tables.Add(table)

        table = New DataTable("tblAntFields")
        column = New DataColumn()
        column.DataType = System.Type.GetType("System.String")
        column.ColumnName = "FieldName"
        table.Columns.Add(column)
        column = New DataColumn()
        column.DataType = System.Type.GetType("System.String")
        column.ColumnName = "FieldDataType"
        table.Columns.Add(column)
        table.CaseSensitive = False
        ds.Tables.Add(table)

        'XML Table
        table = New DataTable("tblNodesToProcess")
        column = New DataColumn()
        column.DataType = System.Type.GetType("System.Int32")
        column.ColumnName = "AntID"
        table.Columns.Add(column)
        column = New DataColumn()
        column.DataType = System.Type.GetType("System.String")
        column.ColumnName = "AntTitle"
        table.Columns.Add(column)
        column = New DataColumn()
        column.DataType = System.Type.GetType("System.String")
        column.ColumnName = "AntDirector"
        table.Columns.Add(column)
        ' Guzzi added: Year for better matching
        column = New DataColumn()
        column.DataType = System.Type.GetType("System.String")
        column.ColumnName = "AntYear"
        table.Columns.Add(column)
        column = New DataColumn()
        column.DataType = System.Type.GetType("System.String")
        column.ColumnName = "AntIMDB_Id"
        table.Columns.Add(column)
        table.CaseSensitive = False
        ds.Tables.Add(table)

        table = New DataTable("dtNone")
        table.Columns.Add(New DataColumn("FileName"))
        table.CaseSensitive = False
        ds.Tables.Add(table)

        'table = New DataTable("dtDone")
        'table.Columns.Add(New DataColumn("FileName"))
        'table.CaseSensitive = False
        'ds.Tables.Add(table)

        PopulateReferenceTables()

    End Sub

    Private Sub PopulateReferenceTables()
        If ds.Tables("tblAntFields") IsNot Nothing Then
            ds.Tables("tblAntFields").Rows.Add(New Object() {"Number", "Int"})
            ds.Tables("tblAntFields").Rows.Add(New Object() {"Date", "Date"})
            ds.Tables("tblAntFields").Rows.Add(New Object() {"Rating", "Int"})
            ds.Tables("tblAntFields").Rows.Add(New Object() {"Year", "Int"})
            ds.Tables("tblAntFields").Rows.Add(New Object() {"Length", "Int"})
            ds.Tables("tblAntFields").Rows.Add(New Object() {"VideoBitrate", "Int"})
            ds.Tables("tblAntFields").Rows.Add(New Object() {"AudioBitrate", "Int"})
            ds.Tables("tblAntFields").Rows.Add(New Object() {"Disks", "Int"})
            ds.Tables("tblAntFields").Rows.Add(New Object() {"Checked", "Boolean"})
            ds.Tables("tblAntFields").Rows.Add(New Object() {"MediaLabel", "String"})
            ds.Tables("tblAntFields").Rows.Add(New Object() {"MediaType", "String"})
            ds.Tables("tblAntFields").Rows.Add(New Object() {"Source", "String"})
            ds.Tables("tblAntFields").Rows.Add(New Object() {"Borrower", "String"})
            ds.Tables("tblAntFields").Rows.Add(New Object() {"OriginalTitle", "String"})
            ds.Tables("tblAntFields").Rows.Add(New Object() {"TranslatedTitle", "String"})
            ds.Tables("tblAntFields").Rows.Add(New Object() {"Director", "String"})
            ds.Tables("tblAntFields").Rows.Add(New Object() {"Producer", "String"})
            ds.Tables("tblAntFields").Rows.Add(New Object() {"Country", "String"})
            ds.Tables("tblAntFields").Rows.Add(New Object() {"Category", "String"})
            ds.Tables("tblAntFields").Rows.Add(New Object() {"Actors", "String"})
            ds.Tables("tblAntFields").Rows.Add(New Object() {"URL", "String"})
            ds.Tables("tblAntFields").Rows.Add(New Object() {"Description", "String"})
            ds.Tables("tblAntFields").Rows.Add(New Object() {"Comments", "String"})
            ds.Tables("tblAntFields").Rows.Add(New Object() {"VideoFormat", "String"})
            ds.Tables("tblAntFields").Rows.Add(New Object() {"AudioFormat", "String"})
            ds.Tables("tblAntFields").Rows.Add(New Object() {"Resolution", "String"})
            ds.Tables("tblAntFields").Rows.Add(New Object() {"Framerate", "String"})
            ds.Tables("tblAntFields").Rows.Add(New Object() {"Languages", "String"})
            ds.Tables("tblAntFields").Rows.Add(New Object() {"Subtitles", "String"})
            ds.Tables("tblAntFields").Rows.Add(New Object() {"Size", "Int"})
            ds.Tables("tblAntFields").Rows.Add(New Object() {"Picture", "String"})
        End If

    End Sub
    Public Function ManualTestMissingFanart(ByVal wtitle As String) As Boolean

        wtitle = Grabber.GrabUtil.CreateFilename(wtitle.ToLower().Trim()).Replace(" ", ".")
        '        If (System.IO.Directory.Exists(CurrentSettings.Movie_Fanart_Path & "\\{" & wtitle & "}")) Then
        '        If (System.IO.File.Exists(CurrentSettings.Movie_Fanart_Path & "\\{" & wtitle & "}\\{" & wtitle & "}.jpg")) And (System.IO.Directory.GetFileSystemEntries(CurrentSettings.Movie_Fanart_Path & "\\{" & wtitle & "}").Length > 1) Then
        If (System.IO.File.Exists(CurrentSettings.Movie_Fanart_Path & "\\{" & wtitle & "}\\{" & wtitle & "}.jpg")) Then
            Return False
        End If
        '        End If
        Return True

    End Function

    Public Function ManualTestMissingTrailer(ByVal wtitle As String) As Boolean

        'Guzzi: To be added
        Return True

    End Function




End Class