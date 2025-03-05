using System.Collections.Generic;

public class UnitList {

    private IList<IUnit> _units = new List<IUnit>();

    public IUnit GetUnit(int id) => _units[id];

    public void AddUnit(IUnit newUnit) {
        newUnit.SetId(_units.Count);
        _units.Add(newUnit);
    }

    public void RegisterUnit(int id, IUnit unit)
    {
        while (id >= _units.Count)
        {
            _units.Add(null);
        }

        _units[id] = unit;
    }

    public void UnregisterUnit(int id)
    {
        if (id < _units.Count - 1)
        {
            _units[id] = null;
            return;
        }

        _units.RemoveAt(_units.Count - 1);

        while (_units.Count > 0 && _units[_units.Count - 1] == null)
        {
            _units.RemoveAt(_units.Count - 1);
        }
    }

    public void Clear(float time) {

        foreach (IUnit unit in _units) {
            if (unit == null) continue;
            unit.Remove(time);
        }
        _units = new List<IUnit>();

    }

}