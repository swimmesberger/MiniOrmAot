namespace MiniOrmAot.Mapping; 

public interface IRecord {
    IRecordValues ByColumName();

    IRecordValues ByPropertyName();
}