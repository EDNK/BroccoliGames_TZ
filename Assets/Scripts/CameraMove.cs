using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMove : MonoBehaviour
{
    private Camera cam;
    private float vertCamBound, horCamBound;
    
    [SerializeField] private float minCameraSize;

    private float leftX, rightX, topY, bottomY;
    private float deltaX, deltaY, targetX, targetY;
    Vector3 startPos;

    private Vector2 toLeftCorner;

    private bool isMovable=true;

    void Start()
    {
        cam=GetComponent<Camera>();

        // Границы камеры
        vertCamBound = cam.orthographicSize;
        horCamBound = cam.orthographicSize * Screen.width / Screen.height ;

        // Границы карты
        MapCreator.GetBounds(out leftX, out rightX, out bottomY, out topY);

        // Перемещение камеры в центр карты
        transform.position=new Vector3((leftX+rightX)/2,(topY+bottomY)/2, -10f);

        //Вектор от центра камеры к левому верхнему углу
        toLeftCorner=new Vector2(-horCamBound,vertCamBound);
    }

    void Update()
    {
        if (isMovable)
        {

            if(Input.GetMouseButtonDown(0)) startPos=cam.ScreenToWorldPoint(Input.mousePosition);
        
            else if (Input.GetMouseButton(0))
            {
                // Получение смещений по осям X, Y
                deltaX=cam.ScreenToWorldPoint(Input.mousePosition).x-startPos.x;
                deltaY=cam.ScreenToWorldPoint(Input.mousePosition).y-startPos.y;

                // Ограничение камеры в пределах карты. Если весь фон прямоугольный и непрерывный, то работает корректно.
                // При изменении размера камеры в PlayMode метод работает со старыми значениями камеры, т.е. не происходит пересчёт для новой.
                targetX=Mathf.Clamp(transform.position.x-deltaX,leftX+horCamBound,rightX-horCamBound);
                targetY=Mathf.Clamp(transform.position.y-deltaY,bottomY+vertCamBound,topY-vertCamBound);
            
                // Перемещение
                transform.position=new Vector3(targetX,targetY,-10f);
            }
        }
    }

    // Смена режима для камеры (разрешает и запрещает перемещение)
    public void ChangeMoveMode(bool mode)
    {
        isMovable=mode;
    }

    // Возвращает имя ближайшего спрайта к левому правому углу камеры
    // При изменении размера камеры в PlayMode метод работает со старыми значениями камеры, т.е. не происходит пересчёт для новой.
    public string LeftTopCornerSpriteName()
    {
        return MapCreator.GetNearestSpriteName((Vector2)transform.position+toLeftCorner);
    }

}