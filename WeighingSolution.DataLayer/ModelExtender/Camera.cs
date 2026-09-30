using System.Collections.Generic;

namespace WeighingSolution.DataLayer
{
    public partial class Camera : NotifyDataErrorInfoBase
    {
        protected override IEnumerable<string> VerifyBusinessLogic(string propertyName)
        {
            switch (propertyName)
            {
                case "IPAddress":
                    if (CameraType == "HikVision" && string.IsNullOrEmpty(IPAddress))
                        yield return "IP Address is requirerd";
                    break;
                case "Port":
                    if (CameraType == "HikVision" && string.IsNullOrEmpty(Port))
                        yield return "Port is requirerd";
                    break;
                case "VideosourceURL":
                    if (CameraType == "RTSPMJPEG" && string.IsNullOrEmpty(VideosourceURL))
                        yield return "Video URL is requirerd";
                    break;

                case "VideosourceMainURL":
                    if (CameraType == "RTSPMJPEG" && string.IsNullOrEmpty(VideosourceMainURL))
                        yield return "Capture URL is requirerd";
                    break;
            }
        }
    }
}
