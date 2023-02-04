namespace MiniOrmAot.Mapping; 

public interface IPropertyMapping {
    IPropertyMapper ByColumName();

    IPropertyMapper ByPropertyName();
}