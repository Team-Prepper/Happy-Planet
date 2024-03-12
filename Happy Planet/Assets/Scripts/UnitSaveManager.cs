using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Xml.Linq;
using UnityEngine;

public class UnitSaveManager : MonoSingleton<UnitSaveManager>
{
    [SerializeField] Unit _unitPrefab;

    List<Unit> _units;

    string _path = "Assets/Resources/UnitData.xml";


    public void AddUnit(Unit data) { 
        _units.Add(data);
        SaveFurnitureXML();
    }

    public void Null() {
        return;
    }
    public void SaveFurnitureXML()
    {
        Write(_path);
    }

    protected override void OnCreate()
    {
        _units = new List<Unit>();

        XmlDocument xmlDoc = new XmlDocument();
        try
        {
            xmlDoc.Load(_path);
        }
        catch (FileNotFoundException)
        {
            return;
        }

        XmlNodeList nodes = xmlDoc.SelectNodes("UnitData/Unit");

        for (int i = 0; i < nodes.Count; i++)
        {
            string unitCode = nodes[i].Attributes["unitCode"].Value;
            Unit newUnit = Instantiate(_unitPrefab);

            newUnit.SetInfor(unitCode, 0, 0);

            Vector3 pos = new Vector3();
            pos.x = float.Parse(nodes[i].Attributes["posX"].Value);
            pos.y = float.Parse(nodes[i].Attributes["posY"].Value);
            pos.z = float.Parse(nodes[i].Attributes["posZ"].Value);

            Vector3 dir = new Vector3();
            dir.x = float.Parse(nodes[i].Attributes["dirX"].Value);
            dir.y = float.Parse(nodes[i].Attributes["dirY"].Value);
            dir.z = float.Parse(nodes[i].Attributes["dirZ"].Value);

            newUnit.transform.position = pos;
            newUnit.transform.up = dir;

            _units.Add(newUnit);
        }
    }

    public void Write(string filePath)
    {

        XmlDocument Document = new XmlDocument();
        XmlElement FList = Document.CreateElement("UnitData");
        Document.AppendChild(FList);

        foreach (Unit infor in _units)
        {
            XmlElement FElement = Document.CreateElement("Unit");
            FElement.SetAttribute("posX", infor.transform.position.x.ToString());
            FElement.SetAttribute("posY", infor.transform.position.y.ToString());
            FElement.SetAttribute("posZ", infor.transform.position.z.ToString());
            FElement.SetAttribute("dirX", infor.transform.up.x.ToString());
            FElement.SetAttribute("dirY", infor.transform.up.y.ToString());
            FElement.SetAttribute("dirZ", infor.transform.up.z.ToString());
            FList.AppendChild(FElement);
        }
        Document.Save(filePath);
    }

}
