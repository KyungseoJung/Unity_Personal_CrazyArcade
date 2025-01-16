using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyCtrl : MonoBehaviour
{
    private Vector3 pos;                   // #101 Enemy가 게임 맵 경계선 밖으로 넘어가지 않도록 확인
    
    [Header("Movement Settings")]
    public float moveForce = 50f;              // 적의 이동 속도
    public float maxSpeed = 2f;                        // 적의 최대 속도
    public float detectionDistance = 0.3f;      // 장애물 감지 거리
    public LayerMask obstacleLayer;           // 장애물 레이어

    private Rigidbody rBody;
    private Vector2 currentDirection;
    private Vector2[] directions = new Vector2[]
    {
        Vector2.up,
        Vector2.down,
        Vector2.left,
        Vector2.right
    };

    void Start()
    {
        rBody = GetComponent<Rigidbody>();
        ChooseNewDirection();   // 시작 시 초기 방향 선택
        obstacleLayer = LayerMask.GetMask("Obstacle");  // 장애물은 "Obstacle" Layer로 구분.
    }

    // Update is called once per frame
    void Update()
    {
        Patrol();
    }

    void FixedUpdate()
    {
        CheckBorder();  // #101 Enemy가 게임 맵 경계선 밖으로 넘어가지 않도록 확인
    }


    private void CheckBorder()
    {
        // #101 플레이어가 게임 맵 경계선 밖으로 넘어가지 않도록 확인
        if((transform.position.x) * (transform.position.x) > 4*4 )
        {
            // Debug.Log("// #101 Enemy(Lodumani)가 x좌표 경계선 넘어감");
            pos = this.transform.position;
            pos.x = (int)this.transform.position.x; // -4 또는 4로 지정
            this.transform.position = pos;

            ChooseNewDirection();   // 다른 방향으로 이동하도록
        }
        
        if((transform.position.y) * (transform.position.y) > 3*3)
        {
            // Debug.Log("// #101 Enemy(Lodumani)가 y좌표 경계선 넘어감");
            pos = this.transform.position;
            pos.y = (int)this.transform.position.y; // -3 또는 3으로 지정
            this.transform.position = pos;

            ChooseNewDirection();   // 다른 방향으로 이동하도록
        }

    }

    void Patrol()
    {
        // 현재 방향으로 힘을 가해 이동
        rBody.AddForce(currentDirection * moveForce);

        // 속도 클램핑
        ClampVelocity();

        // 장애물 감지 및 방향 전환
        if (IsObstacleAhead())
        {
            ChooseNewDirection();
        }
    }

    bool IsObstacleAhead()
    {
        Vector2 rayOrigin = rBody.position;
        RaycastHit2D hit = Physics2D.Raycast(rayOrigin, currentDirection, detectionDistance, obstacleLayer);
        if (hit.collider != null)
        {
            Debug.DrawLine(rayOrigin, rayOrigin + currentDirection * detectionDistance, Color.red);
            return true;
        }
        else
        {
            Debug.DrawLine(rayOrigin, rayOrigin + currentDirection * detectionDistance, Color.green);
            return false;
        }
    }

    void ChooseNewDirection()
    {
        Debug.Log("#101 Enemy의 새로운 이동 방향 탐색");
        Vector2[] possibleDirections = GetPossibleDirections();

        if (possibleDirections.Length > 0)
        {
            // 랜덤으로 새로운 방향 선택
            currentDirection = possibleDirections[Random.Range(0, possibleDirections.Length)];
            Debug.Log($"#101 Enemy direction changed to: {currentDirection}");
        }
        else
        {
            // 모든 방향에 장애물이 있는 경우 정지
            rBody.velocity = Vector2.zero;
            Debug.Log("#101 No available directions. Enemy stopped.");
        }
    }

    Vector2[] GetPossibleDirections()
    {
        var availableDirections = new System.Collections.Generic.List<Vector2>();

        foreach (var dir in directions)
        {
            RaycastHit2D hit = Physics2D.Raycast(rBody.position, dir, detectionDistance, obstacleLayer);
            if (hit.collider == null)
            {
                availableDirections.Add(dir);
            }
        }

        return availableDirections.ToArray();
    }

    void ClampVelocity()
    {
        Vector2 clampedVelocity = rBody.velocity;

        if (Mathf.Abs(clampedVelocity.x) > maxSpeed)
        {
            clampedVelocity.x = Mathf.Sign(clampedVelocity.x) * maxSpeed;
        }

        if (Mathf.Abs(clampedVelocity.y) > maxSpeed)
        {
            clampedVelocity.y = Mathf.Sign(clampedVelocity.y) * maxSpeed;
        }

        rBody.velocity = clampedVelocity;
    }
}
