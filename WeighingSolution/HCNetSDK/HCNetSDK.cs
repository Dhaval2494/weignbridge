using System;
using System.Runtime.InteropServices;
using System.Text;
using System.IO;
using System.Windows.Forms;
namespace WeighingSolution
{
    /// <summary>
    /// HCNetSDK µÄÕªÒªËµÃ÷¡£
    /// </summary>
    public class HCNetSDK
    {

        public HCNetSDK()
        {
            //
            // TODO: ÔÚ´Ë´¦Ìí¼Ó¹¹Ôìº¯ÊýÂß¼­
            //
        }
        #region ¹«¹²²ÎÊý
        public static string ProtocolType;

        #endregion

        /************************************ÊÓÆµ×ÛºÏÆ½Ì¨(begin)***************************************/
        public const int MAX_SUBSYSTEM_NUM = 80;   //Ò»¸ö¾ØÕóÏµÍ³ÖÐ×î¶à×ÓÏµÍ³ÊýÁ¿
        public const int MAX_SERIALLEN = 36;  //×î´óÐòÁÐºÅ³¤¶È

        public const int MAX_LOOPPLANNUM = 16;  //×î´ó¼Æ»®ÇÐ»»×é
        public const int DECODE_TIMESEGMENT = 4;     //¼Æ»®½âÂëÃ¿ÌìÊ±¼ä¶ÎÊý

        public const int MAX_DOMAIN_NAME = 64;  /* ×î´óÓòÃû³¤¶È */
        public const int MAX_DISKNUM_V30 = 33; //9000Éè±¸×î´óÓ²ÅÌÊý/* ×î¶à33¸öÓ²ÅÌ(°üÀ¨16¸öÄÚÖÃSATAÓ²ÅÌ¡¢1¸öeSATAÓ²ÅÌºÍ16¸öNFSÅÌ) */
        public const int MAX_DAYS = 7;       //Ã¿ÖÜÌìÊý

        public const uint DEVICE_ABILITY_INFO = 0x011;     //Éè±¸ÊÂ¼þÄÜÁ¦£¨V2.0£©

        [System.Runtime.InteropServices.StructLayoutAttribute(System.Runtime.InteropServices.LayoutKind.Sequential, CharSet = System.Runtime.InteropServices.CharSet.Ansi)]
        public struct NET_DVR_SUBSYSTEMINFO
        {
            public byte bySubSystemType;//×ÓÏµÍ³ÀàÐÍ£¬1-½âÂëÓÃ×ÓÏµÍ³£¬2-±àÂëÓÃ×ÓÏµÍ³£¬0-NULL£¨´Ë²ÎÊýÖ»ÄÜ»ñÈ¡£©
            public byte byChan;//×ÓÏµÍ³Í¨µÀÊý£¨´Ë²ÎÊýÖ»ÄÜ»ñÈ¡£©
            public byte byLoginType;//×¢²áÀàÐÍ£¬1-Ö±Á¬£¬2-DNS£¬3-»¨Éú¿Ç
            [System.Runtime.InteropServices.MarshalAsAttribute(System.Runtime.InteropServices.UnmanagedType.ByValArray, SizeConst = 5, ArraySubType = System.Runtime.InteropServices.UnmanagedType.I1)]
            public byte[] byRes1;
            public NET_DVR_IPADDR struSubSystemIP;/*IPµØÖ·£¨¿ÉÐÞ¸Ä£©*/
            public ushort wSubSystemPort;//×ÓÏµÍ³¶Ë¿ÚºÅ£¨¿ÉÐÞ¸Ä£©
            [System.Runtime.InteropServices.MarshalAsAttribute(System.Runtime.InteropServices.UnmanagedType.ByValArray, SizeConst = 6, ArraySubType = System.Runtime.InteropServices.UnmanagedType.I1)]
            public byte[] byRes2;

            public NET_DVR_IPADDR struSubSystemIPMask;//×ÓÍøÑÚÂë
            public NET_DVR_IPADDR struGatewayIpAddr;    /* Íø¹ØµØÖ·*/

            [System.Runtime.InteropServices.MarshalAsAttribute(System.Runtime.InteropServices.UnmanagedType.ByValArray, SizeConst = NAME_LEN, ArraySubType = System.Runtime.InteropServices.UnmanagedType.I1)]
            public byte[] sUserName;/* ÓÃ»§Ãû £¨´Ë²ÎÊýÖ»ÄÜ»ñÈ¡£©*/
            [System.Runtime.InteropServices.MarshalAsAttribute(System.Runtime.InteropServices.UnmanagedType.ByValArray, SizeConst = PASSWD_LEN, ArraySubType = System.Runtime.InteropServices.UnmanagedType.I1)]
            public byte[] sPassword;/*ÃÜÂë£¨´Ë²ÎÊýÖ»ÄÜ»ñÈ¡£©*/

            [System.Runtime.InteropServices.MarshalAsAttribute(System.Runtime.InteropServices.UnmanagedType.ByValTStr, SizeConst = MAX_DOMAIN_NAME)]
            public string sDomainName;//ÓòÃû(¿ÉÐÞ¸Ä)
            [System.Runtime.InteropServices.MarshalAsAttribute(System.Runtime.InteropServices.UnmanagedType.ByValTStr, SizeConst = MAX_DOMAIN_NAME)]
            public string sDnsAddress;/*DNSÓòÃû»òIPµØÖ·*/
            [System.Runtime.InteropServices.MarshalAsAttribute(System.Runtime.InteropServices.UnmanagedType.ByValArray, SizeConst = 6, ArraySubType = System.Runtime.InteropServices.UnmanagedType.I1)]
            public byte[] sSerialNumber;//ÐòÁÐºÅ£¨´Ë²ÎÊýÖ»ÄÜ»ñÈ¡£©
        }

        [System.Runtime.InteropServices.StructLayoutAttribute(System.Runtime.InteropServices.LayoutKind.Sequential)]
        public struct NET_DVR_ALLSUBSYSTEMINFO
        {
            public uint dwSize;
            [System.Runtime.InteropServices.MarshalAsAttribute(System.Runtime.InteropServices.UnmanagedType.ByValArray, SizeConst = MAX_SUBSYSTEM_NUM, ArraySubType = System.Runtime.InteropServices.UnmanagedType.Struct)]
            public NET_DVR_SUBSYSTEMINFO[] struSubSystemInfo;
            [System.Runtime.InteropServices.MarshalAsAttribute(System.Runtime.InteropServices.UnmanagedType.ByValArray, SizeConst = 8, ArraySubType = System.Runtime.InteropServices.UnmanagedType.I1)]
            public byte[] byRes;
        }

        [System.Runtime.InteropServices.StructLayoutAttribute(System.Runtime.InteropServices.LayoutKind.Sequential)]
        public struct NET_DVR_LOOPPLAN_SUBCFG
        {
            public uint dwSize;
            public uint dwPoolTime; /*ÂÖÑ¯¼ä¸ô£¬µ¥Î»£ºÃë*/
            [System.Runtime.InteropServices.MarshalAsAttribute(System.Runtime.InteropServices.UnmanagedType.ByValArray, SizeConst = MAX_CYCLE_CHAN_V30, ArraySubType = System.Runtime.InteropServices.UnmanagedType.Struct)]
            public NET_DVR_MATRIX_CHAN_INFO_V30[] struChanConInfo;
            [System.Runtime.InteropServices.MarshalAsAttribute(System.Runtime.InteropServices.UnmanagedType.ByValArray, SizeConst = 16, ArraySubType = System.Runtime.InteropServices.UnmanagedType.I1)]
            public byte[] byRes;
        }

        [System.Runtime.InteropServices.StructLayoutAttribute(System.Runtime.InteropServices.LayoutKind.Sequential)]
        public struct NET_DVR_LOOPPLAN_ARRAYCFG
        {
            public uint dwSize;
            [System.Runtime.InteropServices.MarshalAsAttribute(System.Runtime.InteropServices.UnmanagedType.ByValArray, SizeConst = MAX_LOOPPLANNUM, ArraySubType = System.Runtime.InteropServices.UnmanagedType.Struct)]
            public NET_DVR_LOOPPLAN_SUBCFG[] struLoopPlanSubCfg;
            [System.Runtime.InteropServices.MarshalAsAttribute(System.Runtime.InteropServices.UnmanagedType.ByValArray, SizeConst = 8, ArraySubType = System.Runtime.InteropServices.UnmanagedType.I1)]
            public byte[] byRes;
        }

        [System.Runtime.InteropServices.StructLayoutAttribute(System.Runtime.InteropServices.LayoutKind.Sequential)]
        public struct NET_DVR_ALARMMODECFG
        {
            public uint dwSize;
            public byte byAlarmMode;//±¨¾¯´¥·¢ÀàÐÍ£¬1-ÂÖÑ¯£¬2-±£³Ö 
            public ushort wLoopTime;//ÂÖÑ¯Ê±¼ä, µ¥Î»£ºÃë 
            [System.Runtime.InteropServices.MarshalAsAttribute(System.Runtime.InteropServices.UnmanagedType.ByValArray, SizeConst = 9, ArraySubType = System.Runtime.InteropServices.UnmanagedType.I1)]
            public byte[] byRes;
        }

        [System.Runtime.InteropServices.StructLayoutAttribute(System.Runtime.InteropServices.LayoutKind.Sequential)]
        public struct NET_DVR_CODESYSTEMINFO
        {
            public byte bySerialNum;//×ÓÏµÍ³ÐòºÅ
            public byte byChan;
            [System.Runtime.InteropServices.MarshalAsAttribute(System.Runtime.InteropServices.UnmanagedType.ByValArray, SizeConst = 6, ArraySubType = System.Runtime.InteropServices.UnmanagedType.I1)]
            public byte[] byRes1;
        }

        [System.Runtime.InteropServices.StructLayoutAttribute(System.Runtime.InteropServices.LayoutKind.Sequential)]
        public struct NET_DVR_CODESPLITTERINFO
        {
            public uint dwSize;
            public NET_DVR_IPADDR struIP;/*Âë·ÖÆ÷IPµØÖ·*/
            public ushort wPort;//Âë·ÖÆ÷¶Ë¿ÚºÅ
            [System.Runtime.InteropServices.MarshalAsAttribute(System.Runtime.InteropServices.UnmanagedType.ByValArray, SizeConst = 6, ArraySubType = System.Runtime.InteropServices.UnmanagedType.I1)]
            public byte[] byRes1;
            [System.Runtime.InteropServices.MarshalAsAttribute(System.Runtime.InteropServices.UnmanagedType.ByValArray, SizeConst = NAME_LEN, ArraySubType = System.Runtime.InteropServices.UnmanagedType.I1)]
            public byte[] sUserName;/* ÓÃ»§Ãû */
            [System.Runtime.InteropServices.MarshalAsAttribute(System.Runtime.InteropServices.UnmanagedType.ByValArray, SizeConst = PASSWD_LEN, ArraySubType = System.Runtime.InteropServices.UnmanagedType.I1)]
            public byte[] sPassword;/*ÃÜÂë */
            public byte byChan;//Âë·ÖÆ÷485ºÅ
            public byte by485Port;//485¿ÚµØÖ·
            public ushort wBaudRate;// ²¨ÌØÂÊ(bps)£¬0£­50£¬1£­75£¬2£­110£¬3£­150£¬4£­300£¬5£­600£¬6£­1200£¬7£­2400£¬8£­4800£¬9£­9600£¬10£­19200£¬ 11£­38400£¬12£­57600£¬13£­76800£¬14£­115.2k;
            public byte byDataBit;//Êý¾ÝÓÐ¼¸Î» 0£­5Î»£¬1£­6Î»£¬2£­7Î»£¬3£­8Î»;
            public byte byStopBit;// Í£Ö¹Î»£º0£­1Î»£¬1£­2Î»;
            public byte byParity;// Ð£Ñé£º0£­ÎÞÐ£Ñé£»1£­ÆæÐ£Ñé£»2£­Å¼Ð£Ñé;
            public byte byFlowControl;// 0£­ÎÞ£»1£­ÈíÁ÷¿Ø£»2-Ó²Á÷¿Ø
            public ushort wDecoderType;// ½âÂëÆ÷ÀàÐÍ, ´Ó0¿ªÊ¼£¬Óë»ñÈ¡µÄ½âÂëÆ÷Ð­ÒéÁÐ±íÖÐµÄÏÂ±êÏà¶ÔÓ¦
            [System.Runtime.InteropServices.MarshalAsAttribute(System.Runtime.InteropServices.UnmanagedType.ByValArray, SizeConst = 6, ArraySubType = System.Runtime.InteropServices.UnmanagedType.I1)]
            public byte[] byRes2;
        }
        [System.Runtime.InteropServices.StructLayoutAttribute(System.Runtime.InteropServices.LayoutKind.Sequential)]
        public struct NET_DVR_CODESPLITTERCFG
        {
            public uint dwSize;
            public NET_DVR_CODESYSTEMINFO struCodeSubsystemInfo;//±àÂë×ÓÏµÍ³¶ÔÓ¦ÐÅÏ¢
            public NET_DVR_CODESPLITTERINFO struCodeSplitterInfo;//Âë·ÖÆ÷ÐÅÏ¢
            [System.Runtime.InteropServices.MarshalAsAttribute(System.Runtime.InteropServices.UnmanagedType.ByValArray, SizeConst = 8, ArraySubType = System.Runtime.InteropServices.UnmanagedType.I1)]
            public byte[] byRes;
        }

        [System.Runtime.InteropServices.StructLayoutAttribute(System.Runtime.InteropServices.LayoutKind.Sequential)]
        public struct NET_DVR_ASSOCIATECFG
        {
            public byte byAssociateType;//¹ØÁªÀàÐÍ£¬1-±¨¾¯
            public ushort wAlarmDelay;//±¨¾¯ÑÓÊ±£¬0£­5Ãë£»1£­10Ãë£»2£­30Ãë£»3£­1·ÖÖÓ£»4£­2·ÖÖÓ£»5£­5·ÖÖÓ£»6£­10·ÖÖÓ£»
            public byte byAlarmNum;//±¨¾¯ºÅ£¬¾ßÌåµÄÖµÓÉÓ¦ÓÃ¸³£¬ÏàÍ¬µÄ±¨¾¯¸³ÏàÍ¬µÄÖµ
            [System.Runtime.InteropServices.MarshalAsAttribute(System.Runtime.InteropServices.UnmanagedType.ByValArray, SizeConst = 8, ArraySubType = System.Runtime.InteropServices.UnmanagedType.I1)]
            public byte[] byRes;
        }

        [System.Runtime.InteropServices.StructLayoutAttribute(System.Runtime.InteropServices.LayoutKind.Sequential)]
        public struct NET_DVR_DYNAMICDECODE
        {
            public uint dwSize;
            public NET_DVR_ASSOCIATECFG struAssociateCfg;//´¥·¢¶¯Ì¬½âÂë¹ØÁª½á¹¹
            public NET_DVR_PU_STREAM_CFG struPuStreamCfg;//¶¯Ì¬½âÂë½á¹¹
            [System.Runtime.InteropServices.MarshalAsAttribute(System.Runtime.InteropServices.UnmanagedType.ByValArray, SizeConst = 8, ArraySubType = System.Runtime.InteropServices.UnmanagedType.I1)]
            public byte[] byRes;
        }
        [System.Runtime.InteropServices.StructLayoutAttribute(System.Runtime.InteropServices.LayoutKind.Sequential)]
        public struct NET_DVR_DECODESCHED
        {
            public NET_DVR_SCHEDTIME struSchedTime;
            public byte byDecodeType;/*0-ÎÞ£¬1-ÂÖÑ¯½âÂë£¬2-¶¯Ì¬½âÂë*/
            public byte byLoopGroup;//ÂÖÑ¯×éºÅ
            [System.Runtime.InteropServices.MarshalAsAttribute(System.Runtime.InteropServices.UnmanagedType.ByValArray, SizeConst = 6, ArraySubType = System.Runtime.InteropServices.UnmanagedType.I1)]
            public byte[] byRes;
            public NET_DVR_PU_STREAM_CFG struDynamicDec;//¶¯Ì¬½âÂë
        }
        [System.Runtime.InteropServices.StructLayoutAttribute(System.Runtime.InteropServices.LayoutKind.Sequential)]
        public struct NET_DVR_PLANDECODE
        {
            public uint dwSize;
            [System.Runtime.InteropServices.MarshalAsAttribute(System.Runtime.InteropServices.UnmanagedType.ByValArray, SizeConst = MAX_DAYS * DECODE_TIMESEGMENT, ArraySubType = System.Runtime.InteropServices.UnmanagedType.Struct)]
            public NET_DVR_DECODESCHED[] struDecodeSched;//ÖÜÒ»×÷Îª¿ªÊ¼£¬ºÍ9000Ò»ÖÂ
            [System.Runtime.InteropServices.MarshalAsAttribute(System.Runtime.InteropServices.UnmanagedType.ByValArray, SizeConst = 8, ArraySubType = System.Runtime.InteropServices.UnmanagedType.I1)]
            public byte[] byres;
        }
        [System.Runtime.InteropServices.StructLayoutAttribute(System.Runtime.InteropServices.LayoutKind.Sequential)]
        public struct NET_DVR_VIDEOPLATFORM_ABILITY
        {
            public uint dwSize;
            public byte byCodeSubSystemNums;//±àÂë×ÓÏµÍ³ÊýÁ¿
            public byte byDecodeSubSystemNums;//½âÂë×ÓÏµÍ³ÊýÁ¿
            [System.Runtime.InteropServices.MarshalAsAttribute(System.Runtime.InteropServices.UnmanagedType.ByValArray, SizeConst = 12, ArraySubType = System.Runtime.InteropServices.UnmanagedType.I1)]
            public byte[] byWindowMode; /*ÏÔÊ¾Í¨µÀÖ§³ÖµÄ´°¿ÚÄ£Ê½*/
            [System.Runtime.InteropServices.MarshalAsAttribute(System.Runtime.InteropServices.UnmanagedType.ByValArray, SizeConst = 16, ArraySubType = System.Runtime.InteropServices.UnmanagedType.I1)]
            public byte[] byRes;
        }

        public const int VIDEOPLATFORM_ABILITY = 0x210; //ÊÓÆµ×ÛºÏÆ½Ì¨ÄÜÁ¦¼¯
        /************************************ÊÓÆµ×ÛºÏÆ½Ì¨(end)***************************************/

        //SDKÀàÐÍ
        public const int SDK_PLAYMPEG4 = 1;//²¥·Å¿â
        public const int SDK_HCNETSDK = 2;//ÍøÂç¿â

        //Êý¾Ý¿â²Ù×÷NVRÐÅÏ¢
        //Êý¾Ý¿â²Ù×÷
        public const int INSERTTYPE = 0;        //´ú±í²åÈë
        public const int MODIFYTYPE = 1;        //´ú±íÐÞ¸Ä
        public const int DELETETYPE = 2;        //´ú±íÉ¾³ý
        /****************************************ÈÕÖ¾²Ù×÷******************************************/
        //²Ù×÷ÀàÐÍ
        public const int DEF_OPE_PREVIEW = 1;   //Ô¤ÀÀ
        public const int DEF_OPE_TALK = 2;  //¶Ô½²
        public const int DEF_OPE_SETALARM = 3;   //²¼·À
        public const int DEF_OPE_PTZCTRL = 4;   //ÔÆÌ¨¿ØÖÆ
        public const int DEF_OPE_VIDEOPARAM = 5;   //ÊÓÆµ²ÎÊýÉèÖÃ
        public const int DEF_OPE_PLAYBACK = 6;   //»Ø·Å
        public const int DEF_OPE_REMOTECFG = 7;   //Ô¶³ÌÅäÖÃ
        public const int DEF_OPE_GETSERVSTATE = 8;   //»ñÈ¡Éè±¸×´Ì¬
        public const int DEF_OPE_CHECKTIME = 9;   //Ð£Ê±



        //²Ù×÷ÈÕÖ¾ÄÚÈÝ	
        public const int DEF_OPE_PRE_STARTPREVIEW = 1;   //¿ªÊ¼Ô¤ÀÀ
        public const int DEF_OPE_PRE_STOPPREVIEW = 2;   //Í£Ö¹Ô¤ÀÀ
        public const int DEF_OPE_PRE_STRATCYCPLAY = 3;   //¿ªÊ¼Ñ­»·²¥·Å
        public const int DEF_OPE_PRE_STOPCYCPLAY = 4;   //Í£Ö¹Ñ­»·²¥·Å
        public const int DEF_OPE_PRE_STARTRECORD = 5;   //¿ªÊ¼Â¼Ïñ
        public const int DEF_OPE_PRE_STOPRECORD = 6;   //Í£Ö¹Â¼Ïñ
        public const int DEF_OPE_PRE_CAPTURE = 7;   //×¥Í¼
        public const int DEF_OPE_PRE_OPENSOUND = 8;   //´ò¿ªÉùÒô
        public const int DEF_OPE_PRE_CLOSESOUND = 9;   //¹Ø±ÕÉùÒô

        //¶Ô½²
        public const int DEF_OPE_TALK_STARTTALK = 1;   //¿ªÊ¼¶Ô½²
        public const int DEF_OPE_TALK_STOPTALK = 2;   //Í£Ö¹¶Ô½²

        public const int DEF_OPE_ALARM_SETALARM = 1;   //²¼·À
        public const int DEF_OPE_ALARM_WITHDRAWALARM = 2;   //³··À

        public const int DEF_OPE_PTZ_PTZCTRL = 1;   //ÔÆÌ¨¿ØÖÆ

        public const int DEF_OPE_VIDEOPARAM_SET = 1;   //ÊÓÆµ²ÎÊý

        //»Ø·Å
        public const int DEF_OPE_PLAYBACK_LOCALSEARCH = 1;   //±¾µØ»Ø·Å²éÑ¯ÎÄ¼þ
        public const int DEF_OPE_PLAYBACK_LOCALPLAY = 2;   //±¾µØ»Ø·ÅÎÄ¼þ
        public const int DEF_OPE_PLAYBACK_LOCALDOWNLOAD = 3;   //±¾µØ»Ø·ÅÏÂÔØÎÄ¼þ
        public const int DEF_OPE_PLAYBACK_REMOTESEARCH = 4;   //Ô¶³Ì»Ø·Å²éÑ¯ÎÄ¼þ
        public const int DEF_OPE_PLAYBACK_REMOTEPLAYFILE = 5;   //Ô¶³Ì°´ÎÄ¼þ»Ø·Å
        public const int DEF_OPE_PLAYBACK_REMOTEPLAYTIME = 6;   //Ô¶³Ì°´Ê±¼ä»Ø·Å
        public const int DEF_OPE_PLAYBACK_REMOTEDOWNLOAD = 7;   //Ô¶³Ì»Ø·ÅÏÂÔØÎÄ¼þ

        public const int DEF_OPE_REMOTE_REMOTECFG = 1;   //Ô¶³Ì²ÎÊýÅäÖÃ

        public const int DEF_OPE_STATE_GETSERVSTATE = 1;//»ñÈ¡Éè±¸×´Ì¬

        public const int DEF_OPE_CHECKT_CHECKTIME = 1;//Ð£Ê±

        //±¨¾¯ÀàÐÍ
        public const int DEF_ALARM_IO = 1;   //ÐÅºÅÁ¿±¨¾¯
        public const int DEF_ALARM_HARDFULL = 2;   //Ó²ÅÌÂú±¨¾¯
        public const int DEF_ALARM_VL = 3;  //ÊÓÆµÐÅºÅ¶ªÊ§±¨¾¯
        public const int DEF_ALARM_MV = 4;   //ÒÆ¶¯Õì²â±¨¾¯
        public const int DEF_ALARM_HARDFORMAT = 5;   //Ó²ÅÌÎ´¸ñÊ½»¯±¨¾¯
        public const int DEF_ALARM_HARDERROR = 6;   //Ó²ÅÌ´í±¨¾¯
        public const int DEF_ALARM_VH = 7;   //ÕÚµ²±¨¾¯
        public const int DEF_ALARM_NOPATCH = 8;   //ÖÆÊ½²»Æ¥Åä±¨¾¯
        public const int DEF_ALARM_ERRORVISIT = 9;   //·Ç·¨·ÃÎÊ±¨¾¯
        public const int DEF_ALARM_EXCEPTION = 10;  //Ñ²¼ìÒì³£
        public const int DEF_ALARM_RECERROR = 11;  //Ñ²¼ìÒì³£

        //ÏµÍ³ÈÕÖ¾ÀàÐÍ
        public const int DEF_SYS_LOGIN = 1;   //µÇÂ½ 
        public const int DEF_SYS_LOGOUT = 2;   //×¢Ïú
        public const int DEF_SYS_LOCALCFG = 3;   //±¾µØÅäÖÃ

        /****************************************ÈÕÖ¾²Ù×÷******************************************/


        public const int NAME_LEN = 32;//ÓÃ»§Ãû³¤¶È
        public const int PASSWD_LEN = 16;//ÃÜÂë³¤¶È
        public const int MAX_NAMELEN = 16;//DVR±¾µØµÇÂ½Ãû
        public const int MAX_RIGHT = 32;//Éè±¸Ö§³ÖµÄÈ¨ÏÞ£¨1-12±íÊ¾±¾µØÈ¨ÏÞ£¬13-32±íÊ¾Ô¶³ÌÈ¨ÏÞ£©
        public const int SERIALNO_LEN = 48;//ÐòÁÐºÅ³¤¶È
        public const int MACADDR_LEN = 6;//macµØÖ·³¤¶È
        public const int MAX_ETHERNET = 2;//Éè±¸¿ÉÅäÒÔÌ«ÍøÂç
        public const int PATHNAME_LEN = 128;//Â·¾¶³¤¶È

        public const int MAX_TIMESEGMENT_V30 = 8;//9000Éè±¸×î´óÊ±¼ä¶ÎÊý
        public const int MAX_TIMESEGMENT = 4;//8000Éè±¸×î´óÊ±¼ä¶ÎÊý

        public const int MAX_SHELTERNUM = 4;//8000Éè±¸×î´óÕÚµ²ÇøÓòÊý
        public const int PHONENUMBER_LEN = 32;//pppoe²¦ºÅºÅÂë×î´ó³¤¶È

        public const int MAX_DISKNUM = 16;//8000Éè±¸×î´óÓ²ÅÌÊý
        public const int MAX_DISKNUM_V10 = 8;//1.2°æ±¾Ö®Ç°°æ±¾

        public const int MAX_WINDOW_V30 = 32;//9000Éè±¸±¾µØÏÔÊ¾×î´ó²¥·Å´°¿ÚÊý
        public const int MAX_WINDOW = 16;//8000Éè±¸×î´óÓ²ÅÌÊý
        public const int MAX_VGA_V30 = 4;//9000Éè±¸×î´ó¿É½ÓVGAÊý
        public const int MAX_VGA = 1;//8000Éè±¸×î´ó¿É½ÓVGAÊý

        public const int MAX_USERNUM_V30 = 32;//9000Éè±¸×î´óÓÃ»§Êý
        public const int MAX_USERNUM = 16;//8000Éè±¸×î´óÓÃ»§Êý
        public const int MAX_EXCEPTIONNUM_V30 = 32;//9000Éè±¸×î´óÒì³£´¦ÀíÊý
        public const int MAX_EXCEPTIONNUM = 16;//8000Éè±¸×î´óÒì³£´¦ÀíÊý
        public const int MAX_LINK = 6;//8000Éè±¸µ¥Í¨µÀ×î´óÊÓÆµÁ÷Á¬½ÓÊý

        public const int MAX_DECPOOLNUM = 4;//µ¥Â·½âÂëÆ÷Ã¿¸ö½âÂëÍ¨µÀ×î´ó¿ÉÑ­»·½âÂëÊý
        public const int MAX_DECNUM = 4;//µ¥Â·½âÂëÆ÷µÄ×î´ó½âÂëÍ¨µÀÊý£¨Êµ¼ÊÖ»ÓÐÒ»¸ö£¬ÆäËûÈý¸ö±£Áô£©
        public const int MAX_TRANSPARENTNUM = 2;//µ¥Â·½âÂëÆ÷¿ÉÅäÖÃ×î´óÍ¸Ã÷Í¨µÀÊý
        public const int MAX_CYCLE_CHAN = 16; //µ¥Â·½âÂëÆ÷×î´óÂÖÑ­Í¨µÀÊý
        public const int MAX_CYCLE_CHAN_V30 = 64;//×î´óÂÖÑ¯Í¨µÀÊý£¨À©Õ¹£©
        public const int MAX_DIRNAME_LENGTH = 80;//×î´óÄ¿Â¼³¤¶È
        public const int MAX_WINDOWS = 16;//×î´ó´°¿ÚÊý

        public const int MAX_STRINGNUM_V30 = 8;//9000Éè±¸×î´óOSD×Ö·ûÐÐÊýÊý
        public const int MAX_STRINGNUM = 4;//8000Éè±¸×î´óOSD×Ö·ûÐÐÊýÊý
        public const int MAX_STRINGNUM_EX = 8;//8000¶¨ÖÆÀ©Õ¹
        public const int MAX_AUXOUT_V30 = 16;//9000Éè±¸×î´ó¸¨ÖúÊä³öÊý
        public const int MAX_AUXOUT = 4;//8000Éè±¸×î´ó¸¨ÖúÊä³öÊý
        public const int MAX_HD_GROUP = 16;//9000Éè±¸×î´óÓ²ÅÌ×éÊý
        public const int MAX_NFS_DISK = 8; //8000Éè±¸×î´óNFSÓ²ÅÌÊý

        public const int IW_ESSID_MAX_SIZE = 32;//WIFIµÄSSIDºÅ³¤¶È
        public const int IW_ENCODING_TOKEN_MAX = 32;//WIFIÃÜËø×î´ó×Ö½ÚÊý
        public const int MAX_SERIAL_NUM = 64;//×î¶àÖ§³ÖµÄÍ¸Ã÷Í¨µÀÂ·Êý
        public const int MAX_DDNS_NUMS = 10;//9000Éè±¸×î´ó¿ÉÅäddnsÊý
        public const int MAX_EMAIL_ADDR_LEN = 48;//×î´óemailµØÖ·³¤¶È
        public const int MAX_EMAIL_PWD_LEN = 32;//×î´óemailÃÜÂë³¤¶È

        public const int MAXPROGRESS = 100;//»Ø·ÅÊ±µÄ×î´ó°Ù·ÖÂÊ
        public const int MAX_SERIALNUM = 2;//8000Éè±¸Ö§³ÖµÄ´®¿ÚÊý 1-232£¬ 2-485
        public const int CARDNUM_LEN = 20;//¿¨ºÅ³¤¶È
        public const int MAX_VIDEOOUT_V30 = 4;//9000Éè±¸µÄÊÓÆµÊä³öÊý
        public const int MAX_VIDEOOUT = 2;//8000Éè±¸µÄÊÓÆµÊä³öÊý

        public const int MAX_PRESET_V30 = 256;// 9000Éè±¸Ö§³ÖµÄÔÆÌ¨Ô¤ÖÃµãÊý
        public const int MAX_TRACK_V30 = 256;// 9000Éè±¸Ö§³ÖµÄÔÆÌ¨¹ì¼£Êý
        public const int MAX_CRUISE_V30 = 256;// 9000Éè±¸Ö§³ÖµÄÔÆÌ¨Ñ²º½Êý
        public const int MAX_PRESET = 128;// 8000Éè±¸Ö§³ÖµÄÔÆÌ¨Ô¤ÖÃµãÊý 
        public const int MAX_TRACK = 128;// 8000Éè±¸Ö§³ÖµÄÔÆÌ¨¹ì¼£Êý
        public const int MAX_CRUISE = 128;// 8000Éè±¸Ö§³ÖµÄÔÆÌ¨Ñ²º½Êý 

        public const int CRUISE_MAX_PRESET_NUMS = 32;// Ò»ÌõÑ²º½×î¶àµÄÑ²º½µã 

        public const int MAX_SERIAL_PORT = 8;//9000Éè±¸Ö§³Ö232´®¿ÚÊý
        public const int MAX_PREVIEW_MODE = 8;// Éè±¸Ö§³Ö×î´óÔ¤ÀÀÄ£Ê½ÊýÄ¿ 1»­Ãæ,4»­Ãæ,9»­Ãæ,16»­Ãæ.... 
        public const int MAX_MATRIXOUT = 16;// ×î´óÄ£Äâ¾ØÕóÊä³ö¸öÊý 
        public const int LOG_INFO_LEN = 11840; // ÈÕÖ¾¸½¼ÓÐÅÏ¢ 
        public const int DESC_LEN = 16;// ÔÆÌ¨ÃèÊö×Ö·û´®³¤¶È 
        public const int PTZ_PROTOCOL_NUM = 200;// 9000×î´óÖ§³ÖµÄÔÆÌ¨Ð­ÒéÊý 

        public const int MAX_AUDIO = 1;//8000ÓïÒô¶Ô½²Í¨µÀÊý
        public const int MAX_AUDIO_V30 = 2;//9000ÓïÒô¶Ô½²Í¨µÀÊý
        public const int MAX_CHANNUM = 16;//8000Éè±¸×î´óÍ¨µÀÊý
        public const int MAX_ALARMIN = 16;//8000Éè±¸×î´ó±¨¾¯ÊäÈëÊý
        public const int MAX_ALARMOUT = 4;//8000Éè±¸×î´ó±¨¾¯Êä³öÊý
                                          //9000 IPC½ÓÈë
        public const int MAX_ANALOG_CHANNUM = 32;//×î´ó32¸öÄ£ÄâÍ¨µÀ
        public const int MAX_ANALOG_ALARMOUT = 32; //×î´ó32Â·Ä£Äâ±¨¾¯Êä³ö 
        public const int MAX_ANALOG_ALARMIN = 32;//×î´ó32Â·Ä£Äâ±¨¾¯ÊäÈë

        public const int MAX_IP_DEVICE = 32;//ÔÊÐí½ÓÈëµÄ×î´óIPÉè±¸Êý
        public const int MAX_IP_CHANNEL = 32;//ÔÊÐí¼ÓÈëµÄ×î¶àIPÍ¨µÀÊý
        public const int MAX_IP_ALARMIN = 128;//ÔÊÐí¼ÓÈëµÄ×î¶à±¨¾¯ÊäÈëÊý
        public const int MAX_IP_ALARMOUT = 64;//ÔÊÐí¼ÓÈëµÄ×î¶à±¨¾¯Êä³öÊý

        //SDK_V31 ATM
        public const int MAX_ATM_NUM = 1;
        public const int MAX_ACTION_TYPE = 12;
        public const int ATM_FRAMETYPE_NUM = 4;
        public const int MAX_ATM_PROTOCOL_NUM = 1025;
        public const int ATM_PROTOCOL_SORT = 4;
        public const int ATM_DESC_LEN = 32;
        // SDK_V31 ATM

        /* ×î´óÖ§³ÖµÄÍ¨µÀÊý ×î´óÄ£Äâ¼ÓÉÏ×î´óIPÖ§³Ö */
        public const int MAX_CHANNUM_V30 = MAX_ANALOG_CHANNUM + MAX_IP_CHANNEL;//64
        public const int MAX_ALARMOUT_V30 = MAX_ANALOG_ALARMOUT + MAX_IP_ALARMOUT;//96
        public const int MAX_ALARMIN_V30 = MAX_ANALOG_ALARMIN + MAX_IP_ALARMIN;//160

        public const int MAX_INTERVAL_NUM = 4;

        //ÂëÁ÷Á¬½Ó·½Ê½
        public const int NORMALCONNECT = 1;
        public const int MEDIACONNECT = 2;

        //Éè±¸ÐÍºÅ(´óÀà)
        public const int HCDVR = 1;
        public const int MEDVR = 2;
        public const int PCDVR = 3;
        public const int HC_9000 = 4;
        public const int HF_I = 5;
        public const int PCNVR = 6;
        public const int HC_76NVR = 8;

        //NVRÀàÐÍ
        public const int DS8000HC_NVR = 0;
        public const int DS9000HC_NVR = 1;
        public const int DS8000ME_NVR = 2;

        //ÔÚÏß²âÎÂ±¨¾¯
        public const int THERMOMETRY_ALARMRULE_NUM = 40;           //ÈÈ³ÉÏñ±¨¾¯¹æÔòÊý
        public const int MAX_THERMOMETRY_REGION_NUM = 40;         //ÈÈ¶ÈÍ¼¼ì²âÇøÓò×î´óÖ§³ÖÊý
        public const int MAX_THERMOMETRY_DIFFCOMPARISON_NUM = 40; //ÈÈ³ÉÏñÎÂ²î±¨¾¯¹æÔòÊý

        public const int ISAPI_STATUS_LEN = 4096 * 4;
        public const int ISAPI_DATA_LEN = 4096 * 4;

        /*******************È«¾Ö´íÎóÂë begin**********************/
        public const int NET_DVR_NOERROR = 0;//Ã»ÓÐ´íÎó
        public const int NET_DVR_PASSWORD_ERROR = 1;//ÓÃ»§ÃûÃÜÂë´íÎó
        public const int NET_DVR_NOENOUGHPRI = 2;//È¨ÏÞ²»×ã
        public const int NET_DVR_NOINIT = 3;//Ã»ÓÐ³õÊ¼»¯
        public const int NET_DVR_CHANNEL_ERROR = 4;//Í¨µÀºÅ´íÎó
        public const int NET_DVR_OVER_MAXLINK = 5;//Á¬½Óµ½DVRµÄ¿Í»§¶Ë¸öÊý³¬¹ý×î´ó
        public const int NET_DVR_VERSIONNOMATCH = 6;//°æ±¾²»Æ¥Åä
        public const int NET_DVR_NETWORK_FAIL_CONNECT = 7;//Á¬½Ó·þÎñÆ÷Ê§°Ü
        public const int NET_DVR_NETWORK_SEND_ERROR = 8;//Ïò·þÎñÆ÷·¢ËÍÊ§°Ü
        public const int NET_DVR_NETWORK_RECV_ERROR = 9;//´Ó·þÎñÆ÷½ÓÊÕÊý¾ÝÊ§°Ü
        public const int NET_DVR_NETWORK_RECV_TIMEOUT = 10;//´Ó·þÎñÆ÷½ÓÊÕÊý¾Ý³¬Ê±
        public const int NET_DVR_NETWORK_ERRORDATA = 11;//´«ËÍµÄÊý¾ÝÓÐÎó
        public const int NET_DVR_ORDER_ERROR = 12;//µ÷ÓÃ´ÎÐò´íÎó
        public const int NET_DVR_OPERNOPERMIT = 13;//ÎÞ´ËÈ¨ÏÞ
        public const int NET_DVR_COMMANDTIMEOUT = 14;//DVRÃüÁîÖ´ÐÐ³¬Ê±
        public const int NET_DVR_ERRORSERIALPORT = 15;//´®¿ÚºÅ´íÎó
        public const int NET_DVR_ERRORALARMPORT = 16;//±¨¾¯¶Ë¿Ú´íÎó
        public const int NET_DVR_PARAMETER_ERROR = 17;//²ÎÊý´íÎó
        public const int NET_DVR_CHAN_EXCEPTION = 18;//·þÎñÆ÷Í¨µÀ´¦ÓÚ´íÎó×´Ì¬
        public const int NET_DVR_NODISK = 19;//Ã»ÓÐÓ²ÅÌ
        public const int NET_DVR_ERRORDISKNUM = 20;//Ó²ÅÌºÅ´íÎó
        public const int NET_DVR_DISK_FULL = 21;//·þÎñÆ÷Ó²ÅÌÂú
        public const int NET_DVR_DISK_ERROR = 22;//·þÎñÆ÷Ó²ÅÌ³ö´í
        public const int NET_DVR_NOSUPPORT = 23;//·þÎñÆ÷²»Ö§³Ö
        public const int NET_DVR_BUSY = 24;//·þÎñÆ÷Ã¦
        public const int NET_DVR_MODIFY_FAIL = 25;//·þÎñÆ÷ÐÞ¸Ä²»³É¹¦
        public const int NET_DVR_PASSWORD_FORMAT_ERROR = 26;//ÃÜÂëÊäÈë¸ñÊ½²»ÕýÈ·
        public const int NET_DVR_DISK_FORMATING = 27;//Ó²ÅÌÕýÔÚ¸ñÊ½»¯£¬²»ÄÜÆô¶¯²Ù×÷
        public const int NET_DVR_DVRNORESOURCE = 28;//DVR×ÊÔ´²»×ã
        public const int NET_DVR_DVROPRATEFAILED = 29;//DVR²Ù×÷Ê§°Ü
        public const int NET_DVR_OPENHOSTSOUND_FAIL = 30;//´ò¿ªPCÉùÒôÊ§°Ü
        public const int NET_DVR_DVRVOICEOPENED = 31;//·þÎñÆ÷ÓïÒô¶Ô½²±»Õ¼ÓÃ
        public const int NET_DVR_TIMEINPUTERROR = 32;//Ê±¼äÊäÈë²»ÕýÈ·
        public const int NET_DVR_NOSPECFILE = 33;//»Ø·ÅÊ±·þÎñÆ÷Ã»ÓÐÖ¸¶¨µÄÎÄ¼þ
        public const int NET_DVR_CREATEFILE_ERROR = 34;//´´½¨ÎÄ¼þ³ö´í
        public const int NET_DVR_FILEOPENFAIL = 35;//´ò¿ªÎÄ¼þ³ö´í
        public const int NET_DVR_OPERNOTFINISH = 36; //ÉÏ´ÎµÄ²Ù×÷»¹Ã»ÓÐÍê³É
        public const int NET_DVR_GETPLAYTIMEFAIL = 37;//»ñÈ¡µ±Ç°²¥·ÅµÄÊ±¼ä³ö´í
        public const int NET_DVR_PLAYFAIL = 38;//²¥·Å³ö´í
        public const int NET_DVR_FILEFORMAT_ERROR = 39;//ÎÄ¼þ¸ñÊ½²»ÕýÈ·
        public const int NET_DVR_DIR_ERROR = 40;//Â·¾¶´íÎó
        public const int NET_DVR_ALLOC_RESOURCE_ERROR = 41;//×ÊÔ´·ÖÅä´íÎó
        public const int NET_DVR_AUDIO_MODE_ERROR = 42;//Éù¿¨Ä£Ê½´íÎó
        public const int NET_DVR_NOENOUGH_BUF = 43;//»º³åÇøÌ«Ð¡
        public const int NET_DVR_CREATESOCKET_ERROR = 44;//´´½¨SOCKET³ö´í
        public const int NET_DVR_SETSOCKET_ERROR = 45;//ÉèÖÃSOCKET³ö´í
        public const int NET_DVR_MAX_NUM = 46;//¸öÊý´ïµ½×î´ó
        public const int NET_DVR_USERNOTEXIST = 47;//ÓÃ»§²»´æÔÚ
        public const int NET_DVR_WRITEFLASHERROR = 48;//Ð´FLASH³ö´í
        public const int NET_DVR_UPGRADEFAIL = 49;//DVRÉý¼¶Ê§°Ü
        public const int NET_DVR_CARDHAVEINIT = 50;//½âÂë¿¨ÒÑ¾­³õÊ¼»¯¹ý
        public const int NET_DVR_PLAYERFAILED = 51;//µ÷ÓÃ²¥·Å¿âÖÐÄ³¸öº¯ÊýÊ§°Ü
        public const int NET_DVR_MAX_USERNUM = 52;//Éè±¸¶ËÓÃ»§Êý´ïµ½×î´ó
        public const int NET_DVR_GETLOCALIPANDMACFAIL = 53;//»ñµÃ¿Í»§¶ËµÄIPµØÖ·»òÎïÀíµØÖ·Ê§°Ü
        public const int NET_DVR_NOENCODEING = 54;//¸ÃÍ¨µÀÃ»ÓÐ±àÂë
        public const int NET_DVR_IPMISMATCH = 55;//IPµØÖ·²»Æ¥Åä
        public const int NET_DVR_MACMISMATCH = 56;//MACµØÖ·²»Æ¥Åä
        public const int NET_DVR_UPGRADELANGMISMATCH = 57;//Éý¼¶ÎÄ¼þÓïÑÔ²»Æ¥Åä
        public const int NET_DVR_MAX_PLAYERPORT = 58;//²¥·ÅÆ÷Â·Êý´ïµ½×î´ó
        public const int NET_DVR_NOSPACEBACKUP = 59;//±¸·ÝÉè±¸ÖÐÃ»ÓÐ×ã¹»¿Õ¼ä½øÐÐ±¸·Ý
        public const int NET_DVR_NODEVICEBACKUP = 60;//Ã»ÓÐÕÒµ½Ö¸¶¨µÄ±¸·ÝÉè±¸
        public const int NET_DVR_PICTURE_BITS_ERROR = 61;//Í¼ÏñËØÎ»Êý²»·û£¬ÏÞ24É«
        public const int NET_DVR_PICTURE_DIMENSION_ERROR = 62;//Í¼Æ¬¸ß*¿í³¬ÏÞ£¬ ÏÞ128*256
        public const int NET_DVR_PICTURE_SIZ_ERROR = 63;//Í¼Æ¬´óÐ¡³¬ÏÞ£¬ÏÞ100K
        public const int NET_DVR_LOADPLAYERSDKFAILED = 64;//ÔØÈëµ±Ç°Ä¿Â¼ÏÂPlayer Sdk³ö´í
        public const int NET_DVR_LOADPLAYERSDKPROC_ERROR = 65;//ÕÒ²»µ½Player SdkÖÐÄ³¸öº¯ÊýÈë¿Ú
        public const int NET_DVR_LOADDSSDKFAILED = 66;//ÔØÈëµ±Ç°Ä¿Â¼ÏÂDSsdk³ö´í
        public const int NET_DVR_LOADDSSDKPROC_ERROR = 67;//ÕÒ²»µ½DsSdkÖÐÄ³¸öº¯ÊýÈë¿Ú
        public const int NET_DVR_DSSDK_ERROR = 68;//µ÷ÓÃÓ²½âÂë¿âDsSdkÖÐÄ³¸öº¯ÊýÊ§°Ü
        public const int NET_DVR_VOICEMONOPOLIZE = 69;//Éù¿¨±»¶ÀÕ¼
        public const int NET_DVR_JOINMULTICASTFAILED = 70;//¼ÓÈë¶à²¥×éÊ§°Ü
        public const int NET_DVR_CREATEDIR_ERROR = 71;//½¨Á¢ÈÕÖ¾ÎÄ¼þÄ¿Â¼Ê§°Ü
        public const int NET_DVR_BINDSOCKET_ERROR = 72;//°ó¶¨Ì×½Ó×ÖÊ§°Ü
        public const int NET_DVR_SOCKETCLOSE_ERROR = 73;//socketÁ¬½ÓÖÐ¶Ï£¬´Ë´íÎóÍ¨³£ÊÇÓÉÓÚÁ¬½ÓÖÐ¶Ï»òÄ¿µÄµØ²»¿É´ï
        public const int NET_DVR_USERID_ISUSING = 74;//×¢ÏúÊ±ÓÃ»§IDÕýÔÚ½øÐÐÄ³²Ù×÷
        public const int NET_DVR_SOCKETLISTEN_ERROR = 75;//¼àÌýÊ§°Ü
        public const int NET_DVR_PROGRAM_EXCEPTION = 76;//³ÌÐòÒì³£
        public const int NET_DVR_WRITEFILE_FAILED = 77;//Ð´ÎÄ¼þÊ§°Ü
        public const int NET_DVR_FORMAT_READONLY = 78;//½ûÖ¹¸ñÊ½»¯Ö»¶ÁÓ²ÅÌ
        public const int NET_DVR_WITHSAMEUSERNAME = 79;//ÓÃ»§ÅäÖÃ½á¹¹ÖÐ´æÔÚÏàÍ¬µÄÓÃ»§Ãû
        public const int NET_DVR_DEVICETYPE_ERROR = 80;//µ¼Èë²ÎÊýÊ±Éè±¸ÐÍºÅ²»Æ¥Åä
        public const int NET_DVR_LANGUAGE_ERROR = 81;//µ¼Èë²ÎÊýÊ±ÓïÑÔ²»Æ¥Åä
        public const int NET_DVR_PARAVERSION_ERROR = 82;//µ¼Èë²ÎÊýÊ±Èí¼þ°æ±¾²»Æ¥Åä
        public const int NET_DVR_IPCHAN_NOTALIVE = 83; //Ô¤ÀÀÊ±Íâ½ÓIPÍ¨µÀ²»ÔÚÏß
        public const int NET_DVR_RTSP_SDK_ERROR = 84;//¼ÓÔØ¸ßÇåIPCÍ¨Ñ¶¿âStreamTransClient.dllÊ§°Ü
        public const int NET_DVR_CONVERT_SDK_ERROR = 85;//¼ÓÔØ×ªÂë¿âÊ§°Ü
        public const int NET_DVR_IPC_COUNT_OVERFLOW = 86;//³¬³ö×î´óµÄip½ÓÈëÍ¨µÀÊý

        public const int NET_PLAYM4_NOERROR = 500;//no error
        public const int NET_PLAYM4_PARA_OVER = 501;//input parameter is invalid
        public const int NET_PLAYM4_ORDER_ERROR = 502;//The order of the function to be called is error
        public const int NET_PLAYM4_TIMER_ERROR = 503;//Create multimedia clock failed
        public const int NET_PLAYM4_DEC_VIDEO_ERROR = 504;//Decode video data failed
        public const int NET_PLAYM4_DEC_AUDIO_ERROR = 505;//Decode audio data failed
        public const int NET_PLAYM4_ALLOC_MEMORY_ERROR = 506;//Allocate memory failed
        public const int NET_PLAYM4_OPEN_FILE_ERROR = 507;//Open the file failed
        public const int NET_PLAYM4_CREATE_OBJ_ERROR = 508;//Create thread or event failed
        public const int NET_PLAYM4_CREATE_DDRAW_ERROR = 509;//Create DirectDraw object failed
        public const int NET_PLAYM4_CREATE_OFFSCREEN_ERROR = 510;//failed when creating off-screen surface
        public const int NET_PLAYM4_BUF_OVER = 511;//buffer is overflow
        public const int NET_PLAYM4_CREATE_SOUND_ERROR = 512;//failed when creating audio device
        public const int NET_PLAYM4_SET_VOLUME_ERROR = 513;//Set volume failed
        public const int NET_PLAYM4_SUPPORT_FILE_ONLY = 514;//The function only support play file
        public const int NET_PLAYM4_SUPPORT_STREAM_ONLY = 515;//The function only support play stream
        public const int NET_PLAYM4_SYS_NOT_SUPPORT = 516;//System not support
        public const int NET_PLAYM4_FILEHEADER_UNKNOWN = 517;//No file header
        public const int NET_PLAYM4_VERSION_INCORRECT = 518;//The version of decoder and encoder is not adapted
        public const int NET_PALYM4_INIT_DECODER_ERROR = 519;//Initialize decoder failed
        public const int NET_PLAYM4_CHECK_FILE_ERROR = 520;//The file data is unknown
        public const int NET_PLAYM4_INIT_TIMER_ERROR = 521;//Initialize multimedia clock failed
        public const int NET_PLAYM4_BLT_ERROR = 522;//Blt failed
        public const int NET_PLAYM4_UPDATE_ERROR = 523;//Update failed
        public const int NET_PLAYM4_OPEN_FILE_ERROR_MULTI = 524;//openfile error, streamtype is multi
        public const int NET_PLAYM4_OPEN_FILE_ERROR_VIDEO = 525;//openfile error, streamtype is video
        public const int NET_PLAYM4_JPEG_COMPRESS_ERROR = 526;//JPEG compress error
        public const int NET_PLAYM4_EXTRACT_NOT_SUPPORT = 527;//Don't support the version of this file
        public const int NET_PLAYM4_EXTRACT_DATA_ERROR = 528;//extract video data failed
        /*******************È«¾Ö´íÎóÂë end**********************/

        /*************************************************
        NET_DVR_IsSupport()·µ»ØÖµ
        1£­9Î»·Ö±ð±íÊ¾ÒÔÏÂÐÅÏ¢£¨Î»ÓëÊÇTRUE)±íÊ¾Ö§³Ö£»
        **************************************************/
        public const int NET_DVR_SUPPORT_DDRAW = 1;//Ö§³ÖDIRECTDRAW£¬Èç¹û²»Ö§³Ö£¬Ôò²¥·ÅÆ÷²»ÄÜ¹¤×÷
        public const int NET_DVR_SUPPORT_BLT = 2;//ÏÔ¿¨Ö§³ÖBLT²Ù×÷£¬Èç¹û²»Ö§³Ö£¬Ôò²¥·ÅÆ÷²»ÄÜ¹¤×÷
        public const int NET_DVR_SUPPORT_BLTFOURCC = 4;//ÏÔ¿¨BLTÖ§³ÖÑÕÉ«×ª»»£¬Èç¹û²»Ö§³Ö£¬²¥·ÅÆ÷»áÓÃÈí¼þ·½·¨×÷RGB×ª»»
        public const int NET_DVR_SUPPORT_BLTSHRINKX = 8;//ÏÔ¿¨BLTÖ§³ÖXÖáËõÐ¡£»Èç¹û²»Ö§³Ö£¬ÏµÍ³»áÓÃÈí¼þ·½·¨×ª»»
        public const int NET_DVR_SUPPORT_BLTSHRINKY = 16;//ÏÔ¿¨BLTÖ§³ÖYÖáËõÐ¡£»Èç¹û²»Ö§³Ö£¬ÏµÍ³»áÓÃÈí¼þ·½·¨×ª»»
        public const int NET_DVR_SUPPORT_BLTSTRETCHX = 32;//ÏÔ¿¨BLTÖ§³ÖXÖá·Å´ó£»Èç¹û²»Ö§³Ö£¬ÏµÍ³»áÓÃÈí¼þ·½·¨×ª»»
        public const int NET_DVR_SUPPORT_BLTSTRETCHY = 64;//ÏÔ¿¨BLTÖ§³ÖYÖá·Å´ó£»Èç¹û²»Ö§³Ö£¬ÏµÍ³»áÓÃÈí¼þ·½·¨×ª»»
        public const int NET_DVR_SUPPORT_SSE = 128;//CPUÖ§³ÖSSEÖ¸Áî£¬Intel Pentium3ÒÔÉÏÖ§³ÖSSEÖ¸Áî
        public const int NET_DVR_SUPPORT_MMX = 256;//CPUÖ§³ÖMMXÖ¸Áî¼¯£¬Intel Pentium3ÒÔÉÏÖ§³ÖSSEÖ¸Áî

        /**********************ÔÆÌ¨¿ØÖÆÃüÁî begin*************************/
        public const int LIGHT_PWRON = 2;// ½ÓÍ¨µÆ¹âµçÔ´
        public const int WIPER_PWRON = 3;// ½ÓÍ¨ÓêË¢¿ª¹Ø 
        public const int FAN_PWRON = 4;// ½ÓÍ¨·çÉÈ¿ª¹Ø
        public const int HEATER_PWRON = 5;// ½ÓÍ¨¼ÓÈÈÆ÷¿ª¹Ø
        public const int AUX_PWRON1 = 6;// ½ÓÍ¨¸¨ÖúÉè±¸¿ª¹Ø
        public const int AUX_PWRON2 = 7;// ½ÓÍ¨¸¨ÖúÉè±¸¿ª¹Ø 
        public const int SET_PRESET = 8;// ÉèÖÃÔ¤ÖÃµã 
        public const int CLE_PRESET = 9;// Çå³ýÔ¤ÖÃµã 

        public const int ZOOM_IN = 11;// ½¹¾àÒÔËÙ¶ÈSS±ä´ó(±¶ÂÊ±ä´ó)
        public const int ZOOM_OUT = 12;// ½¹¾àÒÔËÙ¶ÈSS±äÐ¡(±¶ÂÊ±äÐ¡)
        public const int FOCUS_NEAR = 13;// ½¹µãÒÔËÙ¶ÈSSÇ°µ÷ 
        public const int FOCUS_FAR = 14;// ½¹µãÒÔËÙ¶ÈSSºóµ÷
        public const int IRIS_OPEN = 15;// ¹âÈ¦ÒÔËÙ¶ÈSSÀ©´ó
        public const int IRIS_CLOSE = 16;// ¹âÈ¦ÒÔËÙ¶ÈSSËõÐ¡ 

        public const int TILT_UP = 21;/* ÔÆÌ¨ÒÔSSµÄËÙ¶ÈÉÏÑö */
        public const int TILT_DOWN = 22;/* ÔÆÌ¨ÒÔSSµÄËÙ¶ÈÏÂ¸© */
        public const int PAN_LEFT = 23;/* ÔÆÌ¨ÒÔSSµÄËÙ¶È×ó×ª */
        public const int PAN_RIGHT = 24;/* ÔÆÌ¨ÒÔSSµÄËÙ¶ÈÓÒ×ª */
        public const int UP_LEFT = 25;/* ÔÆÌ¨ÒÔSSµÄËÙ¶ÈÉÏÑöºÍ×ó×ª */
        public const int UP_RIGHT = 26;/* ÔÆÌ¨ÒÔSSµÄËÙ¶ÈÉÏÑöºÍÓÒ×ª */
        public const int DOWN_LEFT = 27;/* ÔÆÌ¨ÒÔSSµÄËÙ¶ÈÏÂ¸©ºÍ×ó×ª */
        public const int DOWN_RIGHT = 28;/* ÔÆÌ¨ÒÔSSµÄËÙ¶ÈÏÂ¸©ºÍÓÒ×ª */
        public const int PAN_AUTO = 29;/* ÔÆÌ¨ÒÔSSµÄËÙ¶È×óÓÒ×Ô¶¯É¨Ãè */

        public const int FILL_PRE_SEQ = 30;/* ½«Ô¤ÖÃµã¼ÓÈëÑ²º½ÐòÁÐ */
        public const int SET_SEQ_DWELL = 31;/* ÉèÖÃÑ²º½µãÍ£¶ÙÊ±¼ä */
        public const int SET_SEQ_SPEED = 32;/* ÉèÖÃÑ²º½ËÙ¶È */
        public const int CLE_PRE_SEQ = 33;/* ½«Ô¤ÖÃµã´ÓÑ²º½ÐòÁÐÖÐÉ¾³ý */
        public const int STA_MEM_CRUISE = 34;/* ¿ªÊ¼¼ÇÂ¼¹ì¼£ */
        public const int STO_MEM_CRUISE = 35;/* Í£Ö¹¼ÇÂ¼¹ì¼£ */
        public const int RUN_CRUISE = 36;/* ¿ªÊ¼¹ì¼£ */
        public const int RUN_SEQ = 37;/* ¿ªÊ¼Ñ²º½ */
        public const int STOP_SEQ = 38;/* Í£Ö¹Ñ²º½ */
        public const int GOTO_PRESET = 39;/* ¿ìÇò×ªµ½Ô¤ÖÃµã */
        public const int DEL_SEQ = 43; /* É¾³ýÑ²º½Â·¾¶ */
        /**********************ÔÆÌ¨¿ØÖÆÃüÁî end*************************/
        /*************************************************
        »Ø·ÅÊ±²¥·Å¿ØÖÆÃüÁîºê¶¨Òå 
        NET_DVR_PlayBackControl
        NET_DVR_PlayControlLocDisplay
        NET_DVR_DecPlayBackCtrlµÄºê¶¨Òå
        ¾ßÌåÖ§³Ö²é¿´º¯ÊýËµÃ÷ºÍ´úÂë
        **************************************************/
        public const int NET_DVR_PLAYSTART = 1;//¿ªÊ¼²¥·Å
        public const int NET_DVR_PLAYSTOP = 2;//Í£Ö¹²¥·Å
        public const int NET_DVR_PLAYPAUSE = 3;//ÔÝÍ£²¥·Å
        public const int NET_DVR_PLAYRESTART = 4;//»Ö¸´²¥·Å
        public const int NET_DVR_PLAYFAST = 5;//¿ì·Å
        public const int NET_DVR_PLAYSLOW = 6;//Âý·Å
        public const int NET_DVR_PLAYNORMAL = 7;//Õý³£ËÙ¶È
        public const int NET_DVR_PLAYFRAME = 8;//µ¥Ö¡·Å
        public const int NET_DVR_PLAYSTARTAUDIO = 9;//´ò¿ªÉùÒô
        public const int NET_DVR_PLAYSTOPAUDIO = 10;//¹Ø±ÕÉùÒô
        public const int NET_DVR_PLAYAUDIOVOLUME = 11;//µ÷½ÚÒôÁ¿
        public const int NET_DVR_PLAYSETPOS = 12;//¸Ä±äÎÄ¼þ»Ø·ÅµÄ½ø¶È
        public const int NET_DVR_PLAYGETPOS = 13;//»ñÈ¡ÎÄ¼þ»Ø·ÅµÄ½ø¶È
        public const int NET_DVR_PLAYGETTIME = 14;//»ñÈ¡µ±Ç°ÒÑ¾­²¥·ÅµÄÊ±¼ä(°´ÎÄ¼þ»Ø·ÅµÄÊ±ºòÓÐÐ§)
        public const int NET_DVR_PLAYGETFRAME = 15;//»ñÈ¡µ±Ç°ÒÑ¾­²¥·ÅµÄÖ¡Êý(°´ÎÄ¼þ»Ø·ÅµÄÊ±ºòÓÐÐ§)
        public const int NET_DVR_GETTOTALFRAMES = 16;//»ñÈ¡µ±Ç°²¥·ÅÎÄ¼þ×ÜµÄÖ¡Êý(°´ÎÄ¼þ»Ø·ÅµÄÊ±ºòÓÐÐ§)
        public const int NET_DVR_GETTOTALTIME = 17;//»ñÈ¡µ±Ç°²¥·ÅÎÄ¼þ×ÜµÄÊ±¼ä(°´ÎÄ¼þ»Ø·ÅµÄÊ±ºòÓÐÐ§)
        public const int NET_DVR_THROWBFRAME = 20;//¶ªBÖ¡
        public const int NET_DVR_SETSPEED = 24;//ÉèÖÃÂëÁ÷ËÙ¶È
        public const int NET_DVR_KEEPALIVE = 25;//±£³ÖÓëÉè±¸µÄÐÄÌø(Èç¹û»Øµ÷×èÈû£¬½¨Òé2Ãë·¢ËÍÒ»´Î)
        public const int NET_DVR_PLAYSETTIME = 26;//°´¾ø¶ÔÊ±¼ä¶¨Î»
        public const int NET_DVR_PLAYGETTOTALLEN = 27;//»ñÈ¡°´Ê±¼ä»Ø·Å¶ÔÓ¦Ê±¼ä¶ÎÄÚµÄËùÓÐÎÄ¼þµÄ×Ü³¤¶È
        public const int NET_DVR_PLAY_FORWARD = 29; //µ¹·ÅÇÐ»»ÎªÕý·Å
        public const int NET_DVR_PLAY_REVERSE = 30; //Õý·ÅÇÐ»»Îªµ¹·Å
        public const int NET_DVR_SET_DECODEFFRAMETYPE = 31;
        public const int NET_DVR_SET_TRANS_TYPE = 32; //ÉèÖÃ×ªÂë¸ñÊ½
        public const int NET_DVR_PLAY_CONVERT = 33; //»Ø·Å×ªÂë
        public const int NET_DVR_START_DRAWFRAME = 34; //¿ªÊ¼³éÖ¡»Ø·Å
        public const int NET_DVR_STOP_DRAWFRAME = 35; //Í£Ö¹³éÖ¡»Ø·Å

        //Ô¶³Ì°´¼ü¶¨ÒåÈçÏÂ£º
        /* key value send to CONFIG program */
        public const int KEY_CODE_1 = 1;
        public const int KEY_CODE_2 = 2;
        public const int KEY_CODE_3 = 3;
        public const int KEY_CODE_4 = 4;
        public const int KEY_CODE_5 = 5;
        public const int KEY_CODE_6 = 6;
        public const int KEY_CODE_7 = 7;
        public const int KEY_CODE_8 = 8;
        public const int KEY_CODE_9 = 9;
        public const int KEY_CODE_0 = 10;
        public const int KEY_CODE_POWER = 11;
        public const int KEY_CODE_MENU = 12;
        public const int KEY_CODE_ENTER = 13;
        public const int KEY_CODE_CANCEL = 14;
        public const int KEY_CODE_UP = 15;
        public const int KEY_CODE_DOWN = 16;
        public const int KEY_CODE_LEFT = 17;
        public const int KEY_CODE_RIGHT = 18;
        public const int KEY_CODE_EDIT = 19;
        public const int KEY_CODE_ADD = 20;
        public const int KEY_CODE_MINUS = 21;
        public const int KEY_CODE_PLAY = 22;
        public const int KEY_CODE_REC = 23;
        public const int KEY_CODE_PAN = 24;
        public const int KEY_CODE_M = 25;
        public const int KEY_CODE_A = 26;
        public const int KEY_CODE_F1 = 27;
        public const int KEY_CODE_F2 = 28;

        /* for PTZ control */
        public const int KEY_PTZ_UP_START = KEY_CODE_UP;
        public const int KEY_PTZ_UP_STOP = 32;

        public const int KEY_PTZ_DOWN_START = KEY_CODE_DOWN;
        public const int KEY_PTZ_DOWN_STOP = 33;


        public const int KEY_PTZ_LEFT_START = KEY_CODE_LEFT;
        public const int KEY_PTZ_LEFT_STOP = 34;

        public const int KEY_PTZ_RIGHT_START = KEY_CODE_RIGHT;
        public const int KEY_PTZ_RIGHT_STOP = 35;

        public const int KEY_PTZ_AP1_START = KEY_CODE_EDIT;/* ¹âÈ¦+ */
        public const int KEY_PTZ_AP1_STOP = 36;

        public const int KEY_PTZ_AP2_START = KEY_CODE_PAN;/* ¹âÈ¦- */
        public const int KEY_PTZ_AP2_STOP = 37;

        public const int KEY_PTZ_FOCUS1_START = KEY_CODE_A;/* ¾Û½¹+ */
        public const int KEY_PTZ_FOCUS1_STOP = 38;

        public const int KEY_PTZ_FOCUS2_START = KEY_CODE_M;/* ¾Û½¹- */
        public const int KEY_PTZ_FOCUS2_STOP = 39;

        public const int KEY_PTZ_B1_START = 40;/* ±ä±¶+ */
        public const int KEY_PTZ_B1_STOP = 41;

        public const int KEY_PTZ_B2_START = 42;/* ±ä±¶- */
        public const int KEY_PTZ_B2_STOP = 43;

        //9000ÐÂÔö
        public const int KEY_CODE_11 = 44;
        public const int KEY_CODE_12 = 45;
        public const int KEY_CODE_13 = 46;
        public const int KEY_CODE_14 = 47;
        public const int KEY_CODE_15 = 48;
        public const int KEY_CODE_16 = 49;

        /*************************²ÎÊýÅäÖÃÃüÁî begin*******************************/
        //ÓÃÓÚNET_DVR_SetDVRConfigºÍNET_DVR_GetDVRConfig,×¢ÒâÆä¶ÔÓ¦µÄÅäÖÃ½á¹¹
        public const int NET_DVR_GET_DEVICECFG = 100;//»ñÈ¡Éè±¸²ÎÊý
        public const int NET_DVR_SET_DEVICECFG = 101;//ÉèÖÃÉè±¸²ÎÊý
        public const int NET_DVR_GET_NETCFG = 102;//»ñÈ¡ÍøÂç²ÎÊý
        public const int NET_DVR_SET_NETCFG = 103;//ÉèÖÃÍøÂç²ÎÊý
        public const int NET_DVR_GET_PICCFG = 104;//»ñÈ¡Í¼Ïó²ÎÊý
        public const int NET_DVR_SET_PICCFG = 105;//ÉèÖÃÍ¼Ïó²ÎÊý
        public const int NET_DVR_GET_COMPRESSCFG = 106;//»ñÈ¡Ñ¹Ëõ²ÎÊý
        public const int NET_DVR_SET_COMPRESSCFG = 107;//ÉèÖÃÑ¹Ëõ²ÎÊý
        public const int NET_DVR_GET_RECORDCFG = 108;//»ñÈ¡Â¼ÏñÊ±¼ä²ÎÊý
        public const int NET_DVR_SET_RECORDCFG = 109;//ÉèÖÃÂ¼ÏñÊ±¼ä²ÎÊý
        public const int NET_DVR_GET_DECODERCFG = 110;//»ñÈ¡½âÂëÆ÷²ÎÊý
        public const int NET_DVR_SET_DECODERCFG = 111;//ÉèÖÃ½âÂëÆ÷²ÎÊý
        public const int NET_DVR_GET_RS232CFG = 112;//»ñÈ¡232´®¿Ú²ÎÊý
        public const int NET_DVR_SET_RS232CFG = 113;//ÉèÖÃ232´®¿Ú²ÎÊý
        public const int NET_DVR_GET_ALARMINCFG = 114;//»ñÈ¡±¨¾¯ÊäÈë²ÎÊý
        public const int NET_DVR_SET_ALARMINCFG = 115;//ÉèÖÃ±¨¾¯ÊäÈë²ÎÊý
        public const int NET_DVR_GET_ALARMOUTCFG = 116;//»ñÈ¡±¨¾¯Êä³ö²ÎÊý
        public const int NET_DVR_SET_ALARMOUTCFG = 117;//ÉèÖÃ±¨¾¯Êä³ö²ÎÊý
        public const int NET_DVR_GET_TIMECFG = 118;//»ñÈ¡DVRÊ±¼ä
        public const int NET_DVR_SET_TIMECFG = 119;//ÉèÖÃDVRÊ±¼ä
        public const int NET_DVR_GET_PREVIEWCFG = 120;//»ñÈ¡Ô¤ÀÀ²ÎÊý
        public const int NET_DVR_SET_PREVIEWCFG = 121;//ÉèÖÃÔ¤ÀÀ²ÎÊý
        public const int NET_DVR_GET_VIDEOOUTCFG = 122;//»ñÈ¡ÊÓÆµÊä³ö²ÎÊý
        public const int NET_DVR_SET_VIDEOOUTCFG = 123;//ÉèÖÃÊÓÆµÊä³ö²ÎÊý
        public const int NET_DVR_GET_USERCFG = 124;//»ñÈ¡ÓÃ»§²ÎÊý
        public const int NET_DVR_SET_USERCFG = 125;//ÉèÖÃÓÃ»§²ÎÊý
        public const int NET_DVR_GET_EXCEPTIONCFG = 126;//»ñÈ¡Òì³£²ÎÊý
        public const int NET_DVR_SET_EXCEPTIONCFG = 127;//ÉèÖÃÒì³£²ÎÊý
        public const int NET_DVR_GET_ZONEANDDST = 128;//»ñÈ¡Ê±ÇøºÍÏÄÊ±ÖÆ²ÎÊý
        public const int NET_DVR_SET_ZONEANDDST = 129;//ÉèÖÃÊ±ÇøºÍÏÄÊ±ÖÆ²ÎÊý
        public const int NET_DVR_GET_SHOWSTRING = 130;//»ñÈ¡µþ¼Ó×Ö·û²ÎÊý
        public const int NET_DVR_SET_SHOWSTRING = 131;//ÉèÖÃµþ¼Ó×Ö·û²ÎÊý
        public const int NET_DVR_GET_EVENTCOMPCFG = 132;//»ñÈ¡ÊÂ¼þ´¥·¢Â¼Ïñ²ÎÊý
        public const int NET_DVR_SET_EVENTCOMPCFG = 133;//ÉèÖÃÊÂ¼þ´¥·¢Â¼Ïñ²ÎÊý

        public const int NET_DVR_GET_AUXOUTCFG = 140;//»ñÈ¡±¨¾¯´¥·¢¸¨ÖúÊä³öÉèÖÃ(HSÉè±¸¸¨ÖúÊä³ö2006-02-28)
        public const int NET_DVR_SET_AUXOUTCFG = 141;//ÉèÖÃ±¨¾¯´¥·¢¸¨ÖúÊä³öÉèÖÃ(HSÉè±¸¸¨ÖúÊä³ö2006-02-28)
        public const int NET_DVR_GET_PREVIEWCFG_AUX = 142;//»ñÈ¡-sÏµÁÐË«Êä³öÔ¤ÀÀ²ÎÊý(-sÏµÁÐË«Êä³ö2006-04-13)
        public const int NET_DVR_SET_PREVIEWCFG_AUX = 143;//ÉèÖÃ-sÏµÁÐË«Êä³öÔ¤ÀÀ²ÎÊý(-sÏµÁÐË«Êä³ö2006-04-13)

        public const int NET_DVR_GET_PICCFG_EX = 200;//»ñÈ¡Í¼Ïó²ÎÊý(SDK_V14À©Õ¹ÃüÁî)
        public const int NET_DVR_SET_PICCFG_EX = 201;//ÉèÖÃÍ¼Ïó²ÎÊý(SDK_V14À©Õ¹ÃüÁî)
        public const int NET_DVR_GET_USERCFG_EX = 202;//»ñÈ¡ÓÃ»§²ÎÊý(SDK_V15À©Õ¹ÃüÁî)
        public const int NET_DVR_SET_USERCFG_EX = 203;//ÉèÖÃÓÃ»§²ÎÊý(SDK_V15À©Õ¹ÃüÁî)
        public const int NET_DVR_GET_COMPRESSCFG_EX = 204;//»ñÈ¡Ñ¹Ëõ²ÎÊý(SDK_V15À©Õ¹ÃüÁî2006-05-15)
        public const int NET_DVR_SET_COMPRESSCFG_EX = 205;//ÉèÖÃÑ¹Ëõ²ÎÊý(SDK_V15À©Õ¹ÃüÁî2006-05-15)

        public const int NET_DVR_GET_NETAPPCFG = 222;//»ñÈ¡ÍøÂçÓ¦ÓÃ²ÎÊý NTP/DDNS/EMAIL
        public const int NET_DVR_SET_NETAPPCFG = 223;//ÉèÖÃÍøÂçÓ¦ÓÃ²ÎÊý NTP/DDNS/EMAIL
        public const int NET_DVR_GET_NTPCFG = 224;//»ñÈ¡ÍøÂçÓ¦ÓÃ²ÎÊý NTP
        public const int NET_DVR_SET_NTPCFG = 225;//ÉèÖÃÍøÂçÓ¦ÓÃ²ÎÊý NTP
        public const int NET_DVR_GET_DDNSCFG = 226;//»ñÈ¡ÍøÂçÓ¦ÓÃ²ÎÊý DDNS
        public const int NET_DVR_SET_DDNSCFG = 227;//ÉèÖÃÍøÂçÓ¦ÓÃ²ÎÊý DDNS
                                                   //¶ÔÓ¦NET_DVR_EMAILPARA
        public const int NET_DVR_GET_EMAILCFG = 228;//»ñÈ¡ÍøÂçÓ¦ÓÃ²ÎÊý EMAIL
        public const int NET_DVR_SET_EMAILCFG = 229;//ÉèÖÃÍøÂçÓ¦ÓÃ²ÎÊý EMAIL

        public const int NET_DVR_GET_NFSCFG = 230;/* NFS disk config */
        public const int NET_DVR_SET_NFSCFG = 231;/* NFS disk config */

        public const int NET_DVR_GET_SHOWSTRING_EX = 238;//»ñÈ¡µþ¼Ó×Ö·û²ÎÊýÀ©Õ¹(Ö§³Ö8Ìõ×Ö·û)
        public const int NET_DVR_SET_SHOWSTRING_EX = 239;//ÉèÖÃµþ¼Ó×Ö·û²ÎÊýÀ©Õ¹(Ö§³Ö8Ìõ×Ö·û)
        public const int NET_DVR_GET_NETCFG_OTHER = 244;//»ñÈ¡ÍøÂç²ÎÊý
        public const int NET_DVR_SET_NETCFG_OTHER = 245;//ÉèÖÃÍøÂç²ÎÊý

        //¶ÔÓ¦NET_DVR_EMAILCFG½á¹¹
        public const int NET_DVR_GET_EMAILPARACFG = 250;//Get EMAIL parameters
        public const int NET_DVR_SET_EMAILPARACFG = 251;//Setup EMAIL parameters

        public const int NET_DVR_GET_DDNSCFG_EX = 274;//»ñÈ¡À©Õ¹DDNS²ÎÊý
        public const int NET_DVR_SET_DDNSCFG_EX = 275;//ÉèÖÃÀ©Õ¹DDNS²ÎÊý

        public const int NET_DVR_SET_PTZPOS = 292;//ÔÆÌ¨ÉèÖÃPTZÎ»ÖÃ
        public const int NET_DVR_GET_PTZPOS = 293;//ÔÆÌ¨»ñÈ¡PTZÎ»ÖÃ
        public const int NET_DVR_GET_PTZSCOPE = 294;//ÔÆÌ¨»ñÈ¡PTZ·¶Î§

        /***************************DS9000ÐÂÔöÃüÁî(_V30) begin *****************************/
        //ÍøÂç(NET_DVR_NETCFG_V30½á¹¹)
        public const int NET_DVR_GET_NETCFG_V30 = 1000;//»ñÈ¡ÍøÂç²ÎÊý
        public const int NET_DVR_SET_NETCFG_V30 = 1001;//ÉèÖÃÍøÂç²ÎÊý

        //Í¼Ïó(NET_DVR_PICCFG_V30½á¹¹)
        public const int NET_DVR_GET_PICCFG_V30 = 1002;//»ñÈ¡Í¼Ïó²ÎÊý
        public const int NET_DVR_SET_PICCFG_V30 = 1003;//ÉèÖÃÍ¼Ïó²ÎÊý

        //Â¼ÏñÊ±¼ä(NET_DVR_RECORD_V30½á¹¹)
        public const int NET_DVR_GET_RECORDCFG_V30 = 1004;//»ñÈ¡Â¼Ïñ²ÎÊý
        public const int NET_DVR_SET_RECORDCFG_V30 = 1005;//ÉèÖÃÂ¼Ïñ²ÎÊý

        //ÓÃ»§(NET_DVR_USER_V30½á¹¹)
        public const int NET_DVR_GET_USERCFG_V30 = 1006;//»ñÈ¡ÓÃ»§²ÎÊý
        public const int NET_DVR_SET_USERCFG_V30 = 1007;//ÉèÖÃÓÃ»§²ÎÊý

        //9000DDNS²ÎÊýÅäÖÃ(NET_DVR_DDNSPARA_V30½á¹¹)
        public const int NET_DVR_GET_DDNSCFG_V30 = 1010;//»ñÈ¡DDNS(9000À©Õ¹)
        public const int NET_DVR_SET_DDNSCFG_V30 = 1011;//ÉèÖÃDDNS(9000À©Õ¹)

        //EMAIL¹¦ÄÜ(NET_DVR_EMAILCFG_V30½á¹¹)
        public const int NET_DVR_GET_EMAILCFG_V30 = 1012;//»ñÈ¡EMAIL²ÎÊý 
        public const int NET_DVR_SET_EMAILCFG_V30 = 1013;//ÉèÖÃEMAIL²ÎÊý 

        //Ñ²º½²ÎÊý (NET_DVR_CRUISE_PARA½á¹¹)
        public const int NET_DVR_GET_CRUISE = 1020;
        public const int NET_DVR_SET_CRUISE = 1021;

        //±¨¾¯ÊäÈë½á¹¹²ÎÊý (NET_DVR_ALARMINCFG_V30½á¹¹)
        public const int NET_DVR_GET_ALARMINCFG_V30 = 1024;
        public const int NET_DVR_SET_ALARMINCFG_V30 = 1025;

        //±¨¾¯Êä³ö½á¹¹²ÎÊý (NET_DVR_ALARMOUTCFG_V30½á¹¹)
        public const int NET_DVR_GET_ALARMOUTCFG_V30 = 1026;
        public const int NET_DVR_SET_ALARMOUTCFG_V30 = 1027;

        //ÊÓÆµÊä³ö½á¹¹²ÎÊý (NET_DVR_VIDEOOUT_V30½á¹¹)
        public const int NET_DVR_GET_VIDEOOUTCFG_V30 = 1028;
        public const int NET_DVR_SET_VIDEOOUTCFG_V30 = 1029;

        //µþ¼Ó×Ö·û½á¹¹²ÎÊý (NET_DVR_SHOWSTRING_V30½á¹¹)
        public const int NET_DVR_GET_SHOWSTRING_V30 = 1030;
        public const int NET_DVR_SET_SHOWSTRING_V30 = 1031;

        //Òì³£½á¹¹²ÎÊý (NET_DVR_EXCEPTION_V30½á¹¹)
        public const int NET_DVR_GET_EXCEPTIONCFG_V30 = 1034;
        public const int NET_DVR_SET_EXCEPTIONCFG_V30 = 1035;

        //´®¿Ú232½á¹¹²ÎÊý (NET_DVR_RS232CFG_V30½á¹¹)
        public const int NET_DVR_GET_RS232CFG_V30 = 1036;
        public const int NET_DVR_SET_RS232CFG_V30 = 1037;

        //ÍøÂçÓ²ÅÌ½ÓÈë½á¹¹²ÎÊý (NET_DVR_NET_DISKCFG½á¹¹)
        public const int NET_DVR_GET_NET_DISKCFG = 1038;//ÍøÂçÓ²ÅÌ½ÓÈë»ñÈ¡
        public const int NET_DVR_SET_NET_DISKCFG = 1039;//ÍøÂçÓ²ÅÌ½ÓÈëÉèÖÃ

        //Ñ¹Ëõ²ÎÊý (NET_DVR_COMPRESSIONCFG_V30½á¹¹)
        public const int NET_DVR_GET_COMPRESSCFG_V30 = 1040;
        public const int NET_DVR_SET_COMPRESSCFG_V30 = 1041;

        //»ñÈ¡485½âÂëÆ÷²ÎÊý (NET_DVR_DECODERCFG_V30½á¹¹)
        public const int NET_DVR_GET_DECODERCFG_V30 = 1042;//»ñÈ¡½âÂëÆ÷²ÎÊý
        public const int NET_DVR_SET_DECODERCFG_V30 = 1043;//ÉèÖÃ½âÂëÆ÷²ÎÊý

        //»ñÈ¡Ô¤ÀÀ²ÎÊý (NET_DVR_PREVIEWCFG_V30½á¹¹)
        public const int NET_DVR_GET_PREVIEWCFG_V30 = 1044;//»ñÈ¡Ô¤ÀÀ²ÎÊý
        public const int NET_DVR_SET_PREVIEWCFG_V30 = 1045;//ÉèÖÃÔ¤ÀÀ²ÎÊý

        //¸¨ÖúÔ¤ÀÀ²ÎÊý (NET_DVR_PREVIEWCFG_AUX_V30½á¹¹)
        public const int NET_DVR_GET_PREVIEWCFG_AUX_V30 = 1046;//»ñÈ¡¸¨ÖúÔ¤ÀÀ²ÎÊý
        public const int NET_DVR_SET_PREVIEWCFG_AUX_V30 = 1047;//ÉèÖÃ¸¨ÖúÔ¤ÀÀ²ÎÊý

        //IP½ÓÈëÅäÖÃ²ÎÊý £¨NET_DVR_IPPARACFG½á¹¹£©
        public const int NET_DVR_GET_IPPARACFG = 1048; //»ñÈ¡IP½ÓÈëÅäÖÃÐÅÏ¢ 
        public const int NET_DVR_SET_IPPARACFG = 1049;//ÉèÖÃIP½ÓÈëÅäÖÃÐÅÏ¢

        //IP±¨¾¯ÊäÈë½ÓÈëÅäÖÃ²ÎÊý £¨NET_DVR_IPALARMINCFG½á¹¹£©
        public const int NET_DVR_GET_IPALARMINCFG = 1050; //»ñÈ¡IP±¨¾¯ÊäÈë½ÓÈëÅäÖÃÐÅÏ¢ 
        public const int NET_DVR_SET_IPALARMINCFG = 1051; //ÉèÖÃIP±¨¾¯ÊäÈë½ÓÈëÅäÖÃÐÅÏ¢

        //IP±¨¾¯Êä³ö½ÓÈëÅäÖÃ²ÎÊý £¨NET_DVR_IPALARMOUTCFG½á¹¹£©
        public const int NET_DVR_GET_IPALARMOUTCFG = 1052;//»ñÈ¡IP±¨¾¯Êä³ö½ÓÈëÅäÖÃÐÅÏ¢ 
        public const int NET_DVR_SET_IPALARMOUTCFG = 1053;//ÉèÖÃIP±¨¾¯Êä³ö½ÓÈëÅäÖÃÐÅÏ¢

        //Ó²ÅÌ¹ÜÀíµÄ²ÎÊý»ñÈ¡ (NET_DVR_HDCFG½á¹¹)
        public const int NET_DVR_GET_HDCFG = 1054;//»ñÈ¡Ó²ÅÌ¹ÜÀíÅäÖÃ²ÎÊý
        public const int NET_DVR_SET_HDCFG = 1055;//ÉèÖÃÓ²ÅÌ¹ÜÀíÅäÖÃ²ÎÊý

        //ÅÌ×é¹ÜÀíµÄ²ÎÊý»ñÈ¡ (NET_DVR_HDGROUP_CFG½á¹¹)
        public const int NET_DVR_GET_HDGROUP_CFG = 1056;//»ñÈ¡ÅÌ×é¹ÜÀíÅäÖÃ²ÎÊý
        public const int NET_DVR_SET_HDGROUP_CFG = 1057;//ÉèÖÃÅÌ×é¹ÜÀíÅäÖÃ²ÎÊý

        //Éè±¸±àÂëÀàÐÍÅäÖÃ(NET_DVR_COMPRESSION_AUDIO½á¹¹)
        public const int NET_DVR_GET_COMPRESSCFG_AUD = 1058;//»ñÈ¡Éè±¸ÓïÒô¶Ô½²±àÂë²ÎÊý
        public const int NET_DVR_SET_COMPRESSCFG_AUD = 1059;//ÉèÖÃÉè±¸ÓïÒô¶Ô½²±àÂë²ÎÊý

        //IP½ÓÈëÅäÖÃ²ÎÊý £¨NET_DVR_IPPARACFG_V31½á¹¹£©
        public const int NET_DVR_GET_IPPARACFG_V31 = 1060;//»ñÈ¡IP½ÓÈëÅäÖÃÐÅÏ¢ 
        public const int NET_DVR_SET_IPPARACFG_V31 = 1061; //ÉèÖÃIP½ÓÈëÅäÖÃÐÅÏ¢

        //Ô¶³Ì¿ØÖÆÃüÁî
        public const int NET_DVR_BARRIERGATE_CTRL = 3128; //µÀÕ¢¿ØÖÆ
        /***************************DS9000ÐÂÔöÃüÁî(_V30) end *****************************/

        /*************************²ÎÊýÅäÖÃÃüÁî end*******************************/

        /*******************²éÕÒÎÄ¼þºÍÈÕÖ¾º¯Êý·µ»ØÖµ*************************/
        public const int NET_DVR_FILE_SUCCESS = 1000;//»ñµÃÎÄ¼þÐÅÏ¢
        public const int NET_DVR_FILE_NOFIND = 1001;//Ã»ÓÐÎÄ¼þ
        public const int NET_DVR_ISFINDING = 1002;//ÕýÔÚ²éÕÒÎÄ¼þ
        public const int NET_DVR_NOMOREFILE = 1003;//²éÕÒÎÄ¼þÊ±Ã»ÓÐ¸ü¶àµÄÎÄ¼þ
        public const int NET_DVR_FILE_EXCEPTION = 1004;//²éÕÒÎÄ¼þÊ±Òì³£

        /*********************»Øµ÷º¯ÊýÀàÐÍ begin************************/
        public const int COMM_ALARM = 4352;//8000±¨¾¯ÐÅÏ¢Ö÷¶¯ÉÏ´«
        public const int COMM_TRADEINFO = 5376;//ATMDVRÖ÷¶¯ÉÏ´«½»Ò×ÐÅÏ¢
        public const int COMM_ALARM_V30 = 16384;//9000±¨¾¯ÐÅÏ¢Ö÷¶¯ÉÏ´«
        public const int COMM_ALARM_V40 = 0x4007;//ÒÆ¶¯Õì²â£¬ÊÓÆµ¶ªÊ§£¬ÕÚµ²£¬IOÐÅºÅÁ¿µÈ±¨¾¯ÐÅÏ¢Ö÷¶¯ÉÏ´«£¬±¨¾¯Êý¾ÝÎª¿É±ä³¤
        public const int COMM_UPLOAD_PLATE_RESULT = 0x2800;//½»Í¨×¥ÅÄ½á¹ûÉÏ´«
        public const int COMM_UPLOAD_FACESNAP_RESULT = 0x1112;  //ÈËÁ³Ê¶±ð½á¹ûÉÏ´«
        public const int COMM_ITS_PLATE_RESULT = 0x3050;//½»Í¨×¥ÅÄ½á¹ûÉÏ´«
        public const int COMM_IPCCFG = 16385;//9000Éè±¸IPC½ÓÈëÅäÖÃ¸Ä±ä±¨¾¯ÐÅÏ¢Ö÷¶¯ÉÏ´«
        public const int COMM_IPCCFG_V31 = 16386;//9000Éè±¸IPC½ÓÈëÅäÖÃ¸Ä±ä±¨¾¯ÐÅÏ¢Ö÷¶¯ÉÏ´«À©Õ¹ 9000_1.1
        public const int COMM_ALARM_RULE_CALC = 0x1110;  //ÐÐÎªÍ³¼Æ±¨¾¯ÉÏ´«(ÈËÔ±ÃÜ¶È)
        public const int COMM_ALARM_PDC = 0x1103;   // ÈËÊýÍ³¼Æ±¨¾¯ÐÅÏ¢

        /*************²Ù×÷Òì³£ÀàÐÍ(ÏûÏ¢·½Ê½, »Øµ÷·½Ê½(±£Áô))****************/
        public const int EXCEPTION_EXCHANGE = 32768;//ÓÃ»§½»»¥Ê±Òì³£
        public const int EXCEPTION_AUDIOEXCHANGE = 32769;//ÓïÒô¶Ô½²Òì³£
        public const int EXCEPTION_ALARM = 32770;//±¨¾¯Òì³£
        public const int EXCEPTION_PREVIEW = 32771;//ÍøÂçÔ¤ÀÀÒì³£
        public const int EXCEPTION_SERIAL = 32772;//Í¸Ã÷Í¨µÀÒì³£
        public const int EXCEPTION_RECONNECT = 32773;//Ô¤ÀÀÊ±ÖØÁ¬
        public const int EXCEPTION_ALARMRECONNECT = 32774;//±¨¾¯Ê±ÖØÁ¬
        public const int EXCEPTION_SERIALRECONNECT = 32775;//Í¸Ã÷Í¨µÀÖØÁ¬
        public const int EXCEPTION_PLAYBACK = 32784;//»Ø·ÅÒì³£
        public const int EXCEPTION_DISKFMT = 32785;//Ó²ÅÌ¸ñÊ½»¯

        /********************Ô¤ÀÀ»Øµ÷º¯Êý*********************/
        public const int NET_DVR_SYSHEAD = 1;//ÏµÍ³Í·Êý¾Ý
        public const int NET_DVR_STREAMDATA = 2;//ÊÓÆµÁ÷Êý¾Ý£¨°üÀ¨¸´ºÏÁ÷ºÍÒôÊÓÆµ·Ö¿ªµÄÊÓÆµÁ÷Êý¾Ý£©
        public const int NET_DVR_AUDIOSTREAMDATA = 3;//ÒôÆµÁ÷Êý¾Ý
        public const int NET_DVR_STD_VIDEODATA = 4;//±ê×¼ÊÓÆµÁ÷Êý¾Ý
        public const int NET_DVR_STD_AUDIODATA = 5;//±ê×¼ÒôÆµÁ÷Êý¾Ý

        //»Øµ÷Ô¤ÀÀÖÐµÄ×´Ì¬ºÍÏûÏ¢
        public const int NET_DVR_REALPLAYEXCEPTION = 111;//Ô¤ÀÀÒì³£
        public const int NET_DVR_REALPLAYNETCLOSE = 112;//Ô¤ÀÀÊ±Á¬½Ó¶Ï¿ª
        public const int NET_DVR_REALPLAY5SNODATA = 113;//Ô¤ÀÀ5sÃ»ÓÐÊÕµ½Êý¾Ý
        public const int NET_DVR_REALPLAYRECONNECT = 114;//Ô¤ÀÀÖØÁ¬

        /********************»Ø·Å»Øµ÷º¯Êý*********************/
        public const int NET_DVR_PLAYBACKOVER = 101;//»Ø·ÅÊý¾Ý²¥·ÅÍê±Ï
        public const int NET_DVR_PLAYBACKEXCEPTION = 102;//»Ø·ÅÒì³£
        public const int NET_DVR_PLAYBACKNETCLOSE = 103;//»Ø·ÅÊ±ºòÁ¬½Ó¶Ï¿ª
        public const int NET_DVR_PLAYBACK5SNODATA = 104;//»Ø·Å5sÃ»ÓÐÊÕµ½Êý¾Ý

        /*********************»Øµ÷º¯ÊýÀàÐÍ end************************/
        //Éè±¸ÐÍºÅ(DVRÀàÐÍ)
        /* Éè±¸ÀàÐÍ */
        public const int DVR = 1;/*¶ÔÉÐÎ´¶¨ÒåµÄdvrÀàÐÍ·µ»ØNETRET_DVR*/
        public const int ATMDVR = 2;/*atm dvr*/
        public const int DVS = 3;/*DVS*/
        public const int DEC = 4;/* 6001D */
        public const int ENC_DEC = 5;/* 6001F */
        public const int DVR_HC = 6;/*8000HC*/
        public const int DVR_HT = 7;/*8000HT*/
        public const int DVR_HF = 8;/*8000HF*/
        public const int DVR_HS = 9;/* 8000HS DVR(no audio) */
        public const int DVR_HTS = 10; /* 8016HTS DVR(no audio) */
        public const int DVR_HB = 11; /* HB DVR(SATA HD) */
        public const int DVR_HCS = 12; /* 8000HCS DVR */
        public const int DVS_A = 13; /* ´øATAÓ²ÅÌµÄDVS */
        public const int DVR_HC_S = 14; /* 8000HC-S */
        public const int DVR_HT_S = 15;/* 8000HT-S */
        public const int DVR_HF_S = 16;/* 8000HF-S */
        public const int DVR_HS_S = 17; /* 8000HS-S */
        public const int ATMDVR_S = 18;/* ATM-S */
        public const int LOWCOST_DVR = 19;/*7000HÏµÁÐ*/
        public const int DEC_MAT = 20; /*¶àÂ·½âÂëÆ÷*/
        public const int DVR_MOBILE = 21;/* mobile DVR */
        public const int DVR_HD_S = 22;   /* 8000HD-S */
        public const int DVR_HD_SL = 23;/* 8000HD-SL */
        public const int DVR_HC_SL = 24;/* 8000HC-SL */
        public const int DVR_HS_ST = 25;/* 8000HS_ST */
        public const int DVS_HW = 26; /* 6000HW */
        public const int DS630X_D = 27; /* ¶àÂ·½âÂëÆ÷ */
        public const int IPCAM = 30;/*IP ÉãÏñ»ú*/
        public const int MEGA_IPCAM = 31;/*X52MFÏµÁÐ,752MF,852MF*/
        public const int IPCAM_X62MF = 32;/*X62MFÏµÁÐ¿É½ÓÈë9000Éè±¸,762MF,862MF*/
        public const int IPDOME = 40; /*IP ±êÇåÇò»ú*/
        public const int IPDOME_MEGA200 = 41;/*IP 200Íò¸ßÇåÇò»ú*/
        public const int IPDOME_MEGA130 = 42;/*IP 130Íò¸ßÇåÇò»ú*/
        public const int IPMOD = 50;/*IP Ä£¿é*/
        public const int DS71XX_H = 71;/* DS71XXH_S */
        public const int DS72XX_H_S = 72;/* DS72XXH_S */
        public const int DS73XX_H_S = 73;/* DS73XXH_S */
        public const int DS76XX_H_S = 76;/* DS76XX_H_S */
        public const int DS81XX_HS_S = 81;/* DS81XX_HS_S */
        public const int DS81XX_HL_S = 82;/* DS81XX_HL_S */
        public const int DS81XX_HC_S = 83;/* DS81XX_HC_S */
        public const int DS81XX_HD_S = 84;/* DS81XX_HD_S */
        public const int DS81XX_HE_S = 85;/* DS81XX_HE_S */
        public const int DS81XX_HF_S = 86;/* DS81XX_HF_S */
        public const int DS81XX_AH_S = 87;/* DS81XX_AH_S */
        public const int DS81XX_AHF_S = 88;/* DS81XX_AHF_S */
        public const int DS90XX_HF_S = 90;  /*DS90XX_HF_S*/
        public const int DS91XX_HF_S = 91;  /*DS91XX_HF_S*/
        public const int DS91XX_HD_S = 92; /*91XXHD-S(MD)*/
        /**********************Éè±¸ÀàÐÍ end***********************/

        /*************************************************
        ²ÎÊýÅäÖÃ½á¹¹¡¢²ÎÊý(ÆäÖÐ_V30Îª9000ÐÂÔö)
        **************************************************/
        //Ð£Ê±½á¹¹²ÎÊý
        [StructLayoutAttribute(LayoutKind.Sequential)]
        public struct NET_DVR_TIME
        {
            public int dwYear;
            public int dwMonth;
            public int dwDay;
            public int dwHour;
            public int dwMinute;
            public int dwSecond;
        }

        //Ê±¼ä¶Î(×Ó½á¹¹)
        [StructLayoutAttribute(LayoutKind.Sequential)]
        public struct NET_DVR_SCHEDTIME
        {
            public byte byStartHour;//¿ªÊ¼Ê±¼ä
            public byte byStartMin;//¿ªÊ¼Ê±¼ä
            public byte byStopHour;//½áÊøÊ±¼ä
            public byte byStopMin;//½áÊøÊ±¼ä
        }

        /*Éè±¸±¨¾¯ºÍÒì³£´¦Àí·½Ê½*/
        public const int NOACTION = 0;/*ÎÞÏìÓ¦*/
        public const int WARNONMONITOR = 1;/*¼àÊÓÆ÷ÉÏ¾¯¸æ*/
        public const int WARNONAUDIOOUT = 2;/*ÉùÒô¾¯¸æ*/
        public const int UPTOCENTER = 4;/*ÉÏ´«ÖÐÐÄ*/
        public const int TRIGGERALARMOUT = 8;/*´¥·¢±¨¾¯Êä³ö*/

        //±¨¾¯ºÍÒì³£´¦Àí½á¹¹(×Ó½á¹¹)(¶à´¦Ê¹ÓÃ)(9000À©Õ¹)
        [StructLayoutAttribute(LayoutKind.Sequential)]
        public struct NET_DVR_HANDLEEXCEPTION_V30
        {
            public uint dwHandleType;/*´¦Àí·½Ê½,´¦Àí·½Ê½µÄ"»ò"½á¹û*/
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = MAX_ALARMOUT_V30, ArraySubType = UnmanagedType.I1)]
            public byte[] byRelAlarmOut;//±¨¾¯´¥·¢µÄÊä³öÍ¨µÀ,±¨¾¯´¥·¢µÄÊä³ö,Îª1±íÊ¾´¥·¢¸ÃÊä³ö
        }

        //±¨¾¯ºÍÒì³£´¦Àí½á¹¹(×Ó½á¹¹)(¶à´¦Ê¹ÓÃ)
        [StructLayoutAttribute(LayoutKind.Sequential)]
        public struct NET_DVR_HANDLEEXCEPTION
        {
            public uint dwHandleType;/*´¦Àí·½Ê½,´¦Àí·½Ê½µÄ"»ò"½á¹û*/
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = MAX_ALARMOUT, ArraySubType = UnmanagedType.I1)]
            public byte[] byRelAlarmOut;//±¨¾¯´¥·¢µÄÊä³öÍ¨µÀ,±¨¾¯´¥·¢µÄÊä³ö,Îª1±íÊ¾´¥·¢¸ÃÊä³ö
        }

        //DVRÉè±¸²ÎÊý
        [StructLayoutAttribute(LayoutKind.Sequential)]
        public struct NET_DVR_DEVICECFG
        {
            public uint dwSize;
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = NAME_LEN, ArraySubType = UnmanagedType.I1)]
            public byte[] sDVRName;//DVRÃû³Æ
            public uint dwDVRID;//DVR ID,ÓÃÓÚÒ£¿ØÆ÷ //V1.4(0-99), V1.5(0-255)
            public uint dwRecycleRecord;//ÊÇ·ñÑ­»·Â¼Ïñ,0:²»ÊÇ; 1:ÊÇ
                                        //ÒÔÏÂ²»¿É¸ü¸Ä
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = SERIALNO_LEN, ArraySubType = UnmanagedType.I1)]
            public byte[] sSerialNumber;//ÐòÁÐºÅ
            public uint dwSoftwareVersion;//Èí¼þ°æ±¾ºÅ,¸ß16Î»ÊÇÖ÷°æ±¾,µÍ16Î»ÊÇ´Î°æ±¾
            public uint dwSoftwareBuildDate;//Èí¼þÉú³ÉÈÕÆÚ,0xYYYYMMDD
            public uint dwDSPSoftwareVersion;//DSPÈí¼þ°æ±¾,¸ß16Î»ÊÇÖ÷°æ±¾,µÍ16Î»ÊÇ´Î°æ±¾
            public uint dwDSPSoftwareBuildDate;// DSPÈí¼þÉú³ÉÈÕÆÚ,0xYYYYMMDD
            public uint dwPanelVersion;// Ç°Ãæ°å°æ±¾,¸ß16Î»ÊÇÖ÷°æ±¾,µÍ16Î»ÊÇ´Î°æ±¾
            public uint dwHardwareVersion;// Ó²¼þ°æ±¾,¸ß16Î»ÊÇÖ÷°æ±¾,µÍ16Î»ÊÇ´Î°æ±¾
            public byte byAlarmInPortNum;//DVR±¨¾¯ÊäÈë¸öÊý
            public byte byAlarmOutPortNum;//DVR±¨¾¯Êä³ö¸öÊý
            public byte byRS232Num;//DVR 232´®¿Ú¸öÊý
            public byte byRS485Num;//DVR 485´®¿Ú¸öÊý
            public byte byNetworkPortNum;//ÍøÂç¿Ú¸öÊý
            public byte byDiskCtrlNum;//DVR Ó²ÅÌ¿ØÖÆÆ÷¸öÊý
            public byte byDiskNum;//DVR Ó²ÅÌ¸öÊý
            public byte byDVRType;//DVRÀàÐÍ, 1:DVR 2:ATM DVR 3:DVS ......
            public byte byChanNum;//DVR Í¨µÀ¸öÊý
            public byte byStartChan;//ÆðÊ¼Í¨µÀºÅ,ÀýÈçDVS-1,DVR - 1
            public byte byDecordChans;//DVR ½âÂëÂ·Êý
            public byte byVGANum;//VGA¿ÚµÄ¸öÊý
            public byte byUSBNum;//USB¿ÚµÄ¸öÊý
            public byte byAuxoutNum;//¸¨¿ÚµÄ¸öÊý
            public byte byAudioNum;//ÓïÒô¿ÚµÄ¸öÊý
            public byte byIPChanNum;//×î´óÊý×ÖÍ¨µÀÊý
        }

        /*IPµØÖ·*/
        [StructLayout(LayoutKind.Sequential)]
        public struct NET_DVR_IPADDR
        {

            /// char[16]
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 16, ArraySubType = UnmanagedType.I1)]
            public byte[] sIpV4;

            /// BYTE[128]
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 128, ArraySubType = UnmanagedType.I1)]
            public byte[] byIPv6;
        }

        /*ÍøÂçÊý¾Ý½á¹¹(×Ó½á¹¹)(9000À©Õ¹)*/
        [StructLayoutAttribute(LayoutKind.Sequential)]
        public struct NET_DVR_ETHERNET_V30
        {
            public NET_DVR_IPADDR struDVRIP;//DVR IPµØÖ·
            public NET_DVR_IPADDR struDVRIPMask;//DVR IPµØÖ·ÑÚÂë
            public uint dwNetInterface;//ÍøÂç½Ó¿Ú1-10MBase-T 2-10MBase-TÈ«Ë«¹¤ 3-100MBase-TX 4-100MÈ«Ë«¹¤ 5-10M/100M×ÔÊÊÓ¦
            public ushort wDVRPort;//¶Ë¿ÚºÅ
            public ushort wMTU;//Ôö¼ÓMTUÉèÖÃ£¬Ä¬ÈÏ1500¡£
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = MACADDR_LEN, ArraySubType = UnmanagedType.I1)]
            public byte[] byMACAddr;// ÎïÀíµØÖ·
        }

        /*ÍøÂçÊý¾Ý½á¹¹(×Ó½á¹¹)*/
        [StructLayoutAttribute(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
        public struct NET_DVR_ETHERNET
        {
            [MarshalAsAttribute(UnmanagedType.ByValTStr, SizeConst = 16)]
            public string sDVRIP;//DVR IPµØÖ·
            [MarshalAsAttribute(UnmanagedType.ByValTStr, SizeConst = 16)]
            public string sDVRIPMask;//DVR IPµØÖ·ÑÚÂë
            public uint dwNetInterface;//ÍøÂç½Ó¿Ú 1-10MBase-T 2-10MBase-TÈ«Ë«¹¤ 3-100MBase-TX 4-100MÈ«Ë«¹¤ 5-10M/100M×ÔÊÊÓ¦
            public ushort wDVRPort;//¶Ë¿ÚºÅ
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = MACADDR_LEN, ArraySubType = UnmanagedType.I1)]
            public byte[] byMACAddr;//·þÎñÆ÷µÄÎïÀíµØÖ·
        }

        //pppoe½á¹¹
        [StructLayoutAttribute(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
        public struct NET_DVR_PPPOECFG
        {
            public uint dwPPPOE;//0-²»ÆôÓÃ,1-ÆôÓÃ
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = NAME_LEN, ArraySubType = UnmanagedType.I1)]
            public byte[] sPPPoEUser;//PPPoEÓÃ»§Ãû
            [MarshalAsAttribute(UnmanagedType.ByValTStr, SizeConst = PASSWD_LEN)]
            public string sPPPoEPassword;// PPPoEÃÜÂë
            public NET_DVR_IPADDR struPPPoEIP;//PPPoE IPµØÖ·
        }

        //ÍøÂçÅäÖÃ½á¹¹(9000À©Õ¹)
        [StructLayoutAttribute(LayoutKind.Sequential)]
        public struct NET_DVR_NETCFG_V30
        {
            public uint dwSize;
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = MAX_ETHERNET, ArraySubType = UnmanagedType.Struct)]
            public NET_DVR_ETHERNET_V30[] struEtherNet;//ÒÔÌ«Íø¿Ú
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 2, ArraySubType = UnmanagedType.Struct)]
            public NET_DVR_IPADDR[] struRes1;/*±£Áô*/
            public NET_DVR_IPADDR struAlarmHostIpAddr;/* ±¨¾¯Ö÷»úIPµØÖ· */
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 2, ArraySubType = UnmanagedType.U2)]
            public ushort[] wRes2;
            public ushort wAlarmHostIpPort;
            public byte byUseDhcp;
            public byte byIPv6Mode;//IPv6·ÖÅä·½Ê½£¬0-Â·ÓÉ¹«¸æ£¬1-ÊÖ¶¯ÉèÖÃ£¬2-ÆôÓÃDHCP·ÖÅä
            public NET_DVR_IPADDR struDnsServer1IpAddr;/* ÓòÃû·þÎñÆ÷1µÄIPµØÖ· */
            public NET_DVR_IPADDR struDnsServer2IpAddr;/* ÓòÃû·þÎñÆ÷2µÄIPµØÖ· */
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = MAX_DOMAIN_NAME, ArraySubType = UnmanagedType.I1)]
            public byte[] byIpResolver;
            public ushort wIpResolverPort;
            public ushort wHttpPortNo;
            public NET_DVR_IPADDR struMulticastIpAddr;/* ¶à²¥×éµØÖ· */
            public NET_DVR_IPADDR struGatewayIpAddr;/* Íø¹ØµØÖ· */
            public NET_DVR_PPPOECFG struPPPoE;
            public byte byEnablePrivateMulticastDiscovery;  //Ë½ÓÐ¶à²¥ËÑË÷£¬0~Ä¬ÈÏ£¬1~ÆôÓÃ£¬2-½ûÓÃ
            public byte byEnableOnvifMulticastDiscovery;  //Onvif¶à²¥ËÑË÷£¬0~Ä¬ÈÏ£¬1~ÆôÓÃ£¬2-½ûÓÃ
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 62, ArraySubType = UnmanagedType.I1)]
            public byte[] byRes;

            public void init()
            {
                struEtherNet = new NET_DVR_ETHERNET_V30[MAX_ETHERNET];
                struAlarmHostIpAddr = new NET_DVR_IPADDR();
                struDnsServer1IpAddr = new NET_DVR_IPADDR();
                struDnsServer2IpAddr = new NET_DVR_IPADDR();
                byIpResolver = new byte[MAX_DOMAIN_NAME];
                struMulticastIpAddr = new NET_DVR_IPADDR();
                struGatewayIpAddr = new NET_DVR_IPADDR();
                struPPPoE = new NET_DVR_PPPOECFG();
            }
        }

        //ÍøÂçÅäÖÃ½á¹¹
        [StructLayoutAttribute(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
        public struct NET_DVR_NETCFG
        {
            public uint dwSize;
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = MAX_ETHERNET, ArraySubType = UnmanagedType.Struct)]
            public NET_DVR_ETHERNET[] struEtherNet;/* ÒÔÌ«Íø¿Ú */
            [MarshalAsAttribute(UnmanagedType.ByValTStr, SizeConst = 16)]
            public string sManageHostIP;//Ô¶³Ì¹ÜÀíÖ÷»úµØÖ·
            public ushort wManageHostPort;//Ô¶³Ì¹ÜÀíÖ÷»ú¶Ë¿ÚºÅ
            [MarshalAsAttribute(UnmanagedType.ByValTStr, SizeConst = 16)]
            public string sIPServerIP;//IPServer·þÎñÆ÷µØÖ·
            [MarshalAsAttribute(UnmanagedType.ByValTStr, SizeConst = 16)]
            public string sMultiCastIP;//¶à²¥×éµØÖ·
            [MarshalAsAttribute(UnmanagedType.ByValTStr, SizeConst = 16)]
            public string sGatewayIP;//Íø¹ØµØÖ·
            [MarshalAsAttribute(UnmanagedType.ByValTStr, SizeConst = 16)]
            public string sNFSIP;//NFSÖ÷»úIPµØÖ·
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = PATHNAME_LEN, ArraySubType = UnmanagedType.I1)]
            public byte[] sNFSDirectory;//NFSÄ¿Â¼
            public uint dwPPPOE;//0-²»ÆôÓÃ,1-ÆôÓÃ
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = NAME_LEN, ArraySubType = UnmanagedType.I1)]
            public byte[] sPPPoEUser;//PPPoEÓÃ»§Ãû
            [MarshalAsAttribute(UnmanagedType.ByValTStr, SizeConst = PASSWD_LEN)]
            public string sPPPoEPassword;// PPPoEÃÜÂë
            [MarshalAsAttribute(UnmanagedType.ByValTStr, SizeConst = 16)]
            public string sPPPoEIP;//PPPoE IPµØÖ·(Ö»¶Á)
            public ushort wHttpPort;//HTTP¶Ë¿ÚºÅ
        }

        //Í¨µÀÍ¼Ïó½á¹¹
        //ÒÆ¶¯Õì²â(×Ó½á¹¹)(9000À©Õ¹)
        [StructLayoutAttribute(LayoutKind.Sequential)]
        public struct NET_DVR_MOTION_V30
        {
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 96 * 64, ArraySubType = UnmanagedType.I1)]
            public byte[] byMotionScope;/*Õì²âÇøÓò,0-96Î»,±íÊ¾64ÐÐ,¹²ÓÐ96*64¸öÐ¡ºê¿é,Îª1±íÊ¾ÊÇÒÆ¶¯Õì²âÇøÓò,0-±íÊ¾²»ÊÇ*/
            public byte byMotionSensitive;/*ÒÆ¶¯Õì²âÁéÃô¶È, 0 - 5,Ô½¸ßÔ½ÁéÃô,oxff¹Ø±Õ*/
            public byte byEnableHandleMotion;/* ÊÇ·ñ´¦ÀíÒÆ¶¯Õì²â 0£­·ñ 1£­ÊÇ*/
            public byte byPrecision;/* ÒÆ¶¯Õì²âËã·¨µÄ½ø¶È: 0--16*16, 1--32*32, 2--64*64 ... */
            public byte reservedData;
            public NET_DVR_HANDLEEXCEPTION_V30 struMotionHandleType;/* ´¦Àí·½Ê½ */
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = MAX_DAYS * MAX_TIMESEGMENT_V30, ArraySubType = UnmanagedType.Struct)]
            public NET_DVR_SCHEDTIME[] struAlarmTime;/*²¼·ÀÊ±¼ä*/
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = MAX_CHANNUM_V30, ArraySubType = UnmanagedType.I1)]
            public byte[] byRelRecordChan;/* ±¨¾¯´¥·¢µÄÂ¼ÏóÍ¨µÀ*/
        }

        //ÒÆ¶¯Õì²â(×Ó½á¹¹)
        [StructLayoutAttribute(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
        public struct NET_DVR_MOTION
        {
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 396, ArraySubType = UnmanagedType.I1)]
            public byte[] byMotionScope;/*Õì²âÇøÓò,¹²ÓÐ22*18¸öÐ¡ºê¿é,Îª1±íÊ¾¸Äºê¿éÊÇÒÆ¶¯Õì²âÇøÓò,0-±íÊ¾²»ÊÇ*/
            public byte byMotionSensitive;/*ÒÆ¶¯Õì²âÁéÃô¶È, 0 - 5,Ô½¸ßÔ½ÁéÃô,0xff¹Ø±Õ*/
            public byte byEnableHandleMotion;/* ÊÇ·ñ´¦ÀíÒÆ¶¯Õì²â */
            [MarshalAsAttribute(UnmanagedType.ByValTStr, SizeConst = 2)]
            public string reservedData;
            public NET_DVR_HANDLEEXCEPTION strMotionHandleType;/* ´¦Àí·½Ê½ */
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = MAX_DAYS * MAX_TIMESEGMENT, ArraySubType = UnmanagedType.Struct)]
            public NET_DVR_SCHEDTIME[] struAlarmTime;//²¼·ÀÊ±¼ä
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = MAX_CHANNUM, ArraySubType = UnmanagedType.I1)]
            public byte[] byRelRecordChan;//±¨¾¯´¥·¢µÄÂ¼ÏóÍ¨µÀ,Îª1±íÊ¾´¥·¢¸ÃÍ¨µÀ
        }

        //ÕÚµ²±¨¾¯(×Ó½á¹¹)(9000À©Õ¹)  ÇøÓò´óÐ¡704*576
        [StructLayoutAttribute(LayoutKind.Sequential)]
        public struct NET_DVR_HIDEALARM_V30
        {
            public uint dwEnableHideAlarm;/* ÊÇ·ñÆô¶¯ÕÚµ²±¨¾¯ ,0-·ñ,1-µÍÁéÃô¶È 2-ÖÐÁéÃô¶È 3-¸ßÁéÃô¶È*/
            public ushort wHideAlarmAreaTopLeftX;/* ÕÚµ²ÇøÓòµÄx×ø±ê */
            public ushort wHideAlarmAreaTopLeftY;/* ÕÚµ²ÇøÓòµÄy×ø±ê */
            public ushort wHideAlarmAreaWidth;/* ÕÚµ²ÇøÓòµÄ¿í */
            public ushort wHideAlarmAreaHeight;/*ÕÚµ²ÇøÓòµÄ¸ß*/
            public NET_DVR_HANDLEEXCEPTION_V30 strHideAlarmHandleType;  /* ´¦Àí·½Ê½ */
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = MAX_DAYS * MAX_TIMESEGMENT_V30, ArraySubType = UnmanagedType.Struct)]
            public NET_DVR_SCHEDTIME[] struAlarmTime;//²¼·ÀÊ±¼ä
        }

        //ÕÚµ²±¨¾¯(×Ó½á¹¹)  ÇøÓò´óÐ¡704*576
        [StructLayoutAttribute(LayoutKind.Sequential)]
        public struct NET_DVR_HIDEALARM
        {
            public uint dwEnableHideAlarm;/* ÊÇ·ñÆô¶¯ÕÚµ²±¨¾¯ ,0-·ñ,1-µÍÁéÃô¶È 2-ÖÐÁéÃô¶È 3-¸ßÁéÃô¶È*/
            public ushort wHideAlarmAreaTopLeftX;/* ÕÚµ²ÇøÓòµÄx×ø±ê */
            public ushort wHideAlarmAreaTopLeftY;/* ÕÚµ²ÇøÓòµÄy×ø±ê */
            public ushort wHideAlarmAreaWidth;/* ÕÚµ²ÇøÓòµÄ¿í */
            public ushort wHideAlarmAreaHeight;/*ÕÚµ²ÇøÓòµÄ¸ß*/
            public NET_DVR_HANDLEEXCEPTION strHideAlarmHandleType;/* ´¦Àí·½Ê½ */
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = MAX_DAYS * MAX_TIMESEGMENT, ArraySubType = UnmanagedType.Struct)]
            public NET_DVR_SCHEDTIME[] struAlarmTime;//²¼·ÀÊ±¼ä
        }

        //ÐÅºÅ¶ªÊ§±¨¾¯(×Ó½á¹¹)(9000À©Õ¹)
        [StructLayoutAttribute(LayoutKind.Sequential)]
        public struct NET_DVR_VILOST_V30
        {
            public byte byEnableHandleVILost;/* ÊÇ·ñ´¦ÀíÐÅºÅ¶ªÊ§±¨¾¯ */
            public NET_DVR_HANDLEEXCEPTION_V30 strVILostHandleType;/* ´¦Àí·½Ê½ */
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 56, ArraySubType = UnmanagedType.Struct)]
            public NET_DVR_SCHEDTIME[] struAlarmTime;//²¼·ÀÊ±¼ä
        }

        //ÐÅºÅ¶ªÊ§±¨¾¯(×Ó½á¹¹)
        [StructLayoutAttribute(LayoutKind.Sequential)]
        public struct NET_DVR_VILOST
        {
            public byte byEnableHandleVILost;/* ÊÇ·ñ´¦ÀíÐÅºÅ¶ªÊ§±¨¾¯ */
            public NET_DVR_HANDLEEXCEPTION strVILostHandleType;/* ´¦Àí·½Ê½ */
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = MAX_DAYS * MAX_TIMESEGMENT_V30, ArraySubType = UnmanagedType.Struct)]
            public NET_DVR_SCHEDTIME[] struAlarmTime;//²¼·ÀÊ±¼ä
        }

        //ÕÚµ²ÇøÓò(×Ó½á¹¹)
        [System.Runtime.InteropServices.StructLayoutAttribute(System.Runtime.InteropServices.LayoutKind.Sequential)]
        public struct NET_DVR_SHELTER
        {
            public ushort wHideAreaTopLeftX;/* ÕÚµ²ÇøÓòµÄx×ø±ê */
            public ushort wHideAreaTopLeftY;/* ÕÚµ²ÇøÓòµÄy×ø±ê */
            public ushort wHideAreaWidth;/* ÕÚµ²ÇøÓòµÄ¿í */
            public ushort wHideAreaHeight;/*ÕÚµ²ÇøÓòµÄ¸ß*/
        }

        [StructLayoutAttribute(LayoutKind.Sequential)]
        public struct NET_DVR_COLOR
        {
            public byte byBrightness;/*ÁÁ¶È,0-255*/
            public byte byContrast;/*¶Ô±È¶È,0-255*/
            public byte bySaturation;/*±¥ºÍ¶È,0-255*/
            public byte byHue;/*É«µ÷,0-255*/
        }

        //Í¨µÀÍ¼Ïó½á¹¹(9000À©Õ¹)
        [StructLayoutAttribute(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
        public struct NET_DVR_PICCFG_V30
        {
            public uint dwSize;
            [MarshalAsAttribute(UnmanagedType.ByValTStr, SizeConst = NAME_LEN)]
            //  [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = NAME_LEN, ArraySubType = UnmanagedType.I1)]
            public string sChanName;
            public uint dwVideoFormat;/* Ö»¶Á ÊÓÆµÖÆÊ½ 1-NTSC 2-PAL*/
            public NET_DVR_COLOR struColor;//	Í¼Ïñ²ÎÊý
            [MarshalAsAttribute(UnmanagedType.ByValTStr, SizeConst = 60)]
            public string reservedData;/*±£Áô*/
            //ÏÔÊ¾Í¨µÀÃû
            public uint dwShowChanName;// Ô¤ÀÀµÄÍ¼ÏóÉÏÊÇ·ñÏÔÊ¾Í¨µÀÃû³Æ,0-²»ÏÔÊ¾,1-ÏÔÊ¾ ÇøÓò´óÐ¡704*576
            public ushort wShowNameTopLeftX;/* Í¨µÀÃû³ÆÏÔÊ¾Î»ÖÃµÄx×ø±ê */
            public ushort wShowNameTopLeftY;/* Í¨µÀÃû³ÆÏÔÊ¾Î»ÖÃµÄy×ø±ê */
            //ÊÓÆµÐÅºÅ¶ªÊ§±¨¾¯
            public NET_DVR_VILOST_V30 struVILost;
            public NET_DVR_VILOST_V30 struRes;/*±£Áô*/
            //ÒÆ¶¯Õì²â
            public NET_DVR_MOTION_V30 struMotion;
            //ÕÚµ²±¨¾¯
            public NET_DVR_HIDEALARM_V30 struHideAlarm;
            //ÕÚµ²  ÇøÓò´óÐ¡704*576
            public uint dwEnableHide;/* ÊÇ·ñÆô¶¯ÕÚµ² ,0-·ñ,1-ÊÇ*/
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = MAX_SHELTERNUM, ArraySubType = UnmanagedType.Struct)]
            public NET_DVR_SHELTER[] struShelter;
            //OSD
            public uint dwShowOsd;// Ô¤ÀÀµÄÍ¼ÏóÉÏÊÇ·ñÏÔÊ¾OSD,0-²»ÏÔÊ¾,1-ÏÔÊ¾ ÇøÓò´óÐ¡704*576
            public ushort wOSDTopLeftX;/* OSDµÄx×ø±ê */
            public ushort wOSDTopLeftY;/* OSDµÄy×ø±ê */
            public byte byOSDType;/* OSDÀàÐÍ(Ö÷ÒªÊÇÄêÔÂÈÕ¸ñÊ½) */
            /* 0: XXXX-XX-XX ÄêÔÂÈÕ */
            /* 1: XX-XX-XXXX ÔÂÈÕÄê */
            /* 2: XXXXÄêXXÔÂXXÈÕ */
            /* 3: XXÔÂXXÈÕXXXXÄê */
            /* 4: XX-XX-XXXX ÈÕÔÂÄê*/
            /* 5: XXÈÕXXÔÂXXXXÄê */
            public byte byDispWeek;/* ÊÇ·ñÏÔÊ¾ÐÇÆÚ */
            public byte byOSDAttrib;/* OSDÊôÐÔ:Í¸Ã÷£¬ÉÁË¸ */
            /* 0: ²»ÏÔÊ¾OSD */
            /* 1: Í¸Ã÷,ÉÁË¸ */
            /* 2: Í¸Ã÷,²»ÉÁË¸ */
            /* 3: ÉÁË¸,²»Í¸Ã÷ */
            /* 4: ²»Í¸Ã÷,²»ÉÁË¸ */
            public byte byHourOSDType;/* OSDÐ¡Ê±ÖÆ:0-24Ð¡Ê±ÖÆ,1-12Ð¡Ê±ÖÆ */
            public byte byFontSize;//×ÖÌå´óÐ¡£¬16*16(ÖÐ)/8*16(Ó¢)£¬1-32*32(ÖÐ)/16*32(Ó¢)£¬2-64*64(ÖÐ)/32*64(Ó¢)  3-48*48(ÖÐ)/24*48(Ó¢) 0xff-×ÔÊÊÓ¦(adaptive)
            public byte byOSDColorType; //0-Ä¬ÈÏ£¨ºÚ°×£©£»1-×Ô¶¨Òå
            public byte byAlignment;//¶ÔÆë·½Ê½ 0-×ÔÊÊÓ¦£¬1-ÓÒ¶ÔÆë, 2-×ó¶ÔÆë
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 61, ArraySubType = UnmanagedType.I1)]
            public byte[] byRes;

        }

        //Í¨µÀÍ¼Ïó½á¹¹SDK_V14À©Õ¹
        [StructLayoutAttribute(LayoutKind.Sequential)]
        public struct NET_DVR_PICCFG_EX
        {
            public uint dwSize;
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = NAME_LEN, ArraySubType = UnmanagedType.I1)]
            public byte[] sChanName;
            public uint dwVideoFormat;/* Ö»¶Á ÊÓÆµÖÆÊ½ 1-NTSC 2-PAL*/
            public byte byBrightness;/*ÁÁ¶È,0-255*/
            public byte byContrast;/*¶Ô±È¶È,0-255*/
            public byte bySaturation;/*±¥ºÍ¶È,0-255 */
            public byte byHue;/*É«µ÷,0-255*/
            //ÏÔÊ¾Í¨µÀÃû
            public uint dwShowChanName;// Ô¤ÀÀµÄÍ¼ÏóÉÏÊÇ·ñÏÔÊ¾Í¨µÀÃû³Æ,0-²»ÏÔÊ¾,1-ÏÔÊ¾ ÇøÓò´óÐ¡704*576
            public ushort wShowNameTopLeftX;/* Í¨µÀÃû³ÆÏÔÊ¾Î»ÖÃµÄx×ø±ê */
            public ushort wShowNameTopLeftY;/* Í¨µÀÃû³ÆÏÔÊ¾Î»ÖÃµÄy×ø±ê */
            //ÐÅºÅ¶ªÊ§±¨¾¯
            public NET_DVR_VILOST struVILost;
            //ÒÆ¶¯Õì²â
            public NET_DVR_MOTION struMotion;
            //ÕÚµ²±¨¾¯
            public NET_DVR_HIDEALARM struHideAlarm;
            //ÕÚµ²  ÇøÓò´óÐ¡704*576
            public uint dwEnableHide;/* ÊÇ·ñÆô¶¯ÕÚµ² ,0-·ñ,1-ÊÇ*/
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = MAX_SHELTERNUM, ArraySubType = UnmanagedType.Struct)]
            public NET_DVR_SHELTER[] struShelter;
            //OSD
            public uint dwShowOsd;// Ô¤ÀÀµÄÍ¼ÏóÉÏÊÇ·ñÏÔÊ¾OSD,0-²»ÏÔÊ¾,1-ÏÔÊ¾ ÇøÓò´óÐ¡704*576
            public ushort wOSDTopLeftX;/* OSDµÄx×ø±ê */
            public ushort wOSDTopLeftY;/* OSDµÄy×ø±ê */
            public byte byOSDType;/* OSDÀàÐÍ(Ö÷ÒªÊÇÄêÔÂÈÕ¸ñÊ½) */
            /* 0: XXXX-XX-XX ÄêÔÂÈÕ */
            /* 1: XX-XX-XXXX ÔÂÈÕÄê */
            /* 2: XXXXÄêXXÔÂXXÈÕ */
            /* 3: XXÔÂXXÈÕXXXXÄê */
            /* 4: XX-XX-XXXX ÈÕÔÂÄê*/
            /* 5: XXÈÕXXÔÂXXXXÄê */
            public byte byDispWeek;/* ÊÇ·ñÏÔÊ¾ÐÇÆÚ */
            public byte byOSDAttrib;/* OSDÊôÐÔ:Í¸Ã÷£¬ÉÁË¸ */
            /* 0: ²»ÏÔÊ¾OSD */
            /* 1: Í¸Ã÷,ÉÁË¸ */
            /* 2: Í¸Ã÷,²»ÉÁË¸ */
            /* 3: ÉÁË¸,²»Í¸Ã÷ */
            /* 4: ²»Í¸Ã÷,²»ÉÁË¸ */
            public byte byHourOsdType;/* OSDÐ¡Ê±ÖÆ:0-24Ð¡Ê±ÖÆ,1-12Ð¡Ê±ÖÆ */
        }

        //Í¨µÀÍ¼Ïó½á¹¹(SDK_V13¼°Ö®Ç°°æ±¾)
        [StructLayoutAttribute(LayoutKind.Sequential)]
        public struct NET_DVR_PICCFG
        {
            public uint dwSize;
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = NAME_LEN, ArraySubType = UnmanagedType.I1)]
            public byte[] sChanName;
            public uint dwVideoFormat;/* Ö»¶Á ÊÓÆµÖÆÊ½ 1-NTSC 2-PAL*/
            public byte byBrightness;/*ÁÁ¶È,0-255*/
            public byte byContrast;/*¶Ô±È¶È,0-255*/
            public byte bySaturation;/*±¥ºÍ¶È,0-255 */
            public byte byHue;/*É«µ÷,0-255*/
            //ÏÔÊ¾Í¨µÀÃû
            public uint dwShowChanName;// Ô¤ÀÀµÄÍ¼ÏóÉÏÊÇ·ñÏÔÊ¾Í¨µÀÃû³Æ,0-²»ÏÔÊ¾,1-ÏÔÊ¾ ÇøÓò´óÐ¡704*576
            public ushort wShowNameTopLeftX;/* Í¨µÀÃû³ÆÏÔÊ¾Î»ÖÃµÄx×ø±ê */
            public ushort wShowNameTopLeftY;/* Í¨µÀÃû³ÆÏÔÊ¾Î»ÖÃµÄy×ø±ê */
            //ÐÅºÅ¶ªÊ§±¨¾¯
            public NET_DVR_VILOST struVILost;
            //ÒÆ¶¯Õì²â
            public NET_DVR_MOTION struMotion;
            //ÕÚµ²±¨¾¯
            public NET_DVR_HIDEALARM struHideAlarm;
            //ÕÚµ²  ÇøÓò´óÐ¡704*576
            public uint dwEnableHide;/* ÊÇ·ñÆô¶¯ÕÚµ² ,0-·ñ,1-ÊÇ*/
            public ushort wHideAreaTopLeftX;/* ÕÚµ²ÇøÓòµÄx×ø±ê */
            public ushort wHideAreaTopLeftY;/* ÕÚµ²ÇøÓòµÄy×ø±ê */
            public ushort wHideAreaWidth;/* ÕÚµ²ÇøÓòµÄ¿í */
            public ushort wHideAreaHeight;/*ÕÚµ²ÇøÓòµÄ¸ß*/
            //OSD
            public uint dwShowOsd;// Ô¤ÀÀµÄÍ¼ÏóÉÏÊÇ·ñÏÔÊ¾OSD,0-²»ÏÔÊ¾,1-ÏÔÊ¾ ÇøÓò´óÐ¡704*576
            public ushort wOSDTopLeftX;/* OSDµÄx×ø±ê */
            public ushort wOSDTopLeftY;/* OSDµÄy×ø±ê */
            public byte byOSDType;/* OSDÀàÐÍ(Ö÷ÒªÊÇÄêÔÂÈÕ¸ñÊ½) */
            /* 0: XXXX-XX-XX ÄêÔÂÈÕ */
            /* 1: XX-XX-XXXX ÔÂÈÕÄê */
            /* 2: XXXXÄêXXÔÂXXÈÕ */
            /* 3: XXÔÂXXÈÕXXXXÄê */
            /* 4: XX-XX-XXXX ÈÕÔÂÄê*/
            /* 5: XXÈÕXXÔÂXXXXÄê */
            public byte byDispWeek;/* ÊÇ·ñÏÔÊ¾ÐÇÆÚ */
            public byte byOSDAttrib;/* OSDÊôÐÔ:Í¸Ã÷£¬ÉÁË¸ */
            /* 0: ²»ÏÔÊ¾OSD */
            /* 1: Í¸Ã÷,ÉÁË¸ */
            /* 2: Í¸Ã÷,²»ÉÁË¸ */
            /* 3: ÉÁË¸,²»Í¸Ã÷ */
            /* 4: ²»Í¸Ã÷,²»ÉÁË¸ */
            public byte reservedData2;
        }

        //ÂëÁ÷Ñ¹Ëõ²ÎÊý(×Ó½á¹¹)(9000À©Õ¹)
        [StructLayoutAttribute(LayoutKind.Sequential)]
        public struct NET_DVR_COMPRESSION_INFO_V30
        {
            public byte byStreamType;//ÂëÁ÷ÀàÐÍ 0-ÊÓÆµÁ÷, 1-¸´ºÏÁ÷, ±íÊ¾ÊÂ¼þÑ¹Ëõ²ÎÊýÊ±×î¸ßÎ»±íÊ¾ÊÇ·ñÆôÓÃÑ¹Ëõ²ÎÊý
            public byte byResolution;//·Ö±æÂÊ0-DCIF 1-CIF, 2-QCIF, 3-4CIF, 4-2CIF 5£¨±£Áô£©16-VGA£¨640*480£© 17-UXGA£¨1600*1200£© 18-SVGA £¨800*600£©19-HD720p£¨1280*720£©20-XVGA  21-HD900p
            public byte byBitrateType;//ÂëÂÊÀàÐÍ 0:±äÂëÂÊ, 1:¶¨ÂëÂÊ
            public byte byPicQuality;//Í¼ÏóÖÊÁ¿ 0-×îºÃ 1-´ÎºÃ 2-½ÏºÃ 3-Ò»°ã 4-½Ï²î 5-²î
            public uint dwVideoBitrate;//ÊÓÆµÂëÂÊ 0-±£Áô 1-16K 2-32K 3-48k 4-64K 5-80K 6-96K 7-128K 8-160k 9-192K 10-224K 11-256K 12-320K
                                       // 13-384K 14-448K 15-512K 16-640K 17-768K 18-896K 19-1024K 20-1280K 21-1536K 22-1792K 23-2048K
                                       //×î¸ßÎ»(31Î»)ÖÃ³É1±íÊ¾ÊÇ×Ô¶¨ÒåÂëÁ÷, 0-30Î»±íÊ¾ÂëÁ÷Öµ¡£
            public uint dwVideoFrameRate;//Ö¡ÂÊ 0-È«²¿; 1-1/16; 2-1/8; 3-1/4; 4-1/2; 5-1; 6-2; 7-4; 8-6; 9-8; 10-10; 11-12; 12-16; 13-20; V2.0°æ±¾ÖÐÐÂ¼Ó14-15; 15-18; 16-22;
            public ushort wIntervalFrameI;//IÖ¡¼ä¸ô
                                          //2006-08-11 Ôö¼Óµ¥PÖ¡µÄÅäÖÃ½Ó¿Ú£¬¿ÉÒÔ¸ÄÉÆÊµÊ±Á÷ÑÓÊ±ÎÊÌâ
            public byte byIntervalBPFrame;//0-BBPÖ¡; 1-BPÖ¡; 2-µ¥PÖ¡
            public byte byres1; //±£Áô
            public byte byVideoEncType;//ÊÓÆµ±àÂëÀàÐÍ 0 hik264;1±ê×¼h264; 2±ê×¼mpeg4;
            public byte byAudioEncType; //ÒôÆµ±àÂëÀàÐÍ 0£­OggVorbis
            public byte byVideoEncComplexity; //ÊÓÆµ±àÂë¸´ÔÓ¶È£¬0-µÍ£¬1-ÖÐ£¬2¸ß,0xfe:×Ô¶¯£¬ºÍÔ´Ò»ÖÂ
            public byte byEnableSvc; //0 - ²»ÆôÓÃSVC¹¦ÄÜ£»1- ÆôÓÃSVC¹¦ÄÜ; 2-×Ô¶¯ÆôÓÃSVC¹¦ÄÜ
            public byte byFormatType; //·â×°ÀàÐÍ£¬1-ÂãÁ÷£¬2-RTP·â×°£¬3-PS·â×°£¬4-TS·â×°£¬5-Ë½ÓÐ£¬6-FLV£¬7-ASF£¬8-3GP,9-RTP+PS£¨¹ú±ê£ºGB28181£©£¬0xff-ÎÞÐ§
            public byte byAudioBitRate; //ÒôÆµÂëÂÊ ²Î¿¼ BITRATE_ENCODE_INDEX
            public byte byStreamSmooth;//ÂëÁ÷Æ½»¬ 1¡«100£¨1µÈ¼¶±íÊ¾ÇåÎú(Clear)£¬100±íÊ¾Æ½»¬(Smooth)£©
            public byte byAudioSamplingRate;//ÒôÆµ²ÉÑùÂÊ0-Ä¬ÈÏ,1- 16kHZ, 2-32kHZ, 3-48kHZ, 4- 44.1kHZ,5-8kHZ
            public byte bySmartCodec;//¸ßÐÔÄÜ±àÂë 0-¹Ø±Õ£¬1-´ò¿ª
            public byte byres;
            //Æ½¾ùÂëÂÊ£¨ÔÚSmartCodecÊ¹ÄÜ¿ªÆôÏÂÉúÐ§£©, 0-0K 1-16K 2-32K 3-48k 4-64K 5-80K 6-96K 7-128K 8-160k 9-192K 10-224K 11-256K 12-320K 13-384K 14-448K 15-512K 16-640K 17-768K 18-896K 19-1024K 20-1280K 21-1536K 22-1792K 23-2048K 24-2560K 25-3072K 26-4096K 27-5120K 28-6144K 29-7168K 30-8192K
            //×î¸ßÎ»(15Î»)ÖÃ³É1±íÊ¾ÊÇ×Ô¶¨ÒåÂëÁ÷, 0-14Î»±íÊ¾ÂëÁ÷Öµ(MIN- 0 K)¡£
            public ushort wAverageVideoBitrate;
        }

        //Í¨µÀÑ¹Ëõ²ÎÊý(9000À©Õ¹)
        [StructLayoutAttribute(LayoutKind.Sequential)]
        public struct NET_DVR_COMPRESSIONCFG_V30
        {
            public uint dwSize;
            public NET_DVR_COMPRESSION_INFO_V30 struNormHighRecordPara;//Â¼Ïñ ¶ÔÓ¦8000µÄÆÕÍ¨
            public NET_DVR_COMPRESSION_INFO_V30 struRes;//±£Áô char reserveData[28];
            public NET_DVR_COMPRESSION_INFO_V30 struEventRecordPara;//ÊÂ¼þ´¥·¢Ñ¹Ëõ²ÎÊý
            public NET_DVR_COMPRESSION_INFO_V30 struNetPara;//Íø´«(×ÓÂëÁ÷)
        }

        //ÂëÁ÷Ñ¹Ëõ²ÎÊý(×Ó½á¹¹)
        [StructLayoutAttribute(LayoutKind.Sequential)]
        public struct NET_DVR_COMPRESSION_INFO
        {
            public byte byStreamType;//ÂëÁ÷ÀàÐÍ0-ÊÓÆµÁ÷,1-¸´ºÏÁ÷,±íÊ¾Ñ¹Ëõ²ÎÊýÊ±×î¸ßÎ»±íÊ¾ÊÇ·ñÆôÓÃÑ¹Ëõ²ÎÊý
            public byte byResolution;//·Ö±æÂÊ0-DCIF 1-CIF, 2-QCIF, 3-4CIF, 4-2CIF, 5-2QCIF(352X144)(³µÔØ×¨ÓÃ)
            public byte byBitrateType;//ÂëÂÊÀàÐÍ0:±äÂëÂÊ£¬1:¶¨ÂëÂÊ
            public byte byPicQuality;//Í¼ÏóÖÊÁ¿ 0-×îºÃ 1-´ÎºÃ 2-½ÏºÃ 3-Ò»°ã 4-½Ï²î 5-²î
            public uint dwVideoBitrate; //ÊÓÆµÂëÂÊ 0-±£Áô 1-16K(±£Áô) 2-32K 3-48k 4-64K 5-80K 6-96K 7-128K 8-160k 9-192K 10-224K 11-256K 12-320K
                                        // 13-384K 14-448K 15-512K 16-640K 17-768K 18-896K 19-1024K 20-1280K 21-1536K 22-1792K 23-2048K
                                        //×î¸ßÎ»(31Î»)ÖÃ³É1±íÊ¾ÊÇ×Ô¶¨ÒåÂëÁ÷, 0-30Î»±íÊ¾ÂëÁ÷Öµ(MIN-32K MAX-8192K)¡£
            public uint dwVideoFrameRate;//Ö¡ÂÊ 0-È«²¿; 1-1/16; 2-1/8; 3-1/4; 4-1/2; 5-1; 6-2; 7-4; 8-6; 9-8; 10-10; 11-12; 12-16; 13-20;
        }

        //Í¨µÀÑ¹Ëõ²ÎÊý
        [StructLayoutAttribute(LayoutKind.Sequential)]
        public struct NET_DVR_COMPRESSIONCFG
        {
            public uint dwSize;
            public NET_DVR_COMPRESSION_INFO struRecordPara;//Â¼Ïñ/ÊÂ¼þ´¥·¢Â¼Ïñ
            public NET_DVR_COMPRESSION_INFO struNetPara;//Íø´«/±£Áô
        }

        //ÂëÁ÷Ñ¹Ëõ²ÎÊý(×Ó½á¹¹)(À©Õ¹) Ôö¼ÓIÖ¡¼ä¸ô
        [StructLayoutAttribute(LayoutKind.Sequential)]
        public struct NET_DVR_COMPRESSION_INFO_EX
        {
            public byte byStreamType;//ÂëÁ÷ÀàÐÍ0-ÊÓÆµÁ÷, 1-¸´ºÏÁ÷
            public byte byResolution;//·Ö±æÂÊ0-DCIF 1-CIF, 2-QCIF, 3-4CIF, 4-2CIF, 5-2QCIF(352X144)(³µÔØ×¨ÓÃ)
            public byte byBitrateType;//ÂëÂÊÀàÐÍ0:±äÂëÂÊ£¬1:¶¨ÂëÂÊ
            public byte byPicQuality;//Í¼ÏóÖÊÁ¿ 0-×îºÃ 1-´ÎºÃ 2-½ÏºÃ 3-Ò»°ã 4-½Ï²î 5-²î
            public uint dwVideoBitrate;//ÊÓÆµÂëÂÊ 0-±£Áô 1-16K(±£Áô) 2-32K 3-48k 4-64K 5-80K 6-96K 7-128K 8-160k 9-192K 10-224K 11-256K 12-320K
                                       // 13-384K 14-448K 15-512K 16-640K 17-768K 18-896K 19-1024K 20-1280K 21-1536K 22-1792K 23-2048K
                                       //×î¸ßÎ»(31Î»)ÖÃ³É1±íÊ¾ÊÇ×Ô¶¨ÒåÂëÁ÷, 0-30Î»±íÊ¾ÂëÁ÷Öµ(MIN-32K MAX-8192K)¡£
            public uint dwVideoFrameRate;//Ö¡ÂÊ 0-È«²¿; 1-1/16; 2-1/8; 3-1/4; 4-1/2; 5-1; 6-2; 7-4; 8-6; 9-8; 10-10; 11-12; 12-16; 13-20, //V2.0Ôö¼Ó14-15, 15-18, 16-22;
            public ushort wIntervalFrameI;//IÖ¡¼ä¸ô
                                          //2006-08-11 Ôö¼Óµ¥PÖ¡µÄÅäÖÃ½Ó¿Ú£¬¿ÉÒÔ¸ÄÉÆÊµÊ±Á÷ÑÓÊ±ÎÊÌâ
            public byte byIntervalBPFrame;//0-BBPÖ¡; 1-BPÖ¡; 2-µ¥PÖ¡
            public byte byRes;
        }

        //Í¨µÀÑ¹Ëõ²ÎÊý(À©Õ¹)
        [StructLayoutAttribute(LayoutKind.Sequential)]
        public struct NET_DVR_COMPRESSIONCFG_EX
        {
            public uint dwSize;
            public NET_DVR_COMPRESSION_INFO_EX struRecordPara;//Â¼Ïñ
            public NET_DVR_COMPRESSION_INFO_EX struNetPara;//Íø´«
        }

        //Ê±¼ä¶ÎÂ¼Ïñ²ÎÊýÅäÖÃ(×Ó½á¹¹)
        [StructLayoutAttribute(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
        public struct NET_DVR_RECORDSCHED
        {
            public NET_DVR_SCHEDTIME struRecordTime;
            public byte byRecordType;//0:¶¨Ê±Â¼Ïñ£¬1:ÒÆ¶¯Õì²â£¬2:±¨¾¯Â¼Ïñ£¬3:¶¯²â|±¨¾¯£¬4:¶¯²â&±¨¾¯, 5:ÃüÁî´¥·¢, 6: ÖÇÄÜÂ¼Ïñ
            [MarshalAsAttribute(UnmanagedType.ByValTStr, SizeConst = 3)]
            public string reservedData;
        }

        //È«ÌìÂ¼Ïñ²ÎÊýÅäÖÃ(×Ó½á¹¹)
        [StructLayoutAttribute(LayoutKind.Sequential)]
        public struct NET_DVR_RECORDDAY
        {
            public ushort wAllDayRecord;/* ÊÇ·ñÈ«ÌìÂ¼Ïñ 0-·ñ 1-ÊÇ*/
            public byte byRecordType;/* Â¼ÏóÀàÐÍ 0:¶¨Ê±Â¼Ïñ£¬1:ÒÆ¶¯Õì²â£¬2:±¨¾¯Â¼Ïñ£¬3:¶¯²â|±¨¾¯£¬4:¶¯²â&±¨¾¯ 5:ÃüÁî´¥·¢, 6: ÖÇÄÜÂ¼Ïñ*/
            public byte reservedData;
        }

        //Í¨µÀÂ¼Ïñ²ÎÊýÅäÖÃ(9000À©Õ¹)
        [StructLayoutAttribute(LayoutKind.Sequential)]
        public struct NET_DVR_RECORD_V30
        {
            public uint dwSize;
            public uint dwRecord;/*ÊÇ·ñÂ¼Ïñ 0-·ñ 1-ÊÇ*/
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = MAX_DAYS, ArraySubType = UnmanagedType.Struct)]
            public NET_DVR_RECORDDAY[] struRecAllDay;
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = MAX_DAYS * MAX_TIMESEGMENT_V30, ArraySubType = UnmanagedType.Struct)]
            public NET_DVR_RECORDSCHED[] struRecordSched;
            public uint dwRecordTime;/* Â¼ÏóÑÓÊ±³¤¶È 0-5Ãë£¬ 1-20Ãë£¬ 2-30Ãë£¬ 3-1·ÖÖÓ£¬ 4-2·ÖÖÓ£¬ 5-5·ÖÖÓ£¬ 6-10·ÖÖÓ*/
            public uint dwPreRecordTime;/* Ô¤Â¼Ê±¼ä 0-²»Ô¤Â¼ 1-5Ãë 2-10Ãë 3-15Ãë 4-20Ãë 5-25Ãë 6-30Ãë 7-0xffffffff(¾¡¿ÉÄÜÔ¤Â¼) */
            public uint dwRecorderDuration;/* Â¼Ïñ±£´æµÄ×î³¤Ê±¼ä */
            public byte byRedundancyRec;/*ÊÇ·ñÈßÓàÂ¼Ïñ,ÖØÒªÊý¾ÝË«±¸·Ý£º0/1*/
            public byte byAudioRec;/*Â¼ÏñÊ±¸´ºÏÁ÷±àÂëÊ±ÊÇ·ñ¼ÇÂ¼ÒôÆµÊý¾Ý£º¹úÍâÓÐ´Ë·¨¹æ*/
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 10, ArraySubType = UnmanagedType.I1)]
            public byte[] byReserve;
        }

        //Í¨µÀÂ¼Ïñ²ÎÊýÅäÖÃ
        [StructLayoutAttribute(LayoutKind.Sequential)]
        public struct NET_DVR_RECORD
        {
            public uint dwSize;
            public uint dwRecord;/*ÊÇ·ñÂ¼Ïñ 0-·ñ 1-ÊÇ*/
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = MAX_DAYS, ArraySubType = UnmanagedType.Struct)]
            public NET_DVR_RECORDDAY[] struRecAllDay;
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = MAX_DAYS * MAX_TIMESEGMENT, ArraySubType = UnmanagedType.Struct)]
            public NET_DVR_RECORDSCHED[] struRecordSched;
            public uint dwRecordTime;/* Â¼ÏóÊ±¼ä³¤¶È */
            public uint dwPreRecordTime;/* Ô¤Â¼Ê±¼ä 0-²»Ô¤Â¼ 1-5Ãë 2-10Ãë 3-15Ãë 4-20Ãë 5-25Ãë 6-30Ãë 7-0xffffffff(¾¡¿ÉÄÜÔ¤Â¼) */
        }

        //ÔÆÌ¨Ð­Òé±í½á¹¹ÅäÖÃ
        [StructLayoutAttribute(LayoutKind.Sequential)]
        public struct NET_DVR_PTZ_PROTOCOL
        {
            public uint dwType;/*½âÂëÆ÷ÀàÐÍÖµ£¬´Ó1¿ªÊ¼Á¬ÐøµÝÔö*/
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = DESC_LEN, ArraySubType = UnmanagedType.I1)]
            public byte[] byDescribe;/*½âÂëÆ÷µÄÃèÊö·û£¬ºÍ8000ÖÐµÄÒ»ÖÂ*/
        }

        [StructLayoutAttribute(LayoutKind.Sequential)]
        public struct NET_DVR_PTZCFG
        {
            public uint dwSize;
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = PTZ_PROTOCOL_NUM, ArraySubType = UnmanagedType.Struct)]
            public NET_DVR_PTZ_PROTOCOL[] struPtz;/*×î´ó200ÖÐPTZÐ­Òé*/
            public uint dwPtzNum;/*ÓÐÐ§µÄptzÐ­ÒéÊýÄ¿£¬´Ó0¿ªÊ¼(¼´¼ÆËãÊ±¼Ó1)*/
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 8, ArraySubType = UnmanagedType.I1)]
            public byte[] byRes;
        }
        /***************************ÔÆÌ¨ÀàÐÍ(end)******************************/

        //Í¨µÀ½âÂëÆ÷(ÔÆÌ¨)²ÎÊýÅäÖÃ(9000À©Õ¹)
        [StructLayoutAttribute(LayoutKind.Sequential)]
        public struct NET_DVR_DECODERCFG_V30
        {
            public uint dwSize;
            public uint dwBaudRate;//²¨ÌØÂÊ(bps)£¬0£­50£¬1£­75£¬2£­110£¬3£­150£¬4£­300£¬5£­600£¬6£­1200£¬7£­2400£¬8£­4800£¬9£­9600£¬10£­19200£¬ 11£­38400£¬12£­57600£¬13£­76800£¬14£­115.2k;
            public byte byDataBit;// Êý¾ÝÓÐ¼¸Î» 0£­5Î»£¬1£­6Î»£¬2£­7Î»£¬3£­8Î»;
            public byte byStopBit;// Í£Ö¹Î» 0£­1Î»£¬1£­2Î»
            public byte byParity;// Ð£Ñé 0£­ÎÞÐ£Ñé£¬1£­ÆæÐ£Ñé£¬2£­Å¼Ð£Ñé;
            public byte byFlowcontrol;// 0£­ÎÞ£¬1£­ÈíÁ÷¿Ø,2-Ó²Á÷¿Ø
            public ushort wDecoderType;//½âÂëÆ÷ÀàÐÍ, ´Ó0¿ªÊ¼£¬¶ÔÓ¦ptzÐ­ÒéÁÐ±í
            public ushort wDecoderAddress;/*½âÂëÆ÷µØÖ·:0 - 255*/
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = MAX_PRESET_V30, ArraySubType = UnmanagedType.I1)]
            public byte[] bySetPreset;/* Ô¤ÖÃµãÊÇ·ñÉèÖÃ,0-Ã»ÓÐÉèÖÃ,1-ÉèÖÃ*/
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = MAX_CRUISE_V30, ArraySubType = UnmanagedType.I1)]
            public byte[] bySetCruise;/* Ñ²º½ÊÇ·ñÉèÖÃ: 0-Ã»ÓÐÉèÖÃ,1-ÉèÖÃ */
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = MAX_TRACK_V30, ArraySubType = UnmanagedType.I1)]
            public byte[] bySetTrack;/* ¹ì¼£ÊÇ·ñÉèÖÃ,0-Ã»ÓÐÉèÖÃ,1-ÉèÖÃ*/
        }

        //Í¨µÀ½âÂëÆ÷(ÔÆÌ¨)²ÎÊýÅäÖÃ
        [StructLayoutAttribute(LayoutKind.Sequential)]
        public struct NET_DVR_DECODERCFG
        {
            public uint dwSize;
            public uint dwBaudRate; //²¨ÌØÂÊ(bps)£¬0£­50£¬1£­75£¬2£­110£¬3£­150£¬4£­300£¬5£­600£¬6£­1200£¬7£­2400£¬8£­4800£¬9£­9600£¬10£­19200£¬ 11£­38400£¬12£­57600£¬13£­76800£¬14£­115.2k;
            public byte byDataBit; // Êý¾ÝÓÐ¼¸Î» 0£­5Î»£¬1£­6Î»£¬2£­7Î»£¬3£­8Î»;
            public byte byStopBit;// Í£Ö¹Î» 0£­1Î»£¬1£­2Î»;
            public byte byParity; // Ð£Ñé 0£­ÎÞÐ£Ñé£¬1£­ÆæÐ£Ñé£¬2£­Å¼Ð£Ñé;
            public byte byFlowcontrol;// 0£­ÎÞ£¬1£­ÈíÁ÷¿Ø,2-Ó²Á÷¿Ø
            public ushort wDecoderType;//½âÂëÆ÷ÀàÐÍ, 0£­YouLi£¬1£­LiLin-1016£¬2£­LiLin-820£¬3£­Pelco-p£¬4£­DM DynaColor£¬5£­HD600£¬6£­JC-4116£¬7£­Pelco-d WX£¬8£­Pelco-d PICO
            public ushort wDecoderAddress;/*½âÂëÆ÷µØÖ·:0 - 255*/
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = MAX_PRESET, ArraySubType = UnmanagedType.I1)]
            public byte[] bySetPreset;/* Ô¤ÖÃµãÊÇ·ñÉèÖÃ,0-Ã»ÓÐÉèÖÃ,1-ÉèÖÃ*/
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = MAX_CRUISE, ArraySubType = UnmanagedType.I1)]
            public byte[] bySetCruise;/* Ñ²º½ÊÇ·ñÉèÖÃ: 0-Ã»ÓÐÉèÖÃ,1-ÉèÖÃ */
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = MAX_TRACK, ArraySubType = UnmanagedType.I1)]
            public byte[] bySetTrack;/* ¹ì¼£ÊÇ·ñÉèÖÃ,0-Ã»ÓÐÉèÖÃ,1-ÉèÖÃ*/
        }

        //ppp²ÎÊýÅäÖÃ(×Ó½á¹¹)
        [StructLayoutAttribute(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
        public struct NET_DVR_PPPCFG_V30
        {
            public NET_DVR_IPADDR struRemoteIP;//Ô¶¶ËIPµØÖ·
            public NET_DVR_IPADDR struLocalIP;//±¾µØIPµØÖ·
            [MarshalAsAttribute(UnmanagedType.ByValTStr, SizeConst = 16)]
            public string sLocalIPMask;//±¾µØIPµØÖ·ÑÚÂë
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = NAME_LEN, ArraySubType = UnmanagedType.I1)]
            public byte[] sUsername;/* ÓÃ»§Ãû */
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = PASSWD_LEN, ArraySubType = UnmanagedType.I1)]
            public byte[] sPassword;/* ÃÜÂë */
            public byte byPPPMode;//PPPÄ£Ê½, 0£­Ö÷¶¯£¬1£­±»¶¯
            public byte byRedial;//ÊÇ·ñ»Ø²¦ £º0-·ñ,1-ÊÇ
            public byte byRedialMode;//»Ø²¦Ä£Ê½,0-ÓÉ²¦ÈëÕßÖ¸¶¨,1-Ô¤ÖÃ»Ø²¦ºÅÂë
            public byte byDataEncrypt;//Êý¾Ý¼ÓÃÜ,0-·ñ,1-ÊÇ
            public uint dwMTU;//MTU
            [MarshalAsAttribute(UnmanagedType.ByValTStr, SizeConst = PHONENUMBER_LEN)]
            public string sTelephoneNumber;//µç»°ºÅÂë
        }

        //ppp²ÎÊýÅäÖÃ(×Ó½á¹¹)
        [StructLayoutAttribute(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
        public struct NET_DVR_PPPCFG
        {
            [MarshalAsAttribute(UnmanagedType.ByValTStr, SizeConst = 16)]
            public string sRemoteIP;//Ô¶¶ËIPµØÖ·
            [MarshalAsAttribute(UnmanagedType.ByValTStr, SizeConst = 16)]
            public string sLocalIP;//±¾µØIPµØÖ·
            [MarshalAsAttribute(UnmanagedType.ByValTStr, SizeConst = 16)]
            public string sLocalIPMask;//±¾µØIPµØÖ·ÑÚÂë
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = NAME_LEN, ArraySubType = UnmanagedType.I1)]
            public byte[] sUsername;/* ÓÃ»§Ãû */
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = PASSWD_LEN, ArraySubType = UnmanagedType.I1)]
            public byte[] sPassword;/* ÃÜÂë */
            public byte byPPPMode;//PPPÄ£Ê½, 0£­Ö÷¶¯£¬1£­±»¶¯
            public byte byRedial;//ÊÇ·ñ»Ø²¦ £º0-·ñ,1-ÊÇ
            public byte byRedialMode;//»Ø²¦Ä£Ê½,0-ÓÉ²¦ÈëÕßÖ¸¶¨,1-Ô¤ÖÃ»Ø²¦ºÅÂë
            public byte byDataEncrypt;//Êý¾Ý¼ÓÃÜ,0-·ñ,1-ÊÇ
            public uint dwMTU;//MTU
            [MarshalAsAttribute(UnmanagedType.ByValTStr, SizeConst = PHONENUMBER_LEN)]
            public string sTelephoneNumber;//µç»°ºÅÂë
        }

        //RS232´®¿Ú²ÎÊýÅäÖÃ(9000À©Õ¹)
        [StructLayoutAttribute(LayoutKind.Sequential)]
        public struct NET_DVR_SINGLE_RS232
        {
            public uint dwBaudRate;/*²¨ÌØÂÊ(bps)£¬0£­50£¬1£­75£¬2£­110£¬3£­150£¬4£­300£¬5£­600£¬6£­1200£¬7£­2400£¬8£­4800£¬9£­9600£¬10£­19200£¬ 11£­38400£¬12£­57600£¬13£­76800£¬14£­115.2k;*/
            public byte byDataBit;/* Êý¾ÝÓÐ¼¸Î» 0£­5Î»£¬1£­6Î»£¬2£­7Î»£¬3£­8Î» */
            public byte byStopBit;/* Í£Ö¹Î» 0£­1Î»£¬1£­2Î» */
            public byte byParity;/* Ð£Ñé 0£­ÎÞÐ£Ñé£¬1£­ÆæÐ£Ñé£¬2£­Å¼Ð£Ñé */
            public byte byFlowcontrol;/* 0£­ÎÞ£¬1£­ÈíÁ÷¿Ø,2-Ó²Á÷¿Ø */
            public uint dwWorkMode; /* ¹¤×÷Ä£Ê½£¬0£­232´®¿ÚÓÃÓÚPPP²¦ºÅ£¬1£­232´®¿ÚÓÃÓÚ²ÎÊý¿ØÖÆ£¬2£­Í¸Ã÷Í¨µÀ */
        }

        //RS232´®¿Ú²ÎÊýÅäÖÃ(9000À©Õ¹)
        [StructLayoutAttribute(LayoutKind.Sequential)]
        public struct NET_DVR_RS232CFG_V30
        {
            public uint dwSize;
            public NET_DVR_SINGLE_RS232 struRs232;
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 84, ArraySubType = UnmanagedType.I1)]
            public byte[] byRes;
            public NET_DVR_PPPCFG_V30 struPPPConfig;
        }

        //RS232´®¿Ú²ÎÊýÅäÖÃ
        [StructLayoutAttribute(LayoutKind.Sequential)]
        public struct NET_DVR_RS232CFG
        {
            public uint dwSize;
            public uint dwBaudRate;//²¨ÌØÂÊ(bps)£¬0£­50£¬1£­75£¬2£­110£¬3£­150£¬4£­300£¬5£­600£¬6£­1200£¬7£­2400£¬8£­4800£¬9£­9600£¬10£­19200£¬ 11£­38400£¬12£­57600£¬13£­76800£¬14£­115.2k;
            public byte byDataBit;// Êý¾ÝÓÐ¼¸Î» 0£­5Î»£¬1£­6Î»£¬2£­7Î»£¬3£­8Î»;
            public byte byStopBit;// Í£Ö¹Î» 0£­1Î»£¬1£­2Î»;
            public byte byParity;// Ð£Ñé 0£­ÎÞÐ£Ñé£¬1£­ÆæÐ£Ñé£¬2£­Å¼Ð£Ñé;
            public byte byFlowcontrol;// 0£­ÎÞ£¬1£­ÈíÁ÷¿Ø,2-Ó²Á÷¿Ø
            public uint dwWorkMode;// ¹¤×÷Ä£Ê½£¬0£­Õ­´ø´«Êä(232´®¿ÚÓÃÓÚPPP²¦ºÅ)£¬1£­¿ØÖÆÌ¨(232´®¿ÚÓÃÓÚ²ÎÊý¿ØÖÆ)£¬2£­Í¸Ã÷Í¨µÀ
            public NET_DVR_PPPCFG struPPPConfig;
        }

        //±¨¾¯ÊäÈë²ÎÊýÅäÖÃ(9000À©Õ¹)
        [StructLayoutAttribute(LayoutKind.Sequential)]
        public struct NET_DVR_ALARMINCFG_V30
        {
            public uint dwSize;
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = NAME_LEN, ArraySubType = UnmanagedType.I1)]
            public byte[] sAlarmInName;/* Ãû³Æ */
            public byte byAlarmType; //±¨¾¯Æ÷ÀàÐÍ,0£º³£¿ª,1£º³£±Õ
            public byte byAlarmInHandle; /* ÊÇ·ñ´¦Àí 0-²»´¦Àí 1-´¦Àí*/
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 2, ArraySubType = UnmanagedType.I1)]
            public byte[] byRes1;
            public NET_DVR_HANDLEEXCEPTION_V30 struAlarmHandleType;/* ´¦Àí·½Ê½ */
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = MAX_DAYS * MAX_TIMESEGMENT_V30, ArraySubType = UnmanagedType.Struct)]
            public NET_DVR_SCHEDTIME[] struAlarmTime;//²¼·ÀÊ±¼ä
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = MAX_CHANNUM_V30, ArraySubType = UnmanagedType.I1)]
            public byte[] byRelRecordChan;//±¨¾¯´¥·¢µÄÂ¼ÏóÍ¨µÀ,Îª1±íÊ¾´¥·¢¸ÃÍ¨µÀ
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = MAX_CHANNUM_V30, ArraySubType = UnmanagedType.I1)]
            public byte[] byEnablePreset;/* ÊÇ·ñµ÷ÓÃÔ¤ÖÃµã 0-·ñ,1-ÊÇ*/
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = MAX_CHANNUM_V30, ArraySubType = UnmanagedType.I1)]
            public byte[] byPresetNo;/* µ÷ÓÃµÄÔÆÌ¨Ô¤ÖÃµãÐòºÅ,Ò»¸ö±¨¾¯ÊäÈë¿ÉÒÔµ÷ÓÃ¶à¸öÍ¨µÀµÄÔÆÌ¨Ô¤ÖÃµã, 0xff±íÊ¾²»µ÷ÓÃÔ¤ÖÃµã¡£*/
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 192, ArraySubType = UnmanagedType.I1)]
            public byte[] byRes2;/*±£Áô*/
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = MAX_CHANNUM_V30, ArraySubType = UnmanagedType.I1)]
            public byte[] byEnableCruise;/* ÊÇ·ñµ÷ÓÃÑ²º½ 0-·ñ,1-ÊÇ*/
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = MAX_CHANNUM_V30, ArraySubType = UnmanagedType.I1)]
            public byte[] byCruiseNo;/* Ñ²º½ */
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = MAX_CHANNUM_V30, ArraySubType = UnmanagedType.I1)]
            public byte[] byEnablePtzTrack;/* ÊÇ·ñµ÷ÓÃ¹ì¼£ 0-·ñ,1-ÊÇ*/
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = MAX_CHANNUM_V30, ArraySubType = UnmanagedType.I1)]
            public byte[] byPTZTrack;/* µ÷ÓÃµÄÔÆÌ¨µÄ¹ì¼£ÐòºÅ */
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 16, ArraySubType = UnmanagedType.I1)]
            public byte[] byRes3;
        }

        //±¨¾¯ÊäÈë²ÎÊýÅäÖÃ
        [StructLayoutAttribute(LayoutKind.Sequential)]
        public struct NET_DVR_ALARMINCFG
        {
            public uint dwSize;
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = NAME_LEN, ArraySubType = UnmanagedType.I1)]
            public byte[] sAlarmInName;/* Ãû³Æ */
            public byte byAlarmType;//±¨¾¯Æ÷ÀàÐÍ,0£º³£¿ª,1£º³£±Õ
            public byte byAlarmInHandle;/* ÊÇ·ñ´¦Àí 0-²»´¦Àí 1-´¦Àí*/
            public NET_DVR_HANDLEEXCEPTION struAlarmHandleType;/* ´¦Àí·½Ê½ */
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = MAX_DAYS * MAX_TIMESEGMENT, ArraySubType = UnmanagedType.Struct)]
            public NET_DVR_SCHEDTIME[] struAlarmTime;//²¼·ÀÊ±¼ä
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = MAX_CHANNUM, ArraySubType = UnmanagedType.I1)]
            public byte[] byRelRecordChan;//±¨¾¯´¥·¢µÄÂ¼ÏóÍ¨µÀ,Îª1±íÊ¾´¥·¢¸ÃÍ¨µÀ
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = MAX_CHANNUM, ArraySubType = UnmanagedType.I1)]
            public byte[] byEnablePreset;/* ÊÇ·ñµ÷ÓÃÔ¤ÖÃµã 0-·ñ,1-ÊÇ*/
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = MAX_CHANNUM, ArraySubType = UnmanagedType.I1)]
            public byte[] byPresetNo;/* µ÷ÓÃµÄÔÆÌ¨Ô¤ÖÃµãÐòºÅ,Ò»¸ö±¨¾¯ÊäÈë¿ÉÒÔµ÷ÓÃ¶à¸öÍ¨µÀµÄÔÆÌ¨Ô¤ÖÃµã, 0xff±íÊ¾²»µ÷ÓÃÔ¤ÖÃµã¡£*/
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = MAX_CHANNUM, ArraySubType = UnmanagedType.I1)]
            public byte[] byEnableCruise;/* ÊÇ·ñµ÷ÓÃÑ²º½ 0-·ñ,1-ÊÇ*/
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = MAX_CHANNUM, ArraySubType = UnmanagedType.I1)]
            public byte[] byCruiseNo;/* Ñ²º½ */
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = MAX_CHANNUM, ArraySubType = UnmanagedType.I1)]
            public byte[] byEnablePtzTrack;/* ÊÇ·ñµ÷ÓÃ¹ì¼£ 0-·ñ,1-ÊÇ*/
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = MAX_CHANNUM, ArraySubType = UnmanagedType.I1)]
            public byte[] byPTZTrack;/* µ÷ÓÃµÄÔÆÌ¨µÄ¹ì¼£ÐòºÅ */
        }

        //ÉÏ´«±¨¾¯ÐÅÏ¢(9000À©Õ¹)
        [StructLayoutAttribute(LayoutKind.Sequential)]
        public struct NET_DVR_ALARMINFO_V30
        {
            public int dwAlarmType;/*0-ÐÅºÅÁ¿±¨¾¯,1-Ó²ÅÌÂú,2-ÐÅºÅ¶ªÊ§,3£­ÒÆ¶¯Õì²â,4£­Ó²ÅÌÎ´¸ñÊ½»¯,5-¶ÁÐ´Ó²ÅÌ³ö´í,6-ÕÚµ²±¨¾¯,7-ÖÆÊ½²»Æ¥Åä, 8-·Ç·¨·ÃÎÊ, 0xa-GPS¶¨Î»ÐÅÏ¢(³µÔØ¶¨ÖÆ)*/
            public int dwAlarmInputNumber;/*±¨¾¯ÊäÈë¶Ë¿Ú*/
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = MAX_ALARMOUT_V30, ArraySubType = UnmanagedType.I1)]
            public byte[] byAlarmOutputNumber;/*´¥·¢µÄÊä³ö¶Ë¿Ú£¬Îª1±íÊ¾¶ÔÓ¦Êä³ö*/
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = MAX_CHANNUM_V30, ArraySubType = UnmanagedType.I1)]
            public byte[] byAlarmRelateChannel;/*´¥·¢µÄÂ¼ÏñÍ¨µÀ£¬Îª1±íÊ¾¶ÔÓ¦Â¼Ïñ, dwAlarmRelateChannel[0]¶ÔÓ¦µÚ1¸öÍ¨µÀ*/
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = MAX_CHANNUM_V30, ArraySubType = UnmanagedType.I1)]
            public byte[] byChannel;/*dwAlarmTypeÎª2»ò3,6Ê±£¬±íÊ¾ÄÄ¸öÍ¨µÀ£¬dwChannel[0]¶ÔÓ¦µÚ1¸öÍ¨µÀ*/
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = MAX_DISKNUM_V30, ArraySubType = UnmanagedType.I1)]
            public byte[] byDiskNumber;/*dwAlarmTypeÎª1,4,5Ê±,±íÊ¾ÄÄ¸öÓ²ÅÌ, dwDiskNumber[0]¶ÔÓ¦µÚ1¸öÓ²ÅÌ*/
            public void Init()
            {
                dwAlarmType = 0;
                dwAlarmInputNumber = 0;
                byAlarmRelateChannel = new byte[MAX_CHANNUM_V30];
                byChannel = new byte[MAX_CHANNUM_V30];
                byAlarmOutputNumber = new byte[MAX_ALARMOUT_V30];
                byDiskNumber = new byte[MAX_DISKNUM_V30];
                for (int i = 0; i < MAX_CHANNUM_V30; i++)
                {
                    byAlarmRelateChannel[i] = Convert.ToByte(0);
                    byChannel[i] = Convert.ToByte(0);
                }
                for (int i = 0; i < MAX_ALARMOUT_V30; i++)
                {
                    byAlarmOutputNumber[i] = Convert.ToByte(0);
                }
                for (int i = 0; i < MAX_DISKNUM_V30; i++)
                {
                    byDiskNumber[i] = Convert.ToByte(0);
                }
            }
            public void Reset()
            {
                dwAlarmType = 0;
                dwAlarmInputNumber = 0;

                for (int i = 0; i < MAX_CHANNUM_V30; i++)
                {
                    byAlarmRelateChannel[i] = Convert.ToByte(0);
                    byChannel[i] = Convert.ToByte(0);
                }
                for (int i = 0; i < MAX_ALARMOUT_V30; i++)
                {
                    byAlarmOutputNumber[i] = Convert.ToByte(0);
                }
                for (int i = 0; i < MAX_DISKNUM_V30; i++)
                {
                    byDiskNumber[i] = Convert.ToByte(0);
                }
            }
        }

        //ÐÅºÅÁ¿±¨¾¯
        [StructLayoutAttribute(LayoutKind.Sequential)]
        public struct NET_DVR_ALARMINFO_V40_TRIGER_ALARM
        {
            //±¨¾¯¹Ì¶¨²¿·Ö
            public uint dwAlarmType;//±¨¾¯ÀàÐÍ,½öÎª0-ÐÅºÅÁ¿±¨¾¯
            public NET_DVR_TIME_EX struAlarmTime;   //·¢Éú±¨¾¯µÄÊ±¼ä
            public uint dwAlarmInputNo;     //·¢Éú±¨¾¯µÄ±¨¾¯ÊäÈëÍ¨µÀºÅ£¬Ò»´ÎÖ»ÓÐÒ»¸ö
            public uint dwTrigerAlarmOutNum;    /*´¥·¢µÄ±¨¾¯Êä³ö¸öÊý£¬ÓÃÓÚºóÃæ¼ÆËã±ä³¤Êý¾Ý²¿·ÖÖÐËùÓÐ´¥·¢µÄ±¨¾¯Êä³öÍ¨µÀºÅ£¬ËÄ×Ö½Ú±íÊ¾Ò»¸ö*/
            public uint dwTrigerRecordChanNum;  /*´¥·¢µÄÂ¼ÏñÍ¨µÀ¸öÊý£¬ÓÃÓÚºóÃæ¼ÆËã±ä³¤Êý¾Ý²¿·ÖÖÐËùÓÐ´¥·¢µÄÂ¼ÏñÍ¨µÀºÅ£¬ËÄ×Ö½Ú±íÊ¾Ò»¸ö*/
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 116, ArraySubType = UnmanagedType.I1)]
            public byte[] byRes;/* ±£Áô */
            public IntPtr pAlarmData;   //±¨¾¯¿É±ä²¿·ÖÄÚÈÝ
        }

        //ÒÆ¶¯Õì²âÏà¹Ø±¨¾¯
        [StructLayoutAttribute(LayoutKind.Sequential)]
        public struct NET_DVR_ALARMINFO_V40_CHAN_ALARM
        {
            //±¨¾¯¹Ì¶¨²¿·Ö
            public uint dwAlarmType;//±¨¾¯ÀàÐÍ,2-ÐÅºÅ¶ªÊ§£¬3£­ÒÆ¶¯Õì²â,6-ÕÚµ²±¨¾¯£¬9-ÊÓÆµÐÅºÅÒì³££¬10-Â¼ÏñÒì³££¬13-Ç°¶Ë/Â¼Ïñ·Ö±æÂÊ²»Æ¥Åä
            public NET_DVR_TIME_EX struAlarmTime;   //·¢Éú±¨¾¯µÄÊ±¼ä
            public uint dwAlarmChanNum; /*·¢Éú±¨¾¯Í¨µÀÊý¾Ý¸öÊý£¬ÓÃÓÚºóÃæ¼ÆËã±ä³¤Êý¾Ý²¿·ÖÖÐËùÓÐ·¢ÉúµÄ±¨¾¯Í¨µÀºÅ£¬ËÄ×Ö½Ú±íÊ¾Ò»¸ö*/
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 124, ArraySubType = UnmanagedType.I1)]
            public byte[] byRes;/* ±£Áô */
            public IntPtr pAlarmData;   //±¨¾¯¿É±ä²¿·ÖÄÚÈÝ
        }

        //Ó²ÅÌÏà¹Ø±¨¾¯
        [StructLayoutAttribute(LayoutKind.Sequential)]
        public struct NET_DVR_ALARMINFO_V40_HARDDISK_ALARM
        {
            //±¨¾¯¹Ì¶¨²¿·Ö
            public uint dwAlarmType;//±¨¾¯ÀàÐÍ,1-Ó²ÅÌÂú£¬4£­Ó²ÅÌÎ´¸ñÊ½»¯,5-Ð´Ó²ÅÌ³ö´í,
            public NET_DVR_TIME_EX struAlarmTime;   //·¢Éú±¨¾¯µÄÊ±¼ä
            public uint dwAlarmHardDiskNum; /*·¢Éú±¨¾¯µÄÓ²ÅÌÊý¾Ý³¤¶È£¬ÓÃÓÚºóÃæ¼ÆËã±ä³¤Êý¾Ý²¿·ÖÖÐËùÓÐ·¢Éú±¨¾¯µÄÓ²ÅÌºÅ£¬ËÄ½Ú±íÊ¾Ò»¸ö*/
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 124, ArraySubType = UnmanagedType.I1)]
            public byte[] byRes;/* ±£Áô */
            public IntPtr pAlarmData;   //±¨¾¯¿É±ä²¿·ÖÄÚÈÝ
        }

        //Â¼²¥Ö÷»úÏà¹Ø±¨¾¯
        [StructLayoutAttribute(LayoutKind.Sequential)]
        public struct NET_DVR_ALARMINFO_V40_RECORDHOST_ALARM
        {
            //±¨¾¯¹Ì¶¨²¿·Ö
            public uint dwAlarmType;//±¨¾¯ÀàÐÍ,17-Â¼²¥Ö÷»ú±¨¾¯,
            public NET_DVR_TIME_EX struAlarmTime;   //·¢Éú±¨¾¯µÄÊ±¼ä
            public byte bySubAlarmType;  //±¨¾¯×ÓÀàÐÍ£¬1-Ò»¼üÑÓ³ÙÂ¼Ïñ£» 
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 3, ArraySubType = UnmanagedType.I1)]
            public byte[] byRes1;/* ±£Áô */
            public NET_DVR_TIME_EX struRecordEndTime; //Â¼²¥½áÊøÊ±¼ä
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 116, ArraySubType = UnmanagedType.I1)]
            public byte[] byRes;/* ±£Áô */
            public IntPtr pAlarmData;   //±¨¾¯¿É±ä²¿·ÖÄÚÈÝ
        }

        //ÆäËû±¨¾¯
        [StructLayoutAttribute(LayoutKind.Sequential)]
        public struct NET_DVR_ALARMINFO_V40_OTHER_ALARM
        {
            //±¨¾¯¹Ì¶¨²¿·Ö
            public uint dwAlarmType;//±¨¾¯ÀàÐÍ,7-ÖÆÊ½²»Æ¥Åä, 8-·Ç·¨·ÃÎÊ£¬11-ÖÇÄÜ³¡¾°±ä»¯£¬12-ÕóÁÐÒì³££¬14-ÉêÇë½âÂë×ÊÔ´Ê§°Ü,15-ÖÇÄÜÕì²â±¨¾¯, 16-ÈÈ±¸Òì³££¬18-ÓïÒô¶Ô½²ÇëÇó±¨¾¯
            public NET_DVR_TIME_EX struAlarmTime;   //·¢Éú±¨¾¯µÄÊ±¼ä
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 128, ArraySubType = UnmanagedType.I1)]
            public byte[] byRes;/* ±£Áô */
            public IntPtr pAlarmData;   //±¨¾¯¿É±ä²¿·ÖÄÚÈÝ
        }

        [StructLayoutAttribute(LayoutKind.Sequential)]
        public struct NET_DVR_ALARMINFO
        {
            public int dwAlarmType;/*0-ÐÅºÅÁ¿±¨¾¯,1-Ó²ÅÌÂú,2-ÐÅºÅ¶ªÊ§,3£­ÒÆ¶¯Õì²â,4£­Ó²ÅÌÎ´¸ñÊ½»¯,5-¶ÁÐ´Ó²ÅÌ³ö´í,6-ÕÚµ²±¨¾¯,7-ÖÆÊ½²»Æ¥Åä, 8-·Ç·¨·ÃÎÊ, 9-´®¿Ú×´Ì¬, 0xa-GPS¶¨Î»ÐÅÏ¢(³µÔØ¶¨ÖÆ)*/
            public int dwAlarmInputNumber;/*±¨¾¯ÊäÈë¶Ë¿Ú, µ±±¨¾¯ÀàÐÍÎª9Ê±¸Ã±äÁ¿±íÊ¾´®¿Ú×´Ì¬0±íÊ¾Õý³££¬ -1±íÊ¾´íÎó*/
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = MAX_ALARMOUT, ArraySubType = UnmanagedType.U4)]
            public int[] dwAlarmOutputNumber;/*´¥·¢µÄÊä³ö¶Ë¿Ú£¬ÄÄÒ»Î»Îª1±íÊ¾¶ÔÓ¦ÄÄÒ»¸öÊä³ö*/
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = MAX_CHANNUM, ArraySubType = UnmanagedType.U4)]
            public int[] dwAlarmRelateChannel;/*´¥·¢µÄÂ¼ÏñÍ¨µÀ£¬ÄÄÒ»Î»Îª1±íÊ¾¶ÔÓ¦ÄÄÒ»Â·Â¼Ïñ, dwAlarmRelateChannel[0]¶ÔÓ¦µÚ1¸öÍ¨µÀ*/
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = MAX_CHANNUM, ArraySubType = UnmanagedType.U4)]
            public int[] dwChannel;/*dwAlarmTypeÎª2»ò3,6Ê±£¬±íÊ¾ÄÄ¸öÍ¨µÀ£¬dwChannel[0]Î»¶ÔÓ¦µÚ1¸öÍ¨µÀ*/
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = MAX_DISKNUM, ArraySubType = UnmanagedType.U4)]
            public int[] dwDiskNumber;/*dwAlarmTypeÎª1,4,5Ê±,±íÊ¾ÄÄ¸öÓ²ÅÌ, dwDiskNumber[0]Î»¶ÔÓ¦µÚ1¸öÓ²ÅÌ*/
            public void Init()
            {
                dwAlarmType = 0;
                dwAlarmInputNumber = 0;
                dwAlarmOutputNumber = new int[MAX_ALARMOUT];
                dwAlarmRelateChannel = new int[MAX_CHANNUM];
                dwChannel = new int[MAX_CHANNUM];
                dwDiskNumber = new int[MAX_DISKNUM];
                for (int i = 0; i < MAX_ALARMOUT; i++)
                {
                    dwAlarmOutputNumber[i] = 0;
                }
                for (int i = 0; i < MAX_CHANNUM; i++)
                {
                    dwAlarmRelateChannel[i] = 0;
                    dwChannel[i] = 0;
                }
                for (int i = 0; i < MAX_DISKNUM; i++)
                {
                    dwDiskNumber[i] = 0;
                }
            }
            public void Reset()
            {
                dwAlarmType = 0;
                dwAlarmInputNumber = 0;

                for (int i = 0; i < MAX_ALARMOUT; i++)
                {
                    dwAlarmOutputNumber[i] = 0;
                }
                for (int i = 0; i < MAX_CHANNUM; i++)
                {
                    dwAlarmRelateChannel[i] = 0;
                    dwChannel[i] = 0;
                }
                for (int i = 0; i < MAX_DISKNUM; i++)
                {
                    dwDiskNumber[i] = 0;
                }
            }
        }


        //////////////////////////////////////////////////////////////////////////////////////
        //IPC½ÓÈë²ÎÊýÅäÖÃ
        /* IPÉè±¸½á¹¹ */
        [StructLayoutAttribute(LayoutKind.Sequential)]
        public struct NET_DVR_IPDEVINFO
        {
            public uint dwEnable;/* ¸ÃIPÉè±¸ÊÇ·ñÆôÓÃ */
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = NAME_LEN, ArraySubType = UnmanagedType.I1)]
            public byte[] sUserName;/* ÓÃ»§Ãû */
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = PASSWD_LEN, ArraySubType = UnmanagedType.I1)]
            public byte[] sPassword; /* ÃÜÂë */
            public NET_DVR_IPADDR struIP;/* IPµØÖ· */
            public ushort wDVRPort;/* ¶Ë¿ÚºÅ */
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 34, ArraySubType = UnmanagedType.I1)]
            public byte[] byRes;/* ±£Áô */

            public void Init()
            {
                sUserName = new byte[NAME_LEN];
                sPassword = new byte[PASSWD_LEN];
                byRes = new byte[34];
            }
        }

        //ipc½ÓÈëÉè±¸ÐÅÏ¢À©Õ¹£¬Ö§³ÖipÉè±¸µÄÓòÃûÌí¼Ó
        [StructLayoutAttribute(LayoutKind.Sequential)]
        public struct NET_DVR_IPDEVINFO_V31
        {
            public byte byEnable;//¸ÃIPÉè±¸ÊÇ·ñÓÐÐ§
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 3, ArraySubType = UnmanagedType.I1)]
            public byte[] byRes1;//±£Áô×Ö¶Î£¬ÖÃ0
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = NAME_LEN, ArraySubType = UnmanagedType.I1)]
            public byte[] sUserName;//ÓÃ»§Ãû
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = PASSWD_LEN, ArraySubType = UnmanagedType.I1)]
            public byte[] sPassword;//ÃÜÂë
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = MAX_DOMAIN_NAME, ArraySubType = UnmanagedType.I1)]
            public byte[] byDomain;//Éè±¸ÓòÃû
            public NET_DVR_IPADDR struIP;//IPµØÖ·
            public ushort wDVRPort;// ¶Ë¿ÚºÅ
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 34, ArraySubType = UnmanagedType.I1)]
            public byte[] byRes2;//±£Áô×Ö¶Î£¬ÖÃ0
        }

        /* IPÍ¨µÀÆ¥Åä²ÎÊý */
        [StructLayoutAttribute(LayoutKind.Sequential)]
        public struct NET_DVR_IPCHANINFO
        {
            public byte byEnable;/* ¸ÃÍ¨µÀÊÇ·ñÔÚÏß */
            public byte byIPID;/* IPÉè±¸ID È¡Öµ1- MAX_IP_DEVICE */
            public byte byChannel;/* Í¨µÀºÅ */
            public byte byIPIDHigh;                   // IPÉè±¸IDµÄ¸ß8Î»
            public byte byProType;//Ð­ÒéÀàÐÍ£¬0-º£¿µÐ­Òé(default)£¬1-ËÉÏÂÐ­Òé£¬2-Ë÷Äá
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 31, ArraySubType = UnmanagedType.I1)]
            public byte[] byRes;//±£Áô,ÖÃ0
        }

        /* IP½ÓÈëÅäÖÃ½á¹¹ */
        [StructLayoutAttribute(LayoutKind.Sequential)]
        public struct NET_DVR_IPPARACFG
        {
            public uint dwSize;/* ½á¹¹´óÐ¡ */
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = MAX_IP_DEVICE, ArraySubType = UnmanagedType.Struct)]
            public NET_DVR_IPDEVINFO[] struIPDevInfo;/* IPÉè±¸ */
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = MAX_ANALOG_CHANNUM, ArraySubType = UnmanagedType.I1)]
            public byte[] byAnalogChanEnable; /* Ä£ÄâÍ¨µÀÊÇ·ñÆôÓÃ£¬´ÓµÍµ½¸ß±íÊ¾1-32Í¨µÀ£¬0±íÊ¾ÎÞÐ§ 1ÓÐÐ§ */
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = MAX_IP_CHANNEL, ArraySubType = UnmanagedType.Struct)]
            public NET_DVR_IPCHANINFO[] struIPChanInfo;/* IPÍ¨µÀ */
        }

        /* À©Õ¹IP½ÓÈëÅäÖÃ½á¹¹ */
        [StructLayoutAttribute(LayoutKind.Sequential)]
        public struct NET_DVR_IPPARACFG_V31
        {
            public uint dwSize;/* ½á¹¹´óÐ¡ */
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = MAX_IP_DEVICE, ArraySubType = UnmanagedType.Struct)]
            public NET_DVR_IPDEVINFO_V31[] struIPDevInfo; /* IPÉè±¸ */
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = MAX_ANALOG_CHANNUM, ArraySubType = UnmanagedType.I1)]
            public byte[] byAnalogChanEnable; /* Ä£ÄâÍ¨µÀÊÇ·ñÆôÓÃ£¬´ÓµÍµ½¸ß±íÊ¾1-32Í¨µÀ£¬0±íÊ¾ÎÞÐ§ 1ÓÐÐ§ */
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = MAX_IP_CHANNEL, ArraySubType = UnmanagedType.Struct)]
            public NET_DVR_IPCHANINFO[] struIPChanInfo;/* IPÍ¨µÀ */
        }

        /* ±¨¾¯Êä³ö²ÎÊý */
        [StructLayoutAttribute(LayoutKind.Sequential)]
        public struct NET_DVR_IPALARMOUTINFO
        {
            public byte byIPID;/* IPÉè±¸IDÈ¡Öµ1- MAX_IP_DEVICE */
            public byte byAlarmOut;/* ±¨¾¯Êä³öºÅ */
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 18, ArraySubType = UnmanagedType.I1)]
            public byte[] byRes;/* ±£Áô */

            public void Init()
            {
                byRes = new byte[18];
            }
        }

        /* IP±¨¾¯Êä³öÅäÖÃ½á¹¹ */
        [StructLayoutAttribute(LayoutKind.Sequential)]
        public struct NET_DVR_IPALARMOUTCFG
        {
            public uint dwSize; /* ½á¹¹´óÐ¡ */
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = MAX_IP_ALARMOUT, ArraySubType = UnmanagedType.Struct)]
            public NET_DVR_IPALARMOUTINFO[] struIPAlarmOutInfo;/* IP±¨¾¯Êä³ö */

            public void Init()
            {
                struIPAlarmOutInfo = new NET_DVR_IPALARMOUTINFO[MAX_IP_ALARMOUT];
                for (int i = 0; i < MAX_IP_ALARMOUT; i++)
                {
                    struIPAlarmOutInfo[i].Init();
                }
            }
        }

        /* ±¨¾¯ÊäÈë²ÎÊý */
        [StructLayoutAttribute(LayoutKind.Sequential)]
        public struct NET_DVR_IPALARMININFO
        {
            public byte byIPID;/* IPÉè±¸IDÈ¡Öµ1- MAX_IP_DEVICE */
            public byte byAlarmIn;/* ±¨¾¯ÊäÈëºÅ */
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 18, ArraySubType = UnmanagedType.I1)]
            public byte[] byRes;/* ±£Áô */
        }

        /* IP±¨¾¯ÊäÈëÅäÖÃ½á¹¹ */
        [StructLayoutAttribute(LayoutKind.Sequential)]
        public struct NET_DVR_IPALARMINCFG
        {
            public uint dwSize;/* ½á¹¹´óÐ¡ */
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = MAX_IP_ALARMIN, ArraySubType = UnmanagedType.Struct)]
            public NET_DVR_IPALARMININFO[] struIPAlarmInInfo;/* IP±¨¾¯ÊäÈë */
        }

        //ipc alarm info
        [StructLayoutAttribute(LayoutKind.Sequential)]
        public struct tagNET_DVR_IPALARMINFO
        {
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = MAX_IP_DEVICE, ArraySubType = UnmanagedType.Struct)]
            public NET_DVR_IPDEVINFO[] struIPDevInfo; /* IPÉè±¸ */
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = MAX_ANALOG_CHANNUM, ArraySubType = UnmanagedType.I1)]
            public byte[] byAnalogChanEnable; /* Ä£ÄâÍ¨µÀÊÇ·ñÆôÓÃ£¬0-Î´ÆôÓÃ 1-ÆôÓÃ */
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = MAX_IP_CHANNEL, ArraySubType = UnmanagedType.Struct)]
            public NET_DVR_IPCHANINFO[] struIPChanInfo;/* IPÍ¨µÀ */
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = MAX_IP_ALARMIN, ArraySubType = UnmanagedType.Struct)]
            public NET_DVR_IPALARMININFO[] struIPAlarmInInfo;/* IP±¨¾¯ÊäÈë */
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = MAX_IP_ALARMOUT, ArraySubType = UnmanagedType.Struct)]
            public NET_DVR_IPALARMOUTINFO[] struIPAlarmOutInfo;/* IP±¨¾¯Êä³ö */
        }

        //ipcÅäÖÃ¸Ä±ä±¨¾¯ÐÅÏ¢À©Õ¹ 9000_1.1
        [StructLayoutAttribute(LayoutKind.Sequential)]
        public struct tagNET_DVR_IPALARMINFO_V31
        {
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = MAX_IP_DEVICE, ArraySubType = UnmanagedType.Struct)]
            public NET_DVR_IPDEVINFO_V31[] struIPDevInfo; /* IPÉè±¸ */
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = MAX_ANALOG_CHANNUM, ArraySubType = UnmanagedType.I1)]
            public byte[] byAnalogChanEnable;/* Ä£ÄâÍ¨µÀÊÇ·ñÆôÓÃ£¬0-Î´ÆôÓÃ 1-ÆôÓÃ */
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = MAX_IP_CHANNEL, ArraySubType = UnmanagedType.Struct)]
            public NET_DVR_IPCHANINFO[] struIPChanInfo;/* IPÍ¨µÀ */
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = MAX_IP_ALARMIN, ArraySubType = UnmanagedType.Struct)]
            public NET_DVR_IPALARMININFO[] struIPAlarmInInfo; /* IP±¨¾¯ÊäÈë */
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = MAX_IP_ALARMOUT, ArraySubType = UnmanagedType.Struct)]
            public NET_DVR_IPALARMOUTINFO[] struIPAlarmOutInfo;/* IP±¨¾¯Êä³ö */
        }

        public enum HD_STAT
        {
            HD_STAT_OK = 0,/* Õý³£ */
            HD_STAT_UNFORMATTED = 1,/* Î´¸ñÊ½»¯ */
            HD_STAT_ERROR = 2,/* ´íÎó */
            HD_STAT_SMART_FAILED = 3,/* SMART×´Ì¬ */
            HD_STAT_MISMATCH = 4,/* ²»Æ¥Åä */
            HD_STAT_IDLE = 5, /* ÐÝÃß*/
            NET_HD_STAT_OFFLINE = 6,/*ÍøÂçÅÌ´¦ÓÚÎ´Á¬½Ó×´Ì¬ */
        }

        //±¾µØÓ²ÅÌÐÅÏ¢ÅäÖÃ
        [StructLayoutAttribute(LayoutKind.Sequential)]
        public struct NET_DVR_SINGLE_HD
        {
            public uint dwHDNo;/*Ó²ÅÌºÅ, È¡Öµ0~MAX_DISKNUM_V30-1*/
            public uint dwCapacity;/*Ó²ÅÌÈÝÁ¿(²»¿ÉÉèÖÃ)*/
            public uint dwFreeSpace;/*Ó²ÅÌÊ£Óà¿Õ¼ä(²»¿ÉÉèÖÃ)*/
            public uint dwHdStatus;/*Ó²ÅÌ×´Ì¬(²»¿ÉÉèÖÃ) HD_STAT*/
            public byte byHDAttr;/*0-Ä¬ÈÏ, 1-ÈßÓà; 2-Ö»¶Á*/
            public byte byHDType;/*0-±¾µØÓ²ÅÌ,1-ESATAÓ²ÅÌ,2-NASÓ²ÅÌ*/
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 2, ArraySubType = UnmanagedType.I1)]
            public byte[] byRes1;
            public uint dwHdGroup; /*ÊôÓÚÄÄ¸öÅÌ×é 1-MAX_HD_GROUP*/
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 120, ArraySubType = UnmanagedType.I1)]
            public byte[] byRes2;
        }

        [StructLayoutAttribute(LayoutKind.Sequential)]
        public struct NET_DVR_HDCFG
        {
            public uint dwSize;
            public uint dwHDCount;/*Ó²ÅÌÊý(²»¿ÉÉèÖÃ)*/
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = MAX_DISKNUM_V30, ArraySubType = UnmanagedType.Struct)]
            public NET_DVR_SINGLE_HD[] struHDInfo;//Ó²ÅÌÏà¹Ø²Ù×÷¶¼ÐèÒªÖØÆô²ÅÄÜÉúÐ§£»
        }

        //±¾µØÅÌ×éÐÅÏ¢ÅäÖÃ
        [StructLayoutAttribute(LayoutKind.Sequential)]
        public struct NET_DVR_SINGLE_HDGROUP
        {
            public uint dwHDGroupNo;/*ÅÌ×éºÅ(²»¿ÉÉèÖÃ) 1-MAX_HD_GROUP*/
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 64, ArraySubType = UnmanagedType.I1)]
            public byte[] byHDGroupChans;/*ÅÌ×é¶ÔÓ¦µÄÂ¼ÏñÍ¨µÀ, 0-±íÊ¾¸ÃÍ¨µÀ²»Â¼Ïóµ½¸ÃÅÌ×é£¬1-±íÊ¾Â¼Ïóµ½¸ÃÅÌ×é*/
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 8, ArraySubType = UnmanagedType.I1)]
            public byte[] byRes;
        }

        [StructLayoutAttribute(LayoutKind.Sequential)]
        public struct NET_DVR_HDGROUP_CFG
        {
            public uint dwSize;
            public uint dwHDGroupCount;/*ÅÌ×é×ÜÊý(²»¿ÉÉèÖÃ)*/
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = MAX_HD_GROUP, ArraySubType = UnmanagedType.Struct)]
            public NET_DVR_SINGLE_HDGROUP[] struHDGroupAttr;//Ó²ÅÌÏà¹Ø²Ù×÷¶¼ÐèÒªÖØÆô²ÅÄÜÉúÐ§
        }

        //ÅäÖÃËõ·Å²ÎÊýµÄ½á¹¹
        [StructLayoutAttribute(LayoutKind.Sequential)]
        public struct NET_DVR_SCALECFG
        {
            public uint dwSize;
            public uint dwMajorScale;/* Ö÷ÏÔÊ¾ 0-²»Ëõ·Å£¬1-Ëõ·Å*/
            public uint dwMinorScale;/* ¸¨ÏÔÊ¾ 0-²»Ëõ·Å£¬1-Ëõ·Å*/
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 2, ArraySubType = UnmanagedType.U4)]
            public uint[] dwRes;
        }

        //DVR±¨¾¯Êä³ö(9000À©Õ¹)
        [StructLayoutAttribute(LayoutKind.Sequential)]
        public struct NET_DVR_ALARMOUTCFG_V30
        {
            public uint dwSize;
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = NAME_LEN, ArraySubType = UnmanagedType.I1)]
            public byte[] sAlarmOutName;/* Ãû³Æ */
            public uint dwAlarmOutDelay;/* Êä³ö±£³ÖÊ±¼ä(-1ÎªÎÞÏÞ£¬ÊÖ¶¯¹Ø±Õ) */
            //0-5Ãë,1-10Ãë,2-30Ãë,3-1·ÖÖÓ,4-2·ÖÖÓ,5-5·ÖÖÓ,6-10·ÖÖÓ,7-ÊÖ¶¯
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = MAX_DAYS * MAX_TIMESEGMENT_V30, ArraySubType = UnmanagedType.Struct)]
            public NET_DVR_SCHEDTIME[] struAlarmOutTime;/* ±¨¾¯Êä³ö¼¤»îÊ±¼ä¶Î */
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 16, ArraySubType = UnmanagedType.I1)]
            public byte[] byRes;
        }

        //DVR±¨¾¯Êä³ö
        [StructLayoutAttribute(LayoutKind.Sequential)]
        public struct NET_DVR_ALARMOUTCFG
        {
            public uint dwSize;
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = NAME_LEN, ArraySubType = UnmanagedType.I1)]
            public byte[] sAlarmOutName;/* Ãû³Æ */
            public uint dwAlarmOutDelay;/* Êä³ö±£³ÖÊ±¼ä(-1ÎªÎÞÏÞ£¬ÊÖ¶¯¹Ø±Õ) */
            //0-5Ãë,1-10Ãë,2-30Ãë,3-1·ÖÖÓ,4-2·ÖÖÓ,5-5·ÖÖÓ,6-10·ÖÖÓ,7-ÊÖ¶¯
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = MAX_DAYS * MAX_TIMESEGMENT, ArraySubType = UnmanagedType.Struct)]
            public NET_DVR_SCHEDTIME[] struAlarmOutTime;/* ±¨¾¯Êä³ö¼¤»îÊ±¼ä¶Î */
        }

        //DVR±¾µØÔ¤ÀÀ²ÎÊý(9000À©Õ¹)
        [StructLayoutAttribute(LayoutKind.Sequential)]
        public struct NET_DVR_PREVIEWCFG_V30
        {
            public uint dwSize;
            public byte byPreviewNumber;//Ô¤ÀÀÊýÄ¿,0-1»­Ãæ,1-4»­Ãæ,2-9»­Ãæ,3-16»­Ãæ,0xff:×î´ó»­Ãæ
            public byte byEnableAudio;//ÊÇ·ñÉùÒôÔ¤ÀÀ,0-²»Ô¤ÀÀ,1-Ô¤ÀÀ
            public ushort wSwitchTime;//ÇÐ»»Ê±¼ä,0-²»ÇÐ»»,1-5s,2-10s,3-20s,4-30s,5-60s,6-120s,7-300s
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = MAX_PREVIEW_MODE * MAX_WINDOW_V30, ArraySubType = UnmanagedType.I1)]
            public byte[] bySwitchSeq;//ÇÐ»»Ë³Ðò,Èç¹ûlSwitchSeq[i]Îª 0xff±íÊ¾²»ÓÃ
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 24, ArraySubType = UnmanagedType.I1)]
            public byte[] byRes;
        }

        //DVR±¾µØÔ¤ÀÀ²ÎÊý
        [StructLayoutAttribute(LayoutKind.Sequential)]
        public struct NET_DVR_PREVIEWCFG
        {
            public uint dwSize;
            public byte byPreviewNumber;//Ô¤ÀÀÊýÄ¿,0-1»­Ãæ,1-4»­Ãæ,2-9»­Ãæ,3-16»­Ãæ,0xff:×î´ó»­Ãæ
            public byte byEnableAudio;//ÊÇ·ñÉùÒôÔ¤ÀÀ,0-²»Ô¤ÀÀ,1-Ô¤ÀÀ
            public ushort wSwitchTime;//ÇÐ»»Ê±¼ä,0-²»ÇÐ»»,1-5s,2-10s,3-20s,4-30s,5-60s,6-120s,7-300s
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = MAX_WINDOW, ArraySubType = UnmanagedType.I1)]
            public byte[] bySwitchSeq;//ÇÐ»»Ë³Ðò,Èç¹ûlSwitchSeq[i]Îª 0xff±íÊ¾²»ÓÃ
        }

        //DVRÊÓÆµÊä³ö
        [StructLayoutAttribute(LayoutKind.Sequential)]
        public struct NET_DVR_VGAPARA
        {
            public ushort wResolution;/* ·Ö±æÂÊ */
            public ushort wFreq;/* Ë¢ÐÂÆµÂÊ */
            public uint dwBrightness;/* ÁÁ¶È */
        }

        //MATRIXÊä³ö²ÎÊý½á¹¹
        [StructLayoutAttribute(LayoutKind.Sequential)]
        public struct NET_DVR_MATRIXPARA_V30
        {
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = MAX_ANALOG_CHANNUM, ArraySubType = UnmanagedType.U2)]
            public ushort[] wOrder;/* Ô¤ÀÀË³Ðò, 0xff±íÊ¾ÏàÓ¦µÄ´°¿Ú²»Ô¤ÀÀ */
            public ushort wSwitchTime;// Ô¤ÀÀÇÐ»»Ê±¼ä 
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 14, ArraySubType = UnmanagedType.I1)]
            public byte[] res;
        }

        [StructLayoutAttribute(LayoutKind.Sequential)]
        public struct NET_DVR_MATRIXPARA
        {
            public ushort wDisplayLogo;/* ÏÔÊ¾ÊÓÆµÍ¨µÀºÅ */
            public ushort wDisplayOsd;/* ÏÔÊ¾Ê±¼ä */
        }

        [StructLayoutAttribute(LayoutKind.Sequential)]
        public struct NET_DVR_VOOUT
        {
            public byte byVideoFormat;/* Êä³öÖÆÊ½,0-PAL,1-NTSC */
            public byte byMenuAlphaValue;/* ²Ëµ¥Óë±³¾°Í¼Ïó¶Ô±È¶È */
            public ushort wScreenSaveTime;/* ÆÁÄ»±£»¤Ê±¼ä 0-´Ó²»,1-1·ÖÖÓ,2-2·ÖÖÓ,3-5·ÖÖÓ,4-10·ÖÖÓ,5-20·ÖÖÓ,6-30·ÖÖÓ */
            public ushort wVOffset;/* ÊÓÆµÊä³öÆ«ÒÆ */
            public ushort wBrightness;/* ÊÓÆµÊä³öÁÁ¶È */
            public byte byStartMode;/* Æô¶¯ºóÊÓÆµÊä³öÄ£Ê½(0:²Ëµ¥,1:Ô¤ÀÀ)*/
            public byte byEnableScaler;/* ÊÇ·ñÆô¶¯Ëõ·Å (0-²»Æô¶¯, 1-Æô¶¯)*/
        }

        //DVRÊÓÆµÊä³ö(9000À©Õ¹)
        [StructLayoutAttribute(LayoutKind.Sequential)]
        public struct NET_DVR_VIDEOOUT_V30
        {
            public uint dwSize;
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = MAX_VIDEOOUT_V30, ArraySubType = UnmanagedType.Struct)]
            public NET_DVR_VOOUT[] struVOOut;
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = MAX_VGA_V30, ArraySubType = UnmanagedType.Struct)]
            public NET_DVR_VGAPARA[] struVGAPara;/* VGA²ÎÊý */
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = MAX_MATRIXOUT, ArraySubType = UnmanagedType.Struct)]
            public NET_DVR_MATRIXPARA_V30[] struMatrixPara;/* MATRIX²ÎÊý */
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 16, ArraySubType = UnmanagedType.I1)]
            public byte[] byRes;
        }

        //DVRÊÓÆµÊä³ö
        [StructLayoutAttribute(LayoutKind.Sequential)]
        public struct NET_DVR_VIDEOOUT
        {
            public uint dwSize;
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = MAX_VIDEOOUT, ArraySubType = UnmanagedType.Struct)]
            public NET_DVR_VOOUT[] struVOOut;
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = MAX_VGA, ArraySubType = UnmanagedType.Struct)]
            public NET_DVR_VGAPARA[] struVGAPara;   /* VGA²ÎÊý */
            public NET_DVR_MATRIXPARA struMatrixPara;/* MATRIX²ÎÊý */
        }

        //µ¥ÓÃ»§²ÎÊý(×Ó½á¹¹)(9000À©Õ¹)
        [StructLayoutAttribute(LayoutKind.Sequential)]
        public struct NET_DVR_USER_INFO_V30
        {
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = NAME_LEN, ArraySubType = UnmanagedType.I1)]
            public byte[] sUserName;/* ÓÃ»§Ãû */
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = PASSWD_LEN, ArraySubType = UnmanagedType.I1)]
            public byte[] sPassword;/* ÃÜÂë */
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = MAX_RIGHT, ArraySubType = UnmanagedType.I1)]
            public byte[] byLocalRight;/* ±¾µØÈ¨ÏÞ */
            /*Êý×é0: ±¾µØ¿ØÖÆÔÆÌ¨*/
            /*Êý×é1: ±¾µØÊÖ¶¯Â¼Ïó*/
            /*Êý×é2: ±¾µØ»Ø·Å*/
            /*Êý×é3: ±¾µØÉèÖÃ²ÎÊý*/
            /*Êý×é4: ±¾µØ²é¿´×´Ì¬¡¢ÈÕÖ¾*/
            /*Êý×é5: ±¾µØ¸ß¼¶²Ù×÷(Éý¼¶£¬¸ñÊ½»¯£¬ÖØÆô£¬¹Ø»ú)*/
            /*Êý×é6: ±¾µØ²é¿´²ÎÊý */
            /*Êý×é7: ±¾µØ¹ÜÀíÄ£ÄâºÍIP camera */
            /*Êý×é8: ±¾µØ±¸·Ý */
            /*Êý×é9: ±¾µØ¹Ø»ú/ÖØÆô */
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = MAX_RIGHT, ArraySubType = UnmanagedType.I1)]
            public byte[] byRemoteRight;/* Ô¶³ÌÈ¨ÏÞ */
            /*Êý×é0: Ô¶³Ì¿ØÖÆÔÆÌ¨*/
            /*Êý×é1: Ô¶³ÌÊÖ¶¯Â¼Ïó*/
            /*Êý×é2: Ô¶³Ì»Ø·Å */
            /*Êý×é3: Ô¶³ÌÉèÖÃ²ÎÊý*/
            /*Êý×é4: Ô¶³Ì²é¿´×´Ì¬¡¢ÈÕÖ¾*/
            /*Êý×é5: Ô¶³Ì¸ß¼¶²Ù×÷(Éý¼¶£¬¸ñÊ½»¯£¬ÖØÆô£¬¹Ø»ú)*/
            /*Êý×é6: Ô¶³Ì·¢ÆðÓïÒô¶Ô½²*/
            /*Êý×é7: Ô¶³ÌÔ¤ÀÀ*/
            /*Êý×é8: Ô¶³ÌÇëÇó±¨¾¯ÉÏ´«¡¢±¨¾¯Êä³ö*/
            /*Êý×é9: Ô¶³Ì¿ØÖÆ£¬±¾µØÊä³ö*/
            /*Êý×é10: Ô¶³Ì¿ØÖÆ´®¿Ú*/
            /*Êý×é11: Ô¶³Ì²é¿´²ÎÊý */
            /*Êý×é12: Ô¶³Ì¹ÜÀíÄ£ÄâºÍIP camera */
            /*Êý×é13: Ô¶³Ì¹Ø»ú/ÖØÆô */
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = MAX_CHANNUM_V30, ArraySubType = UnmanagedType.I1)]
            public byte[] byNetPreviewRight;/* Ô¶³Ì¿ÉÒÔÔ¤ÀÀµÄÍ¨µÀ 0-ÓÐÈ¨ÏÞ£¬1-ÎÞÈ¨ÏÞ*/
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = MAX_CHANNUM_V30, ArraySubType = UnmanagedType.I1)]
            public byte[] byLocalPlaybackRight;/* ±¾µØ¿ÉÒÔ»Ø·ÅµÄÍ¨µÀ 0-ÓÐÈ¨ÏÞ£¬1-ÎÞÈ¨ÏÞ*/
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = MAX_CHANNUM_V30, ArraySubType = UnmanagedType.I1)]
            public byte[] byNetPlaybackRight;/* Ô¶³Ì¿ÉÒÔ»Ø·ÅµÄÍ¨µÀ 0-ÓÐÈ¨ÏÞ£¬1-ÎÞÈ¨ÏÞ*/
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = MAX_CHANNUM_V30, ArraySubType = UnmanagedType.I1)]
            public byte[] byLocalRecordRight;/* ±¾µØ¿ÉÒÔÂ¼ÏñµÄÍ¨µÀ 0-ÓÐÈ¨ÏÞ£¬1-ÎÞÈ¨ÏÞ*/
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = MAX_CHANNUM_V30, ArraySubType = UnmanagedType.I1)]
            public byte[] byNetRecordRight;/* Ô¶³Ì¿ÉÒÔÂ¼ÏñµÄÍ¨µÀ 0-ÓÐÈ¨ÏÞ£¬1-ÎÞÈ¨ÏÞ*/
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = MAX_CHANNUM_V30, ArraySubType = UnmanagedType.I1)]
            public byte[] byLocalPTZRight;/* ±¾µØ¿ÉÒÔPTZµÄÍ¨µÀ 0-ÓÐÈ¨ÏÞ£¬1-ÎÞÈ¨ÏÞ*/
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = MAX_CHANNUM_V30, ArraySubType = UnmanagedType.I1)]
            public byte[] byNetPTZRight;/* Ô¶³Ì¿ÉÒÔPTZµÄÍ¨µÀ 0-ÓÐÈ¨ÏÞ£¬1-ÎÞÈ¨ÏÞ*/
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = MAX_CHANNUM_V30, ArraySubType = UnmanagedType.I1)]
            public byte[] byLocalBackupRight;/* ±¾µØ±¸·ÝÈ¨ÏÞÍ¨µÀ 0-ÓÐÈ¨ÏÞ£¬1-ÎÞÈ¨ÏÞ*/
            public NET_DVR_IPADDR struUserIP;/* ÓÃ»§IPµØÖ·(Îª0Ê±±íÊ¾ÔÊÐíÈÎºÎµØÖ·) */
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = MACADDR_LEN, ArraySubType = UnmanagedType.I1)]
            public byte[] byMACAddr;/* ÎïÀíµØÖ· */
            public byte byPriority;/* ÓÅÏÈ¼¶£¬0xff-ÎÞ£¬0--µÍ£¬1--ÖÐ£¬2--¸ß */
            /*
            ÎÞ¡­¡­±íÊ¾²»Ö§³ÖÓÅÏÈ¼¶µÄÉèÖÃ
            µÍ¡­¡­Ä¬ÈÏÈ¨ÏÞ:°üÀ¨±¾µØºÍÔ¶³Ì»Ø·Å,±¾µØºÍÔ¶³Ì²é¿´ÈÕÖ¾ºÍ×´Ì¬,±¾µØºÍÔ¶³Ì¹Ø»ú/ÖØÆô
            ÖÐ¡­¡­°üÀ¨±¾µØºÍÔ¶³Ì¿ØÖÆÔÆÌ¨,±¾µØºÍÔ¶³ÌÊÖ¶¯Â¼Ïñ,±¾µØºÍÔ¶³Ì»Ø·Å,ÓïÒô¶Ô½²ºÍÔ¶³ÌÔ¤ÀÀ
                  ±¾µØ±¸·Ý,±¾µØ/Ô¶³Ì¹Ø»ú/ÖØÆô
            ¸ß¡­¡­¹ÜÀíÔ±
            */
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 17, ArraySubType = UnmanagedType.I1)]
            public byte[] byRes;
        }

        //µ¥ÓÃ»§²ÎÊý(SDK_V15À©Õ¹)(×Ó½á¹¹)
        [StructLayoutAttribute(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
        public struct NET_DVR_USER_INFO_EX
        {
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = NAME_LEN, ArraySubType = UnmanagedType.I1)]
            public byte[] sUserName;/* ÓÃ»§Ãû */
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = PASSWD_LEN, ArraySubType = UnmanagedType.I1)]
            public byte[] sPassword;/* ÃÜÂë */
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = MAX_RIGHT, ArraySubType = UnmanagedType.U4)]
            public uint[] dwLocalRight;/* È¨ÏÞ */
            /*Êý×é0: ±¾µØ¿ØÖÆÔÆÌ¨*/
            /*Êý×é1: ±¾µØÊÖ¶¯Â¼Ïó*/
            /*Êý×é2: ±¾µØ»Ø·Å*/
            /*Êý×é3: ±¾µØÉèÖÃ²ÎÊý*/
            /*Êý×é4: ±¾µØ²é¿´×´Ì¬¡¢ÈÕÖ¾*/
            /*Êý×é5: ±¾µØ¸ß¼¶²Ù×÷(Éý¼¶£¬¸ñÊ½»¯£¬ÖØÆô£¬¹Ø»ú)*/
            public uint dwLocalPlaybackRight;/* ±¾µØ¿ÉÒÔ»Ø·ÅµÄÍ¨µÀ bit0 -- channel 1*/
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = MAX_RIGHT, ArraySubType = UnmanagedType.U4)]
            public uint[] dwRemoteRight;/* È¨ÏÞ */
            /*Êý×é0: Ô¶³Ì¿ØÖÆÔÆÌ¨*/
            /*Êý×é1: Ô¶³ÌÊÖ¶¯Â¼Ïó*/
            /*Êý×é2: Ô¶³Ì»Ø·Å */
            /*Êý×é3: Ô¶³ÌÉèÖÃ²ÎÊý*/
            /*Êý×é4: Ô¶³Ì²é¿´×´Ì¬¡¢ÈÕÖ¾*/
            /*Êý×é5: Ô¶³Ì¸ß¼¶²Ù×÷(Éý¼¶£¬¸ñÊ½»¯£¬ÖØÆô£¬¹Ø»ú)*/
            /*Êý×é6: Ô¶³Ì·¢ÆðÓïÒô¶Ô½²*/
            /*Êý×é7: Ô¶³ÌÔ¤ÀÀ*/
            /*Êý×é8: Ô¶³ÌÇëÇó±¨¾¯ÉÏ´«¡¢±¨¾¯Êä³ö*/
            /*Êý×é9: Ô¶³Ì¿ØÖÆ£¬±¾µØÊä³ö*/
            /*Êý×é10: Ô¶³Ì¿ØÖÆ´®¿Ú*/
            public uint dwNetPreviewRight;/* Ô¶³Ì¿ÉÒÔÔ¤ÀÀµÄÍ¨µÀ bit0 -- channel 1*/
            public uint dwNetPlaybackRight;/* Ô¶³Ì¿ÉÒÔ»Ø·ÅµÄÍ¨µÀ bit0 -- channel 1*/
            [MarshalAsAttribute(UnmanagedType.ByValTStr, SizeConst = 16)]
            public string sUserIP;/* ÓÃ»§IPµØÖ·(Îª0Ê±±íÊ¾ÔÊÐíÈÎºÎµØÖ·) */
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = MACADDR_LEN, ArraySubType = UnmanagedType.I1)]
            public byte[] byMACAddr;/* ÎïÀíµØÖ· */
        }

        //µ¥ÓÃ»§²ÎÊý(×Ó½á¹¹)
        [StructLayoutAttribute(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
        public struct NET_DVR_USER_INFO
        {
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = NAME_LEN, ArraySubType = UnmanagedType.I1)]
            public byte[] sUserName;/* ÓÃ»§Ãû */
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = PASSWD_LEN, ArraySubType = UnmanagedType.I1)]
            public byte[] sPassword;/* ÃÜÂë */
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = MAX_RIGHT, ArraySubType = UnmanagedType.U4)]
            public uint[] dwLocalRight;/* È¨ÏÞ */
            /*Êý×é0: ±¾µØ¿ØÖÆÔÆÌ¨*/
            /*Êý×é1: ±¾µØÊÖ¶¯Â¼Ïó*/
            /*Êý×é2: ±¾µØ»Ø·Å*/
            /*Êý×é3: ±¾µØÉèÖÃ²ÎÊý*/
            /*Êý×é4: ±¾µØ²é¿´×´Ì¬¡¢ÈÕÖ¾*/
            /*Êý×é5: ±¾µØ¸ß¼¶²Ù×÷(Éý¼¶£¬¸ñÊ½»¯£¬ÖØÆô£¬¹Ø»ú)*/
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = MAX_RIGHT, ArraySubType = UnmanagedType.U4)]
            public uint[] dwRemoteRight;/* È¨ÏÞ */
            /*Êý×é0: Ô¶³Ì¿ØÖÆÔÆÌ¨*/
            /*Êý×é1: Ô¶³ÌÊÖ¶¯Â¼Ïó*/
            /*Êý×é2: Ô¶³Ì»Ø·Å */
            /*Êý×é3: Ô¶³ÌÉèÖÃ²ÎÊý*/
            /*Êý×é4: Ô¶³Ì²é¿´×´Ì¬¡¢ÈÕÖ¾*/
            /*Êý×é5: Ô¶³Ì¸ß¼¶²Ù×÷(Éý¼¶£¬¸ñÊ½»¯£¬ÖØÆô£¬¹Ø»ú)*/
            /*Êý×é6: Ô¶³Ì·¢ÆðÓïÒô¶Ô½²*/
            /*Êý×é7: Ô¶³ÌÔ¤ÀÀ*/
            /*Êý×é8: Ô¶³ÌÇëÇó±¨¾¯ÉÏ´«¡¢±¨¾¯Êä³ö*/
            /*Êý×é9: Ô¶³Ì¿ØÖÆ£¬±¾µØÊä³ö*/
            /*Êý×é10: Ô¶³Ì¿ØÖÆ´®¿Ú*/
            [MarshalAsAttribute(UnmanagedType.ByValTStr, SizeConst = 16)]
            public string sUserIP;/* ÓÃ»§IPµØÖ·(Îª0Ê±±íÊ¾ÔÊÐíÈÎºÎµØÖ·) */
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = MACADDR_LEN, ArraySubType = UnmanagedType.I1)]
            public byte[] byMACAddr;/* ÎïÀíµØÖ· */
        }

        //DVRÓÃ»§²ÎÊý(9000À©Õ¹)
        [StructLayoutAttribute(LayoutKind.Sequential)]
        public struct NET_DVR_USER_V30
        {
            public uint dwSize;
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = MAX_USERNUM_V30, ArraySubType = UnmanagedType.Struct)]
            public NET_DVR_USER_INFO_V30[] struUser;
        }

        //DVRÓÃ»§²ÎÊý(SDK_V15À©Õ¹)
        [StructLayoutAttribute(LayoutKind.Sequential)]
        public struct NET_DVR_USER_EX
        {
            public uint dwSize;
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = MAX_USERNUM, ArraySubType = UnmanagedType.Struct)]
            public NET_DVR_USER_INFO_EX[] struUser;
        }

        //DVRÓÃ»§²ÎÊý
        [StructLayoutAttribute(LayoutKind.Sequential)]
        public struct NET_DVR_USER
        {
            public uint dwSize;
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = MAX_USERNUM, ArraySubType = UnmanagedType.Struct)]
            public NET_DVR_USER_INFO[] struUser;
        }

        //DVRÒì³£²ÎÊý(9000À©Õ¹)
        [StructLayoutAttribute(LayoutKind.Sequential)]
        public struct NET_DVR_EXCEPTION_V30
        {
            public uint dwSize;
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = MAX_EXCEPTIONNUM_V30, ArraySubType = UnmanagedType.Struct)]
            public NET_DVR_HANDLEEXCEPTION_V30[] struExceptionHandleType;
            /*Êý×é0-ÅÌÂú,1- Ó²ÅÌ³ö´í,2-ÍøÏß¶Ï,3-¾ÖÓòÍøÄÚIP µØÖ·³åÍ», 4-·Ç·¨·ÃÎÊ, 5-ÊäÈë/Êä³öÊÓÆµÖÆÊ½²»Æ¥Åä, 6-ÊÓÆµÐÅºÅÒì³£, 7-Â¼ÏñÒì³£*/
        }

        //DVRÒì³£²ÎÊý
        [StructLayoutAttribute(LayoutKind.Sequential)]
        public struct NET_DVR_EXCEPTION
        {
            public uint dwSize;
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = MAX_EXCEPTIONNUM, ArraySubType = UnmanagedType.Struct)]
            public NET_DVR_HANDLEEXCEPTION[] struExceptionHandleType;
            /*Êý×é0-ÅÌÂú,1- Ó²ÅÌ³ö´í,2-ÍøÏß¶Ï,3-¾ÖÓòÍøÄÚIP µØÖ·³åÍ»,4-·Ç·¨·ÃÎÊ, 5-ÊäÈë/Êä³öÊÓÆµÖÆÊ½²»Æ¥Åä, 6-ÊÓÆµÐÅºÅÒì³£*/
        }

        //Í¨µÀ×´Ì¬(9000À©Õ¹)
        [StructLayoutAttribute(LayoutKind.Sequential)]
        public struct NET_DVR_CHANNELSTATE_V30
        {
            public byte byRecordStatic;//Í¨µÀÊÇ·ñÔÚÂ¼Ïñ,0-²»Â¼Ïñ,1-Â¼Ïñ
            public byte bySignalStatic;//Á¬½ÓµÄÐÅºÅ×´Ì¬,0-Õý³£,1-ÐÅºÅ¶ªÊ§
            public byte byHardwareStatic;//Í¨µÀÓ²¼þ×´Ì¬,0-Õý³£,1-Òì³£,ÀýÈçDSPËÀµô
            public byte byRes1;//±£Áô
            public uint dwBitRate;//Êµ¼ÊÂëÂÊ
            public uint dwLinkNum;//¿Í»§¶ËÁ¬½ÓµÄ¸öÊý
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = MAX_LINK, ArraySubType = UnmanagedType.Struct)]
            public NET_DVR_IPADDR[] struClientIP;//¿Í»§¶ËµÄIPµØÖ·
            public uint dwIPLinkNum;//Èç¹û¸ÃÍ¨µÀÎªIP½ÓÈë£¬ÄÇÃ´±íÊ¾IP½ÓÈëµ±Ç°µÄÁ¬½ÓÊý
            public byte byExceedMaxLink;//ÊÇ·ñ³¬³öÁËµ¥Â·6Â·Á¬½ÓÊý 0 - Î´³¬³ö, 1-³¬³ö 
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 3, ArraySubType = UnmanagedType.I1)]
            public byte[] byRes;
            public uint dwAllBitRate;//ËùÓÐ¿Í»§¶ËºÍ¸ÃÍ¨µÀÁ¬½ÓµÄÊµ¼ÊÂëÂÊÖ®ºÍ 
            public uint dwChannelNo;//µ±Ç°µÄÍ¨µÀºÅ£¬0xffffffff±íÊ¾µ±Ç°¼°ºóÐøÍ¨µÀÐÅÏ¢ÎÞÐ§
        }

        //Í¨µÀ×´Ì¬
        [StructLayoutAttribute(LayoutKind.Sequential)]
        public struct NET_DVR_CHANNELSTATE
        {
            public byte byRecordStatic;//Í¨µÀÊÇ·ñÔÚÂ¼Ïñ,0-²»Â¼Ïñ,1-Â¼Ïñ
            public byte bySignalStatic;//Á¬½ÓµÄÐÅºÅ×´Ì¬,0-Õý³£,1-ÐÅºÅ¶ªÊ§
            public byte byHardwareStatic;//Í¨µÀÓ²¼þ×´Ì¬,0-Õý³£,1-Òì³£,ÀýÈçDSPËÀµô
            public byte reservedData;//±£Áô
            public uint dwBitRate;//Êµ¼ÊÂëÂÊ
            public uint dwLinkNum;//¿Í»§¶ËÁ¬½ÓµÄ¸öÊý
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = MAX_LINK, ArraySubType = UnmanagedType.U4)]
            public uint[] dwClientIP;//¿Í»§¶ËµÄIPµØÖ·
        }

        //Ó²ÅÌ×´Ì¬
        [StructLayoutAttribute(LayoutKind.Sequential)]
        public struct NET_DVR_DISKSTATE
        {
            public uint dwVolume;//Ó²ÅÌµÄÈÝÁ¿
            public uint dwFreeSpace;//Ó²ÅÌµÄÊ£Óà¿Õ¼ä
            public uint dwHardDiskStatic;//Ó²ÅÌµÄ×´Ì¬,0-»î¶¯,1-ÐÝÃß,2-²»Õý³£
        }

        //DVR¹¤×÷×´Ì¬(9000À©Õ¹)
        [StructLayoutAttribute(LayoutKind.Sequential)]
        public struct NET_DVR_WORKSTATE_V30
        {
            public uint dwDeviceStatic;//Éè±¸µÄ×´Ì¬,0-Õý³£,1-CPUÕ¼ÓÃÂÊÌ«¸ß,³¬¹ý85%,2-Ó²¼þ´íÎó,ÀýÈç´®¿ÚËÀµô
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = MAX_DISKNUM_V30, ArraySubType = UnmanagedType.Struct)]
            public NET_DVR_DISKSTATE[] struHardDiskStatic;
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = MAX_CHANNUM_V30, ArraySubType = UnmanagedType.Struct)]
            public NET_DVR_CHANNELSTATE_V30[] struChanStatic;//Í¨µÀµÄ×´Ì¬
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = MAX_ALARMIN_V30, ArraySubType = UnmanagedType.I1)]
            public byte[] byAlarmInStatic;//±¨¾¯¶Ë¿ÚµÄ×´Ì¬,0-Ã»ÓÐ±¨¾¯,1-ÓÐ±¨¾¯
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = MAX_ALARMOUT_V30, ArraySubType = UnmanagedType.I1)]
            public byte[] byAlarmOutStatic;//±¨¾¯Êä³ö¶Ë¿ÚµÄ×´Ì¬,0-Ã»ÓÐÊä³ö,1-ÓÐ±¨¾¯Êä³ö
            public uint dwLocalDisplay;//±¾µØÏÔÊ¾×´Ì¬,0-Õý³£,1-²»Õý³£
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = MAX_AUDIO_V30, ArraySubType = UnmanagedType.I1)]
            public byte[] byAudioChanStatus;//±íÊ¾ÓïÒôÍ¨µÀµÄ×´Ì¬ 0-Î´Ê¹ÓÃ£¬1-Ê¹ÓÃÖÐ, 0xffÎÞÐ§
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 10, ArraySubType = UnmanagedType.I1)]
            public byte[] byRes;
        }

        //DVR¹¤×÷×´Ì¬
        [StructLayoutAttribute(LayoutKind.Sequential)]
        public struct NET_DVR_WORKSTATE
        {
            public uint dwDeviceStatic;//Éè±¸µÄ×´Ì¬,0-Õý³£,1-CPUÕ¼ÓÃÂÊÌ«¸ß,³¬¹ý85%,2-Ó²¼þ´íÎó,ÀýÈç´®¿ÚËÀµô
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = MAX_DISKNUM, ArraySubType = UnmanagedType.Struct)]
            public NET_DVR_DISKSTATE[] struHardDiskStatic;
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = MAX_CHANNUM, ArraySubType = UnmanagedType.Struct)]
            public NET_DVR_CHANNELSTATE[] struChanStatic;//Í¨µÀµÄ×´Ì¬
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = MAX_ALARMIN, ArraySubType = UnmanagedType.I1)]
            public byte[] byAlarmInStatic;//±¨¾¯¶Ë¿ÚµÄ×´Ì¬,0-Ã»ÓÐ±¨¾¯,1-ÓÐ±¨¾¯
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = MAX_ALARMOUT, ArraySubType = UnmanagedType.I1)]
            public byte[] byAlarmOutStatic;//±¨¾¯Êä³ö¶Ë¿ÚµÄ×´Ì¬,0-Ã»ÓÐÊä³ö,1-ÓÐ±¨¾¯Êä³ö
            public uint dwLocalDisplay;//±¾µØÏÔÊ¾×´Ì¬,0-Õý³£,1-²»Õý³£

            public void Init()
            {
                struHardDiskStatic = new NET_DVR_DISKSTATE[MAX_DISKNUM];
                struChanStatic = new NET_DVR_CHANNELSTATE[MAX_CHANNUM];
                byAlarmInStatic = new byte[MAX_ALARMIN];
                byAlarmOutStatic = new byte[MAX_ALARMOUT];
            }
        }

        /************************DVRÈÕÖ¾ begin***************************/
        /* ±¨¾¯ */
        //Ö÷ÀàÐÍ
        public const int MAJOR_ALARM = 1;
        //´ÎÀàÐÍ
        public const int MINOR_ALARM_IN = 1;/* ±¨¾¯ÊäÈë */
        public const int MINOR_ALARM_OUT = 2;/* ±¨¾¯Êä³ö */
        public const int MINOR_MOTDET_START = 3; /* ÒÆ¶¯Õì²â±¨¾¯¿ªÊ¼ */
        public const int MINOR_MOTDET_STOP = 4; /* ÒÆ¶¯Õì²â±¨¾¯½áÊø */
        public const int MINOR_HIDE_ALARM_START = 5;/* ÕÚµ²±¨¾¯¿ªÊ¼ */
        public const int MINOR_HIDE_ALARM_STOP = 6;/* ÕÚµ²±¨¾¯½áÊø */
        public const int MINOR_VCA_ALARM_START = 7;/*ÖÇÄÜ±¨¾¯¿ªÊ¼*/
        public const int MINOR_VCA_ALARM_STOP = 8;/*ÖÇÄÜ±¨¾¯Í£Ö¹*/
        public const int MINOR_ITS_ALARM_START = 0x09; // ½»Í¨ÊÂ¼þ±¨¾¯¿ªÊ¼ 
        public const int MINOR_ITS_ALARM_STOP = 0x0a; // ½»Í¨ÊÂ¼þ±¨¾¯½áÊø 
        public const int MINOR_NETALARM_START = 0x0b; // ÍøÂç±¨¾¯¿ªÊ¼ 
        public const int MINOR_NETALARM_STOP = 0x0c; // ÍøÂç±¨¾¯½áÊø 
        public const int MINOR_NETALARM_RESUME = 0x0d; // ÍøÂç±¨¾¯»Ö¸´ 
        public const int MINOR_WIRELESS_ALARM_START = 0x0e; // ÎÞÏß±¨¾¯¿ªÊ¼ 
        public const int MINOR_WIRELESS_ALARM_STOP = 0x0f; // ÎÞÏß±¨¾¯½áÊø 
        public const int MINOR_PIR_ALARM_START = 0x10; // ÈËÌå¸ÐÓ¦±¨¾¯¿ªÊ¼ 
        public const int MINOR_PIR_ALARM_STOP = 0x11; // ÈËÌå¸ÐÓ¦±¨¾¯½áÊø 
        public const int MINOR_CALLHELP_ALARM_START = 0x12; // ºô¾È±¨¾¯¿ªÊ¼ 
        public const int MINOR_CALLHELP_ALARM_STOP = 0x13; // ºô¾È±¨¾¯½áÊø 
        public const int MINOR_DETECTFACE_ALARM_START = 0x16; // ÈËÁ³Õì²â±¨¾¯¿ªÊ¼ 
        public const int MINOR_DETECTFACE_ALARM_STOP = 0x17; // ÈËÁ³Õì²â±¨¾¯½áÊø 
        public const int MINOR_VQD_ALARM_START = 0x18; //VQD±¨¾¯ 
        public const int MINOR_VQD_ALARM_STOP = 0x19; //VQD±¨¾¯½áÊø 
        public const int MINOR_VCA_SECNECHANGE_DETECTION = 0x1a; // ³¡¾°Õì²â±¨¾¯ 
        public const int MINOR_SMART_REGION_EXITING_BEGIN = 0x1b; // Àë¿ªÇøÓòÕì²â¿ªÊ¼ 
        public const int MINOR_SMART_REGION_EXITING_END = 0x1c; // Àë¿ªÇøÓòÕì²â½áÊø 
        public const int MINOR_SMART_LOITERING_BEGIN = 0x1d; // ÅÇ»²Õì²â¿ªÊ¼ 
        public const int MINOR_SMART_LOITERING_END = 0x1e; // ÅÇ»²Õì²â½áÊø 
        public const int MINOR_VCA_ALARM_LINE_DETECTION_BEGIN = 0x20; // Ô½½çÕì²â¿ªÊ¼ 
        public const int MINOR_VCA_ALARM_LINE_DETECTION_END = 0x21; // Ô½½çÕì²â½áÊø 
        public const int MINOR_VCA_ALARM_INTRUDE_BEGIN = 0x22; // ÇøÓòÈëÇÖÕì²â¿ªÊ¼ 
        public const int MINOR_VCA_ALARM_INTRUDE_END = 0x23; // ÇøÓòÈëÇÖÕì²â½áÊø 
        public const int MINOR_VCA_ALARM_AUDIOINPUT = 0x24; // ÒôÆµ¶ªÊ§Õì²â 
        public const int MINOR_VCA_ALARM_AUDIOABNORMAL = 0x25; // ÒôÆµÒì³£Õì²â 
        public const int MINOR_VCA_DEFOCUS_DETECTION_BEGIN = 0x26; // Ðé½¹Õì²â¿ªÊ¼ 
        public const int MINOR_VCA_DEFOCUS_DETECTION_END = 0x27; // Ðé½¹Õì²â½áÊø
        public const int MINOR_EXT_ALARM = 0x28; // IPCÍâ²¿±¨¾¯
        public const int MINOR_VCA_FACE_ALARM_BEGIN = 0x29; // ÈËÁ³Õì²â¿ªÊ¼ 
        public const int MINOR_SMART_REGION_ENTRANCE_BEGIN = 0x2a; // ½øÈëÇøÓòÕì²â¿ªÊ¼ 
        public const int MINOR_SMART_REGION_ENTRANCE_END = 0x2b; // ½øÈëÇøÓòÕì²â½áÊø 
        public const int MINOR_SMART_PEOPLE_GATHERING_BEGIN = 0x2c; // ÈËÔ±¾Û¼¯Õì²â¿ªÊ¼ 
        public const int MINOR_SMART_PEOPLE_GATHERING_END = 0x2d; // ÈËÔ±¾Û¼¯Õì²â½áÊø 
        public const int MINOR_SMART_FAST_MOVING_BEGIN = 0x2e; // ¿ìËÙÔË¶¯Õì²â¿ªÊ¼ 
        public const int MINOR_SMART_FAST_MOVING_END = 0x2f; // ¿ìËÙÔË¶¯Õì²â½áÊø 
        public const int MINOR_VCA_FACE_ALARM_END = 0x30; // ÈËÁ³Õì²â½áÊø 
        public const int MINOR_VCA_SCENE_CHANGE_ALARM_BEGIN = 0x31; // ³¡¾°±ä¸üÕì²â¿ªÊ¼ 
        public const int MINOR_VCA_SCENE_CHANGE_ALARM_END = 0x32; // ³¡¾°±ä¸üÕì²â½áÊø 
        public const int MINOR_VCA_ALARM_AUDIOINPUT_BEGIN = 0x33; // ÒôÆµ¶ªÊ§Õì²â¿ªÊ¼ 
        public const int MINOR_VCA_ALARM_AUDIOINPUT_END = 0x34; // ÒôÆµ¶ªÊ§Õì²â½áÊø 
        public const int MINOR_VCA_ALARM_AUDIOABNORMAL_BEGIN = 0x35; // ÉùÇ¿Í»±äÕì²â¿ªÊ¼ 
        public const int MINOR_VCA_ALARM_AUDIOABNORMAL_END = 0x36; // ÉùÇ¿Í»±äÕì²â½áÊø 

        public const int MINOR_VCA_LECTURE_DETECTION_BEGIN = 0x37;  //ÊÚ¿ÎÕì²â¿ªÊ¼±¨¾¯
        public const int MINOR_VCA_LECTURE_DETECTION_END = 0x38;  //ÊÚ¿ÎÕì²â½áÊø±¨¾¯
        public const int MINOR_VCA_ALARM_AUDIOSTEEPDROP = 0x39;  //ÉùÇ¿¶¸½µ 2014-03-21
        public const int MINOR_VCA_ANSWER_DETECTION_BEGIN = 0x3a;  //»Ø´ðÎÊÌâÕì²â¿ªÊ¼±¨¾¯
        public const int MINOR_VCA_ANSWER_DETECTION_END = 0x3b;  //»Ø´ðÎÊÌâÕì²â½áÊø±¨¾¯

        public const int MINOR_SMART_PARKING_BEGIN = 0x3c; // Í£³µÕì²â¿ªÊ¼ 
        public const int MINOR_SMART_PARKING_END = 0x3d; // Í£³µÕì²â½áÊø 
        public const int MINOR_SMART_UNATTENDED_BAGGAGE_BEGIN = 0x3e; // ÎïÆ·ÒÅÁôÕì²â¿ªÊ¼ 
        public const int MINOR_SMART_UNATTENDED_BAGGAGE_END = 0x3f; // ÎïÆ·ÒÅÁôÕì²â½áÊø 
        public const int MINOR_SMART_OBJECT_REMOVAL_BEGIN = 0x40; // ÎïÆ·ÄÃÈ¡Õì²â¿ªÊ¼ 
        public const int MINOR_SMART_OBJECT_REMOVAL_END = 0x41; // ÎïÆ·ÄÃÈ¡Õì²â½áÊø 
        public const int MINOR_SMART_VEHICLE_ALARM_START = 0x46;   //³µÅÆ¼ì²â¿ªÊ¼
        public const int MINOR_SMART_VEHICLE_ALARM_STOP = 0x47;   //³µÅÆ¼ì²â½áÊø
        public const int MINOR_THERMAL_FIREDETECTION = 0x48;   //ÈÈ³ÉÏñ»ðµã¼ì²âÕì²â¿ªÊ¼
        public const int MINOR_THERMAL_FIREDETECTION_END = 0x49;   //ÈÈ³ÉÏñ»ðµã¼ì²âÕì²â½áÊø
        public const int MINOR_SMART_VANDALPROOF_BEGIN = 0x50;   //·ÀÆÆ»µ¼ì²â¿ªÊ¼
        public const int MINOR_SMART_VANDALPROOF_END = 0x51; //·ÀÆÆ»µ¼ì²â½áÊø

        public const int MINOR_ALARMIN_SHORT_CIRCUIT = 0x400; // ·ÀÇø¶ÌÂ·±¨¾¯ 
        public const int MINOR_ALARMIN_BROKEN_CIRCUIT = 0x401; // ·ÀÇø¶ÏÂ·±¨¾¯ 
        public const int MINOR_ALARMIN_EXCEPTION = 0x402; // ·ÀÇøÒì³£±¨¾¯ 
        public const int MINOR_ALARMIN_RESUME = 0x403; // ·ÀÇø±¨¾¯»Ö¸´ 
        public const int MINOR_HOST_DESMANTLE_ALARM = 0x404; //·ÀÇø·À²ð±¨¾¯  
        public const int MINOR_HOST_DESMANTLE_RESUME = 0x405; // ·ÀÇø·À²ð»Ö¸´ 
        public const int MINOR_CARD_READER_DESMANTLE_ALARM = 0x406; //¶Á¿¨Æ÷·À²ð±¨¾¯ 
        public const int MINOR_CARD_READER_DESMANTLE_RESUME = 0x407; // ¶Á¿¨Æ÷·À²ð»Ö¸´  
        public const int MINOR_CASE_SENSOR_ALARM = 0x408; // ÊÂ¼þÊäÈë±¨¾¯ 
        public const int MINOR_CASE_SENSOR_RESUME = 0x409; // ÊÂ¼þÊäÈë»Ö¸´ 
        public const int MINOR_STRESS_ALARM = 0x40a; // Ð²ÆÈ±¨¾¯ 
        public const int MINOR_OFFLINE_ECENT_NEARLY_FULL = 0x40b; // ÀëÏßÊÂ¼þÂú90%±¨¾¯ 
        public const int MINOR_CARD_MAX_AUTHENTICATE_FAIL = 0x40c; // ¿¨ºÅÈÏÖ¤Ê§°Ü³¬´Î±¨¾¯ 
        public const int MINOR_SD_CARD_FULL = 0x40d;  //SD¿¨´æ´¢Âú±¨¾¯
        public const int MINOR_LINKAGE_CAPTURE_PIC = 0x40e;  //Áª¶¯×¥ÅÄÊÂ¼þ±¨¾¯

        /* Òì³£ */
        //Ö÷ÀàÐÍ
        public const int MAJOR_EXCEPTION = 2;
        //´ÎÀàÐÍ
        public const int MINOR_VI_LOST = 33;/* ÊÓÆµÐÅºÅ¶ªÊ§ */
        public const int MINOR_ILLEGAL_ACCESS = 34;/* ·Ç·¨·ÃÎÊ */
        public const int MINOR_HD_FULL = 35;/* Ó²ÅÌÂú */
        public const int MINOR_HD_ERROR = 36;/* Ó²ÅÌ´íÎó */
        public const int MINOR_DCD_LOST = 37;/* MODEM µôÏß(±£Áô²»Ê¹ÓÃ) */
        public const int MINOR_IP_CONFLICT = 38;/* IPµØÖ·³åÍ» */
        public const int MINOR_NET_BROKEN = 39;/* ÍøÂç¶Ï¿ª*/
        public const int MINOR_REC_ERROR = 40;/* Â¼Ïñ³ö´í */
        public const int MINOR_IPC_NO_LINK = 41;/* IPCÁ¬½ÓÒì³£ */
        public const int MINOR_VI_EXCEPTION = 42;/* ÊÓÆµÊäÈëÒì³£(Ö»Õë¶ÔÄ£ÄâÍ¨µÀ) */
        public const int MINOR_IPC_IP_CONFLICT = 43;/*ipc ip µØÖ· ³åÍ»*/
        public const int MINOR_RAID_ERROR = 0x20; // ÕóÁÐÒì³£  
        public const int MINOR_SENCE_EXCEPTION = 0x2c; // ³¡¾°Òì³£ 
        public const int MINOR_PIC_REC_ERROR = 0x2d; // ×¥Í¼³ö´í,»ñÈ¡Í¼Æ¬ÎÄ¼þÊ§°Ü 
        public const int MINOR_VI_MISMATCH = 0x2e; // ÊÓÆµÖÆÊ½²»Æ¥Åä 
        public const int MINOR_RESOLUTION_MISMATCH = 0x2f; // ±àÂë·Ö±æÂÊºÍÇ°¶Ë·Ö±æÂÊ²»Æ¥Åä 

        //2010-01-22 Ôö¼ÓÊÓÆµ×ÛºÏÆ½Ì¨Òì³£ÈÕÖ¾´ÎÀàÐÍ
        public const int MINOR_NET_ABNORMAL = 0x35; /*ÍøÂç×´Ì¬Òì³£*/
        public const int MINOR_MEM_ABNORMAL = 0x36; /*ÄÚ´æ×´Ì¬Òì³£*/
        public const int MINOR_FILE_ABNORMAL = 0x37; /*ÎÄ¼þ×´Ì¬Òì³£*/
        public const int MINOR_PANEL_ABNORMAL = 0x38; /*Ç°Ãæ°åÁ¬½ÓÒì³£*/
        public const int MINOR_PANEL_RESUME = 0x39; /*Ç°Ãæ°å»Ö¸´Õý³£*/
        public const int MINOR_RS485_DEVICE_ABNORMAL = 0x3a; /*RS485Á¬½Ó×´Ì¬Òì³£*/
        public const int MINOR_RS485_DEVICE_REVERT = 0x3b; /*RS485Á¬½Ó×´Ì¬Òì³£»Ö¸´*/
        public const int MINOR_SCREEN_SUBSYSTEM_ABNORMALREBOOT = 0x3c; // ×Ó°åÒì³£Æô¶¯ 
        public const int MINOR_SCREEN_SUBSYSTEM_ABNORMALINSERT = 0x3d; // ×Ó°å²åÈë 
        public const int MINOR_SCREEN_SUBSYSTEM_ABNORMALPULLOUT = 0x3e; // ×Ó°å°Î³ö 
        public const int MINOR_SCREEN_ABNARMALTEMPERATURE = 0x3f; // ÎÂ¶ÈÒì³£ 
        public const int MINOR_RECORD_OVERFLOW = 0x41; // »º³åÇøÒç³ö 
        public const int MINOR_DSP_ABNORMAL = 0x42; // DSPÒì³£ 
        public const int MINOR_ANR_RECORD_FAIED = 0x43; // ANRÂ¼ÏñÊ§°Ü 
        public const int MINOR_SPARE_WORK_DEVICE_EXCEPT = 0x44; // ÈÈ±¸Éè±¸¹¤×÷»úÒì³£ 
        public const int MINOR_START_IPC_MAS_FAILED = 0x45; // ¿ªÆôIPC MASÊ§°Ü 
        public const int MINOR_IPCM_CRASH = 0x46; // IPCMÒì³£ÖØÆô 
        public const int MINOR_POE_POWER_EXCEPTION = 0x47; // POE¹©µçÒì³£ 
        public const int MINOR_UPLOAD_DATA_CS_EXCEPTION = 0x48; // ÔÆ´æ´¢Êý¾ÝÉÏ´«Ê§°Ü 
        public const int MINOR_DIAL_EXCEPTION = 0x49;         /*²¦ºÅÒì³£*/
        public const int MINOR_DEV_EXCEPTION_OFFLINE = 0x50;  //Éè±¸Òì³£ÏÂÏß
        public const int MINOR_UPGRADEFAIL = 0x51; //Ô¶³ÌÉý¼¶Éè±¸Ê§°Ü
        public const int MINOR_AI_LOST = 0x52; /* ÒôÆµÐÅºÅ¶ªÊ§ */

        public const int MINOR_DEV_POWER_ON = 0x400; // Éè±¸ÉÏµçÆô¶¯ 
        public const int MINOR_DEV_POWER_OFF = 0x401; // Éè±¸µôµç¹Ø±Õ 
        public const int MINOR_WATCH_DOG_RESET = 0x402; // ¿´ÃÅ¹·¸´Î» 
        public const int MINOR_LOW_BATTERY = 0x403; // Ðîµç³ØµçÑ¹µÍ 
        public const int MINOR_BATTERY_RESUME = 0x404; // Ðîµç³ØµçÑ¹»Ö¸´Õý³£ 
        public const int MINOR_AC_OFF = 0x405; // ½»Á÷µç¶Ïµç 
        public const int MINOR_AC_RESUME = 0x406; // ½»Á÷µç»Ö¸´ 
        public const int MINOR_NET_RESUME = 0x407; // ÍøÂç»Ö¸´ 
        public const int MINOR_FLASH_ABNORMAL = 0x408; // FLASH¶ÁÐ´Òì³£ 
        public const int MINOR_CARD_READER_OFFLINE = 0x409; // ¶Á¿¨Æ÷µôÏß 
        public const int MINOR_CARD_READER_RESUME = 0x40a; // ¶Á¿¨Æ÷µôÏß»Ö¸´ 
        public const int MINOR_SUBSYSTEM_IP_CONFLICT = 0x4000; // ×Ó°åIP³åÍ» 
        public const int MINOR_SUBSYSTEM_NET_BROKEN = 0x4001; // ×Ó°å¶ÏÍø 
        public const int MINOR_FAN_ABNORMAL = 0x4002; // ·çÉÈÒì³£ 
        public const int MINOR_BACKPANEL_TEMPERATURE_ABNORMAL = 0x4003; // ±³°åÎÂ¶ÈÒì³£ 

        //ÊÓÆµ×ÛºÏÆ½Ì¨
        public const int MINOR_FANABNORMAL = 49;/* ÊÓÆµ×ÛºÏÆ½Ì¨£º·çÉÈ×´Ì¬Òì³£ */
        public const int MINOR_FANRESUME = 50;/* ÊÓÆµ×ÛºÏÆ½Ì¨£º·çÉÈ×´Ì¬»Ö¸´Õý³£ */
        public const int MINOR_SUBSYSTEM_ABNORMALREBOOT = 51;/* ÊÓÆµ×ÛºÏÆ½Ì¨£º6467Òì³£ÖØÆô */
        public const int MINOR_MATRIX_STARTBUZZER = 52;/* ÊÓÆµ×ÛºÏÆ½Ì¨£ºdm6467Òì³££¬Æô¶¯·äÃùÆ÷ */

        /* ²Ù×÷ */
        //Ö÷ÀàÐÍ
        public const int MAJOR_OPERATION = 3;

        //´ÎÀàÐÍ
        public const int MINOR_VCA_MOTIONEXCEPTION = 0x29; //ÖÇÄÜÕì²âÒì³£
        public const int MINOR_START_DVR = 0x41; // ¿ª»ú 
        public const int MINOR_STOP_DVR = 0x42; // ¹Ø»ú 
        public const int MINOR_STOP_ABNORMAL = 0x43; // Òì³£¹Ø»ú 
        public const int MINOR_REBOOT_DVR = 0x44; // ±¾µØÖØÆôÉè±¸ 

        public const int MINOR_LOCAL_LOGIN = 0x50; // ±¾µØµÇÂ½ 
        public const int MINOR_LOCAL_LOGOUT = 0x51; // ±¾µØ×¢ÏúµÇÂ½ 
        public const int MINOR_LOCAL_CFG_PARM = 0x52; // ±¾µØÅäÖÃ²ÎÊý 
        public const int MINOR_LOCAL_PLAYBYFILE = 0x53; // ±¾µØ°´ÎÄ¼þ»Ø·Å»òÏÂÔØ 
        public const int MINOR_LOCAL_PLAYBYTIME = 0x54; // ±¾µØ°´Ê±¼ä»Ø·Å»òÏÂÔØ 
        public const int MINOR_LOCAL_START_REC = 0x55; // ±¾µØ¿ªÊ¼Â¼Ïñ 
        public const int MINOR_LOCAL_STOP_REC = 0x56; // ±¾µØÍ£Ö¹Â¼Ïñ 
        public const int MINOR_LOCAL_PTZCTRL = 0x57; // ±¾µØÔÆÌ¨¿ØÖÆ 
        public const int MINOR_LOCAL_PREVIEW = 0x58; // ±¾µØÔ¤ÀÀ(±£Áô²»Ê¹ÓÃ) 
        public const int MINOR_LOCAL_MODIFY_TIME = 0x59; // ±¾µØÐÞ¸ÄÊ±¼ä(±£Áô²»Ê¹ÓÃ) 
        public const int MINOR_LOCAL_UPGRADE = 0x5a; // ±¾µØÉý¼¶ 
        public const int MINOR_LOCAL_RECFILE_OUTPUT = 0x5b; // ±¾µØ±¸·ÝÂ¼ÏóÎÄ¼þ 
        public const int MINOR_LOCAL_FORMAT_HDD = 0x5c; // ±¾µØ³õÊ¼»¯Ó²ÅÌ 
        public const int MINOR_LOCAL_CFGFILE_OUTPUT = 0x5d; // µ¼³ö±¾µØÅäÖÃÎÄ¼þ 
        public const int MINOR_LOCAL_CFGFILE_INPUT = 0x5e; // µ¼Èë±¾µØÅäÖÃÎÄ¼þ 
        public const int MINOR_LOCAL_COPYFILE = 0x5f; // ±¾µØ±¸·ÝÎÄ¼þ 
        public const int MINOR_LOCAL_LOCKFILE = 0x60; // ±¾µØËø¶¨Â¼ÏñÎÄ¼þ 
        public const int MINOR_LOCAL_UNLOCKFILE = 0x61; // ±¾µØ½âËøÂ¼ÏñÎÄ¼þ 
        public const int MINOR_LOCAL_DVR_ALARM = 0x62; // ±¾µØÊÖ¶¯Çå³ýºÍ´¥·¢±¨¾¯ 
        public const int MINOR_IPC_ADD = 0x63; // ±¾µØÌí¼ÓIPC 
        public const int MINOR_IPC_DEL = 0x64; // ±¾µØÉ¾³ýIPC 
        public const int MINOR_IPC_SET = 0x65; // ±¾µØÉèÖÃIPC 
        public const int MINOR_LOCAL_START_BACKUP = 0x66; // ±¾µØ¿ªÊ¼±¸·Ý 
        public const int MINOR_LOCAL_STOP_BACKUP = 0x67; // ±¾µØÍ£Ö¹±¸·Ý 
        public const int MINOR_LOCAL_COPYFILE_START_TIME = 0x68; // ±¾µØ±¸·Ý¿ªÊ¼Ê±¼ä 
        public const int MINOR_LOCAL_COPYFILE_END_TIME = 0x69; // ±¾µØ±¸·Ý½áÊøÊ±¼ä 
        public const int MINOR_LOCAL_ADD_NAS = 0x6a; // ±¾µØÌí¼ÓÍøÂçÓ²ÅÌ 
        public const int MINOR_LOCAL_DEL_NAS = 0x6b; // ±¾µØÉ¾³ýNASÅÌ 
        public const int MINOR_LOCAL_SET_NAS = 0x6c; // ±¾µØÉèÖÃNASÅÌ 
        public const int MINOR_LOCAL_RESET_PASSWD = 0x6d; /* ±¾µØ»Ö¸´¹ÜÀíÔ±Ä¬ÈÏÃÜÂë*/

        public const int MINOR_REMOTE_LOGIN = 0x70; // Ô¶³ÌµÇÂ¼ 
        public const int MINOR_REMOTE_LOGOUT = 0x71; // Ô¶³Ì×¢ÏúµÇÂ½ 
        public const int MINOR_REMOTE_START_REC = 0x72; // Ô¶³Ì¿ªÊ¼Â¼Ïñ 
        public const int MINOR_REMOTE_STOP_REC = 0x73; // Ô¶³ÌÍ£Ö¹Â¼Ïñ 
        public const int MINOR_START_TRANS_CHAN = 0x74; // ¿ªÊ¼Í¸Ã÷´«Êä 
        public const int MINOR_STOP_TRANS_CHAN = 0x75; // Í£Ö¹Í¸Ã÷´«Êä 
        public const int MINOR_REMOTE_GET_PARM = 0x76; // Ô¶³Ì»ñÈ¡²ÎÊý 
        public const int MINOR_REMOTE_CFG_PARM = 0x77; // Ô¶³ÌÅäÖÃ²ÎÊý 
        public const int MINOR_REMOTE_GET_STATUS = 0x78; // Ô¶³Ì»ñÈ¡×´Ì¬ 
        public const int MINOR_REMOTE_ARM = 0x79; // Ô¶³Ì²¼·À 
        public const int MINOR_REMOTE_DISARM = 0x7a; // Ô¶³Ì³··À 
        public const int MINOR_REMOTE_REBOOT = 0x7b; // Ô¶³ÌÖØÆô 
        public const int MINOR_START_VT = 0x7c; // ¿ªÊ¼ÓïÒô¶Ô½² 
        public const int MINOR_STOP_VT = 0x7d; // Í£Ö¹ÓïÒô¶Ô½² 
        public const int MINOR_REMOTE_UPGRADE = 0x7e; // Ô¶³ÌÉý¼¶ 
        public const int MINOR_REMOTE_PLAYBYFILE = 0x7f; // Ô¶³Ì°´ÎÄ¼þ»Ø·Å 
        public const int MINOR_REMOTE_PLAYBYTIME = 0x80; // Ô¶³Ì°´Ê±¼ä»Ø·Å 
        public const int MINOR_REMOTE_PTZCTRL = 0x81; // Ô¶³ÌÔÆÌ¨¿ØÖÆ 
        public const int MINOR_REMOTE_FORMAT_HDD = 0x82; // Ô¶³Ì¸ñÊ½»¯Ó²ÅÌ 
        public const int MINOR_REMOTE_STOP = 0x83; // Ô¶³Ì¹Ø»ú 
        public const int MINOR_REMOTE_LOCKFILE = 0x84; // Ô¶³ÌËø¶¨ÎÄ¼þ 
        public const int MINOR_REMOTE_UNLOCKFILE = 0x85; // Ô¶³Ì½âËøÎÄ¼þ 
        public const int MINOR_REMOTE_CFGFILE_OUTPUT = 0x86; // Ô¶³Ìµ¼³öÅäÖÃÎÄ¼þ 
        public const int MINOR_REMOTE_CFGFILE_INTPUT = 0x87; // Ô¶³Ìµ¼ÈëÅäÖÃÎÄ¼þ 
        public const int MINOR_REMOTE_RECFILE_OUTPUT = 0x88; // Ô¶³Ìµ¼³öÂ¼ÏóÎÄ¼þ 
        public const int MINOR_REMOTE_DVR_ALARM = 0x89; // Ô¶³ÌÊÖ¶¯Çå³ýºÍ´¥·¢±¨¾¯ 
        public const int MINOR_REMOTE_IPC_ADD = 0x8a; // Ô¶³ÌÌí¼ÓIPC 
        public const int MINOR_REMOTE_IPC_DEL = 0x8b; // Ô¶³ÌÉ¾³ýIPC 
        public const int MINOR_REMOTE_IPC_SET = 0x8c; // Ô¶³ÌÉèÖÃIPC 
        public const int MINOR_REBOOT_VCA_LIB = 0x8d; // ÖØÆôÖÇÄÜ¿â 
        public const int MINOR_REMOTE_ADD_NAS = 0x8e; // Ô¶³ÌÌí¼ÓNASÅÌ 
        public const int MINOR_REMOTE_DEL_NAS = 0x8f; // Ô¶³ÌÉ¾³ýNASÅÌ 

        public const int MINOR_REMOTE_SET_NAS = 0x90; // Ô¶³ÌÉèÖÃNASÅÌ 
        public const int MINOR_LOCAL_START_REC_CDRW = 0x91; // ±¾µØ¿ªÊ¼¿ÌÂ¼ 
        public const int MINOR_LOCAL_STOP_REC_CDRW = 0x92; // ±¾µØÍ£Ö¹¿ÌÂ¼ 
        public const int MINOR_REMOTE_START_REC_CDRW = 0x93; // Ô¶³Ì¿ªÊ¼¿ÌÂ¼ 
        public const int MINOR_REMOTE_STOP_REC_CDRW = 0x94; // Ô¶³ÌÍ£Ö¹¿ÌÂ¼ 
        public const int MINOR_LOCAL_PIC_OUTPUT = 0x95; // ±¾µØ±¸·ÝÍ¼Æ¬ÎÄ¼þ 
        public const int MINOR_REMOTE_PIC_OUTPUT = 0x96; // Ô¶³Ì±¸·ÝÍ¼Æ¬ÎÄ¼þ 
        public const int MINOR_LOCAL_INQUEST_RESUME = 0x97; // ±¾µØ»Ö¸´ÉóÑ¶ÊÂ¼þ 
        public const int MINOR_REMOTE_INQUEST_RESUME = 0x98; // Ô¶³Ì»Ö¸´ÉóÑ¶ÊÂ¼þ 
        public const int MINOR_LOCAL_ADD_FILE = 0x99; // ±¾µØµ¼ÈëÎÄ¼þ 
        public const int MINOR_REMOTE_DELETE_HDISK = 0x9a; // Ô¶³ÌÉ¾³ýÒì³£²»´æÔÚµÄÓ²ÅÌ 
        public const int MINOR_REMOTE_LOAD_HDISK = 0x9b; // Ô¶³Ì¼ÓÔØÓ²ÅÌ 
        public const int MINOR_REMOTE_UNLOAD_HDISK = 0x9c; // Ô¶³ÌÐ¶ÔØÓ²ÅÌ 
        public const int MINOR_LOCAL_OPERATE_LOCK = 0x9d; // ±¾µØ²Ù×÷Ëø¶¨ 
        public const int MINOR_LOCAL_OPERATE_UNLOCK = 0x9e; // ±¾µØ²Ù×÷½â³ýËø¶¨ 
        public const int MINOR_LOCAL_DEL_FILE = 0x9f; // ±¾µØÉ¾³ýÉóÑ¶ÎÄ¼þ 

        public const int MINOR_SUBSYSTEMREBOOT = 0xa0; /*ÊÓÆµ×ÛºÏÆ½Ì¨£ºdm6467 Õý³£ÖØÆô*/
        public const int MINOR_MATRIX_STARTTRANSFERVIDEO = 0xa1; /*ÊÓÆµ×ÛºÏÆ½Ì¨£º¾ØÕóÇÐ»»¿ªÊ¼´«ÊäÍ¼Ïñ*/
        public const int MINOR_MATRIX_STOPTRANSFERVIDEO = 0xa2; /*ÊÓÆµ×ÛºÏÆ½Ì¨£º¾ØÕóÇÐ»»Í£Ö¹´«ÊäÍ¼Ïñ*/
        public const int MINOR_REMOTE_SET_ALLSUBSYSTEM = 0xa3; /*ÊÓÆµ×ÛºÏÆ½Ì¨£ºÉèÖÃËùÓÐ6467×ÓÏµÍ³ÐÅÏ¢*/
        public const int MINOR_REMOTE_GET_ALLSUBSYSTEM = 0xa4; /*ÊÓÆµ×ÛºÏÆ½Ì¨£º»ñÈ¡ËùÓÐ6467×ÓÏµÍ³ÐÅÏ¢*/
        public const int MINOR_REMOTE_SET_PLANARRAY = 0xa5; /*ÊÓÆµ×ÛºÏÆ½Ì¨£ºÉèÖÃ¼Æ»®ÂÖÑ²×é*/
        public const int MINOR_REMOTE_GET_PLANARRAY = 0xa6; /*ÊÓÆµ×ÛºÏÆ½Ì¨£º»ñÈ¡¼Æ»®ÂÖÑ²×é*/
        public const int MINOR_MATRIX_STARTTRANSFERAUDIO = 0xa7; /*ÊÓÆµ×ÛºÏÆ½Ì¨£º¾ØÕóÇÐ»»¿ªÊ¼´«ÊäÒôÆµ*/
        public const int MINOR_MATRIX_STOPRANSFERAUDIO = 0xa8; /*ÊÓÆµ×ÛºÏÆ½Ì¨£º¾ØÕóÇÐ»»Í£Ö¹´«ÊäÒôÆµ*/
        public const int MINOR_LOGON_CODESPITTER = 0xa9; /*ÊÓÆµ×ÛºÏÆ½Ì¨£ºµÇÂ½Âë·ÖÆ÷*/
        public const int MINOR_LOGOFF_CODESPITTER = 0xaa; /*ÊÓÆµ×ÛºÏÆ½Ì¨£ºÍË³öÂë·ÖÆ÷*/

        //KY2013 3.0.0
        public const int MINOR_LOCAL_MAIN_AUXILIARY_PORT_SWITCH = 0X302; //±¾µØÖ÷¸¨¿ÚÇÐ»»
        public const int MINOR_LOCAL_HARD_DISK_CHECK = 0x303; //±¾µØÎïÀíÓ²ÅÌ×Ô¼ì
                                                              //2010-01-22 Ôö¼ÓÊÓÆµ×ÛºÏÆ½Ì¨ÖÐ½âÂëÆ÷²Ù×÷ÈÕÖ¾
        public const int MINOR_START_DYNAMIC_DECODE = 0xb; /*¿ªÊ¼¶¯Ì¬½âÂë*/
        public const int MINOR_STOP_DYNAMIC_DECODE = 0xb1; /*Í£Ö¹¶¯Ì¬½âÂë*/
        public const int MINOR_GET_CYC_CFG = 0xb2; /*»ñÈ¡½âÂëÆ÷Í¨µÀÂÖÑ²ÅäÖÃ*/
        public const int MINOR_SET_CYC_CFG = 0xb3; /*ÉèÖÃ½âÂëÍ¨µÀÂÖÑ²ÅäÖÃ*/
        public const int MINOR_START_CYC_DECODE = 0xb4; /*¿ªÊ¼ÂÖÑ²½âÂë*/
        public const int MINOR_STOP_CYC_DECODE = 0xb5; /*Í£Ö¹ÂÖÑ²½âÂë*/
        public const int MINOR_GET_DECCHAN_STATUS = 0xb6; /*»ñÈ¡½âÂëÍ¨µÀ×´Ì¬*/
        public const int MINOR_GET_DECCHAN_INFO = 0xb7; /*»ñÈ¡½âÂëÍ¨µÀµ±Ç°ÐÅÏ¢*/
        public const int MINOR_START_PASSIVE_DEC = 0xb8; /*¿ªÊ¼±»¶¯½âÂë*/
        public const int MINOR_STOP_PASSIVE_DEC = 0xb9; /*Í£Ö¹±»¶¯½âÂë*/
        public const int MINOR_CTRL_PASSIVE_DEC = 0xba; /*¿ØÖÆ±»¶¯½âÂë*/
        public const int MINOR_RECON_PASSIVE_DEC = 0xbb; /*±»¶¯½âÂëÖØÁ¬*/
        public const int MINOR_GET_DEC_CHAN_SW = 0xbc; /*»ñÈ¡½âÂëÍ¨µÀ×Ü¿ª¹Ø*/
        public const int MINOR_SET_DEC_CHAN_SW = 0xbd; /*ÉèÖÃ½âÂëÍ¨µÀ×Ü¿ª¹Ø*/
        public const int MINOR_CTRL_DEC_CHAN_SCALE = 0xbe; /*½âÂëÍ¨µÀËõ·Å¿ØÖÆ*/
        public const int MINOR_SET_REMOTE_REPLAY = 0xbf; /*ÉèÖÃÔ¶³Ì»Ø·Å*/
        public const int MINOR_GET_REMOTE_REPLAY = 0xc0; /*»ñÈ¡Ô¶³Ì»Ø·Å×´Ì¬*/
        public const int MINOR_CTRL_REMOTE_REPLAY = 0xc1; /*Ô¶³Ì»Ø·Å¿ØÖÆ*/
        public const int MINOR_SET_DISP_CFG = 0xc2; /*ÉèÖÃÏÔÊ¾Í¨µÀ*/
        public const int MINOR_GET_DISP_CFG = 0xc3; /*»ñÈ¡ÏÔÊ¾Í¨µÀÉèÖÃ*/
        public const int MINOR_SET_PLANTABLE = 0xc4; /*ÉèÖÃ¼Æ»®ÂÖÑ²±í*/
        public const int MINOR_GET_PLANTABLE = 0xc5;/*»ñÈ¡¼Æ»®ÂÖÑ²±í*/
        public const int MINOR_START_PPPPOE = 0xc6; /*¿ªÊ¼PPPoEÁ¬½Ó*/
        public const int MINOR_STOP_PPPPOE = 0xc7; /*½áÊøPPPoEÁ¬½Ó*/
        public const int MINOR_UPLOAD_LOGO = 0xc8; /*ÉÏ´«LOGO*/

        //ÍÆÄ£Ê½²Ù×÷ÈÕÖ¾
        public const int MINOR_LOCAL_PIN = 0xc9; /* ±¾µØPIN¹¦ÄÜ²Ù×÷ */
        public const int MINOR_LOCAL_DIAL = 0xca; /* ±¾µØÊÖ¶¯Æô¶¯¶Ï¿ª²¦ºÅ */
        public const int MINOR_SMS_CONTROL = 0xcb; /* ¶ÌÐÅ¿ØÖÆÉÏÏÂÏß */
        public const int MINOR_CALL_ONLINE = 0xc; /* ºô½Ð¿ØÖÆÉÏÏß */
        public const int MINOR_REMOTE_PIN = 0xcd; /* Ô¶³ÌPIN¹¦ÄÜ²Ù×÷ */

        public const int MINOR_REMOTE_BYPASS = 0xd0; // Ô¶³ÌÅÔÂ· 
        public const int MINOR_REMOTE_UNBYPASS = 0xd1; // Ô¶³ÌÅÔÂ·»Ö¸´ 
        public const int MINOR_REMOTE_SET_ALARMIN_CFG = 0xd2; // Ô¶³ÌÉèÖÃ±¨¾¯ÊäÈë²ÎÊý 
        public const int MINOR_REMOTE_GET_ALARMIN_CFG = 0xd3; // Ô¶³Ì»ñÈ¡±¨¾¯ÊäÈë²ÎÊý 
        public const int MINOR_REMOTE_SET_ALARMOUT_CFG = 0xd4; // Ô¶³ÌÉèÖÃ±¨¾¯Êä³ö²ÎÊý 
        public const int MINOR_REMOTE_GET_ALARMOUT_CFG = 0xd5; // Ô¶³Ì»ñÈ¡±¨¾¯Êä³ö²ÎÊý 
        public const int MINOR_REMOTE_ALARMOUT_OPEN_MAN = 0xd6; // Ô¶³ÌÊÖ¶¯¿ªÆô±¨¾¯Êä³ö 
        public const int MINOR_REMOTE_ALARMOUT_CLOSE_MAN = 0xd7; // Ô¶³ÌÊÖ¶¯¹Ø±Õ±¨¾¯Êä³ö 
        public const int MINOR_REMOTE_ALARM_ENABLE_CFG = 0xd8; // Ô¶³ÌÉèÖÃ±¨¾¯Ö÷»úµÄRS485´®¿ÚÊ¹ÄÜ×´Ì¬ 
        public const int MINOR_DBDATA_OUTPUT = 0xd9; // µ¼³öÊý¾Ý¿â¼ÇÂ¼ 
        public const int MINOR_DBDATA_INPUT = 0xda; // µ¼ÈëÊý¾Ý¿â¼ÇÂ¼ 
        public const int MINOR_MU_SWITCH = 0xdb; // ¼¶ÁªÇÐ»» 
        public const int MINOR_MU_PTZ = 0xdc; // ¼¶ÁªPTZ¿ØÖÆ
        public const int MINOR_DELETE_LOGO = 0xdd; /* É¾³ýlogo */

        public const int MINOR_LOCAL_CONF_REB_RAID = 0x101; // ±¾µØÅäÖÃ×Ô¶¯ÖØ½¨ 
        public const int MINOR_LOCAL_CONF_SPARE = 0x102; // ±¾µØÅäÖÃÈÈ±¸ 
        public const int MINOR_LOCAL_ADD_RAID = 0x103; // ±¾µØ´´½¨ÕóÁÐ 
        public const int MINOR_LOCAL_DEL_RAID = 0x104; // ±¾µØÉ¾³ýÕóÁÐ 
        public const int MINOR_LOCAL_MIG_RAID = 0x105; // ±¾µØÇ¨ÒÆÕóÁÐ 
        public const int MINOR_LOCAL_REB_RAID = 0x106; // ±¾µØÊÖ¶¯ÖØ½¨ÕóÁÐ 
        public const int MINOR_LOCAL_QUICK_CONF_RAID = 0x107; // ±¾µØÒ»¼üÅäÖÃ 
        public const int MINOR_LOCAL_ADD_VD = 0x108; // ±¾µØ´´½¨ÐéÄâ´ÅÅÌ 
        public const int MINOR_LOCAL_DEL_VD = 0x109; // ±¾µØÉ¾³ýÐéÄâ´ÅÅÌ 
        public const int MINOR_LOCAL_RP_VD = 0x10a; // ±¾µØÐÞ¸´ÐéÄâ´ÅÅÌ 
        public const int MINOR_LOCAL_FORMAT_EXPANDVD = 0x10b; // ±¾µØÀ©Õ¹ÐéÄâ´ÅÅÌÀ©ÈÝ 
        public const int MINOR_LOCAL_RAID_UPGRADE = 0x10c; // ±¾µØraid¿¨Éý¼¶ 
        public const int MINOR_LOCAL_STOP_RAID = 0x10d; // ±¾µØÔÝÍ£RAID²Ù×÷(¼´°²È«°ÎÅÌ) 
        public const int MINOR_REMOTE_CONF_REB_RAID = 0x111; // Ô¶³ÌÅäÖÃ×Ô¶¯ÖØ½¨ 
        public const int MINOR_REMOTE_CONF_SPARE = 0x112; // Ô¶³ÌÅäÖÃÈÈ±¸ 
        public const int MINOR_REMOTE_ADD_RAID = 0x113; // Ô¶³Ì´´½¨ÕóÁÐ 
        public const int MINOR_REMOTE_DEL_RAID = 0x114; // Ô¶³ÌÉ¾³ýÕóÁÐ 
        public const int MINOR_REMOTE_MIG_RAID = 0x115; // Ô¶³ÌÇ¨ÒÆÕóÁÐ 
        public const int MINOR_REMOTE_REB_RAID = 0x116; // Ô¶³ÌÊÖ¶¯ÖØ½¨ÕóÁÐ 
        public const int MINOR_REMOTE_QUICK_CONF_RAID = 0x117; // Ô¶³ÌÒ»¼üÅäÖÃ 
        public const int MINOR_REMOTE_ADD_VD = 0x118; // Ô¶³Ì´´½¨ÐéÄâ´ÅÅÌ 
        public const int MINOR_REMOTE_DEL_VD = 0x119; // Ô¶³ÌÉ¾³ýÐéÄâ´ÅÅÌ 
        public const int MINOR_REMOTE_RP_VD = 0x11a; // Ô¶³ÌÐÞ¸´ÐéÄâ´ÅÅÌ 
        public const int MINOR_REMOTE_FORMAT_EXPANDVD = 0x11b; // Ô¶³ÌÐéÄâ´ÅÅÌÀ©ÈÝ 
        public const int MINOR_REMOTE_RAID_UPGRADE = 0x11c; // Ô¶³Ìraid¿¨Éý¼¶ 
        public const int MINOR_REMOTE_STOP_RAID = 0x11d; // Ô¶³ÌÔÝÍ£RAID²Ù×÷(¼´°²È«°ÎÅÌ) 
        public const int MINOR_LOCAL_START_PIC_REC = 0x121; // ±¾µØ¿ªÊ¼×¥Í¼ 
        public const int MINOR_LOCAL_STOP_PIC_REC = 0x122; // ±¾µØÍ£Ö¹×¥Í¼ 
        public const int MINOR_LOCAL_SET_SNMP = 0x125; // ±¾µØÅäÖÃSNMP 
        public const int MINOR_LOCAL_TAG_OPT = 0x126; // ±¾µØ±êÇ©²Ù×÷ 
        public const int MINOR_REMOTE_START_PIC_REC = 0x131; // Ô¶³Ì¿ªÊ¼×¥Í¼ 
        public const int MINOR_REMOTE_STOP_PIC_REC = 0x132; // Ô¶³ÌÍ£Ö¹×¥Í¼ 
        public const int MINOR_REMOTE_SET_SNMP = 0x135; // Ô¶³ÌÅäÖÃSNMP 
        public const int MINOR_REMOTE_TAG_OPT = 0x136; // Ô¶³Ì±êÇ©²Ù×÷ 

        public const int MINOR_LOCAL_VOUT_SWITCH = 0x140; // ±¾µØÊä³ö¿ÚÇÐ»»²Ù×÷ 
        public const int MINOR_STREAM_CABAC = 0x141; // ÂëÁ÷Ñ¹ËõÐÔÄÜÑ¡ÏîÅäÖÃ²Ù×÷ 

        public const int MINOR_LOCAL_SPARE_OPT = 0x142;   /*±¾µØN+1 ÈÈ±¸Ïà¹Ø²Ù×÷*/
        public const int MINOR_REMOTE_SPARE_OPT = 0x143;   /*Ô¶³ÌN+1 ÈÈ±¸Ïà¹Ø²Ù×÷*/
        public const int MINOR_LOCAL_IPCCFGFILE_OUTPUT = 0x144;     /* ±¾µØµ¼³öipcÅäÖÃÎÄ¼þ*/
        public const int MINOR_LOCAL_IPCCFGFILE_INPUT = 0x145;   /* ±¾µØµ¼ÈëipcÅäÖÃÎÄ¼þ */
        public const int MINOR_LOCAL_IPC_UPGRADE = 0x146;   /* ±¾µØÉý¼¶IPC */
        public const int MINOR_REMOTE_IPCCFGFILE_OUTPUT = 0x147;   /* Ô¶³Ìµ¼³öipcÅäÖÃÎÄ¼þ*/
        public const int MINOR_REMOTE_IPCCFGFILE_INPUT = 0x148;   /* Ô¶³Ìµ¼ÈëipcÅäÖÃÎÄ¼þ*/
        public const int MINOR_REMOTE_IPC_UPGRADE = 0x149;   /* Ô¶³ÌÉý¼¶IPC */

        public const int MINOR_SET_MULTI_MASTER = 0x201; // ÉèÖÃ´óÆÁÖ÷ÆÁ 
        public const int MINOR_SET_MULTI_SLAVE = 0x202; // ÉèÖÃ´óÆÁ×ÓÆÁ 
        public const int MINOR_CANCEL_MULTI_MASTER = 0x203; // È¡Ïû´óÆÁÖ÷ÆÁ 
        public const int MINOR_CANCEL_MULTI_SLAVE = 0x204; // È¡Ïû´óÆÁ×ÓÆÁ 

        public const int MINOR_DISPLAY_LOGO = 0x205;    /*ÏÔÊ¾LOGO*/
        public const int MINOR_HIDE_LOGO = 0x206;    /*Òþ²ØLOGO*/
        public const int MINOR_SET_DEC_DELAY_LEVEL = 0x207;    /*½âÂëÍ¨µÀÑÓÊ±¼¶±ðÉèÖÃ*/
        public const int MINOR_SET_BIGSCREEN_DIPLAY_AREA = 0x208;    /*ÉèÖÃ´óÆÁÏÔÊ¾ÇøÓò*/
        public const int MINOR_CUT_VIDEO_SOURCE = 0x209;    /*´óÆÁÊÓÆµÔ´ÇÐ¸îÉèÖÃ*/
        public const int MINOR_SET_BASEMAP_AREA = 0x210;    /*´óÆÁµ×Í¼ÇøÓòÉèÖÃ*/
        public const int MINOR_DOWNLOAD_BASEMAP = 0x211;    /*ÏÂÔØ´óÆÁµ×Í¼*/
        public const int MINOR_CUT_BASEMAP = 0x212;    /*µ×Í¼ÇÐ¸îÅäÖÃ*/
        public const int MINOR_CONTROL_ELEC_ENLARGE = 0x213;    /*µç×Ó·Å´ó²Ù×÷(·Å´ó»ò»¹Ô­)*/
        public const int MINOR_SET_OUTPUT_RESOLUTION = 0x214;    /*ÏÔÊ¾Êä³ö·Ö±æÂÊÉèÖÃ*/
        public const int MINOR_SET_TRANCSPARENCY = 0X215;    /*Í¼²ãÍ¸Ã÷¶ÈÉèÖÃ*/
        public const int MINOR_SET_OSD = 0x216;    /*ÏÔÊ¾OSDÉèÖÃ*/
        public const int MINOR_RESTORE_DEC_STATUS = 0x217;    /*»Ö¸´³õÊ¼×´Ì¬(³¡¾°ÇÐ»»Ê±£¬½âÂë»Ö¸´³õÊ¼×´Ì¬)*/

        public const int MINOR_SCREEN_SET_INPUT = 0x251; // ÐÞ¸ÄÊäÈëÔ´ 
        public const int MINOR_SCREEN_SET_OUTPUT = 0x252; // ÐÞ¸ÄÊä³öÍ¨µÀ 
        public const int MINOR_SCREEN_SET_OSD = 0x253; // ÐÞ¸ÄÐéÄâLED 
        public const int MINOR_SCREEN_SET_LOGO = 0x254; // ÐÞ¸ÄLOGO 
        public const int MINOR_SCREEN_SET_LAYOUT = 0x255; // ÉèÖÃ³¡¾° 
        public const int MINOR_SCREEN_PICTUREPREVIEW = 0x256; // »ØÏÔ²Ù×÷ 

        public const int MINOR_SCREEN_GET_OSD = 0x257; // »ñÈ¡ÐéÄâLED 
        public const int MINOR_SCREEN_GET_LAYOUT = 0x258; // »ñÈ¡³¡¾° 
        public const int MINOR_SCREEN_LAYOUT_CTRL = 0x259; // ³¡¾°¿ØÖÆ 
        public const int MINOR_GET_ALL_VALID_WND = 0x260; // »ñÈ¡ËùÓÐÓÐÐ§´°¿Ú 
        public const int MINOR_GET_SIGNAL_WND = 0x261; // »ñÈ¡µ¥¸ö´°¿ÚÐÅÏ¢ 
        public const int MINOR_WINDOW_CTRL = 0x262; // ´°¿Ú¿ØÖÆ 
        public const int MINOR_GET_LAYOUT_LIST = 0x263; // »ñÈ¡³¡¾°ÁÐ±í 
        public const int MINOR_LAYOUT_CTRL = 0x264; // ³¡¾°¿ØÖÆ 
        public const int MINOR_SET_LAYOUT = 0x265; // ÉèÖÃµ¥¸ö³¡¾° 
        public const int MINOR_GET_SIGNAL_LIST = 0x266; // »ñÈ¡ÊäÈëÐÅºÅÔ´ÁÐ±í 
        public const int MINOR_GET_PLAN_LIST = 0x267; // »ñÈ¡Ô¤°¸ÁÐ±í 
        public const int MINOR_SET_PLAN = 0x268; // ÐÞ¸ÄÔ¤°¸ 
        public const int MINOR_CTRL_PLAN = 0x269; // ¿ØÖÆÔ¤°¸ 
        public const int MINOR_CTRL_SCREEN = 0x270; // ÆÁÄ»¿ØÖÆ 
        public const int MINOR_ADD_NETSIG = 0x271; // Ìí¼ÓÐÅºÅÔ´ 
        public const int MINOR_SET_NETSIG = 0x272; // ÐÞ¸ÄÐÅºÅÔ´ 
        public const int MINOR_SET_DECBDCFG = 0x273; // ÉèÖÃ½âÂë°å²ÎÊý 
        public const int MINOR_GET_DECBDCFG = 0x274; // »ñÈ¡½âÂë°å²ÎÊý 
        public const int MINOR_GET_DEVICE_STATUS = 0x275; // »ñÈ¡Éè±¸ÐÅÏ¢ 
        public const int MINOR_UPLOAD_PICTURE = 0x276; // µ×Í¼ÉÏ´« 
        public const int MINOR_SET_USERPWD = 0x277; // ÉèÖÃÓÃ»§ÃÜÂë 
        public const int MINOR_ADD_LAYOUT = 0x278; // Ìí¼Ó³¡¾° 
        public const int MINOR_DEL_LAYOUT = 0x279; // É¾³ý³¡¾° 
        public const int MINOR_DEL_NETSIG = 0x280; // É¾³ýÐÅºÅÔ´ 
        public const int MINOR_ADD_PLAN = 0x281; // Ìí¼ÓÔ¤°¸ 
        public const int MINOR_DEL_PLAN = 0x282; // É¾³ýÔ¤°¸ 
        public const int MINOR_GET_EXTERNAL_MATRIX_CFG = 0x283; // »ñÈ¡Íâ½Ó¾ØÕóÅäÖÃ 
        public const int MINOR_SET_EXTERNAL_MATRIX_CFG = 0x284; // ÉèÖÃÍâ½Ó¾ØÕóÅäÖÃ 
        public const int MINOR_GET_USER_CFG = 0x285; // »ñÈ¡ÓÃ»§ÅäÖÃ 
        public const int MINOR_SET_USER_CFG = 0x286; // ÉèÖÃÓÃ»§ÅäÖÃ 
        public const int MINOR_GET_DISPLAY_PANEL_LINK_CFG = 0x287; // »ñÈ¡ÏÔÊ¾Ç½Á¬½ÓÅäÖÃ 
        public const int MINOR_SET_DISPLAY_PANEL_LINK_CFG = 0x288; // ÉèÖÃÏÔÊ¾Ç½Á¬½ÓÅäÖÃ 

        public const int MINOR_GET_WALLSCENE_PARAM = 0x289; // »ñÈ¡µçÊÓÇ½³¡¾° 
        public const int MINOR_SET_WALLSCENE_PARAM = 0x28a; // ÉèÖÃµçÊÓÇ½³¡¾° 
        public const int MINOR_GET_CURRENT_WALLSCENE = 0x28b; // »ñÈ¡µ±Ç°Ê¹ÓÃ³¡¾° 
        public const int MINOR_SWITCH_WALLSCENE = 0x28c; // ³¡¾°ÇÐ»» 
        public const int MINOR_SIP_LOGIN = 0x28d; //SIP×¢²á³É¹¦
        public const int MINOR_VOIP_START = 0x28e; //VOIP¶Ô½²¿ªÊ¼
        public const int MINOR_VOIP_STOP = 0x28f; //VOIP¶Ô½²Í£Ö¹
        public const int MINOR_WIN_TOP = 0x290; //µçÊÓÇ½´°¿ÚÖÃ¶¥
        public const int MINOR_WIN_BOTTOM = 0x291; //µçÊÓÇ½´°¿ÚÖÃµ×

        // Netra 2.2.2
        public const int MINOR_LOCAL_LOAD_HDISK = 0x300; // ±¾µØ¼ÓÔØÓ²ÅÌ 
        public const int MINOR_LOCAL_DELETE_HDISK = 0x301; // ±¾µØÉ¾³ýÒì³£²»´æÔÚµÄÓ²ÅÌ

        //Netra3.1.0
        public const int MINOR_LOCAL_CFG_DEVICE_TYPE = 0x310; //±¾µØÅäÖÃÉè±¸ÀàÐÍ
        public const int MINOR_REMOTE_CFG_DEVICE_TYPE = 0x311; //Ô¶³ÌÅäÖÃÉè±¸ÀàÐÍ
        public const int MINOR_LOCAL_CFG_WORK_HOT_SERVER = 0x312; //±¾µØÅäÖÃ¹¤×÷»úÈÈ±¸·þÎñÆ÷
        public const int MINOR_REMOTE_CFG_WORK_HOT_SERVER = 0x313; //Ô¶³ÌÅäÖÃ¹¤×÷»úÈÈ±¸·þÎñÆ÷
        public const int MINOR_LOCAL_DELETE_WORK = 0x314; //±¾µØÉ¾³ý¹¤×÷»ú
        public const int MINOR_REMOTE_DELETE_WORK = 0x315; //Ô¶³ÌÉ¾³ý¹¤×÷»ú
        public const int MINOR_LOCAL_ADD_WORK = 0x316; //±¾µØÌí¼Ó¹¤×÷»ú
        public const int MINOR_REMOTE_ADD_WORK = 0x317; //Ô¶³ÌÌí¼Ó¹¤×÷»ú
        public const int MINOR_LOCAL_IPCHEATMAP_OUTPUT = 0x318; /* ±¾µØµ¼³öÈÈ¶ÈÍ¼ÎÄ¼þ      */
        public const int MINOR_LOCAL_IPCHEATFLOW_OUTPUT = 0x319; /* ±¾µØµ¼³öÈÈ¶ÈÁ÷Á¿ÎÄ¼þ      */
        public const int MINOR_REMOTE_SMS_SEND = 0x350; /*Ô¶³Ì·¢ËÍ¶ÌÐÅ*/
        public const int MINOR_LOCAL_SMS_SEND = 0x351; /*±¾µØ·¢ËÍ¶ÌÐÅ*/
        public const int MINOR_ALARM_SMS_SEND = 0x352; /*·¢ËÍ¶ÌÐÅ±¨¾¯*/
        public const int MINOR_SMS_RECV = 0x353; /*½ÓÊÕ¶ÌÐÅ*/
        //£¨±¸×¢£º0x350¡¢0x351ÊÇÖ¸ÈË¹¤ÔÚGUI»òIE¿Ø¼þÉÏ±à¼­²¢·¢ËÍ¶ÌÐÅ£©
        public const int MINOR_LOCAL_SMS_SEARCH = 0x354; /*±¾µØËÑË÷¶ÌÐÅ*/
        public const int MINOR_REMOTE_SMS_SEARCH = 0x355; /*Ô¶³ÌËÑË÷¶ÌÐÅ*/
        public const int MINOR_LOCAL_SMS_READ = 0x356; /*±¾µØ²é¿´¶ÌÐÅ*/
        public const int MINOR_REMOTE_SMS_READ = 0x357; /*Ô¶³Ì²é¿´¶ÌÐÅ*/
        public const int MINOR_REMOTE_DIAL_CONNECT = 0x358; /*Ô¶³Ì¿ªÆôÊÖ¶¯²¦ºÅ*/
        public const int MINOR_REMOTE_DIAL_DISCONN = 0x359; /*Ô¶³ÌÍ£Ö¹ÊÖ¶¯²¦ºÅ*/
        public const int MINOR_LOCAL_WHITELIST_SET = 0x35A; /*±¾µØÅäÖÃ°×Ãûµ¥*/
        public const int MINOR_REMOTE_WHITELIST_SET = 0x35B; /*Ô¶³ÌÅäÖÃ°×Ãûµ¥*/
        public const int MINOR_LOCAL_DIAL_PARA_SET = 0x35C; /*±¾µØÅäÖÃ²¦ºÅ²ÎÊý*/
        public const int MINOR_REMOTE_DIAL_PARA_SET = 0x35D; /*Ô¶³ÌÅäÖÃ²¦ºÅ²ÎÊý*/
        public const int MINOR_LOCAL_DIAL_SCHEDULE_SET = 0x35E; /*±¾µØÅäÖÃ²¦ºÅ¼Æ»®*/
        public const int MINOR_REMOTE_DIAL_SCHEDULE_SET = 0x35F; /*Ô¶³ÌÅäÖÃ²¦ºÅ¼Æ»®*/
        public const int MINOR_PLAT_OPER = 0x36; /* Æ½Ì¨²Ù×÷*/

        public const int MINOR_REMOTE_OPEN_DOOR = 0x400; // Ô¶³Ì¿ªÃÅ 
        public const int MINOR_REMOTE_CLOSE_DOOR = 0x401; // Ô¶³Ì¹ØÃÅ 
        public const int MINOR_REMOTE_ALWAYS_OPEN = 0x402; // Ô¶³Ì³£¿ª 
        public const int MINOR_REMOTE_ALWAYS_CLOSE = 0x403; // Ô¶³Ì³£¹Ø 
        public const int MINOR_REMOTE_CHECK_TIME = 0x404; // Ô¶³ÌÊÖ¶¯Ð£Ê± 
        public const int MINOR_NTP_CHECK_TIME = 0x405; // NTP×Ô¶¯Ð£Ê± 
        public const int MINOR_REMOTE_CLEAR_CARD = 0x406; // Ô¶³ÌÇå¿Õ¿¨ºÅ 
        public const int MINOR_REMOTE_RESTORE_CFG = 0x407; // Ô¶³Ì»Ö¸´Ä¬ÈÏ²ÎÊý 
        public const int MINOR_ALARMIN_ARM = 0x408; // ·ÀÇø²¼·À 
        public const int MINOR_ALARMIN_DISARM = 0x409; // ·ÀÇø³··À 
        public const int MINOR_LOCAL_RESTORE_CFG = 0x40a; // ±¾µØ»Ö¸´Ä¬ÈÏ²ÎÊý 
        public const int MINOR_REMOTE_CAPTURE_PIC = 0x40b; //Ô¶³Ì×¥ÅÄ
        public const int MINOR_MOD_NET_REPORT_CFG = 0x40c; //ÐÞ¸ÄÍøÂçÖÐÐÄ²ÎÊýÅäÖÃ
        public const int MINOR_MOD_GPRS_REPORT_PARAM = 0x40d; //ÐÞ¸ÄGPRSÖÐÐÄ²ÎÊýÅäÖÃ
        public const int MINOR_MOD_REPORT_GROUP_PARAM = 0x40e; //ÐÞ¸ÄÖÐÐÄ×é²ÎÊýÅäÖÃ
        public const int MINOR_UNLOCK_PASSWORD_OPEN_DOOR = 0x40f; //½â³ýÂëÊäÈë

        public const int MINOR_SET_TRIGGERMODE_CFG = 0x1001; // ÉèÖÃ´¥·¢Ä£Ê½²ÎÊý 
        public const int MINOR_GET_TRIGGERMODE_CFG = 0x1002; // »ñÈ¡´¥·¢Ä£Ê½²ÎÊý 
        public const int MINOR_SET_IOOUT_CFG = 0x1003; // ÉèÖÃIOÊä³ö²ÎÊý 
        public const int MINOR_GET_IOOUT_CFG = 0x1004; // »ñÈ¡IOÊä³ö²ÎÊý 
        public const int MINOR_GET_TRIGGERMODE_DEFAULT = 0x1005; // »ñÈ¡´¥·¢Ä£Ê½ÍÆ¼ö²ÎÊý 
        public const int MINOR_GET_ITCSTATUS = 0x1006; // »ñÈ¡×´Ì¬¼ì²â²ÎÊý 
        public const int MINOR_SET_STATUS_DETECT_CFG = 0x1007; // ÉèÖÃ×´Ì¬¼ì²â²ÎÊý 
        public const int MINOR_GET_STATUS_DETECT_CFG = 0x1008; // »ñÈ¡×´Ì¬¼ì²â²ÎÊý 
        public const int MINOR_GET_VIDEO_TRIGGERMODE_CFG = 0x1009; // »ñÈ¡ÊÓÆµµç¾¯Ä£Ê½²ÎÊý 
        public const int MINOR_SET_VIDEO_TRIGGERMODE_CFG = 0x100a; // ÉèÖÃÊÓÆµµç¾¯Ä£Ê½²ÎÊý 

        public const int MINOR_LOCAL_ADD_CAR_INFO = 0x2001; // ±¾µØÌí¼Ó³µÁ¾ÐÅÏ¢ 
        public const int MINOR_LOCAL_MOD_CAR_INFO = 0x2002; // ±¾µØÐÞ¸Ä³µÁ¾ÐÅÏ¢ 
        public const int MINOR_LOCAL_DEL_CAR_INFO = 0x2003; // ±¾µØÉ¾³ý³µÁ¾ÐÅÏ¢ 
        public const int MINOR_LOCAL_FIND_CAR_INFO = 0x2004; // ±¾µØ²éÕÒ³µÁ¾ÐÅÏ¢ 
        public const int MINOR_LOCAL_ADD_MONITOR_INFO = 0x2005; // ±¾µØÌí¼Ó²¼¿ØÐÅÏ¢ 
        public const int MINOR_LOCAL_MOD_MONITOR_INFO = 0x2006; // ±¾µØÐÞ¸Ä²¼¿ØÐÅÏ¢ 
        public const int MINOR_LOCAL_DEL_MONITOR_INFO = 0x2007; // ±¾µØÉ¾³ý²¼¿ØÐÅÏ¢ 
        public const int MINOR_LOCAL_FIND_MONITOR_INFO = 0x2008; // ±¾µØ²éÑ¯²¼¿ØÐÅÏ¢ 
        public const int MINOR_LOCAL_FIND_NORMAL_PASS_INFO = 0x2009; // ±¾µØ²éÑ¯Õý³£Í¨ÐÐÐÅÏ¢ 
        public const int MINOR_LOCAL_FIND_ABNORMAL_PASS_INFO = 0x200a; // ±¾µØ²éÑ¯Òì³£Í¨ÐÐÐÅÏ¢ 
        public const int MINOR_LOCAL_FIND_PEDESTRIAN_PASS_INFO = 0x200b; // ±¾µØ²éÑ¯Õý³£Í¨ÐÐÐÅÏ¢ 
        public const int MINOR_LOCAL_PIC_PREVIEW = 0x200c; // ±¾µØÍ¼Æ¬Ô¤ÀÀ 
        public const int MINOR_LOCAL_SET_GATE_PARM_CFG = 0x200d; // ÉèÖÃ±¾µØÅäÖÃ³öÈë¿Ú²ÎÊý 
        public const int MINOR_LOCAL_GET_GATE_PARM_CFG = 0x200e; // »ñÈ¡±¾µØÅäÖÃ³öÈë¿Ú²ÎÊý 
        public const int MINOR_LOCAL_SET_DATAUPLOAD_PARM_CFG = 0x200f; // ÉèÖÃ±¾µØÅäÖÃÊý¾ÝÉÏ´«²ÎÊý 
        public const int MINOR_LOCAL_GET_DATAUPLOAD_PARM_CFG = 0x2010; // »ñÈ¡±¾µØÅäÖÃÊý¾ÝÉÏ´«²ÎÊý 

        public const int MINOR_LOCAL_DEVICE_CONTROL = 0x2011; // ±¾µØÉè±¸¿ØÖÆ(±¾µØ¿ª¹ØÕ¢) 
        public const int MINOR_LOCAL_ADD_EXTERNAL_DEVICE_INFO = 0x2012; // ±¾µØÌí¼ÓÍâ½ÓÉè±¸ÐÅÏ¢ 
        public const int MINOR_LOCAL_MOD_EXTERNAL_DEVICE_INFO = 0x2013; // ±¾µØÐÞ¸ÄÍâ½ÓÉè±¸ÐÅÏ¢ 
        public const int MINOR_LOCAL_DEL_EXTERNAL_DEVICE_INFO = 0x2014; // ±¾µØÉ¾³ýÍâ½ÓÉè±¸ÐÅÏ¢ 
        public const int MINOR_LOCAL_FIND_EXTERNAL_DEVICE_INFO = 0x2015; // ±¾µØ²éÑ¯Íâ½ÓÉè±¸ÐÅÏ¢ 
        public const int MINOR_LOCAL_ADD_CHARGE_RULE = 0x2016; // ±¾µØÌí¼ÓÊÕ·Ñ¹æÔò 
        public const int MINOR_LOCAL_MOD_CHARGE_RULE = 0x2017; // ±¾µØÐÞ¸ÄÊÕ·Ñ¹æÔò 
        public const int MINOR_LOCAL_DEL_CHARGE_RULE = 0x2018; // ±¾µØÉ¾³ýÊÕ·Ñ¹æÔò 
        public const int MINOR_LOCAL_FIND_CHARGE_RULE = 0x2019; // ±¾µØ²éÑ¯ÊÕ·Ñ¹æÔò 
        public const int MINOR_LOCAL_COUNT_NORMAL_CURRENTINFO = 0x2020; // ±¾µØÍ³¼ÆÕý³£Í¨ÐÐÐÅÏ¢ 
        public const int MINOR_LOCAL_EXPORT_NORMAL_CURRENTINFO_REPORT = 0x2021; // ±¾µØµ¼³öÕý³£Í¨ÐÐÐÅÏ¢Í³¼Æ±¨±í 
        public const int MINOR_LOCAL_COUNT_ABNORMAL_CURRENTINFO = 0x2022; // ±¾µØÍ³¼ÆÒì³£Í¨ÐÐÐÅÏ¢ 
        public const int MINOR_LOCAL_EXPORT_ABNORMAL_CURRENTINFO_REPORT = 0x2023; // ±¾µØµ¼³öÒì³£Í¨ÐÐÐÅÏ¢Í³¼Æ±¨±í 
        public const int MINOR_LOCAL_COUNT_PEDESTRIAN_CURRENTINFO = 0x2024; // ±¾µØÍ³¼ÆÐÐÈËÍ¨ÐÐÐÅÏ¢ 
        public const int MINOR_LOCAL_EXPORT_PEDESTRIAN_CURRENTINFO_REPORT = 0x2025; // ±¾µØµ¼³öÐÐÈËÍ¨ÐÐÐÅÏ¢Í³¼Æ±¨±í 
        public const int MINOR_LOCAL_FIND_CAR_CHARGEINFO = 0x2026; // ±¾µØ²éÑ¯¹ý³µÊÕ·ÑÐÅÏ¢ 
        public const int MINOR_LOCAL_COUNT_CAR_CHARGEINFO = 0x2027; // ±¾µØÍ³¼Æ¹ý³µÊÕ·ÑÐÅÏ¢ 
        public const int MINOR_LOCAL_EXPORT_CAR_CHARGEINFO_REPORT = 0x2028; // ±¾µØµ¼³ö¹ý³µÊÕ·ÑÐÅÏ¢Í³¼Æ±¨±í 
        public const int MINOR_LOCAL_FIND_SHIFTINFO = 0x2029; // ±¾µØ²éÑ¯½»½Ó°àÐÅÏ¢ 
        public const int MINOR_LOCAL_FIND_CARDINFO = 0x2030; // ±¾µØ²éÑ¯¿¨Æ¬ÐÅÏ¢ 
        public const int MINOR_LOCAL_ADD_RELIEF_RULE = 0x2031; // ±¾µØÌí¼Ó¼õÃâ¹æÔò 
        public const int MINOR_LOCAL_MOD_RELIEF_RULE = 0x2032; // ±¾µØÐÞ¸Ä¼õÃâ¹æÔò 
        public const int MINOR_LOCAL_DEL_RELIEF_RULE = 0x2033; // ±¾µØÉ¾³ý¼õÃâ¹æÔò 
        public const int MINOR_LOCAL_FIND_RELIEF_RULE = 0x2034; // ±¾µØ²éÑ¯¼õÃâ¹æÔò 
        public const int MINOR_LOCAL_GET_ENDETCFG = 0x2035; // ±¾µØ»ñÈ¡³öÈë¿Ú¿ØÖÆ»úÀëÏß¼ì²âÅäÖÃ 
        public const int MINOR_LOCAL_SET_ENDETCFG = 0x2036; // ±¾µØÉèÖÃ³öÈë¿Ú¿ØÖÆ»úÀëÏß¼ì²âÅäÖÃ 
        public const int MINOR_LOCAL_SET_ENDEV_ISSUEDDATA = 0x2037; // ±¾µØÉèÖÃ³öÈë¿Ú¿ØÖÆ»úÏÂ·¢¿¨Æ¬ÐÅÏ¢ 
        public const int MINOR_LOCAL_DEL_ENDEV_ISSUEDDATA = 0x2038; // ±¾µØÇå¿Õ³öÈë¿Ú¿ØÖÆ»úÏÂ·¢¿¨Æ¬ÐÅÏ¢ 
        public const int MINOR_REMOTE_DEVICE_CONTROL = 0x2101; // Ô¶³ÌÉè±¸¿ØÖÆ 
        public const int MINOR_REMOTE_SET_GATE_PARM_CFG = 0x2102; // ÉèÖÃÔ¶³ÌÅäÖÃ³öÈë¿Ú²ÎÊý 
        public const int MINOR_REMOTE_GET_GATE_PARM_CFG = 0x2103; // »ñÈ¡Ô¶³ÌÅäÖÃ³öÈë¿Ú²ÎÊý 
        public const int MINOR_REMOTE_SET_DATAUPLOAD_PARM_CFG = 0x2104; // ÉèÖÃÔ¶³ÌÅäÖÃÊý¾ÝÉÏ´«²ÎÊý 
        public const int MINOR_REMOTE_GET_DATAUPLOAD_PARM_CFG = 0x2105; // »ñÈ¡Ô¶³ÌÅäÖÃÊý¾ÝÉÏ´«²ÎÊý 
        public const int MINOR_REMOTE_GET_BASE_INFO = 0x2106; // Ô¶³Ì»ñÈ¡ÖÕ¶Ë»ù±¾ÐÅÏ¢ 
        public const int MINOR_REMOTE_GET_OVERLAP_CFG = 0x2107; // Ô¶³Ì»ñÈ¡×Ö·ûµþ¼Ó²ÎÊýÅäÖÃ 
        public const int MINOR_REMOTE_SET_OVERLAP_CFG = 0x2108; // Ô¶³ÌÉèÖÃ×Ö·ûµþ¼Ó²ÎÊýÅäÖÃ 
        public const int MINOR_REMOTE_GET_ROAD_INFO = 0x2109; // Ô¶³Ì»ñÈ¡Â·¿ÚÐÅÏ¢ 
        public const int MINOR_REMOTE_START_TRANSCHAN = 0x210a; // Ô¶³Ì½¨Á¢Í¬²½Êý¾Ý·þÎñÆ÷ 
        public const int MINOR_REMOTE_GET_ECTWORKSTATE = 0x210b; // Ô¶³Ì»ñÈ¡³öÈë¿ÚÖÕ¶Ë¹¤×÷×´Ì¬ 
        public const int MINOR_REMOTE_GET_ECTCHANINFO = 0x210c; // Ô¶³Ì»ñÈ¡³öÈë¿ÚÖÕ¶ËÍ¨µÀ×´Ì¬ 
        public const int MINOR_REMOTE_ADD_EXTERNAL_DEVICE_INFO = 0x210d; // Ô¶³ÌÌí¼ÓÍâ½ÓÉè±¸ÐÅÏ¢ 
        public const int MINOR_REMOTE_MOD_EXTERNAL_DEVICE_INFO = 0x210e; // Ô¶³ÌÐÞ¸ÄÍâ½ÓÉè±¸ÐÅÏ¢ 
        public const int MINOR_REMOTE_GET_ENDETCFG = 0x210f; // Ô¶³Ì»ñÈ¡³öÈë¿Ú¿ØÖÆ»úÀëÏß¼ì²âÅäÖÃ 
        public const int MINOR_REMOTE_SET_ENDETCFG = 0x2110; // Ô¶³ÌÉèÖÃ³öÈë¿Ú¿ØÖÆ»úÀëÏß¼ì²âÅäÖÃ 
        public const int MINOR_REMOTE_ENDEV_ISSUEDDATA = 0x2111; // Ô¶³ÌÉèÖÃ³öÈë¿Ú¿ØÖÆ»úÏÂ·¢¿¨Æ¬ÐÅÏ¢ 
        public const int MINOR_REMOTE_DEL_ENDEV_ISSUEDDATA = 0x2112; // Ô¶³ÌÇå¿Õ³öÈë¿Ú¿ØÖÆ»úÏÂ·¢¿¨Æ¬ÐÅÏ¢ 

        public const int MINOR_REMOTE_ON_CTRL_LAMP = 0x2115; // ¿ªÆôÔ¶³Ì¿ØÖÆ³µÎ»Ö¸Ê¾µÆ 
        public const int MINOR_REMOTE_OFF_CTRL_LAMP = 0x2116; // ¹Ø±ÕÔ¶³Ì¿ØÖÆ³µÎ»Ö¸Ê¾µÆ 
        public const int MINOR_SET_VOICE_LEVEL_PARAM = 0x2117; // ÉèÖÃÒôÁ¿´óÐ¡ 
        public const int MINOR_SET_VOICE_INTERCOM_PARAM = 0x2118; // ÉèÖÃÒôÁ¿Â¼Òô 
        public const int MINOR_SET_INTELLIGENT_PARAM = 0x2119; // ÖÇÄÜÅäÖÃ 
        public const int MINOR_LOCAL_SET_RAID_SPEED = 0x211a; // ±¾µØÉèÖÃraidËÙ¶È 
        public const int MINOR_REMOTE_SET_RAID_SPEED = 0x211b; // Ô¶³ÌÉèÖÃraidËÙ¶È 
        public const int MINOR_REMOTE_CREATE_STORAGE_POOL = 0x211c; // Ô¶³ÌÌí¼Ó´æ´¢³Ø 
        public const int MINOR_REMOTE_DEL_STORAGE_POOL = 0x211d; // Ô¶³ÌÉ¾³ý´æ´¢³Ø 

        public const int MINOR_REMOTE_DEL_PIC = 0x2120; // Ô¶³ÌÉ¾³ýÍ¼Æ¬Êý¾Ý 
        public const int MINOR_REMOTE_DEL_RECORD = 0x2121; // Ô¶³ÌÉ¾³ýÂ¼ÏñÊý¾Ý 
        public const int MINOR_REMOTE_CLOUD_ENABLE = 0x2123; // Ô¶³ÌÉèÖÃÔÆ´æ´¢ÆôÓÃ 
        public const int MINOR_REMOTE_CLOUD_DISABLE = 0x2124; // Ô¶³ÌÉèÖÃÔÆ´æ´¢½ûÓÃ 
        public const int MINOR_REMOTE_CLOUD_MODIFY_PARAM = 0x2125; // Ô¶³ÌÐÞ¸ÄÔÆ´æ´¢³Ø²ÎÊý 
        public const int MINOR_REMOTE_CLOUD_MODIFY_VOLUME = 0x2126; // Ô¶³ÌÐÞ¸ÄÔÆ´æ´¢³ØÈÝÁ¿ 
        public const int MINOR_REMOTE_GET_GB28181_SERVICE_PARAM = 0x2127; //Ô¶³Ì»ñÈ¡GB28181·þÎñ²ÎÊý
        public const int MINOR_REMOTE_SET_GB28181_SERVICE_PARAM = 0x2128; //Ô¶³ÌÉèÖÃGB28181·þÎñ²ÎÊý
        public const int MINOR_LOCAL_GET_GB28181_SERVICE_PARAM = 0x2129; //±¾µØ»ñÈ¡GB28181·þÎñ²ÎÊý
        public const int MINOR_LOCAL_SET_GB28181_SERVICE_PARAM = 0x212a; //±¾µØÅäÖÃB28181·þÎñ²ÎÊý
        public const int MINOR_REMOTE_SET_SIP_SERVER = 0x212b; //Ô¶³ÌÅäÖÃSIP SERVER
        public const int MINOR_LOCAL_SET_SIP_SERVER = 0x212c; //±¾µØÅäÖÃSIP SERVER
        public const int MINOR_LOCAL_BLACKWHITEFILE_OUTPUT = 0x212d; //±¾µØºÚ°×Ãûµ¥µ¼³ö
        public const int MINOR_LOCAL_BLACKWHITEFILE_INPUT = 0x212e; //±¾µØºÚ°×Ãûµ¥µ¼Èë
        public const int MINOR_REMOTE_BALCKWHITECFGFILE_OUTPUT = 0x212f; //Ô¶³ÌºÚ°×Ãûµ¥µ¼³ö
        public const int MINOR_REMOTE_BALCKWHITECFGFILE_INPUT = 0x2130; //Ô¶³ÌºÚ°×Ãûµ¥µ¼Èë

        public const int MINOR_REMOTE_CREATE_MOD_VIEWLIB_SPACE = 0x2200; // Ô¶³Ì´´½¨/ÐÞ¸ÄÊÓÍ¼¿â¿Õ¼ä 
        public const int MINOR_REMOTE_DELETE_VIEWLIB_FILE = 0x2201; // Ô¶³ÌÉ¾³ýÊÓÍ¼¿âÎÄ¼þ 
        public const int MINOR_REMOTE_DOWNLOAD_VIEWLIB_FILE = 0x2202; // Ô¶³ÌÏÂÔØÊÓÍ¼¿âÎÄ¼þ 
        public const int MINOR_REMOTE_UPLOAD_VIEWLIB_FILE = 0x2203; // Ô¶³ÌÉÏ´«ÊÓÍ¼¿âÎÄ¼þ 
        public const int MINOR_LOCAL_CREATE_MOD_VIEWLIB_SPACE = 0x2204; // ±¾µØ´´½¨/ÐÞ¸ÄÊÓÍ¼¿â¿Õ¼ä 

        public const int MINOR_LOCAL_SET_DEVICE_ACTIVE = 0x3000; //±¾µØ¼¤»îÉè±¸
        public const int MINOR_REMOTE_SET_DEVICE_ACTIVE = 0x3001; //Ô¶³Ì¼¤»îÉè±¸
        public const int MINOR_LOCAL_PARA_FACTORY_DEFAULT = 0x3002; //±¾µØ»Ø¸´³ö³§ÉèÖÃ
        public const int MINOR_REMOTE_PARA_FACTORY_DEFAULT = 0x3003; //Ô¶³Ì»Ö¸´³ö³§ÉèÖÃ


        public const int NET_DVR_GET_THERMOMETRY_BASICPARAM_CAPABILITIES = 3620; //»ñÈ¡²âÎÂÅäÖÃÄÜÁ¦
        public const int NET_DVR_GET_THERMOMETRY_BASICPARAM = 3621;    //»ñÈ¡²âÎÂÅäÖÃ²ÎÊý
        public const int NET_DVR_SET_THERMOMETRY_BASICPARAM = 3622;    //ÉèÖÃ²âÎÂÅäÖÃ²ÎÊý
        public const int NET_DVR_GET_THERMOMETRY_SCENE_CAPABILITIES = 3623; //»ñÈ¡²âÎÂÔ¤ÖÃµã¹ØÁªÅäÖÃÄÜÁ¦
        public const int NET_DVR_GET_THERMOMETRY_PRESETINFO = 3624;    //»ñÈ¡²âÎÂÔ¤ÖÃµã¹ØÁªÅäÖÃ²ÎÊý
        public const int NET_DVR_SET_THERMOMETRY_PRESETINFO = 3625;    //ÉèÖÃ²âÎÂÔ¤ÖÃµã¹ØÁªÅäÖÃ²ÎÊý
        public const int NET_DVR_GET_THERMOMETRY_ALARMRULE_CAPABILITIES = 3626;//»ñÈ¡²âÎÂ±¨¾¯·½Ê½ÅäÖÃÄÜÁ¦
        public const int NET_DVR_GET_THERMOMETRY_ALARMRULE = 3627;    //»ñÈ¡²âÎÂÔ¤ÖÃµã±¨¾¯¹æÔòÅäÖÃ²ÎÊý
        public const int NET_DVR_SET_THERMOMETRY_ALARMRULE = 3628;    //ÉèÖÃ²âÎÂÔ¤ÖÃµã±¨¾¯¹æÔòÅäÖÃ²ÎÊý
        public const int NET_DVR_GET_REALTIME_THERMOMETRY = 3629;    //ÊµÊ±ÎÂ¶È¼ì²â
        public const int NET_DVR_GET_THERMOMETRY_DIFFCOMPARISON = 3630;    //»ñÈ¡²âÎÂÔ¤ÖÃµãÎÂ²î¹æÔòÅäÖÃ²ÎÊý
        public const int NET_DVR_SET_THERMOMETRY_DIFFCOMPARISON = 3631;    //ÉèÖÃ²âÎÂÔ¤ÖÃµãÎÂ²î¹æÔòÅäÖÃ²ÎÊý
        public const int NET_DVR_GET_THERMOMETRY_TRIGGER = 3632;    //»ñÈ¡²âÎÂÁª¶¯ÅäÖÃ
        public const int NET_DVR_SET_THERMOMETRY_TRIGGER = 3633;    //ÉèÖÃ²âÎÂÁª¶¯ÅäÖÃ


        /*ÈÕÖ¾¸½¼ÓÐÅÏ¢*/
        //Ö÷ÀàÐÍ
        public const int MAJOR_INFORMATION = 4;/*¸½¼ÓÐÅÏ¢*/
        //´ÎÀàÐÍ
        public const int MINOR_HDD_INFO = 0xa1; // Ó²ÅÌÐÅÏ¢ 
        public const int MINOR_SMART_INFO = 0xa2; // S.M.A.R.TÐÅÏ¢ 
        public const int MINOR_REC_START = 0xa3; // ¿ªÊ¼Â¼Ïñ 
        public const int MINOR_REC_STOP = 0xa4; // Í£Ö¹Â¼Ïñ 
        public const int MINOR_REC_OVERDUE = 0xa5; // ¹ýÆÚÂ¼ÏñÉ¾³ý 
        public const int MINOR_LINK_START = 0xa6; // Á¬½ÓÇ°¶ËÉè±¸ 
        public const int MINOR_LINK_STOP = 0xa7; // ¶Ï¿ªÇ°¶ËÉè±¸ 
        public const int MINOR_NET_DISK_INFO = 0xa8; // ÍøÂçÓ²ÅÌÐÅÏ¢ 
        public const int MINOR_RAID_INFO = 0xa9; // raidÏà¹ØÐÅÏ¢ 
        public const int MINOR_RUN_STATUS_INFO = 0xaa; // ÏµÍ³ÔËÐÐ×´Ì¬ÐÅÏ¢ 
        public const int MINOR_SPARE_START_BACKUP = 0xab; // ÈÈ±¸ÏµÍ³¿ªÊ¼±¸·ÝÖ¸¶¨¹¤×÷»ú 
        public const int MINOR_SPARE_STOP_BACKUP = 0xac; // ÈÈ±¸ÏµÍ³Í£Ö¹±¸·ÝÖ¸¶¨¹¤×÷»ú 
        public const int MINOR_SPARE_CLIENT_INFO = 0xad; // ÈÈ±¸¿Í»§»úÐÅÏ¢ 
        public const int MINOR_ANR_RECORD_START = 0xae; // ANRÂ¼Ïñ¿ªÊ¼ 
        public const int MINOR_ANR_RECORD_END = 0xaf; // ANRÂ¼Ïñ½áÊø 
        public const int MINOR_ANR_ADD_TIME_QUANTUM = 0xb0; // ANRÌí¼ÓÊ±¼ä¶Î 
        public const int MINOR_ANR_DEL_TIME_QUANTUM = 0xb1; // ANRÉ¾³ýÊ±¼ä¶Î 
        public const int MINOR_PIC_REC_START = 0xb3; // ¿ªÊ¼×¥Í¼ 
        public const int MINOR_PIC_REC_STOP = 0xb4; // Í£Ö¹×¥Í¼ 
        public const int MINOR_PIC_REC_OVERDUE = 0xb5; //¹ýÆÚÍ¼Æ¬ÎÄ¼þÉ¾³ý 
        public const int MINOR_CLIENT_LOGIN = 0xb6; // µÇÂ¼·þÎñÆ÷³É¹¦ 
        public const int MINOR_CLIENT_RELOGIN = 0xb7; // ÖØÐÂµÇÂ¼·þÎñÆ÷ 
        public const int MINOR_CLIENT_LOGOUT = 0xb8; // ÍË³ö·þÎñÆ÷³É¹¦ 
        public const int MINOR_CLIENT_SYNC_START = 0xb9; // Â¼ÏñÍ¬²½¿ªÊ¼ 
        public const int MINOR_CLIENT_SYNC_STOP = 0xba; // Â¼ÏñÍ¬²½ÖÕÖ¹ 
        public const int MINOR_CLIENT_SYNC_SUCC = 0xbb; // Â¼ÏñÍ¬²½³É¹¦ 
        public const int MINOR_CLIENT_SYNC_EXCP = 0xbc; // Â¼ÏñÍ¬²½Òì³£ 
        public const int MINOR_GLOBAL_RECORD_ERR_INFO = 0xbd; // È«¾Ö´íÎó¼ÇÂ¼ÐÅÏ¢ 
        public const int MINOR_BUFFER_STATE = 0xbe; // »º³åÇø×´Ì¬ÈÕÖ¾¼ÇÂ¼ 
        public const int MINOR_DISK_ERRORINFO_V2 = 0xbf; // Ó²ÅÌ´íÎóÏêÏ¸ÐÅÏ¢V2 
        public const int MINOR_UNLOCK_RECORD = 0xc3; // ¿ªËø¼ÇÂ¼ 
        public const int MINOR_VIS_ALARM = 0xc4; // ·ÀÇø±¨¾¯ 
        public const int MINOR_TALK_RECORD = 0xc5; // Í¨»°¼ÇÂ¼ 

        /*ÈÕÖ¾¸½¼ÓÐÅÏ¢*/
        //Ö÷ÀàÐÍ
        public const int MAJOR_EVENT = 0x5;/*ÊÂ¼þ*/
        //´ÎÀàÐÍ
        public const int MINOR_LEGAL_CARD_PASS = 0x01; // ºÏ·¨¿¨ÈÏÖ¤Í¨¹ý 
        public const int MINOR_CARD_AND_PSW_PASS = 0x02; // Ë¢¿¨¼ÓÃÜÂëÈÏÖ¤Í¨¹ý 
        public const int MINOR_CARD_AND_PSW_FAIL = 0x03; // Ë¢¿¨¼ÓÃÜÂëÈÏÖ¤Ê§°Ü 
        public const int MINOR_CARD_AND_PSW_TIMEOUT = 0x04; // Êý¿¨¼ÓÃÜÂëÈÏÖ¤³¬Ê± 
        public const int MINOR_CARD_AND_PSW_OVER_TIME = 0x05; // Ë¢¿¨¼ÓÃÜÂë³¬´Î 
        public const int MINOR_CARD_NO_RIGHT = 0x06; // Î´·ÖÅäÈ¨ÏÞ 
        public const int MINOR_CARD_INVALID_PERIOD = 0x07; // ÎÞÐ§Ê±¶Î 
        public const int MINOR_CARD_OUT_OF_DATE = 0x08; // ¿¨ºÅ¹ýÆÚ 
        public const int MINOR_INVALID_CARD = 0x09; // ÎÞ´Ë¿¨ºÅ 
        public const int MINOR_ANTI_SNEAK_FAIL = 0x0a; // ·´Ç±»ØÈÏÖ¤Ê§°Ü 
        public const int MINOR_INTERLOCK_DOOR_NOT_CLOSE = 0x0b; // »¥ËøÃÅÎ´¹Ø±Õ 
        public const int MINOR_NOT_BELONG_MULTI_GROUP = 0x0c; // ¿¨²»ÊôÓÚ¶àÖØÈÏÖ¤Èº×é 
        public const int MINOR_INVALID_MULTI_VERIFY_PERIOD = 0x0d; // ¿¨²»ÔÚ¶àÖØÈÏÖ¤Ê±¼ä¶ÎÄÚ 
        public const int MINOR_MULTI_VERIFY_SUPER_RIGHT_FAIL = 0x0e; // ¶àÖØÈÏÖ¤Ä£Ê½³¬¼¶È¨ÏÞÈÏÖ¤Ê§°Ü 
        public const int MINOR_MULTI_VERIFY_REMOTE_RIGHT_FAIL = 0x0f; // ¶àÖØÈÏÖ¤Ä£Ê½Ô¶³ÌÈÏÖ¤Ê§°Ü 
        public const int MINOR_MULTI_VERIFY_SUCCESS = 0x10; // ¶àÖØÈÏÖ¤³É¹¦ 
        public const int MINOR_LEADER_CARD_OPEN_BEGIN = 0x11; // Ê×¿¨¿ªÃÅ¿ªÊ¼ 
        public const int MINOR_LEADER_CARD_OPEN_END = 0x12; // Ê×¿¨¿ªÃÅ½áÊø 
        public const int MINOR_ALWAYS_OPEN_BEGIN = 0x13; // ³£¿ª×´Ì¬¿ªÊ¼ 
        public const int MINOR_ALWAYS_OPEN_END = 0x14; // ³£¿ª×´Ì¬½áÊø 
        public const int MINOR_LOCK_OPEN = 0x15; // ÃÅËø´ò¿ª 
        public const int MINOR_LOCK_CLOSE = 0x16; // ÃÅËø¹Ø±Õ 
        public const int MINOR_DOOR_BUTTON_PRESS = 0x17; // ¿ªÃÅ°´Å¥´ò¿ª 
        public const int MINOR_DOOR_BUTTON_RELEASE = 0x18; // ¿ªÃÅ°´Å¥·Å¿ª 
        public const int MINOR_DOOR_OPEN_NORMAL = 0x19; // Õý³£¿ªÃÅ£¨ÃÅ´Å£© 
        public const int MINOR_DOOR_CLOSE_NORMAL = 0x1a; // Õý³£¹ØÃÅ£¨ÃÅ´Å£© 
        public const int MINOR_DOOR_OPEN_ABNORMAL = 0x1b; // ÃÅÒì³£´ò¿ª£¨ÃÅ´Å£© 
        public const int MINOR_DOOR_OPEN_TIMEOUT = 0x1c; // ÃÅ´ò¿ª³¬Ê±£¨ÃÅ´Å£©  
        public const int MINOR_ALARMOUT_ON = 0x1d; // ±¨¾¯Êä³ö´ò¿ª 
        public const int MINOR_ALARMOUT_OFF = 0x1e; // ±¨¾¯Êä³ö¹Ø±Õ 
        public const int MINOR_ALWAYS_CLOSE_BEGIN = 0x1f; // ³£¹Ø×´Ì¬¿ªÊ¼ 
        public const int MINOR_ALWAYS_CLOSE_END = 0x20; // ³£¹Ø×´Ì¬½áÊø 
        public const int MINOR_MULTI_VERIFY_NEED_REMOTE_OPEN = 0x21; // ¶àÖØ¶àÖØÈÏÖ¤ÐèÒªÔ¶³Ì¿ªÃÅ 
        public const int MINOR_MULTI_VERIFY_SUPERPASSWD_VERIFY_SUCCESS = 0x22; // ¶àÖØÈÏÖ¤³¬¼¶ÃÜÂëÈÏÖ¤³É¹¦ÊÂ¼þ 
        public const int MINOR_MULTI_VERIFY_REPEAT_VERIFY = 0x23; // ¶àÖØÈÏÖ¤ÖØ¸´ÈÏÖ¤ÊÂ¼þ 
        public const int MINOR_MULTI_VERIFY_TIMEOUT = 0x24; // ¶àÖØÈÏÖ¤ÖØ¸´ÈÏÖ¤ÊÂ¼þ 
        public const int MINOR_DOORBELL_RINGING = 0x25; // ÃÅÁåÏì
        public const int MINOR_FINGERPRINT_COMPARE_PASS = 0x26; // Ö¸ÎÆ±È¶ÔÍ¨¹ý
        public const int MINOR_FINGERPRINT_COMPARE_FAIL = 0x27; // Ö¸ÎÆ±È¶ÔÊ§°Ü
        public const int MINOR_CARD_FINGERPRINT_VERIFY_PASS = 0x28; // Ë¢¿¨¼ÓÖ¸ÎÆÈÏÖ¤Í¨¹ý
        public const int MINOR_CARD_FINGERPRINT_VERIFY_FAIL = 0x29; // Ë¢¿¨¼ÓÖ¸ÎÆÈÏÖ¤Ê§°Ü
        public const int MINOR_CARD_FINGERPRINT_VERIFY_TIMEOUT = 0x2a; // Ë¢¿¨¼ÓÖ¸ÎÆÈÏÖ¤³¬Ê±
        public const int MINOR_CARD_FINGERPRINT_PASSWD_VERIFY_PASS = 0x2b; // Ë¢¿¨¼ÓÖ¸ÎÆ¼ÓÃÜÂëÈÏÖ¤Í¨¹ý
        public const int MINOR_CARD_FINGERPRINT_PASSWD_VERIFY_FAIL = 0x2c; // Ë¢¿¨¼ÓÖ¸ÎÆ¼ÓÃÜÂëÈÏÖ¤Ê§°Ü
        public const int MINOR_CARD_FINGERPRINT_PASSWD_VERIFY_TIMEOUT = 0x2d; // Ë¢¿¨¼ÓÖ¸ÎÆ¼ÓÃÜÂëÈÏÖ¤³¬Ê±
        public const int MINOR_FINGERPRINT_PASSWD_VERIFY_PASS = 0x2e; // Ö¸ÎÆ¼ÓÃÜÂëÈÏÖ¤Í¨¹ý
        public const int MINOR_FINGERPRINT_PASSWD_VERIFY_FAIL = 0x2f; // Ö¸ÎÆ¼ÓÃÜÂëÈÏÖ¤Ê§°Ü
        public const int MINOR_FINGERPRINT_PASSWD_VERIFY_TIMEOUT = 0x30; // Ö¸ÎÆ¼ÓÃÜÂëÈÏÖ¤³¬Ê±
        public const int MINOR_FINGERPRINT_INEXISTENCE = 0x31; // Ö¸ÎÆ²»´æÔÚ

        //µ±ÈÕÖ¾µÄÖ÷ÀàÐÍÎªMAJOR_OPERATION=03£¬´ÎÀàÐÍÎªMINOR_LOCAL_CFG_PARM=0x52»òÕßMINOR_REMOTE_GET_PARM=0x76»òÕßMINOR_REMOTE_CFG_PARM=0x77Ê±£¬dwParaType:²ÎÊýÀàÐÍÓÐÐ§£¬Æäº¬ÒåÈçÏÂ£º
        public const int PARA_VIDEOOUT = 1;
        public const int PARA_IMAGE = 2;
        public const int PARA_ENCODE = 4;
        public const int PARA_NETWORK = 8;
        public const int PARA_ALARM = 16;
        public const int PARA_EXCEPTION = 32;
        public const int PARA_DECODER = 64;/*½âÂëÆ÷*/
        public const int PARA_RS232 = 128;
        public const int PARA_PREVIEW = 256;
        public const int PARA_SECURITY = 512;
        public const int PARA_DATETIME = 1024;
        public const int PARA_FRAMETYPE = 2048;/*Ö¡¸ñÊ½*/

        //vca
        //        public const int PARA_VCA_RULE = 4096;//ÐÐÎª¹æÔò

        public const int PARA_DETECTION = 0x1000;   //Õì²âÅäÖÃ
        public const int PARA_VCA_RULE = 0x1001;  //ÐÐÎª¹æÔò 
        public const int PARA_VCA_CTRL = 0x1002;  //ÅäÖÃÖÇÄÜ¿ØÖÆÐÅÏ¢
        public const int PARA_VCA_PLATE = 0x1003; // ³µÅÆÊ¶±ð

        public const int PARA_CODESPLITTER = 0x2000; /*Âë·ÖÆ÷²ÎÊý*/
        //2010-01-22 Ôö¼ÓÊÓÆµ×ÛºÏÆ½Ì¨ÈÕÖ¾ÐÅÏ¢´ÎÀàÐÍ
        public const int PARA_RS485 = 0x2001; /* RS485ÅäÖÃÐÅÏ¢*/
        public const int PARA_DEVICE = 0x2002; /* Éè±¸ÅäÖÃÐÅÏ¢*/
        public const int PARA_HARDDISK = 0x2003; /* Ó²ÅÌÅäÖÃÐÅÏ¢ */
        public const int PARA_AUTOBOOT = 0x2004; /* ×Ô¶¯ÖØÆôÅäÖÃÐÅÏ¢*/
        public const int PARA_HOLIDAY = 0x2005; /* ½Ú¼ÙÈÕÅäÖÃÐÅÏ¢*/
        public const int PARA_IPC = 0x2006; /* IPÍ¨µÀÅäÖÃ */




        //ÈÕÖ¾ÐÅÏ¢(9000À©Õ¹)
        [StructLayoutAttribute(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
        public struct NET_DVR_LOG_V30
        {
            public NET_DVR_TIME strLogTime;
            public uint dwMajorType;//Ö÷ÀàÐÍ 1-±¨¾¯; 2-Òì³£; 3-²Ù×÷; 0xff-È«²¿
            public uint dwMinorType;//´ÎÀàÐÍ 0-È«²¿;
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = MAX_NAMELEN, ArraySubType = UnmanagedType.I1)]
            public byte[] sPanelUser;//²Ù×÷Ãæ°åµÄÓÃ»§Ãû
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = MAX_NAMELEN, ArraySubType = UnmanagedType.I1)]
            public byte[] sNetUser;//ÍøÂç²Ù×÷µÄÓÃ»§Ãû
            public NET_DVR_IPADDR struRemoteHostAddr;//Ô¶³ÌÖ÷»úµØÖ·
            public uint dwParaType;//²ÎÊýÀàÐÍ
            public uint dwChannel;//Í¨µÀºÅ
            public uint dwDiskNumber;//Ó²ÅÌºÅ
            public uint dwAlarmInPort;//±¨¾¯ÊäÈë¶Ë¿Ú
            public uint dwAlarmOutPort;//±¨¾¯Êä³ö¶Ë¿Ú
            public uint dwInfoLen;
            [MarshalAsAttribute(UnmanagedType.ByValTStr, SizeConst = LOG_INFO_LEN)]
            public string sInfo;
        }

        //ÈÕÖ¾ÐÅÏ¢
        [StructLayoutAttribute(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
        public struct NET_DVR_LOG
        {
            public NET_DVR_TIME strLogTime;
            public uint dwMajorType;//Ö÷ÀàÐÍ 1-±¨¾¯; 2-Òì³£; 3-²Ù×÷; 0xff-È«²¿
            public uint dwMinorType;//´ÎÀàÐÍ 0-È«²¿;
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = MAX_NAMELEN, ArraySubType = UnmanagedType.I1)]
            public byte[] sPanelUser;//²Ù×÷Ãæ°åµÄÓÃ»§Ãû
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = MAX_NAMELEN, ArraySubType = UnmanagedType.I1)]
            public byte[] sNetUser;//ÍøÂç²Ù×÷µÄÓÃ»§Ãû
            [MarshalAsAttribute(UnmanagedType.ByValTStr, SizeConst = 16)]
            public string sRemoteHostAddr;//Ô¶³ÌÖ÷»úµØÖ·
            public uint dwParaType;//²ÎÊýÀàÐÍ
            public uint dwChannel;//Í¨µÀºÅ
            public uint dwDiskNumber;//Ó²ÅÌºÅ
            public uint dwAlarmInPort;//±¨¾¯ÊäÈë¶Ë¿Ú
            public uint dwAlarmOutPort;//±¨¾¯Êä³ö¶Ë¿Ú
        }

        /************************DVRÈÕÖ¾ end***************************/

        //±¨¾¯Êä³ö×´Ì¬(9000À©Õ¹)
        [StructLayoutAttribute(LayoutKind.Sequential)]
        public struct NET_DVR_ALARMOUTSTATUS_V30
        {
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = MAX_ALARMOUT_V30, ArraySubType = UnmanagedType.I1)]
            public byte[] Output;

            public void Init()
            {
                Output = new byte[MAX_ALARMOUT_V30];
            }
        }

        //±¨¾¯Êä³ö×´Ì¬
        [StructLayoutAttribute(LayoutKind.Sequential)]
        public struct NET_DVR_ALARMOUTSTATUS
        {
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 4, ArraySubType = UnmanagedType.I1)]
            public byte[] Output;
        }

        //½»Ò×ÐÅÏ¢
        [StructLayoutAttribute(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
        public struct NET_DVR_TRADEINFO
        {
            public ushort m_Year;
            public ushort m_Month;
            public ushort m_Day;
            public ushort m_Hour;
            public ushort m_Minute;
            public ushort m_Second;
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 24, ArraySubType = UnmanagedType.I1)]
            public byte[] DeviceName;//Éè±¸Ãû³Æ
            public uint dwChannelNumer;//Í¨µÀºÅ
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 32, ArraySubType = UnmanagedType.I1)]
            public byte[] CardNumber;//¿¨ºÅ
            [MarshalAsAttribute(UnmanagedType.ByValTStr, SizeConst = 12)]
            public string cTradeType;//½»Ò×ÀàÐÍ
            public uint dwCash;//½»Ò×½ð¶î
        }

        //ATM×¨ÓÃ
        /****************************ATM(begin)***************************/
        public const int NCR = 0;
        public const int DIEBOLD = 1;
        public const int WINCOR_NIXDORF = 2;
        public const int SIEMENS = 3;
        public const int OLIVETTI = 4;
        public const int FUJITSU = 5;
        public const int HITACHI = 6;
        public const int SMI = 7;
        public const int IBM = 8;
        public const int BULL = 9;
        public const int YiHua = 10;
        public const int LiDe = 11;
        public const int GDYT = 12;
        public const int Mini_Banl = 13;
        public const int GuangLi = 14;
        public const int DongXin = 15;
        public const int ChenTong = 16;
        public const int NanTian = 17;
        public const int XiaoXing = 18;
        public const int GZYY = 19;
        public const int QHTLT = 20;
        public const int DRS918 = 21;
        public const int KALATEL = 22;
        public const int NCR_2 = 23;
        public const int NXS = 24;

        /*Ö¡¸ñÊ½*/
        [StructLayoutAttribute(LayoutKind.Sequential)]
        public struct NET_DVR_FRAMETYPECODE
        {
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 12, ArraySubType = UnmanagedType.I1)]
            public byte[] code;/* ´úÂë */
        }

        //ATM²ÎÊý(9000À©Õ¹)
        [StructLayoutAttribute(LayoutKind.Sequential)]
        public struct NET_DVR_FRAMEFORMAT_V30
        {
            public uint dwSize;
            public NET_DVR_IPADDR struATMIP;/* ATM IPµØÖ· */
            public uint dwATMType;/* ATMÀàÐÍ */
            public uint dwInputMode;/* ÊäÈë·½Ê½	0-ÍøÂçÕìÌý 1-ÍøÂç½ÓÊÕ 2-´®¿ÚÖ±½ÓÊäÈë 3-´®¿ÚATMÃüÁîÊäÈë*/
            public uint dwFrameSignBeginPos;/* ±¨ÎÄ±êÖ¾Î»µÄÆðÊ¼Î»ÖÃ*/
            public uint dwFrameSignLength;/* ±¨ÎÄ±êÖ¾Î»µÄ³¤¶È */
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 12, ArraySubType = UnmanagedType.I1)]
            public byte[] byFrameSignContent;/* ±¨ÎÄ±êÖ¾Î»µÄÄÚÈÝ */
            public uint dwCardLengthInfoBeginPos;/* ¿¨ºÅ³¤¶ÈÐÅÏ¢µÄÆðÊ¼Î»ÖÃ */
            public uint dwCardLengthInfoLength;/* ¿¨ºÅ³¤¶ÈÐÅÏ¢µÄ³¤¶È */
            public uint dwCardNumberInfoBeginPos;/* ¿¨ºÅÐÅÏ¢µÄÆðÊ¼Î»ÖÃ */
            public uint dwCardNumberInfoLength;/* ¿¨ºÅÐÅÏ¢µÄ³¤¶È */
            public uint dwBusinessTypeBeginPos;/* ½»Ò×ÀàÐÍµÄÆðÊ¼Î»ÖÃ */
            public uint dwBusinessTypeLength;/* ½»Ò×ÀàÐÍµÄ³¤¶È */
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 10, ArraySubType = UnmanagedType.Struct)]
            public NET_DVR_FRAMETYPECODE[] frameTypeCode;/* ÀàÐÍ */
            public ushort wATMPort;/* ¿¨ºÅ²¶×½¶Ë¿ÚºÅ(ÍøÂçÐ­Òé·½Ê½) */
            public ushort wProtocolType;/* ÍøÂçÐ­ÒéÀàÐÍ */
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 24, ArraySubType = UnmanagedType.I1)]
            public byte[] byRes;
        }

        //ATM²ÎÊý
        [StructLayoutAttribute(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
        public struct NET_DVR_FRAMEFORMAT
        {
            public uint dwSize;
            [MarshalAsAttribute(UnmanagedType.ByValTStr, SizeConst = 16)]
            public string sATMIP;/* ATM IPµØÖ· */
            public uint dwATMType;/* ATMÀàÐÍ */
            public uint dwInputMode;/* ÊäÈë·½Ê½	0-ÍøÂçÕìÌý 1-ÍøÂç½ÓÊÕ 2-´®¿ÚÖ±½ÓÊäÈë 3-´®¿ÚATMÃüÁîÊäÈë*/
            public uint dwFrameSignBeginPos;/* ±¨ÎÄ±êÖ¾Î»µÄÆðÊ¼Î»ÖÃ*/
            public uint dwFrameSignLength;/* ±¨ÎÄ±êÖ¾Î»µÄ³¤¶È */
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 12, ArraySubType = UnmanagedType.I1)]
            public byte[] byFrameSignContent;/* ±¨ÎÄ±êÖ¾Î»µÄÄÚÈÝ */
            public uint dwCardLengthInfoBeginPos;/* ¿¨ºÅ³¤¶ÈÐÅÏ¢µÄÆðÊ¼Î»ÖÃ */
            public uint dwCardLengthInfoLength;/* ¿¨ºÅ³¤¶ÈÐÅÏ¢µÄ³¤¶È */
            public uint dwCardNumberInfoBeginPos;/* ¿¨ºÅÐÅÏ¢µÄÆðÊ¼Î»ÖÃ */
            public uint dwCardNumberInfoLength;/* ¿¨ºÅÐÅÏ¢µÄ³¤¶È */
            public uint dwBusinessTypeBeginPos;/* ½»Ò×ÀàÐÍµÄÆðÊ¼Î»ÖÃ */
            public uint dwBusinessTypeLength;/* ½»Ò×ÀàÐÍµÄ³¤¶È */
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 10, ArraySubType = UnmanagedType.Struct)]
            public NET_DVR_FRAMETYPECODE[] frameTypeCode;/* ÀàÐÍ */
        }

        //SDK_V31 ATM
        /*¹ýÂËÉèÖÃ*/
        [StructLayoutAttribute(LayoutKind.Sequential)]
        public struct tagNET_DVR_FILTER
        {
            public byte byEnable;/*0,²»ÆôÓÃ;1,ÆôÓÃ*/
            public byte byMode;/*0,ASCII;1,HEX*/
            public byte byFrameBeginPos;// ±¨ÎÄ±êÖ¾Î»µÄÆðÊ¼Î»ÖÃ     
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 1, ArraySubType = UnmanagedType.I1)]
            public byte[] byRes;
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 16, ArraySubType = UnmanagedType.I1)]
            public byte[] byFilterText;/*¹ýÂË×Ö·û´®*/
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 12, ArraySubType = UnmanagedType.I1)]
            public byte[] byRes2;
        }

        /*ÆðÊ¼±êÊ¶ÉèÖÃ*/
        [StructLayoutAttribute(LayoutKind.Sequential)]
        public struct tagNET_DVR_IDENTIFICAT
        {
            public byte byStartMode;/*0,ASCII;1,HEX*/
            public byte byEndMode;/*0,ASCII;1,HEX*/
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 2, ArraySubType = UnmanagedType.I1)]
            public byte[] byRes;
            public NET_DVR_FRAMETYPECODE struStartCode;/*ÆðÊ¼×Ö·û*/
            public NET_DVR_FRAMETYPECODE struEndCode;/*½áÊø×Ö·û*/
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 12, ArraySubType = UnmanagedType.I1)]
            public byte[] byRes1;
        }

        /*±¨ÎÄÐÅÏ¢Î»ÖÃ*/
        [StructLayoutAttribute(LayoutKind.Sequential)]
        public struct tagNET_DVR_PACKAGE_LOCATION
        {
            public byte byOffsetMode;/*0,token;1,fix*/
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 3, ArraySubType = UnmanagedType.I1)]
            public byte[] byRes1;
            public uint dwOffsetPos;/*modeÎª1µÄÊ±ºòÊ¹ÓÃ*/
            public NET_DVR_FRAMETYPECODE struTokenCode;/*±êÖ¾Î»*/
            public byte byMultiplierValue;/*±êÖ¾Î»¶àÉÙ´Î³öÏÖ*/
            public byte byEternOffset;/*¸½¼ÓµÄÆ«ÒÆÁ¿*/
            public byte byCodeMode;/*0,ASCII;1,HEX*/
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 9, ArraySubType = UnmanagedType.I1)]
            public byte[] byRes2;
        }

        /*±¨ÎÄÐÅÏ¢³¤¶È*/
        [StructLayoutAttribute(LayoutKind.Sequential)]
        public struct tagNET_DVR_PACKAGE_LENGTH
        {
            public byte byLengthMode;/*³¤¶ÈÀàÐÍ£¬0,variable;1,fix;2,get from package(ÉèÖÃ¿¨ºÅ³¤¶ÈÊ¹ÓÃ)*/
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 3, ArraySubType = UnmanagedType.I1)]
            public byte[] byRes1;
            public uint dwFixLength;/*modeÎª1µÄÊ±ºòÊ¹ÓÃ*/
            public uint dwMaxLength;
            public uint dwMinLength;
            public byte byEndMode;/*ÖÕ½á·û0,ASCII;1,HEX*/
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 3, ArraySubType = UnmanagedType.I1)]
            public byte[] byRes2;
            public NET_DVR_FRAMETYPECODE struEndCode;/*ÖÕ½á·û*/
            public uint dwLengthPos;/*lengthModeÎª2µÄÊ±ºòÊ¹ÓÃ£¬¿¨ºÅ³¤¶ÈÔÚ±¨ÎÄÖÐµÄÎ»ÖÃ*/
            public uint dwLengthLen;/*lengthModeÎª2µÄÊ±ºòÊ¹ÓÃ£¬¿¨ºÅ³¤¶ÈµÄ³¤¶È*/
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 8, ArraySubType = UnmanagedType.I1)]
            public byte[] byRes3;
        }

        /*OSD µþ¼ÓµÄÎ»ÖÃ*/
        [StructLayoutAttribute(LayoutKind.Sequential)]
        public struct tagNET_DVR_OSD_POSITION
        {
            public byte byPositionMode;/*µþ¼Ó·ç¸ñ£¬¹²2ÖÖ£»0£¬²»ÏÔÊ¾£»1£¬Custom*/
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 3, ArraySubType = UnmanagedType.I1)]
            public byte[] byRes1;
            public uint dwPos_x;/*x×ø±ê£¬positionmodeÎªCustomÊ±Ê¹ÓÃ*/
            public uint dwPos_y;/*y×ø±ê£¬positionmodeÎªCustomÊ±Ê¹ÓÃ*/
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 8, ArraySubType = UnmanagedType.I1)]
            public byte[] byRes2;
        }

        /*ÈÕÆÚÏÔÊ¾¸ñÊ½*/
        [StructLayoutAttribute(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
        public struct tagNET_DVR_DATE_FORMAT
        {
            public byte byItem1;/*Month,0.mm;1.mmm;2.mmmm*/
            public byte byItem2;/*Day,0.dd;*/
            public byte byItem3;/*Year,0.yy;1.yyyy*/
            public byte byDateForm;/*0~5£¬3¸öitemµÄÅÅÁÐ×éºÏ*/
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 20, ArraySubType = UnmanagedType.I1)]
            public byte[] byRes;
            [MarshalAsAttribute(UnmanagedType.ByValTStr, SizeConst = 4)]
            public string chSeprator;/*·Ö¸ô·û*/
            [MarshalAsAttribute(UnmanagedType.ByValTStr, SizeConst = 4)]
            public string chDisplaySeprator;/*ÏÔÊ¾·Ö¸ô·û*/
            public byte byDisplayForm;/*0~5£¬3¸öitemµÄÅÅÁÐ×éºÏ*///lili mode by lili
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 27, ArraySubType = UnmanagedType.I1)]
            public byte[] res;
        }

        /*Ê±¼äÏÔÊ¾¸ñÊ½*/
        [StructLayoutAttribute(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
        public struct tagNET_DVRT_TIME_FORMAT
        {
            public byte byTimeForm;/*1. HH MM SS;0. HH MM*/
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 23, ArraySubType = UnmanagedType.I1)]
            public byte[] byRes1;
            public byte byHourMode;/*0,12;1,24*/ //lili mode
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 3, ArraySubType = UnmanagedType.I1)]
            public byte[] byRes2;
            [MarshalAsAttribute(UnmanagedType.ByValTStr, SizeConst = 4)]
            public string chSeprator;/*±¨ÎÄ·Ö¸ô·û£¬ÔÝÊ±Ã»ÓÃ*/
            [MarshalAsAttribute(UnmanagedType.ByValTStr, SizeConst = 4)]
            public string chDisplaySeprator;/*ÏÔÊ¾·Ö¸ô·û*/
            public byte byDisplayForm;/*0~5£¬3¸öitemµÄÅÅÁÐ×éºÏ*/
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 3, ArraySubType = UnmanagedType.I1)]
            public byte[] byRes3;
            public byte byDisplayHourMode;/*0,12;1,24*/ //lili mode
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 19, ArraySubType = UnmanagedType.I1)]
            public byte[] byRes4;
        }

        [StructLayoutAttribute(LayoutKind.Sequential)]
        public struct tagNET_DVR_OVERLAY_CHANNEL
        {
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 64, ArraySubType = UnmanagedType.I1)]
            public byte[] byChannel;/*µþ¼ÓµÄÍ¨µÀ*/
            public uint dwDelayTime;/*µþ¼ÓÑÓÊ±Ê±¼ä*/
            public byte byEnableDelayTime;/*ÊÇ·ñÆôÓÃµþ¼ÓÑÓÊ±£¬ÔÚÎÞÍË¿¨ÃüÁîÊ±ÆôÓÃ*/
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 11, ArraySubType = UnmanagedType.I1)]
            public byte[] byRes;
        }

        [StructLayoutAttribute(LayoutKind.Sequential)]
        public struct tagNET_DVR_ATM_PACKAGE_ACTION
        {
            public tagNET_DVR_PACKAGE_LOCATION struPackageLocation;
            public tagNET_DVR_OSD_POSITION struOsdPosition;
            public NET_DVR_FRAMETYPECODE struActionCode;/*½»Ò×ÀàÐÍµÈ¶ÔÓ¦µÄÂë*/
            public NET_DVR_FRAMETYPECODE struPreCode;/*µþ¼Ó×Ö·ûÇ°µÄ×Ö·û*/
            public byte byActionCodeMode;/*½»Ò×ÀàÐÍµÈ¶ÔÓ¦µÄÂë0,ASCII;1,HEX*/
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 7, ArraySubType = UnmanagedType.I1)]
            public byte[] byRes;
        }

        [StructLayoutAttribute(LayoutKind.Sequential)]
        public struct tagNET_DVR_ATM_PACKAGE_DATE
        {
            public tagNET_DVR_PACKAGE_LOCATION struPackageLocation;
            public tagNET_DVR_DATE_FORMAT struDateForm;
            public tagNET_DVR_OSD_POSITION struOsdPosition;
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 8, ArraySubType = UnmanagedType.I1)]
            public byte[] res;
        }

        [StructLayoutAttribute(LayoutKind.Sequential)]
        public struct tagNET_DVR_ATM_PACKAGE_TIME
        {
            public tagNET_DVR_PACKAGE_LOCATION location;
            public tagNET_DVRT_TIME_FORMAT struTimeForm;
            public tagNET_DVR_OSD_POSITION struOsdPosition;
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 8, ArraySubType = UnmanagedType.I1)]
            public byte[] byRes;
        }

        [StructLayoutAttribute(LayoutKind.Sequential)]
        public struct tagNET_DVR_ATM_PACKAGE_OTHERS
        {
            public tagNET_DVR_PACKAGE_LOCATION struPackageLocation;
            public tagNET_DVR_PACKAGE_LENGTH struPackageLength;
            public tagNET_DVR_OSD_POSITION struOsdPosition;
            public NET_DVR_FRAMETYPECODE struPreCode;/*µþ¼Ó×Ö·ûÇ°µÄ×Ö·û*/
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 8, ArraySubType = UnmanagedType.I1)]
            public byte[] res;
        }

        [StructLayoutAttribute(LayoutKind.Sequential)]
        public struct tagNET_DVR_ATM_FRAMETYPE_NEW
        {
            public byte byEnable;/*ÊÇ·ñÆôÓÃ0,²»ÆôÓÃ;1,ÆôÓÃ*/
            public byte byInputMode;/*ÊäÈë·½Ê½:ÍøÂç¼àÌý¡¢´®¿Ú¼àÌý¡¢ÍøÂçÐ­Òé¡¢´®¿ÚÐ­Òé¡¢´®¿Ú·þÎñÆ÷*/
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 2, ArraySubType = UnmanagedType.I1)]
            public byte[] byRes1;
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 32, ArraySubType = UnmanagedType.I1)]
            public byte[] byAtmName;/*ATM Ãû³Æ*/
            public NET_DVR_IPADDR struAtmIp;/*ATM »úIP  */
            public ushort wAtmPort;/* ¿¨ºÅ²¶×½¶Ë¿ÚºÅ(ÍøÂçÐ­Òé·½Ê½) »ò´®¿Ú·þÎñÆ÷¶Ë¿ÚºÅ*/
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 2, ArraySubType = UnmanagedType.I1)]
            public byte[] byRes2;
            public uint dwAtmType;/*ATM »úÀàÐÍ*/
            public tagNET_DVR_IDENTIFICAT struIdentification;/*±¨ÎÄ±êÖ¾*/
            public tagNET_DVR_FILTER struFilter;/*¹ýÂËÉèÖÃ*/
            public tagNET_DVR_ATM_PACKAGE_OTHERS struCardNoPara;/*µþ¼Ó¿¨ºÅÉèÖÃ*/
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = MAX_ACTION_TYPE, ArraySubType = UnmanagedType.Struct)]
            public tagNET_DVR_ATM_PACKAGE_ACTION[] struTradeActionPara;/*µþ¼Ó½»Ò×ÐÐÎªÉèÖÃ*/
            public tagNET_DVR_ATM_PACKAGE_OTHERS struAmountPara;/*µþ¼Ó½»Ò×½ð¶îÉèÖÃ*/
            public tagNET_DVR_ATM_PACKAGE_OTHERS struSerialNoPara;/*µþ¼Ó½»Ò×ÐòºÅÉèÖÃ*/
            public tagNET_DVR_OVERLAY_CHANNEL struOverlayChan;/*µþ¼ÓÍ¨µÀÉèÖÃ*/
            public tagNET_DVR_ATM_PACKAGE_DATE byRes4;/*µþ¼ÓÈÕÆÚÉèÖÃ£¬ÔÝÊ±±£Áô*/
            public tagNET_DVR_ATM_PACKAGE_TIME byRes5;/*µþ¼ÓÊ±¼äÉèÖÃ£¬ÔÝÊ±±£Áô*/
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 132, ArraySubType = UnmanagedType.I1)]
            public byte[] byRes3;
        }

        [StructLayoutAttribute(LayoutKind.Sequential)]
        public struct tagNET_DVR_FRAMEFORMAT_V31
        {
            public uint dwSize;
            public tagNET_DVR_ATM_FRAMETYPE_NEW struAtmFrameTypeNew;
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 3, ArraySubType = UnmanagedType.Struct)]
            public tagNET_DVR_ATM_FRAMETYPE_NEW[] byRes;
        }

        [StructLayoutAttribute(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
        public struct tagNET_DVR_ATM_PROTOIDX
        {
            public uint dwAtmType;/*ATMÐ­ÒéÀàÐÍ£¬Í¬Ê±×÷ÎªË÷ÒýÐòºÅ*/
            [MarshalAsAttribute(UnmanagedType.ByValTStr, SizeConst = ATM_DESC_LEN)]
            public string chDesc;/*ATMÐ­Òé¼òµ¥ÃèÊö*/
        }

        [StructLayoutAttribute(LayoutKind.Sequential)]
        public struct tagNET_DVR_ATM_PROTOCOL
        {
            public uint dwSize;
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = MAX_ATM_PROTOCOL_NUM, ArraySubType = UnmanagedType.Struct)]
            public tagNET_DVR_ATM_PROTOIDX[] struAtmProtoidx;
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = ATM_PROTOCOL_SORT, ArraySubType = UnmanagedType.U4)]
            public uint[] dwAtmNumPerSort;/*Ã¿¶ÎÐ­ÒéÊý*/
        }

        /*****************************DS-6001D/F(begin)***************************/
        //DS-6001D Decoder
        [StructLayoutAttribute(LayoutKind.Sequential)]
        public struct NET_DVR_DECODERINFO
        {
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 16, ArraySubType = UnmanagedType.I1)]
            public byte[] byEncoderIP;//½âÂëÉè±¸Á¬½ÓµÄ·þÎñÆ÷IP
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 16, ArraySubType = UnmanagedType.I1)]
            public byte[] byEncoderUser;//½âÂëÉè±¸Á¬½ÓµÄ·þÎñÆ÷µÄÓÃ»§Ãû
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 16, ArraySubType = UnmanagedType.I1)]
            public byte[] byEncoderPasswd;//½âÂëÉè±¸Á¬½ÓµÄ·þÎñÆ÷µÄÃÜÂë
            public byte bySendMode;//½âÂëÉè±¸Á¬½Ó·þÎñÆ÷µÄÁ¬½ÓÄ£Ê½
            public byte byEncoderChannel;//½âÂëÉè±¸Á¬½ÓµÄ·þÎñÆ÷µÄÍ¨µÀºÅ
            public ushort wEncoderPort;//½âÂëÉè±¸Á¬½ÓµÄ·þÎñÆ÷µÄ¶Ë¿ÚºÅ
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 4, ArraySubType = UnmanagedType.I1)]
            public byte[] reservedData;//±£Áô
        }

        [StructLayoutAttribute(LayoutKind.Sequential)]
        public struct NET_DVR_DECODERSTATE
        {
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 16, ArraySubType = UnmanagedType.I1)]
            public byte[] byEncoderIP;//½âÂëÉè±¸Á¬½ÓµÄ·þÎñÆ÷IP
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 16, ArraySubType = UnmanagedType.I1)]
            public byte[] byEncoderUser;//½âÂëÉè±¸Á¬½ÓµÄ·þÎñÆ÷µÄÓÃ»§Ãû
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 16, ArraySubType = UnmanagedType.I1)]
            public byte[] byEncoderPasswd;//½âÂëÉè±¸Á¬½ÓµÄ·þÎñÆ÷µÄÃÜÂë
            public byte byEncoderChannel;//½âÂëÉè±¸Á¬½ÓµÄ·þÎñÆ÷µÄÍ¨µÀºÅ
            public byte bySendMode;//½âÂëÉè±¸Á¬½ÓµÄ·þÎñÆ÷µÄÁ¬½ÓÄ£Ê½
            public ushort wEncoderPort;//½âÂëÉè±¸Á¬½ÓµÄ·þÎñÆ÷µÄ¶Ë¿ÚºÅ
            public uint dwConnectState;//½âÂëÉè±¸Á¬½Ó·þÎñÆ÷µÄ×´Ì¬
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 4, ArraySubType = UnmanagedType.I1)]
            public byte[] reservedData;//±£Áô
        }

        /*½âÂëÉè±¸¿ØÖÆÂë¶¨Òå*/
        public const int NET_DEC_STARTDEC = 1;
        public const int NET_DEC_STOPDEC = 2;
        public const int NET_DEC_STOPCYCLE = 3;
        public const int NET_DEC_CONTINUECYCLE = 4;

        /*Á¬½ÓµÄÍ¨µÀÅäÖÃ*/
        [StructLayoutAttribute(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
        public struct NET_DVR_DECCHANINFO
        {
            [MarshalAsAttribute(UnmanagedType.ByValTStr, SizeConst = 16)]
            public string sDVRIP;/* DVR IPµØÖ· */
            public ushort wDVRPort;/* ¶Ë¿ÚºÅ */
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = NAME_LEN, ArraySubType = UnmanagedType.I1)]
            public byte[] sUserName;/* ÓÃ»§Ãû */
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = PASSWD_LEN, ArraySubType = UnmanagedType.I1)]
            public byte[] sPassword;/* ÃÜÂë */
            public byte byChannel;/* Í¨µÀºÅ */
            public byte byLinkMode;/* Á¬½ÓÄ£Ê½ */
            public byte byLinkType;/* Á¬½ÓÀàÐÍ 0£­Ö÷ÂëÁ÷ 1£­×ÓÂëÁ÷ */
        }

        /*Ã¿¸ö½âÂëÍ¨µÀµÄÅäÖÃ*/
        [StructLayoutAttribute(LayoutKind.Sequential)]
        public struct NET_DVR_DECINFO
        {
            public byte byPoolChans;/*Ã¿Â·½âÂëÍ¨µÀÉÏµÄÑ­»·Í¨µÀÊýÁ¿, ×î¶à4Í¨µÀ 0±íÊ¾Ã»ÓÐ½âÂë*/
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = MAX_DECPOOLNUM, ArraySubType = UnmanagedType.Struct)]
            public NET_DVR_DECCHANINFO[] struchanConInfo;
            public byte byEnablePoll;/*ÊÇ·ñÂÖÑ² 0-·ñ 1-ÊÇ*/
            public byte byPoolTime;/*ÂÖÑ²Ê±¼ä 0-±£Áô 1-10Ãë 2-15Ãë 3-20Ãë 4-30Ãë 5-45Ãë 6-1·ÖÖÓ 7-2·ÖÖÓ 8-5·ÖÖÓ */
        }

        /*Õû¸öÉè±¸½âÂëÅäÖÃ*/
        [StructLayoutAttribute(LayoutKind.Sequential)]
        public struct NET_DVR_DECCFG
        {
            public uint dwSize;
            public uint dwDecChanNum;/*½âÂëÍ¨µÀµÄÊýÁ¿*/
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = MAX_DECNUM, ArraySubType = UnmanagedType.Struct)]
            public NET_DVR_DECINFO[] struDecInfo;
        }

        //2005-08-01
        /* ½âÂëÉè±¸Í¸Ã÷Í¨µÀÉèÖÃ */
        [StructLayoutAttribute(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
        public struct NET_DVR_PORTINFO
        {
            public uint dwEnableTransPort;/* ÊÇ·ñÆô¶¯Í¸Ã÷Í¨µÀ 0£­²»ÆôÓÃ 1£­ÆôÓÃ*/
            [MarshalAsAttribute(UnmanagedType.ByValTStr, SizeConst = 16)]
            public string sDecoderIP;/* DVR IPµØÖ· */
            public ushort wDecoderPort;/* ¶Ë¿ÚºÅ */
            public ushort wDVRTransPort;/* ÅäÖÃÇ°¶ËDVRÊÇ´Ó485/232Êä³ö£¬1±íÊ¾232´®¿Ú,2±íÊ¾485´®¿Ú */
            [MarshalAsAttribute(UnmanagedType.ByValTStr, SizeConst = 4)]
            public string cReserve;
        }

        [StructLayoutAttribute(LayoutKind.Sequential)]
        public struct NET_DVR_PORTCFG
        {
            public uint dwSize;
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = MAX_TRANSPARENTNUM, ArraySubType = UnmanagedType.Struct)]
            public NET_DVR_PORTINFO[] struTransPortInfo;/* Êý×é0±íÊ¾232 Êý×é1±íÊ¾485 */
        }

        /* ¿ØÖÆÍøÂçÎÄ¼þ»Ø·Å */
        [StructLayoutAttribute(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
        public struct NET_DVR_PLAYREMOTEFILE
        {
            public uint dwSize;
            [MarshalAsAttribute(UnmanagedType.ByValTStr, SizeConst = 16)]
            public string sDecoderIP;/* DVR IPµØÖ· */
            public ushort wDecoderPort;/* ¶Ë¿ÚºÅ */
            public ushort wLoadMode;/* »Ø·ÅÏÂÔØÄ£Ê½ 1£­°´Ãû×Ö 2£­°´Ê±¼ä */

            [StructLayoutAttribute(LayoutKind.Explicit)]
            public struct mode_size
            {
                [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 100, ArraySubType = UnmanagedType.I1)]
                [FieldOffsetAttribute(0)]
                public byte[] byFile;/* »Ø·ÅµÄÎÄ¼þÃû */

                [StructLayoutAttribute(LayoutKind.Sequential)]
                public struct bytime
                {
                    public uint dwChannel;
                    [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 32, ArraySubType = UnmanagedType.I1)]
                    public byte[] sUserName;/*ÇëÇóÊÓÆµÓÃ»§Ãû*/
                    [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 16, ArraySubType = UnmanagedType.I1)]
                    public byte[] sPassword;/* ÃÜÂë */
                    public NET_DVR_TIME struStartTime;/* °´Ê±¼ä»Ø·ÅµÄ¿ªÊ¼Ê±¼ä */
                    public NET_DVR_TIME struStopTime;/* °´Ê±¼ä»Ø·ÅµÄ½áÊøÊ±¼ä */
                }
            }
        }

        /*µ±Ç°Éè±¸½âÂëÁ¬½Ó×´Ì¬*/
        [StructLayoutAttribute(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
        public struct NET_DVR_DECCHANSTATUS
        {
            public uint dwWorkType;/*¹¤×÷·½Ê½£º1£ºÂÖÑ²¡¢2£º¶¯Ì¬Á¬½Ó½âÂë¡¢3£ºÎÄ¼þ»Ø·ÅÏÂÔØ 4£º°´Ê±¼ä»Ø·ÅÏÂÔØ*/
            [MarshalAsAttribute(UnmanagedType.ByValTStr, SizeConst = 16)]
            public string sDVRIP;/*Á¬½ÓµÄÉè±¸ip*/
            public ushort wDVRPort;/*Á¬½Ó¶Ë¿ÚºÅ*/
            public byte byChannel;/* Í¨µÀºÅ */
            public byte byLinkMode;/* Á¬½ÓÄ£Ê½ */
            public uint dwLinkType;/*Á¬½ÓÀàÐÍ 0£­Ö÷ÂëÁ÷ 1£­×ÓÂëÁ÷*/

            [StructLayoutAttribute(LayoutKind.Explicit)]
            public struct objectInfo
            {
                [StructLayoutAttribute(LayoutKind.Sequential)]
                public struct userInfo
                {
                    [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 32, ArraySubType = UnmanagedType.I1)]
                    public byte[] sUserName;/*ÇëÇóÊÓÆµÓÃ»§Ãû*/
                    [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 16, ArraySubType = UnmanagedType.I1)]
                    public byte[] sPassword;/* ÃÜÂë */
                    [MarshalAsAttribute(UnmanagedType.ByValTStr, SizeConst = 52)]
                    public string cReserve;
                }

                [StructLayoutAttribute(LayoutKind.Sequential)]
                public struct fileInfo
                {
                    [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 100, ArraySubType = UnmanagedType.I1)]
                    public byte[] fileName;
                }
                [StructLayoutAttribute(LayoutKind.Sequential)]
                public struct timeInfo
                {
                    public uint dwChannel;
                    [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 32, ArraySubType = UnmanagedType.I1)]
                    public byte[] sUserName;/*ÇëÇóÊÓÆµÓÃ»§Ãû*/
                    [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 16, ArraySubType = UnmanagedType.I1)]
                    public byte[] sPassword;/* ÃÜÂë */
                    public NET_DVR_TIME struStartTime;/* °´Ê±¼ä»Ø·ÅµÄ¿ªÊ¼Ê±¼ä */
                    public NET_DVR_TIME struStopTime;/* °´Ê±¼ä»Ø·ÅµÄ½áÊøÊ±¼ä */
                }
            }
        }


        [StructLayoutAttribute(LayoutKind.Sequential)]
        public struct NET_DVR_DECSTATUS
        {
            public uint dwSize;
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = MAX_DECNUM, ArraySubType = UnmanagedType.Struct)]
            public NET_DVR_DECCHANSTATUS[] struTransPortInfo;
        }

        //µ¥×Ö·û²ÎÊý(×Ó½á¹¹)
        [StructLayoutAttribute(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
        public struct NET_DVR_SHOWSTRINGINFO
        {
            public ushort wShowString;// Ô¤ÀÀµÄÍ¼ÏóÉÏÊÇ·ñÏÔÊ¾×Ö·û,0-²»ÏÔÊ¾,1-ÏÔÊ¾ ÇøÓò´óÐ¡704*576,µ¥¸ö×Ö·ûµÄ´óÐ¡Îª32*32
            public ushort wStringSize;/* ¸ÃÐÐ×Ö·ûµÄ³¤¶È£¬²»ÄÜ´óÓÚ44¸ö×Ö·û */
            public ushort wShowStringTopLeftX;/* ×Ö·ûÏÔÊ¾Î»ÖÃµÄx×ø±ê */
            public ushort wShowStringTopLeftY;/* ×Ö·ûÃû³ÆÏÔÊ¾Î»ÖÃµÄy×ø±ê */
            [MarshalAsAttribute(UnmanagedType.ByValTStr, SizeConst = 44)]
            public string sString;/* ÒªÏÔÊ¾µÄ×Ö·ûÄÚÈÝ */
        }

        //µþ¼Ó×Ö·û(9000À©Õ¹)
        [StructLayoutAttribute(LayoutKind.Sequential)]
        public struct NET_DVR_SHOWSTRING_V30
        {
            public uint dwSize;
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = MAX_STRINGNUM_V30, ArraySubType = UnmanagedType.Struct)]
            public NET_DVR_SHOWSTRINGINFO[] struStringInfo;/* ÒªÏÔÊ¾µÄ×Ö·ûÄÚÈÝ */

            public void Init()
            {
                struStringInfo = new NET_DVR_SHOWSTRINGINFO[MAX_STRINGNUM_V30];
            }
        }

        //µþ¼Ó×Ö·ûÀ©Õ¹(8Ìõ×Ö·û)
        [StructLayoutAttribute(LayoutKind.Sequential)]
        public struct NET_DVR_SHOWSTRING_EX
        {
            public uint dwSize;
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = MAX_STRINGNUM_EX, ArraySubType = UnmanagedType.Struct)]
            public NET_DVR_SHOWSTRINGINFO[] struStringInfo;/* ÒªÏÔÊ¾µÄ×Ö·ûÄÚÈÝ */
        }

        //µþ¼Ó×Ö·û
        [StructLayoutAttribute(LayoutKind.Sequential)]
        public struct NET_DVR_SHOWSTRING
        {
            public uint dwSize;
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = MAX_STRINGNUM, ArraySubType = UnmanagedType.Struct)]
            public NET_DVR_SHOWSTRINGINFO[] struStringInfo;/* ÒªÏÔÊ¾µÄ×Ö·ûÄÚÈÝ */
        }

        /****************************DS9000ÐÂÔö½á¹¹(begin)******************************/

        /*EMAIL²ÎÊý½á¹¹*/
        //ÓëÔ­½á¹¹ÌåÓÐ²îÒì
        [StructLayoutAttribute(LayoutKind.Sequential)]
        public struct struReceiver
        {
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = NAME_LEN, ArraySubType = UnmanagedType.I1)]
            public byte[] sName;/* ÊÕ¼þÈËÐÕÃû */
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = MAX_EMAIL_ADDR_LEN, ArraySubType = UnmanagedType.I1)]
            public byte[] sAddress;/* ÊÕ¼þÈËµØÖ· */
        }
        [StructLayoutAttribute(LayoutKind.Sequential)]
        public struct NET_DVR_EMAILCFG_V30
        {
            public uint dwSize;
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = NAME_LEN, ArraySubType = UnmanagedType.I1)]
            public byte[] sAccount;/* ÕËºÅ*/
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = MAX_EMAIL_PWD_LEN, ArraySubType = UnmanagedType.I1)]
            public byte[] sPassword;/*ÃÜÂë */

            [StructLayoutAttribute(LayoutKind.Sequential)]
            public struct struSender
            {
                [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = NAME_LEN, ArraySubType = UnmanagedType.I1)]
                public byte[] sName;/* ·¢¼þÈËÐÕÃû */
                [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = MAX_EMAIL_ADDR_LEN, ArraySubType = UnmanagedType.I1)]
                public byte[] sAddress;/* ·¢¼þÈËµØÖ· */
            }

            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = MAX_EMAIL_ADDR_LEN, ArraySubType = UnmanagedType.I1)]
            public byte[] sSmtpServer;/* smtp·þÎñÆ÷ */
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = MAX_EMAIL_ADDR_LEN, ArraySubType = UnmanagedType.I1)]
            public byte[] sPop3Server;/* pop3·þÎñÆ÷ */

            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 3, ArraySubType = UnmanagedType.Struct)]
            public struReceiver[] struStringInfo;/* ×î¶à¿ÉÒÔÉèÖÃ3¸öÊÕ¼þÈË */

            public byte byAttachment;/* ÊÇ·ñ´ø¸½¼þ */
            public byte bySmtpServerVerify;/* ·¢ËÍ·þÎñÆ÷ÒªÇóÉí·ÝÑéÖ¤ */
            public byte byMailInterval;/* mail interval */
            public byte byEnableSSL;//sslÊÇ·ñÆôÓÃ9000_1.1
            public ushort wSmtpPort;//gmailµÄ465£¬ÆÕÍ¨µÄÎª25  
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 74, ArraySubType = UnmanagedType.I1)]
            public byte[] byRes;//±£Áô
        }

        /*DVRÊµÏÖÑ²º½Êý¾Ý½á¹¹*/
        [StructLayoutAttribute(LayoutKind.Sequential)]
        public struct NET_DVR_CRUISE_PARA
        {
            public uint dwSize;
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = CRUISE_MAX_PRESET_NUMS, ArraySubType = UnmanagedType.I1)]
            public byte[] byPresetNo;/* Ô¤ÖÃµãºÅ */
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = CRUISE_MAX_PRESET_NUMS, ArraySubType = UnmanagedType.I1)]
            public byte[] byCruiseSpeed;/* Ñ²º½ËÙ¶È */
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = CRUISE_MAX_PRESET_NUMS, ArraySubType = UnmanagedType.U2)]
            public ushort[] wDwellTime;/* Í£ÁôÊ±¼ä */
            public byte byEnableThisCruise;/* ÊÇ·ñÆôÓÃ */
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 15, ArraySubType = UnmanagedType.I1)]
            public byte[] res;
        }
        /****************************DS9000ÐÂÔö½á¹¹(end)******************************/

        //Ê±¼äµã
        [StructLayoutAttribute(LayoutKind.Sequential)]
        public struct NET_DVR_TIMEPOINT
        {
            public uint dwMonth;//ÔÂ 0-11±íÊ¾1-12¸öÔÂ
            public uint dwWeekNo;//µÚ¼¸ÖÜ 0£­µÚ1ÖÜ 1£­µÚ2ÖÜ 2£­µÚ3ÖÜ 3£­µÚ4ÖÜ 4£­×îºóÒ»ÖÜ
            public uint dwWeekDate;//ÐÇÆÚ¼¸ 0£­ÐÇÆÚÈÕ 1£­ÐÇÆÚÒ» 2£­ÐÇÆÚ¶þ 3£­ÐÇÆÚÈý 4£­ÐÇÆÚËÄ 5£­ÐÇÆÚÎå 6£­ÐÇÆÚÁù
            public uint dwHour;//Ð¡Ê±	¿ªÊ¼Ê±¼ä0£­23 ½áÊøÊ±¼ä1£­23
            public uint dwMin;//·Ö	0£­59
        }

        //ÏÄÁîÊ±²ÎÊý
        [StructLayoutAttribute(LayoutKind.Sequential)]
        public struct NET_DVR_ZONEANDDST
        {
            public uint dwSize;
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 16, ArraySubType = UnmanagedType.I1)]
            public byte[] byRes1;//±£Áô
            public uint dwEnableDST;//ÊÇ·ñÆôÓÃÏÄÊ±ÖÆ 0£­²»ÆôÓÃ 1£­ÆôÓÃ
            public byte byDSTBias;//ÏÄÁîÊ±Æ«ÒÆÖµ£¬30min, 60min, 90min, 120min, ÒÔ·ÖÖÓ¼Æ£¬´«µÝÔ­Ê¼ÊýÖµ
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 3, ArraySubType = UnmanagedType.I1)]
            public byte[] byRes2;
            public NET_DVR_TIMEPOINT struBeginPoint;//ÏÄÊ±ÖÆ¿ªÊ¼Ê±¼ä
            public NET_DVR_TIMEPOINT struEndPoint;//ÏÄÊ±ÖÆÍ£Ö¹Ê±¼ä
        }

        //Í¼Æ¬ÖÊÁ¿
        [StructLayoutAttribute(LayoutKind.Sequential)]
        public struct NET_DVR_JPEGPARA
        {
            /*×¢Òâ£ºµ±Í¼ÏñÑ¹Ëõ·Ö±æÂÊÎªVGAÊ±£¬Ö§³Ö0=CIF, 1=QCIF, 2=D1×¥Í¼£¬
            µ±·Ö±æÂÊÎª3=UXGA(1600x1200), 4=SVGA(800x600), 5=HD720p(1280x720),6=VGA,7=XVGA, 8=HD900p
            ½öÖ§³Öµ±Ç°·Ö±æÂÊµÄ×¥Í¼*/
            public ushort wPicSize;/* 0=CIF, 1=QCIF, 2=D1 3=UXGA(1600x1200), 4=SVGA(800x600), 5=HD720p(1280x720),6=VGA*/
            public ushort wPicQuality;/* Í¼Æ¬ÖÊÁ¿ÏµÊý 0-×îºÃ 1-½ÏºÃ 2-Ò»°ã */
        }

        /* aux video out parameter */
        //¸¨ÖúÊä³ö²ÎÊýÅäÖÃ
        [StructLayoutAttribute(LayoutKind.Sequential)]
        public struct NET_DVR_AUXOUTCFG
        {
            public uint dwSize;
            public uint dwAlarmOutChan;/* Ñ¡Ôñ±¨¾¯µ¯³ö´ó±¨¾¯Í¨µÀÇÐ»»Ê±¼ä£º1»­ÃæµÄÊä³öÍ¨µÀ: 0:Ö÷Êä³ö/1:¸¨1/2:¸¨2/3:¸¨3/4:¸¨4 */
            public uint dwAlarmChanSwitchTime;/* :1Ãë - 10:10Ãë */
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = MAX_AUXOUT, ArraySubType = UnmanagedType.U4)]
            public uint[] dwAuxSwitchTime;/* ¸¨ÖúÊä³öÇÐ»»Ê±¼ä: 0-²»ÇÐ»»,1-5s,2-10s,3-20s,4-30s,5-60s,6-120s,7-300s */
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = MAX_AUXOUT * MAX_WINDOW, ArraySubType = UnmanagedType.I1)]
            public byte[] byAuxOrder;/* ¸¨ÖúÊä³öÔ¤ÀÀË³Ðò, 0xff±íÊ¾ÏàÓ¦µÄ´°¿Ú²»Ô¤ÀÀ */
        }

        //ntp
        [StructLayoutAttribute(LayoutKind.Sequential)]
        public struct NET_DVR_NTPPARA
        {
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 64, ArraySubType = UnmanagedType.I1)]
            public byte[] sNTPServer;/* Domain Name or IP addr of NTP server */
            public ushort wInterval;/* adjust time interval(hours) */
            public byte byEnableNTP;/* enable NPT client 0-no£¬1-yes*/
            public byte cTimeDifferenceH;/* Óë¹ú¼Ê±ê×¼Ê±¼äµÄ Ð¡Ê±Æ«ÒÆ-12 ... +13 */
            public byte cTimeDifferenceM;/* Óë¹ú¼Ê±ê×¼Ê±¼äµÄ ·ÖÖÓÆ«ÒÆ0, 30, 45*/
            public byte res1;
            public ushort wNtpPort; /* ntp server port 9000ÐÂÔö Éè±¸Ä¬ÈÏÎª123*/
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 8, ArraySubType = UnmanagedType.I1)]
            public byte[] res2;
        }

        //ddns
        [StructLayoutAttribute(LayoutKind.Sequential)]
        public struct NET_DVR_DDNSPARA
        {
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = NAME_LEN, ArraySubType = UnmanagedType.I1)]
            public byte[] sUsername;/* DDNSÕËºÅÓÃ»§Ãû/ÃÜÂë */
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = PASSWD_LEN, ArraySubType = UnmanagedType.I1)]
            public byte[] sPassword;
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 64, ArraySubType = UnmanagedType.I1)]
            public byte[] sDomainName; /* ÓòÃû */
            public byte byEnableDDNS;/*ÊÇ·ñÓ¦ÓÃ 0-·ñ£¬1-ÊÇ*/
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 15, ArraySubType = UnmanagedType.I1)]
            public byte[] res;
        }

        [StructLayoutAttribute(LayoutKind.Sequential)]
        public struct NET_DVR_DDNSPARA_EX
        {
            public byte byHostIndex;/* 0-Hikvision DNS 1£­Dyndns 2£­PeanutHull(»¨Éú¿Ç)*/
            public byte byEnableDDNS;/*ÊÇ·ñÓ¦ÓÃDDNS 0-·ñ£¬1-ÊÇ*/
            public ushort wDDNSPort;/* DDNS¶Ë¿ÚºÅ */
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = NAME_LEN, ArraySubType = UnmanagedType.I1)]
            public byte[] sUsername;/* DDNSÓÃ»§Ãû*/
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = PASSWD_LEN, ArraySubType = UnmanagedType.I1)]
            public byte[] sPassword;/* DDNSÃÜÂë */
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = MAX_DOMAIN_NAME, ArraySubType = UnmanagedType.I1)]
            public byte[] sDomainName;/* Éè±¸Åä±¸µÄÓòÃûµØÖ· */
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = MAX_DOMAIN_NAME, ArraySubType = UnmanagedType.I1)]
            public byte[] sServerName;/* DDNS ¶ÔÓ¦µÄ·þÎñÆ÷µØÖ·£¬¿ÉÒÔÊÇIPµØÖ·»òÓòÃû */
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 16, ArraySubType = UnmanagedType.I1)]
            public byte[] byRes;
        }

        //9000À©Õ¹
        [StructLayoutAttribute(LayoutKind.Sequential)]
        public struct struDDNS
        {
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = NAME_LEN, ArraySubType = UnmanagedType.I1)]
            public byte[] sUsername;/* DDNSÕËºÅÓÃ»§Ãû*/
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = PASSWD_LEN, ArraySubType = UnmanagedType.I1)]
            public byte[] sPassword;/* ÃÜÂë */
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = MAX_DOMAIN_NAME, ArraySubType = UnmanagedType.I1)]
            public byte[] sDomainName;/* Éè±¸Åä±¸µÄÓòÃûµØÖ· */
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = MAX_DOMAIN_NAME, ArraySubType = UnmanagedType.I1)]
            public byte[] sServerName;/* DDNSÐ­Òé¶ÔÓ¦µÄ·þÎñÆ÷µØÖ·£¬¿ÉÒÔÊÇIPµØÖ·»òÓòÃû */
            public ushort wDDNSPort;/* ¶Ë¿ÚºÅ */
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 10, ArraySubType = UnmanagedType.I1)]
            public byte[] byRes;
        }

        [StructLayoutAttribute(LayoutKind.Sequential)]
        public struct NET_DVR_DDNSPARA_V30
        {
            public byte byEnableDDNS;
            public byte byHostIndex;/* 0-Hikvision DNS(±£Áô) 1£­Dyndns 2£­PeanutHull(»¨Éú¿Ç)*/
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 2, ArraySubType = UnmanagedType.I1)]
            public byte[] byRes1;
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = MAX_DDNS_NUMS, ArraySubType = UnmanagedType.Struct)]
            public struDDNS[] struDDNS;
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 16, ArraySubType = UnmanagedType.I1)]
            public byte[] byRes2;
        }

        //email
        [StructLayoutAttribute(LayoutKind.Sequential)]
        public struct NET_DVR_EMAILPARA
        {
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 64, ArraySubType = UnmanagedType.I1)]
            public byte[] sUsername;/* ÓÊ¼þÕËºÅ/ÃÜÂë */
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 64, ArraySubType = UnmanagedType.I1)]
            public byte[] sPassword;
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 64, ArraySubType = UnmanagedType.I1)]
            public byte[] sSmtpServer;
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 64, ArraySubType = UnmanagedType.I1)]
            public byte[] sPop3Server;
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 64, ArraySubType = UnmanagedType.I1)]
            public byte[] sMailAddr;/* email */
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 64, ArraySubType = UnmanagedType.I1)]
            public byte[] sEventMailAddr1;/* ÉÏ´«±¨¾¯/Òì³£µÈµÄemail */
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 64, ArraySubType = UnmanagedType.I1)]
            public byte[] sEventMailAddr2;
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 16, ArraySubType = UnmanagedType.I1)]
            public byte[] res;
        }

        //ÍøÂç²ÎÊýÅäÖÃ
        [StructLayoutAttribute(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
        public struct NET_DVR_NETAPPCFG
        {
            public uint dwSize;
            [MarshalAsAttribute(UnmanagedType.ByValTStr, SizeConst = 16)]
            public string sDNSIp; /* DNS·þÎñÆ÷µØÖ· */
            public NET_DVR_NTPPARA struNtpClientParam;/* NTP²ÎÊý */
            public NET_DVR_DDNSPARA struDDNSClientParam;/* DDNS²ÎÊý */
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 464, ArraySubType = UnmanagedType.I1)]
            public byte[] res;/* ±£Áô */
        }

        //nfs½á¹¹ÅäÖÃ
        [StructLayoutAttribute(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
        public struct NET_DVR_SINGLE_NFS
        {
            [MarshalAsAttribute(UnmanagedType.ByValTStr, SizeConst = 16)]
            public string sNfsHostIPAddr;
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = PATHNAME_LEN, ArraySubType = UnmanagedType.I1)]
            public byte[] sNfsDirectory;

            public void Init()
            {
                this.sNfsDirectory = new byte[PATHNAME_LEN];
            }
        }

        [StructLayoutAttribute(LayoutKind.Sequential)]
        public struct NET_DVR_NFSCFG
        {
            public uint dwSize;
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = MAX_NFS_DISK, ArraySubType = UnmanagedType.Struct)]
            public NET_DVR_SINGLE_NFS[] struNfsDiskParam;

            public void Init()
            {
                this.struNfsDiskParam = new NET_DVR_SINGLE_NFS[MAX_NFS_DISK];

                for (int i = 0; i < MAX_NFS_DISK; i++)
                {
                    struNfsDiskParam[i].Init();
                }
            }
        }

        //Ñ²º½µãÅäÖÃ(HIK IP¿ìÇò×¨ÓÃ)
        [StructLayoutAttribute(LayoutKind.Sequential)]
        public struct NET_DVR_CRUISE_POINT
        {
            public byte PresetNum;//Ô¤ÖÃµã
            public byte Dwell;//Í£ÁôÊ±¼ä
            public byte Speed;//ËÙ¶È
            public byte Reserve;//±£Áô

            public void Init()
            {
                PresetNum = 0;
                Dwell = 0;
                Speed = 0;
                Reserve = 0;
            }
        }

        [StructLayoutAttribute(LayoutKind.Sequential)]
        public struct NET_DVR_CRUISE_RET
        {
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 32, ArraySubType = UnmanagedType.Struct)]
            public NET_DVR_CRUISE_POINT[] struCruisePoint;//×î´óÖ§³Ö32¸öÑ²º½µã

            public void Init()
            {
                struCruisePoint = new NET_DVR_CRUISE_POINT[32];
                for (int i = 0; i < 32; i++)
                {
                    struCruisePoint[i].Init();
                }
            }
        }

        /************************************¶àÂ·½âÂëÆ÷(begin)***************************************/
        [StructLayoutAttribute(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
        public struct NET_DVR_NETCFG_OTHER
        {
            public uint dwSize;
            [MarshalAsAttribute(UnmanagedType.ByValTStr, SizeConst = 16)]
            public string sFirstDNSIP;
            [MarshalAsAttribute(UnmanagedType.ByValTStr, SizeConst = 16)]
            public string sSecondDNSIP;
            [MarshalAsAttribute(UnmanagedType.ByValTStr, SizeConst = 32)]
            public string sRes;
        }

        [StructLayoutAttribute(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
        public struct NET_DVR_MATRIX_DECINFO
        {
            [MarshalAsAttribute(UnmanagedType.ByValTStr, SizeConst = 16)]
            public string sDVRIP;/* DVR IPµØÖ· */
            public ushort wDVRPort;/* ¶Ë¿ÚºÅ */
            public byte byChannel;/* Í¨µÀºÅ */
            public byte byTransProtocol;/* ´«ÊäÐ­ÒéÀàÐÍ 0-TCP, 1-UDP */
            public byte byTransMode;/* ´«ÊäÂëÁ÷Ä£Ê½ 0£­Ö÷ÂëÁ÷ 1£­×ÓÂëÁ÷*/
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 3, ArraySubType = UnmanagedType.I1)]
            public byte[] byRes;
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = NAME_LEN, ArraySubType = UnmanagedType.I1)]
            public byte[] sUserName;/* ¼à¿ØÖ÷»úµÇÂ½ÕÊºÅ */
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = PASSWD_LEN, ArraySubType = UnmanagedType.I1)]
            public byte[] sPassword;/* ¼à¿ØÖ÷»úÃÜÂë */
        }

        //Æô¶¯/Í£Ö¹¶¯Ì¬½âÂë
        [StructLayoutAttribute(LayoutKind.Sequential)]
        public struct NET_DVR_MATRIX_DYNAMIC_DEC
        {
            public uint dwSize;
            public NET_DVR_MATRIX_DECINFO struDecChanInfo;/* ¶¯Ì¬½âÂëÍ¨µÀÐÅÏ¢ */
        }

        [StructLayoutAttribute(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
        public struct NET_DVR_MATRIX_DEC_CHAN_STATUS
        {
            public uint dwSize;
            public uint dwIsLinked;/* ½âÂëÍ¨µÀ×´Ì¬ 0£­ÐÝÃß 1£­ÕýÔÚÁ¬½Ó 2£­ÒÑÁ¬½Ó 3-ÕýÔÚ½âÂë */
            public uint dwStreamCpRate;/* Stream copy rate, X kbits/second */
            [MarshalAsAttribute(UnmanagedType.ByValTStr, SizeConst = 64)]
            public string cRes;/* ±£Áô */
        }

        [StructLayoutAttribute(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
        public struct NET_DVR_MATRIX_DEC_CHAN_INFO
        {
            public uint dwSize;
            public NET_DVR_MATRIX_DECINFO struDecChanInfo;/* ½âÂëÍ¨µÀÐÅÏ¢ */
            public uint dwDecState;/* 0-¶¯Ì¬½âÂë 1£­Ñ­»·½âÂë 2£­°´Ê±¼ä»Ø·Å 3£­°´ÎÄ¼þ»Ø·Å */
            public NET_DVR_TIME StartTime;/* °´Ê±¼ä»Ø·Å¿ªÊ¼Ê±¼ä */
            public NET_DVR_TIME StopTime;/* °´Ê±¼ä»Ø·ÅÍ£Ö¹Ê±¼ä */
            [MarshalAsAttribute(UnmanagedType.ByValTStr, SizeConst = 128)]
            public string sFileName;/* °´ÎÄ¼þ»Ø·ÅÎÄ¼þÃû */
        }

        //Á¬½ÓµÄÍ¨µÀÅäÖÃ 2007-11-05
        [StructLayoutAttribute(LayoutKind.Sequential)]
        public struct NET_DVR_MATRIX_DECCHANINFO
        {
            public uint dwEnable;/* ÊÇ·ñÆôÓÃ 0£­·ñ 1£­ÆôÓÃ*/
            public NET_DVR_MATRIX_DECINFO struDecChanInfo;/* ÂÖÑ­½âÂëÍ¨µÀÐÅÏ¢ */
        }

        //2007-11-05 ÐÂÔöÃ¿¸ö½âÂëÍ¨µÀµÄÅäÖÃ
        [StructLayoutAttribute(LayoutKind.Sequential)]
        public struct NET_DVR_MATRIX_LOOP_DECINFO
        {
            public uint dwSize;
            public uint dwPoolTime;/*ÂÖÑ²Ê±¼ä */
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = MAX_CYCLE_CHAN, ArraySubType = UnmanagedType.Struct)]
            public NET_DVR_MATRIX_DECCHANINFO[] struchanConInfo;
        }

        //2007-12-22
        [StructLayoutAttribute(LayoutKind.Sequential)]
        public struct TTY_CONFIG
        {
            public byte baudrate;/* ²¨ÌØÂÊ */
            public byte databits;/* Êý¾ÝÎ» */
            public byte stopbits;/* Í£Ö¹Î» */
            public byte parity;/* ÆæÅ¼Ð£ÑéÎ» */
            public byte flowcontrol;/* Á÷¿Ø */
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 3, ArraySubType = UnmanagedType.I1)]
            public byte[] res;
        }

        [StructLayoutAttribute(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
        public struct NET_DVR_MATRIX_TRAN_CHAN_INFO
        {
            public byte byTranChanEnable;/* µ±Ç°Í¸Ã÷Í¨µÀÊÇ·ñ´ò¿ª 0£º¹Ø±Õ 1£º´ò¿ª */
            /*
             *	¶àÂ·½âÂëÆ÷±¾µØÓÐ1¸ö485´®¿Ú£¬1¸ö232´®¿Ú¶¼¿ÉÒÔ×÷ÎªÍ¸Ã÷Í¨µÀ,Éè±¸ºÅ·ÖÅäÈçÏÂ£º
             *	0 RS485
             *	1 RS232 Console
             */
            public byte byLocalSerialDevice;/* Local serial device */
            /*
             *	Ô¶³Ì´®¿ÚÊä³ö»¹ÊÇÁ½¸ö,Ò»¸öRS232£¬Ò»¸öRS485
             *	1±íÊ¾232´®¿Ú
             *	2±íÊ¾485´®¿Ú
             */
            public byte byRemoteSerialDevice;/* Remote output serial device */
            public byte res1;/* ±£Áô */
            [MarshalAsAttribute(UnmanagedType.ByValTStr, SizeConst = 16)]
            public string sRemoteDevIP;/* Remote Device IP */
            public ushort wRemoteDevPort;/* Remote Net Communication Port */
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 2, ArraySubType = UnmanagedType.I1)]
            public byte[] res2;/* ±£Áô */
            public TTY_CONFIG RemoteSerialDevCfg;
        }

        [StructLayoutAttribute(LayoutKind.Sequential)]
        public struct NET_DVR_MATRIX_TRAN_CHAN_CONFIG
        {
            public uint dwSize;
            public byte by232IsDualChan;/* ÉèÖÃÄÄÂ·232Í¸Ã÷Í¨µÀÊÇÈ«Ë«¹¤µÄ È¡Öµ1µ½MAX_SERIAL_NUM */
            public byte by485IsDualChan;/* ÉèÖÃÄÄÂ·485Í¸Ã÷Í¨µÀÊÇÈ«Ë«¹¤µÄ È¡Öµ1µ½MAX_SERIAL_NUM */
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 2, ArraySubType = UnmanagedType.I1)]
            public byte[] res;/* ±£Áô */
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = MAX_SERIAL_NUM, ArraySubType = UnmanagedType.Struct)]
            public NET_DVR_MATRIX_TRAN_CHAN_INFO[] struTranInfo;/*Í¬Ê±Ö§³Ö½¨Á¢MAX_SERIAL_NUM¸öÍ¸Ã÷Í¨µÀ*/
        }

        //2007-12-24 Merry Christmas Eve...
        [StructLayoutAttribute(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
        public struct NET_DVR_MATRIX_DEC_REMOTE_PLAY
        {
            public uint dwSize;
            [MarshalAsAttribute(UnmanagedType.ByValTStr, SizeConst = 16)]
            public string sDVRIP;/* DVR IPµØÖ· */
            public ushort wDVRPort;/* ¶Ë¿ÚºÅ */
            public byte byChannel;/* Í¨µÀºÅ */
            public byte byReserve;
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = NAME_LEN, ArraySubType = UnmanagedType.I1)]
            public byte[] sUserName;/* ÓÃ»§Ãû */
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = PASSWD_LEN, ArraySubType = UnmanagedType.I1)]
            public byte[] sPassword;/* ÃÜÂë */
            public uint dwPlayMode;/* 0£­°´ÎÄ¼þ 1£­°´Ê±¼ä*/
            public NET_DVR_TIME StartTime;
            public NET_DVR_TIME StopTime;
            [MarshalAsAttribute(UnmanagedType.ByValTStr, SizeConst = 128)]
            public string sFileName;
        }

        [StructLayoutAttribute(LayoutKind.Sequential)]
        public struct NET_DVR_MATRIX_DEC_REMOTE_PLAY_CONTROL
        {
            public uint dwSize;
            public uint dwPlayCmd;/* ²¥·ÅÃüÁî ¼ûÎÄ¼þ²¥·ÅÃüÁî*/
            public uint dwCmdParam;/* ²¥·ÅÃüÁî²ÎÊý */
        }

        [StructLayoutAttribute(LayoutKind.Sequential)]
        public struct NET_DVR_MATRIX_DEC_REMOTE_PLAY_STATUS
        {
            public uint dwSize;
            public uint dwCurMediaFileLen;/* µ±Ç°²¥·ÅµÄÃ½ÌåÎÄ¼þ³¤¶È */
            public uint dwCurMediaFilePosition;/* µ±Ç°²¥·ÅÎÄ¼þµÄ²¥·ÅÎ»ÖÃ */
            public uint dwCurMediaFileDuration;/* µ±Ç°²¥·ÅÎÄ¼þµÄ×ÜÊ±¼ä */
            public uint dwCurPlayTime;/* µ±Ç°ÒÑ¾­²¥·ÅµÄÊ±¼ä */
            public uint dwCurMediaFIleFrames;/* µ±Ç°²¥·ÅÎÄ¼þµÄ×ÜÖ¡Êý */
            public uint dwCurDataType;/* µ±Ç°´«ÊäµÄÊý¾ÝÀàÐÍ£¬19-ÎÄ¼þÍ·£¬20-Á÷Êý¾Ý£¬ 21-²¥·Å½áÊø±êÖ¾ */
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 72, ArraySubType = UnmanagedType.I1)]
            public byte[] res;
        }

        //2009-4-11 added by likui ¶àÂ·½âÂëÆ÷new
        [StructLayoutAttribute(LayoutKind.Sequential)]
        public struct tagNET_MATRIX_PASSIVEMODE
        {
            public ushort wTransProtol;//´«ÊäÐ­Òé£¬0-TCP, 1-UDP, 2-MCAST
            public ushort wPassivePort;//UDP¶Ë¿Ú, TCPÊ±Ä¬ÈÏ
                                       // char	sMcastIP[16];  //TCP,UDPÊ±ÎÞÐ§, MCASTÊ±Îª¶à²¥µØÖ·
            public NET_DVR_IPADDR struMcastIP;
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 8, ArraySubType = UnmanagedType.I1)]
            public byte[] res;
        }

        [StructLayoutAttribute(LayoutKind.Sequential)]
        public struct tagDEV_CHAN_INFO
        {
            public NET_DVR_IPADDR struIP;/* DVR IPµØÖ· */
            public ushort wDVRPort;/* ¶Ë¿ÚºÅ */
            public byte byChannel;/* Í¨µÀºÅ */
            public byte byTransProtocol;/* ´«ÊäÐ­ÒéÀàÐÍ0-TCP£¬1-UDP */
            public byte byTransMode;/* ´«ÊäÂëÁ÷Ä£Ê½ 0£­Ö÷ÂëÁ÷ 1£­×ÓÂëÁ÷*/
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 71, ArraySubType = UnmanagedType.I1)]
            public byte[] byRes;
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = NAME_LEN, ArraySubType = UnmanagedType.I1)]
            public byte[] sUserName;/* ¼à¿ØÖ÷»úµÇÂ½ÕÊºÅ */
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = PASSWD_LEN, ArraySubType = UnmanagedType.I1)]
            public byte[] sPassword;/* ¼à¿ØÖ÷»úÃÜÂë */
        }

        [StructLayoutAttribute(LayoutKind.Sequential)]
        public struct tagMATRIX_TRAN_CHAN_INFO
        {
            public byte byTranChanEnable;/* µ±Ç°Í¸Ã÷Í¨µÀÊÇ·ñ´ò¿ª 0£º¹Ø±Õ 1£º´ò¿ª */
            /*
             *	¶àÂ·½âÂëÆ÷±¾µØÓÐ1¸ö485´®¿Ú£¬1¸ö232´®¿Ú¶¼¿ÉÒÔ×÷ÎªÍ¸Ã÷Í¨µÀ,Éè±¸ºÅ·ÖÅäÈçÏÂ£º
             *	0 RS485
             *	1 RS232 Console
             */
            public byte byLocalSerialDevice;/* Local serial device */
            /*
             *	Ô¶³Ì´®¿ÚÊä³ö»¹ÊÇÁ½¸ö,Ò»¸öRS232£¬Ò»¸öRS485
             *	1±íÊ¾232´®¿Ú
             *	2±íÊ¾485´®¿Ú
             */
            public byte byRemoteSerialDevice;/* Remote output serial device */
            public byte byRes1;/* ±£Áô */
            public NET_DVR_IPADDR struRemoteDevIP;/* Remote Device IP */
            public ushort wRemoteDevPort;/* Remote Net Communication Port */
            public byte byIsEstablished;/* Í¸Ã÷Í¨µÀ½¨Á¢³É¹¦±êÖ¾£¬0-Ã»ÓÐ³É¹¦£¬1-½¨Á¢³É¹¦ */
            public byte byRes2;/* ±£Áô */
            public TTY_CONFIG RemoteSerialDevCfg;
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = NAME_LEN, ArraySubType = UnmanagedType.I1)]
            public byte[] byUsername;/* 32BYTES */
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = PASSWD_LEN, ArraySubType = UnmanagedType.I1)]
            public byte[] byPassword;/* 16BYTES */
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 16, ArraySubType = UnmanagedType.I1)]
            public byte[] byRes3;
        }

        [StructLayoutAttribute(LayoutKind.Sequential)]
        public struct tagMATRIX_TRAN_CHAN_CONFIG
        {
            public uint dwSize;
            public byte by232IsDualChan;/* ÉèÖÃÄÄÂ·232Í¸Ã÷Í¨µÀÊÇÈ«Ë«¹¤µÄ È¡Öµ1µ½MAX_SERIAL_NUM */
            public byte by485IsDualChan;/* ÉèÖÃÄÄÂ·485Í¸Ã÷Í¨µÀÊÇÈ«Ë«¹¤µÄ È¡Öµ1µ½MAX_SERIAL_NUM */
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 2, ArraySubType = UnmanagedType.I1)]
            public byte[] vyRes;/* ±£Áô */
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = MAX_SERIAL_NUM, ArraySubType = UnmanagedType.Struct)]
            public tagMATRIX_TRAN_CHAN_INFO[] struTranInfo;/*Í¬Ê±Ö§³Ö½¨Á¢MAX_SERIAL_NUM¸öÍ¸Ã÷Í¨µÀ*/
        }

        /*Á÷Ã½Ìå·þÎñÆ÷»ù±¾ÅäÖÃ*/
        [StructLayoutAttribute(LayoutKind.Sequential)]
        public struct NET_DVR_STREAM_MEDIA_SERVER_CFG
        {
            public byte byValid;/*ÊÇ·ñÆôÓÃÁ÷Ã½Ìå·þÎñÆ÷È¡Á÷,0±íÊ¾ÎÞÐ§£¬·Ç0±íÊ¾ÓÐÐ§*/
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 3, ArraySubType = UnmanagedType.I1)]
            public byte[] byRes1;
            public NET_DVR_IPADDR struDevIP;
            public ushort wDevPort;/*Á÷Ã½Ìå·þÎñÆ÷¶Ë¿Ú*/
            public byte byTransmitType;/*´«ÊäÐ­ÒéÀàÐÍ 0-TCP£¬1-UDP*/
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 69, ArraySubType = UnmanagedType.I1)]
            public byte[] byRes2;
        }

        [StructLayoutAttribute(LayoutKind.Sequential)]
        public struct NET_DVR_PU_STREAM_CFG
        {
            public uint dwSize;
            public NET_DVR_STREAM_MEDIA_SERVER_CFG struStreamMediaSvrCfg;
            public tagDEV_CHAN_INFO struDevChanInfo;
        }

        [StructLayoutAttribute(LayoutKind.Sequential)]
        public struct NET_DVR_MATRIX_CHAN_INFO_V30
        {
            public uint dwEnable;/* ÊÇ·ñÆôÓÃ 0£­·ñ 1£­ÆôÓÃ*/
            public NET_DVR_STREAM_MEDIA_SERVER_CFG streamMediaServerCfg;
            public tagDEV_CHAN_INFO struDevChanInfo;/* ÂÖÑ­½âÂëÍ¨µÀÐÅÏ¢ */
        }

        [StructLayoutAttribute(LayoutKind.Sequential)]
        public struct tagMATRIX_LOOP_DECINFO_V30
        {
            public uint dwSize;
            public uint dwPoolTime;
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = MAX_CYCLE_CHAN_V30, ArraySubType = UnmanagedType.Struct)]
            public NET_DVR_MATRIX_CHAN_INFO_V30[] struchanConInfo;
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 16, ArraySubType = UnmanagedType.I1)]
            public byte[] byRes;
        }

        [StructLayoutAttribute(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
        public struct tagDEC_MATRIX_CHAN_INFO
        {
            public uint dwSize;
            public NET_DVR_STREAM_MEDIA_SERVER_CFG streamMediaServerCfg;/*Á÷Ã½Ìå·þÎñÆ÷ÅäÖÃ*/
            public tagDEV_CHAN_INFO struDevChanInfo;/* ½âÂëÍ¨µÀÐÅÏ¢ */
            public uint dwDecState;/* 0-¶¯Ì¬½âÂë 1£­Ñ­»·½âÂë 2£­°´Ê±¼ä»Ø·Å 3£­°´ÎÄ¼þ»Ø·Å */
            public NET_DVR_TIME StartTime;/* °´Ê±¼ä»Ø·Å¿ªÊ¼Ê±¼ä */
            public NET_DVR_TIME StopTime;/* °´Ê±¼ä»Ø·ÅÍ£Ö¹Ê±¼ä */
            [MarshalAsAttribute(UnmanagedType.ByValTStr, SizeConst = 128)]
            public string sFileName;/* °´ÎÄ¼þ»Ø·ÅÎÄ¼þÃû */
            public uint dwGetStreamMode;/*È¡Á÷Ä£Ê½:1-Ö÷¶¯£¬2-±»¶¯*/
            public tagNET_MATRIX_PASSIVEMODE struPassiveMode;
        }

        [StructLayoutAttribute(LayoutKind.Sequential)]
        public struct tagNET_DVR_MATRIX_ABILITY
        {
            public uint dwSize;
            public byte byDecNums;
            public byte byStartChan;
            public byte byVGANums;
            public byte byBNCNums;
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 12, ArraySubType = UnmanagedType.I1)]
            public byte[] byVGAWindowMode;/*VGAÖ§³ÖµÄ´°¿ÚÄ£Ê½*/
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 4, ArraySubType = UnmanagedType.I1)]
            public byte[] byBNCWindowMode;/*BNCÖ§³ÖµÄ´°¿ÚÄ£Ê½*/
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 16, ArraySubType = UnmanagedType.I1)]
            public byte[] res;
        }

        //ÉÏ´«logo½á¹¹
        [StructLayoutAttribute(LayoutKind.Sequential)]
        public struct tagNET_DVR_DISP_LOGOCFG
        {
            public uint dwCorordinateX;//Í¼Æ¬ÏÔÊ¾ÇøÓòX×ø±ê
            public uint dwCorordinateY;//Í¼Æ¬ÏÔÊ¾ÇøÓòY×ø±ê
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 8, ArraySubType = UnmanagedType.I1)]
            public byte[] byRes1;
            public byte byFlash;//ÊÇ·ñÉÁË¸1-ÉÁË¸£¬0-²»ÉÁË¸
            public byte byTranslucent;//ÊÇ·ñ°ëÍ¸Ã÷1-°ëÍ¸Ã÷£¬0-²»°ëÍ¸Ã÷
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 6, ArraySubType = UnmanagedType.I1)]
            public byte[] byRes2;//±£Áô
            public uint dwLogoSize;//LOGO´óÐ¡£¬°üÀ¨BMPµÄÎÄ¼þÍ·
        }

        /*±àÂëÀàÐÍ*/
        public const int NET_DVR_ENCODER_UNKOWN = 0;/*Î´Öª±àÂë¸ñÊ½*/
        public const int NET_DVR_ENCODER_H264 = 1;/*HIK 264*/
        public const int NET_DVR_ENCODER_S264 = 2;/*Standard H264*/
        public const int NET_DVR_ENCODER_MPEG4 = 3;/*MPEG4*/
        /* ´ò°ü¸ñÊ½ */
        public const int NET_DVR_STREAM_TYPE_UNKOWN = 0;/*Î´Öª´ò°ü¸ñÊ½*/
        public const int NET_DVR_STREAM_TYPE_HIKPRIVT = 1; /*º£¿µ×Ô¶¨Òå´ò°ü¸ñÊ½*/
        public const int NET_DVR_STREAM_TYPE_TS = 7;/* TS´ò°ü */
        public const int NET_DVR_STREAM_TYPE_PS = 8;/* PS´ò°ü */
        public const int NET_DVR_STREAM_TYPE_RTP = 9;/* RTP´ò°ü */

        /*½âÂëÍ¨µÀ×´Ì¬*/
        [StructLayoutAttribute(LayoutKind.Sequential)]
        public struct NET_DVR_MATRIX_CHAN_STATUS
        {
            public byte byDecodeStatus;/*µ±Ç°×´Ì¬:0:Î´Æô¶¯£¬1£ºÆô¶¯½âÂë*/
            public byte byStreamType;/*ÂëÁ÷ÀàÐÍ*/
            public byte byPacketType;/*´ò°ü·½Ê½*/
            public byte byRecvBufUsage;/*½ÓÊÕ»º³åÊ¹ÓÃÂÊ*/
            public byte byDecBufUsage;/*½âÂë»º³åÊ¹ÓÃÂÊ*/
            public byte byFpsDecV;/*ÊÓÆµ½âÂëÖ¡ÂÊ*/
            public byte byFpsDecA;/*ÒôÆµ½âÂëÖ¡ÂÊ*/
            public byte byCpuLoad;/*DSP CPUÊ¹ÓÃÂÊ*/
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 4, ArraySubType = UnmanagedType.I1)]
            public byte[] byRes1;
            public uint dwDecodedV;/*½âÂëµÄÊÓÆµÖ¡*/
            public uint dwDecodedA;/*½âÂëµÄÒôÆµÖ¡*/
            public ushort wImgW;/*½âÂëÆ÷µ±Ç°µÄÍ¼Ïñ´óÐ¡,¿í*/
            public ushort wImgH; //¸ß
            public byte byVideoFormat;/*ÊÓÆµÖÆÊ½:0-NON,NTSC--1,PAL--2*/
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 27, ArraySubType = UnmanagedType.I1)]
            public byte[] byRes2;
        }

        /*ÏÔÊ¾Í¨µÀ×´Ì¬*/
        public const int NET_DVR_MAX_DISPREGION = 16;         /*Ã¿¸öÏÔÊ¾Í¨µÀ×î¶à¿ÉÒÔÏÔÊ¾µÄ´°¿Ú*/
        //VGA·Ö±æÂÊ£¬Ä¿Ç°ÄÜÓÃµÄÊÇ£ºVGA_THS8200_MODE_XGA_60HZ¡¢VGA_THS8200_MODE_SXGA_60HZ¡¢
        //
        public enum VGA_MODE
        {
            VGA_NOT_AVALIABLE,
            VGA_THS8200_MODE_SVGA_60HZ,//£¨800*600£©
            VGA_THS8200_MODE_SVGA_75HZ, //£¨800*600£©
            VGA_THS8200_MODE_XGA_60HZ,//£¨1024*768£©
            VGA_THS8200_MODE_XGA_70HZ, //£¨1024*768£©
            VGA_THS8200_MODE_SXGA_60HZ,//£¨1280*1024£©
            VGA_THS8200_MODE_720P_60HZ,//£¨1280*720 £©
            VGA_THS8200_MODE_1080i_60HZ,//£¨1920*1080£©
            VGA_THS8200_MODE_1080P_30HZ,//£¨1920*1080£©
            VGA_THS8200_MODE_1080P_25HZ,//£¨1920*1080£©
            VGA_THS8200_MODE_UXGA_30HZ,//£¨1600*1200£©
        }

        /*ÊÓÆµÖÆÊ½±ê×¼*/
        public enum VIDEO_STANDARD
        {
            VS_NON = 0,
            VS_NTSC = 1,
            VS_PAL = 2,
        }

        [StructLayoutAttribute(LayoutKind.Sequential)]
        public struct tagNET_DVR_VGA_DISP_CHAN_CFG
        {
            public uint dwSize;
            public byte byAudio;/*ÒôÆµÊÇ·ñ¿ªÆô,0-·ñ£¬1-ÊÇ*/
            public byte byAudioWindowIdx;/*ÒôÆµ¿ªÆô×Ó´°¿Ú*/
            public byte byVgaResolution;/*VGAµÄ·Ö±æÂÊ*/
            public byte byVedioFormat;/*ÊÓÆµÖÆÊ½£¬1:NTSC,2:PAL,0-NON*/
            public uint dwWindowMode;/*»­ÃæÄ£Ê½£¬´ÓÄÜÁ¦¼¯Àï»ñÈ¡£¬Ä¿Ç°Ö§³Ö1,2,4,9,16*/
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = MAX_WINDOWS, ArraySubType = UnmanagedType.I1)]
            public byte[] byJoinDecChan;/*¸÷¸ö×Ó´°¿Ú¹ØÁªµÄ½âÂëÍ¨µÀ*/
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 20, ArraySubType = UnmanagedType.I1)]
            public byte[] byRes;
        }

        [StructLayoutAttribute(LayoutKind.Sequential)]
        public struct NET_DVR_DISP_CHAN_STATUS
        {
            public byte byDispStatus;/*ÏÔÊ¾×´Ì¬£º0£ºÎ´ÏÔÊ¾£¬1£ºÆô¶¯ÏÔÊ¾*/
            public byte byBVGA; /*VGA/BNC*/
            public byte byVideoFormat;/*ÊÓÆµÖÆÊ½:1:NTSC,2:PAL,0-NON*/
            public byte byWindowMode;/*µ±Ç°»­ÃæÄ£Ê½*/
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 10, ArraySubType = UnmanagedType.I1)]
            public byte[] byJoinDecChan;/*¸÷¸ö×Ó´°¿Ú¹ØÁªµÄ½âÂëÍ¨µÀ*/
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = NET_DVR_MAX_DISPREGION, ArraySubType = UnmanagedType.I1)]
            public byte[] byFpsDisp;/*Ã¿¸ö×Ó»­ÃæµÄÏÔÊ¾Ö¡ÂÊ*/
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 32, ArraySubType = UnmanagedType.I1)]
            public byte[] byRes2;
        }

        public const int MAX_DECODECHANNUM = 32;//¶àÂ·½âÂëÆ÷×î´ó½âÂëÍ¨µÀÊý
        public const int MAX_DISPCHANNUM = 24;//¶àÂ·½âÂëÆ÷×î´óÏÔÊ¾Í¨µÀÊý

        /*½âÂëÆ÷Éè±¸×´Ì¬*/
        [StructLayoutAttribute(LayoutKind.Sequential)]
        public struct tagNET_DVR__DECODER_WORK_STATUS
        {
            public uint dwSize;
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = MAX_DECODECHANNUM, ArraySubType = UnmanagedType.Struct)]
            public NET_DVR_MATRIX_CHAN_STATUS[] struDecChanStatus;/*½âÂëÍ¨µÀ×´Ì¬*/
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = MAX_DISPCHANNUM, ArraySubType = UnmanagedType.Struct)]
            public NET_DVR_DISP_CHAN_STATUS[] struDispChanStatus;/*ÏÔÊ¾Í¨µÀ×´Ì¬*/
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = MAX_ANALOG_ALARMIN, ArraySubType = UnmanagedType.I1)]
            public byte[] byAlarmInStatus;/*±¨¾¯ÊäÈë×´Ì¬*/
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = MAX_ANALOG_ALARMOUT, ArraySubType = UnmanagedType.I1)]
            public byte[] byAalarmOutStatus;/*±¨¾¯Êä³ö×´Ì¬*/
            public byte byAudioInChanStatus;/*ÓïÒô¶Ô½²×´Ì¬*/
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 127, ArraySubType = UnmanagedType.I1)]
            public byte[] byRes;
        }
        /************************************¶àÂ·½âÂëÆ÷(end)***************************************/

        [StructLayoutAttribute(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
        public struct NET_DVR_EMAILCFG
        {
            public uint dwSize;
            [MarshalAsAttribute(UnmanagedType.ByValTStr, SizeConst = 32)]
            public string sUserName;
            [MarshalAsAttribute(UnmanagedType.ByValTStr, SizeConst = 32)]
            public string sPassWord;
            [MarshalAsAttribute(UnmanagedType.ByValTStr, SizeConst = 32)]
            public string sFromName;/* Sender *///×Ö·û´®ÖÐµÄµÚÒ»¸ö×Ö·ûºÍ×îºóÒ»¸ö×Ö·û²»ÄÜÊÇ"@",²¢ÇÒ×Ö·û´®ÖÐÒªÓÐ"@"×Ö·û
            [MarshalAsAttribute(UnmanagedType.ByValTStr, SizeConst = 48)]
            public string sFromAddr;/* Sender address */
            [MarshalAsAttribute(UnmanagedType.ByValTStr, SizeConst = 32)]
            public string sToName1;/* Receiver1 */
            [MarshalAsAttribute(UnmanagedType.ByValTStr, SizeConst = 32)]
            public string sToName2;/* Receiver2 */
            [MarshalAsAttribute(UnmanagedType.ByValTStr, SizeConst = 48)]
            public string sToAddr1;/* Receiver address1 */
            [MarshalAsAttribute(UnmanagedType.ByValTStr, SizeConst = 48)]
            public string sToAddr2;/* Receiver address2 */
            [MarshalAsAttribute(UnmanagedType.ByValTStr, SizeConst = 32)]
            public string sEmailServer;/* Email server address */
            public byte byServerType;/* Email server type: 0-SMTP, 1-POP, 2-IMTP¡­*/
            public byte byUseAuthen;/* Email server authentication method: 1-enable, 0-disable */
            public byte byAttachment;/* enable attachment */
            public byte byMailinterval;/* mail interval 0-2s, 1-3s, 2-4s. 3-5s*/
        }

        [StructLayoutAttribute(LayoutKind.Sequential)]
        public struct NET_DVR_COMPRESSIONCFG_NEW
        {
            public uint dwSize;
            public NET_DVR_COMPRESSION_INFO_EX struLowCompression;//¶¨Ê±Â¼Ïñ
            public NET_DVR_COMPRESSION_INFO_EX struEventCompression;//ÊÂ¼þ´¥·¢Â¼Ïñ
        }

        //Çò»úÎ»ÖÃÐÅÏ¢
        [StructLayoutAttribute(LayoutKind.Sequential)]
        public struct NET_DVR_PTZPOS
        {
            public ushort wAction;//»ñÈ¡Ê±¸Ã×Ö¶ÎÎÞÐ§
            public ushort wPanPos;//Ë®Æ½²ÎÊý
            public ushort wTiltPos;//´¹Ö±²ÎÊý
            public ushort wZoomPos;//±ä±¶²ÎÊý
        }

        //Çò»ú·¶Î§ÐÅÏ¢
        [StructLayoutAttribute(LayoutKind.Sequential)]
        public struct NET_DVR_PTZSCOPE
        {
            public ushort wPanPosMin;//Ë®Æ½²ÎÊýmin
            public ushort wPanPosMax;//Ë®Æ½²ÎÊýmax
            public ushort wTiltPosMin;//´¹Ö±²ÎÊýmin
            public ushort wTiltPosMax;//´¹Ö±²ÎÊýmax
            public ushort wZoomPosMin;//±ä±¶²ÎÊýmin
            public ushort wZoomPosMax;//±ä±¶²ÎÊýmax
        }

        //rtspÅäÖÃ ipcamera×¨ÓÃ
        [StructLayoutAttribute(LayoutKind.Sequential)]
        public struct NET_DVR_RTSPCFG
        {
            public uint dwSize;//³¤¶È
            public ushort wPort;//rtsp·þÎñÆ÷ÕìÌý¶Ë¿Ú
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 54, ArraySubType = UnmanagedType.I1)]
            public byte[] byReserve;//Ô¤Áô
        }

        /********************************½Ó¿Ú²ÎÊý½á¹¹(begin)*********************************/

        //NET_DVR_Login()²ÎÊý½á¹¹
        [StructLayoutAttribute(LayoutKind.Sequential)]
        public struct NET_DVR_DEVICEINFO
        {
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = SERIALNO_LEN, ArraySubType = UnmanagedType.I1)]
            public byte[] sSerialNumber;//ÐòÁÐºÅ
            public byte byAlarmInPortNum;//DVR±¨¾¯ÊäÈë¸öÊý
            public byte byAlarmOutPortNum;//DVR±¨¾¯Êä³ö¸öÊý
            public byte byDiskNum;//DVRÓ²ÅÌ¸öÊý
            public byte byDVRType;//DVRÀàÐÍ, 1:DVR 2:ATM DVR 3:DVS ......
            public byte byChanNum;//DVR Í¨µÀ¸öÊý
            public byte byStartChan;//ÆðÊ¼Í¨µÀºÅ,ÀýÈçDVS-1,DVR - 1
        }

        //NET_DVR_Login_V30()²ÎÊý½á¹¹
        [StructLayoutAttribute(LayoutKind.Sequential)]
        public struct NET_DVR_DEVICEINFO_V30
        {
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = SERIALNO_LEN, ArraySubType = UnmanagedType.I1)]
            public byte[] sSerialNumber;//ÐòÁÐºÅ
            public byte byAlarmInPortNum;//±¨¾¯ÊäÈë¸öÊý
            public byte byAlarmOutPortNum;//±¨¾¯Êä³ö¸öÊý
            public byte byDiskNum;//Ó²ÅÌ¸öÊý
            public byte byDVRType;//Éè±¸ÀàÐÍ, 1:DVR 2:ATM DVR 3:DVS ...
            public byte byChanNum;//Ä£ÄâÍ¨µÀ¸öÊý
            public byte byStartChan;//ÆðÊ¼Í¨µÀºÅ,ÀýÈçDVS-1,DVR - 1
            public byte byAudioChanNum;//ÓïÒôÍ¨µÀÊý
            public byte byIPChanNum;//×î´óÊý×ÖÍ¨µÀ¸öÊý  
            public byte byZeroChanNum;      //ÁãÍ¨µÀ±àÂë¸öÊý //2010-01-16
            public byte byMainProto;    //Ö÷ÂëÁ÷´«ÊäÐ­ÒéÀàÐÍ 0-private, 1-rtsp,2-Í¬Ê±Ö§³ÖprivateºÍrtsp
            public byte bySubProto;    //×ÓÂëÁ÷´«ÊäÐ­ÒéÀàÐÍ0-private, 1-rtsp,2-Í¬Ê±Ö§³ÖprivateºÍrtsp
            public byte bySupport;        //ÄÜÁ¦£¬Î»Óë½á¹ûÎª0±íÊ¾²»Ö§³Ö£¬1±íÊ¾Ö§³Ö£¬
                                          //bySupport & 0x1, ±íÊ¾ÊÇ·ñÖ§³ÖÖÇÄÜËÑË÷
                                          //bySupport & 0x2, ±íÊ¾ÊÇ·ñÖ§³Ö±¸·Ý
                                          //bySupport & 0x4, ±íÊ¾ÊÇ·ñÖ§³ÖÑ¹Ëõ²ÎÊýÄÜÁ¦»ñÈ¡
                                          //bySupport & 0x8, ±íÊ¾ÊÇ·ñÖ§³Ö¶àÍø¿¨
                                          //bySupport & 0x10, ±íÊ¾Ö§³ÖÔ¶³ÌSADP
                                          //bySupport & 0x20, ±íÊ¾Ö§³ÖRaid¿¨¹¦ÄÜ
                                          //bySupport & 0x40, ±íÊ¾Ö§³ÖIPSAN Ä¿Â¼²éÕÒ
                                          //bySupport & 0x80, ±íÊ¾Ö§³Örtp over rtsp
            public byte bySupport1;        // ÄÜÁ¦¼¯À©³ä£¬Î»Óë½á¹ûÎª0±íÊ¾²»Ö§³Ö£¬1±íÊ¾Ö§³Ö
                                           //bySupport1 & 0x1, ±íÊ¾ÊÇ·ñÖ§³Ösnmp v30
                                           //bySupport1 & 0x2, Ö§³ÖÇø·Ö»Ø·ÅºÍÏÂÔØ
                                           //bySupport1 & 0x4, ÊÇ·ñÖ§³Ö²¼·ÀÓÅÏÈ¼¶	
                                           //bySupport1 & 0x8, ÖÇÄÜÉè±¸ÊÇ·ñÖ§³Ö²¼·ÀÊ±¼ä¶ÎÀ©Õ¹
                                           //bySupport1 & 0x10, ±íÊ¾ÊÇ·ñÖ§³Ö¶à´ÅÅÌÊý£¨³¬¹ý33¸ö£©
                                           //bySupport1 & 0x20, ±íÊ¾ÊÇ·ñÖ§³Örtsp over http	
                                           //bySupport1 & 0x80, ±íÊ¾ÊÇ·ñÖ§³Ö³µÅÆÐÂ±¨¾¯ÐÅÏ¢2012-9-28, ÇÒ»¹±íÊ¾ÊÇ·ñÖ§³ÖNET_DVR_IPPARACFG_V40½á¹¹Ìå
            public byte bySupport2; /*ÄÜÁ¦£¬Î»Óë½á¹ûÎª0±íÊ¾²»Ö§³Ö£¬·Ç0±íÊ¾Ö§³Ö      	
                     bySupport2 & 0x1, ±íÊ¾½âÂëÆ÷ÊÇ·ñÖ§³ÖÍ¨¹ýURLÈ¡Á÷½âÂë
                     bySupport2 & 0x2,  ±íÊ¾Ö§³ÖFTPV40
                     bySupport2 & 0x4,  ±íÊ¾Ö§³ÖANR
                     bySupport2 & 0x8,  ±íÊ¾Ö§³ÖCCDµÄÍ¨µÀ²ÎÊýÅäÖÃ
                     bySupport2 & 0x10,  ±íÊ¾Ö§³Ö²¼·À±¨¾¯»Ø´«ÐÅÏ¢£¨½öÖ§³Ö×¥ÅÄ»ú±¨¾¯ ÐÂÀÏ±¨¾¯½á¹¹£©
                     bySupport2 & 0x20,  ±íÊ¾ÊÇ·ñÖ§³Öµ¥¶À»ñÈ¡Éè±¸×´Ì¬×ÓÏî
            bySupport2 & 0x40,  ±íÊ¾ÊÇ·ñÊÇÂëÁ÷¼ÓÃÜÉè±¸*/
            public ushort wDevType;              //Éè±¸ÐÍºÅ
            public byte bySupport3; //ÄÜÁ¦¼¯À©Õ¹£¬Î»Óë½á¹ûÎª0±íÊ¾²»Ö§³Ö£¬1±íÊ¾Ö§³Ö
                                    //bySupport3 & 0x1, ±íÊ¾ÊÇ·ñ¶àÂëÁ÷
                                    // bySupport3 & 0x4 ±íÊ¾Ö§³Ö°´×éÅäÖÃ£¬ ¾ßÌå°üº¬ Í¨µÀÍ¼Ïñ²ÎÊý¡¢±¨¾¯ÊäÈë²ÎÊý¡¢IP±¨¾¯ÊäÈë¡¢Êä³ö½ÓÈë²ÎÊý¡¢
                                    // ÓÃ»§²ÎÊý¡¢Éè±¸¹¤×÷×´Ì¬¡¢JPEG×¥Í¼¡¢¶¨Ê±ºÍÊ±¼ä×¥Í¼¡¢Ó²ÅÌÅÌ×é¹ÜÀí 
                                    //bySupport3 & 0x8Îª1 ±íÊ¾Ö§³ÖÊ¹ÓÃTCPÔ¤ÀÀ¡¢UDPÔ¤ÀÀ¡¢¶à²¥Ô¤ÀÀÖÐµÄ"ÑÓÊ±Ô¤ÀÀ"×Ö¶ÎÀ´ÇëÇóÑÓÊ±Ô¤ÀÀ£¨ºóÐø¶¼½«Ê¹ÓÃÕâÖÖ·½Ê½ÇëÇóÑÓÊ±Ô¤ÀÀ£©¡£¶øµ±bySupport3 & 0x8Îª0Ê±£¬½«Ê¹ÓÃ "Ë½ÓÐÑÓÊ±Ô¤ÀÀ"Ð­Òé¡£
                                    //bySupport3 & 0x10 ±íÊ¾Ö§³Ö"»ñÈ¡±¨¾¯Ö÷»úÖ÷Òª×´Ì¬£¨V40£©"¡£
                                    //bySupport3 & 0x20 ±íÊ¾ÊÇ·ñÖ§³ÖÍ¨¹ýDDNSÓòÃû½âÎöÈ¡Á÷
            public byte byMultiStreamProto;//ÊÇ·ñÖ§³Ö¶àÂëÁ÷,°´Î»±íÊ¾,0-²»Ö§³Ö,1-Ö§³Ö,bit1-ÂëÁ÷3,bit2-ÂëÁ÷4,bit7-Ö÷ÂëÁ÷£¬bit-8×ÓÂëÁ÷
            public byte byStartDChan;  //ÆðÊ¼Êý×ÖÍ¨µÀºÅ,0±íÊ¾ÎÞÐ§
            public byte byStartDTalkChan;   //ÆðÊ¼Êý×Ö¶Ô½²Í¨µÀºÅ£¬Çø±ðÓÚÄ£Äâ¶Ô½²Í¨µÀºÅ£¬0±íÊ¾ÎÞÐ§
            public byte byHighDChanNum;  //Êý×ÖÍ¨µÀ¸öÊý£¬¸ßÎ»
            public byte bySupport4;        //ÄÜÁ¦¼¯À©Õ¹£¬Î»Óë½á¹ûÎª0±íÊ¾²»Ö§³Ö£¬1±íÊ¾Ö§³Ö
                                           //bySupport4 & 0x4±íÊ¾ÊÇ·ñÖ§³ÖÆ´¿ØÍ³Ò»½Ó¿Ú
                                           // bySupport4 & 0x80 Ö§³ÖÉè±¸ÉÏ´«ÖÐÐÄ±¨¾¯Ê¹ÄÜ¡£±íÊ¾ÅÐ¶Ïµ÷ÓÃ½Ó¿ÚÊÇ NET_DVR_PDC_RULE_CFG_V42»¹ÊÇ NET_DVR_PDC_RULE_CFG_V41
            public byte byLanguageType;// Ö§³ÖÓïÖÖÄÜÁ¦,°´Î»±íÊ¾,Ã¿Ò»Î»0-²»Ö§³Ö,1-Ö§³Ö  
                                       //  byLanguageType µÈÓÚ0 ±íÊ¾ ÀÏÉè±¸
                                       //  byLanguageType & 0x1±íÊ¾Ö§³ÖÖÐÎÄ
                                       //  byLanguageType & 0x2±íÊ¾Ö§³ÖÓ¢ÎÄ
            public byte byVoiceInChanNum;   //ÒôÆµÊäÈëÍ¨µÀÊý 
            public byte byStartVoiceInChanNo; //ÒôÆµÊäÈëÆðÊ¼Í¨µÀºÅ 0±íÊ¾ÎÞÐ§
            public byte bySupport5;  //°´Î»±íÊ¾,0-²»Ö§³Ö,1-Ö§³Ö,bit0-Ö§³Ö¶àÂëÁ÷
                                     //bySupport5 &0x01±íÊ¾Ö§³ÖwEventTypeEx ,¼æÈÝdwEventType µÄÊÂ¼þÀàÐÍ£¨Ö§³ÖÐÐÎªÊÂ¼þÀ©Õ¹£©--ÏÈÕ¼×¡£¬·ÀÖ¹³åÍ»
                                     //bySupport5 &0x04±íÊ¾ÊÇ·ñÖ§³ÖÊ¹ÓÃÀ©Õ¹µÄ³¡¾°Ä£Ê½½Ó¿Ú
            /*
               bySupport5 &0x08 Éè±¸·µ»Ø¸ÃÖµ±íÊ¾ÊÇ·ñÖ§³Ö¼Æ»®Â¼ÏñÀàÐÍV40½Ó¿ÚÐ­Òé(DVR_SET_RECORDCFG_V40/ DVR_GET_RECORDCFG_V40)(ÔÚ¸ÃÐ­ÒéÖÐÉè±¸Ö§³ÖÀàÐÍÀàÐÍ13µÄÅäÖÃ)
               Ö®Ç°µÄ²¿·Ö·¢²¼µÄÉè±¸£¬Ö§³ÖÂ¼ÏñÀàÐÍ13£¬ÔòÅäÖÃÂ¼ÏñÀàÐÍ13¡£Èç¹û²»Ö§³Ö£¬Í³Ò»×ª»»³ÉÂ¼ÏñÀàÐÍ3¼æÈÝ´¦Àí¡£SDKÍ¨¹ýÃüÁîÌ½²â´¦Àí)
            */
            public byte bySupport6;   //ÄÜÁ¦£¬°´Î»±íÊ¾£¬0-²»Ö§³Ö,1-Ö§³Ö
                                      //bySupport6 0x1  ±íÊ¾Éè±¸ÊÇ·ñÖ§³ÖÑ¹Ëõ 
                                      //bySupport6 0x2 ±íÊ¾ÊÇ·ñÖ§³ÖÁ÷ID·½Ê½ÅäÖÃÁ÷À´Ô´À©Õ¹ÃüÁî£¬DVR_SET_STREAM_SRC_INFO_V40
                                      //bySupport6 0x4 ±íÊ¾ÊÇ·ñÖ§³ÖÊÂ¼þËÑË÷V40½Ó¿Ú
                                      //bySupport6 0x8 ±íÊ¾ÊÇ·ñÖ§³ÖÀ©Õ¹ÖÇÄÜÕì²âÅäÖÃÃüÁî
                                      //bySupport6 0x40±íÊ¾Í¼Æ¬²éÑ¯½á¹ûV40À©Õ¹
            public byte byMirrorChanNum;    //¾µÏñÍ¨µÀ¸öÊý£¬<Â¼²¥Ö÷»úÖÐÓÃÓÚ±íÊ¾µ¼²¥Í¨µÀ>
            public ushort wStartMirrorChanNo;  //ÆðÊ¼¾µÏñÍ¨µÀºÅ
            public byte bySupport7;   //ÄÜÁ¦,°´Î»±íÊ¾,0-²»Ö§³Ö,1-Ö§³Ö
                                      // bySupport7 & 0x1  ±íÊ¾Éè±¸ÊÇ·ñÖ§³Ö INTER_VCA_RULECFG_V42 À©Õ¹
                                      // bySupport7 & 0x2  ±íÊ¾Éè±¸ÊÇ·ñÖ§³Ö IPC HVT Ä£Ê½À©Õ¹
                                      // bySupport7 & 0x04  ±íÊ¾Éè±¸ÊÇ·ñÖ§³Ö ·µ»ØËø¶¨Ê±¼ä
                                      // bySupport7 & 0x08  ±íÊ¾ÉèÖÃÔÆÌ¨PTZÎ»ÖÃÊ±£¬ÊÇ·ñÖ§³Ö´øÍ¨µÀºÅ
                                      // bySupport7 & 0x10  ±íÊ¾Éè±¸ÊÇ·ñÖ§³ÖË«ÏµÍ³Éý¼¶±¸·Ý
                                      // bySupport7 & 0x20  ±íÊ¾Éè±¸ÊÇ·ñÖ§³Ö OSD×Ö·ûµþ¼Ó V50
                                      // bySupport7 & 0x40  ±íÊ¾Éè±¸ÊÇ·ñÖ§³Ö Ö÷´Ó¸ú×Ù£¨´ÓÉãÏñ»ú£©
                                      // bySupport7 & 0x80  ±íÊ¾Éè±¸ÊÇ·ñÖ§³Ö ±¨ÎÄ¼ÓÃÜ
            public byte byRes2;
        }

        //sdkÍøÂç»·¾³Ã¶¾Ù±äÁ¿£¬ÓÃÓÚÔ¶³ÌÉý¼¶
        public enum SDK_NETWORK_ENVIRONMENT
        {
            LOCAL_AREA_NETWORK = 0,
            WIDE_AREA_NETWORK,
        }

        //ÏÔÊ¾Ä£Ê½
        public enum DISPLAY_MODE
        {
            NORMALMODE = 0,
            OVERLAYMODE
        }

        //·¢ËÍÄ£Ê½
        public enum SEND_MODE
        {
            PTOPTCPMODE = 0,
            PTOPUDPMODE,
            MULTIMODE,
            RTPMODE,
            RESERVEDMODE
        }

        //×¥Í¼Ä£Ê½
        public enum CAPTURE_MODE
        {
            BMP_MODE = 0,  //BMPÄ£Ê½
            JPEG_MODE = 1  //JPEGÄ£Ê½ 
        }

        //ÊµÊ±ÉùÒôÄ£Ê½
        public enum REALSOUND_MODE
        {
            MONOPOLIZE_MODE = 1,//¶ÀÕ¼Ä£Ê½
            SHARE_MODE = 2  //¹²ÏíÄ£Ê½
        }



        //SDK×´Ì¬ÐÅÏ¢(9000ÐÂÔö)
        [StructLayoutAttribute(LayoutKind.Sequential)]
        public struct NET_DVR_SDKSTATE
        {
            public uint dwTotalLoginNum;//µ±Ç°loginÓÃ»§Êý
            public uint dwTotalRealPlayNum;//µ±Ç°realplayÂ·Êý
            public uint dwTotalPlayBackNum;//µ±Ç°»Ø·Å»òÏÂÔØÂ·Êý
            public uint dwTotalAlarmChanNum;//µ±Ç°½¨Á¢±¨¾¯Í¨µÀÂ·Êý
            public uint dwTotalFormatNum;//µ±Ç°Ó²ÅÌ¸ñÊ½»¯Â·Êý
            public uint dwTotalFileSearchNum;//µ±Ç°ÈÕÖ¾»òÎÄ¼þËÑË÷Â·Êý
            public uint dwTotalLogSearchNum;//µ±Ç°ÈÕÖ¾»òÎÄ¼þËÑË÷Â·Êý
            public uint dwTotalSerialNum;//µ±Ç°Í¸Ã÷Í¨µÀÂ·Êý
            public uint dwTotalUpgradeNum;//µ±Ç°Éý¼¶Â·Êý
            public uint dwTotalVoiceComNum;//µ±Ç°ÓïÒô×ª·¢Â·Êý
            public uint dwTotalBroadCastNum;//µ±Ç°ÓïÒô¹ã²¥Â·Êý
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 10, ArraySubType = UnmanagedType.U4)]
            public uint[] dwRes;
        }

        //SDK¹¦ÄÜÖ§³ÖÐÅÏ¢(9000ÐÂÔö)
        [StructLayoutAttribute(LayoutKind.Sequential)]
        public struct NET_DVR_SDKABL
        {
            public uint dwMaxLoginNum;//×î´óloginÓÃ»§Êý MAX_LOGIN_USERS
            public uint dwMaxRealPlayNum;//×î´órealplayÂ·Êý WATCH_NUM
            public uint dwMaxPlayBackNum;//×î´ó»Ø·Å»òÏÂÔØÂ·Êý WATCH_NUM
            public uint dwMaxAlarmChanNum;//×î´ó½¨Á¢±¨¾¯Í¨µÀÂ·Êý ALARM_NUM
            public uint dwMaxFormatNum;//×î´óÓ²ÅÌ¸ñÊ½»¯Â·Êý SERVER_NUM
            public uint dwMaxFileSearchNum;//×î´óÎÄ¼þËÑË÷Â·Êý SERVER_NUM
            public uint dwMaxLogSearchNum;//×î´óÈÕÖ¾ËÑË÷Â·Êý SERVER_NUM
            public uint dwMaxSerialNum;//×î´óÍ¸Ã÷Í¨µÀÂ·Êý SERVER_NUM
            public uint dwMaxUpgradeNum;//×î´óÉý¼¶Â·Êý SERVER_NUM
            public uint dwMaxVoiceComNum;//×î´óÓïÒô×ª·¢Â·Êý SERVER_NUM
            public uint dwMaxBroadCastNum;//×î´óÓïÒô¹ã²¥Â·Êý MAX_CASTNUM
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 10, ArraySubType = UnmanagedType.U4)]
            public uint[] dwRes;
        }

        //±¨¾¯Éè±¸ÐÅÏ¢
        [StructLayoutAttribute(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
        public struct NET_DVR_ALARMER
        {
            public byte byUserIDValid;/* useridÊÇ·ñÓÐÐ§ 0-ÎÞÐ§£¬1-ÓÐÐ§ */
            public byte bySerialValid;/* ÐòÁÐºÅÊÇ·ñÓÐÐ§ 0-ÎÞÐ§£¬1-ÓÐÐ§ */
            public byte byVersionValid;/* °æ±¾ºÅÊÇ·ñÓÐÐ§ 0-ÎÞÐ§£¬1-ÓÐÐ§ */
            public byte byDeviceNameValid;/* Éè±¸Ãû×ÖÊÇ·ñÓÐÐ§ 0-ÎÞÐ§£¬1-ÓÐÐ§ */
            public byte byMacAddrValid; /* MACµØÖ·ÊÇ·ñÓÐÐ§ 0-ÎÞÐ§£¬1-ÓÐÐ§ */
            public byte byLinkPortValid;/* login¶Ë¿ÚÊÇ·ñÓÐÐ§ 0-ÎÞÐ§£¬1-ÓÐÐ§ */
            public byte byDeviceIPValid;/* Éè±¸IPÊÇ·ñÓÐÐ§ 0-ÎÞÐ§£¬1-ÓÐÐ§ */
            public byte bySocketIPValid;/* socket ipÊÇ·ñÓÐÐ§ 0-ÎÞÐ§£¬1-ÓÐÐ§ */
            public int lUserID; /* NET_DVR_Login()·µ»ØÖµ, ²¼·ÀÊ±ÓÐÐ§ */
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = SERIALNO_LEN, ArraySubType = UnmanagedType.I1)]
            public byte[] sSerialNumber;/* ÐòÁÐºÅ */
            public uint dwDeviceVersion;/* °æ±¾ÐÅÏ¢ ¸ß16Î»±íÊ¾Ö÷°æ±¾£¬µÍ16Î»±íÊ¾´Î°æ±¾*/
            [MarshalAsAttribute(UnmanagedType.ByValTStr, SizeConst = NAME_LEN)]
            public string sDeviceName;/* Éè±¸Ãû×Ö */
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = MACADDR_LEN, ArraySubType = UnmanagedType.I1)]
            public byte[] byMacAddr;/* MACµØÖ· */
            public ushort wLinkPort; /* link port */
            [MarshalAsAttribute(UnmanagedType.ByValTStr, SizeConst = 128)]
            public string sDeviceIP;/* IPµØÖ· */
            [MarshalAsAttribute(UnmanagedType.ByValTStr, SizeConst = 128)]
            public string sSocketIP;/* ±¨¾¯Ö÷¶¯ÉÏ´«Ê±µÄsocket IPµØÖ· */
            public byte byIpProtocol; /* IpÐ­Òé 0-IPV4, 1-IPV6 */
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 11, ArraySubType = UnmanagedType.I1)]
            public byte[] byRes2;
        }

        //Ó²½âÂëÏÔÊ¾ÇøÓò²ÎÊý(×Ó½á¹¹)
        [StructLayoutAttribute(LayoutKind.Sequential)]
        public struct NET_DVR_DISPLAY_PARA
        {
            public int bToScreen;
            public int bToVideoOut;
            public int nLeft;
            public int nTop;
            public int nWidth;
            public int nHeight;
            public int nReserved;
        }

        //Ó²½âÂëÔ¤ÀÀ²ÎÊý
        [StructLayoutAttribute(LayoutKind.Sequential)]
        public struct NET_DVR_CARDINFO
        {
            public int lChannel;//Í¨µÀºÅ
            public int lLinkMode;//×î¸ßÎ»(31)Îª0±íÊ¾Ö÷ÂëÁ÷£¬Îª1±íÊ¾×Ó£¬0£­30Î»±íÊ¾ÂëÁ÷Á¬½Ó·½Ê½:0£ºTCP·½Ê½,1£ºUDP·½Ê½,2£º¶à²¥·½Ê½,3 - RTP·½Ê½£¬4-µç»°Ïß£¬5£­128k¿í´ø£¬6£­256k¿í´ø£¬7£­384k¿í´ø£¬8£­512k¿í´ø£»
            [MarshalAsAttribute(UnmanagedType.LPStr)]
            public string sMultiCastIP;
            public NET_DVR_DISPLAY_PARA struDisplayPara;
        }

        //Â¼ÏóÎÄ¼þ²ÎÊý
        [StructLayoutAttribute(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
        public struct NET_DVR_FIND_DATA
        {
            [MarshalAsAttribute(UnmanagedType.ByValTStr, SizeConst = 100)]
            public string sFileName;//ÎÄ¼þÃû
            public NET_DVR_TIME struStartTime;//ÎÄ¼þµÄ¿ªÊ¼Ê±¼ä
            public NET_DVR_TIME struStopTime;//ÎÄ¼þµÄ½áÊøÊ±¼ä
            public uint dwFileSize;//ÎÄ¼þµÄ´óÐ¡
        }

        //Â¼ÏóÎÄ¼þ²ÎÊý(9000)
        [StructLayoutAttribute(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
        public struct NET_DVR_FINDDATA_V30
        {
            [MarshalAsAttribute(UnmanagedType.ByValTStr, SizeConst = 100)]
            public string sFileName;//ÎÄ¼þÃû
            public NET_DVR_TIME struStartTime;//ÎÄ¼þµÄ¿ªÊ¼Ê±¼ä
            public NET_DVR_TIME struStopTime;//ÎÄ¼þµÄ½áÊøÊ±¼ä
            public uint dwFileSize;//ÎÄ¼þµÄ´óÐ¡
            [MarshalAsAttribute(UnmanagedType.ByValTStr, SizeConst = 32)]
            public string sCardNum;
            public byte byLocked;//9000Éè±¸Ö§³Ö,1±íÊ¾´ËÎÄ¼þÒÑ¾­±»Ëø¶¨,0±íÊ¾Õý³£µÄÎÄ¼þ
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 3, ArraySubType = UnmanagedType.I1)]
            public byte[] byRes;
        }

        //Â¼ÏóÎÄ¼þ²ÎÊý(´ø¿¨ºÅ)
        [StructLayoutAttribute(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
        public struct NET_DVR_FINDDATA_CARD
        {
            [MarshalAsAttribute(UnmanagedType.ByValTStr, SizeConst = 100)]
            public string sFileName;//ÎÄ¼þÃû
            public NET_DVR_TIME struStartTime;//ÎÄ¼þµÄ¿ªÊ¼Ê±¼ä
            public NET_DVR_TIME struStopTime;//ÎÄ¼þµÄ½áÊøÊ±¼ä
            public uint dwFileSize;//ÎÄ¼þµÄ´óÐ¡
            [MarshalAsAttribute(UnmanagedType.ByValTStr, SizeConst = 32)]
            public string sCardNum;
        }

        //Â¼ÏóÎÄ¼þ²éÕÒÌõ¼þ½á¹¹
        [StructLayoutAttribute(LayoutKind.Sequential)]
        public struct NET_DVR_FILECOND
        {
            public int lChannel;//Í¨µÀºÅ
            public uint dwFileType;//Â¼ÏóÎÄ¼þÀàÐÍ0xff£­È«²¿£¬0£­¶¨Ê±Â¼Ïñ,1-ÒÆ¶¯Õì²â £¬2£­±¨¾¯´¥·¢£¬
                                   //3-±¨¾¯|ÒÆ¶¯Õì²â 4-±¨¾¯&ÒÆ¶¯Õì²â 5-ÃüÁî´¥·¢ 6-ÊÖ¶¯Â¼Ïñ
            public uint dwIsLocked;//ÊÇ·ñËø¶¨ 0-Õý³£ÎÄ¼þ,1-Ëø¶¨ÎÄ¼þ, 0xff±íÊ¾ËùÓÐÎÄ¼þ
            public uint dwUseCardNo;//ÊÇ·ñÊ¹ÓÃ¿¨ºÅ
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 32, ArraySubType = UnmanagedType.I1)]
            public byte[] sCardNumber;//¿¨ºÅ
            public NET_DVR_TIME struStartTime;//¿ªÊ¼Ê±¼ä
            public NET_DVR_TIME struStopTime;//½áÊøÊ±¼ä
        }

        //ÔÆÌ¨ÇøÓòÑ¡Ôñ·Å´óËõÐ¡(HIK ¿ìÇò×¨ÓÃ)
        [StructLayoutAttribute(LayoutKind.Sequential)]
        public struct NET_DVR_POINT_FRAME
        {
            public int xTop;//·½¿òÆðÊ¼µãµÄx×ø±ê
            public int yTop;//·½¿ò½áÊøµãµÄy×ø±ê
            public int xBottom;//·½¿ò½áÊøµãµÄx×ø±ê
            public int yBottom;//·½¿ò½áÊøµãµÄy×ø±ê
            public int bCounter;//±£Áô
        }

        //ÓïÒô¶Ô½²²ÎÊý
        [StructLayoutAttribute(LayoutKind.Sequential)]
        public struct tagNET_DVR_COMPRESSION_AUDIO
        {
            public byte byAudioEncType;//ÒôÆµ±àÂëÀàÐÍ 0-G722; 1-G711
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 7, ArraySubType = UnmanagedType.I1)]
            public byte[] byres;//ÕâÀï±£ÁôÒôÆµµÄÑ¹Ëõ²ÎÊý 
        }




        ////////////////////////////////////////////////////////////////////////////////////////
        ///×¥ÅÄ»ú
        ///
        public const int MAX_OVERLAP_ITEM_NUM = 50;       //×î´ó×Ö·ûµþ¼ÓÖÖÊý
        public const int NET_ITS_GET_OVERLAP_CFG = 5072;//»ñÈ¡×Ö·ûµþ¼Ó²ÎÊýÅäÖÃ£¨Ïà»ú»òITSÖÕ¶Ë£©
        public const int NET_ITS_SET_OVERLAP_CFG = 5073;//ÉèÖÃ×Ö·ûµþ¼Ó²ÎÊýÅäÖÃ£¨Ïà»ú»òITSÖÕ¶Ë£©

        [StructLayoutAttribute(LayoutKind.Sequential)]
        public struct NET_DVR_PLATE_INFO
        {
            public byte byPlateType;
            public byte byColor;
            public byte byBright;
            public byte byLicenseLen;
            public byte byEntireBelieve;
            public byte byRegion;     // ÇøÓòË÷ÒýÖµ 0-±£Áô£¬1-Å·ÖÞ(EU)£¬2-¶íÓïÇøÓò(ER)£¬3-Å·ÖÞ&¶íÂÞË¹(EU&CIS) ,0xff-ËùÓÐ
            public byte byCountry;     // ¹ú¼ÒË÷ÒýÖµ£¬²ÎÕÕÃ¶¾ÙCOUNTRY_INDEX£¨²»Ö§³Ö"COUNTRY_ALL = 0xff, //ALL  È«²¿"£©
            public byte byArea;                         //ÇøÓò£¨Ê¡·Ý£©£¬¸÷¹ú¼ÒÄÚ²¿ÇøÓòÃ¶¾Ù£¬°¢ÁªÇõ²ÎÕÕ EMI_AREA
            public byte byPlateSize;                    //³µÅÆ³ß´ç£¬0~Î´Öª£¬1~long, 2~short(ÖÐ¶«³µÅÆÊ¹ÓÃ)
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 15, ArraySubType = UnmanagedType.I1)]
            public byte[] byRes;
            [MarshalAsAttribute(UnmanagedType.ByValTStr, SizeConst = MAX_CATEGORY_LEN)]
            public string sPlateCategory;
            public uint dwXmlLen;                        //XML±¨¾¯ÐÅÏ¢³¤¶È
            [MarshalAsAttribute(UnmanagedType.LPStr)]
            public string pXmlBuf;                      // XML±¨¾¯ÐÅÏ¢Ö¸Õë,±¨¾¯ÀàÐÍÎª COMM_ITS_PLATE_RESULÊ±ÓÐÐ§£¬ÆäXML¶ÔÓ¦µ½EventNotificationAlert XML Block
            public NET_VCA_RECT struPlateRect;
            [MarshalAsAttribute(UnmanagedType.ByValTStr, SizeConst = MAX_LICENSE_LEN)]
            public string sLicense;
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = MAX_LICENSE_LEN, ArraySubType = UnmanagedType.I1)]
            public byte[] byBelieve;

            public void Init()
            {
                byRes = new byte[15];
                byBelieve = new byte[MAX_LICENSE_LEN];
            }
        }

        [StructLayoutAttribute(LayoutKind.Sequential)]
        public struct NET_DVR_VEHICLE_INFO
        {
            public uint dwIndex;
            public byte byVehicleType;
            public byte byColorDepth;
            public byte byColor;
            public byte byRadarState;
            public ushort wSpeed;
            public ushort wLength;
            public byte byIllegalType;
            public byte byVehicleLogoRecog; //²Î¿¼Ã¶¾ÙÀàÐÍ VLR_VEHICLE_CLASS
            public byte byVehicleSubLogoRecog; //³µÁ¾Æ·ÅÆ×ÓÀàÐÍÊ¶±ð£»²Î¿¼VSB_VOLKSWAGEN_CLASSµÈ×ÓÀàÐÍÃ¶¾Ù¡£
            public byte byVehicleModel; //³µÁ¾×ÓÆ·ÅÆÄê¿î£¬0-Î´Öª£¬²Î¿¼"³µÁ¾×ÓÆ·ÅÆÄê¿î.xlsx"
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 16, ArraySubType = UnmanagedType.I1)]
            public byte[] byCustomInfo;  //×Ô¶¨ÒåÐÅÏ¢
            public uint wVehicleLogoRecog;  //³µÁ¾Ö÷Æ·ÅÆ£¬²Î¿¼"³µÁ¾Ö÷Æ·ÅÆ.xlsx" (¸Ã×Ö¶Î¼æÈÝbyVehicleLogoRecog);
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 12, ArraySubType = UnmanagedType.I1)]
            public byte[] byRes;

            public void Init()
            {
                byRes = new byte[12];
            }
        }

        [StructLayoutAttribute(LayoutKind.Sequential)]
        public struct NET_DVR_PLATE_RESULT
        {
            public uint dwSize;
            public byte byResultType;
            public byte byChanIndex;
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 2, ArraySubType = UnmanagedType.I1)]
            public byte[] byRes1;
            public uint dwRelativeTime;
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 32, ArraySubType = UnmanagedType.I1)]
            public byte[] byAbsTime;
            public uint dwPicLen;
            public uint dwPicPlateLen;
            public uint dwVideoLen;
            public byte byTrafficLight;
            public byte byPicNum;
            public byte byDriveChan;
            public byte byRes2;
            public uint dwBinPicLen;
            public uint dwCarPicLen;
            public uint dwFarCarPicLen;
            public IntPtr pBuffer3;
            public IntPtr pBuffer4;
            public IntPtr pBuffer5;
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 8, ArraySubType = UnmanagedType.I1)]
            public byte[] byRes3;
            public NET_DVR_PLATE_INFO struPlateInfo;
            public NET_DVR_VEHICLE_INFO struVehicleInfo;
            public IntPtr pBuffer1;
            public IntPtr pBuffer2;

            public void Init()
            {
                byRes1 = new byte[2];
                byAbsTime = new byte[32];
                byRes3 = new byte[8];
            }
        }

        //ÈËÁ³×¥ÅÄ½á¹û
        [StructLayoutAttribute(LayoutKind.Sequential)]
        public struct NET_VCA_TARGET_INFO
        {
            public uint dwID;//Ä¿±êID ,ÈËÔ±ÃÜ¶È¹ý¸ß±¨¾¯Ê±Îª0
            public NET_VCA_RECT struRect; //Ä¿±ê±ß½ç¿ò 
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 4, ArraySubType = UnmanagedType.I1)]
            public byte[] byRes;//±£Áô
        }

        [StructLayoutAttribute(LayoutKind.Sequential)]
        public struct NET_VCA_DEV_INFO
        {
            public NET_DVR_IPADDR struDevIP;//Ç°¶ËÉè±¸µØÖ·£¬
            public ushort wPort;//Ç°¶ËÉè±¸¶Ë¿ÚºÅ£¬ 
            public byte byChannel;//Ç°¶ËÉè±¸Í¨µÀ£¬
            public byte byIvmsChannel;// Ivms Í¨µÀ 
        }

        [StructLayoutAttribute(LayoutKind.Sequential)]
        public struct NET_VCA_HUMAN_FEATURE
        {
            public byte byAgeGroup;           //ÄêÁä¶Î,²Î¼û HUMAN_AGE_GROUP_ENUM
            public byte bySex;                //ÐÔ±ð, 1 ¨C ÄÐ , 2 ¨C Å®
            public byte byEyeGlass;           //ÊÇ·ñ´÷ÑÛ¾µ 1 ¨C ²»´÷, 2 ¨C ´÷
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 13, ArraySubType = UnmanagedType.I1)]
            public byte[] byRes;           //±£Áô
        }

        [StructLayoutAttribute(LayoutKind.Sequential)]
        public struct NET_VCA_FACESNAP_RESULT
        {
            public uint dwSize;             // ½á¹¹´óÐ¡
            public uint dwRelativeTime;     // Ïà¶ÔÊ±±ê
            public uint dwAbsTime;            // ¾ø¶ÔÊ±±ê
            public uint dwFacePicID;       //ÈËÁ³Í¼ID
            public uint dwFaceScore;        //ÈËÁ³ÆÀ·Ö,0-100
            public NET_VCA_TARGET_INFO struTargetInfo;    //±¨¾¯Ä¿±êÐÅÏ¢
            public NET_VCA_RECT struRect;      //ÈËÁ³×ÓÍ¼ÇøÓò
            public NET_VCA_DEV_INFO struDevInfo;        //Ç°¶ËÉè±¸ÐÅÏ¢
            public uint dwFacePicLen;        //ÈËÁ³×ÓÍ¼µÄ³¤¶È£¬Îª0±íÊ¾Ã»ÓÐÍ¼Æ¬£¬´óÓÚ0±íÊ¾ÓÐÍ¼Æ¬
            public uint dwBackgroundPicLen; //±³¾°Í¼µÄ³¤¶È£¬Îª0±íÊ¾Ã»ÓÐÍ¼Æ¬£¬´óÓÚ0±íÊ¾ÓÐÍ¼Æ¬
            public byte bySmart;             //0- IDSÉè±¸·µ»Ø0,1-SmartÉè±¸·µ»Ø
            public byte byAlarmEndMark;//±¨¾¯½áÊø±ê¼Ç0-±£Áô£¬1-½áÊø±ê¼Ç£¨¸Ã×Ö¶Î½áºÏÈËÁ³ID×Ö¶ÎÊ¹ÓÃ£¬±íÊ¾¸ÃID¶ÔÓ¦µÄÏÂ±¨¾¯½áÊø£¬Ö÷ÒªÌá¹©¸øNVRÊ¹ÓÃ£¬ÓÃÓÚÅÐ¶Ï±¨¾¯½áÊø£¬ÌáÈ¡Ê¶±ðÍ¼Æ¬Êý¾ÝÖÐ£¬ÇåÎú¶È×î¸ßµÄÍ¼Æ¬£©
            public byte byRepeatTimes;   //ÖØ¸´±¨¾¯´ÎÊý£¬0-ÎÞÒâÒå
            public byte byRes;
            public NET_VCA_HUMAN_FEATURE struFeature; //ÈËÌåÊôÐÔ
            public float fStayDuration;     //Í£Áô»­ÃæÖÐÊ±¼ä(µ¥Î»:Ãë)
            [MarshalAsAttribute(UnmanagedType.ByValTStr, SizeConst = 16)]
            public string sStorageIP;       //´æ´¢·þÎñIPµØÖ·
            public ushort wStoragePort;         //´æ´¢·þÎñ¶Ë¿ÚºÅ
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 18, ArraySubType = UnmanagedType.I1)]
            public byte[] byRes1;        // ±£Áô×Ö½Ú
            public IntPtr pBuffer1;          //ÈËÁ³×ÓÍ¼µÄÍ¼Æ¬Êý¾Ý
            public IntPtr pBuffer2;          //±³¾°Í¼µÄÍ¼Æ¬Êý¾Ý
        }

        [StructLayoutAttribute(LayoutKind.Sequential)]
        public struct NET_DVR_TIME_V30
        {
            public ushort wYear;
            public byte byMonth;
            public byte byDay;
            public byte byHour;
            public byte byMinute;
            public byte bySecond;
            public byte byRes;
            public ushort wMilliSec;
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 2, ArraySubType = UnmanagedType.I1)]
            public byte[] byRes1;
        }

        [StructLayoutAttribute(LayoutKind.Sequential)]
        public struct NET_ITS_PICTURE_INFO
        {
            public uint dwDataLen;              //Ã½ÌåÊý¾Ý³¤¶È
            public byte byType;                           // 0:³µÅÆÍ¼;1:³¡¾°Í¼;2:ºÏ³ÉÍ¼;3:ÂëÁ÷
            public byte byDataType;                       // 0-Êý¾ÝÖ±½ÓÉÏ´«; 1-ÔÆ´æ´¢·þÎñÆ÷URL(3.7Ver)Ô­ÏÈµÄÍ¼Æ¬Êý¾Ý±ä³ÉURLÊý¾Ý£¬Í¼Æ¬³¤¶È±ä³ÉURL³¤¶È
            public byte byCloseUpType;                    //ÌØÐ´Í¼ÀàÐÍ£¬0-±£Áô,1-·Ç»ú¶¯³µ,2-ÐÐÈË
            public byte byPicRecogMode;                   //Í¼Æ¬±³ÏòÊ¶±ð£º0-ÕýÏò³µÅÆÊ¶±ð£¬1-±³ÏòÊ¶±ð(Î²ÅÆÊ¶±ð)
            public uint dwRedLightTime;                   //¾­¹ýµÄºìµÆÊ±¼ä  £¨s£©
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 32, ArraySubType = UnmanagedType.I1)]
            public byte[] byAbsTime;                 //¾ø¶ÔÊ±¼äµã,yyyymmddhhmmssxxx,e.g.20090810235959999  ×îºóÈýÎ»ÎªºÁÃëÊý
            public NET_VCA_RECT struPlateRect;         //³µÅÆÎ»ÖÃ
            public NET_VCA_RECT struPlateRecgRect;   //ÅÆÊ¶ÇøÓò×ø±ê
            public IntPtr pBuffer;     //Êý¾ÝÖ¸Õë
            public uint dwUTCTime;//UTCÊ±¼ä¶¨Òå
            public byte byCompatibleAblity; //¼æÈÝÄÜÁ¦×Ö¶Î 0±íÊ¾ÎÞÐ§£¬1±íÊ¾ÓÐÐ§; bit0-±íÊ¾dwUTCTime×Ö¶ÎÓÐÐ§
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 7, ArraySubType = UnmanagedType.I1)]
            public byte[] byRes2;              //±£Áô
        }

        [StructLayoutAttribute(LayoutKind.Sequential)]
        public struct NET_ITS_PLATE_RESULT
        {
            public uint dwSize;
            public uint dwMatchNo;
            public byte byGroupNum;
            public byte byPicNo;
            public byte bySecondCam;    //ÊÇ·ñµÚ¶þÏà»ú×¥ÅÄ£¨ÈçÔ¶½ü¾°×¥ÅÄµÄÔ¶¾°Ïà»ú£¬»òÇ°ºó×¥ÅÄµÄºóÏà»ú£¬ÌØÊâÏîÄ¿ÖÐ»áÓÃµ½£©
            public byte byFeaturePicNo; //´³ºìµÆµç¾¯£¬È¡µÚ¼¸ÕÅÍ¼×÷ÎªÌØÐ´Í¼,0xff-±íÊ¾²»È¡
            public byte byDriveChan;                //´¥·¢³µµÀºÅ
            public byte byVehicleType;     //0- Î´Öª£¬1-¿Í³µ£¬2-»õ³µ£¬3-½Î³µ£¬4-Ãæ°ü³µ£¬5-Ð¡»õ³µ
            public byte byDetSceneID;//¼ì²â³¡¾°ºÅ[1,4], IPCÄ¬ÈÏÊÇ0
            public byte byVehicleAttribute;//³µÁ¾ÊôÐÔ£¬°´Î»±íÊ¾£¬0- ÎÞ¸½¼ÓÊôÐÔ(ÆÕÍ¨³µ)£¬bit1- »Æ±ê³µ(ÀàËÆÄê¼ìµÄ±êÖ¾)£¬bit2- Î£ÏÕÆ·³µÁ¾£¬Öµ£º0- ·ñ£¬1- ÊÇ                      //±£Áô
            public ushort wIllegalType;       //Î¥ÕÂÀàÐÍ²ÉÓÃ¹ú±ê¶¨Òå
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 8, ArraySubType = UnmanagedType.I1)]
            public byte[] byIllegalSubType;   //Î¥ÕÂ×ÓÀàÐÍ
            public byte byPostPicNo;    //Î¥ÕÂÊ±È¡µÚ¼¸ÕÅÍ¼Æ¬×÷Îª¿¨¿ÚÍ¼,0xff-±íÊ¾²»È¡
            public byte byChanIndex;                //Í¨µÀºÅ£¨±£Áô£©
            public ushort wSpeedLimit;            //ÏÞËÙÉÏÏÞ£¨³¬ËÙÊ±ÓÐÐ§£©
            public byte byChanIndexEx;      //byChanIndexEx*256+byChanIndex±íÊ¾ÕæÊµÍ¨µÀºÅ¡£
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 1, ArraySubType = UnmanagedType.I1)]
            public byte[] byRes2;
            public NET_DVR_PLATE_INFO struPlateInfo;       //³µÅÆÐÅÏ¢½á¹¹
            public NET_DVR_VEHICLE_INFO struVehicleInfo;        //³µÁ¾ÐÅÏ¢
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 48, ArraySubType = UnmanagedType.I1)]
            public byte[] byMonitoringSiteID;          //¼à²âµã±àºÅ
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 48, ArraySubType = UnmanagedType.I1)]
            public byte[] byDeviceID;                                   //Éè±¸±àºÅ
            public byte byDir;                //¼à²â·½Ïò£¬1-ÉÏÐÐ£¬2-ÏÂÐÐ£¬3-Ë«Ïò£¬4-ÓÉ¶«ÏòÎ÷£¬5-ÓÉÄÏÏò±±,6-ÓÉÎ÷Ïò¶«£¬7-ÓÉ±±ÏòÄÏ£¬8-ÆäËü
            public byte byDetectType;    //¼ì²â·½Ê½,1-µØ¸Ð´¥·¢£¬2-ÊÓÆµ´¥·¢£¬3-¶àÖ¡Ê¶±ð£¬4-À×´ï´¥·¢
            public byte byRelaLaneDirectionType;
            public byte byCarDirectionType; //³µÁ¾¾ßÌåÐÐÊ»µÄ·½Ïò£¬0±íÊ¾´ÓÉÏÍùÏÂ£¬1±íÊ¾´ÓÏÂÍùÉÏ£¨¸ù¾ÝÊµ¼Ê³µÁ¾µÄÐÐÊ»·½ÏòÀ´µÄÇø·Ö£©
                                            //µ±wIllegalType²ÎÊýÎª¿ÕÊ±£¬Ê¹ÓÃ¸Ã²ÎÊý¡£ÈôwIllegalType²ÎÊýÎªÓÐÖµÊ±£¬ÒÔwIllegalType²ÎÊýÎª×¼£¬¸Ã²ÎÊýÎÞÐ§¡£
            public uint dwCustomIllegalType; //Î¥ÕÂÀàÐÍ¶¨Òå(ÓÃ»§×Ô¶¨Òå)
            /*Îª0~Êý×Ö¸ñÊ½Ê±£¬ÎªÀÏµÄÎ¥ÕÂÀàÐÍ£¬wIllegalType¡¢dwCustomIllegalType²ÎÊýÉúÐ§£¬¸³Öµ¹ú±êÎ¥·¨´úÂë¡£
              Îª1~×Ö·û¸ñÊ½Ê±£¬pIllegalInfoBuf²ÎÊýÉúÐ§¡£ÀÏµÄÎ¥ÕÂÀàÐÍ£¬wIllegalType¡¢dwCustomIllegalType²ÎÊýÒÀÈ»¸³Öµ¹ú±êÎ¥·¨´úÂë*/
            public IntPtr pIllegalInfoBuf;    //Î¥·¨´úÂë×Ö·ûÐÅÏ¢½á¹¹ÌåÖ¸Õë£»Ö¸ÏòNET_ITS_ILLEGAL_INFO 
            public byte byIllegalFromatType; //Î¥ÕÂÐÅÏ¢¸ñÊ½ÀàÐÍ£» 0~Êý×Ö¸ñÊ½£¬ 1~×Ö·û¸ñÊ½
            public byte byPendant;// 0-±íÊ¾Î´Öª,1-³µ´°ÓÐÐü¹ÒÎï£¬2-³µ´°ÎÞÐü¹ÒÎï
            public byte byDataAnalysis;            //0-Êý¾ÝÎ´·ÖÎö, 1-Êý¾ÝÒÑ·ÖÎö
            public byte byYellowLabelCar;        //0-±íÊ¾Î´Öª, 1-·Ç»Æ±ê³µ,2-»Æ±ê³µ
            public byte byDangerousVehicles;    //0-±íÊ¾Î´Öª, 1-·ÇÎ£ÏÕÆ·³µ,2-Î£ÏÕÆ·³µ
                                                //ÒÔÏÂ×Ö¶Î°üº¬Pilot×Ö·û¾ùÎªÖ÷¼ÝÊ»£¬º¬Copilot×Ö·û¾ùÎª¸±¼ÝÊ»
            public byte byPilotSafebelt;//0-±íÊ¾Î´Öª,1-Ïµ°²È«´ø,2-²»Ïµ°²È«´ø
            public byte byCopilotSafebelt;//0-±íÊ¾Î´Öª,1-Ïµ°²È«´ø,2-²»Ïµ°²È«´ø
            public byte byPilotSunVisor;//0-±íÊ¾Î´Öª,1-²»´ò¿ªÕÚÑô°å,2-´ò¿ªÕÚÑô°å
            public byte byCopilotSunVisor;//0-±íÊ¾Î´Öª, 1-²»´ò¿ªÕÚÑô°å,2-´ò¿ªÕÚÑô°å
            public byte byPilotCall;// 0-±íÊ¾Î´Öª, 1-²»´òµç»°,2-´òµç»°
                                    //0-¿ªÕ¢£¬1-Î´¿ªÕ¢ (×¨ÓÃÓÚÀúÊ·Êý¾ÝÖÐÏà»ú¸ù¾ÝºÚ°×Ãûµ¥Æ¥Åäºó£¬ÊÇ·ñ¿ªÕ¢³É¹¦µÄ±êÖ¾)
            public byte byBarrierGateCtrlType;
            public byte byAlarmDataType;//0-ÊµÊ±Êý¾Ý£¬1-ÀúÊ·Êý¾Ý
            public NET_DVR_TIME_V30 struSnapFirstPicTime;//¶ËµãÊ±¼ä(ms)£¨×¥ÅÄµÚÒ»ÕÅÍ¼Æ¬µÄÊ±¼ä£©
            public uint dwIllegalTime;//Î¥·¨³ÖÐøÊ±¼ä£¨ms£© = ×¥ÅÄ×îºóÒ»ÕÅÍ¼Æ¬µÄÊ±¼ä - ×¥ÅÄµÚÒ»ÕÅÍ¼Æ¬µÄÊ±¼ä
            public uint dwPicNum;            //Í¼Æ¬ÊýÁ¿£¨ÓëpicGroupNum²»Í¬£¬´ú±í±¾ÌõÐÅÏ¢¸½´øµÄÍ¼Æ¬ÊýÁ¿£¬Í¼Æ¬ÐÅÏ¢ÓÉstruVehicleInfoEx¶¨Òå   
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 6, ArraySubType = UnmanagedType.Struct)]
            public NET_ITS_PICTURE_INFO[] struPicInfo;                //Í¼Æ¬ÐÅÏ¢,µ¥ÕÅ»Øµ÷£¬×î¶à6ÕÅÍ¼£¬ÓÉÐòºÅÇø·Ö            
        }

        //×Ö·ûµþ¼ÓÅäÖÃÌõ¼þ²ÎÊý½á¹¹Ìå
        [StructLayoutAttribute(LayoutKind.Sequential)]
        public struct NET_ITS_OVERLAPCFG_COND
        {
            public uint dwSize;
            public uint dwChannel;//Í¨µÀºÅ 
            public uint dwConfigMode;//ÅäÖÃÄ£Ê½£º0- ÖÕ¶Ë£¬1- Ç°¶Ë(Ö±Á¬Ç°¶Ë»òÖÕ¶Ë½ÓÇ°¶Ë)
            public byte byPicModeType;//0-±íÊ¾Ð¡Í¼(¶ÀÁ¢Í¼)£¬1-±íÊ¾´óÍ¼(ºÏ³ÉÍ¼)
            /*
            0±íÊ¾¹ØÁª ×¥ÅÄMPRÄ£Ê½£¨¶àÖ¡´¥·¢×¥ÅÄ IPCÊ¹ÓÃ£©
            1 ±íÊ¾¹ØÁª ×¥ÅÄ HVT Ä£Ê½£¨»ì¿¨IPCÊ¹ÓÃ£©
*/
            public byte byRelateType;
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 14, ArraySubType = UnmanagedType.I1)]
            public byte[] byRes;//±£Áô
        }

        public enum ITS_OVERLAP_ITEM_TYPE
        {
            OVERLAP_ITEM_NULL = 0,          //0-Î´Öª
            OVERLAP_ITEM_SITE,                //1-µØµã
            OVERLAP_ITEM_ROADNUM,             //2-Â·¿Ú±àºÅ
            OVERLAP_ITEM_INSTRUMENTNUM,       //3-Éè±¸±àºÅ
            OVERLAP_ITEM_DIRECTION,           //4-·½Ïò±àºÅ
            OVERLAP_ITEM_DIRECTIONDESC,       //5-·½Ïò
            OVERLAP_ITEM_LANENUM,             //6-³µµÀºÅ
            OVERLAP_ITEM_LANEDES,             //7-³µµÀ
            OVERLAP_ITEM_CAPTIME,             //8-×¥ÅÄÊ±¼ä(²»´øºÁÃë)
            OVERLAP_ITEM_CAPTIME_MILLSECOND,  //9-×¥ÅÄÊ±¼ä(´øºÁÃë)
            OVERLAP_ITEM_PLATENUM,            //10-³µÅÆºÅ
            OVERLAP_ITEM_CARCOLOR,            //11-³µÉíÑÕÉ«
            OVERLAP_ITEM_CARTYPE,             //12-³µÁ¾ÀàÐÍ
            OVERLAP_ITEM_CARBRAND,            //13-³µÁ¾Æ·ÅÆ
            OVERLAP_ITEM_CARSPEED,            //14-³µÁ¾ËÙ¶È
            OVERLAP_ITEM_SPEEDLIMIT,          //15-ÏÞËÙ±êÖ¾
            OVERLAP_ITEM_CARLENGTH,           //16-³µÁ¾³¤¶È1~99m
            OVERLAP_ITEM_ILLEGALNUM,          //17-Î¥·¨´úÂë(Î¥·¨´úÂëµþ¼ÓÓ¦¸ÃÃ»ÓÃµÄ£¬Ó¦¸ÃÖ±½Óµþ¼ÓÎ¥·¨ÐÅÏ¢£¬±ÈÈçÕý³£¡¢µÍËÙ¡¢³¬ËÙ¡¢ÄæÐÐ¡¢´³ºìµÆ¡¢Õ¼µÀ¡¢Ñ¹»ÆÏßµÈ)
            OVERLAP_ITEM_MONITOR_INFO,      //18-¼à²âµãÐÅÏ¢
            OVERLAP_ITEM_ILLEGALDES,          //19-Î¥·¨ÐÐÎª
            OVERLAP_ITEM_OVERSPEED_PERCENT,    //20-³¬ËÙ±È
            OVERLAP_ITEM_RED_STARTTIME,           //21-ºìµÆ¿ªÊ¼Ê±¼ä
            OVERLAP_ITEM_RED_STOPTIME,        //22-ºìµÆ½áÊøÊ±¼ä
            OVERLAP_ITEM_RED_DURATION,        //23-ºìµÆÒÑÁÁÊ±¼ä
            OVERLAP_ITEM_SECUNITY_CODE,        //24-·ÀÎ±Âë
            OVERLAP_ITEM_CAP_CODE,        //25-×¥ÅÄ±àºÅ
            OVERLAP_ITEM_SEATBELT,      //26-°²È«´ø  
            OVERLAP_ITEM_MONITOR_ID,    //27-¼à²âµã±àºÅ
            OVERLAP_ITEM_SUN_VISOR,     //28-ÕÚÑô°å 
            OVERLAP_ITEM_LANE_DIRECTION,  //29-³µµÀÐÐÊ»·½Ïò 
            OVERLAP_ITEM_LICENSE_PLATE_COLOR,  // 30-³µÅÆÑÕÉ«
            OVERLAP_ITEM_SCENE_NUMBER,  //31-³¡¾°±àºÅ
            OVERLAP_ITEM_SCENE_NAME,   //32-³¡¾°Ãû³Æ
            OVERLAP_ITEM_YELLOW_SIGN_CAR,  //33-»Æ±ê³µ
            OVERLAP_ITEM_DANGEROUS_CAR,    //34-Î£ÏÕÆ·³µ
            OVERLAP_ITEM_CAR_SUBBRAND,  //35-³µÁ¾×ÓÆ·ÅÆ
            OVERLAP_ITEM_CAR_DIRECTION,  //36-³µÁ¾·½Ïò
            OVERLAP_ITEM_PENDANT,  //37-³µ´°Ðü¹ÒÎï
            OVERLAP_ITEM_CALL,  //38-´òµç»°
            OVERLAP_ITEM_CAR_VALIDITY  //39-ÖÃÐÅ¶È
        }


        //µ¥Ìõ×Ö·ûµþ¼ÓÐÅÏ¢½á¹¹Ìå
        [StructLayoutAttribute(LayoutKind.Sequential)]
        public struct NET_ITS_OVERLAP_SINGLE_ITEM_PARAM
        {
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 2, ArraySubType = UnmanagedType.I1)]
            public byte[] byRes1;//±£Áô
            public byte byItemType;//ÀàÐÍ
            public byte byChangeLineNum;//µþ¼ÓÏîºóµÄ»»ÐÐÊý£¬È¡Öµ·¶Î§£º[0,10]£¬Ä¬ÈÏ£º0 
            public byte bySpaceNum;//µþ¼ÓÏîºóµÄ¿Õ¸ñÊý£¬È¡Öµ·¶Î§£º[0-255]£¬Ä¬ÈÏ£º0
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 15, ArraySubType = UnmanagedType.I1)]
            public byte[] byRes;//±£Áô
        }

        //×Ö·û´®²ÎÊýÅäÖÃ½á¹¹Ìå
        [StructLayoutAttribute(LayoutKind.Sequential)]
        public struct NET_ITS_OVERLAP_ITEM_PARAM
        {
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = MAX_OVERLAP_ITEM_NUM, ArraySubType = UnmanagedType.Struct)]
            public NET_ITS_OVERLAP_SINGLE_ITEM_PARAM[] struSingleItem;//×Ö·û´®ÄÚÈÝÐÅÏ¢
            public uint dwLinePercent;
            public uint dwItemsStlye;
            public ushort wStartPosTop;
            public ushort wStartPosLeft;
            public ushort wCharStyle;
            public ushort wCharSize;
            public ushort wCharInterval;
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 2, ArraySubType = UnmanagedType.I1)]
            public byte[] byRes1;//±£Áô
            public uint dwForeClorRGB;//Ç°¾°É«µÄRGBÖµ£¬bit0~bit7: B£¬bit8~bit15: G£¬bit16~bit23: R£¬Ä¬ÈÏ£ºx00FFFFFF-°×
            public uint dwBackClorRGB;//±³¾°É«µÄRGBÖµ£¬Ö»¶ÔÍ¼Æ¬Íâµþ¼ÓÓÐÐ§£¬bit0~bit7: B£¬bit8~bit15: G£¬bit16~bit23: R£¬Ä¬ÈÏ£ºx00000000-ºÚ 
            public byte byColorAdapt;//ÑÕÉ«ÊÇ·ñ×ÔÊÊÓ¦£º0-·ñ£¬1-ÊÇ
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 31, ArraySubType = UnmanagedType.I1)]
            public byte[] byRes;//±£Áô
        }

        //×Ö·ûµþ¼ÓÄÚÈÝÐÅÏ¢½á¹¹Ìå
        [StructLayoutAttribute(LayoutKind.Sequential)]
        public struct NET_ITS_OVERLAP_INFO_PARAM
        {
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 128, ArraySubType = UnmanagedType.I1)]
            public byte[] bySite;//
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 32, ArraySubType = UnmanagedType.I1)]
            public byte[] byRoadNum;//
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 32, ArraySubType = UnmanagedType.I1)]
            public byte[] byInstrumentNum;//
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 32, ArraySubType = UnmanagedType.I1)]
            public byte[] byDirection;//
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 32, ArraySubType = UnmanagedType.I1)]
            public byte[] byDirectionDesc;//
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 32, ArraySubType = UnmanagedType.I1)]
            public byte[] byLaneDes;//
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 32, ArraySubType = UnmanagedType.I1)]
            public byte[] byRes1;//ÕâÀï±£ÁôÒôÆµµÄÑ¹Ëõ²ÎÊý
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 44, ArraySubType = UnmanagedType.I1)]
            public byte[] byMonitoringSite1;//
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 32, ArraySubType = UnmanagedType.I1)]
            public byte[] byMonitoringSite2;//ÕâÀï±£ÁôÒôÆµµÄÑ¹Ëõ²ÎÊý
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 64, ArraySubType = UnmanagedType.I1)]
            public byte[] byRes;//ÕâÀï±£ÁôÒôÆµµÄÑ¹Ëõ²ÎÊý 
        }

        //×Ö·ûµþ¼ÓÅäÖÃÌõ¼þ²ÎÊý½á¹¹Ìå
        [StructLayoutAttribute(LayoutKind.Sequential)]
        public struct NET_ITS_OVERLAP_CFG
        {
            public uint dwSize;
            public byte byEnable;//ÊÇ·ñÆôÓÃ£º0- ²»ÆôÓÃ£¬1- ÆôÓÃ
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 3, ArraySubType = UnmanagedType.I1)]
            public byte[] byRes1;//ÕâÀï±£ÁôÒôÆµµÄÑ¹Ëõ²ÎÊý
            public NET_ITS_OVERLAP_ITEM_PARAM struOverLapItem;//×Ö·û´®²ÎÊý
            public NET_ITS_OVERLAP_INFO_PARAM struOverLapInfo;//×Ö·û´®ÄÚÈÝÐÅÏ¢
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 32, ArraySubType = UnmanagedType.I1)]
            public byte[] byRes;//ÕâÀï±£ÁôÒôÆµµÄÑ¹Ëõ²ÎÊý 
        }

        //±¨¾¯²¼·À²ÎÊý½á¹¹Ìå
        [StructLayoutAttribute(LayoutKind.Sequential)]
        public struct NET_DVR_SETUPALARM_PARAM
        {
            public uint dwSize;
            public byte byLevel;//²¼·ÀÓÅÏÈ¼¶£º0- Ò»µÈ¼¶£¨¸ß£©£¬1- ¶þµÈ¼¶£¨ÖÐ£©£¬2- ÈýµÈ¼¶£¨µÍ£¬±£Áô£©
            public byte byAlarmInfoType;//ÉÏ´«±¨¾¯ÐÅÏ¢ÀàÐÍ£¨ÖÇÄÜ½»Í¨ÉãÏñ»úÖ§³Ö£©£º0- ÀÏ±¨¾¯ÐÅÏ¢£¨NET_DVR_PLATE_RESULT£©£¬1- ÐÂ±¨¾¯ÐÅÏ¢(NET_ITS_PLATE_RESULT) 
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 3, ArraySubType = UnmanagedType.I1)]
            public byte[] byRes1;//ÕâÀï±£ÁôÒôÆµµÄÑ¹Ëõ²ÎÊý 
            public byte byFaceAlarmDetection;//1-±íÊ¾ÈËÁ³Õì²â±¨¾¯À©Õ¹(INTER_FACE_DETECTION),0-±íÊ¾Ô­ÏÈÖ§³Ö½á¹¹(INTER_FACESNAP_RESULT)
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 10, ArraySubType = UnmanagedType.I1)]
            public byte[] byRes;//ÕâÀï±£ÁôÒôÆµµÄÑ¹Ëõ²ÎÊý 
        }

        //µÀÕ¢¿ØÖÆ²ÎÊý
        [StructLayoutAttribute(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
        public struct NET_DVR_BARRIERGATE_CFG
        {
            public uint dwSize;
            public uint dwChannel;
            public byte byLaneNo;
            public byte byBarrierGateCtrl;
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 14, ArraySubType = UnmanagedType.I1)]
            public byte[] byRes;
        }

        public const int MAX_RELAY_NUM = 12;
        public const int MAX_IOIN_NUM = 8;
        public const int MAX_VEHICLE_TYPE_NUM = 8;

        public const int NET_DVR_GET_ENTRANCE_PARAMCFG = 3126; //»ñÈ¡³öÈë¿Ú¿ØÖÆ²ÎÊý
        public const int NET_DVR_SET_ENTRANCE_PARAMCFG = 3127; //ÉèÖÃ³öÈë¿Ú¿ØÖÆ²ÎÊý

        //³öÈë¿Ú¿ØÖÆÌõ¼þ
        [StructLayoutAttribute(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
        public struct NET_DVR_BARRIERGATE_COND
        {
            public byte byLaneNo;//³µµÀºÅ£º0- ±íÊ¾ÎÞÐ§Öµ(Éè±¸ÐèÒª×öÓÐÐ§ÖµÅÐ¶Ï)£¬1- ³µµÀ1
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 3, ArraySubType = UnmanagedType.I1)]
            public byte[] byRes;
        }

        //¼ÌµçÆ÷¹ØÁªÅäÖÃ
        [StructLayoutAttribute(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
        public struct NET_DVR_RELAY_PARAM
        {
            public byte byAccessDevInfo;//0-²»½ÓÈëÉè±¸£¬1-¿ªµÀÕ¢¡¢2-¹ØµÀÕ¢¡¢3-Í£µÀÕ¢¡¢4-±¨¾¯ÐÅºÅ¡¢5-³£ÁÁµÆ
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 3, ArraySubType = UnmanagedType.I1)]
            public byte[] byRes;
        }

        //³µÁ¾ÐÅÏ¢¹Ü¿Ø²ÎÊý
        [StructLayoutAttribute(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
        public struct NET_DVR_VEHICLE_CONTROL
        {
            public byte byGateOperateType;//²Ù×÷ÀàÐÍ£º0- ÎÞ²Ù×÷£¬1- ¿ªµÀÕ¢
            public byte byRes1;
            public ushort wAlarmOperateType; //±¨¾¯´¦ÀíÀàÐÍ£º0- ÎÞ²Ù×÷£¬bit0- ¼ÌµçÆ÷Êä³ö±¨¾¯£¬bit1- ²¼·ÀÉÏ´«±¨¾¯£¬bit3- ¸æ¾¯Ö÷»úÉÏ´«£¬Öµ£º0-±íÊ¾¹Ø£¬1-±íÊ¾¿ª£¬¿É¸´Ñ¡
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 8, ArraySubType = UnmanagedType.I1)]
            public byte[] byRes2;
        }

        //³öÈë¿Ú¿ØÖÆ²ÎÊý
        [StructLayoutAttribute(LayoutKind.Sequential)]
        public struct NET_DVR_ENTRANCE_CFG
        {
            public uint dwSize;
            public byte byEnable;
            public byte byBarrierGateCtrlMode;
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 2, ArraySubType = UnmanagedType.I1)]
            public byte[] byRes1;//±£Áô
            public uint dwRelateTriggerMode;
            public uint dwMatchContent;
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = MAX_RELAY_NUM, ArraySubType = UnmanagedType.Struct)]
            public NET_DVR_RELAY_PARAM[] struRelayRelateInfo;//¼ÌµçÆ÷¹ØÁªÅäÖÃÐÅÏ¢
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = MAX_IOIN_NUM, ArraySubType = UnmanagedType.I1)]
            public byte[] byGateSingleIO;
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = MAX_VEHICLE_TYPE_NUM, ArraySubType = UnmanagedType.Struct)]
            public NET_DVR_VEHICLE_CONTROL[] struVehicleCtrl;//³µÁ¾ÐÅÏ¢¹Ü¿Ø
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 64, ArraySubType = UnmanagedType.I1)]
            public byte[] byRes2;//±£Áô
        }

        [StructLayoutAttribute(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
        public struct NET_DVR_MANUALSNAP
        {
            public byte byOSDEnable;
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 23, ArraySubType = UnmanagedType.I1)]
            public byte[] byRes;
        }




        /********************************½Ó¿Ú²ÎÊý½á¹¹(end)*********************************/


        /********************************SDK½Ó¿Úº¯ÊýÉùÃ÷*********************************/

        /*********************************************************
        Function:	NET_DVR_Init
        Desc:  ³õÊ¼»¯SDK£¬µ÷ÓÃÆäËûSDKº¯ÊýµÄÇ°Ìá¡£
        Input:	
        Output:	
        Return:	TRUE±íÊ¾³É¹¦£¬FALSE±íÊ¾Ê§°Ü¡£
        **********************************************************/
        [DllImport(@"HCNetSDK.dll")]
        public static extern bool NET_DVR_Init();

        /*********************************************************
        Function:	NET_DVR_Cleanup
        Desc:  ÊÍ·ÅSDK×ÊÔ´£¬ÔÚ½áÊøÖ®Ç°×îºóµ÷ÓÃ
        Input:	
        Output:	
        Return:	TRUE±íÊ¾³É¹¦£¬FALSE±íÊ¾Ê§°Ü
        **********************************************************/
        [DllImport(@"HCNetSDK.dll")]
        public static extern bool NET_DVR_Cleanup();

        [DllImport(@"HCNetSDK.dll")]
        public static extern bool NET_DVR_SetDVRMessage(uint nMessage, IntPtr hWnd);

        /*********************************************************
        Function:	EXCEPYIONCALLBACK
        Desc:  (»Øµ÷º¯Êý)
        Input:	
        Output:	
        Return:	
        **********************************************************/
        public delegate void EXCEPYIONCALLBACK(uint dwType, int lUserID, int lHandle, IntPtr pUser);

        [DllImport(@"HCNetSDK.dll")]
        public static extern bool NET_DVR_SetExceptionCallBack_V30(uint nMessage, IntPtr hWnd, EXCEPYIONCALLBACK fExceptionCallBack, IntPtr pUser);


        /*********************************************************
        Function:	MESSCALLBACK
        Desc:  (»Øµ÷º¯Êý)
        Input:	
        Output:	
        Return:	
        **********************************************************/
        public delegate int MESSCALLBACK(int lCommand, string sDVRIP, string pBuf, uint dwBufLen);

        [DllImport(@"HCNetSDK.dll")]
        public static extern bool NET_DVR_SetDVRMessCallBack(MESSCALLBACK fMessCallBack);

        /*********************************************************
        Function:	MESSCALLBACKEX
        Desc:  (»Øµ÷º¯Êý)
        Input:	
        Output:	
        Return:	
        **********************************************************/
        public delegate int MESSCALLBACKEX(int iCommand, int iUserID, string pBuf, uint dwBufLen);

        [DllImport(@"HCNetSDK.dll")]
        public static extern bool NET_DVR_SetDVRMessCallBack_EX(MESSCALLBACKEX fMessCallBack_EX);

        /*********************************************************
        Function:	MESSCALLBACKNEW
        Desc:  (»Øµ÷º¯Êý)
        Input:	
        Output:	
        Return:	
        **********************************************************/
        public delegate int MESSCALLBACKNEW(int lCommand, string sDVRIP, string pBuf, uint dwBufLen, ushort dwLinkDVRPort);

        [DllImport(@"HCNetSDK.dll")]
        public static extern bool NET_DVR_SetDVRMessCallBack_NEW(MESSCALLBACKNEW fMessCallBack_NEW);

        /*********************************************************
        Function:	MESSAGECALLBACK
        Desc:  (»Øµ÷º¯Êý)
        Input:	
        Output:	
        Return:	
        **********************************************************/
        public delegate int MESSAGECALLBACK(int lCommand, System.IntPtr sDVRIP, System.IntPtr pBuf, uint dwBufLen, uint dwUser);

        [DllImport(@"HCNetSDK.dll")]
        public static extern bool NET_DVR_SetDVRMessageCallBack(MESSAGECALLBACK fMessageCallBack, uint dwUser);


        /*********************************************************
        Function:	MSGCallBack
        Desc:  (»Øµ÷º¯Êý)
        Input:	
        Output:	
        Return:	
        **********************************************************/
        public delegate void MSGCallBack(int lCommand, ref NET_DVR_ALARMER pAlarmer, IntPtr pAlarmInfo, uint dwBufLen, IntPtr pUser);

        [DllImport(@"HCNetSDK.dll")]
        public static extern bool NET_DVR_SetDVRMessageCallBack_V30(MSGCallBack fMessageCallBack, IntPtr pUser);

        [DllImport(@"HCNetSDK.dll")]
        public static extern bool NET_DVR_SetDVRMessageCallBack_V31(MSGCallBack fMessageCallBack, IntPtr pUser);

        [DllImport(@"HCNetSDK.dll")]
        public static extern bool NET_DVR_SetConnectTime(uint dwWaitTime, uint dwTryTimes);

        [DllImport(@"HCNetSDK.dll")]
        public static extern bool NET_DVR_SetReconnect(uint dwInterval, int bEnableRecon);

        [DllImport(@"HCNetSDK.dll")]
        public static extern uint NET_DVR_GetSDKVersion();

        [DllImport(@"HCNetSDK.dll")]
        public static extern uint NET_DVR_GetSDKBuildVersion();

        [DllImport(@"HCNetSDK.dll")]
        public static extern Int32 NET_DVR_IsSupport();

        [DllImport(@"HCNetSDK.dll")]
        public static extern bool NET_DVR_StartListen(string sLocalIP, ushort wLocalPort);

        [DllImport(@"HCNetSDK.dll")]
        public static extern bool NET_DVR_StopListen();

        [DllImport(@"HCNetSDK.dll")]
        public static extern int NET_DVR_StartListen_V30(String sLocalIP, ushort wLocalPort, MSGCallBack DataCallback, IntPtr pUserData);

        [DllImport(@"HCNetSDK.dll")]
        public static extern bool NET_DVR_StopListen_V30(Int32 lListenHandle);

        [DllImport(@"HCNetSDK.dll")]
        public static extern Int32 NET_DVR_Login(string sDVRIP, ushort wDVRPort, string sUserName, string sPassword, ref NET_DVR_DEVICEINFO lpDeviceInfo);

        [DllImport(@"HCNetSDK.dll")]
        public static extern bool NET_DVR_Logout(int iUserID);

        [DllImport(@"HCNetSDK.dll")]
        public static extern uint NET_DVR_GetLastError();

        [DllImport(@"HCNetSDK.dll")]
        public static extern IntPtr NET_DVR_GetErrorMsg(ref int pErrorNo);

        [DllImport(@"HCNetSDK.dll")]
        public static extern bool NET_DVR_SetShowMode(uint dwShowType, uint colorKey);

        [DllImport(@"HCNetSDK.dll")]
        public static extern bool NET_DVR_GetDVRIPByResolveSvr(string sServerIP, ushort wServerPort, string sDVRName, ushort wDVRNameLen, string sDVRSerialNumber, ushort wDVRSerialLen, IntPtr pGetIP);

        [DllImport(@"HCNetSDK.dll")]
        public static extern bool NET_DVR_GetDVRIPByResolveSvr_EX(string sServerIP, ushort wServerPort, ref byte sDVRName, ushort wDVRNameLen, ref byte sDVRSerialNumber, ushort wDVRSerialLen, string sGetIP, ref uint dwPort);

        //Ô¤ÀÀÏà¹Ø½Ó¿Ú
        [DllImport(@"HCNetSDK.dll")]
        public static extern Int32 NET_DVR_RealPlay(int iUserID, ref NET_DVR_CLIENTINFO lpClientInfo);

        [DllImport(@"HCNetSDK.dll")]
        public static extern Int32 NET_SDK_RealPlay(int iUserLogID, ref NET_SDK_CLIENTINFO lpDVRClientInfo);
        /*********************************************************
        Function:	REALDATACALLBACK
        Desc:  Ô¤ÀÀ»Øµ÷
        Input:	lRealHandle µ±Ç°µÄÔ¤ÀÀ¾ä±ú 
                dwDataType Êý¾ÝÀàÐÍ
                pBuffer ´æ·ÅÊý¾ÝµÄ»º³åÇøÖ¸Õë 
                dwBufSize »º³åÇø´óÐ¡ 
                pUser ÓÃ»§Êý¾Ý 
        Output:	
        Return:	void
        **********************************************************/
        public delegate void REALDATACALLBACK(Int32 lRealHandle, UInt32 dwDataType, ref byte pBuffer, UInt32 dwBufSize, IntPtr pUser);
        [DllImport(@"HCNetSDK.dll")]

        /*********************************************************
        Function:	NET_DVR_RealPlay_V30
        Desc:  ÊµÊ±Ô¤ÀÀ¡£
        Input:	lUserID [in] NET_DVR_Login()»òNET_DVR_Login_V30()µÄ·µ»ØÖµ 
                lpClientInfo [in] Ô¤ÀÀ²ÎÊý 
                cbRealDataCallBack [in] ÂëÁ÷Êý¾Ý»Øµ÷º¯Êý 
                pUser [in] ÓÃ»§Êý¾Ý 
                bBlocked [in] ÇëÇóÂëÁ÷¹ý³ÌÊÇ·ñ×èÈû£º0£­·ñ£»1£­ÊÇ 
        Output:	
        Return:	1±íÊ¾Ê§°Ü£¬ÆäËûÖµ×÷ÎªNET_DVR_StopRealPlayµÈº¯ÊýµÄ¾ä±ú²ÎÊý
        **********************************************************/
        public static extern int NET_DVR_RealPlay_V30(int iUserID, ref NET_DVR_CLIENTINFO lpClientInfo, REALDATACALLBACK fRealDataCallBack_V30, IntPtr pUser, UInt32 bBlocked);

        /*********************************************************
        Function:	NET_DVR_RealPlay_V40
        Desc:  ÊµÊ±Ô¤ÀÀ¡£
        Input:	lUserID [in] NET_DVR_Login()»òNET_DVR_Login_V30()µÄ·µ»ØÖµ 
                lpClientInfo [in] Ô¤ÀÀ²ÎÊý 
                cbRealDataCallBack [in] ÂëÁ÷Êý¾Ý»Øµ÷º¯Êý 
                pUser [in] ÓÃ»§Êý¾Ý 
                bBlocked [in] ÇëÇóÂëÁ÷¹ý³ÌÊÇ·ñ×èÈû£º0£­·ñ£»1£­ÊÇ 
        Output:	
        Return:	1±íÊ¾Ê§°Ü£¬ÆäËûÖµ×÷ÎªNET_DVR_StopRealPlayµÈº¯ÊýµÄ¾ä±ú²ÎÊý
        **********************************************************/
        [DllImport(@"HCNetSDK.dll")]
        public static extern int NET_DVR_RealPlay_V40(int iUserID, ref NET_DVR_PREVIEWINFO lpPreviewInfo, REALDATACALLBACK fRealDataCallBack_V30, IntPtr pUser);

        /*********************************************************
        Function:	NET_DVR_StopRealPlay
        Desc:  Í£Ö¹Ô¤ÀÀ¡£
        Input:	lRealHandle [in] Ô¤ÀÀ¾ä±ú£¬NET_DVR_RealPlay»òÕßNET_DVR_RealPlay_V30µÄ·µ»ØÖµ 
        Output:	
        Return:	
        **********************************************************/
        [DllImport(@"HCNetSDK.dll")]
        public static extern bool NET_DVR_StopRealPlay(int iRealHandle);

        /*********************************************************
        Function:	DRAWFUN
        Desc:  (»Øµ÷º¯Êý)
        Input:	
        Output:	
        Return:	
        **********************************************************/
        public delegate void DRAWFUN(int lRealHandle, IntPtr hDc, uint dwUser);

        [DllImport(@"HCNetSDK.dll")]
        public static extern bool NET_DVR_RigisterDrawFun(int lRealHandle, DRAWFUN fDrawFun, uint dwUser);

        [DllImport(@"HCNetSDK.dll")]
        public static extern bool NET_DVR_SetPlayerBufNumber(Int32 lRealHandle, uint dwBufNum);

        [DllImport(@"HCNetSDK.dll")]
        public static extern bool NET_DVR_ThrowBFrame(Int32 lRealHandle, uint dwNum);

        [DllImport(@"HCNetSDK.dll")]
        public static extern bool NET_DVR_SetAudioMode(uint dwMode);

        [DllImport(@"HCNetSDK.dll")]
        public static extern bool NET_DVR_OpenSound(Int32 lRealHandle);

        [DllImport(@"HCNetSDK.dll")]
        public static extern bool NET_DVR_CloseSound();

        [DllImport(@"HCNetSDK.dll")]
        public static extern bool NET_DVR_OpenSoundShare(Int32 lRealHandle);

        [DllImport(@"HCNetSDK.dll")]
        public static extern bool NET_DVR_CloseSoundShare(Int32 lRealHandle);

        [DllImport(@"HCNetSDK.dll")]
        public static extern bool NET_DVR_Volume(Int32 lRealHandle, ushort wVolume);

        [DllImport(@"HCNetSDK.dll")]
        public static extern bool NET_DVR_SaveRealData(Int32 lRealHandle, string sFileName);

        [DllImport(@"HCNetSDK.dll")]
        public static extern bool NET_DVR_StopSaveRealData(Int32 lRealHandle);

        /*********************************************************
        Function:	REALDATACALLBACK
        Desc:  (»Øµ÷º¯Êý)
        Input:	
        Output:	
        Return:	
        **********************************************************/
        public delegate void SETREALDATACALLBACK(int lRealHandle, uint dwDataType, IntPtr pBuffer, uint dwBufSize, uint dwUser);

        [DllImport(@"HCNetSDK.dll")]
        public static extern bool NET_DVR_SetRealDataCallBack(int lRealHandle, SETREALDATACALLBACK fRealDataCallBack, uint dwUser);

        /*********************************************************
        Function:	STDDATACALLBACK
        Desc:  (»Øµ÷º¯Êý)
        Input:	
        Output:	
        Return:	
        **********************************************************/
        public delegate void STDDATACALLBACK(int lRealHandle, uint dwDataType, ref byte pBuffer, uint dwBufSize, uint dwUser);

        [DllImport(@"HCNetSDK.dll")]
        public static extern bool NET_DVR_SetStandardDataCallBack(int lRealHandle, STDDATACALLBACK fStdDataCallBack, uint dwUser);

        [DllImport(@"HCNetSDK.dll")]
        public static extern bool NET_DVR_STDControl(int lUserID, uint dwCommand, ref NET_DVR_STD_CONTROL lpControlParam);

        [DllImport(@"HCNetSDK.dll")]
        public static extern bool NET_DVR_CapturePicture(Int32 lRealHandle, string sPicFileName);

        //¶¯Ì¬Éú³ÉIÖ¡
        [DllImport(@"HCNetSDK.dll")]
        public static extern bool NET_DVR_MakeKeyFrame(Int32 lUserID, Int32 lChannel);//Ö÷ÂëÁ÷

        [DllImport(@"HCNetSDK.dll")]
        public static extern bool NET_DVR_MakeKeyFrameSub(Int32 lUserID, Int32 lChannel);//×ÓÂëÁ÷

        //ÔÆÌ¨¿ØÖÆÏà¹Ø½Ó¿Ú
        [DllImport(@"HCNetSDK.dll")]
        public static extern bool NET_DVR_GetPTZCtrl(Int32 lRealHandle);

        [DllImport(@"HCNetSDK.dll")]
        public static extern bool NET_DVR_GetPTZCtrl_Other(Int32 lUserID, int lChannel);

        [DllImport(@"HCNetSDK.dll")]
        public static extern bool NET_DVR_PTZControl(Int32 lRealHandle, uint dwPTZCommand, uint dwStop);

        [DllImport(@"HCNetSDK.dll")]
        public static extern bool NET_DVR_PTZControl_Other(Int32 lUserID, Int32 lChannel, uint dwPTZCommand, uint dwStop);

        [DllImport(@"HCNetSDK.dll")]
        public static extern bool NET_DVR_TransPTZ(Int32 lRealHandle, string pPTZCodeBuf, uint dwBufSize);

        [DllImport(@"HCNetSDK.dll")]
        public static extern bool NET_DVR_TransPTZ_Other(int lUserID, int lChannel, string pPTZCodeBuf, uint dwBufSize);

        [DllImport(@"HCNetSDK.dll")]
        public static extern bool NET_DVR_PTZPreset(int lRealHandle, uint dwPTZPresetCmd, uint dwPresetIndex);

        [DllImport(@"HCNetSDK.dll")]
        public static extern bool NET_DVR_PTZPreset_Other(int lUserID, int lChannel, uint dwPTZPresetCmd, uint dwPresetIndex);

        [DllImport(@"HCNetSDK.dll")]
        public static extern bool NET_DVR_TransPTZ_EX(int lRealHandle, string pPTZCodeBuf, uint dwBufSize);

        [DllImport(@"HCNetSDK.dll")]
        public static extern bool NET_DVR_PTZControl_EX(int lRealHandle, uint dwPTZCommand, uint dwStop);

        [DllImport(@"HCNetSDK.dll")]
        public static extern bool NET_DVR_PTZPreset_EX(int lRealHandle, uint dwPTZPresetCmd, uint dwPresetIndex);

        [DllImport(@"HCNetSDK.dll")]
        public static extern bool NET_DVR_PTZCruise(int lRealHandle, uint dwPTZCruiseCmd, byte byCruiseRoute, byte byCruisePoint, ushort wInput);

        [DllImport(@"HCNetSDK.dll")]
        public static extern bool NET_DVR_PTZCruise_Other(int lUserID, int lChannel, uint dwPTZCruiseCmd, byte byCruiseRoute, byte byCruisePoint, ushort wInput);

        [DllImport(@"HCNetSDK.dll")]
        public static extern bool NET_DVR_PTZCruise_EX(int lRealHandle, uint dwPTZCruiseCmd, byte byCruiseRoute, byte byCruisePoint, ushort wInput);

        [DllImport(@"HCNetSDK.dll")]
        public static extern bool NET_DVR_PTZTrack(int lRealHandle, uint dwPTZTrackCmd);

        [DllImport(@"HCNetSDK.dll")]
        public static extern bool NET_DVR_PTZTrack_Other(int lUserID, int lChannel, uint dwPTZTrackCmd);

        [DllImport(@"HCNetSDK.dll")]
        public static extern bool NET_DVR_PTZTrack_EX(int lRealHandle, uint dwPTZTrackCmd);

        [DllImport(@"HCNetSDK.dll")]
        public static extern bool NET_DVR_PTZControlWithSpeed(int lRealHandle, uint dwPTZCommand, uint dwStop, uint dwSpeed);

        [DllImport(@"HCNetSDK.dll")]
        public static extern bool NET_DVR_PTZControlWithSpeed_Other(int lUserID, int lChannel, int dwPTZCommand, int dwStop, int dwSpeed);

        [DllImport(@"HCNetSDK.dll")]
        public static extern bool NET_DVR_PTZControlWithSpeed_EX(int lRealHandle, uint dwPTZCommand, uint dwStop, uint dwSpeed);

        [DllImport(@"HCNetSDK.dll")]
        public static extern bool NET_DVR_GetPTZCruise(int lUserID, int lChannel, int lCruiseRoute, ref NET_DVR_CRUISE_RET lpCruiseRet);

        [DllImport(@"HCNetSDK.dll")]
        public static extern bool NET_DVR_PTZMltTrack(int lRealHandle, uint dwPTZTrackCmd, uint dwTrackIndex);

        [DllImport(@"HCNetSDK.dll")]
        public static extern bool NET_DVR_PTZMltTrack_Other(int lUserID, int lChannel, uint dwPTZTrackCmd, uint dwTrackIndex);

        [DllImport(@"HCNetSDK.dll")]
        public static extern bool NET_DVR_PTZMltTrack_EX(int lRealHandle, uint dwPTZTrackCmd, uint dwTrackIndex);

        //ÎÄ¼þ²éÕÒÓë»Ø·Å
        [DllImport(@"HCNetSDK.dll")]
        public static extern int NET_DVR_FindFile(int lUserID, int lChannel, uint dwFileType, ref NET_DVR_TIME lpStartTime, ref NET_DVR_TIME lpStopTime);

        [DllImport(@"HCNetSDK.dll")]
        public static extern int NET_DVR_FindNextFile(int lFindHandle, ref NET_DVR_FIND_DATA lpFindData);

        [DllImport(@"HCNetSDK.dll")]
        public static extern bool NET_DVR_FindClose(int lFindHandle);

        [DllImport(@"HCNetSDK.dll")]
        public static extern int NET_DVR_FindNextFile_V30(int lFindHandle, ref NET_DVR_FINDDATA_V30 lpFindData);

        [DllImport(@"HCNetSDK.dll")]
        public static extern int NET_DVR_FindFile_V30(int lUserID, ref NET_DVR_FILECOND pFindCond);

        [DllImport(@"HCNetSDK.dll")]
        public static extern bool NET_DVR_FindClose_V30(int lFindHandle);

        //2007-04-16Ôö¼Ó²éÑ¯½á¹û´ø¿¨ºÅµÄÎÄ¼þ²éÕÒ
        [DllImport(@"HCNetSDK.dll")]
        public static extern int NET_DVR_FindNextFile_Card(int lFindHandle, ref NET_DVR_FINDDATA_CARD lpFindData);

        [DllImport(@"HCNetSDK.dll")]
        public static extern int NET_DVR_FindFile_Card(int lUserID, int lChannel, uint dwFileType, ref NET_DVR_TIME lpStartTime, ref NET_DVR_TIME lpStopTime);

        [DllImport(@"HCNetSDK.dll")]
        public static extern bool NET_DVR_LockFileByName(int lUserID, string sLockFileName);

        [DllImport(@"HCNetSDK.dll")]
        public static extern bool NET_DVR_UnlockFileByName(int lUserID, string sUnlockFileName);

        [DllImport(@"HCNetSDK.dll")]
        public static extern int NET_DVR_PlayBackByName(int lUserID, string sPlayBackFileName, IntPtr hWnd);

        [DllImport(@"HCNetSDK.dll")]
        public static extern int NET_DVR_PlayBackByTime(int lUserID, int lChannel, ref NET_DVR_TIME lpStartTime, ref NET_DVR_TIME lpStopTime, System.IntPtr hWnd);

        [DllImport(@"HCNetSDK.dll")]
        public static extern bool NET_DVR_PlayBackControl(int lPlayHandle, uint dwControlCode, uint dwInValue, ref uint LPOutValue);

        [DllImport(@"HCNetSDK.dll")]
        public static extern bool NET_DVR_StopPlayBack(int lPlayHandle);

        [DllImport(@"HCNetSDK.dll")]
        public static extern int NET_DVR_PlayBackReverseByName(int lUserID, string sPlayBackFileName, IntPtr hWnd);

        /*********************************************************
        Function:	PLAYDATACALLBACK
        Desc:  (»Øµ÷º¯Êý)
        Input:	
        Output:	
        Return:	
        **********************************************************/
        public delegate void PLAYDATACALLBACK(int lPlayHandle, uint dwDataType, IntPtr pBuffer, uint dwBufSize, uint dwUser);

        [DllImport(@"HCNetSDK.dll")]
        public static extern bool NET_DVR_SetPlayDataCallBack(int lPlayHandle, PLAYDATACALLBACK fPlayDataCallBack, uint dwUser);

        public delegate void PLAYDATACALLBACK_V40(int lPlayHandle, uint dwDataType, ref byte pBuffer, uint dwBufSize, IntPtr pUser);

        [DllImport(@"HCNetSDK.dll")]
        public static extern bool NET_DVR_SetPlayDataCallBack_V40(int lPlayHandle, PLAYDATACALLBACK_V40 fPlayDataCallBack_V40, IntPtr pUser);

        [DllImport(@"HCNetSDK.dll")]
        public static extern bool NET_DVR_PlayBackSaveData(int lPlayHandle, string sFileName);

        [DllImport(@"HCNetSDK.dll")]
        public static extern bool NET_DVR_StopPlayBackSave(int lPlayHandle);

        [DllImport(@"HCNetSDK.dll")]
        public static extern bool NET_DVR_GetPlayBackOsdTime(int lPlayHandle, ref NET_DVR_TIME lpOsdTime);

        [DllImport(@"HCNetSDK.dll")]
        public static extern bool NET_DVR_PlayBackCaptureFile(int lPlayHandle, string sFileName);

        [DllImport(@"HCNetSDK.dll")]
        public static extern int NET_DVR_GetFileByName(int lUserID, string sDVRFileName, string sSavedFileName);

        [DllImport(@"HCNetSDK.dll")]
        public static extern int NET_DVR_GetFileByTime(int lUserID, int lChannel, ref NET_DVR_TIME lpStartTime, ref NET_DVR_TIME lpStopTime, string sSavedFileName);

        [DllImport(@"HCNetSDK.dll")]
        public static extern bool NET_DVR_StopGetFile(int lFileHandle);

        [DllImport(@"HCNetSDK.dll")]
        public static extern int NET_DVR_GetDownloadPos(int lFileHandle);

        [DllImport(@"HCNetSDK.dll")]
        public static extern int NET_DVR_GetPlayBackPos(int lPlayHandle);

        //Éý¼¶
        [DllImport(@"HCNetSDK.dll")]
        public static extern int NET_DVR_Upgrade(int lUserID, string sFileName);

        [DllImport(@"HCNetSDK.dll")]
        public static extern int NET_DVR_GetUpgradeState(int lUpgradeHandle);

        [DllImport(@"HCNetSDK.dll")]
        public static extern int NET_DVR_GetUpgradeProgress(int lUpgradeHandle);

        [DllImport(@"HCNetSDK.dll")]
        public static extern bool NET_DVR_CloseUpgradeHandle(int lUpgradeHandle);

        [DllImport(@"HCNetSDK.dll")]
        public static extern bool NET_DVR_SetNetworkEnvironment(uint dwEnvironmentLevel);

        [DllImport(@"HCNetSDK.dll")]
        public static extern int NET_DVR_AdapterUpgrade(int lUserID, string sFileName);

        [DllImport(@"HCNetSDK.dll")]
        public static extern int NET_DVR_VcalibUpgrade(int lUserID, int Channel, string sFileName);

        [DllImport(@"HCNetSDK.dll")]
        public static extern int NET_DVR_Upgrade_V40(int lUserID, uint dwUpgradeType, string sFileName, IntPtr pInbuffer, ushort dwBufferLen);



        //Ô¶³Ì¸ñÊ½»¯Ó²ÅÌ
        [DllImport(@"HCNetSDK.dll")]
        public static extern int NET_DVR_FormatDisk(int lUserID, int lDiskNumber);

        [DllImport(@"HCNetSDK.dll")]
        public static extern bool NET_DVR_GetFormatProgress(int lFormatHandle, ref int pCurrentFormatDisk, ref int pCurrentDiskPos, ref int pFormatStatic);

        [DllImport(@"HCNetSDK.dll")]
        public static extern bool NET_DVR_CloseFormatHandle(int lFormatHandle);

        //±¨¾¯
        [DllImport(@"HCNetSDK.dll")]
        public static extern int NET_DVR_SetupAlarmChan(int lUserID);

        [DllImport(@"HCNetSDK.dll")]
        public static extern bool NET_DVR_CloseAlarmChan(int lAlarmHandle);

        [DllImport(@"HCNetSDK.dll")]
        public static extern int NET_DVR_SetupAlarmChan_V30(int lUserID);

        [DllImport(@"HCNetSDK.dll")]
        public static extern int NET_DVR_SetupAlarmChan_V41(int lUserID, ref NET_DVR_SETUPALARM_PARAM lpSetupParam);

        [DllImport(@"HCNetSDK.dll")]
        public static extern bool NET_DVR_CloseAlarmChan_V30(int lAlarmHandle);

        //ÓïÒô¶Ô½²
        /*********************************************************
        Function:	VOICEDATACALLBACK
        Desc:  (»Øµ÷º¯Êý)
        Input:	
        Output:	
        Return:	
        **********************************************************/
        public delegate void VOICEDATACALLBACK(int lVoiceComHandle, string pRecvDataBuffer, uint dwBufSize, byte byAudioFlag, uint dwUser);

        [DllImport(@"HCNetSDK.dll")]
        public static extern int NET_DVR_StartVoiceCom(int lUserID, VOICEDATACALLBACK fVoiceDataCallBack, uint dwUser);

        /*********************************************************
        Function:	VOICEDATACALLBACKV30
        Desc:  (»Øµ÷º¯Êý)
        Input:	
        Output:	
        Return:	
        **********************************************************/
        public delegate void VOICEDATACALLBACKV30(int lVoiceComHandle, string pRecvDataBuffer, uint dwBufSize, byte byAudioFlag, System.IntPtr pUser);

        [DllImport(@"HCNetSDK.dll")]
        public static extern int NET_DVR_StartVoiceCom_V30(int lUserID, uint dwVoiceChan, bool bNeedCBNoEncData, VOICEDATACALLBACKV30 fVoiceDataCallBack, IntPtr pUser);


        [DllImport(@"HCNetSDK.dll")]
        public static extern bool NET_DVR_SetVoiceComClientVolume(int lVoiceComHandle, ushort wVolume);

        [DllImport(@"HCNetSDK.dll")]
        public static extern bool NET_DVR_StopVoiceCom(int lVoiceComHandle);

        //ÓïÒô×ª·¢
        [DllImport(@"HCNetSDK.dll")]
        public static extern int NET_DVR_StartVoiceCom_MR(int lUserID, VOICEDATACALLBACK fVoiceDataCallBack, uint dwUser);

        [DllImport(@"HCNetSDK.dll")]
        public static extern int NET_DVR_StartVoiceCom_MR_V30(int lUserID, uint dwVoiceChan, VOICEDATACALLBACKV30 fVoiceDataCallBack, IntPtr pUser);

        [DllImport(@"HCNetSDK.dll")]
        public static extern bool NET_DVR_VoiceComSendData(int lVoiceComHandle, string pSendBuf, uint dwBufSize);

        //ÓïÒô¹ã²¥
        [DllImport(@"HCNetSDK.dll")]
        public static extern bool NET_DVR_ClientAudioStart();

        /*********************************************************
        Function:	VOICEAUDIOSTART
        Desc:  (»Øµ÷º¯Êý)
        Input:	
        Output:	
        Return:	
        **********************************************************/
        public delegate void VOICEAUDIOSTART(string pRecvDataBuffer, uint dwBufSize, IntPtr pUser);

        [DllImport(@"HCNetSDK.dll")]
        public static extern bool NET_DVR_ClientAudioStart_V30(VOICEAUDIOSTART fVoiceAudioStart, IntPtr pUser);


        [DllImport(@"HCNetSDK.dll")]
        public static extern bool NET_DVR_ClientAudioStop();

        [DllImport(@"HCNetSDK.dll")]
        public static extern bool NET_DVR_AddDVR(int lUserID);

        [DllImport(@"HCNetSDK.dll")]
        public static extern int NET_DVR_AddDVR_V30(int lUserID, uint dwVoiceChan);

        [DllImport(@"HCNetSDK.dll")]
        public static extern bool NET_DVR_DelDVR(int lUserID);

        [DllImport(@"HCNetSDK.dll")]
        public static extern bool NET_DVR_DelDVR_V30(int lVoiceHandle);


        //Í¸Ã÷Í¨µÀÉèÖÃ
        /*********************************************************
        Function:	SERIALDATACALLBACK
        Desc:  (»Øµ÷º¯Êý)
        Input:	
        Output:	
        Return:	
        **********************************************************/
        public delegate void SERIALDATACALLBACK(int lSerialHandle, string pRecvDataBuffer, uint dwBufSize, uint dwUser);

        [DllImport(@"HCNetSDK.dll")]
        public static extern bool NET_DVR_SerialStart(int lUserID, int lSerialPort, SERIALDATACALLBACK fSerialDataCallBack, uint dwUser);

        //485×÷ÎªÍ¸Ã÷Í¨µÀÊ±£¬ÐèÒªÖ¸Ã÷Í¨µÀºÅ£¬ÒòÎª²»Í¬Í¨µÀºÅ485µÄÉèÖÃ¿ÉÒÔ²»Í¬(±ÈÈç²¨ÌØÂÊ)
        [DllImport(@"HCNetSDK.dll")]
        public static extern bool NET_DVR_SerialSend(int lSerialHandle, int lChannel, string pSendBuf, uint dwBufSize);

        [DllImport(@"HCNetSDK.dll")]
        public static extern bool NET_DVR_SerialStop(int lSerialHandle);

        [DllImport(@"HCNetSDK.dll")]
        public static extern bool NET_DVR_SendTo232Port(int lUserID, string pSendBuf, uint dwBufSize);

        [DllImport(@"HCNetSDK.dll")]
        public static extern bool NET_DVR_SendToSerialPort(int lUserID, uint dwSerialPort, uint dwSerialIndex, string pSendBuf, uint dwBufSize);

        //½âÂë nBitrate = 16000
        [DllImport(@"HCNetSDK.dll")]
        public static extern System.IntPtr NET_DVR_InitG722Decoder(int nBitrate);

        [DllImport(@"HCNetSDK.dll")]
        public static extern void NET_DVR_ReleaseG722Decoder(IntPtr pDecHandle);

        [DllImport(@"HCNetSDK.dll")]
        public static extern bool NET_DVR_DecodeG722Frame(IntPtr pDecHandle, ref byte pInBuffer, ref byte pOutBuffer);

        //±àÂë
        [DllImport(@"HCNetSDK.dll")]
        public static extern IntPtr NET_DVR_InitG722Encoder();

        [DllImport(@"HCNetSDK.dll")]
        public static extern bool NET_DVR_EncodeG722Frame(IntPtr pEncodeHandle, ref byte pInBuffer, ref byte pOutBuffer);

        [DllImport(@"HCNetSDK.dll")]
        public static extern void NET_DVR_ReleaseG722Encoder(IntPtr pEncodeHandle);

        //Ô¶³Ì¿ØÖÆ±¾µØÏÔÊ¾
        [DllImport(@"HCNetSDK.dll")]
        public static extern bool NET_DVR_ClickKey(int lUserID, int lKeyIndex);

        //Ô¶³Ì¿ØÖÆÉè±¸¶ËÊÖ¶¯Â¼Ïñ
        [DllImport(@"HCNetSDK.dll")]
        public static extern bool NET_DVR_StartDVRRecord(int lUserID, int lChannel, int lRecordType);

        [DllImport(@"HCNetSDK.dll")]
        public static extern bool NET_DVR_StopDVRRecord(int lUserID, int lChannel);

        //½âÂë¿¨
        [DllImport(@"HCNetSDK.dll")]
        public static extern bool NET_DVR_InitDevice_Card(ref int pDeviceTotalChan);

        [DllImport(@"HCNetSDK.dll")]
        public static extern bool NET_DVR_ReleaseDevice_Card();

        [DllImport(@"HCNetSDK.dll")]
        public static extern bool NET_DVR_InitDDraw_Card(IntPtr hParent, uint colorKey);

        [DllImport(@"HCNetSDK.dll")]
        public static extern bool NET_DVR_ReleaseDDraw_Card();

        [DllImport(@"HCNetSDK.dll")]
        public static extern int NET_DVR_RealPlay_Card(int lUserID, ref NET_DVR_CARDINFO lpCardInfo, int lChannelNum);

        [DllImport(@"HCNetSDK.dll")]
        public static extern bool NET_DVR_ResetPara_Card(int lRealHandle, ref NET_DVR_DISPLAY_PARA lpDisplayPara);

        [DllImport(@"HCNetSDK.dll")]
        public static extern bool NET_DVR_RefreshSurface_Card();

        [DllImport(@"HCNetSDK.dll")]
        public static extern bool NET_DVR_ClearSurface_Card();

        [DllImport(@"HCNetSDK.dll")]
        public static extern bool NET_DVR_RestoreSurface_Card();

        [DllImport(@"HCNetSDK.dll")]
        public static extern bool NET_DVR_OpenSound_Card(int lRealHandle);

        [DllImport(@"HCNetSDK.dll")]
        public static extern bool NET_DVR_CloseSound_Card(int lRealHandle);

        [DllImport(@"HCNetSDK.dll")]
        public static extern bool NET_DVR_SetVolume_Card(int lRealHandle, ushort wVolume);

        [DllImport(@"HCNetSDK.dll")]
        public static extern bool NET_DVR_AudioPreview_Card(int lRealHandle, int bEnable);

        [DllImport(@"HCNetSDK.dll")]
        public static extern int NET_DVR_GetCardLastError_Card();

        [DllImport(@"HCNetSDK.dll")]
        public static extern System.IntPtr NET_DVR_GetChanHandle_Card(int lRealHandle);

        [DllImport(@"HCNetSDK.dll")]
        public static extern bool NET_DVR_CapturePicture_Card(int lRealHandle, string sPicFileName);

        //»ñÈ¡½âÂë¿¨ÐòÁÐºÅ´Ë½Ó¿ÚÎÞÐ§£¬¸ÄÓÃGetBoardDetail½Ó¿Ú»ñµÃ(2005-12-08Ö§³Ö)
        [DllImport(@"HCNetSDK.dll")]
        public static extern bool NET_DVR_GetSerialNum_Card(int lChannelNum, ref uint pDeviceSerialNo);

        //ÈÕÖ¾
        [DllImport(@"HCNetSDK.dll")]
        public static extern int NET_DVR_FindDVRLog(int lUserID, int lSelectMode, uint dwMajorType, uint dwMinorType, ref NET_DVR_TIME lpStartTime, ref NET_DVR_TIME lpStopTime);

        [DllImport(@"HCNetSDK.dll")]
        public static extern int NET_DVR_FindNextLog(int lLogHandle, ref NET_DVR_LOG lpLogData);

        [DllImport(@"HCNetSDK.dll")]
        public static extern bool NET_DVR_FindLogClose(int lLogHandle);

        [DllImport(@"HCNetSDK.dll")]
        public static extern int NET_DVR_FindDVRLog_V30(int lUserID, int lSelectMode, uint dwMajorType, uint dwMinorType, ref NET_DVR_TIME lpStartTime, ref NET_DVR_TIME lpStopTime, bool bOnlySmart);

        [DllImport(@"HCNetSDK.dll")]
        public static extern int NET_DVR_FindNextLog_V30(int lLogHandle, ref NET_DVR_LOG_V30 lpLogData);

        [DllImport(@"HCNetSDK.dll")]
        public static extern bool NET_DVR_FindLogClose_V30(int lLogHandle);

        //½ØÖ¹2004Äê8ÔÂ5ÈÕ,¹²113¸ö½Ó¿Ú
        //ATM DVR
        [DllImport(@"HCNetSDK.dll")]
        public static extern int NET_DVR_FindFileByCard(int lUserID, int lChannel, uint dwFileType, int nFindType, ref byte sCardNumber, ref NET_DVR_TIME lpStartTime, ref NET_DVR_TIME lpStopTime);


        //2005-09-15
        [DllImport(@"HCNetSDK.dll")]
        public static extern bool NET_DVR_CaptureJPEGPicture(int lUserID, int lChannel, ref NET_DVR_JPEGPARA lpJpegPara, string sPicFileName);

        //JPEG×¥Í¼µ½ÄÚ´æ
        [DllImport(@"HCNetSDK.dll")]
        public static extern bool NET_DVR_CaptureJPEGPicture_NEW(int lUserID, int lChannel, ref NET_DVR_JPEGPARA lpJpegPara, string sJpegPicBuffer, uint dwPicSize, ref uint lpSizeReturned);

        //2006-02-16
        [DllImport(@"HCNetSDK.dll")]
        public static extern int NET_DVR_GetRealPlayerIndex(int lRealHandle);

        [DllImport(@"HCNetSDK.dll")]
        public static extern int NET_DVR_GetPlayBackPlayerIndex(int lPlayHandle);

        //2006-08-28 704-640 Ëõ·ÅÅäÖÃ
        [DllImport(@"HCNetSDK.dll")]
        public static extern bool NET_DVR_SetScaleCFG(int lUserID, uint dwScale);

        [DllImport(@"HCNetSDK.dll")]
        public static extern bool NET_DVR_GetScaleCFG(int lUserID, ref uint lpOutScale);

        [DllImport(@"HCNetSDK.dll")]
        public static extern bool NET_DVR_SetScaleCFG_V30(int lUserID, ref NET_DVR_SCALECFG pScalecfg);

        [DllImport(@"HCNetSDK.dll")]
        public static extern bool NET_DVR_GetScaleCFG_V30(int lUserID, ref NET_DVR_SCALECFG pScalecfg);

        //2006-08-28 ATM»ú¶Ë¿ÚÉèÖÃ
        [DllImport(@"HCNetSDK.dll")]
        public static extern bool NET_DVR_SetATMPortCFG(int lUserID, ushort wATMPort);

        [DllImport(@"HCNetSDK.dll")]
        public static extern bool NET_DVR_GetATMPortCFG(int lUserID, ref ushort LPOutATMPort);

        //2006-11-10 Ö§³ÖÏÔ¿¨¸¨ÖúÊä³ö
        [DllImport(@"HCNetSDK.dll")]
        public static extern bool NET_DVR_InitDDrawDevice();

        [DllImport(@"HCNetSDK.dll")]
        public static extern bool NET_DVR_ReleaseDDrawDevice();

        [DllImport(@"HCNetSDK.dll")]
        public static extern int NET_DVR_GetDDrawDeviceTotalNums();

        [DllImport(@"HCNetSDK.dll")]
        public static extern bool NET_DVR_SetDDrawDevice(int lPlayPort, uint nDeviceNum);

        [DllImport(@"HCNetSDK.dll")]
        public static extern bool NET_DVR_PTZSelZoomIn(int lRealHandle, ref NET_DVR_POINT_FRAME pStruPointFrame);

        [DllImport(@"HCNetSDK.dll")]
        public static extern bool NET_DVR_PTZSelZoomIn_EX(int lUserID, int lChannel, ref NET_DVR_POINT_FRAME pStruPointFrame);

        //½âÂëÉè±¸DS-6001D/DS-6001F
        [DllImport(@"HCNetSDK.dll")]
        public static extern bool NET_DVR_StartDecode(int lUserID, int lChannel, ref NET_DVR_DECODERINFO lpDecoderinfo);

        [DllImport(@"HCNetSDK.dll")]
        public static extern bool NET_DVR_StopDecode(int lUserID, int lChannel);

        [DllImport(@"HCNetSDK.dll")]
        public static extern bool NET_DVR_GetDecoderState(int lUserID, int lChannel, ref NET_DVR_DECODERSTATE lpDecoderState);

        //2005-08-01
        [DllImport(@"HCNetSDK.dll")]
        public static extern bool NET_DVR_SetDecInfo(int lUserID, int lChannel, ref NET_DVR_DECCFG lpDecoderinfo);

        [DllImport(@"HCNetSDK.dll")]
        public static extern bool NET_DVR_GetDecInfo(int lUserID, int lChannel, ref NET_DVR_DECCFG lpDecoderinfo);

        [DllImport(@"HCNetSDK.dll")]
        public static extern bool NET_DVR_SetDecTransPort(int lUserID, ref NET_DVR_PORTCFG lpTransPort);

        [DllImport(@"HCNetSDK.dll")]
        public static extern bool NET_DVR_GetDecTransPort(int lUserID, ref NET_DVR_PORTCFG lpTransPort);

        [DllImport(@"HCNetSDK.dll")]
        public static extern bool NET_DVR_DecPlayBackCtrl(int lUserID, int lChannel, uint dwControlCode, uint dwInValue, ref uint LPOutValue, ref NET_DVR_PLAYREMOTEFILE lpRemoteFileInfo);

        [DllImport(@"HCNetSDK.dll")]
        public static extern bool NET_DVR_StartDecSpecialCon(int lUserID, int lChannel, ref NET_DVR_DECCHANINFO lpDecChanInfo);

        [DllImport(@"HCNetSDK.dll")]
        public static extern bool NET_DVR_StopDecSpecialCon(int lUserID, int lChannel, ref NET_DVR_DECCHANINFO lpDecChanInfo);

        [DllImport(@"HCNetSDK.dll")]
        public static extern bool NET_DVR_DecCtrlDec(int lUserID, int lChannel, uint dwControlCode);

        [DllImport(@"HCNetSDK.dll")]
        public static extern bool NET_DVR_DecCtrlScreen(int lUserID, int lChannel, uint dwControl);

        [DllImport(@"HCNetSDK.dll")]
        public static extern bool NET_DVR_GetDecCurLinkStatus(int lUserID, int lChannel, ref NET_DVR_DECSTATUS lpDecStatus);

        //¶àÂ·½âÂëÆ÷
        //2007-11-30 V211Ö§³ÖÒÔÏÂ½Ó¿Ú //11
        [DllImport(@"HCNetSDK.dll")]
        public static extern bool NET_DVR_MatrixStartDynamic(int lUserID, uint dwDecChanNum, ref NET_DVR_MATRIX_DYNAMIC_DEC lpDynamicInfo);

        [DllImport(@"HCNetSDK.dll")]
        public static extern bool NET_DVR_MatrixStopDynamic(int lUserID, uint dwDecChanNum);

        [DllImport(@"HCNetSDK.dll")]
        public static extern bool NET_DVR_MatrixGetDecChanInfo(int lUserID, uint dwDecChanNum, ref NET_DVR_MATRIX_DEC_CHAN_INFO lpInter);

        [DllImport(@"HCNetSDK.dll")]
        public static extern bool NET_DVR_MatrixSetLoopDecChanInfo(int lUserID, uint dwDecChanNum, ref NET_DVR_MATRIX_LOOP_DECINFO lpInter);

        [DllImport(@"HCNetSDK.dll")]
        public static extern bool NET_DVR_MatrixGetLoopDecChanInfo(int lUserID, uint dwDecChanNum, ref NET_DVR_MATRIX_LOOP_DECINFO lpInter);

        [DllImport(@"HCNetSDK.dll")]
        public static extern bool NET_DVR_MatrixSetLoopDecChanEnable(int lUserID, uint dwDecChanNum, uint dwEnable);

        [DllImport(@"HCNetSDK.dll")]
        public static extern bool NET_DVR_MatrixGetLoopDecChanEnable(int lUserID, uint dwDecChanNum, ref uint lpdwEnable);

        [DllImport(@"HCNetSDK.dll")]
        public static extern bool NET_DVR_MatrixGetLoopDecEnable(int lUserID, ref uint lpdwEnable);

        [DllImport(@"HCNetSDK.dll")]
        public static extern bool NET_DVR_MatrixSetDecChanEnable(int lUserID, uint dwDecChanNum, uint dwEnable);

        [DllImport(@"HCNetSDK.dll")]
        public static extern bool NET_DVR_MatrixGetDecChanEnable(int lUserID, uint dwDecChanNum, ref uint lpdwEnable);

        [DllImport(@"HCNetSDK.dll")]
        public static extern bool NET_DVR_MatrixGetDecChanStatus(int lUserID, uint dwDecChanNum, ref NET_DVR_MATRIX_DEC_CHAN_STATUS lpInter);

        //2007-12-22 Ôö¼ÓÖ§³Ö½Ó¿Ú //18
        [DllImport(@"HCNetSDK.dll")]
        public static extern bool NET_DVR_MatrixSetTranInfo(int lUserID, ref NET_DVR_MATRIX_TRAN_CHAN_CONFIG lpTranInfo);

        [DllImport(@"HCNetSDK.dll")]
        public static extern bool NET_DVR_MatrixGetTranInfo(int lUserID, ref NET_DVR_MATRIX_TRAN_CHAN_CONFIG lpTranInfo);

        [DllImport(@"HCNetSDK.dll")]
        public static extern bool NET_DVR_MatrixSetRemotePlay(int lUserID, uint dwDecChanNum, ref NET_DVR_MATRIX_DEC_REMOTE_PLAY lpInter);

        [DllImport(@"HCNetSDK.dll")]
        public static extern bool NET_DVR_MatrixSetRemotePlayControl(int lUserID, uint dwDecChanNum, uint dwControlCode, uint dwInValue, ref uint LPOutValue);

        [DllImport(@"HCNetSDK.dll")]
        public static extern bool NET_DVR_MatrixGetRemotePlayStatus(int lUserID, uint dwDecChanNum, ref NET_DVR_MATRIX_DEC_REMOTE_PLAY_STATUS lpOuter);

        //2009-4-13 ÐÂÔö
        [DllImport(@"HCNetSDK.dll")]
        public static extern bool NET_DVR_MatrixStartDynamic_V30(int lUserID, uint dwDecChanNum, ref NET_DVR_PU_STREAM_CFG lpDynamicInfo);

        [DllImport(@"HCNetSDK.dll")]
        public static extern bool NET_DVR_MatrixSetLoopDecChanInfo_V30(int lUserID, uint dwDecChanNum, ref tagMATRIX_LOOP_DECINFO_V30 lpInter);

        [DllImport(@"HCNetSDK.dll")]
        public static extern bool NET_DVR_MatrixGetLoopDecChanInfo_V30(int lUserID, uint dwDecChanNum, ref tagMATRIX_LOOP_DECINFO_V30 lpInter);

        [DllImport(@"HCNetSDK.dll")]
        public static extern bool NET_DVR_MatrixGetDecChanInfo_V30(int lUserID, uint dwDecChanNum, ref tagDEC_MATRIX_CHAN_INFO lpInter);

        [DllImport(@"HCNetSDK.dll")]
        public static extern bool NET_DVR_MatrixSetTranInfo_V30(int lUserID, ref tagMATRIX_TRAN_CHAN_CONFIG lpTranInfo);

        [DllImport(@"HCNetSDK.dll")]
        public static extern bool NET_DVR_MatrixGetTranInfo_V30(int lUserID, ref tagMATRIX_TRAN_CHAN_CONFIG lpTranInfo);

        [DllImport(@"HCNetSDK.dll")]
        public static extern bool NET_DVR_MatrixGetDisplayCfg(int lUserID, uint dwDispChanNum, ref tagNET_DVR_VGA_DISP_CHAN_CFG lpDisplayCfg);

        [DllImport(@"HCNetSDK.dll")]
        public static extern bool NET_DVR_MatrixSetDisplayCfg(int lUserID, uint dwDispChanNum, ref tagNET_DVR_VGA_DISP_CHAN_CFG lpDisplayCfg);


        [DllImport(@"HCNetSDK.dll")]
        public static extern int NET_DVR_MatrixStartPassiveDecode(int lUserID, uint dwDecChanNum, ref tagNET_MATRIX_PASSIVEMODE lpPassiveMode);

        [DllImport(@"HCNetSDK.dll")]
        public static extern bool NET_DVR_MatrixSendData(int lPassiveHandle, System.IntPtr pSendBuf, uint dwBufSize);

        [DllImport(@"HCNetSDK.dll")]
        public static extern bool NET_DVR_MatrixStopPassiveDecode(int lPassiveHandle);

        [DllImport(@"HCNetSDK.dll")]
        public static extern bool NET_DVR_UploadLogo(int lUserID, uint dwDispChanNum, ref tagNET_DVR_DISP_LOGOCFG lpDispLogoCfg, System.IntPtr sLogoBuffer);

        public const int NET_DVR_SHOWLOGO = 1;/*ÏÔÊ¾LOGO*/
        public const int NET_DVR_HIDELOGO = 2;/*Òþ²ØLOGO*/

        [DllImport(@"HCNetSDK.dll")]
        public static extern bool NET_DVR_LogoSwitch(int lUserID, uint dwDecChan, uint dwLogoSwitch);

        [DllImport(@"HCNetSDK.dll")]
        public static extern bool NET_DVR_MatrixGetDeviceStatus(int lUserID, ref tagNET_DVR__DECODER_WORK_STATUS lpDecoderCfg);

        /*ÏÔÊ¾Í¨µÀÃüÁîÂë¶¨Òå*/
        //ÉÏº£ÊÀ²© ¶¨ÖÆ
        [DllImport(@"HCNetSDK.dll")]
        public static extern bool NET_DVR_RigisterPlayBackDrawFun(int lRealHandle, DRAWFUN fDrawFun, uint dwUser);


        public const int DISP_CMD_ENLARGE_WINDOW = 1;   /*ÏÔÊ¾Í¨µÀ·Å´óÄ³¸ö´°¿Ú*/
        public const int DISP_CMD_RENEW_WINDOW = 2; /*ÏÔÊ¾Í¨µÀ´°¿Ú»¹Ô­*/

        [DllImport(@"HCNetSDK.dll")]
        public static extern bool NET_DVR_MatrixDiaplayControl(int lUserID, uint dwDispChanNum, uint dwDispChanCmd, uint dwCmdParam);

        //end
        [DllImport(@"HCNetSDK.dll")]
        public static extern bool NET_DVR_RefreshPlay(int lPlayHandle);

        //»Ö¸´Ä¬ÈÏÖµ
        [DllImport(@"HCNetSDK.dll")]
        public static extern bool NET_DVR_RestoreConfig(int lUserID);

        //±£´æ²ÎÊý
        [DllImport(@"HCNetSDK.dll")]
        public static extern bool NET_DVR_SaveConfig(int lUserID);

        //ÖØÆô
        [DllImport(@"HCNetSDK.dll")]
        public static extern bool NET_DVR_RebootDVR(int lUserID);

        //¹Ø±ÕDVR
        [DllImport(@"HCNetSDK.dll")]
        public static extern bool NET_DVR_ShutDownDVR(int lUserID);

        //²ÎÊýÅäÖÃ begin
        [DllImport(@"HCNetSDK.dll")]
        public static extern bool NET_DVR_GetDVRConfig(int lUserID, uint dwCommand, int lChannel, IntPtr lpOutBuffer, uint dwOutBufferSize, ref uint lpBytesReturned);

        [DllImport(@"HCNetSDK.dll")]
        public static extern bool NET_DVR_SetDVRConfig(int lUserID, uint dwCommand, int lChannel, System.IntPtr lpInBuffer, uint dwInBufferSize);

        //±¨¾¯Ö÷»úÉè±¸ÓÃ»§ÅäÖÃ
        [DllImport(@"HCNetSDK.dll")]
        public static extern bool NET_DVR_SetAlarmDeviceUser(int lUserID, int lUserIndex, ref NET_DVR_ALARM_DEVICE_USER lpDeviceUser);

        [DllImport(@"HCNetSDK.dll")]
        public static extern bool NET_DVR_GetAlarmDeviceUser(int lUserID, int lUserIndex, ref NET_DVR_ALARM_DEVICE_USER lpDeviceUser);

        //ÅúÁ¿²ÎÊýÅäÖÃ
        [DllImport(@"HCNetSDK.dll")]
        public static extern bool NET_DVR_GetDeviceConfig(int lUserID, uint dwCommand, uint dwCount, IntPtr lpInBuffer, uint dwInBufferSize, IntPtr lpStatusList, IntPtr lpOutBuffer, uint dwOutBufferSize);

        [DllImport(@"HCNetSDK.dll")]
        public static extern bool NET_DVR_SetDeviceConfig(int lUserID, uint dwCommand, uint dwCount, IntPtr lpInBuffer, uint dwInBufferSize, IntPtr lpStatusList, IntPtr lpInParamBuffer, uint dwInParamBufferSize);

        [DllImport(@"HCNetSDK.dll")]
        public static extern bool NET_DVR_RemoteControl(int lUserID, uint dwCommand, IntPtr lpInBuffer, uint dwInBufferSize);

        [DllImport(@"HCNetSDK.dll")]
        public static extern bool NET_DVR_GetDVRWorkState_V30(int lUserID, IntPtr pWorkState);

        [DllImport(@"HCNetSDK.dll")]
        public static extern bool NET_DVR_GetDVRWorkState(int lUserID, ref NET_DVR_WORKSTATE lpWorkState);

        [DllImport(@"HCNetSDK.dll")]
        public static extern bool NET_DVR_SetVideoEffect(int lUserID, int lChannel, uint dwBrightValue, uint dwContrastValue, uint dwSaturationValue, uint dwHueValue);

        [DllImport(@"HCNetSDK.dll")]
        public static extern bool NET_DVR_GetVideoEffect(int lUserID, int lChannel, ref uint pBrightValue, ref uint pContrastValue, ref uint pSaturationValue, ref uint pHueValue);

        [DllImport(@"HCNetSDK.dll")]
        public static extern bool NET_DVR_ClientGetframeformat(int lUserID, ref NET_DVR_FRAMEFORMAT lpFrameFormat);

        [DllImport(@"HCNetSDK.dll")]
        public static extern bool NET_DVR_ClientSetframeformat(int lUserID, ref NET_DVR_FRAMEFORMAT lpFrameFormat);

        [DllImport(@"HCNetSDK.dll")]
        public static extern bool NET_DVR_ClientGetframeformat_V30(int lUserID, ref NET_DVR_FRAMEFORMAT_V30 lpFrameFormat);

        [DllImport(@"HCNetSDK.dll")]
        public static extern bool NET_DVR_ClientSetframeformat_V30(int lUserID, ref NET_DVR_FRAMEFORMAT_V30 lpFrameFormat);

        [DllImport(@"HCNetSDK.dll")]
        public static extern bool NET_DVR_Getframeformat_V31(int lUserID, ref tagNET_DVR_FRAMEFORMAT_V31 lpFrameFormat);

        [DllImport(@"HCNetSDK.dll")]
        public static extern bool NET_DVR_Setframeformat_V31(int lUserID, ref tagNET_DVR_FRAMEFORMAT_V31 lpFrameFormat);

        [DllImport(@"HCNetSDK.dll")]
        public static extern bool NET_DVR_GetAtmProtocol(int lUserID, ref tagNET_DVR_ATM_PROTOCOL lpAtmProtocol);

        [DllImport(@"HCNetSDK.dll")]
        public static extern bool NET_DVR_GetAlarmOut_V30(int lUserID, IntPtr lpAlarmOutState);

        [DllImport(@"HCNetSDK.dll")]
        public static extern bool NET_DVR_GetAlarmOut(int lUserID, ref NET_DVR_ALARMOUTSTATUS lpAlarmOutState);

        [DllImport(@"HCNetSDK.dll")]
        public static extern bool NET_DVR_SetAlarmOut(int lUserID, int lAlarmOutPort, int lAlarmOutStatic);

        //ÊÓÆµ²ÎÊýµ÷½Ú
        [DllImport(@"HCNetSDK.dll")]
        public static extern bool NET_DVR_ClientSetVideoEffect(int lRealHandle, uint dwBrightValue, uint dwContrastValue, uint dwSaturationValue, uint dwHueValue);

        [DllImport(@"HCNetSDK.dll")]
        public static extern bool NET_DVR_ClientGetVideoEffect(int lRealHandle, ref uint pBrightValue, ref uint pContrastValue, ref uint pSaturationValue, ref uint pHueValue);

        //ÅäÖÃÎÄ¼þ
        [DllImport(@"HCNetSDK.dll")]
        public static extern bool NET_DVR_GetConfigFile(int lUserID, string sFileName);

        [DllImport(@"HCNetSDK.dll")]
        public static extern bool NET_DVR_SetConfigFile(int lUserID, string sFileName);

        [DllImport(@"HCNetSDK.dll")]
        public static extern bool NET_DVR_GetConfigFile_V30(int lUserID, string sOutBuffer, uint dwOutSize, ref uint pReturnSize);

        [DllImport(@"HCNetSDK.dll")]
        public static extern bool NET_DVR_GetConfigFile_EX(int lUserID, string sOutBuffer, uint dwOutSize);

        [DllImport(@"HCNetSDK.dll")]
        public static extern bool NET_DVR_SetConfigFile_EX(int lUserID, string sInBuffer, uint dwInSize);

        //ÆôÓÃÈÕÖ¾ÎÄ¼þÐ´Èë½Ó¿Ú
        [DllImport(@"HCNetSDK.dll")]
        public static extern bool NET_DVR_SetLogToFile(int bLogEnable, string strLogDir, bool bAutoDel);

        [DllImport(@"HCNetSDK.dll")]
        public static extern bool NET_DVR_GetSDKState(ref NET_DVR_SDKSTATE pSDKState);

        [DllImport(@"HCNetSDK.dll")]
        public static extern bool NET_DVR_GetSDKAbility(ref NET_DVR_SDKABL pSDKAbl);

        [DllImport(@"HCNetSDK.dll")]
        public static extern bool NET_DVR_GetPTZProtocol(int lUserID, ref NET_DVR_PTZCFG pPtzcfg);

        //Ç°Ãæ°åËø¶¨
        [DllImport(@"HCNetSDK.dll")]
        public static extern bool NET_DVR_LockPanel(int lUserID);

        [DllImport(@"HCNetSDK.dll")]
        public static extern bool NET_DVR_UnLockPanel(int lUserID);

        [DllImport(@"HCNetSDK.dll")]
        public static extern bool NET_DVR_SetRtspConfig(int lUserID, uint dwCommand, ref NET_DVR_RTSPCFG lpInBuffer, uint dwInBufferSize);

        [DllImport(@"HCNetSDK.dll")]
        public static extern bool NET_DVR_GetRtspConfig(int lUserID, uint dwCommand, ref NET_DVR_RTSPCFG lpOutBuffer, uint dwOutBufferSize);


        //SDK_V222
        //ÖÇÄÜÉè±¸ÀàÐÍ
        public const int DS6001_HF_B = 60;//ÐÐÎª·ÖÎö£ºDS6001-HF/B
        public const int DS6001_HF_P = 61;//³µÅÆÊ¶±ð£ºDS6001-HF/P
        public const int DS6002_HF_B = 62;//Ë«»ú¸ú×Ù£ºDS6002-HF/B
        public const int DS6101_HF_B = 63;//ÐÐÎª·ÖÎö£ºDS6101-HF/B
        public const int IDS52XX = 64;//ÖÇÄÜ·ÖÎöÒÇIVMS
        public const int DS9000_IVS = 65;//9000ÏµÁÐÖÇÄÜDVR
        public const int DS8004_AHL_A = 66;//ÖÇÄÜATM, DS8004AHL-S/A
        public const int DS6101_HF_P = 67;//³µÅÆÊ¶±ð£ºDS6101-HF/P

        //ÄÜÁ¦»ñÈ¡ÃüÁî
        public const int VCA_DEV_ABILITY = 256;//Éè±¸ÖÇÄÜ·ÖÎöµÄ×ÜÄÜÁ¦
        public const int VCA_CHAN_ABILITY = 272;//ÐÐÎª·ÖÎöÄÜÁ¦
        public const int MATRIXDECODER_ABILITY = 512;//¶àÂ·½âÂëÆ÷ÏÔÊ¾¡¢½âÂëÄÜÁ¦
                                                     //»ñÈ¡/ÉèÖÃ´ó½Ó¿Ú²ÎÊýÅäÖÃÃüÁî
                                                     //³µÅÆÊ¶±ð£¨NET_VCA_PLATE_CFG£©
        public const int NET_DVR_SET_PLATECFG = 150;//ÉèÖÃ³µÅÆÊ¶±ð²ÎÊý
        public const int NET_DVR_GET_PLATECFG = 151;//»ñÈ¡³µÅÆÊ¶±ð²ÎÊý
                                                    //ÐÐÎª¶ÔÓ¦£¨NET_VCA_RULECFG£©
        public const int NET_DVR_SET_RULECFG = 152;//ÉèÖÃÐÐÎª·ÖÎö¹æÔò
        public const int NET_DVR_GET_RULECFG = 153;//»ñÈ¡ÐÐÎª·ÖÎö¹æÔò

        //Ë«ÉãÏñ»ú±ê¶¨²ÎÊý£¨NET_DVR_LF_CFG£©
        public const int NET_DVR_SET_LF_CFG = 160;//ÉèÖÃË«ÉãÏñ»úµÄÅäÖÃ²ÎÊý
        public const int NET_DVR_GET_LF_CFG = 161;//»ñÈ¡Ë«ÉãÏñ»úµÄÅäÖÃ²ÎÊý

        //ÖÇÄÜ·ÖÎöÒÇÈ¡Á÷ÅäÖÃ½á¹¹
        public const int NET_DVR_SET_IVMS_STREAMCFG = 162;//ÉèÖÃÖÇÄÜ·ÖÎöÒÇÈ¡Á÷²ÎÊý
        public const int NET_DVR_GET_IVMS_STREAMCFG = 163;//»ñÈ¡ÖÇÄÜ·ÖÎöÒÇÈ¡Á÷²ÎÊý

        //ÖÇÄÜ¿ØÖÆ²ÎÊý½á¹¹
        public const int NET_DVR_SET_VCA_CTRLCFG = 164;//ÉèÖÃÖÇÄÜ¿ØÖÆ²ÎÊý
        public const int NET_DVR_GET_VCA_CTRLCFG = 165;//»ñÈ¡ÖÇÄÜ¿ØÖÆ²ÎÊý

        //ÆÁ±ÎÇøÓòNET_VCA_MASK_REGION_LIST
        public const int NET_DVR_SET_VCA_MASK_REGION = 166;//ÉèÖÃÆÁ±ÎÇøÓò²ÎÊý
        public const int NET_DVR_GET_VCA_MASK_REGION = 167;//»ñÈ¡ÆÁ±ÎÇøÓò²ÎÊý

        //ATM½øÈëÇøÓò NET_VCA_ENTER_REGION
        public const int NET_DVR_SET_VCA_ENTER_REGION = 168;//ÉèÖÃ½øÈëÇøÓò²ÎÊý
        public const int NET_DVR_GET_VCA_ENTER_REGION = 169;//»ñÈ¡½øÈëÇøÓò²ÎÊý

        //±ê¶¨ÏßÅäÖÃNET_VCA_LINE_SEGMENT_LIST
        public const int NET_DVR_SET_VCA_LINE_SEGMENT = 170;//ÉèÖÃ±ê¶¨Ïß
        public const int NET_DVR_GET_VCA_LINE_SEGMENT = 171;//»ñÈ¡±ê¶¨Ïß

        // ivmsÆÁ±ÎÇøÓòNET_IVMS_MASK_REGION_LIST
        public const int NET_DVR_SET_IVMS_MASK_REGION = 172;//ÉèÖÃIVMSÆÁ±ÎÇøÓò²ÎÊý
        public const int NET_DVR_GET_IVMS_MASK_REGION = 173;//»ñÈ¡IVMSÆÁ±ÎÇøÓò²ÎÊý
                                                            // ivms½øÈë¼ì²âÇøÓòNET_IVMS_ENTER_REGION
        public const int NET_DVR_SET_IVMS_ENTER_REGION = 174;//ÉèÖÃIVMS½øÈëÇøÓò²ÎÊý
        public const int NET_DVR_GET_IVMS_ENTER_REGION = 175;//»ñÈ¡IVMS½øÈëÇøÓò²ÎÊý

        public const int NET_DVR_SET_IVMS_BEHAVIORCFG = 176;//ÉèÖÃÖÇÄÜ·ÖÎöÒÇÐÐÎª¹æÔò²ÎÊý
        public const int NET_DVR_GET_IVMS_BEHAVIORCFG = 177;//»ñÈ¡ÖÇÄÜ·ÖÎöÒÇÐÐÎª¹æÔò²ÎÊý

        // IVMS »Ø·Å¼ìË÷
        public const int NET_DVR_IVMS_SET_SEARCHCFG = 178;//ÉèÖÃIVMS»Ø·Å¼ìË÷²ÎÊý
        public const int NET_DVR_IVMS_GET_SEARCHCFG = 179;//»ñÈ¡IVMS»Ø·Å¼ìË÷²ÎÊý

        //±¨¾¯»Øµ÷ÃüÁî
        //¶ÔÓ¦NET_VCA_PLATE_RESULT
        public const int COMM_ALARM_PLATE = 4353;//³µÅÆÊ¶±ð±¨¾¯ÐÅÏ¢
                                                 //¶ÔÓ¦NET_VCA_RULE_ALARM
        public const int COMM_ALARM_RULE = 4354;//ÐÐÎª·ÖÎö±¨¾¯ÐÅÏ¢

        //½á¹¹²ÎÊýºê¶¨Òå 
        public const int VCA_MAX_POLYGON_POINT_NUM = 10;//¼ì²âÇøÓò×î¶àÖ§³Ö10¸öµãµÄ¶à±ßÐÎ
        public const int MAX_RULE_NUM = 8;//×î¶à¹æÔòÌõÊý
        public const int MAX_TARGET_NUM = 30;//×î¶àÄ¿±ê¸öÊý
        public const int MAX_CALIB_PT = 6;//×î´ó±ê¶¨µã¸öÊý
        public const int MIN_CALIB_PT = 4;//×îÐ¡±ê¶¨µã¸öÊý
        public const int MAX_TIMESEGMENT_2 = 2;//×î´óÊ±¼ä¶ÎÊý
        public const int MAX_LICENSE_LEN = 16;//³µÅÆºÅ×î´ó³¤¶È
        public const int MAX_PLATE_NUM = 3;//³µÅÆ¸öÊý
        public const int MAX_MASK_REGION_NUM = 4;//×î¶àËÄ¸öÆÁ±ÎÇøÓò
        public const int MAX_SEGMENT_NUM = 6;//ÉãÏñ»ú±ê¶¨×î´óÑù±¾ÏßÊýÄ¿
        public const int MIN_SEGMENT_NUM = 3;//ÉãÏñ»ú±ê¶¨×îÐ¡Ñù±¾ÏßÊýÄ¿
        public const int MAX_CATEGORY_LEN = 8;       //³µÅÆ¸½¼ÓÐÅÏ¢×î´ó×Ö·û

        //ÖÇÄÜ¿ØÖÆÐÅÏ¢
        public const int MAX_VCA_CHAN = 16;//×î´óÖÇÄÜÍ¨µÀÊý
        [StructLayoutAttribute(LayoutKind.Sequential)]
        public struct tagNET_VCA_CTRLINFO
        {
            public byte byVCAEnable;//ÊÇ·ñ¿ªÆôÖÇÄÜ
            public byte byVCAType;//ÖÇÄÜÄÜÁ¦ÀàÐÍ£¬VCA_CHAN_ABILITY_TYPE 
            public byte byStreamWithVCA;//ÂëÁ÷ÖÐÊÇ·ñ´øÖÇÄÜÐÅÏ¢
            public byte byMode;//Ä£Ê½£¬VCA_CHAN_MODE_TYPE ,atmÄÜÁ¦µÄÊ±ºòÐèÒªÅäÖÃ
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 4, ArraySubType = UnmanagedType.I1)]
            public byte[] byRes;//±£Áô£¬ÉèÖÃÎª0 
        }

        //ÖÇÄÜ¿ØÖÆÐÅÏ¢½á¹¹
        [StructLayoutAttribute(LayoutKind.Sequential)]
        public struct tagNET_VCA_CTRLCFG
        {
            public uint dwSize;
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = MAX_VCA_CHAN, ArraySubType = UnmanagedType.Struct)]
            public tagNET_VCA_CTRLINFO[] struCtrlInfo;//¿ØÖÆÐÅÏ¢,Êý×é0¶ÔÓ¦Éè±¸µÄÆðÊ¼Í¨µÀ
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 16, ArraySubType = UnmanagedType.I1)]
            public byte[] byRes;
        }

        //ÖÇÄÜÉè±¸ÄÜÁ¦¼¯
        [StructLayoutAttribute(LayoutKind.Sequential)]
        public struct tagNET_VCA_DEV_ABILITY
        {
            public uint dwSize;//½á¹¹³¤¶È
            public byte byVCAChanNum;//ÖÇÄÜÍ¨µÀ¸öÊý
            public byte byPlateChanNum;//³µÅÆÍ¨µÀ¸öÊý
            public byte byBBaseChanNum;//ÐÐÎª»ù±¾°æ¸öÊý
            public byte byBAdvanceChanNum;//ÐÐÎª¸ß¼¶°æ¸öÊý
            public byte byBFullChanNum;//ÐÐÎªÍêÕû°æ¸öÊý
            public byte byATMChanNum;//ÖÇÄÜATM¸öÊý
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 34, ArraySubType = UnmanagedType.I1)]
            public byte[] byRes;
        }

        //ÐÐÎª·ÖÎöÄÜÁ¦ÀàÐÍ
        public enum VCA_ABILITY_TYPE
        {
            TRAVERSE_PLANE_ABILITY = 1,//´©Ô½¾¯½äÃæ
            ENTER_AREA_ABILITY = 2,//½øÈëÇøÓò
            EXIT_AREA_ABILITY = 4,//Àë¿ªÇøÓò
            INTRUSION_ABILITY = 8,//ÈëÇÖ
            LOITER_ABILITY = 16,//ÅÇ»²
            LEFT_TAKE_ABILITY = 32,//¶ª°ü¼ñ°ü
            PARKING_ABILITY = 64,//Í£³µ
            RUN_ABILITY = 128,//±¼ÅÜ
            HIGH_DENSITY_ABILITY = 256,//ÄÚÈËÔ±ÃÜ¶È
            LF_TRACK_ABILITY = 512,//Ë«ÉãÏñ»ú¸ú×Ù
            STICK_UP_ABILITY = 1073741824,//ÌùÖ½Ìõ
            INSTALL_SCANNER_ABILITY = -2147483648,//°²×°¶Á¿¨Æ÷
        }

        //ÖÇÄÜÍ¨µÀÀàÐÍ
        public enum VCA_CHAN_ABILITY_TYPE
        {
            VCA_BEHAVIOR_BASE = 1,//ÐÐÎª·ÖÎö»ù±¾°æ
            VCA_BEHAVIOR_ADVANCE = 2,//ÐÐÎª·ÖÎö¸ß¼¶°æ
            VCA_BEHAVIOR_FULL = 3,//ÐÐÎª·ÖÎöÍêÕû°æ
            VCA_PLATE = 4,//³µÅÆÄÜÁ¦
            VCA_ATM = 5,//ATMÄÜÁ¦
        }

        //ÖÇÄÜATMÄ£Ê½ÀàÐÍ(ATMÄÜÁ¦ÌØÓÐ)
        public enum VCA_CHAN_MODE_TYPE
        {
            VCA_ATM_PANEL = 0,//ATMÃæ°å
            VCA_ATM_SURROUND = 1,//ATM»·¾³
        }

        //Í¨µÀÄÜÁ¦ÊäÈë²ÎÊý
        [StructLayoutAttribute(LayoutKind.Sequential)]
        public struct tagNET_VCA_CHAN_IN_PARAM
        {
            public byte byVCAType;//VCA_CHAN_ABILITY_TYPEÃ¶¾ÙÖµ
            public byte byMode;//Ä£Ê½£¬VCA_CHAN_MODE_TYPE ,atmÄÜÁ¦µÄÊ±ºòÐèÒªÅäÖÃ
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 2, ArraySubType = UnmanagedType.I1)]
            public byte[] byRes;//±£Áô£¬ÉèÖÃÎª0 
        }

        //ÐÐÎªÄÜÁ¦¼¯½á¹¹
        [StructLayoutAttribute(LayoutKind.Sequential)]
        public struct tagNET_VCA_BEHAVIOR_ABILITY
        {
            public uint dwSize;//½á¹¹³¤¶È
            public uint dwAbilityType;//Ö§³ÖµÄÄÜÁ¦ÀàÐÍ£¬°´Î»±íÊ¾£¬¼ûVCA_ABILITY_TYPE¶¨Òå
            public byte byMaxRuleNum;//×î´ó¹æÔòÊý
            public byte byMaxTargetNum;//×î´óÄ¿±êÊý
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 10, ArraySubType = UnmanagedType.I1)]
            public byte[] byRes; //±£Áô£¬ÉèÖÃÎª0
        }

        /*********************************************************
        Function:	NET_DVR_GetDeviceAbility
        Desc:  
        Input:	
        Output:	
        Return:	TRUE±íÊ¾³É¹¦£¬FALSE±íÊ¾Ê§°Ü¡£
        **********************************************************/
        [DllImport(@"HCNetSDK.dll")]
        public static extern bool NET_DVR_GetDeviceAbility(int lUserID, uint dwAbilityType, IntPtr pInBuf, uint dwInLength, IntPtr pOutBuf, uint dwOutLength);


        //ÖÇÄÜ¹²ÓÃ½á¹¹
        //×ø±êÖµ¹éÒ»»¯,¸¡µãÊýÖµÎªµ±Ç°»­ÃæµÄ°Ù·Ö±È´óÐ¡, ¾«¶ÈÎªÐ¡ÊýµãºóÈýÎ»
        //µã×ø±ê½á¹¹
        [StructLayoutAttribute(LayoutKind.Sequential)]
        public struct tagNET_VCA_POINT
        {
            public float fX;// XÖá×ø±ê, 0.001~1
            public float fY;//YÖá×ø±ê, 0.001~1
        }

        public const int DRAG_PTZ = 51;//ÍÏ¶¯PTZ
        public const int NET_DVR_FISHEYE_CFG = 3244; //ÓãÑÛ³¤Á¬½ÓÅäÖÃ

        [StructLayoutAttribute(LayoutKind.Sequential)]
        public struct NET_DVR_DRAG_POS_PARAM
        {
            public uint dwChannel;  //Éè±¸Í¨µÀºÅ
            public uint dwPtzChannel;  //dwChannelËùÖ¸Í¨µÀÄ£Ê½ÎªFISHEYE_STREAM_MODE_FISHEYE£¨ÓãÑÛÄ£Ê½Ê±£©£¬´ËÖµÎªÆäÍÏ¶¯ËùÁª¶¯µÄptzÍ¨µÀ£»dwChannelÍ¨µÀÄ£Ê½Îª·ÇÓãÑÛÄ£Ê½Ê±£¬ÖÃÎª0¼´¿É
            public tagNET_VCA_POINT struToPoint;  //ÍÏ¶¯»­ÃæÒªÌø×ªµ½µÄÄ¿±êµã£¬Ä¿±êµãÎ»ÖÃÏà¶ÔÓÚÔ¤ÀÀ»­ÃæµÄ×óÉÏ½Ç
            public tagNET_VCA_POINT struOriPoint;  //ÍÏ¶¯²Ù×÷ÆðÊ¼µã£¬´ËµãÎªµ±´ÎÍÏ¶¯²Ù×÷¿ªÊ¼Ê±£¬Êó±êÖ¸ÕëÏà¶ÔÓÚÔ¤ÀÀ»­Ãæ×óÉÏ½ÇµÄÎ»ÖÃ 
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 56, ArraySubType = UnmanagedType.I1)]
            public byte[] byRes; //±£Áô£¬ÉèÖÃÎª0
        }
        public enum FISHEYE_STREAM_OUTPUT_MODE
        {
            FISHEYE_STREAM_MODE_FISHEYE = 1,
            FISHEYE_STREAM_MODE_PTZ = 2,
            FISHEYE_STREAM_MODE_PANORAMA = 3
        }

        public enum CALLBACK_TYPE_DATA_ENUM
        {
            ENUM_FISHEYE_STREAM_STATUS = 1,  //ÓãÑÛÂëÁ÷Êä³ö×´Ì¬
            ENUM_FISHEYE_PTZPOS = 2,  //ptzÍ¨µÀµ±Ç°Ëù´¦ÓãÑÛÄ£Ê½ÏÂµÄ×ø±ê
            ENUM_FISHEYE_REALTIME_OUTPUT = 3   //ÊµÊ±Êä³öÄ£Ê½
        }

        //ÓãÑÛÂëÁ÷×´Ì¬
        [StructLayoutAttribute(LayoutKind.Sequential)]
        public struct NET_DVR_FISHEYE_STREAM_STATUS
        {
            public uint dwSize;
            public byte byStreamMode;    //ÂëÁ÷Êä³öÄ£Ê½£¬²Î¼ûFISHEYE_STREAM_OUTPUT_MODE
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 63, ArraySubType = UnmanagedType.I1)]
            public byte[] byRes; //±£Áô£¬ÉèÖÃÎª0
        }

        //³¤Á¬½Ó»Øµ÷Êý¾Ý½á¹¹Ìå
        [StructLayoutAttribute(LayoutKind.Sequential)]
        public struct NET_DVR_CALLBACK_TYPE_DATA
        {
            public uint dwChannel;      //Í¨µÀºÅ
            public uint dwDataType;     //²Î¼ûCALLBACK_TYPE_DATA_ENUM
            public uint dwDataLen;      //Êý¾Ý³¤¶È
            public IntPtr pData;          //Êý¾Ý£¬µ±dwTypeDataÎª ENUM_FISHEYE_STREAM_STATUS£¬Æä¶ÔÓ¦ÎªNET_DVR_FISHEYE_STREAM_STATUS
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 64, ArraySubType = UnmanagedType.I1)]
            public byte[] byRes; //±£Áô£¬ÉèÖÃÎª0
        }

        //ÇøÓò¿ò½á¹¹
        [StructLayoutAttribute(LayoutKind.Sequential)]
        public struct NET_VCA_RECT
        {
            public float fX;//±ß½ç¿ò×óÉÏ½ÇµãµÄXÖá×ø±ê, 0.001~1
            public float fY;//±ß½ç¿ò×óÉÏ½ÇµãµÄYÖá×ø±ê, 0.001~1
            public float fWidth;//±ß½ç¿òµÄ¿í¶È, 0.001~1
            public float fHeight;//±ß½ç¿òµÄ¸ß¶È, 0.001~1
        }

        //ÐÐÎª·ÖÎöÊÂ¼þÀàÐÍ
        public enum VCA_EVENT_TYPE
        {
            VCA_TRAVERSE_PLANE = 1,//´©Ô½¾¯½äÃæ
            VCA_ENTER_AREA = 2,//Ä¿±ê½øÈëÇøÓò,Ö§³ÖÇøÓò¹æÔò
            VCA_EXIT_AREA = 4,//Ä¿±êÀë¿ªÇøÓò,Ö§³ÖÇøÓò¹æÔò
            VCA_INTRUSION = 8,//ÖÜ½çÈëÇÖ,Ö§³ÖÇøÓò¹æÔò
            VCA_LOITER = 16,//ÅÇ»²,Ö§³ÖÇøÓò¹æÔò
            VCA_LEFT_TAKE = 32,//¶ª°ü¼ñ°ü,Ö§³ÖÇøÓò¹æÔò
            VCA_PARKING = 64,//Í£³µ,Ö§³ÖÇøÓò¹æÔò
            VCA_RUN = 128,//±¼ÅÜ,Ö§³ÖÇøÓò¹æÔò
            VCA_HIGH_DENSITY = 256,//ÇøÓòÄÚÈËÔ±ÃÜ¶È,Ö§³ÖÇøÓò¹æÔò
            VCA_STICK_UP = 1073741824,//ÌùÖ½Ìõ,Ö§³ÖÇøÓò¹æÔò
            VCA_INSTALL_SCANNER = -2147483648,//°²×°¶Á¿¨Æ÷,Ö§³ÖÇøÓò¹æÔò
        }

        //¾¯½äÃæ´©Ô½·½ÏòÀàÐÍ
        public enum VCA_CROSS_DIRECTION
        {
            VCA_BOTH_DIRECTION,// Ë«Ïò 
            VCA_LEFT_GO_RIGHT,// ÓÉ×óÖÁÓÒ 
            VCA_RIGHT_GO_LEFT,// ÓÉÓÒÖÁ×ó 
        }

        //Ïß½á¹¹
        [StructLayoutAttribute(LayoutKind.Sequential)]
        public struct tagNET_VCA_LINE
        {
            public tagNET_VCA_POINT struStart;//Æðµã 
            public tagNET_VCA_POINT struEnd; //ÖÕµã

            //             public void init()
            //             {
            //                 struStart = new tagNET_VCA_POINT();
            //                 struEnd = new tagNET_VCA_POINT();
            //             }
        }

        //¸Ã½á¹¹»áµ¼ÖÂxaml½çÃæ³ö²»À´£¡£¡£¡£¡£¡£¡£¡£¡£¡£¿£¿ÎÊÌâÔÝÊ±»¹Ã»ÓÐÕÒµ½  
        //ÔÝÊ±ÆÁ±Î½á¹¹ÏÈ
        //¶à±ßÐÍ½á¹¹Ìå
        [StructLayoutAttribute(LayoutKind.Sequential)]
        public struct tagNET_VCA_POLYGON
        {
            /// DWORD->unsigned int
            public uint dwPointNum;

            /// NET_VCA_POINT[10]
            //             [System.Runtime.InteropServices.MarshalAsAttribute(System.Runtime.InteropServices.UnmanagedType.ByValArray, SizeConst = 10, ArraySubType = System.Runtime.InteropServices.UnmanagedType.Struct)]
            //             public tagNET_VCA_POINT[] struPos;

        }

        [StructLayoutAttribute(LayoutKind.Sequential)]
        public struct tagNET_VCA_TRAVERSE_PLANE
        {
            public tagNET_VCA_LINE struPlaneBottom;//¾¯½äÃæµ×±ß
            public VCA_CROSS_DIRECTION dwCrossDirection;//´©Ô½·½Ïò: 0-Ë«Ïò£¬1-´Ó×óµ½ÓÒ£¬2-´ÓÓÒµ½×ó
            public byte byRes1;//±£Áô
            public byte byPlaneHeight;//¾¯½äÃæ¸ß¶È
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 38, ArraySubType = UnmanagedType.I1)]
            public byte[] byRes2;

            //             public void init()
            //             {
            //                 struPlaneBottom = new tagNET_VCA_LINE();
            //                 struPlaneBottom.init();
            //                 byRes2 = new byte[38];
            //             }
        }

        //½øÈë/Àë¿ªÇøÓò²ÎÊý
        [StructLayoutAttribute(LayoutKind.Sequential)]
        public struct tagNET_VCA_AREA
        {
            public tagNET_VCA_POLYGON struRegion;//ÇøÓò·¶Î§
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 8, ArraySubType = UnmanagedType.I1)]
            public byte[] byRes;
        }

        //¸ù¾Ý±¨¾¯ÑÓ³ÙÊ±¼äÀ´±êÊ¶±¨¾¯ÖÐ´øÍ¼Æ¬£¬±¨¾¯¼ä¸ôºÍIO±¨¾¯Ò»ÖÂ£¬1Ãë·¢ËÍÒ»¸ö¡£
        //ÈëÇÖ²ÎÊý
        [StructLayoutAttribute(LayoutKind.Sequential)]
        public struct tagNET_VCA_INTRUSION
        {
            public tagNET_VCA_POLYGON struRegion;//ÇøÓò·¶Î§
            public ushort wDuration;//±¨¾¯ÑÓ³ÙÊ±¼ä: 1-120Ãë£¬½¨Òé5Ãë£¬ÅÐ¶ÏÊÇÓÐÐ§±¨¾¯µÄÊ±¼ä
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 6, ArraySubType = UnmanagedType.I1)]
            public byte[] byRes;
        }

        //ÅÇ»²²ÎÊý
        [StructLayoutAttribute(LayoutKind.Sequential)]
        public struct tagNET_VCA_PARAM_LOITER
        {
            public tagNET_VCA_POLYGON struRegion;//ÇøÓò·¶Î§
            public ushort wDuration;//´¥·¢ÅÇ»²±¨¾¯µÄ³ÖÐøÊ±¼ä£º1-120Ãë£¬½¨Òé10Ãë
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 6, ArraySubType = UnmanagedType.I1)]
            public byte[] byRes;
        }

        //¶ª°ü/¼ñ°ü²ÎÊý
        [StructLayoutAttribute(LayoutKind.Sequential)]
        public struct tagNET_VCA_TAKE_LEFT
        {
            public tagNET_VCA_POLYGON struRegion;//ÇøÓò·¶Î§
            public ushort wDuration;//´¥·¢¶ª°ü/¼ñ°ü±¨¾¯µÄ³ÖÐøÊ±¼ä£º1-120Ãë£¬½¨Òé10Ãë
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 6, ArraySubType = UnmanagedType.I1)]
            public byte[] byRes;
        }

        //Í£³µ²ÎÊý
        [StructLayoutAttribute(LayoutKind.Sequential)]
        public struct tagNET_VCA_PARKING
        {
            public tagNET_VCA_POLYGON struRegion;//ÇøÓò·¶Î§
            public ushort wDuration;//´¥·¢Í£³µ±¨¾¯³ÖÐøÊ±¼ä£º1-120Ãë£¬½¨Òé10Ãë
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 6, ArraySubType = UnmanagedType.I1)]
            public byte[] byRes;
        }

        //±¼ÅÜ²ÎÊý
        [StructLayoutAttribute(LayoutKind.Sequential)]
        public struct tagNET_VCA_RUN
        {
            public tagNET_VCA_POLYGON struRegion;//ÇøÓò·¶Î§
            public float fRunDistance;//ÈË±¼ÅÜ×î´ó¾àÀë, ·¶Î§: [0.1, 1.00]
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 4, ArraySubType = UnmanagedType.I1)]
            public byte[] byRes;
        }

        //ÈËÔ±¾Û¼¯²ÎÊý
        [StructLayoutAttribute(LayoutKind.Sequential)]
        public struct tagNET_VCA_HIGH_DENSITY
        {
            public tagNET_VCA_POLYGON struRegion;//ÇøÓò·¶Î§
            public float fDensity;//ÃÜ¶È±ÈÂÊ, ·¶Î§: [0.1, 1.0]
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 4, ArraySubType = UnmanagedType.I1)]
            public byte[] byRes;
        }

        //ÌùÖ½Ìõ²ÎÊý
        [StructLayoutAttribute(LayoutKind.Sequential)]
        public struct tagNET_VCA_STICK_UP
        {
            public tagNET_VCA_POLYGON struRegion;//ÇøÓò·¶Î§
            public ushort wDuration;//±¨¾¯³ÖÐøÊ±¼ä£º10-60Ãë£¬½¨Òé10Ãë
            public byte bySensitivity;//ÁéÃô¶È²ÎÊý£¬·¶Î§[1,5]
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 5, ArraySubType = UnmanagedType.I1)]
            public byte[] byRes;
        }

        //¶Á¿¨Æ÷²ÎÊý
        [StructLayoutAttribute(LayoutKind.Sequential)]
        public struct tagNET_VCA_SCANNER
        {
            public tagNET_VCA_POLYGON struRegion;//ÇøÓò·¶Î§
            public ushort wDuration;//¶Á¿¨³ÖÐøÊ±¼ä£º10-60Ãë
            public byte bySensitivity;//ÁéÃô¶È²ÎÊý£¬·¶Î§[1,5]
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 5, ArraySubType = UnmanagedType.I1)]
            public byte[] byRes;
        }

        //¾¯½äÊÂ¼þ²ÎÊý
        [StructLayoutAttribute(LayoutKind.Explicit)]
        public struct tagNET_VCA_EVENT_UNION
        {
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 23, ArraySubType = UnmanagedType.U4)]
            [FieldOffsetAttribute(0)]
            public uint[] uLen;//²ÎÊý
            [FieldOffsetAttribute(0)]
            public tagNET_VCA_TRAVERSE_PLANE struTraversePlane;//´©Ô½¾¯½äÃæ²ÎÊý 
            [FieldOffsetAttribute(0)]
            public tagNET_VCA_AREA struArea;//½øÈë/Àë¿ªÇøÓò²ÎÊý
            [FieldOffsetAttribute(0)]
            public tagNET_VCA_INTRUSION struIntrusion;//ÈëÇÖ²ÎÊý
            [FieldOffsetAttribute(0)]
            public tagNET_VCA_PARAM_LOITER struLoiter;//ÅÇ»²²ÎÊý
            [FieldOffsetAttribute(0)]
            public tagNET_VCA_TAKE_LEFT struTakeTeft;//¶ª°ü/¼ñ°ü²ÎÊý
            [FieldOffsetAttribute(0)]
            public tagNET_VCA_PARKING struParking;//Í£³µ²ÎÊý
            [FieldOffsetAttribute(0)]
            public tagNET_VCA_RUN struRun;//±¼ÅÜ²ÎÊý
            [FieldOffsetAttribute(0)]
            public tagNET_VCA_HIGH_DENSITY struHighDensity;//ÈËÔ±¾Û¼¯²ÎÊý  
            [FieldOffsetAttribute(0)]
            public tagNET_VCA_STICK_UP struStickUp;//ÌùÖ½Ìõ
            [FieldOffsetAttribute(0)]
            public tagNET_VCA_SCANNER struScanner;//¶Á¿¨Æ÷²ÎÊý 

            //             public void init()
            //             {
            //                 uLen = new uint[23];
            //                 struTraversePlane = new tagNET_VCA_TRAVERSE_PLANE();
            //                 struTraversePlane.init();
            //                 struArea = new tagNET_VCA_AREA();
            //                 struArea.init();
            //                 struIntrusion = new tagNET_VCA_INTRUSION();
            //                 struIntrusion.init();
            //                 struLoiter = new tagNET_VCA_PARAM_LOITER();
            //                 struLoiter.init();
            //                 struTakeTeft = new tagNET_VCA_TAKE_LEFT();
            //                 struTakeTeft.init();
            //                 struParking = new tagNET_VCA_PARKING();
            //                 struParking.init();
            //                 struRun = new tagNET_VCA_RUN();
            //                 struRun.init();
            //                 struHighDensity = new tagNET_VCA_HIGH_DENSITY();
            //                 struHighDensity.init();
            //                 struStickUp = new tagNET_VCA_STICK_UP();
            //                 struStickUp.init();
            //                 struScanner = new tagNET_VCA_SCANNER();
            //                 struScanner.init();
            //             }
        }

        // ³ß´ç¹ýÂËÆ÷ÀàÐÍ
        public enum SIZE_FILTER_MODE
        {
            IMAGE_PIX_MODE,//¸ù¾ÝÏñËØ´óÐ¡ÉèÖÃ
            REAL_WORLD_MODE,//¸ù¾ÝÊµ¼Ê´óÐ¡ÉèÖÃ
        }

        //³ß´ç¹ýÂËÆ÷
        [StructLayoutAttribute(LayoutKind.Sequential)]
        public struct tagNET_VCA_SIZE_FILTER
        {
            public byte byActive;//ÊÇ·ñ¼¤»î³ß´ç¹ýÂËÆ÷ 0-·ñ ·Ç0-ÊÇ
            public byte byMode;//¹ýÂËÆ÷Ä£Ê½SIZE_FILTER_MODE
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 2, ArraySubType = UnmanagedType.I1)]
            public byte[] byRes;//±£Áô£¬ÖÃ0
            public NET_VCA_RECT struMiniRect;//×îÐ¡Ä¿±ê¿ò,È«0±íÊ¾²»ÉèÖÃ
            public NET_VCA_RECT struMaxRect;//×î´óÄ¿±ê¿ò,È«0±íÊ¾²»ÉèÖÃ
        }

        //¾¯½ä¹æÔò½á¹¹
        [StructLayoutAttribute(LayoutKind.Sequential)]
        public struct tagNET_VCA_ONE_RULE
        {
            public byte byActive;//ÊÇ·ñ¼¤»î¹æÔò,0-·ñ,·Ç0-ÊÇ
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 7, ArraySubType = UnmanagedType.I1)]
            public byte[] byRes;//±£Áô£¬ÉèÖÃÎª0×Ö¶Î
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = NAME_LEN, ArraySubType = UnmanagedType.I1)]
            public byte[] byRuleName;//¹æÔòÃû³Æ
            public VCA_EVENT_TYPE dwEventType;//ÐÐÎª·ÖÎöÊÂ¼þÀàÐÍ
            public tagNET_VCA_EVENT_UNION uEventParam;//ÐÐÎª·ÖÎöÊÂ¼þ²ÎÊý
            public tagNET_VCA_SIZE_FILTER struSizeFilter;//³ß´ç¹ýÂËÆ÷
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = MAX_DAYS * MAX_TIMESEGMENT_2, ArraySubType = UnmanagedType.Struct)]
            public NET_DVR_SCHEDTIME[] struAlarmTime;//²¼·ÀÊ±¼ä
            public NET_DVR_HANDLEEXCEPTION_V30 struHandleType;//´¦Àí·½Ê½ 
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = MAX_CHANNUM_V30, ArraySubType = UnmanagedType.I1)]
            public byte[] byRelRecordChan;//±¨¾¯´¥·¢µÄÂ¼ÏóÍ¨µÀ,Îª1±íÊ¾´¥·¢¸ÃÍ¨µÀ
        }

        //ÐÐÎª·ÖÎöÅäÖÃ½á¹¹Ìå
        [StructLayoutAttribute(LayoutKind.Sequential)]
        public struct tagNET_VCA_RULECFG
        {
            public uint dwSize;//½á¹¹³¤¶È
            public byte byPicProType;//±¨¾¯Ê±Í¼Æ¬´¦Àí·½Ê½ 0-²»´¦Àí ·Ç0-ÉÏ´«
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 3, ArraySubType = UnmanagedType.I1)]
            public byte[] byRes;
            public NET_DVR_JPEGPARA struPictureParam;//Í¼Æ¬¹æ¸ñ½á¹¹
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = MAX_RULE_NUM, ArraySubType = UnmanagedType.Struct)]
            public tagNET_VCA_ONE_RULE[] struRule;//¹æÔòÊý×é
        }

        //¼ò»¯Ä¿±ê½á¹¹Ìå
        [StructLayoutAttribute(LayoutKind.Sequential)]
        public struct tagNET_VCA_TARGET_INFO
        {
            public uint dwID;//Ä¿±êID ,ÈËÔ±ÃÜ¶È¹ý¸ß±¨¾¯Ê±Îª0
            public NET_VCA_RECT struRect; //Ä¿±ê±ß½ç¿ò 
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 4, ArraySubType = UnmanagedType.I1)]
            public byte[] byRes;//±£Áô
        }

        //¼ò»¯µÄ¹æÔòÐÅÏ¢, °üº¬¹æÔòµÄ»ù±¾ÐÅÏ¢
        [StructLayoutAttribute(LayoutKind.Sequential)]
        public struct tagNET_VCA_RULE_INFO
        {
            public byte byRuleID;//¹æÔòID,0-7
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 3, ArraySubType = UnmanagedType.I1)]
            public byte[] byRes;//±£Áô
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = NAME_LEN, ArraySubType = UnmanagedType.I1)]
            public byte[] byRuleName;//¹æÔòÃû³Æ
            public VCA_EVENT_TYPE dwEventType;//¾¯½äÊÂ¼þÀàÐÍ
            public tagNET_VCA_EVENT_UNION uEventParam;//ÊÂ¼þ²ÎÊý
        }

        //Ç°¶ËÉè±¸µØÖ·ÐÅÏ¢£¬ÖÇÄÜ·ÖÎöÒÇ±íÊ¾µÄÊÇÇ°¶ËÉè±¸µÄµØÖ·ÐÅÏ¢£¬ÆäËûÉè±¸±íÊ¾±¾»úµÄµØÖ·
        [StructLayoutAttribute(LayoutKind.Sequential)]
        public struct tagNET_VCA_DEV_INFO
        {
            public NET_DVR_IPADDR struDevIP;//Ç°¶ËÉè±¸µØÖ·£¬
            public ushort wPort;//Ç°¶ËÉè±¸¶Ë¿ÚºÅ£¬ 
            public byte byChannel;//Ç°¶ËÉè±¸Í¨µÀ£¬
            public byte byRes;// ±£Áô×Ö½Ú
        }

        //ÐÐÎª·ÖÎö½á¹ûÉÏ±¨½á¹¹
        [StructLayoutAttribute(LayoutKind.Sequential)]
        public struct tagNET_VCA_RULE_ALARM
        {
            public uint dwSize;//½á¹¹³¤¶È
            public uint dwRelativeTime;//Ïà¶ÔÊ±±ê
            public uint dwAbsTime;//¾ø¶ÔÊ±±ê
            public tagNET_VCA_RULE_INFO struRuleInfo;//ÊÂ¼þ¹æÔòÐÅÏ¢
            public tagNET_VCA_TARGET_INFO struTargetInfo;//±¨¾¯Ä¿±êÐÅÏ¢
            public tagNET_VCA_DEV_INFO struDevInfo;//Ç°¶ËÉè±¸ÐÅÏ¢
            public uint dwPicDataLen;//·µ»ØÍ¼Æ¬µÄ³¤¶È Îª0±íÊ¾Ã»ÓÐÍ¼Æ¬£¬´óÓÚ0±íÊ¾¸Ã½á¹¹ºóÃæ½ô¸úÍ¼Æ¬Êý¾Ý*/
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 4, ArraySubType = UnmanagedType.U4)]
            public uint[] dwRes;//±£Áô£¬ÉèÖÃÎª0
            public IntPtr pImage;//Ö¸ÏòÍ¼Æ¬µÄÖ¸Õë
        }

        //²ÎÊý¹Ø¼ü×Ö
        public enum IVS_PARAM_KEY
        {
            OBJECT_DETECT_SENSITIVE = 1,//Ä¿±ê¼ì²âÁéÃô¶È
            BACKGROUND_UPDATE_RATE = 2,//±³¾°¸üÐÂËÙ¶È
            SCENE_CHANGE_RATIO = 3,//³¡¾°±ä»¯¼ì²â×îÐ¡Öµ
            SUPPRESS_LAMP = 4,//ÊÇ·ñÒÖÖÆ³µÍ·µÆ
            MIN_OBJECT_SIZE = 5,//ÄÜ¼ì²â³öµÄ×îÐ¡Ä¿±ê´óÐ¡
            OBJECT_GENERATE_RATE = 6,//Ä¿±êÉú³ÉËÙ¶È
            MISSING_OBJECT_HOLD = 7,//Ä¿±êÏûÊ§ºó¼ÌÐø¸ú×ÙÊ±¼ä
            MAX_MISSING_DISTANCE = 8,//Ä¿±êÏûÊ§ºó¼ÌÐø¸ú×Ù¾àÀë
            OBJECT_MERGE_SPEED = 9,//¶à¸öÄ¿±ê½»´íÊ±£¬Ä¿±êµÄÈÚºÏËÙ¶È
            REPEATED_MOTION_SUPPRESS = 10,//ÖØ¸´ÔË¶¯ÒÖÖÆ
            ILLUMINATION_CHANGE = 11,//¹âÓ°±ä»¯ÒÖÖÆ¿ª¹Ø
            TRACK_OUTPUT_MODE = 12,//¹ì¼£Êä³öÄ£Ê½£º0-Êä³öÄ¿±êµÄÖÐÐÄ£¬1-Êä³öÄ¿±êµÄµ×²¿ÖÐÐÄ
            ENTER_CHANGE_HOLD = 13,//¼ì²âÇøÓò±ä»¯ãÐÖµ
            RESUME_DEFAULT_PARAM = 255,//»Ö¸´Ä¬ÈÏ¹Ø¼ü×Ö²ÎÊý
        }

        //ÉèÖÃ/»ñÈ¡²ÎÊý¹Ø¼ü×Ö
        [DllImport(@"HCNetSDK.dll")]
        public static extern bool NET_DVR_SetBehaviorParamKey(int lUserID, int lChannel, uint dwParameterKey, int nValue);

        [DllImport(@"HCNetSDK.dll")]
        public static extern bool NET_DVR_GetBehaviorParamKey(int lUserID, int lChannel, uint dwParameterKey, ref int pValue);

        //ÐÐÎª·ÖÎö¹æÔòDSPÐÅÏ¢µþ¼Ó½á¹¹
        [StructLayoutAttribute(LayoutKind.Sequential)]
        public struct tagNET_VCA_DRAW_MODE
        {
            public uint dwSize;
            public byte byDspAddTarget;//±àÂëÊÇ·ñµþ¼ÓÄ¿±ê
            public byte byDspAddRule;//±àÂëÊÇ·ñµþ¼Ó¹æÔò
            public byte byDspPicAddTarget;//×¥Í¼ÊÇ·ñµþ¼ÓÄ¿±ê
            public byte byDspPicAddRule;//×¥Í¼ÊÇ·ñµþ¼Ó¹æÔò
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 4, ArraySubType = UnmanagedType.I1)]
            public byte[] byRes;
        }


        //»ñÈ¡/ÉèÖÃÐÐÎª·ÖÎöÄ¿±êµþ¼Ó½Ó¿Ú
        [DllImport(@"HCNetSDK.dll")]
        public static extern bool NET_DVR_GetVCADrawMode(int lUserID, int lChannel, ref tagNET_VCA_DRAW_MODE lpDrawMode);

        [DllImport(@"HCNetSDK.dll")]
        public static extern bool NET_DVR_SetVCADrawMode(int lUserID, int lChannel, ref tagNET_VCA_DRAW_MODE lpDrawMode);

        //±ê¶¨µã×Ó½á¹¹
        [StructLayoutAttribute(LayoutKind.Sequential)]
        public struct tagNET_DVR_CB_POINT
        {
            public tagNET_VCA_POINT struPoint;//±ê¶¨µã£¬Ö÷ÉãÏñ»ú£¨Ç¹»ú£©
            public NET_DVR_PTZPOS struPtzPos;//Çò»úÊäÈëµÄPTZ×ø±ê
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 8, ArraySubType = UnmanagedType.I1)]
            public byte[] byRes;
        }

        //±ê¶¨²ÎÊýÅäÖÃ½á¹¹
        [StructLayoutAttribute(LayoutKind.Sequential)]
        public struct tagNET_DVR_LF_CALIBRATION_PARAM
        {
            public byte byPointNum;//ÓÐÐ§±ê¶¨µã¸öÊý
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 3, ArraySubType = UnmanagedType.I1)]
            public byte[] byRes;
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = MAX_CALIB_PT, ArraySubType = UnmanagedType.Struct)]
            public tagNET_DVR_CB_POINT[] struCBPoint;//±ê¶¨µã×é
        }

        //LFË«ÉãÏñ»úÅäÖÃ½á¹¹
        [StructLayoutAttribute(LayoutKind.Sequential)]
        public struct tagNET_DVR_LF_CFG
        {
            public uint dwSize;//½á¹¹³¤¶È	
            public byte byEnable;//±ê¶¨Ê¹ÄÜ
            public byte byFollowChan;// ±»¿ØÖÆµÄ´ÓÍ¨µÀ
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 2, ArraySubType = UnmanagedType.I1)]
            public byte[] byRes;
            public tagNET_DVR_LF_CALIBRATION_PARAM struCalParam;//±ê¶¨µã×é
        }

        //L/F¸ú×ÙÄ£Ê½
        public enum TRACK_MODE
        {
            MANUAL_CTRL = 0,//ÊÖ¶¯¸ú×Ù
            ALARM_TRACK,//±¨¾¯´¥·¢¸ú×Ù
            TARGET_TRACK,//Ä¿±ê¸ú×Ù
        }

        //L/FÊÖ¶¯¿ØÖÆ½á¹¹
        [StructLayoutAttribute(LayoutKind.Sequential)]
        public struct tagNET_DVR_LF_MANUAL_CTRL_INFO
        {
            public tagNET_VCA_POINT struCtrlPoint;
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 8, ArraySubType = UnmanagedType.I1)]
            public byte[] byRes;
        }

        //L/FÄ¿±ê¸ú×Ù½á¹¹
        [StructLayoutAttribute(LayoutKind.Sequential)]
        public struct tagNET_DVR_LF_TRACK_TARGET_INFO
        {
            public uint dwTargetID;
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 8, ArraySubType = UnmanagedType.I1)]
            public byte[] byRes;
        }

        [StructLayoutAttribute(LayoutKind.Sequential)]
        public struct tagNET_DVR_LF_TRACK_MODE
        {
            public uint dwSize;//½á¹¹³¤¶È
            public byte byTrackMode;//¸ú×ÙÄ£Ê½
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 3, ArraySubType = UnmanagedType.I1)]
            public byte[] byRes;//±£Áô£¬ÖÃ0
            [StructLayoutAttribute(LayoutKind.Explicit)]
            public struct uModeParam
            {
                [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 4, ArraySubType = UnmanagedType.U4)]
                [FieldOffsetAttribute(0)]
                public uint[] dwULen;
                [FieldOffsetAttribute(0)]
                public tagNET_DVR_LF_MANUAL_CTRL_INFO struManualCtrl;//ÊÖ¶¯¸ú×Ù½á¹¹
                [FieldOffsetAttribute(0)]
                public tagNET_DVR_LF_TRACK_TARGET_INFO struTargetTrack;//Ä¿±ê¸ú×Ù½á¹¹
            }
        }

        //Ë«ÉãÏñ»ú¸ú×ÙÄ£Ê½ÉèÖÃ½Ó¿Ú
        [DllImport(@"HCNetSDK.dll")]
        public static extern bool NET_DVR_SetLFTrackMode(int lUserID, int lChannel, ref tagNET_DVR_LF_TRACK_MODE lpTrackMode);

        [DllImport(@"HCNetSDK.dll")]
        public static extern bool NET_DVR_GetLFTrackMode(int lUserID, int lChannel, ref tagNET_DVR_LF_TRACK_MODE lpTrackMode);

        //Ê¶±ð³¡¾°
        public enum VCA_RECOGNIZE_SCENE
        {
            VCA_LOW_SPEED_SCENE = 0,//µÍËÙÍ¨¹ý³¡¾°£¨ÊÕ·ÑÕ¾¡¢Ð¡ÇøÃÅ¿Ú¡¢Í£³µ³¡£©
            VCA_HIGH_SPEED_SCENE = 1,//¸ßËÙÍ¨¹ý³¡¾°£¨¿¨¿Ú¡¢¸ßËÙ¹«Â·¡¢ÒÆ¶¯»ü²é)
            VCA_MOBILE_CAMERA_SCENE = 2,//ÒÆ¶¯ÉãÏñ»úÓ¦ÓÃ£© 
        }

        //Ê¶±ð½á¹û±êÖ¾
        public enum VCA_RECOGNIZE_RESULT
        {
            VCA_RECOGNIZE_FAILURE = 0,//Ê¶±ðÊ§°Ü
            VCA_IMAGE_RECOGNIZE_SUCCESS,//Í¼ÏñÊ¶±ð³É¹¦
            VCA_VIDEO_RECOGNIZE_SUCCESS_OF_BEST_LICENSE,//ÊÓÆµÊ¶±ð¸üÓÅ½á¹û
            VCA_VIDEO_RECOGNIZE_SUCCESS_OF_NEW_LICENSE,//ÊÓÆµÊ¶±ðµ½ÐÂµÄ³µÅÆ
            VCA_VIDEO_RECOGNIZE_FINISH_OF_CUR_LICENSE,//ÊÓÆµÊ¶±ð³µÅÆ½áÊø
        }

        //³µÅÆÑÕÉ«
        public enum VCA_PLATE_COLOR
        {
            VCA_BLUE_PLATE = 0,//À¶É«³µÅÆ
            VCA_YELLOW_PLATE,//»ÆÉ«³µÅÆ
            VCA_WHITE_PLATE,//°×É«³µÅÆ
            VCA_BLACK_PLATE,       //ºÚÉ«³µÅÆ
            VCA_GREEN_PLATE,       //ÂÌÉ«³µÅÆ
            VCA_BKAIR_PLATE,       //Ãñº½ºÚÉ«³µÅÆ
            VCA_OTHER = 0xff       //ÆäËû
        }

        //³µÅÆÀàÐÍ
        public enum VCA_PLATE_TYPE
        {
            VCA_STANDARD92_PLATE = 0,   //±ê×¼ÃñÓÃ³µÓë¾ü³µ
            VCA_STANDARD02_PLATE,  //02Ê½ÃñÓÃ³µÅÆ 
            VCA_WJPOLICE_PLATE,      //Îä¾¯³µ 
            VCA_JINGCHE_PLATE,      //¾¯³µ
            STANDARD92_BACK_PLATE,      //ÃñÓÃ³µË«ÐÐÎ²ÅÆ
            VCA_SHIGUAN_PLATE,          //Ê¹¹Ý³µÅÆ
            VCA_NONGYONG_PLATE,         //Å©ÓÃ³µ
            VCA_MOTO_PLATE,             //Ä¦ÍÐ³µ
            NEW_ENERGY_PLATE,             //ÐÂÄÜÔ´³µÅÆ
                                          //2017-05-18 Ôö¼Ó ÖÐ¶«µØÇø³µÅÆÀàÐÍ
            TRANSPORT_PLATE = 0x20,         //ÔËÊä³µ
            COMMERCIAL_PLATE,               //ÉÌÓÃ³µ
            PRIVATE_PLATE,                      //Ë½¼Ò³µ
            LEARNING_PLATE,                 //½ÌÁ·³µ
            CD_PLATE,                           //Ê¹¹Ý³µ
            CC_PLATE,                           //Ê¹¹Ý³µ
            ARMY_PLATE,                     //¾ü³µ
            PROTOCOL_PLATE,                 // PROTOCOL
            GOVERNMENT_PLATE,               //Õþ¸®³µ
            EXPORT_PLATE,                               // EXPORT
            TAXI_PLATE,                         //TAXI
            TESTING_PLATE,                          // TESTING
            TRANSFER_PLATE,                 // TRANSFER
            TRUCK_PLATE,                    //»õ³µ
            BUS_PLATE,                  //¹«½»³µ
            PUBLIC_PLATE,               //PUBLIC
            PUB_TRANS_PLATE,            //PUBLIC TRANSFER
            PRI_TRANS_PLATE,            //PRIVATE TRANSPORT
            UNKNOWN_PLATE = 0xff,                   // Î´Öª£¨Î´Ê¶±ð£©
        }

        //ÊÓÆµÊ¶±ð´¥·¢ÀàÐÍ
        public enum VCA_TRIGGER_TYPE
        {
            INTER_TRIGGER = 0,// Ä£¿éÄÚ²¿´¥·¢Ê¶±ð
            EXTER_TRIGGER = 1,// Íâ²¿ÎïÀíÐÅºÅ´¥·¢£ºÏßÈ¦¡¢À×´ï¡¢ÊÖ¶¯´¥·¢ÐÅºÅ£»
        }

        public const int MAX_CHINESE_CHAR_NUM = 64;    // ×î´óºº×ÖÀà±ðÊýÁ¿
                                                       //³µÅÆ¿É¶¯Ì¬ÐÞ¸Ä²ÎÊý
        [StructLayoutAttribute(LayoutKind.Sequential)]
        public struct tagNET_VCA_PLATE_PARAM
        {
            public NET_VCA_RECT struSearchRect;//ËÑË÷ÇøÓò(¹éÒ»»¯)
            public NET_VCA_RECT struInvalidateRect;//ÎÞÐ§ÇøÓò£¬ÔÚËÑË÷ÇøÓòÄÚ²¿ (¹éÒ»»¯)
            public ushort wMinPlateWidth;//³µÅÆ×îÐ¡¿í¶È
            public ushort wTriggerDuration;//´¥·¢³ÖÐøÖ¡Êý
            public byte byTriggerType;//´¥·¢Ä£Ê½, VCA_TRIGGER_TYPE
            public byte bySensitivity;//ÁéÃô¶È
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 2, ArraySubType = UnmanagedType.I1)]
            public byte[] byRes;//±£Áô£¬ÖÃ0
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 64, ArraySubType = UnmanagedType.I1)]
            public byte[] byCharPriority;// ºº×ÖÓÅÏÈ¼¶
        }

        /*wMinPlateWidth:¸Ã²ÎÊýÄ¬ÈÏÅäÖÃÎª80ÏñËØ£»¸Ã²ÎÊýµÄÅäÖÃ¶ÔÓÚ³µÅÆº£¿µÍþÊÓ³µÅÆÊ¶±ðËµÃ÷ÎÄµµ 
        Ê¶±ðÓÐÓ°Ïì£¬Èç¹ûÉèÖÃ¹ý´ó£¬ÄÇÃ´Èç¹û³¡¾°ÖÐ³öÏÖÐ¡³µÅÆ¾Í»áÂ©Ê¶±ð£»Èç¹û³¡¾°ÖÐ³µÅÆ¿í¶ÈÆÕ±é½Ï´ó£¬¿ÉÒÔ°Ñ¸Ã²ÎÊýÉèÖÃÉÔ´ó£¬±ãÓÚ¼õÉÙ¶ÔÐé¼Ù³µÅÆµÄ´¦Àí¡£ÔÚ±êÇåÇé¿öÏÂ½¨ÒéÉèÖÃÎª80£¬ ÔÚ¸ßÇåÇé¿öÏÂ½¨ÒéÉèÖÃÎª120
        wTriggerDuration £­ Íâ²¿´¥·¢ÐÅºÅ³ÖÐøÖ¡ÊýÁ¿£¬Æäº¬ÒåÊÇ´Ó´¥·¢ÐÅºÅ¿ªÊ¼Ê¶±ðµÄÖ¡ÊýÁ¿¡£¸ÃÖµÔÚµÍËÙ³¡¾°½¨ÒéÉèÖÃÎª50¡«100£»¸ßËÙ³¡¾°½¨ÒéÉèÖÃÎª15¡«25£»ÒÆ¶¯Ê¶±ðÊ±Èç¹ûÒ²ÓÐÍâ²¿´¥·¢£¬ÉèÖÃÎª15¡«25£»¾ßÌå¿ÉÒÔ¸ù¾ÝÏÖ³¡Çé¿ö½øÐÐÅäÖÃ
        */
        //³µÅÆÊ¶±ð²ÎÊý×Ó½á¹¹
        [StructLayoutAttribute(LayoutKind.Sequential)]
        public struct tagNET_VCA_PLATEINFO
        {
            public VCA_RECOGNIZE_SCENE eRecogniseScene;//Ê¶±ð³¡¾°(µÍËÙºÍ¸ßËÙ)
            public tagNET_VCA_PLATE_PARAM struModifyParam;//³µÅÆ¿É¶¯Ì¬ÐÞ¸Ä²ÎÊý
        }

        //³µÅÆÊ¶±ðÅäÖÃ²ÎÊý
        [StructLayoutAttribute(LayoutKind.Sequential)]
        public struct tagNET_VCA_PLATECFG
        {
            public uint dwSize;
            public byte byPicProType;//±¨¾¯Ê±Í¼Æ¬´¦Àí·½Ê½ 0-²»´¦Àí 1-ÉÏ´«
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 2, ArraySubType = UnmanagedType.I1)]
            public byte[] byRes;//±£Áô£¬ÉèÖÃÎª0
            public NET_DVR_JPEGPARA struPictureParam;//Í¼Æ¬¹æ¸ñ½á¹¹
            public tagNET_VCA_PLATEINFO struPlateInfo;//³µÅÆÐÅÏ¢
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = MAX_DAYS * MAX_TIMESEGMENT_2, ArraySubType = UnmanagedType.Struct)]
            public NET_DVR_SCHEDTIME[] struAlarmTime;//²¼·ÀÊ±¼ä
            public NET_DVR_HANDLEEXCEPTION_V30 struHandleType;//´¦Àí·½Ê½
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = MAX_CHANNUM_V30, ArraySubType = UnmanagedType.I1)]
            public byte[] byRelRecordChan;//±¨¾¯´¥·¢µÄÂ¼ÏóÍ¨µÀ,Îª1±íÊ¾´¥·¢¸ÃÍ¨µÀ
        }

        //³µÅÆÊ¶±ð½á¹û×Ó½á¹¹
        [StructLayoutAttribute(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
        public struct tagNET_VCA_PLATE_INFO
        {
            public VCA_RECOGNIZE_RESULT eResultFlag;//Ê¶±ð½á¹û±êÖ¾ 
            public VCA_PLATE_TYPE ePlateType;//³µÅÆÀàÐÍ
            public VCA_PLATE_COLOR ePlateColor;//³µÅÆÑÕÉ«
            public NET_VCA_RECT struPlateRect;//³µÅÆÎ»ÖÃ
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 4, ArraySubType = UnmanagedType.U4)]
            public uint[] dwRes;//±£Áô£¬ÉèÖÃÎª0 
            public uint dwLicenseLen;//³µÅÆ³¤¶È
            [MarshalAsAttribute(UnmanagedType.ByValTStr, SizeConst = MAX_LICENSE_LEN)]
            public string sLicense;//³µÅÆºÅÂë 
            [MarshalAsAttribute(UnmanagedType.ByValTStr, SizeConst = MAX_LICENSE_LEN)]
            public string sBelieve;//¸÷¸öÊ¶±ð×Ö·ûµÄÖÃÐÅ¶È£¬Èç¼ì²âµ½³µÅÆ"ÕãA12345", ÖÃÐÅ¶ÈÎª10,20,30,40,50,60,70£¬Ôò±íÊ¾"Õã"×ÖÕýÈ·µÄ¿ÉÄÜÐÔÖ»ÓÐ10%£¬"A"×ÖµÄÕýÈ·µÄ¿ÉÄÜÐÔÊÇ20%
        }

        //³µÅÆ¼ì²â½á¹û
        [StructLayoutAttribute(LayoutKind.Sequential)]
        public struct tagNET_VCA_PLATE_RESULT
        {
            public uint dwSize;//½á¹¹³¤¶È
            public uint dwRelativeTime;//Ïà¶ÔÊ±±ê
            public uint dwAbsTime;//¾ø¶ÔÊ±±ê
            public byte byPlateNum;//³µÅÆ¸öÊý
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 3, ArraySubType = UnmanagedType.I1)]
            public byte[] byRes1;
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = MAX_PLATE_NUM, ArraySubType = UnmanagedType.Struct)]
            public tagNET_VCA_PLATE_INFO[] struPlateInfo;//³µÅÆÐÅÏ¢½á¹¹
            public uint dwPicDataLen;//·µ»ØÍ¼Æ¬µÄ³¤¶È Îª0±íÊ¾Ã»ÓÐÍ¼Æ¬£¬´óÓÚ0±íÊ¾¸Ã½á¹¹ºóÃæ½ô¸úÍ¼Æ¬Êý¾Ý
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 4, ArraySubType = UnmanagedType.U4)]
            public uint[] dwRes2;//±£Áô£¬ÉèÖÃÎª0 Í¼Æ¬µÄ¸ß¿í
            public System.IntPtr pImage;//Ö¸ÏòÍ¼Æ¬µÄÖ¸Õë
        }

        //·ÖÎöÒÇÐÐÎª·ÖÎö¹æÔò½á¹¹
        //¾¯½ä¹æÔò½á¹¹
        [StructLayoutAttribute(LayoutKind.Sequential)]
        public struct tagNET_IVMS_ONE_RULE_
        {
            public byte byActive;/* ÊÇ·ñ¼¤»î¹æÔò,0-·ñ, ·Ç0-ÊÇ */
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 7, ArraySubType = UnmanagedType.I1)]
            public byte[] byRes1;//±£Áô£¬ÉèÖÃÎª0×Ö¶Î
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = NAME_LEN, ArraySubType = UnmanagedType.I1)]
            public byte[] byRuleName;//¹æÔòÃû³Æ
            public VCA_EVENT_TYPE dwEventType;//ÐÐÎª·ÖÎöÊÂ¼þÀàÐÍ
            public tagNET_VCA_EVENT_UNION uEventParam;//ÐÐÎª·ÖÎöÊÂ¼þ²ÎÊý
            public tagNET_VCA_SIZE_FILTER struSizeFilter;//³ß´ç¹ýÂËÆ÷
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 68, ArraySubType = UnmanagedType.I1)]
            public byte[] byRes2;/*±£Áô£¬ÉèÖÃÎª0*/
        }

        // ·ÖÎöÒÇ¹æÔò½á¹¹
        [StructLayoutAttribute(LayoutKind.Sequential)]
        public struct tagNET_IVMS_RULECFG
        {
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = MAX_RULE_NUM, ArraySubType = UnmanagedType.Struct)]
            public tagNET_IVMS_ONE_RULE_[] struRule; //¹æÔòÊý×é
        }

        // IVMSÐÐÎª·ÖÎöÅäÖÃ½á¹¹
        [StructLayoutAttribute(LayoutKind.Sequential)]
        public struct tagNET_IVMS_BEHAVIORCFG
        {
            public uint dwSize;
            public byte byPicProType;//±¨¾¯Ê±Í¼Æ¬´¦Àí·½Ê½ 0-²»´¦Àí ·Ç0-ÉÏ´«
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 3, ArraySubType = UnmanagedType.I1)]
            public byte[] byRes;
            public NET_DVR_JPEGPARA struPicParam;//Í¼Æ¬¹æ¸ñ½á¹¹
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = MAX_DAYS * MAX_TIMESEGMENT, ArraySubType = UnmanagedType.Struct)]
            public tagNET_IVMS_RULECFG[] struRuleCfg;//Ã¿¸öÊ±¼ä¶Î¶ÔÓ¦¹æÔò
        }

        //ÖÇÄÜ·ÖÎöÒÇÈ¡Á÷¼Æ»®×Ó½á¹¹
        [StructLayoutAttribute(LayoutKind.Sequential)]
        public struct tagNET_IVMS_DEVSCHED
        {
            public NET_DVR_SCHEDTIME struTime;//Ê±¼ä²ÎÊý
            public NET_DVR_PU_STREAM_CFG struPUStream;//Ç°¶ËÈ¡Á÷²ÎÊý
        }

        //ÖÇÄÜ·ÖÎöÒÇ²ÎÊýÅäÖÃ½á¹¹
        [StructLayoutAttribute(LayoutKind.Sequential)]
        public struct tagNET_IVMS_STREAMCFG
        {
            public uint dwSize;
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = MAX_DAYS * MAX_TIMESEGMENT, ArraySubType = UnmanagedType.Struct)]
            public tagNET_IVMS_DEVSCHED[] struDevSched;//°´Ê±¼ä¶ÎÅäÖÃÇ°¶ËÈ¡Á÷ÒÔ¼°¹æÔòÐÅÏ¢
        }

        //ÆÁ±ÎÇøÓò
        [StructLayoutAttribute(LayoutKind.Sequential)]
        public struct tagNET_VCA_MASK_REGION
        {
            public byte byEnable;//ÊÇ·ñ¼¤»î, 0-·ñ£¬·Ç0-ÊÇ
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 3, ArraySubType = UnmanagedType.I1)]
            public byte[] byRes;//±£Áô£¬ÖÃ0
            public tagNET_VCA_POLYGON struPolygon;//ÆÁ±Î¶à±ßÐÎ
        }

        //ÆÁ±ÎÇøÓòÁ´±í½á¹¹
        [StructLayoutAttribute(LayoutKind.Sequential)]
        public struct tagNET_VCA_MASK_REGION_LIST
        {
            public uint dwSize;//½á¹¹³¤¶È
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 4, ArraySubType = UnmanagedType.I1)]
            public byte[] byRes; //±£Áô£¬ÖÃ0
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = MAX_MASK_REGION_NUM, ArraySubType = UnmanagedType.Struct)]
            public tagNET_VCA_MASK_REGION[] struMask;//ÆÁ±ÎÇøÓòÊý×é
        }

        //ATM½øÈëÇøÓò²ÎÊý
        [StructLayoutAttribute(LayoutKind.Sequential)]
        public struct tagNET_VCA_ENTER_REGION
        {
            public uint dwSize;
            public byte byEnable;//ÊÇ·ñ¼¤»î£¬0-·ñ£¬·Ç0-ÊÇ
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 3, ArraySubType = UnmanagedType.I1)]
            public byte[] byRes1;
            public tagNET_VCA_POLYGON struPolygon;//½øÈëÇøÓò
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 16, ArraySubType = UnmanagedType.I1)]
            public byte[] byRes2;
        }

        //	ÖØÆôÖÇÄÜ¿â
        [DllImport(@"HCNetSDK.dll")]
        public static extern bool NET_VCA_RestartLib(int lUserID, int lChannel);

        [StructLayoutAttribute(LayoutKind.Sequential)]
        public struct tagNET_VCA_LINE_SEGMENT
        {
            public tagNET_VCA_POINT struStartPoint;//±íÊ¾¸ß¶ÈÏßÊ±£¬±íÊ¾Í·²¿µã
            public tagNET_VCA_POINT struEndPoint;//±íÊ¾¸ß¶ÈÏßÊ±£¬±íÊ¾½Å²¿µã
            public float fValue;//¸ß¶ÈÖµ£¬µ¥Î»Ã×
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 4, ArraySubType = UnmanagedType.I1)]
            public byte[] byRes;
        }

        //±ê¶¨ÏßÁ´±í
        [StructLayoutAttribute(LayoutKind.Sequential)]
        public struct tagNET_VCA_LINE_SEG_LIST
        {
            public uint dwSize;//½á¹¹³¤¶È
            public byte bySegNum;//±ê¶¨ÏßÌõÊý
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 3, ArraySubType = System.Runtime.InteropServices.UnmanagedType.I1)]
            public byte[] byRes;//±£Áô£¬ÖÃ0
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = MAX_SEGMENT_NUM, ArraySubType = UnmanagedType.Struct)]
            public tagNET_VCA_LINE_SEGMENT[] struSeg;
        }

        [DllImport(@"HCNetSDK.dll")]
        public static extern bool NET_DVR_GetRealHeight(int lUserID, int lChannel, ref tagNET_VCA_LINE lpLine, ref Single lpHeight);

        [DllImport(@"HCNetSDK.dll")]
        public static extern bool NET_DVR_GetRealLength(int lUserID, int lChannel, ref tagNET_VCA_LINE lpLine, ref Single lpLength);

        //IVMSÆÁ±ÎÇøÓòÁ´±í
        [StructLayoutAttribute(LayoutKind.Sequential)]
        public struct tagNET_IVMS_MASK_REGION_LIST
        {
            public uint dwSize;//½á¹¹³¤¶È
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = MAX_DAYS * MAX_TIMESEGMENT, ArraySubType = UnmanagedType.Struct)]
            public tagNET_VCA_MASK_REGION_LIST[] struList;
        }

        //IVMSµÄATM½øÈëÇøÓò²ÎÊý
        [StructLayoutAttribute(LayoutKind.Sequential)]
        public struct tagNET_IVMS_ENTER_REGION
        {
            public uint dwSize;
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = MAX_DAYS * MAX_TIMESEGMENT, ArraySubType = UnmanagedType.Struct)]
            public tagNET_VCA_ENTER_REGION[] struEnter;//½øÈëÇøÓò
        }

        // ivms ±¨¾¯Í¼Æ¬ÉÏ´«½á¹¹
        [StructLayoutAttribute(LayoutKind.Sequential)]
        public struct tagNET_IVMS_ALARM_JPEG
        {
            public byte byPicProType;
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 3, ArraySubType = UnmanagedType.I1)]
            public byte[] byRes;
            public NET_DVR_JPEGPARA struPicParam;
        }

        // IVMS ºó¼ìË÷ÅäÖÃ
        [StructLayoutAttribute(LayoutKind.Sequential)]
        public struct tagNET_IVMS_SEARCHCFG
        {
            public uint dwSize;
            public NET_DVR_MATRIX_DEC_REMOTE_PLAY struRemotePlay;// Ô¶³Ì»Ø·Å
            public tagNET_IVMS_ALARM_JPEG struAlarmJpeg;// ±¨¾¯ÉÏ´«Í¼Æ¬ÅäÖÃ
            public tagNET_IVMS_RULECFG struRuleCfg;//IVMS ÐÐÎª¹æÔòÅäÖÃ
        }

        //2009-7-22
        public const int NET_DVR_GET_AP_INFO_LIST = 305;//»ñÈ¡ÎÞÏßÍøÂç×ÊÔ´²ÎÊý
        public const int NET_DVR_SET_WIFI_CFG = 306;//ÉèÖÃIP¼à¿ØÉè±¸ÎÞÏß²ÎÊý
        public const int NET_DVR_GET_WIFI_CFG = 307;//»ñÈ¡IP¼à¿ØÉè±¸ÎÞÏß²ÎÊý
        public const int NET_DVR_SET_WIFI_WORKMODE = 308;//ÉèÖÃIP¼à¿ØÉè±¸Íø¿Ú¹¤×÷Ä£Ê½²ÎÊý
        public const int NET_DVR_GET_WIFI_WORKMODE = 309;//»ñÈ¡IP¼à¿ØÉè±¸Íø¿Ú¹¤×÷Ä£Ê½²ÎÊý

        //public const int IW_ESSID_MAX_SIZE = 32;
        public const int WIFI_WEP_MAX_KEY_COUNT = 4;
        public const int WIFI_WEP_MAX_KEY_LENGTH = 33;
        public const int WIFI_WPA_PSK_MAX_KEY_LENGTH = 63;
        public const int WIFI_WPA_PSK_MIN_KEY_LENGTH = 8;
        public const int WIFI_MAX_AP_COUNT = 20;

        [StructLayoutAttribute(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
        public struct tagNET_DVR_AP_INFO
        {
            [MarshalAsAttribute(UnmanagedType.ByValTStr, SizeConst = IW_ESSID_MAX_SIZE)]
            public string sSsid;
            public uint dwMode;/* 0 mange Ä£Ê½;1 ad-hocÄ£Ê½£¬²Î¼ûNICMODE */
            public uint dwSecurity;  /*0 ²»¼ÓÃÜ£»1 wep¼ÓÃÜ£»2 wpa-psk;3 wpa-Enterprise£¬²Î¼ûWIFISECURITY*/
            public uint dwChannel;/*1-11±íÊ¾11¸öÍ¨µÀ*/
            public uint dwSignalStrength;/*0-100ÐÅºÅÓÉ×îÈõ±äÎª×îÇ¿*/
            public uint dwSpeed;/*ËÙÂÊ,µ¥Î»ÊÇ0.01mbps*/
        }

        [StructLayoutAttribute(LayoutKind.Sequential)]
        public struct tagNET_DVR_AP_INFO_LIST
        {
            public uint dwSize;
            public uint dwCount;/*ÎÞÏßAPÊýÁ¿£¬²»³¬¹ý20*/
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = WIFI_MAX_AP_COUNT, ArraySubType = UnmanagedType.Struct)]
            public tagNET_DVR_AP_INFO[] struApInfo;
        }

        [StructLayoutAttribute(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
        public struct tagNET_DVR_WIFIETHERNET
        {
            [MarshalAsAttribute(UnmanagedType.ByValTStr, SizeConst = 16)]
            public string sIpAddress;/*IPµØÖ·*/
            [MarshalAsAttribute(UnmanagedType.ByValTStr, SizeConst = 16)]
            public string sIpMask;/*ÑÚÂë*/
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = MACADDR_LEN, ArraySubType = UnmanagedType.I1)]
            public byte[] byMACAddr;/*ÎïÀíµØÖ·£¬Ö»ÓÃÀ´ÏÔÊ¾*/
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 2, ArraySubType = UnmanagedType.I1)]
            public byte[] bRes;
            public uint dwEnableDhcp;/*ÊÇ·ñÆô¶¯dhcp  0²»Æô¶¯ 1Æô¶¯*/
            public uint dwAutoDns;/*Èç¹ûÆô¶¯dhcpÊÇ·ñ×Ô¶¯»ñÈ¡dns,0²»×Ô¶¯»ñÈ¡ 1×Ô¶¯»ñÈ¡£»¶ÔÓÚÓÐÏßÈç¹ûÆô¶¯dhcpÄ¿Ç°×Ô¶¯»ñÈ¡dns*/
            [MarshalAsAttribute(UnmanagedType.ByValTStr, SizeConst = 16)]
            public string sFirstDns; /*µÚÒ»¸ödnsÓòÃû*/
            [MarshalAsAttribute(UnmanagedType.ByValTStr, SizeConst = 16)]
            public string sSecondDns;/*µÚ¶þ¸ödnsÓòÃû*/
            [MarshalAsAttribute(UnmanagedType.ByValTStr, SizeConst = 16)]
            public string sGatewayIpAddr;/* Íø¹ØµØÖ·*/
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 8, ArraySubType = UnmanagedType.I1)]
            public byte[] bRes2;
        }

        [StructLayoutAttribute(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
        public struct tagNET_DVR__WIFI_CFG_EX
        {
            public tagNET_DVR_WIFIETHERNET struEtherNet;/*wifiÍø¿Ú*/
            [MarshalAsAttribute(UnmanagedType.ByValTStr, SizeConst = IW_ESSID_MAX_SIZE)]
            public string sEssid;/*SSID*/
            public uint dwMode;/* 0 mange Ä£Ê½;1 ad-hocÄ£Ê½£¬²Î¼û*/
            public uint dwSecurity;/*0 ²»¼ÓÃÜ£»1 wep¼ÓÃÜ£»2 wpa-psk; */
            [StructLayoutAttribute(LayoutKind.Explicit)]
            public struct key
            {
                [StructLayoutAttribute(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
                public struct wep
                {
                    public uint dwAuthentication;/*0 -¿ª·ÅÊ½ 1-¹²ÏíÊ½*/
                    public uint dwKeyLength;/* 0 -64Î»£»1- 128Î»£»2-152Î»*/
                    public uint dwKeyType;/*0 16½øÖÆ;1 ASCI */
                    public uint dwActive;/*0 Ë÷Òý£º0---3±íÊ¾ÓÃÄÄÒ»¸öÃÜÔ¿*/
                    [MarshalAsAttribute(UnmanagedType.ByValTStr, SizeConst = WIFI_WEP_MAX_KEY_COUNT * WIFI_WEP_MAX_KEY_LENGTH)]
                    public string sKeyInfo;
                }

                [StructLayoutAttribute(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
                public struct wpa_psk
                {
                    public uint dwKeyLength;/*8-63¸öASCII×Ö·û*/
                    [MarshalAsAttribute(UnmanagedType.ByValTStr, SizeConst = WIFI_WPA_PSK_MAX_KEY_LENGTH)]
                    public string sKeyInfo;
                    public byte sRes;
                }
            }
        }


        [StructLayoutAttribute(LayoutKind.Sequential)]
        public struct tagNET_DVR_WIFI_CFG
        {
            public uint dwSize;
            public tagNET_DVR__WIFI_CFG_EX struWifiCfg;
        }

        [StructLayoutAttribute(LayoutKind.Sequential)]
        public struct tagNET_DVR_WIFI_WORKMODE
        {
            public uint dwSize;
            public uint dwNetworkInterfaceMode;/*0 ×Ô¶¯ÇÐ»»Ä£Ê½¡¡1 ÓÐÏßÄ£Ê½*/
        }

        [DllImport(@"HCNetSDK.dll")]
        public static extern bool NET_DVR_SaveRealData_V30(int lRealHandle, uint dwTransType, string sFileName);

        [DllImport(@"HCNetSDK.dll")]
        public static extern bool NET_DVR_EncodeG711Frame(uint iType, ref byte pInBuffer, ref byte pOutBuffer);

        [DllImport(@"HCNetSDK.dll")]
        public static extern bool NET_DVR_DecodeG711Frame(uint iType, ref byte pInBuffer, ref byte pOutBuffer);

        //2009-7-22 end

        //SDK 9000_1.1
        //ÍøÂçÓ²ÅÌ½á¹¹ÅäÖÃ
        [StructLayoutAttribute(LayoutKind.Sequential)]
        public struct tagNET_DVR_SINGLE_NET_DISK_INFO
        {
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 4, ArraySubType = UnmanagedType.I1)]
            public byte[] byRes1;//±£Áô
            public NET_DVR_IPADDR struNetDiskAddr;//ÍøÂçÓ²ÅÌµØÖ·
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = PATHNAME_LEN, ArraySubType = UnmanagedType.I1)]
            public byte[] sDirectory;// PATHNAME_LEN = 128
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 68, ArraySubType = UnmanagedType.I1)]
            public byte[] byRes2;//±£Áô
        }

        public const int MAX_NET_DISK = 16;

        [StructLayoutAttribute(LayoutKind.Sequential)]
        public struct tagNET_DVR_NET_DISKCFG
        {
            public uint dwSize;
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = MAX_NET_DISK, ArraySubType = UnmanagedType.Struct)]
            public tagNET_DVR_SINGLE_NET_DISK_INFO[] struNetDiskParam;
        }

        //ÊÂ¼þÀàÐÍ
        //Ö÷ÀàÐÍ
        public enum MAIN_EVENT_TYPE
        {
            EVENT_MOT_DET = 0,//ÒÆ¶¯Õì²â
            EVENT_ALARM_IN = 1,//±¨¾¯ÊäÈë
            EVENT_VCA_BEHAVIOR = 2,//ÐÐÎª·ÖÎö
        }

        //ÐÐÎª·ÖÎöÖ÷ÀàÐÍ¶ÔÓ¦µÄ´ËÀàÐÍ£¬ 0xffff±íÊ¾È«²¿
        public enum BEHAVIOR_MINOR_TYPE
        {
            EVENT_TRAVERSE_PLANE = 0,// ´©Ô½¾¯½äÃæ,
            EVENT_ENTER_AREA,//Ä¿±ê½øÈëÇøÓò,Ö§³ÖÇøÓò¹æÔò
            EVENT_EXIT_AREA,//Ä¿±êÀë¿ªÇøÓò,Ö§³ÖÇøÓò¹æÔò
            EVENT_INTRUSION,// ÖÜ½çÈëÇÖ,Ö§³ÖÇøÓò¹æÔò
            EVENT_LOITER,//ÅÇ»²,Ö§³ÖÇøÓò¹æÔò
            EVENT_LEFT_TAKE,//¶ª°ü¼ñ°ü,Ö§³ÖÇøÓò¹æÔò
            EVENT_PARKING,//Í£³µ,Ö§³ÖÇøÓò¹æÔò
            EVENT_RUN,//±¼ÅÜ,Ö§³ÖÇøÓò¹æÔò
            EVENT_HIGH_DENSITY,//ÇøÓòÄÚÈËÔ±ÃÜ¶È,Ö§³ÖÇøÓò¹æÔò
            EVENT_STICK_UP,//ÌùÖ½Ìõ,Ö§³ÖÇøÓò¹æÔò
            EVENT_INSTALL_SCANNER,//°²×°¶Á¿¨Æ÷,Ö§³ÖÇøÓò¹æÔò
        }

        //ÊÂ¼þËÑË÷Ìõ¼þ 200-04-07 9000_1.1
        public const int SEARCH_EVENT_INFO_LEN = 300;

        [StructLayoutAttribute(LayoutKind.Sequential)]
        //±¨¾¯ÊäÈë
        public struct struAlarmParam
        {
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = MAX_ALARMIN_V30, ArraySubType = UnmanagedType.I1)]
            public byte[] byAlarmInNo;//±¨¾¯ÊäÈëºÅ£¬byAlarmInNo[0]ÈôÖÃ1Ôò±íÊ¾²éÕÒÓÉ±¨¾¯ÊäÈë1´¥·¢µÄÊÂ¼þ
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = SEARCH_EVENT_INFO_LEN - MAX_ALARMIN_V30, ArraySubType = UnmanagedType.I1)]
            public byte[] byRes;

            public void init()
            {
                byAlarmInNo = new byte[MAX_ALARMIN_V30];
                byRes = new byte[SEARCH_EVENT_INFO_LEN - MAX_CHANNUM_V30];
            }
        }

        //ÒÆ¶¯Õì²â
        [StructLayoutAttribute(LayoutKind.Sequential)]
        public struct struMotionParam
        {
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = MAX_CHANNUM_V30, ArraySubType = UnmanagedType.I1)]
            public byte[] byMotDetChanNo;//ÒÆ¶¯Õì²âÍ¨µÀ£¬byMotDetChanNo[0]ÈôÖÃ1Ôò±íÊ¾²éÕÒÓÉÍ¨µÀ1·¢ÉúÒÆ¶¯Õì²â´¥·¢µÄÊÂ¼þ
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = SEARCH_EVENT_INFO_LEN - MAX_CHANNUM_V30, ArraySubType = UnmanagedType.I1)]
            public byte[] byRes;

            public void init()
            {
                byMotDetChanNo = new byte[MAX_CHANNUM_V30];
                byRes = new byte[SEARCH_EVENT_INFO_LEN - MAX_CHANNUM_V30];
            }
        }

        //ÐÐÎª·ÖÎö
        [StructLayoutAttribute(LayoutKind.Sequential)]
        public struct struVcaParam
        {
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = MAX_CHANNUM_V30, ArraySubType = UnmanagedType.I1)]
            public byte[] byChanNo;//´¥·¢ÊÂ¼þµÄÍ¨µÀ
            public byte byRuleID;//¹æÔòID£¬0xff±íÊ¾È«²¿
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 43, ArraySubType = UnmanagedType.I1)]
            public byte[] byRes1;//±£Áô

            public void init()
            {
                byChanNo = new byte[MAX_CHANNUM_V30];
                byRes1 = new byte[43];
            }
        }

        [StructLayout(LayoutKind.Explicit)]
        public struct uSeniorParam
        {
            //             [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = SEARCH_EVENT_INFO_LEN, ArraySubType = UnmanagedType.I1)]
            //             public byte[] byLen;
            [FieldOffset(0)]
            public struMotionParam struMotionPara;
            [FieldOffset(0)]
            public struAlarmParam struAlarmPara;

            //             public struVcaParam struVcaPara;

            public void init()
            {
                //                 byLen = new byte[SEARCH_EVENT_INFO_LEN];
                struAlarmPara = new struAlarmParam();
                struAlarmPara.init();
                //                 struMotionPara = new struMotionParam();
                //                 struMotionPara.init();
                //                 struVcaPara = new struVcaParam();
                //                 struVcaPara.init();
            }
        }

        [StructLayoutAttribute(LayoutKind.Sequential)]
        public struct tagNET_DVR_SEARCH_EVENT_PARAM
        {
            public ushort wMajorType;//0-ÒÆ¶¯Õì²â£¬1-±¨¾¯ÊäÈë, 2-ÖÇÄÜÊÂ¼þ
            public ushort wMinorType;//ËÑË÷´ÎÀàÐÍ- ¸ù¾ÝÖ÷ÀàÐÍ±ä»¯£¬0xffff±íÊ¾È«²¿
            public NET_DVR_TIME struStartTime;//ËÑË÷µÄ¿ªÊ¼Ê±¼ä£¬Í£Ö¹Ê±¼ä: Í¬Ê±Îª(0, 0) ±íÊ¾´Ó×îÔçµÄÊ±¼ä¿ªÊ¼£¬µ½×îºó£¬×îÇ°ÃæµÄ4000¸öÊÂ¼þ
            public NET_DVR_TIME struEndTime;
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 132, ArraySubType = UnmanagedType.I1)]
            public byte[] byRes;//±£Áô
            public uSeniorParam uSeniorPara;

            public void init()
            {
                byRes = new byte[132];
                uSeniorPara = new uSeniorParam();
                uSeniorPara.init();
            }
        }

        //±¨¾¯ÊäÈë½á¹û
        [StructLayoutAttribute(LayoutKind.Sequential)]
        public struct struAlarmRet
        {
            public uint dwAlarmInNo;//±¨¾¯ÊäÈëºÅ
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = SEARCH_EVENT_INFO_LEN, ArraySubType = UnmanagedType.I1)]
            public byte[] byRes;

            public void init()
            {
                byRes = new byte[SEARCH_EVENT_INFO_LEN];
            }
        }
        //ÒÆ¶¯Õì²â½á¹û
        [StructLayoutAttribute(LayoutKind.Sequential)]
        public struct struMotionRet
        {
            public uint dwMotDetNo;//ÒÆ¶¯Õì²âÍ¨µÀ
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = SEARCH_EVENT_INFO_LEN, ArraySubType = UnmanagedType.I1)]
            public byte[] byRes;

            public void init()
            {
                byRes = new byte[SEARCH_EVENT_INFO_LEN];
            }
        }
        //ÐÐÎª·ÖÎö½á¹û 
        [StructLayoutAttribute(LayoutKind.Sequential)]
        public struct struVcaRet
        {
            public uint dwChanNo;//´¥·¢ÊÂ¼þµÄÍ¨µÀºÅ
            public byte byRuleID;//¹æÔòID
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 3, ArraySubType = UnmanagedType.I1)]
            public byte[] byRes1;//±£Áô
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = NAME_LEN, ArraySubType = UnmanagedType.I1)]
            public byte[] byRuleName;//¹æÔòÃû³Æ
            public tagNET_VCA_EVENT_UNION uEvent;//ÐÐÎªÊÂ¼þ²ÎÊý£¬wMinorType = VCA_EVENT_TYPE¾ö¶¨ÊÂ¼þÀàÐÍ

            public void init()
            {
                byRes1 = new byte[3];
                byRuleName = new byte[NAME_LEN];
            }
        }

        [StructLayout(LayoutKind.Explicit)]
        public struct uSeniorRet
        {
            [FieldOffset(0)]
            public struAlarmRet struAlarmRe;
            [FieldOffset(0)]
            public struMotionRet struMotionRe;
            //             public struVcaRet struVcaRe;

            public void init()
            {
                struAlarmRe = new struAlarmRet();
                struAlarmRe.init();
                //                 struVcaRe = new struVcaRet();
                //                 struVcaRe.init();
            }
        }
        //²éÕÒ·µ»Ø½á¹û
        [StructLayoutAttribute(LayoutKind.Sequential)]
        public struct tagNET_DVR_SEARCH_EVENT_RET
        {
            public ushort wMajorType;//Ö÷ÀàÐÍMA
            public ushort wMinorType;//´ÎÀàÐÍ
            public NET_DVR_TIME struStartTime;//ÊÂ¼þ¿ªÊ¼µÄÊ±¼ä
            public NET_DVR_TIME struEndTime;//ÊÂ¼þÍ£Ö¹µÄÊ±¼ä£¬Âö³åÊÂ¼þÊ±ºÍ¿ªÊ¼Ê±¼äÒ»Ñù
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = MAX_CHANNUM_V30, ArraySubType = UnmanagedType.I1)]
            public byte[] byChan;
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 36, ArraySubType = UnmanagedType.I1)]
            public byte[] byRes;
            public uSeniorRet uSeniorRe;

            public void init()
            {
                byChan = new byte[MAX_CHANNUM_V30];
                byRes = new byte[36];
                uSeniorRe = new uSeniorRet();
                uSeniorRe.init();
            }
        }


        //ÓÊ¼þ·þÎñ²âÊÔ 9000_1.1
        [DllImport(@"HCNetSDK.dll")]
        public static extern bool NET_DVR_EmailTest(int lUserID);

        [DllImport(@"HCNetSDK.dll")]
        public static extern int NET_DVR_FindFileByEvent(int lUserID, ref tagNET_DVR_SEARCH_EVENT_PARAM lpSearchEventParam);

        [DllImport(@"HCNetSDK.dll")]
        public static extern int NET_DVR_FindNextEvent(int lSearchHandle, ref tagNET_DVR_SEARCH_EVENT_RET lpSearchEventRet);


        //2009-8-18 ×¥ÅÄ»ú
        public const int PLATE_INFO_LEN = 1024;
        public const int PLATE_NUM_LEN = 16;
        public const int FILE_NAME_LEN = 256;

        // ³µÅÆÑÕÉ«
        public enum Anonymous_26594f67_851c_4f7d_bec4_094765b7ff83
        {
            BLUE_PLATE, // À¶É«³µÅÆ 
            YELLOW_PLATE, // »ÆÉ«³µÅÆ
            WHITE_PLATE,// °×É«³µÅÆ
            BLACK_PLATE,// ºÚÉ«³µÅÆ
        }

        //liscense plate result
        [StructLayoutAttribute(LayoutKind.Sequential)]
        public struct tagNET_DVR_PLATE_RET
        {
            public uint dwSize;//½á¹¹³¤¶È
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = PLATE_NUM_LEN, ArraySubType = UnmanagedType.I1)]
            public byte[] byPlateNum;//³µÅÆºÅ
            public byte byVehicleType;// ³µÀàÐÍ
            public byte byTrafficLight;//0-ÂÌµÆ£»1-ºìµÆ
            public byte byPlateColor;//³µÅÆÑÕÉ«
            public byte byDriveChan;//´¥·¢³µµÀºÅ
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 32, ArraySubType = UnmanagedType.I1)]
            public byte[] byTimeInfo;/*Ê±¼äÐÅÏ¢*///plate_172.6.113.64_20090724155526948_197170484 
                                                 //Ä¿Ç°ÊÇ17Î»£¬¾«È·µ½ms:20090724155526948
            public byte byCarSpeed;/*µ¥Î»km/h*/
            public byte byCarSpeedH;/*cm/s¸ß8Î»*/
            public byte byCarSpeedL;/*cm/sµÍ8Î»*/
            public byte byRes;
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = PLATE_INFO_LEN - 36, ArraySubType = UnmanagedType.I1)]
            public byte[] byInfo;
            public uint dwPicLen;
        }
        /*×¢£ººóÃæ½ô¸ú dwPicLen ³¤¶ÈµÄ Í¼Æ¬ ÐÅÏ¢*/

        [DllImport(@"HCNetSDK.dll")]
        public static extern bool NET_DVR_INVOKE_PLATE_RECOGNIZE(int lUserID, int lChannel, string pPicFileName, ref tagNET_DVR_PLATE_RET pPlateRet, string pPicBuf, uint dwPicBufLen);

        [StructLayoutAttribute(LayoutKind.Sequential)]
        public struct tagNET_DVR_CCD_CFG
        {
            public uint dwSize;//½á¹¹³¤¶È
            public byte byBlc;/*±³¹â²¹³¥0-off; 1-on*/
            public byte byBlcMode;/*blcÀàÐÍ0-×Ô¶¨Òå1-ÉÏ£»2-ÏÂ£»3-×ó£»4-ÓÒ£»5-ÖÐ£»×¢£º´ËÏîÔÚblcÎª on Ê±²ÅÆðÐ§*/
            public byte byAwb;/*×Ô¶¯°×Æ½ºâ0-×Ô¶¯1; 1-×Ô¶¯2; 2-×Ô¶¯¿ØÖÆ*/
            public byte byAgc;/*×Ô¶¯ÔöÒæ0-¹Ø; 1-µÍ; 2-ÖÐ; 3-¸ß*/
            public byte byDayNight;/*ÈÕÒ¹×ª»»£»0 ²ÊÉ«£»1ºÚ°×£»2×Ô¶¯*/
            public byte byMirror;/*¾µÏñ0-¹Ø;1-×óÓÒ;2-ÉÏÏÂ;3-ÖÐÐÄ*/
            public byte byShutter;/*¿ìÃÅ0-×Ô¶¯; 1-1/25; 2-1/50; 3-1/100; 4-1/250;5-1/500; 6-1/1k ;7-1/2k; 8-1/4k; 9-1/10k; 10-1/100k;*/
            public byte byIrCutTime;/*IRCUTÇÐ»»Ê±¼ä£¬5, 10, 15, 20, 25*/
            public byte byLensType;/*¾µÍ·ÀàÐÍ0-µç×Ó¹âÈ¦; 1-×Ô¶¯¹âÈ¦*/
            public byte byEnVideoTrig;/*ÊÓÆµ´¥·¢Ê¹ÄÜ£º1-Ö§³Ö£»0-²»Ö§³Ö¡£ÊÓÆµ´¥·¢Ä£Ê½ÏÂÊÓÆµ¿ìÃÅËÙ¶È°´ÕÕbyShutterËÙ¶È£¬×¥ÅÄÍ¼Æ¬µÄ¿ìÃÅËÙ¶È°´ÕÕbyCapShutterËÙ¶È£¬×¥ÅÄÍê³Éºó»á×Ô¶¯µ÷½Ú»ØÊÓÆµÄ£Ê½*/
            public byte byCapShutter;/*×¥ÅÄÊ±µÄ¿ìÃÅËÙ¶È£¬1-1/25; 2-1/50; 3-1/100; 4-1/250;5-1/500; 6-1/1k ;7-1/2k; 8-1/4k; 9-1/10k; 10-1/100k; 11-1/150; 12-1/200*/
            public byte byEnRecognise;/*1-Ö§³ÖÊ¶±ð£»0-²»Ö§³ÖÊ¶±ð*/
        }

        [DllImport(@"HCNetSDK.dll")]
        public static extern bool NET_DVR_GetCCDCfg(int lUserID, int lChannel, ref tagNET_DVR_CCD_CFG lpCCDCfg);

        [DllImport(@"HCNetSDK.dll")]
        public static extern bool NET_DVR_SetCCDCfg(int lUserID, int lChannel, ref tagNET_DVR_CCD_CFG lpCCDCfg);

        [StructLayoutAttribute(LayoutKind.Sequential)]
        public struct tagCAMERAPARAMCFG
        {
            public uint dwSize;
            public uint dwPowerLineFrequencyMode;/*0-50HZ; 1-60HZ*/
            public uint dwWhiteBalanceMode;/*0ÊÖ¶¯°×Æ½ºâ; 1×Ô¶¯°×Æ½ºâ1£¨·¶Î§Ð¡£©; 2 ×Ô¶¯°×Æ½ºâ2£¨·¶Î§¿í£¬2200K-15000K£©;3×Ô¶¯¿ØÖÆ3*/
            public uint dwWhiteBalanceModeRGain;/*ÊÖ¶¯°×Æ½ºâÊ±ÓÐÐ§£¬ÊÖ¶¯°×Æ½ºâ RÔöÒæ*/
            public uint dwWhiteBalanceModeBGain;/*ÊÖ¶¯°×Æ½ºâÊ±ÓÐÐ§£¬ÊÖ¶¯°×Æ½ºâ BÔöÒæ*/
            public uint dwExposureMode;/*0 ÊÖ¶¯ÆØ¹â 1×Ô¶¯ÆØ¹â*/
            public uint dwExposureSet;/* 0-USERSET, 1-×Ô¶¯x2£¬2-×Ô¶¯4£¬3-×Ô¶¯81/25, 4-1/50, 5-1/100, 6-1/250, 7-1/500, 8-1/750, 9-1/1000, 10-1/2000, 11-1/4000,12-1/10,000; 13-1/100,000*/
            public uint dwExposureUserSet;/* ×Ô¶¯×Ô¶¨ÒåÆØ¹âÊ±¼ä*/
            public uint dwExposureTarget;/*ÊÖ¶¯ÆØ¹âÊ±¼ä ·¶Î§£¨ManumalÓÐÐ§£¬Î¢Ãë£©*/
            public uint dwIrisMode;/*0 ×Ô¶¯¹âÈ¦ 1ÊÖ¶¯¹âÈ¦*/
            public uint dwGainLevel;/*ÔöÒæ£º0-100*/
            public uint dwBrightnessLevel;/*0-100*/
            public uint dwContrastLevel;/*0-100*/
            public uint dwSharpnessLevel;/*0-100*/
            public uint dwSaturationLevel;/*0-100*/
            public uint dwHueLevel;/*0-100£¬£¨±£Áô£©*/
            public uint dwGammaCorrectionEnabled;/*0 dsibale  1 enable*/
            public uint dwGammaCorrectionLevel;/*0-100*/
            public uint dwWDREnabled;/*¿í¶¯Ì¬£º0 dsibale  1 enable*/
            public uint dwWDRLevel1;/*0-F*/
            public uint dwWDRLevel2;/*0-F*/
            public uint dwWDRContrastLevel;/*0-100*/
            public uint dwDayNightFilterType;/*ÈÕÒ¹ÇÐ»»£º0 day,1 night,2 auto */
            public uint dwSwitchScheduleEnabled;/*0 dsibale  1 enable,(±£Áô)*/
            //Ä£Ê½1(±£Áô)
            public uint dwBeginTime;    /*0-100*/
            public uint dwEndTime;/*0-100*/
            //Ä£Ê½2
            public uint dwDayToNightFilterLevel;//0-7
            public uint dwNightToDayFilterLevel;//0-7
            public uint dwDayNightFilterTime;//(60Ãë)
            public uint dwBacklightMode;/*±³¹â²¹³¥:0 USERSET 1 UP¡¢2 DOWN¡¢3 LEFT¡¢4 RIGHT¡¢5MIDDLE*/
            public uint dwPositionX1;//£¨X×ø±ê1£©
            public uint dwPositionY1;//£¨Y×ø±ê1£©
            public uint dwPositionX2;//£¨X×ø±ê2£©
            public uint dwPositionY2;//£¨Y×ø±ê2£©
            public uint dwBacklightLevel;/*0x0-0xF*/
            public uint dwDigitalNoiseRemoveEnable; /*Êý×ÖÈ¥Ôë£º0 dsibale  1 enable*/
            public uint dwDigitalNoiseRemoveLevel;/*0x0-0xF*/
            public uint dwMirror; /* ¾µÏñ£º0 Left;1 Right,;2 Up;3Down */
            public uint dwDigitalZoom;/*Êý×ÖËõ·Å:0 dsibale  1 enable*/
            public uint dwDeadPixelDetect;/*»µµã¼ì²â,0 dsibale  1 enable*/
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 20, ArraySubType = UnmanagedType.U4)]
            public uint[] dwRes;
        }

        public const int NET_DVR_GET_CCDPARAMCFG = 1067;       //IPC»ñÈ¡CCD²ÎÊýÅäÖÃ
        public const int NET_DVR_SET_CCDPARAMCFG = 1068;      //IPCÉèÖÃCCD²ÎÊýÅäÖÃ

        //Í¼ÏñÔöÇ¿ÒÇ
        //Í¼ÏñÔöÇ¿È¥ÔïÇøÓòÅäÖÃ
        [StructLayoutAttribute(LayoutKind.Sequential)]
        public struct tagIMAGEREGION
        {
            public uint dwSize;//×ÜµÄ½á¹¹³¤¶È
            public ushort wImageRegionTopLeftX;/* Í¼ÏñÔöÇ¿È¥ÔïµÄ×óÉÏx×ø±ê */
            public ushort wImageRegionTopLeftY;/* Í¼ÏñÔöÇ¿È¥ÔïµÄ×óÉÏy×ø±ê */
            public ushort wImageRegionWidth;/* Í¼ÏñÔöÇ¿È¥ÔïÇøÓòµÄ¿í */
            public ushort wImageRegionHeight;/*Í¼ÏñÔöÇ¿È¥ÔïÇøÓòµÄ¸ß*/
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 16, ArraySubType = UnmanagedType.I1)]
            public byte[] byRes;
        }

        //Í¼ÏñÔöÇ¿¡¢È¥Ôë¼¶±ð¼°ÎÈ¶¨ÐÔÊ¹ÄÜÅäÖÃ
        [StructLayoutAttribute(LayoutKind.Sequential)]
        public struct tagIMAGESUBPARAM
        {
            public NET_DVR_SCHEDTIME struImageStatusTime;//Í¼Ïñ×´Ì¬Ê±¼ä¶Î
            public byte byImageEnhancementLevel;//Í¼ÏñÔöÇ¿µÄ¼¶±ð£¬0-7£¬0±íÊ¾¹Ø±Õ
            public byte byImageDenoiseLevel;//Í¼ÏñÈ¥ÔëµÄ¼¶±ð£¬0-7£¬0±íÊ¾¹Ø±Õ
            public byte byImageStableEnable;//Í¼ÏñÎÈ¶¨ÐÔÊ¹ÄÜ£¬0±íÊ¾¹Ø±Õ£¬1±íÊ¾´ò¿ª
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 9, ArraySubType = UnmanagedType.I1)]
            public byte[] byRes;
        }

        public const int NET_DVR_GET_IMAGEREGION = 1062;       //Í¼ÏñÔöÇ¿ÒÇÍ¼ÏñÔöÇ¿È¥ÔïÇøÓò»ñÈ¡
        public const int NET_DVR_SET_IMAGEREGION = 1063;       //Í¼ÏñÔöÇ¿ÒÇÍ¼ÏñÔöÇ¿È¥ÔïÇøÓò»ñÈ¡
        public const int NET_DVR_GET_IMAGEPARAM = 1064;       // Í¼ÏñÔöÇ¿ÒÇÍ¼Ïñ²ÎÊý(È¥Ôë¡¢ÔöÇ¿¼¶±ð£¬ÎÈ¶¨ÐÔÊ¹ÄÜ)»ñÈ¡
        public const int NET_DVR_SET_IMAGEPARAM = 1065;       // Í¼ÏñÔöÇ¿ÒÇÍ¼Ïñ²ÎÊý(È¥Ôë¡¢ÔöÇ¿¼¶±ð£¬ÎÈ¶¨ÐÔÊ¹ÄÜ)ÉèÖÃ

        [StructLayoutAttribute(LayoutKind.Sequential)]
        public struct tagIMAGEPARAM
        {
            public uint dwSize;
            //Í¼ÏñÔöÇ¿Ê±¼ä¶Î²ÎÊýÅäÖÃ£¬ÖÜÈÕ¿ªÊ¼	
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = MAX_DAYS * MAX_TIMESEGMENT, ArraySubType = UnmanagedType.Struct)]
            public tagIMAGESUBPARAM[] struImageParamSched;
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 16, ArraySubType = UnmanagedType.I1)]
            public byte[] byRes;
        }

        [DllImport(@"HCNetSDK.dll")]
        public static extern bool NET_DVR_GetParamSetMode(int lUserID, ref uint dwParamSetMode);

        public struct NET_DVR_CLIENTINFO
        {
            public Int32 lChannel;//Í¨µÀºÅ
            public uint lLinkMode;//×î¸ßÎ»(31)Îª0±íÊ¾Ö÷ÂëÁ÷£¬Îª1±íÊ¾×ÓÂëÁ÷£¬0£­30Î»±íÊ¾ÂëÁ÷Á¬½Ó·½Ê½: 0£ºTCP·½Ê½,1£ºUDP·½Ê½,2£º¶à²¥·½Ê½,3 - RTP·½Ê½£¬4-ÒôÊÓÆµ·Ö¿ª(TCP)
            public IntPtr hPlayWnd;//²¥·Å´°¿ÚµÄ¾ä±ú,ÎªNULL±íÊ¾²»²¥·ÅÍ¼Ïó
            public string sMultiCastIP;//¶à²¥×éµØÖ·
        }

        public struct NET_SDK_CLIENTINFO
        {
            public Int32 lChannel;//Í¨µÀºÅ
            public Int32 lLinkType; //Á¬½ÓsdkµÄ·½Ê½£¬ÊÇ·ñÍ¨¹ýÁ÷Ã½ÌåµÄ±êÖ¾
            public Int32 lLinkMode;//×î¸ßÎ»(31)Îª0±íÊ¾Ö÷ÂëÁ÷£¬Îª1±íÊ¾×ÓÂëÁ÷£¬0£­30Î»±íÊ¾ÂëÁ÷Á¬½Ó·½Ê½: 0£ºTCP·½Ê½,1£ºUDP·½Ê½,2£º¶à²¥·½Ê½,3 - RTP·½Ê½£¬4-ÒôÊÓÆµ·Ö¿ª(TCP)
            public IntPtr hPlayWnd;//²¥·Å´°¿ÚµÄ¾ä±ú,ÎªNULL±íÊ¾²»²¥·ÅÍ¼Ïó
            public string sMultiCastIP;//¶à²¥×éµØÖ·
            public Int32 iMediaSrvNum;
            public System.IntPtr pMediaSrvDir;
        }

        public struct NET_DVR_PREVIEWINFO
        {
            public Int32 lChannel;
            public uint dwStreamType;
            public uint dwLinkMode;
            public IntPtr hPlayWnd;
            public bool bBlocked;
            public bool bPassbackRecord;
            public Byte byPreviewMode;
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = HCNetSDK.STREAM_ID_LEN, ArraySubType = UnmanagedType.I1)]
            public byte[] byStreamID;
            public Byte byProtoType;
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 2, ArraySubType = UnmanagedType.I1)]
            public byte[] byRes1;
            public uint DisplayBufNum;
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 216, ArraySubType = UnmanagedType.I1)]
            public byte[] byRes;
        }

        /*********************************************************
        Function:	NET_DVR_Login_V30
        Desc:  
        Input:	sDVRIP [in] Éè±¸IPµØÖ· 
                wServerPort [in] Éè±¸¶Ë¿ÚºÅ 
                sUserName [in] µÇÂ¼µÄÓÃ»§Ãû 
                sPassword [in] ÓÃ»§ÃÜÂë 
        Output:	lpDeviceInfo [out] Éè±¸ÐÅÏ¢ 
        Return:	-1±íÊ¾Ê§°Ü£¬ÆäËûÖµ±íÊ¾·µ»ØµÄÓÃ»§IDÖµ
        **********************************************************/
        [DllImport(@"HCNetSDK.dll")]
        public static extern Int32 NET_DVR_Login_V30(string sDVRIP, Int32 wDVRPort, string sUserName, string sPassword, ref NET_DVR_DEVICEINFO_V30 lpDeviceInfo);

        /*********************************************************
        Function:	NET_DVR_Logout_V30
        Desc:  ÓÃ»§×¢²áÉè±¸¡£
        Input:	lUserID [in] ÓÃ»§IDºÅ
        Output:	
        Return:	TRUE±íÊ¾³É¹¦£¬FALSE±íÊ¾Ê§°Ü
        **********************************************************/
        [DllImport(@"HCNetSDK.dll")]
        public static extern bool NET_DVR_Logout_V30(Int32 lUserID);

        [StructLayoutAttribute(LayoutKind.Sequential)]
        public struct NET_DVR_SNAPCFG
        {
            public uint dwSize;
            public byte byRelatedDriveWay;
            public byte bySnapTimes;
            public ushort wSnapWaitTime;
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 4, ArraySubType = UnmanagedType.U2)]
            public ushort[] wIntervalTime;
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 24, ArraySubType = UnmanagedType.I1)]
            public byte[] byRes2;
        }

        /*********************************************************
        Function:	NET_DVR_ContinuousShoot
        Desc:  ÊÖ¶¯´¥·¢Á¬ÅÄ¡£
        Input:	    lUserID [in] ÓÃ»§IDºÅ
                    lpInter [in] ÊÖ¶¯Á¬ÅÄ²ÎÊý½á¹¹
        Output:	
        Return:	TRUE±íÊ¾³É¹¦£¬FALSE±íÊ¾Ê§°Ü
        **********************************************************/
        [DllImport(@"HCNetSDK.dll")]
        public static extern bool NET_DVR_ContinuousShoot(Int32 lUserID, ref NET_DVR_SNAPCFG lpInter);

        [DllImport(@"HCNetSDK.dll")]
        public static extern bool NET_DVR_ManualSnap(Int32 lUserID, ref NET_DVR_MANUALSNAP lpInter, ref NET_DVR_PLATE_RESULT lpOuter);

        #region  È¡Á÷Ä£¿éÏà¹Ø½á¹¹Óë½Ó¿Ú

        [StructLayoutAttribute(LayoutKind.Sequential)]
        public struct PLAY_INFO
        {
            public int iUserID;      //×¢²áÓÃ»§ID
            [MarshalAsAttribute(UnmanagedType.ByValTStr, SizeConst = 20)]
            public string strDeviceIP;
            public int iDevicePort;
            [MarshalAsAttribute(UnmanagedType.ByValTStr, SizeConst = 32)]
            public string strDevAdmin;
            [MarshalAsAttribute(UnmanagedType.ByValTStr, SizeConst = 16)]
            public string strDevPsd;
            public int iChannel;      //²¥·ÅÍ¨µÀºÅ(´Ó0¿ªÊ¼)
            public int iLinkMode;   //×î¸ßÎ»(31)Îª0±íÊ¾Ö÷ÂëÁ÷£¬Îª1±íÊ¾×ÓÂëÁ÷£¬0£­30Î»±íÊ¾ÂëÁ÷Á¬½Ó·½Ê½: 0£ºTCP·½Ê½,1£ºUDP·½Ê½,2£º¶à²¥·½Ê½,3 - RTP·½Ê½£¬4-ÒôÊÓÆµ·Ö¿ª(TCP)
            public bool bUseMedia;     //ÊÇ·ñÆôÓÃÁ÷Ã½Ìå
            [MarshalAsAttribute(UnmanagedType.ByValTStr, SizeConst = 20)]
            public string strMediaIP; //Á÷Ã½ÌåIPµØÖ·
            public int iMediaPort;   //Á÷Ã½Ìå¶Ë¿ÚºÅ
        }


        [DllImport("GetStream.dll")]
        public static extern bool CLIENT_SDK_Init();

        [DllImport("GetStream.dll")]
        public static extern bool CLIENT_SDK_UnInit();


        [DllImport("GetStream.dll")]
        public static extern int CLIENT_SDK_GetStream(PLAY_INFO lpPlayInfo); //

        [DllImport("GetStream.dll")]
        public static extern bool CLIENT_SetRealDataCallBack(int iRealHandle, SETREALDATACALLBACK fRealDataCallBack, uint lUser); //

        [DllImport("GetStream.dll")]
        public static extern bool CLIENT_SDK_StopStream(int iRealHandle);

        [DllImport("GetStream.dll")]
        public static extern bool CLIENT_SDK_GetVideoEffect(int iRealHandle, ref int iBrightValue, ref int iContrastValue, ref int iSaturationValue, ref int iHueValue);

        [DllImport("GetStream.dll")]
        public static extern bool CLIENT_SDK_SetVideoEffect(int iRealHandle, int iBrightValue, int iContrastValue, int iSaturationValue, int iHueValue);

        [DllImport("GetStream.dll")]
        public static extern bool CLIENT_SDK_MakeKeyFrame(int iRealHandle);

        #endregion


        #region VODµã²¥·Å¿â

        public const int WM_NETERROR = 0x0400 + 102;          //ÍøÂçÒì³£ÏûÏ¢
        public const int WM_STREAMEND = 0x0400 + 103;    //ÎÄ¼þ²¥·Å½áÊø

        public const int FILE_HEAD = 0;      //ÎÄ¼þÍ·
        public const int VIDEO_I_FRAME = 1;  //ÊÓÆµIÖ¡
        public const int VIDEO_B_FRAME = 2;  //ÊÓÆµBÖ¡
        public const int VIDEO_P_FRAME = 3;  //ÊÓÆµPÖ¡
        public const int VIDEO_BP_FRAME = 4; //ÊÓÆµBPÖ¡
        public const int VIDEO_BBP_FRAME = 5; //ÊÓÆµBÖ¡BÖ¡PÖ¡
        public const int AUDIO_PACKET = 10;   //ÒôÆµ°ü

        [StructLayoutAttribute(LayoutKind.Sequential)]
        public struct BLOCKTIME
        {
            public ushort wYear;
            public byte bMonth;
            public byte bDay;
            public byte bHour;
            public byte bMinute;
            public byte bSecond;
        }


        [StructLayoutAttribute(LayoutKind.Sequential)]
        public struct VODSEARCHPARAM
        {
            public IntPtr sessionHandle;                                    //[in]VOD¿Í»§¶Ë¾ä±ú
            [MarshalAsAttribute(UnmanagedType.ByValTStr, SizeConst = 50)]
            public string dvrIP;                                            //	[in]DVRµÄÍøÂçµØÖ·
            public uint dvrPort;                                            //	[in]DVRµÄ¶Ë¿ÚµØÖ·
            public uint channelNum;                                         //  [in]DVRµÄÍ¨µÀºÅ
            public BLOCKTIME startTime;                                     //	[in]²éÑ¯µÄ¿ªÊ¼Ê±¼ä
            public BLOCKTIME stopTime;                                      //	[in]²éÑ¯µÄ½áÊøÊ±¼ä
            public bool bUseIPServer;                                       //  [in]ÊÇ·ñÊ¹ÓÃIPServer 
            [MarshalAsAttribute(UnmanagedType.ByValTStr, SizeConst = 128)]
            public string SerialNumber;                                     //  [in]Éè±¸µÄÐòÁÐºÅ
        }

        [StructLayoutAttribute(LayoutKind.Sequential)]
        public struct SECTIONLIST
        {
            public BLOCKTIME startTime;
            public BLOCKTIME stopTime;
            public byte byRecType;
            public IntPtr pNext;
        }

        [StructLayoutAttribute(LayoutKind.Sequential)]
        public struct VODOPENPARAM
        {
            public IntPtr sessionHandle;                                    //[in]VOD¿Í»§¶Ë¾ä±ú
            [MarshalAsAttribute(UnmanagedType.ByValTStr, SizeConst = 50)]
            public string dvrIP;                                            //	[in]DVRµÄÍøÂçµØÖ·
            public uint dvrPort;                                            //	[in]DVRµÄ¶Ë¿ÚµØÖ·
            public uint channelNum;                                         //  [in]DVRµÄÍ¨µÀºÅ
            public BLOCKTIME startTime;                                     //	[in]²éÑ¯µÄ¿ªÊ¼Ê±¼ä
            public BLOCKTIME stopTime;                                      //	[in]²éÑ¯µÄ½áÊøÊ±¼ä
            public uint uiUser;
            public bool bUseIPServer;                                       //  [in]ÊÇ·ñÊ¹ÓÃIPServer 
            [MarshalAsAttribute(UnmanagedType.ByValTStr, SizeConst = 128)]
            public string SerialNumber;                                     //  [in]Éè±¸µÄÐòÁÐºÅ

            public VodStreamFrameData streamFrameData;
        }


        [StructLayoutAttribute(LayoutKind.Sequential)]
        public struct CONNPARAM
        {
            public uint uiUser;
            public ErrorCallback errorCB;
        }


        // Òì³£»Øµ÷º¯Êý
        public delegate void ErrorCallback(System.IntPtr hSession, uint dwUser, int lErrorType);
        //Ö¡Êý¾Ý»Øµ÷º¯Êý
        public delegate void VodStreamFrameData(System.IntPtr hStream, uint dwUser, int lFrameType, System.IntPtr pBuffer, uint dwSize);

        //Ä£¿é³õÊ¼»¯
        [DllImport("PdCssVodClient.dll")]
        public static extern bool VODServerConnect(string strServerIp, uint uiServerPort, ref IntPtr hSession, ref CONNPARAM struConn, IntPtr hWnd);

        //Ä£¿éÏú»Ù
        [DllImport("PdCssVodClient.dll")]
        public static extern bool VODServerDisconnect(IntPtr hSession);

        [DllImport("PdCssVodClient.dll")]
        public static extern bool VODStreamSearch(IntPtr pSearchParam, ref IntPtr pSecList);

        [DllImport("PdCssVodClient.dll")]
        public static extern bool VODDeleteSectionList(IntPtr pSecList);

        // ¸ù¾ÝID¡¢Ê±¼ä¶Î´ò¿ªÁ÷»ñÈ¡Á÷¾ä±ú
        [DllImport("PdCssVodClient.dll")]
        public static extern bool VODOpenStream(IntPtr pOpenParam, ref IntPtr phStream);

        [DllImport("PdCssVodClient.dll")]
        public static extern bool VODCloseStream(IntPtr hStream);

        //¸ù¾ÝID¡¢Ê±¼ä¶Î´ò¿ªÅúÁ¿ÏÂÔØ
        [DllImport("PdCssVodClient.dll")]
        public static extern bool VODOpenDownloadStream(ref VODOPENPARAM struVodParam, ref IntPtr phStream);

        [DllImport("PdCssVodClient.dll")]
        public static extern bool VODCloseDownloadStream(IntPtr hStream);

        // ¿ªÊ¼Á÷½âÎö£¬·¢ËÍÊý¾ÝÖ¡
        [DllImport("PdCssVodClient.dll")]
        public static extern bool VODStartStreamData(IntPtr phStream);
        [DllImport("PdCssVodClient.dll")]
        public static extern bool VODPauseStreamData(IntPtr hStream, bool bPause);
        [DllImport("PdCssVodClient.dll")]
        public static extern bool VODStopStreamData(IntPtr hStream);

        // ¸ù¾ÝÊ±¼ä¶¨Î»
        [DllImport("PdCssVodClient.dll")]
        public static extern bool VODSeekStreamData(IntPtr hStream, IntPtr pStartTime);


        // ¸ù¾ÝÊ±¼ä¶¨Î»
        [DllImport("PdCssVodClient.dll")]
        public static extern bool VODSetStreamSpeed(IntPtr hStream, int iSpeed);

        // ¸ù¾ÝÊ±¼ä¶¨Î»
        [DllImport("PdCssVodClient.dll")]
        public static extern bool VODGetStreamCurrentTime(IntPtr hStream, ref BLOCKTIME pCurrentTime);

        #endregion


        #region Ö¡·ÖÎö¿â


        [StructLayoutAttribute(LayoutKind.Sequential)]
        public struct PACKET_INFO
        {
            public int nPacketType;     // packet type
                                        // 0:  file head
                                        // 1:  video I frame
                                        // 2:  video B frame
                                        // 3:  video P frame
                                        // 10: audio frame
                                        // 11: private frame only for PS


            //      [System.Runtime.InteropServices.MarshalAsAttribute(System.Runtime.InteropServices.UnmanagedType.LPStr)]
            public IntPtr pPacketBuffer;
            public uint dwPacketSize;
            public int nYear;
            public int nMonth;
            public int nDay;
            public int nHour;
            public int nMinute;
            public int nSecond;
            public uint dwTimeStamp;
        }



        /******************************************************************************
        * function£ºget a empty port number
        * parameters£º
        * return£º 0 - 499 : empty port number
        *          -1      : server is full    	
        * comment£º
        ******************************************************************************/
        [DllImport("AnalyzeData.dll")]
        public static extern int AnalyzeDataGetSafeHandle();


        /******************************************************************************
        * function£ºopen standard stream data for analyzing
        * parameters£ºlHandle - working port number
        *             pHeader - pointer to file header or info header
        * return£ºTRUE or FALSE
        * comment£º
        ******************************************************************************/
        [DllImport("AnalyzeData.dll")]
        public static extern bool AnalyzeDataOpenStreamEx(int iHandle, byte[] pFileHead);


        /******************************************************************************
        * function£ºclose analyzing
        * parameters£ºlHandle - working port number
        * return£º
        * comment£º
        ******************************************************************************/
        [DllImport("AnalyzeData.dll")]
        public static extern bool AnalyzeDataClose(int iHandle);


        /******************************************************************************
        * function£ºinput stream data
        * parameters£ºlHandle  - working port number
        *  	  pBuffer  - data pointer
        *  	  dwBuffersize	- data size
        * return£ºTRUE or FALSE
        * comment£º
        ******************************************************************************/
        [DllImport("AnalyzeData.dll")]
        public static extern bool AnalyzeDataInputData(int iHandle, IntPtr pBuffer, uint uiSize); //byte []


        /******************************************************************************
        * function£ºget analyzed packet
        * parameters£ºlHandle  - working port number
        *  	  pPacketInfo	- returned structure
        * return£º-1 : error
        *          0 : succeed
        *     1 : failed
        *     2 : file end (only in file mode)    
        * comment£º
        ******************************************************************************/
        [DllImport("AnalyzeData.dll")]
        public static extern int AnalyzeDataGetPacket(int iHandle, ref PACKET_INFO pPacketInfo);  //Òª°ÑpPacketInfo×ª»»³ÉPACKET_INFO½á¹¹


        /******************************************************************************
        * function£ºget remain data from input buffer
        * parameters£ºlHandle  - working port number
        *  	  pBuf	        - pointer to the mem which stored remain data
        *             dwSize        - size of remain data  
        * return£º TRUE or FALSE    
        * comment£º
        ******************************************************************************/
        [DllImport("AnalyzeData.dll")]
        public static extern bool AnalyzeDataGetTail(int iHandle, ref IntPtr pBuffer, ref uint uiSize);


        [DllImport("AnalyzeData.dll")]
        public static extern uint AnalyzeDataGetLastError(int iHandle);

        #endregion


        #region Â¼Ïñ¿â

        public const int DATASTREAM_HEAD = 0;  //Êý¾ÝÍ·
        public const int DATASTREAM_BITBLOCK = 1;  //×Ö½ÚÊý¾Ý
        public const int DATASTREAM_KEYFRAME = 2;  //¹Ø¼üÖ¡Êý¾Ý
        public const int DATASTREAM_NORMALFRAME = 3;  //·Ç¹Ø¼üÖ¡Êý¾Ý


        public const int MESSAGEVALUE_DISKFULL = 0x01;
        public const int MESSAGEVALUE_SWITCHDISK = 0x02;
        public const int MESSAGEVALUE_CREATEFILE = 0x03;
        public const int MESSAGEVALUE_DELETEFILE = 0x04;
        public const int MESSAGEVALUE_SWITCHFILE = 0x05;




        [StructLayoutAttribute(LayoutKind.Sequential)]
        public struct STOREINFO
        {
            public int iMaxChannels;
            public int iDiskGroup;
            public int iStreamType;
            public bool bAnalyze;
            public bool bCycWrite;
            public uint uiFileSize;

            public CALLBACKFUN_MESSAGE funCallback;
        }

        [StructLayoutAttribute(LayoutKind.Sequential)]
        public struct CREATEFILE_INFO
        {
            public int iHandle;

            [MarshalAsAttribute(UnmanagedType.ByValTStr, SizeConst = 128)]
            public string strCameraid;

            [MarshalAsAttribute(UnmanagedType.ByValTStr, SizeConst = 128)]
            public string strFileName;

            public BLOCKTIME tFileCreateTime;
        }

        [StructLayoutAttribute(LayoutKind.Sequential)]
        public struct CLOSEFILE_INFO
        {
            public int iHandle;

            [MarshalAsAttribute(UnmanagedType.ByValTStr, SizeConst = 128)]
            public string strCameraid;

            [MarshalAsAttribute(UnmanagedType.ByValTStr, SizeConst = 128)]
            public string strFileName;

            public BLOCKTIME tFileSwitchTime;
        }



        public delegate int CALLBACKFUN_MESSAGE(int iMessageType, System.IntPtr pBuf, int iBufLen);


        [DllImport("RecordDLL.dll")]
        public static extern int Initialize(STOREINFO struStoreInfo);

        [DllImport("RecordDLL.dll")]
        public static extern int Release();

        [DllImport("RecordDLL.dll")]
        public static extern int OpenChannelRecord(string strCameraid, IntPtr pHead, uint dwHeadLength);

        [DllImport("RecordDLL.dll")]
        public static extern bool CloseChannelRecord(int iRecordHandle);

        [DllImport("RecordDLL.dll")]
        public static extern int GetData(int iHandle, int iDataType, IntPtr pBuf, uint uiSize);

        #endregion

        //Éè±¸ÇøÓòÉèÖÃ
        public const int REGIONTYPE = 0;//´ú±íÇøÓò
        public const int MATRIXTYPE = 11;//¾ØÕó½Úµã
        public const int DEVICETYPE = 2;//´ú±íÉè±¸
        public const int CHANNELTYPE = 3;//´ú±íÍ¨µÀ
        public const int USERTYPE = 5;//´ú±íÓÃ»§

        [StructLayoutAttribute(LayoutKind.Sequential)]
        public struct NET_DVR_LOG_MATRIX
        {
            public NET_DVR_TIME strLogTime;
            public uint dwMajorType;
            public uint dwMinorType;
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = MAX_NAMELEN, ArraySubType = UnmanagedType.I1)]
            public byte[] sPanelUser;
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = MAX_NAMELEN, ArraySubType = UnmanagedType.I1)]
            public byte[] sNetUser;
            public NET_DVR_IPADDR struRemoteHostAddr;
            public uint dwParaType;
            public uint dwChannel;
            public uint dwDiskNumber;
            public uint dwAlarmInPort;
            public uint dwAlarmOutPort;
            public uint dwInfoLen;
            public byte byDevSequence;//²ÛÎ»ºÅ
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = MACADDR_LEN, ArraySubType = UnmanagedType.I1)]
            public byte[] byMacAddr;//MACµØÖ·
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = SERIALNO_LEN, ArraySubType = UnmanagedType.I1)]
            public byte[] sSerialNumber;//ÐòÁÐºÅ
            [MarshalAsAttribute(UnmanagedType.ByValTStr, SizeConst = LOG_INFO_LEN - SERIALNO_LEN - MACADDR_LEN - 1)]
            public string sInfo;
        }

        //ÊÓÆµ×ÛºÏÆ½Ì¨Èí¼þ
        [StructLayoutAttribute(LayoutKind.Sequential)]
        public struct tagVEDIOPLATLOG
        {
            public byte bySearchCondition;//ËÑË÷Ìõ¼þ£¬0-°´²ÛÎ»ºÅËÑË÷£¬1-°´ÐòÁÐºÅËÑË÷ 2-°´MACµØÖ·½øÐÐËÑË÷
            public byte byDevSequence;//²ÛÎ»ºÅ£¬0-79£º¶ÔÓ¦×ÓÏµÍ³µÄ²ÛÎ»ºÅ£»
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = SERIALNO_LEN, ArraySubType = UnmanagedType.I1)]
            public byte[] sSerialNumber;//ÐòÁÐºÅ
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = MACADDR_LEN, ArraySubType = UnmanagedType.I1)]
            public byte[] byMacAddr;//MACµØÖ·
        }

        [DllImportAttribute(@"HCNetSDK.dll")]
        public static extern int NET_DVR_FindNextLog_MATRIX(int iLogHandle, ref NET_DVR_LOG_MATRIX lpLogData);


        [DllImportAttribute(@"HCNetSDK.dll")]
        public static extern int NET_DVR_FindDVRLog_Matrix(int iUserID, int lSelectMode, uint dwMajorType, uint dwMinorType, ref tagVEDIOPLATLOG lpVedioPlatLog, ref NET_DVR_TIME lpStartTime, ref NET_DVR_TIME lpStopTime);




        /*************************************************
        Æô¶¯Ô¶³ÌÅäÖÃºê¶¨Òå 
        NET_DVR_StartRemoteConfig
        ¾ßÌåÖ§³Ö²é¿´º¯ÊýËµÃ÷ºÍ´úÂë
        **************************************************/
        public const int MAX_CARDNO_LEN = 48;
        public const int MAX_OPERATE_INDEX_LEN = 32;
        public const int NET_DVR_GET_ALL_VEHICLE_CONTROL_LIST = 3124;// »ñÈ¡ËùÓÐ³µÁ¾ºÚ°×Ãûµ¥ÐÅÏ¢
        public const int NET_DVR_VEHICLE_DELINFO_CTRL = 3125; // É¾³ýÉè±¸ÄÚºÚÃûµ¥Êý¾Ý¿âÐÅÏ¢ 
        public const int NET_DVR_VEHICLELIST_CTRL_START = 3133;

        /*********************************************************
        Function:	REMOTECONFIGCALLBACK
        Desc:  (»Øµ÷º¯Êý)
        Input:	
        Output:	
        Return:	
        **********************************************************/
        public delegate void REMOTECONFIGCALLBACK(uint dwType, IntPtr lpBuffer, uint dwBufLen, IntPtr pUserData);
        [DllImportAttribute(@"HCNetSDK.dll")]
        public static extern Int32 NET_DVR_StartRemoteConfig(Int32 lUserID, uint dwCommand, IntPtr lpInBuffer, Int32 dwInBufferLen, REMOTECONFIGCALLBACK cbStateCallback, IntPtr pUserData);

        // ½¨Á¢·¢ËÍ³¤Á¬½ÓÊý¾ÝÓë¹Ø±Õ³¤Á¬½ÓÅäÖÃ½Ó¿ÚËù´´½¨µÄ¾ä±ú£¬ÊÍ·Å×ÊÔ´
        [DllImportAttribute(@"HCNetSDK.dll")]
        public static extern bool NET_DVR_SendRemoteConfig(Int32 lHandle, uint dwDataType, IntPtr pSendBuf, uint dwBufSize);

        [DllImportAttribute(@"HCNetSDK.dll")]
        public static extern bool NET_DVR_StopRemoteConfig(Int32 lHandle);

        // Öð¸ö»ñÈ¡²éÕÒµ½µÄ³µÁ¾Êý¾ÝÐÅÏ¢¡£
        [DllImportAttribute(@"HCNetSDK.dll")]
        public static extern Int32 NET_DVR_GetNextRemoteConfig(Int32 lHandle, IntPtr lpOutBuff, uint dwOutBuffSize);


        // ÖÇÄÜ½»Í¨²Ù×÷ÀàÐÍ
        public enum VCA_OPERATE_TYPE
        {
            VCA_LICENSE_TYPE = 0x1,  //³µÅÆºÅÂë
            VCA_PLATECOLOR_TYPE = 0x2,  //³µÅÆÑÕÉ«
            VCA_CARDNO_TYPE = 0x4,  //¿¨ºÅ
            VCA_PLATETYPE_TYPE = 0x8,  //³µÅÆÀàÐÍ
            VCA_LISTTYPE_TYPE = 0x10,  //³µÁ¾Ãûµ¥ÀàÐÍ
            VCA_INDEX_TYPE = 0x20,  //Êý¾ÝÁ÷Ë®ºÅ 2014-02-25
            VCA_OPERATE_INDEX_TYPE = 0x40  //²Ù×÷Êý 2014-03-03
        }
        // NET_DVR_StartRemoteConfig CallBack ·µ»ØÀàÐÍ
        public enum NET_SDK_CALLBACK_TYPE
        {
            NET_SDK_CALLBACK_TYPE_STATUS = 0,
            NET_SDK_CALLBACK_TYPE_PROGRESS,
            NET_SDK_CALLBACK_TYPE_DATA
        }

        // NET_DVR_StartRemoteConfig CallBackµ÷ÓÃÉè±¸·µ»ØµÄ×´Ì¬Öµ
        public enum NET_SDK_CALLBACK_STATUS_NORMAL
        {
            NET_SDK_CALLBACK_STATUS_SUCCESS = 1000,  // ³É¹¦
            NET_SDK_CALLBACK_STATUS_PROCESSING,     // ´¦ÀíÖÐ
            NET_SDK_CALLBACK_STATUS_FAILED,    // Ê§°Ü
            NET_SDK_CALLBACK_STATUS_EXCEPTION,      // Òì³£
            NET_SDK_CALLBACK_STATUS_LANGUAGE_MISMATCH,  //£¨IPCÅäÖÃÎÄ¼þµ¼Èë£©ÓïÑÔ²»Æ¥Åä
            NET_SDK_CALLBACK_STATUS_DEV_TYPE_MISMATCH,  //£¨IPCÅäÖÃÎÄ¼þµ¼Èë£©Éè±¸ÀàÐÍ²»Æ¥Åä
            NET_DVR_CALLBACK_STATUS_SEND_WAIT           // ·¢ËÍµÈ´ý
        }

        // Êý¾ÝÈ«²¿»ñÈ¡½Ó¿Ú £¨³¤Á¬½Ó»ñÈ¡£©
        public struct NET_DVR_VEHICLE_CONTROL_COND
        {
            public uint dwChannel;
            public uint dwOperateType;
            [MarshalAsAttribute(UnmanagedType.ByValTStr, SizeConst = MAX_LICENSE_LEN)]
            public String sLicense;
            [MarshalAsAttribute(UnmanagedType.ByValTStr, SizeConst = MAX_CARDNO_LEN)]
            public String sCardNo;
            public byte byListType;
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 3, ArraySubType = UnmanagedType.I1)]
            public byte[] byRes1;
            public uint dwDataIndex;
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 116, ArraySubType = UnmanagedType.I1)]
            public byte[] byRes;

        }

        // NET_DVR_GetNextRemoteConfig Éè±¸·µ»Ø×´Ì¬
        public enum NET_SDK_GET_NEXT_STATUS
        {
            NET_SDK_GET_NEXT_STATUS_SUCCESS = 1000, // ³É¹¦¶ÁÈ¡µ½Êý¾Ý£¬¿Í»§¶Ë´¦ÀíÍê±¾´ÎÊý¾ÝºóÐèÒªÔÙ´Îµ÷ÓÃNET_DVR_RemoteConfigGetNext»ñÈ¡ÏÂÒ»ÌõÊý¾Ý
            NET_SDK_GET_NETX_STATUS_NEED_WAIT,  // ÐèµÈ´ýÉè±¸·¢ËÍÊý¾Ý£¬¼ÌÐøµ÷ÓÃNET_DVR_RemoteConfigGetNextº¯Êý
            NET_SDK_GET_NEXT_STATUS_FINISH,     // Êý¾ÝÈ«²¿È¡Íê£¬´ËÊ±¿Í»§¶Ë¿Éµ÷ÓÃNET_DVR_StopRemoteConfig½áÊø³¤Á¬½Ó
            NET_SDK_GET_NEXT_STATUS_FAILED,     // ³öÏÖÒì³££¬¿Í»§¶Ë¿Éµ÷ÓÃNET_DVR_StopRemoteConfig½áÊø³¤Á¬½Ó
        }

        // ³öÈë¿ÚºÚ°×Ãûµ¥µÄÊý¾ÝÍ¬²½ÐÅÏ¢½á¹¹Ìå
        public struct tagNET_DVR_VEHICLE_CONTROL_LIST_INFO
        {
            public uint dwSize;
            public uint dwChannel;//Í¨µÀºÅ0xff - È«²¿Í¨µÀ£¨ITC Ä¬ÈÏÊÇ1£©
            public uint dwDataIndex;//Êý¾ÝÁ÷Ë®ºÅ£¨Æ½Ì¨Î¬»¤µÄÊý¾ÝÎ¨Ò»Öµ£¬¿Í»§¶Ë²Ù×÷µÄÊ±ºò£¬¸ÃÖµ²»»áÆðÐ§¡£¸ÃÖµÖ÷ÒªÓÃÓÚÊý¾ÝÔöÁ¿Í¬²½£©
            [MarshalAsAttribute(UnmanagedType.ByValTStr, SizeConst = MAX_LICENSE_LEN)]
            public String sLicense; //³µÅÆºÅÂë
            public byte byListType;//Ãûµ¥ÊôÐÔ£¨ºÚ°×Ãûµ¥£©0-°×Ãûµ¥£¬1-ºÚÃûµ¥
            public byte byPlateType;    //³µÅÆÀàÐÍ
            public byte byPlateColor;   //³µÅÆÑÕÉ«
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 21, ArraySubType = UnmanagedType.I1)]
            public byte[] byRes;
            [MarshalAsAttribute(UnmanagedType.ByValTStr, SizeConst = MAX_CARDNO_LEN)]
            public String sCardNo; // ¿¨ºÅ
            public NET_DVR_TIME_V30 struStartTime;//ÓÐÐ§¿ªÊ¼Ê±¼ä
            public NET_DVR_TIME_V30 struStopTime;//ÓÐÐ§½áÊøÊ±¼ä
                                                 //²Ù×÷Êý£¨Æ½Ì¨Í¬²½±íÁ÷Ë®ºÅ²»»áÖØ¸´£¬ÓÃÓÚÔöÁ¿¸üÐÂ£¬´ú±íÍ¬²½µ½Í¬²½±íµÄÄ³Ò»Ìõ¼ÇÂ¼ÁË£¬´æÔÚÏà»úÄÚ´æ£¬ÖØÆôºó»áÇå0£©2014-03-03
            [MarshalAsAttribute(UnmanagedType.ByValTStr, SizeConst = MAX_OPERATE_INDEX_LEN)]
            public String sOperateIndex;
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 224, ArraySubType = UnmanagedType.I1)]
            public byte[] byRes1; // ±£Áô×Ö½Ú
        }

        // Êý¾ÝÈ«²¿»ñÈ¡½Ó¿Ú £¨³¤Á¬½Ó»ñÈ¡£©
        public struct tagNET_DVR_VEHICLE_CONTROL_COND
        {
            public uint dwChannel;//Í¨µÀºÅ0xffffffff - È«²¿Í¨µÀ£¨ITC Ä¬ÈÏÊÇ1£©
            public uint dwOperateType;//²Ù×÷ÀàÐÍ£¬²ÎÕÕVCA_OPERATE _TYPE¡££¨¿É¸´Ñ¡£©
            [MarshalAsAttribute(UnmanagedType.ByValTStr, SizeConst = MAX_LICENSE_LEN)]
            public String sLicense; //³µÅÆºÅÂë
            [MarshalAsAttribute(UnmanagedType.ByValTStr, SizeConst = MAX_CARDNO_LEN)]
            public String sCardNo; // ¿¨ºÅ
            public byte byListType;//Ãûµ¥ÊôÐÔ£¨ºÚ°×Ãûµ¥£©0-°×Ãûµ¥£¬1-ºÚÃûµ¥£¬0xff-È«²¿
                                   //2014-02-25
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 3, ArraySubType = UnmanagedType.I1)]
            public byte[] byRes1;
            public uint dwDataIndex;//Êý¾ÝÁ÷Ë®ºÅ
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 116, ArraySubType = UnmanagedType.I1)]
            public byte[] byRes;
        }

        // Çå³ýÉè±¸³µÅÆºÚÃûµ¥Êý¾Ý¿âÐÅÏ¢ ½á¹¹Ìå
        public struct NET_DVR_VEHICLE_CONTROL_DELINFO
        {
            public uint dwSize;
            public uint dwDelType;//É¾³ýÌõ¼þÀàÐÍ£¬É¾³ýÌõ¼þÀàÐÍ£¬²ÎÕÕVCA_OPERATE _TYPE¡££¨¿É¸´Ñ¡£©
            [MarshalAsAttribute(UnmanagedType.ByValTStr, SizeConst = 16)]
            public String sLicense; //³µÅÆºÅÂë
            [MarshalAsAttribute(UnmanagedType.ByValTStr, SizeConst = 48)]
            public String sCardNo; // ¿¨ºÅ 
            public byte byPlateType;    //³µÅÆÀàÐÍ
            public byte byPlateColor;   //³µÅÆÑÕÉ«
            public byte byOperateType;  //É¾³ý²Ù×÷ÀàÐÍ(0-Ìõ¼þÉ¾³ý,0xff-É¾³ýÈ«²¿)
                                        //2014-02-25
            public byte byListType;//Ãûµ¥ÊôÐÔ£¨ºÚ°×Ãûµ¥£©0-°×Ãûµ¥£¬1-ºÚÃûµ¥ 2014-03-03
            public uint dwDataIndex;//Êý¾ÝÁ÷Ë®ºÅ 	
                                    //²Ù×÷Êý£¨Æ½Ì¨Í¬²½±íÁ÷Ë®ºÅ²»»áÖØ¸´£¬ÓÃÓÚÔöÁ¿¸üÐÂ£¬´ú±íÍ¬²½µ½Í¬²½±íµÄÄ³Ò»Ìõ¼ÇÂ¼ÁË£¬´æÔÚÏà»úÄÚ´æ£¬ÖØÆôºó»áÇå0£©2014-03-03
            [MarshalAsAttribute(UnmanagedType.ByValTStr, SizeConst = MAX_OPERATE_INDEX_LEN)]
            public String sOperateIndex;
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 24, ArraySubType = UnmanagedType.I1)]
            public byte[] byRes;
        }

        //±¾µØÅäÖÃ ±¨¾¯¼àÌý
        [DllImportAttribute(@"HCNetSDK.dll")]
        public static extern bool NET_DVR_GetLocalIP(byte[] strIP, ref Int32 pValidNum, ref bool pEnableBind);

        [DllImportAttribute(@"HCNetSDK.dll")]
        public static extern bool NET_DVR_SetValidIP(uint dwIPIndex, bool bEnableBind);

        // ÃÅ²ÎÊýÅäÖÃ½á¹¹Ìå
        public const int DOOR_NAME_LEN = 32;
        public const int STRESS_PASSWORD_LEN = 8;
        public const int SUPER_PASSWORD_LEN = 8;
        public const int UNLOCK_PASSWORD_LEN = 8;
        public const int NET_DVR_GET_DOOR_CFG = 2108; // »ñÈ¡ÃÅ²ÎÊý
        public const int NET_DVR_SET_DOOR_CFG = 2109; // ÉèÖÃÃÅ²ÎÊý
        public const int COMM_ALARM_ACS = 0x5002; // ÃÅ½ûÖ÷»ú±¨¾¯
        public const int ACS_CARD_NO_LEN = 32; // ÃÅ½û¿¨ºÅ³¤¶È
        public const int MAX_DOOR_NUM = 32;
        public const int MAX_GROUP_NUM = 32;
        public const int MAX_CARD_RIGHT_PLAN_NUM = 4;
        public const int CARD_PASSWORD_LEN = 8;

        public struct NET_DVR_DOOR_CFG
        {
            public uint dwSize;
            [MarshalAsAttribute(UnmanagedType.ByValTStr, SizeConst = DOOR_NAME_LEN)]
            public String byDoorName;
            public byte byMagneticType;
            public byte byOpenButtonType;
            public byte byOpenDuration;
            public byte byDisabledOpenDuration;
            public byte byMagneticAlarmTimeout;
            public byte byEnableDoorLock;
            public byte byEnableLeaderCard;
            public byte byRes1;
            public uint dwLeaderCardOpenDuration;
            [MarshalAsAttribute(UnmanagedType.ByValTStr, SizeConst = STRESS_PASSWORD_LEN)]
            public String byStressPassword;
            [MarshalAsAttribute(UnmanagedType.ByValTStr, SizeConst = SUPER_PASSWORD_LEN)]
            public String bySuperPassword;
            [MarshalAsAttribute(UnmanagedType.ByValTStr, SizeConst = UNLOCK_PASSWORD_LEN)]
            public String byUnlockPassword;
            [MarshalAsAttribute(UnmanagedType.ByValTStr, SizeConst = 56)]
            public String byRes2;
        }

        // ÃÅ½ûÖ÷»úÊÂ¼þÐÅÏ¢
        public struct NET_DVR_ACS_EVENT_INFO
        {
            public uint dwSize;
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = ACS_CARD_NO_LEN, ArraySubType = UnmanagedType.I1)]
            public byte[] byCardNo; //¿¨ºÅ£¬Îª0ÎÞÐ§
            public byte byCardType; //¿¨ÀàÐÍ£¬1-ÆÕÍ¨¿¨£¬2-²Ð¼²ÈË¿¨£¬3-ºÚÃûµ¥¿¨£¬4-Ñ²¸ü¿¨£¬5-Ð²ÆÈ¿¨£¬6-³¬¼¶¿¨£¬7-À´±ö¿¨£¬Îª0ÎÞÐ§
            public byte byWhiteListNo; //°×Ãûµ¥µ¥ºÅ,1-8£¬Îª0ÎÞÐ§
            public byte byReportChannel; //±¨¸æÉÏ´«Í¨µÀ£¬1-²¼·ÀÉÏ´«£¬2-ÖÐÐÄ×é1ÉÏ´«£¬3-ÖÐÐÄ×é2ÉÏ´«£¬Îª0ÎÞÐ§
            public byte byCardReaderKind; //¶Á¿¨Æ÷ÊôÓÚÄÄÒ»Àà£¬0-ÎÞÐ§£¬1-IT¶Á¿¨Æ÷£¬2-Éí·ÝÖ¤¶Á¿¨Æ÷£¬3-¶þÎ¬Âë¶Á¿¨Æ÷
            public uint dwCardReaderNo; //¶Á¿¨Æ÷±àºÅ£¬Îª0ÎÞÐ§
            public uint dwDoorNo; //ÃÅ±àºÅ£¬Îª0ÎÞÐ§
            public uint dwVerifyNo; //¶àÖØ¿¨ÈÏÖ¤ÐòºÅ£¬Îª0ÎÞÐ§
            public uint dwAlarmInNo; //±¨¾¯ÊäÈëºÅ£¬Îª0ÎÞÐ§
            public uint dwAlarmOutNo; //±¨¾¯Êä³öºÅ£¬Îª0ÎÞÐ§
            public uint dwCaseSensorNo; //ÊÂ¼þ´¥·¢Æ÷±àºÅ
            public uint dwRs485No; //RS485Í¨µÀºÅ£¬Îª0ÎÞÐ§
            public uint dwMultiCardGroupNo; //Èº×é±àºÅ
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 32, ArraySubType = UnmanagedType.I1)]
            public byte[] byRes;
        }


        //ÅúÁ¿²ÎÊýÅäÖÃ
        public const int ZERO_CHAN_INDEX = 500;
        public const int MIRROR_CHAN_INDEX = 400;
        public const int STREAM_ID_LEN = 32;
        public const int MAX_CHANNUM_V40 = 512;

        //compression parameter
        public const int NORM_HIGH_STREAM_COMPRESSION = 0; //Ö÷ÂëÁ÷Í¼ÏñÑ¹Ëõ²ÎÊý,Ñ¹ËõÄÜÁ¦Ç¿,ÐÔÄÜ¿ÉÒÔ¸ü¸ß
        public const int SUB_STREAM_COMPRESSION = 1; //×ÓÂëÁ÷Í¼ÏñÑ¹Ëõ²ÎÊý
        public const int EVENT_INVOKE_COMPRESSION = 2; //ÊÂ¼þ´¥·¢Í¼ÏñÑ¹Ëõ²ÎÊý,Ò»Ð©²ÎÊýÏà¶Ô¹Ì¶¨
        public const int THIRD_STREAM_COMPRESSION = 3;  //µÚÈýÂëÁ÷
        public const int TRANS_STREAM_COMPRESSION = 4;  //×ªÂëÂëÁ÷

        public const int NET_DVR_GET_AUDIO_INPUT = 3201; //»ñÈ¡ÒôÆµÊäÈë²ÎÊý
        public const int NET_DVR_SET_AUDIO_INPUT = 3202; //ÉèÖÃÒôÆµÊäÈë²ÎÊý
        public const int NET_DVR_GET_MULTI_STREAM_COMPRESSIONCFG = 3216; //Ô¶³Ì»ñÈ¡¶àÂëÁ÷Ñ¹Ëõ²ÎÊý
        public const int NET_DVR_SET_MULTI_STREAM_COMPRESSIONCFG = 3217; //Ô¶³ÌÉèÖÃ¶àÂëÁ÷Ñ¹Ëõ²ÎÊý

        public struct NET_DVR_VALID_PERIOD_CFG
        {
            public byte byEnable; //Ê¹ÄÜÓÐÐ§ÆÚ£¬0-²»Ê¹ÄÜ£¬1Ê¹ÄÜ
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 3, ArraySubType = UnmanagedType.I1)]
            public byte[] byRes1;
            public NET_DVR_TIME_EX struBeginTime; //ÓÐÐ§ÆÚÆðÊ¼Ê±¼ä
            public NET_DVR_TIME_EX struEndTime; //ÓÐÐ§ÆÚ½áÊøÊ±¼ä
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 32, ArraySubType = UnmanagedType.I1)]
            public byte[] byRes2;
        }
        public struct NET_DVR_TIME_EX
        {
            public Int16 wYear;
            public byte byMonth;
            public byte byDay;
            public byte byHour;
            public byte byMinute;
            public byte bySecond;
            public byte byRes;
        }
        // Á÷ÐÅÏ¢ - 72×Ö½Ú³¤
        [StructLayoutAttribute(LayoutKind.Sequential)]
        public struct NET_DVR_STREAM_INFO
        {
            public uint dwSize;
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = STREAM_ID_LEN, ArraySubType = UnmanagedType.I1)]
            public byte[] byID;
            public uint dwChannel;
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 32, ArraySubType = UnmanagedType.I1)]
            public byte[] byRes;
        }

        //¶àÂëÁ÷Ñ¹Ëõ²ÎÊýÅäÖÃÌõ¼þ½á¹¹Ìå
        [StructLayoutAttribute(LayoutKind.Sequential)]
        public struct NET_DVR_MULTI_STREAM_COMPRESSIONCFG_COND
        {
            public uint dwSize;
            public NET_DVR_STREAM_INFO struStreamInfo;
            public uint dwStreamType; //ÂëÁ÷ÀàÐÍ£¬0-Ö÷ÂëÁ÷£¬1-×ÓÂëÁ÷£¬2-ÊÂ¼þÀàÐÍ£¬3-ÂëÁ÷3£¬¡­¡­
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 32, ArraySubType = UnmanagedType.I1)]
            public byte[] byRes;
        }

        //¶àÂëÁ÷Ñ¹Ëõ²ÎÊý½á¹¹Ìå
        [StructLayoutAttribute(LayoutKind.Sequential)]
        public struct NET_DVR_MULTI_STREAM_COMPRESSIONCFG
        {
            public uint dwSize;
            public uint dwStreamType; //ÂëÁ÷ÀàÐÍ£¬0-Ö÷ÂëÁ÷£¬1-×ÓÂëÁ÷£¬2-ÊÂ¼þÀàÐÍ£¬3-ÂëÁ÷3£¬¡­¡­
            public NET_DVR_COMPRESSION_INFO_V30 struStreamPara; //ÂëÁ÷Ñ¹Ëõ²ÎÊý
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 80, ArraySubType = UnmanagedType.I1)]
            public byte[] byRes;
        }

        [StructLayoutAttribute(LayoutKind.Sequential)]
        public struct NET_DVR_AUDIO_INPUT_PARAM
        {
            public byte byAudioInputType;  //ÒôÆµÊäÈëÀàÐÍ£¬0-mic in£¬1-line in
            public byte byVolume; //volume,[0-100]
            public byte byEnableNoiseFilter; //ÊÇ·ñ¿ªÆôÉùÒô¹ýÂË-¹Ø£¬-¿ª
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 5, ArraySubType = UnmanagedType.I1)]
            public byte[] byRes;
        }

        //Ê±¼ä¶ÎÂ¼Ïñ²ÎÊýÅäÖÃ(×Ó½á¹¹)
        public struct NET_DVR_RECORDSCHED_V40
        {
            public NET_DVR_SCHEDTIME struRecordTime;
            /*Â¼ÏñÀàÐÍ£¬0:¶¨Ê±Â¼Ïñ£¬1:ÒÆ¶¯Õì²â£¬2:±¨¾¯Â¼Ïñ£¬3:¶¯²â|±¨¾¯£¬4:¶¯²â&±¨¾¯ 5:ÃüÁî´¥·¢, 
            6-ÖÇÄÜ±¨¾¯Â¼Ïñ£¬10-PIR±¨¾¯£¬11-ÎÞÏß±¨¾¯£¬12-ºô¾È±¨¾¯£¬13-È«²¿ÊÂ¼þ,14-ÖÇÄÜ½»Í¨ÊÂ¼þ, 
            15-Ô½½çÕì²â,16-ÇøÓòÈëÇÖ,17-ÉùÒôÒì³£,18-³¡¾°±ä¸üÕì²â,
            19-ÖÇÄÜÕì²â(Ô½½çÕì²â|ÇøÓòÈëÇÖ|ÈËÁ³Õì²â|ÉùÒôÒì³£|³¡¾°±ä¸üÕì²â),20£­ÈËÁ³Õì²â,21-POSÂ¼Ïñ,
            22-½øÈëÇøÓòÕì²â, 23-Àë¿ªÇøÓòÕì²â,24-ÅÇ»²Õì²â,25-ÈËÔ±¾Û¼¯Õì²â,26-¿ìËÙÔË¶¯Õì²â,27-Í£³µÕì²â,
            28-ÎïÆ·ÒÅÁôÕì²â,29-ÎïÆ·ÄÃÈ¡Õì²â,30-»ðµã¼ì²â£¬31-·ÀÆÆ»µ¼ì²â*/
            public byte byRecordType;
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 31, ArraySubType = UnmanagedType.I1)]
            public byte[] byRes;
        }

        //È«ÌìÂ¼Ïñ²ÎÊýÅäÖÃ(×Ó½á¹¹)
        [StructLayoutAttribute(LayoutKind.Sequential)]
        public struct NET_DVR_RECORDDAY_V40
        {
            public byte byAllDayRecord;/* ÊÇ·ñÈ«ÌìÂ¼Ïñ 0-·ñ 1-ÊÇ*/
            /*Â¼ÏñÀàÐÍ£¬0:¶¨Ê±Â¼Ïñ£¬1:ÒÆ¶¯Õì²â£¬2:±¨¾¯Â¼Ïñ£¬3:¶¯²â|±¨¾¯£¬4:¶¯²â&±¨¾¯ 5:ÃüÁî´¥·¢, 
            6-ÖÇÄÜ±¨¾¯Â¼Ïñ£¬10-PIR±¨¾¯£¬11-ÎÞÏß±¨¾¯£¬12-ºô¾È±¨¾¯£¬13-È«²¿ÊÂ¼þ,14-ÖÇÄÜ½»Í¨ÊÂ¼þ, 
            15-Ô½½çÕì²â,16-ÇøÓòÈëÇÖ,17-ÉùÒôÒì³£,18-³¡¾°±ä¸üÕì²â,
            19-ÖÇÄÜÕì²â(Ô½½çÕì²â|ÇøÓòÈëÇÖ|ÈËÁ³Õì²â|ÉùÒôÒì³£|³¡¾°±ä¸üÕì²â),20£­ÈËÁ³Õì²â,21-POSÂ¼Ïñ,
            22-½øÈëÇøÓòÕì²â, 23-Àë¿ªÇøÓòÕì²â,24-ÅÇ»²Õì²â,25-ÈËÔ±¾Û¼¯Õì²â,26-¿ìËÙÔË¶¯Õì²â,27-Í£³µÕì²â,
            28-ÎïÆ·ÒÅÁôÕì²â,29-ÎïÆ·ÄÃÈ¡Õì²â,30-»ðµã¼ì²â£¬31-·ÀÆÆ»µ¼ì²â*/
            public byte byRecordType;
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 62, ArraySubType = UnmanagedType.I1)]
            public byte[] byRes;
        }

        [StructLayoutAttribute(LayoutKind.Sequential)]
        public struct NET_DVR_RECORD_V40
        {
            public uint dwSize;
            public uint dwRecord; /*ÊÇ·ñÂ¼Ïñ 0-·ñ 1-ÊÇ*/
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = MAX_DAYS, ArraySubType = UnmanagedType.Struct)]
            public NET_DVR_RECORDDAY_V40[] struRecAllDay;
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = MAX_DAYS * MAX_TIMESEGMENT_V30, ArraySubType = UnmanagedType.Struct)]
            public NET_DVR_RECORDSCHED_V40[] struRecordSched;
            public uint dwRecordTime; /* Â¼ÏóÑÓÊ±³¤¶È 0-5Ãë£¬ 1-10Ãë£¬ 2-30Ãë£¬ 3-1·ÖÖÓ£¬ 4-2·ÖÖÓ£¬ 5-5·ÖÖÓ£¬ 6-10·ÖÖÓ*/
            public uint dwPreRecordTime; /* Ô¤Â¼Ê±¼ä 0-²»Ô¤Â¼ 1-5Ãë 2-10Ãë 3-15Ãë 4-20Ãë 5-25Ãë 6-30Ãë 7-0xffffffff(¾¡¿ÉÄÜÔ¤Â¼) */
            public uint dwRecorderDuration; /* Â¼Ïñ±£´æµÄ×î³¤Ê±¼ä */
            public byte byRedundancyRec; /*ÊÇ·ñÈßÓàÂ¼Ïñ,ÖØÒªÊý¾ÝË«±¸·Ý£º0/1*/
            public byte byAudioRec; /*Â¼ÏñÊ±¸´ºÏÁ÷±àÂëÊ±ÊÇ·ñ¼ÇÂ¼ÒôÆµÊý¾Ý£º¹úÍâÓÐ´Ë·¨¹æ*/
            public byte byStreamType;  // 0-Ö÷ÂëÁ÷£¬1-×ÓÂëÁ÷£¬2-Ö÷×ÓÂëÁ÷Í¬Ê± 3-ÈýÂëÁ÷
            public byte byPassbackRecord;  // 0:²»»Ø´«Â¼Ïñ 1£º»Ø´«Â¼Ïñ
            public ushort wLockDuration;  // Â¼ÏñËø¶¨Ê±³¤£¬µ¥Î»Ð¡Ê± 0±íÊ¾²»Ëø¶¨£¬0xffff±íÊ¾ÓÀ¾ÃËø¶¨£¬Â¼Ïñ¶ÎµÄÊ±³¤´óÓÚËø¶¨µÄ³ÖÐøÊ±³¤µÄÂ¼Ïñ£¬½«²»»áËø¶¨
            public byte byRecordBackup;  // 0:Â¼Ïñ²»´æµµ 1£ºÂ¼Ïñ´æµµ
            public byte bySVCLevel; //SVC³éÖ¡ÀàÐÍ£º0-²»³é£¬1-³é¶þ·ÖÖ®Ò» 2-³éËÄ·ÖÖ®Èý
            public byte byRecordManage;   //Â¼Ïñµ÷¶È£¬0-ÆôÓÃ£¬ 1-²»ÆôÓÃ; ÆôÓÃÊ±½øÐÐ¶¨Ê±Â¼Ïñ£»²»ÆôÓÃÊ±²»½øÐÐ¶¨Ê±Â¼Ïñ£¬µ«ÊÇÂ¼Ïñ¼Æ»®ÈÔÔÚÊ¹ÓÃ£¬±ÈÈçÒÆ¶¯Õì²â£¬»Ø´«¶¼»¹ÔÚ°´ÕâÌõÂ¼Ïñ¼Æ»®½øÐÐ
            public byte byExtraSaveAudio;//ÒôÆµµ¥¶À´æ´¢
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 126, ArraySubType = UnmanagedType.I1)]
            public byte[] byRes;
        }

        public const int NET_DVR_GET_CARD_CFG = 2116;    //»ñÈ¡¿¨²ÎÊý
        public const int NET_DVR_SET_CARD_CFG = 2117;    //ÉèÖÃ¿¨²ÎÊý

        [StructLayoutAttribute(LayoutKind.Sequential)]
        public struct NET_DVR_CARD_CFG
        {
            public uint dwSize;
            public uint dwModifyParamType;
            // ÐèÒªÐÞ¸ÄµÄ¿¨²ÎÊý£¬ÉèÖÃ¿¨²ÎÊýÊ±ÓÐÐ§£¬°´Î»±íÊ¾£¬Ã¿Î»´ú±íÒ»ÖÖ²ÎÊý£¬1ÎªÐèÒªÐÞ¸Ä£¬0Îª²»ÐÞ¸Ä
            // #define CARD_PARAM_CARD_VALID       0x00000001 //¿¨ÊÇ·ñÓÐÐ§²ÎÊý
            // #define CARD_PARAM_VALID            0x00000002  //ÓÐÐ§ÆÚ²ÎÊý
            // #define CARD_PARAM_CARD_TYPE        0x00000004  //¿¨ÀàÐÍ²ÎÊý
            // #define CARD_PARAM_DOOR_RIGHT       0x00000008  //ÃÅÈ¨ÏÞ²ÎÊý
            // #define CARD_PARAM_LEADER_CARD      0x00000010  //Ê×¿¨²ÎÊý
            // #define CARD_PARAM_SWIPE_NUM        0x00000020  //×î´óË¢¿¨´ÎÊý²ÎÊý
            // #define CARD_PARAM_GROUP            0x00000040  //ËùÊôÈº×é²ÎÊý
            // #define CARD_PARAM_PASSWORD         0x00000080  //¿¨ÃÜÂë²ÎÊý
            // #define CARD_PARAM_RIGHT_PLAN       0x00000100  //¿¨È¨ÏÞ¼Æ»®²ÎÊý
            // #define CARD_PARAM_SWIPED_NUM       0x00000200  //ÒÑË¢¿¨´ÎÊý
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = ACS_CARD_NO_LEN, ArraySubType = UnmanagedType.I1)]
            public byte[] byCardNo; //¿¨ºÅ
            public byte byCardValid; //¿¨ÊÇ·ñÓÐÐ§£¬0-ÎÞÐ§£¬1-ÓÐÐ§£¨ÓÃÓÚÉ¾³ý¿¨£¬ÉèÖÃÊ±ÖÃÎª0½øÐÐÉ¾³ý£¬»ñÈ¡Ê±´Ë×Ö¶ÎÊ¼ÖÕÎª1£©
            public byte byCardType; //¿¨ÀàÐÍ£¬1-ÆÕÍ¨¿¨£¬2-²Ð¼²ÈË¿¨£¬3-ºÚÃûµ¥¿¨£¬4-Ñ²¸ü¿¨£¬5-Ð²ÆÈ¿¨£¬6-³¬¼¶¿¨£¬7-À´±ö¿¨£¬8-½â³ý¿¨£¬Ä¬ÈÏÆÕÍ¨¿¨
            public byte byLeaderCard; //ÊÇ·ñÎªÊ×¿¨£¬1-ÊÇ£¬0-·ñ
            public byte byRes1;
            public uint dwDoorRight; //ÃÅÈ¨ÏÞ£¬°´Î»±íÊ¾£¬1ÎªÓÐÈ¨ÏÞ£¬0ÎªÎÞÈ¨ÏÞ£¬´ÓµÍÎ»µ½¸ßÎ»±íÊ¾¶ÔÃÅ1-NÊÇ·ñÓÐÈ¨ÏÞ
            public NET_DVR_VALID_PERIOD_CFG struValid; //ÓÐÐ§ÆÚ²ÎÊý
            public uint dwBelongGroup; //ËùÊôÈº×é£¬°´Î»±íÊ¾£¬1-ÊôÓÚ£¬0-²»ÊôÓÚ£¬´ÓµÍÎ»µ½¸ßÎ»±íÊ¾ÊÇ·ñ´ÓÊôÈº×é1-N
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = CARD_PASSWORD_LEN, ArraySubType = UnmanagedType.I1)]
            public byte[] byCardPassword; //¿¨ÃÜÂë
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = MAX_DOOR_NUM * MAX_CARD_RIGHT_PLAN_NUM, ArraySubType = UnmanagedType.I1)]
            public byte[] byCardRightPlan; //¿¨È¨ÏÞ¼Æ»®£¬È¡ÖµÎª¼Æ»®Ä£°å±àºÅ£¬Í¬¸öÃÅ²»Í¬¼Æ»®Ä£°å²ÉÓÃÈ¨ÏÞ»òµÄ·½Ê½´¦Àí
            public uint dwMaxSwipeTime; //×î´óË¢¿¨´ÎÊý£¬0ÎªÎÞ´ÎÊýÏÞÖÆ
            public uint dwSwipeTime; //ÒÑË¢¿¨´ÎÊý
            public ushort wRoomNumber; //·¿¼äºÅ 
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 22, ArraySubType = UnmanagedType.I1)]
            public byte[] byRes2;
        }

        [StructLayoutAttribute(LayoutKind.Sequential)]
        public struct NET_DVR_CARD_CFG_COND
        {
            public uint dwSize;
            public uint dwCardNum; //ÉèÖÃ»ò»ñÈ¡¿¨ÊýÁ¿£¬»ñÈ¡Ê±ÖÃÎª0xffffffff±íÊ¾»ñÈ¡ËùÓÐ¿¨ÐÅÏ¢
            public byte byCheckCardNo; //Éè±¸ÊÇ·ñ½øÐÐ¿¨ºÅÐ£Ñé£¬0-²»Ð£Ñé£¬1-Ð£Ñé
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 31, ArraySubType = UnmanagedType.I1)]
            public byte[] byRes;
        }

        [StructLayoutAttribute(LayoutKind.Sequential)]
        public struct NET_DVR_CARD_CFG_SEND_DATA
        {
            public uint dwSize;
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 32, ArraySubType = UnmanagedType.I1)]
            public byte[] byCardNo; //¿¨ºÅ
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 16, ArraySubType = UnmanagedType.I1)]
            public byte[] byRes;
        }


        public const int NET_DVR_GET_GROUP_CFG = 2112;   //»ñÈ¡Èº×é²ÎÊý
        public const int NET_DVR_SET_GROUP_CFG = 2113;    //ÉèÖÃÈº×é²ÎÊý

        //ÐÂÔö½á¹¹Ìå£¨Èº×é£©
        [StructLayoutAttribute(LayoutKind.Sequential)]
        public struct NET_DVR_GROUP_CFG
        {
            public uint dwSize;
            public byte byEnable;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
            public byte[] byRes1;
            public NET_DVR_VALID_PERIOD_CFG struValidPeriodCfg;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
            public byte[] byGroupName;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
            public byte[] byRes2;

        }

        [StructLayoutAttribute(LayoutKind.Sequential)]
        public struct NET_DVR_ALARM_DEVICE_USER
        {
            public uint dwSize;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
            public byte[] sUserName;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)]
            public byte[] sPassword;
            public NET_DVR_IPADDR struUserIP;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 6)]
            public byte[] byAMCAddr;
            public byte byUserType;
            public byte byAlarmOnRight;
            public byte byAlarmOffRight;
            public byte byBypassRight;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
            public byte[] byOtherRight;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
            public byte[] byNetPreviewRight;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
            public byte[] byNetRecordRight;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
            public byte[] byNetPlaybackRight;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
            public byte[] byNetPTZRight;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 128)]
            public byte[] byRes2;

        }

        public const int NET_DVR_GET_FINGERPRINT_CFG = 2150;    //»ñÈ¡Ö¸ÎÆ²ÎÊý
        public const int NET_DVR_SET_FINGERPRINT_CFG = 2151;    //ÉèÖÃÖ¸ÎÆ²ÎÊý
        public const int NET_DVR_DEL_FINGERPRINT_CFG = 2152;    //É¾³ýÖ¸ÎÆ²ÎÊý
        public const int MAX_FINGER_PRINT_LEN = 768;            //×î´óÖ¸ÎÆ³¤¶È

        [StructLayout(LayoutKind.Sequential)]
        public struct NET_DVR_FINGER_PRINT_CFG
        {
            public uint dwSize;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 32, ArraySubType = UnmanagedType.I1)]
            public byte[] byCardNo;  //Ö¸ÎÆ¹ØÁª¿¨ºÅ
            public uint dwFingerPrintLen; //Ö¸ÎÆÊý¾Ý³¤¶È
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 512, ArraySubType = UnmanagedType.I1)]
            public byte[] byEnableCardReader; //ÐèÒªÏÂ·¢Ö¸ÎÆµÄ¶Á¿¨Æ÷£¬°´Êý×é±íÊ¾£¬0-²»ÏÂ·¢¸Ã¶Á¿¨Æ÷£¬1-ÏÂ·¢µ½¸Ã¶Á¿¨Æ÷
            public byte byFingerPrintID;     //Ö¸ÎÆ±àºÅ£¬ÓÐÐ§Öµ·¶Î§Îª1-10
            public byte byFingerType;       //Ö¸ÎÆÀàÐÍ  0-ÆÕÍ¨Ö¸ÎÆ£¬1-Ð²ÆÈÖ¸ÎÆ
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 30, ArraySubType = UnmanagedType.I1)]
            public byte[] byRes1;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = MAX_FINGER_PRINT_LEN, ArraySubType = UnmanagedType.I1)]
            public byte[] byFingerData;        //Ö¸ÎÆÊý¾ÝÄÚÈÝ
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 64, ArraySubType = UnmanagedType.I1)]
            public byte[] byRes;
        }

        public struct StrucTEST
        {
            public uint dwSize;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct NET_DVR_FINGER_PRINT_STATUS
        {
            public uint dwSize;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
            public byte[] byCardNo; //Ö¸ÎÆ¹ØÁªµÄ¿¨ºÅ
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 512)]
            public byte[] byCardReaderRecvStatus;   //Ö¸ÎÆ¶Á¿¨Æ÷×´Ì¬£¬°´Êý×é±íÊ¾
            public byte byFingerPrintID;  //Ö¸ÎÆ±àºÅ£¬ÓÐÐ§Öµ·¶Î§Îª1-10
            public byte byFingerType;   //Ö¸ÎÆÀàÐÍ  0-ÆÕÍ¨Ö¸ÎÆ£¬1-Ð²ÆÈÖ¸ÎÆ
            public byte byTotalStatus;  //ÏÂ·¢×ÜµÄ×´Ì¬£¬0-µ±Ç°Ö¸ÎÆÎ´ÏÂÍêËùÓÐ¶Á¿¨Æ÷£¬1-ÒÑÏÂÍêËùÓÐ¶Á¿¨Æ÷(ÕâÀïµÄËùÓÐÖ¸µÄÊÇÃÅ½ûÖ÷»úÍùËùÓÐµÄ¶Á¿¨Æ÷ÏÂ·¢ÁË£¬²»¹Ü³É¹¦Óë·ñ)
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 61)]
            public byte[] byRes;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct NET_DVR_FINGER_PRINT_INFO_COND
        {
            public uint dwSize;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
            public byte[] byCardNo; //Ö¸ÎÆ¹ØÁªµÄ¿¨ºÅ
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 512)]
            public byte[] byEnableCardReader; //Ö¸ÎÆµÄ¶Á¿¨Æ÷ÐÅÏ¢£¬°´Êý×é±íÊ¾
            public uint dwFingerPrintNum; //ÉèÖÃ»ò»ñÈ¡¿¨ÊýÁ¿£¬»ñÈ¡Ê±ÖÃÎª0xffffffff±íÊ¾»ñÈ¡ËùÓÐ¿¨ÐÅÏ¢
            public byte byFingerPrintID;  //Ö¸ÎÆ±àºÅ£¬ÓÐÐ§Öµ·¶Î§Îª-10   0xff±íÊ¾¸Ã¿¨ËùÓÐÖ¸ÎÆ
            public byte byCallbackMode;   //Éè±¸»Øµ÷·½Ê½£¬0-Éè±¸ËùÓÐ¶Á¿¨Æ÷ÏÂÍêÁË·¶Î§£¬1-ÔÚÊ±¼ä¶ÎÄÚÏÂÁË²¿·ÖÒ²·µ»Ø
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 26)]
            public byte[] byRes1;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct NET_DVR_FINGER_PRINT_BYCARD
        {
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
            public byte[] byCardNo; //Ö¸ÎÆ¹ØÁªµÄ¿¨ºÅ
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 512)]
            public byte[] byEnableCardReader; //Ö¸ÎÆµÄ¶Á¿¨Æ÷ÐÅÏ¢£¬°´Êý×é±íÊ¾
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 10)]
            public byte[] byFingerPrintID;    //ÐèÒª»ñÈ¡µÄÖ¸ÎÆÐÅÏ¢£¬°´Êý×éÏÂ±ê£¬Öµ±íÊ¾0-²»É¾³ý£¬1-É¾³ý¸ÃÖ¸ÎÆ
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 34)]
            public byte[] byRes1;

            //public void init()
            //{
            //    byCardNo = new byte[32];
            //    byEnableCardReader = new byte[512];
            //    byFingerPrintID = new byte[10];
            //    byRes1 = new byte[34];
            //}
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct NET_DVR_FINGER_PRINT_BYREADER
        {
            public uint dwCardReaderNo;  //°´Öµ±íÊ¾£¬Ö¸ÎÆ¶Á¿¨Æ÷±àºÅ
            public byte byClearAllCard;  //ÊÇ·ñÉ¾³ýËùÓÐ¿¨µÄÖ¸ÎÆÐÅÏ¢£¬0-°´¿¨ºÅÉ¾³ýÖ¸ÎÆÐÅÏ¢£¬1-É¾³ýËùÓÐ¿¨µÄÖ¸ÎÆÐÅÏ¢
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
            public byte[] byRes1;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
            public byte[] byCardNo; //Ö¸ÎÆ¹ØÁªµÄ¿¨ºÅ
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 548)]
            public byte[] byRes;

            //public void init()
            //{
            //    byRes1 = new byte[3];
            //    byCardNo = new byte[32];
            //    byRes = new byte[548];
            //}
        }

        //public const int DEL_FINGER_PRINT_MODE_LEN = 588; //ÁªºÏÌå´óÐ¡
        //[StructLayoutAttribute(LayoutKind.Sequential)]
        //public struct NET_DVR_DEL_FINGER_PRINT_MODE
        //{
        //    public NET_DVR_FINGER_PRINT_BYCARD struByCard;   //°´¿¨ºÅµÄ·½Ê½É¾³ý
        //    public NET_DVR_FINGER_PRINT_BYREADER struByReader; //°´¶Á¿¨Æ÷µÄ·½Ê½É¾³ý

        //    //public void init()
        //    //{
        //    //    struByCard = new NET_DVR_FINGER_PRINT_BYCARD();
        //    //    struByCard.init();
        //    //}
        //}

        [StructLayout(LayoutKind.Sequential)]
        public struct NET_DVR_FINGER_PRINT_INFO_CTRL_BYCARD
        {
            public uint dwSize;
            public byte byMode;          //É¾³ý·½Ê½£¬0-°´¿¨ºÅ·½Ê½É¾³ý£¬1-°´¶Á¿¨Æ÷É¾³ý
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
            public byte[] byRes1;

            public NET_DVR_FINGER_PRINT_BYCARD struByCard;   //°´¿¨ºÅµÄ·½Ê½É¾³ý
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 64)]
            public byte[] byRes;

        }

        [StructLayout(LayoutKind.Sequential)]
        public struct NET_DVR_FINGER_PRINT_INFO_CTRL_BYREADER
        {
            public uint dwSize;
            public byte byMode;          //É¾³ý·½Ê½£¬0-°´¿¨ºÅ·½Ê½É¾³ý£¬1-°´¶Á¿¨Æ÷É¾³ý
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
            public byte[] byRes1;

            public NET_DVR_FINGER_PRINT_BYREADER struByReader; //°´¶Á¿¨Æ÷µÄ·½Ê½É¾³ý
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 64)]
            public byte[] byRes;

        }

        public const int NET_DVR_GET_CARD_READER_CFG = 2140;      //»ñÈ¡¶Á¿¨Æ÷²ÎÊý
        public const int NET_DVR_SET_CARD_READER_CFG = 2141;      //ÉèÖÃ¶Á¿¨Æ÷²ÎÊý

        [StructLayout(LayoutKind.Sequential)]
        public struct NET_DVR_CARD_READER_CFG
        {
            public uint dwSize;
            public byte byEnable; //ÊÇ·ñÊ¹ÄÜ£¬1-Ê¹ÄÜ£¬0-²»Ê¹ÄÜ
            public byte byCardReaderType; //¶Á¿¨Æ÷ÀàÐÍ£¬1-DS-K110XM/MK/C/CK£¬2-DS-K192AM/AMP£¬3-DS-K192BM/BMP£¬4-DS-K182AM/AMP£¬5-DS-K182BM/BMP£¬6-DS-K182AMF/ACF£¬7-Î¤¸ù»ò485²»ÔÚÏß,8- DS-K1101M/MK£¬9- DS-K1101C/CK£¬10- DS-K1102M/MK/M-A
                                          //11- DS-K1102C/CK£¬12- DS-K1103M/MK£¬13- DS-K1103C/CK£¬14- DS-K1104M/MK£¬15- DS-K1104C/CK£¬16- DS-K1102S/SK/S-A£¬17- DS-K1102G/GK£¬18- DS-K1100S-B£¬19- DS-K1102EM/EMK£¬20- DS-K1102E/EK£¬
                                          //21- DS-K1200EF£¬22- DS-K1200MF£¬23- DS-K1200CF£¬24- DS-K1300EF£¬25- DS-K1300MF£¬26- DS-K1300CF£¬27- DS-K1105E£¬28- DS-K1105M£¬29- DS-K1105C£¬30- DS-K182AMF£¬31- DS-K196AMF£¬32-DS-K194AMP
                                          //33-DS-K1T200EF/EF-C/MF/MF-C/CF/CF-C,34-DS-K1T300EF/EF-C/MF/MF-C/CF/CF-C£¬35-DS-K1T105E/E-C/M/M-C/C/C-C
            public byte byOkLedPolarity; //OK LED¼«ÐÔ£¬0-Òõ¼«£¬1-Ñô¼«
            public byte byErrorLedPolarity; //Error LED¼«ÐÔ£¬0-Òõ¼«£¬1-Ñô¼«
            public byte byBuzzerPolarity; //·äÃùÆ÷¼«ÐÔ£¬0-Òõ¼«£¬1-Ñô¼«
            public byte bySwipeInterval; //ÖØ¸´Ë¢¿¨¼ä¸ôÊ±¼ä£¬µ¥Î»£ºÃë
            public byte byPressTimeout;  //°´¼ü³¬Ê±Ê±¼ä£¬µ¥Î»£ºÃë
            public byte byEnableFailAlarm; //ÊÇ·ñÆôÓÃ¶Á¿¨Ê§°Ü³¬´Î±¨¾¯£¬0-²»ÆôÓÃ£¬1-ÆôÓÃ
            public byte byMaxReadCardFailNum; //×î´ó¶Á¿¨Ê§°Ü´ÎÊý
            public byte byEnableTamperCheck;  //ÊÇ·ñÖ§³Ö·À²ð¼ì²â£¬0-disable £¬1-enable
            public byte byOfflineCheckTime;  //µôÏß¼ì²âÊ±¼ä µ¥Î»Ãë
            public byte byFingerPrintCheckLevel;   //Ö¸ÎÆÊ¶±ðµÈ¼¶£¬1-1/10ÎóÈÏÂÊ£¬2-1/100ÎóÈÏÂÊ£¬3-1/1000ÎóÈÏÂÊ£¬4-1/10000ÎóÈÏÂÊ£¬5-1/100000ÎóÈÏÂÊ£¬6-1/1000000ÎóÈÏÂÊ£¬7-1/10000000ÎóÈÏÂÊ£¬8-1/100000000ÎóÈÏÂÊ£¬9-3/100ÎóÈÏÂÊ£¬10-3/1000ÎóÈÏÂÊ£¬11-3/10000ÎóÈÏÂÊ£¬12-3/100000ÎóÈÏÂÊ£¬13-3/1000000ÎóÈÏÂÊ£¬14-3/10000000ÎóÈÏÂÊ£¬15-3/100000000ÎóÈÏÂÊ£¬16-Automatic Normal,17-Automatic Secure,18-Automatic More Secure
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 24)]
            public byte[] byRes;
        }

        public const int NET_DVR_GET_WEEK_PLAN_CFG = 2100; //»ñÈ¡ÃÅ×´Ì¬ÖÜ¼Æ»®²ÎÊý
        public const int NET_DVR_SET_WEEK_PLAN_CFG = 2101; //ÉèÖÃÃÅ×´Ì¬ÖÜ¼Æ»®²ÎÊý
        public const int NET_DVR_GET_DOOR_STATUS_HOLIDAY_PLAN = 2102; //»ñÈ¡ÃÅ×´Ì¬¼ÙÈÕ¼Æ»®²ÎÊý
        public const int NET_DVR_SET_DOOR_STATUS_HOLIDAY_PLAN = 2103; //ÉèÖÃÃÅ×´Ì¬¼ÙÈÕ¼Æ»®²ÎÊý
        public const int NET_DVR_GET_DOOR_STATUS_HOLIDAY_GROUP = 2104; //»ñÈ¡ÃÅ×´Ì¬¼ÙÈÕ×é²ÎÊý
        public const int NET_DVR_SET_DOOR_STATUS_HOLIDAY_GROUP = 2105; //ÉèÖÃÃÅ×´Ì¬¼ÙÈÕ×é²ÎÊý
        public const int NET_DVR_GET_DOOR_STATUS_PLAN_TEMPLATE = 2106; //»ñÈ¡ÃÅ×´Ì¬¼Æ»®Ä£°å²ÎÊý
        public const int NET_DVR_SET_DOOR_STATUS_PLAN_TEMPLATE = 2107; //ÉèÖÃÃÅ×´Ì¬¼Æ»®Ä£°å²ÎÊý
        public const int NET_DVR_GET_DOOR_STATUS_PLAN = 2110; //»ñÈ¡ÃÅ×´Ì¬¼Æ»®²ÎÊý
        public const int NET_DVR_SET_DOOR_STATUS_PLAN = 2111; //ÉèÖÃÃÅ×´Ì¬¼Æ»®²ÎÊý
        public const int NET_DVR_CLEAR_ACS_PARAM = 2118; //Çå¿ÕÃÅ½ûÖ÷»ú²ÎÊý
        public const int NET_DVR_GET_VERIFY_WEEK_PLAN = 2124; //»ñÈ¡¶Á¿¨Æ÷ÑéÖ¤·½Ê½ÖÜ¼Æ»®²ÎÊý
        public const int NET_DVR_SET_VERIFY_WEEK_PLAN = 2125; //ÉèÖÃ¶Á¿¨Æ÷ÑéÖ¤·½Ê½ÖÜ¼Æ»®²ÎÊý
        public const int NET_DVR_GET_CARD_RIGHT_WEEK_PLAN = 2126; //»ñÈ¡¿¨È¨ÏÞÖÜ¼Æ»®²ÎÊý
        public const int NET_DVR_SET_CARD_RIGHT_WEEK_PLAN = 2127; //ÉèÖÃ¿¨È¨ÏÞÖÜ¼Æ»®²ÎÊý
        public const int NET_DVR_GET_VERIFY_HOLIDAY_PLAN = 2128; //»ñÈ¡¶Á¿¨Æ÷ÑéÖ¤·½Ê½¼ÙÈÕ¼Æ»®²ÎÊý
        public const int NET_DVR_SET_VERIFY_HOLIDAY_PLAN = 2129; //ÉèÖÃ¶Á¿¨Æ÷ÑéÖ¤·½Ê½¼ÙÈÕ¼Æ»®²ÎÊý
        public const int NET_DVR_GET_CARD_RIGHT_HOLIDAY_PLAN = 2130; //»ñÈ¡¿¨È¨ÏÞ¼ÙÈÕ¼Æ»®²ÎÊý
        public const int NET_DVR_SET_CARD_RIGHT_HOLIDAY_PLAN = 2131; //ÉèÖÃ¿¨È¨ÏÞ¼ÙÈÕ¼Æ»®²ÎÊý
        public const int NET_DVR_GET_VERIFY_HOLIDAY_GROUP = 2132; //»ñÈ¡¶Á¿¨Æ÷ÑéÖ¤·½Ê½¼ÙÈÕ×é²ÎÊý
        public const int NET_DVR_SET_VERIFY_HOLIDAY_GROUP = 2133; //ÉèÖÃ¶Á¿¨Æ÷ÑéÖ¤·½Ê½¼ÙÈÕ×é²ÎÊý
        public const int NET_DVR_GET_CARD_RIGHT_HOLIDAY_GROUP = 2134; //»ñÈ¡¿¨È¨ÏÞ¼ÙÈÕ×é²ÎÊý
        public const int NET_DVR_SET_CARD_RIGHT_HOLIDAY_GROUP = 2135; //ÉèÖÃ¿¨È¨ÏÞ¼ÙÈÕ×é²ÎÊý
        public const int NET_DVR_GET_VERIFY_PLAN_TEMPLATE = 2136; //»ñÈ¡¶Á¿¨Æ÷ÑéÖ¤·½Ê½¼Æ»®Ä£°å²ÎÊý
        public const int NET_DVR_SET_VERIFY_PLAN_TEMPLATE = 2137; //ÉèÖÃ¶Á¿¨Æ÷ÑéÖ¤·½Ê½¼Æ»®Ä£°å²ÎÊý
        public const int NET_DVR_GET_CARD_RIGHT_PLAN_TEMPLATE = 2138; //»ñÈ¡¿¨È¨ÏÞ¼Æ»®Ä£°å²ÎÊý
        public const int NET_DVR_SET_CARD_RIGHT_PLAN_TEMPLATE = 2139; //ÉèÖÃ¿¨È¨ÏÞ¼Æ»®Ä£°å²ÎÊý
        public const int NET_DVR_GET_CARD_READER_PLAN = 2142; //»ñÈ¡¶Á¿¨Æ÷ÑéÖ¤¼Æ»®²ÎÊý
        public const int NET_DVR_SET_CARD_READER_PLAN = 2143; //ÉèÖÃ¶Á¿¨Æ÷ÑéÖ¤¼Æ»®²ÎÊý
        public const int NET_DVR_GET_CARD_USERINFO_CFG = 2163; //»ñÈ¡¿¨ºÅ¹ØÁªÓÃ»§ÐÅÏ¢²ÎÊý
        public const int NET_DVR_SET_CARD_USERINFO_CFG = 2164; //ÉèÖÃ¿¨ºÅ¹ØÁªÓÃ»§ÐÅÏ¢²ÎÊý

        public const int NET_DVR_CONTROL_PTZ_MANUALTRACE = 3316; //ÊÖ¶¯¸ú×Ù¶¨Î»
        public const int NET_DVR_GET_SMARTTRACKCFG = 3293; //»ñÈ¡ÖÇÄÜÔË¶¯¸ú×ÙÅäÖÃÐÅÏ¢    
        public const int NET_DVR_SET_SMARTTRACKCFG = 3294; //ÉèÖÃÖÇÄÜÔË¶¯¸ú×ÙÅäÖÃÐÅÏ¢

        [StructLayout(LayoutKind.Sequential)]
        public struct NET_DVR_DATE
        {
            public ushort wYear; //Äê
            public byte byMonth; //ÔÂ
            public byte byDay; //ÈÕ
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct NET_DVR_SIMPLE_DAYTIME
        {
            public byte byHour; //Ê±
            public byte byMinute; //·Ö
            public byte bySecond; //Ãë
            public byte byRes;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct NET_DVR_TIME_SEGMENT
        {
            public NET_DVR_SIMPLE_DAYTIME struBeginTime; //¿ªÊ¼Ê±¼äµã
            public NET_DVR_SIMPLE_DAYTIME struEndTime;   //½áÊøÊ±¼äµã
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct NET_DVR_SINGLE_PLAN_SEGMENT
        {
            public byte byEnable; //ÊÇ·ñÊ¹ÄÜ£¬1-Ê¹ÄÜ£¬0-²»Ê¹ÄÜ
            public byte byDoorStatus; //ÃÅ×´Ì¬Ä£Ê½£¬0-ÎÞÐ§£¬1-³£¿ª×´Ì¬£¬2-³£±Õ×´Ì¬£¬3-ÆÕÍ¨×´Ì¬£¨ÃÅ×´Ì¬¼Æ»®Ê¹ÓÃ£©
            public byte byVerifyMode; //ÑéÖ¤·½Ê½£¬0-ÎÞÐ§£¬1-Ë¢¿¨£¬2-Ë¢¿¨+ÃÜÂë(¶Á¿¨Æ÷ÑéÖ¤·½Ê½¼Æ»®Ê¹ÓÃ)£¬3-Ë¢¿¨,4-Ë¢¿¨»òÃÜÂë(¶Á¿¨Æ÷ÑéÖ¤·½Ê½¼Æ»®Ê¹ÓÃ), 5-Ö¸ÎÆ£¬6-Ö¸ÎÆ+ÃÜÂë£¬7-Ö¸ÎÆ»òË¢¿¨£¬8-Ö¸ÎÆ+Ë¢¿¨£¬9-Ö¸ÎÆ+Ë¢¿¨+ÃÜÂë£¨ÎÞÏÈºóË³Ðò£©
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 5)]
            public byte[] byRes;
            public NET_DVR_TIME_SEGMENT struTimeSegment; //Ê±¼ä¶Î²ÎÊý
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct NET_DVR_WEEK_PLAN_CFG
        {
            public uint dwSize;
            public byte byEnable;  //ÊÇ·ñÊ¹ÄÜ£¬1-Ê¹ÄÜ£¬0-²»Ê¹ÄÜ
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
            public byte[] byRes1;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = HCNetSDK.MAX_DAYS * HCNetSDK.MAX_TIMESEGMENT_V30, ArraySubType = UnmanagedType.Struct)]
            public NET_DVR_SINGLE_PLAN_SEGMENT[] struPlanCfg; //ÖÜ¼Æ»®²ÎÊý
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)]
            public byte[] byRes2;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct NET_DVR_HOLIDAY_PLAN_CFG
        {
            public uint dwSize;
            public byte byEnable;  //ÊÇ·ñÊ¹ÄÜ£¬1-Ê¹ÄÜ£¬0-²»Ê¹ÄÜ
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
            public byte[] byRes1;
            public HCNetSDK.NET_DVR_DATE struBeginDate; //¼ÙÈÕ¿ªÊ¼ÈÕÆÚ
            public HCNetSDK.NET_DVR_DATE struEndDate; //¼ÙÈÕ½áÊøÈÕÆÚ
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = HCNetSDK.MAX_TIMESEGMENT_V30, ArraySubType = UnmanagedType.Struct)]
            public HCNetSDK.NET_DVR_SINGLE_PLAN_SEGMENT[] struPlanCfg; //Ê±¼ä¶Î²ÎÊý
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)]
            public byte[] byRes2;
        }

        public const int TEMPLATE_NAME_LEN = 32; //¼Æ»®Ä£°åÃû³Æ³¤¶È
        public const int MAX_HOLIDAY_GROUP_NUM = 16; //¼Æ»®Ä£°å×î´ó¼ÙÈÕ×éÊý
        public const int HOLIDAY_GROUP_NAME_LEN = 32; //¼ÙÈÕ×éÃû³Æ³¤¶È
        public const int MAX_HOLIDAY_PLAN_NUM = 16; //¼ÙÈÕ×é×î´ó¼ÙÈÕ¼Æ»®Êý

        [StructLayout(LayoutKind.Sequential)]
        public struct NET_DVR_HOLIDAY_GROUP_CFG
        {
            public uint dwSize;
            public byte byEnable;  //ÊÇ·ñÊ¹ÄÜ£¬1-Ê¹ÄÜ£¬0-²»Ê¹ÄÜ
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
            public byte[] byRes1;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = HCNetSDK.HOLIDAY_GROUP_NAME_LEN)]
            public byte[] byGroupName; //¼ÙÈÕ×éÃû³Æ
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = HCNetSDK.MAX_HOLIDAY_GROUP_NUM, ArraySubType = UnmanagedType.U4)]
            public uint[] dwHolidayPlanNo; //¼ÙÈÕ×é±àºÅ£¬¾ÍÇ°Ìî³ä£¬Óö0ÎÞÐ§
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
            public byte[] byRes2;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct NET_DVR_PLAN_TEMPLATE
        {
            public uint dwSize;
            public byte byEnable; //ÊÇ·ñÆôÓÃ£¬1-ÆôÓÃ£¬0-²»ÆôÓÃ
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
            public byte[] byRes1;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = HCNetSDK.TEMPLATE_NAME_LEN)]
            public byte[] byTemplateName; //Ä£°åÃû³Æ
            public uint dwWeekPlanNo; //ÖÜ¼Æ»®±àºÅ£¬0ÎªÎÞÐ§
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = HCNetSDK.MAX_HOLIDAY_GROUP_NUM, ArraySubType = UnmanagedType.U4)]
            public uint[] dwHolidayGroupNo; //¼ÙÈÕ×é±àºÅ£¬¾ÍÇ°Ìî³ä£¬Óö0ÎÞÐ§
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
            public byte[] byRes2;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct NET_DVR_DOOR_STATUS_PLAN
        {
            public uint dwSize;
            public uint dwTemplateNo; //¼Æ»®Ä£°å±àºÅ£¬Îª0±íÊ¾È¡Ïû¹ØÁª£¬»Ö¸´Ä¬ÈÏ×´Ì¬£¨ÆÕÍ¨×´Ì¬£©
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 64)]
            public byte[] byRes;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct NET_DVR_CARD_READER_PLAN
        {
            public uint dwSize;
            public uint dwTemplateNo; //¼Æ»®Ä£°å±àºÅ£¬Îª0±íÊ¾È¡Ïû¹ØÁª£¬»Ö¸´Ä¬ÈÏ×´Ì¬£¨Ë¢¿¨¿ªÃÅ£©
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 64)]
            public byte[] byRes;
        }

        public const int ACS_PARAM_DOOR_STATUS_WEEK_PLAN = 0x00000001;      //ÃÅ×´Ì¬ÖÜ¼Æ»®²ÎÊý
        public const int ACS_PARAM_VERIFY_WEEK_PALN = 0x00000002;           //¶Á¿¨Æ÷ÖÜ¼Æ»®²ÎÊý
        public const int ACS_PARAM_CARD_RIGHT_WEEK_PLAN = 0x00000004;       //¿¨È¨ÏÞÖÜ¼Æ»®²ÎÊý
        public const int ACS_PARAM_DOOR_STATUS_HOLIDAY_PLAN = 0x00000008;   //ÃÅ×´Ì¬¼ÙÈÕ¼Æ»®²ÎÊý
        public const int ACS_PARAM_VERIFY_HOLIDAY_PALN = 0x00000010;        //¶Á¿¨Æ÷¼ÙÈÕ¼Æ»®²ÎÊý
        public const int ACS_PARAM_CARD_RIGHT_HOLIDAY_PLAN = 0x00000020;    //¿¨È¨ÏÞ¼ÙÈÕ¼Æ»®²ÎÊý
        public const int ACS_PARAM_DOOR_STATUS_HOLIDAY_GROUP = 0x00000040;  //ÃÅ×´Ì¬¼ÙÈÕ×é²ÎÊý
        public const int ACS_PARAM_VERIFY_HOLIDAY_GROUP = 0x00000080;       //¶Á¿¨Æ÷ÑéÖ¤·½Ê½¼ÙÈÕ×é²ÎÊý
        public const int ACS_PARAM_CARD_RIGHT_HOLIDAY_GROUP = 0x00000100;   //¿¨È¨ÏÞ¼ÙÈÕ×é²ÎÊý
        public const int ACS_PARAM_DOOR_STATUS_PLAN_TEMPLATE = 0x00000200;  //ÃÅ×´Ì¬¼Æ»®Ä£°å²ÎÊý
        public const int ACS_PARAM_VERIFY_PALN_TEMPLATE = 0x00000400;       //¶Á¿¨Æ÷ÑéÖ¤·½Ê½¼Æ»®Ä£°å²ÎÊý
        public const int ACS_PARAM_CARD_RIGHT_PALN_TEMPLATE = 0x00000800;   //¿¨È¨ÏÞ¼Æ»®Ä£°å²ÎÊý
        public const int ACS_PARAM_CARD = 0x00001000;                       //¿¨²ÎÊý
        public const int ACS_PARAM_GROUP = 0x00002000;                      //Èº×é²ÎÊý
        public const int ACS_PARAM_ANTI_SNEAK_CFG = 0x00004000;             //·´Ç±»Ø²ÎÊý
        public const int ACS_PAPAM_EVENT_CARD_LINKAGE = 0x00008000;         //ÊÂ¼þ¼°¿¨ºÅÁª¶¯²ÎÊý
        public const int ACS_PAPAM_CARD_PASSWD_CFG = 0x00010000;            //ÃÜÂë¿ªÃÅÊ¹ÄÜ²ÎÊý

        [StructLayout(LayoutKind.Sequential)]
        public struct NET_DVR_ACS_PARAM_TYPE
        {
            public uint dwSize;
            public uint dwParamType;
            //²ÎÊýÀàÐÍ£¬°´Î»±íÊ¾
            //#define ACS_PARAM_DOOR_STATUS_WEEK_PLAN        0x00000001 //ÃÅ×´Ì¬ÖÜ¼Æ»®²ÎÊý
            //#define ACS_PARAM_VERIFY_WEEK_PALN             0x00000002 //¶Á¿¨Æ÷ÖÜ¼Æ»®²ÎÊý
            //#define ACS_PARAM_CARD_RIGHT_WEEK_PLAN         0x00000004 //¿¨È¨ÏÞÖÜ¼Æ»®²ÎÊý
            //#define ACS_PARAM_DOOR_STATUS_HOLIDAY_PLAN     0x00000008 //ÃÅ×´Ì¬¼ÙÈÕ¼Æ»®²ÎÊý
            //#define ACS_PARAM_VERIFY_HOLIDAY_PALN          0x00000010 //¶Á¿¨Æ÷¼ÙÈÕ¼Æ»®²ÎÊý
            //#define ACS_PARAM_CARD_RIGHT_HOLIDAY_PLAN      0x00000020 //¿¨È¨ÏÞ¼ÙÈÕ¼Æ»®²ÎÊý
            //#define ACS_PARAM_DOOR_STATUS_HOLIDAY_GROUP    0x00000040 //ÃÅ×´Ì¬¼ÙÈÕ×é²ÎÊý
            //#define ACS_PARAM_VERIFY_HOLIDAY_GROUP         0x00000080 //¶Á¿¨Æ÷ÑéÖ¤·½Ê½¼ÙÈÕ×é²ÎÊý
            //#define ACS_PARAM_CARD_RIGHT_HOLIDAY_GROUP     0x00000100 //¿¨È¨ÏÞ¼ÙÈÕ×é²ÎÊý
            //#define ACS_PARAM_DOOR_STATUS_PLAN_TEMPLATE    0x00000200 //ÃÅ×´Ì¬¼Æ»®Ä£°å²ÎÊý
            //#define ACS_PARAM_VERIFY_PALN_TEMPLATE         0x00000400 //¶Á¿¨Æ÷ÑéÖ¤·½Ê½¼Æ»®Ä£°å²ÎÊý
            //#define ACS_PARAM_CARD_RIGHT_PALN_TEMPLATE     0x00000800 //¿¨È¨ÏÞ¼Æ»®Ä£°å²ÎÊý
            //#define ACS_PARAM_CARD                         0x00001000 //¿¨²ÎÊý
            //#define ACS_PARAM_GROUP                        0x00002000 //Èº×é²ÎÊý
            //#define ACS_PARAM_ANTI_SNEAK_CFG  	     0x00004000 //·´Ç±»Ø²ÎÊý
            //#define ACS_PAPAM_EVENT_CARD_LINKAGE           0x00008000 //ÊÂ¼þ¼°¿¨ºÅÁª¶¯²ÎÊý
            //#define ACS_PAPAM_CARD_PASSWD_CFG              0x00010000 //ÃÜÂë¿ªÃÅÊ¹ÄÜ²ÎÊý
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
            public byte[] byRes;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct NET_DVR_CARD_USER_INFO_CFG
        {
            public uint dwSize;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = NAME_LEN)]
            public byte[] byUsername;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 256)]
            public byte[] byRes2;
        }


        //ÊÓÆµ²ÎÊýÅäÖÃ
        [StructLayout(LayoutKind.Sequential)]
        public struct NET_DVR_VIDEOEFFECT
        {
            public byte byBrightnessLevel; /*0-100*/
            public byte byContrastLevel; /*0-100*/
            public byte bySharpnessLevel; /*0-100*/
            public byte bySaturationLevel; /*0-100*/
            public byte byHueLevel; /*0-100,£¨±£Áô£©*/
            public byte byEnableFunc; //Ê¹ÄÜ£¬°´Î»±íÊ¾£¬bit0-SMART IR(·À¹ýÆØ)£¬bit1-µÍÕÕ¶È,bit2-Ç¿¹âÒÖÖÆÊ¹ÄÜ£¬0-·ñ£¬1-ÊÇ
            public byte byLightInhibitLevel; //Ç¿¹âÒÖÖÆµÈ¼¶£¬[1-3]±íÊ¾µÈ¼¶
            public byte byGrayLevel; //»Ò¶ÈÖµÓò£¬0-[0-255]£¬1-[16-235]
        }

        //ÔöÒæÅäÖÃ
        [StructLayout(LayoutKind.Sequential)]
        public struct NET_DVR_GAIN
        {
            public byte byGainLevel; /*ÔöÒæ£º0-100*/
            public byte byGainUserSet; /*ÓÃ»§×Ô¶¨ÒåÔöÒæ£»0-100£¬¶ÔÓÚ×¥ÅÄ»ú£¬ÊÇCCDÄ£Ê½ÏÂµÄ×¥ÅÄÔöÒæ*/
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
            public byte[] byRes;
            public uint dwMaxGainValue;/*×î´óÔöÒæÖµ£¬µ¥Î»dB*/
        }

        //°×Æ½ºâÅäÖÃ
        [StructLayout(LayoutKind.Sequential)]
        public struct NET_DVR_WHITEBALANCE
        {
            public byte byWhiteBalanceMode; /*0-ÊÖ¶¯°×Æ½ºâ£¨MWB£©,1-×Ô¶¯°×Æ½ºâ1£¨AWB1£©,2-×Ô¶¯°×Æ½ºâ2 (AWB2),3-×Ô¶¯¿ØÖÆ¸ÄÃûÎªËø¶¨°×Æ½ºâ(Locked WB)£¬
                                            4-ÊÒÍâ(Indoor)£¬5-ÊÒÄÚ(Outdoor)6-ÈÕ¹âµÆ(Fluorescent Lamp)£¬7-ÄÆµÆ(Sodium Lamp)£¬
                                            8-×Ô¶¯¸ú×Ù(Auto-Track)9-Ò»´Î°×Æ½ºâ(One Push)£¬10-ÊÒÍâ×Ô¶¯(Auto-Outdoor)£¬
                                            11-ÄÆµÆ×Ô¶¯ (Auto-Sodiumlight)£¬12-Ë®ÒøµÆ(Mercury Lamp)£¬13-×Ô¶¯°×Æ½ºâ(Auto)£¬
                                            14-°×³ãµÆ (IncandescentLamp)£¬15-Å¯¹âµÆ(Warm Light Lamp)£¬16-×ÔÈ»¹â(Natural Light) */
            public byte byWhiteBalanceModeRGain; /*ÊÖ¶¯°×Æ½ºâÊ±ÓÐÐ§£¬ÊÖ¶¯°×Æ½ºâ RÔöÒæ*/
            public byte byWhiteBalanceModeBGain; /*ÊÖ¶¯°×Æ½ºâÊ±ÓÐÐ§£¬ÊÖ¶¯°×Æ½ºâ BÔöÒæ*/
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 5)]
            public byte[] byRes;
        }

        //ÆØ¹â¿ØÖÆ
        [StructLayout(LayoutKind.Sequential)]
        public struct NET_DVR_EXPOSURE
        {
            public byte byExposureMode; /*0 ÊÖ¶¯ÆØ¹â 1×Ô¶¯ÆØ¹â*/
            public byte byAutoApertureLevel; /* ×Ô¶¯¹âÈ¦ÁéÃô¶È, 0-10 */
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
            public byte[] byRes;
            public uint dwVideoExposureSet; /* ×Ô¶¨ÒåÊÓÆµÆØ¹âÊ±¼ä£¨µ¥Î»us£©*//*×¢:×Ô¶¯ÆØ¹âÊ±¸ÃÖµÎªÆØ¹â×îÂýÖµ ÐÂÔö20-1s(1000000us)*/
            public uint dwExposureUserSet; /* ×Ô¶¨ÒåÆØ¹âÊ±¼ä,ÔÚ×¥ÅÄ»úÉÏÓ¦ÓÃÊ±£¬CCDÄ£Ê½Ê±ÊÇ×¥ÅÄ¿ìÃÅËÙ¶È*/
            public uint dwRes;
        }

        //GammaÐ£Õý
        [StructLayout(LayoutKind.Sequential)]
        public struct NET_DVR_GAMMACORRECT
        {
            public byte byGammaCorrectionEnabled; /*0 dsibale  1 enable*/
            public byte byGammaCorrectionLevel; /*0-100*/
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 6)]
            public byte[] byRes;
        }

        //¿í¶¯Ì¬ÅäÖÃ
        [StructLayout(LayoutKind.Sequential)]
        public struct NET_DVR_WDR
        {
            public byte byWDREnabled; /*¿í¶¯Ì¬£º0 dsibale  1 enable 2 auto*/
            public byte byWDRLevel1; /*0-F*/
            public byte byWDRLevel2; /*0-F*/
            public byte byWDRContrastLevel; /*0-100*/
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)]
            public byte[] byRes;
        }

        //ÈÕÒ¹×ª»»¹¦ÄÜÅäÖÃ
        [StructLayout(LayoutKind.Sequential)]
        public struct NET_DVR_DAYNIGHT
        {
            public byte byDayNightFilterType; /*ÈÕÒ¹ÇÐ»»£º0-°×Ìì£¬1-Ò¹Íí£¬2-×Ô¶¯£¬3-¶¨Ê±£¬4-±¨¾¯ÊäÈë´¥·¢*/
            public byte bySwitchScheduleEnabled; /*0 dsibale  1 enable,(±£Áô)*/
            //¶¨Ê±Ä£Ê½²ÎÊý
            public byte byBeginTime; /*¿ªÊ¼Ê±¼ä£¨Ð¡Ê±£©£¬0-23*/
            public byte byEndTime; /*½áÊøÊ±¼ä£¨Ð¡Ê±£©£¬0-23*/
            //Ä£Ê½2
            public byte byDayToNightFilterLevel; //0-7
            public byte byNightToDayFilterLevel; //0-7
            public byte byDayNightFilterTime;//(60Ãë)
                                             //¶¨Ê±Ä£Ê½²ÎÊý
            public byte byBeginTimeMin; //¿ªÊ¼Ê±¼ä£¨·Ö£©£¬0-59
            public byte byBeginTimeSec; //¿ªÊ¼Ê±¼ä£¨Ãë£©£¬0-59
            public byte byEndTimeMin; //½áÊøÊ±¼ä£¨·Ö£©£¬0-59
            public byte byEndTimeSec; //½áÊøÊ±¼ä£¨Ãë£©£¬0-59
                                      //±¨¾¯ÊäÈë´¥·¢Ä£Ê½²ÎÊý
            public byte byAlarmTrigState; //±¨¾¯ÊäÈë´¥·¢×´Ì¬£¬0-°×Ìì£¬1-Ò¹Íí
        }

        //±³¹â²¹³¥ÅäÖÃ
        [StructLayout(LayoutKind.Sequential)]
        public struct NET_DVR_BACKLIGHT
        {
            public byte byBacklightMode; /*±³¹â²¹³¥:0 off 1 UP¡¢2 DOWN¡¢3 LEFT¡¢4 RIGHT¡¢5MIDDLE¡¢6×Ô¶¨Òå£¬10-¿ª£¬11-×Ô¶¯£¬12-¶àÇøÓò±³¹â²¹³¥*/
            public byte byBacklightLevel; /*0x0-0xF*/
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
            public byte[] byRes1;
            public uint dwPositionX1; //£¨X×ø±ê1£©
            public uint dwPositionY1; //£¨Y×ø±ê1£©
            public uint dwPositionX2; //£¨X×ø±ê2£©
            public uint dwPositionY2; //£¨Y×ø±ê2£©
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
            public byte[] byRes2;
        }

        //Êý×Ö½µÔë¹¦ÄÜ
        [StructLayout(LayoutKind.Sequential)]
        public struct NET_DVR_NOISEREMOVE
        {
            public byte byDigitalNoiseRemoveEnable; /*0-²»ÆôÓÃ£¬1-ÆÕÍ¨Ä£Ê½Êý×Ö½µÔë£¬2-×¨¼ÒÄ£Ê½Êý×Ö½µÔë*/
            public byte byDigitalNoiseRemoveLevel; /*ÆÕÍ¨Ä£Ê½Êý×Ö½µÔë¼¶±ð£º0x0-0xF*/
            public byte bySpectralLevel;       /*×¨¼ÒÄ£Ê½ÏÂ¿ÕÓòÇ¿¶È£º0-100*/
            public byte byTemporalLevel;   /*×¨¼ÒÄ£Ê½ÏÂÊ±ÓòÇ¿¶È£º0-100*/
            public byte byDigitalNoiseRemove2DEnable;         /* ×¥ÅÄÖ¡2D½µÔë£¬0-²»ÆôÓÃ£¬1-ÆôÓÃ */
            public byte byDigitalNoiseRemove2DLevel;            /* ×¥ÅÄÖ¡2D½µÔë¼¶±ð£¬0-100 */
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
            public byte[] byRes;
        }

        //CMOSÄ£Ê½ÏÂÇ°¶Ë¾µÍ·ÅäÖÃ
        [StructLayout(LayoutKind.Sequential)]
        public struct NET_DVR_CMOSMODECFG
        {
            public byte byCaptureMod;   //×¥ÅÄÄ£Ê½£º0-×¥ÅÄÄ£Ê½1£»1-×¥ÅÄÄ£Ê½2
            public byte byBrightnessGate;//ÁÁ¶ÈãÐÖµ
            public byte byCaptureGain1;   //×¥ÅÄÔöÒæ1,0-100
            public byte byCaptureGain2;   //×¥ÅÄÔöÒæ2,0-100
            public uint dwCaptureShutterSpeed1;//×¥ÅÄ¿ìÃÅËÙ¶È1
            public uint dwCaptureShutterSpeed2;//×¥ÅÄ¿ìÃÅËÙ¶È2
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
            public byte[] byRes;
        }

        //Í¸Îí
        [StructLayout(LayoutKind.Sequential)]
        public struct NET_DVR_DEFOGCFG
        {
            public byte byMode; //Ä£Ê½£¬0-²»ÆôÓÃ£¬1-×Ô¶¯Ä£Ê½£¬2-³£¿ªÄ£Ê½
            public byte byLevel; //µÈ¼¶£¬0-100
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 6)]
            public byte[] byRes;
        }

        //µç×Ó·À¶¶
        [StructLayout(LayoutKind.Sequential)]
        public struct NET_DVR_ELECTRONICSTABILIZATION
        {
            public byte byEnable;//Ê¹ÄÜ 0- ²»ÆôÓÃ£¬1- ÆôÓÃ
            public byte byLevel; //µÈ¼¶£¬0-100
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 6)]
            public byte[] byRes;
        }

        //×ßÀÈÄ£Ê½
        [StructLayout(LayoutKind.Sequential)]
        public struct NET_DVR_CORRIDOR_MODE_CCD
        {
            public byte byEnableCorridorMode; //ÊÇ·ñÆôÓÃ×ßÀÈÄ£Ê½ 0¡«²»ÆôÓÃ£¬ 1¡«ÆôÓÃ
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 11)]
            public byte[] byRes;
        }

        // SMART IR(·À¹ýÆØ)ÅäÖÃ²ÎÊý
        [StructLayout(LayoutKind.Sequential)]
        public struct NET_DVR_SMARTIR_PARAM
        {
            public byte byMode;//0¡«ÊÖ¶¯£¬1¡«×Ô¶¯
            public byte byIRDistance;//ºìÍâ¾àÀëµÈ¼¶(µÈ¼¶£¬¾àÀëÕý±ÈÀý)level:1~100 Ä¬ÈÏ:50£¨ÊÖ¶¯Ä£Ê½ÏÂÔö¼Ó£©
            public byte byShortIRDistance;// ½ü¹âµÆ¾àÀëµÈ¼¶(1~100)
            public byte byInt32IRDistance;// Ô¶¹âµÆ¾àÀëµÈ¼¶(1~100)
        }

        //ÔÚbyIrisMode ÎªP-Iris1Ê±ÉúÐ§£¬ÅäÖÃºìÍâ¹âÈ¦´óÐ¡µÈ¼¶£¬ÅäÖÃÄ£Ê½
        [StructLayout(LayoutKind.Sequential)]
        public struct NET_DVR_PIRIS_PARAM
        {
            public byte byMode;//0-×Ô¶¯£¬1-ÊÖ¶¯
            public byte byPIrisAperture;//ºìÍâ¹âÈ¦´óÐ¡µÈ¼¶(µÈ¼¶,¹âÈ¦´óÐ¡Õý±ÈÀý)level:1~100 Ä¬ÈÏ:50£¨ÊÖ¶¯Ä£Ê½ÏÂÔö¼Ó£©
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 6)]
            public byte[] byRes;
        }

        //¼¤¹â²ÎÊýÅäÖÃ 2014-02-25
        [StructLayout(LayoutKind.Sequential)]
        public struct NET_DVR_LASER_PARAM_CFG
        {
            //Length = 16
            public byte byControlMode;  //¿ØÖÆÄ£Ê½  	0-ÎÞÐ§£¬1-×Ô¶¯£¬2-ÊÖ¶¯ Ä¬ÈÏ×Ô¶¯
            public byte bySensitivity;  //¼¤¹âµÆÁéÃô¶È  0-100 Ä¬ÈÏ50
            public byte byTriggerMode;  //¼¤¹âµÆ´¥·¢Ä£Ê½	0-ÎÞÐ§£¬1-»úÐ¾´¥·¢£¬2-¹âÃô´¥·¢ Ä¬ÈÏ»úÐ¾´¥·¢
            public byte byBrightness;  //¿ØÖÆÄ£Ê½ÎªÊÖ¶¯Ä£Ê½ÏÂÓÐÐ§£»¼¤¹âµÆÁÁ¶È  0-255 Ä¬ÈÏ100
            public byte byAngle;    //¼¤¹âµÆ½Ç¶È  0-ÎÞÐ§£¬·¶Î§1-36  Ä¬ÈÏ12£¬¼¤¹âµÆÕÕÉä·¶Î§ÎªÒ»¸öÔ²È¦£¬µ÷½Ú¼¤¹â½Ç¶ÈÊÇµ÷½ÚÕâ¸öÔ²µÄ°ë¾¶µÄ´óÐ¡
            public byte byLimitBrightness;  //¿ØÖÆÄ£Ê½Îª×Ô¶¯Ä£Ê½ÏÂÓÐÐ§£»¼¤¹âµÆÁÁ¶ÈÏÞÖÆ 0~100 £¨ÐÂÔö£©2014-01-26
            public byte byEnabled;         //ÊÖ¶¯¿ØÖÆ¼¤¹âµÆÊ¹ÄÜ 0-¹Ø±Õ£¬1-Æô¶¯
            public byte byIllumination;     //¼¤¹âµÆÇ¿¶ÈÅäÖÃ0~100
            public byte byLightAngle;       //²¹¹â½Ç¶È 0~100
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 7)]
            public byte[] byRes;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct NET_DVR_FFC_PARAM
        {
            //1-Schedule Mode,2-Temperature Mode, 3-Off 
            public byte byMode;
            //£¨Ê±¼ä:°´ÄÜÁ¦ÏÔÊ¾£¬µ¥Î»·ÖÖÓ£¬Ñ¡ÏîÓÐ10,20,30,40,50,60,120,180,240£©
            public byte byRes1;
            public ushort wCompensateTime; //¶¨Ê±Ä£Ê½ÏÂÉúÐ§
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
            public byte[] byRes2;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct NET_DVR_DDE_PARAM   //ÔÚsensorÖÐÍê³É
        {
            public byte byMode;//1-Off,2-Normal Mode,3-Expert Mode
            public byte byNormalLevel;//ÆÕÍ¨Ä£Ê½µÈ¼¶·¶Î§[1,100]£¬ÆÕÍ¨Ä£Ê½ÏÂÉúÐ§
            public byte byExpertLevel;//×¨¼ÒÄ£Ê½µÈ¼¶·¶Î§[1,100]£¬×¨¼ÒÄ£Ê½ÏÂÉúÐ§
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 5)]
            public byte[] byRes;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct NET_DVR_AGC_PARAM
        {
            public byte bySceneType;//1-Normal Sence,2-Highlight Sence,3-Manual Sence
            public byte byLightLevel;//ÁÁ¶ÈµÈ¼¶[1,100]£»ÊÖ¶¯Ä£Ê½ÏÂÉúÐ§
            public byte byGainLevel; //ÔöÒæµÈ¼¶[1,100]£»ÊÖ¶¯Ä£Ê½ÏÂÉúÐ§
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 5)]
            public byte[] byRes;
        }

        //×¥ÅÄ»úCCD²ÎÊý ¹²64×Ö½Ú
        [StructLayout(LayoutKind.Sequential)]
        public struct NET_DVR_SNAP_CAMERAPARAMCFG
        {
            public byte byWDRMode;   // ¿í¶¯Ì¬Ä£Ê½;0~¹Ø±Õ£¬1~Êý×Ö¿í¶¯Ì¬ 2~¿í¶¯Ì¬
            public byte byWDRType;    // ¿í¶¯Ì¬ÇÐ»»Ä£Ê½; 0~Ç¿ÖÆÆôÓÃ£¬1~°´Ê±¼äÆôÓÃ£¬2~°´ÁÁ¶ÈÆôÓÃ
            public byte byWDRLevel;   // ¿í¶¯Ì¬µÈ¼¶£¬0~6Ë÷Òý¶ÔÓ¦1-7£¬Ä¬ÈÏË÷Òý2£¨¼´3¼¶£©£»
            public byte byRes1;
            public NET_DVR_TIME_EX struStartTime; //¿ªÊ¼¿í¶¯Ì¬Ê±¼ä
            public NET_DVR_TIME_EX struEndTime; //½áÊø¿í¶¯Ì¬Ê±¼ä
            public byte byDayNightBrightness; //ÈÕÒ¹×ª»»ÁÁ¶ÈãÐÖµ£¬0-100£¬Ä¬ÈÏ50£»
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 43)]
            public byte[] byRes;
        }

        //¹âÑ§Í¸Îí²ÎÊý
        [StructLayout(LayoutKind.Sequential)]
        public struct NET_DVR_OPTICAL_DEHAZE
        {
            public byte byEnable; //0~²»ÆôÓÃ¹âÑ§Í¸Îí£¬1~ÆôÓÃ¹âÑ§Í¸Îí
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 7)]
            public byte[] byRes;
        }

        public const int NET_DVR_GET_ISP_CAMERAPARAMCFG = 3255; //»ñÈ¡ISPÇ°¶Ë²ÎÊýÅäÖÃ
        public const int NET_DVR_SET_ISP_CAMERAPARAMCFG = 3256; //ÉèÖÃISPÇ°¶Ë²ÎÊýÅäÖÃ
        public const int NET_DVR_GET_CCDPARAMCFG_EX = 3368; //»ñÈ¡CCD²ÎÊýÅäÖÃ
        public const int NET_DVR_SET_CCDPARAMCFG_EX = 3369; //ÉèÖÃCCD²ÎÊýÅäÖÃ

        //Ç°¶Ë²ÎÊýÅäÖÃ
        [StructLayout(LayoutKind.Sequential)]
        public struct NET_DVR_CAMERAPARAMCFG_EX
        {
            public uint dwsize;
            public NET_DVR_VIDEOEFFECT struVideoEffect;/*ÁÁ¶È¡¢¶Ô±È¶È¡¢±¥ºÍ¶È¡¢Èñ¶È¡¢É«µ÷ÅäÖÃ*/
            public NET_DVR_GAIN struGain;/*×Ô¶¯ÔöÒæ*/
            public NET_DVR_WHITEBALANCE struWhiteBalance;/*°×Æ½ºâ*/
            public NET_DVR_EXPOSURE struExposure; /*ÆØ¹â¿ØÖÆ*/
            public NET_DVR_GAMMACORRECT struGammaCorrect;/*GammaÐ£Õý*/
            public NET_DVR_WDR struWdr;/*¿í¶¯Ì¬*/
            public NET_DVR_DAYNIGHT struDayNight;/*ÈÕÒ¹×ª»»*/
            public NET_DVR_BACKLIGHT struBackLight;/*±³¹â²¹³¥*/
            public NET_DVR_NOISEREMOVE struNoiseRemove;/*Êý×Ö½µÔë*/
            public byte byPowerLineFrequencyMode; /*0-50HZ; 1-60HZ*/
            public byte byIrisMode; /*0-×Ô¶¯¹âÈ¦ 1-ÊÖ¶¯¹âÈ¦, 2-P-Iris1, 3-Union 3-9mm F1.6-2.7 (T5280-PQ1), 4-Union 2.8-12mm F1.6-2.7 (T5289-PQ1),5-HIK 3.8-16mm F1.5£¨HV3816P-8MPIR£©*/
            public byte byMirror;  /* ¾µÏñ£º0 off£¬1- leftright£¬2- updown£¬3-center 4-Auto*/
            public byte byDigitalZoom;  /*Êý×ÖËõ·Å:0 dsibale  1 enable*/
            public byte byDeadPixelDetect;   /*»µµã¼ì²â,0 dsibale  1 enable*/
            public byte byBlackPwl;/*ºÚµçÆ½²¹³¥ ,  0-255*/
            public byte byEptzGate;// EPTZ¿ª¹Ø±äÁ¿:0-²»ÆôÓÃµç×ÓÔÆÌ¨£¬1-ÆôÓÃµç×ÓÔÆÌ¨
            public byte byLocalOutputGate;//±¾µØÊä³ö¿ª¹Ø±äÁ¿0-±¾µØÊä³ö¹Ø±Õ1-±¾µØBNCÊä³ö´ò¿ª 2-HDMIÊä³ö¹Ø±Õ  
                                          //20-HDMI_720P50Êä³ö¿ª
                                          //21-HDMI_720P60Êä³ö¿ª
                                          //22-HDMI_1080I60Êä³ö¿ª
                                          //23-HDMI_1080I50Êä³ö¿ª
                                          //24-HDMI_1080P24Êä³ö¿ª
                                          //25-HDMI_1080P25Êä³ö¿ª
                                          //26-HDMI_1080P30Êä³ö¿ª
                                          //27-HDMI_1080P50Êä³ö¿ª
                                          //28-HDMI_1080P60Êä³ö¿ª
            public byte byCoderOutputMode;//±àÂëÆ÷fpgaÊä³öÄ£Ê½0Ö±Í¨3ÏñËØ°á¼Ò
            public byte byLineCoding; //ÊÇ·ñ¿ªÆôÐÐ±àÂë£º0-·ñ£¬1-ÊÇ
            public byte byDimmerMode; //µ÷¹âÄ£Ê½£º0-°ë×Ô¶¯£¬1-×Ô¶¯
            public byte byPaletteMode; //µ÷É«°å£º0-°×ÈÈ£¬1-ºÚÈÈ£¬2-µ÷É«°å2£¬¡­£¬8-µ÷É«°å8, 9-ÈÚºÏ1,10-²Êºç,11-ÈÚºÏ2,12-Ìúºì1,13-Ìúºì2,14-ÉîºÖÉ«,15-É«²Ê1,16-É«²Ê2,17-±ù»ð,18-Óê,19-ºìÈÈ,20-ÂÌÈÈ
            public byte byEnhancedMode; //ÔöÇ¿·½Ê½£¨Ì½²âÎïÌåÖÜ±ß£©£º0-²»ÔöÇ¿£¬1-1£¬2-2£¬3-3£¬4-4
            public byte byDynamicContrastEN;    //¶¯Ì¬¶Ô±È¶ÈÔöÇ¿ 0-1
            public byte byDynamicContrast;    //¶¯Ì¬¶Ô±È¶È 0-100
            public byte byJPEGQuality;    //JPEGÍ¼ÏñÖÊÁ¿ 0-100
            public NET_DVR_CMOSMODECFG struCmosModeCfg;//CMOSÄ£Ê½ÏÂÇ°¶Ë²ÎÊýÅäÖÃ£¬¾µÍ·Ä£Ê½´ÓÄÜÁ¦¼¯»ñÈ¡
            public byte byFilterSwitch; //ÂË²¨¿ª¹Ø£º0-²»ÆôÓÃ£¬1-ÆôÓÃ
            public byte byFocusSpeed; //¾µÍ·µ÷½¹ËÙ¶È£º0-10
            public byte byAutoCompensationInterval; //¶¨Ê±×Ô¶¯¿ìÃÅ²¹³¥£º1-120£¬µ¥Î»£º·ÖÖÓ
            public byte bySceneMode;  //³¡¾°Ä£Ê½£º0-ÊÒÍâ£¬1-ÊÒÄÚ£¬2-Ä¬ÈÏ£¬3-Èõ¹â
            public NET_DVR_DEFOGCFG struDefogCfg;//Í¸Îí²ÎÊý
            public NET_DVR_ELECTRONICSTABILIZATION struElectronicStabilization;//µç×Ó·À¶¶
            public NET_DVR_CORRIDOR_MODE_CCD struCorridorMode;//×ßÀÈÄ£Ê½
            public byte byExposureSegmentEnable; //0~²»ÆôÓÃ,1~ÆôÓÃ  ÆØ¹âÊ±¼äºÍÔöÒæ³Ê½×ÌÝ×´µ÷Õû£¬±ÈÈçÆØ¹âÍùÉÏµ÷ÕûÊ±£¬ÏÈÌá¸ßÆØ¹âÊ±¼äµ½ÖÐ¼äÖµ£¬È»ºóÌá¸ßÔöÒæµ½ÖÐ¼äÖµ£¬ÔÙÌá¸ßÆØ¹âµ½×î´óÖµ£¬×îºóÌá¸ßÔöÒæµ½×î´óÖµ
            public byte byBrightCompensate;//ÁÁ¶ÈÔöÇ¿ [0~100]
            /*
             0-¹Ø±Õ¡¢1-640*480@25fps¡¢2-640*480@30ps¡¢3-704*576@25fps¡¢4-704*480@30fps¡¢5-1280*720@25fps¡¢6-1280*720@30fps¡¢
             7-1280*720@50fps¡¢8-1280*720@60fps¡¢9-1280*960@15fps¡¢10-1280*960@25fps¡¢11-1280*960@30fps¡¢
             12-1280*1024@25fps¡¢13--1280*1024@30fps¡¢14-1600*900@15fps¡¢15-1600*1200@15fps¡¢16-1920*1080@15fps¡¢
             17-1920*1080@25fps¡¢18-1920*1080@30fps¡¢19-1920*1080@50fps¡¢20-1920*1080@60fps¡¢21-2048*1536@15fps¡¢22-2048*1536@20fps¡¢
             23-2048*1536@24fps¡¢24-2048*1536@25fps¡¢25-2048*1536@30fps¡¢26-2560*2048@25fps¡¢27-2560*2048@30fps¡¢
             28-2560*1920@7.5fps¡¢29-3072*2048@25fps¡¢30-3072*2048@30fps¡¢31-2048*1536@12.5¡¢32-2560*1920@6.25¡¢
             33-1600*1200@25¡¢34-1600*1200@30¡¢35-1600*1200@12.5¡¢36-1600*900@12.5¡¢37-1280*960@12.5fps¡¢38-800*600@25fps¡¢39-800*600@30fps40¡¢
             4000*3000@12.5fps¡¢41-4000*3000@15fps¡¢42-4096*2160@20fps¡¢43-3840x2160@20fps ¡¢44-960*576@25fps¡¢45-960*480@30fps¡¢46-752*582@25fps¡¢
             47-768*494@30fps¡¢48-2560*1440@25fps¡¢49-2560*1440@30fps ¡¢50-720P@100fps¡¢51-720P@120fps¡¢52-2048*1536@50fps¡¢53-2048*1536@60fps¡¢
             54-3840*2160@25fps¡¢55-3840*2160@30fps¡¢56-4096*2160@25fps¡¢57-4096*2160@30fps ¡¢58-1280*1024@50fps¡¢59-1280*1024@60fps¡¢
             60-3072*2048@50fps¡¢61-3072*2048@60fps¡¢62-3072*1728@25fps¡¢63-3072*1728@30fps¡¢64-3072*1728@50fps¡¢65-3072*1728@60fps¡¢66-336*256@50fps¡¢67-336*256@60fps¡¢
             68-384*288@50fps¡¢69-384*288@60fps ¡¢70- 640 * 512@50fps ¡¢71- 640 * 512@60fps¡¢72-2592*1944@25fps¡¢73-2592*1944@30fps¡¢74-2688*1536@25fps¡¢75-2688*1536@30fps 
             76-2592*1944@20fps¡¢77-2592*1944@15fps¡¢78-2688*1520@20fps¡¢79-2688*1520@15fps¡¢80-2688*1520@25fps¡¢81-2688*1520@30fps¡¢82- 2720*2048@25fps¡¢ 83- 2720*2048@30fps¡¢
             84-336*256@25fps¡¢85- 384*288@25fps¡¢86-640*512@25fps¡¢87-1280*960@50fps¡¢88-1280*960@60fps¡¢89-1280*960@100fps¡¢90-1280*960@120fps¡¢91-4000*3000@20fps¡¢
             92-1920*1200@25fps¡¢93-1920*1200@30fps¡¢94-2560*1920@25fps¡¢95-2560*1920@20fps¡¢96-2560*1920@30fps¡¢97-1280*1920@25fps¡¢98-1280*1920@30fps
             99-4000*3000@24fps¡¢100-4000*3000@25fps¡¢101-4000*3000@10fps¡¢102- 384*288@30fps¡¢103-2560*1920@15fps¡¢104-2400*3840@25fps¡¢105-1200*1920@25fps
             106-4096*1800@30fps¡¢107-3840*1680@30fps¡¢108-2560*1120@30fps¡¢109-704*320@30fps¡¢110-1280*560@30fps¡¢111-4096*1800@25fps¡¢112-3840*1680@25fps
             113-2560*1120@25fps¡¢114-704*320@25fps¡¢115-1280*560@25fps¡¢116-2400*3840@24fps¡¢117-3840*2400@24fps¡¢118-3840*2400@25fps¡¢119-2560*1920@12.5fps
             120-2560*2048@12fps¡¢121-2560*2048@15fps¡¢122-2560*1536@25fps¡¢123-2560*1536@30fps
            */
            public byte byCaptureModeN; //ÊÓÆµÊäÈëÄ£Ê½£¨NÖÆ£©
            public byte byCaptureModeP; //ÊÓÆµÊäÈëÄ£Ê½£¨PÖÆ£©
            public NET_DVR_SMARTIR_PARAM struSmartIRParam; //ºìÍâ·Å¹ý±¬ÅäÖÃÐÅÏ¢
            public NET_DVR_PIRIS_PARAM struPIrisParam;//PIrisÅäÖÃÐÅÏ¢¶ÔÓ¦byIrisMode×Ö¶Î´Ó2-PIris1¿ªÊ¼ÉúÐ§
                                                      //2014-02-25 ÐÂÔö²ÎÊý
            public NET_DVR_LASER_PARAM_CFG struLaserParam;  //¼¤¹â²ÎÊý
            public NET_DVR_FFC_PARAM struFFCParam;
            public NET_DVR_DDE_PARAM struDDEParam;
            public NET_DVR_AGC_PARAM struAGCParam;
            public byte byLensDistortionCorrection;//¾µÍ·»û±äÐ£Õý 0-¹Ø±Õ,1-¿ªÆô
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
            public byte[] byRes1;
            public NET_DVR_SNAP_CAMERAPARAMCFG struSnapCCD; //×¥ÅÄ»úCCD²ÎÊý£¬Ö»ÓÃÓÚ×¥ÅÄ»ú
            public NET_DVR_OPTICAL_DEHAZE struOpticalDehaze;//¹âÑ§Í¸Îí²ÎÊý
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 180)]
            public byte[] byRes2;
        }

        public const int NET_DVR_GET_FOCUSMODECFG = 3305; //»ñÈ¡¿ìÇò¾Û½¹Ä£Ê½ÐÅÏ¢
        public const int NET_DVR_SET_FOCUSMODECFG = 3306; //ÉèÖÃ¿ìÇò¾Û½¹Ä£Ê½ÐÅÏ¢

        [StructLayout(LayoutKind.Sequential)]
        public struct NET_DVR_FOCUSMODE_CFG
        {
            public uint dwSize;
            public byte byFocusMode;  /* ¾Û½¹Ä£Ê½£¬0-×Ô¶¯£¬1-ÊÖ¶¯£¬2-°ë×Ô¶¯ */
            public byte byAutoFocusMode; /* ×Ô¶¯¾Û½¹Ä£Ê½£¬0-¹Ø£¬1-Ä£Ê½A£¬2-Ä£Ê½B£¬3-Ä£Ê½AB£¬4-Ä£Ê½C ×Ô¶¯¾Û½¹Ä£Ê½£¬ÐèÒªÔÚ¾Û½¹Ä£Ê½Îª×Ô¶¯Ê±²ÅÏÔÊ¾*/
            public uint wMinFocusDistance; /* ×îÐ¡¾Û½¹¾àÀë£¬µ¥Î»CM,  0-×Ô¶¯£¬0xffff-ÎÞÇîÔ¶ */
            public byte byZoomSpeedLevel;  /* ±ä±¶ËÙ¶È£¬ÎªÊµ¼ÊÈ¡Öµ£¬1-3 */
            public byte byFocusSpeedLevel; /* ¾Û½¹ËÙ¶È£¬ÎªÊµ¼ÊÈ¡Öµ£¬1-3 */
            public byte byOpticalZoom;  /* ¹âÑ§±ä±¶£¬0-255 */
            public byte byDigtitalZoom;  /* Êý×Ö±ä±¶£¬0-255 */
            public float fOpticalZoomLevel; /* ¹âÑ§±ä±¶(±¶ÂÊÖµ) [1,32], ×îÐ¡¼ä¸ô0.5 £¬ÄÚ²¿Éè±¸½»»¥µÄÊ±ºò*1000 */
            public uint dwFocusPos;/* dwFocusPos ÊÇfocusÖµ£¨¾Û½¹Öµ£©£¬·¶Î§Îª[0x1000,0xC000]£¬Õâ¸öÖµÊÇsony×ø±êÖµ£¬Ê¹ÓÃÕâ¸öÖµÊÇÎªÁË¶ÔÍâÍ³Ò»£¬±£Ö¤²»Í¬µÄ¾µÍ·¶ÔÍâfocusÖµ¶¼×ª»»ÔÚÕâ¸ö·¶Î§ÄÚ (ÊÖ¶¯¾Û½¹Ä£Ê½ÏÂÏÂÓ¦ÓÃ)*/
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 56)]
            public byte[] byRes;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct NET_DVR_ATMFINDINFO
        {
            public byte byTransactionType;       //½»Ò×ÀàÐÍ 0-È«²¿£¬1-²éÑ¯£¬ 2-È¡¿î£¬ 3-´æ¿î£¬ 4-ÐÞ¸ÄÃÜÂë£¬5-×ªÕË£¬ 6-ÎÞ¿¨²éÑ¯ 7-ÎÞ¿¨´æ¿î£¬ 8-ÍÌ³® 9-ÍÌ¿¨ 10-×Ô¶¨Òå
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
            public byte[] byRes;
            public uint dwTransationAmount;     //½»Ò×½ð¶î ;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct NET_DVR_SPECIAL_FINDINFO_UNION
        {
            //[MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
            //public byte[] byLenth;
            public NET_DVR_ATMFINDINFO struATMFindInfo;        //ATM²éÑ¯
        }

        public const int CARDNUM_LEN_OUT = 32;//Íâ²¿½á¹¹Ìå¿¨ºÅ³¤¶È
        public const int GUID_LEN = 16;//GUID³¤¶È

        //[DllImport(@"HCNetSDK.dll")]
        //public static extern bool NET_DVR_GetDiskList(int lUserID, )

        [DllImport(@"HCNetSDK.dll")]
        public static extern Int32 NET_DVR_FindFile_V40(int lUserID, ref NET_DVR_FILECOND_V40 pFindCond);

        [DllImport(@"HCNetSDK.dll")]
        public static extern Int32 NET_DVR_FindNextFile_V40(int lFindHandle, ref NET_DVR_FINDDATA_V40 lpFindData);

        [StructLayout(LayoutKind.Sequential)]
        public struct NET_DVR_FILECOND_V40
        {
            public int lChannel;
            public uint dwFileType;//²»´ø¿¨ºÅÂ¼ÏóÎÄ¼þÀàÐÍ0xff£­È«²¿£¬0£­¶¨Ê±Â¼Ïñ,1-ÒÆ¶¯Õì²â £¬2£­±¨¾¯´¥·¢£¬3-±¨¾¯|ÒÆ¶¯Õì²â 4-±¨¾¯&ÒÆ¶¯Õì²â 5-ÃüÁî´¥·¢ 6-ÊÖ¶¯Â¼Ïñ£¬7-ÖÇÄÜÂ¼Ïñ£¬10-PIR±¨¾¯£¬11-ÎÞÏß±¨¾¯£¬12-ºô¾È±¨¾¯£¬13-È«²¿ÊÂ¼þ£¬14-ÖÇÄÜ½»Í¨ÊÂ¼þ
                                   //´ø¿¨ºÅÂ¼ÏñÀàÐÍ£º 0xff£­È«²¿£¬0£­¶¨Ê±Â¼Ïñ£¬1-ÒÆ¶¯Õì²â£¬2£­½Ó½ü±¨¾¯£¬3£­³ö³®±¨¾¯£¬4£­½ø³®±¨¾¯£¬5-ÃüÁî´¥·¢£¬6£­ÊÖ¶¯Â¼Ïñ£¬7£­Õð¶¯±¨¾¯£¬8-»·¾³±¨¾¯£¬9-ÖÇÄÜ±¨¾¯£¬10-PIR±¨¾¯£¬11-ÎÞÏß±¨¾¯£¬12-ºô¾È±¨¾¯£¬13-È«²¿ÊÂ¼þ£¬14-ÖÇÄÜ½»Í¨ÊÂ¼þ
                                   //15-Ô½½çÕì²â£¬16-ÇøÓòÈëÇÖ£¬17-ÉùÒôÒì³££¬18-³¡¾°±ä¸üÕì²â£¬19-ÖÇÄÜÕì²â£¨Ô½½çÕì²â|ÇøÓòÈëÇÖ|½øÈëÇøÓò|Àë¿ªÇøÓò|ÈËÁ³Ê¶±ð£©20-ÈËÁ³Õì²â  21-ÐÅºÅÁ¿¡¢22-»Ø´«¡¢23-»ØÇ¨Â¼Ïñ¡¢24-ÕÚµ² 25-posÂ¼Ïñ
            public uint dwIsLocked;
            public uint dwUseCardNo;//ÊÇ·ñ´øATMÐÅÏ¢½øÐÐ²éÑ¯£º0-²»´øATMÐÅÏ¢£¬1-°´½»Ò×¿¨ºÅ²éÑ¯£¬2-°´½»Ò×ÀàÐÍ²éÑ¯£¬3-°´½»Ò×½ð¶î²éÑ¯£¬4-°´¿¨ºÅ¡¢½»Ò×ÀàÐÍ¼°½»Ò×½ð¶îµÄ×éºÏ²éÑ¯ 5-°´¿Î³ÌÃû³Æ²éÕÒ£¬´ËÊ±¿¨ºÅ±íÊ¾¿Î³ÌÃû³Æ
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = CARDNUM_LEN_OUT)]
            public byte[] sCardNumber;
            public NET_DVR_TIME struStartTime;
            public NET_DVR_TIME struStopTime;
            public byte byDrawFrame; //0:²»³éÖ¡£¬1£º³éÖ¡
            public byte byFindType; //0:²éÑ¯ÆÕÍ¨¾í£¬1£º²éÑ¯´æµµ¾í
            public byte byQuickSearch; //0:ÆÕÍ¨²éÑ¯£¬1£º¿ìËÙ£¨ÈÕÀú£©²éÑ¯
            public byte bySpecialFindInfoType;    //×¨ÓÐ²éÑ¯Ìõ¼þÀàÐÍ 0-ÎÞÐ§£¬ 1-´øATM²éÑ¯Ìõ¼þ  
            public uint dwVolumeNum;  //´æµµ¾íºÅ
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = GUID_LEN)]
            public byte[] byWorkingDeviceGUID;//¹¤×÷»úGUID£¬Í¨¹ý»ñÈ¡N+1µÃµ½
            public NET_DVR_SPECIAL_FINDINFO_UNION uSpecialFindInfo;   //×¨ÓÐ²éÑ¯Ìõ¼þ
            public byte byStreamType;   //0-Ö÷ÂëÁ÷£¬1-×ÓÂëÁ÷£¬2-ÈýÂëÁ÷£¬0xff-È«²¿
            public byte byAudioFile;    //ÒôÆµÎÄ¼þ 0-·ÇÒôÆµÎÄ¼þ£¬1-ÒôÆµÎÄ¼þ
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 30)]
            public byte[] byRes2;
        }

        //Â¼ÏóÎÄ¼þ²ÎÊý(cvr)
        [StructLayout(LayoutKind.Sequential)]
        public struct NET_DVR_FINDDATA_V40
        {
            [MarshalAsAttribute(UnmanagedType.ByValTStr, SizeConst = 100)]
            public string sFileName;//ÎÄ¼þÃû
            public NET_DVR_TIME struStartTime;//ÎÄ¼þµÄ¿ªÊ¼Ê±¼ä
            public NET_DVR_TIME struStopTime;//ÎÄ¼þµÄ½áÊøÊ±¼ä
            public uint dwFileSize;//ÎÄ¼þµÄ´óÐ¡
            [MarshalAsAttribute(UnmanagedType.ByValTStr, SizeConst = 32)]
            public string sCardNum;
            public byte byLocked;//9000Éè±¸Ö§³Ö,1±íÊ¾´ËÎÄ¼þÒÑ¾­±»Ëø¶¨,0±íÊ¾Õý³£µÄÎÄ¼þ
            public byte byFileType;  //ÎÄ¼þÀàÐÍ:0£­¶¨Ê±Â¼Ïñ,1-ÒÆ¶¯Õì²â £¬2£­±¨¾¯´¥·¢£¬
                                     //3-±¨¾¯|ÒÆ¶¯Õì²â 4-±¨¾¯&ÒÆ¶¯Õì²â 5-ÃüÁî´¥·¢ 6-ÊÖ¶¯Â¼Ïñ,7£­Õð¶¯±¨¾¯£¬8-»·¾³±¨¾¯£¬9-ÖÇÄÜ±¨¾¯£¬10-PIR±¨¾¯£¬11-ÎÞÏß±¨¾¯£¬12-ºô¾È±¨¾¯,14-ÖÇÄÜ½»Í¨ÊÂ¼þ
            public byte byQuickSearch; //0:ÆÕÍ¨²éÑ¯½á¹û£¬1£º¿ìËÙ£¨ÈÕÀú£©²éÑ¯½á¹û
            public byte byRes;
            public uint dwFileIndex; //ÎÄ¼þË÷ÒýºÅ
            public byte byStreamType;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 127)]
            public byte[] byRes1;
        }

        public const int DEV_TYPE_NAME_LEN = 24;

        //DVRÉè±¸²ÎÊý
        [StructLayout(LayoutKind.Sequential)]
        public struct NET_DVR_DEVICECFG_V40
        {
            public uint dwSize;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = NAME_LEN)]
            public byte[] sDVRName;//DVRÃû³Æ
            public uint dwDVRID;    //DVR ID,ÓÃÓÚÒ£¿ØÆ÷ //V1.4(0-99), V1.5(0-255)
            public uint dwRecycleRecord;  //ÊÇ·ñÑ­»·Â¼Ïñ,0:²»ÊÇ; 1:ÊÇ
                                          //ÒÔÏÂ²»¿É¸ü¸Ä
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = SERIALNO_LEN)]
            public byte[] sSerialNumber;//ÐòÁÐºÅ
            public uint dwSoftwareVersion;      //Èí¼þ°æ±¾ºÅ,¸ß16Î»ÊÇÖ÷°æ±¾,µÍ16Î»ÊÇ´Î°æ±¾
            public uint dwSoftwareBuildDate;    //Èí¼þÉú³ÉÈÕÆÚ,0xYYYYMMDD
            public uint dwDSPSoftwareVersion;      //DSPÈí¼þ°æ±¾,¸ß16Î»ÊÇÖ÷°æ±¾,µÍ16Î»ÊÇ´Î°æ±¾
            public uint dwDSPSoftwareBuildDate;  // DSPÈí¼þÉú³ÉÈÕÆÚ,0xYYYYMMDD
            public uint dwPanelVersion;    // Ç°Ãæ°å°æ±¾,¸ß16Î»ÊÇÖ÷°æ±¾,µÍ16Î»ÊÇ´Î°æ±¾
            public uint dwHardwareVersion;  // Ó²¼þ°æ±¾,¸ß16Î»ÊÇÖ÷°æ±¾,µÍ16Î»ÊÇ´Î°æ±¾
            public byte byAlarmInPortNum;  //DVR±¨¾¯ÊäÈë¸öÊý
            public byte byAlarmOutPortNum;  //DVR±¨¾¯Êä³ö¸öÊý
            public byte byRS232Num;     //DVR 232´®¿Ú¸öÊý
            public byte byRS485Num;     //DVR 485´®¿Ú¸öÊý 
            public byte byNetworkPortNum;  //ÍøÂç¿Ú¸öÊý
            public byte byDiskCtrlNum;      //DVR Ó²ÅÌ¿ØÖÆÆ÷¸öÊý
            public byte byDiskNum;    //DVR Ó²ÅÌ¸öÊý
            public byte byDVRType;    //DVRÀàÐÍ, 1:DVR 2:ATM DVR 3:DVS ......
            public byte byChanNum;    //DVR Í¨µÀ¸öÊý
            public byte byStartChan;    //ÆðÊ¼Í¨µÀºÅ,ÀýÈçDVS-1,DVR - 1
            public byte byDecordChans;      //DVR ½âÂëÂ·Êý
            public byte byVGANum;    //VGA¿ÚµÄ¸öÊý 
            public byte byUSBNum;    //USB¿ÚµÄ¸öÊý
            public byte byAuxoutNum;    //¸¨¿ÚµÄ¸öÊý
            public byte byAudioNum;     //ÓïÒô¿ÚµÄ¸öÊý
            public byte byIPChanNum;    //×î´óÊý×ÖÍ¨µÀÊý µÍ8Î»£¬¸ß8Î»¼ûbyHighIPChanNum 
            public byte byZeroChanNum;      //ÁãÍ¨µÀ±àÂë¸öÊý
            public byte bySupport;        //ÄÜÁ¦£¬Î»Óë½á¹ûÎª0±íÊ¾²»Ö§³Ö£¬1±íÊ¾Ö§³Ö£¬
                                          //bySupport & 0x1, ±íÊ¾ÊÇ·ñÖ§³ÖÖÇÄÜËÑË÷
                                          //bySupport & 0x2, ±íÊ¾ÊÇ·ñÖ§³Ö±¸·Ý
                                          //bySupport & 0x4, ±íÊ¾ÊÇ·ñÖ§³ÖÑ¹Ëõ²ÎÊýÄÜÁ¦»ñÈ¡
                                          //bySupport & 0x8, ±íÊ¾ÊÇ·ñÖ§³Ö¶àÍø¿¨
                                          //bySupport & 0x10, ±íÊ¾Ö§³ÖÔ¶³ÌSADP
                                          //bySupport & 0x20, ±íÊ¾Ö§³ÖRaid¿¨¹¦ÄÜ
                                          //bySupport & 0x40, ±íÊ¾Ö§³ÖIPSANËÑË÷
                                          //bySupport & 0x80, ±íÊ¾Ö§³Örtp over rtsp
            public byte byEsataUseage;  //EsataµÄÄ¬ÈÏÓÃÍ¾£¬0-Ä¬ÈÏ±¸·Ý£¬1-Ä¬ÈÏÂ¼Ïñ
            public byte byIPCPlug;      //0-¹Ø±Õ¼´²å¼´ÓÃ£¬1-´ò¿ª¼´²å¼´ÓÃ
            public byte byStorageMode;  //0-ÅÌ×éÄ£Ê½,1-´ÅÅÌÅä¶î, 2³éÖ¡Ä£Ê½
            public byte bySupport1;  //ÄÜÁ¦£¬Î»Óë½á¹ûÎª0±íÊ¾²»Ö§³Ö£¬1±íÊ¾Ö§³Ö
                                     //bySupport1 & 0x1, ±íÊ¾ÊÇ·ñÖ§³Ösnmp v30
                                     //bySupport1 & 0x2, Ö§³ÖÇø·Ö»Ø·ÅºÍÏÂÔØ
                                     //bySupport1 & 0x4, ÊÇ·ñÖ§³Ö²¼·ÀÓÅÏÈ¼¶	
                                     //bySupport1 & 0x8, ÖÇÄÜÉè±¸ÊÇ·ñÖ§³Ö²¼·ÀÊ±¼ä¶ÎÀ©Õ¹
                                     //bySupport1 & 0x10, ±íÊ¾ÊÇ·ñÖ§³Ö¶à´ÅÅÌÊý£¨³¬¹ý33¸ö£©
                                     //bySupport1 & 0x20, ±íÊ¾ÊÇ·ñÖ§³Örtsp over http	
            public ushort wDevType;//Éè±¸ÐÍºÅ
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = DEV_TYPE_NAME_LEN)]
            public byte[] byDevTypeName;//Éè±¸ÐÍºÅÃû³Æ 
            public byte bySupport2; //ÄÜÁ¦¼¯À©Õ¹£¬Î»Óë½á¹ûÎª0±íÊ¾²»Ö§³Ö£¬1±íÊ¾Ö§³Ö
                                    //bySupport2 & 0x1, ±íÊ¾ÊÇ·ñÖ§³ÖÀ©Õ¹µÄOSD×Ö·ûµþ¼Ó(ÖÕ¶ËºÍ×¥ÅÄ»úÀ©Õ¹Çø·Ö)
            public byte byAnalogAlarmInPortNum; //Ä£Äâ±¨¾¯ÊäÈë¸öÊý
            public byte byStartAlarmInNo;    //Ä£Äâ±¨¾¯ÊäÈëÆðÊ¼ºÅ
            public byte byStartAlarmOutNo;  //Ä£Äâ±¨¾¯Êä³öÆðÊ¼ºÅ
            public byte byStartIPAlarmInNo;  //IP±¨¾¯ÊäÈëÆðÊ¼ºÅ
            public byte byStartIPAlarmOutNo; //IP±¨¾¯Êä³öÆðÊ¼ºÅ
            public byte byHighIPChanNum;      //Êý×ÖÍ¨µÀ¸öÊý£¬¸ß8Î» 
            public byte byEnableRemotePowerOn;//ÊÇ·ñÆôÓÃÔÚÉè±¸ÐÝÃßµÄ×´Ì¬ÏÂÔ¶³Ì¿ª»ú¹¦ÄÜ£¬0-²»ÆôÓÃ£¬1-ÆôÓÃ
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
            public byte[] byRes2;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct NET_DVR_DEVICEINFO_V40
        {
            public NET_DVR_DEVICEINFO_V30 struDeviceV30;
            public byte bySupportLock;        //Éè±¸Ö§³ÖËø¶¨¹¦ÄÜ£¬¸Ã×Ö¶ÎÓÉSDK¸ù¾ÝÉè±¸·µ»ØÖµÀ´¸³ÖµµÄ¡£bySupportLockÎª1Ê±£¬dwSurplusLockTimeºÍbyRetryLoginTimeÓÐÐ§
            public byte byRetryLoginTime;       //Ê£Óà¿É³¢ÊÔµÇÂ½µÄ´ÎÊý£¬ÓÃ»§Ãû£¬ÃÜÂë´íÎóÊ±£¬´Ë²ÎÊýÓÐÐ§
            public byte byPasswordLevel;      //adminÃÜÂë°²È«µÈ¼¶0-ÎÞÐ§£¬1-Ä¬ÈÏÃÜÂë£¬2-ÓÐÐ§ÃÜÂë,3-·çÏÕ½Ï¸ßµÄÃÜÂë¡£µ±ÓÃ»§µÄÃÜÂëÎª³ö³§Ä¬ÈÏÃÜÂë£¨12345£©»òÕß·çÏÕ½Ï¸ßµÄÃÜÂëÊ±£¬ÉÏ²ã¿Í»§¶ËÐèÒªÌáÊ¾ÓÃ»§¸ü¸ÄÃÜÂë¡£      
            public byte byRes1;
            public uint dwSurplusLockTime;  //Ê£ÓàÊ±¼ä£¬µ¥Î»Ãë£¬ÓÃ»§Ëø¶¨Ê±£¬´Ë²ÎÊýÓÐÐ§
            public byte byCharEncodeType;     //×Ö·û±àÂëÀàÐÍ
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 255)]
            public byte[] byRes2;
        }

        public const int NET_DVR_DEV_ADDRESS_MAX_LEN = 129;
        public const int NET_DVR_LOGIN_USERNAME_MAX_LEN = 64;
        public const int NET_DVR_LOGIN_PASSWD_MAX_LEN = 64;

        public delegate void LOGINRESULTCALLBACK(Int32 lUserID, UInt32 dwResult, IntPtr lpDeviceInfo, IntPtr pUser);

        [StructLayout(LayoutKind.Sequential)]
        public struct NET_DVR_USER_LOGIN_INFO
        {
            [MarshalAsAttribute(UnmanagedType.ByValTStr, SizeConst = NET_DVR_DEV_ADDRESS_MAX_LEN)]
            public string sDeviceAddress;
            public byte byRes1;
            public ushort wPort;
            [MarshalAsAttribute(UnmanagedType.ByValTStr, SizeConst = NET_DVR_LOGIN_USERNAME_MAX_LEN)]
            public string sUserName;
            [MarshalAsAttribute(UnmanagedType.ByValTStr, SizeConst = NET_DVR_LOGIN_PASSWD_MAX_LEN)]
            public string sPassword;
            public LOGINRESULTCALLBACK cbLoginResult;
            public IntPtr pUser;
            public bool bUseAsynLogin;
            public byte byUseUTCTime;   //0-²»½øÐÐ×ª»»£¬Ä¬ÈÏ,1-½Ó¿ÚÉÏÊäÈëÊä³öÈ«²¿Ê¹ÓÃUTCÊ±¼ä,SDKÍê³ÉUTCÊ±¼äÓëÉè±¸Ê±ÇøµÄ×ª»»,2-½Ó¿ÚÉÏÊäÈëÊä³öÈ«²¿Ê¹ÓÃÆ½Ì¨±¾µØÊ±¼ä£¬SDKÍê³ÉÆ½Ì¨±¾µØÊ±¼äÓëÉè±¸Ê±ÇøµÄ×ª»»
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 127)]
            public byte[] byRes2;
        }



        [DllImport(@"HCNetSDK.dll")]
        public static extern int NET_DVR_Login_V40(ref NET_DVR_USER_LOGIN_INFO pLoginInfo, ref NET_DVR_DEVICEINFO_V40 lpDeviceInfo);

        const int XML_ABILITY_OUT_LEN = 3 * 1024 * 1024;

        [StructLayout(LayoutKind.Sequential)]
        public struct NET_DVR_STD_ABILITY
        {
            public IntPtr lpCondBuffer; //[in]Ìõ¼þ²ÎÊý(Âë×Ö¸ñÊ½),ÀýÈçÍ¨µÀºÅµÈ.¿ÉÒÔÎªNULL
            public uint dwCondSize;     //[in] dwCondSizeÖ¸ÏòµÄÄÚ´æ´óÐ¡
            public IntPtr lpOutBuffer;  //[out]Êä³ö²ÎÊý(XML¸ñÊ½),²»ÎªNULL
            public uint dwOutSize;      //[in] lpOutBufferÖ¸ÏòµÄÄÚ´æ´óÐ¡
            public IntPtr lpStatusBuffer;   //[out]·µ»ØµÄ×´Ì¬²ÎÊý(XML¸ñÊ½),»ñÈ¡³É¹¦Ê±²»»á¸³Öµ,Èç¹û²»ÐèÒª,¿ÉÒÔÖÃNULL
            public uint dwStatusSize;   //[in] lpStatusBufferÖ¸ÏòµÄÄÚ´æ´óÐ¡
            public uint dwRetSize;      //[out]»ñÈ¡µ½µÄÊý¾Ý³¤¶È(lpOutBuffer»òÕßlpStatusBufferÖ¸ÏòµÄÊµ¼ÊÊý¾Ý³¤¶È)
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
            public byte[] byRes;        //±£Áô×Ö½Ú
        }

        [DllImport(@"HCNetSDK.dll")]
        public static extern bool NET_DVR_GetSTDAbility(int lUserID, uint dwAbilityType, IntPtr lpAbilityParam);

        [StructLayout(LayoutKind.Sequential)]
        public struct NET_DVR_XML_CONFIG_INPUT
        {
            public uint dwSize;                    //½á¹¹Ìå´óÐ¡
            public IntPtr lpRequestUrl;                //ÇëÇóÐÅÁî£¬×Ö·û´®¸ñÊ½
            public uint dwRequestUrlLen;            //ÇëÇóÐÅÁî³¤¶È£¬×Ö·û´®³¤¶È
            public IntPtr lpInBuffer;                //ÊäÈë²ÎÊý»º³åÇø£¬XML¸ñÊ½
            public uint dwInBufferSize;            //ÊäÈë²ÎÊý»º³åÇø´óÐ¡
            public uint dwRecvTimeOut;                //½ÓÊÕ³¬Ê±Ê±¼ä£¬µ¥Î»£ºms£¬Ìî0ÔòÊ¹ÓÃÄ¬ÈÏ³¬Ê±5s
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
            public byte[] byRes;        //±£Áô×Ö½Ú
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct NET_DVR_XML_CONFIG_OUTPUT
        {
            public uint dwSize;                    //½á¹¹Ìå´óÐ¡
            public IntPtr lpOutBuffer;                //Êä³ö²ÎÊý»º³åÇø£¬XML¸ñÊ½
            public uint dwOutBufferSize;            //Êä³ö²ÎÊý»º³åÇø´óÐ¡(ÄÚ´æ´óÐ¡)
            public uint dwReturnedXMLSize;            //Êµ¼ÊÊä³öµÄXMLÄÚÈÝ´óÐ¡
            public IntPtr lpStatusBuffer;            //·µ»ØµÄ×´Ì¬²ÎÊý(XML¸ñÊ½),»ñÈ¡ÃüÁî³É¹¦Ê±²»»á¸³Öµ,Èç¹û²»ÐèÒª,¿ÉÒÔÖÃNULL
            public uint dwStatusSize;                //×´Ì¬»º³åÇø´óÐ¡(ÄÚ´æ´óÐ¡)
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
            public byte[] byRes;        //±£Áô×Ö½Ú
        }

        [DllImport(@"HCNetSDK.dll")]
        public static extern bool NET_DVR_STDXMLConfig(int lUserID, IntPtr lpInputParam, IntPtr lpOutputParam);

        [DllImport("kernel32.dll")]
        public static extern int WinExec(string exeName, int operType);

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        public struct NET_DVR_STD_CONFIG
        {
            public IntPtr lpCondBuffer;    //[in]Ìõ¼þ²ÎÊý(½á¹¹Ìå¸ñÊ½),ÀýÈçÍ¨µÀºÅµÈ.¿ÉÒÔÎªNULL
            public uint dwCondSize;        //[in] dwCondSizeÖ¸ÏòµÄÄÚ´æ´óÐ¡
            public IntPtr lpInBuffer;        //[in]byDataType = 0Ê±ÓÐÐ§,ÊäÈë²ÎÊý(½á¹¹Ìå¸ñÊ½),ÉèÖÃÊ±²»ÎªNULL£¬»ñÈ¡Ê±ÎªNULL
            public uint dwInSize;        //[in] lpInBufferÖ¸ÏòµÄÄÚ´æ´óÐ¡
            public IntPtr lpOutBuffer;    //[out]byDataType = 0Ê±ÓÐÐ§,Êä³ö²ÎÊý(½á¹¹Ìå¸ñÊ½),»ñÈ¡Ê±²»ÎªNULL,ÉèÖÃÊ±ÎªNULL
            public uint dwOutSize;        //[in] lpOutBufferÖ¸ÏòµÄÄÚ´æ´óÐ¡
            public IntPtr lpStatusBuffer;    //[out]·µ»ØµÄ×´Ì¬²ÎÊý(XML¸ñÊ½),»ñÈ¡³É¹¦Ê±²»»á¸³Öµ,Èç¹û²»ÐèÒª,¿ÉÒÔÖÃNULL
            public uint dwStatusSize;    //[in] lpStatusBufferÖ¸ÏòµÄÄÚ´æ´óÐ¡
            public IntPtr lpXmlBuffer;    //[in/out]byDataType = 1Ê±ÓÐÐ§,xml¸ñÊ½Êý¾Ý
            public uint dwXmlSize;      //[in/out]lpXmlBufferÖ¸ÏòµÄÄÚ´æ´óÐ¡,»ñÈ¡Ê±Í¬Ê±×÷ÎªÊäÈëºÍÊä³ö²ÎÊý£¬»ñÈ¡³É¹¦ºó»áÐÞ¸Ä»áÊµ¼Ê³¤¶È£¬ÉèÖÃÊ±±íÊ¾Êµ¼Ê³¤¶È£¬¶ø²»ÊÇÕû¸öÄÚ´æ´óÐ¡
            public byte byDataType;     //[in]ÊäÈë/Êä³ö²ÎÊýÀàÐÍ,0-Ê¹ÓÃ½á¹¹ÌåÀàÐÍlpInBuffer/lpOutBufferÓÐÐ§,1-Ê¹ÓÃXMLÀàÐÍlpXmlBufferÓÐÐ§
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 23)]
            public byte[] byRes1;       //±£Áô×Ö½Ú
                                        //              uint            dwRetSize;        //»ñÈ¡µ½µÄÊý¾Ý³¤¶È(lpOutBuffer»òÕßlpStatusBufferÖ¸ÏòµÄÊµ¼ÊÊý¾Ý³¤¶È)
                                        //              [MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)]
                                        //              public byte[]   byRes2;		//±£Áô×Ö½Ú
        }

        [DllImport(@"HCNetSDK.dll")]
        public static extern bool NET_DVR_GetSTDConfig(int lUserID, uint dwCommand, IntPtr lpConfigParam);

        [DllImport(@"HCNetSDK.dll")]
        public static extern bool NET_DVR_SetSTDConfig(int lUserID, uint dwCommand, IntPtr lpConfigParam);

        public const int ALARM_INFO_T = 0;
        public const int OPERATION_SUCC_T = 1;
        public const int OPERATION_FAIL_T = 2;
        public const int PLAY_SUCC_T = 3;
        public const int PLAY_FAIL_T = 4;

        public const int MAX_AUDIO_V40 = 8;
        public const int NET_DVR_USER_LOCKED = 153;//ÓÃ»§±»Ëø¶¨

        public enum DEMO_CHANNEL_TYPE
        {
            DEMO_CHANNEL_TYPE_INVALID = -1,
            DEMO_CHANNEL_TYPE_ANALOG = 0,
            DEMO_CHANNEL_TYPE_IP = 1,
            DEMO_CHANNEL_TYPE_MIRROR = 2
        }

        //device index info
        [StructLayout(LayoutKind.Sequential)]
        public struct STRU_CHANNEL_INFO
        {
            public int iDeviceIndex;    //device index
            public int iChanIndex;      //channel index
            public DEMO_CHANNEL_TYPE iChanType;
            public int iChannelNO;         //channel NO.       
            [MarshalAsAttribute(UnmanagedType.ByValTStr, SizeConst = 100)]
            public string chChanName;         //channel name
            public uint dwProtocol;     //network protocol
            public uint dwStreamType; //ÂëÁ÷ÀàÐÍ£¬0-Ö÷ÂëÁ÷£¬1-×ÓÂëÁ÷£¬2-ÂëÁ÷3£¬
            public uint dwLinkMode;//ÂëÁ÷Á¬½Ó·½Ê½: 0£ºTCP·½Ê½,1£ºUDP·½Ê½,2£º¶à²¥·½Ê½,3 - RTP·½Ê½£¬4-RTP/RTSP,5-RSTP/HTTP
            public bool bPassbackRecord; //0-²»ÆôÓÃÂ¼Ïñ»Ø´«,1ÆôÓÃÂ¼Ïñ»Ø´«
            public uint dwPreviewMode;  //Ô¤ÀÀÄ£Ê½ 0-Õý³£Ä£Ê½ 1-ÑÓÊ±Ô¤ÀÀ
            public int iPicResolution;    //resolution
            public int iPicQuality;    //image quality
            public Int32 lRealHandle;          //preview handle
            public bool bLocalManualRec;     //manual record
            public bool bAlarm;    //alarm
            public bool bEnable;    //enable
            public uint dwImageType;  //channel status icon
            [MarshalAsAttribute(UnmanagedType.ByValTStr, SizeConst = 16)]
            public string chAccessChanIP;   //ip addr of IP channel
            public uint nPreviewProtocolType;   //È¡Á÷Ð­ÒéÀàÐÍ 0-Ë½ÓÐÐ­Òé 1-RTSPÐ­Òé
            public IntPtr pNext;

            public void init()
            {
                iDeviceIndex = -1;
                iChanIndex = -1;
                iChannelNO = -1;
                iChanType = DEMO_CHANNEL_TYPE.DEMO_CHANNEL_TYPE_INVALID;
                chChanName = null;
                dwProtocol = 0;

                dwStreamType = 0;
                dwLinkMode = 0;

                iPicResolution = 0;
                iPicQuality = 2;

                lRealHandle = -1;
                bLocalManualRec = false;
                bAlarm = false;
                bEnable = false;
                dwImageType = CHAN_ORIGINAL;
                chAccessChanIP = null;
                pNext = IntPtr.Zero;
                dwPreviewMode = 0;
                bPassbackRecord = false;
                nPreviewProtocolType = 0;
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct NET_DVR_STREAM_MODE
        {
            public byte byGetStreamType; //È¡Á÷·½Ê½GET_STREAM_TYPE£¬0-Ö±½Ó´ÓÉè±¸È¡Á÷£¬1-´ÓÁ÷Ã½ÌåÈ¡Á÷¡¢2-Í¨¹ýIPServer»ñµÃipµØÖ·ºóÈ¡Á÷,3.Í¨¹ýIPServerÕÒµ½Éè±¸£¬ÔÙÍ¨¹ýÁ÷Ã½ÌåÈ¥Éè±¸µÄÁ÷
                                         //4-Í¨¹ýÁ÷Ã½ÌåÓÉURLÈ¥È¡Á÷,5-Í¨¹ýhkDDNSÈ¡Á÷£¬6-Ö±½Ó´ÓÉè±¸È¡Á÷(À©Õ¹)£¬Ê¹ÓÃNET_DVR_IPCHANINFO_V40½á¹¹, 7-Í¨¹ýRTSPÐ­Òé·½Ê½½øÐÐÈ¡Á÷
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 3, ArraySubType = UnmanagedType.I1)]
            public byte[] byRes;
            public NET_DVR_GET_STREAM_UNION uGetStream;    // ²»Í¬È¡Á÷·½Ê½½á¹¹Ìå
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct NET_DVR_IPSERVER_STREAM
        {
            public byte byEnable;   // ÊÇ·ñÔÚÏß
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 3, ArraySubType = UnmanagedType.I1)]
            public byte[] byRes;
            public NET_DVR_IPADDR struIPServer;    //IPServer µØÖ·
            public ushort wPort;                  //IPServer ¶Ë¿Ú
            public ushort wDvrNameLen;            // DVR Ãû³Æ³¤¶È
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = NAME_LEN, ArraySubType = UnmanagedType.I1)]
            public byte[] byDVRName;              // DVRÃû³Æ
            public ushort wDVRSerialLen;          // ÐòÁÐºÅ³¤¶È
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 4, ArraySubType = UnmanagedType.I1)]
            public byte[] byRes1;
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = SERIALNO_LEN, ArraySubType = UnmanagedType.I1)]
            public byte[] byDVRSerialNumber;      // DVRÐòÁÐºÅ³¤¶È
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = NAME_LEN, ArraySubType = UnmanagedType.I1)]
            public byte[] byUserName;             // DVR µÇÂ½ÓÃ»§Ãû
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = PASSWD_LEN, ArraySubType = UnmanagedType.I1)]
            public byte[] byPassWord;             // DVRµÇÂ½ÃÜÂë
            public byte byChannel;              // DVR Í¨µÀ
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 11, ArraySubType = UnmanagedType.I1)]
            public ushort[] byRes2;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct NET_DVR_DDNS_STREAM_CFG
        {
            public byte byEnable;   // ÊÇ·ñÆôÓÃ
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 3, ArraySubType = UnmanagedType.I1)]
            public byte[] byRes;
            public NET_DVR_IPADDR struStreamServer;
            public ushort wStreamServerPort;           //Á÷Ã½Ìå·þÎñÆ÷¶Ë¿Ú   
            public byte byStreamServerTransmitType;  //Á÷Ã½Ìå´«ÊäÐ­ÒéÀàÐÍ 0-TCP£¬1-UDP
            public byte byRes2;
            public NET_DVR_IPADDR struIPServer;          //IPSERVERµØÖ·
            public ushort wIPServerPort;        //IPserver¶Ë¿ÚºÅ
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 2, ArraySubType = UnmanagedType.I1)]
            public byte[] byRes3;
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = NAME_LEN, ArraySubType = UnmanagedType.I1)]
            public byte[] sDVRName;             //DVRÃû³Æ 
            public ushort wDVRNameLen;            // DVRÃû³Æ³¤¶È
            public ushort wDVRSerialLen;          // ÐòÁÐºÅ³¤¶È
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = SERIALNO_LEN, ArraySubType = UnmanagedType.I1)]
            public byte[] sDVRSerialNumber;       // DVRÐòÁÐºÅ
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = NAME_LEN, ArraySubType = UnmanagedType.I1)]
            public byte[] sUserName;              // DVR µÇÂ½ÓÃ»§Ãû
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = PASSWD_LEN, ArraySubType = UnmanagedType.I1)]
            public byte[] sPassWord;              // DVRµÇÂ½ÃÜÂë
            public ushort wDVRPort;   //DVR¶Ë¿ÚºÅ
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 2, ArraySubType = UnmanagedType.I1)]
            public byte[] byRes4;
            public byte byChannel;              // DVR Í¨µÀ
            public byte byTransProtocol; //´«ÊäÐ­ÒéÀàÐÍ0-TCP£¬1-UDP
            public byte byTransMode; //´«ÊäÂëÁ÷Ä£Ê½ 0£­Ö÷ÂëÁ÷ 1£­×ÓÂëÁ÷
            public byte byFactoryType; //Ç°¶ËÉè±¸³§¼ÒÀàÐÍ,Í¨¹ý½Ó¿Ú»ñÈ¡
        }

        public const int URL_LEN = 240;  //URL³¤¶È

        [StructLayout(LayoutKind.Sequential)]
        public struct NET_DVR_PU_STREAM_URL
        {
            public byte byEnable;
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = URL_LEN, ArraySubType = UnmanagedType.I1)]
            public byte[] strURL;
            public byte byTransPortocol; // ´«ÊäÐ­ÒéÀàÐÍ 0-tcp  1-UDP
            public ushort wIPID;  //Éè±¸IDºÅ£¬wIPID = iDevInfoIndex + iGroupNO*64 +1
            public byte byChannel;  //Í¨µÀºÅ
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 7)]
            public byte[] byRes;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct NET_DVR_HKDDNS_STREAM
        {
            public byte byEnable;
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 3, ArraySubType = UnmanagedType.I1)]
            public byte[] byRes;
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 64, ArraySubType = UnmanagedType.I1)]
            public byte[] byDDNSDomain;   // hiDDNS·þÎñÆ÷
            public ushort wPort;                  // hiDDNS ¶Ë¿Ú
            public ushort wAliasLen;              // ±ðÃû³¤¶È
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = NAME_LEN, ArraySubType = UnmanagedType.I1)]
            public byte[] byAlias;              // ±ðÃû
            public ushort wDVRSerialLen;          // ÐòÁÐºÅ³¤¶È
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 2, ArraySubType = UnmanagedType.I1)]
            public byte[] byRes1;
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = SERIALNO_LEN, ArraySubType = UnmanagedType.I1)]
            public byte[] byDVRSerialNumber;     // DVRÐòÁÐºÅ
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = NAME_LEN, ArraySubType = UnmanagedType.I1)]
            public byte[] byUserName;              // DVR µÇÂ½ÓÃ»§Ãû
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = PASSWD_LEN, ArraySubType = UnmanagedType.I1)]
            public byte[] byPassWord;
            public byte byChannel;              // DVRÍ¨µÀ
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 11, ArraySubType = UnmanagedType.I1)]
            public byte[] byRes2;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct NET_DVR_IPCHANINFO_V40
        {
            public byte byEnable;
            public byte byRes1;
            public ushort wIPID;                  //IPÉè±¸ID
            public uint dwChannel;    //Í¨µÀºÅ
            public byte byTransProtocol;  //´«ÊäÐ­ÒéÀàÐÍ0-TCP£¬1-UDP
            public byte byTransMode;    //´«ÊäÂëÁ÷Ä£Ê½ 0£­Ö÷ÂëÁ÷ 1£­×ÓÂëÁ÷
            public byte byFactoryType;      /*Ç°¶ËÉè±¸³§¼ÒÀàÐÍ,Í¨¹ý½Ó¿Ú»ñÈ¡*/
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 241, ArraySubType = UnmanagedType.I1)]
            public byte[] byRes;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct NET_DVR_GET_STREAM_UNION
        {
            public NET_DVR_IPCHANINFO struChanInfo; /*IPÍ¨µÀÐÅÏ¢*/
            public NET_DVR_IPSERVER_STREAM struIPServerStream;  // IPServerÈ¥Á÷
            public NET_DVR_PU_STREAM_CFG struPUStream;     //  Í¨¹ýÇ°¶ËÉè±¸»ñÈ¡Á÷Ã½ÌåÈ¥Á÷
            public NET_DVR_DDNS_STREAM_CFG struDDNSStream;     //Í¨¹ýIPServerºÍÁ÷Ã½ÌåÈ¡Á÷
            public NET_DVR_PU_STREAM_URL struStreamUrl;        //Í¨¹ýÁ÷Ã½Ìåµ½urlÈ¡Á÷
            public NET_DVR_HKDDNS_STREAM struHkDDNSStream;   //Í¨¹ýhiDDNSÈ¥È¡Á÷
            public NET_DVR_IPCHANINFO_V40 struIPChan; //Ö±½Ó´ÓÉè±¸È¡Á÷£¨À©Õ¹£©
        }

        public const int MAX_IP_DEVICE_V40 = 64;// ÔÊÐí½ÓÈëµÄ×î´óIPÉè±¸Êý ×î¶à¿ÉÌí¼Ó64¸ö IVMS 2000µÈÐÂÉè±¸
                                                // Í¨µÀ×ÊÔ´ÅäÖÃ (NET_DVR_IPPARACFG_V40½á¹¹)
        public const int NET_DVR_GET_IPPARACFG_V40 = 1062;// »ñÈ¡IP½ÓÈëÅäÖÃ
        public const int NET_DVR_SET_IPPARACFG_V40 = 1063;// ÉèÖÃIP½ÓÈëÅäÖÃ

        [StructLayout(LayoutKind.Sequential)]
        public struct NET_DVR_IPPARACFG_V40
        {
            public uint dwSize;
            public uint dwGroupNum;     //	 Éè±¸Ö§³ÖµÄ×Ü×éÊý    
            public uint dwAChanNum;     //×î´óÄ£ÄâÍ¨µÀ¸öÊý
            public uint dwDChanNum;                  //Êý×ÖÍ¨µÀ¸öÊý
            public uint dwStartDChan;              //ÆðÊ¼Êý×ÖÍ¨µÀ
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = MAX_CHANNUM_V30)]
            public byte[] byAnalogChanEnable;       /* Ä£ÄâÍ¨µÀÊÇ·ñÆôÓÃ£¬´ÓµÍµ½¸ß±íÊ¾1-64Í¨µÀ£¬0±íÊ¾ÎÞÐ§ 1ÓÐÐ§ */
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = MAX_IP_DEVICE_V40)]
            public NET_DVR_IPDEVINFO_V31[] struIPDevInfo;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = MAX_CHANNUM_V30)]
            public NET_DVR_STREAM_MODE[] struStreamMode;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 20)]
            public byte[] byRes2;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct NET_DVR_IPALARMININFO_V40
        {
            public uint dwIPID;     /* IPÉè±¸ID */
            public uint dwAlarmIn;    /* IPÉè±¸ID¶ÔÓ¦µÄ±¨¾¯ÊäÈëºÅ */
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
            public byte[] byRes;
        }

        public const int MAX_IP_ALARMIN_V40 = 4096;//ÔÊÐí¼ÓÈëµÄ×î¶à±¨¾¯ÊäÈëÊý

        [StructLayout(LayoutKind.Sequential)]
        public struct NET_DVR_IPALARMINCFG_V40
        {
            public uint dwSize;  //½á¹¹Ìå³¤¶È
            public uint dwCurIPAlarmInNum;  //µ±Ç°±¨¾¯ÊäÈë¿ÚÊý
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = MAX_IP_ALARMIN_V40)]
            public NET_DVR_IPALARMININFO_V40[] struIPAlarmInInfo; /* IP±¨¾¯ÊäÈë*/
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 256)]
            public byte[] byRes;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct NET_DVR_IPALARMOUTINFO_V40
        {
            public uint dwIPID;                 /* IPÉè±¸ID */
            public uint dwAlarmOut;    /* IPÉè±¸ID¶ÔÓ¦µÄ±¨¾¯Êä³öºÅ */
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
            public byte[] byRes;
        }
        public const int NET_DVR_CONTROL_PTZ_PATTERN = 3313;
        public const int DELETE_CRUISE = 45;
        public const int DELETE_ALL_CRUISE = 46;
        public const int STOP_CRUISE = 44;
        public struct NET_DVR_PTZ_PATTERN
        {
            public uint dwSize;
            public uint dwChannel; //Í¨µÀºÅ
            public uint dwPatternCmd; //ÔÆÌ¨¹ì¼£²Ù×÷ÃüÁîÂë,Ïê¼ûÏÂÃæ¶¨Òå
            public uint dwPatternID; //ÔÆÌ¨¹ì¼£ID£¨É¾³ýËùÓÐ¹ì¼£Ê±ÎÞÐ§£©
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 64, ArraySubType = UnmanagedType.I1)]
            public byte[] byRes;
        }
        // ¸¨ÖúÉè±¸Éý¼¶Ìõ¼þ½á¹¹Ìå
        public struct NET_DVR_AUXILIARY_DEV_UPGRADE_PARAM
        {
            public uint dwSize;
            public uint dwDevNo;
            public byte byDevType;
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 131, ArraySubType = UnmanagedType.I1)]
            public byte[] byRes;
        }
        public enum ENUM_UPGRADE_TYPE
        {
            ENUM_UPGRADE_DVR = 0, //ÆÕÍ¨Éè±¸Éý¼¶
            ENUM_UPGRADE_ADAPTER = 1, //DVRÊÊÅäÆ÷Éý¼¶
            ENUM_UPGRADE_VCALIB = 2,  //ÖÇÄÜ¿âÉý¼¶
            ENUM_UPGRADE_OPTICAL = 3, //¹â¶Ë»úÉý¼¶
            ENUM_UPGRADE_ACS = 4, //ÃÅ½ûÏµÍ³Éý¼¶
            ENUM_UPGRADE_AUXILIARY_DEV = 5,//¸¨ÖúÉè±¸Éý¼¶
        }
        public const int MAX_IP_ALARMOUT_V40 = 4096;//ÔÊÐí¼ÓÈëµÄ×î¶à±¨¾¯Êä³öÊý

        [StructLayout(LayoutKind.Sequential)]
        public struct NET_DVR_IPALARMOUTCFG_V40
        {
            public uint dwSize;  //½á¹¹Ìå³¤¶È
            public uint dwCurIPAlarmOutNum;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = MAX_IP_ALARMOUT_V40)]
            public NET_DVR_IPALARMOUTINFO_V40[] struIPAlarmOutInfo; /* IP±¨¾¯Êä³ö*/
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 256)]
            public byte[] byRes;
        }

        //2011-12-16 ±»¶¯½âÂë½á¹¹ silujie
        [StructLayout(LayoutKind.Sequential)]
        public struct PASSIVEDECODE_CHANINFO
        {
            public Int32 lPassiveHandle;
            public Int32 lRealHandle;
            public Int32 lUserID;
            public Int32 lSel;
            public IntPtr hFileThread;
            public IntPtr hFileHandle;
            public IntPtr hExitThread;
            public IntPtr hThreadExit;
            public string strRecordFilePath;

            public void init()
            {
                lPassiveHandle = -1;
                lRealHandle = -1;
                lUserID = -1;
                lSel = -1;
                hFileThread = IntPtr.Zero;
                hFileHandle = IntPtr.Zero;
                hExitThread = IntPtr.Zero;
                hThreadExit = IntPtr.Zero;
                strRecordFilePath = null;
            }
        }

        public const int NET_DVR_GET_DEVICECFG_V40 = 1100;//»ñÈ¡À©Õ¹Éè±¸²ÎÊý 
        public const int NET_DVR_SET_DEVICECFG_V40 = 1101;//ÉèÖÃÀ©Õ¹Éè±¸²ÎÊý

        public const int MAX_DEVICES = 512;//max device number
                                           //bmp status
        public const int TREE_ALL = 0;//device list	
        public const int DEVICE_LOGOUT = 1;//device not log in
        public const int DEVICE_LOGIN = 2;//device login
        public const int DEVICE_FORTIFY = 3;//on guard
        public const int DEVICE_ALARM = 4;//alarm on device
        public const int DEVICE_FORTIFY_ALARM = 5;//onguard & alarm on device
        public const int CHAN_ORIGINAL = 6;//no preview, no record
        public const int CHAN_PLAY = 7;   //preview
        public const int CHAN_RECORD = 8;   //record
        public const int CHAN_PLAY_RECORD = 9;   //preview and record

        public const int CHAN_ALARM = 10;   //no preview, no record, only alarm
        public const int CHAN_PLAY_ALARM = 11;   //review, no record, with alarm info
        public const int CHAN_PLAY_RECORD_ALARM = 12;   //preview & record & alarm
        public const int CHAN_OFF_LINE = 13;     //channel off-line

        [StructLayout(LayoutKind.Sequential)]
        public struct PREVIEW_IFNO
        {
            public int iDeviceIndex;    //device index
            public int iChanIndex;      //channel index
            public byte PanelNo;
            public int lRealHandle;
            public IntPtr hPlayWnd;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct STRU_DEVICE_INFO
        {
            public int iDeviceIndex;    //device index
            public Int32 lLoginID;    //ID
            public uint dwDevSoftVer;   //erserved
            [MarshalAsAttribute(UnmanagedType.ByValTStr, SizeConst = 100)]
            public string chLocalNodeName;      //local device node
            [MarshalAsAttribute(UnmanagedType.ByValTStr, SizeConst = 100)]
            public string chDeviceName;         //device name
            [MarshalAsAttribute(UnmanagedType.ByValTStr, SizeConst = 130)]
            public string chDeviceIP;           //device IP: IP,pppoe address, or network gate address, etc
            [MarshalAsAttribute(UnmanagedType.ByValTStr, SizeConst = 130)]
            public string chDeviceIPInFileName; //if chDeviceIP includes ':'(IPV6),change it to '.', for '.'is not allowed in window's file name
            [MarshalAsAttribute(UnmanagedType.ByValTStr, SizeConst = NAME_LEN)]
            public string chLoginUserName;      //login user name
            [MarshalAsAttribute(UnmanagedType.ByValTStr, SizeConst = NAME_LEN)]
            public string chLoginPwd;           //password
            [MarshalAsAttribute(UnmanagedType.ByValTStr, SizeConst = 130)]
            public string chDeviceMultiIP;      //multi-cast group address
            [MarshalAsAttribute(UnmanagedType.ByValTStr, SizeConst = 50)]
            public string chSerialNumber;       //SN
            public int iDeviceChanNum;      //channel numder  (analog + ip)
            public int iStartChan;    //start channel number
            public int iStartDChan;    //start IP channel number
            public int iDeviceType;     //device type
            public int iDiskNum;    //HD number
            public Int32 lDevicePort;   //port number
            public int iAlarmInNum;     //alarm in number
            public int iAlarmOutNum;    //alarm out number
            public int iAudioNum;    //voice talk number
            public int iAnalogChanNum;      //analog channel number
            public int iIPChanNum;    //IP channel number
            public int iGroupNO;               //IP Group NO.
            public bool bCycle;     //if this device is on cycle recording
            public bool bPlayDevice;    //will be added later
            public bool bVoiceTalk;    //on voice talkor not
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = MAX_AUDIO_V40)]
            public Int32[] lAudioHandle;         //voicebroadcast handle
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = MAX_AUDIO_V40)]
            public bool[] bCheckBroadcast;      //Add to broad cast group
            public Int32 lFortifyHandle;    //on guard handle
            public bool bAlarm;     //whether there is alarm
            public int iDeviceLoginType;  //register mode  0 - fix IP   1- IPSERVER mode    2 -  domain name 
            public uint dwImageType;    //device status icon
            public bool bIPRet;     //support IP conection
            public int iEnableChanNum;      //valid channel number
            public bool bDVRLocalAllRec;  //local recording
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = MAX_AUDIO_V40)]
            public Int32[] lVoiceCom;            //voice transmit
            public Int32 lFirstEnableChanIndex;     //first enabled channel index
            public Int32 lTranHandle;    //232 transparent channel handle
            public byte byZeroChanNum;  //Zero channel number
            public byte byMainProto;    //main stream protocol type 0-Private, 1-rtp/tcp, 2-rtp/rtsp
            public byte bySubProto;    //sub stream protocol type 0-Private, 1-rtp/tcp, 2-rtp/rtsp
            public byte bySupport;             //ability
            public byte byStartDTalkChan;
            public byte byAudioInputChanNum;
            public byte byStartAudioInputChanNo;
            public byte byLanguageType;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = HCNetSDK.MAX_CHANNUM_V40)]
            public STRU_CHANNEL_INFO[] pStruChanInfo; //channel structure
            public NET_DVR_IPPARACFG_V40[] pStruIPParaCfgV40;    //IP channel parameters
            public NET_DVR_IPALARMINCFG struAlarmInCfg;
            public NET_DVR_IPALARMINCFG_V40 pStruIPAlarmInCfgV40;  // IP alarm In parameters
            public NET_DVR_IPALARMOUTCFG_V40 pStruIPAlarmOutCfgV40; // IP alarm Out parameters
            public NET_DVR_IPALARMOUTCFG struAlarmOutCfg;
            public IntPtr pNext;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)]
            public STRU_CHANNEL_INFO[] struZeroChan;
            [MarshalAsAttribute(UnmanagedType.ByValTStr, SizeConst = 16)]
            public string sSecretKey;
            public int iAudioEncType;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 256)]
            public PASSIVEDECODE_CHANINFO[] struPassiveDecode;
            public byte bySupport1; //ÄÜÁ¦£¬Î»Óë½á¹ûÎª0±íÊ¾²»Ö§³Ö£¬1±íÊ¾Ö§³Ö
                                    // bySupport1 & 0x1, ±íÊ¾ÊÇ·ñÖ§³Ösnmp v30
                                    //bySupport1 & 0x2, Ö§³ÖÇø·Ö»Ø·ÅºÍÏÂÔØ	
                                    //bySupport1 & 0x4, ÊÇ·ñÖ§³Ö²¼·ÀÓÅÏÈ¼¶	
                                    //bySupport1 & 0x8, ÖÇÄÜÉè±¸ÊÇ·ñÖ§³Ö²¼·ÀÊ±¼ä¶ÎÀ©Õ¹
                                    //bySupport1 & 0x10, ±íÊ¾ÊÇ·ñÖ§³Ö¶à´ÅÅÌÊý£¨³¬¹ý33¸ö£©
                                    //bySupport1 & 0x40 ±íÊ¾ÊÇ·ñÖ§³ÖÑÓ³ÙÔ¤ÀÀ
            public byte bySupport2; //ÄÜÁ¦¼¯À©Õ¹£¬Î»Óë½á¹ûÎª0±íÊ¾²»Ö§³Ö£¬1±íÊ¾Ö§³Ö
                                    //bySupport2 & 0x1, ±íÊ¾½âÂëÆ÷ÊÇ·ñÖ§³ÖÍ¨¹ýURLÈ¡Á÷½âÂë
                                    //bySupport2 & 0x2,  ±íÊ¾Ö§³ÖFTPV40
                                    //bySupport2 & 0x4,  ±íÊ¾Ö§³ÖANR(¶ÏÍøÂ¼Ïñ)
            public byte byStartIPAlarmOutNo;  //ÆðÊ¼IP±¨¾¯Êä³öºÅ
            public byte byMirrorChanNum; //¾µÏñÍ¨µÀ¸öÊý£¬<Â¼²¥Ö÷»úÖÐÓÃÓÚ±íÊ¾µ¼²¥Í¨µÀ>
            public ushort wStartMirrorChanNo;  //ÆðÊ¼¾µÏñÍ¨µÀºÅ
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)]
            public STRU_CHANNEL_INFO[] struMirrorChan;
            public byte bySupport5;
            public byte bySupport7;// bySupport7 & 0x2  ±íÊ¾Éè±¸ÊÇ·ñÖ§³Ö IPC HVT Ä£Ê½À©Õ¹
            public byte byCharaterEncodeType;
            public bool bInit;
            public byte byPanelNo; //

            public void Init()
            {
                iGroupNO = -1;
                iDeviceIndex = -1;
                lLoginID = -1;
                dwDevSoftVer = 0;
                chLocalNodeName = null;
                chDeviceName = null;
                chDeviceIP = null;
                chDeviceIPInFileName = null;
                //chDevNetCard1IP[0]	= '\0';
                chLoginUserName = null;
                chLoginPwd = null;
                chDeviceMultiIP = null;
                chSerialNumber = null;
                iDeviceChanNum = -1;
                iStartChan = 0;
                iDeviceType = 0;
                iDiskNum = 0;
                lDevicePort = 8000;
                iAlarmInNum = 0;
                iAlarmOutNum = 0;
                iAnalogChanNum = 0;
                iIPChanNum = 0;
                byAudioInputChanNum = 0;
                byStartAudioInputChanNo = 0;
                bCycle = false;
                bPlayDevice = false;
                bVoiceTalk = false;
                lAudioHandle = new Int32[MAX_AUDIO_V40];
                bCheckBroadcast = new bool[MAX_AUDIO_V40];
                lFortifyHandle = -1;
                bAlarm = false;
                iDeviceLoginType = 0;
                dwImageType = DEVICE_LOGOUT;
                bIPRet = false;
                pNext = IntPtr.Zero;
                lVoiceCom = new Int32[MAX_AUDIO_V40];
                for (int i = 0; i < lVoiceCom.Length; i++)
                {
                    lVoiceCom[i] = -1;
                }
                lFirstEnableChanIndex = 0;
                lTranHandle = -1;
                byZeroChanNum = 0;
                lAudioHandle[0] = -1;
                lAudioHandle[1] = -1;
                struAlarmInCfg = new NET_DVR_IPALARMINCFG();
                struAlarmOutCfg = new NET_DVR_IPALARMOUTCFG();
                sSecretKey = "StreamNotEncrypt";
                iAudioEncType = 0;
                bySupport1 = 0;
                bySupport2 = 0;
                bySupport5 = 0;
                bySupport7 = 0;
                byStartDTalkChan = 0;
                byLanguageType = 0;
                byCharaterEncodeType = 0;
                bInit = true;
                byPanelNo = 4;
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct NET_DVR_SERIALSTART_V40   //Í¸Ã÷Í¨µÀ
        {
            public uint dwSize;         //½á¹¹Ìå´óÐ¡
            public uint dwSerialType;    //´®¿ÚºÅ£¨1-232´®¿Ú£¬2-485´®¿Ú£©
            public byte bySerialNum;   //´®¿Ú±àºÅ
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 255, ArraySubType = UnmanagedType.I1)]
            public byte[] byRes;
        }

        public delegate void CallBackSerialData(int lSerialHandle, int lCHannel, IntPtr pRecvDataBuffer, uint dwBufSize, IntPtr pUser);
        [DllImport(@"HCNetSDK.dll")]
        public static extern int NET_DVR_SerialStart_V40(int lUserID, IntPtr lpInBuffer, int dwInBufferSize, CallBackSerialData fSerialDataCallBack, IntPtr pUser);

        [DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true)]
        public static extern IntPtr GetForegroundWindow();

        [DllImport(@"HCNetSDK.dll")]
        public static extern bool NET_DVR_PlayBackControl_V40(int lPlayHandle, int dwControlCode,
            IntPtr lpInBuffer, uint dwInLen, IntPtr lpOutBuffer, IntPtr lpOutLen);


        public delegate bool CHARENCODECONVERT(string strInput, uint dwInputLen, uint dwInEncodeType, string strOutput, uint dwOutputLen, uint dwOutEncodeType);

        [StructLayout(LayoutKind.Sequential)]
        public struct NET_DVR_LOCAL_BYTE_ENCODE_CONVERT
        {
            CHARENCODECONVERT fnCharConvertCallBack;
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 256, ArraySubType = UnmanagedType.I1)]
            public byte[] byRes;
        }

        public enum CHAR_ENCODE_TYPE
        {
            ENUM_MEM_CHAR_ENCODE_ERR = -1,         //Error   
            ENUM_MEM_CHAR_ENCODE_NO = 0,          //Don't know.
            ENUM_MEM_CHAR_ENCODE_CN = 1,          //EUC-CN, GB2312
            ENUM_MEM_CHAR_ENCODE_GBK = 2,          //GBK
            ENUM_MEM_CHAR_ENCODE_BIG5 = 3,          //BIG5
            ENUM_MEM_CHAR_ENCODE_JP = 4,          //JISX0208-1, EUC-JP
            ENUM_MEM_CHAR_ENCODE_KR = 5,          //EUC-KR
            ENUM_MEM_CHAR_ENCODE_UTF8 = 6,          //UTF-8
            ENUM_MEM_CHAR_ENCODE_ISO8859_1 = 7,    //ISO-8859-n: ENUM_MEM_CHAR_ENCODE_ISO8859_1 + n -1
        }

        [DllImport("kernel32.dll")]
        public static extern int MultiByteToWideChar(int CodePage, int dwFlags, string lpMultiByteStr, int cchMultiByte, [MarshalAs(UnmanagedType.LPWStr)] string lpWideCharStr, int cchWideChar);

        [DllImport("Kernel32.dll")]
        public static extern int WideCharToMultiByte(uint CodePage, uint dwFlags, [In, MarshalAs(UnmanagedType.LPWStr)] string lpWideCharStr, int cchWideChar,
        [Out, MarshalAs(UnmanagedType.LPStr)] StringBuilder lpMultiByteStr, int cbMultiByte, IntPtr lpDefaultChar, // Defined as IntPtr because in most cases is better to pass
        IntPtr lpUsedDefaultChar);


        public enum NET_SDK_LOCAL_CFG_TYPE
        {
            NET_SDK_LOCAL_CFG_TYPE_TCP_PORT_BIND = 0,  //±¾µØTCP¶Ë¿Ú°ó¶¨ÅäÖÃ£¬¶ÔÓ¦½á¹¹ÌåNET_DVR_LOCAL_TCP_PORT_BIND_CFG
            NET_SDK_LOCAL_CFG_TYPE_UDP_PORT_BIND,   //±¾µØUDP¶Ë¿Ú°ó¶¨ÅäÖÃ£¬¶ÔÓ¦½á¹¹ÌåNET_DVR_LOCAL_UDP_PORT_BIND_CFG
            NET_SDK_LOCAL_CFG_TYPE_MEM_POOL,    //ÄÚ´æ³Ø±¾µØÅäÖÃ£¬¶ÔÓ¦½á¹¹ÌåNET_DVR_LOCAL_MEM_POOL_CFG
            NET_SDK_LOCAL_CFG_TYPE_MODULE_RECV_TIMEOUT,  //°´Ä£¿éÅäÖÃ³¬Ê±Ê±¼ä£¬¶ÔÓ¦½á¹¹ÌåNET_DVR_LOCAL_MODULE_RECV_TIMEOUT_CFG
            NET_SDK_LOCAL_CFG_TYPE_ABILITY_PARSE,   //ÊÇ·ñÊ¹ÓÃÄÜÁ¦¼¯½âÎö¿â£¬¶ÔÓ¦½á¹¹ÌåNET_DVR_LOCAL_ABILITY_PARSE_CFG
            NET_SDK_LOCAL_CFG_TYPE_TALK_MODE,    //¶Ô½²Ä£Ê½£¬¶ÔÓ¦½á¹¹ÌåNET_DVR_LOCAL_TALK_MODE_CFG
            NET_SDK_LOCAL_CFG_TYPE_PROTECT_KEY,    //ÃÜÔ¿ÉèÖÃ£¬¶ÔÓ¦½á¹¹ÌåNET_DVR_LOCAL_PROTECT_KEY_CFG
            NET_SDK_LOCAL_CFG_TYPE_CFG_VERSION,             //ÓÃÓÚ²âÊÔ°æ±¾Í·µÄÉè±¸¶Ë¼æÈÝÇé¿ö, Ö»ÓÐÔÚÉèÖÃ²ÎÊýÊ±²ÅÆð×÷ÓÃ¡£
            NET_SDK_LOCAL_CFG_TYPE_RTSP_PARAMS,    //rtsp²ÎÊýÅäÖÃ£¬¶ÔÓÚ½á¹¹ÌåNET_DVR_RTSP_PARAMS_CFG
            NET_SDK_LOCAL_CFG_TYPE_SIMXML_LOGIN,            //ÔÚµÇÂ¼Ê±Ê¹ÓÃÄ£ÄâÄÜÁ¦²¹³äsupport×Ö¶Î, ¶ÔÓ¦½á¹¹NET_DVR_SIMXML_LOGIN
            NET_SDK_LOCAL_CFG_TYPE_CHECK_DEV,                //ÐÄÌø½»»¥¼ä¸ôÊ±¼ä
            NET_SDK_LOCAL_CFG_TYPE_SECURITY,                  //SDK±¾´Î°²È«ÅäÖÃ£¬
            NET_SDK_LOCAL_CFG_TYPE_EZVIZLIB_PATH,            //ÅäÖÃÓ©Ê¯ÔÆÍ¨ÐÅ¿âµØÖ·£¬
            NET_SDK_LOCAL_CFG_TYPE_CHAR_ENCODE,               //13.ÅäÖÃ×Ö·û±àÂëÏà¹Ø´¦Àí»Øµ÷
            NET_SDK_LOCAL_CFG_TYPE_PROXYS                     //ÉèÖÃ»ñÈ¡´ú
        }
        //ÉèÖÃSDK±¾µØ²ÎÊý
        [DllImport(@"HCNetSDK.dll")]
        public static extern bool NET_DVR_SetSDKLocalCfg(NET_SDK_LOCAL_CFG_TYPE enumType, IntPtr lpInBuff);

        public const int CP_UTF8 = 65001;
        public const int CP_ACP = 0;

        // ÃÀ·Ö¶¨ÖÆ²Ëµ¥Êä³öÄ£Ê½Íâ²¿ÃüÁî
        public const int NET_DVR_GET_MEMU_OUTPUT_MODE = 155649;// »ñÈ¡²Ëµ¥Êä³öÄ£Ê½
        public const int NET_DVR_SET_MEMU_OUTPUT_MODE = 155650;// ÉèÖÃ²Ëµ¥Êä³öÄ£Ê½

        [StructLayout(LayoutKind.Sequential)]
        public struct NET_DVR_MENU_OUTPUT_MODE
        {
            public uint dwSize;
            public byte byMenuOutputMode; //·ÇÍ¬Ô´Éè±¸£º0-Auto 1-Ö÷CVBS 2-HDMI 3-VGA Í¬Ô´Éè±¸£º0-Auto 1-Ö÷CVBS 2-HDMI/VGA
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 63, ArraySubType = UnmanagedType.I1)]
            public byte[] byRes;
        }

        //default image parameter
        public const int DEFAULT_BRIGHTNESS = 6;        //default brightness
        public const int DEFAULT_CONTRAST = 6;          //default contrast
        public const int DEFAULT_SATURATION = 6;        //default saturation
        public const int DEFAULT_HUE = 6;               //default hue
        public const int DEFAULT_SHARPNESS = 6;         //default sharpness
        public const int DEFAULT_DENOISING = 6;         //default denoising
        public const int DEFAULT_VOLUME = 50;           //default volume
        public const int MAX_OUTPUTS = 512;

        [StructLayout(LayoutKind.Sequential)]
        public struct VIDEO_INFO
        {
            public uint m_iBrightness;              //1-10
            public uint m_iContrast;                //1-10
            public uint m_iSaturation;              //1-10
            public uint m_iHue;                     //1-10
            public uint m_iSharpness;
            public uint m_iDenoising;
            public void Init()
            {
                m_iBrightness = DEFAULT_BRIGHTNESS;
                m_iContrast = DEFAULT_CONTRAST;
                m_iSaturation = DEFAULT_SATURATION;
                m_iHue = DEFAULT_HUE;
                m_iSharpness = DEFAULT_SHARPNESS;
                m_iDenoising = DEFAULT_DENOISING;
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct LOCAL_RECORD_TIME
        {
            public ushort iStartHour;
            public ushort iStartMinute;
            public ushort iStopHour;
            public ushort iStopMinute;
            public ushort iStartTime;
            public ushort iStopTime;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct StruDomain
        {
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 64, ArraySubType = UnmanagedType.I1)]
            public byte[] szDomain;//·þÎñÆ÷µØÖ·£¬ÓòÃû 
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 80, ArraySubType = UnmanagedType.I1)]
            public byte[] byRes1;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct StruAddrIP
        {
            public NET_DVR_IPADDR struIp;/*IPµØÖ·*/       //IPv4 IPv6µØÖ·, 144×Ö½Ú
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct UnionServer
        {
            public StruDomain struDomain;
            public StruAddrIP stryAddrIP;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct NET_DVR_SLAVECAMERA_CFG
        {
            public uint dwSize;
            public byte byAddressType;   //0-Êµ¼Êipv4 ipv6µØÖ· 1-ÓòÃû
                                         // ¿ØÖÆunionServerÊÇÊ¹ÓÃÊµ¼ÊµØÖ·»¹ÊÇÓòÃû
            public ushort wPort;            /*¶Ë¿Ú*/
            public byte byLoginStatus; /*´ÓÉè±¸µÄµÇÂ½×´Ì¬ 0-logout,1-login*/
            public UnionServer unionServer;//Ê¹ÓÃÁªºÏÌå½á¹¹£¬Í¨¹ýbyAddressType×Ö¶Î±íÊ¾ÊÇIPµØÖ·»¹ÊÇÓòÃû 64	
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = NAME_LEN, ArraySubType = UnmanagedType.I1)]
            public byte[] szUserName;/*ÓÃ»§Ãû*/
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = PASSWD_LEN, ArraySubType = UnmanagedType.I1)]
            public byte[] szPassWord;/*ÃÜÂë*/
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 128, ArraySubType = UnmanagedType.I1)]
            public byte[] byRes1;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct STRU_LOCAL_PARAM
        {
            public bool bReconnect;                 //reconnect
            public bool bCyclePlay;                 //cycle play
            public int iCycleTime;                  //cycle time, default 20
            public bool bUseCard;                   //hadrware decode
            public bool bNTSC;                      //hardware decode mode,FALSE,PAL;TRUE,NTSC;default as pal
            public bool bAutoRecord;                //auto record;
            public bool bCycleRecord;               //cycle record
            public int iStartRecordDriver;          //client record starting HD dirve
            public int iEndRecordDriver;            //client record stop HD drive
            public int iRecFileSize;                //record file size
            public int iRecordFileInterval;         //record file interval
            [MarshalAsAttribute(UnmanagedType.ByValTStr, SizeConst = 100)]
            public string chDownLoadPath;           //remote file download directory
            [MarshalAsAttribute(UnmanagedType.ByValTStr, SizeConst = 100)]
            public string chPictureSavePath;        //image capture directory
            [MarshalAsAttribute(UnmanagedType.ByValTStr, SizeConst = 100)]
            public string chRemoteCfgSavePath;      //remote config file saving directory
            [MarshalAsAttribute(UnmanagedType.ByValTStr, SizeConst = 100)]
            public string chClientRecordPath;       //client record path
            public bool bAutoCheckDeviceTime;       //check time with device
            public Int32 lCheckDeviceTime;          //check time interval
            public int iAlarmDelayTime;             //alarm delay time
            public int iAlarmListenPort;
            public bool bAutoSaveLog;               //auto save local log info
            public bool bAlarmInfo;                 //display alarm info on log list
            public bool bSuccLog;                   //display log access on log list
            public bool bFailLog;                   //display filaure operation on log list
            public bool bAllDiskFull;               //HD full

            //preview
            public bool bPlaying;                   //on playing
            public bool bCycling;                   //cycle playing
            public bool bPaused;                    //cycle pause
            public bool bNextPage;                  //next page
            public bool bFrontPage;                 //previous page
            public bool bEnlarged;                  //enlarge image
            public bool bFullScreen;                //full screen
            public bool bMultiScreen;               //multi-split-window full screen
            public bool bNoDecode;                  //soft decode or not
            public bool bPreviewBlock;              //preview block or not

            [MarshalAs(UnmanagedType.ByValArray, SizeConst = MAX_OUTPUTS)]
            public VIDEO_INFO[] struVideoInfo;              //video parameter
            public int iVolume;                     //volume
            public bool bBroadCast;                 //voice broadcast
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 128)]
            public string chIPServerIP;
            public bool bOutputDebugString;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 28)]
            public LOCAL_RECORD_TIME[] struLocalRecordTime;
            public uint dwBFrameNum;//throw B frame number
            public uint nLogLevel;
            public bool bCycleWriteLog;
            public uint nTimeout;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
            public NET_DVR_SLAVECAMERA_CFG[] struSlaveCameraCfg;
            public bool bInit;
            public void Init()
            {
                bReconnect = true;
                bCyclePlay = false;
                iCycleTime = 20;
                bUseCard = false;
                bNTSC = false;
                bAutoRecord = false;
                bCycleRecord = false;
                iStartRecordDriver = 0;
                iEndRecordDriver = 0;
                iRecFileSize = 1;
                iRecordFileInterval = 60;
                chDownLoadPath = "C:\\DownLoad";
                chPictureSavePath = "C:\\Picture";
                chRemoteCfgSavePath = "C:\\SaveRemoteCfgFile";
                chClientRecordPath = "C:\\mpeg4record\\2008-04-30";
                chIPServerIP = "172.7.28.123";

                bAutoCheckDeviceTime = false;
                lCheckDeviceTime = 0;

                iAlarmDelayTime = 10;
                iAlarmListenPort = 7200;

                bAutoSaveLog = true;
                bAlarmInfo = true;
                bSuccLog = true;
                bFailLog = true;

                bAllDiskFull = false;
                bPlaying = false;
                bCycling = false;
                bPaused = false;
                bNextPage = false;
                bFrontPage = false;
                bEnlarged = false;
                bFullScreen = false;
                bMultiScreen = false;
                iVolume = DEFAULT_VOLUME;
                bBroadCast = false;
                bNoDecode = false;
                bPreviewBlock = true;
                bOutputDebugString = false;
                dwBFrameNum = 0;
                nLogLevel = 3;
                bCycleWriteLog = false;
                nTimeout = 5000;
                struVideoInfo = new VIDEO_INFO[MAX_OUTPUTS];
                for (int i = 0; i < MAX_OUTPUTS; i++)
                {
                    struVideoInfo[i].Init();
                }
                struLocalRecordTime = new LOCAL_RECORD_TIME[28];
                struSlaveCameraCfg = new NET_DVR_SLAVECAMERA_CFG[4];
                bInit = true;
            }
        }
        [StructLayout(LayoutKind.Sequential)]
        public struct NET_DVR_VOD_PARA
        {
            public uint dwSize;
            public NET_DVR_STREAM_INFO struIDInfo;
            public NET_DVR_TIME struBeginTime;
            public NET_DVR_TIME struEndTime;
            public IntPtr hWnd;
            public byte byDrawFrame; //0:²»³éÖ¡£¬1£º³éÖ¡
            public byte byVolumeType;  //0-ÆÕÍ¨Â¼Ïñ¾í  1-´æµµ¾í
            public byte byVolumeNum;  //¾íºÅ£¬Ä¿Ç°Ö¸´æµµ¾íºÅ
            public byte byStreamType;   //ÂëÁ÷ÀàÐÍ 0-Ö÷ÂëÁ÷£¬ 1-×ÓÂëÁ÷£¬2-ÂëÁ÷Èý
            public uint dwFileIndex;    //´æµµ¾íÉÏµÄÂ¼ÏñÎÄ¼þË÷Òý£¬ËÑË÷´æµµ¾íÂ¼ÏñÊ±·µ»ØµÄÖµ
            public byte byAudioFile;    //ÒôÆµÎÄ¼þ0-·ñ£¬1-ÊÇ
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 23, ArraySubType = UnmanagedType.I1)]
            public byte[] byRes2;
            public void Init()
            {
                struBeginTime = new NET_DVR_TIME();
                struEndTime = new NET_DVR_TIME();
                struIDInfo = new NET_DVR_STREAM_INFO();
                struIDInfo.byID = new byte[STREAM_ID_LEN];
                hWnd = IntPtr.Zero;
            }
        }

        [StructLayoutAttribute(LayoutKind.Sequential)]
        public struct NET_DVR_PLAYCOND
        {
            public uint dwChannel;
            public NET_DVR_TIME struStartTime;
            public NET_DVR_TIME struStopTime;
            public byte byDrawFrame;  //0:²»³éÖ¡£¬1£º³éÖ¡
            public byte byStreamType; //ÂëÁ÷ÀàÐÍ£¬0-Ö÷ÂëÁ÷ 1-×ÓÂëÁ÷ 2-ÂëÁ÷Èý
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = HCNetSDK.STREAM_ID_LEN, ArraySubType = UnmanagedType.I1)]
            public byte[] byStreamID;
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 30, ArraySubType = UnmanagedType.I1)]
            public byte[] byRes;    //±£Áô
        }


        [DllImport(@"HCNetSDK.dll")]
        public static extern Int32 NET_DVR_PlayBackByTime_V40(uint lUserID, ref NET_DVR_VOD_PARA pVodPara);

        [DllImport(@"HCNetSDK.dll")]
        public static extern Int32 NET_DVR_PlayBackReverseByTime_V40(uint lUserID, IntPtr hWnd, ref HCNetSDK.NET_DVR_PLAYCOND pPlayCond);

        public const int NET_DVR_GET_WORK_STATUS = 6189; //Get device status

        [StructLayoutAttribute(LayoutKind.Sequential)]
        public struct NET_DVR_GETWORKSTATE_COND
        {
            public uint dwSize;
            public byte byFindHardByCond;
            public byte byFindChanByCond;
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 2, ArraySubType = UnmanagedType.I1)]
            public byte[] byRes1;  //±£Áô
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = MAX_DISKNUM_V30, ArraySubType = UnmanagedType.U4)]
            public uint[] dwFindHardStatus;
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = MAX_CHANNUM_V40, ArraySubType = UnmanagedType.U4)]
            public uint[] dwFindChanNo;
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 64, ArraySubType = UnmanagedType.I1)]
            public byte[] byRes;  //±£Áô
        }

        public const int MAX_ALARMIN_V40 = 4128;
        public const int MAX_ALARMOUT_V40 = 4128;

        [StructLayoutAttribute(LayoutKind.Sequential)]
        public struct NET_DVR_WORKSTATE_V40
        {
            public uint dwChannel;
            public uint dwDeviceStatic;
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = MAX_DISKNUM_V30, ArraySubType = UnmanagedType.Struct)]
            public NET_DVR_DISKSTATE[] struHardDiskStatic;
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = MAX_CHANNUM_V40, ArraySubType = UnmanagedType.Struct)]
            public NET_DVR_CHANNELSTATE_V30[] struChanStatic;
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = MAX_ALARMIN_V40, ArraySubType = UnmanagedType.U4)]
            public uint[] dwHasAlarmInStatic;
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = MAX_ALARMOUT_V40, ArraySubType = UnmanagedType.U4)]
            public uint[] dwHasAlarmOutStatic;
            public uint dwLocalDisplay;
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = MAX_AUDIO_V30, ArraySubType = UnmanagedType.I1)]
            public byte[] byAudioInChanStatus;
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 126, ArraySubType = UnmanagedType.I1)]
            public byte[] byRes;  //±£Áô
        }

        //ÎÂ¶È¼ì²â±¨¾¯
        public const int COMM_THERMOMETRY_ALARM = 0x5212;  //ÎÂ¶È±¨¾¯ÉÏ´«  5210Ó¦¸ÄÎªox5212,ÊÖ²áÉÏÊÇ5212
        public const int COMM_THERMOMETRY_DIFF_ALARM = 0x5211;  //ÎÂ²î±¨¾¯ÉÏ´«

        [StructLayoutAttribute(LayoutKind.Sequential)]
        public struct NET_PTZ_INFO
        {
            public float fPan;
            public float fTilt;
            public float fZoom;
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 8, ArraySubType = UnmanagedType.I1)]
            public byte[] byRes;
        }
        //×ø±êÖµ¹éÒ»»¯,¸¡µãÊýÖµÎªµ±Ç°»­ÃæµÄ°Ù·Ö±È´óÐ¡, ¾«¶ÈÎªÐ¡ÊýµãºóÈýÎ»
        //µã×ø±ê½á¹¹
        [StructLayoutAttribute(LayoutKind.Sequential)]
        public struct NET_VCA_POINT
        {
            public float fX;                                // XÖá×ø±ê, 0.001~1
            public float fY;                                //YÖá×ø±ê, 0.001~1
        }

        //¶à±ßÐÍ½á¹¹Ìå
        [StructLayoutAttribute(LayoutKind.Sequential)]
        public struct NET_VCA_POLYGON
        {
            public uint dwPointNum;                                  //ÓÐÐ§µã ´óÓÚµÈÓÚ3£¬ÈôÊÇ3µãÔÚÒ»ÌõÏßÉÏÈÏÎªÊÇÎÞÐ§ÇøÓò£¬Ïß½»²æÈÏÎªÊÇÎÞÐ§ÇøÓò 
            [System.Runtime.InteropServices.MarshalAsAttribute(System.Runtime.InteropServices.UnmanagedType.ByValArray, SizeConst = HCNetSDK.VCA_MAX_POLYGON_POINT_NUM, ArraySubType = System.Runtime.InteropServices.UnmanagedType.Struct)]
            public NET_VCA_POINT[] struPos; //¶à±ßÐÎ±ß½çµã,×î¶àÊ®¸ö 
        }

        //ÎÂ¶È±¨¾¯£¨¼ì²âÎÂ¶ÈºÍÅäÖÃÎÂ¶È±È½Ï±¨¾¯£©
        [StructLayoutAttribute(LayoutKind.Sequential)]
        public struct NET_DVR_THERMOMETRY_ALARM
        {
            public uint dwSize;
            public uint dwChannel;//Í¨µÀºÅ
            public byte byRuleID;//¹æÔòID
            public byte byThermometryUnit;//²âÎÂµ¥Î»: 0-ÉãÊÏ¶È£¨¡æ£©£¬1-»ªÊÏ¶È£¨¨H£©£¬2-¿ª¶ûÎÄ(K)
            public ushort wPresetNo; //Ô¤ÖÃµãºÅ
            NET_PTZ_INFO struPtzInfo;//ptz×ø±êÐÅÏ¢
            public byte byAlarmLevel;//0-Ô¤¾¯ 1-±¨¾¯
            public byte byAlarmType;/*±¨¾¯ÀàÐÍ 0-×î¸ßÎÂ¶È 1-×îµÍÎÂ¶È 2-Æ½¾ùÎÂ¶È 3-ÎÂ²î*/
            public byte byAlarmRule;//0-´óÓÚ£¬1-Ð¡ÓÚ
            public byte byRuleCalibType;//¹æÔò±ê¶¨ÀàÐÍ 0-µã£¬1-¿ò£¬2Ïß
            NET_VCA_POINT struPoint;//µã²âÎÂ×ø±ê£¨µ±¹æÔò±ê¶¨ÀàÐÍÎªµãµÄÊ±ºòÉúÐ§£©
            NET_VCA_POLYGON struRegion;//ÇøÓò£¨µ±¹æÔò±ê¶¨ÀàÐÍÎª¿òµÄÊ±ºòÉúÐ§£©
            public float fRuleTemperature;/*ÅäÖÃ¹æÔòÎÂ¶È,¾«È·µ½Ð¡ÊýµãºóÒ»Î»(-40-1000),£¨¸¡µãÊý+100£© */
            public float fCurrTemperature;/*µ±Ç°ÎÂ¶È,¾«È·µ½Ð¡ÊýµãºóÒ»Î»(-40-1000),£¨¸¡µãÊý+100£© */
            public uint dwPicLen;//¿É¼û¹âÍ¼Æ¬³¤¶È
            public uint dwThermalPicLen;//ÈÈ³ÉÏñÍ¼Æ¬³¤¶È
            public uint dwThermalInfoLen;//ÈÈ³ÉÏñ¸½¼ÓÐÅÏ¢³¤¶È
            public IntPtr pPicBuff; ///¿É¼û¹âÍ¼Æ¬Ö¸Õë
            public IntPtr pThermalPicBuff;// ÈÈ³ÉÏñÍ¼Æ¬Ö¸Õë
            public IntPtr pThermalInfoBuff; //ÈÈ³ÉÏñ¸½¼ÓÐÅÏ¢Ö¸Õë
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 68, ArraySubType = UnmanagedType.I1)]
            public byte[] byRes;
        }

        [StructLayoutAttribute(LayoutKind.Sequential)]
        public struct NET_DVR_TEMPERATURE_COLOR
        {
            /*
            Ñ¡Ôñ0~¸ßÎÂ±¨¾¯ÀàÐÍÊ±£¬<highTemperature>×Ö¶ÎÉúÐ§,µ±¸ßÓÚ¸ÃÎÂ¶ÈÖµÊ±£¬»áÓÐ½øÐÐÑÕÉ«±ê×¢£¬
            Ñ¡Ôñ1~µÍÎÂ±¨¾¯ÀàÐÍÊ±, <lowTemperature>×Ö¶ÎÉúÐ§,µ±µÍÓÚ¸ÃÎÂ¶ÈÖµÊ±£¬»áÓÐ½øÐÐÑÕÉ«±ê×¢¡£
            Ñ¡Ôñ2~Çø¼ä±¨¾¯ÀàÐÍÊ±£¬<highTemperature>¡¢<lowTemperature>×Ö¶ÎÉúÐ§£¬µ±ÔÚÎÂ¶ÈÔÚ¸ÃÎÂ¶ÈÇø¼äÊ±£¬»áÓÐ½øÐÐÑÕÉ«±ê×¢¡£
            Ñ¡Ôñ3~±£ÎÂ±¨¾¯ÀàÐÍÊ±£¬<highTemperature>¡¢<lowTemperature>×Ö¶ÎÉúÐ§£¬µ±ÎÂ¶È²»ÔÚ¸ÃÎÂ¶ÈÇø¼äÊ±£¬»áÓÐ½øÐÐÑÕÉ«±ê×¢¡£ 
            Ñ¡Ôñ4~ÎªÎÞ±¨¾¯ÀàÐÍ£¬<nullAlarm>×Ö¶ÎÉúÐ§£¬¹Ø±Õ±¨¾¯£¬*/
            public byte byType;//²âÎÂ±¨¾¯ÑÕÉ«¿ØÖÆÀàÐÍ£¬0~ÎÞ±¨¾¯ÀàÐÍ£¨¹Ø±Õ£©£¬1~¸ßÎÂ±¨¾¯ÀàÐÍ£¬2~µÍÎÂ±¨¾¯ÀàÐÍ£¬3~Çø¼ä±¨¾¯ÀàÐÍ£¬4~±£ÎÂ±¨¾¯ÀàÐÍ
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 3, ArraySubType = UnmanagedType.I1)]
            public byte[] byRes1;
            public uint iHighTemperature;//¸ßÎÂÖµ£¬-273~10000
            public uint iLowTemperature;//µÍÎÂÖµ£¬-273~10000
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 8, ArraySubType = UnmanagedType.I1)]
            public byte[] byRes;
        }


        [StructLayoutAttribute(LayoutKind.Sequential)]
        public struct NET_DVR_THERMOMETRY_BASICPARAM
        {
            public uint dwSize;//½á¹¹Ìå´óÐ¡
            public byte byEnabled;  //ÊÇ·ñÊ¹ÄÜ£º0- ·ñ£¬1- ÊÇ
            public byte byStreamOverlay; //ÂëÁ÷µþ¼ÓÎÂ¶ÈÐÅÏ¢£º0- ·ñ£¬1- ÊÇ
            public byte byPictureOverlay;//×¥Í¼µþ¼ÓÎÂ¶ÈÐÅÏ¢£º0- ·ñ£¬1- ÊÇ
            public byte byThermometryRange;//²âÎÂ·¶Î§(): 0-Ä¬ÈÏÖµ,1-(-20~150),2-(0~550)£¨ÕâÀïÒÔÉãÊÏ¶ÈÎªµ¥Î»¼ÆËã£©,3-(ÉãÊÏ¶È:0-650¡æ£»»ªÊÏÎÂ¶È:32-1200¨H),4-£¨ÉãÊÏ¶È: -40-150¡æ£©,5-(ÉãÊÏ¶È: 0~1200¡æ)£¨ÕâÀïÒÔÉãÊÏ¶ÈÎªµ¥Î»¼ÆËã£¬¸ù¾Ý²âÎÂµ¥Î»Éè¶¨²»Í¬²âÎÂ·¶Î§µÄÏÔÊ¾£©
            public byte byThermometryUnit;//²âÎÂµ¥Î»: 0-ÉãÊÏ¶È£¨¡æ£©£¬1-»ªÊÏ¶È£¨¨H£©£¬2-¿ª¶ûÎÄ(K)¡£
            public byte byThermometryCurve;//²âÎÂÇúÏßÄ£Ê½ÏÔÊ¾·½Ê½£¬0-¹Ø±Õ£¬1-Ä£Ê½1£¨ºáÏòÎÂ¶ÈÇ÷ÊÆÏßÄ£Ê½£©£¬2-Ä£Ê½2£¨×ÝÏòÎÂ¶ÈÇ÷ÊÆÏßÄ£Ê½£©
            public byte byFireImageModea;//Ïû·ÀÍ¼ÏñÄ£Ê½£¬0-±£Áô£¬1-ºÚ°×Ä£Ê½£¬2-ÈÈÌ½²âÄ£Ê½£¬3-»ð³¡Ä£Ê½(×Ö¶Î0Ä¿Ç°±£Áô£¬±ÜÃâÓëÖ®Ç°½Ó¿Ú²»¼æÈÝ)    
            public byte byShowTempStripEnable;//ÏÔÊ¾ÎÂ¶ÈÌõÊ¹ÄÜ£º0- ·ñ£¬1- ÊÇ
            public float fEmissivity;//·¢ÉäÂÊ(·¢ÉäÂÊ ¾«È·µ½Ð¡ÊýµãºóÁ½Î»)[0.01, 1.00](¼´£ºÎïÌåÏòÍâ·øÉäÄÜÁ¿µÄ±¾Áì)
            public byte byDistanceUnit;//¾àÀëµ¥Î»: 0-Ã×£¨m£©£¬1-Ó¢³ß£¨feet£©¡£
            public byte byEnviroHumidity;//»·¾³Ïà¶ÔÊª¶È£¬È¡Öµ·¶Î§£º0~100%
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 2, ArraySubType = UnmanagedType.I1)]
            public byte[] byRes2;
            NET_DVR_TEMPERATURE_COLOR struTempColor;//²âÎÂ±¨¾¯ÑÕÉ«
            public uint iEnviroTemperature;//»·¾³ÎÂ¶È£¬È¡Öµ·¶Î§£º-273~10000ÉãÊÏ¶È
            public uint iCorrectionVolume;//²âÎÂÐÞÕýÁ¿£¬È¡Öµ·¶Î§£º-100~100
            /*bit0-ÖÐÐÄµã²âÎÂ£º0-²»ÏÔÊ¾£¬1-ÏÔÊ¾£»
            bit1-×î¸ßµã²âÎÂ£º0-²»ÏÔÊ¾£¬1-ÏÔÊ¾£»
            bit2-×îµÍµã²âÎÂ£º0-²»ÏÔÊ¾£¬1-ÏÔÊ¾£»
            */
            public byte bySpecialPointThermType;// ÌØÊâ²âÎÂµãÏÔÊ¾
            public byte byReflectiveEnabled;//·´ÉäÎÂ¶ÈÊ¹ÄÜ£º0- ·ñ£¬1- ÊÇ
            public ushort wDistance;//¾àÀë(m)[0, 10000]
            public float fReflectiveTemperature;//·´ÉäÎÂ¶È ¾«È·µ½Ð¡ÊýºóÒ»Î»
            public float fAlert;//Ô¤¾¯ÎÂ¶ÈãÐÖµ£¬-100.0-1000.0¶È£¨¾«È·µ½Ð¡ÊýµãºóÒ»Î»£©
            public float fAlarm;//±¨¾¯ÎÂ¶ÈãÐÖµ£¬-100.0-1000.0¶È£¨¾«È·µ½Ð¡ÊýµãºóÒ»Î»£©
            public float fThermalOpticalTransmittance;// ¹âÑ§Í¸¹ýÂÊ, ¾«È·µ½Ð¡Êýµãºó3Î»£¬·¶Î§0.001-1.000£¬Ä¬ÈÏ1.000
            public float fExternalOpticsWindowCorrection;//Íâ²¿¹âÑ§ÎÂ¶È£¬Ä¬ÈÏÖµ20¡æ£¬·¶Î§Îª-40.0~80.0¡æ£¬Êµ¼ÊÏÔÊ¾µ¥Î»ÒÔ½çÃæÏÔÊ¾Îª×¼
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 64, ArraySubType = UnmanagedType.I1)]
            public byte[] byRes;

        }


        [StructLayoutAttribute(LayoutKind.Sequential)]
        public struct NET_DVR_THERMOMETRY_COND
        {
            public uint dwSize;//½á¹¹Ìå´óÐ¡
            public uint dwChannel;
            public ushort wPresetNo;//0-±£Áô
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 62, ArraySubType = UnmanagedType.I1)]
            public byte[] byRes;
        }

        [StructLayoutAttribute(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        public struct NET_DVR_THERMOMETRY_PRESETINFO_PARAM
        {
            public byte byEnabled;  //ÊÇ·ñÊ¹ÄÜ£º0- ·ñ£¬1- ÊÇ
            public byte byRuleID;//¹æÔòID 0-±íÊ¾ÎÞÐ§£¬´Ó1¿ªÊ¼ £¨listÄÚ²¿ÅÐ¶ÏÊý¾ÝÓÐÐ§ÐÔ£©
            public ushort wDistance;//¾àÀë(m)[0, 10000]
            public float fEmissivity;//·¢ÉäÂÊ(·¢ÉäÂÊ ¾«È·µ½Ð¡ÊýµãºóÁ½Î»)[0.01, 1.00](¼´£ºÎïÌåÏòÍâ·øÉäÄÜÁ¿µÄ±¾Áì)
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 3, ArraySubType = UnmanagedType.I1)]
            public byte[] byRes;
            public byte byReflectiveEnabled;//·´ÉäÎÂ¶ÈÊ¹ÄÜ£º0- ·ñ£¬1- ÊÇ
            public float fReflectiveTemperature;//·´ÉäÎÂ¶È ¾«È·µ½Ð¡ÊýºóÒ»Î»
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 32, ArraySubType = UnmanagedType.I1)]
            public byte[] szRuleName;//¹æÔòÃû³Æ    
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 63, ArraySubType = UnmanagedType.I1)]
            public byte[] byRes1;
            public byte byRuleCalibType;//¹æÔò±ê¶¨ÀàÐÍ 0-µã£¬1-¿ò£¬2-Ïß
            public NET_VCA_POINT struPoint;//µã²âÎÂ×ø±ê£¨µ±¹æÔò±ê¶¨ÀàÐÍÎª"µã"µÄÊ±ºòÉúÐ§£©
            public NET_VCA_POLYGON struRegion;//ÇøÓò¡¢Ïß£¨µ±¹æÔò±ê¶¨ÀàÐÍÎª"¿ò"»òÕß"Ïß"µÄÊ±ºòÉúÐ§£©
        }

        [StructLayoutAttribute(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        public struct NET_DVR_THERMOMETRY_PRESETINFO
        {
            public uint dwSize;//½á¹¹Ìå´óÐ¡
            public ushort wPresetNo;//0-±£Áô
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 2, ArraySubType = UnmanagedType.I1)]
            public byte[] byRes;
            [System.Runtime.InteropServices.MarshalAsAttribute(System.Runtime.InteropServices.UnmanagedType.ByValArray, SizeConst = MAX_THERMOMETRY_REGION_NUM, ArraySubType = System.Runtime.InteropServices.UnmanagedType.Struct)]
            public NET_DVR_THERMOMETRY_PRESETINFO_PARAM[] struPresetInfo;
        }

        [StructLayoutAttribute(LayoutKind.Sequential/*, CharSet = CharSet.Unicode*/)]
        public struct NET_DVR_THERMOMETRY_ALARMRULE_PARAM
        {
            public byte byEnable;
            public byte byRuleID;//¹æÔòID
            public byte byRule;//±¨¾¯ÎÂ¶È±È½Ï·½Ê½ 0-¸ßÎÂ´óÓÚ,1-¸ßÎÂÐ¡ÓÚ,2-µÍÎÂ´óÓÚ,3-µÍÎÂÐ¡ÓÚ,4-Æ½¾ùÎÂ´óÓÚ,5-Æ½¾ùÎÂÐ¡ÓÚ,6-ÎÂ²î´óÓÚ,7-ÎÂ²îÐ¡ÓÚ    
            public byte byRes;
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 32, ArraySubType = UnmanagedType.I1)]
            public byte[] szRuleName;//¹æÔòÃû³Æ
            public float fAlert;//Ô¤¾¯ÎÂ¶È
            public float fAlarm;//±¨¾¯ÎÂ¶È
            public float fThreshold;//ÃÅÏÞÎÂ¶È
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 64, ArraySubType = UnmanagedType.I1)]
            public byte[] byRes1;
        }

        [StructLayoutAttribute(LayoutKind.Sequential/*, CharSet = CharSet.Unicode*/)]
        public struct NET_DVR_THERMOMETRY_ALARMRULE
        {
            public uint dwSize;//½á¹¹Ìå´óÐ¡
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = THERMOMETRY_ALARMRULE_NUM, ArraySubType = UnmanagedType.Struct)]
            public NET_DVR_THERMOMETRY_ALARMRULE_PARAM[] struThermometryAlarmRuleParam;
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 128, ArraySubType = UnmanagedType.I1)]
            public byte[] byRes;
        }

        //ÎÂ²î±¨¾¯
        [StructLayoutAttribute(LayoutKind.Sequential)]
        public struct NET_DVR_THERMOMETRY_DIFF_ALARM
        {
            public uint dwSize;
            public uint dwChannel;//Í¨µÀºÅ
            public byte byAlarmID1;//¹æÔòAlarmID1
            public byte byAlarmID2;//¹æÔòAlarmID2
            public ushort wPresetNo; //Ô¤ÖÃµãºÅ
            public byte byAlarmLevel;//0-Ô¤¾¯ 1-±¨¾¯
            public byte byAlarmType;/*±¨¾¯ÀàÐÍ 0-×î¸ßÎÂ¶È 1-×îµÍÎÂ¶È 2-Æ½¾ùÎÂ¶È*/
            public byte byAlarmRule;//0-´óÓÚ£¬1-Ð¡ÓÚ
            public byte byRuleCalibType;//¹æÔò±ê¶¨ÀàÐÍ 0-µã£¬1-¿ò£¬2Ïß
            [System.Runtime.InteropServices.MarshalAsAttribute(System.Runtime.InteropServices.UnmanagedType.ByValArray, SizeConst = 2, ArraySubType = System.Runtime.InteropServices.UnmanagedType.Struct)]
            public NET_VCA_POINT[] struPoint;//µã²âÎÂ×ø±ê£¨µ±¹æÔò±ê¶¨ÀàÐÍÎªµãµÄÊ±ºòÉúÐ§£©Êý×éÏÂ±ê0´ú±í×ÅAlarmID1£¬Êý×éÏÂ±ê1´ú±í×ÅAlarmID2.
            [System.Runtime.InteropServices.MarshalAsAttribute(System.Runtime.InteropServices.UnmanagedType.ByValArray, SizeConst = 2, ArraySubType = System.Runtime.InteropServices.UnmanagedType.Struct)]
            public NET_VCA_POLYGON[] struRegion;//ÇøÓò£¨µ±¹æÔò±ê¶¨ÀàÐÍÎª¿òµÄÊ±ºòÉúÐ§£©Êý×éÏÂ±ê0´ú±í×ÅAlarmID1£¬Êý×éÏÂ±ê1´ú±í×ÅAlarmID2.
            public float fRuleTemperatureDiff;/*ÅäÖÃ¹æÔòÎÂ²î,¾«È·µ½Ð¡ÊýµãºóÒ»Î»(-40-1000),£¨¸¡µãÊý+100£©*/
            public float fCurTemperatureDiff;/*µ±Ç°ÎÂ²î,¾«È·µ½Ð¡ÊýµãºóÒ»Î»(-40-1000),£¨¸¡µãÊý+100£© */
            NET_PTZ_INFO struPtzInfo;//ptz×ø±êÐÅÏ¢
            public uint dwPicLen;//¿É¼û¹âÍ¼Æ¬³¤¶È
            public uint dwThermalPicLen;//ÈÈ³ÉÏñÍ¼Æ¬³¤¶È
            public uint dwThermalInfoLen;//ÈÈ³ÉÏñ¸½¼ÓÐÅÏ¢³¤¶È
            public IntPtr pPicBuff; ///¿É¼û¹âÍ¼Æ¬Ö¸Õë
            public IntPtr pThermalPicBuff;// ÈÈ³ÉÏñÍ¼Æ¬Ö¸Õë
            public IntPtr pThermalInfoBuff; //ÈÈ³ÉÏñ¸½¼ÓÐÅÏ¢Ö¸Õë
            public byte byThermometryUnit;//²âÎÂµ¥Î»: 0-ÉãÊÏ¶È£¨¡æ£©£¬1-»ªÊÏ¶È£¨¨H£©£¬2-¿ª¶ûÎÄ(K)
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 63, ArraySubType = UnmanagedType.I1)]
            public byte[] byRes;
        }


        //ÊµÊ±ÎÂ¶È¼ì²âÌõ¼þ½á¹¹
        public struct NET_DVR_REALTIME_THERMOMETRY_COND
        {
            public uint dwSize;
            public uint dwChan;
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 64, ArraySubType = UnmanagedType.I1)]
            public byte[] byRes; //±£Áô
        }

        public struct NET_DVR_POINT_THERM_CFG
        {
            public float fTemperature;//µ±Ç°ÎÂ¶È
            public NET_VCA_POINT struPoint;//µã²âÎÂ×ø±ê£¨µ±¹æÔò±ê¶¨ÀàÐÍÎªµãµÄÊ±ºòÉúÐ§£©
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 120, ArraySubType = UnmanagedType.I1)]
            public byte[] byRes; //±£Áô
        }

        public struct NET_DVR_LINEPOLYGON_THERM_CFG
        {
            public float fMaxTemperature;//×î¸ßÎÂ
            public float fMinTemperature;//×îµÍÎÂ
            public float fAverageTemperature;//Æ½¾ùÎÂ
            public float fTemperatureDiff;//ÎÂ²î
            public NET_VCA_POLYGON struRegion;//ÇøÓò£¨µ±¹æÔò±ê¶¨ÀàÐÍÎª¿ò/ÏßµÄÊ±ºòÉúÐ§£©
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 32, ArraySubType = UnmanagedType.I1)]
            public byte[] byRes; //±£Áô
        }

        public struct NET_DVR_THERMOMETRY_UPLOAD
        {
            public uint dwSize;
            public uint dwRelativeTime;     // Ïà¶ÔÊ±±ê
            public uint dwAbsTime;          // ¾ø¶ÔÊ±±ê
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 32, ArraySubType = UnmanagedType.I1)]
            public char[] szRuleName;//¹æÔòÃû³Æ
            public byte byRuleID;//¹æÔòIDºÅ
            public byte byRuleCalibType;//¹æÔò±ê¶¨ÀàÐÍ 0-µã£¬1-¿ò£¬2-Ïß
            public ushort wPresetNo; //Ô¤ÖÃµãºÅ
            public NET_DVR_POINT_THERM_CFG struPointThermCfg;
            public NET_DVR_LINEPOLYGON_THERM_CFG struLinePolygonThermCfg;
            public byte byThermometryUnit;//²âÎÂµ¥Î»: 0-ÉãÊÏ¶È£¨¡æ£©£¬1-»ªÊÏ¶È£¨¨H£©£¬2-¿ª¶ûÎÄ(K)
            public byte byDataType;//Êý¾Ý×´Ì¬ÀàÐÍ:0-¼ì²âÖÐ£¬1-¿ªÊ¼£¬2-½áÊø
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 126, ArraySubType = UnmanagedType.I1)]
            public byte[] byRes; //±£Áô
            public void Init()
            {
                struPointThermCfg = new HCNetSDK.NET_DVR_POINT_THERM_CFG();
                struLinePolygonThermCfg = new HCNetSDK.NET_DVR_LINEPOLYGON_THERM_CFG();
            }
        }
        // Ïà¶ÔÊ±±ê:Ò»°ãÉæ¼°µ½Ê±Çø£¬±ÈÈç´«µÝµÄ¶«°ËÇøÊ±¼ä¡¢GMTÊ±¼ä¡£
        // ¾ø¶ÔÊ±±ê£º¼´UTCÊ±¼ä£¬²»´øÊ±Çø£¬±ÈÈç1970Äê1ÔÂ1ÈÕ0Ê±0·Ö0Ãëµ½ÏÖÔÚµÄÃëÊý£¬Ïà»úÏÖÔÚÒ»°ã´«µÝµÄ¾ø¶ÔÊ±±ê¶¼ÊÇÕâ¸ö¡£
        //  
        // ±ÈÈçÒ»¸öµ±Ç°Ïà»ú¶«°ËÇøµÄtime_tÖµ1400252410 
        // Ïà¶ÔÊ±±êµÄ»°Ó¦¸ÃÊÇ£º2014-05-16 23:00:10
        // ¾ø¶ÔÊ±±êµÄ»°Ó¦¸ÃÊÇ£º2014-05-16 15:00:10

        public const int NET_DVR_GET_FIGURE = 6640;

        public struct NET_DVR_GET_FIGURE_COND
        {
            public uint dwLength;       //½á¹¹³¤¶È
            public uint dwChannel;      //Í¨µÀ
            public NET_DVR_TIME_V30 struTimePoint;  //Ê±¼äµã
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 64, ArraySubType = UnmanagedType.I1)]
            public byte[] byRes;
        }

        public struct NET_DVR_FIGURE_INFO
        {
            public uint dwPicLen;   //Í¼Æ¬³¤¶È
            public IntPtr pPicBuf;  //Í¼Æ¬Êý¾Ý
        }

        public struct NET_DVR_PTZ_MANUALTRACE
        {
            public uint dwSize;
            public uint dwChannel;      // Ïà¶ÔÊ±±ê
            public NET_VCA_POINT struPoint; //¶¨Î»×ø±ê(ÆôÊ¾×ø±ê)
            public byte byTrackType;    //¸ú×ÙÀàÐÍ£¨ÊÖ¶¯¸ú×ÙÄ¬ÈÏÊÇ0£© 0¡¢·Ç×Ô¶¯È¡Ö¤(ÆÕÍ¨È¡Ö¤) 1¡¢¸ßËÙµÀÂ·¸ú×Ù 2¡¢³ÇÊÐµÀÂ·¸ú×Ù£¨ÊÖ¶¯¸ú×ÙÈ¡Ö¤£©3¡¢¾²Ì¬È¡Ö¤
            public byte byLinkageType;//Áª¶¯¶¯×÷: 0-ÊÖ¶¯¸ú×Ù 1-Áª¶¯²»¸ú×Ù
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 2, ArraySubType = UnmanagedType.I1)]
            public byte[] byRes; //±£Áô
            public NET_VCA_POINT struPointEnd;  //¶¨Î»×ø±ê(ÆôÊ¾×ø±ê)
            public NET_DVR_TIME_V30 struTime;   //ÊÖ¶¯¸ú×Ù¶¨Î»£¬µ±Ç°Ê±¼ä
            public uint dwSerialNo;         //ÐòºÅ;
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 36, ArraySubType = UnmanagedType.I1)]
            public byte[] byRes1; //±£Áô
        }

        public struct NET_DVR_SMARTTRACKCFG
        {
            public uint dwSize;
            public byte byEnable;//Æô¶¯Ê¹ÄÜ 0-·ñ£¬1-ÊÇ
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 3, ArraySubType = UnmanagedType.I1)]
            public byte[] byRes;//±£Áô
            public uint dwDuration;//³ÖÐøÊ±¼ä£º0--300Ãë£¬Ä¬ÈÏ300Ãë
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 124, ArraySubType = UnmanagedType.I1)]
            public byte[] byRes1;//±£Áô
        }

        //»ñÈ¡µ¥¸öÍ¨µÀÊôÐÔÊý¾Ý
        public const int NET_DVR_GET_SINGLE_CHANNELINFO = 4360;

        //ÈËÁ³Õì²âÅäÖÃ
        public const int NET_DVR_GET_FACE_DETECT = 3352;
        public const int NET_DVR_SET_FACE_DETECT = 3353;

        public struct NET_DVR_CHANNEL_GROUP
        {
            public uint dwSize;
            public uint dwChannel; //Í¨µÀºÅ
            public uint dwGroup;  //×éºÅ 
            public byte byID;//Éè±¸ÇøÓòÉèÖÃID
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 3, ArraySubType = UnmanagedType.I1)]
            public byte[] byRes1;//±£Áô
            public uint dwPositionNo;//³¡¾°Î»ÖÃË÷ÒýºÅ,IPCÊÇ0£¬IPDÊÇ´Ó1¿ªÊ¼
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 56, ArraySubType = UnmanagedType.I1)]
            public byte[] byRes;//±£Áô
        }

        public struct NET_DVR_HANDLEEXCEPTION_V40
        {
            public uint dwHandleType;//Òì³£´¦Àí,Òì³£´¦Àí·½Ê½µÄ"»ò"½á¹û   
            /*0x00: ÎÞÏìÓ¦*/
            /*0x01: ¼àÊÓÆ÷ÉÏ¾¯¸æ*/
            /*0x02: ÉùÒô¾¯¸æ*/
            /*0x04: ÉÏ´«ÖÐÐÄ*/
            /*0x08: ´¥·¢±¨¾¯Êä³ö*/
            /*0x10: ´¥·¢JPRG×¥Í¼²¢ÉÏ´«Email*/
            /*0x20: ÎÞÏßÉù¹â±¨¾¯Æ÷Áª¶¯*/
            /*0x40: Áª¶¯µç×ÓµØÍ¼(Ä¿Ç°Ö»ÓÐPCNVRÖ§³Ö)*/
            /*0x80: ±¨¾¯´¥·¢Â¼Ïñ(Ä¿Ç°Ö»ÓÐPCNVRÖ§³Ö) */
            /*0x100: ±¨¾¯´¥·¢ÔÆÌ¨Ô¤ÖÃµã (Ä¿Ç°Ö»ÓÐPCNVRÖ§³Ö)*/
            /*0x200: ×¥Í¼²¢ÉÏ´«FTP*/
            /*0x400: Ðé½»Õì²â Áª¶¯ ¾Û½¹Ä£Ê½£¨Ìá¹©¿ÉÅäÖÃÏî£¬Ô­ÏÈÉè±¸×Ô¶¯Íê³É£©IPC5.1.0*/
            /*0x800: PTZÁª¶¯¸ú×Ù(Çò»ú¸ú×ÙÄ¿±ê)*/
            /*0x1000:×¥Í¼ÉÏ´«µ½ÔÆ*/
            public uint dwMaxRelAlarmOutChanNum;//´¥·¢µÄ±¨¾¯Êä³öÍ¨µÀÊý£¨Ö»¶Á£©×î´óÖ§³ÖÊýÁ¿
            public uint dwRelAlarmOutChanNum;//´¥·¢µÄ±¨¾¯Êä³öÍ¨µÀÊý Êµ¼ÊÖ§³ÖÊý
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = MAX_CHANNUM_V30, ArraySubType = UnmanagedType.U4)]
            public uint[] dwRelAlarmOut;//´¥·¢±¨¾¯Í¨µÀ
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 64, ArraySubType = UnmanagedType.U1)]
            public byte[] byRes;//±£Áô
        }

        public struct NET_DVR_DETECT_FACE
        {
            public uint dwSize;
            public byte byEnableDetectFace;//ÊÇ·ñÆôÓÃ 0¡«²»ÆôÓÃ£¬ 1¡«ÆôÓÃ
            public byte byDetectSensitive;//ÁéÃô¶È 10¸öµÈ¼¶ 1¡«10
            public byte byEnableDisplay;/*ÆôÓÃÒÆ¶¯Õì²â¸ßÁÁÏÔÊ¾£¬0-·ñ£¬1-ÊÇ*/
            public byte byRes;
            public NET_DVR_HANDLEEXCEPTION_V40 struAlarmHandleType;  /*´¦Àí·½Ê½*/
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = HCNetSDK.MAX_DAYS * HCNetSDK.MAX_TIMESEGMENT_V30, ArraySubType = UnmanagedType.Struct)]
            public NET_DVR_SCHEDTIME[] struAlarmTime;  /*²¼·ÀÊ±¼ä*/
            public uint dwMaxRelRecordChanNum;//±¨¾¯´¥·¢µÄÂ¼ÏóÍ¨µÀ Êý£¨Ö»¶Á£©×î´óÖ§³ÖÊýÁ¿
            public uint dwRelRecordChanNum;//±¨¾¯´¥·¢µÄÂ¼ÏóÍ¨µÀÊý Êµ¼ÊÖ§³ÖÊý
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = MAX_CHANNUM_V30, ArraySubType = UnmanagedType.U4)]
            public uint[] dwRelRecordChan;//±¨¾¯´¥·¢µÄÂ¼ÏóÍ¨µÀ
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = HCNetSDK.MAX_TIMESEGMENT_V30, ArraySubType = UnmanagedType.Struct)]
            public NET_DVR_SCHEDTIME[] struHolidayTime;  /*¼ÙÈÕ²¼·ÀÊ±¼ä*/
            public ushort wDuration;//±¨¾¯³ÖÐøÊ±¼ä 5s ÄÜÁ¦¼¯
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 30, ArraySubType = UnmanagedType.U1)]
            public byte[] byRes1;//±£Áô
        }

        public const int MAX_FACE_PIC_NUM = 30;
        public const int COMM_ALARM_FACE_DETECTION = 0x4010;//ÈËÁ³Õì²â±¨¾¯

        [StructLayoutAttribute(LayoutKind.Sequential)]
        public struct NET_DVR_FACE_DETECTION
        {
            public uint dwSize;
            public uint dwRelativeTime; //Ïà¶ÔÊ±±ê
            public uint dwAbsTime; //¾ø¶ÔÊ±±ê
            public uint dwBackgroundPicLen; //±³¾°Í¼µÄ³¤¶È£¬Îª0±íÊ¾Ã»ÓÐÍ¼Æ¬£¬´óÓÚ0±íÊ¾ÓÐÍ¼Æ¬
            public NET_VCA_DEV_INFO struDevInfo;   //Ç°¶ËÉè±¸ÐÅÏ¢
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = HCNetSDK.MAX_FACE_PIC_NUM, ArraySubType = UnmanagedType.Struct)]
            public NET_VCA_RECT[] struFacePic;   //ÈËÁ³×ÓÍ¼ÇøÓò
            public byte byFacePicNum;//×ÓÍ¼ÊýÁ¿
            public byte byRes1;
            public ushort wDevInfoIvmsChannelEx;//ÓëNET_VCA_DEV_INFOÀïµÄbyIvmsChannelº¬ÒåÏàÍ¬£¬ÄÜ±íÊ¾¸ü´óµÄÖµ¡£ÀÏ¿Í»§¶ËÓÃbyIvmsChannelÄÜ¼ÌÐø¼æÈÝ£¬µ«ÊÇ×î´óµ½255¡£ÐÂ¿Í»§¶Ë°æ±¾ÇëÊ¹ÓÃwDevInfoIvmsChannelEx¡£
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 252, ArraySubType = UnmanagedType.I1)]
            public byte[] byRes;//±£Áô
            public IntPtr pBackgroundPicpBuffer;//±³¾°Í¼µÄÍ¼Æ¬Êý¾Ý
        }



        public const int NET_DVR_GET_PDC_RESULT = 5089;//¿ÍÁ÷Á¿Êý¾Ý²éÑ¯ 
        public const int NET_DVR_REMOVE_FLASHSTORAGE = 3756;    //¿ÍÁ÷Êý¾ÝÇå³ý²Ù×÷
        public const int NET_DVR_GET_PDC_RULECFG_V42 = 3405;  //ÉèÖÃÈËÁ÷Á¿Í³¼Æ¹æÔò(À©Õ¹)
        public const int NET_DVR_SET_PDC_RULECFG_V42 = 3406;  //»ñÈ¡ÈËÁ÷Á¿Í³¼Æ¹æÔò(À©Õ¹)


        [System.Runtime.InteropServices.StructLayoutAttribute(System.Runtime.InteropServices.LayoutKind.Sequential)]
        public struct NET_DVR_PDC_QUERY_COND
        {
            public uint dwSize;
            public uint dwChannel; //Í¨µÀºÅ
            public NET_DVR_TIME_EX struStartTime; //¿ªÊ¼Ê±¼ä
            public NET_DVR_TIME_EX struEndTime; //½áÊøÊ±¼ä
            public byte byReportType; //0-ÎÞÐ§Öµ£¬1-ÈÕ±¨±í£¬2-ÖÜ±¨±í£¬3-ÔÂ±¨±í£¬4-Äê±¨±í
            public byte byEnableProgramStatistics; //ÊÇ·ñ°´½ÚÄ¿Í³¼Æ£¬0-·ñ£¬1-ÊÇ
            [System.Runtime.InteropServices.MarshalAsAttribute(System.Runtime.InteropServices.UnmanagedType.ByValArray, SizeConst = 2, ArraySubType = System.Runtime.InteropServices.UnmanagedType.I1)]
            public byte[] byRes1;
            public uint dwPlayScheduleNo; //°´½ÚÄ¿Í³¼ÆÊ±¹ØÁªµÄÈÕ³ÌºÅ
            [System.Runtime.InteropServices.MarshalAsAttribute(System.Runtime.InteropServices.UnmanagedType.ByValArray, SizeConst = 120, ArraySubType = System.Runtime.InteropServices.UnmanagedType.I1)]
            public byte[] byRes;
        }

        [System.Runtime.InteropServices.StructLayoutAttribute(System.Runtime.InteropServices.LayoutKind.Sequential)]
        public struct NET_DVR_PROGRAM_INFO
        {
            public uint dwProgramNo; //½ÚÄ¿±àºÅ
            [System.Runtime.InteropServices.MarshalAsAttribute(System.Runtime.InteropServices.UnmanagedType.ByValArray, SizeConst = 32, ArraySubType = System.Runtime.InteropServices.UnmanagedType.I1)]
            public byte[] sProgramName; //½ÚÄ¿Ãû³Æ
            [System.Runtime.InteropServices.MarshalAsAttribute(System.Runtime.InteropServices.UnmanagedType.ByValArray, SizeConst = 16, ArraySubType = System.Runtime.InteropServices.UnmanagedType.I1)]
            public byte[] byRes;
        }

        [System.Runtime.InteropServices.StructLayoutAttribute(System.Runtime.InteropServices.LayoutKind.Sequential)]
        public struct NET_DVR_PDC_RESULT
        {
            public uint dwSize;
            public NET_DVR_TIME_EX struStartTime;/*¿ªÊ¼Ê±¼ä*/
            public NET_DVR_TIME_EX struEndTime;/*½áÊøÊ±¼ä*/
            public uint dwEnterNum;   //½øÈëÈËÊý
            public uint dwLeaveNum;  //Àë¿ªÈËÊý
            public NET_DVR_PROGRAM_INFO struProgramInfo;        //½ÚÄ¿ÐÅÏ¢
            public uint dwPeoplePassing;        //¾­¹ýÈËÊý
            [System.Runtime.InteropServices.MarshalAsAttribute(System.Runtime.InteropServices.UnmanagedType.ByValArray, SizeConst = 200, ArraySubType = System.Runtime.InteropServices.UnmanagedType.I1)]
            public byte[] byRes;    //±£Áô×Ö½Ú
        }


        [System.Runtime.InteropServices.StructLayoutAttribute(System.Runtime.InteropServices.LayoutKind.Sequential)]
        public struct NET_DVR_PDC_ALRAM_INFO
        {
            public uint dwSize;          // PDCÈËÁ÷Á¿±¨¾¯ÉÏ´«½á¹¹Ìå´óÐ¡
            public byte byMode;          // 0 µ¥Ö¡Í³¼Æ½á¹û 1×îÐ¡Ê±¼ä¶ÎÍ³¼Æ½á¹û  
            public byte byChannel;       // ±¨¾¯ÉÏ´«Í¨µÀºÅ
            /********* IPC5.1.7 ÐÂÔö²ÎÊý Begin 2014-03-21***********/
            public byte bySmart;         //×¨ÒµÖÇÄÜ·µ»Ø0£¬Smart ·µ»Ø 1
            public byte byRes1;          // ±£Áô×Ö½Ú    
            /********* IPC5.1.7 ÐÂÔö²ÎÊý End 2014-03-21***********/
            public NET_VCA_DEV_INFO struDevInfo;             //Ç°¶ËÉè±¸ÐÅÏ¢
            public NET_MODE_PARAM uStatModeParam;
            public uint dwLeaveNum;        // Àë¿ªÈËÊý
            public uint dwEnterNum;        // ½øÈëÈËÊý
            public byte byBrokenNetHttp;     //¶ÏÍøÐø´«±êÖ¾Î»£¬0-²»ÊÇÖØ´«Êý¾Ý£¬1-ÖØ´«Êý¾Ý
            public byte byRes3;
            public short wDevInfoIvmsChannelEx;     //ÓëNET_VCA_DEV_INFOÀïµÄbyIvmsChannelº¬ÒåÏàÍ¬£¬ÄÜ±íÊ¾¸ü´óµÄÖµ¡£ÀÏ¿Í»§¶ËÓÃbyIvmsChannelÄÜ¼ÌÐø¼æÈÝ£¬µ«ÊÇ×î´óµ½255¡£ÐÂ¿Í»§¶Ë°æ±¾ÇëÊ¹ÓÃwDevInfoIvmsChannelEx
            public uint dwPassingNum;        // ¾­¹ýÈËÊý£¨½øÈëÇøÓòºóÅÇ»²Ã»ÓÐ´¥·¢½øÈë¡¢Àë¿ªµÄÈËÊý£©
            [System.Runtime.InteropServices.MarshalAsAttribute(System.Runtime.InteropServices.UnmanagedType.ByValArray, SizeConst = 32, ArraySubType = System.Runtime.InteropServices.UnmanagedType.I1)]
            public byte[] byRes;    //±£Áô×Ö½Ú
        }

        [StructLayoutAttribute(LayoutKind.Explicit)]
        public struct NET_MODE_PARAM
        {
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 100, ArraySubType = UnmanagedType.U1)]
            [FieldOffsetAttribute(0)]
            public byte[] byLen;//²ÎÊý
            [FieldOffsetAttribute(0)]
            public NET_MODE_FRAME struStatFrame;
            [FieldOffsetAttribute(0)]
            public NET_MODE_START_TIME struStatTime;
        }
        [System.Runtime.InteropServices.StructLayoutAttribute(System.Runtime.InteropServices.LayoutKind.Sequential)]
        public struct NET_MODE_FRAME
        {
            public uint dwRelativeTime;     // Ïà¶ÔÊ±±ê
            public uint dwAbsTime;          // ¾ø¶ÔÊ±±ê
            [System.Runtime.InteropServices.MarshalAsAttribute(System.Runtime.InteropServices.UnmanagedType.ByValArray, SizeConst = 92, ArraySubType = System.Runtime.InteropServices.UnmanagedType.I1)]
            public byte[] byRes;    //±£Áô×Ö½Ú
        }


        [System.Runtime.InteropServices.StructLayoutAttribute(System.Runtime.InteropServices.LayoutKind.Sequential)]
        public struct NET_MODE_START_TIME
        {
            public NET_DVR_TIME tmStart; // Í³¼ÆÆðÊ¼Ê±¼ä 
            public NET_DVR_TIME tmEnd;  //  Í³¼Æ½áÊøÊ±¼ä 
            [System.Runtime.InteropServices.MarshalAsAttribute(System.Runtime.InteropServices.UnmanagedType.ByValArray, SizeConst = 92, ArraySubType = System.Runtime.InteropServices.UnmanagedType.I1)]
            public byte[] byRes;    //±£Áô×Ö½Ú
        }

        public struct NET_DVR_FLASHSTORAGE_REMOVE
        {
            public uint dwSize;
            public uint dwChannel;
            public byte byPDCRemoveEnable;//Çå³ý¿ÍÁ÷Êý¾ÝÊ¹ÄÜ 0-²»Çå³ý£¬1-Çå³ý
            [System.Runtime.InteropServices.MarshalAsAttribute(System.Runtime.InteropServices.UnmanagedType.ByValArray, SizeConst = 127, ArraySubType = System.Runtime.InteropServices.UnmanagedType.I1)]
            public byte[] byRes;    //±£Áô×Ö½Ú;
        }

        public struct NET_DVR_STD_CONTROL
        {
            public IntPtr lpCondBuffer;    //[in]Ìõ¼þ²ÎÊý(Âë×Ö¸ñÊ½),ÀýÈçÍ¨µÀºÅµÈ.¿ÉÒÔÎªNULL
            public uint dwCondSize;        //[in] dwCondSizeÖ¸ÏòµÄÄÚ´æ´óÐ¡
            public IntPtr lpStatusBuffer;    //[out]·µ»ØµÄ×´Ì¬²ÎÊý(XML¸ñÊ½),»ñÈ¡³É¹¦Ê±²»»á¸³Öµ,Èç¹û²»ÐèÒª,¿ÉÒÔÖÃNULL
            public uint dwStatusSize;    //[in] lpStatusBufferÖ¸ÏòµÄÄÚ´æ´óÐ¡
            public IntPtr lpXmlBuffer;    //[in/out]byDataType = 1Ê±ÓÐÐ§,xml¸ñÊ½Êý¾Ý
            public uint dwXmlSize;      //[in/out]lpXmlBufferÖ¸ÏòµÄÄÚ´æ´óÐ¡,»ñÈ¡Ê±Í¬Ê±×÷ÎªÊäÈëºÍÊä³ö²ÎÊý£¬»ñÈ¡³É¹¦ºó»áÐÞ¸Ä»áÊµ¼Ê³¤¶È£¬ÉèÖÃÊ±±íÊ¾Êµ¼Ê³¤¶È£¬¶ø²»ÊÇÕû¸öÄÚ´æ´óÐ¡
            public byte byDataType;     //[in]ÊäÈë/Êä³ö²ÎÊýÀàÐÍ,0-Ê¹ÓÃ½á¹¹ÌåÀàÐÍlpCondBufferÓÐÐ§,1-Ê¹ÓÃXMLÀàÐÍlpXmlBufferÓÐÐ§
            [System.Runtime.InteropServices.MarshalAsAttribute(System.Runtime.InteropServices.UnmanagedType.ByValArray, SizeConst = 55, ArraySubType = System.Runtime.InteropServices.UnmanagedType.I1)]
            public byte[] byRes;    //±£Áô×Ö½Ú
        };

        public const int NET_DVR_GET_TRAFFIC_CAP = 6630;
        public const int UPLOAD_VEHICLE_BLACKWHITELST_FILE = 13; //ÉÏ´«ºÚ°×Ãûµ¥ÅäÖÃÎÄ¼þ¹¦ÄÜ

        [DllImport(@"HCNetSDK.dll")]
        public static extern int NET_DVR_UploadFile_V40(int lUserID, uint dwUploadType, IntPtr lpInBuffer, uint dwInBufferSize, string sFileName, IntPtr lpOutBuffer, uint dwOutBufferSize);
        [DllImport(@"HCNetSDK.dll")]
        public static extern int NET_DVR_GetUploadState(int lUploadHandle, IntPtr pProgress);
        [DllImport(@"HCNetSDK.dll")]
        public static extern bool NET_DVR_GetUploadResult(int lUploadHandle, IntPtr lpOutBuffer, uint dwOutBufferSize);
        [DllImport(@"HCNetSDK.dll")]
        public static extern bool NET_DVR_UploadClose(int lUploadHandle);

        public const int NET_SDK_DOWNLOAD_VEHICLE_BLACKWHITELST_FILE = 8; //ÏÂÔØºÚ°×Ãûµ¥ÅäÖÃÎÄ¼þ

        [DllImport(@"HCNetSDK.dll")]
        public static extern int NET_DVR_StartDownload(int lUserID, uint dwDownloadType, IntPtr lpInBuffer, uint dwInBufferSize, string sFileName);
        [DllImport(@"HCNetSDK.dll")]
        public static extern int NET_DVR_GetDownloadState(int lFileHandle, IntPtr pProgress);
        [DllImport(@"HCNetSDK.dll")]
        public static extern int NET_DVR_GetDownloadStateInfo(int lFileHandle, IntPtr pStatusInfo);
        [DllImport(@"HCNetSDK.dll")]
        public static extern bool NET_DVR_StopDownload(int lFileHandle);

        public const int NET_DVR_GET_GUARDCFG = 3134;//»ñÈ¡³µÅÆÊ¶±ð¼ì²â¼Æ»® 
        public const int NET_DVR_SET_GUARDCFG = 3135;//ÉèÖÃ³µÅÆÊ¶±ð¼ì²â¼Æ»®

        //×¥ÅÄ´¥·¢Ä£Ê½Ö§³Ö¹ØÁª²¼·ÀÊ±¼ä¶ÎºÍÉÏ´«ÖÐÐÄÌõ¼þ
        [System.Runtime.InteropServices.StructLayoutAttribute(System.Runtime.InteropServices.LayoutKind.Sequential)]
        public struct NET_DVR_GUARD_COND
        {
            public uint dwSize;
            public uint dwChannel; //Í¨µÀºÅ
            /*
            0±íÊ¾ÎÞÐ§£¬ 
            1±íÊ¾¹ØÁª ×¥ÅÄVIAÄ£Ê½£¨ÊÓÆµ´¥·¢×¥ÅÄ IPCÊ¹ÓÃ£©;
            2 ±íÊ¾¹ØÁª ×¥ÅÄ HVT Ä£Ê½ £¨»ì¿¨IPCÊ¹ÓÃ£©
            */
            public byte byRelateType;
            public byte byGroupNo;  //×éºÅ
            [System.Runtime.InteropServices.MarshalAsAttribute(System.Runtime.InteropServices.UnmanagedType.ByValArray, SizeConst = 62, ArraySubType = System.Runtime.InteropServices.UnmanagedType.U1)]
            public byte[] byRes;
        };

        public struct NET_DVR_TIME_DETECTION
        {
            public NET_DVR_SCHEDTIME struSchedTime; //Ê±¼ä
            public byte byDetSceneID;//¼ì²â³¡¾°ºÅ[1,4],IPCÄ¬ÈÏÊÇ0
            [System.Runtime.InteropServices.MarshalAsAttribute(System.Runtime.InteropServices.UnmanagedType.ByValArray, SizeConst = 15, ArraySubType = System.Runtime.InteropServices.UnmanagedType.U1)]
            public byte[] byRes;
        };

        //×¥ÅÄ´¥·¢Ä£Ê½Ö§³Ö¹ØÁª²¼·ÀÊ±¼ä¶ÎºÍÉÏ´«ÖÐÐÄ
        [System.Runtime.InteropServices.StructLayoutAttribute(System.Runtime.InteropServices.LayoutKind.Sequential)]
        public struct NET_DVR_GUARD_CFG
        {
            public uint dwSize;
            //²¼·ÀÊ±¼ä
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = MAX_DAYS * MAX_TIMESEGMENT_V30, ArraySubType = UnmanagedType.Struct)]
            public NET_DVR_TIME_DETECTION[] struAlarmSched;
            public NET_DVR_HANDLEEXCEPTION_V40 struHandleException;  //Òì³£´¦Àí·½Ê½
            public uint dwMaxRelRecordChanNum;  //±¨¾¯´¥·¢µÄÂ¼ÏóÍ¨µÀ Êý£¨Ö»¶Á£©×î´óÖ§³ÖÊýÁ¿
            public uint dwRelRecordChanNum;     //±¾×éÄÚÊµ¼Ê´¥·¢µÄÂ¼ÏóÍ¨µÀÊý
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = MAX_CHANNUM_V30, ArraySubType = UnmanagedType.U4)]
            public uint[] dwRelRecordChan;    //±¨¾¯´¥·¢µÄÂ¼ÏóÍ¨µÀ(0xffff ffff±íÊ¾ºóÐøÎÞÐ§£©
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = MAX_TIMESEGMENT_V30, ArraySubType = UnmanagedType.Struct)]
            NET_DVR_TIME_DETECTION[] struHolidayTime; //¼ÙÈÕ²¼·ÀÊ±¼ä 
            public byte byDirection;//´¥·¢·½Ïò£º0-±£Áô£»1-È«²¿£»2-ÕýÏò£»3-·´Ïò
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 87, ArraySubType = UnmanagedType.U1)]
            public byte[] byRes;
        };

        public const int NET_DVR_GET_EVENT_TRIGGERS_CAPABILITIES = 3501;

        #region  ¶ÁÈ¡³õÊ¼»¯²ÎÊýÎÄ¼þ
        [DllImport("KERNEL32.DLL", EntryPoint = "GetPrivateProfileStringW", CharSet = CharSet.Unicode)]
        private static extern uint GetPrivateProfileStringByByteArray(string lpAppName, string lpKeyName, string lpDefault, byte[] lpReturnedString, uint nSize, string lpFileName);
        public static string ReadIniData(string Section, string Key, string NoText, string iniFilePath)
        {
            //¼ì²éÎÄ¼þÊÇ·ñ´æÔÚ
            if (File.Exists(iniFilePath))
            {
                //                 StringBuilder temp = new StringBuilder(1024);
                //                 //¶ÁÈ¡ÅäÖÃÎÄ¼þÖÐÄ³¸ökeyµÄÖµ
                //                 GetPrivateProfileString(Section, Key, NoText, temp, 1024, iniFilePath);
                //                 return temp.ToString();
                /*                ReadIniData("DataBase", "DB_SHARE_User", m_DB_SHARE_User, inifile);*/
                byte[] byteAr = new byte[1024];
                uint resultSize = GetPrivateProfileStringByByteArray(Section, Key, NoText, byteAr, (uint)byteAr.Length, iniFilePath);
                string strall = Encoding.Unicode.GetString(byteAr, 0, (int)resultSize * 2);
                return strall;
            }
            else
            {
                //²»´æÔÚÔò·µ»Ø¿Õ
                return String.Empty;
            }
        }
        #endregion


        //Á÷Á¿Í³¼Æ·½Ïò½á¹¹Ìå
        public struct NET_DVR_PDC_ENTER_DIRECTION
        {
            public NET_VCA_POINT struStartPoint; //Á÷Á¿Í³¼Æ·½ÏòÆðÊ¼µã
            public NET_VCA_POINT struEndPoint;    // Á÷Á¿Í³¼Æ·½Ïò½áÊøµã 
        }

        public struct NET_DVR_PDC_RULE_CFG_V41
        {
            public uint dwSize;              //½á¹¹´óÐ¡
            public byte byEnable;             // ÊÇ·ñ¼¤»î¹æÔò;
            [System.Runtime.InteropServices.MarshalAsAttribute(System.Runtime.InteropServices.UnmanagedType.ByValArray, SizeConst = 23, ArraySubType = System.Runtime.InteropServices.UnmanagedType.I1)]
            public byte[] byRes1;    //±£Áô×Ö½Ú
            public NET_VCA_POLYGON struPolygon;            // ¶à±ßÐÎ
            public NET_DVR_PDC_ENTER_DIRECTION struEnterDirection;    // Á÷Á¿½øÈë·½Ïò
            [System.Runtime.InteropServices.MarshalAsAttribute(System.Runtime.InteropServices.UnmanagedType.ByValArray, SizeConst = 56, ArraySubType = System.Runtime.InteropServices.UnmanagedType.Struct)]
            public NET_DVR_SCHEDTIME[] struAlarmTime;//²¼·ÀÊ±¼ä
            public NET_DVR_TIME_EX struDayStartTime; //°×Ìì¿ªÊ¼Ê±¼ä£¬Ê±·ÖÃëÓÐÐ§
            public NET_DVR_TIME_EX struNightStartTime; //Ò¹Íí¿ªÊ¼Ê±¼ä£¬Ê±·ÖÃëÓÐÐ§
            [System.Runtime.InteropServices.MarshalAsAttribute(System.Runtime.InteropServices.UnmanagedType.ByValArray, SizeConst = 100, ArraySubType = System.Runtime.InteropServices.UnmanagedType.I1)]
            public byte[] byRes;    //±£Áô×Ö½Ú
        }

        public struct NET_DVR_PDC_RULE_COND
        {
            public uint dwSize; //½á¹¹´óÐ¡
            public uint dwChannel; //Í¨µÀºÅ
            public uint dwID; //³¡¾°ID£¬¼æÈÝÇò»ú¶à³¡¾°¸ÅÄî£¬¼æÈÝÀÏ°æ±¾SDKÅäÖÃÐÂÉè±¸Ê±£¬±£Áô×Ö½ÚÎª0µÄÇé¿ö£¬ËùÒÔÕâ¸ö×Ö½ÚÎª0Ê±£¬Ò²Ä¬ÈÏÎª³¡¾°1 
            [System.Runtime.InteropServices.MarshalAsAttribute(System.Runtime.InteropServices.UnmanagedType.ByValArray, SizeConst = 60, ArraySubType = System.Runtime.InteropServices.UnmanagedType.I1)]
            public byte[] byRes;    //±£Áô×Ö½Ú 
        }

        //Ïß½á¹¹
        public struct NET_VCA_LINE
        {
            public NET_VCA_POINT struStart;    //Æðµã 
            public NET_VCA_POINT struEnd;      //ÖÕµã
        }


        //ÅäÖÃ½á¹¹
        public struct NET_DVR_PDC_RULE_CFG_V42
        {
            public uint dwSize; //½á¹¹´óÐ¡
            public byte byEnable; //ÊÇ·ñ¼¤»î¹æÔò;
            public byte byOSDEnable;//¿ÍÁ÷Í³¼ÆOSDÏÔÊ¾ÊÇ·ñÆôÓÃ,  0-·ñ£¨0-ÎÞ£©£¬1-ÊÇ£¨0-½øÈëºÍÀë¿ª£©£¬ 2-½øÈë£¬ 3-Àë¿ª   ¶ÔÓÚÀÏÉè±¸µÄ²»ÆôÓÃOSDµþ¼Ó¶ÔÓ¦£¨ÎÞ£©£¬ÆôÓÃ¶ÔÓ¦£¨½øÈë/Àë¿ª£©¡£
            public byte byCurDetectType;//µ±Ç°¼ì²âÇøÓòÀàÐÍ£¬0-¶à±ßÐÎ£¬1-¼ì²âÏß
            public byte byInterferenceSuppression; //¸ÉÈÅÒÖÖÆ£¬°´Î»±íÊ¾,0-Î´¹´Ñ¡,1-¹´Ñ¡£¬bit0-ÒõÓ°£¬bit1-ÅÇ»²£¬bit2-ÍÆ³µ
            NET_VCA_POINT struOSDPoint;//¿ÍÁ÷Í³¼ÆÏÔÊ¾OSDÏÔÊ¾×óÉÏ½Ç×ø±ê
                                       //¿ÍÁ÷Á¿¼ì²âÊý¾ÝÉÏ´«ÖÜÆÚ£¨0-15¡¢1-1¡¢2-5¡¢3-10¡¢4-20¡¢5-30¡¢6-60£©µ¥Î»£º·ÖÖÓ
            public byte byDataUploadCycle;
            //Ã¿ÃëÉÏ´«»úÖÆÊ¹ÄÜ£¨0-¹Ø±Õ£¬1-¿ªÆô£©
            public byte bySECUploadEnable;
            public byte byEmailDayReport;//¿ÍÁ÷ÈÕ±¨±íÊ¹ÄÜ 0-¹Ø±Õ£¬1-¿ªÆô
            public byte byEmailWeekReport;//¿ÍÁ÷ÖÜ±¨±íÊ¹ÄÜ 0-¹Ø±Õ£¬1-¿ªÆô
            public byte byEmailMonthReport;//¿ÍÁ÷ÔÂ±¨±íÊ¹ÄÜ0-¹Ø±Õ£¬1-¿ªÆô
            public byte byEmailYearReport;//¿ÍÁ÷Äê±¨±íÊ¹ÄÜ0-¹Ø±Õ£¬1-¿ªÆô
            [System.Runtime.InteropServices.MarshalAsAttribute(System.Runtime.InteropServices.UnmanagedType.ByValArray, SizeConst = 6, ArraySubType = System.Runtime.InteropServices.UnmanagedType.I1)]
            public byte[] byRes2;    //±£Áô×Ö½Ú 
            public NET_VCA_POLYGON struPolygon; // ¶à±ßÐÎ
            public NET_DVR_PDC_ENTER_DIRECTION struEnterDirection; // Á÷Á¿½øÈë·½Ïò
            [System.Runtime.InteropServices.MarshalAsAttribute(System.Runtime.InteropServices.UnmanagedType.ByValArray, SizeConst = 56, ArraySubType = System.Runtime.InteropServices.UnmanagedType.Struct)]
            public NET_DVR_SCHEDTIME[] struAlarmTime;//²¼·ÀÊ±¼ä
            public NET_DVR_TIME_EX struDayStartTime; //°×Ìì¿ªÊ¼Ê±¼ä£¬Ê±·ÖÃëÓÐÐ§
            public NET_DVR_TIME_EX struNightStartTime; //Ò¹Íí¿ªÊ¼Ê±¼ä£¬Ê±·ÖÃëÓÐÐ§
            public NET_DVR_HANDLEEXCEPTION_V40 struAlarmHandleType; /*´¦Àí·½Ê½ ½öÖ§³ÖÉÏ´«ÖÐÐÄ*/
            public byte byDetecteSensitivity;//Ä¿±ê¼ì²âÁéÃô¶È£º·¶Î§1-100£¬Ä¬ÈÏ50
            public byte byGenerateSpeedSpace;//Ä¿±êÉú³ÉËÙ¶È£¨¿ÕÓò£©£º·¶Î§1-100£¬Ä¬ÈÏ50
            public byte byGenerateSpeedTime;// Ä¿±êÉú³ÉËÙ¶È£¨Ê±Óò£©£º·¶Î§1-100£¬Ä¬ÈÏ50
            public byte byCountSpeed;// ¼ÆÊýËÙ¶È£º·¶Î§1-100£¬Ä¬ÈÏ50
            public byte byDetecteType;// Ä¿±ê¼ì²âÀàÐÍ£º0-×Ô¶¯£¬1-ÈËÍ·£¬2-Í·¼ç£¬Ä¬ÈÏ0-×Ô¶¯£¬×Ô¶¯Ä£Ê½ÏÂDSPµ÷ÕûËã·¨µÄ²ÎÊýÅäÖÃ¸øËã·¨¿â
            public byte byTargetSizeCorrect;//Ä¿±ê³ß´çÐÞÕý£º·¶Î§1-100£¬Ä¬ÈÏ50
            [System.Runtime.InteropServices.MarshalAsAttribute(System.Runtime.InteropServices.UnmanagedType.ByValArray, SizeConst = 2, ArraySubType = System.Runtime.InteropServices.UnmanagedType.I1)]
            public byte[] byRes3;    //±£Áô×Ö½Ú 
            public NET_VCA_LINE struLine;//¼ì²âÏß
            public byte byHeightFilterEnable;//¸ß¶È¹ýÂËÊÇ·ñ¿ªÆô£¬0-¹Ø±Õ£¬1-¿ªÆô
            [System.Runtime.InteropServices.MarshalAsAttribute(System.Runtime.InteropServices.UnmanagedType.ByValArray, SizeConst = 3, ArraySubType = System.Runtime.InteropServices.UnmanagedType.I1)]
            public byte[] byRes4;    //±£Áô×Ö½Ú 
            float fHeightFilter;//¹ýÂË¸ß¶È£¬µ¥Î»£ºÀåÃ×£¬Ä¬ÈÏÖµ£º120ÀåÃ×£¬·¶Î§£º40-200ÀåÃ×¡£byHeightFilterEnableÎª1ÊÇ²ÅÓÐÐ§
            public byte byCalibrateType;//±ê¶¨ÀàÐÍ£¬0-Î´½øÐÐ±ê¶¨£¬1-×Ô¶¯±ê¶¨£¬2-ÊÖ¶¯±ê¶¨
            [System.Runtime.InteropServices.MarshalAsAttribute(System.Runtime.InteropServices.UnmanagedType.ByValArray, SizeConst = 3, ArraySubType = System.Runtime.InteropServices.UnmanagedType.I1)]
            public byte[] byRes6;    //±£Áô×Ö½Ú 
            float fTiltAngle;//¸©Ñö½Ç,µ¥Î»£º¶È£»¸©Ñö½ÇÄ¬ÈÏÖµ£º0£»¸©Ñö½Ç·¶Î§£º0-180¶È,Ö»¶Á
            float fHeelAngle;//ÇãÐ±½Ç,µ¥Î»£º¶È£»ÇãÐ±½ÇÄ¬ÈÏÖµ£º0£»ÇãÐ±½Ç·¶Î§£º-90-90¶È,Ö»¶Á
            float fHeight;//¸ß¶È£¬¸ß¶Èµ¥Î»£ºÀåÃ×£»¸ß¶ÈÄ¬ÈÏÖµ300ÀåÃ×£º¸ß¶È·¶Î§£º200-500ÀåÃ×,µ±byCalibrateTypeÎª2Ê±ÉèÖÃÓÐÐ§£¬ÆäÓàÊ±Ö»¶Á
            NET_VCA_POLYGON struCountPolygon;//¼ÆÊýÇøÓò,Ö»¶ÁÇøÓò
            NET_VCA_POLYGON struAutoCalibPolygon;//±ê¶¨ÇøÓò£¬µ±byCalibrateTypeÎª1Ê±ÓÐÐ§
            [System.Runtime.InteropServices.MarshalAsAttribute(System.Runtime.InteropServices.UnmanagedType.ByValArray, SizeConst = 44, ArraySubType = System.Runtime.InteropServices.UnmanagedType.I1)]
            public byte[] byRes;    //±£Áô×Ö½Ú 
        }

        public const int NET_DVR_SET_POS_INFO_OVERLAY = 3913;//ÉèÖÃPosÐÅÏ¢ÂëÁ÷µþ¼Ó¿ØÖÆ
        public const int NET_DVR_GET_POS_INFO_OVERLAY = 3914;//»ñÈ¡PosÐÅÏ¢ÂëÁ÷µþ¼Ó¿ØÖÆ
        [System.Runtime.InteropServices.StructLayoutAttribute(System.Runtime.InteropServices.LayoutKind.Sequential)]
        public struct NET_DVR_POS_INFO_OVERLAY
        {
            public uint dwSize; //½á¹¹´óÐ¡
            public byte byPosInfoOverlayEnable;// PosÐÅÏ¢ÂëÁ÷µþ¼Ó¿ØÖÆ£¬0-²»µþ¼Ó£¬1-µþ¼Ó
            public byte byOverlayType;//0-µþ¼Ó½øÈëºÍÀë¿ª£¬1-µþ¼Ó½øÈë¡¢Àë¿ª¡¢PASS¡¢ID¡¢¸ß¶È
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 126, ArraySubType = UnmanagedType.I1)]
            public byte[] byRes;    //±£Áô×Ö½Ú 
        }
        //[MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 14, ArraySubType = UnmanagedType.I1)]

        #region ²Ù×÷ÌáÊ¾
        public struct LOCAL_LOG_INFO
        {
            public int iLogType;
            public string strTime;
            public string strLogInfo;
            public string strDevInfo;
            public string strErrInfo;
        }
        public static void AddList(LOCAL_LOG_INFO struLogInfo)
        {

            string strLogType = "";
            switch (struLogInfo.iLogType)
            {
                case HCNetSDK.OPERATION_SUCC_T:
                    strLogType = "SUCC";
                    break;
                case HCNetSDK.OPERATION_FAIL_T:
                    strLogType = "FAIL";
                    break;
                case HCNetSDK.PLAY_SUCC_T:
                    strLogType = "SUCC";
                    break;
                case HCNetSDK.PLAY_FAIL_T:
                    strLogType = "FAIL";
                    break;
                default:
                    strLogType = "FAIL";
                    break;

            }
            string loginfo = "[" + strLogType + "]" + struLogInfo.strLogInfo + "\r\n" + struLogInfo.strErrInfo;
            MessageBox.Show(loginfo, "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }




        #endregion


    }

}
