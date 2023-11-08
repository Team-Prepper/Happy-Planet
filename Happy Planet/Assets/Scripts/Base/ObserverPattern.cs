namespace ObserberPattern {
    public interface ISubject {
        public void AddObserver(IObserver ops);
        public void RemoveObserver(IObserver ops);
        public void NotifyToObserver();

    }

    public interface IObserver {
        public void Notified();

    }
}