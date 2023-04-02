using doob.Scripter.Shared;
using Zio;
using Zio.FileSystems;

namespace doob.Scripter.Module.VirtualFileSystem
{

    public class VirtualFileSystemModule: IScripterModule
    {
        
        public SubFileSystem SubFileSystem(UPath uPath)
        {
            var fs = new PhysicalFileSystem();
            return new SubFileSystem(fs, uPath);
        }

        public AggregateFileSystem AggregateFileSystem()
        {
            return new AggregateFileSystem(true);
        }

        public AggregateFileSystem AggregateFileSystem(IFileSystem fileSystem)
        {
            return AggregateFileSystem(new[] { fileSystem });
        }

        public AggregateFileSystem AggregateFileSystem(IFileSystem[] fileSystems)
        {
            var aggregateFileSystem = AggregateFileSystem();
            foreach (var fileSystem in fileSystems)
            {
                aggregateFileSystem.AddFileSystem((fileSystem));
            }

            return aggregateFileSystem;
        }

        public MountFileSystem MountFileSystem()
        {
            return new MountFileSystem(true);
        }


        public UPath BuildUPath(string path)
        {
            return new UPath(path);
        }

        public UPath BuildUPath(string[] paths)
        {
            return UPath.Combine(paths.Select(p => (UPath)p).ToArray());
        }
    }
}
