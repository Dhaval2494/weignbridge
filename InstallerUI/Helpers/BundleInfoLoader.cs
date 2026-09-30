using System.Collections.ObjectModel;
using System.Linq;
using System.Xml;
using System.Xml.Serialization;

namespace InstallerUI.Helpers
{
    public class BundleInfoLoader
    {
        public static BundleInfo Load()
        {
            string path = System.IO.Path.Combine(System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location), "BootstrapperApplicationData.xml");

            XmlSerializer xmlSerializer = new XmlSerializer(typeof(BundleInfo));
            BundleInfo result;
            using (System.IO.FileStream fileStream = new System.IO.FileStream(path, System.IO.FileMode.Open))
            {
                XmlReader xmlReader = XmlReader.Create(fileStream);
                result = (BundleInfo)xmlSerializer.Deserialize(xmlReader);
            }
            //exclude the MBA prereq packages, such as the .Net 4 installer
            var pkgs = result.Packages.Where(x => !result.MBAPrereqPackages.Any(preReq => preReq.PackageId == x.Id)).ToList();
            result.Packages.Clear();
            pkgs.ForEach(x => result.Packages.Add(x));
            return result;
        }
    }

    [XmlRoot("BootstrapperApplicationData", IsNullable = false, Namespace = "http://schemas.microsoft.com/wix/2010/BootstrapperApplicationData")]
    public class BundleInfo
    {
        [XmlElement("WixBundleProperties")]
        public BundleAttributes BundleAttributes { get; set; }
        [XmlElement("WixMbaPrereqInformation")]
        public Collection<MBAPrereqPackageInfo> MBAPrereqPackages { get; private set; }
        [XmlElement("WixPackageProperties")]
        public Collection<PackageInfo> Packages { get; private set; }
        [XmlElement("WixPackageFeatureInfo")]
        public Collection<FeatureInfo> PackageFeatures { get; private set; }

        public BundleInfo()
        {
            this.MBAPrereqPackages = new Collection<MBAPrereqPackageInfo>();
            this.Packages = new Collection<PackageInfo>();
            this.PackageFeatures = new Collection<FeatureInfo>();
        }
    }
    public class MBAPrereqPackageInfo
    {
        [XmlAttribute("PackageId")]
        public string PackageId
        {
            get;
            set;
        }
    }
    public class BundleAttributes
    {
        [XmlAttribute("DisplayName")]
        public string DisplayName
        {
            get;
            set;
        }
    }
    public class PackageInfo
    {
        [XmlAttribute("Package")]
        public string Id
        {
            get;
            set;
        }
        [XmlAttribute("DisplayName")]
        public string DisplayName
        {
            get;
            set;
        }
        [XmlAttribute("Description")]
        public string Description
        {
            get;
            set;
        }
    }
    public class FeatureInfo
    {
        [XmlAttribute("Package")]
        public string PackageId { get; set; }

        [XmlAttribute("Feature")]
        public string FeatureId { get; set; }

        [XmlAttribute("Size")]
        public string Size { get; set; }

        [XmlAttribute("Title")]
        public string Title { get; set; }

        [XmlAttribute("Description")]
        public string Description { get; set; }
    }
}