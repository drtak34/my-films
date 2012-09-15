Imports System.Windows.Forms
Imports System.Globalization
Imports Grabber
Imports Cornerstone.Tools
Imports System.Xml

Public Class AntRecord
    'Inherits Xml.XmlElement

    Private _MovieNumber As Integer
    Private _FilePath As String = String.Empty
    Private _AllFilesPath As String = String.Empty
    Private _FileName As String = String.Empty
    Private _MediaLabel As String = String.Empty
    Private _MediaType As String = String.Empty
    Private _SourceField As String = "Source"
    Private _OverridePath As String = String.Empty
    Private _ParserPath As String = String.Empty
    Private _GrabberOverrideLanguage As String = String.Empty
    Private _GrabberOverrideGetRoles As String = String.Empty
    Private _GrabberOverridePersonLimit As String = String.Empty
    Private _GrabberOverrideTitleLimit As String = String.Empty
    'Private _XMLPath As String = String.Empty
    Private _InternetLookupOK As Boolean
    Private _InteractiveMode As Boolean
    Private _ExcludeFile As String = String.Empty
    Private _ImagePath As String = String.Empty
    Private _IsBatchMode As Boolean
    Private _InternetLookupAlwaysPrompt As Boolean
    Private _DateHandling As String = String.Empty
    Private _MovieTitleHandling As String = String.Empty
    Private _LastOutputMessage As String = String.Empty
    Private _Read_DVD_Label As Boolean = False
    Private _Use_InternetData_For_Languages As Boolean = False
    Private _Dont_Ask_Interactive As Boolean = False
    Private _XMLFilePath As String = String.Empty
    Private _XMLTempFilePath As String = String.Empty
    Private _InternetSearchHint As String = String.Empty
    Private _InternetSearchHintYear As String = String.Empty
    Private _InternetSearchHintIMDB_Id As String = String.Empty
    Private _GroupName As String = String.Empty
    Private _DownloadImage As Boolean = False
    Private _MasterTitle As String = String.Empty
    Private _OnlyAddMissing As Boolean = False
    Private _OnlyUpdateNonEmptyData As Boolean = False

    '    Private _Process_Mode As Process_Mode_Names
    Private _DatabaseFields As New Hashtable
    Private Shared _InternetData() As String
    Private Shared _MediaInfoData() As String
    Public ProhibitInternetLookup As Boolean

    Private Shared _XMLDoc As New Xml.XmlDocument
    Private Shared _XMLElement As Xml.XmlElement

    Private Enum Grabber_Output
        'OriginalTitle = 0
        'TranslatedTitle = 1
        'PicturePathLong = 2
        'Description = 3
        'Rating = 4
        'Actors = 5
        'Director = 6
        'Producer = 7
        'Year = 8
        'Country = 9
        'Category = 10
        'URL = 11
        'PicturePathShort = 12
        'Writer = 13
        'Comments = 14
        'Language = 15
        'Tagline = 16
        'Certification = 17
        'IMDB_Id = 18
        'IMDB_Rank = 19
        'Studio = 20
        'Edition = 21
        'Fanart = 22
        'Generic1 = 23
        'Generic2 = 24
        'Generic3 = 25
        'TranslatedTitleAllNames = 26
        'TranslatedTitleAllValues = 27
        'CertificationAllNames = 28
        'CertificationAllValues = 29
        'MultiPosters = 30
        'Photos = 31
        'PersonImages = 32
        'MultiFanart = 33
        'Trailer = 34
        'TMDB_Id = 35
        'Runtime = 36
        'Collection = 37
        'CollectionImageURL = 38
        'PictureURL = 39

        OriginalTitle = 40
        TranslatedTitle = 41
        PicturePathLong = 42
        Description = 43
        Rating = 44
        Actors = 45
        Director = 46
        Producer = 47
        Year = 48
        Country = 49
        Category = 50
        URL = 51
        PicturePathShort = 52
        Writer = 53
        Comments = 54
        Language = 55
        Tagline = 56
        Certification = 57
        IMDB_Id = 58
        IMDB_Rank = 59
        Studio = 60
        Edition = 61
        Fanart = 62
        Generic1 = 63
        Generic2 = 64
        Generic3 = 65
        TranslatedTitleAllNames = 66
        TranslatedTitleAllValues = 67
        CertificationAllNames = 68
        CertificationAllValues = 69
        MultiPosters = 70
        Photos = 71
        PersonImages = 72
        MultiFanart = 73
        Trailer = 74
        TMDB_Id = 75
        Runtime = 76
        Collection = 77
        CollectionImageURL = 78
        PictureURL = 79
    End Enum

    Private Enum MediaInfo_Output
        Filename = 0
        VideoFormat = 1
        VideoBitrate = 2
        Framerate = 3
        Resolution = 4
        AudioBitrate = 5
        AudioFormat = 6
        runtime = 7
        filesize = 8
        audiostreamcount = 9
        audiostreamcodeclist = 10
        audiostreamlanguagelist = 11
        textstreamcodeclist = 12
        textstreamlanguagelist = 13
    End Enum
    Public Enum Process_Mode_Names
        Import
        Update
    End Enum

    Public Property InternetLookupAlwaysPrompt() As Boolean
        Get
            Return _InternetLookupAlwaysPrompt
        End Get
        Set(ByVal value As Boolean)
            _InternetLookupAlwaysPrompt = value
        End Set
    End Property

    Public Property SourceField() As String
        Get
            Return _SourceField
        End Get
        Set(ByVal Value As String)
            ' Sets the property value.
            _SourceField = Value
        End Set
    End Property
    Public Property MediaType() As String
        Get
            Return _MediaType
        End Get
        Set(ByVal Value As String)
            ' Sets the property value.
            _MediaType = Value
        End Set
    End Property
    Public Property MediaLabel() As String
        Get
            Return _MediaLabel
        End Get
        Set(ByVal Value As String)
            ' Sets the property value.
            _MediaLabel = Value
        End Set
    End Property
    Public Property FileName() As String
        Get
            Return _FileName
        End Get
        Set(ByVal Value As String)
            ' Sets the property value.
            _FileName = Value
        End Set
    End Property
    Public Property FilePath() As String
        Get
            Return _FilePath
        End Get
        Set(ByVal Value As String)
            'If there's more than one file in FilePath just take the first one:
            If Value.Contains(";") Then
                'First reset _AllFilesPath...
                _AllFilesPath = Value
                _FilePath = Value.Substring(0, Value.IndexOf(";"))
            Else
                _FilePath = Value
            End If

        End Set
    End Property
    Public Property AllFilesPath() As String
        Get
            Return _AllFilesPath
        End Get
        Set(ByVal Value As String)
            ' Sets the property value.
            _AllFilesPath = Value
        End Set
    End Property
    Public Property OverridePath() As String
        Get
            Return _OverridePath
        End Get
        Set(ByVal Value As String)
            ' Sets the property value.
            _OverridePath = Value
        End Set
    End Property
    Public Property MovieNumber() As Integer
        Get
            Return _MovieNumber
        End Get
        Set(ByVal Value As Integer)
            ' Sets the property value.
            _MovieNumber = Value
        End Set
    End Property
    Public Property XMLElement() As Xml.XmlElement
        Get
            Return _XMLElement
        End Get
        Set(ByVal value As Xml.XmlElement)
            _XMLElement = value
        End Set
    End Property
    Public Property XMLDoc() As Xml.XmlDocument
        Get
            Return _XMLDoc
        End Get
        Set(ByVal value As Xml.XmlDocument)
            _XMLDoc = value
        End Set
    End Property
    Public Property ParserPath()
        Get
            Return _ParserPath
        End Get
        Set(ByVal value)
            _ParserPath = value
        End Set
    End Property
    Public Property GrabberOverrideLanguage()
        Get
            Return _GrabberOverrideLanguage
        End Get
        Set(ByVal value)
            _GrabberOverrideLanguage = value
        End Set
    End Property
    Public Property GrabberOverrideGetRoles()
        Get
            Return _GrabberOverrideGetRoles
        End Get
        Set(ByVal value)
            _GrabberOverrideGetRoles = value
        End Set
    End Property
    Public Property GrabberOverridePersonLimit()
        Get
            Return _GrabberOverridePersonLimit
        End Get
        Set(ByVal value)
            _GrabberOverridePersonLimit = value
        End Set
    End Property
    Public Property GrabberOverrideTitleLimit()
        Get
            Return _GrabberOverrideTitleLimit
        End Get
        Set(ByVal value)
            _GrabberOverrideTitleLimit = value
        End Set
    End Property
    Public ReadOnly Property InternetLookupOK() As Boolean
        Get
            Return _InternetLookupOK
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
    Public Property ExcludeFile() As String
        Get
            Return _ExcludeFile
        End Get
        Set(ByVal value As String)
            _ExcludeFile = value
        End Set
    End Property
    Public Property ImagePath() As String
        Get
            Return _ImagePath
        End Get
        Set(ByVal value As String)
            _ImagePath = value
        End Set
    End Property
    Public Property DateHandling() As String
        Get
            Return _DateHandling
        End Get
        Set(ByVal value As String)
            _DateHandling = value
        End Set
    End Property
    Public Property MovieTitleHandling() As String
        Get
            Return _MovieTitleHandling
        End Get
        Set(ByVal value As String)
            _MovieTitleHandling = value
        End Set
    End Property
    Public Property Read_DVD_Label() As Boolean
        Get
            Return _Read_DVD_Label
        End Get
        Set(ByVal value As Boolean)
            _Read_DVD_Label = value
        End Set
    End Property
    Public Property Use_InternetData_For_Languages() As Boolean
        Get
            Return _Use_InternetData_For_Languages
        End Get
        Set(ByVal value As Boolean)
            _Use_InternetData_For_Languages = value
        End Set
    End Property
    Public Property Dont_Ask_Interactive() As Boolean
        Get
            Return _Dont_Ask_Interactive
        End Get
        Set(ByVal value As Boolean)
            _Dont_Ask_Interactive = value
        End Set
    End Property
    Public Property XMLFilePath() As String
        Get
            Return _XMLFilePath
        End Get
        Set(ByVal value As String)
            _XMLFilePath = value
        End Set
    End Property
    Public ReadOnly Property LastOutputMessage() As String
        Get
            Return _LastOutputMessage
        End Get
    End Property
    Public Property InternetSearchHint() As String
        Get
            Return _InternetSearchHint
        End Get
        Set(ByVal value As String)
            _InternetSearchHint = value
        End Set
    End Property
    Public Property InternetSearchHintYear() As String
        Get
            Return _InternetSearchHintYear
        End Get
        Set(ByVal value As String)
            _InternetSearchHintYear = value
        End Set
    End Property
    Public Property InternetSearchHintIMDB_Id() As String
        Get
            Return _InternetSearchHintIMDB_Id
        End Get
        Set(ByVal value As String)
            _InternetSearchHintIMDB_Id = value
        End Set
    End Property
    Public Property GroupName() As String
        Get
            Return _GroupName
        End Get
        Set(ByVal value As String)
            _GroupName = value
        End Set
    End Property
    Public Property MasterTitle() As String
        Get
            Return _MasterTitle
        End Get
        Set(ByVal value As String)
            _MasterTitle = value
        End Set
    End Property
    Public Property OnlyAddMissingData() As Boolean
        Get
            Return _OnlyAddMissing
        End Get
        Set(ByVal value As Boolean)
            _OnlyAddMissing = value
        End Set
    End Property
    Public Property OnlyUpdateNonEmptyData() As Boolean
        Get
            Return _OnlyUpdateNonEmptyData
        End Get
        Set(ByVal value As Boolean)
            _OnlyUpdateNonEmptyData = value
        End Set
    End Property

    Public Sub New()

        Dim DBFields() = CurrentSettings.Database_Fields_To_Import.Split(";")
        Dim FieldName As String
        Dim FieldChecked As Boolean

        For Each blah As String In DBFields
            FieldName = blah.Substring(0, blah.IndexOf("|")).ToLower
            FieldChecked = blah.Substring(blah.IndexOf("|") + 1)
            If Not _DatabaseFields.ContainsKey(FieldName) Then
                _DatabaseFields.Add(FieldName, FieldChecked)
            End If
            If FieldName = "picture" Then
                If FieldChecked = True Then
                    _DownloadImage = True
                End If
            End If
        Next
    End Sub
    Public Sub CreateElement()
        _XMLElement = XMLDoc.CreateElement("Movie")
    End Sub
    Public Function VerifyElement(ByVal otitle As String, ByVal currentNode As Xml.XmlNode) As Xml.XmlNode
        '' added to avoid exception with empty otitle reported by z3us
        'If otitle Is Nothing Then
        '    Return currentNode
        'End If

        'Dim CurrentNode2 As Xml.XmlNode
        'Dim CurrentAttribute2 As String
        ''CurrentNode2 = XMLDoc.SelectSingleNodeFast("//AntMovieCatalog/Catalog/Contents/Movie[@OriginalTitle='" & otitle & "']")
        'CurrentNode2 = XMLDoc.SelectSingleNodeFast("//AntMovieCatalog/Catalog/Contents/Movie[@OriginalTitle=""" & otitle & """]")
        'If (Not CurrentNode2 Is Nothing) Then
        '    If (CurrentNode2.Attributes("Number").Value) <> (currentNode.Attributes("Number").Value) Then 'check, if two movies with same otitle but different recordnumber exist
        '        CurrentAttribute2 = _SourceField

        '        ' This also doesn't work, as there is not yet a source for new movie available in XML Element - might be called later?
        '        'If (CurrentNode2.Attributes(_SourceField).Value = currentNode.Attributes(_SourceField).Value) Then
        '        '    currentNode.Attributes.RemoveAll()
        '        '    Return CurrentNode2
        '        'End If

        '        ' Guzzi: Removed the following stuff, as it replaced existing movies with same title, but different content (e.g. director's cut or extended versions)
        '        'If _XMLElement.Attributes(CurrentAttribute2) Is Nothing Then
        '        '    currentNode.Attributes.RemoveAll()
        '        '    Return CurrentNode2
        '        'End If
        '        'If _XMLElement.Attributes(CurrentAttribute2).Value.ToString = String.Empty Then
        '        '    currentNode.Attributes.RemoveAll()
        '        '    Return CurrentNode2
        '        'End If

        '    End If
        'End If
        Return currentNode
    End Function
    Private Sub DoInternetLookup(ByVal SearchString As String, Optional ByVal Year As String = "", Optional ByVal IMDB_Id As String = "") ' Guzzi: Added year and imdb id as optional (search) parameters
        'This is now reset on ProcessFile, since all processing will begin with that Sub.
        '_LastOutputMessage = ""
        If CurrentSettings.Prohibit_Internet_Lookup = True Or ProhibitInternetLookup = True Then
            _InternetLookupOK = False
            Exit Sub
        End If
        Try
            If IsInternetLookupNeeded() = True Then
                wurl.Clear()
                Dim wpage As Int16 = -1
                Dim Gb As Grabber.Grabber_URLClass = New Grabber.Grabber_URLClass
                While (True)
                    'If _InternetSearchHintIMDB_Id.Length > 0 Then
                    If _InternetSearchHintIMDB_Id.Length > 0 And (_Dont_Ask_Interactive = True Or _InternetLookupAlwaysPrompt = False) Then
                        wurl = Gb.ReturnURL(_InternetSearchHintIMDB_Id, _ParserPath, wpage, _InternetLookupAlwaysPrompt, _FilePath)
                        If (wurl.Count = 1) And _InternetLookupAlwaysPrompt = False Then
                            _InternetData = Gb.GetDetail(wurl.Item(0).URL, _ImagePath, _ParserPath, _DownloadImage, _GrabberOverrideLanguage, _GrabberOverridePersonLimit, _GrabberOverrideTitleLimit, _GrabberOverrideGetRoles)
                            _InternetLookupOK = True
                            _LastOutputMessage = SearchString & " - " & " Movie found by imdb hint (" & _InternetSearchHintIMDB_Id & ")."
                            If bgwFolderScanUpdate.CancellationPending = True Then
                                Exit Sub
                            End If
                            Exit While
                        End If
                    End If

                    wurl.Clear() ' clear to make sure nothing left from tt request ...
                    wurl = Gb.ReturnURL(SearchString, _ParserPath, wpage, _InternetLookupAlwaysPrompt, _FilePath)
                    If (wurl.Count = 1) And _InternetLookupAlwaysPrompt = False Then

                        '_InternetData = Gb.GetDetail(wurl.Item(0).URL, _ImagePath, _ParserPath, _DownloadImage)
                        _InternetData = Gb.GetDetail(wurl.Item(0).URL, _ImagePath, _ParserPath, _DownloadImage, _GrabberOverrideLanguage, _GrabberOverridePersonLimit, _GrabberOverrideTitleLimit, _GrabberOverrideGetRoles)
                        _InternetLookupOK = True
                        _LastOutputMessage = SearchString & " - " & " Movie found by 'single result' automatch."

                        If bgwFolderScanUpdate.CancellationPending = True Then
                            Exit Sub
                        End If
                        Exit While
                    Else
                        Dim wThumb As String
                        Dim wtitle As String
                        Dim wdirector As String
                        Dim wmovieurl As String
                        Dim wyear As String
                        Dim wOptions As String
                        Dim wAkas As String
                        Dim wID As String
                        Dim dyear As Double
                        Dim wimdb As String
                        Dim wtmdb As String
                        Dim wlimityear As Boolean = False
                        Dim distance As String

                        Dim CountTitleMatch As Integer = 0
                        Dim TitleMatch As String = ""
                        Dim matchingDistance As Integer = Integer.MaxValue
                        Dim matchingDistanceWithOptions As Integer = Integer.MaxValue
                        Dim index As Integer = -1
                        Dim indexWithOptions As Integer = -1
                        Dim searchyearHint As Double = 0

                        If Double.TryParse(_InternetSearchHintYear, searchyearHint) = False Then
                            searchyearHint = 0
                        End If


                        'If _InteractiveMode = True Then
                        If (_InteractiveMode = True And _Dont_Ask_Interactive = False) Then

                            'First try to find matches due to "try to find best match automatically" - this is enhanced matching option as feature on top of grabber matching itself

                            If _InternetLookupAlwaysPrompt = False Then
                                'Try to match by IMDB_Id
                                If _InternetSearchHintIMDB_Id.Length > 0 Then
                                    For i As Integer = 0 To wurl.Count - 1
                                        wtitle = wurl.Item(i).Title.ToString
                                        wimdb = wurl.Item(i).IMDB_ID.ToString
                                        wtmdb = wurl.Item(i).TMDB_ID.ToString
                                        If (wimdb = _InternetSearchHintIMDB_Id And wimdb.Length > 3) Then
                                            _InternetData = Gb.GetDetail(wurl.Item(i).URL, _ImagePath, _ParserPath, _DownloadImage, _GrabberOverrideLanguage, _GrabberOverridePersonLimit, _GrabberOverrideTitleLimit, _GrabberOverrideGetRoles)
                                            _InternetLookupOK = True
                                            _LastOutputMessage = SearchString & " - " & " Movie found by imdb hint (" & _InternetSearchHintIMDB_Id & ")."
                                            If bgwFolderScanUpdate.CancellationPending = True Then
                                                Exit Sub
                                            End If
                                            Exit While
                                        End If
                                    Next
                                End If

                                'Try to match by year hint (and name)
                                If searchyearHint > 0 Then
                                    indexWithOptions = FuzzyMatch(SearchString, wurl, False, searchyearHint, 0, matchingDistanceWithOptions, CountTitleMatch, TitleMatch)
                                    index = FuzzyMatch(SearchString, wurl, True, searchyearHint, 0, matchingDistance, CountTitleMatch, TitleMatch)
                                    If index > -1 And matchingDistance < 3 And matchingDistance <= matchingDistanceWithOptions Then
                                        _InternetData = Gb.GetDetail(wurl.Item(index).URL, _ImagePath, _ParserPath, _DownloadImage, _GrabberOverrideLanguage, _GrabberOverridePersonLimit, _GrabberOverrideTitleLimit, _GrabberOverrideGetRoles)
                                        _InternetLookupOK = True
                                        _LastOutputMessage = SearchString & " - " & " Movie found by year hint (" & _InternetSearchHintYear & ") and name match (" & TitleMatch & ") with FuzziDistance = '" & matchingDistance.ToString & "' and Optionsfilter 'on'."
                                        If bgwFolderScanUpdate.CancellationPending = True Then
                                            Exit Sub
                                        End If
                                        Exit While
                                    End If

                                    index = FuzzyMatch(SearchString, wurl, False, searchyearHint, 0, matchingDistance, CountTitleMatch, TitleMatch)
                                    If index > -1 And matchingDistance < 3 Then
                                        _InternetData = Gb.GetDetail(wurl.Item(index).URL, _ImagePath, _ParserPath, _DownloadImage, _GrabberOverrideLanguage, _GrabberOverridePersonLimit, _GrabberOverrideTitleLimit, _GrabberOverrideGetRoles)
                                        _InternetLookupOK = True
                                        _LastOutputMessage = SearchString & " - " & " Movie found by year hint (" & _InternetSearchHintYear & ") and name match (" & TitleMatch & ") with FuzziDistance = '" & matchingDistance.ToString & "' and Optionsfilter 'off'."
                                        If bgwFolderScanUpdate.CancellationPending = True Then
                                            Exit Sub
                                        End If
                                        Exit While
                                    End If
                                Else
                                    index = FuzzyMatch(SearchString, wurl, True, 0, 0, matchingDistance, CountTitleMatch, TitleMatch)
                                    If index > -1 And matchingDistance < 2 Then
                                        _InternetData = Gb.GetDetail(wurl.Item(index).URL, _ImagePath, _ParserPath, _DownloadImage, _GrabberOverrideLanguage, _GrabberOverridePersonLimit, _GrabberOverrideTitleLimit, _GrabberOverrideGetRoles)
                                        _InternetLookupOK = True
                                        _LastOutputMessage = SearchString & " - " & " Movie found by name match (" & TitleMatch & ") with FuzziDistance = '" & matchingDistance.ToString & "' and Optionsfilter 'on'."
                                        If bgwFolderScanUpdate.CancellationPending = True Then
                                            Exit Sub
                                        End If
                                        Exit While
                                    End If
                                    index = FuzzyMatch(SearchString, wurl, False, 0, 0, matchingDistance, CountTitleMatch, TitleMatch)
                                    If index > -1 And matchingDistance < 2 Then
                                        _InternetData = Gb.GetDetail(wurl.Item(index).URL, _ImagePath, _ParserPath, _DownloadImage, _GrabberOverrideLanguage, _GrabberOverridePersonLimit, _GrabberOverrideTitleLimit, _GrabberOverrideGetRoles)
                                        _InternetLookupOK = True
                                        _LastOutputMessage = SearchString & " - " & " Movie found by name match (" & TitleMatch & ") with FuzziDistance = '" & matchingDistance.ToString & "' and Optionsfilter 'on'."
                                        If bgwFolderScanUpdate.CancellationPending = True Then
                                            Exit Sub
                                        End If
                                        Exit While
                                    End If
                                End If
                            End If

                            'If no automatches found, show dialog box to choose from
                            frmList.txtSearchString.Text = SearchString
                            frmList.txtSearchintYear.Text = _InternetSearchHintYear
                            frmList.txtSearchhintIMDB_Id.Text = _InternetSearchHintIMDB_Id
                            If _InternetSearchHintIMDB_Id <> "" Then
                                frmList.btnSearchAgainWithIMDB_Id.Enabled = True
                            Else
                                frmList.btnSearchAgainWithIMDB_Id.Enabled = False
                            End If
                            frmList.chkDontAskAgain.Checked = False
                            frmList.txtTmpParserFilePath.Text = _ParserPath
                            frmList.txtTmpParserFilePathShort.Text = _ParserPath.Substring(_ParserPath.LastIndexOf("\") + 1)
                            frmList.lstOptionsExt.Rows.Clear()

                            If _FileName.ToString <> "" Then
                                frmList.Text = _FileName
                                frmList.txtSource.Text = _FileName
                            Else
                                frmList.Text = _FilePath
                                frmList.txtSource.Text = _FilePath
                            End If
                            frmList.txtSourceFullAllPath.Text = _AllFilesPath
                            frmList.txtSourceFull.Text = _FilePath
                            If (wurl.Count = 0) Then
                                frmList.lstOptionsExt.Rows.Add(New String() {Nothing, "Movie not found...", "", "", "", "", "", ""})
                                frmList.btnSearchAllPages.Enabled = False
                            Else
                                ' index = FuzzyMatch(SearchString, wurl, False, searchyearHint, 0, matchingDistance, CountTitleMatch, TitleMatch)
                                frmList.btnSearchAllPages.Enabled = True
                                For i As Integer = 0 To wurl.Count - 1
                                    If wurl.Item(i).Year.ToString = _InternetSearchHintYear Then
                                        wlimityear = True
                                    End If
                                Next
                                For i As Integer = 0 To wurl.Count - 1
                                    wThumb = wurl.Item(i).Thumb.ToString
                                    wtitle = wurl.Item(i).Title.ToString
                                    wdirector = wurl.Item(i).Director.ToString
                                    wyear = wurl.Item(i).Year.ToString
                                    wAkas = wurl.Item(i).Akas.ToString
                                    wID = wurl.Item(i).ID.ToString
                                    wOptions = wurl.Item(i).Options.ToString
                                    wmovieurl = wurl.Item(i).URL.ToString
                                    If (_InternetSearchHint.Length > 0 And (wdirector.Contains(_InternetSearchHint) Or wyear.Contains(_InternetSearchHint)) And _InternetLookupAlwaysPrompt = False And (wlimityear = False Or wyear = _InternetSearchHintYear)) Then
                                        _InternetData = Gb.GetDetail(wurl.Item(i).URL, _ImagePath, _ParserPath, _DownloadImage, _GrabberOverrideLanguage, _GrabberOverridePersonLimit, _GrabberOverrideTitleLimit, _GrabberOverrideGetRoles)
                                        _InternetLookupOK = True
                                        If bgwFolderScanUpdate.CancellationPending = True Then
                                            Exit Sub
                                        End If
                                        Exit While
                                    End If
                                    If FuzziDistance(SearchString, wtitle) = Integer.MaxValue Then
                                        distance = ""
                                    Else
                                        distance = FuzziDistance(SearchString, wtitle).ToString
                                    End If

                                    'Dim img As DataGridViewImageColumn = New DataGridViewImageColumn()
                                    Dim image As System.Drawing.Image = GrabUtil.GetImageFromUrl(wThumb)
                                    'img.Image = image
                                    ' Image smallImage = image.GetThumbnailImage(20, 30, null, IntPtr.Zero);
                                    ' dataGridViewSearchResults.Rows[i].Cells[1].Style.Tag = "BLANK";
                                    ' dataGridViewSearchResults.Rows[i].Cells[1].Style.NullValue = null;

                                    If wyear = _InternetSearchHintYear Then
                                        frmList.lstOptionsExt.Rows.Add(New Object() {image, wtitle & " - (+++ year match! +++)", wyear, wOptions, wAkas, wID, wmovieurl, distance})
                                    Else
                                        frmList.lstOptionsExt.Rows.Add(New Object() {image, wtitle, wyear, wOptions, wAkas, wID, wmovieurl, distance})
                                    End If
                                Next
                            End If
                            If frmList.lstOptionsExt.Rows.Count > 0 Then
                                frmList.lstOptionsExt.SelectionMode = Windows.Forms.DataGridViewSelectionMode.FullRowSelect
                                frmList.lstOptionsExt.Rows(0).Selected = True 'set first line as selected
                                frmList.btnOK.Enabled = True
                            Else
                                frmList.btnOK.Enabled = False
                            End If

                            Dim returnValue As System.Windows.Forms.DialogResult

                            'frmList.Parent = Form1
                            returnValue = frmList.ShowDialog()
                            SearchString = frmList.txtSearchString.Text
                            Dim blah4 As Boolean = frmList.chkDontAskAgain.Checked
                            'Dim wentry As Integer = frmList.lstOptions.SelectedIndex
                            Dim wentry As Integer = frmList.lstOptionsExt.Rows.GetFirstRow(DataGridViewElementStates.Selected)

                            If ((returnValue = Windows.Forms.DialogResult.OK) And (wentry > -1) And frmList.lstOptionsExt("Title", wentry).Value.ToString() = "Movie not found...") Then
                                'If ((returnValue = Windows.Forms.DialogResult.OK) And (wentry > -1) And frmList.lstOptions.SelectedItem.ToString = "Movie not found...") Then
                                _InternetLookupOK = False
                                _LastOutputMessage = "Failed to load Internet Data for " & FilePath
                                Exit While
                            End If
                            If ((returnValue = Windows.Forms.DialogResult.OK) And (wentry > -1) And frmList.lstOptionsExt("Title", wentry).Value.ToString() = "+++") Then
                                wpage = Convert.ToInt16(wurl.Item(wentry).IMDBURL)
                            Else
                                If ((returnValue = Windows.Forms.DialogResult.OK) And (wentry > -1) And frmList.lstOptionsExt("Title", wentry).Value.ToString() = "---") Then
                                    wpage = Convert.ToInt16(wurl.Item(wentry).IMDBURL)
                                Else
                                    If (returnValue = Windows.Forms.DialogResult.Abort) Then
                                        _InternetLookupOK = False
                                        _LastOutputMessage = "UserAbort ! - Scan cancelled from Userdialog !"
                                        Exit While
                                    End If
                                    If ((returnValue = Windows.Forms.DialogResult.OK) And (wentry > -1) And (frmList.lstOptionsExt("Title", wentry).Value.ToString().Length > 0)) Then
                                        '_InternetData = Gb.GetDetail(wurl.Item(wentry).url, _ImagePath, frmList.txtTmpParserFilePath.Text, _DownloadImage)
                                        _InternetData = Gb.GetDetail(wurl.Item(wentry).url, _ImagePath, frmList.txtTmpParserFilePath.Text, _DownloadImage, _GrabberOverrideLanguage, _GrabberOverridePersonLimit, _GrabberOverrideTitleLimit, _GrabberOverrideGetRoles)
                                        '_InternetData = Gb.GetDetail(frmList.lstOptionsExt("Weblink", wentry).Value.ToString(), _ImagePath, frmList.txtTmpParserFilePath.Text, _DownloadImage, _GrabberOverrideLanguage, _GrabberOverridePersonLimit, _GrabberOverrideTitleLimit, _GrabberOverrideGetRoles)
                                        _InternetLookupOK = True
                                        _LastOutputMessage = SearchString & " - " & " Movie found by user's manual selection"
                                        Exit While
                                    Else 'If returnValue = Windows.Forms.DialogResult.Cancel Then
                                        'The ElseIf didn't always fire if you close the window or something.
                                        'User cancelled - check for 'Ignore File' flag.
                                        If frmList.chkDontAskAgain.Checked And Not CurrentSettings.Excluded_File_Strings.Contains(FilePath.ToLower) Then
                                            'Add to excluded file list to ignore in future scans:
                                            My.Computer.FileSystem.WriteAllText(_ExcludeFile, FilePath.ToLower + vbCrLf, True) ' reenabled, aw below option did not work
                                            CurrentSettings.Excluded_File_Strings += ("|" + FilePath.ToLower) ' this might not be needed - seems not to work?
                                        End If
                                        _InternetLookupOK = False
                                        'LogEvent("Failed to load Internet Data for " & FilePath, EventLogLevel.ImportantEvent)
                                        _LastOutputMessage = "Failed to load Internet Data for " & FilePath
                                        Exit While
                                    End If
                                End If
                            End If
                        Else
                            ' In batch mode (or GUI mode with 'don't ask interactive'), try to identify the right movie with  optseachstring
                            'LogEvent(SearchString & " - " & wurl.Count.ToString & " Movies found", EventLogLevel.Informational)
                            If _InternetSearchHint.Length > 0 Then ' this is old searchhint method !
                                For i As Integer = 0 To wurl.Count - 1
                                    wtitle = wurl.Item(i).Title.ToString
                                    wyear = wurl.Item(i).Year.ToString
                                    wdirector = wurl.Item(i).Director.ToString
                                    If wyear.Length > 4 Then
                                        wyear = wyear.Substring(0, 4)
                                    End If
                                    If Double.TryParse(wyear, dyear) = False Or wyear.Length < 4 Then
                                        wyear = ""
                                    End If
                                    'If (_InternetSearchHint.Length > 0 And wtitle.Contains(_InternetSearchHint)) Then
                                    If ((wdirector.Contains(_InternetSearchHint) Or wyear.Contains(_InternetSearchHint))) Then
                                        'Dim datas As String()
                                        _InternetData = Gb.GetDetail(wurl.Item(i).URL, _ImagePath, _ParserPath, _DownloadImage, _GrabberOverrideLanguage, _GrabberOverridePersonLimit, _GrabberOverrideTitleLimit, _GrabberOverrideGetRoles)
                                        'CreateXmlnetInfos(datas)
                                        _InternetLookupOK = True
                                        _LastOutputMessage = SearchString & " - " & " Movie found by AMCU internetsearchhint (" & _InternetSearchHint & ")."
                                        If bgwFolderScanUpdate.CancellationPending = True Then
                                            Exit Sub
                                        End If
                                        Exit While
                                    End If
                                Next
                            End If

                            If (wurl.Count = 0) Then
                                _LastOutputMessage = SearchString & " - " & wurl.Count.ToString & " Movies found from Internet lookup."
                                _InternetLookupOK = False
                                Exit While
                            Else 'If _InternetSearchHint.Length > 0 Or _InternetSearchHintYear.Length > 0 Or _InternetSearchHintIMDB_Id.Length > 0 Then

                                ' Check for direct IMDB match
                                If _InternetSearchHintIMDB_Id.Length > 0 Then
                                    For i As Integer = 0 To wurl.Count - 1
                                        wtitle = wurl.Item(i).Title.ToString
                                        wimdb = wurl.Item(i).IMDB_ID.ToString
                                        wtmdb = wurl.Item(i).TMDB_ID.ToString
                                        If (wimdb = _InternetSearchHintIMDB_Id) Then
                                            _InternetData = Gb.GetDetail(wurl.Item(i).URL, _ImagePath, _ParserPath, _DownloadImage, _GrabberOverrideLanguage, _GrabberOverridePersonLimit, _GrabberOverrideTitleLimit, _GrabberOverrideGetRoles)
                                            _InternetLookupOK = True
                                            _LastOutputMessage = SearchString & " - " & " Movie found by imdb hint (" & _InternetSearchHintIMDB_Id & ")."
                                            If bgwFolderScanUpdate.CancellationPending = True Then
                                                Exit Sub
                                            End If
                                            Exit While
                                        End If
                                    Next
                                End If

                                ' searches with yearhint
                                If searchyearHint > 0 Then

                                    ' Check for exact year match (and name match) - without Options
                                    indexWithOptions = FuzzyMatch(SearchString, wurl, False, searchyearHint, 0, matchingDistanceWithOptions, CountTitleMatch, TitleMatch) ' get also value with Options for proper match decision
                                    index = FuzzyMatch(SearchString, wurl, True, searchyearHint, 0, matchingDistance, CountTitleMatch, TitleMatch)
                                    If index < 0 And matchingDistance = 0 And CountTitleMatch > 1 Then ' multiple exact matches - exit, as no matching possible
                                        _InternetLookupOK = False
                                        _LastOutputMessage = SearchString & " - Multiple matches (optionsfilter on) - no matching possible. Closest Match Distance: '" & matchingDistance.ToString & "' with '" & CountTitleMatch.ToString & "' matches and match name '" & TitleMatch & "'."
                                        If bgwFolderScanUpdate.CancellationPending = True Then
                                            Exit Sub
                                        End If
                                        Exit While
                                    ElseIf index > -1 And matchingDistance < 5 And matchingDistance <= matchingDistanceWithOptions Then
                                        _InternetData = Gb.GetDetail(wurl.Item(index).URL, _ImagePath, _ParserPath, _DownloadImage, _GrabberOverrideLanguage, _GrabberOverridePersonLimit, _GrabberOverrideTitleLimit, _GrabberOverrideGetRoles)
                                        _InternetLookupOK = True
                                        _LastOutputMessage = SearchString & " - " & " Movie found by year hint (" & _InternetSearchHintYear & ") and name match (" & TitleMatch & ") with FuzziDistance = '" & matchingDistance.ToString & "' and Optionsfilter 'on'."
                                        If bgwFolderScanUpdate.CancellationPending = True Then
                                            Exit Sub
                                        End If
                                        Exit While
                                    End If

                                    ' Check for exact year match (and name match) - with Options (including TV, series, etc.)
                                    index = FuzzyMatch(SearchString, wurl, False, searchyearHint, 0, matchingDistance, CountTitleMatch, TitleMatch)
                                    If index < 0 And matchingDistance = 0 And CountTitleMatch > 1 Then ' multiple exact matches - exit, as no matching possible
                                        _InternetLookupOK = False
                                        _LastOutputMessage = SearchString & " - Multiple matches (optionsfilter off) - no matching possible. Closest Match Distance: '" & matchingDistance.ToString & "' with '" & CountTitleMatch.ToString & "' matches and match name '" & TitleMatch & "'."
                                        If bgwFolderScanUpdate.CancellationPending = True Then
                                            Exit Sub
                                        End If
                                        Exit While
                                    ElseIf index > -1 And matchingDistance < 5 Then
                                        _InternetData = Gb.GetDetail(wurl.Item(index).URL, _ImagePath, _ParserPath, _DownloadImage, _GrabberOverrideLanguage, _GrabberOverridePersonLimit, _GrabberOverrideTitleLimit, _GrabberOverrideGetRoles)
                                        _InternetLookupOK = True
                                        _LastOutputMessage = SearchString & " - " & " Movie found by year hint (" & _InternetSearchHintYear & ") and name match (" & TitleMatch & ") with FuzziDistance = '" & matchingDistance.ToString & "' and Optionsfilter 'off'."
                                        If bgwFolderScanUpdate.CancellationPending = True Then
                                            Exit Sub
                                        End If
                                        Exit While
                                    End If

                                    ' Check for "near" year match (and name match) - without Options 
                                    index = FuzzyMatch(SearchString, wurl, True, searchyearHint, 1, matchingDistance, CountTitleMatch, TitleMatch)
                                    If index < 0 And matchingDistance = 0 And CountTitleMatch > 1 Then ' multiple matches - exit, as no matching possible
                                        _InternetLookupOK = False
                                        _LastOutputMessage = SearchString & " - Multiple matches (optionsfilter on, close year match) - no matching possible. Closest Match Distance: '" & matchingDistance.ToString & "' with '" & CountTitleMatch.ToString & "' matches and match name '" & TitleMatch & "'."
                                        If bgwFolderScanUpdate.CancellationPending = True Then
                                            Exit Sub
                                        End If
                                        Exit While
                                    ElseIf index > -1 And matchingDistance < 3 Then
                                        _InternetData = Gb.GetDetail(wurl.Item(index).URL, _ImagePath, _ParserPath, _DownloadImage, _GrabberOverrideLanguage, _GrabberOverridePersonLimit, _GrabberOverrideTitleLimit, _GrabberOverrideGetRoles)
                                        _InternetLookupOK = True
                                        _LastOutputMessage = SearchString & " - " & " Movie found by year hint and close match (+/- 1) (" & _InternetSearchHintYear & ") and name match (" & TitleMatch & ") with FuzziDistance = '" & matchingDistance.ToString & "' and Optionsfilter 'on'."
                                        If bgwFolderScanUpdate.CancellationPending = True Then
                                            Exit Sub
                                        End If
                                        Exit While
                                    End If

                                    ' Check for "near" year match (and name match) - with Options (including TV, series, etc.)
                                    index = FuzzyMatch(SearchString, wurl, False, searchyearHint, 1, matchingDistance, CountTitleMatch, TitleMatch)
                                    If index < 0 And matchingDistance = 0 And CountTitleMatch > 1 Then ' multiple matches - exit, as no matching possible
                                        _InternetLookupOK = False
                                        _LastOutputMessage = SearchString & " - Multiple matches (optionsfilter off, close year match) - no matching possible. Closest Match Distance: '" & matchingDistance.ToString & "' with '" & CountTitleMatch.ToString & "' matches and match name '" & TitleMatch & "'."
                                        If bgwFolderScanUpdate.CancellationPending = True Then
                                            Exit Sub
                                        End If
                                        Exit While
                                    ElseIf index > -1 And matchingDistance < 3 Then
                                        _InternetData = Gb.GetDetail(wurl.Item(index).URL, _ImagePath, _ParserPath, _DownloadImage, _GrabberOverrideLanguage, _GrabberOverridePersonLimit, _GrabberOverrideTitleLimit, _GrabberOverrideGetRoles)
                                        _InternetLookupOK = True
                                        _LastOutputMessage = SearchString & " - " & " Movie found by year hint and close match (+/- 1) (" & _InternetSearchHintYear & ") and name match (" & TitleMatch & ") with FuzziDistance = '" & matchingDistance.ToString & "' and Optionsfilter 'off'."
                                        If bgwFolderScanUpdate.CancellationPending = True Then
                                            Exit Sub
                                        End If
                                        Exit While
                                    End If


                                Else ' searches without year hint

                                    ' Check for name matches without year hint and options on
                                    index = FuzzyMatch(SearchString, wurl, True, 0, 0, matchingDistance, CountTitleMatch, TitleMatch)
                                    If index < 0 And matchingDistance = 0 And CountTitleMatch > 1 Then ' multiple matches - exit, as no matching possible
                                        _InternetLookupOK = False
                                        _LastOutputMessage = SearchString & " - Multiple matches (optionsfilter on, only title match) - no matching possible. Closest Match Distance: '" & matchingDistance.ToString & "' with '" & CountTitleMatch.ToString & "' matches and match name '" & TitleMatch & "'."
                                        If bgwFolderScanUpdate.CancellationPending = True Then
                                            Exit Sub
                                        End If
                                        Exit While
                                    ElseIf index > -1 And matchingDistance < 4 Then
                                        _InternetData = Gb.GetDetail(wurl.Item(index).URL, _ImagePath, _ParserPath, _DownloadImage, _GrabberOverrideLanguage, _GrabberOverridePersonLimit, _GrabberOverrideTitleLimit, _GrabberOverrideGetRoles)
                                        _InternetLookupOK = True
                                        _LastOutputMessage = SearchString & " - " & " Movie found by name match (" & TitleMatch & ") with FuzziDistance = '" & matchingDistance.ToString & "' and Optionsfilter 'on'."
                                        If bgwFolderScanUpdate.CancellationPending = True Then
                                            Exit Sub
                                        End If
                                        Exit While
                                    End If

                                    ' Check for name matches without year hint and options off
                                    index = FuzzyMatch(SearchString, wurl, False, 0, 0, matchingDistance, CountTitleMatch, TitleMatch)
                                    If index < 0 And matchingDistance = 0 And CountTitleMatch > 1 Then ' multiple matches - exit, as no matching possible
                                        _InternetLookupOK = False
                                        _LastOutputMessage = SearchString & " - Multiple matches (optionsfilter off, only title match) - no matching possible. Closest Match Distance: '" & matchingDistance.ToString & "' with '" & CountTitleMatch.ToString & "' matches and match name '" & TitleMatch & "'."
                                        If bgwFolderScanUpdate.CancellationPending = True Then
                                            Exit Sub
                                        End If
                                        Exit While
                                    ElseIf index > -1 And matchingDistance < 4 Then
                                        _InternetData = Gb.GetDetail(wurl.Item(index).URL, _ImagePath, _ParserPath, _DownloadImage, _GrabberOverrideLanguage, _GrabberOverridePersonLimit, _GrabberOverrideTitleLimit, _GrabberOverrideGetRoles)
                                        _InternetLookupOK = True
                                        _LastOutputMessage = SearchString & " - " & " Movie found by name match (" & TitleMatch & ") with FuzziDistance = '" & matchingDistance.ToString & "' and Optionsfilter 'on'."
                                        If bgwFolderScanUpdate.CancellationPending = True Then
                                            Exit Sub
                                        End If
                                        Exit While
                                    End If

                                End If

                                ' Check  - only if "Try to find best match automatically" is chosen
                                If _InternetLookupAlwaysPrompt = False Then
                                    If searchyearHint > 0 Then

                                    Else

                                    End If
                                End If

                                '' Check  - only if ""Try to find best match automatically" is chosen
                                'CountTitleMatch = 0
                                'index = 0
                                'indexFirstMatch = 0
                                'titleFirstMatch = ""
                                'titleLastMatch = ""

                                'For i As Integer = 0 To wurl.Count - 1
                                '    wtitle = wurl.Item(i).Title.ToString
                                '    wyear = wurl.Item(i).Year.ToString.Substring(0, 4)
                                '    If Double.TryParse(wyear, dyear) = False Then
                                '        wyear = ""
                                '    End If
                                '    If (wyear = _InternetSearchHintYear And _InternetLookupAlwaysPrompt = False) Then
                                '        If CountTitleMatch = 0 Then
                                '            indexFirstMatch = i
                                '            titleFirstMatch = wtitle
                                '        End If
                                '        CountTitleMatch = CountTitleMatch + 1
                                '        index = i
                                '        titleLastMatch = wtitle
                                '    End If
                                'Next
                                'If (CountTitleMatch = 1 And FuzziDistance(SearchString, wurl.Item(index).Title.ToString) < 5) Then
                                '    _InternetData = Gb.GetDetail(wurl.Item(index).URL, _ImagePath, _ParserPath, _DownloadImage, GrabberOverrideLanguage, _GrabberOverridePersonLimit, _GrabberOverrideTitleLimit, _GrabberOverrideGetRoles)
                                '    _InternetLookupOK = True
                                '    _LastOutputMessage = SearchString & " - " & " Movie found by year hint (" & _InternetSearchHintYear & ") with single match and name match (" & titleFirstMatch & ") with FuzziDistance = '" & FuzziDistance(SearchString, titleFirstMatch).ToString & "'."
                                '    If bgwFolderScanUpdate.CancellationPending = True Then
                                '        Exit Sub
                                '    End If
                                '    Exit While
                                'End If
                                '' more results found
                                'If (CountTitleMatch = 2 And FuzziDistance(SearchString, wurl.Item(index).Title.ToString) < 5) Then
                                '    _InternetData = Gb.GetDetail(wurl.Item(indexFirstMatch).URL, _ImagePath, _ParserPath, _DownloadImage, GrabberOverrideLanguage, _GrabberOverridePersonLimit, _GrabberOverrideTitleLimit, _GrabberOverrideGetRoles)
                                '    _InternetLookupOK = True
                                '    _LastOutputMessage = SearchString & " - " & " Movie found by year hint (" & _InternetSearchHintYear & ") with DOUBLE match and FuzziDistance = '" & FuzziDistance(SearchString, titleFirstMatch).ToString & "' - using first match (" & titleFirstMatch & ")."
                                '    If bgwFolderScanUpdate.CancellationPending = True Then
                                '        Exit Sub
                                '    End If
                                '    Exit While
                                'End If

                                '' Check for name matches (as we didn't have success with year&name matches)
                                'CountTitleMatch = 0
                                'index = 0
                                'TitleMatch = ""
                                'For i As Integer = 0 To wurl.Count - 1
                                '    wtitle = wurl.Item(i).Title.ToString
                                '    wyear = wurl.Item(i).Year.ToString.Substring(0, 4)
                                '    If Double.TryParse(wyear, dyear) = False Then
                                '        wyear = ""
                                '    End If
                                '    If ((wtitle.Contains(SearchString) Or FuzziDistance(SearchString, wurl.Item(i).Title.ToString) < 5) And _InternetLookupAlwaysPrompt = False) Then
                                '        CountTitleMatch = CountTitleMatch + 1
                                '        index = i
                                '        TitleMatch = wtitle
                                '    End If
                                'Next
                                'If (CountTitleMatch = 1) Then
                                '    _InternetData = Gb.GetDetail(wurl.Item(index).URL, _ImagePath, _ParserPath, _DownloadImage, GrabberOverrideLanguage, _GrabberOverridePersonLimit, _GrabberOverrideTitleLimit, _GrabberOverrideGetRoles)
                                '    _InternetLookupOK = True
                                '    _LastOutputMessage = SearchString & " - " & " Movie found by name match (" & TitleMatch & ") with single match."
                                '    If bgwFolderScanUpdate.CancellationPending = True Then
                                '        Exit Sub
                                '    End If
                                '    Exit While
                                'End If

                            End If

                            Dim md As Integer = Integer.MaxValue
                            Dim count As Integer = 0
                            Dim title As String = ""
                            Dim idx = FuzzyMatch(SearchString, wurl, False, 0, 0, md, count, title)
                            _LastOutputMessage = SearchString & " - " & wurl.Count.ToString & " Movies found from Internet lookup - no matching possible. Closest Match Distance: '" & md.ToString & "' with '" & count.ToString & "' matches and match name '" & title & "'."
                            _InternetLookupOK = False
                            Exit While
                            'dtmulti.Rows.Add(FilePath)
                        End If
                    End If
                End While
            End If
        Catch ex As Exception
            _LastOutputMessage = "ErrorEvent : ErrorEvent on Internet Lookup for " & _FileName.ToString & " : " & ex.Message.ToString & " - Stacktrace: " & ex.StackTrace.ToString
        End Try
    End Sub

    Function FuzzyMatch(ByVal searchtitle As String, ByVal wurl As ArrayList, ByVal restrictoptions As Boolean, ByVal searchyear As Double, ByVal yearDistance As Integer, ByRef matchingDistance As Integer, ByRef matchingCount As Integer, ByRef matchingTitle As String) As Integer
        Dim matchingIndex As Integer = -1
        matchingDistance = Integer.MaxValue
        matchingCount = 0
        matchingTitle = ""

        Dim isAmbiguous As Boolean = False

        Dim wyear As String
        Dim wOptions As String
        Dim dyear As Double

        'If _InternetSearchHintYear <> "" And _InternetSearchHintYear.Length >= 4 And wyear <> "" And wyear.Length >= 4 And wOptions = "" Then
        'If ((wtitle.Contains(SearchString) Or FuzziDistance(SearchString, wtitle) < 3) And wyear <> "" And ((wyear - 1) <= _InternetSearchHintYear) And ((wyear + 1) >= _InternetSearchHintYear)) Then


        For index As Integer = 0 To wurl.Count - 1
            wOptions = wurl.Item(index).Options.ToString
            ' calculate year
            If wurl.Item(index).Year.ToString.Length > 3 Then
                wyear = wurl.Item(index).Year.ToString.Substring(0, 4)
                If Double.TryParse(wyear, dyear) = False Then
                    wyear = ""
                    dyear = 0
                End If
            Else
                wyear = ""
                dyear = 0
            End If

            If restrictoptions And wOptions <> "" Then Continue For ' skip Option movies
            If searchyear > 0 And ((dyear > searchyear + yearDistance) Or (dyear < searchyear - yearDistance)) Then Continue For ' skip non year match movies
            'searchtitle = GetSearchString(searchtitle) ' might be unneccessary, as already done by AMCU
            'Dim distance As Integer = Levenshtein.Match(searchtitle, wurl.Item(index).Title.ToString.ToLower())

            ' check main title
            Dim distance As Integer = AdvancedStringComparer.Levenshtein(searchtitle, wurl.Item(index).Title.ToString)
            If (distance = matchingDistance And matchingDistance <> Integer.MaxValue) Then 'check, if a second match with same distance is found - then no valid result (until result with lower matchingdistance))
                isAmbiguous = True
                matchingCount = matchingCount + 1
            End If

            If (distance < matchingDistance) Then
                isAmbiguous = False
                matchingDistance = distance
                matchingIndex = index
                matchingCount = 1
                matchingTitle = wurl.Item(index).Title.ToString & "(" & wurl.Item(index).Year.ToString & ")"
            End If

            ' get and check AKAs if present
            If wurl.Item(index).Akas.ToString <> "" Then
                Dim AKAstring As String = wurl.Item(index).Akas.ToString
                'Dim MatchList As MatchCollection
                'Dim matcher As Match
                'Dim p As New Regex("aka." & Chr(34) & ".*?" & Chr(34) & ".-")
                'MatchList = p.Matches(AKAstring)
                'If MatchList.Count > 0 Then
                '    For Each matcher In MatchList
                '        Dim AKAtitle As String = matcher.Value
                '        Dim AKAdistance As Integer = AdvancedStringComparer.Levenshtein(searchtitle, AKAtitle.ToString)
                '        'If (AKAdistance = matchingDistance And matchingDistance <> Integer.MaxValue) Then
                '        '    isAmbiguous = True
                '        '    matchingCount = matchingCount + 1
                '        'End If

                '        If (AKAdistance < matchingDistance) Then
                '            isAmbiguous = False
                '            matchingDistance = AKAdistance
                '            matchingIndex = index
                '            matchingCount = 1
                '            matchingTitle = AKAtitle & " (AKA) - " & wurl.Item(index).Title.ToString
                '        End If
                '    Next
                'End If
                Dim StringList As String() = AKAstring.Split(New Char() {"|"}, System.StringSplitOptions.RemoveEmptyEntries)
                If StringList.Length > 0 And matchingDistance > 0 Then ' Only, if no perfect match yet done
                    For Each AKAtitle As String In StringList
                        Dim AKAdistance As Integer = AdvancedStringComparer.Levenshtein(searchtitle, AKAtitle.ToString.Trim)
                        'If (AKAdistance = matchingDistance And matchingDistance <> Integer.MaxValue) Then 'check, if a second match with same distance is found - then no valid result (until result with lower matchingdistance))
                        '    isAmbiguous = True
                        '    matchingCount = matchingCount + 1
                        'End If
                        If (AKAdistance < matchingDistance) Then
                            isAmbiguous = False
                            matchingDistance = AKAdistance
                            matchingIndex = index
                            matchingCount = 1
                            matchingTitle = "'" & AKAtitle & "' (AKA) - main title '" & wurl.Item(index).Title.ToString & "(" & wurl.Item(index).Year.ToString & ")" & "'"
                        End If
                        If (AKAtitle.Contains(":")) Then
                            Dim iTrimIndex As Integer = 0
                            iTrimIndex = AKAtitle.IndexOf(":")
                            If iTrimIndex > 0 Then
                                AKAtitle = AKAtitle.Substring(0, iTrimIndex)
                                AKAdistance = AdvancedStringComparer.Levenshtein(searchtitle, AKAtitle.ToString.Trim)
                                If (AKAdistance < matchingDistance) Then
                                    isAmbiguous = False
                                    matchingDistance = AKAdistance
                                    matchingIndex = index
                                    matchingCount = 1
                                    matchingTitle = "'" & AKAtitle & "' (AKA-Trim) - main title '" & wurl.Item(index).Title.ToString & "(" & wurl.Item(index).Year.ToString & ")" & "'"
                                End If
                            End If
                        End If
                    Next
                    'If (AKAdistance = matchingDistance And matchingDistance <> Integer.MaxValue) Then 'check, if a second match with same distance is found - then no valid result (until result with lower matchingdistance))
                    '    isAmbiguous = True
                    '    matchingCount = matchingCount + 1
                    'End If
                End If
            End If
        Next
        If isAmbiguous = True Then
            Return -2           ' this indicates multiple matches
        End If
        Return matchingIndex    ' returns "-1" if no matches
    End Function

    Function GetSearchString(ByVal strMovie As String) As String
        Dim strUrl As String = strMovie
        strUrl = strUrl.Trim()
        Dim rx As New Regex("(([\(\{\[]|\b)((576|720|1080)[pi]|dir(ectors )?cut|dvd([r59]|rip|scr(eener)?)|(avc)?hd|wmv|ntsc|pal|mpeg|dsr|r[1-5]|bd[59]|dts|ac3|blu(-)?ray|[hp]dtv|stv|hddvd|xvid|divx|x264|dxva|(?-i)FEST[Ii]VAL|L[iI]M[iI]TED|[WF]S|PROPER|REPACK|RER[Ii]P|REAL|RETA[Ii]L|EXTENDED|REMASTERED|UNRATED|CHRONO|THEATR[Ii]CAL|DC|SE|UNCUT|[Ii]NTERNAL|V\d{1}|BR[Rr]ip|[DS]UBBED)([\]\)\}]|\b)(-[^\s]+$)?)", RegexOptions.IgnoreCase)
        strUrl = rx.Replace(strUrl, "").Replace(".", " ").Replace("_", " ").Trim()
        Return strUrl
    End Function

    Function CloseEnough(ByVal searchtitle As String, ByVal movietitle As String) As Boolean
        Const AutoApprovalTreshold As Integer = 2
        Dim distance As Integer
        distance = AdvancedStringComparer.Levenshtein(searchtitle, movietitle)
        If (distance <= AutoApprovalTreshold = True) Then
            Return True
        End If
        'For Each currAltTitle As String in movie.AlternateTitles) DO
        '    distance = Grabber.GrabUtil.AdvancedStringComparer.Levenshtein(title, currAltTitle)

        '    If (distance <= MovingPicturesCore.Settings.AutoApproveThreshold) Then
        '        Return True
        '    End If
        'Next
        Return False
    End Function

    Function FuzziDistance(ByVal searchtitle As String, ByVal movietitle As String) As Integer
        Return AdvancedStringComparer.Levenshtein(searchtitle, movietitle)
    End Function

    Public Sub ProcessFile(ByVal ProcessMode As Process_Mode_Names)

        _LastOutputMessage = ""
        'LogEvent("ProcessFile() - Start !", EventLogLevel.InformationalWithGrabbing)
        Try
            'Dim attr As Xml.XmlAttribute
            'Dim element As Xml.XmlElement
            Dim CurrentAttribute As String
            Dim TempValue As String

            Dim title As String = ""
            Dim ttitle As String = ""
            Dim director As String = ""
            Dim year As Int16 = 0
            Dim imdb_id As String = ""

            'First ensure we have a valid movie number so the record can be saved:
            CurrentAttribute = "Number"
            TempValue = _MovieNumber
            If _XMLElement.Attributes(CurrentAttribute) Is Nothing Then ' only update when there is no content
                CreateOrUpdateAttribute(CurrentAttribute, TempValue, ProcessMode)
            ElseIf ProcessMode = Process_Mode_Names.Update Then
                _MovieNumber = Integer.Parse(_XMLElement.Attributes(CurrentAttribute).Value)
            End If
            'LogEvent("ProcessFile() - get valid record number: '" & TempValue & "'", EventLogLevel.InformationalWithGrabbing)

            If (ProcessMode = Process_Mode_Names.Import) Then
                'Second get a decent Movie Title which we can then use for Internet Lookups as well as the Original Title field.
                'LogEvent("ProcessFile() - Import: Get search & matching hints...", EventLogLevel.InformationalWithGrabbing)

                CurrentAttribute = "OriginalTitle" 'add a test for manual update when no file specified => Internet lookup with OriginalTitle     
                If (_FilePath.Length > 0) Then
                    TempValue = GetTitleFromFilePath(_FilePath)
                    title = TempValue
                    'LogEvent("ProcessFile() - Import - hints - title: '" & title & "'", EventLogLevel.InformationalWithGrabbing)
                    If IsUpdateRequested(CurrentAttribute, ProcessMode) = True Then
                        CreateOrUpdateAttribute(CurrentAttribute, TempValue, ProcessMode)
                    End If
                End If

                CurrentAttribute = "Year" 'try to get year from filepath/name 
                If (_FilePath.Length > 0) Then
                    TempValue = GetYearFromFilePath(_FilePath)
                    _InternetSearchHintYear = TempValue
                    If IsUpdateRequested(CurrentAttribute, ProcessMode) = True Then
                        CreateOrUpdateAttribute(CurrentAttribute, TempValue, ProcessMode)
                    End If
                End If

                CurrentAttribute = "IMDB_Id" 'try to get IMDB Id from filepath/name
                If (_FilePath.Length > 0) Then
                    TempValue = GetIMDBidFromFilePath(_FilePath)
                    _InternetSearchHintIMDB_Id = TempValue
                    If IsUpdateRequested(CurrentAttribute, ProcessMode) = True Then
                        CreateOrUpdateElement(CurrentAttribute, TempValue, ProcessMode)
                    End If
                    TempValue = ""
                End If
            End If

            If IsInternetLookupNeeded() = True Then
                ' set the search hints for update mode (AMC, IMDB, Year)
                If ProcessMode = Process_Mode_Names.Update Then
                    Dim wdirector As String
                    If _XMLElement.Attributes("Director") IsNot Nothing Then
                        If _XMLElement.Attributes("Director").Value.Length > 0 Then
                            wdirector = System.Text.RegularExpressions.Regex.Replace(_XMLElement.Attributes("Director").Value, "\b(and|und|en|et|y|&)\b", ",")
                            If wdirector.IndexOf(",") > 0 Then
                                _InternetSearchHint = wdirector.Substring(0, wdirector.IndexOf(",") - 1).Trim()
                            Else
                                _InternetSearchHint = wdirector.Trim()
                            End If
                        End If
                    End If

                    If _XMLElement.Attributes("Year") IsNot Nothing Then
                        _InternetSearchHintYear = _XMLElement.Attributes("Year").Value.ToString
                    End If

                    If _XMLElement.Item("IMDB_Id") IsNot Nothing Then
                        If _XMLElement.Item("IMDB_Id").InnerText.Length > 0 Then
                            _InternetSearchHintIMDB_Id = _XMLElement.Item("IMDB_Id").InnerText.ToString
                        ElseIf _XMLElement.Attributes("URL") IsNot Nothing Then
                            If _XMLElement.Attributes("URL").Value.Length > 0 Then
                                Dim URL As String = _XMLElement.Attributes("URL").Value.ToString
                                Dim IMDB As String = GetIMDBidFromFilePath(URL)
                                If IMDB.Length > 0 Then
                                    _InternetSearchHintIMDB_Id = IMDB
                                ElseIf (_FilePath.Length > 0) Then 'try to get IMDB Id from filepath/name
                                    IMDB = GetIMDBidFromFilePath(_FilePath)
                                    If IMDB.Length > 0 Then
                                        _InternetSearchHintIMDB_Id = IMDB
                                    End If
                                End If
                            End If
                        End If
                    End If
                End If

                ' Get Internetdata with "best title possible"
                If IsValidTitle(_XMLElement, "FormattedTitle") Then
                    DoInternetLookup(RemoveGroupNameAndEdition(_XMLElement.Attributes("FormattedTitle").Value.ToString, GetEdition(_FilePath)))
                ElseIf IsValidTitle(_XMLElement, "TranslatedTitle") Then
                    DoInternetLookup(RemoveGroupNameAndEdition(_XMLElement.Attributes("TranslatedTitle").Value.ToString, GetEdition(_FilePath)))
                ElseIf IsValidTitle(_XMLElement, "OriginalTitle") Then
                    DoInternetLookup(RemoveGroupNameAndEdition(_XMLElement.Attributes("OriginalTitle").Value.ToString, GetEdition(_FilePath)))
                Else
                    DoInternetLookup(GetTitleFromFilePath(_FilePath)) 'No DB title available, so use the cleaned filename instead:
                End If
            End If

            ' Check, if internetlookup has given proper title name - otherwise set to failed
            If _InternetLookupOK = True Then
                TempValue = _InternetData(Grabber_Output.OriginalTitle)
                If TempValue Is Nothing Or TempValue = "" Then
                    TempValue = _InternetData(Grabber_Output.TranslatedTitle)
                    If TempValue Is Nothing Or TempValue = "" Then
                        _InternetLookupOK = False
                        _LastOutputMessage = "ErrorEvent : ErrorEvent importing " & _FileName.ToString & " : Matching the movie was successful, but grabber failed getting movie details data (title)"
                    End If
                End If
            End If

            ' Now update all requested fields ...

            CurrentAttribute = "OriginalTitle" 'Now update the Original Title with the Internet value, if set to do so:
            If IsUpdateRequested(CurrentAttribute, ProcessMode) = True Then
                TempValue = GetTitleFromFilePath(_FilePath)
                If _InternetLookupOK = True Then
                    If _MovieTitleHandling.Contains("Internet Lookup") = True Then
                        TempValue = _InternetData(Grabber_Output.OriginalTitle)
                        title = TempValue
                    End If
                End If
                TempValue = AddGroupName(TempValue, "Original Title") 'Check to see if there's a group name attached to this, and apply it.
                TempValue = AddEdition(TempValue, "Original Title") 'Add Edition, if available and requested
                CreateOrUpdateAttribute(CurrentAttribute, TempValue, ProcessMode)
            End If

            CurrentAttribute = "TranslatedTitle"
            If IsUpdateRequested(CurrentAttribute, ProcessMode) = True Then
                If _InternetLookupOK = True Then
                    TempValue = _InternetData(Grabber_Output.TranslatedTitle)
                Else
                    TempValue = GetTitleFromFilePath(_FilePath)
                End If
                TempValue = AddGroupName(TempValue, "Translated Title")
                TempValue = AddEdition(TempValue, "Translated Title")
                CreateOrUpdateAttribute(CurrentAttribute, TempValue, ProcessMode)
            End If


            ' Guzzi: Original Code does remove old entries with same otitle name
            'Try to see if entry already exist with empty movie filename => delete the new entry and update existing one
            If _DatabaseFields("originaltitle") = True Or _DatabaseFields("translatedtitle") = True Then
                If _DatabaseFields("originaltitle") = True Then
                    _XMLElement = VerifyElement(_XMLElement.Attributes("OriginalTitle").Value.ToString, _XMLElement)
                End If
            ElseIf ProcessMode = Process_Mode_Names.Import Then
                _LastOutputMessage = "ErrorEvent : ErrorEvent importing " & _FileName.ToString & " : No originaltitle or translatedtitle activated"
            End If

            CurrentAttribute = "FormattedTitle"
            If IsUpdateRequested(CurrentAttribute, ProcessMode) = True Then
                If _XMLElement.Attributes("TranslatedTitle") IsNot Nothing Then
                    TempValue = Grabber.GrabUtil.TitleToArchiveName(RemoveGroupNameAndEdition(_XMLElement.Attributes("TranslatedTitle").Value.ToString, GetEdition(_FilePath)))
                    TempValue = AddGroupName(TempValue, "Translated Title")
                    TempValue = AddEdition(TempValue, "Translated Title")
                ElseIf _XMLElement.Attributes("OriginalTitle") IsNot Nothing Then
                    TempValue = Grabber.GrabUtil.TitleToArchiveName(_XMLElement.Attributes("OriginalTitle").Value)
                    TempValue = AddGroupName(TempValue, "Original Title")
                    TempValue = AddEdition(TempValue, "Original Title")
                End If
                CreateOrUpdateAttribute(CurrentAttribute, TempValue, ProcessMode)
            End If

            CurrentAttribute = "Date"
            If (_FilePath.Length > 0) And IsUpdateRequested(CurrentAttribute, ProcessMode) = True Then
                TempValue = GetFileData(_FilePath, "Date")
                CreateOrUpdateAttribute(CurrentAttribute, TempValue, ProcessMode)
            End If

            CurrentAttribute = "Checked"
            If IsUpdateRequested(CurrentAttribute, ProcessMode) = True Then
                TempValue = CurrentSettings.Check_Field_Handling.ToString
                CreateOrUpdateAttribute(CurrentAttribute, TempValue, ProcessMode)
            End If

            CurrentAttribute = "MediaLabel"
            If (_FilePath.Length > 0) And IsUpdateRequested(CurrentAttribute, ProcessMode) = True Then
                If _Read_DVD_Label = True Then
                    Dim DriveLabel As String = String.Empty
                    DriveLabel = DrvLabel(_FilePath)

                    If DriveLabel = String.Empty Then
                        TempValue = _MediaLabel
                    Else
                        TempValue = DriveLabel
                    End If
                Else
                    TempValue = _MediaLabel
                End If
                CreateOrUpdateAttribute(CurrentAttribute, TempValue, ProcessMode)
            End If

            CurrentAttribute = "MediaType"
            If Not String.IsNullOrEmpty(_MediaType) And IsUpdateRequested(CurrentAttribute, ProcessMode) = True Then
                TempValue = _MediaType
                CreateOrUpdateAttribute(CurrentAttribute, TempValue, ProcessMode)
            End If

            CurrentAttribute = _SourceField ' Sourcefile - field depends on configuration - always update, if mode is "import"
            If Not (CurrentAttribute = "(none)" Or String.IsNullOrEmpty(CurrentAttribute)) And (IsUpdateRequested(CurrentAttribute, ProcessMode) Or ProcessMode = Process_Mode_Names.Import) Then
                If (_FilePath.Length > 0) Then
                    If Not String.IsNullOrEmpty(_OverridePath) Then
                        TempValue = _OverridePath
                    Else
                        TempValue = _FilePath
                    End If
                    If CurrentSettings.Store_Short_Names_Only Then
                        If TempValue.Contains("\") Then
                            TempValue = TempValue.Substring(TempValue.LastIndexOf("\") + 1)
                        End If
                    End If
                    CreateOrUpdateAttribute(CurrentAttribute, TempValue, ProcessMode)
                End If
            End If

            CurrentAttribute = "Subtitles"
            If (_FilePath.Length > 0) And IsUpdateRequested(CurrentAttribute, ProcessMode) = True Then
                TempValue = GetFileData(_FilePath, "textstreamlanguagelist")
                CreateOrUpdateAttribute(CurrentAttribute, TempValue, ProcessMode)
            End If

            CurrentAttribute = "Languages"
            If (_FilePath.Length > 0) And IsUpdateRequested(CurrentAttribute, ProcessMode) = True Then
                If Not _Use_InternetData_For_Languages = True Then
                    TempValue = GetFileData(_FilePath, "audiostreamlanguagelist")
                    CreateOrUpdateAttribute(CurrentAttribute, TempValue, ProcessMode)
                End If
            End If

            CurrentAttribute = "Resolution"
            If (_FilePath.Length > 0) And IsUpdateRequested(CurrentAttribute, ProcessMode) = True Then
                TempValue = GetFileData(_FilePath, "Resolution")
                CreateOrUpdateAttribute(CurrentAttribute, TempValue, ProcessMode)
            End If

            CurrentAttribute = "Length"
            If IsUpdateRequested(CurrentAttribute, ProcessMode) = True Then
                'TempValue = fnGetFileData(_FilePath, "Runtime")
                If _AllFilesPath <> "" Then
                    For Each wfile As String In _AllFilesPath.Split(";")
                        If GetFileData(wfile, "runtime") <> "" And Not GetFileData(wfile, "runtime").Contains("ErrorEvent") Then
                            If TempValue = "" Then
                                TempValue = GetFileData(wfile, "runtime")
                            Else
                                TempValue = CLng(TempValue) + GetFileData(wfile, "runtime")
                            End If
                            'Diskcount += 1
                        End If
                    Next
                Else
                    TempValue = GetFileData(_FilePath, "runtime")
                End If

				If _InternetLookupOK = True Then
					If TempValue.Contains("ErrorEvent") Then
						TempValue = _InternetData(Grabber_Output.Runtime)
					End If
				End If

                If Not TempValue.Contains("ErrorEvent") Then
                    CreateOrUpdateAttribute(CurrentAttribute, TempValue, ProcessMode)
                End If
            End If

            CurrentAttribute = "VideoFormat"
            If (_FilePath.Length > 0) And IsUpdateRequested(CurrentAttribute, ProcessMode) = True Then
                TempValue = GetFileData(_FilePath, "VideoFormat")
                CreateOrUpdateAttribute(CurrentAttribute, TempValue, ProcessMode)
            End If

            CurrentAttribute = "VideoBitrate"
            If (_FilePath.Length > 0) And IsUpdateRequested(CurrentAttribute, ProcessMode) = True Then
                TempValue = GetFileData(_FilePath, "VideoBitrate")
                CreateOrUpdateAttribute(CurrentAttribute, TempValue, ProcessMode)
            End If

            CurrentAttribute = "AudioFormat"
            If (_FilePath.Length > 0) And IsUpdateRequested(CurrentAttribute, ProcessMode) = True Then
                TempValue = GetFileData(_FilePath, "AudioFormat")
                CreateOrUpdateAttribute(CurrentAttribute, TempValue, ProcessMode)
            End If

            CurrentAttribute = "AudioBitrate"
            If (_FilePath.Length > 0) And IsUpdateRequested(CurrentAttribute, ProcessMode) = True Then
                TempValue = GetFileData(_FilePath, "AudioBitrate")
                CreateOrUpdateAttribute(CurrentAttribute, TempValue, ProcessMode)
            End If

            CurrentAttribute = "AudioChannelCount"
            If (_FilePath.Length > 0) And IsUpdateRequested(CurrentAttribute, ProcessMode) = True Then
                TempValue = GetFileData(_FilePath, "AudioChannelCount")
                CreateOrUpdateElement(CurrentAttribute, TempValue, ProcessMode)
            End If

            CurrentAttribute = "Aspectratio"
            If IsUpdateRequested(CurrentAttribute, ProcessMode) = True Then
                If (_FilePath.Length > 0) Then
                    TempValue = GetFileData(_FilePath, "Aspectratio")
                Else
                    If _XMLElement.Attributes("Resolution") IsNot Nothing AndAlso _XMLElement.Attributes("Resolution").Value.ToString.Length > 0 Then
                        Dim aspectratio As Decimal
                        Dim Resolution As String = _XMLElement.Attributes("Resolution").Value.ToString
                        Try
                            If Not Decimal.TryParse(Resolution, aspectratio) Then
                                Dim arSplit() As String = Resolution.Split(New String() {"x"}, StringSplitOptions.RemoveEmptyEntries)
                                aspectratio = Math.Round(Decimal.Divide(Convert.ToInt32(arSplit(0)), Convert.ToInt32(arSplit(1))), 2)
                            End If
                            TempValue = aspectratio.ToString(CultureInfo.InvariantCulture)
                        Catch ex As Exception
                            TempValue = ""
                        End Try
                    Else
                        TempValue = ""
                    End If
                End If
                CreateOrUpdateElement(CurrentAttribute, TempValue, ProcessMode)
            End If

            CurrentAttribute = "Framerate"
            If (_FilePath.Length > 0) And IsUpdateRequested(CurrentAttribute, ProcessMode) = True Then
                TempValue = GetFileData(_FilePath, "Framerate")
                CreateOrUpdateAttribute(CurrentAttribute, TempValue, ProcessMode)
            End If

            CurrentAttribute = "Size"
            If IsUpdateRequested(CurrentAttribute, ProcessMode) = True Then
                If _AllFilesPath <> "" Then
                    For Each wfile As String In _AllFilesPath.Split(";")
                        If GetFileData(wfile, "FileSize") <> "" And Not GetFileData(wfile, "FileSize").Contains("ErrorEvent") Then
                            If TempValue = "" Then
                                TempValue = GetFileData(wfile, "FileSize")
                            Else
                                TempValue = CLng(TempValue) + GetFileData(wfile, "FileSize")
                            End If
                        End If
                    Next
                Else
                    TempValue = GetFileData(_FilePath, "FileSize")
                End If

                If Not TempValue.Contains("ErrorEvent") Then
                    CreateOrUpdateAttribute(CurrentAttribute, TempValue, ProcessMode)
                End If
            End If

            CurrentAttribute = "Disks"
            If IsUpdateRequested(CurrentAttribute, ProcessMode) = True Then
                Dim Diskcount As Integer = 0
                If _AllFilesPath <> "" Then
                    For Each wfile As String In _AllFilesPath.Split(";")
                        Diskcount += 1
                    Next
                Else
                    Diskcount = 1
                End If
                TempValue = Diskcount.ToString
                CreateOrUpdateAttribute(CurrentAttribute, TempValue, ProcessMode)
            End If

            CurrentAttribute = "Edition" ' Get "Edition" from filename for separate field
            If IsUpdateRequested(CurrentAttribute, ProcessMode) = True Then
                TempValue = GetEdition(_FilePath)
                CreateOrUpdateElement(CurrentAttribute, TempValue, ProcessMode)
            End If


            If _InternetLookupOK = False Then

                'Additional attempt to load picture with folder.jpg settings, in case Internet lookup fails
                CurrentAttribute = "Picture"
                If IsUpdateRequested(CurrentAttribute, ProcessMode) = True Then
                    Dim CoverFileExists As Boolean = False
                    Dim Filename As String = _FilePath ' media file
                    Dim PicturePathToSave As String = String.Empty ' the strintg to save in DB field Pictures = "..."

                    If CurrentSettings.Create_Cover_From_Movie = True Then ' create missing covers by thumbnailer
                        'Not using folder.jpg - use default location instead (with the xml file, maybe using override path) _ImagePath
                        CoverFileExists = CreateCoverFromMovie(Filename, PicturePathToSave)
                    ElseIf CurrentSettings.Use_Folder_Dot_Jpg = True Then
                        Dim NewCoverThumbName As String = _FilePath
                        If NewCoverThumbName.Contains("\") = True Then
                            NewCoverThumbName = NewCoverThumbName.Substring(0, NewCoverThumbName.LastIndexOf("\"))
                        End If
                        NewCoverThumbName += "\folder.jpg"
                        Try
                            If File.Exists(NewCoverThumbName) Then
                                CoverFileExists = True
                                PicturePathToSave = NewCoverThumbName
                            End If
                        Catch ex As Exception
                        End Try
                    End If

                    ' now update with either folder.jpg or filename.jpg
                    If CoverFileExists = True And PicturePathToSave <> "" Then
                        TempValue = PicturePathToSave
                        CreateOrUpdateAttribute(CurrentAttribute, TempValue, ProcessMode)
                    End If
                End If


            Else 'Load all the Internet data...

                title = _InternetData(Grabber_Output.OriginalTitle)

                ' Guzzi: Update Languages, if it shouold get internet data
                CurrentAttribute = "Languages"
                If IsUpdateRequested(CurrentAttribute, ProcessMode) = True Then
                    If _Use_InternetData_For_Languages = True Then
                        TempValue = _InternetData(Grabber_Output.Language)
                        CreateOrUpdateAttribute(CurrentAttribute, TempValue, ProcessMode)
                    End If
                End If

                CurrentAttribute = "Year"
                If IsUpdateRequested(CurrentAttribute, ProcessMode) = True Then
                    TempValue = _InternetData(Grabber_Output.Year)
                    CreateOrUpdateAttribute(CurrentAttribute, TempValue, ProcessMode)
                End If

                CurrentAttribute = "Country"
                If IsUpdateRequested(CurrentAttribute, ProcessMode) = True Then
                    TempValue = _InternetData(Grabber_Output.Country)
                    CreateOrUpdateAttribute(CurrentAttribute, TempValue, ProcessMode)
                End If

                CurrentAttribute = "Category"
                If IsUpdateRequested(CurrentAttribute, ProcessMode) = True Then
                    TempValue = _InternetData(Grabber_Output.Category)
                    CreateOrUpdateAttribute(CurrentAttribute, TempValue, ProcessMode)
                End If

                CurrentAttribute = "URL"
                If IsUpdateRequested(CurrentAttribute, ProcessMode) = True Then
                    TempValue = _InternetData(Grabber_Output.URL)
                    CreateOrUpdateAttribute(CurrentAttribute, TempValue, ProcessMode)
                End If

                CurrentAttribute = "Rating"
                If IsUpdateRequested(CurrentAttribute, ProcessMode) = True Then
                    TempValue = _InternetData(Grabber_Output.Rating)
                    CreateOrUpdateAttribute(CurrentAttribute, TempValue, ProcessMode)
                End If

                CurrentAttribute = "Director"
                If IsUpdateRequested(CurrentAttribute, ProcessMode) = True Then
                    TempValue = _InternetData(Grabber_Output.Director)
                    CreateOrUpdateAttribute(CurrentAttribute, TempValue, ProcessMode)
                End If

                CurrentAttribute = "Producer"
                If IsUpdateRequested(CurrentAttribute, ProcessMode) = True Then
                    TempValue = _InternetData(Grabber_Output.Producer)
                    CreateOrUpdateAttribute(CurrentAttribute, TempValue, ProcessMode)
                End If

                CurrentAttribute = "Actors"
                If IsUpdateRequested(CurrentAttribute, ProcessMode) = True Then
                    TempValue = _InternetData(Grabber_Output.Actors)
                    CreateOrUpdateAttribute(CurrentAttribute, TempValue, ProcessMode)
                End If

                CurrentAttribute = "Description"
                If IsUpdateRequested(CurrentAttribute, ProcessMode) = True Then
                    TempValue = _InternetData(Grabber_Output.Description)
                    CreateOrUpdateAttribute(CurrentAttribute, TempValue, ProcessMode)
                End If

                CurrentAttribute = "Comments"
                If IsUpdateRequested(CurrentAttribute, ProcessMode) = True Then
                    TempValue = _InternetData(Grabber_Output.Comments)
                    CreateOrUpdateAttribute(CurrentAttribute, TempValue, ProcessMode)
                End If

                CurrentAttribute = "Picture"
                If IsUpdateRequested(CurrentAttribute, ProcessMode) = True Then
                    If CurrentSettings.Use_Folder_Dot_Jpg = True Then
                        'First check to see if the file exists in a nice safe way:
                        Dim FileExists As Boolean = False
                        Dim FileIsLinked As Boolean = False
                        Dim NewFileName As String = _FilePath
                        If NewFileName.Contains("\") = True Then
                            NewFileName = NewFileName.Substring(0, NewFileName.LastIndexOf("\"))
                        End If
                        NewFileName += "\folder.jpg"
                        Try
                            If File.Exists(NewFileName) Then
                                FileExists = True
                            End If
                        Catch ex As Exception

                        End Try

                        'Then check if the current record already has a reference to that file:
                        If _XMLElement.Attributes(CurrentAttribute) IsNot Nothing Then
                            If _XMLElement.Attributes(CurrentAttribute).Value.ToLower = NewFileName.ToLower Then
                                FileIsLinked = True
                            End If
                        End If


                        If ProcessMode = Process_Mode_Names.Import Then
                            'Import run - use existing folder.jpg if present; add it if not.
                            If FileExists = True Then
                                'It's there; use it.  Delete the one we just downloaded, if needed:
                                If _InternetData(Grabber_Output.PicturePathLong) <> String.Empty And File.Exists(Grabber_Output.PicturePathLong) Then
                                    File.Delete(_InternetData(Grabber_Output.PicturePathLong))
                                End If
                            Else
                                'If we have anything from the grabber, copy it and use that.
                                If _InternetData(Grabber_Output.PicturePathLong) <> String.Empty And File.Exists(Grabber_Output.PicturePathLong) Then
                                    File.Copy(_InternetData(Grabber_Output.PicturePathLong), NewFileName)
                                    File.Delete(_InternetData(Grabber_Output.PicturePathLong))
                                    FileExists = True
                                End If
                            End If
                        Else
                            'Manual Update - have to assume we'll overwrite the existing folder.jpg with the new one, if possible.
                            If FileExists = True Then
                                'check it's used; if not just use it and we're done.
                                If FileIsLinked = True Then
                                    'It's already in use, so update it:
                                    If _InternetData(Grabber_Output.PicturePathShort) <> String.Empty And File.Exists(Grabber_Output.PicturePathLong) Then
                                        If FileExists = True Then
                                            System.IO.File.Delete(NewFileName)
                                        End If
                                        File.Copy(_InternetData(Grabber_Output.PicturePathLong), NewFileName)
                                        File.Delete(_InternetData(Grabber_Output.PicturePathLong))
                                        FileExists = True
                                    End If
                                End If
                            Else
                                'Try and get a new one:
                                If _InternetData(Grabber_Output.PicturePathShort) <> String.Empty And File.Exists(Grabber_Output.PicturePathLong) Then
                                    If FileExists = True Then
                                        System.IO.File.Delete(NewFileName)
                                    End If
                                    File.Copy(_InternetData(Grabber_Output.PicturePathLong), NewFileName)
                                    File.Delete(_InternetData(Grabber_Output.PicturePathLong))
                                    FileExists = True
                                End If
                            End If
                        End If

                        'Now check if we have a valid link; and use it.
                        If FileExists = True Then
                            TempValue = NewFileName
                            CreateOrUpdateAttribute(CurrentAttribute, TempValue, ProcessMode)
                        End If

                    Else
                        'Not using folder.jpg - use default location instead (with the xml file, maybe using override path)
                        If _InternetData(Grabber_Output.PicturePathShort) <> String.Empty Then
                            Dim PicturePathPrefix As String = CurrentSettings.Image_Download_Filename_Prefix.ToString 'Covers\'
                            Dim PicturePathSuffix As String = CurrentSettings.Image_Download_Filename_Suffix.ToString '_(%year%)' - supports placeholder, like e.g. year, number
                            Dim PicturePathFull As String = _InternetData(Grabber_Output.PicturePathLong) 'C:\Ant Movie Catalog\2001_ A Space Odyssey.jpg
                            Dim PictureFileName As String = _InternetData(Grabber_Output.PicturePathShort) '2001_ A Space Odyssey.jpg
                            Dim PictureFileNameWithSuffix As String = Path.GetFileNameWithoutExtension(PictureFileName) + PictureNameWithSuffix(PicturePathSuffix, _InternetData(Grabber_Output.Year), _MovieNumber.ToString()) + Path.GetExtension(PictureFileName)
                            Dim PicturePathToSave As String = String.Empty


                            'Check, if the returned picture is existing - it might not due to download errors like 404
                            If System.IO.File.Exists(PicturePathFull) Then
                                'Separate the folder from the prefix string (if needed)
                                Dim PrefixString As String = String.Empty
                                Dim SuffixString As String = PictureNameWithSuffix(PicturePathSuffix, _InternetData(Grabber_Output.Year), _MovieNumber.ToString())
                                Dim PrefixPath As String = String.Empty
                                If PicturePathPrefix <> String.Empty Then
                                    If PicturePathPrefix.Contains("\") = True Then
                                        PrefixPath = PicturePathPrefix.Substring(0, PicturePathPrefix.LastIndexOf("\") + 1)
                                        PrefixString = PicturePathPrefix.Substring(PicturePathPrefix.LastIndexOf("\") + 1)
                                    Else
                                        PrefixString = PicturePathPrefix
                                    End If
                                End If

                                Dim NewFileName As String = String.Empty
                                If PrefixString <> String.Empty Or SuffixString <> String.Empty Then
                                    'Need to rename the file.
                                    NewFileName = PicturePathFull.Replace(PictureFileName, PrefixString & PictureFileNameWithSuffix)
                                    If Not File.Exists(NewFileName) Then
                                        File.Copy(PicturePathFull, NewFileName)
                                        Thread.Sleep(20)
                                    End If
                                    File.Delete(PicturePathFull)
                                    PicturePathFull = NewFileName
                                    PictureFileName = PrefixString & PictureFileNameWithSuffix
                                End If

                                If PrefixPath <> String.Empty Then
                                    'Need to add the new folder to the short (relative) path:
                                    PictureFileName = PrefixPath & PictureFileName
                                End If

                                If CurrentSettings.Store_Image_With_Relative_Path = True Then
                                    PicturePathToSave = PictureFileName
                                Else
                                    PicturePathToSave = PicturePathFull
                                End If

                                If PicturePathToSave <> String.Empty Then
                                    TempValue = PicturePathToSave
                                    CreateOrUpdateAttribute(CurrentAttribute, TempValue, ProcessMode)
                                End If
                            End If
                        Else
                            Dim CoverFileExists As Boolean = False
                            Dim Filename As String = _FilePath ' media file
                            Dim PicturePathToSave As String = String.Empty ' the strintg to save in DB field Pictures = "..."

                            If CurrentSettings.Create_Cover_From_Movie = True Then ' create missing covers by thumbnailer
                                CoverFileExists = CreateCoverFromMovie(Filename, PicturePathToSave)
                            ElseIf CurrentSettings.Use_Folder_Dot_Jpg = True Then
                                Dim NewCoverThumbName As String = _FilePath
                                If NewCoverThumbName.Contains("\") = True Then
                                    NewCoverThumbName = NewCoverThumbName.Substring(0, NewCoverThumbName.LastIndexOf("\"))
                                End If
                                NewCoverThumbName += "\folder.jpg"
                                Try
                                    If File.Exists(NewCoverThumbName) Then
                                        CoverFileExists = True
                                        PicturePathToSave = NewCoverThumbName
                                    End If
                                Catch ex As Exception
                                End Try
                            End If
                            If PicturePathToSave <> String.Empty Then
                                TempValue = PicturePathToSave
                                CreateOrUpdateAttribute(CurrentAttribute, TempValue, ProcessMode)
                            End If
                        End If
                    End If
                End If


                CurrentAttribute = "Certification"
                If IsUpdateRequested(CurrentAttribute, ProcessMode) = True Then
                    TempValue = _InternetData(Grabber_Output.Certification)
                    CreateOrUpdateElement(CurrentAttribute, TempValue, ProcessMode)
                End If

                CurrentAttribute = "Writer"
                If IsUpdateRequested(CurrentAttribute, ProcessMode) = True Then
                    TempValue = _InternetData(Grabber_Output.Writer)
                    CreateOrUpdateElement(CurrentAttribute, TempValue, ProcessMode)
                End If

                CurrentAttribute = "TagLine"
                If IsUpdateRequested(CurrentAttribute, ProcessMode) = True Then
                    TempValue = _InternetData(Grabber_Output.Tagline)
                    CreateOrUpdateElement(CurrentAttribute, TempValue, ProcessMode)
                End If

                CurrentAttribute = "IMDB_Id"
                If IsUpdateRequested(CurrentAttribute, ProcessMode) = True Then
                    TempValue = _InternetData(Grabber_Output.IMDB_Id)
                    CreateOrUpdateElement(CurrentAttribute, TempValue, ProcessMode)
                End If

                CurrentAttribute = "IMDB_Rank"
                If IsUpdateRequested(CurrentAttribute, ProcessMode) = True Then
                    TempValue = _InternetData(Grabber_Output.IMDB_Rank)
                    CreateOrUpdateElement(CurrentAttribute, TempValue, ProcessMode)
                End If

                CurrentAttribute = "TMDB_Id"
                If IsUpdateRequested(CurrentAttribute, ProcessMode) = True Then
                    TempValue = _InternetData(Grabber_Output.TMDB_Id)
                    CreateOrUpdateElement(CurrentAttribute, TempValue, ProcessMode)
                End If

                CurrentAttribute = "Studio"
                If IsUpdateRequested(CurrentAttribute, ProcessMode) = True Then
                    TempValue = _InternetData(Grabber_Output.Studio)
                    CreateOrUpdateElement(CurrentAttribute, TempValue, ProcessMode)
                End If

                CurrentAttribute = "Edition" ' Disabled, as we use edition from local media file in noninternet section abovedone in separate section
                If IsUpdateRequested(CurrentAttribute, ProcessMode) = True Then
                    'TempValue = _InternetData(Grabber_Output.Edition)
                    'TempValue = GetEdition(_FilePath, CurrentSettings.Movie_Title_Handling)
                    'CreateOrUpdateElement(CurrentAttribute, TempValue, ProcessMode) 
                End If

            End If

            'get fanart
            CurrentAttribute = "Fanart"
            If IsUpdateRequested(CurrentAttribute, ProcessMode) = True Then ' Old: If _DatabaseFields(CurrentAttribute.ToLower) = True Then

                Dim fanartTitle As String = ""
                Dim ftitle As String = ""
                fanartTitle = GetFanartTitle(_XMLElement, title, ttitle, ftitle, year, director)

                If fanartTitle.Length > 0 And Not String.IsNullOrEmpty(CurrentSettings.Movie_Fanart_Path) Then
                    If _InternetLookupOK = True And CurrentSettings.Prohibit_Internet_Lookup = False And Not CurrentSettings.Use_Folder_Dot_Jpg = True Then
                        Dim fanart As List(Of Grabber.DBMovieInfo)
                        Dim Gb As Grabber.Grabber_URLClass = New Grabber.Grabber_URLClass
                        fanart = Gb.GetFanart(title, ttitle, year, director, _InternetSearchHintIMDB_Id, CurrentSettings.Movie_Fanart_Path, True, False, CurrentSettings.Master_Title, CurrentSettings.Movie_PersonArtwork_Path, CurrentSettings.Movie_Fanart_Number_Limit, CurrentSettings.Movie_Fanart_Resolution_Min, CurrentSettings.Movie_Fanart_Resolution_Max)
                        If fanart.Count = 1 Then
                            If fanart(0).Backdrops.Count > 0 Then
                                TempValue = fanart(0).Backdrops(0).ToString
                                If String.IsNullOrEmpty(TempValue) = False Then
                                    CreateOrUpdateElement(CurrentAttribute, TempValue, ProcessMode)
                                End If
                            End If
                            If CurrentSettings.Movie_PersonArtwork_Path.Length > 0 And System.IO.Directory.Exists(CurrentSettings.Movie_PersonArtwork_Path) Then
                                Dim filenameperson As String = String.Empty
                                Dim listepersons As List(Of Grabber.DBPersonInfo) = fanart(0).Persons
                                For Each person As Grabber.DBPersonInfo In listepersons
                                    Dim TheMoviedb As New Grabber.TheMoviedb()
                                    Dim persondetails As Grabber.DBPersonInfo = TheMoviedb.getPersonsById(person.Id, String.Empty)
                                    If persondetails.Images.Count > 0 Then
                                        For Each image As String In persondetails.Images
                                            If System.IO.File.Exists(System.IO.Path.Combine(CurrentSettings.Movie_PersonArtwork_Path, persondetails.Name + ".jpg")) Then
                                                Exit For
                                            End If
                                            Grabber.GrabUtil.DownloadPersonArtwork(CurrentSettings.Movie_PersonArtwork_Path, image, persondetails.Name, True, True, filenameperson)
                                        Next
                                    End If
                                Next
                            End If
                        End If
                    ElseIf CurrentSettings.Create_Cover_From_Movie Then ' create missing fanart by thumbnailer
                        Dim FanartFileExists As Boolean = False
                        Dim NewFanartThumbName As String = _FilePath
                        NewFanartThumbName = System.IO.Path.GetDirectoryName(NewFanartThumbName) + "\" + System.IO.Path.GetFileNameWithoutExtension(NewFanartThumbName) + "-fanart.jpg" ' Now set to filename-fanart.jpg to get "better matching" if existing...
                        Dim Filename As String = _FilePath
                        Try
                            If Not File.Exists(NewFanartThumbName) Then
                                FanartFileExists = Grabber.GrabUtil.GetFanartFromMovie(fanartTitle, year, CurrentSettings.Movie_Fanart_Path, True, Filename, NewFanartThumbName, 0)
                            End If
                        Catch ex As Exception
                        End Try
                        Try
                            If File.Exists(NewFanartThumbName) Then ' recheck, if file is existing now after trying to create thumb from movie
                                FanartFileExists = True
                            End If
                        Catch ex As Exception
                        End Try

                        If FanartFileExists = True Then
                            TempValue = NewFanartThumbName
                            CreateOrUpdateElement(CurrentAttribute, TempValue, ProcessMode)
                        End If
                    ElseIf CurrentSettings.Use_Folder_Dot_Jpg = True And CurrentSettings.Prohibit_Internet_Lookup = False Then ' works only, when internet lookup is allowed, as otherwise no nfo file reader datra returns
                        'If IsUpdateRequested(CurrentAttribute) = True And CurrentSettings.Use_Folder_Dot_Jpg = True Then
                        Dim FanartFileExists As Boolean = False
                        Dim NewFanartThumbName As String = _FilePath

                        TempValue = _InternetData(Grabber_Output.Fanart)

                        If TempValue.Length > 0 Then
                            NewFanartThumbName = _InternetData(Grabber_Output.Fanart)
                        Else
                            If NewFanartThumbName.Contains("\") = True Then
                                NewFanartThumbName = NewFanartThumbName.Substring(0, NewFanartThumbName.LastIndexOf("\"))
                            End If
                            NewFanartThumbName += "\fanart.jpg"
                        End If

                        Try
                            If File.Exists(NewFanartThumbName) Then
                                FanartFileExists = True
                            End If
                        Catch ex As Exception
                        End Try

                        If FanartFileExists = True Then
                            TempValue = NewFanartThumbName
                            CreateOrUpdateElement(CurrentAttribute, TempValue, ProcessMode)
                            'Copy fanart to fanartfolder:
                            GrabUtil.CopyFanartToFanartFolder(NewFanartThumbName, CurrentSettings.Movie_Fanart_Path, fanartTitle)
                        End If
                    End If

                End If
            End If

        Catch ex As Exception
            _LastOutputMessage = "ErrorEvent : ErrorEvent importing " & _FileName.ToString & " : " & ex.Message.ToString & ", " & ex.StackTrace.ToString
        End Try
    End Sub

    Private Function IsUpdateRequested(ByVal currentAttribute As String, ByVal ProcessMode As Process_Mode_Names) As Boolean
        Dim attr As Xml.XmlAttribute
        Dim element As Xml.XmlElement
        Dim customfieldselement As Xml.XmlElement
        Dim customfieldsattr As Xml.XmlAttribute
        Dim IsUpdateRequired As Boolean = False

        If _DatabaseFields(currentAttribute.ToLower) = False Then ' Field not selected !
            Return False
        Else
            If OnlyAddMissingData = True And ProcessMode = Process_Mode_Names.Update Then
                attr = _XMLElement.Attributes(currentAttribute)
                element = _XMLElement.Item(currentAttribute)
                customfieldselement = _XMLElement.Item("CustomFields")
                If attr Is Nothing And element Is Nothing And customfieldselement Is Nothing Then ' no values exist at all
                    Return True
                Else
                    If Not attr Is Nothing Then ' check for standard attr value
                        If attr.Value Is Nothing Then
                            Return True
                        ElseIf attr.Value = "" Then
                            Return True
                        Else
                            Return False
                        End If
                    ElseIf Not customfieldselement Is Nothing Then  ' check for new AMC4 enhanced element value (Customfields)
                        customfieldsattr = customfieldselement.Attributes(currentAttribute)
                        If Not customfieldsattr Is Nothing Then ' check for  attr value inf customfields element
                            If customfieldsattr.Value Is Nothing Then
                                Return True
                            ElseIf customfieldsattr.Value = "" Then
                                Return True
                            Else
                                Return False
                            End If
                        Else
                            Return True
                        End If
                    ElseIf Not element Is Nothing Then  ' check for old MyFilms enhanced element value
                        If element.InnerText Is Nothing Then
                            Return True
                        ElseIf element.InnerText = "" Then
                            Return True
                        Else
                            Return False
                        End If
                    End If
                End If
            Else
                Return True
            End If
        End If
    End Function

    Private Sub CreateOrUpdateAttribute(ByVal currentAttribute As String, ByRef currentValue As String, ByVal ProcessMode As Process_Mode_Names)
        Dim attr As Xml.XmlAttribute
        If currentValue <> "" Then
            CleanValueForInnerXML(currentValue)
        End If
        If currentValue <> "" Or (currentValue = "" And OnlyUpdateNonEmptyData = False And ProcessMode = Process_Mode_Names.Update) Then
            If _XMLElement.Attributes(currentAttribute) Is Nothing Then
                attr = _XMLDoc.CreateAttribute(currentAttribute)
                attr.Value = currentValue
                _XMLElement.Attributes.Append(attr)
            Else
                _XMLElement.Attributes(currentAttribute).Value = currentValue
            End If
            If ProcessMode = Process_Mode_Names.Update Then
                If Not _LastOutputMessage.Contains(" - Updated: ") Then
                    _LastOutputMessage += " - Updated: " + currentAttribute
                Else
                    _LastOutputMessage += ", " + currentAttribute
                End If
            End If
        End If
        currentValue = ""
    End Sub

    Private Sub CreateOrUpdateElement(ByVal currentAttribute As String, ByRef currentValue As String, ByVal ProcessMode As Process_Mode_Names)

        ' First create new AMC4 objects - does NOT (yet) reset currentValue
        CreateOrUpdateCustomFieldsAttribute(currentAttribute, currentValue, ProcessMode)

        Dim element As Xml.XmlElement
        If currentValue <> "" Then
            CleanValueForInnerXML(currentValue)
        End If
        If currentValue <> "" Or (currentValue = "" And OnlyUpdateNonEmptyData = False And ProcessMode = Process_Mode_Names.Update) Then
            If _XMLElement.Item(currentAttribute) Is Nothing Then
                element = _XMLDoc.CreateElement(currentAttribute)
                element.InnerText = currentValue
                _XMLElement.AppendChild(element)
            Else
                _XMLElement.Item(currentAttribute).InnerText = currentValue
            End If
            If ProcessMode = Process_Mode_Names.Update Then
                If Not _LastOutputMessage.Contains(" - Updated: ") Then
                    _LastOutputMessage += " - Updated: " + currentAttribute
                Else
                    _LastOutputMessage += ", " + currentAttribute
                End If
            End If
        End If
        currentValue = ""
    End Sub
    Private Sub CreateOrUpdateCustomFieldsAttribute(ByVal currentAttribute As String, ByRef currentValue As String, ByVal ProcessMode As Process_Mode_Names)
        ' First check, if the CustomFields element exists or create it
        Const SubElementName As String = "CustomFields"
        Dim element As Xml.XmlElement
        If _XMLElement.Item(SubElementName) Is Nothing Then
            element = _XMLDoc.CreateElement(SubElementName)
        End If

        'now check and update or create Attributes in the CustomFields Element
        Dim attr As Xml.XmlAttribute
        If currentValue <> "" Then
            CleanValueForInnerXML(currentValue)
        End If
        If currentValue <> "" Or (currentValue = "" And OnlyUpdateNonEmptyData = False And ProcessMode = Process_Mode_Names.Update) Then
            If _XMLElement.Item(SubElementName) Is Nothing Then
                element = _XMLDoc.CreateElement(SubElementName)
                element.InnerText = ""
                _XMLElement.AppendChild(element)
            End If

            If _XMLElement.Item(SubElementName).Attributes(currentAttribute) Is Nothing Then
                attr = _XMLDoc.CreateAttribute(currentAttribute)
                attr.Value = currentValue
                _XMLElement.Item(SubElementName).Attributes.Append(attr)
            Else
                _XMLElement.Item(SubElementName).Attributes(currentAttribute).Value = currentValue
            End If
            If ProcessMode = Process_Mode_Names.Update Then
                If Not _LastOutputMessage.Contains(" - Updated: ") Then
                    _LastOutputMessage += " - Updated: " + currentAttribute
                Else
                    _LastOutputMessage += ", " + currentAttribute + " (CustomFields)"
                End If
            End If
        End If
        'currentValue = ""
    End Sub

    Private Sub CleanValueForInnerXML(ByRef currentValue)
        If currentValue <> "" Then
            '' currentValue = currentValue.Replace("&", "&amp;").Replace("<", "&lt;").Replace(">", "&gt;").Replace("\"", "&quot;").Replace("'", "&apos;")
            '' currentValue.Replace("""", "'")
            'Dim pattern As String = "#x((10?|[2-F])FFF[EF]|FDD[0-9A-F]|[19][0-9A-F]|7F|8[0-46-9A-F]|0?[1-8BCEF])"
            'Dim regex As New Regex(pattern, RegexOptions.IgnoreCase)
            'If (regex.IsMatch(currentValue)) Then
            '    currentValue = regex.Replace(currentValue, String.Empty)
            'End If
        End If
    End Sub

    Private Function IsValidTitle(ByVal _XMLElement As Xml.XmlElement, ByRef TitleType As String) As Boolean
        Dim ValidTitle As Boolean = False
        If _MasterTitle = TitleType Then
            If _XMLElement.Attributes(TitleType) IsNot Nothing Then
                If Not _XMLElement.Attributes(TitleType).Value.ToString = String.Empty Then
                    ValidTitle = True
                End If
            End If
        End If
        Return ValidTitle
    End Function
    Private Function AddEdition(ByRef Title As String, ByRef TitleField As String) As String
        Dim TempValue = Title
        If TempValue.Length > 0 Then
            Dim Edition As String = GetEdition(_FilePath)
            If Edition <> "" Then
                If CurrentSettings.Edition_Name_Applies_To = TitleField Or CurrentSettings.Edition_Name_Applies_To = "Both Titles" Then
                    If Not TempValue.EndsWith(" (" & Edition & ")") Then
                        TempValue = TempValue & " (" & Edition & ")"
                    End If
                End If
            End If
        End If
        Return TempValue
    End Function
    Private Function AddGroupName(ByRef Title As String, ByRef TitleField As String) As String
        Dim TempValue = Title

        If TempValue.Length > 0 Then
            If _GroupName <> "" Then
                If CurrentSettings.Folder_Name_Is_Group_Name = True Then
                    If CurrentSettings.Group_Name_Applies_To = TitleField Or CurrentSettings.Group_Name_Applies_To = "Both Titles" Then
                        'If TempValue <> _GroupName.ToString And Not TempValue.StartsWith(_GroupName.ToString & "\") Then
                        If Not TempValue.StartsWith(_GroupName.ToString & "\") Then
                            TempValue = _GroupName.ToString & "\" & TempValue
                        End If
                    End If
                End If
            Else
                If Not _InternetData Is Nothing Then
                    If Not _InternetData(Grabber_Output.Collection) Is Nothing Then
                        If _InternetData(Grabber_Output.Collection).Length > 0 Then
                            TempValue = _InternetData(Grabber_Output.Collection).ToString() & "\" & TempValue
                        End If
                    End If
                End If
            End If
        End If
        Return TempValue
    End Function

    Public Sub UpdateElement()

        Dim CurrentNode As Xml.XmlNode
        CurrentNode = XMLDoc.SelectSingleNodeFast("//AntMovieCatalog/Catalog/Contents/Movie[@Number='" & _MovieNumber & "']")
        CurrentNode.Attributes(_SourceField).Value = _OverridePath
        _XMLElement = CurrentNode

    End Sub

    Private Function CreateCoverFromMovie(ByVal FileName As String, ByRef PicturePathToSave As String) As Boolean
        Dim NewCoverThumbName As String = ""
        Dim CoverFileExists As Boolean = False
        If CurrentSettings.Store_Image_With_Relative_Path = True Then
            Dim PicturePathPrefix As String = CurrentSettings.Image_Download_Filename_Prefix.ToString 'Covers\'
            Dim PictureFileName As String = _MovieNumber.ToString + "_" + System.IO.Path.GetFileName(System.IO.Path.ChangeExtension(FileName, "jpg"))
            Dim PicturePathFull As String = System.IO.Path.Combine(_ImagePath, PictureFileName)
            NewCoverThumbName = PicturePathFull
            'Check, if the returned picture is existing - it might not due to download errors like 404
            Try
                If Not File.Exists(NewCoverThumbName) Then
                    ' CoverFileExists = CreateArtworkFromMovie(FileName, NewCoverThumbName, Artwork_Thumb_Mode.Cover) ' try creating artwork from movie
                    CoverFileExists = Grabber.GrabUtil.GetCoverartFromMovie(FileName, NewCoverThumbName, Grabber.GrabUtil.Artwork_Thumb_Mode.Cover, False, True, 0) ' try creating artwork from movie
                Else
                    CoverFileExists = True
                End If
            Catch ex As Exception
            End Try

            If System.IO.File.Exists(PicturePathFull) Then

                'Separate the folder from the prefix string (if needed)
                Dim PrefixString As String = String.Empty
                Dim PrefixPath As String = String.Empty
                If PicturePathPrefix <> String.Empty Then
                    If PicturePathPrefix.Contains("\") = True Then
                        PrefixPath = PicturePathPrefix.Substring(0, PicturePathPrefix.LastIndexOf("\") + 1)
                        PrefixString = PicturePathPrefix.Substring(PicturePathPrefix.LastIndexOf("\") + 1)
                    Else
                        PrefixString = PicturePathPrefix
                    End If
                End If

                Dim NewFileName As String = String.Empty
                If PrefixString <> String.Empty Then
                    'Need to rename the file.
                    NewFileName = PicturePathFull.Replace(PictureFileName, PrefixString & PictureFileName)
                    If Not File.Exists(NewFileName) Then
                        File.Copy(PicturePathFull, NewFileName)
                    End If
                    File.Delete(PicturePathFull)
                    PicturePathFull = NewFileName
                    PictureFileName = PrefixString & PictureFileName
                End If

                If PrefixPath <> String.Empty Then
                    'Need to add the new folder to the short (relative) path:
                    PictureFileName = PrefixPath & PictureFileName
                End If

                If CurrentSettings.Store_Image_With_Relative_Path = True Then
                    PicturePathToSave = PictureFileName
                Else
                    PicturePathToSave = PicturePathFull
                End If
            End If

        Else
            NewCoverThumbName = _FilePath
            NewCoverThumbName = System.IO.Path.ChangeExtension(NewCoverThumbName, "jpg") ' Now set to filename.jpg and use, if it exists
            PicturePathToSave = NewCoverThumbName
            Try
                If Not File.Exists(NewCoverThumbName) Then
                    ' CoverFileExists = CreateArtworkFromMovie(FileName, NewCoverThumbName, Artwork_Thumb_Mode.Cover) ' try creating artwork from movie
                    CoverFileExists = Grabber.GrabUtil.GetCoverartFromMovie(FileName, NewCoverThumbName, GrabUtil.Artwork_Thumb_Mode.Cover, False, True, 0) ' try creating artwork from movie
                End If
            Catch ex As Exception
            End Try
            Try
                If File.Exists(NewCoverThumbName) Then
                    CoverFileExists = True
                End If
            Catch ex As Exception
            End Try
        End If
        Return CoverFileExists
    End Function

    Private Function PictureNameWithSuffix(ByVal Expression As String, ByVal Year As String, ByVal Number As String) As String
        ' Expression sample: " (%year%)"

        If Year Is Nothing Then
            Year = ""
        End If
        If Number Is Nothing Then
            Number = ""
        End If

        If Year.Length = 0 And Number.Length = 0 Then
            Return ""
        End If

        Dim NewExpression As String = Expression
        If NewExpression.Contains("%year%") Then
            NewExpression = NewExpression.Replace("%year%", Year)
            If Year.Length = 0 And NewExpression.Contains("%number%") = False Then
                Return ""
            End If
        End If
        If NewExpression.Contains("%number%") Then
            NewExpression = NewExpression.Replace("%number%", Year)
            If Number.Length = 0 Then
                Return ""
            End If
        End If
        Return NewExpression
    End Function

    Private Function DrvLabel(ByVal MediaPath As String) As String
        Dim Label As String = ""

        ' Try to get DVD-Drive-Label
        Dim DrivePath As String = String.Empty
        Dim DriveLetter As String = String.Empty
        If _FilePath.IndexOf(":") > 0 Then
            'We're scanning a drive not a UNC, check what drive:
            DriveLetter = _FilePath.Substring(0, _FilePath.IndexOf(":") + 1) ' e.g. C:
            Dim myObjectSearcher As System.Management.ManagementObjectSearcher
            Dim myObject As System.Management.ManagementObject
            myObjectSearcher = New System.Management.ManagementObjectSearcher("SELECT * FROM Win32_CDROMDrive")
            For Each myObject In myObjectSearcher.Get
                If myObject("MediaLoaded") = "True" Then
                    If myObject("Drive").ToString.ToLower = DriveLetter.ToLower Then
                        Label = myObject("VolumeName").ToString
                    End If
                End If
            Next

            If Label = String.Empty Then ' Try to get Volumelabel from Disk Drive
                DriveLetter = DriveLetter.Substring(0, 2) ' e.g. C:\
                Dim allDrives() As DriveInfo = DriveInfo.GetDrives()
                Dim d As DriveInfo
                For Each d In allDrives
                    If d.IsReady = True Then
                        If d.Name.ToString.ToLower.Substring(0, 2) = DriveLetter.ToLower Then
                            Label = d.VolumeLabel ' d.DriveFormat, d.DriveType
                        End If
                    End If
                Next
            End If
        Else ' not a drive name, but UNC path - so populate with UNC server name
            If _FilePath.StartsWith("\\") Then
                Dim tString As String
                tString = _FilePath.Substring(2)
                If tString.IndexOf("\") > 0 Then
                    Label = _FilePath.Substring(0, tString.IndexOf("\"))
                End If
            End If
        End If
        Return Label
    End Function

    Public Sub SaveProgress()
        'XMLDoc.Save(_XMLFilePath)

        'Dim xmlFile As New FileStream(_XMLFilePath, FileMode.Open, FileAccess.Write, FileShare.Read)
        'xmlFile.SetLength(0)
        'XMLDoc.Save(xmlFile)
        'xmlFile.Close()

        Try
            Using fsLock As FileStream = File.Create(LockFilename(_XMLFilePath), 1000, FileOptions.DeleteOnClose) ' create lock file to avoid concurrent writing
                Using fs As New FileStream(_XMLFilePath, FileMode.OpenOrCreate, FileAccess.Write, FileShare.None) ' Dim xmlFile As New FileStream(_XMLFilePath, FileMode.Open, FileAccess.Write, FileShare.Read) ' lock the file for any other use, as we do write to it now !
                    fs.SetLength(0) ' do not append, owerwrite !
                    XMLDoc.Save(fs)
                    fs.Close() ' write buffer and release lock on file (either Flush, Dispose or Close is required)
                End Using
            End Using
            ' retry later 
        Catch ex As Exception
        End Try
    End Sub

    Private Shared Function LockFilename(ByVal CatalogFile As String) As String
        Dim lockerfilename As String = ""
        Try
            Dim path As String = System.IO.Path.GetDirectoryName(CatalogFile)
            Dim filename As String = System.IO.Path.GetFileNameWithoutExtension(CatalogFile)
            Dim machineName As String = System.Environment.MachineName
            lockerfilename = path & "\" & filename & "_" & machineName & ".lck"
        Catch ex As Exception
        End Try
        Return lockerfilename
    End Function

End Class





