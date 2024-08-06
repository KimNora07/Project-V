//System
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;


//UnityEngine
using UnityEngine;
using UnityEngine.UI;

public class GenertateLevel : MonoBehaviour
{
    // 방 번호
    // 시작방 : 0
    // 보스방 : 1
    // 상점방 : 2
    // 보물방 : 3
    // 비밀방 : 4

    public Sprite emptyRoom;    // 기본적인 방 이미지
    public Sprite currentRoom;  // 플레이어가 위치한 방 이미지
    public Sprite unExploredRoom;   // 플레이어가 도달하지 않은 방 이미지
    public Sprite bossRoom;     // 보스 방 이미지
    public Sprite shopRoom;     // 상점 방 이미지
    public Sprite treasureRoom; // 보물방 방 이미지
    public Sprite secretRoom;

    private int failSafe = 0;   // 방 생성 실패 방지를 위함
    private int maxtries = 0;
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
        startRoom.RoomNumber = 0;

        // 방을 생성
        Generate(startRoom);
    }

    private void Update()
    {
        // Tab을 눌렀을 때 방을 초기화 시키고 다시 생성
        if(Input.GetKey(KeyCode.Tab) && !reGenerating)
        {
            Regenerate();
        }

        if (Input.GetKey(KeyCode.P) && !reGenerating)
        {
            reGenerating = true;
            Invoke(nameof(StopRegenerating), 1);

            string log = "Room List:\n--------------------\n";

            foreach(Room room in Level.Rooms)
            {
                Debug.Log("Room#:" +  room.RoomNumber + " Location: " + room.Location);
            }
            Debug.Log(log);
        }
    }

    /// <summary>
    /// 'Level' 정적 클래스에 있는 여러가지 방 아이콘을 지정
    /// </summary>
    private void Init()
    {
        Level.DefaultRoomIcon = emptyRoom;
        Level.CurrentRoomIcon = currentRoom;
        Level.UnExploredRoomIcon = unExploredRoom;
        Level.BossRoomIcon = bossRoom;
        Level.ShopRoomIcon = shopRoom;
        Level.TreasureRoonIcon = treasureRoom;
        Level.SecretRoomIcon = secretRoom;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="room">방</param>
    private void DrawRoomOnMap(Room room)
    {
        string tileName = "MapTile";

        switch (room.RoomNumber)
        {
            case 1: tileName = "BossRoomTile"; break;
            case 2: tileName = "ShopRoomTile"; break;
            case 3: tileName = "TreasureRoomTile"; break;
        }

        // 오브젝트를 생성
        GameObject mapTile = new GameObject(tileName);     
        
        // 이미지 컴포넌트를 넣는다
        Image roomImage = mapTile.AddComponent<Image>();
        roomImage.sprite = room.RoomImage;
        RectTransform rect = roomImage.GetComponent<RectTransform>();
        rect.sizeDelta = new Vector2(Level.Height, Level.Width) * Level.IconScale;
        rect.position = room.Location * (Level.IconScale * Level.Height * Level.Scale + (Level.Padding * Level.Height * Level.Scale));
        roomImage.transform.SetParent(transform, false);

        Level.Rooms.Add(room);
        Debug.Log("Drawing Room:" + room.RoomNumber + " at Location " + room.Location);
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
            newRoom.RoomNumber = GetRandomRoomNumber();

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
            newRoom.RoomNumber = GetRandomRoomNumber();

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
            newRoom.RoomNumber = GetRandomRoomNumber();

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
            newRoom.RoomNumber = GetRandomRoomNumber();

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
        bool isShop = GenerateSpecialRoom(Level.ShopRoomIcon, 2);
        bool isTreasure = GenerateSpecialRoom(Level.TreasureRoonIcon, 3);
        bool isSecret = GenerateSpecialRoom(Level.SecretRoomIcon, 4);

        if (!isShop || !isTreasure || !isSecret)
        {
            Regenerate();
        }
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
        bossRoom.RoomNumber = 1;

        // 왼쪽
        if (!CheckIfRoomExists(fathestRoom + new Vector2(-1, 0)))
        {
            if (!CheckIfRoomsAroundGeneratedRoom(fathestRoom + new Vector2(-1, 0), "Right"))
            {
                bossRoom.Location = fathestRoom + new Vector2(-1, 0);
            }
        }


        // 오른쪽
        else if (!CheckIfRoomExists(fathestRoom + new Vector2(1, 0)))
        {
            if (!CheckIfRoomsAroundGeneratedRoom(fathestRoom + new Vector2(1, 0), "Left"))
            {
                bossRoom.Location = fathestRoom + new Vector2(1, 0);
            }
        }

        // 위쪽
        else if (!CheckIfRoomExists(fathestRoom + new Vector2(0, 1)))
        {
            if (!CheckIfRoomsAroundGeneratedRoom(fathestRoom + new Vector2(0, 1), "Down"))
            {
                bossRoom.Location = fathestRoom + new Vector2(0, 1);
            }
        }

        // 아래쪽
        else if (!CheckIfRoomExists(fathestRoom + new Vector2(0, -1)))
        {
            if (!CheckIfRoomsAroundGeneratedRoom(fathestRoom + new Vector2(0, -1), "Up"))
            {
                bossRoom.Location = fathestRoom + new Vector2(0, -1);
            }
        }

        DrawRoomOnMap(bossRoom);

    }
    #endregion

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    private int GetRandomRoomNumber()
    {
        return 6;
    }

    private bool GenerateSpecialRoom(Sprite mapIcon, int roomNumber)
    {
        List<Room> shuffledList = new List<Room>(Level.Rooms);

        Room specialRoom = new Room();
        specialRoom.RoomImage = mapIcon;
        specialRoom.RoomNumber = roomNumber;

        bool foundAvailableLocation = false;

        foreach (Room room in shuffledList)
        {
            Vector2 specialRoomLocation = room.Location;

            if (room.RoomNumber < 6) continue;

            // 왼쪽
            if (!CheckIfRoomExists(specialRoomLocation + new Vector2(-1, 0)))
            {
                if (!CheckIfRoomsAroundGeneratedRoom(specialRoomLocation + new Vector2(-1, 0), "Right"))
                {
                    specialRoom.Location = specialRoomLocation + new Vector2(-1, 0);
                    foundAvailableLocation = true;
                }
            }

            // 오른쪽
            else if (!CheckIfRoomExists(specialRoomLocation + new Vector2(1, 0)))
            {
                if (!CheckIfRoomsAroundGeneratedRoom(specialRoomLocation + new Vector2(1, 0), "Left"))
                {
                    specialRoom.Location = specialRoomLocation + new Vector2(1, 0);
                    foundAvailableLocation = true;
                }
            }

            // 위쪽
            else if (!CheckIfRoomExists(specialRoomLocation + new Vector2(0, 1)))
            {
                if (!CheckIfRoomsAroundGeneratedRoom(specialRoomLocation + new Vector2(0, 1), "Down"))
                {
                    specialRoom.Location = specialRoomLocation + new Vector2(0, 1);
                    foundAvailableLocation = true;
                }
            }

            // 아래쪽
            else if (!CheckIfRoomExists(specialRoomLocation + new Vector2(0, -1)))
            {
                if (!CheckIfRoomsAroundGeneratedRoom(specialRoomLocation + new Vector2(0, -1), "Up"))
                {
                    specialRoom.Location = specialRoomLocation + new Vector2(0, -1);
                    foundAvailableLocation = true;
                }
            }

            if (foundAvailableLocation)
            {
                DrawRoomOnMap(specialRoom);
                return true;
            }
        }

        return false;
    }

    /// <summary>
    /// Fisher Yates Shuffle 알고리즘
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="list"></param>
    private void ShuffleList<T>(List<T> list)
    {
        int number = list.Count;
        System.Random rand = new System.Random();

        while(number > 1)
        {
            number--;

            int k = rand.Next(number + 1);
            T value = list[k];
            list[k] = list[number];
            list[number] = value;
        }
    }

    private void Regenerate()
    {
        reGenerating = true;
        failSafe = 0;
        Level.Rooms.Clear();
        Invoke(nameof(StopRegenerating), 1);

        for (int i = transform.childCount - 1; i >= 0; i--)
        {
            Transform child = transform.GetChild(i);
            Destroy(child.gameObject);
        }

        Start();
    }
}
