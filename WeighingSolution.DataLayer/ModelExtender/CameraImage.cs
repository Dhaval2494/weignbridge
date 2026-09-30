namespace WeighingSolution.DataLayer
{
    public partial class CameraImage : NotifyDataErrorInfoBase
    {
        public int TicketID { get; set; }
        public string CameraDirectoryPath
        {
            get { return CameraConstant.CameraImageLocation + "\\" + TicketID; }
        }
        public string CameraImagePath
        {
            get { return CameraConstant.CameraImageLocation + "\\" + TicketID + "\\" + ImageLocation; }
        }
    }
}
