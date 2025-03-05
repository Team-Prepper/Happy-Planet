class LevelUpEvent : ILogEvent {

    public override string ToString()
    {
        return "03/";
    }
    
    public void Action(IField target, float time, int id, bool isAction = false)
    {
        if (!isAction)
            SoundManager.Instance.PlaySound("LevelUp", "VFX");
        target.GetUnit(id)?.LevelUp(time, isAction);
    }

    public void Undo(IField target, float time, int id)
    {
        SoundManager.Instance.PlaySound("LevelDown", "VFX");
        target.GetUnit(id)?.LevelDown(time);
    }

}
