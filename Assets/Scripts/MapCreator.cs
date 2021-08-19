using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapCreator : MonoBehaviour
{
    [SerializeField] private Sprite[] spriteMas;    
    [SerializeField] private TextAsset textJSON;
    [SerializeField] private GameObject SpritePrefab;
    private Dictionary<string, Sprite> spriteDict;

    private static float leftX, rightX, bottomY, topY;

    // Класс для сбора информации из JSON
    [System.Serializable]
    public class MapPart
    {
        public string Id="";
        public float X=0f;
        public float Y=0f;
        public float Width;
        public float Height;
    }

    // Класс с полем - массивом предыдущего класса. Послужит контейнером информации с JSON
    [System.Serializable]
    public class MapPartList
    {
        public MapPart[] List;
    }

    public static MapPartList myMapPartList = new MapPartList();
    
    void Awake()
    {
        // Создание и заполнение словаря для поиска спрайта по имени
        spriteDict=new Dictionary <string,Sprite> (spriteMas.Length);
        for(int i=0;i<spriteMas.Length;i++)
        {
            spriteDict.Add(spriteMas[i].name,spriteMas[i]);
        }
        
        // Чтение файла JSON
        myMapPartList=JsonUtility.FromJson<MapPartList>(textJSON.text);
        
        // Цикл по элементам массива, позиционирование спрайтов и нахождение крайних точек фона
        foreach (var item in myMapPartList.List)
        {
            if (item.X+item.Width/2>rightX||rightX==null) rightX=item.X+item.Width/2;
            else if (item.X-item.Width/2<leftX||leftX==null) leftX=item.X-item.Width/2;

            if (item.Y+item.Height/2>topY||topY==null) topY=item.Y+item.Height/2;
            else if (item.Y-item.Height/2<bottomY||bottomY==null) bottomY=item.Y-item.Height/2;
            
            var tmp = Instantiate(SpritePrefab, new Vector3(item.X, item.Y, 0f), Quaternion.identity);
            tmp.GetComponent<SpriteRenderer>().sprite=spriteDict[item.Id];
        }
    }

    // Метод отдаёт координаты левого верхнего и правого нижнего углов фона
    public static void GetBounds(out float minX, out float maxX, out float minY, out float maxY)
    {
        minX=leftX;
        maxX=rightX;
        minY=bottomY;
        maxY=topY;
    }

    // Метод возвращает имя ближайшего спрайта к точке (X,Y) из вектора
    public static string GetNearestSpriteName(Vector2 pos)
    {
        float maxDistance=float.PositiveInfinity;
        string result="";

        float tmp;
        foreach (var item in myMapPartList.List)
        {
            tmp=Mathf.Sqrt(Mathf.Pow(pos.x-item.X,2)+Mathf.Pow(pos.y-item.Y,2));
            
            if (tmp<=maxDistance) 
            {
                maxDistance=tmp;
                result=item.Id;
            }
        }
        return result;
    }
}