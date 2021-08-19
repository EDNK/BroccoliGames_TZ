using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMove : MonoBehaviour
{
    // Камера и половины длины и ширины камеры в юнитах
    private Camera cam;
    [SerializeField] private float vertCamBound, horCamBound;
    
    // Поля-ограничения размера камеры. min задаётся в инспекторе, max высчитывается автоматически
    [SerializeField] private float minCameraSize;
    private float maxCameraSize;

    // Соотношение сторон экрана
    private float screenAspect;

    private Vector3 startPos;
    
    // Границы карты
    private float leftX, rightX, topY, bottomY;

    // Вектор от центра камеры к левому верхнему углу для нахождения нужного спрайта
    private Vector2 toLeftCorner;

    // Разрешает/запрещает изменение позиции/зума камеры
    private bool isMovable=true;

    void Start()
    {
        cam=GetComponent<Camera>();
        screenAspect = Screen.width / Screen.height;

        // Границы камеры
        vertCamBound = cam.orthographicSize;
        horCamBound = cam.orthographicSize * screenAspect;

        // Границы карты
        MapCreator.GetBounds(out leftX, out rightX, out bottomY, out topY);

        // Определение максимального размера камеры
        maxCameraSize=Mathf.Min((topY-bottomY)/2,(rightX-leftX)/(2*screenAspect));

        // Перемещение камеры в центр карты
        transform.position=new Vector3((leftX+rightX)/2,(topY+bottomY)/2, -10f);

        //Вектор от центра камеры к левому верхнему углу
        toLeftCorner=new Vector2(-horCamBound,vertCamBound);
    }

    void Update()
    {
        SizeHandle();
        MoveHandle();
    }

    private void SizeHandle()
    {
        if (isMovable)
        {
            float mw = Input.GetAxis("Mouse ScrollWheel");
            if (mw!=0) 
            {
                // Применение зума
                cam.orthographicSize-=mw;

                //Ограничение зума, чтобы камера не выпирала за границы карты
                cam.orthographicSize=Mathf.Clamp(cam.orthographicSize,minCameraSize,maxCameraSize);

                // Вычисление новых границ камеры
                vertCamBound = cam.orthographicSize;
                horCamBound = cam.orthographicSize * Screen.width / Screen.height;
                ClampCamera();
            }
        }
    }

    private void MoveHandle()
    {
        if (isMovable)
        {
            float deltaX, deltaY, targetX, targetY;
            

            if(Input.GetMouseButtonDown(0)) startPos=cam.ScreenToWorldPoint(Input.mousePosition);
        
            else if (Input.GetMouseButton(0))
            {
                // Получение смещений по осям X, Y
                deltaX=cam.ScreenToWorldPoint(Input.mousePosition).x-startPos.x;
                deltaY=cam.ScreenToWorldPoint(Input.mousePosition).y-startPos.y;

                targetX=transform.position.x-deltaX;
                targetY=transform.position.y-deltaY;

                // Перемещение
                transform.position=new Vector3(targetX,targetY,-10f);

                ClampCamera();
            }
        }
    }

    // Метод ограничивает камеру при изменении размера/позиции, пересчитывает вектор к левому верхнему углу
    private void ClampCamera()
    {
        float targetX, targetY;

        targetX=Mathf.Clamp(transform.position.x,leftX+horCamBound,rightX-horCamBound);
        targetY=Mathf.Clamp(transform.position.y,bottomY+vertCamBound,topY-vertCamBound);
        
        transform.position=new Vector3(targetX,targetY,-10f);

        toLeftCorner=new Vector2(-horCamBound,vertCamBound);
    }

    // Смена режима для камеры (разрешает и запрещает перемещение)
    public void ChangeMoveMode(bool mode)
    {
        isMovable=mode;
    }

    // Возвращает имя ближайшего спрайта к левому правому углу камеры
    public string LeftTopCornerSpriteName()
    {
        return MapCreator.GetNearestSpriteName((Vector2)transform.position+toLeftCorner);
    }

}