namespace WeighingSolution.DataLayer
{
    public partial class Feature : NotifyDataErrorInfoBase
    {
        //WITH FeaturesCTE (FeatureID, FeatureName, Action, SortOrder, ParentFeatureID, IsMaster, Level)
        //AS
        //(
        //  SELECT FeatureID, FeatureName, Action, SortOrder, ParentFeatureID, IsMaster, 0 AS Level
        //  FROM Feature m
        //  WHERE IsMaster=1
        //  UNION ALL
        //  SELECT m.FeatureID, m.FeatureName, m.Action, m.SortOrder, m.ParentFeatureID, m.IsMaster, Level+1 AS Level
        //  FROM Feature m
        //  INNER JOIN FeaturesCTE p
        //    ON p.FeatureID = m.ParentFeatureID
        //)
        //SELECT * FROM FeaturesCTE order by SortOrder
        public bool CreateVisible
        {
            get { return Action.Contains("C"); }
        }

        public bool ReadVisible
        {
            get { return Action.Contains("R"); }
        }

        public bool UpdateVisible
        {
            get { return Action.Contains("U"); }
        }

        public bool DeleteVisible
        {
            get { return Action.Contains("D"); }
        }

        public bool ExecuteVisible
        {
            get { return Action.Contains("E"); }
        }
    }
}