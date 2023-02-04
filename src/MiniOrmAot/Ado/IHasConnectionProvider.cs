namespace MiniOrmAot.Ado; 

public interface IHasConnectionProvider {
    IConnectionProvider ConnectionProvider { get; }
}