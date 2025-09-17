class LevelUpEvent : ILogEvent {

    public override string ToString()
    {
        return "03/";
    }
    
    public void Action(IField target, float time, int id, bool isAction = false)
    {
        target.GetUnit(id)?.LevelUp(time, isAction);
    }

    public void Undo(IField target, float time, int id)
    {
        target.GetUnit(id)?.LevelDown(time);
    }

}
