using System;
using System.IO;

[Serializable]
public class SaveFileViewModel
{
    private readonly FileInfo fileInfo;

    public SaveFileViewModel(FileInfo fileInfo)
    {
        this.fileInfo = fileInfo;
    }

    public string Name => Path.GetFileNameWithoutExtension(fileInfo.Name);
    public string ModifiedTimeString => fileInfo.LastWriteTime.ToString("yyyy-MM-dd HH:mm:ss");
    public DateTime ModifiedTime => fileInfo.LastWriteTime;

    public override string ToString()
    {
        return Name;
    }
}
