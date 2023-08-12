using System.Collections;

public interface ISubject {
    public void RegisterObserver(IObserver observer);
    public void RemoveObserver(IObserver observer);
    public void Notify();
}

public interface IObserver {
    public void UpdateData();
    public void Attach();
}
