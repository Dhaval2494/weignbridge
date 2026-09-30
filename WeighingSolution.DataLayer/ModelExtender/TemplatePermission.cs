namespace WeighingSolution.DataLayer
{
    public partial class TemplatePermission : NotifyDataErrorInfoBase
    {
        public bool Create
        {
            get { return PermissionSet[0] == '1'; }
            set { PermissionSet = PermissionSet.ReplaceAt(0, value ? '1' : '0'); OnPropertyChanged(); }
        }
        public bool Read
        {
            get { return PermissionSet[1] == '1'; }
            set { PermissionSet = PermissionSet.ReplaceAt(1, value ? '1' : '0'); OnPropertyChanged(); }
        }
        public bool Update
        {
            get { return PermissionSet[2] == '1'; }
            set { PermissionSet = PermissionSet.ReplaceAt(2, value ? '1' : '0'); OnPropertyChanged(); }
        }
        public bool Delete
        {
            get { return PermissionSet[3] == '1'; }
            set { PermissionSet = PermissionSet.ReplaceAt(3, value ? '1' : '0'); OnPropertyChanged(); }
        }
        public bool Execute
        {
            get { return PermissionSet[4] == '1'; }
            set { PermissionSet = PermissionSet.ReplaceAt(4, value ? '1' : '0'); OnPropertyChanged(); }
        }
    }
}
