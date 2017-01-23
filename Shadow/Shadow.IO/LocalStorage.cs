using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using PCLStorage;

namespace Shadow.IO
{
	public static class LocalStorage
	{

		public static async Task<bool> WriteTextFile(string Directory, string FileName, string Contents)
		{
			IFolder rootFolder;
			IFolder folder;
			IFile file;

			try
			{
				rootFolder = FileSystem.Current.LocalStorage;
				folder = rootFolder.CreateFolderAsync(Directory, CreationCollisionOption.OpenIfExists).Result;		
				file = folder.CreateFileAsync(FileName, CreationCollisionOption.ReplaceExisting).Result;
				file.WriteAllTextAsync(Contents).Wait();
				return true;
			}
			catch(Exception ex) {
				Shadow.Data.Runtime.LastErrorMessage = ex.Message;
				return false;
			}
			finally {
				rootFolder = null;
				folder = null;
				file = null;
			}
		}

		private static async Task<string> ReadFile(IFile f, string fileName){
			return await Task.Run(() => f.ReadAllTextAsync ()).ConfigureAwait(false);
		}

		public static async Task<string> ReadTextFile(string Directory, string FileName)
		{
			IFolder rootFolder;
			IFolder folder;
			IFile file;
			try
			{
				rootFolder = FileSystem.Current.LocalStorage;
				folder = rootFolder.CreateFolderAsync(Directory, CreationCollisionOption.OpenIfExists).Result;
				file = folder.GetFileAsync (FileName).Result;
				return ReadFile (file, FileName).Result;
			}
			catch(Exception ex) {
				Shadow.Data.Runtime.LastErrorMessage = ex.Message;
				return null;
			}
			finally {
				rootFolder = null;
				folder = null;
				file = null;
			}
		}

		public static async Task<bool> WriteBinaryFile(string Directory, string FileName, byte[] Contents)
		{
			IFolder rootFolder;
			IFolder folder;
			IFile file;
			try
			{				
				rootFolder = FileSystem.Current.LocalStorage;
				folder = rootFolder.CreateFolderAsync(Directory, CreationCollisionOption.OpenIfExists).Result;		
				file = folder.CreateFileAsync(FileName, CreationCollisionOption.ReplaceExisting).Result;

				using (System.IO.Stream stream = file.OpenAsync(FileAccess.ReadAndWrite).Result)
				{
					stream.Write(Contents, 0, Contents.Length);
				}

				return true;
			}
			catch(Exception ex) {
				Shadow.Data.Runtime.LastErrorMessage = ex.Message;
				return false;
			}
			finally {
				rootFolder = null;
				folder = null;
				file = null;
			}
		}

		public static async Task<string> GetLocalFilePath(string Directory, string FileName)
		{
			IFolder rootFolder;
			IFolder folder;
			try
			{
				rootFolder = FileSystem.Current.LocalStorage;
				folder = rootFolder.CreateFolderAsync(Directory, CreationCollisionOption.OpenIfExists).Result;
				return folder.Path + FileName;
			}
			catch(Exception ex) {
				Shadow.Data.Runtime.LastErrorMessage = ex.Message;
				return null;
			}
			finally {
				rootFolder = null;
				folder = null;
			}
		}

		public static async Task<string> GetLocalPath(string Directory)
		{
			IFolder rootFolder;
			IFolder folder;
			try
			{
				rootFolder = FileSystem.Current.LocalStorage;
				folder = rootFolder.CreateFolderAsync(Directory, CreationCollisionOption.OpenIfExists).Result;
				return folder.Path;
			}
			catch(Exception ex) {
				Shadow.Data.Runtime.LastErrorMessage = ex.Message;
				return null;
			}
			finally {
				rootFolder = null;
				folder = null;
			}
		}

		public static async Task<bool> DeleteLocalPath(string Directory)
		{
			IFolder rootFolder;
			IFolder folder;
			try
			{
				rootFolder = FileSystem.Current.LocalStorage;
				folder = rootFolder.CreateFolderAsync(Directory, CreationCollisionOption.OpenIfExists).Result;
				folder.DeleteAsync();
				return true;
			}
			catch(Exception ex) {
				Shadow.Data.Runtime.LastErrorMessage = ex.Message;
				return false;
			}
			finally {
				rootFolder = null;
			}
		}

		public static async Task<bool> DeleteFile(string Directory, string FileName)
		{
			IFolder rootFolder;
			IFolder folder;
			IFile file;
			try
			{
				rootFolder = FileSystem.Current.LocalStorage;
				folder = rootFolder.CreateFolderAsync(Directory, CreationCollisionOption.OpenIfExists).Result;
				file = folder.GetFileAsync (FileName).Result;
				file.DeleteAsync();
				return true;
			}
			catch(Exception ex) {
				Shadow.Data.Runtime.LastErrorMessage = ex.Message;
				return false;
			}
			finally {
				rootFolder = null;
				file = null;
			}
		}

		public static async Task<bool> FileExists(string Directory, string FileName)
		{
			IFolder rootFolder;
			IFolder folder;
			try
			{
				rootFolder = FileSystem.Current.LocalStorage;
				folder = rootFolder.CreateFolderAsync(Directory, CreationCollisionOption.OpenIfExists).Result;
				if(folder.CheckExistsAsync (FileName).Result == ExistenceCheckResult.FileExists) 
					return true;
				else
					return false;
			}
			catch(Exception ex) {
				Shadow.Data.Runtime.LastErrorMessage = ex.Message;
				return false;
			}
			finally {
				rootFolder = null;
				folder = null;
			}
		}

		public static async Task<IList<string>> GetFileList(string Directory)
		{
			IFolder rootFolder;
			IFolder folder;
			try
			{
				List<string> stringfilelist = new List<string>();
				rootFolder = FileSystem.Current.LocalStorage;
				folder = rootFolder.CreateFolderAsync(Directory, CreationCollisionOption.OpenIfExists).Result;
				IList<PCLStorage.IFile> files = folder.GetFilesAsync().Result;
				foreach(PCLStorage.IFile file in files)
				{
					stringfilelist.Add(file.Name);
				}
				return stringfilelist;
			}
			catch(Exception ex) {
				Shadow.Data.Runtime.LastErrorMessage = ex.Message;
				return null;
			}
			finally {
				rootFolder = null;
				folder = null;
			}
		}
	}
}