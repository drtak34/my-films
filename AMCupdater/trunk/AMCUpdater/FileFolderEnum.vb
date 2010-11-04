Public Class FileFolderEnum

    Private _Root As String
    Private _Files As List(Of String)
    Private _Folders As List(Of String)
    Private _TotalSize As Long
    Private _TotalFiles As Long
    Private _TotalFolders As Long

    Private Shared tblExcludedFiles As Hashtable
    Private Shared tblExcludedFolders As Hashtable

    Public ExcludedFiles As String = String.Empty
    Public ExcludedFolders As String = String.Empty


    Public Property Root() As String
        Get
            Return _Root
        End Get
        Set(ByVal value As String)
            _Root = value
        End Set
    End Property


    Public Property Files() As List(Of String)
        Get
            _Files.Sort()
            Return _Files
        End Get
        Set(ByVal value As List(Of String))
            _Files = value
        End Set
    End Property


    Public Property Folders() As List(Of String)
        Get
            _Folders.Sort()
            Return _Folders
        End Get
        Set(ByVal value As List(Of String))
            _Folders = value
        End Set
    End Property


    Public Property TotalSize() As Long
        Get
            Return _TotalSize
        End Get
        Set(ByVal value As Long)
            _TotalSize = value
        End Set
    End Property


    Public Property TotalFiles() As Long
        Get
            Return _TotalFiles
        End Get
        Set(ByVal value As Long)
            _TotalFiles = value
        End Set
    End Property


    Public Property TotalFolders() As Long
        Get
            Return _TotalFolders
        End Get
        Set(ByVal value As Long)
            _TotalFolders = value
        End Set
    End Property

    Private Sub LoadExclusions()
        tblExcludedFiles = New Hashtable
        tblExcludedFolders = New Hashtable

        Dim item As String

        If ExcludedFiles.Length > 0 Then
            For Each item In ExcludedFiles.Split("|")
                tblExcludedFiles.Add(item, item)
                'tblExcludedFiles.Rows.Add(item)
            Next
        End If

        If ExcludedFolders.Length > 0 Then
            For Each item In ExcludedFolders.Split("|")
                tblExcludedFolders.Add(item, item)
                'tblExcludedFolders.Rows.Add(item)
            Next
        End If

    End Sub


    Public Sub New()


        _Root = ""
        _Files = New List(Of String)
        _Folders = New List(Of String)
        _TotalSize = 0
        _TotalFiles = 0
        _TotalFolders = 0
        LoadExclusions()

    End Sub

    Public Sub New(ByVal Root As String)


        _Root = Root
        _Files = New List(Of String)
        _Folders = New List(Of String)
        _TotalSize = 0
        _TotalFiles = 0
        _TotalFolders = 0
        LoadExclusions()
        'Me.GetFiles(_Root)


    End Sub


    Public Sub GetFiles(ByVal Path As String)

        LoadExclusions()

        Dim myDirectoryRoot As New DirectoryInfo(Path)
        Dim di As DirectoryInfo
        Dim fi As FileInfo
        Dim lSize As Long = 0
        Dim IncludeItem As Boolean = True

        Try
            For Each fi In myDirectoryRoot.GetFiles
                'Check the file is not marked as hidden:
                If Not (fi.Attributes And FileAttributes.Hidden) = FileAttributes.Hidden Then
                    If Not fnMatchExclusions(fi.Name.Substring(0, fi.Name.LastIndexOf(".")), tblExcludedFiles) = True Then
                        _Files.Add(fi.FullName)
                        _TotalFiles += 1
                        _TotalSize += fi.Length
                    End If
                End If
            Next
        Catch ex As Exception
        End Try

        Try
            For Each di In myDirectoryRoot.GetDirectories()
                'Check folder not marked as hidden (don't recurse any more if so):
                If Not (di.Attributes And FileAttributes.Hidden) = FileAttributes.Hidden Then
                    If Not fnMatchExclusions(di.Name, tblExcludedFolders) = True Then
                        _Folders.Add(di.FullName)
                        _TotalFolders += 1
                        GetFiles(di.FullName)
                    End If
                End If
            Next
        Catch ex As Exception
        End Try

        myDirectoryRoot = Nothing


    End Sub

    Private Function fnMatchExclusions(ByVal ItemName As String, ByVal ExclusionList As Hashtable) As Boolean
        Dim ReturnValue As Boolean = False

        'Dim RegCheck As Regex
        'Dim m As Match

        'For Each blah as string In ExclusionList.Keys
        '    If blah.Length > 0 Then
        '        RegCheck = New Regex(blah.ToLower)
        '        m = RegCheck.Match(ItemName.ToLower)
        '        If m.Success Then
        '            ReturnValue = True
        '        End If
        '    End If
        'Next
        For Each blah As String In ExclusionList.Keys
            If blah.Length > 0 Then
                If blah.ToLower = ItemName.ToLower Then
                    ReturnValue = True
                End If
            End If
        Next

        Return ReturnValue
    End Function

End Class

