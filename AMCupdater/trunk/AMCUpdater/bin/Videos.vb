Public MustInherit Class Videos

    Implements IStoreData

    'Public ListVidéos As New List(Of Vidéo)
    Public Shared ListVidéos As New DataTable
    Public MustOverride Sub LoadData() Implements IStoreData.LoadData
    Public MustOverride Sub SaveData() Implements IStoreData.SaveData

End Class