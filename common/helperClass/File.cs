using System.Diagnostics;

namespace CafeBisleriumPOS.common.helperClass;
public class FileManagement
{
   public string DirectoryPath(string dir, string fileName)
    {
        // Efficient file management with System.Diagnostics in action.
        string baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
#pragma warning disable CS8602 // Dereference of a possibly null reference.
        string projectDirectory = Directory.GetParent(baseDirectory).Parent.Parent.Parent.Parent.Parent.FullName;
#pragma warning restore CS8602 // Dereference of a possibly null reference.
        Trace.WriteLine(projectDirectory);
        if (projectDirectory == null)
        {
            throw new InvalidOperationException("Could not find the project directory.");
        }
        var path = Path.Combine(projectDirectory, dir, fileName);
        Trace.WriteLine("This is Path in FileManagementClass: " + path);
        return path;
    }
}
