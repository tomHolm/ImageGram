namespace ImageGram.DB.Container;

public interface IContainerFactory {
    public ICosmosDBContainer getContainer(string name);
}