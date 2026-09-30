using System;
using System.Configuration;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.Integration;

namespace WeighingSolution
{
    public class HikVisionController : VideoControllerBase
    {
        #region --- Fields ---
        public static string lockVar = "";
        public static bool isSDKInitialized = false;

        private bool isLogin = false;
        private int LoggedInUserID = -1;
        private int RealtimePlayHandle = -1;
        public WindowsFormsHost wfh;
        private HCNetSDK.MSGCallBack m_falarmData = null;
        private PictureBox pbCamera;
        #endregion

        [System.Runtime.InteropServices.DllImport("kernel32.dll", CharSet = System.Runtime.InteropServices.CharSet.Auto, SetLastError = true)]
        static extern bool SetDllDirectory(string lpPathName);

        #region --- Initialization ---
        public HikVisionController(string _Url, string _IpAddress, ushort _Port, string _UserName, string _Password)
            : base(_Url, _IpAddress, _Port, _UserName, _Password)
        {
            wfh = new WindowsFormsHost();
            pbCamera = new PictureBox();
            wfh.Child = pbCamera;
            VideoDisplay = wfh;
            SetDllDirectory(System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + "\\HCNetSDK");
        }
        #endregion

        #region --- Methods ---
        public async override Task StartVideo()
        {
            try
            {
                if (!isLogin)
                {
                    if (String.IsNullOrEmpty(IpAddress) || Port <= 0 || String.IsNullOrEmpty(UserName) || String.IsNullOrEmpty(Password))
                        return;

                    lock (lockVar)
                    {
                        if (!isSDKInitialized)
                        {
                            isSDKInitialized = HCNetSDK.NET_DVR_Init();
                            if (!isSDKInitialized)
                                return;
                            HCNetSDK.NET_DVR_SetConnectTime(5000, 3);
                            HCNetSDK.NET_DVR_SetReconnect(10000, 1);
                        }
                    }

                    if (isSDKInitialized)
                    {
                        HCNetSDK.NET_DVR_DEVICEINFO_V30 structDeviceInfo = new HCNetSDK.NET_DVR_DEVICEINFO_V30();

                        await Task.Run(() =>
                        {
                            LoggedInUserID = HCNetSDK.NET_DVR_Login_V30(IpAddress, Port, UserName, Password, ref structDeviceInfo);
                            if (LoggedInUserID == -1)
                            {
                                uint iLastErr = HCNetSDK.NET_DVR_GetLastError();
                                string strErr = "NET_DVR_ManualSnap fail, Err：" + iLastErr;
                            }
                        });
                        if (LoggedInUserID >= 0)
                            isLogin = true;
                    }
                }

                if (isLogin)
                {
                    //HCNetSDK.NET_DVR_CLIENTINFO nETDVRCLIENTINFO = new HCNetSDK.NET_DVR_CLIENTINFO()
                    //{ hPlayWnd = pbCamera.Handle, lChannel = 1, lLinkMode = 0 }; //, sMultiCastIP = ""
                    //HCNetSDK.REALDATACALLBACK realDataCallBack = new HCNetSDK.REALDATACALLBACK(RealDataCallBack_New);

                    UInt32 StreamT = 1;
                    try
                    {
                        string StreamType = !string.IsNullOrEmpty(ConfigurationManager.AppSettings["StreamType"]) ? ConfigurationManager.AppSettings["StreamType"] : "";
                        StreamT = !string.IsNullOrEmpty(StreamType) ? Convert.ToUInt32(StreamType) : 1;

                    }
                    catch { StreamT = 1; }

                    HCNetSDK.NET_DVR_PREVIEWINFO nETDVRCLIENTINFO = new HCNetSDK.NET_DVR_PREVIEWINFO();
                    nETDVRCLIENTINFO.hPlayWnd = pbCamera.Handle;
                    nETDVRCLIENTINFO.lChannel = 1;
                    nETDVRCLIENTINFO.dwStreamType = StreamT;
                    nETDVRCLIENTINFO.dwLinkMode = 0;
                    nETDVRCLIENTINFO.bBlocked = true;
                    nETDVRCLIENTINFO.byProtoType = 0;
                    nETDVRCLIENTINFO.byPreviewMode = 0;

                    IntPtr pUser = new IntPtr();

                    //System.Runtime.InteropServices.GCHandle.Alloc(realDataCallBack);
                    await Task.Run(() =>
                    {
                        RealtimePlayHandle = HCNetSDK.NET_DVR_RealPlay_V40(LoggedInUserID, ref nETDVRCLIENTINFO, null, pUser);
                    });
                    if (RealtimePlayHandle >= 0)
                    {
                        RaiseVideoPlayStarted();
                    }
                }
                else
                {
                    RaiseVideoPlayFailed();
                }
            }
            catch (Exception ex)
            {
                RaiseVideoPlayFailed();
            }
        }
        public async override Task PauseVideo()
        {
            try
            {
                if (RealtimePlayHandle >= 0)
                {
                    bool VideoPaused = await Task.Run(() =>
                    {
                        return HCNetSDK.NET_DVR_StopRealPlay(RealtimePlayHandle);
                    });
                    if (VideoPaused)
                    {
                        RealtimePlayHandle = -1;
                        RaiseVideoPlayPaused();
                    }
                }
            }
            catch (Exception)
            {
            }
        }
        public override async Task StopVideo()
        {
            try
            {
                if (RealtimePlayHandle >= 0)
                {
                    bool VideoStopped = await Task.Run(() => { return HCNetSDK.NET_DVR_StopRealPlay(RealtimePlayHandle); });
                    if (VideoStopped)
                    {
                        RealtimePlayHandle = -1;
                        RaiseVideoPlayStopped();
                        bool LoggedOut = await Task.Run(() => { return HCNetSDK.NET_DVR_Logout_V30(LoggedInUserID); });
                        if (LoggedOut)
                        {
                            isLogin = false;
                            LoggedInUserID = -1;
                            lock (lockVar)
                            {
                                if (isSDKInitialized && HCNetSDK.NET_DVR_Cleanup())
                                    isSDKInitialized = false;
                            }
                        }
                    }
                }
            }
            catch (Exception)
            {
            }
        }



        public override async Task TakeSnapshot(string FilePath)
        {
            int oldHandle = RealtimePlayHandle;
            //If video is stopped/paused, then start video
            if (oldHandle < 0)
                await StartVideo();

            try
            {
                //If video is playing, then take snapshot
                if (RealtimePlayHandle >= 0)
                {
                    //if (HCNetSDK.NET_DVR_SetCapturePictureMode(1)) //JPG                    
                    HCNetSDK.NET_DVR_CapturePicture(RealtimePlayHandle, FilePath);
                }
            }
            catch (Exception)
            {
            }

            //Wait here for 100 msecs for file write to complete
            Task<bool> op = Task.Factory.StartNew<bool>(() =>
            {
                System.Threading.SpinWait.SpinUntil(() => false, 100);
                return true;
            });
            await op;

            //If previously video was paused, and we started it to take a snapshot
            //Then pause the video again after 1 second
            if (oldHandle < 0)
            {
                Task task = Task.Delay(1000).ContinueWith(t => wfh.Dispatcher.BeginInvoke((Action)(() => { PauseVideo(); })));
                task.Wait();
            }
        }

        public override void TakePlateSnapshot(out string PlateNumber)
        {
            try
            {
                var lpInter = new HCNetSDK.NET_DVR_MANUALSNAP();
                var lpOuter = new HCNetSDK.NET_DVR_PLATE_RESULT();

                HCNetSDK.NET_DVR_ManualSnap(LoggedInUserID, ref lpInter, ref lpOuter);

                PlateNumber = lpOuter.struPlateInfo.sLicense;
            }
            catch (Exception)
            {
                PlateNumber = "unknown";
            }
        }

        public override void TakePlateSnapshotWithNumber(string FilePath, out string PlateNumber)
        {
            try
            {
                HCNetSDK.NET_DVR_MANUALSNAP struManualSnap = new HCNetSDK.NET_DVR_MANUALSNAP();
                HCNetSDK.NET_DVR_PLATE_RESULT struPlateResult = new HCNetSDK.NET_DVR_PLATE_RESULT();

                struManualSnap.byOSDEnable = 1;

                struPlateResult.pBuffer1 = Marshal.AllocHGlobal(2 * 1024 * 1024);
                struPlateResult.pBuffer2 = Marshal.AllocHGlobal(2 * 1024 * 1024);

                if (HCNetSDK.NET_DVR_ManualSnap(LoggedInUserID, ref struManualSnap, ref struPlateResult))
                {
                    //sLicense

                    PlateNumber = struPlateResult.struPlateInfo.sLicense;

                    if (struPlateResult.pBuffer2 != IntPtr.Zero && struPlateResult.dwPicPlateLen != 0)
                    {
                        FileStream fs = new FileStream(FilePath, FileMode.Create);
                        int iLen = (int)struPlateResult.dwPicPlateLen;
                        byte[] by = new byte[iLen];
                        Marshal.Copy(struPlateResult.pBuffer2, by, 0, iLen);
                        fs.Write(by, 0, iLen);
                        fs.Close();
                    }

                    Marshal.FreeHGlobal(struPlateResult.pBuffer1);
                    Marshal.FreeHGlobal(struPlateResult.pBuffer2);
                }
                else
                {
                    uint iLastErr = HCNetSDK.NET_DVR_GetLastError();
                    string strErr = "NET_DVR_ManualSnap fail, Err：" + iLastErr;
                    string TempPathFolder = System.IO.Path.GetDirectoryName(FilePath);
                    File.AppendAllText(TempPathFolder + @"\AnprError.txt", strErr);
                    PlateNumber = "NOTFOUND";
                }

            }
            catch (Exception ex)
            {
                string TempPathFolder = System.IO.Path.GetDirectoryName(FilePath);
                File.AppendAllText(TempPathFolder + @"\AnprError.txt", Newtonsoft.Json.JsonConvert.SerializeObject(ex));
                PlateNumber = "ERROR";
            }
        }
        #endregion

        #region --- Events ---
        private void RealDataCallBack_New(int lRealHandle, uint dwDataType, byte[] pBuffer, uint dwBufSize, IntPtr pUser)
        {
        }

        public void RealDataCallBack(Int32 lRealHandle, UInt32 dwDataType, IntPtr pBuffer, UInt32 dwBufSize, IntPtr pUser)
        {
            if (dwBufSize > 0)
            {
                byte[] sData = new byte[dwBufSize];
                Marshal.Copy(pBuffer, sData, 0, (Int32)dwBufSize);

                string str = "ÊµÊ±Á÷Êý¾Ý.ps";
                FileStream fs = new FileStream(str, FileMode.Create);
                int iLen = (int)dwBufSize;
                fs.Write(sData, 0, iLen);
                fs.Close();
            }
        }
        #endregion
    }
}
