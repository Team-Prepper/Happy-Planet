using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IRecord<T> {

    public T Cur { get; }

    public void Push(T data);

    public T Undo();

    public T Redo();

}
