﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Serialization;
using NewLife.Log;
using NewLife.Xml;

namespace NewLife.Reflection
{
    public class ScriptEngine
    {
        static void Main()
        {
            //PathHelper.BaseDirectory = @"E:\X\Src\NewLife.Cube";
            XTrace.Debug = true;
            XTrace.UseConsole();
            //"cmd".Run("/c del *.nuspec /f/q");
			foreach(var item in ".".AsDirectory().GetAllFiles("*.nuspec"))
			{
				Console.WriteLine("删除 {0}", item);
				item.Delete();
			}
            // 找到名称
            var proj = ".".AsDirectory().FullName.EnsureEnd("\\");

            Console.WriteLine("proj项目：{0}", proj);
            string[] pathsplit = proj.Split("\\");

            var name = pathsplit[pathsplit.Count() - 1];
                Console.WriteLine("项目：{0}", name);
            proj = name+".csproj";
            var spec = name + ".nuspec";
            
            if (!File.Exists(spec.GetFullPath()))
            {
				var tar = "..\\..\\Bin\\" + name + ".dll";
				tar = tar.GetFullPath();
                if (!File.Exists(tar))
                {
					tar = "..\\..\\Bin4\\" + name + ".exe";
					tar = tar.GetFullPath();
                }
                if (!File.Exists(tar))
                {
					tar = "..\\..\\XCoder\\" + name + ".exe";
					tar = tar.GetFullPath();
                }
                if (!File.Exists(tar))
                {
					Console.WriteLine("只能找项目文件了，总得做点啥不是");
					//编译当前工程
					"msbuild".Run(proj + " /t:Rebuild /p:Configuration=Release /p:VisualStudioVersion=12.0 /noconlog /nologo", 8000);
					//"NuGet".Run("spec -f -a " + name, 5000);
					return;
                }
				Console.WriteLine("目标 {0}", tar);
				"NuGet".Run("spec -f -a " + tar, 5000);
				
                var spec2 = ".".AsDirectory().GetAllFiles(spec).First().Name;
                if (!spec.EqualIgnoreCase(spec2)) File.Move(spec2, spec);
            }

            // 部分项目加上前缀
            var name2 = name.EnsureStart("NewLife.");

            var cfg = Manifest.Load(spec.GetFullPath());
            // 修改配置文件
            cfg.Metadata.Id = name2;
            cfg.Metadata.LicenseUrl = "http://www.NewLifeX.com";
            cfg.Metadata.ProjectUrl = "http://www.NewLifeX.com/showtopic-51.aspx";
            cfg.Metadata.IconUrl = "http://www.NewLifeX.com/favicon.ico";
            cfg.Metadata.Copyright = "Copyright 2002-{0} 新生命开发团队 http://www.NewLifeX.com".F(DateTime.Now.Year);
            cfg.Metadata.Tags = "新生命团队 X组件 NewLife " + name;
            cfg.Metadata.ReleaseNotes = "http://www.newlifex.com/showtopic-51.aspx";
            //cfg.Metadata.Authors="新生命开发团队";
            //cfg.Metadata.Owners="新生命开发团队";
            // 清空依赖
            if (cfg.Metadata.DependencySets != null && cfg.Metadata.DependencySets.Dependencies != null)
                cfg.Metadata.DependencySets.Dependencies.Clear();

            // 自动添加所有文件
            if (cfg.Files == null) cfg.Files = new List<ManifestFile>();
            cfg.Files.Clear();
            if (cfg.Files.Count == 0)
            {
                AddFile(cfg, name, "dll");
                AddFile(cfg, name, "xml");
                AddFile(cfg, name, "pdb");
                AddFile(cfg, name, "exe");

                AddFile(cfg, name, "dll", false);
                AddFile(cfg, name, "xml", false);
                AddFile(cfg, name, "pdb", false);
                AddFile(cfg, name, "exe", false);
            }

            cfg.Save();

            //var pack = "pack {0} -IncludeReferencedProjects -Build -Prop Configuration={1} -Exclude **\\*.txt;**\\*.png;content\\*.xml";
            // *\\*.*干掉下级的所有文件
            var pack = "pack {0} -IncludeReferencedProjects -Exclude **\\*.txt;**\\*.png;*.jpg;*.xml;*\\*.*";
            Console.WriteLine("打包：{0}", proj);
            //"cmd".Run("/c del *.nupkg /f/q");
			foreach(var item in ".".AsDirectory().GetAllFiles("*.nupkg"))
			{
				Console.WriteLine("删除 {0}", item);
				item.Delete();
			}
            "NuGet".Run(pack.F(proj), 30000);
            var fi = ".".AsDirectory().GetAllFiles("*.nupkg").FirstOrDefault();
            if (fi != null)
            {
                var nupkg = fi.Name;
                Console.WriteLine("发布：{0}", nupkg);
                "NuGet".Run("push {0}".F(nupkg), 30000);
            }
        }

        static void AddFile(Manifest cfg, String name, String ext, Boolean fx45 = true)
        {
            var mf = new ManifestFile();

            if (fx45)
            {
                mf.Source = @"..\..\BIN\{0}.{1}".F(name, ext);
                mf.Target = @"lib\net45\{0}.{1}".F(name, ext);
            }
            else
            {
                mf.Source = @"..\..\BIN4\{0}.{1}".F(name, ext);
                mf.Target = @"lib\net40\{0}.{1}".F(name, ext);
            }
            if (File.Exists(mf.Source.GetFullPath())) cfg.Files.Add(mf);
        }
    }

    [XmlType("package")]
    public class Manifest : XmlConfig<Manifest>
    {
        [XmlElement("metadata", IsNullable = false)]
        public ManifestMetadata Metadata { get; set; }

        [XmlArray("files")]
        public List<ManifestFile> Files { get; set; }

        public Manifest()
        {
            this.Metadata = new ManifestMetadata();
        }
    }

    [XmlType("file")]
    public class ManifestFile
    {
        [XmlAttribute("src")]
        public string Source { get; set; }

        [XmlAttribute("target")]
        public string Target { get; set; }

        [XmlAttribute("exclude")]
        public string Exclude { get; set; }
    }

    [XmlType("metadata")]
    public class ManifestMetadata
    {
        [XmlAttribute("minClientVersion")]
        public string MinClientVersionString { get; set; }

        [XmlElement("id")]
        public string Id { get; set; }

        [XmlElement("version")]
        public string Version { get; set; }

        [XmlElement("title")]
        public string Title { get; set; }

        [XmlElement("authors")]
        public string Authors { get; set; }

        [XmlElement("owners")]
        public string Owners { get; set; }

        [XmlElement("licenseUrl")]
        public string LicenseUrl { get; set; }

        [XmlElement("projectUrl")]
        public string ProjectUrl { get; set; }

        [XmlElement("iconUrl")]
        public string IconUrl { get; set; }

        [XmlElement("requireLicenseAcceptance")]
        public bool RequireLicenseAcceptance { get; set; }

        [XmlElement("description")]
        public string Description { get; set; }

        [XmlElement("summary")]
        public string Summary { get; set; }

        [XmlElement("releaseNotes")]
        public string ReleaseNotes { get; set; }

        [XmlElement("copyright")]
        public string Copyright { get; set; }

        [XmlElement("language")]
        public string Language { get; set; }

        [XmlElement("tags")]
        public string Tags { get; set; }

        [XmlElement("dependencies")]
        public ManifestDependencySet DependencySets { get; set; }

        [XmlElement("frameworkAssemblies")]
        public List<ManifestFrameworkAssembly> FrameworkAssemblies { get; set; }

        [XmlElement("references")]
        public ManifestReferenceSet ReferenceSets { get; set; }
    }

    public class ManifestDependencySet
    {
        [XmlAttribute("targetFramework")]
        public string TargetFramework { get; set; }

        [XmlElement("dependency")]
        public List<ManifestDependency> Dependencies { get; set; }
    }

    [XmlType("dependency")]
    public class ManifestDependency
    {
        [XmlAttribute("id")]
        public string Id { get; set; }

        [XmlAttribute("version")]
        public string Version { get; set; }
    }

    [XmlType("frameworkAssembly")]
    public class ManifestFrameworkAssembly
    {
        [XmlAttribute("assemblyName")]
        public string AssemblyName { get; set; }

        [XmlAttribute("targetFramework")]
        public string TargetFramework { get; set; }
    }

    public class ManifestReferenceSet
    {
        [XmlAttribute("targetFramework")]
        public string TargetFramework { get; set; }

        [XmlElement("reference")]
        public List<ManifestReference> References { get; set; }
    }

    [XmlType("reference")]
    public class ManifestReference
    {
        [XmlAttribute("file")]
        public string File { get; set; }
    }
}