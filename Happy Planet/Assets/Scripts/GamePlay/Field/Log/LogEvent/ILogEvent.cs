public interface ILogEvent {
    public void Action(IField target, float time, int id, bool isAction = false);
    public void Undo(IField target, float time, int id);
    
}