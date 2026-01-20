namespace SwissDwellings
{
    public interface IMsdLoader
    {
        SwissDwellings.Data.MsdBuilding Load(string jsonFilePath);
        SwissDwellings.Data.MsdGraph LoadStructureOnly(string jsonFilePath);
    }
}
