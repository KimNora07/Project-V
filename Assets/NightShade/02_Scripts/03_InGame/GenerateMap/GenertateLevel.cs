//System
using System.Collections;
using System.Collections.Generic;

//UnityEngine
using UnityEngine;
using UnityEngine.UI;

public class GenertateLevel : MonoBehaviour
{
    public Sprite emptyRoom;    // 기본적인 방 이미지
    public Sprite currentRoom;  // 플레이어가 위치한 방 이미지
    public Sprite unExplored;   // 플레이어가 도달하지 않은 방 이미지
    public Sprite bossRoom;     // 보스 방 이미지
    public Sprite shopRoom;     // 상점 방 이미지
    public Sprite treasureRoom; // 보물방 방 이미지

    private int failSafe = 0;   // 방 생성 실패 방지를 위함
    private bool reGenerating = false;

    private void Awake()
    {
        Init();
    }

    private void Start()
    {
        // 처음 플레이어가 있을 방을 지정
        Room startRoom = new Room();
        startRoom.Location = new Vector2(0, 0);
        startRoom.RoomImage = Level.CurrentRoomIcon;

        // 방을 생성
        Generate(startRoom);
    }

    private void Update()
    {
        // Tab을 눌렀을 때 방을 초기화 시키고 다시 생성
        if(Input.GetKeyDown(KeyCode.Tab) && !reGenerating)
        {
            reGenerating = true;
            Invoke(nameof(StopRegenerating), 1);

            for(int i = transform.childCount - 1; i >= 0; i--)
            {
                Transform child = transform.GetChild(i);
                Destroy(child.gameObject);
            }

            Level.Rooms.Clear();

            Start();
        }
    }

    /// <summary>
    /// 'Level' 정적 클래스에 있는 여러가지 방 아이콘을 지정
    /// </summary>
    private void Init()
    {
        Level.DefaultRoomIcon = emptyRoom;
        Level.CurrentRoomIcon = currentRoom;
        Level.UnExploredRoomIcon = unExplored;
        Level.BossRoomIcon = bossRoom;
        Level.ShopRoomIcon = shopRoom;
        Level.TreasureRoonIcon = treasureRoom;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="room">방</param>
    private void DrawRoomOnMap(Room room)
    {
        // 오브젝트를 생성
        GameObject mapTile = new GameObject("MapTile");     
        
        // 이미지 컴포넌트를 넣는다
        Image roomImage = mapTile.AddComponent<Image>();
        roomImage.sprite = room.RoomImage;
        RectTransform rect = roomImage.GetComponent<RectTransform>();
        rect.sizeDelta = new Vector2(Level.Height, Level.Width) * Level.IconScale;
        rect.position = room.Location * (Level.IconScale * Level.Height * Level.Scale + (Level.Padding * Level.Height * Level.Scale));
        roomImage.transform.SetParent(transform, false);

        Level.Rooms.Add(room);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="vector"></param>
    /// <returns></returns>
    private bool CheckIfRoomExists(Vector2 vector)
    {
        // Exists: 괄호 안에 조건문에 따라 True또는 False를 반환
        // 방의 위치와 vector라는 메소드의 인자값이 같음에 따라 True 또는 False를 반환
        return (Level.Rooms.Exists(x => x.Location == vector));
    }

    /// <summary>
    /// 생성 되야할 방 주변에 생성 되기 전 방향을 제외하고 또 다른 방이 존재하는지 확인 
    /// </summary>
    /// <param name="vector">좌표</param>
    /// <param name="direction">방향</param>
    /// <returns></returns>
    private bool CheckIfRoomsAroundGeneratedRoom(Vector2 vector, string direction)
    {
        switch(direction)
        {
            // 오른쪽에서 왼쪽으로 방이 생성된다고 가정
            case "Right":
                {
                    // 왼쪽, 위, 아래를 확인 
                    if (Level.Rooms.Exists(x => x.Location == new Vector2(vector.x - 1, vector.y)) ||
                        Level.Rooms.Exists(x => x.Location == new Vector2(vector.x, vector.y + 1)) ||
                        Level.Rooms.Exists(x => x.Location == new Vector2(vector.x, vector.y - 1)))
                    {
                        return true;
                    }
                    break;
                }
            // 왼쪽에서 오른쪽으로 방이 생성된다고 가정
            case "Left":
                {
                    // 오른쪽, 위, 아래를 확인 
                    if (Level.Rooms.Exists(x => x.Location == new Vector2(vector.x + 1, vector.y)) ||
                        Level.Rooms.Exists(x => x.Location == new Vector2(vector.x, vector.y + 1)) ||
                        Level.Rooms.Exists(x => x.Location == new Vector2(vector.x, vector.y - 1)))
                    {
                        return true;
                    }
                    break;
                }
            // 위쪽에서 아래쪽으로 방이 생성된다고 가정
            case "Up":
                {
                    // 왼쪽, 오른쪽, 아래를 확인 
                    if (Level.Rooms.Exists(x => x.Location == new Vector2(vector.x - 1, vector.y)) ||
                        Level.Rooms.Exists(x => x.Location == new Vector2(vector.x + 1, vector.y)) ||
                        Level.Rooms.Exists(x => x.Location == new Vector2(vector.x, vector.y - 1)))
                    {
                        return true;
                    }
                    break;
                }
            // 아래쪽에서 위쪽으로 방이 생성된다고 가정
            case "Down":
                {
                    // 왼쪽, 오른쪽, 위를 확인 
                    if (Level.Rooms.Exists(x => x.Location == new Vector2(vector.x - 1, vector.y)) ||
                        Level.Rooms.Exists(x => x.Location == new Vector2(vector.x + 1, vector.y)) ||
                        Level.Rooms.Exists(x => x.Location == new Vector2(vector.x, vector.y + 1)))
                    {
                        return true;
                    }
                    break;
                }
        }

        return false;
    }

    /// <summary>
    /// 방을 생성
    /// </summary>
    /// <param name="room">방</param>
    private void Generate(Room room)
    {
        failSafe++;
        if (failSafe > 50) return;

        DrawRoomOnMap(room);

        // 왼쪽생성
        if (Random.value > Level.RoomGenerationChance)
        {
            Room newRoom = new Room();
            newRoom.Location = new Vector2(-1, 0) + room.Location;
            newRoom.RoomImage = Level.DefaultRoomIcon;

            if (!CheckIfRoomExists(newRoom.Location))
            {
                if (!CheckIfRoomsAroundGeneratedRoom(newRoom.Location, "Right"))
                {
                    if (Mathf.Abs(newRoom.Location.x) < Level.RoomLimit && Mathf.Abs(newRoom.Location.y) < Level.RoomLimit)
                    {
                        Generate(newRoom);
                    }
                }
            }           
        }

        // 오른쪽생성
        if (Random.value > Level.RoomGenerationChance)
        {
            Room newRoom = new Room();
            newRoom.Location = new Vector2(1, 0) + room.Location;
            newRoom.RoomImage = Level.DefaultRoomIcon;

            if (!CheckIfRoomExists(newRoom.Location))
            {
                if (!CheckIfRoomsAroundGeneratedRoom(newRoom.Location, "Left"))
                {
                    if (Mathf.Abs(newRoom.Location.x) < Level.RoomLimit && Mathf.Abs(newRoom.Location.y) < Level.RoomLimit)
                    {
                        Generate(newRoom);
                    }
                }
            }
        }

        // 위쪽생성
        if (Random.value > Level.RoomGenerationChance)
        {
            Room newRoom = new Room();
            newRoom.Location = new Vector2(0, 1) + room.Location;
            newRoom.RoomImage = Level.DefaultRoomIcon;

            if (!CheckIfRoomExists(newRoom.Location))
            {
                if (!CheckIfRoomsAroundGeneratedRoom(newRoom.Location, "Down"))
                {
                    if (Mathf.Abs(newRoom.Location.x) < Level.RoomLimit && Mathf.Abs(newRoom.Location.y) < Level.RoomLimit)
                    {
                        Generate(newRoom);
                    }
                }
            }
        }

        // 아래쪽생성
        if (Random.value > Level.RoomGenerationChance)
        {
            Room newRoom = new Room();
            newRoom.Location = new Vector2(0, -1) + room.Location;
            newRoom.RoomImage = Level.DefaultRoomIcon;

            if (!CheckIfRoomExists(newRoom.Location))
            {
                if (!CheckIfRoomsAroundGeneratedRoom(newRoom.Location, "Up"))
                {
                    if (Mathf.Abs(newRoom.Location.x) < Level.RoomLimit && Mathf.Abs(newRoom.Location.y) < Level.RoomLimit)
                    {
                        Generate(newRoom);
                    }
                }
            }
        }

        GenerateBossRoom();
    }

    /// <summary>
    /// 
    /// </summary>
    private void StopRegenerating()
    {
        reGenerating = false;
    }

    #region Rooms Generation
    /// <summary>
    /// 
    /// </summary>
    private void GenerateBossRoom()
    {
        float maxNumber = 0;
        Vector2 fathestRoom = Vector2.zero;

        foreach(Room r in Level.Rooms)
        {
            if(Mathf.Abs(r.Location.x) + Mathf.Abs(r.Location.y) >= maxNumber)
            {
                maxNumber = Mathf.Abs(r.Location.x) + Mathf.Abs(r.Location.y);
                fathestRoom = r.Location;
            }
        }

        Room bossRoom = new Room();
        bossRoom.RoomImage = Level.BossRoomIcon;
        bossRoom.RoomNumber = 3;

        // 왼쪽
        if (!CheckIfRoomExists(fathestRoom))
            if (!CheckIfRoomsAroundGeneratedRoom(fathestRoom + new Vector2(-1, 0), "Right"))
            {
                bossRoom.Location = fathestRoom + new Vector2(-1, 0);
            }

        // 오른쪽
        else if (!CheckIfRoomExists(fathestRoom))
            if (!CheckIfRoomsAroundGeneratedRoom(fathestRoom + new Vector2(1, 0), "Left"))
            {
                bossRoom.Location = fathestRoom + new Vector2(1, 0);
            }

        // 위쪽
        else if (!CheckIfRoomExists(fathestRoom))
            if (!CheckIfRoomsAroundGeneratedRoom(fathestRoom + new Vector2(0, 1), "Down"))
            {
                bossRoom.Location = fathestRoom + new Vector2(0, 1);
            }

        // 아래쪽
        else if (!CheckIfRoomExists(fathestRoom))
            if (!CheckIfRoomsAroundGeneratedRoom(fathestRoom + new Vector2(0, -1), "Up"))
            {
                bossRoom.Location = fathestRoom + new Vector2(0, -1);
            }

        DrawRoomOnMap(bossRoom);

    }
    #endregion
}
