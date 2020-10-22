using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    private float MAX_SWIPE_TIME = 0.5f;
    float MIN_SWIPE_DISTANCE = 0.17f;
    float startTime;
    Vector2 startPos;
    Vector2 currentDirection = Vector2.zero;
    Vector2 newDirection = Vector2.zero;
    Quaternion newRotation;

    float speed = 4.5f;

    bool isMoving;
    bool playerInsideScreen = true;
    Vector2 originPos;
    Vector2 targetPos;
    float timeToMove = 0.2f;
    float leftConstraint = Screen.width;
    float rightConstraint = Screen.width;
    float bottomConstraint = Screen.height;
    float topConstraint = Screen.height;
    Vector2 cellSize;

    void Start(){
        //Screen constraints
        Camera cam = Camera.main;
        float distanceZ = Mathf.Abs(cam.transform.position.z + transform.position.z);

        leftConstraint = cam.ScreenToWorldPoint(new Vector3(0.0f, 0.0f, distanceZ)).x;
        rightConstraint = cam.ScreenToWorldPoint(new Vector3(Screen.width, 0.0f, distanceZ)).x;
        bottomConstraint = cam.ScreenToWorldPoint(new Vector3(0.0f, 0.0f, distanceZ)).y;
        topConstraint = cam.ScreenToWorldPoint(new Vector3(0.0f, Screen.height, distanceZ)).y;

        //cell size of the grid
        cellSize = GameObject.Find("GridManager").GetComponent<GridUtil>().getCellSize();
    }

    void Update()
    {   
        //Screen transition Check and Swap
        if (transform.position.x < leftConstraint) {
            playerInsideScreen = false;
            if(currentDirection==Vector2.left)
            transform.position = new Vector3(Mathf.Abs(transform.position.x), transform.position.y, transform.position.z);
        }
        else if (transform.position.x > rightConstraint) {
            playerInsideScreen = false;
            if(currentDirection==Vector2.right)
            transform.position = new Vector3(-transform.position.x, transform.position.y, transform.position.z);
        }
        else if (transform.position.y < bottomConstraint) {
            playerInsideScreen = false;
            if(currentDirection==Vector2.down)
            transform.position = new Vector3(transform.position.x, Mathf.Abs(transform.position.y), transform.position.z);
        }
        else if (transform.position.y > topConstraint) {
            playerInsideScreen = false;
            if(currentDirection==Vector2.up)
            transform.position = new Vector3(transform.position.x, -transform.position.y, transform.position.z);
        }else{
            playerInsideScreen = true;
        }


        //Swipe Input
        if(Input.touches.Length > 0 && playerInsideScreen){
            Touch t = Input.GetTouch(0);
            if(t.phase == TouchPhase.Began){
                startPos = t.position;
                startTime = Time.time;
            }
            if(t.phase == TouchPhase.Ended){
                if(Time.time - startTime > MAX_SWIPE_TIME)
                return;

                Vector2 endPos = t.position;
                Vector2 swipe = new Vector2(endPos.x-startPos.x,endPos.y-startPos.y);

                if(swipe.magnitude < MIN_SWIPE_DISTANCE)
                return;

                if(Mathf.Abs(swipe.x) > Mathf.Abs(swipe.y)){
                    if(swipe.x > 0){
                        newRotation = Quaternion.Euler(0,0,-90);
                        newDirection = Vector2.right;
                    }else{
                        newRotation = Quaternion.Euler(0,0,90);
                        newDirection = Vector2.left;
                    }
                }else{
                    if(swipe.y > 0){
                        newRotation = Quaternion.Euler(0,0,0);
                        newDirection = Vector2.up;
                    }else{
                        newRotation = Quaternion.Euler(0,0,180);
                        newDirection = Vector2.down;
                    }
                }
            }
        }

        //WASD Input
        if(!isMoving && playerInsideScreen){
            if(Input.GetKey(KeyCode.W)){
                newRotation = Quaternion.Euler(0,0,0);
                newDirection = Vector2.up;
            }
            if(Input.GetKey(KeyCode.A)){
                newRotation = Quaternion.Euler(0,0,90);
                newDirection = Vector2.left;
            }
            if(Input.GetKey(KeyCode.S)){
                newRotation = Quaternion.Euler(0,0,180);
                newDirection = Vector2.down;
            }
            if(Input.GetKey(KeyCode.D)){
                newRotation = Quaternion.Euler(0,0,-90);
                newDirection = Vector2.right;
            }
        }

        //Movement Loop
        if(!isMoving){
            if(!newDirection.Equals(currentDirection)){
                currentDirection = newDirection;
                transform.rotation = newRotation;
            }
            StartCoroutine(Move());
        }
    }


    IEnumerator Move(){//Movement Logic
        isMoving = true;
        float elaspedTime = 0;
        originPos = transform.position;
        targetPos = originPos + currentDirection * cellSize;
        float distance = Vector2.Distance(originPos,targetPos);
        timeToMove = distance/speed;//varies on distance

        while(elaspedTime<timeToMove){
            transform.position = Vector2.Lerp(originPos,targetPos,(elaspedTime/timeToMove));
            elaspedTime += Time.deltaTime;
            yield return null;
        }
        transform.position = targetPos;
        isMoving = false;
    }
}
