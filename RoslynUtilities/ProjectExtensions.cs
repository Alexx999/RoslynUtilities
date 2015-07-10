using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using Microsoft.CodeAnalysis;
using System.Xml.Linq;

namespace RoslynUtilities
{
    public static class ProjectExtensions
    {
        public static bool IsCSharpProject(this Project project)
        {
            return project.Language.Equals("C#");
        }

        public static Enums.ProjectType GetProjectType(this Project project)
        {
            //var result = project.IsWindowsPhoneProject();
            //if (result == 1)
            //    return Enums.ProjectType.WP7;
            //else if (result == 2)
            //    return Enums.ProjectType.WP8;

            if (project.IsWPProject())
            {
                if (project.IsWP8Project())
                    return Enums.ProjectType.WP8;
                return Enums.ProjectType.WP7;
            }
            else if (project.IsNet40Project())
                return Enums.ProjectType.NET4;
            else if (project.IsNet45Project())
                return Enums.ProjectType.NET45;
            else
                return Enums.ProjectType.NETOther;
        }

        // return 2 if the project targets windows phone 8 os, return 1 if targetting windows phone 7,7.1.
        public static int IsWindowsPhoneProject(this Project project)
        {

            if (project.FilePath == null)
                return 0;

            XDocument doc = XDocument.Load(project.FilePath);


            //XmlNamespaceManager mgr = new XmlNamespaceManager(doc.NameTable);
            //mgr.AddNamespace("x", "http://schemas.microsoft.com/developer/msbuild/2003");
            
            //var node = doc.SelectSingleNode("//x:TargetFrameworkIdentifier", mgr);
            //if (node != null && node.InnerText.ToString().Equals("WindowsPhone"))
            //    return 2;

            //var profileNode = doc.SelectSingleNode("//x:TargetFrameworkProfile", mgr);
            //if (profileNode != null && profileNode.InnerText.ToString().Contains("WindowsPhone"))
            //    return 1;

            //var node2 = doc.SelectSingleNode("//x:XnaPlatform", mgr);
            //if (node2 != null && node2.InnerText.ToString().Equals("Windows Phone"))
            //    return 1;


            return 0;
        }

        public static bool IsWPProject(this Project project)
        {
            return project.MetadataReferences.Any(a => a.Display.Contains("Windows Phone") || a.Display.Contains("WindowsPhone"));
        }

        public static bool IsWP8Project(this Project project)
        {
            return project.MetadataReferences.Any(a => a.Display.Contains("Windows Phone\\v8"));
        }

        public static bool IsNet40Project(this Project project)
        {
            return project.MetadataReferences.Any(a => a.Display.Contains("Framework\\v4.0"));
        }

        public static bool IsNet45Project(this Project project)
        {
            return project.MetadataReferences.Any(a => a.Display.Contains("Framework\\v4.5") || a.Display.Contains(".NETCore\\v4.5"));
        }
    }
}
