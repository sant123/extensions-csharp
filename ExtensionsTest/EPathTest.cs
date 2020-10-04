using System.IO;
using System.Text.RegularExpressions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ExtensionsTest
{
    [TestClass]
    public class EPathTest : MainTest
    {
        [TestMethod]
        public void CleanDirectoryNameTest()
        {
            var folderName = IllegalFolderName.CleanDirectoryName();

            var path = Path.Combine(DesktopPath, folderName);

            Directory.CreateDirectory(path);
            Directory.Delete(path);
        }

        [TestMethod]
        public void CleanFileNameTest()
        {
            var fileName = IllegalFileName.CleanFileName();

            var path = Path.Combine(DesktopPath, fileName);

            File.WriteAllText(path, "Some test :)");
            File.Delete(path);
        }

        [TestMethod]
        public void CreateUniqueFolderTest()
        {
            var folderName = LegalFolderName;
            var path = Path.Combine(DesktopPath, folderName);

            Directory.CreateDirectory(path);

            var uniqueFolder = EPath.CreateUniqueFolder(DesktopPath, folderName);

            Assert.IsTrue(Regex.IsMatch(uniqueFolder, @"\(\d+\)"));

            Directory.Delete(path);
            Directory.Delete(uniqueFolder);
        }

        [TestMethod]
        public void GetLastPathTest()
        {
            var folderName = EPath.GetLastPath(DesktopPath);

            Assert.AreEqual("Desktop", folderName);
        }

        [TestMethod]
        public void GetUniqueFileNameTest()
        {
            var fileName = LegalFileName;
            var path = Path.Combine(DesktopPath, fileName);

            File.WriteAllText(path, "Some test :)");

            var uniqueFilename = EPath.GetUniqueFileName(DesktopPath, fileName);

            Assert.IsTrue(Regex.IsMatch(uniqueFilename, @"\(\d+\)"));

            File.Delete(path);
        }

        [TestMethod]
        public void GetUniqueFolderTest()
        {
            var folderName = LegalFolderName;
            var path = Path.Combine(DesktopPath, folderName);

            Directory.CreateDirectory(path);

            var uniqueFolder = EPath.GetUniqueFolder(DesktopPath, folderName);

            Assert.IsTrue(Regex.IsMatch(uniqueFolder, @"\(\d+\)"));

            Directory.Delete(path);
        }
    }
}
