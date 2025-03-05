using System.Collections.Generic;

[System.Serializable]
public class Record<T> : IRecord<T> {
    public List<T> _elementList;
    int _idx;

    public T Cur { get { return _elementList[_idx]; } }

    public Record()
    {
        _elementList = new List<T>();
        _idx = 0;

    }

    public void Push(T data)
    {
        if (_idx < _elementList.Count)
            _elementList.RemoveRange(_idx, _elementList.Count - _idx);

        _elementList.Add(data);
        _idx++;
    }

    public T Undo()
    {
        if (_idx < 1)
            return default;

        return _elementList[--_idx];

    }

    public T Redo()
    {
        if (_idx > _elementList.Count - 1)
            return default;

        return _elementList[_idx++];

    }
}
