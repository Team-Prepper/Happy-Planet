using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Xml.Linq;
using UnityEngine;

public class DataManager : MonoSingleton<DataManager>
{
    [SerializeField] Unit _unitPrefab;

    IList<Unit> _units;

    string _path = "UnitData";


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

    public void MapGenerate()
    {
        _units = new List<Unit>();

        XmlDocument xmlDoc = AssetOpener.ReadXML(_path);

        if (xmlDoc == null) return;

        XmlNodeList nodes = xmlDoc.SelectNodes("UnitData/Unit");

        for (int i = 0; i < nodes.Count; i++)
        {
            string unitCode = nodes[i].Attributes["unitCode"].Value;
            Unit newUnit = AssetOpener.ImportGameObject("Prefabs/unit").GetComponent<Unit>();

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

            float instantiateTime = float.Parse(nodes[i].Attributes["createTime"].Value);
            int level = int.Parse(nodes[i].Attributes["level"].Value);

            newUnit.SetInfor(UnitDataManager.Instance.GetUnitData(unitCode), instantiateTime, level);


            _units.Add(newUnit);
        }

    }

    protected override void OnCreate()
    {
        _units = new List<Unit>();
    }

    public void Write(string filePath)
    {

        XmlDocument Document = new XmlDocument();
        XmlElement FList = Document.CreateElement("UnitData");
        Document.AppendChild(FList);

        foreach (Unit infor in _units)
        {
            XmlElement FElement = Document.CreateElement("Unit");

            FElement.SetAttribute("unitCode", infor.GetInfor().UnitCode);
            FElement.SetAttribute("posX", infor.transform.position.x.ToString());
            FElement.SetAttribute("posY", infor.transform.position.y.ToString());
            FElement.SetAttribute("posZ", infor.transform.position.z.ToString());

            FElement.SetAttribute("dirX", infor.transform.up.x.ToString());
            FElement.SetAttribute("dirY", infor.transform.up.y.ToString());
            FElement.SetAttribute("dirZ", infor.transform.up.z.ToString());

            FElement.SetAttribute("createTime", infor.InstantiateTime.ToString());
            FElement.SetAttribute("level", infor.NowLevel.ToString());

            FList.AppendChild(FElement);
        }
        Document.Save("Assets/Resources/XML/" + filePath + ".xml");
    }

}
