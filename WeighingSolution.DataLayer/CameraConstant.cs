using System;
using System.Data.Entity.Core.EntityClient;
using System.IO;
using System.Linq;
using System.Security.AccessControl;

namespace WeighingSolution.DataLayer
{
    public class CameraConstant
    {
        private static string _cameraentityconnectionstring;
        public static string CameraEntityConnectionString
        {
            get
            {
                if (String.IsNullOrEmpty(_cameraentityconnectionstring))
                {
                    EntityConnectionStringBuilder mainentityStringBuilder = new EntityConnectionStringBuilder(Constant.EntityConnectionString);
                    EntityConnectionStringBuilder cameraentityStringBuilder = new EntityConnectionStringBuilder("metadata=res://*/WeighBridgeModelCamera.csdl|res://*/WeighBridgeModelCamera.ssdl|res://*/WeighBridgeModelCamera.msl;provider=System.Data.SqlClient;provider connection string=';Data Source=XXXDataSourceXXX;initial catalog=svt_weighbridge;user id=XXXuseridXXX;password=XXXpasswordXXX;MultipleActiveResultSets=True;App=EntityFramework';");
                    cameraentityStringBuilder.ProviderConnectionString = mainentityStringBuilder.ProviderConnectionString;
                    _cameraentityconnectionstring = cameraentityStringBuilder.ToString();
                }
                return _cameraentityconnectionstring;
            }
        }

        private static string _cameraimagelocation;

        public static string CameraImageLocation
        {
            get
            {
                //if (String.IsNullOrEmpty(_cameraimagelocation))
                //{
                using (weighbridgeEntities ctx = new weighbridgeEntities(Constant.EntityConnectionString))
                {
                    ApplicationSetting CameraSetting = ctx.ApplicationSettings.FirstOrDefault(x => x.ModuleName == "CameraSetting" && x.SettingName == "CameraImagePath");
                    if (CameraSetting != null && CameraSetting.SettingValue != "")
                    {
                        try
                        {
                            if (!Directory.Exists(CameraSetting.SettingValue))
                            {
                                DirectoryInfo di = Directory.CreateDirectory(CameraSetting.SettingValue);
                                DirectorySecurity security = di.GetAccessControl();
                                security.AddAccessRule(new FileSystemAccessRule(@"Users", FileSystemRights.Modify, AccessControlType.Allow));
                                di.SetAccessControl(security);
                            }
                        }
                        catch (Exception)
                        {
                        }
                        _cameraimagelocation = CameraSetting.SettingValue;
                    }
                }
                //}
                return _cameraimagelocation;
            }
            set { _cameraimagelocation = value; }
        }
    }
}
