//System
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;


//UnityEngine
using UnityEngine;
using UnityEngine.UI;

public class GenertateLevel : MonoBehaviour
{
    // �� ��ȣ
    // ���۹� : 0
    // ������ : 1
    // ������ : 2
    // ������ : 3
    // ��й� : 4

    public Sprite emptyRoom;    // �⺻���� �� �̹���
    public Sprite currentRoom;  // �÷��̾ ��ġ�� �� �̹���
    public Sprite unExploredRoom;   // �÷��̾ �������� ���� �� �̹���
    public Sprite bossRoom;     // ���� �� �̹���
    public Sprite shopRoom;     // ���� �� �̹���
    public Sprite treasureRoom; // ������ �� �̹���
    public Sprite secretRoom;

    private int failSafe = 0;   // �� ���� ���� ������ ����
    private int maxtries = 0;
    private bool reGenerating = false;

    private void Awake()
    {
        Init();
    }

    private void Start()
    {
        // ó�� �÷��̾ ���� ���� ����
        Room startRoom = new Room();
        startRoom.Location = new Vector2(0, 0);
        startRoom.RoomImage = Level.CurrentRoomIcon;
        startRoom.RoomNumber = 0;

        // ���� ����
        Generate(startRoom);
    }

    private void Update()
    {
        // Tab�� ������ �� ���� �ʱ�ȭ ��Ű�� �ٽ� ����
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
    /// 'Level' ���� Ŭ������ �ִ� �������� �� �������� ����
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
    /// <param name="room">��</param>
    private void DrawRoomOnMap(Room room)
    {
        string tileName = "MapTile";

        switch (room.RoomNumber)
        {
            case 1: tileName = "BossRoomTile"; break;
            case 2: tileName = "ShopRoomTile"; break;
            case 3: tileName = "TreasureRoomTile"; break;
        }

        // ������Ʈ�� ����
        GameObject mapTile = new GameObject(tileName);     
        
        // �̹��� ������Ʈ�� �ִ´�
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
        // Exists: ��ȣ �ȿ� ���ǹ��� ���� True�Ǵ� False�� ��ȯ
        // ���� ��ġ�� vector��� �޼ҵ��� ���ڰ��� ������ ���� True �Ǵ� False�� ��ȯ
        return (Level.Rooms.Exists(x => x.Location == vector));
    }

    /// <summary>
    /// ���� �Ǿ��� �� �ֺ��� ���� �Ǳ� �� ������ �����ϰ� �� �ٸ� ���� �����ϴ��� Ȯ�� 
    /// </summary>
    /// <param name="vector">��ǥ</param>
    /// <param name="direction">����</param>
    /// <returns></returns>
    private bool CheckIfRoomsAroundGeneratedRoom(Vector2 vector, string direction)
    {
        switch(direction)
        {
            // �����ʿ��� �������� ���� �����ȴٰ� ����
            case "Right":
                {
                    // ����, ��, �Ʒ��� Ȯ�� 
                    if (Level.Rooms.Exists(x => x.Location == new Vector2(vector.x - 1, vector.y)) ||
                        Level.Rooms.Exists(x => x.Location == new Vector2(vector.x, vector.y + 1)) ||
                        Level.Rooms.Exists(x => x.Location == new Vector2(vector.x, vector.y - 1)))
                    {
                        return true;
                    }
                    break;
                }
            // ���ʿ��� ���������� ���� �����ȴٰ� ����
            case "Left":
                {
                    // ������, ��, �Ʒ��� Ȯ�� 
                    if (Level.Rooms.Exists(x => x.Location == new Vector2(vector.x + 1, vector.y)) ||
                        Level.Rooms.Exists(x => x.Location == new Vector2(vector.x, vector.y + 1)) ||
                        Level.Rooms.Exists(x => x.Location == new Vector2(vector.x, vector.y - 1)))
                    {
                        return true;
                    }
                    break;
                }
            // ���ʿ��� �Ʒ������� ���� �����ȴٰ� ����
            case "Up":
                {
                    // ����, ������, �Ʒ��� Ȯ�� 
                    if (Level.Rooms.Exists(x => x.Location == new Vector2(vector.x - 1, vector.y)) ||
                        Level.Rooms.Exists(x => x.Location == new Vector2(vector.x + 1, vector.y)) ||
                        Level.Rooms.Exists(x => x.Location == new Vector2(vector.x, vector.y - 1)))
                    {
                        return true;
                    }
                    break;
                }
            // �Ʒ��ʿ��� �������� ���� �����ȴٰ� ����
            case "Down":
                {
                    // ����, ������, ���� Ȯ�� 
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
    /// ���� ����
    /// </summary>
    /// <param name="room">��</param>
    private void Generate(Room room)
    {
        failSafe++;
        if (failSafe > 50) return;

        DrawRoomOnMap(room);

        // ���ʻ���
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

        // �����ʻ���
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

        // ���ʻ���
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

        // �Ʒ��ʻ���
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

        // ����
        if (!CheckIfRoomExists(fathestRoom + new Vector2(-1, 0)))
        {
            if (!CheckIfRoomsAroundGeneratedRoom(fathestRoom + new Vector2(-1, 0), "Right"))
            {
                bossRoom.Location = fathestRoom + new Vector2(-1, 0);
            }
        }


        // ������
        else if (!CheckIfRoomExists(fathestRoom + new Vector2(1, 0)))
        {
            if (!CheckIfRoomsAroundGeneratedRoom(fathestRoom + new Vector2(1, 0), "Left"))
            {
                bossRoom.Location = fathestRoom + new Vector2(1, 0);
            }
        }

        // ����
        else if (!CheckIfRoomExists(fathestRoom + new Vector2(0, 1)))
        {
            if (!CheckIfRoomsAroundGeneratedRoom(fathestRoom + new Vector2(0, 1), "Down"))
            {
                bossRoom.Location = fathestRoom + new Vector2(0, 1);
            }
        }

        // �Ʒ���
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

            // ����
            if (!CheckIfRoomExists(specialRoomLocation + new Vector2(-1, 0)))
            {
                if (!CheckIfRoomsAroundGeneratedRoom(specialRoomLocation + new Vector2(-1, 0), "Right"))
                {
                    specialRoom.Location = specialRoomLocation + new Vector2(-1, 0);
                    foundAvailableLocation = true;
                }
            }

            // ������
            else if (!CheckIfRoomExists(specialRoomLocation + new Vector2(1, 0)))
            {
                if (!CheckIfRoomsAroundGeneratedRoom(specialRoomLocation + new Vector2(1, 0), "Left"))
                {
                    specialRoom.Location = specialRoomLocation + new Vector2(1, 0);
                    foundAvailableLocation = true;
                }
            }

            // ����
            else if (!CheckIfRoomExists(specialRoomLocation + new Vector2(0, 1)))
            {
                if (!CheckIfRoomsAroundGeneratedRoom(specialRoomLocation + new Vector2(0, 1), "Down"))
                {
                    specialRoom.Location = specialRoomLocation + new Vector2(0, 1);
                    foundAvailableLocation = true;
                }
            }

            // �Ʒ���
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
    /// Fisher Yates Shuffle �˰���
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
