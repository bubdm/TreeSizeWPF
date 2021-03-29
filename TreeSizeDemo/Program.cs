using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace TreeSizeDemo
{
    public class Program
    {
        public static void Main(string[] args)
        {
            
            Program program = new Program();
            string userPath = Console.ReadLine();
            DirectoryInfo directoryInfo = new DirectoryInfo(userPath);
            program.WalkDirectoryTree(directoryInfo);
            
        }
        //Метод осуществляет индексацию всех файлов и директорий в заданом пользователем каталоге
        public async void WalkDirectoryTree(DirectoryInfo root)
        {
            var files = root.GetFiles().Where(x => !(x.Attributes.HasFlag(FileAttributes.Hidden))); // получаем файлы текущей директории
            var subDirs = root.GetDirectories().Where(x => (x.Attributes & FileAttributes.Hidden) == 0);
            
            foreach (var directoryInfo in subDirs)
            {
                Task<long> getSize = DirSizeAsync(directoryInfo);
                DoWork();
                long size = await getSize;
                Console.WriteLine($"Size of '{directoryInfo.FullName}' {size} bytes");
            }
            foreach (var fi in files)
            {
                 Console.WriteLine($"{fi.Name} {fi.Length}");
                
            }
        }

        //Метод возвращает размер непосредственно каждой папки,уникальный тем,что считает все возможные файлы
        public async Task<long> DirSizeAsync(DirectoryInfo d)
        {
            long size = 0;
            try
            {
                var fileInfos = d.GetFiles();
                foreach (FileInfo fileInfo in fileInfos)
                {
                    size += fileInfo.Length;
                }
            }
            catch(Exception)
            {
                return 0;
            }

            var dis = d.GetDirectories();
            foreach (var di in dis)
            {
                Task<long> sized = DirSizeAsync(di);
                size += await sized;
            }

            return size;
        }

        //теста ради
        public void DoWork()
        {
            Console.WriteLine("Work...");
            Thread.Sleep(5000);
        }

    }
}
